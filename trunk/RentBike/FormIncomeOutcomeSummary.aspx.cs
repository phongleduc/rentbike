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
    public partial class FormIncomeOutcomeSummary : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["store_id"] == null)
            {
                Response.Redirect("FormLogin.aspx");
            }
            LoadMiddle();
            LoadData();
        }

        private void LoadData()
        {
            int storeId = 0;
            if (CheckAdminPermission())
            {
                DropDownList drpStore = this.Master.FindControl("ddlStore") as DropDownList;
                storeId = Helper.parseInt(drpStore.SelectedValue);
            }
            else
            {
                storeId = Convert.ToInt32(Session["store_id"]);
            }

            List<SummaryInfo> listSum = GetSummaryData(storeId);
            if (listSum.Any())
            {
                if (!string.IsNullOrEmpty(txtStartDate.Text) && !string.IsNullOrEmpty(txtEndDate.Text))
                {
                    listSum = listSum.Where(c =>c.InOutDate >= Convert.ToDateTime(txtStartDate.Text) && c.InOutDate <= Convert.ToDateTime(txtEndDate.Text)).ToList();
                }
                else if (!string.IsNullOrEmpty(txtStartDate.Text) && string.IsNullOrEmpty(txtEndDate.Text))
                {
                    listSum = listSum.Where(c =>c.InOutDate >= Convert.ToDateTime(txtStartDate.Text)).ToList();
                }
                else if (string.IsNullOrEmpty(txtStartDate.Text) && !string.IsNullOrEmpty(txtEndDate.Text))
                {
                    listSum = listSum.Where(c =>c.InOutDate <= Convert.ToDateTime(txtEndDate.Text)).ToList();
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

        private List<SummaryInfo> GetSummaryData(int storeId)
        {
            using (var db = new RentBikeEntities())
            {
                List<InOut> lstInOut = db.InOuts.ToList();
                if (storeId != 0)
                {
                    lstInOut = lstInOut.Where(c =>c.STORE_ID == storeId).ToList();
                }
                var data = from d in lstInOut
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

        private List<Contract> GetContractFeeByDay(DateTime date, RentBikeEntities db)
        {
            var data = from d in db.Contracts
                       where EntityFunctions.TruncateTime(d.RENT_DATE) == EntityFunctions.TruncateTime(date)
                       select d;
            return data.ToList();
        }

        private decimal GetRentFeeByDay(int rentType, DateTime date, RentBikeEntities db)
        {
            var data = from d in db.Contracts
                       where d.RENT_TYPE_ID == rentType
                       && EntityFunctions.TruncateTime(d.RENT_DATE) == EntityFunctions.TruncateTime(date)
                       select d;
            if (data.Any())
            {
                return data.Sum(x =>x.FEE_PER_DAY);
            }
            return 0;
        }


        private void LoadMiddle()
        {
            using (var db = new RentBikeEntities())
            {
                int storeId = 0;
                if (CheckAdminPermission())
                {
                    DropDownList drpStore = this.Master.FindControl("ddlStore") as DropDownList;
                    storeId = Helper.parseInt(drpStore.SelectedValue);
                }
                else
                {
                    storeId = Convert.ToInt32(Session["store_id"]);
                }

                List<CONTRACT_FULL_VW> listContract = GetMiddleContract(storeId);

                decimal bikeAmount = listContract.Where(c =>c.RENT_TYPE_ID == 1).Select(c =>c.CONTRACT_AMOUNT).DefaultIfEmpty(0).Sum();
                decimal equipAmount = listContract.Where(c =>c.RENT_TYPE_ID == 2).Select(c =>c.CONTRACT_AMOUNT).DefaultIfEmpty(0).Sum();
                decimal otherAmount = listContract.Where(c =>c.RENT_TYPE_ID == 3).Select(c =>c.CONTRACT_AMOUNT).DefaultIfEmpty(0).Sum();

                lblRentBikeAmount.Text = bikeAmount == 0 ? "0" : string.Format("{0:0,0}", bikeAmount);
                lblRentEquipAmount.Text = equipAmount == 0 ? "0" : string.Format("{0:0,0}", equipAmount);
                lblRentOtherAmount.Text = otherAmount == 0 ? "0" : string.Format("{0:0,0}", otherAmount);
                lblRentAll.Text = bikeAmount + equipAmount + otherAmount == 0 ? "0" : string.Format("{0:0,0}", (bikeAmount + equipAmount + otherAmount));


                //============================================================
                List<InOut> listInOut = GetMiddleInOut(storeId);

                decimal totalIn = listInOut.Select(c =>c.IN_AMOUNT).DefaultIfEmpty(0).Sum();
                decimal totalOut = listInOut.Select(c =>c.OUT_AMOUNT).DefaultIfEmpty(0).Sum(); ;

                lblSumAllIn.Text = totalIn == 0 ? "0" : string.Format("{0:0,0}", totalIn);
                lblSumAllOut.Text = totalOut == 0 ? "0" : string.Format("{0:0,0}", totalOut);

                List<Store> listStore = GetMiddleStore(storeId);
                decimal totalCapital = listStore.Select(c =>c.START_CAPITAL).DefaultIfEmpty(0).Sum();
                lblTotalInvest.Text = totalCapital == 0 ? "0" : string.Format("{0:0,0}", totalCapital);
            }
        }

        private List<CONTRACT_FULL_VW> GetMiddleContract(int storeId)
        {
            using (var db = new RentBikeEntities())
            {
                var listContract = db.CONTRACT_FULL_VW.Where(c =>c.CONTRACT_STATUS == true).ToList();
                if (storeId != 0)
                {
                    listContract = listContract.Where(c =>c.STORE_ID == storeId).ToList();
                }
                if (!string.IsNullOrEmpty(txtStartDate.Text) && !string.IsNullOrEmpty(txtEndDate.Text))
                {
                    listContract = listContract.Where(c =>c.CREATED_DATE >= Convert.ToDateTime(txtStartDate.Text) && c.CREATED_DATE <= Convert.ToDateTime(txtEndDate.Text)).ToList();
                }
                else if (!string.IsNullOrEmpty(txtStartDate.Text) && string.IsNullOrEmpty(txtEndDate.Text))
                {
                    listContract = listContract.Where(c =>c.CREATED_DATE >= Convert.ToDateTime(txtStartDate.Text)).ToList();
                }
                else if (string.IsNullOrEmpty(txtStartDate.Text) && !string.IsNullOrEmpty(txtEndDate.Text))
                {
                    listContract = listContract.Where(c =>c.CREATED_DATE <= Convert.ToDateTime(txtEndDate.Text)).ToList();
                }

                return listContract;
            }
        }

        private List<InOut> GetMiddleInOut(int storeId)
        {
            using (var db = new RentBikeEntities())
            {
                var listInOut = (from inout in db.InOuts
                                select inout).ToList();
                if (storeId != 0)
                {
                    listInOut = listInOut.Where(c =>c.STORE_ID == storeId).ToList();
                }

                if (!string.IsNullOrEmpty(txtStartDate.Text) && !string.IsNullOrEmpty(txtEndDate.Text))
                {
                    listInOut = listInOut.Where(c =>c.INOUT_DATE >= Convert.ToDateTime(txtStartDate.Text) && c.INOUT_DATE <= Convert.ToDateTime(txtEndDate.Text)).ToList();
                }
                else if (!string.IsNullOrEmpty(txtStartDate.Text) && string.IsNullOrEmpty(txtEndDate.Text))
                {
                    listInOut = listInOut.Where(c =>c.INOUT_DATE >= Convert.ToDateTime(txtStartDate.Text)).ToList();
                }
                else if (string.IsNullOrEmpty(txtStartDate.Text) && !string.IsNullOrEmpty(txtEndDate.Text))
                {
                    listInOut = listInOut.Where(c =>c.INOUT_DATE <= Convert.ToDateTime(txtEndDate.Text)).ToList();
                }

                return listInOut;
            }
        }

        private List<Store> GetMiddleStore(int storeId)
        {
            using (var db = new RentBikeEntities())
            {
                var listStore = (from c in db.Stores
                                 select c).ToList();
                if (storeId != 0)
                {
                    listStore = listStore.Where(c =>c.ID == storeId).ToList();
                }
                return listStore.ToList();
            } 
        }
        public bool CheckAdminPermission()
        {
            string acc = Convert.ToString(Session["username"]);
            using (var db = new RentBikeEntities())
            {
                var item = db.Accounts.FirstOrDefault(s =>s.ACC == acc);

                if (item.PERMISSION_ID == 1)
                    return true;
                return false;
            }
        }
    }
}