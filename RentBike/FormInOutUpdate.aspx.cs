using RentBike.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RentBike
{
    public partial class FormInOutUpdate : FormBase
    {
        public DateTime PeriodDate { get; set; }
        public string CustomerName { get; set; }
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!IsPostBack)
            {
                // LOAD PAYPERIOD
                int periodId = Convert.ToInt32(Request.QueryString["ID"]);
                PayPeriod pp = new PayPeriod();
                using (var db = new RentBikeEntities())
                {
                    var item = db.PayPeriods.Where(s => s.ID == periodId).FirstOrDefault();
                    pp = item;
                    PeriodDate = item.PAY_DATE;
                }

                LoadGrid(pp);
                LoadPaidAmountAndTheLeft(pp);
                // DISPLAY SREEN
                hplContract.NavigateUrl = string.Format("FormContractUpdate.aspx?ID={0}", pp.CONTRACT_ID);
                hplContract.Text = "Xem chi tiết hợp đồng";

                Store st = new Store();
                using (var db = new RentBikeEntities())
                {
                    var contract = db.Contracts.Where(c => c.ID == pp.CONTRACT_ID).FirstOrDefault();
                    if (contract != null)
                    {
                        STORE_ID = contract.STORE_ID;
                        var item = db.Stores.Where(s => s.ID == contract.STORE_ID).FirstOrDefault();
                        if (item != null)
                        {
                            st = item;
                            txtStore.Text = st.NAME;
                            txtStore.Enabled = false;
                        }

                        var customer = db.Customers.FirstOrDefault(c =>c.ID == contract.CUSTOMER_ID);
                        if (customer != null) CustomerName = customer.NAME;
                    }
                }

                using (var db = new RentBikeEntities())
                {
                    var item = db.CONTRACT_FULL_VW.FirstOrDefault(itm => itm.ID == pp.CONTRACT_ID && itm.ACTIVE == true);
                    int inoutType = 0;
                    switch (item.RENT_TYPE_NAME)
                    {
                        case "Cho thuê xe":
                            inoutType = 14;
                            break;
                        case "Cho thuê thiết bị văn phòng":
                            inoutType = 15;
                            break;
                        case "Cho thuê mặt hàng khác":
                            inoutType = 16;
                            break;
                    }

                    var inouttypelist = db.InOutTypes.Where(s => s.IS_CONTRACT == true && s.ACTIVE == true).ToList();

                    ddInOutType.DataSource = inouttypelist;
                    ddInOutType.DataTextField = "NAME";
                    ddInOutType.DataValueField = "ID";
                    ddInOutType.DataBind();
                    ddInOutType.SelectedValue = inoutType.ToString();

                    //txtIncome.Text = pp.ACTUAL_PAY.ToString();

                    //
                }
            }
            //Disable UI for Admin account
            if (IS_ADMIN) pnlTable.Enabled = false;
        }

        private void LoadGrid(PayPeriod pp)
        {
            List<InOut> payList = new List<InOut>();
            using (var db = new RentBikeEntities())
            {
                var itemLst = db.InOuts.Where(s => s.PERIOD_ID == pp.ID && s.IN_AMOUNT != 0).ToList();

                rptContractInOut.DataSource = itemLst;
                rptContractInOut.DataBind();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // SAVE INOUT
            int periodId = Convert.ToInt32(Request.QueryString["ID"]);
            using (var db = new RentBikeEntities())
            {
                if (!string.IsNullOrEmpty(txtIncome.Text))
                {
                    var pp = db.PayPeriods.FirstOrDefault(s => s.ID == periodId);
                    pp.ACTUAL_PAY = Convert.ToDecimal(txtIncome.Text) + pp.ACTUAL_PAY;
                    db.SaveChanges();

                    var contract = db.Contracts.FirstOrDefault(c => c.ID == pp.CONTRACT_ID && c.CONTRACT_STATUS == true);
                    List<PayPeriod> payList = db.PayPeriods.Where(c => c.CONTRACT_ID == pp.CONTRACT_ID).ToList();
                    decimal totalActualPay = payList.Select(c => c.ACTUAL_PAY).DefaultIfEmpty(0).Sum();
                    decimal totalPlanPay = payList.Select(c => c.AMOUNT_PER_PERIOD).DefaultIfEmpty(0).Sum();

                    if (totalActualPay > totalPlanPay)
                    {
                        CommonList.CreatePayPeriod(db, contract.ID, payList.LastOrDefault().PAY_DATE, false);
                    }

                    InOut io = new InOut();
                    io.IN_AMOUNT = Convert.ToDecimal(txtIncome.Text);
                    io.OUT_AMOUNT = 0;
                    io.CONTRACT_ID = pp.CONTRACT_ID;
                    io.PERIOD_ID = pp.ID;
                    io.RENT_TYPE_ID = contract.RENT_TYPE_ID;
                    io.INOUT_TYPE_ID = Convert.ToInt32(ddInOutType.SelectedValue);
                    io.PERIOD_DATE = pp.PAY_DATE;
                    io.MORE_INFO = txtMoreInfo.Text.Trim();
                    io.STORE_ID = Convert.ToInt32(Session["store_id"]);
                    io.SEARCH_TEXT = string.Format("{0} ", io.MORE_INFO);
                    io.INOUT_DATE = DateTime.Now;
                    io.CREATED_BY = Session["username"].ToString();
                    io.CREATED_DATE = DateTime.Now;
                    io.UPDATED_BY = Session["username"].ToString();
                    io.UPDATED_DATE = DateTime.Now;

                    db.InOuts.Add(io);
                    db.SaveChanges();

                    Response.Redirect("FormContractUpdate.aspx?ID=" + pp.CONTRACT_ID, false);
                }
            }
        }

        private void LoadPaidAmountAndTheLeft(PayPeriod pp)
        {
            List<InOut> lst = new List<InOut>();
            List<PayPeriod> lst1 = new List<PayPeriod>();
            decimal total = 0;
            decimal remain = 0;
            decimal amountLeft = 0;
            decimal totalAmountLeft = 0;
            using (var db = new RentBikeEntities())
            {
                var result = db.InOuts.Where(itm => itm.CONTRACT_ID == pp.CONTRACT_ID && itm.PERIOD_ID == pp.ID).ToList();
                foreach (InOut io in result)
                {
                    total += io.IN_AMOUNT;
                    total -= io.OUT_AMOUNT;
                }

                var lstPeriod = db.PayPeriods.Where(c => c.CONTRACT_ID == pp.CONTRACT_ID).ToList();
                if (lstPeriod != null)
                {
                    if (pp.ID == lstPeriod[0].ID)
                    {
                        remain = 0;
                    }
                    else
                    {
                        decimal totalPerAmount = 0;
                        decimal totalActualPay = 0;
                        foreach (PayPeriod pay in lstPeriod)
                        {
                            if (pay.ID < pp.ID)
                            {
                                totalPerAmount = totalPerAmount + pay.AMOUNT_PER_PERIOD;
                                totalActualPay = totalActualPay + pay.ACTUAL_PAY;
                            }
                        }
                        if (totalActualPay > totalPerAmount)
                            remain = totalActualPay - totalPerAmount;
                    }

                    decimal totalAmountPeriod = lstPeriod.Where(c => c.PAY_DATE <= DateTime.Today).Select(c => c.AMOUNT_PER_PERIOD).DefaultIfEmpty(0).Sum();
                    decimal totalAmountPaid = lstPeriod.Where(c => c.PAY_DATE <= DateTime.Today).Select(c => c.ACTUAL_PAY).DefaultIfEmpty(0).Sum();
                    totalAmountLeft = totalAmountPeriod - totalAmountPaid <= 0 ? 0 : totalAmountPeriod - totalAmountPaid;
                }

                if (pp.AMOUNT_PER_PERIOD - total > 0)
                {
                    amountLeft = pp.AMOUNT_PER_PERIOD - total - remain <= 0 ? 0 : pp.AMOUNT_PER_PERIOD - total - remain;
                }
            }

            Label lblAmountPerDay = (Label)rptContractInOut.Controls[rptContractInOut.Controls.Count - 1].Controls[0].FindControl("lblAmountPerDay");
            lblAmountPerDay.Text = string.Format("{0:0,0}", pp.AMOUNT_PER_PERIOD);
            Label lblTotalPaid = (Label)rptContractInOut.Controls[rptContractInOut.Controls.Count - 1].Controls[0].FindControl("lblTotalPaid");
            lblTotalPaid.Text = string.Format("{0:0,0}", total);
            Label lblAmountRemain = (Label)rptContractInOut.Controls[rptContractInOut.Controls.Count - 1].Controls[0].FindControl("lblAmountRemain");
            lblAmountRemain.Text = string.Format("{0:0,0}", remain);
            Label lblAmountLeft = (Label)rptContractInOut.Controls[rptContractInOut.Controls.Count - 1].Controls[0].FindControl("lblAmountLeft");
            lblAmountLeft.Text = txtIncome.Text = string.Format("{0:0,0}", amountLeft);
            Label lblTotalAmoutLeft = (Label)rptContractInOut.Controls[rptContractInOut.Controls.Count - 1].Controls[0].FindControl("lblTotalAmoutLeft");
            lblTotalAmoutLeft.Text = string.Format("{0:0,0}", totalAmountLeft);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            int periodId = Convert.ToInt32(Request.QueryString["ID"]);
            PayPeriod pp = new PayPeriod();
            using (var db = new RentBikeEntities())
            {
                var item = db.PayPeriods.FirstOrDefault(s => s.ID == periodId);
                pp = item;
            }

            Response.Redirect(string.Format("FormContractUpdate.aspx?ID={0}", pp.CONTRACT_ID));
        }
    }
}