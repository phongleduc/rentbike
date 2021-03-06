﻿using RentBike.Common;
using RentBike.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RentBike
{
    public partial class MasterPage : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                try
                {
                    int permissionid = Convert.ToInt32(Session["permission"]);
                    int storeid = Convert.ToInt32(Session["store_id"]);
                    LoadStore(permissionid);
                    CalcFeeStore(storeid);
                    hplCommonListSetting.Visible = false;
                    if (permissionid != (int)ROLE.ADMIN)
                    {
                        hplStoreManagement.Visible = false;
                    }
                    if(permissionid == (int)ROLE.STAFF)
                    {
                        hplAccountManagement.Visible = false;
                    }
                    txtUserFullName.Text = "Chào " + Session["name"] + ",";

                }
                catch (Exception ex)
                {
                    Logger.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                }
            }
        }

        private void LoadStore(int permissionid)
        {
            ddlStore.Items.Add(new ListItem("--Tất cả cửa hàng--", ""));
            CommonList.LoadStore(ddlStore);
            if (permissionid != (int)ROLE.ADMIN)
            {
                ddlStore.SelectedValue = Session["store_id"].ToString();
                ddlStore.Enabled = false;
            }
        }

        private static List<Store> GetStoreByCity(int city_id)
        {
            List<Store> lst;
            using (var db = new RentBikeEntities())
            {
                var st = from s in db.Stores
                         where s.CITY_ID == city_id && s.ACTIVE == true
                         select s;

                lst = st.ToList<Store>();
            }
            return lst;
        }

        private static List<Store> GetAllStore()
        {
            List<Store> lst;
            using (var db = new RentBikeEntities())
            {
                var st = from s in db.Stores
                         where s.ACTIVE == true
                         select s;

                lst = st.ToList<Store>();
            }
            return lst;
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            try
            {
                Helper.WriteLog(Convert.ToString(Session["username"]), Convert.ToString(Session["store_name"]), Constants.ACTION_LOGOUT, false);

                Session.RemoveAll();
                System.Web.Security.FormsAuthentication.SignOut();
                Response.Redirect("FormLogin.aspx", false);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public bool CheckAdminPermission()
        {
            string acc = Convert.ToString(Session["username"]);
            using (var db = new RentBikeEntities())
            {
                var item = db.Accounts.FirstOrDefault(s => s.ACC == acc);

                if (item.PERMISSION_ID == 1)
                    return true;
                return false;
            }
        }

        protected void ddlStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalcFeeStore(Helper.parseInt(ddlStore.SelectedValue));
        }

        protected void CalcFeeStore(int storeId)
        {
            using (var db = new RentBikeEntities())
            {
                IQueryable<INOUT_FULL_VW> inOut = db.INOUT_FULL_VW.Where(c =>c.ACTIVE == true);
                if (storeId != 0)
                {
                    inOut = inOut.Where(c => c.STORE_ID == storeId);
                }
                var data = from d in inOut.ToList()
                           group d by d.INOUT_DATE into g
                           select new
                           {
                               Period = g.Key,
                               Record = from o in g
                                        select new
                                        {
                                            TotalIn = g.Sum(x => x.IN_AMOUNT),
                                            TotalOut = g.Sum(x => x.OUT_AMOUNT),

                                        }
                           };

                if (data.Any())
                {
                    decimal sumIn = 0;
                    decimal sumOut = 0;
                    decimal sumEnd = 0;

                    sumIn = data.Select(c => c.Record.ToList()[0].TotalIn).DefaultIfEmpty(0).Sum();
                    sumOut = data.Select(c => c.Record.ToList()[0].TotalOut).DefaultIfEmpty(0).Sum();
                    sumEnd = sumIn - sumOut;
                    lblTotalValue.Text = sumEnd == 0 ? "0" : string.Format("{0:0,0}", sumEnd);
                }
            }
        }
    }
}