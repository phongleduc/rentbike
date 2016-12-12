using RentBike.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RentBike
{
    public partial class FormBase : System.Web.UI.Page
    {
        private int IsRunAutoExtendContract = Convert.ToInt32(System.Web.Configuration.WebConfigurationManager.AppSettings["RentBike.IsRunAutoExtendContract"]);
        public ROLE PERMISSION { get; set; }
        public bool IS_ADMIN { get; set; }
        public int STORE_ID { get; set; }
        public string STORE_NAME { get; set; }
        public string USER_NAME { get; set; }

        private DropDownList drpStore;

        protected virtual void Page_Load(object sender, EventArgs e)
        {
            //Run aunto extend contract manually.
            if(IsRunAutoExtendContract == 1)
                CommonList.AutoExtendContract();

            if (Session["store_id"] == null)
            {
                if (Page.User.Identity.IsAuthenticated)
                    LoadUser(Page.User.Identity.Name);
                else
                    Response.Redirect("FormLogin.aspx");
            }

            //raise button click events on content page for the buttons on master page
            drpStore = this.Master.FindControl("ddlStore") as DropDownList;
            drpStore.SelectedIndexChanged += new EventHandler(ddlStore_SelectedIndexChanged);

            IS_ADMIN = Helper.parseInt(Session["permission"]) == 1 ? true : false;

            STORE_ID = Helper.parseInt(Session["store_id"]);
            STORE_NAME = Convert.ToString(Session["store_name"]);
            USER_NAME = Convert.ToString(Session["username"]);

            if (IS_ADMIN)
            {
                if (!string.IsNullOrEmpty(drpStore.SelectedValue))
                {
                    STORE_ID = Helper.parseInt(drpStore.SelectedValue);
                    STORE_NAME = drpStore.SelectedItem.Text;
                }
            }
            if (string.IsNullOrEmpty(STORE_NAME)) STORE_NAME = "Tất cả";

            switch(Helper.parseInt(Session["permission"]))
            {
                case 1:
                    PERMISSION = ROLE.ADMIN;
                    break;
                case 2:
                    PERMISSION = ROLE.STORE_MANAGER;
                    break;
                default:
                    PERMISSION = ROLE.STAFF;
                    break;
            }
        }
        protected virtual void ddlStore_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void LoadUser(string user)
        {
            using (var db = new RentBikeEntities())
            {
                var acc = (from s in db.Accounts
                           where s.ACC == user && s.ACTIVE == true
                           select s).FirstOrDefault();

                if (acc.STORE_ID != 0)
                {
                    var item = db.Stores.FirstOrDefault(s => s.ID == acc.STORE_ID);
                    if (item != null)
                        Session["store_name"] = item.NAME;
                }
                else
                    Session["store_name"] = string.Empty;

                Session["username"] = acc.ACC;
                Session["password"] = acc.PASSWORD;
                Session["name"] = acc.NAME;
                Session["permission"] = acc.PERMISSION_ID;
                Session["city_id"] = acc.CITY_ID;
                Session["store_id"] = acc.STORE_ID;
            }
        }
    }
}