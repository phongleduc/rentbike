﻿using RentBike.Common;
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
            //Check session timeout or not existing
            if (Session["store_id"] == null)
            {
                Response.Redirect("FormLogin.aspx");
            }
            if (!Page.IsPostBack)
            {
                try
                {
                    int permissionid = Convert.ToInt16(Session["permission"]);
                    LoadStore(permissionid);
                    if (permissionid != 1)
                    {
                        hplStoreManagement.Visible = false;
                        hplPendingContract.Visible = false;
                        hplCommonListSetting.Visible = false;
                        hplAccountManagement.Visible = false;
                        int storeid = Convert.ToInt16(Session["store_id"]);
                        CalcFeeStore(storeid);
                    }
                    else if (permissionid == 1)
                    {
                        CalcFeeStore(0);
                    }
                    if (permissionid != 1 && permissionid != 2)
                    {
                        hplAccountManagement.Visible = false;
                    }

                    txtUserFullName.Text = Session["name"].ToString();
                    //int storeid = Convert.ToInt16(Session["store_id"]);
                    //using (var db = new RentBikeEntities())
                    //{
                    //    var item = db.Stores.First(s => s.ID == storeid);
                    //    lblStoreName.Text = item.NAME;
                    //    lblTotalValue.Text = item.START_CAPITAL == 0 ? "0" : string.Format("{0:0,0}", item.START_CAPITAL);
                    //}

                }
                catch (Exception ex)
                { }
            }
        }

        private void LoadStore(int permissionid)
        {
            ddlStore.Items.Add(new ListItem("--Tất cả cửa hàng--", ""));
            CommonList.LoadStore(ddlStore);
            if (permissionid != 1)
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
                WriteLog(CommonList.ACTION_LOGOUT, false);
            }
            catch (Exception ex)
            {
                WriteLog(CommonList.ACTION_LOGOUT, true);
            }
            finally
            {
                Session.RemoveAll();
                Response.Redirect("FormLogin.aspx");
            }
        }

        private void WriteLog(string action, bool isCrashed)
        {
            try
            {
                Log lg = new Log();
                lg.ACCOUNT = Session["username"].ToString();
                string strStore = string.Empty;
                using (var db = new RentBikeEntities())
                {
                    int storeid = 0;
                    if (CheckAdminPermission())
                    {
                        DropDownList drpStore = this.Master.FindControl("ddlStore") as DropDownList;
                        storeid = Helper.parseInt(drpStore.SelectedValue);
                    }
                    else
                    {
                        storeid = Convert.ToInt16(Session["store_id"]);
                    }
                    var item = from itm in db.Stores
                               where itm.ID == storeid
                               select itm;
                    List<Store> lst = item.ToList();

                    if (lst.Count > 0)
                    {
                        lg.STORE = lst[0].NAME;
                        strStore = string.Format("cửa hàng {0} ", lst[0].NAME);
                    }
                    else
                    { lg.STORE = string.Empty; }
                }
                lg.LOG_ACTION = action;
                lg.LOG_DATE = DateTime.Now;
                lg.IS_CRASH = isCrashed;
                lg.LOG_MSG = string.Format("Tài khoản {0} {1}thực hiện {2} vào lúc {3}", lg.ACCOUNT, strStore, lg.LOG_ACTION, lg.LOG_DATE);
                lg.SEARCH_TEXT = lg.LOG_MSG;


                using (var db = new RentBikeEntities())
                {
                    db.Logs.Add(lg);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                WriteLog(CommonList.ACTION_LOGOUT, true);
            }
        }

        public bool CheckAdminPermission()
        {
            string acc = Convert.ToString(Session["username"]);
            using (var db = new RentBikeEntities())
            {
                var item = db.Accounts.First(s => s.ACC == acc);

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
                if (storeId == 0)
                {
                    var data = from d in db.InOuts
                               group d by d.INOUT_DATE into g
                               select new
                               {
                                   Period = g.Key,
                                   Record = from o in g
                                            select new
                                            {
                                                ID = o.STORE_ID,
                                                Period = o.INOUT_DATE,
                                                InAmount = o.IN_AMOUNT,
                                                OutAmount = o.OUT_AMOUNT,
                                                TotalIn = g.Sum(x => x.IN_AMOUNT),
                                                TotalOut = g.Sum(x => x.OUT_AMOUNT),
                                                BeginAmount = 0,
                                                EndAmount = 0,
                                                ContractFeeCar = 0,
                                                RentFeeCar = 0,
                                                CloseFeeCar = 0,
                                                ContractFeeEquip = 0,
                                                RentFeeEquip = 0,
                                                CloseFeeEquip = 0,
                                                ContractFeeOther = 0,
                                                RentFeeOther = 0,
                                                CloseFeeOther = 0,
                                                RemainEndOfDay = 0,
                                                InOutTypeId = o.INOUT_TYPE_ID,
                                                InCapital = 0,
                                                OutCapital = 0,
                                                InOther = 0,
                                                OutOther = 0

                                            }
                               };

                    List<SummaryInfo> lst = new List<SummaryInfo>();
                    foreach (var g in data)
                    {
                        SummaryInfo si = new SummaryInfo();
                        si.StoreId = storeId;
                        si.Period = Convert.ToDateTime(g.Period.Value).ToString("dd/MM/yyyy");
                        si.TotalIn = g.Record.Select(x => x.InAmount).DefaultIfEmpty().Sum();
                        si.TotalOut = g.Record.Select(x => x.OutAmount).DefaultIfEmpty().Sum();
                        si.BeginAmount = 0;
                        si.EndAmount = si.TotalIn - si.TotalOut;

                        lst.Add(si);
                    }

                    for (int i = 0; i < lst.Count; i++)
                    {
                        if (i > 0)
                        {
                            lst[i].BeginAmount = lst[i - 1].EndAmount;
                            lst[i].EndAmount += lst[i].BeginAmount;
                        }
                    }


                    decimal sumIn = 0;
                    decimal sumOut = 0;
                    decimal sumBegin = 0;
                    decimal sumEnd = 0;

                    if (lst.Any())
                    {
                        sumBegin = lst[0].BeginAmount;
                        foreach (SummaryInfo itm in lst)
                        {
                            sumIn += itm.TotalIn;
                            sumOut += itm.TotalOut;
                        }
                        sumEnd = sumIn - sumOut;
                        lblTotalValue.Text = string.Format("{0:0,0}", sumEnd);
                    }
                }
                else
                {
                    var data = from d in db.InOuts
                               where d.STORE_ID == storeId
                               group d by d.INOUT_DATE into g
                               select new
                               {
                                   Period = g.Key,
                                   Record = from o in g
                                            select new
                                            {
                                                ID = o.STORE_ID,
                                                Period = o.INOUT_DATE,
                                                InAmount = o.IN_AMOUNT,
                                                OutAmount = o.OUT_AMOUNT,
                                                TotalIn = g.Sum(x => x.IN_AMOUNT),
                                                TotalOut = g.Sum(x => x.OUT_AMOUNT),
                                                BeginAmount = 0,
                                                EndAmount = 0,
                                                ContractFeeCar = 0,
                                                RentFeeCar = 0,
                                                CloseFeeCar = 0,
                                                ContractFeeEquip = 0,
                                                RentFeeEquip = 0,
                                                CloseFeeEquip = 0,
                                                ContractFeeOther = 0,
                                                RentFeeOther = 0,
                                                CloseFeeOther = 0,
                                                RemainEndOfDay = 0,
                                                InOutTypeId = o.INOUT_TYPE_ID,
                                                InCapital = 0,
                                                OutCapital = 0,
                                                InOther = 0,
                                                OutOther = 0

                                            }
                               };

                    List<SummaryInfo> lst = new List<SummaryInfo>();
                    foreach (var g in data)
                    {
                        SummaryInfo si = new SummaryInfo();
                        si.StoreId = storeId;
                        si.Period = Convert.ToDateTime(g.Period.Value).ToString("dd/MM/yyyy");
                        si.TotalIn = g.Record.Select(x => x.InAmount).DefaultIfEmpty().Sum();
                        si.TotalOut = g.Record.Select(x => x.OutAmount).DefaultIfEmpty().Sum();
                        si.BeginAmount = 0;
                        si.EndAmount = si.TotalIn - si.TotalOut;

                        lst.Add(si);
                    }

                    for (int i = 0; i < lst.Count; i++)
                    {
                        if (i > 0)
                        {
                            lst[i].BeginAmount = lst[i - 1].EndAmount;
                            lst[i].EndAmount += lst[i].BeginAmount;
                        }
                    }


                    decimal sumIn = 0;
                    decimal sumOut = 0;
                    decimal sumBegin = 0;
                    decimal sumEnd = 0;
                    if (lst.Any())
                    {
                        sumBegin = lst[0].BeginAmount;
                        sumIn = lst.Select(c => c.TotalIn).DefaultIfEmpty().Sum();
                        sumOut = lst.Select(c => c.TotalOut).DefaultIfEmpty().Sum();
                        sumEnd = sumIn - sumOut;
                        lblTotalValue.Text = string.Format("{0:0,0}", sumEnd);
                    }
                }
            }
        }
    }
}