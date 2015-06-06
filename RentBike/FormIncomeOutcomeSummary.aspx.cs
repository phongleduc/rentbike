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
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if(!IsPostBack)
            {
                LoadMiddle(txtStartDate.Text, txtEndDate.Text);
                LoadData(txtStartDate.Text, txtEndDate.Text);
            }
        }

        private void LoadData(string startDate, string endDate)
        {
            List<SummaryInfo> listSum = GetSummaryData(STORE_ID);
            if (listSum.Any())
            {
                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    listSum = listSum.Where(c =>c.InOutDate >= Convert.ToDateTime(startDate) && c.InOutDate <= Convert.ToDateTime(endDate)).ToList();
                }
                else if (!string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))
                {
                    listSum = listSum.Where(c =>c.InOutDate >= Convert.ToDateTime(startDate)).ToList();
                }
                else if (string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    listSum = listSum.Where(c =>c.InOutDate <= Convert.ToDateTime(endDate)).ToList();
                }

                rptInOut.DataSource = listSum.OrderByDescending(c =>c.InOutDate);
                rptInOut.DataBind();
                decimal sumIn = 0;
                decimal sumOut = 0;
                decimal sumBegin = 0;
                decimal sumEnd = 0;

                sumIn = listSum.Select(c =>c.TotalIn).DefaultIfEmpty(0).Sum();
                sumOut = listSum.Select(c =>c.TotalOut).DefaultIfEmpty(0).Sum();
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

        private List<SummaryInfo> GetSummaryData(int STORE_ID)
        {
            using (var db = new RentBikeEntities())
            {
                var lstInOut = db.INOUT_FULL_VW.Where(c =>c.ACTIVE == true);
                if (STORE_ID != 0)
                {
                    lstInOut = lstInOut.Where(c =>c.STORE_ID == STORE_ID);
                }
                var data = from d in lstInOut.ToList()
                           group d by d.INOUT_DATE into g
                           select new
                           {
                               Period = g.Key,
                               Record = from o in g
                                        select new
                                        {
                                            ID = o.STORE_ID,
                                            InOutDate = o.INOUT_DATE,
                                            InAmount = o.IN_AMOUNT,
                                            OutAmount = o.OUT_AMOUNT,
                                            TotalIn = g.Sum(x =>x.IN_AMOUNT),
                                            TotalOut = g.Sum(x =>x.OUT_AMOUNT),
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
                                            OutOther = 0

                                        }
                           };
                List<SummaryInfo> listSum = new List<SummaryInfo>();
                foreach (var g in data)
                {
                    SummaryInfo si = new SummaryInfo();
                    si.StoreId = g.Record.ToList()[0].ID;
                    si.InOutDate = g.Record.ToList()[0].InOutDate.Value;
                    si.TotalIn = g.Record.ToList()[0].TotalIn;
                    si.TotalOut = g.Record.ToList()[0].TotalOut;
                    si.BeginAmount = 0;
                    si.EndAmount = g.Record.ToList()[0].TotalIn - g.Record.ToList()[0].TotalOut;

                    var inout = g.Record.Where(x =>x.InOutTypeId == 17);
                    if (inout.Any())
                    {
                        si.ContractFeeCar = inout.Sum(x =>x.OutAmount);
                    }

                    inout = g.Record.Where(x =>x.InOutTypeId == 22);
                    if (inout.Any())
                    {
                        si.ContractFeeEquip = inout.Sum(x =>x.OutAmount);
                    }

                    inout = g.Record.Where(x =>x.InOutTypeId == 23);
                    if (inout.Any())
                    {
                        si.ContractFeeOther = inout.Sum(x =>x.OutAmount);
                    }

                    inout = g.Record.Where(x =>x.InOutTypeId == 14);
                    if (inout.Any())
                    {
                        si.RentFeeCar = inout.Sum(x =>x.InAmount);
                    }

                    inout = g.Record.Where(x =>x.InOutTypeId == 15);
                    if (inout.Any())
                    {
                        si.RentFeeEquip = inout.Sum(x =>x.InAmount);
                    }

                    inout = g.Record.Where(x =>x.InOutTypeId == 16);
                    if (inout.Any())
                    {
                        si.RentFeeOther = inout.Sum(x =>x.InAmount);
                    }

                    inout = g.Record.Where(x =>x.InOutTypeId == 10);
                    if (inout.Any())
                    {
                        si.InCapital = inout.Sum(x =>x.InAmount);
                    }

                    inout = g.Record.Where(x =>x.InOutTypeId == 11);
                    if (inout.Any())
                    {
                        si.OutCapital = inout.Sum(x =>x.OutAmount);
                    }

                    inout = g.Record.Where(x =>x.InOutTypeId == 12);
                    if (inout.Any())
                    {
                        si.InOther = inout.Sum(x =>x.InAmount);
                    }

                    inout = g.Record.Where(x =>x.InOutTypeId == 13);
                    if (inout.Any())
                    {
                        si.OutOther = inout.Sum(x =>x.OutAmount);
                    }

                    inout = g.Record.Where(x =>x.InOutTypeId == 18 && x.RentTypeId == 1);
                    if (inout.Any())
                    {
                        si.CloseFeeCar = inout.Sum(x =>x.InAmount);
                    }

                    inout = g.Record.Where(x =>x.InOutTypeId == 18 && x.RentTypeId == 2);
                    if (inout.Any())
                    {
                        si.CloseFeeEquip = inout.Sum(x =>x.InAmount);
                    }

                    inout = g.Record.Where(x =>x.InOutTypeId == 18 && x.RentTypeId == 3);
                    if (inout.Any())
                    {
                        si.CloseFeeOther = inout.Sum(x =>x.InAmount);
                    }

                    inout = g.Record.Where(x =>x.InOutTypeId == 19 && x.RentTypeId == 1);
                    if (inout.Any())
                    {
                        si.RedundantFeeCar = inout.Sum(x =>x.OutAmount);
                    }

                    inout = g.Record.Where(x =>x.InOutTypeId == 19 && x.RentTypeId == 2);
                    if (inout.Any())
                    {
                        si.RedundantFeeEquip = inout.Sum(x =>x.OutAmount);
                    }

                    inout = g.Record.Where(x =>x.InOutTypeId == 19 && x.RentTypeId == 3);
                    if (inout.Any())
                    {
                        si.RedundantFeeOther = inout.Sum(x =>x.OutAmount);
                    }

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
            LoadData(txtStartDate.Text, txtEndDate.Text);
        }

        protected override void ddlStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData(txtStartDate.Text, txtEndDate.Text);
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
                return data.Select(x =>x.FEE_PER_DAY).DefaultIfEmpty(0).Sum();
            }
            return 0;
        }


        private void LoadMiddle(string startDate, string endDate)
        {
            using (var db = new RentBikeEntities())
            {
                List<CONTRACT_FULL_VW> listContract = GetMiddleContract(STORE_ID, startDate, endDate);

                decimal bikeAmount = listContract.Where(c =>c.RENT_TYPE_ID == 1).Select(c =>c.CONTRACT_AMOUNT).DefaultIfEmpty(0).Sum();
                decimal equipAmount = listContract.Where(c =>c.RENT_TYPE_ID == 2).Select(c =>c.CONTRACT_AMOUNT).DefaultIfEmpty(0).Sum();
                decimal otherAmount = listContract.Where(c =>c.RENT_TYPE_ID == 3).Select(c =>c.CONTRACT_AMOUNT).DefaultIfEmpty(0).Sum();

                lblRentBikeAmount.Text = bikeAmount == 0 ? "0" : string.Format("{0:0,0}", bikeAmount);
                lblRentEquipAmount.Text = equipAmount == 0 ? "0" : string.Format("{0:0,0}", equipAmount);
                lblRentOtherAmount.Text = otherAmount == 0 ? "0" : string.Format("{0:0,0}", otherAmount);
                lblRentAll.Text = bikeAmount + equipAmount + otherAmount == 0 ? "0" : string.Format("{0:0,0}", (bikeAmount + equipAmount + otherAmount));


                //============================================================
                List<INOUT_FULL_VW> listInOut = GetMiddleInOut(STORE_ID, startDate, endDate);

                decimal totalIn = listInOut.Select(c =>c.IN_AMOUNT).DefaultIfEmpty(0).Sum();
                decimal totalOut = listInOut.Select(c =>c.OUT_AMOUNT).DefaultIfEmpty(0).Sum(); ;

                lblSumAllIn.Text = totalIn == 0 ? "0" : string.Format("{0:0,0}", totalIn);
                lblSumAllOut.Text = totalOut == 0 ? "0" : string.Format("{0:0,0}", totalOut);

                List<Store> listStore = GetMiddleStore(STORE_ID);
                decimal totalCapital = listStore.Select(c =>c.START_CAPITAL).DefaultIfEmpty(0).Sum();
                lblTotalInvest.Text = totalCapital == 0 ? "0" : string.Format("{0:0,0}", totalCapital);
            }
        }

        private List<CONTRACT_FULL_VW> GetMiddleContract(int STORE_ID, string startDate, string endDate)
        {
            using (var db = new RentBikeEntities())
            {
                var listContract = db.CONTRACT_FULL_VW.Where(c =>c.CONTRACT_STATUS == true && c.ACTIVE == true).ToList();
                if (STORE_ID != 0)
                {
                    listContract = listContract.Where(c =>c.STORE_ID == STORE_ID).ToList();
                }
                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    listContract = listContract.Where(c =>c.CREATED_DATE >= Convert.ToDateTime(startDate) && c.CREATED_DATE <= Convert.ToDateTime(endDate)).ToList();
                }
                else if (!string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))
                {
                    listContract = listContract.Where(c =>c.CREATED_DATE >= Convert.ToDateTime(startDate)).ToList();
                }
                else if (string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    listContract = listContract.Where(c =>c.CREATED_DATE <= Convert.ToDateTime(endDate)).ToList();
                }

                return listContract;
            }
        }

        private List<INOUT_FULL_VW> GetMiddleInOut(int STORE_ID, string startDate, string endDate)
        {
            using (var db = new RentBikeEntities())
            {
                var listInOut = db.INOUT_FULL_VW.Where(c =>c.ACTIVE == true);
                if (STORE_ID != 0)
                {
                    listInOut = listInOut.Where(c =>c.STORE_ID == STORE_ID);
                }

                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    listInOut = listInOut.Where(c =>c.INOUT_DATE >= Convert.ToDateTime(startDate) && c.INOUT_DATE <= Convert.ToDateTime(endDate));
                }
                else if (!string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))
                {
                    listInOut = listInOut.Where(c =>c.INOUT_DATE >= Convert.ToDateTime(startDate));
                }
                else if (string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    listInOut = listInOut.Where(c =>c.INOUT_DATE <= Convert.ToDateTime(endDate));
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
                    listStore = listStore.Where(c =>c.ID == STORE_ID).ToList();
                }
                return listStore.ToList();
            } 
        }
    }
}