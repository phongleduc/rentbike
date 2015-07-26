using RentBike.Common;
using RentBike.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RentBike
{
    public partial class FormIncomeOutcomeSummary : FormBase
    {
        public DateTime SearchDate { get; set; }
        public DateTime StartDate { get; set; }
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!IsPostBack)
            {
                //LoadData(string.Empty, string.Empty);
                LoadData(1);
            }
        }

        private void LoadData(string startDate, string endDate)
        {
            List<SummaryInfo> listSum = GetSummaryData(STORE_ID);
            if (listSum.Any())
            {
                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    listSum = listSum.Where(c => c.InOutDate >= Convert.ToDateTime(startDate) && c.InOutDate <= Convert.ToDateTime(endDate)).ToList();
                }
                else if (!string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))
                {
                    listSum = listSum.Where(c => c.InOutDate >= Convert.ToDateTime(startDate)).ToList();
                }
                else if (string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    listSum = listSum.Where(c => c.InOutDate <= Convert.ToDateTime(endDate)).ToList();
                }

                listSum = listSum.OrderByDescending(c => c.InOutDate).ToList();

                rptInOut.DataSource = listSum;
                rptInOut.DataBind();
                decimal sumIn = 0;
                decimal sumOut = 0;
                decimal sumBegin = 0;
                decimal sumEnd = 0;

                sumIn = listSum.Select(c => c.TotalIn).DefaultIfEmpty(0).Sum();
                sumOut = listSum.Select(c => c.TotalOut).DefaultIfEmpty(0).Sum();
                sumEnd = sumIn - sumOut;

                Label lblTotalIn = (Label)rptInOut.Controls[rptInOut.Controls.Count - 1].Controls[0].FindControl("lblTotalIn");
                Label lblTotalOut = (Label)rptInOut.Controls[rptInOut.Controls.Count - 1].Controls[0].FindControl("lblTotalOut");
                Label lblTotalBegin = (Label)rptInOut.Controls[rptInOut.Controls.Count - 1].Controls[0].FindControl("lblTotalBegin");
                Label lblTotalEnd = (Label)rptInOut.Controls[rptInOut.Controls.Count - 1].Controls[0].FindControl("lblTotalEnd");
                lblTotalIn.Text = string.Format("{0:0,0}", sumIn);
                lblTotalOut.Text = string.Format("{0:0,0}", sumOut);
                lblTotalBegin.Text = string.Format("{0:0,0}", sumBegin);
                lblTotalEnd.Text = string.Format("{0:0,0}", sumEnd);
            }
        }

        private void LoadData(int page)
        {
            using (var db = new RentBikeEntities())
            {
                List<SummaryInfo> listSum = GetSummaryData(STORE_ID);
                if (listSum.Any())
                {
                    int count = ((listSum.LastOrDefault().InOutDate.Year - listSum.FirstOrDefault().InOutDate.Year) * 12) + (listSum.LastOrDefault().InOutDate.Month - listSum.FirstOrDefault().InOutDate.Month) + 1;
                    IEnumerable<int> pageList = Enumerable.Range(1, count);

                    DateTime startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 6);
                    StartDate = startDate = DateTime.Today < startDate == true ? new DateTime(DateTime.Today.Year, DateTime.Today.Month - 1, 6) : startDate;

                    if (page > 1)
                    {
                        startDate = startDate.AddMonths(1 - page);
                        StartDate = startDate = new DateTime(startDate.Year, startDate.Month, 6);
                    }
                    if (ViewState["StartDate"] == null)
                        ViewState["StartDate"] = StartDate;

                    DateTime datePager = Convert.ToDateTime(ViewState["StartDate"]);
                    List<ListItem> listItem = new List<ListItem>();
                    listItem.Add(new ListItem { Text = datePager.ToString("MM/yyyy"), Value = "1" });
                    for (int i = 1; i < pageList.Count() - 1; i++)
                    {
                        listItem.Add(new ListItem { Text = datePager.AddMonths(-i).ToString("MM/yyyy"), Value = (i + 1).ToString() });
                    }

                    ddlPager.DataSource = listItem;
                    ddlPager.DataTextField = "Text";
                    ddlPager.DataValueField = "Value";
                    ddlPager.DataBind();
                    if (pageList.Count() > 0)
                    {
                        ddlPager.SelectedValue = page.ToString();
                    }
                    if (count > page) count = page;

                    int endYear = startDate.Year;
                    int endMonth = startDate.Month;
                    if (endMonth == 12)
                    {
                        endMonth = 1;
                        endYear = endYear + 1;
                    }
                    else
                        endMonth += 1;
                    DateTime endDate = new DateTime(endYear, endMonth, 5);

                    txtStartDate.Text = startDate.ToString("dd/MM/yyyy");
                    txtEndDate.Text = endDate.ToString("dd/MM/yyyy");

                    listSum = listSum.Where(c => c.InOutDate >= startDate && c.InOutDate <= endDate).OrderByDescending(c => c.InOutDate).ToList();
                    rptInOut.DataSource = listSum;
                    rptInOut.DataBind();

                    decimal beginAmount = listSum.Select(c => c.BeginAmount).DefaultIfEmpty(0).Sum();
                    decimal contractFee = listSum.Select(c => c.ContractFee).DefaultIfEmpty(0).Sum();
                    decimal rentFee = listSum.Select(c => c.RentFee).DefaultIfEmpty(0).Sum();
                    decimal closedFee = listSum.Select(c => c.CloseFee).DefaultIfEmpty(0).Sum();
                    decimal redundantFee = listSum.Select(c => c.RedundantFee).DefaultIfEmpty(0).Sum();
                    decimal outOtherFee = listSum.Select(c => c.OutOther).DefaultIfEmpty(0).Sum();
                    decimal inOtherFee = listSum.Select(c => c.InOther).DefaultIfEmpty(0).Sum();
                    decimal outCapitalFee = listSum.Select(c => c.OutCapital).DefaultIfEmpty(0).Sum();
                    decimal inCapitalFee = listSum.Select(c => c.InCapital).DefaultIfEmpty(0).Sum();
                    decimal endAmount = listSum.Select(c => c.EndAmount).DefaultIfEmpty(0).Sum();

                    Label lblTotalBegin = (Label)rptInOut.Controls[rptInOut.Controls.Count - 1].Controls[0].FindControl("lblTotalBegin");
                    Label lblTotalContractFee = (Label)rptInOut.Controls[rptInOut.Controls.Count - 1].Controls[0].FindControl("lblTotalContractFee");
                    Label lblTotalRentFee = (Label)rptInOut.Controls[rptInOut.Controls.Count - 1].Controls[0].FindControl("lblTotalRentFee");
                    Label lblTotalClosedFee = (Label)rptInOut.Controls[rptInOut.Controls.Count - 1].Controls[0].FindControl("lblTotalClosedFee");
                    Label lblTotalRedundantFee = (Label)rptInOut.Controls[rptInOut.Controls.Count - 1].Controls[0].FindControl("lblTotalRedundantFee");
                    Label lblTotalOutOtherFee = (Label)rptInOut.Controls[rptInOut.Controls.Count - 1].Controls[0].FindControl("lblTotalOutOtherFee");
                    Label lblTotalInOtherFee = (Label)rptInOut.Controls[rptInOut.Controls.Count - 1].Controls[0].FindControl("lblTotalInOtherFee");
                    Label lblTotalOutCapital = (Label)rptInOut.Controls[rptInOut.Controls.Count - 1].Controls[0].FindControl("lblTotalOutCapital");
                    Label lblTotalInCapital = (Label)rptInOut.Controls[rptInOut.Controls.Count - 1].Controls[0].FindControl("lblTotalInCapital");
                    Label lblTotalEnd = (Label)rptInOut.Controls[rptInOut.Controls.Count - 1].Controls[0].FindControl("lblTotalEnd");

                    lblTotalContractFee.Text = contractFee == 0 ? "0" : string.Format("{0:0,0}", contractFee);
                    lblTotalRentFee.Text = rentFee == 0 ? "0" : string.Format("{0:0,0}", rentFee);
                    lblTotalClosedFee.Text = closedFee == 0 ? "0" : string.Format("{0:0,0}", closedFee);
                    lblTotalRedundantFee.Text = redundantFee == 0 ? "0" : string.Format("{0:0,0}", redundantFee);
                    lblTotalOutOtherFee.Text = outOtherFee == 0 ? "0" : string.Format("{0:0,0}", outOtherFee);
                    lblTotalInOtherFee.Text = inOtherFee == 0 ? "0" : string.Format("{0:0,0}", inOtherFee);
                    lblTotalOutCapital.Text = outCapitalFee == 0 ? "0" : string.Format("{0:0,0}", outCapitalFee);
                    lblTotalInCapital.Text = inCapitalFee == 0 ? "0" : string.Format("{0:0,0}", inCapitalFee);

                    lblTotalContractAmount.Text = contractFee == 0 ? "0" : string.Format("{0:0,0}", contractFee);
                    lblClosedAmount.Text = closedFee == 0 ? "0" : string.Format("{0:0,0}", closedFee);
                    lblResultAmount.Text = contractFee - closedFee == 0 ? "0" : string.Format("{0:0,0}", contractFee - closedFee);
                    lblTotalInAmount.Text = rentFee + inOtherFee == 0 ? "0" : string.Format("{0:0,0}", rentFee + inOtherFee);
                    lblTotalOutAmount.Text = redundantFee + outOtherFee == 0 ? "0" : string.Format("{0:0,0}", redundantFee + outOtherFee);
                    lblRevenue.Text = (rentFee + inOtherFee) - (redundantFee + outOtherFee) == 0 ? "0" : string.Format("{0:0,0}", (rentFee + inOtherFee) - (redundantFee + outOtherFee));

                    List<SummaryPayFeeDaily> dataListMonthly = CommonList.GetSummaryPayFeeDailyData(startDate, endDate, string.Empty, STORE_ID);
                    int[] monthlycontractIds = dataListMonthly.Select(c => c.CONTRACT_ID).ToArray();
                    decimal totalMonthlyFee = dataListMonthly.Where(c => monthlycontractIds.Contains(c.CONTRACT_ID)).Select(c => c.PAY_FEE).DefaultIfEmpty(0).Sum();

                    lblTotalInCapitalF.Text = inCapitalFee == 0 ? "0" : string.Format("{0:0,0}", inCapitalFee);
                    lblTotalOutCapitalF.Text = outCapitalFee == 0 ? "0" : string.Format("{0:0,0}", outCapitalFee);
                    lblTotalTheoryFee.Text = totalMonthlyFee == 0 ? "0" : string.Format("{0:0,0}", totalMonthlyFee);
                    lblTotalRealFee.Text = rentFee == 0 ? "0" : string.Format("{0:0,0}", rentFee);

                    List<CONTRACT_FULL_VW> dataSlowList = new List<CONTRACT_FULL_VW>();
                    List<CONTRACT_FULL_VW> dataDebtList = new List<CONTRACT_FULL_VW>();
                    var st = db.CONTRACT_FULL_VW.Where(c => c.CONTRACT_STATUS == true && c.ACTIVE == true);

                    if (STORE_ID != 0)
                    {
                        st = st.Where(c => c.STORE_ID == STORE_ID);
                    }
                    st = st.OrderByDescending(c => c.ID);

                    var lstPeriod = db.PayPeriods.Where(s => s.STATUS == true).ToList();
                    foreach (CONTRACT_FULL_VW c in st)
                    {
                        var inOutList = db.InOuts.Where(s => s.CONTRACT_ID == c.ID).ToList();

                        c.PAYED_TIME = 0;
                        c.PAY_DATE = c.RENT_DATE;
                        c.DAY_DONE = DateTime.Now.Subtract(c.PAY_DATE).Days;

                        var tmpLstPeriod = lstPeriod.Where(s => s.CONTRACT_ID == c.ID);
                        if (tmpLstPeriod != null)
                        {
                            decimal totalAmountPeriod = tmpLstPeriod.Where(s => s.PAY_DATE <= DateTime.Today).Select(s => s.AMOUNT_PER_PERIOD).DefaultIfEmpty(0).Sum();
                            decimal totalAmountPaid = tmpLstPeriod.Where(s => s.PAY_DATE <= DateTime.Today).Select(s => s.ACTUAL_PAY).DefaultIfEmpty(0).Sum();
                            c.AMOUNT_LEFT = totalAmountPeriod - totalAmountPaid <= 0 ? 0 : totalAmountPeriod - totalAmountPaid;

                            decimal paidAmount = tmpLstPeriod.Where(s => s.ACTUAL_PAY > 0).Select(s => s.ACTUAL_PAY).DefaultIfEmpty(0).Sum();
                            foreach (PayPeriod pp in tmpLstPeriod)
                            {
                                if (pp.AMOUNT_PER_PERIOD == 0)
                                {
                                    c.OVER_DATE = 0;
                                    break;
                                }
                                paidAmount -= pp.AMOUNT_PER_PERIOD;
                                if (paidAmount <= 0)
                                {
                                    if (paidAmount < 0)
                                    {
                                        c.OVER_DATE = DateTime.Today.Subtract(pp.PAY_DATE).Days;
                                    }
                                    else
                                    {
                                        if (tmpLstPeriod.Any(s => s.PAY_DATE == pp.PAY_DATE.AddDays(9)))
                                        {
                                            c.OVER_DATE = DateTime.Today.Subtract(pp.PAY_DATE.AddDays(9)).Days;
                                        }
                                        else
                                        {
                                            c.OVER_DATE = DateTime.Today.Subtract(pp.PAY_DATE.AddDays(10)).Days;
                                        }
                                    }
                                    c.PERIOD_ID = pp.ID;
                                    if (c.OVER_DATE >= 0)
                                    {
                                        if(c.OVER_DATE <= 50)
                                            dataSlowList.Add(c);

                                        if(c.OVER_DATE >= 11)
                                            dataDebtList.Add(c);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    decimal totalSlowFee = dataSlowList.Select(c => c.AMOUNT_LEFT).DefaultIfEmpty(0).Sum();
                    decimal totalDebtFee = dataDebtList.Select(c => c.AMOUNT_LEFT).DefaultIfEmpty(0).Sum();
                    lblTotalSlowFee.Text = totalSlowFee == 0 ? "0" : string.Format("{0:0,0}", totalSlowFee);
                    lblTotalDebtFee.Text = totalDebtFee == 0 ? "0" : string.Format("{0:0,0}", totalDebtFee);
                }
            }
        }

        private List<SummaryInfo> GetSummaryData(int STORE_ID)
        {
            using (var db = new RentBikeEntities())
            {
                var listInOut = db.INOUT_FULL_VW.Where(c => c.ACTIVE == true);
                if (STORE_ID != 0)
                {
                    listInOut = listInOut.Where(c => c.STORE_ID == STORE_ID);
                }
                var data = from d in listInOut
                           group d by d.INOUT_DATE into g
                           select new
                           {
                               Period = g.Key,
                               Record = from o in g
                                        select new
                                        {
                                            ID = o.STORE_ID,
                                            InOutDate = o.INOUT_DATE,
                                            RentTypeName = o.RENT_TYPE_NAME,
                                            CustomerId = o.CUSTOMER_ID,
                                            CustomerName = o.CUSTOMER_NAME,
                                            InAmount = o.IN_AMOUNT,
                                            OutAmount = o.OUT_AMOUNT,
                                            TotalIn = g.Sum(x => x.IN_AMOUNT),
                                            TotalOut = g.Sum(x => x.OUT_AMOUNT),
                                            BeginAmount = 0,
                                            EndAmount = 0,
                                            ContractFeeCar = 0,
                                            RentFeeCar = 0,
                                            CloseFeeCar = 0,
                                            ContractFeeEquip = 0,
                                            RentFeeEquip = 0,
                                            CloseFeeEquip = 0,
                                            ContractFeeOther = 0,
                                            RentFeeOther = 0,
                                            CloseFeeOther = 0,
                                            RemainEndOfDay = 0,
                                            InOutTypeId = o.INOUT_TYPE_ID,
                                            RentTypeId = o.RENT_TYPE_ID,
                                            InCapital = 0,
                                            OutCapital = 0,
                                            InOther = 0,
                                            OutOther = 0,
                                            IsDummy = o.IS_DUMMY

                                        }
                           };
                List<SummaryInfo> listSum = new List<SummaryInfo>();
                foreach (var g in data)
                {
                    SummaryInfo si = new SummaryInfo();
                    si.StoreId = g.Record.ToList()[0].ID;
                    si.InOutDate = g.Record.ToList()[0].InOutDate.Value;
                    si.RentTypeName = g.Record.ToList()[0].RentTypeName;
                    si.CustomerId = g.Record.ToList()[0].CustomerId;
                    si.CustomerName = g.Record.ToList()[0].CustomerName;
                    si.TotalIn = g.Record.ToList()[0].TotalIn;
                    si.TotalOut = g.Record.ToList()[0].TotalOut;
                    si.BeginAmount = 0;
                    si.EndAmount = g.Record.ToList()[0].TotalIn - g.Record.ToList()[0].TotalOut;
                    si.IsDummy = g.Record.ToList()[0].IsDummy;

                    var inout = g.Record.Where(x => x.InOutTypeId == 17);
                    if (inout.Any())
                    {
                        si.ContractFeeCar = inout.Sum(x => x.OutAmount);
                    }

                    inout = g.Record.Where(x => x.InOutTypeId == 22);
                    if (inout.Any())
                    {
                        si.ContractFeeEquip = inout.Sum(x => x.OutAmount);
                    }

                    inout = g.Record.Where(x => x.InOutTypeId == 23);
                    if (inout.Any())
                    {
                        si.ContractFeeOther = inout.Sum(x => x.OutAmount);
                    }

                    si.ContractFee = si.ContractFeeCar + si.ContractFeeEquip + si.ContractFeeOther;

                    inout = g.Record.Where(x => x.InOutTypeId == 14);
                    if (inout.Any())
                    {
                        si.RentFeeCar = inout.Sum(x => x.InAmount);
                    }

                    inout = g.Record.Where(x => x.InOutTypeId == 15);
                    if (inout.Any())
                    {
                        si.RentFeeEquip = inout.Sum(x => x.InAmount);
                    }

                    inout = g.Record.Where(x => x.InOutTypeId == 16);
                    if (inout.Any())
                    {
                        si.RentFeeOther = inout.Sum(x => x.InAmount);
                    }

                    si.RentFee = si.RentFeeCar + si.RentFeeEquip + si.RentFeeOther;

                    inout = g.Record.Where(x => x.InOutTypeId == 10);
                    if (inout.Any())
                    {
                        si.InCapital = inout.Sum(x => x.InAmount);
                    }

                    inout = g.Record.Where(x => x.InOutTypeId == 11);
                    if (inout.Any())
                    {
                        si.OutCapital = inout.Sum(x => x.OutAmount);
                    }

                    inout = g.Record.Where(x => x.InOutTypeId == 12);
                    if (inout.Any())
                    {
                        si.InOther = inout.Sum(x => x.InAmount);
                    }

                    inout = g.Record.Where(x => x.InOutTypeId == 13);
                    if (inout.Any())
                    {
                        si.OutOther = inout.Sum(x => x.OutAmount);
                    }

                    inout = g.Record.Where(x => x.InOutTypeId == 18 && x.RentTypeId == 1);
                    if (inout.Any())
                    {
                        si.CloseFeeCar = inout.Sum(x => x.InAmount);
                    }

                    inout = g.Record.Where(x => x.InOutTypeId == 18 && x.RentTypeId == 2);
                    if (inout.Any())
                    {
                        si.CloseFeeEquip = inout.Sum(x => x.InAmount);
                    }

                    inout = g.Record.Where(x => x.InOutTypeId == 18 && x.RentTypeId == 3);
                    if (inout.Any())
                    {
                        si.CloseFeeOther = inout.Sum(x => x.InAmount);
                    }

                    si.CloseFee = si.CloseFeeCar + si.CloseFeeEquip + si.CloseFeeOther;

                    inout = g.Record.Where(x => x.InOutTypeId == 19 && x.RentTypeId == 1);
                    if (inout.Any())
                    {
                        si.RedundantFeeCar = inout.Sum(x => x.OutAmount);
                    }

                    inout = g.Record.Where(x => x.InOutTypeId == 19 && x.RentTypeId == 2);
                    if (inout.Any())
                    {
                        si.RedundantFeeEquip = inout.Sum(x => x.OutAmount);
                    }

                    inout = g.Record.Where(x => x.InOutTypeId == 19 && x.RentTypeId == 3);
                    if (inout.Any())
                    {
                        si.RedundantFeeOther = inout.Sum(x => x.OutAmount);
                    }

                    si.RedundantFee = si.RedundantFeeCar + si.RedundantFeeEquip + si.RedundantFeeOther;

                    listSum.Add(si);
                }
                for (int i = 0; i < listSum.Count; i++)
                {
                    if (i > 0)
                    {
                        listSum[i].BeginAmount = listSum[i - 1].EndAmount;
                        listSum[i].EndAmount += listSum[i].BeginAmount;
                    }
                }
                return listSum;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData(1);
        }

        protected override void ddlStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData(1);
        }

        protected void ddlPager_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData(Helper.parseInt(ddlPager.SelectedValue));
        }

        private List<Contract> GetContractFeeByDay(DateTime date, RentBikeEntities db)
        {
            var data = from d in db.Contracts
                       where d.RENT_DATE == date
                       select d;
            return data.ToList();
        }

        private decimal GetRentFeeByDay(int rentType, DateTime date, RentBikeEntities db)
        {
            var data = from d in db.Contracts
                       where d.RENT_TYPE_ID == rentType
                       && d.RENT_DATE == date
                       select d;
            if (data.Any())
            {
                return data.Select(x => x.FEE_PER_DAY).DefaultIfEmpty(0).Sum();
            }
            return 0;
        }

        private List<CONTRACT_FULL_VW> GetMiddleContract(int STORE_ID, string startDate, string endDate)
        {
            using (var db = new RentBikeEntities())
            {
                var listContract = db.CONTRACT_FULL_VW.Where(c => c.CONTRACT_STATUS == true && c.ACTIVE == true).ToList();
                if (STORE_ID != 0)
                {
                    listContract = listContract.Where(c => c.STORE_ID == STORE_ID).ToList();
                }
                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    listContract = listContract.Where(c => c.CREATED_DATE >= Convert.ToDateTime(startDate) && c.CREATED_DATE <= Convert.ToDateTime(endDate)).ToList();
                }
                else if (!string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))
                {
                    listContract = listContract.Where(c => c.CREATED_DATE >= Convert.ToDateTime(startDate)).ToList();
                }
                else if (string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    listContract = listContract.Where(c => c.CREATED_DATE <= Convert.ToDateTime(endDate)).ToList();
                }

                return listContract;
            }
        }

        private List<INOUT_FULL_VW> GetMiddleInOut(int STORE_ID, string startDate, string endDate)
        {
            using (var db = new RentBikeEntities())
            {
                var listInOut = db.INOUT_FULL_VW.Where(c => c.ACTIVE == true);
                if (STORE_ID != 0)
                {
                    listInOut = listInOut.Where(c => c.STORE_ID == STORE_ID);
                }

                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    listInOut = listInOut.Where(c => c.INOUT_DATE >= Convert.ToDateTime(startDate) && c.INOUT_DATE <= Convert.ToDateTime(endDate));
                }
                else if (!string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))
                {
                    listInOut = listInOut.Where(c => c.INOUT_DATE >= Convert.ToDateTime(startDate));
                }
                else if (string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    listInOut = listInOut.Where(c => c.INOUT_DATE <= Convert.ToDateTime(endDate));
                }

                return listInOut.ToList();
            }
        }

        private List<Store> GetMiddleStore(int STORE_ID)
        {
            using (var db = new RentBikeEntities())
            {
                var listStore = (from c in db.Stores
                                 select c).ToList();
                if (STORE_ID != 0)
                {
                    listStore = listStore.Where(c => c.ID == STORE_ID).ToList();
                }
                return listStore.ToList();
            }
        }
    }
}