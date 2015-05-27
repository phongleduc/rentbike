using RentBike.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RentBike
{
    public partial class FormContractUpdate : FormBase
    {
        protected string ContractID { get; set; }
        protected int RentTypeID { get; set; }
        protected bool IsNewContract { get; set; }
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!IsPostBack)
            {
                try
                {
                    CommonList.LoadRentType(ddlRentType);
                    CommonList.LoadStore(ddlStore);
                    hdfFeeRate.Value = (GetFeeRate(Convert.ToInt32(Session["store_id"])) / 10000).ToString();
                    string id = Request.QueryString["ID"];
                    string sId = Request.QueryString["sID"];
                    string copy = Request.QueryString["copy"];
                    int STORE_ID = Convert.ToInt32(Session["store_id"]);

                    if (!string.IsNullOrEmpty(id) && string.IsNullOrEmpty(copy)) // EDIT
                    {
                        using (var db = new RentBikeEntities())
                        {
                            int contractId = Helper.parseInt(id);
                            var contract = db.Contracts.FirstOrDefault(c => c.CONTRACT_STATUS == true && c.ID == contractId);

                            IsNewContract = false;
                            ContractID = id;
                            List<CONTRACT_FULL_VW> lst;
                            int contractid = Convert.ToInt32(id);

                            Store stor = new Store();
                            stor = db.Stores.FirstOrDefault(s => s.ID == STORE_ID);

                            var st = from s in db.CONTRACT_FULL_VW
                                     where s.ID == contractid
                                     select s;

                            lst = st.ToList<CONTRACT_FULL_VW>();
                            ddlStore.SelectedValue = STORE_ID.ToString();

                            bool bDifferentSTORE_ID = false;
                            if (Helper.parseInt(sId) != STORE_ID)
                            {
                                if (!IS_ADMIN)
                                    bDifferentSTORE_ID = true;
                                STORE_ID = Helper.parseInt(sId);
                            }

                            ddlStore.SelectedValue = STORE_ID.ToString();
                            if (!IS_ADMIN)
                            {
                                ddlStore.Enabled = false;
                            }

                            if (!lst[0].CONTRACT_STATUS || (bDifferentSTORE_ID && !string.IsNullOrEmpty(Request.QueryString["sID"])))
                            {
                                pnlTable.Enabled = false;
                                rptPayFeeSchedule.Visible = false;
                            }

                            CONTRACT_FULL_VW cntrct = lst[0];
                            txtLicenseNumber.Text = cntrct.LICENSE_NO;
                            txtCustomerName.Text = cntrct.CUSTOMER_NAME;
                            txtBirthDay.Text = string.Format("{0:dd/MM/yyyy}", cntrct.BIRTH_DAY);
                            txtRangeDate.Text = string.Format("{0:dd/MM/yyyy}", cntrct.LICENSE_RANGE_DATE);
                            txtPlaceDate.Text = cntrct.LICENSE_RANGE_PLACE;
                            txtPhone.Text = cntrct.PHONE;
                            txtPermanentResidence.Text = cntrct.PERMANENT_RESIDENCE;
                            txtCurrentResidence.Text = cntrct.CURRENT_RESIDENCE;
                            txtContractNo.Text = cntrct.CONTRACT_NO;
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
                            txtImplementer.Text = cntrct.IMPLEMENTER;
                            txtBackDocument.Text = cntrct.BACK_TO_DOCUMENTS;
                            txtItemDetail.Text = cntrct.DETAIL;
                            txtReferencePhone.Text = cntrct.REFERENCE_PHONE;
                            txtSchool.Text = cntrct.SCHOOL_NAME;
                            txtClass.Text = cntrct.CLASS_NAME;

                            BuildPhotoLibrary(cntrct);

                            ddlStore.Enabled = txtContractNo.Enabled = txtRentDate.Enabled = txtEndDate.Enabled = false;

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
                            if (!IS_ADMIN)
                            {
                                ddlStore.SelectedValue = STORE_ID.ToString();
                                ddlStore.Enabled = false;
                            }
                            RentTypeID = Convert.ToInt32(ddlRentType.SelectedValue);
                            txtRentDate.Text = string.Format("{0:dd/MM/yyyy}", DateTime.Now);
                            txtEndDate.Text = string.Format("{0:dd/MM/yyyy}", DateTime.Now.AddDays(29));

                            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(copy))
                            {
                                int contractid = Convert.ToInt32(id);
                                CONTRACT_FULL_VW cntrct = db.CONTRACT_FULL_VW.Where(s => s.ID == contractid).FirstOrDefault();
                                if (cntrct != null)
                                {
                                    if (IS_ADMIN)
                                        STORE_ID = cntrct.STORE_ID;
                                    else
                                        ddlStore.Enabled = false;

                                    ddlStore.SelectedValue = STORE_ID.ToString();

                                    txtLicenseNumber.Text = cntrct.LICENSE_NO;
                                    txtCustomerName.Text = cntrct.CUSTOMER_NAME;
                                    txtBirthDay.Text = string.Format("{0:dd/MM/yyyy}", cntrct.BIRTH_DAY);
                                    txtRangeDate.Text = string.Format("{0:dd/MM/yyyy}", cntrct.LICENSE_RANGE_DATE);
                                    txtPlaceDate.Text = cntrct.LICENSE_RANGE_PLACE;
                                    txtPhone.Text = cntrct.PHONE;
                                    txtPermanentResidence.Text = cntrct.PERMANENT_RESIDENCE;
                                    txtCurrentResidence.Text = cntrct.CURRENT_RESIDENCE;
                                    txtContractNo.Text = cntrct.CONTRACT_NO;
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
                                    txtImplementer.Text = cntrct.IMPLEMENTER;
                                    txtBackDocument.Text = cntrct.BACK_TO_DOCUMENTS;
                                    txtItemDetail.Text = cntrct.DETAIL;
                                    txtReferencePhone.Text = cntrct.REFERENCE_PHONE;
                                    txtSchool.Text = cntrct.SCHOOL_NAME;
                                    txtClass.Text = cntrct.CLASS_NAME;

                                    BuildPhotoLibrary(cntrct);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = ex.Message;
                    lblMessage.CssClass = "text-center text-danger";
                }
            }
        }

        private void BuildPhotoLibrary(CONTRACT_FULL_VW cntrct)
        {
            // Initialize StringWriter instance.
            StringWriter stringWriter = new StringWriter();

            // Put HtmlTextWriter in using block because it needs to call Dispose.
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                writer.RenderBeginTag(HtmlTextWriterTag.P); // Start of P
                for (int i = 1; i <= 5; i++)
                {
                    BuildPhotoData(writer, Convert.ToString(Helper.GetPropValue(cntrct, "PHOTO_" + i)), Convert.ToString(Helper.GetPropValue(cntrct, "THUMBNAIL_PHOTO_" + i)));
                }
                writer.RenderEndTag();  //End of P

                litPhoto.Text = stringWriter.ToString();
            }
        }
        private void BuildPhotoData(HtmlTextWriter writer, string photo, string thumbnailPhoto)
        {
            // Put HtmlTextWriter in using block because it needs to call Dispose.
            if (!string.IsNullOrEmpty(photo) && !string.IsNullOrEmpty(thumbnailPhoto))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "fancybox-buttons");
                writer.AddAttribute("data-fancybox-group", "button");
                writer.AddAttribute(HtmlTextWriterAttribute.Href, photo);
                writer.RenderBeginTag(HtmlTextWriterTag.A);

                writer.AddAttribute("src", thumbnailPhoto);
                writer.AddAttribute("alt", "");
                writer.RenderBeginTag(HtmlTextWriterTag.Img);
                writer.RenderEndTag(); //End of Img

                writer.RenderEndTag(); //End of A
            }
        }

        protected string ValidateFields()
        {
            if (string.IsNullOrEmpty(txtCustomerName.Text.Trim()))
            {
                return "Bạn cần phải nhập tên khách hàng.";
            }
            if (string.IsNullOrEmpty(txtBirthDay.Text.Trim()))
            {
                return "Bạn cần phải nhập ngày sinh.";
            }
            if (string.IsNullOrEmpty(txtLicenseNumber.Text.Trim()))
            {
                return "Bạn phải nhập số CMT.";
            }
            if (string.IsNullOrEmpty(txtRangeDate.Text.Trim()))
            {
                return "Bạn phải nhập ngày cấp.";
            }
            if (string.IsNullOrEmpty(txtPlaceDate.Text.Trim()))
            {
                return "Bạn phải nhập nơi cấp.";
            }
            if (string.IsNullOrEmpty(txtPhone.Text.Trim()))
            {
                return "Bạn phải nhập số điện thoại.";
            }
            if (string.IsNullOrEmpty(txtPermanentResidence.Text.Trim()))
            {
                return "Bạn phải nhập hộ khẩu thường trú.";
            }
            if (string.IsNullOrEmpty(txtCurrentResidence.Text.Trim()))
            {
                return "Bạn phải nhập hiện trú tại đâu.";
            }
            if (string.IsNullOrEmpty(txtAmount.Text.Trim()))
            {
                return "Bạn phải nhập số tiền hợp đồng.";
            }
            if (string.IsNullOrEmpty(txtFeePerDay.Text.Trim()))
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
                if (string.IsNullOrEmpty(txtImplementer.Text.Trim()))
                {
                    return "Bạn phải nhập người làm.";
                }
                if (string.IsNullOrEmpty(txtBackDocument.Text.Trim()))
                {
                    return "Bạn phải nhập giấy tờ để lại.";
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
            if (Request.Files.Count > 5)
            {
                return "Bạn không thể chọn quá 5 file ảnh.";
            }
            return string.Empty;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    string id = Request.QueryString["ID"];
                    string copy = Request.QueryString["copy"];
                    string result = ValidateFields();
                    int cusid = 0;
                    if (string.IsNullOrEmpty(id) || (!string.IsNullOrEmpty(copy) && copy == "1")) // NEW
                    {
                        if (!string.IsNullOrEmpty(result))
                        {
                            lblMessage.Text = result;
                            return;
                        }

                        Contract contract = CommonList.GetContractByLicenseNo(txtLicenseNumber.Text.Trim());
                        if (contract != null)
                        {
                            // Initialize StringWriter instance.
                            StringWriter stringWriter = new StringWriter();
                            // Put HtmlTextWriter in using block because it needs to call Dispose.
                            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
                            {
                                writer.Write("Số CMTND/GPLX này hiện tại đã đăng ký hợp đồng ");
                                writer.AddAttribute(HtmlTextWriterAttribute.Href, string.Format("FormContractUpdate.aspx?ID={0}&sID={1}", contract.ID, contract.STORE_ID));
                                writer.RenderBeginTag(HtmlTextWriterTag.A); // Start of A
                                writer.Write(contract.CONTRACT_NO);
                                writer.RenderEndTag();  //End of A

                                lblMessage.Text = stringWriter.ToString();
                                return;
                            }

                        }
                        using (var db = new RentBikeEntities())
                        {
                            bool IsNewCust = false;
                            Customer cusItem = db.Customers.FirstOrDefault(c => c.LICENSE_NO == txtLicenseNumber.Text.Trim() && c.NAME == txtCustomerName.Text.Trim());
                            if (cusItem == null)
                            {
                                IsNewCust = true;
                                cusItem = new Customer();
                            }

                            cusItem.NAME = txtCustomerName.Text.Trim();
                            cusItem.BIRTH_DAY = DateTime.ParseExact(txtBirthDay.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            cusItem.LICENSE_NO = txtLicenseNumber.Text.Trim();
                            cusItem.LICENSE_RANGE_DATE = DateTime.ParseExact(txtRangeDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            cusItem.LICENSE_RANGE_PLACE = txtPlaceDate.Text.Trim();
                            cusItem.PHONE = txtPhone.Text.Trim();
                            cusItem.PERMANENT_RESIDENCE = txtPermanentResidence.Text.Trim();
                            cusItem.CURRENT_RESIDENCE = txtCurrentResidence.Text.Trim();

                            if (IsNewCust)
                            {
                                db.Customers.Add(cusItem);
                            }
                            db.SaveChanges();
                            cusid = cusItem.ID;
                        }

                        // New Contract
                        Contract item = new Contract();
                        item.RENT_TYPE_ID = Convert.ToInt32(ddlRentType.SelectedValue);
                        item.FEE_PER_DAY = Math.Round(Convert.ToDecimal(txtFeePerDay.Text.Replace(",", string.Empty)));
                        if (!string.IsNullOrEmpty(txtRentDate.Text))
                        {
                            item.RENT_DATE = DateTime.ParseExact(txtRentDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        else
                            item.RENT_DATE = DateTime.Now;

                        if (!string.IsNullOrEmpty(txtEndDate.Text))
                        {
                            item.END_DATE = DateTime.ParseExact(txtEndDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        else
                            item.END_DATE = item.RENT_DATE.AddDays(29);
                        item.EXTEND_END_DATE = item.END_DATE;
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
                        item.IMPLEMENTER = txtImplementer.Text.Trim();
                        item.BACK_TO_DOCUMENTS = txtBackDocument.Text.Trim();
                        item.DETAIL = txtItemDetail.Text.Trim();
                        item.CUSTOMER_ID = cusid;
                        item.CONTRACT_STATUS = true;
                        item.CONTRACT_AMOUNT = Convert.ToDecimal(txtAmount.Text.Replace(",", string.Empty));
                        item.CREATED_BY = Session["username"].ToString();
                        item.CREATED_DATE = DateTime.Now;
                        item.UPDATED_BY = Session["username"].ToString();
                        item.UPDATED_DATE = DateTime.Now;
                        //item.CONTRACT_NO = txtContractNo.Text.Trim();
                        if (ddlStore.Enabled == true)
                            item.STORE_ID = Convert.ToInt32(ddlStore.SelectedValue);
                        else
                            item.STORE_ID = Convert.ToInt32(Session["store_id"]);
                        item.SEARCH_TEXT = string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8}",
                                                        txtCustomerName.Text.Trim(),
                                                        txtBirthDay.Text.Trim(),
                                                        txtLicenseNumber.Text.Trim(),
                                                        txtRangeDate.Text.Trim(),
                                                        txtPermanentResidence.Text.Trim(),
                                                        txtCurrentResidence.Text.Trim(),
                                                        txtPhone.Text.Trim(),
                                                        item.CONTRACT_NO,
                                                        item.RENT_DATE.ToString("dd/MM/yyyy"));
                        SavePhoto(item);
                        using (var db = new RentBikeEntities())
                        {
                            db.Contracts.Add(item);
                            db.SaveChanges();


                            DateTime periodTime = DateTime.Now;
                            if (!string.IsNullOrEmpty(txtRentDate.Text))
                            {
                                periodTime = DateTime.ParseExact(txtRentDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            }
                            CommonList.CreatePayPeriod(db, item.ID, periodTime, true);
                        }

                        InOut io = new InOut();
                        io.CONTRACT_ID = item.ID;
                        io.IN_AMOUNT = 0;
                        io.OUT_AMOUNT = Convert.ToDecimal(txtAmount.Text);
                        io.RENT_TYPE_ID = Convert.ToInt32(ddlRentType.SelectedValue);
                        io.PERIOD_DATE = DateTime.Now;
                        io.MORE_INFO = string.Format("Cho khách {0} thuê: {1} ngày {2} trị giá {3}", txtCustomerName.Text.Trim(), txtItemName.Text.Trim(), DateTime.Now.ToString("dd/MM/yyyy"), txtAmount.Text.Trim());
                        if (ddlStore.Enabled == true)
                            io.STORE_ID = Convert.ToInt32(ddlStore.SelectedValue);
                        else
                            io.STORE_ID = Convert.ToInt32(Session["store_id"]);
                        io.SEARCH_TEXT = string.Format("{0} ", io.MORE_INFO);
                        io.INOUT_DATE = DateTime.Now;
                        io.CREATED_BY = Session["username"].ToString();
                        io.CREATED_DATE = DateTime.Now;
                        io.UPDATED_BY = Session["username"].ToString();
                        io.UPDATED_DATE = DateTime.Now;
                        io.INOUT_TYPE_ID = CommonList.GetInoutTypeFromRentType(Helper.parseInt(ddlRentType.SelectedValue));

                        using (var rbdb = new RentBikeEntities())
                        {
                            rbdb.InOuts.Add(io);
                            rbdb.SaveChanges();

                            CommonList.AutoExtendPeriod(rbdb, item.ID);
                        }

                        WriteLog(Constants.ACTION_CREATE_CONTRACT, false);
                        ts.Complete();
                    }
                    else // EDIT
                    {
                        int contractId = Helper.parseInt(id);
                        bool bUpdateInOutAndPeriod = false;
                        using (var db = new RentBikeEntities())
                        {
                            var item = db.Contracts.FirstOrDefault(itm => itm.ID == contractId);
                            int rentTypeId = Helper.parseInt(ddlRentType.SelectedValue);
                            decimal contractAmount = Convert.ToDecimal(txtAmount.Text);
                            decimal feePerDay = Convert.ToDecimal(txtFeePerDay.Text);

                            if (contractAmount != item.CONTRACT_AMOUNT || rentTypeId != item.RENT_TYPE_ID || feePerDay != item.FEE_PER_DAY)
                                bUpdateInOutAndPeriod = true;


                            if (bUpdateInOutAndPeriod)
                            {
                                //Update for contract record only.
                                int inOutTypeId = CommonList.GetInoutTypeFromRentType(item.RENT_TYPE_ID);
                                var inOut = db.InOuts.FirstOrDefault(c => c.CONTRACT_ID == contractId && c.INOUT_TYPE_ID == inOutTypeId);
                                if (inOut != null)
                                {
                                    // SAVE INOUT
                                    inOut.OUT_AMOUNT = contractAmount;
                                    inOut.RENT_TYPE_ID = rentTypeId;
                                    inOut.INOUT_TYPE_ID = CommonList.GetInoutTypeFromRentType(rentTypeId);
                                    inOut.MORE_INFO = inOut.SEARCH_TEXT = string.Format("Cho khách {0} thuê: {1} ngày {2} trị giá {3}", txtCustomerName.Text.Trim(), txtItemName.Text.Trim(), DateTime.Now.ToString("dd/MM/yyyy"), txtAmount.Text.Trim());
                                    inOut.UPDATED_BY = Session["username"].ToString();
                                    inOut.UPDATED_DATE = DateTime.Now;
                                }

                                //Update for others in out record of the contract
                                var listInOut = db.InOuts.Where(c => c.CONTRACT_ID == contractId && c.RENT_TYPE_ID == item.RENT_TYPE_ID).ToList();
                                foreach (var io in listInOut)
                                {
                                    io.RENT_TYPE_ID = rentTypeId;
                                    io.UPDATED_BY = Session["username"].ToString();
                                    io.UPDATED_DATE = DateTime.Now;
                                }

                                //Update for 3 first PayPeriod records
                                List<PayPeriod> listPayPeriod = db.PayPeriods.Where(c => c.CONTRACT_ID == contractId).ToList();
                                foreach (var pp in listPayPeriod.Take(3))
                                {
                                    pp.AMOUNT_PER_PERIOD = feePerDay * 10;
                                }

                                //Update for remain PayPeriod records
                                int multipleFee = Convert.ToInt32(Decimal.Floor(contractAmount / 100000));
                                decimal increateFeeCar = (feePerDay * 10) + (multipleFee * 50 * 10);
                                decimal increateFeeEquip = (feePerDay * 10) + (multipleFee * 100 * 10);
                                decimal increateFeeOther = (feePerDay * 10);
                                foreach (var pp in listPayPeriod.Skip(3))
                                {
                                    switch (rentTypeId)
                                    {
                                        case 1:
                                            if (((feePerDay / multipleFee) * 10) < 4000)
                                                pp.AMOUNT_PER_PERIOD = increateFeeCar;
                                            else
                                                pp.AMOUNT_PER_PERIOD = feePerDay * 10;
                                            break;
                                        case 2:
                                            if (((feePerDay / multipleFee) * 10) < 6000)
                                                pp.AMOUNT_PER_PERIOD = increateFeeEquip;
                                            else
                                                pp.AMOUNT_PER_PERIOD = feePerDay * 10;
                                            break;
                                        default:
                                            pp.AMOUNT_PER_PERIOD = increateFeeOther;
                                            break;
                                    }
                                }
                            }

                            //Update contract infor
                            item.NOTE = txtNote.Text;
                            item.REFERENCE_ID = -1;
                            item.REFERENCE_NAME = txtReferencePerson.Text.Trim();
                            item.ITEM_TYPE = txtItemName.Text.Trim();
                            item.ITEM_LICENSE_NO = txtItemLicenseNo.Text.Trim();
                            item.RENT_TYPE_ID = Helper.parseInt(ddlRentType.SelectedValue);
                            item.CONTRACT_AMOUNT = Convert.ToDecimal(txtAmount.Text);
                            item.FEE_PER_DAY = Convert.ToDecimal(txtFeePerDay.Text);
                            item.SERIAL_1 = txtSerial1.Text.Trim();
                            item.SERIAL_2 = txtSerial2.Text.Trim();
                            item.REFERENCE_PHONE = txtReferencePhone.Text.Trim();
                            item.SCHOOL_NAME = txtSchool.Text.Trim();
                            item.CLASS_NAME = txtClass.Text.Trim();
                            item.IMPLEMENTER = txtImplementer.Text.Trim();
                            item.BACK_TO_DOCUMENTS = txtBackDocument.Text.Trim();
                            item.DETAIL = txtItemDetail.Text.Trim();
                            item.SEARCH_TEXT = string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8}",
                                                        txtCustomerName.Text.Trim(),
                                                        txtBirthDay.Text.Trim(),
                                                        txtLicenseNumber.Text.Trim(),
                                                        txtRangeDate.Text.Trim(),
                                                        txtPermanentResidence.Text.Trim(),
                                                        txtCurrentResidence.Text.Trim(),
                                                        txtPhone.Text.Trim(),
                                                        item.CONTRACT_NO,
                                                        item.RENT_DATE.ToString("dd/MM/yyyy"));

                            item.UPDATED_BY = Session["username"].ToString();
                            item.UPDATED_DATE = DateTime.Now;
                            //Contract photo
                            SavePhoto(item);
                            //Update customer infor
                            var cusItem = db.Customers.FirstOrDefault(c => c.ID == item.CUSTOMER_ID);
                            if (cusItem != null)
                            {
                                cusItem.NAME = txtCustomerName.Text.Trim();
                                cusItem.BIRTH_DAY = DateTime.ParseExact(txtBirthDay.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                cusItem.LICENSE_NO = txtLicenseNumber.Text.Trim();
                                cusItem.LICENSE_RANGE_DATE = DateTime.ParseExact(txtRangeDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                cusItem.LICENSE_RANGE_PLACE = txtPlaceDate.Text.Trim();
                                cusItem.PHONE = txtPhone.Text.Trim();
                                cusItem.PERMANENT_RESIDENCE = txtPermanentResidence.Text.Trim();
                                cusItem.CURRENT_RESIDENCE = txtCurrentResidence.Text.Trim();
                            }

                            db.SaveChanges();
                        }

                        WriteLog(Constants.ACTION_UPDATE_CONTRACT, false);
                        ts.Complete();
                    }
                    Response.Redirect("FormContractManagement.aspx", false);
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
                lblMessage.CssClass = "text-center text-danger";
            }
        }

        private void SavePhoto(Contract item)
        {
            string filepath = Server.MapPath("\\upload");
            HttpFileCollection uploadedFiles = Request.Files;
            for (int i = 0; i < uploadedFiles.Count; i++)
            {
                HttpPostedFile userPostedFile = uploadedFiles[i];
                try
                {
                    if (userPostedFile.ContentLength > 0)
                    {
                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(userPostedFile.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(userPostedFile.ContentLength);
                        }
                        byte[] photo = ImageHelper.CreateThumbnail(fileData, 612, 612);
                        byte[] thumbnailPhoto = ImageHelper.CreateThumbnail(fileData, 128, 128);

                        string guid = Guid.NewGuid().ToString();
                        string datetime = DateTime.Now.ToString("yyyyMMddhhmmssfff");

                        string photoPath = filepath + "\\" + guid + "-" + datetime + "-" + (i + 1) + "_b" + ".png";
                        string thumbnailPhotoPath = filepath + "\\" + guid + "-" + datetime + "-" + (i + 1) + "_s" + ".png";

                        using (var fs = new BinaryWriter(new FileStream(photoPath, FileMode.Append, FileAccess.Write)))
                        {
                            fs.Write(photo);
                        }
                        using (var fs = new BinaryWriter(new FileStream(thumbnailPhotoPath, FileMode.Append, FileAccess.Write)))
                        {
                            fs.Write(thumbnailPhoto);
                        }

                        DeletePhoto(Convert.ToString(Helper.GetPropValue(item, "PHOTO_" + (i + 1))));
                        DeletePhoto(Convert.ToString(Helper.GetPropValue(item, "THUMBNAIL_PHOTO_" + (i + 1))));
                        Helper.SetPropValue(item, "PHOTO_" + (i + 1), photoPath.Replace(Server.MapPath("~"), string.Empty));
                        Helper.SetPropValue(item, "THUMBNAIL_PHOTO_" + (i + 1), thumbnailPhotoPath.Replace(Server.MapPath("~"), string.Empty));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void DeletePhoto(string path)
        {
            try
            {
                if (!string.IsNullOrEmpty(path) && File.Exists(Server.MapPath("~") + path))
                    File.Delete(Server.MapPath("~") + path);
            }
            catch { ; }
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
                using (var db = new RentBikeEntities())
                {
                    int contractID = Convert.ToInt32(id);
                    List<PayPeriod> payList = db.PayPeriods.Where(c => c.CONTRACT_ID == contractID).ToList();
                    decimal totalPaid = payList.Select(c => c.ACTUAL_PAY).DefaultIfEmpty(0).Sum();
                    for (int i = 0; i < payList.Count; i++)
                    {
                        bool bCheck = false;
                        if (totalPaid > 0)
                        {
                            if (totalPaid < payList[i].AMOUNT_PER_PERIOD)
                            {
                                payList[i].ACTUAL_PAY = totalPaid;
                                bCheck = true;
                            }
                            else
                                payList[i].ACTUAL_PAY = payList[i].AMOUNT_PER_PERIOD;
                        }

                        totalPaid -= payList[i].AMOUNT_PER_PERIOD;

                        if (totalPaid < 0 && !bCheck)
                            payList[i].ACTUAL_PAY = 0;
                    }

                    rptPayFeeSchedule.DataSource = payList;
                    rptPayFeeSchedule.DataBind();
                }
            }
        }

        protected void btnFinishContract_Click(object sender, EventArgs e)
        {
            string id = Request.QueryString["ID"];
            Response.Redirect(string.Format("FormCloseContract.aspx?ID={0}", id));
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("FormContractUpdate.aspx");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect("FormContractManagement.aspx?q=" + txtSearch.Text.Trim());
        }

        private decimal GetFeeRate(int STORE_ID)
        {
            decimal fee = 0;
            List<StoreFee> lst = new List<StoreFee>();
            using (var db = new RentBikeEntities())
            {
                var item = from s in db.StoreFees
                           where s.STORE_ID == STORE_ID
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
            lg.STORE = STORE_NAME;
            lg.LOG_ACTION = action;
            lg.LOG_DATE = DateTime.Now;
            lg.IS_CRASH = isCrashed;
            lg.LOG_MSG = string.Format("Tài khoản {0} {1}thực hiện {2} vào lúc {3}", lg.ACCOUNT, STORE_NAME, lg.LOG_ACTION, lg.LOG_DATE);
            lg.SEARCH_TEXT = lg.LOG_MSG;

            using (var db = new RentBikeEntities())
            {
                db.Logs.Add(lg);
                db.SaveChanges();
            }
        }


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