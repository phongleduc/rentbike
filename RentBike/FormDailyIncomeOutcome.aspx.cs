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
    public partial class FormDailyIncomeOutcome : System.Web.UI.Page
    {
        private DropDownList drpStore;
        private List<SummaryInfo> listSum;
        private List<INOUT_FULL_VW> listInOut;

        //raise button click events on content page for the buttons on master page
        protected void Page_Init(object sender, EventArgs e)
        {
            drpStore = this.Master.FindControl("ddlStore") as DropDownList;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["store_id"] == null)
            {
                Response.Redirect("FormLogin.aspx");
            }
            LoadData();
        }

        private void LoadData()
        {
            DropDownList drpStore = this.Master.FindControl("ddlStore") as DropDownList;
            int storeId = Helper.parseInt(drpStore.SelectedValue);
            using (var db = new RentBikeEntities())
            {
                if (CheckAdminPermission())
                {
                    listInOut = db.INOUT_FULL_VW.ToList();
                    if (storeId != 0)
                    {
                        listInOut = listInOut.Where(c => c.STORE_ID == storeId).ToList();
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
                                                OutOther = 0

                                            }
                               };
                    listSum = new List<SummaryInfo>();
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

                        //var inout = g.Record.Where(x => x.InOutTypeId == 17);
                        //if (inout.Any())
                        //{
                        //    si.ContractFeeCar = inout.Sum(x => x.OutAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 22);
                        //if (inout.Any())
                        //{
                        //    si.ContractFeeEquip = inout.Sum(x => x.OutAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 23);
                        //if (inout.Any())
                        //{
                        //    si.ContractFeeOther = inout.Sum(x => x.OutAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 14);
                        //if (inout.Any())
                        //{
                        //    si.RentFeeCar = inout.Sum(x => x.InAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 15);
                        //if (inout.Any())
                        //{
                        //    si.RentFeeEquip = inout.Sum(x => x.InAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 16);
                        //if (inout.Any())
                        //{
                        //    si.RentFeeOther = inout.Sum(x => x.InAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 10);
                        //if (inout.Any())
                        //{
                        //    si.InCapital = inout.Sum(x => x.InAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 11);
                        //if (inout.Any())
                        //{
                        //    si.OutCapital = inout.Sum(x => x.OutAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 12);
                        //if (inout.Any())
                        //{
                        //    si.InOther = inout.Sum(x => x.InAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 13);
                        //if (inout.Any())
                        //{
                        //    si.OutOther = inout.Sum(x => x.OutAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 18 && x.RentTypeId == 1);
                        //if (inout.Any())
                        //{
                        //    si.CloseFeeCar = inout.Sum(x => x.InAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 18 && x.RentTypeId == 2);
                        //if (inout.Any())
                        //{
                        //    si.CloseFeeEquip = inout.Sum(x => x.InAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 18 && x.RentTypeId == 3);
                        //if (inout.Any())
                        //{
                        //    si.CloseFeeOther = inout.Sum(x => x.InAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 19 && x.RentTypeId == 1);
                        //if (inout.Any())
                        //{
                        //    si.RedundantFeeCar = inout.Sum(x => x.OutAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 19 && x.RentTypeId == 2);
                        //if (inout.Any())
                        //{
                        //    si.RedundantFeeEquip = inout.Sum(x => x.OutAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 19 && x.RentTypeId == 3);
                        //if (inout.Any())
                        //{
                        //    si.RedundantFeeOther = inout.Sum(x => x.OutAmount);
                        //}

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


                    rptInOut.DataSource = listSum.OrderByDescending(c => c.InOutDate);
                    rptInOut.DataBind();
                    decimal sumIn = 0;
                    decimal sumOut = 0;
                    decimal sumBegin = 0;
                    decimal sumEnd = 0;

                    if (listSum.Any())
                    {
                        sumBegin = listSum[0].BeginAmount;
                        foreach (SummaryInfo itm in listSum)
                        {
                            sumIn += itm.TotalIn;
                            sumOut += itm.TotalOut;
                        }
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
                else
                {
                    int storeid = Convert.ToInt32(Session["store_id"]);
                    listInOut = db.INOUT_FULL_VW.ToList();
                    if (storeid != 0)
                    {
                        listInOut = listInOut.Where(c => c.STORE_ID == storeid).ToList();
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
                                                OutOther = 0

                                            }
                               };

                    listSum = new List<SummaryInfo>();
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

                        //var inout = g.Record.Where(x => x.InOutTypeId == 17);
                        //if (inout.Any())
                        //{
                        //    si.ContractFeeCar = inout.Sum(x => x.OutAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 22);
                        //if (inout.Any())
                        //{
                        //    si.ContractFeeEquip = inout.Sum(x => x.OutAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 23);
                        //if (inout.Any())
                        //{
                        //    si.ContractFeeOther = inout.Sum(x => x.OutAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 14);
                        //if (inout.Any())
                        //{
                        //    si.RentFeeCar = inout.Sum(x => x.InAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 15);
                        //if (inout.Any())
                        //{
                        //    si.RentFeeEquip = inout.Sum(x => x.InAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 16);
                        //if (inout.Any())
                        //{
                        //    si.RentFeeOther = inout.Sum(x => x.InAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 10);
                        //if (inout.Any())
                        //{
                        //    si.InCapital = inout.Sum(x => x.InAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 11);
                        //if (inout.Any())
                        //{
                        //    si.OutCapital = inout.Sum(x => x.OutAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 12);
                        //if (inout.Any())
                        //{
                        //    si.InOther = inout.Sum(x => x.InAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 13);
                        //if (inout.Any())
                        //{
                        //    si.OutOther = inout.Sum(x => x.OutAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 18 && x.RentTypeId == 1);
                        //if (inout.Any())
                        //{
                        //    si.CloseFeeCar = inout.Sum(x => x.InAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 18 && x.RentTypeId == 2);
                        //if (inout.Any())
                        //{
                        //    si.CloseFeeEquip = inout.Sum(x => x.InAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 18 && x.RentTypeId == 3);
                        //if (inout.Any())
                        //{
                        //    si.CloseFeeOther = inout.Sum(x => x.InAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 19 && x.RentTypeId == 1);
                        //if (inout.Any())
                        //{
                        //    si.RedundantFeeCar = inout.Sum(x => x.OutAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 19 && x.RentTypeId == 2);
                        //if (inout.Any())
                        //{
                        //    si.RedundantFeeEquip = inout.Sum(x => x.OutAmount);
                        //}

                        //inout = g.Record.Where(x => x.InOutTypeId == 19 && x.RentTypeId == 3);
                        //if (inout.Any())
                        //{
                        //    si.RedundantFeeOther = inout.Sum(x => x.OutAmount);
                        //}

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


                    rptInOut.DataSource = listSum.OrderByDescending(c => c.InOutDate);
                    rptInOut.DataBind();
                    decimal sumIn = 0;
                    decimal sumOut = 0;
                    decimal sumBegin = 0;
                    decimal sumEnd = 0;
                    if (listSum.Any())
                    {
                        sumBegin = listSum[0].BeginAmount;
                        foreach (SummaryInfo itm in listSum)
                        {
                            sumIn += itm.TotalIn;
                            sumOut += itm.TotalOut;
                        }
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
                return data.Sum(x => x.FEE_PER_DAY);
            }
            return 0;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {

        }

        public bool CheckAdminPermission()
        {
            string acc = Convert.ToString(Session["username"]);
            using (var db = new RentBikeEntities())
            {
                var item = db.Accounts.FirstOrDefault(s => s.ACC == acc);

                if (item.PERMISSION_ID == 1)
                    return true;
                return false;
            }
        }

        protected void rptInOut_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Repeater rptInOutDayDetail = (Repeater)e.Item.FindControl("rptInOutDayDetail");
                if (rptInOutDayDetail != null)
                {
                    SummaryInfo summaryInfo = e.Item.DataItem as SummaryInfo;
                    List<SummaryInfo> listSI = new List<SummaryInfo>();

                    List<INOUT_FULL_VW> listInOutTemp = listInOut.Where(c => c.INOUT_DATE == summaryInfo.InOutDate).ToList();
                    List<INOUT_FULL_VW> listInOutEquipAndCarAndOther = listInOutTemp.Where(c => c.RENT_TYPE_ID == 1 || c.RENT_TYPE_ID == 2 || c.RENT_TYPE_ID == 3).ToList();
                    List<INOUT_FULL_VW> listInOutOther = listInOutTemp.Where(c => c.INOUT_TYPE_ID == 10 || c.INOUT_TYPE_ID == 11 || c.INOUT_TYPE_ID == 12 || c.INOUT_TYPE_ID == 13).ToList();

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
                                                                        InOutDate = o.INOUT_DATE,
                                                                        RentTypeName = o.RENT_TYPE_NAME,
                                                                        InAmount = o.IN_AMOUNT,
                                                                        OutAmount = o.OUT_AMOUNT,
                                                                        TotalIn = c.Sum(x => x.IN_AMOUNT),
                                                                        TotalOut = c.Sum(x => x.OUT_AMOUNT),
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

                        si.ContractFeeCar = c.Record.Where(s => s.InOutTypeId == 17).Select(s => s.OutAmount).DefaultIfEmpty().Sum();
                        si.ContractFeeEquip = c.Record.Where(s => s.InOutTypeId == 22).Select(s => s.OutAmount).DefaultIfEmpty().Sum();
                        si.ContractFeeOther = c.Record.Where(s => s.InOutTypeId == 23).Select(s => s.OutAmount).DefaultIfEmpty().Sum();

                        si.RentFeeCar = c.Record.Where(s => s.InOutTypeId == 14).Select(s => s.InAmount).DefaultIfEmpty().Sum();
                        si.RentFeeEquip = c.Record.Where(s => s.InOutTypeId == 15).Select(s => s.InAmount).DefaultIfEmpty().Sum();
                        si.RentFeeOther = c.Record.Where(s => s.InOutTypeId == 16).Select(s => s.InAmount).DefaultIfEmpty().Sum();

                        si.CloseFeeCar = c.Record.Where(s => s.InOutTypeId == 18 && s.RentTypeId == 1).Select(s => s.InAmount).DefaultIfEmpty().Sum();
                        si.CloseFeeEquip = c.Record.Where(s => s.InOutTypeId == 18 && s.RentTypeId == 2).Select(s => s.InAmount).DefaultIfEmpty().Sum();
                        si.CloseFeeOther = c.Record.Where(s => s.InOutTypeId == 18 && s.RentTypeId == 3).Select(s => s.InAmount).DefaultIfEmpty().Sum();

                        si.RedundantFeeCar = c.Record.Where(s => s.InOutTypeId == 19 && s.RentTypeId == 1).Select(s => s.OutAmount).DefaultIfEmpty().Sum();
                        si.RedundantFeeEquip = c.Record.Where(s => s.InOutTypeId == 19 && s.RentTypeId == 2).Select(s => s.OutAmount).DefaultIfEmpty().Sum();
                        si.RedundantFeeOther = c.Record.Where(s => s.InOutTypeId == 19 && s.RentTypeId == 3).Select(s => s.OutAmount).DefaultIfEmpty().Sum();

                        listSI.Add(si);
                    }

                    foreach (var c in listInOutOther)
                    {
                        SummaryInfo si = new SummaryInfo();

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

                        decimal totalContractFeeEquip = listSI.Select(c => c.ContractFeeEquip).DefaultIfEmpty().Sum();
                        litTotalContractFeeEquip.Text = totalContractFeeEquip == 0 ? "0" : string.Format("{0:0,0}", totalContractFeeEquip);
                        decimal totalRentFeeEquip = listSI.Select(c => c.RentFeeEquip).DefaultIfEmpty().Sum();
                        litTotalRentFeeEquip.Text = totalRentFeeEquip == 0 ? "0" : string.Format("{0:0,0}", totalRentFeeEquip);
                        decimal totalCloseFeeEquip = listSI.Select(c => c.CloseFeeEquip).DefaultIfEmpty().Sum();
                        litTotalCloseFeeEquip.Text = totalCloseFeeEquip == 0 ? "0" : string.Format("{0:0,0}", totalCloseFeeEquip);
                        decimal totalRedundantFeeEquip = listSI.Select(c => c.RedundantFeeEquip).DefaultIfEmpty().Sum();
                        litTotalRedundantFeeEquip.Text = totalRedundantFeeEquip == 0 ? "0" : string.Format("{0:0,0}", totalRedundantFeeEquip);

                        decimal totalContractFeeCarAndOther = listSI.Select(c => c.ContractFeeCar).DefaultIfEmpty().Sum() + listSI.Select(c => c.ContractFeeOther).DefaultIfEmpty().Sum();
                        litTotalContractFeeCarAndOther.Text = totalContractFeeCarAndOther == 0 ? "0" : string.Format("{0:0,0}", totalContractFeeCarAndOther);
                        decimal totalRentFeeCarAndOther = listSI.Select(c => c.RentFeeCar).DefaultIfEmpty().Sum() + listSI.Select(c => c.RentFeeOther).DefaultIfEmpty().Sum();
                        litTotalRentFeeCarAndOther.Text = totalRentFeeCarAndOther == 0 ? "0" : string.Format("{0:0,0}", totalRentFeeCarAndOther);
                        decimal totalCloseFeeCarAndOther = listSI.Select(c => c.CloseFeeCar).DefaultIfEmpty().Sum() + listSI.Select(c => c.CloseFeeOther).DefaultIfEmpty().Sum();
                        litTotalCloseFeeCarAndOther.Text = totalCloseFeeCarAndOther == 0 ? "0" : string.Format("{0:0,0}", totalCloseFeeCarAndOther);
                        decimal totalRedundantFeeCarAndOther = listSI.Select(c => c.RedundantFeeCar).DefaultIfEmpty().Sum() + listSI.Select(c => c.RedundantFeeOther).DefaultIfEmpty().Sum();
                        litTotalRedundantFeeCarAndOther.Text = totalRedundantFeeCarAndOther == 0 ? "0" : string.Format("{0:0,0}", totalRedundantFeeCarAndOther);

                        decimal totalInOther = listSI.Select(c => c.InOther).DefaultIfEmpty().Sum();
                        litTotalInOther.Text = totalInOther == 0 ? "0" : string.Format("{0:0,0}", totalInOther);
                        decimal totalOutOther = listSI.Select(c => c.OutOther).DefaultIfEmpty().Sum();
                        litTotalOutOther.Text = totalOutOther == 0 ? "0" : string.Format("{0:0,0}", totalOutOther);
                        decimal totalInCapital = listSI.Select(c => c.InCapital).DefaultIfEmpty().Sum();
                        litTotalInCapital.Text = totalInCapital == 0 ? "0" : string.Format("{0:0,0}", totalInCapital);
                        decimal totalOutCapital = listSI.Select(c => c.OutCapital).DefaultIfEmpty().Sum();
                        litTotalOutCapital.Text = totalOutCapital == 0 ? "0" : string.Format("{0:0,0}", totalOutCapital);

                        decimal inTotal = (totalRentFeeEquip + totalCloseFeeEquip + totalRentFeeCarAndOther + totalCloseFeeCarAndOther + totalInOther + totalInCapital);
                        decimal outTotal = (totalContractFeeEquip + totalRedundantFeeEquip + totalContractFeeCarAndOther + totalRedundantFeeCarAndOther + totalOutOther + totalOutCapital);
                        decimal total = inTotal - outTotal;
                        litTotal.Text = total == 0 ? "0" : string.Format("{0:0,0}", total); ;
                    }
                }
            }
        }

        protected void rptInOutDayDetail_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var inout = e.Item.DataItem as SummaryInfo;

                Literal litNo = e.Item.FindControl("litNo") as Literal;
                Literal litCustomerName = e.Item.FindControl("litCustomerName") as Literal;

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

                litNo.Text = (e.Item.ItemIndex + 1).ToString();
                litCustomerName.Text = inout.CustomerName;

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
    }
}