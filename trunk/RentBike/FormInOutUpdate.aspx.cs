﻿using RentBike.Common;
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
                    var item = db.CONTRACT_FULL_VW.FirstOrDefault(itm => itm.ID == pp.CONTRACT_ID);
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
        }

        private void LoadGrid(PayPeriod pp)
        {
            List<InOut> payList = new List<InOut>();
            using (var db = new RentBikeEntities())
            {
                var itemLst = db.InOuts.Where(s => s.PERIOD_ID == pp.ID);
                payList = itemLst.ToList();

                //PayPeriod payPeriod = db.PayPeriods.FirstOrDefault(c => c.CONTRACT_ID == pp.CONTRACT_ID && c.ID == pp.ID);
                //if(payPeriod != null)
                //{
                //    payPeriod.ACTUAL_PAY = payList.Select(c => c.IN_AMOUNT).DefaultIfEmpty(0).Sum();
                //    db.SaveChanges();
                //}
            }

            rptContractInOut.DataSource = payList;
            rptContractInOut.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // SAVE INOUT
            int periodId = Convert.ToInt16(Request.QueryString["ID"]);
            PayPeriod pp = new PayPeriod();
            using (var db = new RentBikeEntities())
            {
                var item = db.PayPeriods.FirstOrDefault(s => s.ID == periodId);
                item.ACTUAL_PAY = Convert.ToDecimal(txtIncome.Text);
                pp = item;
                db.SaveChanges();
            }

            using (var db = new RentBikeEntities())
            {
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
                io.UPDATED_BY = Session["username"].ToString();
                io.UPDATED_DATE = DateTime.Now;

                db.InOuts.Add(io);
                db.SaveChanges();
            }

            //LoadGrid(pp);
            //LoadPaidAmountAndTheLeft(pp.CONTRACT_ID, pp.ID);
            Response.Redirect("FormContractUpdate.aspx?ID=" + pp.CONTRACT_ID);
        }

        private void LoadPaidAmountAndTheLeft(PayPeriod pp)
        {
            List<InOut> lst = new List<InOut>();
            List<PayPeriod> lst1 = new List<PayPeriod>();
            decimal total = 0;
            decimal remain = 0;
            decimal amountLeft = 0;

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
                    //else if (period_id == lstPeriod[1].ID)
                    //{
                    //    if (lstPeriod[0].ACTUAL_PAY > lstPeriod[0].AMOUNT_PER_PERIOD)
                    //    {
                    //        remain = lstPeriod[0].ACTUAL_PAY - lstPeriod[0].AMOUNT_PER_PERIOD;
                    //    }
                    //}
                    //else
                    //{
                    //    if ((lstPeriod[0].ACTUAL_PAY + lstPeriod[1].ACTUAL_PAY ) > (lstPeriod[0].AMOUNT_PER_PERIOD * 2))
                    //    {
                    //        remain = (lstPeriod[0].ACTUAL_PAY + lstPeriod[1].ACTUAL_PAY) - (lstPeriod[0].AMOUNT_PER_PERIOD * 2);
                    //    }
                    //}
                }

                if (pp.AMOUNT_PER_PERIOD - total > 0)
                {
                    amountLeft = pp.AMOUNT_PER_PERIOD - total - remain;
                }
            }

            Label lblAmountPerDay = (Label)rptContractInOut.Controls[rptContractInOut.Controls.Count - 1].Controls[0].FindControl("lblAmountPerDay");
            lblAmountPerDay.Text = string.Format("{0:0,0}", pp.AMOUNT_PER_PERIOD);
            Label lblTotalPaid = (Label)rptContractInOut.Controls[rptContractInOut.Controls.Count - 1].Controls[0].FindControl("lblTotalPaid");
            lblTotalPaid.Text = string.Format("{0:0,0}", total);
            Label lblAmountRemain = (Label)rptContractInOut.Controls[rptContractInOut.Controls.Count - 1].Controls[0].FindControl("lblAmountRemain");
            lblAmountRemain.Text = string.Format("{0:0,0}", remain);
            Label lblAmountLeft = (Label)rptContractInOut.Controls[rptContractInOut.Controls.Count - 1].Controls[0].FindControl("lblAmountLeft");
            lblAmountLeft.Text = string.Format("{0:0,0}", amountLeft);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            int periodId = Convert.ToInt16(Request.QueryString["ID"]);
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