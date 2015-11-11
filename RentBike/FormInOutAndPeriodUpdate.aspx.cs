using RentBike.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RentBike
{
    public partial class FormInOutAndPeriodUpdate : FormBase
    {
        public DateTime InOutDate { get; set; }
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!IsPostBack)
            {
                // LOAD PAYPERIOD
                int inOutId = Helper.parseInt(Request.QueryString["id"]);
                if (inOutId != 0)
                {
                    using (var db = new RentBikeEntities())
                    {
                        var io = db.InOuts.FirstOrDefault(c =>c.ID == inOutId);
                        InOutDate = io.INOUT_DATE.Value;
                        // DISPLAY SREEN
                        txtIncome.Text = Convert.ToString(io.IN_AMOUNT.ToString());
                        txtMoreInfo.Text = io.MORE_INFO;
                        hplContract.NavigateUrl = string.Format("FormContractUpdate.aspx?ID={0}", io.CONTRACT_ID);
                        hplContract.Text = "Xem chi tiết hợp đồng";

                        var store = db.Stores.FirstOrDefault(s =>s.ID == io.STORE_ID && s.ACTIVE == true);
                        if (store != null)
                        {
                            txtStore.Text = store.NAME;
                            txtStore.Enabled = false;
                        }

                        var lst = db.InOutTypes.Where(s => s.IS_CONTRACT == true && s.ACTIVE == true).ToList();
                        foreach (var data in lst)
                        {
                            ddInOutType.Items.Add(new ListItem { Text = data.NAME, Value = data.ID.ToString() });
                        }
                        ddInOutType.SelectedValue = io.INOUT_TYPE_ID.ToString();
                    }
                }
            }
            //Disable UI for Admin account
            if (IS_ADMIN) pnlTable.Enabled = false;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string result = ValidateFields();
            if (!string.IsNullOrEmpty(result))
            {
                lblMessage.Text = result;
                return;
            }

            using (var db = new RentBikeEntities())
            {
                int inOutId = Helper.parseInt(Request.QueryString["id"]);
                if (inOutId != 0)
                {
                    var io = db.InOuts.FirstOrDefault(c =>c.ID == inOutId);
                    // SAVE INOUT
                    io.IN_AMOUNT = Convert.ToDecimal(txtIncome.Text);
                    io.MORE_INFO = txtMoreInfo.Text.Trim();
                    io.UPDATED_BY = Session["username"].ToString();
                    io.UPDATED_DATE = DateTime.Now;
                    db.SaveChanges();
                }

                int periodId = Helper.parseInt(Request.QueryString["pid"]);
                if (periodId != 0)
                {
                    // SAVE PERIOD
                    decimal totalInAmountOfPeriod = db.InOuts.Where(c =>c.PERIOD_ID == periodId).Select(c =>c.IN_AMOUNT).DefaultIfEmpty(0).Sum(); 
                    var pp = db.PayPeriods.FirstOrDefault(c =>c.ID == periodId);
                    pp.ACTUAL_PAY = totalInAmountOfPeriod;
                    db.SaveChanges();

                    var contract = db.Contracts.FirstOrDefault(c => c.ID == pp.CONTRACT_ID);
                    var customer = db.Customers.FirstOrDefault(c =>c.ID == contract.CUSTOMER_ID);
                    string message = string.Format("Tài khoản {0} cửa hàng {1} thực hiện chỉnh sửa thu phí kỳ hạn ngày {2} của hợp đồng {3} số tiền {4} vào lúc {5}", Convert.ToString(Session["username"]), STORE_NAME, pp.PAY_DATE.ToString("dd/MM/yyyy"), customer.NAME, Helper.FormatedAsCurrency(Convert.ToDecimal(txtIncome.Text.Trim())), DateTime.Now);
                    Helper.WriteLog(Convert.ToString(Session["username"]), STORE_NAME, Constants.ACTION_UPDATE_INOUT, message, false);

                    Response.Redirect(string.Format("FormInOutUpdate.aspx?ID={0}", periodId), false);
                }
                Response.Redirect("FormDailyIncomeOutcome.aspx", false);
            }
        }

        protected string ValidateFields()
        {
            if (string.IsNullOrEmpty(ddInOutType.SelectedValue.Trim()) || "-1".Equals(ddInOutType.SelectedValue))
            {
                return "Bạn cần phải chọn loại chi phí.";
            }

            if (PERMISSION == ROLE.STAFF)
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
            return string.Empty;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("FormInOutUpdate.aspx?ID={0}", Request.QueryString["pid"]), false);
        }
    }
}