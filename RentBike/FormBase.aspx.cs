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
        public bool IS_ADMIN { get; set; }
        public int STORE_ID { get; set; }
        public string STORE_NAME { get; set; }

        private DropDownList drpStore;

        //raise button click events on content page for the buttons on master page
        protected virtual void Page_Load(object sender, EventArgs e)
        {
            if (Session["store_id"] == null)
            {
                Response.Redirect("FormLogin.aspx");
            }

            drpStore = this.Master.FindControl("ddlStore") as DropDownList;
            drpStore.SelectedIndexChanged += new EventHandler(ddlStore_SelectedIndexChanged);

            IS_ADMIN = Convert.ToBoolean(Session["permission"]);

            STORE_ID = Helper.parseInt(Session["store_id"]);
            STORE_NAME = Convert.ToString(Session["store_name"]);
            if (IS_ADMIN)
            {
                if (!string.IsNullOrEmpty(drpStore.SelectedValue))
                {
                    STORE_ID = Helper.parseInt(drpStore.SelectedValue);
                    STORE_NAME = drpStore.SelectedItem.Text;
                }
            }
            if (string.IsNullOrEmpty(STORE_NAME)) STORE_NAME = "Tất cả";
        }
        protected virtual void ddlStore_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}