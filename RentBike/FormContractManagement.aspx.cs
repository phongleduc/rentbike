﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RentBike.Common;

namespace RentBike
{
    public partial class ContractManagement : System.Web.UI.Page
    {
        int pageSize = 20;
        private decimal TotalFeeBike = 0;
        private decimal TotalFeeEquip = 0;
        private decimal TotalFeeOther = 0;
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
                LoadGeneralInfo();

                if (CheckAdminPermission())
                    LoadDataAdmin(string.Empty, Helper.parseInt(drpStore.SelectedValue), Helper.parseInt(drpRentType.SelectedValue));
                else
                    LoadData(string.Empty, Helper.parseInt(drpRentType.SelectedValue));
            }
        }

        protected void ddlStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList drpStore = sender as DropDownList;
            if (CheckAdminPermission())
                LoadDataAdmin(string.Empty, Helper.parseInt(drpStore.SelectedValue), Helper.parseInt(drpRentType.SelectedValue));
            else
                LoadData(string.Empty, Helper.parseInt(drpRentType.SelectedValue));

            LoadGeneralInfo();
            //LoadData(txtSearch.Text.Trim(), Helper.parseInt(drpStore.SelectedValue), Convert.ToInt32(ddlPager.SelectedValue) - 1);
        }

        private void LoadData(string strSearch, int rentType)
        {
            int storeid = 0;
            if (CheckAdminPermission())
            {
                storeid = Helper.parseInt(drpStore.SelectedValue);
            }
            else
            { 
               storeid = Helper.parseInt(Session["store_id"].ToString()); 
            }
            List<CONTRACT_FULL_VW> dataList;
            using (var db = new RentBikeEntities())
            {
                dataList = (from s in db.CONTRACT_FULL_VW
                            where s.SEARCH_TEXT.Contains(strSearch) && s.CONTRACT_STATUS == true && s.STORE_ID == storeid
                            select s).OrderByDescending(c =>c.RENT_DATE).ToList();

                if (dataList.Any())
                {
                    if (rentType != 0)
                    {
                        dataList = dataList.Where(c => c.RENT_TYPE_ID == rentType).ToList();
                    }
                    rptContract.DataSource = dataList;
                    rptContract.DataBind();
                }
            }


        }

        private void LoadDataAdmin(string strSearch, int storeId, int rentType)
        {
            List<CONTRACT_FULL_VW> dataList;
            using (var db = new RentBikeEntities())
            {
                dataList = (from s in db.CONTRACT_FULL_VW
                            where s.SEARCH_TEXT.Contains(strSearch) && s.CONTRACT_STATUS == true
                            select s).OrderByDescending(c => c.RENT_DATE).ToList();
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
                    //int totalRecord = st.Count();
                    //int totalPage = totalRecord % pageSize == 0 ? totalRecord / pageSize : totalRecord / pageSize + 1;
                    //List<int> pageList = new List<int>();
                    //for (int i = 1; i <= totalPage; i++)
                    //{
                    //    pageList.Add(i);
                    //}

                    //ddlPager.DataSource = pageList;
                    //ddlPager.DataBind();
                    //if (pageList.Count > 0)
                    //{
                    //    ddlPager.SelectedIndex = page;
                    //}

                    //int skip = page * pageSize;
                    //dataList = st.Skip(skip).Take(pageSize).ToList();

                    rptContract.DataSource = dataList;
                    rptContract.DataBind();
                }
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("FormContractUpdate.aspx");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (CheckAdminPermission())
                LoadDataAdmin(txtSearch.Text.Trim(), Helper.parseInt(drpStore.SelectedValue), Helper.parseInt(drpRentType.SelectedValue));
            else
                LoadData(txtSearch.Text.Trim(), Helper.parseInt(drpRentType.SelectedValue));
        }

        //protected void ddlPager_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (CheckAdminPermission())
        //        LoadDataAdmin(txtSearch.Text.Trim(), Convert.ToInt32(ddlPager.SelectedValue) - 1);
        //    else
        //        LoadData(txtSearch.Text.Trim(), Convert.ToInt32(ddlPager.SelectedValue) - 1);
        //}

        protected void drpRentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CheckAdminPermission())
                LoadDataAdmin(txtSearch.Text.Trim(), Helper.parseInt(drpStore.SelectedValue), Helper.parseInt(drpRentType.SelectedValue));
            else
                LoadData(txtSearch.Text.Trim(), Helper.parseInt(drpRentType.SelectedValue));
        }

        private void LoadGeneralInfo()
        {
            int rentbikeNo = CheckAdminPermission() ? GetNoRentBikeContractAdmin() : GetNoRentBikeContract();
            int rentequipNo = CheckAdminPermission() ? GetNoRentOfficeEquipContractAdmin() : GetNoRentOfficeEquipContract();
            int rentotherNo = CheckAdminPermission() ? GetNoRentOtherContractAdmin() : GetNoRentOtherContract();
            int notFinishContract = CheckAdminPermission() ? GetNoOfNotFinishedContractAdmin() : GetNoOfNotFinishedContract();
            decimal totalmoneynotfinishContract = CheckAdminPermission() ? GetAmountOfNotFinishedContractAdmin() : GetAmountOfNotFinishedContract();

            lblRentBikeNo.Text = rentbikeNo == 0 ? "0" : string.Format("{0:0,0}", rentbikeNo);
            lblRentOfficeEquip.Text = rentequipNo == 0 ? "0" : string.Format("{0:0,0}", rentequipNo);
            lblRentOther.Text = rentotherNo == 0 ? "0" : string.Format("{0:0,0}", rentotherNo);
            lblNotFinishedContract.Text = notFinishContract == 0 ? "0" : string.Format("{0:0,0}", notFinishContract);
            lblTotalMoneyOfNotFinishContract.Text = totalmoneynotfinishContract == 0 ? "0" : string.Format("{0:0,0}", totalmoneynotfinishContract);
            lblTotalFeeContract.Text = TotalFeeBike + TotalFeeEquip + TotalFeeOther == 0 ? "0" : string.Format("{0:0,0}", TotalFeeBike + TotalFeeEquip + TotalFeeOther);

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

        private int GetNoRentBikeContract()
        {
            int storeid = Convert.ToInt32(Session["store_id"]);
            int no = 0;
            using (var db = new RentBikeEntities())
            {
                var item = (from itm in db.Contracts
                            where itm.RENT_TYPE_ID == 1 // BIKE
                            && itm.STORE_ID == storeid && itm.CONTRACT_STATUS == true
                            select itm).ToList();

                if (item != null && item.Any())
                {
                    no = Convert.ToInt32(item.Count());
                    TotalFeeBike = item.Select(c => c.FEE_PER_DAY).DefaultIfEmpty(0).Sum();
                }
            }

            return no;
        }

        private int GetNoRentBikeContractAdmin()
        {
            int no = 0;
            int storeid = Helper.parseInt(drpStore.SelectedValue);
            using (var db = new RentBikeEntities())
            {
                if (storeid != 0)
                {
                    var item = (from itm in db.Contracts
                                where itm.RENT_TYPE_ID == 1 && itm.CONTRACT_STATUS == true && itm.STORE_ID == storeid// BIKE
                                select itm).ToList();

                    if (item != null && item.Any())
                    {
                        no = Convert.ToInt32(item.Count());
                        TotalFeeBike = item.Select(c => c.FEE_PER_DAY).DefaultIfEmpty(0).Sum();
                    }
                }
                else
                {
                    var item = (from itm in db.Contracts
                                where itm.RENT_TYPE_ID == 1 && itm.CONTRACT_STATUS == true // BIKE
                                select itm).ToList();

                    if (item != null && item.Any())
                    {
                        no = Convert.ToInt32(item.Count());
                        TotalFeeBike = item.Select(c => c.FEE_PER_DAY).DefaultIfEmpty(0).Sum();
                    }
                }
            }

            return no;
        }

        private int GetNoRentOfficeEquipContract()
        {
            int storeid = Convert.ToInt32(Session["store_id"]);
            int no = 0;
            using (var db = new RentBikeEntities())
            {
                var item = (from itm in db.Contracts
                            where itm.RENT_TYPE_ID == 2 // Office device
                            && itm.STORE_ID == storeid && itm.CONTRACT_STATUS == true
                            select itm).ToList();

                if (item != null && item.Any())
                {
                    no = Convert.ToInt32(item.Count());
                    TotalFeeEquip = item.Select(c => c.FEE_PER_DAY).DefaultIfEmpty(0).Sum();
                }
            }

            return no;
        }

        private int GetNoRentOfficeEquipContractAdmin()
        {
            int no = 0;
            int storeid = Helper.parseInt(drpStore.SelectedValue);
            using (var db = new RentBikeEntities())
            {
                if (storeid != 0)
                {
                    var item = (from itm in db.Contracts
                                where itm.RENT_TYPE_ID == 2 && itm.CONTRACT_STATUS == true && itm.STORE_ID == storeid// Office device
                                select itm).ToList();

                    if (item != null && item.Any())
                    {
                        no = Convert.ToInt32(item.Count());
                        TotalFeeEquip = item.Select(c => c.FEE_PER_DAY).DefaultIfEmpty(0).Sum();
                    }
                }
                else
                {
                    var item = (from itm in db.Contracts
                                where itm.RENT_TYPE_ID == 2 && itm.CONTRACT_STATUS == true // Office device
                                select itm).ToList();

                    if (item != null && item.Any())
                    {
                        no = Convert.ToInt32(item.Count());
                        TotalFeeEquip = item.Select(c => c.FEE_PER_DAY).DefaultIfEmpty(0).Sum();
                    }
                }
            }

            return no;
        }

        private int GetNoRentOtherContract()
        {
            int storeid = Convert.ToInt32(Session["store_id"]);
            int no = 0;
            using (var db = new RentBikeEntities())
            {
                var item = (from itm in db.Contracts
                            where itm.RENT_TYPE_ID == 3 // Other
                            && itm.STORE_ID == storeid && itm.CONTRACT_STATUS == true
                            select itm).ToList();

                if (item != null && item.Any())
                {
                    no = Convert.ToInt32(item.Count());
                    TotalFeeOther = item.Select(c => c.FEE_PER_DAY).DefaultIfEmpty(0).Sum();
                }
            }

            return no;
        }

        private int GetNoRentOtherContractAdmin()
        {
            int no = 0;
            int storeid = Helper.parseInt(drpStore.SelectedValue);
            using (var db = new RentBikeEntities())
            {
                if (storeid != 0)
                {
                    var item = (from itm in db.Contracts
                                where itm.RENT_TYPE_ID == 3 && itm.CONTRACT_STATUS == true && itm.STORE_ID == storeid// Other
                                select itm).ToList();

                    if (item != null && item.Any())
                    {
                        no = Convert.ToInt32(item.Count());
                        TotalFeeOther = item.Select(c => c.FEE_PER_DAY).DefaultIfEmpty(0).Sum();
                    }
                }
                else
                {
                    var item = (from itm in db.Contracts
                                where itm.RENT_TYPE_ID == 3 && itm.CONTRACT_STATUS == true // Other
                                select itm).ToList();

                    if (item != null && item.Any())
                    {
                        no = Convert.ToInt32(item.Count());
                        TotalFeeOther = item.Select(c => c.FEE_PER_DAY).DefaultIfEmpty(0).Sum();
                    }
                }
            }

            return no;
        }

        private int GetNoOfNotFinishedContract()
        {
            int storeid = Convert.ToInt32(Session["store_id"]);
            int no = 0;
            using (var db = new RentBikeEntities())
            {
                var item = (from itm in db.Contracts
                            where itm.CONTRACT_STATUS == true // true == active (not finished contract)
                            && itm.STORE_ID == storeid
                            select itm).Count();

                no = Convert.ToInt32(item);
            }

            return no;
        }

        private int GetNoOfNotFinishedContractAdmin()
        {
            int no = 0;
            int storeid = Helper.parseInt(drpStore.SelectedValue);
            using (var db = new RentBikeEntities())
            {
                if (storeid != 0)
                {
                    var item = (from itm in db.Contracts
                                where itm.CONTRACT_STATUS == true && itm.STORE_ID == storeid// true == active (not finished contract)
                                select itm).Count();

                    no = Convert.ToInt32(item);
                }
                else
                {
                    var item = (from itm in db.Contracts
                                where itm.CONTRACT_STATUS == true// true == active (not finished contract)
                                select itm).Count();

                    no = Convert.ToInt32(item);
                }
            }

            return no;
        }

        private decimal GetAmountOfNotFinishedContract()
        {
            int storeid = Convert.ToInt32(Session["store_id"]);
            decimal amount = 0;
            List<Contract> lst = new List<Contract>();
            using (var db = new RentBikeEntities())
            {
                var item = from itm in db.Contracts
                           where itm.CONTRACT_STATUS == true // true == active (not finished contract)
                           && itm.STORE_ID == storeid
                           select itm;

                lst = item.ToList();

                for (int i = 0; i < lst.Count; i++)
                {
                    amount += lst[i].CONTRACT_AMOUNT;
                }
            }

            return amount;
        }

        private decimal GetAmountOfNotFinishedContractAdmin()
        {
            decimal amount = 0;
            int storeid = Helper.parseInt(drpStore.SelectedValue);
            List<Contract> lst = new List<Contract>();
            using (var db = new RentBikeEntities())
            {
                if (storeid != 0)
                {
                    var item = from itm in db.Contracts
                               where itm.CONTRACT_STATUS == true && itm.STORE_ID == storeid// true == active (not finished contract)
                               select itm;

                    lst = item.ToList();
                }
                else
                {
                    var item = from itm in db.Contracts
                               where itm.CONTRACT_STATUS == true // true == active (not finished contract)
                               select itm;

                    lst = item.ToList();
                }

                for (int i = 0; i < lst.Count; i++)
                {
                    amount += lst[i].CONTRACT_AMOUNT;
                }
            }

            return amount;
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