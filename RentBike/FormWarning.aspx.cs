using OfficeOpenXml;
using OfficeOpenXml.Style;
using RentBike.Common;
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
    public partial class FormWarning : System.Web.UI.Page
    {
        int pageSize = 20;
        int storeId = 0;
        public string SearchDate { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["store_id"] == null)
            {
                Response.Redirect("FormLogin.aspx");
            }
            if (CheckAdminPermission())
            {
                DropDownList ddlStore = Master.FindControl("ddlStore") as DropDownList;
                if (ddlStore != null && !string.IsNullOrEmpty(ddlStore.SelectedValue))
                {
                    storeId = Helper.parseInt(ddlStore.SelectedValue);
                }
            }
            else
                storeId = Helper.parseInt(Session["store_id"].ToString());

            if (!IsPostBack)
            {
                LoadData(string.Empty, string.Empty, 0, storeId);
            }
            else
            {
                //if (!string.IsNullOrEmpty(hfPager.Value))
                //{
                //    LoadData(txtDate.Text, txtSearch.Text, Convert.ToInt32(ddlPager.SelectedValue) - 1, storeId);
                //}
                //else
                //{
                LoadData(txtDate.Text, txtSearch.Text, 0, storeId);
                //}
            }
            if (!string.IsNullOrEmpty(txtDate.Text))
            {
                SearchDate = Convert.ToDateTime(txtDate.Text).ToString("dd/MM/yyyy");
            }
            else
            {
                SearchDate = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
        }

        private void LoadData(string date, string strSearch, int page, int storeId)
        {
            List<CONTRACT_FULL_VW> dataList = GetWarningData(date, strSearch, storeId);
            rptWarning.DataSource = dataList;
            rptWarning.DataBind();
        }

        private List<CONTRACT_FULL_VW> GetWarningData(string date, string strSearch, int storeId)
        {
            //int totalRecord = 0;
            List<CONTRACT_FULL_VW> dataList = new List<CONTRACT_FULL_VW>();
            using (var db = new RentBikeEntities())
            {
                var st = db.CONTRACT_FULL_VW.Where(c => c.CONTRACT_STATUS == true);
                if (storeId != 0)
                {
                    st = st.Where(c => c.STORE_ID == storeId);
                }
                if (!string.IsNullOrEmpty(strSearch))
                {
                    st = st.Where(c => c.SEARCH_TEXT.Contains(strSearch));
                }
                st = st.OrderByDescending(c => c.ID);

                string searchDate = string.Empty;
                if (!string.IsNullOrEmpty(date))
                {
                    searchDate = Convert.ToDateTime(date).ToString("yyyyMMdd");
                }

                var lstPeriod = db.PayPeriods.Where(s => s.STATUS == true).ToList();
                foreach (CONTRACT_FULL_VW c in st)
                {
                    var inOutList = db.InOuts.Where(s => s.CONTRACT_ID == c.ID).ToList();

                    c.PAYED_TIME = 0;
                    c.PAY_DATE = c.RENT_DATE;
                    c.DAY_DONE = DateTime.Now.Subtract(c.PAY_DATE).Days;

                    DateTime nowDate = DateTime.Today;
                    if (!string.IsNullOrEmpty(date))
                    {
                        nowDate = Convert.ToDateTime(date);
                    }
                    string contactId = c.ID.ToString();
                    var tmpLstPeriod = lstPeriod.Where(s => s.CONTRACT_ID == c.ID).ToList();
                    if (tmpLstPeriod != null)
                    {
                        decimal paidAmount = tmpLstPeriod.Where(s => s.ACTUAL_PAY > 0).Select(s => s.ACTUAL_PAY).DefaultIfEmpty().Sum();
                        int paidNumberOfFee = 0;
                        bool paidFull = false;
                        foreach (PayPeriod pp in tmpLstPeriod)
                        {
                            c.PERIOD_MESSAGE = GetPeriodMessage(tmpLstPeriod, nowDate);
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
                                c.OVER_DATE = nowDate.Subtract(pp.PAY_DATE).Days;
                                if (paidAmount < 0 && tmpLstPeriod.Any(s => s.PAY_DATE == pp.PAY_DATE.AddDays(9)))
                                {
                                    c.OVER_DATE = nowDate.Subtract(pp.PAY_DATE).Days + 2;
                                }
                                c.PAY_DATE = pp.PAY_DATE;
                                c.PERIOD_ID = pp.ID;
                                if (paidAmount == 0 || c.OVER_DATE <= 0)
                                    paidFull = true;
                                break;
                            }
                        }
                        c.PAYED_TIME = paidNumberOfFee;
                        c.DAY_DONE = DateTime.Now.Subtract(c.RENT_DATE).Days + 1;

                        if (string.IsNullOrEmpty(date) || DateTime.Now.Subtract(Convert.ToDateTime(date)).Days <= 0)
                        {
                            if (paidFull && c.OVER_DATE <= 0)
                            {
                                c.CSS_CLASS = "background-green";
                            }
                            else if (c.OVER_DATE > 10)
                            {
                                c.CSS_CLASS = "background-red";
                            }
                        }
                        else
                        {
                            if (c.OVER_DATE <= 0)
                            {
                                var inout = inOutList.Where(s => s.PERIOD_DATE.ToString("yyyyMMdd").Equals(nowDate.ToString("yyyyMMdd"))).OrderByDescending(s => s.INOUT_DATE).FirstOrDefault();
                                if (inout != null && inout.INOUT_DATE.HasValue && inout.INOUT_DATE.Value.Subtract(nowDate).Days > 0)
                                {
                                    c.CSS_CLASS = "background-amber";
                                }
                                else
                                {
                                    c.CSS_CLASS = "background-green";
                                }
                            }
                            else
                            {
                                c.CSS_CLASS = "background-red";
                            }
                        }

                        if (c.FEE_PER_DAY == 0)
                            c.CSS_CLASS = "background-green";

                        c.RENT_TYPE_NAME = ReBuildRentTypeName(c);
                        if (!string.IsNullOrEmpty(searchDate))
                        {
                            if (tmpLstPeriod.Any(s => s.PAY_DATE.ToString("yyyyMMdd").Equals(searchDate)))
                            {
                                c.FEE_PER_DAY = tmpLstPeriod.FirstOrDefault(s => s.PAY_DATE.ToString("yyyyMMdd").Equals(searchDate)).AMOUNT_PER_PERIOD;
                                dataList.Add(c);
                            }
                        }
                        else if (tmpLstPeriod.Any(s => s.PAY_DATE.ToString("yyyyMMdd").Equals(DateTime.Now.ToString("yyyyMMdd"))))
                        {
                            c.FEE_PER_DAY = tmpLstPeriod.FirstOrDefault(s => s.PAY_DATE.ToString("yyyyMMdd").Equals(DateTime.Now.ToString("yyyyMMdd"))).AMOUNT_PER_PERIOD;
                            dataList.Add(c);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    dataList = dataList.Where(s => s.SEARCH_TEXT.Contains(txtSearch.Text)).ToList();
                }
            }
            return dataList.OrderBy(c => c.DAY_DONE).ToList();
        }

        private string ReBuildRentTypeName(CONTRACT_FULL_VW con)
        {
            switch (con.RENT_TYPE_ID)
            { 
                case 1:
                    return con.RENT_TYPE_NAME = "Thuê xe";
                case 2:
                    return con.RENT_TYPE_NAME = "TBVP";
                case 3:
                    return con.RENT_TYPE_NAME = "Khác";
                default:
                    return string.Empty;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            //LoadData(txtSearch.Text.Trim(), 0);
        }

        protected void ddlPager_SelectedIndexChanged(object sender, EventArgs e)
        {
            //LoadData(txtSearch.Text.Trim(), Convert.ToInt32(ddlPager.SelectedValue) - 1);
        }

        protected void rptWarning_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            //if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            //{

            //    if (DateTime.Now.Subtract(((CONTRACT_FULL_VW)e.Item.DataItem).END_DATE).Days < 10)
            //    {
            //        HtmlTableRow tr = (HtmlTableRow)e.Item.FindControl(string.Format("HtmlTableRow{0}", e.Item.ItemIndex));
            //        int overDays = Convert.ToInt32(((HiddenField)e.Item.FindControl("hdfOverDay")).Value);
            //        if (overDays < 10)
            //        {
            //            tr.Style.Add(HtmlTextWriterStyle.BackgroundColor, "Yellow");
            //        }
            //    }
            //}
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
        private string GetPeriodMessage(List<PayPeriod> listPay, DateTime searchDate)
        {
            var index = listPay.FindIndex(c => c.PAY_DATE == searchDate) + 1;
            var periodNum = index;
            var monthNum = 1;

            periodNum = periodNum % 3 == 0 ? 3 : periodNum % 3;

            for (int i = 1; i <= index; i++)
            {
                if (i % 3 == 0)
                    monthNum += 1;
            }

            if (index <= 3)
            {
                return "Kỳ " + periodNum;
            }
            else
            {
                if (periodNum % 3 == 1)
                    return "Hết hạn T" + (monthNum - 1);
                else
                {
                    if (periodNum % 3 == 0)
                        return "Kỳ " + periodNum + " - T" + (monthNum - 1);
                    else
                        return "Kỳ " + periodNum + " - T" + monthNum;
                }
            }
        }

        protected void lnkExportExcel_Click(object sender, EventArgs e)
        {
            List<CONTRACT_FULL_VW> dataList = GetWarningData(txtDate.Text, txtSearch.Text, storeId);
            if (dataList.Any())
            {
                using (ExcelPackage package = new ExcelPackage())
                {
                    // add a new worksheet to the empty workbook
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("DSGP");
                    worksheet.View.ZoomScale = 90;
                    worksheet.Cells.Style.Font.Size = 12;
                    worksheet.Cells.Style.Font.Name = "Times New Roman";

                    worksheet.Cells[1, 1, 1, 8].Merge = true;
                    worksheet.Cells[1, 1, 1, 8].Value = "Danh Sách Gọi Khách " + dataList[0].STORE_NAME;
                    worksheet.Row(1).Height = 20;
                    worksheet.Cells[1, 1, 1, 8].Style.Font.Bold = true;
                    worksheet.Cells[1, 1, 1, 8].Style.Font.Size = 14;
                    worksheet.Cells[1, 1, 1, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.Cells[2, 1, 2, 8].Merge = true;
                    worksheet.Cells[2, 1, 2, 8].Value = "(" + SearchDate + ")";
                    worksheet.Row(2).Height = 35;
                    worksheet.Cells[2, 1, 2, 8].Style.Font.Bold = true;
                    worksheet.Cells[2, 1, 2, 8].Style.Font.Size = 18;
                    worksheet.Cells[2, 1, 2, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.Cells[3, 1, 3, 8].Merge = true;
                    worksheet.Cells[3, 1, 3, 8].Value = "";
                    worksheet.Row(2).Height = 20;

                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 35;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 25;
                    worksheet.Column(6).Width = 30;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(7).Style.WrapText = true;

                    worksheet.Cells[4, 1].Value = "#";
                    worksheet.Cells[4, 2].Value = "Tên khách hàng";
                    worksheet.Cells[4, 3].Value = "Loại hình thuê";
                    worksheet.Cells[4, 4].Value = "Số ĐT khách hàng";
                    worksheet.Cells[4, 5].Value = "Giá trị HĐ/Phí";
                    worksheet.Cells[4, 6].Value = "Ghi chú";
                    worksheet.Cells[4, 7].Value = "Số lần đóng phí";
                    worksheet.Cells[4, 8].Value = "Thông báo";
                    worksheet.Cells[4, 1, 4, 8].Style.Font.Bold = true;
                    worksheet.Cells[4, 1, 4, 8].Style.Font.Size = 13;
                    worksheet.Row(4).Height = 25;

                    int no = 1;
                    int index = 5;
                    foreach (var contract in dataList)
                    {
                        worksheet.Cells[index, 1].Value = no;

                        worksheet.Cells[index, 2].Style.WrapText = true;
                        worksheet.Cells[index, 2].IsRichText = true;
                        ExcelRichText ert = worksheet.Cells[index, 2].RichText.Add(contract.CUSTOMER_NAME);
                        ert.Bold = true;
                        ert = worksheet.Cells[index, 2].RichText.Add("\n(" + (contract.BIRTH_DAY == null ? "" : contract.BIRTH_DAY.Value.ToString("dd/MM/yyyy")) + ")");
                        ert.Bold = false;

                        worksheet.Cells[index, 3].Value = contract.RENT_TYPE_NAME;
                        worksheet.Cells[index, 4].Value = contract.PHONE;
                        worksheet.Cells[index, 5].Value = string.Format("{0:0,0}", contract.FEE_PER_DAY);
                        worksheet.Cells[index, 6].Value = contract.NOTE;
                        worksheet.Cells[index, 7].Value = contract.PAYED_TIME + " lần";
                        worksheet.Cells[index, 8].Value = contract.PERIOD_MESSAGE;

                        no += 1;
                        index += 1;
                    }

                    worksheet.Cells[4, 1, index, 8].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[4, 1, index, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[4, 1, index, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[4, 1, index, 8].Style.Border.Left.Style = ExcelBorderStyle.Thin;

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

                    DateTime date = DateTime.Now;
                    if (!string.IsNullOrEmpty(txtDate.Text))
                    {
                        date = Convert.ToDateTime(txtDate.Text);
                    }
                    string fileName = string.Format("DSGP {0}.{1}", date.ToString("dd-MM-yyyy"), ".xlsx");
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment; filename=\"" + fileName + "\"");
                    Response.BinaryWrite(package.GetAsByteArray());
                    Response.Flush();
                    Response.Close();
                    Response.End();
                }
            }
        }
    }

    public partial class CONTRACT_FULL_VW
    {
        public int OVER_DATE { get; set; }
        public int DAY_DONE { get; set; }
        public DateTime PAY_DATE { get; set; }
        public int PAYED_TIME { get; set; }
        public int PERIOD_ID { get; set; }
        public string PERIOD_MESSAGE { get; set; }
        public string CSS_CLASS { get; set; }
    }
}