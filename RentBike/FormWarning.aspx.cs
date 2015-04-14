using RentBike.Common;
using System;
using System.Collections.Generic;
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
        }

        private void LoadData(string date, string strSearch, int page, int storeId)
        {
            //int totalRecord = 0;
            List<CONTRACT_FULL_VW> dataList = new List<CONTRACT_FULL_VW>();
            using (var db = new RentBikeEntities())
            {
                var st = db.CONTRACT_FULL_VW.Where(c => c.SEARCH_TEXT.Contains(strSearch) && c.CONTRACT_STATUS == true);
                if (storeId != 0)
                {
                    st = st.Where(c => c.STORE_ID == storeId);
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
                    c.PAYED_TIME = 0;
                    c.PAY_DATE = c.RENT_DATE;
                    c.OVER_DATE = DateTime.Now.Subtract(c.PAY_DATE).Days;
                    c.MUST_PAY_IN_TODAY = false;
                    string contactId = c.ID.ToString();
                    var tmpLstPeriod = lstPeriod.Where(s => s.CONTRACT_ID == c.ID).OrderByDescending(s => s.PAY_DATE).ToList();
                    if (tmpLstPeriod != null)
                    {
                        decimal paidAmount = tmpLstPeriod.Where(s => s.ACTUAL_PAY > 0).Select(s => s.ACTUAL_PAY).DefaultIfEmpty().Sum();
                        int paidNumberOfFee = 0;
                        foreach (PayPeriod pp in tmpLstPeriod)
                        {
                            if (paidAmount <= 0)
                                break;

                            paidAmount -= pp.AMOUNT_PER_PERIOD;
                            paidNumberOfFee += 1;
                        }
                        c.PAYED_TIME = paidNumberOfFee;

                        PayPeriod pp1 = tmpLstPeriod.Where(s => s.PAY_DATE >= DateTime.Now).FirstOrDefault();
                        if (pp1 != null)
                            c.FEE_PER_DAY = pp1.AMOUNT_PER_PERIOD / 10;

                        var payItemToday = tmpLstPeriod.FirstOrDefault(s => s.PAY_DATE.ToString("yyyyMMdd").Equals(searchDate) && s.ACTUAL_PAY < s.AMOUNT_PER_PERIOD);
                        if (payItemToday != null)
                        {
                            c.MUST_PAY_IN_TODAY = true;
                        }

                        var lstPeriodPayed = tmpLstPeriod.Any() ? tmpLstPeriod.Where(s => s.ACTUAL_PAY >= s.AMOUNT_PER_PERIOD) : null;
                        if (lstPeriodPayed != null && lstPeriodPayed.Any())
                        {
                            c.PAY_DATE = lstPeriodPayed.LastOrDefault().PAY_DATE;
                            c.OVER_DATE = DateTime.Now.Subtract(c.PAY_DATE.AddDays(10)).Days;
                            //c.PAYED_TIME = lstPeriodPayed.Count();
                        }
                        dataList.Add(c);
                    }
                }
                if (!string.IsNullOrEmpty(searchDate))
                {
                    dataList = dataList.Where(s => s.MUST_PAY_IN_TODAY == true).ToList();
                }

                //totalRecord = Convert.ToInt32(dataList.Count());
                //int totalPage = totalRecord % pageSize == 0 ? totalRecord / pageSize : totalRecord / pageSize + 1;
                //List<int> pageList = new List<int>();
                //for (int i = 1; i <= totalPage; i++)
                //{
                //    pageList.Add(i);
                //}

                //ddlPager.DataSource = pageList;
                //ddlPager.DataBind();
                //if (pageList.Count > 0)
                //{
                //    ddlPager.SelectedIndex = page;
                //}

                //int skip = page * pageSize;
                //dataList = dataList.OrderByDescending(s => s.OVER_DATE).Skip(skip).Take(pageSize).ToList();
            }

            rptWarning.DataSource = dataList.OrderByDescending(s => s.OVER_DATE);
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
        public DateTime PAY_DATE { get; set; }
        public int PAYED_TIME { get; set; }
        public bool MUST_PAY_IN_TODAY { get; set; }
    }
}