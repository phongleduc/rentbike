using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RentBike.Common;

namespace RentBike
{
    public partial class FormShopManagement : FormBase
    {
        int pageSize = 20;
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!IsPostBack)
            {
                LoadData(string.Empty, 0, 0);
            }
        }

        protected override void ddlStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text.Trim(), STORE_ID, Convert.ToInt32(ddlPager.SelectedValue) - 1);
        }

        private void CheckPermission()
        {
            if (IS_ADMIN)
            {
                foreach (RepeaterItem itm in rptStore.Items)
                {
                    HiddenField hdf = (HiddenField)(itm.FindControl("hdfStore_id"));
                    HyperLink hpl = (HyperLink)(itm.FindControl("hplStoreDetail"));
                    if (hdf.Value.ToString() == STORE_ID.ToString())
                    {
                        hpl.Visible = true;
                    }
                    else
                    { hpl.Visible = false; }
                }
            }
            else
            {
                btnNew.Enabled = false;
            }
        }

        private void LoadData(string strSearch, int storeId, int page)
        {
            // LOAD PAGER
            using (var db = new RentBikeEntities())
            {
                IQueryable<STORE_FULL_VW> dataList = db.STORE_FULL_VW;

                if (storeId != 0)
                    dataList = dataList.Where(c => c.ID == storeId);

                if (!string.IsNullOrEmpty(strSearch))
                    dataList = dataList.Where(c => c.ID == storeId);

                var result = dataList.ToList();
                int totalRecord = result.Count;
                int totalPage = totalRecord % pageSize == 0 ? totalRecord / pageSize : totalRecord / pageSize + 1;
                IEnumerable<int> pageList = Enumerable.Range(1, totalPage);

                ddlPager.DataSource = pageList;
                ddlPager.DataBind();

                if (pageList.Count() > 0)
                    ddlPager.SelectedIndex = page;

                // LOAD DATA WITH PAGING
                int skip = page * pageSize;
                result = result.Skip(skip).Take(pageSize).ToList();

                rptStore.DataSource = result;
                rptStore.DataBind();
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("FormStoreDetail.aspx");
        }

        protected void rptStore_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            Response.Redirect("FormStoreDetail.aspx");
        }

        protected void ddlPager_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text.Trim(), 0, Convert.ToInt32(ddlPager.SelectedValue) - 1);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text.Trim(), 0, 0);
        }
    }
}