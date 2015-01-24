using RentBike.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RentBike
{
    public partial class FormContractUpdate : System.Web.UI.Page
    {
        int pageSize = 10;
        protected string ContractID { get; set; }
        protected int RentTypeID { get; set; }
        protected bool IsNewContract { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["store_id"] == null)
            {
                Response.Redirect("FormLogin.aspx");
            }
            if (!IsPostBack)
            {
                CommonList.LoadCity(ddlCity);
                CommonList.LoadRentType(ddlRentType);
                CommonList.LoadStore(ddlStore);
                hdfFeeRate.Value = (GetFeeRate(Convert.ToInt16(Session["store_id"])) / 10000).ToString();
                string id = Request.QueryString["ID"];
                string copy = Request.QueryString["copy"];
                int storeId = Convert.ToInt16(Session["store_id"]);
                if (!string.IsNullOrEmpty(id) && string.IsNullOrEmpty(copy)) // EDIT
                {
                    IsNewContract = false;
                    ContractID = id;
                    List<CONTRACT_FULL_VW> lst;
                    int contractid = Convert.ToInt32(id);
                    using (var db = new RentBikeEntities())
                    {
                        Store stor = new Store();
                        stor = db.Stores.FirstOrDefault(s => s.ID == storeId);

                        var st = from s in db.CONTRACT_FULL_VW
                                 where s.ID == contractid
                                 select s;

                        lst = st.ToList<CONTRACT_FULL_VW>();
                        bool bAdmin = CheckAdminPermission();
                        if (bAdmin)
                        {
                            storeId = lst[0].STORE_ID;
                        }
                        ddlStore.SelectedValue = storeId.ToString();
                        if (!bAdmin)
                        {
                            ddlStore.SelectedValue = storeId.ToString();
                            ddlStore.Enabled = false;
                        }
                        pnlTable.Enabled = lst[0].CONTRACT_STATUS;
                        rptPayFeeSchedule.Visible = lst[0].CONTRACT_STATUS;

                        CONTRACT_FULL_VW cntrct = lst[0];
                        txtLicenseNumber.Text = cntrct.LICENSE_NO;
                        txtCustomerName.Text = cntrct.CUSTOMER_NAME;
                        txtPhone.Text = cntrct.PHONE;
                        txtAddress.Text = cntrct.ADDRESS;
                        //ddlCity.SelectedValue = cntrct.CUS_CITY_ID.ToString();
                        if (stor != null)
                        {
                            ddlCity.SelectedValue = stor.CITY_ID.ToString();
                        }
                        else
                        {
                            ddlCity.SelectedValue = cntrct.CUS_CITY_ID.ToString();
                        }

                        txtContractNo.Text = cntrct.AUTO_CONTRACT_NO;
                        var rentType = db.RentTypes.Where(c => c.NAME == cntrct.RENT_TYPE_NAME).FirstOrDefault();
                        ddlRentType.SelectedValue = rentType.ID.ToString();
                        RentTypeID = cntrct.RENT_TYPE_ID;
                        txtAmount.Text = string.Format("{0:0,0}", cntrct.CONTRACT_AMOUNT);
                        txtFeePerDay.Text = string.Format("{0:0,0}", cntrct.FEE_PER_DAY);
                        txtRentDate.Text = string.Format("{0:dd/MM/yyyy}", cntrct.RENT_DATE);
                        txtEndDate.Text = string.Format("{0:dd/MM/yyyy}", cntrct.END_DATE);
                        txtNote.Text = cntrct.NOTE;

                        txtReferencePerson.Text = cntrct.REFERENCE_NAME;
                        txtItemName.Text = cntrct.ITEM_TYPE;
                        txtItemLicenseNo.Text = cntrct.ITEM_LICENSE_NO;
                        txtSerial1.Text = cntrct.SERIAL_1;
                        txtSerial2.Text = cntrct.SERIAL_2;
                        txtItemDetail.Text = cntrct.DETAIL;
                        txtReferencePhone.Text = cntrct.REFERENCE_PHONE;
                        txtSchool.Text = cntrct.SCHOOL_NAME;
                        txtClass.Text = cntrct.CLASS_NAME;

                        ddlCity.Enabled = txtLicenseNumber.Enabled = txtCustomerName.Enabled = txtPhone.Enabled = txtAddress.Enabled = false;
                        txtContractNo.Enabled = ddlRentType.Enabled = txtAmount.Enabled = txtFeePerDay.Enabled = txtRentDate.Enabled = txtEndDate.Enabled = false;
                        txtReferencePerson.Enabled = txtItemName.Enabled = txtItemLicenseNo.Enabled = txtSerial1.Enabled = txtSerial2.Enabled = txtReferencePhone.Enabled = txtSchool.Enabled = txtClass.Enabled = false;

                        LoadPayFeeSchedule();
                    }
                }
                else // NEW
                {
                    IsNewContract = true;
                    btnFinishContract.Visible = false;
                    txtContractNo.Visible = false;
                    using (var db = new RentBikeEntities())
                    {
                        Store stor = new Store();
                        if (storeId != 0)
                        {
                            stor = db.Stores.FirstOrDefault(s => s.ID == storeId);
                            //txtRentDate.Enabled = false;
                            //txtEndDate.Enabled = false;
                            if (!CheckAdminPermission())
                            {
                                ddlStore.SelectedValue = storeId.ToString();
                                ddlStore.Enabled = false;
                            }
                        }
                        else
                        {

                        }
                        RentTypeID = Convert.ToInt16(ddlRentType.SelectedValue);
                        txtRentDate.Text = string.Format("{0:dd/MM/yyyy}", DateTime.Now);
                        txtEndDate.Text = string.Format("{0:dd/MM/yyyy}", DateTime.Now.AddDays(29));

                        if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(copy))
                        {
                            int contractid = Convert.ToInt32(id);
                            CONTRACT_FULL_VW cntrct = db.CONTRACT_FULL_VW.Where(s => s.ID == contractid).FirstOrDefault();
                            if (cntrct != null)
                            {
                                bool bAdmin = CheckAdminPermission();
                                if (bAdmin)
                                {
                                    storeId = cntrct.STORE_ID;
                                }
                                ddlStore.SelectedValue = storeId.ToString();
                                if (!bAdmin)
                                {
                                    ddlStore.SelectedValue = storeId.ToString();
                                    ddlStore.Enabled = false;
                                }
                                txtLicenseNumber.Text = cntrct.LICENSE_NO;
                                txtCustomerName.Text = cntrct.CUSTOMER_NAME;
                                txtPhone.Text = cntrct.PHONE;
                                txtAddress.Text = cntrct.ADDRESS;
                                //ddlCity.SelectedValue = cntrct.CUSTOMER_ID.ToString();
                                ddlCity.SelectedValue = stor.CITY_ID.ToString();

                                txtContractNo.Text = cntrct.AUTO_CONTRACT_NO;
                                var rentType = db.RentTypes.Where(c => c.NAME == cntrct.RENT_TYPE_NAME).FirstOrDefault();
                                ddlRentType.SelectedValue = rentType.ID.ToString();
                                RentTypeID = cntrct.RENT_TYPE_ID;
                                txtAmount.Text = string.Format("{0:0,0}", cntrct.CONTRACT_AMOUNT);
                                txtFeePerDay.Text = string.Format("{0:0,0}", cntrct.FEE_PER_DAY);
                                txtNote.Text = cntrct.NOTE;

                                txtReferencePerson.Text = cntrct.REFERENCE_NAME;
                                txtItemName.Text = cntrct.ITEM_TYPE;
                                txtItemLicenseNo.Text = cntrct.ITEM_LICENSE_NO;
                                txtSerial1.Text = cntrct.SERIAL_1;
                                txtSerial2.Text = cntrct.SERIAL_2;
                                txtItemDetail.Text = cntrct.DETAIL;
                                txtReferencePhone.Text = cntrct.REFERENCE_PHONE;
                                txtSchool.Text = cntrct.SCHOOL_NAME;
                                txtClass.Text = cntrct.CLASS_NAME;
                            }
                        }
                    }
                }
            }
        }

        private List<CONTRACT_FULL_VW> LoadData(string strSearch, int page)
        {
            // LOAD PAGER
            int totalRecord = 0;
            using (var db = new RentBikeEntities())
            {
                var count = (from c in db.Contracts
                             where c.SEARCH_TEXT.Contains(strSearch)
                             select c).Count();
                totalRecord = Convert.ToInt16(count);
            }

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

            // LOAD DATA WITH PAGING
            List<CONTRACT_FULL_VW> dataList;
            int skip = page * pageSize;
            using (var db = new RentBikeEntities())
            {
                var st = from s in db.CONTRACT_FULL_VW
                         where s.SEARCH_TEXT.Contains(strSearch)
                         orderby s.ID
                         select s;

                dataList = st.Skip(skip).Take(pageSize).ToList();
            }

            rptCustomer.DataSource = dataList;
            rptCustomer.DataBind();
            return dataList;
        }

        protected string ValidateFields()
        {
            if (string.IsNullOrEmpty(txtCustomerName.Text.Trim()))
            {
                return "Bạn cần phải nhập tên khách hàng.";
            }
            if (string.IsNullOrEmpty(txtLicenseNumber.Text.Trim()))
            {
                return "Bạn phải nhập số CMT.";
            }
            if (string.IsNullOrEmpty(txtPhone.Text.Trim()))
            {
                return "Bạn phải nhập số điện thoại.";
            }
            if (string.IsNullOrEmpty(txtAddress.Text.Trim()))
            {
                return "Bạn phải nhập địa chỉ.";
            }
            //if (string.IsNullOrEmpty(txtContractNo.Text.Trim()))
            //{
            //    return "Bạn phải nhập số hợp đồng.";
            //}
            if (string.IsNullOrEmpty(txtAmount.Text.Trim()))
            {
                return "Bạn phải nhập số tiền hợp đồng.";
            }
            if (string.IsNullOrEmpty(txtFeePerDay.Text.Trim()) || txtFeePerDay.Text.Trim() == "0")
            {
                return "Bạn phải nhập phí thuê ngày.";
            }
            if (string.IsNullOrEmpty(txtRentDate.Text.Trim()))
            {
                return "Bạn phải nhập ngày bắt đầu hợp đồng.";
            }
            if (string.IsNullOrEmpty(txtEndDate.Text.Trim()))
            {
                return "Bạn phải nhập ngày kết thúc hợp đồng.";
            }
            if (ddlRentType.SelectedValue == "1")
            {
                if (string.IsNullOrEmpty(txtReferencePerson.Text.Trim()))
                {
                    return "Bạn phải nhập tên người xác minh.";
                }
                if (string.IsNullOrEmpty(txtItemName.Text.Trim()))
                {
                    return "Bạn phải nhập loại xe.";
                }
                if (string.IsNullOrEmpty(txtItemLicenseNo.Text.Trim()))
                {
                    return "Bạn phải nhập biến kiểm soát.";
                }
                if (string.IsNullOrEmpty(txtSerial1.Text.Trim()))
                {
                    return "Bạn phải nhập số khung.";
                }
                if (string.IsNullOrEmpty(txtSerial2.Text.Trim()))
                {
                    return "Bạn phải nhập số máy.";
                }
            }
            else if (ddlRentType.SelectedValue == "2")
            {
                if (string.IsNullOrEmpty(txtReferencePerson.Text.Trim()))
                {
                    return "Bạn phải nhập tên người xác minh.";
                }
                if (string.IsNullOrEmpty(txtReferencePhone.Text.Trim()))
                {
                    return "Bạn phải nhập số điện thoại.";
                }
                if (string.IsNullOrEmpty(txtSchool.Text.Trim()))
                {
                    return "Bạn phải nhập tên trường.";
                }
                if (string.IsNullOrEmpty(txtClass.Text.Trim()))
                {
                    return "Bạn phải nhập tên lớp.";
                }
            }
            return string.Empty;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                string id = Request.QueryString["ID"];
                string copy = Request.QueryString["copy"];
                string result = ValidateFields();
                int cusid;
                if (string.IsNullOrEmpty(id) || (!string.IsNullOrEmpty(copy) && copy == "1")) // NEW
                {
                    if (!string.IsNullOrEmpty(result))
                    {
                        lblMessage.Text = result;
                        return;
                    }
                    int status = ExistingContract(txtLicenseNumber.Text.Trim());
                    if (status == 1)
                    {
                        //LoadData(txtLicenseNumber.Text.Trim(), 0);
                        RentTypeID = Convert.ToInt16(ddlRentType.SelectedValue);
                        lblMessage.Text = string.Format("Hợp đồng với CMT {0} chưa được thanh lý.", txtLicenseNumber.Text.Trim());
                        lblMessage.ForeColor = Color.Red;
                    }
                    else
                    {
                        Customer cusItem = new Customer();
                        cusItem.NAME = txtCustomerName.Text.Trim();
                        cusItem.LICENSE_NO = txtLicenseNumber.Text.Trim();
                        cusItem.PHONE = txtPhone.Text.Trim();
                        cusItem.ADDRESS = txtAddress.Text.Trim();
                        cusItem.CITY_ID = Convert.ToInt16(ddlCity.SelectedValue);

                        using (var db = new RentBikeEntities())
                        {
                            var citm = from itm in db.Customers
                                       where itm.NAME == cusItem.NAME &
                                             itm.LICENSE_NO == cusItem.LICENSE_NO &
                                             itm.PHONE == cusItem.PHONE &
                                             itm.ADDRESS == cusItem.ADDRESS &
                                             itm.CITY_ID == cusItem.CITY_ID
                                       select itm;
                            List<Customer> cLst = citm.ToList();

                            if (cLst.Count > 0)
                            {
                                cusid = cLst[0].ID;
                            }
                            else
                            {
                                db.Customers.Add(cusItem);
                                db.SaveChanges();
                                cusid = cusItem.ID;
                            }
                        }

                        // New Contract
                        Contract item = new Contract();
                        item.RENT_TYPE_ID = Convert.ToInt16(ddlRentType.SelectedValue);
                        item.FEE_PER_DAY = Math.Round(Convert.ToDecimal(txtFeePerDay.Text.Replace(",", string.Empty)));
                        if (!string.IsNullOrEmpty(txtRentDate.Text))
                        {
                            string strRentDate = txtRentDate.Text;
                            string[] arr = strRentDate.Split('/');
                            item.RENT_DATE = Convert.ToDateTime(arr[1] + "/" + arr[0] + "/" + arr[2]);
                        }
                        else
                            item.RENT_DATE = DateTime.Now;

                        if (!string.IsNullOrEmpty(txtEndDate.Text))
                        {
                            string strEndDate = txtEndDate.Text;
                            string[] arr = strEndDate.Split('/');
                            item.END_DATE = Convert.ToDateTime(arr[1] + "/" + arr[0] + "/" + arr[2]);
                        }
                        else
                            item.END_DATE = item.RENT_DATE.AddDays(29);
                        item.CLOSE_CONTRACT_DATE = new DateTime(1, 1, 1);
                        item.PAY_FEE_MESSAGE = string.Empty;
                        item.NOTE = txtNote.Text;
                        item.REFERENCE_ID = -1;
                        item.REFERENCE_NAME = txtReferencePerson.Text.Trim();
                        item.ITEM_TYPE = txtItemName.Text.Trim();
                        item.ITEM_LICENSE_NO = txtItemLicenseNo.Text.Trim();
                        item.SERIAL_1 = txtSerial1.Text.Trim();
                        item.SERIAL_2 = txtSerial2.Text.Trim();
                        item.REFERENCE_PHONE = txtReferencePhone.Text.Trim();
                        item.SCHOOL_NAME = txtSchool.Text.Trim();
                        item.CLASS_NAME = txtClass.Text.Trim();
                        item.DETAIL = txtItemDetail.Text.Trim();
                        item.CUSTOMER_ID = hdfCus.Value != string.Empty ? Convert.ToInt16(hdfCus.Value) : item.CUSTOMER_ID = cusid;
                        item.CONTRACT_STATUS = true;
                        item.CONTRACT_AMOUNT = Convert.ToDecimal(txtAmount.Text.Replace(",", string.Empty));
                        item.CREATED_BY = Session["username"].ToString();
                        item.CREATED_DATE = DateTime.Now;
                        item.UPDATED_BY = Session["username"].ToString();
                        item.UPDATED_DATE = DateTime.Now;
                        //item.CONTRACT_NO = txtContractNo.Text.Trim();
                        if (ddlStore.Enabled == true)
                            item.STORE_ID = Convert.ToInt16(ddlStore.SelectedValue);
                        else
                            item.STORE_ID = Convert.ToInt16(Session["store_id"]);
                        item.SEARCH_TEXT = string.Format("{0} {1} {2} {3} {4} {5}",
                                                        txtCustomerName.Text.Trim(),
                                                        txtLicenseNumber.Text.Trim(),
                                                        txtAddress.Text.Trim(),
                                                        txtPhone.Text.Trim(),
                                                        item.AUTO_CONTRACT_NO,
                                                        item.RENT_DATE.ToString("dd/MM/yyyy"));

                        using (var db = new RentBikeEntities())
                        {
                            db.Contracts.Add(item);
                            db.SaveChanges();
                        }

                        DateTime periodTime = DateTime.Now;
                        if (!string.IsNullOrEmpty(txtRentDate.Text))
                        {
                            string strRentDate = txtRentDate.Text;
                            string[] arr = strRentDate.Split('/');
                            periodTime = Convert.ToDateTime(arr[1] + "/" + arr[0] + "/" + arr[2]);
                        }
                        PayPeriod pp1 = new PayPeriod();
                        pp1.CONTRACT_ID = item.ID;
                        if (ddlRentType.SelectedValue == "2")
                        {
                            pp1.PAY_DATE = periodTime;
                        }
                        else
                        {
                            pp1.PAY_DATE = periodTime.AddDays(9);
                        }
                        pp1.AMOUNT_PER_PERIOD = item.FEE_PER_DAY * 10;
                        pp1.STATUS = true;
                        pp1.ACTUAL_PAY = 0;

                        PayPeriod pp2 = new PayPeriod();
                        pp2.CONTRACT_ID = item.ID;
                        if (ddlRentType.SelectedValue == "2")
                        {
                            pp2.PAY_DATE = periodTime.AddDays(9);
                        }
                        else
                        {
                            pp2.PAY_DATE = periodTime.AddDays(19);
                        }
                        pp2.AMOUNT_PER_PERIOD = item.FEE_PER_DAY * 10;
                        pp2.STATUS = true;
                        pp2.ACTUAL_PAY = 0;

                        PayPeriod pp3 = new PayPeriod();
                        pp3.CONTRACT_ID = item.ID;
                        if (ddlRentType.SelectedValue == "2")
                        {
                            pp3.PAY_DATE = periodTime.AddDays(19);
                        }
                        else
                        {
                            pp3.PAY_DATE = periodTime.AddDays(29);
                        }
                        pp3.AMOUNT_PER_PERIOD = item.FEE_PER_DAY * 10;
                        pp3.STATUS = true;
                        pp3.ACTUAL_PAY = 0;

                        using (var rbdb = new RentBikeEntities())
                        {
                            rbdb.PayPeriods.Add(pp1);
                            rbdb.PayPeriods.Add(pp2);
                            rbdb.PayPeriods.Add(pp3);
                            rbdb.SaveChanges();
                        }

                        InOut io = new InOut();
                        io.CONTRACT_ID = item.ID;
                        io.IN_AMOUNT = 0;
                        io.OUT_AMOUNT = Convert.ToDecimal(txtAmount.Text);
                        io.RENT_TYPE_ID = Convert.ToInt16(ddlRentType.SelectedValue);
                        io.PERIOD_DATE = DateTime.Now;
                        io.MORE_INFO = string.Format("Cho khách {0} thuê: {1} ngày {2} trị giá {3}", txtCustomerName.Text.Trim(), txtItemName.Text.Trim(), DateTime.Now.ToString("dd/MM/yyyy"), txtAmount.Text.Trim());
                        if (ddlStore.Enabled == true)
                            io.STORE_ID = Convert.ToInt16(ddlStore.SelectedValue);
                        else
                            io.STORE_ID = Convert.ToInt16(Session["store_id"]);
                        io.SEARCH_TEXT = string.Format("{0} ", io.MORE_INFO);
                        io.INOUT_DATE = DateTime.Now;
                        io.CREATED_BY = Session["username"].ToString();
                        io.CREATED_DATE = DateTime.Now;
                        io.UPDATED_BY = Session["username"].ToString();
                        io.UPDATED_DATE = DateTime.Now;
                        switch (ddlRentType.SelectedValue)
                        {
                            case "1":
                                io.INOUT_TYPE_ID = 17;
                                break;
                            case "2":
                                io.INOUT_TYPE_ID = 22;
                                break;
                            case "3":
                                io.INOUT_TYPE_ID = 23;
                                break;
                            default:
                                List<InOutType> lstiot = new List<InOutType>();
                                using (var db = new RentBikeEntities())
                                {
                                    var iot = from itm in db.InOutTypes
                                              where itm.IS_CONTRACT == true && itm.ACTIVE == false && itm.IS_INCOME == false
                                              select itm;

                                    lstiot = iot.ToList();
                                    if (lstiot.Count > 0)
                                    {
                                        io.INOUT_TYPE_ID = lstiot[0].ID;
                                    }
                                }
                                break;
                        }
                        using (var rbdb = new RentBikeEntities())
                        {
                            rbdb.InOuts.Add(io);
                            rbdb.SaveChanges();
                        }

                        WriteLog(CommonList.ACTION_CREATE_CONTRACT, false);
                        ts.Complete();
                        Response.Redirect("FormContractManagement.aspx");
                    }

                }
                else // EDIT
                {
                    int contractId = Convert.ToInt16(id);
                    using (var rbdb = new RentBikeEntities())
                    {
                        var item = rbdb.Contracts.FirstOrDefault(itm => itm.ID == contractId);

                        item.NOTE = txtNote.Text.Trim();
                        item.DETAIL = txtItemDetail.Text.Trim();
                        item.UPDATED_BY = Session["username"].ToString();
                        item.UPDATED_DATE = DateTime.Now;

                        int num = rbdb.SaveChanges();
                    }

                    WriteLog(CommonList.ACTION_UPDATE_CONTRACT, false);
                    ts.Complete();
                    Response.Redirect("FormContractManagement.aspx");
                }
            }

        }

        private int ExistingContract(string licenseNo)
        {
            using (var db = new RentBikeEntities())
            {
                int storeId = Convert.ToInt16(Session["store_id"]);
                CONTRACT_FULL_VW cntrct = db.CONTRACT_FULL_VW.Where(s => s.LICENSE_NO == licenseNo).FirstOrDefault();
                if (cntrct != null)
                {
                    if (cntrct != null)
                    {
                        if (cntrct.CONTRACT_STATUS == true)
                        {
                            LoadData(licenseNo, 0);
                            if (storeId != 0 && storeId != cntrct.STORE_ID)
                            {
                                foreach (RepeaterItem rptItem in rptCustomer.Items)
                                {
                                    if (rptItem.FindControl("hplContractInfo") != null)
                                        ((HyperLink)rptItem.FindControl("hplContractInfo")).Enabled = false;

                                    if (rptItem.FindControl("btnChoose") != null)
                                        ((Button)rptItem.FindControl("btnChoose")).Enabled = false;
                                }

                            }
                            return 1;
                        }
                        else
                            return 0;
                    }
                }
            }
            return -1;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("FormContractManagement.aspx");
        }

        private void LoadPayFeeSchedule()
        {
            string id = Request.QueryString["ID"];
            if (!string.IsNullOrEmpty(id))
            {
                int contractID = Convert.ToInt16(id);
                List<PayPeriod> lst;
                using (var db = new RentBikeEntities())
                {
                    var st = from s in db.PayPeriods
                             where s.CONTRACT_ID == contractID
                             select s;

                    lst = st.ToList<PayPeriod>();

                    foreach (PayPeriod pp in lst)
                    {
                        var payList = db.InOuts.Where(s => s.PERIOD_ID == pp.ID);
                        if (pp != null)
                        {
                            var sumPay = payList.Select(c => c.IN_AMOUNT).DefaultIfEmpty(0).Sum();
                            if (sumPay > 0)
                            {
                                pp.ACTUAL_PAY = sumPay;
                                db.SaveChanges();
                            }
                        }
                    }

                    lst = (from s in db.PayPeriods
                           where s.CONTRACT_ID == contractID
                           select s).ToList();

                    if (lst != null && lst.Count == 3)
                    {
                        if (lst[0].ACTUAL_PAY > lst[0].AMOUNT_PER_PERIOD)
                        {
                            lst[1].ACTUAL_PAY += lst[0].ACTUAL_PAY - lst[0].AMOUNT_PER_PERIOD;
                        }

                        if (lst[1].ACTUAL_PAY > lst[1].AMOUNT_PER_PERIOD)
                        {
                            lst[2].ACTUAL_PAY += lst[1].ACTUAL_PAY - lst[1].AMOUNT_PER_PERIOD;
                        }
                    }
                }

                rptPayFeeSchedule.DataSource = lst;
                rptPayFeeSchedule.DataBind();

            }
            else
            {
                List<PayPeriod> lst = new List<PayPeriod>();
                PayPeriod p1 = new PayPeriod();
                p1.PAY_DATE = DateTime.Now;
                PayPeriod p2 = new PayPeriod();
                p2.PAY_DATE = DateTime.Now.AddDays(9);
                PayPeriod p3 = new PayPeriod();
                p3.PAY_DATE = DateTime.Now.AddDays(19);

                lst.Add(p1);
                lst.Add(p2);
                lst.Add(p3);

                rptPayFeeSchedule.DataSource = lst;
                rptPayFeeSchedule.DataBind();
            }
        }

        protected void btnFinishContract_Click(object sender, EventArgs e)
        {
            string id = Request.QueryString["ID"];
            Response.Redirect(string.Format("FormCloseContract.aspx?ID={0}", id));
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text.Trim(), 0);
        }

        protected void ddlPager_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text.Trim(), Convert.ToInt16(ddlPager.SelectedValue) - 1);
        }

        private decimal GetFeeRate(int storeId)
        {
            decimal fee = 0;
            List<StoreFee> lst = new List<StoreFee>();
            using (var db = new RentBikeEntities())
            {
                var item = from s in db.StoreFees
                           where s.STORE_ID == storeId
                           select s;
                lst = item.ToList();
                if (lst.Count > 0)
                {
                    fee = Convert.ToDecimal(lst[0].FEE_PERCENT);
                }
            }

            return fee;
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

        protected void rptCustomer_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "btnChoose")
            {
                int idx = e.Item.ItemIndex;
                int cusid = Convert.ToInt16(((HiddenField)rptCustomer.Items[idx].FindControl("hdfCustomerID")).Value);
                hdfCus.Value = cusid.ToString();
                List<Customer> lst = new List<Customer>();
                using (var db = new RentBikeEntities())
                {
                    var item = from itm in db.Customers
                               where itm.ID == cusid
                               select itm;

                    lst = item.ToList();
                }

                if (lst.Count > 0)
                {
                    txtCustomerName.Text = lst[0].NAME;
                    txtLicenseNumber.Text = lst[0].LICENSE_NO;
                    txtPhone.Text = lst[0].PHONE;
                    txtAddress.Text = lst[0].ADDRESS;
                }

            }
        }


        // NOT USE ANYMORE
        private void LoadStore(List<Store> lst)
        {
            ddlStore.DataSource = lst;
            ddlStore.DataValueField = "ID";
            ddlStore.DataTextField = "NAME";
            ddlStore.DataBind();
        }

        private static List<Store> GetStoreByCity(int city_id)
        {
            List<Store> lst;
            using (var db = new RentBikeEntities())
            {
                var st = from s in db.Stores
                         where s.CITY_ID == city_id
                         select s;

                lst = st.ToList<Store>();
            }
            return lst;
        }

        private static List<Customer> GetCustomerByCity(int city_id)
        {
            List<Customer> lst;
            using (var db = new RentBikeEntities())
            {
                var st = from s in db.Customers
                         where s.CITY_ID == city_id
                         orderby s.NAME
                         select s;

                lst = st.ToList<Customer>();
            }
            return lst;
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

        public bool ShowBlueImage(decimal amountPer, decimal actualPay)
        {
            if (amountPer <= actualPay)
                return true;
            return false;
        }

        public bool ShowOrangeImage(decimal amountPer, decimal actualPay)
        {
            if (actualPay > 0 && amountPer > actualPay)
                return true;
            return false;
        }
    }
}