using RentBike.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RentBike
{
    public partial class FormSearch : System.Web.UI.Page
    {
        int pageSize = 30;
        private DropDownList drpStore;

        //raise button click events on content page for the buttons on master page
        protected void Page_Init(object sender, EventArgs e)
        {
            drpStore = this.Master.FindControl("ddlStore") as DropDownList;
            drpStore.SelectedIndexChanged += new EventHandler(ddlStore_SelectedIndexChanged);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["store_id"] == null)
            {
                Response.Redirect("FormLogin.aspx");
            }

            string searchText = Request.QueryString["q"];
            if (!IsPostBack)
            {
                txtSearch.Text = searchText;
            }

            if (!string.IsNullOrEmpty(txtSearch.Text))
                searchText = txtSearch.Text;

            int page = Helper.parseInt(ddlPager.SelectedValue);
            if (page > 0) page -= 1;
            LoadData(searchText, page);
            ddlPager.Visible = true;
        }

        protected void ddlStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            //DropDownList drpStore = sender as DropDownList;
            //if (CheckAdminPermission())
            //    LoadDataAdmin(Helper.parseInt(drpStore.SelectedValue), string.Empty, 0);
            //else
            //    LoadData(string.Empty, 0);

        }

        private void LoadData(string strSearch, int page)
        {
            // LOAD PAGER
            using (var db = new RentBikeEntities())
            {
                List<CONTRACT_FULL_VW> dataList = db.CONTRACT_FULL_VW.Where(c => c.SEARCH_TEXT.Contains(strSearch)).OrderBy(c => c.ID).ToList();

                if (drpStore.Enabled == true && Helper.parseInt(drpStore.SelectedValue) != 0)
                {
                    dataList = dataList.Where(c => c.STORE_ID == Helper.parseInt(drpStore.SelectedValue)).ToList();
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

                rptCustomer.DataSource = dataList;
                rptCustomer.DataBind();
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