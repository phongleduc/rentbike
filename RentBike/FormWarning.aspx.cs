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

                    DateTime nowDate = DateTime.Now;
                    if (!string.IsNullOrEmpty(date))
                    {
                        nowDate = Convert.ToDateTime(date);
                    }
                    string contactId = c.ID.ToString();
                    var tmpLstPeriod = lstPeriod.Where(s => s.CONTRACT_ID == c.ID);
                    if (tmpLstPeriod != null)
                    {
                        decimal paidAmount = tmpLstPeriod.Where(s => s.ACTUAL_PAY > 0).Select(s => s.ACTUAL_PAY).DefaultIfEmpty().Sum();
                        int paidNumberOfFee = 0;
                        bool paidFull = false;
                        foreach (PayPeriod pp in tmpLstPeriod)
                        {
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

            rptWarning.DataSource = dataList.OrderByDescending(c => c.DAY_DONE);
            rptWarning.DataBind();
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
    }

    public partial class CONTRACT_FULL_VW
    {
        public int OVER_DATE { get; set; }
        public int DAY_DONE { get; set; }
        public DateTime PAY_DATE { get; set; }
        public int PAYED_TIME { get; set; }
        public int PERIOD_ID { get; set; }
        public string CSS_CLASS { get; set; }
    }
}