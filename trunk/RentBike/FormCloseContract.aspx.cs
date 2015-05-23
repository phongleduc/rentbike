﻿using RentBike.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RentBike
{
    public partial class FormCloseContract : FormBase
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!IsPostBack)
            {
                int id = Convert.ToInt32(Request.QueryString["ID"]);
                using (var db = new RentBikeEntities())
                {
                    Contract con = db.Contracts.FirstOrDefault(c =>c.ID == id);

                    txtEndDate.Text = string.Format("{0:dd/MM/yyyy}", con.END_DATE);
                    txtAmount.Text = string.Format("{0:0,0}", con.CONTRACT_AMOUNT);
                    txtOverDate.Text = DateTime.Now.Date.Subtract(con.END_DATE).TotalDays <= 0 ? "0" : DateTime.Now.Date.Subtract(con.END_DATE).TotalDays.ToString();

                    List<PayPeriod> lstPayperiod = db.PayPeriods.Where(c =>c.CONTRACT_ID == id).ToList();
                    decimal paidAmount = lstPayperiod.Where(c =>c.ACTUAL_PAY > 0).Select(c =>c.ACTUAL_PAY).DefaultIfEmpty(0).Sum();
                    decimal total = 0;

                    for (DateTime date = con.RENT_DATE; date <= DateTime.Now; date = date.AddDays(1))
                    {
                        foreach (PayPeriod pay in lstPayperiod)
                        {
                            if (date <= pay.PAY_DATE.AddDays(10))
                            {
                                total += pay.AMOUNT_PER_PERIOD / 10;
                                break;
                            }
                        }
                    }
                    if (total - paidAmount < 0)
                    {
                        txtRealIncome.Text = "0";
                        txtReduceAmount.Text = string.Format("{0:0,0}", paidAmount - total);
                    }
                    else
                    {
                        txtRealIncome.Text = string.Format("{0:0,0}", total - paidAmount);
                        txtReduceAmount.Text = "0";
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                int contractId = Convert.ToInt32(Request.QueryString["ID"]);
                using (var db = new RentBikeEntities())
                {
                    var con = db.CONTRACT_FULL_VW.FirstOrDefault(c =>c.ID == contractId);
                    string closedContractName = string.Empty;
                    switch (con.RENT_TYPE_ID)
                    {
                        case 1:
                            closedContractName = "Thanh lý thuê xe";
                            break;
                        case 2:
                            closedContractName = "Thanh lý thuê thiết bị";
                            break;
                        default:
                            closedContractName = "Thanh lý thuê khác";
                            break;
                    }
                    // INOUT --> IN amount
                    InOut io1 = new InOut();
                    io1.IN_AMOUNT = con.CONTRACT_AMOUNT;
                    io1.OUT_AMOUNT = 0;
                    io1.CONTRACT_ID = con.ID;
                    io1.PERIOD_ID = -1;
                    io1.PERIOD_DATE = new DateTime(1, 1, 1);
                    io1.RENT_TYPE_ID = con.RENT_TYPE_ID;

                    var item = db.InOutTypes.FirstOrDefault(s =>s.NAME == "Thanh lý");
                    io1.INOUT_TYPE_ID = item.ID;
                    io1.MORE_INFO = txtMoreInfo.Text.Trim();
                    io1.STORE_ID = con.STORE_ID;
                    io1.SEARCH_TEXT = string.Format("{0} {1} {2} {3} {4}", con.CONTRACT_NO, con.CUSTOMER_NAME, con.STORE_NAME, closedContractName, txtMoreInfo.Text.Trim());
                    io1.INOUT_DATE = DateTime.Now;
                    io1.CREATED_BY = Session["username"].ToString();
                    io1.CREATED_DATE = DateTime.Now;
                    io1.UPDATED_BY = Session["username"].ToString();
                    io1.UPDATED_DATE = DateTime.Now;
                    db.InOuts.Add(io1);

                    // IN --> Rent Fee
                    if (!string.IsNullOrEmpty(txtRealIncome.Text))
                    {
                        decimal realInAmount = Convert.ToDecimal(txtRealIncome.Text.Replace(",", string.Empty));
                        if (realInAmount > 0)
                        {
                            InOut io2 = new InOut();

                            string feeName = string.Empty;
                            switch (con.RENT_TYPE_NAME)
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
                            item = db.InOutTypes.First(s =>s.NAME == feeName);

                            io2.INOUT_TYPE_ID = item.ID;
                            io2.RENT_TYPE_ID = con.RENT_TYPE_ID;
                            io2.CONTRACT_ID = con.ID;
                            io2.IN_AMOUNT = realInAmount;
                            io2.OUT_AMOUNT = 0;
                            io2.MORE_INFO = txtMoreInfo.Text;
                            io2.PERIOD_DATE = DateTime.Now;
                            io2.STORE_ID = con.STORE_ID;
                            io2.SEARCH_TEXT = string.Format("{0} {1} {2} {3} {4}", con.CONTRACT_NO, con.CUSTOMER_NAME, con.STORE_NAME, feeName, txtMoreInfo.Text.Trim());
                            io2.INOUT_DATE = DateTime.Now;
                            io2.CREATED_BY = Session["username"].ToString();
                            io2.CREATED_DATE = DateTime.Now;
                            io2.UPDATED_BY = Session["username"].ToString();
                            io2.UPDATED_DATE = DateTime.Now;
                            db.InOuts.Add(io2);
                        }
                    }


                    //Out --> Return redundant fee if the client comes before deadline date.
                    if (!string.IsNullOrEmpty(txtReduceAmount.Text))
                    {
                        decimal reduceAmount = Convert.ToDecimal(txtReduceAmount.Text.Replace(",", string.Empty));
                        if (reduceAmount > 0)
                        {
                            InOut io3 = new InOut();
                            item = db.InOutTypes.FirstOrDefault(s =>s.NAME == "Trả lại phí thừa");

                            io3.INOUT_TYPE_ID = item.ID;
                            io3.RENT_TYPE_ID = con.RENT_TYPE_ID;
                            io3.CONTRACT_ID = con.ID;
                            io3.IN_AMOUNT = 0;
                            io3.OUT_AMOUNT = reduceAmount;
                            io3.RENT_TYPE_ID = con.RENT_TYPE_ID;
                            io3.MORE_INFO = txtMoreInfo.Text.Trim();
                            io3.PERIOD_DATE = DateTime.Now;
                            io3.STORE_ID = con.STORE_ID;
                            io3.SEARCH_TEXT = string.Format("{0} {1} {2} {3} {4}", con.CONTRACT_NO, con.CUSTOMER_NAME, con.STORE_NAME, "Trả lại phí thừa", txtMoreInfo.Text.Trim());
                            io3.INOUT_DATE = DateTime.Now;
                            io3.CREATED_BY = Session["username"].ToString();
                            io3.CREATED_DATE = DateTime.Now;
                            io3.UPDATED_BY = Session["username"].ToString();
                            io3.UPDATED_DATE = DateTime.Now;

                            db.InOuts.Add(io3);
                        }
                    }
                    db.SaveChanges();
                }
                // Writelog
                WriteLog(Constants.ACTION_CLOSE_CONTRACT, false);

                using (var db = new RentBikeEntities())
                {
                    // Update status contract
                    var con = db.Contracts.FirstOrDefault(c =>c.ID == contractId);
                    con.CONTRACT_STATUS = false;
                    con.CLOSE_CONTRACT_DATE = DateTime.Now;

                    // Insert History row
                    ContractHistory ch = new ContractHistory();
                    ch.CONTRACT_ID = con.ID;
                    ch.CONTRACT_NO = con.CONTRACT_NO;
                    ch.CUSTOMER_ID = con.CUSTOMER_ID;
                    ch.CONTRACT_AMOUNT = con.CONTRACT_AMOUNT;
                    ch.DETAIL = con.DETAIL;
                    ch.RENT_DATE = con.RENT_DATE;
                    ch.END_DATE = con.END_DATE;
                    ch.FEE_PER_DAY = con.FEE_PER_DAY;
                    ch.ITEM_LICENSE_NO = con.ITEM_LICENSE_NO;
                    ch.ITEM_TYPE = con.ITEM_TYPE;
                    ch.NOTE = con.NOTE;
                    ch.REFERENCE_NAME = con.REFERENCE_NAME;
                    ch.RENT_TYPE_ID = con.RENT_TYPE_ID;
                    ch.SERIAL_1 = con.SERIAL_1;
                    ch.SERIAL_2 = con.SERIAL_2;
                    ch.STORE_ID = con.STORE_ID;
                    ch.SEARCH_TEXT = con.SEARCH_TEXT;
                    ch.PAY_FEE_MESSAGE = string.Empty;
                    ch.CLOSE_CONTRACT_DATE = DateTime.Now;
                    ch.REFERENCE_PHONE = con.REFERENCE_PHONE;
                    ch.SCHOOL_NAME = con.SCHOOL_NAME;
                    ch.CLASS_NAME = con.CLASS_NAME;
                    ch.IMPLEMENTER = con.IMPLEMENTER;
                    ch.BACK_TO_DOCUMENTS = con.BACK_TO_DOCUMENTS;
                    ch.PHOTO_1 = con.PHOTO_1;
                    ch.THUMBNAIL_PHOTO_1 = con.THUMBNAIL_PHOTO_1;
                    ch.PHOTO_2 = con.PHOTO_2;
                    ch.THUMBNAIL_PHOTO_2 = con.THUMBNAIL_PHOTO_2;
                    ch.PHOTO_3 = con.PHOTO_3;
                    ch.THUMBNAIL_PHOTO_3 = con.THUMBNAIL_PHOTO_3;
                    ch.PHOTO_4 = con.PHOTO_4;
                    ch.THUMBNAIL_PHOTO_4 = con.THUMBNAIL_PHOTO_4;
                    ch.PHOTO_5 = con.PHOTO_5;
                    ch.THUMBNAIL_PHOTO_5 = con.THUMBNAIL_PHOTO_5;
                    ch.CREATED_BY = Session["username"].ToString();
                    ch.CREATED_DATE = DateTime.Now;
                    ch.UPDATED_BY = Session["username"].ToString();
                    ch.UPDATED_DATE = DateTime.Now;

                    if (CommonList.IsBadContract(db, contractId))
                    {
                        con.IS_BAD_CONTRACT = ch.IS_BAD_CONTRACT = true;
                    }

                    db.ContractHistories.Add(ch);
                    db.SaveChanges();
                    trans.Complete();
                }
            }
            Response.Redirect("FormContractManagement.aspx");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string id = Request.QueryString["ID"];
            Response.Redirect(string.Format("FormContractUpdate.aspx?ID={0}", id));
        }

        private void WriteLog(string action, bool isCrashed)
        {
            Log lg = new Log();
            lg.ACCOUNT = Session["username"].ToString();
            string strStoreName = string.Empty;
            if (IS_ADMIN)
            {
                DropDownList drpStore = this.Master.FindControl("ddlStore") as DropDownList;
                strStoreName = drpStore.SelectedItem.Text;
            }
            else
                strStoreName = Session["store_name"].ToString();

            lg.STORE = strStoreName;
            lg.LOG_ACTION = action;
            lg.LOG_DATE = DateTime.Now;
            lg.IS_CRASH = isCrashed;
            lg.LOG_MSG = string.Format("Tài khoản {0} {1}thực hiện {2} vào lúc {3}", lg.ACCOUNT, strStoreName, lg.LOG_ACTION, lg.LOG_DATE);
            lg.SEARCH_TEXT = lg.LOG_MSG;

            using (var db = new RentBikeEntities())
            {
                db.Logs.Add(lg);
                db.SaveChanges();
            }
        }
    }
}