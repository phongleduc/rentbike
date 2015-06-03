using RentBike.Common;
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
                    if (permissionid != 1)
                    {
                        hplStoreManagement.Visible = false;
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
                WriteLog(Constants.ACTION_LOGOUT, false);
                Session.RemoveAll();
                System.Web.Security.FormsAuthentication.SignOut();
                Response.Redirect("FormLogin.aspx", false);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message + Environment.NewLine + ex.StackTrace);
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
                        storeid = Helper.parseInt(ddlStore.SelectedValue);
                    }
                    else
                    {
                        storeid = Convert.ToInt32(Session["store_id"]);
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
                WriteLog(Constants.ACTION_LOGOUT, true);
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
                                            ID = o.STORE_ID,
                                            InOutDate = o.INOUT_DATE,
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
                    si.StoreId = g.Record.ToList()[0].ID;
                    si.InOutDate = g.Record.ToList()[0].InOutDate.Value;
                    si.TotalIn = g.Record.ToList()[0].TotalIn;
                    si.TotalOut = g.Record.ToList()[0].TotalOut;
                    si.BeginAmount = 0;
                    si.EndAmount = g.Record.ToList()[0].TotalIn - g.Record.ToList()[0].TotalOut;

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
                decimal sumEnd = 0;

                if (lst.Any())
                {
                    sumIn = lst.Select(c =>c.TotalIn).DefaultIfEmpty(0).Sum();
                    sumOut = lst.Select(c => c.TotalOut).DefaultIfEmpty(0).Sum();
                    sumEnd = sumIn - sumOut;
                    lblTotalValue.Text = string.Format("{0:0,0}", sumEnd);
                }
            }
        }
    }
}