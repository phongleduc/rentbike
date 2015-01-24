using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RentBike
{
    public partial class FormInOutUpdate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["store_id"] == null)
            {
                Response.Redirect("FormLogin.aspx");
            }
            if (!IsPostBack)
            {
                // LOAD PAYPERIOD
                int periodId = Convert.ToInt16(Request.QueryString["ID"]);
                PayPeriod pp = new PayPeriod();
                using (var db = new RentBikeEntities())
                {
                    var item = db.PayPeriods.Where(s => s.ID == periodId).FirstOrDefault();
                    pp = item;
                }

                LoadGrid(pp);
                LoadPaidAmountAndTheLeft(pp.CONTRACT_ID, pp.ID);
                // DISPLAY SREEN
                hplContract.NavigateUrl = string.Format("FormContractUpdate.aspx?ID={0}", pp.CONTRACT_ID);
                hplContract.Text = "Xem chi tiết hợp đồng";

                Store st = new Store();
                using (var db = new RentBikeEntities())
                {
                    var contract = db.Contracts.Where(c => c.ID == pp.CONTRACT_ID).FirstOrDefault();
                    if (contract != null)
                    {
                        Session["store_id"] = contract.STORE_ID;
                        var item = db.Stores.Where(s => s.ID == contract.STORE_ID).FirstOrDefault();
                        if (item != null)
                        {
                            st = item;
                            txtStore.Text = st.NAME;
                            txtStore.Enabled = false;
                        }
                    }
                }

                using (var db = new RentBikeEntities())
                {
                    var item = db.CONTRACT_FULL_VW.First(itm => itm.ID == pp.CONTRACT_ID);
                    string feeName = string.Empty;
                    switch (item.RENT_TYPE_NAME)
                    {
                        case "Cho thuê xe":
                            feeName = "Phí thuê xe";
                            break;
                        case "Cho thuê thiết bị văn phòng":
                            feeName = "Phí thuê thiết bị";
                            break;
                        case "Cho thuê mặt hàng khác":
                            feeName = "Phí khác";
                            break;
                    }

                    var item1 = db.InOutTypes.First(s => s.NAME == feeName);
                    var inouttypelist = db.InOutTypes.Where(s => s.IS_CONTRACT == true && s.ACTIVE == true).ToList();

                    ddInOutType.DataSource = inouttypelist;
                    ddInOutType.DataTextField = "NAME";
                    ddInOutType.DataValueField = "ID";
                    ddInOutType.DataBind();
                    ddInOutType.SelectedItem.Text = item.RENT_TYPE_NAME;

                    txtIncome.Text = "0";
                }
            }
        }

        private void LoadGrid(PayPeriod pp)
        {
            List<InOut> payList = new List<InOut>();
            using (var db = new RentBikeEntities())
            {
                var itemLst = db.InOuts.Where(s => s.PERIOD_ID == pp.ID);
                payList = itemLst.ToList();
            }

            rptContractInOut.DataSource = payList;
            rptContractInOut.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // SAVE INOUT
            int periodId = Convert.ToInt16(Request.QueryString["ID"]);
            using (var db = new RentBikeEntities())
            {
                var pp = db.PayPeriods.First(s => s.ID == periodId);
                InOut io = new InOut();
                io.IN_AMOUNT = Convert.ToDecimal(txtIncome.Text);
                io.OUT_AMOUNT = 0;
                io.CONTRACT_ID = pp.CONTRACT_ID;
                io.PERIOD_ID = pp.ID;
                //io.RENT_TYPE_ID = Convert.ToInt16(ddlRentType.SelectedValue);
                io.INOUT_TYPE_ID = Convert.ToInt16(ddInOutType.SelectedValue);
                io.PERIOD_DATE = pp.PAY_DATE;
                io.MORE_INFO = txtMoreInfo.Text.Trim();
                io.STORE_ID = Convert.ToInt16(Session["store_id"]);
                io.SEARCH_TEXT = string.Format("{0} ", io.MORE_INFO);
                io.INOUT_DATE = DateTime.Now;

                db.InOuts.Add(io);
                db.SaveChanges();

                int contract_id = pp.CONTRACT_ID;
                var result = db.PayPeriods.Where(c => c.CONTRACT_ID == contract_id).ToList();
                var first = result[0];
                var second = result[1];
                var three = result[2];

                decimal remain = 0;
                if (pp.PAY_DATE.Subtract(first.PAY_DATE).Days == 10)
                {
                    if (first.ACTUAL_PAY > first.AMOUNT_PER_PERIOD)
                    {
                        remain = first.ACTUAL_PAY - first.AMOUNT_PER_PERIOD;
                        first = db.PayPeriods.First(s => s.ID == first.ID);
                        first.ACTUAL_PAY -= remain;
                    }
                }
                else if (pp.PAY_DATE.Subtract(first.PAY_DATE).Days == 20)
                {
                    if (first.ACTUAL_PAY > first.AMOUNT_PER_PERIOD)
                    {
                        remain = first.ACTUAL_PAY - first.AMOUNT_PER_PERIOD;
                        first = db.PayPeriods.First(s => s.ID == first.ID);
                        first.ACTUAL_PAY -= remain;
                    }
                    if (second.ACTUAL_PAY > second.AMOUNT_PER_PERIOD)
                    {
                        remain = second.ACTUAL_PAY - second.AMOUNT_PER_PERIOD;
                        second = db.PayPeriods.First(s => s.ID == second.ID);
                        second.ACTUAL_PAY -= remain;
                    }
                }
                pp.ACTUAL_PAY += Convert.ToDecimal(txtIncome.Text) + remain;
                db.SaveChanges();

                Response.Redirect("FormContractUpdate.aspx?ID=" + pp.CONTRACT_ID);
            }

            //LoadGrid(pp);
            //LoadPaidAmountAndTheLeft(pp.CONTRACT_ID, pp.ID);

        }

        private void LoadPaidAmountAndTheLeft(int contract_id, int period_id)
        {
            List<InOut> lst = new List<InOut>();
            List<PayPeriod> lst1 = new List<PayPeriod>();
            using (var db = new RentBikeEntities())
            {
                var result = db.InOuts.Where(itm => itm.CONTRACT_ID == contract_id && itm.PERIOD_ID == period_id);
                lst = result.ToList();

                var result1 = db.PayPeriods.Where(itm1 => itm1.ID == period_id);
                lst1 = result1.ToList();
            }

            decimal total = 0;
            for (int i = 0; i < lst.Count; i++)
            {
                total += lst[i].IN_AMOUNT;
                total -= lst[i].OUT_AMOUNT;
            }

            Label lblTotalPaid = (Label)rptContractInOut.Controls[rptContractInOut.Controls.Count - 1].Controls[0].FindControl("lblTotalPaid");
            lblTotalPaid.Text = string.Format("{0:0,0}", total);

            decimal remain = 0;
            using (var db = new RentBikeEntities())
            {
                PayPeriod pp = new PayPeriod();
                var result = db.PayPeriods.Where(c => c.CONTRACT_ID == contract_id).ToList();
                var item = result.First(s => s.ID == period_id);
                if (total != lst1[0].ACTUAL_PAY)
                {
                    item.ACTUAL_PAY = total;
                    db.SaveChanges();
                }

                var first = result[0];
                var second = result[1];
                var three = result[2];

                if (item.PAY_DATE.Subtract(first.PAY_DATE).Days == 10)
                {
                    if (first.ACTUAL_PAY > first.AMOUNT_PER_PERIOD)
                    {
                        remain = first.ACTUAL_PAY - first.AMOUNT_PER_PERIOD;
                    }
                }
                else if (item.PAY_DATE.Subtract(first.PAY_DATE).Days == 20)
                {
                    if (first.ACTUAL_PAY > first.AMOUNT_PER_PERIOD)
                    {
                        remain = first.ACTUAL_PAY - first.AMOUNT_PER_PERIOD;
                    }
                    if (second.ACTUAL_PAY > second.AMOUNT_PER_PERIOD)
                    {
                        remain += second.ACTUAL_PAY - second.AMOUNT_PER_PERIOD;
                    }
                }

                Label lblAmountRemain = (Label)rptContractInOut.Controls[rptContractInOut.Controls.Count - 1].Controls[0].FindControl("lblAmountRemain");
                lblAmountRemain.Text = string.Format("{0:0,0}", remain);

                Label lblAmountLeft = (Label)rptContractInOut.Controls[rptContractInOut.Controls.Count - 1].Controls[0].FindControl("lblAmountLeft");
                lblAmountLeft.Text = string.Format("{0:0,0}", (lst1.Count > 0 ? lst1[0].AMOUNT_PER_PERIOD : 0) - total - remain);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            int periodId = Convert.ToInt16(Request.QueryString["ID"]);
            PayPeriod pp = new PayPeriod();
            using (var db = new RentBikeEntities())
            {
                var item = db.PayPeriods.First(s => s.ID == periodId);
                pp = item;
            }

            Response.Redirect(string.Format("FormContractUpdate.aspx?ID={0}", pp.CONTRACT_ID));
        }
    }
}