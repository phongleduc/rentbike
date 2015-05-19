using RentBike.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RentBike
{
    public partial class ContractHistoryManagement : System.Web.UI.Page
    {
        int pageSize = 20;
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
                LoadData(string.Empty, 0);
            }
        }

        protected void ddlStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text, 0);
        }

        private void LoadData(string strSearch, int page)
        {
            // LOAD PAGER
            int totalRecord = 0;
            int storeid = 0;  

            // LOAD DATA WITH PAGING
            List<CONTRACT_HISTORY_FULL_VW> dataList;
            int skip = page * pageSize;
            using (var db = new RentBikeEntities())
            {
                var st = from s in db.CONTRACT_HISTORY_FULL_VW
                         select s;

                if (!string.IsNullOrEmpty(strSearch))
                {
                    st = st.Where(c => c.SEARCH_TEXT.ToLower().Contains(strSearch.ToLower()));
                }
                if (CheckAdminPermission())
                {
                    storeid = Helper.parseInt(drpStore.SelectedValue);
                }
                else
                {
                    storeid = Convert.ToInt32(Session["store_id"]);
                }
                if(storeid != 0)
                    st = st.Where(c => c.STORE_ID == storeid);

                dataList = st.OrderByDescending(c =>c.CREATED_DATE).ToList();
                totalRecord = dataList.Count();

                dataList = dataList.Skip(skip).Take(pageSize).ToList();
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
            }

            rptContractHistory.DataSource = dataList;
            rptContractHistory.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text.Trim(), 0);
        }

        protected void ddlPager_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text.Trim(), Convert.ToInt32(ddlPager.SelectedValue) - 1);
        }

        public bool CheckAdminPermission()
        {
            string acc = Convert.ToString(Session["username"]);
            using (var db = new RentBikeEntities())
            {
                var item = db.Accounts.FirstOrDefault(s =>s.ACC == acc);

                if (item.PERMISSION_ID == 1)
                    return true;
                return false;
            }
        }
    }
}