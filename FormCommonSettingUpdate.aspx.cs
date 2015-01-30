using RentBike.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RentBike
{
    public partial class FormCommonSettingUpdate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["store_id"] == null)
            {
                Response.Redirect("FormLogin.aspx");
            }
            if (!IsPostBack)
            {
                LoadRentType();
            }
        }

        protected string ValidateFields()
        {
            if (string.IsNullOrEmpty(txtItem.Text.Trim()))
            {
                return "Bạn cần phải nhập tên danh mục.";
            }
            return string.Empty;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string result = ValidateFields();
            if (!string.IsNullOrEmpty(result))
            {
                lblMessage.Text = result;
                return;
            }
            string id = Request.QueryString["ID"];
            if (string.IsNullOrEmpty(id))
            {
                // New account
                RentType item = new RentType();
                item.NAME = txtItem.Text.Trim();
                item.ACTIVE = rdbActive.Checked;

                using (var db = new RentBikeEntities())
                {
                    db.RentTypes.Add(item);
                    db.SaveChanges();
                }
                WriteLog(CommonList.ACTION_CREATE_TYPE, false);
            }
            else
            {
                //// Edit
                using (var db = new RentBikeEntities())
                {
                    int typeId = Helper.parseInt(Request.QueryString["ID"]);
                    var item = (from s in db.RentTypes
                                where s.ID == typeId
                                select s).FirstOrDefault();

                    item.NAME = txtItem.Text.Trim();
                    item.ACTIVE = rdbActive.Checked;

                    db.SaveChanges();
                    WriteLog(CommonList.ACTION_UPDATE_TYPE, false);
                }
            }
            Response.Redirect("FormCommonListSetting.aspx");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("FormCommonListSetting.aspx");
        }

        private void LoadRentType()
        {
            int id = Helper.parseInt(Request.QueryString["ID"]);
            using (var db = new RentBikeEntities())
            {
                var item = (from s in db.RentTypes
                            where s.ID == id
                            select s).FirstOrDefault();
                if (item != null)
                {
                    txtItem.Text = item.NAME;
                    if (item.ACTIVE)
                        rdbActive.Checked = true;
                    else
                        rdbDeActive.Checked = true;
                }
            }          
        }

        private void WriteLog(string action, bool isCrashed)
        {
            Log lg = new Log();
            lg.ACCOUNT = Session["username"].ToString();
            string strStoreName = string.Empty;
            lg.STORE = strStoreName;
            lg.LOG_ACTION = action;
            lg.LOG_DATE = DateTime.Now;
            lg.IS_CRASH = isCrashed;
            lg.LOG_MSG = string.Format("Danh mục {0} {1} thực hiện {2} vào lúc {3}", lg.ACCOUNT, strStoreName, lg.LOG_ACTION, lg.LOG_DATE);
            lg.SEARCH_TEXT = lg.LOG_MSG;

            using (var db = new RentBikeEntities())
            {
                db.Logs.Add(lg);
                db.SaveChanges();
            }
        }
    }
}