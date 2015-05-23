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
    public partial class FormWarning : FormBase
    {
        public string SearchDate;
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!IsPostBack)
            {
                LoadData(string.Empty, string.Empty);
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

        private void LoadData(string date, string strSearch)
        {
            List<CONTRACT_FULL_VW> dataList = CommonList.GetWarningData(date, strSearch, STORE_ID);
            rptWarning.DataSource = dataList;
            rptWarning.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData(txtDate.Text, txtSearch.Text.Trim());
        }

        protected override void ddlStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData(txtDate.Text, txtSearch.Text.Trim());
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

        protected void lnkExportExcel_Click(object sender, EventArgs e)
        {
            List<CONTRACT_FULL_VW> dataList = CommonList.GetWarningData(txtDate.Text, txtSearch.Text, STORE_ID);
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
                        //ert = worksheet.Cells[index, 2].RichText.Add("\n(" + (contract.BIRTH_DAY == null ? "" : contract.BIRTH_DAY.Value.ToString("dd/MM/yyyy")) + ")");
                        //ert.Bold = false;

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
                    string fileName = string.Format("DSGP {0}.{1}", date.ToString("dd-MM-yyyy"), "xlsx");
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