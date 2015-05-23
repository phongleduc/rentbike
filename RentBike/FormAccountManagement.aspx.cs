using RentBike.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RentBike
{
    public partial class FormAccountManagement : FormBase
    {
        int pageSize = 20;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData(0, string.Empty, 0);
            }
        }

        protected new void ddlStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData(STORE_ID, string.Empty, 0);
        }

        private void LoadData(int storeId, string strSearch, int page)
        {
            // LOAD PAGER
            using (var db = new RentBikeEntities())
            {
                IQueryable<Account> accList = db.Accounts;

                if (storeId != 0)
                    accList = accList.Where(c => c.STORE_ID == storeId);

                if(!string.IsNullOrEmpty(strSearch))
                    accList = accList.Where(c =>c.SEARCH_TEXT.ToLower().Contains(strSearch.ToLower()));

                int totalRecord = accList.Count();
                int totalPage = totalRecord % pageSize == 0 ? totalRecord / pageSize : totalRecord / pageSize + 1;

                IEnumerable<int> pageList = Enumerable.Range(1, totalPage);
                ddlPager.DataSource = pageList;
                ddlPager.DataBind();

                if (pageList.Count() > 0)
                    ddlPager.SelectedIndex = page;

                // LOAD DATA WITH PAGING
                int skip = page * pageSize;
                accList = accList.Skip(skip).Take(pageSize);

                rptAccount.DataSource = accList.ToList();
                rptAccount.DataBind();
            }
        }

        public string GetStoreName(int storeId)
        {
            return Session["store_name"] == null ? "Tất cả" : Session["store_name"].ToString();
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("FormAccountUpdate.aspx");
        }

        protected void ddlPager_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData(STORE_ID, txtSearch.Text.Trim(), Convert.ToInt32(ddlPager.SelectedValue) - 1);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData(STORE_ID, txtSearch.Text.Trim(), 0);
        }
    }
}