using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RentBike
{
    public partial class FormCommonListSetting : FormBase
    {
        int pageSize = 20;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!IsPostBack)
            {
                LoadData(string.Empty, 0);
            }
        }

        private void LoadData(string strSearch, int page)
        {
            // LOAD PAGER
            using (var db = new RentBikeEntities())
            {
                var dataList = (from c in db.RentTypes
                                select c);

                int totalRecord = dataList.Count();
                int totalPage = totalRecord % pageSize == 0 ? totalRecord / pageSize : totalRecord / pageSize + 1;

                IEnumerable<int> pageList = Enumerable.Range(1, totalPage);
                ddlPager.DataSource = pageList;
                ddlPager.DataBind();

                if (pageList.Count() > 0)
                    ddlPager.SelectedIndex = page;

                // LOAD DATA WITH PAGING
                int skip = page * pageSize;
                dataList = dataList.Skip(skip).Take(pageSize);
                rptRentType.DataSource = dataList.ToList();
                rptRentType.DataBind();
            }
        }

        protected void ddlPager_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData(string.Empty, Convert.ToInt32(ddlPager.SelectedValue) - 1);
        }
    }
}