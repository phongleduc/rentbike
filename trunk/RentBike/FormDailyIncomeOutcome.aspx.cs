﻿using RentBike.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RentBike
{
    public partial class DailyIncomeOutcome : System.Web.UI.Page
    {
        int pageSize = 10;
        int storeId = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["store_id"] == null)
            {
                Response.Redirect("FormLogin.aspx");
            }
            if (CheckAdminPermission())
            {
                DropDownList ddlStore = Master.FindControl("ddlStore") as DropDownList;
                if (ddlStore != null && !string.IsNullOrEmpty(ddlStore.SelectedValue))
                {
                    storeId = Helper.parseInt(ddlStore.SelectedValue);
                }
            }
            else
                storeId = Helper.parseInt(Session["store_id"].ToString());

            if (!IsPostBack)
            {
                LoadData(string.Empty, string.Empty, 0, storeId);
            }
            else
            {
                if (!string.IsNullOrEmpty(hfPager.Value))
                {
                    LoadData(txtSearch.Text, txtDate.Text, Convert.ToInt16(ddlPager.SelectedValue) - 1, storeId);
                }
                else
                {
                    LoadData(txtSearch.Text, txtDate.Text, 0, storeId);
                }
            }
        }

        private void LoadData(string strSearch, string date, int page, int storeId)
        {
            int totalRecord = 0;
            // LOAD DATA WITH PAGING
            List<INOUT_FULL_VW> dataList;
            int skip = page * pageSize;
            using (var db = new RentBikeEntities())
            {
                if (storeId != 0)
                {
                    dataList = db.INOUT_FULL_VW.Where(s => s.SEARCH_TEXT.Contains(strSearch) && s.STORE_ID == storeId).OrderByDescending(s => s.ID).ToList();
                    if (!string.IsNullOrEmpty(date))
                    {
                        date = Convert.ToDateTime(date).ToString("yyyyMMdd");
                        dataList = dataList.Where(s => s.INOUT_DATE.HasValue ? s.INOUT_DATE.Value.ToString("yyyyMMdd").Equals(date) : false).ToList();

                    }
                    totalRecord = Convert.ToInt16(dataList.Count());
                    dataList = dataList.Skip(skip).Take(pageSize).ToList();
                }
                else
                {
                    dataList = db.INOUT_FULL_VW.Where(s => s.SEARCH_TEXT.Contains(strSearch)).OrderByDescending(s => s.ID).ToList();
                    if (!string.IsNullOrEmpty(date))
                    {
                        date = Convert.ToDateTime(date).ToString("yyyyMMdd");
                        dataList = dataList.Where(s => s.INOUT_DATE.HasValue ? s.INOUT_DATE.Value.ToString("yyyyMMdd").Equals(date) : false).ToList();

                    }
                    totalRecord = Convert.ToInt16(dataList.Count());
                    dataList = dataList.Skip(skip).Take(pageSize).ToList();
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
            }

            rptInOut.DataSource = dataList;
            rptInOut.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            //LoadData(txtSearch.Text.Trim(), 0, storeId);
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("FormDailyIncomeOutcomeUpdate.aspx");
        }

        protected void ddlPager_SelectedIndexChanged(object sender, EventArgs e)
        {
            //LoadData(txtSearch.Text.Trim(), Convert.ToInt16(ddlPager.SelectedValue) - 1, storeId);
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

