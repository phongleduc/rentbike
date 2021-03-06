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
    public partial class FormDailyIncomeOutcomeUpdate : FormBase
    {
        private int inOutId = 0;
        private int storeId = 0;
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            inOutId = Helper.parseInt(Request.QueryString["id"]);
            storeId = Helper.parseInt(Request.QueryString["sID"]);
            if (!IsPostBack)
            {
                txtFeeDate.Text = DateTime.Today.ToString("MM/dd/yyyy");
                LoadStore();
                LoadInOutType();
                LoadInfor();
            }
            //Disable UI for Admin account
            if (IS_ADMIN) pnlTable.Enabled = false;
        }

        private void LoadStore()
        {
            List<Store> lst = new List<Store>();
            using (var db = new RentBikeEntities())
            {
                var data = from itm in db.Stores
                           select itm;

                if (data.Any())
                {
                    foreach (Store store in data)
                    {
                        ddlStore.Items.Add(new ListItem(store.NAME, store.ID.ToString()));
                    }
                    ddlStore.SelectedValue = storeId.ToString();
                    if (!IS_ADMIN)
                        ddlStore.Enabled = false;
                }
            }
        }

        private void LoadInOutType()
        {
            List<InOutType> lst = new List<InOutType>();
            using (var db = new RentBikeEntities())
            {
                var item = from itm in db.InOutTypes
                           where itm.IS_CONTRACT == false && itm.ACTIVE == true
                           select itm;

                lst = item.ToList();

                if (lst.Count > 0)
                {
                    ddlInOutFee.Items.Add(new ListItem { Text = "Hãy chọn loại chi phí", Value = "-1"});
                    foreach(var data in lst)
                    {
                        ddlInOutFee.Items.Add(new ListItem { Text = data.NAME, Value = data.ID.ToString() });
                    }
                    if (inOutId != 0)
                    {
                        var io = db.InOuts.FirstOrDefault(c =>c.ID == inOutId);
                        ddlInOutFee.SelectedValue = io.INOUT_TYPE_ID.ToString();
                    }
                }
            }
        }

        private void LoadInfor()
        {
            using (var db = new RentBikeEntities())
            {
                if (inOutId == 0) return;

                var io = db.InOuts.FirstOrDefault(c =>c.ID == inOutId);
                if (io != null)
                {
                    var ioType = db.InOutTypes.FirstOrDefault(c =>c.ID == io.INOUT_TYPE_ID);
                    if (ioType != null)
                    {
                        if (ioType.IS_INCOME)
                            txtFeeAmount.Text = io.IN_AMOUNT.ToString();
                        else
                            txtFeeAmount.Text = io.OUT_AMOUNT.ToString();
                    }
                    txtMoreInfo.Text = io.MORE_INFO;
                    txtFeeDate.Text = io.INOUT_DATE.Value.ToString("MM/dd/yyyy");
                }
            }
        }

        protected string ValidateFields()
        {
            if (string.IsNullOrEmpty(ddlInOutFee.SelectedValue.Trim()) || "-1".Equals(ddlInOutFee.SelectedValue))
            {
                return "Bạn cần phải chọn loại chi phí.";
            }

            if (PERMISSION == ROLE.STAFF)
            {
                if(DateTime.Today.Subtract(Convert.ToDateTime(txtFeeDate.Text)).Days > 0)
                {
                    if (string.IsNullOrEmpty(txtUsername.Text.Trim()) || string.IsNullOrEmpty(txtPassword.Text.Trim()))
                    {
                        return "Bạn cần phải nhập tài khoản cửa hàng trưởng để xác nhận.";
                    }

                    using (var db = new RentBikeEntities())
                    {

                        var acc = db.Accounts.ToList().Where(c => c.ACC == txtUsername.Text.Trim()
                        && c.PASSWORD == Helper.EncryptPassword(txtPassword.Text.Trim())
                        && c.STORE_ID == STORE_ID).FirstOrDefault();

                        if (acc == null)
                        {
                            return "Thông tin cửa hàng trưởng bạn nhập không tồn tại.";
                        }
                    }
                }
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

            using (TransactionScope ts = new TransactionScope())
            {

                using (var db = new RentBikeEntities())
                {
                    string message = string.Empty;
                    InOut io = db.InOuts.FirstOrDefault(c =>c.ID == inOutId);
                    if (io == null)
                    {
                        io = new InOut();
                        io.INOUT_TYPE_ID = Convert.ToInt32(ddlInOutFee.SelectedValue);
                        io.MORE_INFO = txtMoreInfo.Text.Trim();
                        io.CONTRACT_ID = -1;
                        io.PERIOD_ID = -1;
                        io.RENT_TYPE_ID = -1;
                        io.PERIOD_DATE = new DateTime(1, 1, 1);
                        if (ddlStore.Enabled == false)
                        {
                            io.STORE_ID = STORE_ID;
                        }
                        else
                        {
                            io.STORE_ID = Convert.ToInt32(ddlStore.SelectedValue);
                        }

                        var item = db.InOutTypes.FirstOrDefault(s =>s.ID == io.INOUT_TYPE_ID);

                        if (item.IS_INCOME)
                        {
                            io.IN_AMOUNT = Convert.ToDecimal(txtFeeAmount.Text.Trim());
                            io.OUT_AMOUNT = 0;
                            message = string.Format("Tài khoản {0} cửa hàng {1} thực hiện {2} ({3}) vào lúc {4}", Convert.ToString(Session["username"]), STORE_NAME, ddlInOutFee.SelectedItem.Text, Helper.FormatedAsCurrency(io.IN_AMOUNT), DateTime.Now);
                        }
                        else
                        {
                            io.IN_AMOUNT = 0;
                            io.OUT_AMOUNT = Convert.ToDecimal(txtFeeAmount.Text.Trim());
                            message = string.Format("Tài khoản {0} cửa hàng {1} thực hiện {2} ({3}) vào lúc {4}", Convert.ToString(Session["username"]), STORE_NAME, ddlInOutFee.SelectedItem.Text, Helper.FormatedAsCurrency(io.IN_AMOUNT), DateTime.Now);
                        }

                        io.INOUT_DATE = Convert.ToDateTime(txtFeeDate.Text);
                        io.SEARCH_TEXT = string.Format("{0} {1} {2}", io.INOUT_DATE, io.MORE_INFO, item.NAME);
                        io.CREATED_BY = Session["username"].ToString();
                        io.CREATED_DATE = DateTime.Now;
                        io.UPDATED_BY = Session["username"].ToString();
                        io.UPDATED_DATE = DateTime.Now;

                        db.InOuts.Add(io);
                        Helper.WriteLog(Convert.ToString(Session["username"]), STORE_NAME, Constants.ACTION_CREATE_INOUT, message, false);
                    }
                    else
                    {
                        io.INOUT_TYPE_ID = Convert.ToInt32(ddlInOutFee.SelectedValue);
                        io.INOUT_DATE = Convert.ToDateTime(txtFeeDate.Text);
                        io.MORE_INFO = txtMoreInfo.Text.Trim();
                        if (ddlStore.Enabled == false)
                        {
                            io.STORE_ID = Convert.ToInt32(Session["store_id"]);
                        }
                        else
                        {
                            io.STORE_ID = Convert.ToInt32(ddlStore.SelectedValue);
                        }

                        var item = db.InOutTypes.FirstOrDefault(s =>s.ID == io.INOUT_TYPE_ID);
                        if (item.IS_INCOME)
                        {
                            io.IN_AMOUNT = Convert.ToDecimal(txtFeeAmount.Text.Trim());
                            io.OUT_AMOUNT = 0;
                            message = string.Format("Tài khoản {0} cửa hàng {1} thực hiện chỉnh sửa {2} ({3}) vào lúc {4}", Convert.ToString(Session["username"]), STORE_NAME, ddlInOutFee.SelectedItem.Text, Helper.FormatedAsCurrency(io.IN_AMOUNT), DateTime.Now);
                        }
                        else
                        {
                            io.IN_AMOUNT = 0;
                            io.OUT_AMOUNT = Convert.ToDecimal(txtFeeAmount.Text.Trim());
                            message = string.Format("Tài khoản {0} cửa hàng {1} thực hiện chỉnh sửa {2} ({3}) vào lúc {4}", Convert.ToString(Session["username"]), STORE_NAME, ddlInOutFee.SelectedItem.Text, Helper.FormatedAsCurrency(io.IN_AMOUNT), DateTime.Now);
                        }
                        io.SEARCH_TEXT = string.Format("{0} {1} {2}", io.INOUT_DATE, io.MORE_INFO, item.NAME);
                        io.UPDATED_BY = Session["username"].ToString();
                        io.UPDATED_DATE = DateTime.Now;

                        Helper.WriteLog(Convert.ToString(Session["username"]), STORE_NAME, Constants.ACTION_CREATE_INOUT, message, false);
                    }
                    db.SaveChanges();
                }

                ts.Complete();
            }
            Response.Redirect("FormDailyIncomeOutcome.aspx", false);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("FormDailyIncomeOutcome.aspx", false);
        }
    }
}