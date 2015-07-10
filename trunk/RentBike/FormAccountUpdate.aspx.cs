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
    public partial class FormAccountUpdate : FormBase
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!IsPostBack)
            {
                LoadPermission();
                Common.CommonList.LoadCity(ddlCity);
                //List<Store> storeList = GetStoreByCity(1);
                ddlStore.Items.Add(new ListItem("--Tất cả cửa hàng--", "0"));
                Common.CommonList.LoadStore(ddlStore);

                string id = Request.QueryString["ID"];
                if (!string.IsNullOrEmpty(id)) // Update account
                {
                    int accid = Convert.ToInt32(id);
                    using (var db = new RentBikeEntities())
                    {
                        var item = (from s in db.Accounts
                                    where s.ID == accid
                                    select s).FirstOrDefault();

                        txtAccount.Text = item.ACC;
                        //txtNewPassword.Text = item.PASSWORD;
                        //item.PERMISSION_ID =
                        //item.STORE_ID = 
                        txtName.Text = item.NAME;
                        //item.ADDRESS = txtAddress.Text.Trim();
                        //item.CITY_ID = Convert.ToInt32(ddlCity.SelectedValue);
                        txtPhone.Text = item.PHONE;
                        txtRegisterDate.Text = item.REGISTER_DATE.ToShortDateString();
                        rdbActive.Checked = item.ACTIVE;
                        rdbDeActive.Checked = !rdbActive.Checked;
                        txtNote.Text = item.NOTE;
                        ddlPermission.SelectedValue = item.PERMISSION_ID.ToString();
                        ddlStore.SelectedValue = item.STORE_ID.ToString();
                        ddlCity.SelectedValue = item.CITY_ID.ToString();
                    }
                }
                else // new account
                {
                    txtOldPassword.Enabled = false;
                }
            }
        }

        protected string ValidateFields()
        {
            string id = Request.QueryString["ID"];
            if (string.IsNullOrEmpty(txtAccount.Text.Trim()))
            {
                return "Bạn cần phải nhập tên tài khoản.";
            }
            if (txtAccount.Text.Trim().Length < 5)
            {
                return "Tên tài khoản phải có ít nhất 5 ký tự.";
            }
            if (!Regex.IsMatch(txtAccount.Text.Trim(), "^[a-zA-Z0-9]+$"))
            {
                return "Tên tài khoản chỉ có thể là số hoặc chữ.";
            }
            if (!string.IsNullOrEmpty(id))
            {
                int accid = Convert.ToInt32(id);
                Account item = null;
                using (var db = new RentBikeEntities())
                {
                    var st = from s in db.Accounts
                             where s.ID == accid
                             select s;

                    item = st.ToList<Account>()[0];
                }
                if (txtOldPassword.Text.Trim().Length > 0)
                {
                    if (Helper.EncryptPassword(txtOldPassword.Text.Trim()) != item.PASSWORD)
                    {
                        return "Mật khẩu cũ không đúng.";
                    }

                    if (string.IsNullOrEmpty(txtNewPassword.Text.Trim()))
                    {
                        return "Bạn cần phải nhập mật khẩu.";
                    }
                    if (txtNewPassword.Text.Trim().Length < 6)
                    {
                        return "Mật khẩu phải có ít nhất là 6 ký tự.";
                    }
                    if (txtConfirmPassword.Text.Trim() != txtNewPassword.Text.Trim())
                    {
                        return "Mật khẩu không khớp nhau.";
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(txtNewPassword.Text.Trim()))
                {
                    return "Bạn cần phải nhập mật khẩu.";
                }
                if (txtNewPassword.Text.Trim().Length < 6)
                {
                    return "Mật khẩu phải có ít nhất là 6 ký tự.";
                }
                if (txtConfirmPassword.Text.Trim() != txtNewPassword.Text.Trim())
                {
                    return "Mật khẩu không khớp nhau.";
                }
            }
            if (string.IsNullOrEmpty(txtName.Text.Trim()))
            {
                return "Bạn phải nhập tên người sử dụng tài khoản.";
            }
            if (string.IsNullOrEmpty(txtPhone.Text.Trim()))
            {
                return "Bạn phải nhập số điện thoại.";
            }
            if (string.IsNullOrEmpty(txtRegisterDate.Text.Trim()))
            {
                return "Bạn phải nhập ngày đăng ký.";
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
                using (var db = new RentBikeEntities())
                {
                    int accid = Convert.ToInt32(id);
                    var acc = (from s in db.Accounts
                               where s.ACC == txtAccount.Text.Trim()
                                select s).FirstOrDefault();
                    if(acc != null)
                    {
                        lblMessage.Text = "Tài khoản này đã tồn tại trên hệ thống.";
                        return;
                    }

                    // New account
                    Account item = new Account();
                    item.ACC = txtAccount.Text.Trim();
                    item.PASSWORD = Helper.EncryptPassword(txtNewPassword.Text.Trim());
                    txtOldPassword.Enabled = false;
                    txtConfirmPassword.Enabled = false;
                    item.PERMISSION_ID = Convert.ToInt32(ddlPermission.SelectedValue);
                    item.STORE_ID = Convert.ToInt32(ddlStore.SelectedValue);

                    item.NAME = txtName.Text.Trim();
                    item.CITY_ID = Convert.ToInt32(ddlCity.SelectedValue);
                    item.PHONE = txtPhone.Text.Trim();
                    item.REGISTER_DATE = Convert.ToDateTime(txtRegisterDate.Text);
                    item.ACTIVE = rdbActive.Checked;
                    item.NOTE = txtNote.Text.Trim();

                    item.SEARCH_TEXT = string.Format("{0} {1}", item.ACC, item.NAME);


                    db.Accounts.Add(item);
                    db.SaveChanges();
                }
                WriteLog(Constants.ACTION_CREATE_ACCOUNT, false);
            }
            else
            {
                //// Edit
                using (var db = new RentBikeEntities())
                {
                    int accid = Convert.ToInt32(id);
                    var item = (from s in db.Accounts
                                where s.ID == accid
                                select s).FirstOrDefault();

                    item.ACC = txtAccount.Text.Trim();
                    if (txtOldPassword.Text.Trim().Length > 0)
                    {
                        item.PASSWORD = Helper.EncryptPassword(txtNewPassword.Text.Trim());
                    }
                    item.PERMISSION_ID = Convert.ToInt32(ddlPermission.SelectedValue);
                    item.STORE_ID = Convert.ToInt32(ddlStore.SelectedValue);
                    item.NAME = txtName.Text.Trim();
                    item.CITY_ID = Convert.ToInt32(ddlCity.SelectedValue);
                    item.PHONE = txtPhone.Text.Trim();
                    item.REGISTER_DATE = Convert.ToDateTime(txtRegisterDate.Text);
                    item.ACTIVE = rdbActive.Checked;
                    item.NOTE = txtNote.Text.Trim();

                    db.SaveChanges();
                    WriteLog(Constants.ACTION_UPDATE_ACCOUNT, false);
                }
            }
            Response.Redirect("FormAccountManagement.aspx");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("FormAccountManagement.aspx");
        }

        private void LoadPermission()
        {
            int crtPermissionId = Convert.ToInt32(Session["permission"]);
            int filterId = crtPermissionId == 1 ? 1 : crtPermissionId + 1;
            List<AccountPermission> perList = new List<AccountPermission>();
            using (var db = new RentBikeEntities())
            {
                var item = from itm in db.AccountPermissions
                           where itm.ID >= filterId
                           select itm;

                perList = item.ToList();
            }

            ddlPermission.DataSource = perList;
            if (perList.Count > 0)
            {
                ddlPermission.DataTextField = "NAME";
                ddlPermission.DataValueField = "ID";
            }
            ddlPermission.DataBind();
        }

        private void LoadStore(List<Store> lst)
        {
            ddlStore.Items.Add(new ListItem("--Tất cả--", "0"));
            foreach (Store store in lst)
            {
                ddlStore.Items.Add(new ListItem(store.NAME, store.ID.ToString()));
            }
        }

        private static List<Store> GetStoreByCity(int city_id)
        {
            List<Store> lst;
            using (var db = new RentBikeEntities())
            {
                var st = from s in db.Stores
                         where s.CITY_ID == city_id
                         select s;

                lst = st.ToList<Store>();
            }
            return lst;
        }

        private void WriteLog(string action, bool isCrashed)
        {
            Log lg = new Log();
            lg.ACCOUNT = Session["username"].ToString();
            lg.STORE = STORE_NAME;
            lg.LOG_ACTION = action;
            lg.LOG_DATE = DateTime.Now;
            lg.IS_CRASH = isCrashed;
            lg.LOG_MSG = string.Format("Tài khoản {0} cửa hàng {1} thực hiện {2} vào lúc {3}", lg.ACCOUNT, STORE_NAME, lg.LOG_ACTION, lg.LOG_DATE);
            lg.SEARCH_TEXT = lg.LOG_MSG;

            using (var db = new RentBikeEntities())
            {
                db.Logs.Add(lg);
                db.SaveChanges();
            }
        }
    }
}