using RentBike.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RentBike
{
    public partial class FormCloseContract : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["store_id"] == null)
            {
                Response.Redirect("FormLogin.aspx");
            }
            if (!IsPostBack)
            {
                int id = Convert.ToInt16(Request.QueryString["ID"]);
                Contract con;
                using (var db = new RentBikeEntities())
                {
                    var contract = db.Contracts.First(c => c.ID == id);
                    con = contract;
                }

                txtEndDate.Text = string.Format("{0:dd/MM/yyyy}", con.END_DATE);
                txtAmount.Text = string.Format("{0:0,0}", con.CONTRACT_AMOUNT);

                TimeSpan ts = DateTime.Now.Date.Subtract(con.END_DATE);
                txtOverDate.Text = Math.Round(ts.TotalDays).ToString();

                List<InOut> ioLst;
                using (var db = new RentBikeEntities())
                {

                    var inoutList = db.InOuts.Where(io => io.CONTRACT_ID == id).ToList();
                    ioLst = inoutList;
                }
                decimal paidAmount = 0;
                for (int i = 0; i < ioLst.Count; i++)
                {
                    paidAmount += ioLst[i].IN_AMOUNT;
                }

                if (ts.TotalDays >= 0)
                {
                    txtPayFee.Text = string.Format("{0:0,0}", Math.Round(con.FEE_PER_DAY * 30 - paidAmount + con.FEE_PER_DAY * Convert.ToDecimal(Math.Ceiling(ts.TotalDays))));
                    txtRealIncome.Text = txtPayFee.Text;
                }
                else
                {
                    TimeSpan ts1 = DateTime.Now.Subtract(con.RENT_DATE);
                    //txtPayFee.Text = string.Format("{0:0,0}", con.FEE_PER_DAY * Convert.ToDecimal(Math.Ceiling(ts1.TotalDays)) - paidAmount);
                    if (con.RENT_TYPE_ID == 2)
                    {
                        txtPayFee.Text = string.Format("{0:0,0}", Convert.ToInt32(con.FEE_PER_DAY) * 10);
                    }
                    else
                    {
                        txtPayFee.Text = string.Format("{0:0,0}", con.FEE_PER_DAY * Convert.ToDecimal(Math.Ceiling(ts1.TotalDays)) - paidAmount);
                    }
                }
                txtRealIncome.Text = txtPayFee.Text;
                txtReduceAmount.Text = "0";
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                int contractId = Convert.ToInt16(Request.QueryString["ID"]);
                CONTRACT_FULL_VW con = new CONTRACT_FULL_VW();
                using (var db = new RentBikeEntities())
                {
                    var contract = db.CONTRACT_FULL_VW.First(c => c.ID == contractId);
                    con = contract;
                }

                // INOUT --> IN amount
                InOut io1 = new InOut();
                io1.IN_AMOUNT = con.CONTRACT_AMOUNT;
                io1.OUT_AMOUNT = 0;
                io1.CONTRACT_ID = con.ID;
                io1.PERIOD_ID = -1;
                io1.PERIOD_DATE = new DateTime(1, 1, 1);
                io1.RENT_TYPE_ID = con.RENT_TYPE_ID;
                using (var db = new RentBikeEntities())
                {
                    var item = db.InOutTypes.First(s => s.NAME == "Thanh lý");
                    io1.INOUT_TYPE_ID = item.ID;
                }
                io1.MORE_INFO = txtMoreInfo.Text.Trim();
                io1.STORE_ID = con.STORE_ID;
                io1.SEARCH_TEXT = string.Format("{0} {1} {2}", con.AUTO_CONTRACT_NO, con.CUSTOMER_NAME, con.STORE_NAME);
                io1.INOUT_DATE = DateTime.Now;
                io1.CREATED_BY = Session["username"].ToString();
                io1.CREATED_DATE = DateTime.Now;
                io1.UPDATED_BY = Session["username"].ToString();
                io1.UPDATED_DATE = DateTime.Now;

                // IN --> Fee hoac la OUT neu tra truoc ngay cuoi cung cua ki da tra tien
                InOut io2 = new InOut();
                io2.RENT_TYPE_ID = con.RENT_TYPE_ID;
                io2.CONTRACT_ID = con.ID;
                io2.PERIOD_ID = -1;
                io2.PERIOD_DATE = new DateTime(1, 1, 1);
                io2.INOUT_DATE = DateTime.Now;
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

                if (Convert.ToDecimal(txtPayFee.Text) < 0)
                {
                    io2.IN_AMOUNT = 0;
                    io2.OUT_AMOUNT = Math.Abs(Convert.ToDecimal(txtRealIncome.Text));
                    feeName = "Trả lại phí thừa";
                }
                else
                {
                    io2.IN_AMOUNT = Convert.ToDecimal(txtRealIncome.Text.Replace(".",string.Empty));
                    io2.OUT_AMOUNT = 0;
                }

                using (var db = new RentBikeEntities())
                {
                    var item = db.InOutTypes.First(s => s.NAME == feeName);
                    io2.INOUT_TYPE_ID = item.ID;
                }
                io2.MORE_INFO = string.Format("Trả phí thừa hợp đồng {0}", con.AUTO_CONTRACT_NO);
                io2.PERIOD_DATE = DateTime.Now;
                io2.STORE_ID = con.STORE_ID;
                io2.SEARCH_TEXT = string.Format("{0} {1} {2}", con.AUTO_CONTRACT_NO, con.CUSTOMER_NAME, con.STORE_NAME);
                io2.INOUT_DATE = DateTime.Now;
                io2.CREATED_BY = Session["username"].ToString();
                io2.CREATED_DATE = DateTime.Now;
                io2.UPDATED_BY = Session["username"].ToString();
                io2.UPDATED_DATE = DateTime.Now;

                // Out --> Fee reduce
                decimal reduceAmount = Convert.ToDecimal(txtReduceAmount.Text.Replace(",",string.Empty));
                InOut io3 = new InOut();
                if (reduceAmount != 0)
                {
                    io3.CONTRACT_ID = con.ID;
                    io3.IN_AMOUNT = 0;
                    io3.OUT_AMOUNT = reduceAmount;
                    io3.RENT_TYPE_ID = con.RENT_TYPE_ID;
                    using (var db = new RentBikeEntities())
                    {
                        var item = db.InOutTypes.First(s => s.NAME == "Giảm trừ phí");
                        io3.INOUT_TYPE_ID = item.ID;
                    }
                    io3.MORE_INFO = txtMoreInfo.Text.Trim();
                    io3.PERIOD_DATE = DateTime.Now;
                    io3.STORE_ID = con.STORE_ID;
                    io3.SEARCH_TEXT = string.Format("{0} {1} {2}", con.AUTO_CONTRACT_NO, con.CUSTOMER_NAME, con.STORE_NAME);
                    io3.INOUT_DATE = DateTime.Now;
                    io3.CREATED_BY = Session["username"].ToString();
                    io3.CREATED_DATE = DateTime.Now;
                    io3.UPDATED_BY = Session["username"].ToString();
                    io3.UPDATED_DATE = DateTime.Now;
                }

                using (var db = new RentBikeEntities())
                {
                    db.InOuts.Add(io1);
                    db.InOuts.Add(io2);
                    if (reduceAmount != 0)
                    {
                        db.InOuts.Add(io3);
                    }
                    db.SaveChanges();
                }

                // Writelog
                WriteLog(CommonList.ACTION_CLOSE_CONTRACT, false);

                // Update status contract
                using (var db = new RentBikeEntities())
                {
                    var contract = db.Contracts.First(c => c.ID == contractId);
                    contract.CONTRACT_STATUS = false;
                    contract.CLOSE_CONTRACT_DATE = DateTime.Now;
                    db.SaveChanges();
                }

                // Insert History row
                ContractHistory ch = new ContractHistory();
                ch.CONTRACT_ID = con.ID;
                ch.AUTO_CONTRACT_NO = con.AUTO_CONTRACT_NO;
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
                ch.CREATED_BY = Session["username"].ToString();
                ch.CREATED_DATE = DateTime.Now;
                ch.UPDATED_BY = Session["username"].ToString();
                ch.UPDATED_DATE = DateTime.Now;
                using (var db = new RentBikeEntities())
                {
                    db.ContractHistories.Add(ch);
                    db.SaveChanges();
                }

                trans.Complete();
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
            if (CheckAdminPermission())
            {
                DropDownList drpStore = this.Master.FindControl("ddlStore") as DropDownList;
                strStoreName = drpStore.SelectedItem.Text;
            }
            else
            {
                strStoreName = Session["store_name"].ToString();
            }
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

        public bool CheckAdminPermission()
        {
            string acc = Convert.ToString(Session["username"]);
            using (var db = new RentBikeEntities())
            {
                var item = db.Accounts.First(s => s.ACC == acc);

                if (item.PERMISSION_ID == 1)
                    return true;
                return false;
            }
        }
    }
}