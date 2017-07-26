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
        private List<Store> listStore = new List<Store>();
        private List<AccountPermission> listPermission = new List<AccountPermission>();

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            if (PERMISSION == ROLE.ADMIN)
            {
                LoadData(0, string.Empty, 0);
            }
            else
            {
                LoadData(STORE_ID, string.Empty, 0);
            }
        }

        protected override void ddlStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData(STORE_ID, string.Empty, 0);
        }

        private void LoadData(int storeId, string strSearch, int page)
        {
            // LOAD PAGER
            using (var db = new RentBikeEntities())
            {
                listStore = db.Stores.ToList();
                listPermission = db.AccountPermissions.ToList();

                IQueryable<Account> accList = db.Accounts;
                
                if (storeId != 0)
                    accList = accList.Where(c => c.STORE_ID == storeId);

                if(!string.IsNullOrEmpty(strSearch))
                    accList = accList.Where(c =>c.SEARCH_TEXT.ToLower().Contains(strSearch.ToLower()));

                var result = accList.ToList();
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

                rptAccount.DataSource = result;
                rptAccount.DataBind();
            }
        }

        public string GetStoreName(int storeId)
        {
            var item = listStore.FirstOrDefault(c => c.ID == storeId);
            if (item != null)
                return item.NAME;

            return string.Empty;
        }

        public string GetPermissionName(int permission)
        {
            var item = listPermission.FirstOrDefault(c => c.ID == permission);
            if (item != null)
                return item.NAME;

            return string.Empty;
        }

        public string GetStatus(bool status)
        {
            return status == true 
                ? "Hoạt động" : "Không hoạt động";
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

        protected void rptAccount_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Delete" && !string.IsNullOrEmpty(e.CommandArgument.ToString()))
                {
                    var id = Convert.ToInt32(e.CommandArgument.ToString());
                    using (var db = new RentBikeEntities())
                    {
                        var account = db.Accounts.FirstOrDefault(c => c.ID == id);
                        if (account != null)
                        {
                            db.Accounts.Remove(account);
                            db.SaveChanges();
                        }
                        LoadData();
                        ViewState["DeleteSuccess"] = "Xóa tài khoản thành công";
                    }
                }
            }
            catch(Exception ex)
            {
                ViewState["DeleteFail"] = ex.Message;
            }
        }
    }
}