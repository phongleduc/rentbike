﻿using RentBike.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RentBike
{
    public partial class FormActivityLog : FormBase
    {
        private int pageSize = 10;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!IsPostBack)
            {
                LoadData(string.Empty, 0);
            }
        }

        protected override void ddlStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData(string.Empty, 0);
        }

        private void LoadData(string strSearch, int page)
        {
            using (var db = new RentBikeEntities())
            {
                IQueryable<Log> dataList = db.Logs.Where(c => c.IS_CRASH == false).OrderByDescending(c => c.ID);

                if (STORE_ID != 0)
                {
                    var store = db.Stores.FirstOrDefault(c =>c.ID == STORE_ID);
                    dataList = dataList.Where(c => c.STORE == store.NAME);  
                }

                if (!string.IsNullOrEmpty(strSearch))
                    dataList = dataList.Where(c => c.SEARCH_TEXT.ToLower().Contains(strSearch.ToLower()));

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

                rptActivity.DataSource = dataList.ToList();
                rptActivity.DataBind();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text.Trim(), 0);
        }

        protected void ddlPager_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text.Trim(), Convert.ToInt32(ddlPager.SelectedValue) - 1);
        }
    }
}