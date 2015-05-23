using OfficeOpenXml;
using OfficeOpenXml.Style;
using RentBike.Common;
using RentBike.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace RentBike
{
    public partial class FormDailyIncomeOutcome : FormBase
    {
        int pageSize = 10;
        private List<INOUT_FULL_VW> listInOut;

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                LoadData(string.Empty, string.Empty, 0);
            }
        }

        private void LoadData(string startDate, string endDate, int page)
        {
            int totalRecord = 0;
            List<SummaryInfo> listSum = GetSummaryData(STORE_ID);
            if (listSum.Any())
            {
                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    listSum = listSum.Where(c => c.InOutDate >= Convert.ToDateTime(startDate) && c.InOutDate <= Convert.ToDateTime(endDate)).ToList();
                }
                else if (!string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))
                {
                    listSum = listSum.Where(c =>c.InOutDate >= Convert.ToDateTime(startDate)).ToList();
                }
                else if (string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    listSum = listSum.Where(c =>c.InOutDate <= Convert.ToDateTime(endDate)).ToList();
                }

                listSum = listSum.OrderByDescending(c => c.InOutDate).ToList();
                totalRecord = listSum.Count();

                int skip = page * pageSize;
                listSum = listSum.Skip(skip).Take(pageSize).ToList();
                int totalPage = totalRecord % pageSize == 0 ? totalRecord / pageSize : totalRecord / pageSize + 1;
                List<int> pageList = new List<int>();
                for (int i = 1; i <= totalPage; i++)
                {
                    pageList.Add(i);
                }

                ddlPager.DataSource = pageList;
                ddlPager.DataBind();
                if (pageList.Count > 0)
                {
                    ddlPager.SelectedIndex = page;
                }

                rptInOut.DataSource = listSum;
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
                listInOut = db.INOUT_FULL_VW.ToList();
                if (STORE_ID != 0)
                {
                    listInOut = listInOut.Where(c =>c.STORE_ID == STORE_ID).ToList();
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
                    si.RentTypeName = g.Record.ToList()[0].RentTypeName;
                    si.CustomerId = g.Record.ToList()[0].CustomerId;
                    si.CustomerName = g.Record.ToList()[0].CustomerName;
                    si.TotalIn = g.Record.ToList()[0].TotalIn;
                    si.TotalOut = g.Record.ToList()[0].TotalOut;
                    si.BeginAmount = 0;
                    si.EndAmount = g.Record.ToList()[0].TotalIn - g.Record.ToList()[0].TotalOut;

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

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData(txtStartDate.Text, txtEndDate.Text, 0);
        }

        protected void rptInOut_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Repeater rptInOutDayDetail = (Repeater)e.Item.FindControl("rptInOutDayDetail");
                if (rptInOutDayDetail != null)
                {
                    SummaryInfo summaryInfo = e.Item.DataItem as SummaryInfo;
                    List<SummaryInfo> listSI = GetDailyData(summaryInfo.InOutDate);
                    if (listSI.Any())
                    {
                        rptInOutDayDetail.DataSource = listSI;
                        rptInOutDayDetail.DataBind();


                        Literal litInoutDate = rptInOutDayDetail.Controls[0].Controls[0].FindControl("litInoutDate") as Literal;
                        litInoutDate.Text = listSI[0].InOutDate.ToString("dd/MM/yyyy");


                        Literal litTotalContractFeeEquip = rptInOutDayDetail.Controls[rptInOutDayDetail.Controls.Count - 1].Controls[0].FindControl("litTotalContractFeeEquip") as Literal;
                        Literal litTotalRentFeeEquip = rptInOutDayDetail.Controls[rptInOutDayDetail.Controls.Count - 1].Controls[0].FindControl("litTotalRentFeeEquip") as Literal;
                        Literal litTotalCloseFeeEquip = rptInOutDayDetail.Controls[rptInOutDayDetail.Controls.Count - 1].Controls[0].FindControl("litTotalCloseFeeEquip") as Literal;
                        Literal litTotalRedundantFeeEquip = rptInOutDayDetail.Controls[rptInOutDayDetail.Controls.Count - 1].Controls[0].FindControl("litTotalRedundantFeeEquip") as Literal;

                        Literal litTotalContractFeeCarAndOther = rptInOutDayDetail.Controls[rptInOutDayDetail.Controls.Count - 1].Controls[0].FindControl("litTotalContractFeeCarAndOther") as Literal;
                        Literal litTotalRentFeeCarAndOther = rptInOutDayDetail.Controls[rptInOutDayDetail.Controls.Count - 1].Controls[0].FindControl("litTotalRentFeeCarAndOther") as Literal;
                        Literal litTotalCloseFeeCarAndOther = rptInOutDayDetail.Controls[rptInOutDayDetail.Controls.Count - 1].Controls[0].FindControl("litTotalCloseFeeCarAndOther") as Literal;
                        Literal litTotalRedundantFeeCarAndOther = rptInOutDayDetail.Controls[rptInOutDayDetail.Controls.Count - 1].Controls[0].FindControl("litTotalRedundantFeeCarAndOther") as Literal;

                        Literal litTotalInOther = rptInOutDayDetail.Controls[rptInOutDayDetail.Controls.Count - 1].Controls[0].FindControl("litTotalInOther") as Literal;
                        Literal litTotalOutOther = rptInOutDayDetail.Controls[rptInOutDayDetail.Controls.Count - 1].Controls[0].FindControl("litTotalOutOther") as Literal;
                        Literal litTotalInCapital = rptInOutDayDetail.Controls[rptInOutDayDetail.Controls.Count - 1].Controls[0].FindControl("litTotalInCapital") as Literal;
                        Literal litTotalOutCapital = rptInOutDayDetail.Controls[rptInOutDayDetail.Controls.Count - 1].Controls[0].FindControl("litTotalOutCapital") as Literal;

                        Literal litTotal = rptInOutDayDetail.Controls[rptInOutDayDetail.Controls.Count - 1].Controls[0].FindControl("litTotal") as Literal;
                        //LinkButton lnkExportExcel = rptInOutDayDetail.Controls[rptInOutDayDetail.Controls.Count - 1].Controls[0].FindControl("lnkExportExcel") as LinkButton;

                        decimal totalContractFeeEquip = listSI.Select(c =>c.ContractFeeEquip).DefaultIfEmpty(0).Sum();
                        litTotalContractFeeEquip.Text = totalContractFeeEquip == 0 ? "0" : string.Format("{0:0,0}", totalContractFeeEquip);
                        decimal totalRentFeeEquip = listSI.Select(c =>c.RentFeeEquip).DefaultIfEmpty(0).Sum();
                        litTotalRentFeeEquip.Text = totalRentFeeEquip == 0 ? "0" : string.Format("{0:0,0}", totalRentFeeEquip);
                        decimal totalCloseFeeEquip = listSI.Select(c =>c.CloseFeeEquip).DefaultIfEmpty(0).Sum();
                        litTotalCloseFeeEquip.Text = totalCloseFeeEquip == 0 ? "0" : string.Format("{0:0,0}", totalCloseFeeEquip);
                        decimal totalRedundantFeeEquip = listSI.Select(c =>c.RedundantFeeEquip).DefaultIfEmpty(0).Sum();
                        litTotalRedundantFeeEquip.Text = totalRedundantFeeEquip == 0 ? "0" : string.Format("{0:0,0}", totalRedundantFeeEquip);

                        decimal totalContractFeeCarAndOther = listSI.Select(c =>c.ContractFeeCar).DefaultIfEmpty(0).Sum() + listSI.Select(c =>c.ContractFeeOther).DefaultIfEmpty(0).Sum();
                        litTotalContractFeeCarAndOther.Text = totalContractFeeCarAndOther == 0 ? "0" : string.Format("{0:0,0}", totalContractFeeCarAndOther);
                        decimal totalRentFeeCarAndOther = listSI.Select(c =>c.RentFeeCar).DefaultIfEmpty(0).Sum() + listSI.Select(c =>c.RentFeeOther).DefaultIfEmpty(0).Sum();
                        litTotalRentFeeCarAndOther.Text = totalRentFeeCarAndOther == 0 ? "0" : string.Format("{0:0,0}", totalRentFeeCarAndOther);
                        decimal totalCloseFeeCarAndOther = listSI.Select(c =>c.CloseFeeCar).DefaultIfEmpty(0).Sum() + listSI.Select(c =>c.CloseFeeOther).DefaultIfEmpty(0).Sum();
                        litTotalCloseFeeCarAndOther.Text = totalCloseFeeCarAndOther == 0 ? "0" : string.Format("{0:0,0}", totalCloseFeeCarAndOther);
                        decimal totalRedundantFeeCarAndOther = listSI.Select(c =>c.RedundantFeeCar).DefaultIfEmpty(0).Sum() + listSI.Select(c =>c.RedundantFeeOther).DefaultIfEmpty(0).Sum();
                        litTotalRedundantFeeCarAndOther.Text = totalRedundantFeeCarAndOther == 0 ? "0" : string.Format("{0:0,0}", totalRedundantFeeCarAndOther);

                        decimal totalInOther = listSI.Select(c =>c.InOther).DefaultIfEmpty(0).Sum();
                        litTotalInOther.Text = totalInOther == 0 ? "0" : string.Format("{0:0,0}", totalInOther);
                        decimal totalOutOther = listSI.Select(c =>c.OutOther).DefaultIfEmpty(0).Sum();
                        litTotalOutOther.Text = totalOutOther == 0 ? "0" : string.Format("{0:0,0}", totalOutOther);
                        decimal totalInCapital = listSI.Select(c =>c.InCapital).DefaultIfEmpty(0).Sum();
                        litTotalInCapital.Text = totalInCapital == 0 ? "0" : string.Format("{0:0,0}", totalInCapital);
                        decimal totalOutCapital = listSI.Select(c =>c.OutCapital).DefaultIfEmpty(0).Sum();
                        litTotalOutCapital.Text = totalOutCapital == 0 ? "0" : string.Format("{0:0,0}", totalOutCapital);

                        decimal inTotal = (totalRentFeeEquip + totalCloseFeeEquip + totalRentFeeCarAndOther + totalCloseFeeCarAndOther + totalInOther + totalInCapital);
                        decimal outTotal = (totalContractFeeEquip + totalRedundantFeeEquip + totalContractFeeCarAndOther + totalRedundantFeeCarAndOther + totalOutOther + totalOutCapital);
                        decimal total = inTotal - outTotal;
                        litTotal.Text = total == 0 ? "0" : string.Format("{0:0,0}", total);
                        //lnkExportExcel.CommandArgument = listSI[0].InOutDate.ToString();
                    }
                }
            }
        }

        private List<SummaryInfo> GetDailyData(DateTime inoutDate)
        {
            List<SummaryInfo> listSI = new List<SummaryInfo>();

            List<INOUT_FULL_VW> listInOutTemp = listInOut.Where(c =>c.INOUT_DATE == inoutDate).ToList();
            List<INOUT_FULL_VW> listInOutEquipAndCarAndOther = listInOutTemp.Where(c =>c.RENT_TYPE_ID == 1 || c.RENT_TYPE_ID == 2 || c.RENT_TYPE_ID == 3).ToList();
            List<INOUT_FULL_VW> listInOutOther = listInOutTemp.Where(c =>c.INOUT_TYPE_ID == 10 || c.INOUT_TYPE_ID == 11 || c.INOUT_TYPE_ID == 12 || c.INOUT_TYPE_ID == 13).ToList();

            var inoutEquipAndCarAndOther = from d in listInOutEquipAndCarAndOther
                                           group d by new { d.CUSTOMER_ID, d.CUSTOMER_NAME }
                                               into c
                                               select new
                                               {
                                                   CustomerId = c.Key.CUSTOMER_ID,
                                                   CustomerName = c.Key.CUSTOMER_NAME,
                                                   Record = from o in c
                                                            select new
                                                            {
                                                                ID = o.ID,
                                                                ContractId = o.CONTRACT_ID,
                                                                Period = o.PERIOD_DATE,
                                                                InOutDate = o.INOUT_DATE,
                                                                RentTypeName = o.RENT_TYPE_NAME,
                                                                InAmount = o.IN_AMOUNT,
                                                                OutAmount = o.OUT_AMOUNT,
                                                                TotalIn = c.Sum(x =>x.IN_AMOUNT),
                                                                TotalOut = c.Sum(x =>x.OUT_AMOUNT),
                                                                InOutTypeId = o.INOUT_TYPE_ID,
                                                                RentTypeId = o.RENT_TYPE_ID,
                                                                InCapital = 0,
                                                                OutCapital = 0,
                                                                InOther = 0,
                                                                OutOther = 0
                                                            }
                                               };

            foreach (var c in inoutEquipAndCarAndOther)
            {
                SummaryInfo si = new SummaryInfo();
                si.InOutDate = c.Record.ToList()[0].InOutDate.Value;
                si.TotalIn = c.Record.ToList()[0].TotalIn;
                si.TotalOut = c.Record.ToList()[0].TotalOut;
                si.BeginAmount = 0;
                si.EndAmount = c.Record.ToList()[0].TotalIn - c.Record.ToList()[0].TotalOut;
                si.CustomerName = c.CustomerName;

                si.ContractFeeCar = c.Record.Where(s =>s.InOutTypeId == 17).Select(s =>s.OutAmount).DefaultIfEmpty(0).Sum();
                si.ContractFeeEquip = c.Record.Where(s =>s.InOutTypeId == 22).Select(s =>s.OutAmount).DefaultIfEmpty(0).Sum();
                si.ContractFeeOther = c.Record.Where(s =>s.InOutTypeId == 23).Select(s =>s.OutAmount).DefaultIfEmpty(0).Sum();

                si.RentFeeCar = c.Record.Where(s =>s.InOutTypeId == 14).Select(s =>s.InAmount).DefaultIfEmpty(0).Sum();
                si.RentFeeEquip = c.Record.Where(s =>s.InOutTypeId == 15).Select(s =>s.InAmount).DefaultIfEmpty(0).Sum();
                si.RentFeeOther = c.Record.Where(s =>s.InOutTypeId == 16).Select(s =>s.InAmount).DefaultIfEmpty(0).Sum();

                si.CloseFeeCar = c.Record.Where(s =>s.InOutTypeId == 18 && s.RentTypeId == 1).Select(s =>s.InAmount).DefaultIfEmpty(0).Sum();
                si.CloseFeeEquip = c.Record.Where(s =>s.InOutTypeId == 18 && s.RentTypeId == 2).Select(s =>s.InAmount).DefaultIfEmpty(0).Sum();
                si.CloseFeeOther = c.Record.Where(s =>s.InOutTypeId == 18 && s.RentTypeId == 3).Select(s =>s.InAmount).DefaultIfEmpty(0).Sum();

                si.RedundantFeeCar = c.Record.Where(s =>s.InOutTypeId == 19 && s.RentTypeId == 1).Select(s =>s.OutAmount).DefaultIfEmpty(0).Sum();
                si.RedundantFeeEquip = c.Record.Where(s =>s.InOutTypeId == 19 && s.RentTypeId == 2).Select(s =>s.OutAmount).DefaultIfEmpty(0).Sum();
                si.RedundantFeeOther = c.Record.Where(s =>s.InOutTypeId == 19 && s.RentTypeId == 3).Select(s =>s.OutAmount).DefaultIfEmpty(0).Sum();

                if (si.RentFeeEquip > 0 || si.RentFeeCar > 0 || si.RentFeeOther > 0)
                {
                    si.ListPeriodDate = c.Record.Where(s => s.InOutTypeId == 14 || s.InOutTypeId == 15 || s.InOutTypeId == 16).Select(s => s.Period).ToList();
                }

                listSI.Add(si);
            }

            foreach (var c in listInOutOther)
            {
                SummaryInfo si = new SummaryInfo();
                si.InOutId = c.ID;
                si.CssClass = "background-yellow";

                switch (c.INOUT_TYPE_ID)
                {
                    case 10:
                        si.InCapital = c.IN_AMOUNT;
                        si.CustomerName = "Nhập Vốn";
                        break;
                    case 11:
                        si.OutCapital = c.OUT_AMOUNT;
                        si.CustomerName = "Xuất Vốn";
                        break;
                    case 12:
                        si.InOther = c.IN_AMOUNT;
                        si.CustomerName = "Thu Khác";
                        break;
                    case 13:
                        si.OutOther = c.OUT_AMOUNT;
                        si.CustomerName = "Chi Khác";
                        break;
                    default:
                        break;

                }
                if (!string.IsNullOrEmpty(c.MORE_INFO))
                    si.CustomerName = c.MORE_INFO;

                listSI.Add(si);
            }
            return listSI;
        }

        protected void rptInOutDayDetail_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var inout = e.Item.DataItem as SummaryInfo;

                HtmlTableRow trItem = e.Item.FindControl("trItem") as HtmlTableRow;

                Literal litNo = e.Item.FindControl("litNo") as Literal;
                Literal litCustomerName = e.Item.FindControl("litCustomerName") as Literal;
                Literal litPeriod = e.Item.FindControl("litPeriod") as Literal;

                Literal litContractFeeEquip = e.Item.FindControl("litContractFeeEquip") as Literal;
                Literal litRentFeeEquip = e.Item.FindControl("litRentFeeEquip") as Literal;
                Literal litClosedFeeEquip = e.Item.FindControl("litClosedFeeEquip") as Literal;
                Literal litRedundantFeeEquip = e.Item.FindControl("litRedundantFeeEquip") as Literal;

                Literal litContractFeeCarAndOther = e.Item.FindControl("litContractFeeCarAndOther") as Literal;
                Literal litRentFeeCarAndOther = e.Item.FindControl("litRentFeeCarAndOther") as Literal;
                Literal litClosedFeeCarAndOther = e.Item.FindControl("litClosedFeeCarAndOther") as Literal;
                Literal litRedundantFeeCarAndOther = e.Item.FindControl("litRedundantFeeCarAndOther") as Literal;

                Literal litInOther = e.Item.FindControl("litInOther") as Literal;
                Literal litOutOther = e.Item.FindControl("litOutOther") as Literal;
                Literal litInCapital = e.Item.FindControl("litInCapital") as Literal;
                Literal litOutCapital = e.Item.FindControl("litOutCapital") as Literal;

                trItem.Attributes.Add("class", inout.CssClass);

                litNo.Text = (e.Item.ItemIndex + 1).ToString();
                if (inout.InOutId > 0)
                {
                    using (StringWriter stringWriter = new StringWriter())
                    {
                        // Put HtmlTextWriter in using block because it needs to call Dispose.
                        using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Href, "FormDailyIncomeOutcomeUpdate.aspx?id=" + inout.InOutId);
                            writer.RenderBeginTag(HtmlTextWriterTag.A);
                            writer.Write(inout.CustomerName);
                            writer.RenderEndTag();
                            litCustomerName.Text = stringWriter.ToString();
                        }
                    }
                }
                else
                    litCustomerName.Text = inout.CustomerName;
                if (inout.ListPeriodDate != null && inout.ListPeriodDate.Any())
                    litPeriod.Text = string.Join("<br/>", inout.ListPeriodDate.Select(c => c.ToString("dd/MM/yyyy")).Distinct());

                litContractFeeEquip.Text = inout.ContractFeeEquip == 0 ? string.Empty : string.Format("{0:0,0}", inout.ContractFeeEquip);
                litRentFeeEquip.Text = inout.RentFeeEquip == 0 ? string.Empty : string.Format("{0:0,0}", inout.RentFeeEquip);
                litClosedFeeEquip.Text = inout.CloseFeeEquip == 0 ? string.Empty : string.Format("{0:0,0}", inout.CloseFeeEquip);
                litRedundantFeeEquip.Text = inout.RedundantFeeEquip == 0 ? string.Empty : string.Format("{0:0,0}", inout.RedundantFeeEquip);

                litContractFeeCarAndOther.Text = (inout.ContractFeeCar + inout.ContractFeeOther) == 0 ? string.Empty : string.Format("{0:0,0}", (inout.ContractFeeCar + inout.ContractFeeOther));
                litRentFeeCarAndOther.Text = (inout.RentFeeCar + inout.RentFeeOther) == 0 ? string.Empty : string.Format("{0:0,0}", (inout.RentFeeCar + inout.RentFeeOther));
                litClosedFeeCarAndOther.Text = (inout.CloseFeeCar + inout.CloseFeeOther) == 0 ? string.Empty : string.Format("{0:0,0}", (inout.CloseFeeCar + inout.CloseFeeOther));
                litRedundantFeeCarAndOther.Text = (inout.RedundantFeeCar + inout.RedundantFeeOther) == 0 ? string.Empty : string.Format("{0:0,0}", (inout.RedundantFeeCar + inout.RedundantFeeOther));

                litInCapital.Text = inout.InCapital == 0 ? string.Empty : string.Format("{0:0,0}", inout.InCapital);
                litOutCapital.Text = inout.OutCapital == 0 ? string.Empty : string.Format("{0:0,0}", inout.OutCapital);
                litInOther.Text = inout.InOther == 0 ? string.Empty : string.Format("{0:0,0}", inout.InOther);
                litOutOther.Text = inout.OutOther == 0 ? string.Empty : string.Format("{0:0,0}", inout.OutOther);
            }
        }

        protected new void ddlStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData(txtStartDate.Text, txtEndDate.Text, 0);
        }

        protected void ddlPager_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData(txtStartDate.Text, txtEndDate.Text, Convert.ToInt32(ddlPager.SelectedValue) - 1);
        }

        protected void lnkExportExcel_Click(object sender, EventArgs e)
        {
            LinkButton lnkExportExcel = sender as LinkButton;
            if (string.IsNullOrEmpty(lnkExportExcel.CommandArgument)) return;

            DateTime date = Convert.ToDateTime(lnkExportExcel.CommandArgument);
            List<SummaryInfo> listSI = GetDailyData(date);

            using (ExcelPackage package = new ExcelPackage())
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("CTHN");
                worksheet.View.ZoomScale = 90;
                worksheet.Cells.Style.Font.Size = 12;
                worksheet.Cells.Style.Font.Name = "Times New Roman";

                worksheet.Cells[1, 1, 2, 16].Style.Font.Bold = true;
                worksheet.Cells[1, 1, 2, 16].Style.Font.Size = 14;
                worksheet.Cells[1, 1, 2, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells[1, 1, 2, 1].Merge = true;
                worksheet.Cells[1, 1, 2, 1].Value = "NGÀY";
                worksheet.Column(1).Width = 15;

                worksheet.Cells[1, 2, 2, 2].Merge = true;
                worksheet.Cells[1, 2, 2, 2].Value = "STT";
                worksheet.Column(2).Width = 8;

                worksheet.Cells[1, 3, 2, 3].Merge = true;
                worksheet.Cells[1, 3, 2, 3].Value = "KHÁCH HÀNG";
                worksheet.Column(3).Width = 35;
                worksheet.Column(3).Style.WrapText = true;

                worksheet.Cells[1, 4, 1, 7].Merge = true;
                worksheet.Cells[1, 4, 1, 7].Value = "THIẾT BỊ VĂN PHÒNG";
                worksheet.Cells[2, 4, 2, 4].Value = "THUÊ";
                worksheet.Cells[2, 5, 2, 5].Value = "THU PHÍ";
                worksheet.Cells[2, 6, 2, 6].Value = "THANH LÝ";
                worksheet.Cells[2, 7, 2, 7].Value = "THỪA PHÍ";
                worksheet.Column(4).Width = 15;
                worksheet.Column(5).Width = 15;
                worksheet.Column(6).Width = 15;
                worksheet.Column(7).Width = 15;

                worksheet.Cells[1, 8, 1, 11].Merge = true;
                worksheet.Cells[1, 8, 1, 11].Value = "GIẤY TỜ XE & KHÁC";
                worksheet.Cells[2, 8, 2, 8].Value = "THUÊ";
                worksheet.Cells[2, 9, 2, 9].Value = "THU PHÍ";
                worksheet.Cells[2, 10, 2, 10].Value = "THANH LÝ";
                worksheet.Cells[2, 11, 2, 11].Value = "THỪA PHÍ";
                worksheet.Column(8).Width = 15;
                worksheet.Column(9).Width = 15;
                worksheet.Column(10).Width = 15;
                worksheet.Column(11).Width = 15;

                worksheet.Cells[1, 12, 2, 12].Merge = true;
                worksheet.Cells[1, 12, 2, 12].Value = "CHI KHÁC";
                worksheet.Column(12).Width = 15;

                worksheet.Cells[1, 13, 2, 13].Merge = true;
                worksheet.Cells[1, 13, 2, 13].Value = "THU KHÁC";
                worksheet.Column(13).Width = 15;

                worksheet.Cells[1, 14, 2, 14].Merge = true;
                worksheet.Cells[1, 14, 2, 14].Value = "TIỀN XUẤT";
                worksheet.Column(14).Width = 15;

                worksheet.Cells[1, 15, 2, 15].Merge = true;
                worksheet.Cells[1, 15, 2, 15].Value = "TIỀN NHẬP";
                worksheet.Column(15).Width = 15;

                worksheet.Cells[1, 16, 2, 16].Merge = true;
                worksheet.Cells[1, 16, 2, 16].Value = "GHI CHÚ";
                worksheet.Column(16).Width = 15;

                int no = 1;
                int index = 3;
                foreach (var si in listSI)
                {
                    worksheet.Cells[index, 2].Value = no;
                    worksheet.Cells[index, 3].Value = si.CustomerName;

                    worksheet.Cells[index, 4].Value = si.ContractFeeEquip == 0 ? string.Empty : string.Format("{0:0,0}", si.ContractFeeEquip);
                    worksheet.Cells[index, 5].Value = si.RentFeeEquip == 0 ? string.Empty : string.Format("{0:0,0}", si.RentFeeEquip);
                    worksheet.Cells[index, 6].Value = si.CloseFeeEquip == 0 ? string.Empty : string.Format("{0:0,0}", si.CloseFeeEquip);
                    worksheet.Cells[index, 7].Value = si.RedundantFeeEquip == 0 ? string.Empty : string.Format("{0:0,0}", si.RedundantFeeEquip);

                    worksheet.Cells[index, 8].Value = (si.ContractFeeCar + si.ContractFeeOther) == 0 ? string.Empty : string.Format("{0:0,0}", (si.ContractFeeCar + si.ContractFeeOther));
                    worksheet.Cells[index, 9].Value = (si.RentFeeCar + si.RentFeeOther) == 0 ? string.Empty : string.Format("{0:0,0}", (si.RentFeeCar + si.RentFeeOther));
                    worksheet.Cells[index, 10].Value = (si.CloseFeeCar + si.CloseFeeOther) == 0 ? string.Empty : string.Format("{0:0,0}", (si.CloseFeeCar + si.CloseFeeOther));
                    worksheet.Cells[index, 11].Value = (si.RedundantFeeCar + si.RedundantFeeOther) == 0 ? string.Empty : string.Format("{0:0,0}", (si.RedundantFeeCar + si.RedundantFeeOther));

                    worksheet.Cells[index, 12].Value = si.OutOther == 0 ? string.Empty : string.Format("{0:0,0}", si.OutOther);
                    worksheet.Cells[index, 13].Value = si.InOther == 0 ? string.Empty : string.Format("{0:0,0}", si.InOther);
                    worksheet.Cells[index, 14].Value = si.OutCapital == 0 ? string.Empty : string.Format("{0:0,0}", si.OutCapital);
                    worksheet.Cells[index, 15].Value = si.InCapital == 0 ? string.Empty : string.Format("{0:0,0}", si.InCapital);

                    no += 1;
                    index += 1;
                }

                decimal totalContractFeeEquip = listSI.Select(c =>c.ContractFeeEquip).DefaultIfEmpty(0).Sum();
                worksheet.Cells[index + 1, 4].Value = totalContractFeeEquip == 0 ? "0" : string.Format("{0:0,0}", totalContractFeeEquip);
                decimal totalRentFeeEquip = listSI.Select(c =>c.RentFeeEquip).DefaultIfEmpty(0).Sum();
                worksheet.Cells[index + 1, 5].Value = totalRentFeeEquip == 0 ? "0" : string.Format("{0:0,0}", totalRentFeeEquip);
                decimal totalCloseFeeEquip = listSI.Select(c =>c.CloseFeeEquip).DefaultIfEmpty(0).Sum();
                worksheet.Cells[index + 1, 6].Value = totalCloseFeeEquip == 0 ? "0" : string.Format("{0:0,0}", totalCloseFeeEquip);
                decimal totalRedundantFeeEquip = listSI.Select(c =>c.RedundantFeeEquip).DefaultIfEmpty(0).Sum();
                worksheet.Cells[index + 1, 7].Value = totalRedundantFeeEquip == 0 ? "0" : string.Format("{0:0,0}", totalRedundantFeeEquip);

                decimal totalContractFeeCarAndOther = listSI.Select(c =>c.ContractFeeCar).DefaultIfEmpty(0).Sum() + listSI.Select(c =>c.ContractFeeOther).DefaultIfEmpty(0).Sum();
                worksheet.Cells[index + 1, 8].Value = totalContractFeeCarAndOther == 0 ? "0" : string.Format("{0:0,0}", totalContractFeeCarAndOther);
                decimal totalRentFeeCarAndOther = listSI.Select(c =>c.RentFeeCar).DefaultIfEmpty(0).Sum() + listSI.Select(c =>c.RentFeeOther).DefaultIfEmpty(0).Sum();
                worksheet.Cells[index + 1, 9].Value = totalRentFeeCarAndOther == 0 ? "0" : string.Format("{0:0,0}", totalRentFeeCarAndOther);
                decimal totalCloseFeeCarAndOther = listSI.Select(c =>c.CloseFeeCar).DefaultIfEmpty(0).Sum() + listSI.Select(c =>c.CloseFeeOther).DefaultIfEmpty(0).Sum();
                worksheet.Cells[index + 1, 10].Value = totalCloseFeeCarAndOther == 0 ? "0" : string.Format("{0:0,0}", totalCloseFeeCarAndOther);
                decimal totalRedundantFeeCarAndOther = listSI.Select(c =>c.RedundantFeeCar).DefaultIfEmpty(0).Sum() + listSI.Select(c =>c.RedundantFeeOther).DefaultIfEmpty(0).Sum();
                worksheet.Cells[index + 1, 11].Value = totalRedundantFeeCarAndOther == 0 ? "0" : string.Format("{0:0,0}", totalRedundantFeeCarAndOther);

                decimal totalOutOther = listSI.Select(c =>c.OutOther).DefaultIfEmpty(0).Sum();
                worksheet.Cells[index + 1, 12].Value = totalOutOther == 0 ? "0" : string.Format("{0:0,0}", totalOutOther);
                decimal totalInOther = listSI.Select(c =>c.InOther).DefaultIfEmpty(0).Sum();
                worksheet.Cells[index + 1, 13].Value = totalInOther == 0 ? "0" : string.Format("{0:0,0}", totalInOther);
                decimal totalOutCapital = listSI.Select(c =>c.OutCapital).DefaultIfEmpty(0).Sum();
                worksheet.Cells[index + 1, 14].Value = totalOutCapital == 0 ? "0" : string.Format("{0:0,0}", totalOutCapital);
                decimal totalInCapital = listSI.Select(c =>c.InCapital).DefaultIfEmpty(0).Sum();
                worksheet.Cells[index + 1, 15].Value = totalInCapital == 0 ? "0" : string.Format("{0:0,0}", totalInCapital);

                decimal inTotal = (totalRentFeeEquip + totalCloseFeeEquip + totalRentFeeCarAndOther + totalCloseFeeCarAndOther + totalInOther + totalInCapital);
                decimal outTotal = (totalContractFeeEquip + totalRedundantFeeEquip + totalContractFeeCarAndOther + totalRedundantFeeCarAndOther + totalOutOther + totalOutCapital);
                decimal total = inTotal - outTotal;
                worksheet.Cells[index + 1, 16].Value = total == 0 ? "0" : string.Format("{0:0,0}", total);
                worksheet.Row(index + 1).Style.Font.Bold = true;
                Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#92D050");
                worksheet.Cells[index + 1, 1, index + 1, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[index + 1, 1, index + 1, 16].Style.Fill.BackgroundColor.SetColor(colFromHex);
                worksheet.Cells[index + 1, 16, index + 1, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[index + 1, 16, index + 1, 16].Style.Font.Color.SetColor(Color.Red);

                worksheet.Cells[3, 1, index, 1].Merge = true;
                worksheet.Cells[3, 1, index, 1].Value = date.ToString("dd/MM/yyyy");
                worksheet.Cells[3, 1, index, 1].Style.Font.Bold = true;
                worksheet.Cells[3, 1, index, 1].Style.Font.Size = 16;
                worksheet.Cells[3, 1, index, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[1, 1, index + 1, 16].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[1, 1, index + 1, 16].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[1, 1, index + 1, 16].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[1, 1, index + 1, 16].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();

                if ((Request.Browser.Browser.ToLower() == "ie") && (Request.Browser.MajorVersion < 9))
                {
                    Response.Cache.SetCacheability(System.Web.HttpCacheability.Private);
                    Response.Cache.SetMaxAge(TimeSpan.FromMilliseconds(1));
                }
                else
                {
                    Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);//IE set to not cache
                    Response.Cache.SetNoStore();//Firefox/Chrome not to cache
                    Response.Cache.SetExpires(DateTime.UtcNow); //for safe measure expire it immediately
                }

                string fileName = string.Format("CTHN {0}.{1}", date.ToString("dd-MM-yyyy"), ".xlsx");
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=\"" + fileName + "\"");
                Response.BinaryWrite(package.GetAsByteArray());
                Response.Flush();
                Response.Close();
                Response.End();
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("FormDailyIncomeOutcomeUpdate.aspx");
        }
    }
}