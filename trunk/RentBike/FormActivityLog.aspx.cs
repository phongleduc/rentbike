using RentBike.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RentBike
{
    public partial class FormActivityLog : System.Web.UI.Page
    {
        private int pageSize = 10;
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
            if (!IsPostBack)
            {
                if (CheckAdminPermission())
                    LoadDataAdmin(0, string.Empty, 0);
                else
                    LoadData(string.Empty, 0);
            }
        }

        protected void ddlStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CheckAdminPermission())
                LoadDataAdmin(Helper.parseInt(drpStore.SelectedValue), string.Empty, 0);
            else
                LoadData(string.Empty, 0);
        }

        private void LoadData(string strSearch, int page)
        {
            // LOAD PAGER
            int totalRecord = 0;
            int storeid = Convert.ToInt16(Session["store_id"]);
            using (var db = new RentBikeEntities())
            {
                var count = (from c in db.Logs
                             where c.SEARCH_TEXT.Contains(strSearch) && c.IS_CRASH == false && c.STORE_ID == storeid
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
            List<Log> dataList;
            int skip = page * pageSize;
            using (var db = new RentBikeEntities())
            {
                var st = from s in db.Logs
                         where s.SEARCH_TEXT.Contains(strSearch) && s.IS_CRASH == false && s.STORE_ID == storeid
                         orderby s.ID descending
                         select s;

                dataList = st.Skip(skip).Take(pageSize).ToList();
            }

            rptActivity.DataSource = dataList;
            rptActivity.DataBind();
        }

        private void LoadDataAdmin(int storeId, string strSearch, int page)
        {
            // LOAD PAGER
            int totalRecord = 0;
            using (var db = new RentBikeEntities())
            {
                if (storeId != 0)
                {
                    var count = (from c in db.Logs
                                 where c.SEARCH_TEXT.Contains(strSearch) && c.IS_CRASH == false && (!string.IsNullOrEmpty(c.STORE) && c.STORE.Contains(drpStore.SelectedItem.Text))
                                 select c).Count();
                    totalRecord = Convert.ToInt16(count);
                }
                else
                {
                    var count = (from c in db.Logs
                                 where c.SEARCH_TEXT.Contains(strSearch) && c.IS_CRASH == false
                                 select c).Count();
                    totalRecord = Convert.ToInt16(count);
                }
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
            List<Log> dataList;
            int skip = page * pageSize;
            using (var db = new RentBikeEntities())
            {
                if (storeId != 0)
                {
                    var st = from s in db.Logs
                             where s.SEARCH_TEXT.Contains(strSearch) && s.IS_CRASH == false && (!string.IsNullOrEmpty(s.STORE) && s.STORE.Contains(drpStore.SelectedItem.Text))
                             orderby s.ID descending
                             select s;

                    dataList = st.Skip(skip).Take(pageSize).ToList();
                }
                else
                {
                    var st = from s in db.Logs
                             where s.SEARCH_TEXT.Contains(strSearch) && s.IS_CRASH == false
                             orderby s.ID descending
                             select s;

                    dataList = st.Skip(skip).Take(pageSize).ToList();
                }
            }

            rptActivity.DataSource = dataList;
            rptActivity.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (CheckAdminPermission())
                LoadDataAdmin(Helper.parseInt(drpStore.SelectedValue), txtSearch.Text.Trim(), 0);
            else
                LoadData(txtSearch.Text.Trim(), 0);
        }

        protected void ddlPager_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CheckAdminPermission())
                LoadDataAdmin(Helper.parseInt(drpStore.SelectedValue), txtSearch.Text.Trim(), Convert.ToInt16(ddlPager.SelectedValue) - 1);
            else
                LoadData(txtSearch.Text.Trim(), Convert.ToInt16(ddlPager.SelectedValue) - 1);
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
    }
}