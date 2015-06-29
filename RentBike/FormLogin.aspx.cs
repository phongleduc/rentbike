using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using RentBike.Common;
using System.Web.Security;

namespace RentBike
{
    public partial class FormLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Page.User.Identity.IsAuthenticated)
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
                        Response.Redirect(Request.QueryString["ReturnUrl"]);
                    else
                    {
                        if (Session["permission"] != null && Session["permission"].ToString() == "1")
                            Response.Redirect("FormIncomeOutcomeSummary.aspx");
                        else
                            Response.Redirect("FormReport.aspx");
                    }
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (LoadUser(txtUsername.Text.Trim(), Helper.EncryptPassword(txtPassword.Text.Trim())))
                {
                    Response.Cookies.Clear();
                    //Create the ticket, and add the groups.
                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1,
                              txtUsername.Text.Trim(), DateTime.Now, DateTime.Now.AddDays(30), chkRememberMe.Checked, string.Empty);

                    //Encrypt the ticket.
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

                    //Create a cookie, and then add the encrypted ticket to the cookie as data.
                    HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

                    if (chkRememberMe.Checked)
                        authCookie.Expires = authTicket.Expiration;

                    //Add the cookie to the outgoing cookies collection.
                    Response.Cookies.Add(authCookie);

                    //You can redirect now.
                    string redirectURL = FormsAuthentication.GetRedirectUrl(txtUsername.Text, chkRememberMe.Checked);
                    if (Session["permission"] != null && Session["permission"].ToString() == "1")
                        redirectURL = "FormIncomeOutcomeSummary.aspx";
                    else
                    {
                        if (redirectURL == "/default.aspx")
                            redirectURL = "FormReport.aspx";
                    }

                    Response.Redirect(redirectURL, false);
                }
                else
                    lblMessage.Text = "Đăng nhập không thành công.";
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                lblMessage.Text = ex.Message;
            }
        }

        private bool LoadUser(string user, string password)
        {
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password))
                return false;

            using (var db = new RentBikeEntities())
            {
                var acc = (from s in db.Accounts
                           where s.ACC == user && s.PASSWORD == password
                           select s).FirstOrDefault();

                if (acc == null) return false;

                if (acc.STORE_ID != 0)
                {
                    var item = db.Stores.FirstOrDefault(s => s.ID == acc.STORE_ID);
                    if (item != null)
                    {
                        if (!item.ACTIVE)
                            return false;
                        Session["store_name"] = item.NAME;
                    }
                }
                else
                    Session["store_name"] = string.Empty;

                Session["username"] = acc.ACC;
                Session["name"] = acc.NAME;
                Session["permission"] = acc.PERMISSION_ID;
                Session["city_id"] = acc.CITY_ID;
                Session["store_id"] = acc.STORE_ID;
            }
            return true;
        }

        private void WriteLog(string action, bool isCrashed)
        {
            Log lg = new Log();
            lg.ACCOUNT = Session["username"].ToString();
            string strStore = string.Empty;
            string strStoreName = Convert.ToString(Session["store_name"]);
            if (!string.IsNullOrEmpty(strStoreName))
                strStore = string.Format("cửa hàng {0} ", strStoreName);
            lg.STORE = strStore;
            lg.LOG_ACTION = action;
            lg.LOG_DATE = DateTime.Now;
            lg.IS_CRASH = isCrashed;
            lg.LOG_MSG = string.Format("Tài khoản {0} {1}thực hiện {2} vào lúc {3}", lg.ACCOUNT, strStore, lg.LOG_ACTION, lg.LOG_DATE);
            lg.SEARCH_TEXT = lg.LOG_MSG;
            lg.STORE_ID = Convert.ToInt32(Session["store_id"]);
            using (var db = new RentBikeEntities())
            {
                db.Logs.Add(lg);
                db.SaveChanges();
            }
        }
    }
}