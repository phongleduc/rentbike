using RentBike.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RentBike
{
    public partial class FormSearch : FormBase
    {
        int pageSize = 30;
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            string searchText = Request.QueryString["q"];
            if (!IsPostBack)
            {
                txtSearch.Text = searchText;
                LoadData(txtSearch.Text, 0);
            }
        }

        protected override void ddlStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text.Trim(), 0);
        }

        private void LoadData(string strSearch, int page)
        {
            // LOAD PAGER
            using (var db = new RentBikeEntities())
            {
                List<CONTRACT_FULL_VW> dataList = db.CONTRACT_FULL_VW.Where(c =>c.ACTIVE == true && ( c.SEARCH_TEXT.ToLower().Contains(strSearch.ToLower()) 
                    || c.CUSTOMER_NAME.ToLower().Contains(strSearch.ToLower())))
                    .OrderByDescending(c => c.ID)
                    .OrderByDescending(c => c.CONTRACT_STATUS)
                    .ToList();

                if (IS_ADMIN && STORE_ID != 0)
                {
                    dataList = dataList.Where(c =>c.STORE_ID == STORE_ID).ToList();
                }

                var totalRecord = dataList.Count;
                litSearchResult.Text = string.Format("Có {0} kết quả được tìm thấy.", totalRecord);

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

                int skip = page * pageSize;
                dataList = dataList.Skip(skip).Take(pageSize).ToList();

                rptSearch.DataSource = dataList;
                rptSearch.DataBind();
            }
        }

        public bool IsBadContract(int contractId)
        {
            using (var db = new RentBikeEntities())
            {
                return CommonList.IsBadContract(db, contractId);
            }
        }
        public string GetURL(string id, string storeId)
        {
            return string.Format("FormContractUpdate.aspx?ID={0}&sID={1}", id, storeId);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text.Trim(), 0);
        }

        protected void ddlPager_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text.Trim(), Helper.parseInt(ddlPager.SelectedValue) - 1);
        }
    }
}