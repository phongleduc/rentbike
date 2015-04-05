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
        int pageSize = 10;
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
                //LoadData(string.Empty, 0);
                if (CheckAdminPermission())
                    LoadDataAdmin(0, string.Empty, 0);
                else
                    LoadData(string.Empty, 0);

                drpRentType.Items.Clear();
                drpRentType.Items.Add(new ListItem("Tất cả", ""));
                using (var db = new RentBikeEntities())
                {
                    foreach (RentType rentType in db.RentTypes.ToList())
                    {
                        drpRentType.Items.Add(new ListItem(rentType.NAME, rentType.ID.ToString()));
                    }
                }
            }
        }

        protected void ddlStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList drpStore = sender as DropDownList;
            if (CheckAdminPermission())
                LoadDataAdmin(Helper.parseInt(drpStore.SelectedValue), string.Empty, 0);
            else
                LoadData(string.Empty, 0);

        }
        private void LoadData(string strSearch, int rentType)
        {
            int storeid = Helper.parseInt(Session["store_id"].ToString());
            List<CONTRACT_HISTORY_FULL_VW> dataList;
            using (var db = new RentBikeEntities())
            {
                dataList = (from s in db.CONTRACT_HISTORY_FULL_VW
                            where s.SEARCH_TEXT.Contains(strSearch) && s.STORE_ID == storeid
                            select s).OrderByDescending(c => c.CLOSE_CONTRACT_DATE).ToList();

                if (dataList.Any())
                {
                    if (rentType != 0)
                    {
                        dataList = dataList.Where(c => c.RENT_TYPE_ID == rentType).ToList();
                    }
                    rptContractHistory.DataSource = dataList;
                    rptContractHistory.DataBind();
                }
            }


        }

        private void LoadDataAdmin(int storeId, string strSearch, int rentType)
        {
            List<CONTRACT_HISTORY_FULL_VW> dataList;
            using (var db = new RentBikeEntities())
            {
                dataList = (from s in db.CONTRACT_HISTORY_FULL_VW
                            where s.SEARCH_TEXT.Contains(strSearch)
                            select s).OrderByDescending(c => c.CLOSE_CONTRACT_DATE).ToList();
                if (dataList.Any())
                {
                    if (storeId != 0)
                    {
                        dataList = dataList.Where(c => c.STORE_ID == storeId).ToList();
                    }

                    if (rentType != 0)
                    {
                        dataList = dataList.Where(c => c.RENT_TYPE_ID == rentType).ToList();
                    }
                    rptContractHistory.DataSource = dataList;
                    rptContractHistory.DataBind();
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text.Trim(), 0);
        }

        protected void drpRentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CheckAdminPermission())
                LoadDataAdmin(Helper.parseInt(drpStore.SelectedValue), txtSearch.Text.Trim(), Helper.parseInt(drpRentType.SelectedValue));
            else
                LoadData(txtSearch.Text.Trim(), Helper.parseInt(drpRentType.SelectedValue));
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