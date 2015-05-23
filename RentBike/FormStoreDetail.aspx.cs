﻿using RentBike.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RentBike
{
    public partial class FormStoreDetail : FormBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Common.CommonList.LoadCity(ddlCity);
                string id = Request.QueryString["ID"];
                if (!string.IsNullOrEmpty(id))
                {
                    LoadStoreById(id);
                }
            }
        }

        private void LoadStoreById(string id)
        {
            int storeid = Convert.ToInt32(id);
            List<Store> lst;
            using (var db = new RentBikeEntities())
            {
                var st = from s in db.Stores
                         where s.ID == storeid
                         select s;

                lst = st.ToList<Store>();
            }

            Store store = lst[0];
            txtName.Text = store.NAME;
            txtAddress.Text = store.ADDRESS;
            ddlCity.SelectedValue = store.CITY_ID.ToString();
            txtPhone.Text = store.PHONE;
            txtStartCapital.Text = store.START_CAPITAL.ToString(); // Common.CommonList.FormatedAsCurrency(Convert.ToInt32(store.START_CAPITAL));
            txtCurrentCapital.Text = store.CURRENT_CAPITAL.ToString();
            txtApplyDate.Text = store.APPLY_DATE.ToShortDateString();
            txtTotalRevenueBefore.Text = store.REVENUE_BEFORE_APPLY.ToString();
            txtTotalCostBefore.Text = store.TOTAL_COST_BEFORE.ToString();
            txtTotalInvesmentBefore.Text = store.TOTAL_INVESMENT_BEFORE.ToString();
            txtRegisterDate.Text = store.REGISTER_DATE.ToShortDateString();
            rdbActive.Checked = store.ACTIVE;
            rdbDeActive.Checked = !rdbActive.Checked;
            txtNote.Text = store.NOTE;
        }

        protected string ValidateFields()
        {
            string id = Request.QueryString["ID"];
            if (string.IsNullOrEmpty(txtName.Text.Trim()))
            {
                return "Bạn cần phải nhập tên cửa hàng";
            }
            if (string.IsNullOrEmpty(txtAddress.Text.Trim()))
            {
                return "Bạn cần phải nhập địa chỉ cửa hàng.";
            }
            if (string.IsNullOrEmpty(txtPhone.Text.Trim()))
            {
                return "Bạn cần phải nhập số điện thoại.";
            }
            if (string.IsNullOrEmpty(txtStartCapital.Text.Trim()) || Convert.ToDecimal(txtStartCapital.Text) == 0)
            {
                return "Bạn cần phải nhập số vốn ban đầu.";
            }
            if (string.IsNullOrEmpty(txtRegisterDate.Text.Trim()))
            {
                return "Bạn cần phải nhập ngày đăng ký.";
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
                // New store
                CreateNewStore();
            }
            else
            {
                // Edit
                UpdateStore(id);
            }

            Response.Redirect("FormStoreManagement.aspx");
        }

        private void CreateNewStore()
        {
            using (TransactionScope scope = new TransactionScope())
            {

                Store st = new Store();
                st.NAME = txtName.Text.Trim();
                st.ADDRESS = txtAddress.Text.Trim();
                st.CITY_ID = Convert.ToInt32(ddlCity.SelectedValue);
                st.PHONE = txtPhone.Text.Trim();
                st.FAX = string.Empty;
                st.START_CAPITAL = Convert.ToDecimal(txtStartCapital.Text);
                //st.CURRENT_CAPITAL = Convert.ToDecimal(txtCurrentCapital.Text);
                //st.APPLY_DATE = Convert.ToDateTime(txtApplyDate.Text);
                //st.REVENUE_BEFORE_APPLY = Convert.ToDecimal(txtTotalRevenueBefore.Text);
                //st.TOTAL_COST_BEFORE = Convert.ToDecimal(txtTotalCostBefore.Text);
                //st.TOTAL_INVESMENT_BEFORE = Convert.ToDecimal(txtTotalInvesmentBefore.Text);
                st.REGISTER_DATE = Convert.ToDateTime(txtRegisterDate.Text);
                st.ACTIVE = rdbActive.Checked;
                st.NOTE = txtNote.Text.Trim();
                st.SEARCH_TEXT = string.Format("{0} {1} {2}", st.NAME, st.ADDRESS, st.PHONE);

                using (var rb = new RentBikeEntities())
                {
                    rb.Stores.Add(st);
                    rb.SaveChanges();
                }

                using (var rb1 = new RentBikeEntities())
                {
                    var item = rb1.InOutTypes.FirstOrDefault(s =>s.NAME == "Nhập vốn");

                    InOut io = new InOut();
                    io.IN_AMOUNT = Convert.ToDecimal(txtStartCapital.Text.Replace(",", string.Empty));
                    io.OUT_AMOUNT = 0;
                    io.CONTRACT_ID = -1;
                    io.PERIOD_ID = -1;
                    io.RENT_TYPE_ID = -1;
                    io.PERIOD_DATE = DateTime.Now;
                    io.MORE_INFO = "Vốn đầu tư ban đầu khi đăng ký cửa hàng";
                    io.STORE_ID = st.ID;
                    io.INOUT_TYPE_ID = item.ID;
                    io.INOUT_DATE = DateTime.Now;
                    io.SEARCH_TEXT = string.Format("{0} {1} ngày {2}", io.MORE_INFO, io.IN_AMOUNT, io.INOUT_DATE);

                    rb1.InOuts.Add(io);
                    rb1.SaveChanges();

                    StoreFee sf = new StoreFee();
                    sf.STORE_ID = st.ID;
                    sf.FEE_PERCENT = 0;
                    rb1.StoreFees.Add(sf);
                    rb1.SaveChanges();
                }

                WriteLog(Constants.ACTION_CREATE_STORE, false);

                scope.Complete();
            }
        }

        private void UpdateStore(string id)
        {
            using (var db = new RentBikeEntities())
            {
                int storeid = Convert.ToInt32(id);
                var st = (from s in db.Stores
                          where s.ID == storeid
                          select s).FirstOrDefault();

                st.NAME = txtName.Text.Trim();
                st.ADDRESS = txtAddress.Text.Trim();
                st.CITY_ID = Convert.ToInt32(ddlCity.SelectedValue);
                st.PHONE = txtPhone.Text.Trim();
                st.FAX = string.Empty;
                st.START_CAPITAL = Convert.ToDecimal(txtStartCapital.Text);
                st.CURRENT_CAPITAL = Convert.ToDecimal(txtCurrentCapital.Text);
                //st.APPLY_DATE = Convert.ToDateTime(txtApplyDate.Text.Trim());
                //st.REVENUE_BEFORE_APPLY = Convert.ToDecimal(txtTotalRevenueBefore.Text);
                //st.TOTAL_COST_BEFORE = Convert.ToDecimal(txtTotalCostBefore.Text);
                //st.TOTAL_INVESMENT_BEFORE = Convert.ToDecimal(txtTotalInvesmentBefore.Text);
                st.REGISTER_DATE = Convert.ToDateTime(txtRegisterDate.Text.Trim());
                st.ACTIVE = rdbActive.Checked;
                st.NOTE = txtNote.Text.Trim();
                st.SEARCH_TEXT = string.Format("{0} {1} {2}", st.NAME, st.ADDRESS, st.PHONE);

                db.SaveChanges();


                var storefee = db.StoreFees.FirstOrDefault(s =>s.STORE_ID == storeid);
                storefee.FEE_PERCENT = 0;
                db.SaveChanges();
            }

            WriteLog(Constants.ACTION_UPDATE_STORE, false);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("FormStoreManagement.aspx");
        }

        private void WriteLog(string action, bool isCrashed)
        {
            Log lg = new Log();
            lg.ACCOUNT = Session["username"].ToString();
            lg.STORE = STORE_NAME;
            lg.LOG_ACTION = action;
            lg.LOG_DATE = DateTime.Now;
            lg.IS_CRASH = isCrashed;
            lg.LOG_MSG = string.Format("Tài khoản {0} {1} thực hiện {2} vào lúc {3}", lg.ACCOUNT, STORE_NAME, lg.LOG_ACTION, lg.LOG_DATE);
            lg.SEARCH_TEXT = lg.LOG_MSG;

            using (var db = new RentBikeEntities())
            {
                db.Logs.Add(lg);
                db.SaveChanges();
            }
        }
    }
}