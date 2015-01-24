using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using RentBike.Common;

namespace RentBike
{
    public partial class FormLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (LoadUser())
                {
                    WriteLog(CommonList.ACTION_LOGIN, false); 
                    //if (Session["permission"].ToString() == "1")
                    //{
                    //    Response.Redirect("FormStoreManagement.aspx", false);
                    //}
                    //else
                    //{
                    //    Response.Redirect("FormContractManagement.aspx", false);
                    //}
                    Response.Redirect("FormWarning.aspx", false);
                }
                else
                {
                    lblMessage.Text = "Đăng nhập không thành công.";
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message, true);
                throw ex;
            } 
        }

        private bool LoadUser()
        {
            string user = txtUsername.Text.Trim();
            string password = CommonList.EncryptPassword(txtPassword.Text.Trim());

            List<Account> lst = new List<Account>();
            using (var db = new RentBikeEntities())
            {
                var acc = from s in db.Accounts
                          where s.ACC == user && s.PASSWORD == password
                          select s;

                lst = acc.ToList();
            }

            if (lst.Count > 0)
            {
                int storeid = lst[0].STORE_ID;
                using (var db = new RentBikeEntities())
                {
                    if (storeid != 0)
                    {
                        var item = db.Stores.First(s => s.ID == storeid);

                        if (!item.ACTIVE)
                            return false;
                    }
                } // Xu ly khi tai khoan cua hang bi khoa thi khong dang nhap duoc


                Session["username"] = lst[0].ACC;
                Session["name"] = lst[0].NAME;
                Session["permission"] = lst[0].PERMISSION_ID;
                Session["city_id"] = lst[0].CITY_ID;
                Session["store_id"] = lst[0].STORE_ID;

                using (var db = new RentBikeEntities())
                {
                    var st = from s in db.Stores
                             where s.ID == storeid
                             select s;

                    List<Store> lstStore = st.ToList();
                    if (lstStore.Count > 0)
                    {
                        Session["store_name"] = lstStore[0].NAME;
                    }
                    else
                    {
                        Session["store_name"] = string.Empty;
                    }
                }
            }

            return lst.Count > 0;
        }

        private void WriteLog(string action, bool isCrashed)
        {
            Log lg = new Log();
            lg.ACCOUNT = Session["username"].ToString();
            string strStore = string.Empty;
            string strStoreName = Session["store_name"].ToString();
            if (strStoreName != string.Empty)
            {
                strStore = string.Format("cửa hàng {0} ", strStoreName);
            }
            lg.STORE = strStore;
            lg.LOG_ACTION = action;
            lg.LOG_DATE = DateTime.Now;
            lg.IS_CRASH = isCrashed;
            lg.LOG_MSG = string.Format("Tài khoản {0} {1}thực hiện {2} vào lúc {3}", lg.ACCOUNT, strStore, lg.LOG_ACTION, lg.LOG_DATE);
            lg.SEARCH_TEXT = lg.LOG_MSG;
            lg.STORE_ID = Convert.ToInt16(Session["store_id"]);
            using (var db = new RentBikeEntities())
            {
                db.Logs.Add(lg);
                db.SaveChanges();
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtUsername.Text = txtPassword.Text = string.Empty;
        }
    }
}