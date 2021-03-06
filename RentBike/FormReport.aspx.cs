﻿using RentBike.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace RentBike
{
    public partial class FormReport : FormBase
    {
        public string SearchDate { get; set; }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!IsPostBack)
            {
                LoadData(string.Empty);
            }
        }

        private void LoadData(string searchText)
        {
            List<CONTRACT_FULL_VW> dataList = new List<CONTRACT_FULL_VW>();
            using (var db = new RentBikeEntities())
            {
                var st = db.CONTRACT_FULL_VW.Where(c =>c.CONTRACT_STATUS == true && c.ACTIVE == true);

                if (STORE_ID != 0)
                {
                    st = st.Where(c => c.STORE_ID == STORE_ID);
                }
                st = st.OrderByDescending(c =>c.ID);

                var lstPeriod = db.PayPeriods.Where(s =>s.STATUS == true).ToList();
                foreach (CONTRACT_FULL_VW c in st)
                {
                    var inOutList = db.InOuts.Where(s =>s.CONTRACT_ID == c.ID).ToList();

                    c.PAYED_TIME = 0;
                    c.PAY_DATE = c.RENT_DATE;
                    c.DAY_DONE = DateTime.Now.Subtract(c.PAY_DATE).Days;

                    var tmpLstPeriod = lstPeriod.Where(s =>s.CONTRACT_ID == c.ID);
                    if (tmpLstPeriod != null)
                    {
                        decimal totalAmountPeriod = tmpLstPeriod.Where(s => s.PAY_DATE <= DateTime.Today).Select(s => s.AMOUNT_PER_PERIOD).DefaultIfEmpty(0).Sum();
                        decimal totalAmountPaid = tmpLstPeriod.Where(s => s.PAY_DATE <= DateTime.Today).Select(s => s.ACTUAL_PAY).DefaultIfEmpty(0).Sum();
                        c.AMOUNT_LEFT = totalAmountPeriod - totalAmountPaid <= 0 ? 0 : totalAmountPeriod - totalAmountPaid;

                        decimal paidAmount = tmpLstPeriod.Where(s =>s.ACTUAL_PAY > 0).Select(s =>s.ACTUAL_PAY).DefaultIfEmpty(0).Sum();
                        int paidNumberOfFee = 0;
                        bool bAdd = false;
                        foreach (PayPeriod pp in tmpLstPeriod)
                        {
                            if (pp.AMOUNT_PER_PERIOD == 0)
                            {
                                c.OVER_DATE = 0;
                                break;
                            }
                            paidAmount -= pp.AMOUNT_PER_PERIOD;
                            if (paidAmount >= 0)
                                paidNumberOfFee += 1;

                            if (paidAmount <= 0)
                            {
                                if (paidAmount < 0)
                                {
                                    c.OVER_DATE = DateTime.Today.Subtract(pp.PAY_DATE).Days;
                                    c.PAY_DATE = pp.PAY_DATE;
                                }
                                else
                                {
                                    if (tmpLstPeriod.Any(s =>s.PAY_DATE == pp.PAY_DATE.AddDays(9)))
                                    {
                                        c.OVER_DATE = DateTime.Today.Subtract(pp.PAY_DATE.AddDays(9)).Days;
                                        c.PAY_DATE = pp.PAY_DATE.AddDays(9);
                                    }
                                    else
                                    {
                                        c.OVER_DATE = DateTime.Today.Subtract(pp.PAY_DATE.AddDays(10)).Days;
                                        c.PAY_DATE = pp.PAY_DATE.AddDays(10);
                                    }
                                }
                                c.PERIOD_ID = pp.ID;
                                if (c.OVER_DATE >= 0 && c.OVER_DATE <= 50)
                                    bAdd = true;
                                break;
                            }
                        }

                        if (bAdd)
                        {
                            c.PAYED_TIME = paidNumberOfFee;
                            c.DAY_DONE = DateTime.Now.Subtract(c.RENT_DATE).Days + 1;
                            c.FEE_PER_DAY = tmpLstPeriod.Where(s => DateTime.Today.Subtract(s.PAY_DATE).Days >= 0).OrderByDescending(s =>s.PAY_DATE).FirstOrDefault().AMOUNT_PER_PERIOD;
                            dataList.Add(c);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(searchText))
                {
                    dataList = dataList.Where(s => s.SEARCH_TEXT.ToLower().Contains(searchText.ToLower()) 
                        || s.CUSTOMER_NAME.ToLower().Contains(searchText.ToLower())).ToList();
                }
            }

            lblTotalAmountLeft.Text = string.Format("{0:0,0}", dataList.Select(c => c.AMOUNT_LEFT).DefaultIfEmpty(0).Sum());
            rptReport.DataSource = dataList.OrderByDescending(c => c.OVER_DATE);
            rptReport.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text.Trim());
        }

        protected override void ddlStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text.Trim());
        }

        public string ShowClass(int overDate)
        {
            if (overDate <= 5)
            {
                return "green";
            }
            else if (overDate <= 10)
            {
                return "orange";
            }
            else if (overDate > 10)
            {
                return "red";
            }
            return string.Empty;
        }
    }
}