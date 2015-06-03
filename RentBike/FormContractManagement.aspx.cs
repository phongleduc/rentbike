using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RentBike.Common;

namespace RentBike
{
    public partial class ContractManagement : FormBase
    {
        private decimal TotalFeeBike = 0;
        private decimal TotalFeeEquip = 0;
        private decimal TotalFeeOther = 0;

        private decimal TotalAmountBikeContract = 0;
        private decimal TotalAmountEquipContract = 0;
        private decimal TotalAmountOtherContract = 0;
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!IsPostBack)
            {
                string searchText = Request.QueryString["q"];
                txtSearch.Text = searchText;
                LoadGeneralInfo();
                LoadData(txtSearch.Text, 0);
            }
        }

        protected override void ddlStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text.Trim(), Helper.parseInt(drpRentType.SelectedValue));
            LoadGeneralInfo();
        }

        private void LoadData(string strSearch, int rentType)
        {
            using (var db = new RentBikeEntities())
            {
                var dataList = (from s in db.CONTRACT_FULL_VW
                                where s.CONTRACT_STATUS == true && s.ACTIVE == true
                                select s).OrderByDescending(c => c.RENT_DATE).ToList();

                if (STORE_ID != 0)
                {
                    dataList = dataList.Where(c => c.STORE_ID == STORE_ID).ToList();
                }

                if (!string.IsNullOrEmpty(strSearch))
                {
                    dataList = dataList.Where(c => c.SEARCH_TEXT.ToLower().Contains(strSearch.ToLower())
                        || c.CUSTOMER_NAME.ToLower().Contains(strSearch.ToLower())).ToList();
                }


                if (rentType != 0)
                {
                    dataList = dataList.Where(c => c.RENT_TYPE_ID == rentType).ToList();
                }
                rptContract.DataSource = dataList;
                rptContract.DataBind();
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("FormContractUpdate.aspx");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text.Trim(), Helper.parseInt(drpRentType.SelectedValue));
        }


        protected void drpRentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text.Trim(), Helper.parseInt(drpRentType.SelectedValue));
        }

        private void LoadGeneralInfo()
        {
            int rentbikeNo = GetNoRentBikeContract();
            int rentequipNo = GetNoRentOfficeEquipContract();
            int rentotherNo = GetNoRentOtherContract();
            int notFinishContract = GetNoOfNotFinishedContract();
            decimal totalmoneynotfinishContract = GetAmountOfNotFinishedContract();

            lblRentBikeNo.Text = rentbikeNo == 0 ? "0" : string.Format("{0:0,0}", rentbikeNo);
            lblRentOfficeEquip.Text = rentequipNo == 0 ? "0" : string.Format("{0:0,0}", rentequipNo);
            lblRentOther.Text = rentotherNo == 0 ? "0" : string.Format("{0:0,0}", rentotherNo);

            lblTotalBikeContractAmount.Text = TotalAmountBikeContract == 0 ? "0" : string.Format("{0:0,0}", TotalAmountBikeContract);
            lblTotalEquipContractAmount.Text = TotalAmountEquipContract == 0 ? "0" : string.Format("{0:0,0}", TotalAmountEquipContract);
            lblTotalOtherContractAmount.Text = TotalAmountOtherContract == 0 ? "0" : string.Format("{0:0,0}", TotalAmountOtherContract);

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
            int no = 0;
            using (var db = new RentBikeEntities())
            {
                var item = (from itm in db.Contracts
                            where itm.RENT_TYPE_ID == 1 && itm.CONTRACT_STATUS == true
                            select itm);

                if (STORE_ID != 0)
                    item = item.Where(c => c.STORE_ID == STORE_ID);

                if (item != null && item.Any())
                {
                    no = item.Count();
                    TotalFeeBike = item.Select(c => c.FEE_PER_DAY).DefaultIfEmpty(0).Sum();
                    TotalAmountBikeContract = item.Select(c => c.CONTRACT_AMOUNT).DefaultIfEmpty(0).Sum();
                }
            }
            return no;
        }

        private int GetNoRentOfficeEquipContract()
        {
            int no = 0;
            using (var db = new RentBikeEntities())
            {
                var item = (from itm in db.Contracts
                            where itm.RENT_TYPE_ID == 2 && itm.CONTRACT_STATUS == true
                            select itm);

                if (STORE_ID != 0)
                    item = item.Where(c => c.STORE_ID == STORE_ID);

                if (item != null && item.Any())
                {
                    no = item.Count();
                    TotalFeeEquip = item.Select(c => c.FEE_PER_DAY).DefaultIfEmpty(0).Sum();
                    TotalAmountEquipContract = item.Select(c => c.CONTRACT_AMOUNT).DefaultIfEmpty(0).Sum();
                }
            }
            return no;
        }

        private int GetNoRentOtherContract()
        {
            int no = 0;
            using (var db = new RentBikeEntities())
            {
                var item = (from itm in db.Contracts
                            where itm.RENT_TYPE_ID == 3 // Other
                            && itm.STORE_ID == STORE_ID && itm.CONTRACT_STATUS == true
                            select itm);

                if (STORE_ID != 0)
                    item = item.Where(c => c.STORE_ID == STORE_ID);

                if (item != null && item.Any())
                {
                    no = item.Count();
                    TotalFeeOther = item.Select(c => c.FEE_PER_DAY).DefaultIfEmpty(0).Sum();
                    TotalAmountOtherContract = item.Select(c => c.CONTRACT_AMOUNT).DefaultIfEmpty(0).Sum();
                }
            }

            return no;
        }

        private int GetNoOfNotFinishedContract()
        {
            int no = 0;
            using (var db = new RentBikeEntities())
            {
                var item = (from itm in db.Contracts
                            where itm.CONTRACT_STATUS == true
                            select itm);

                if (STORE_ID != 0)
                    item = item.Where(c => c.STORE_ID == STORE_ID);

                if (item != null && item.Any())
                    no = item.Count();
            }

            return no;
        }

        private decimal GetAmountOfNotFinishedContract()
        {
            decimal amount = 0;
            using (var db = new RentBikeEntities())
            {
                var item = from itm in db.Contracts
                           where itm.CONTRACT_STATUS == true
                           select itm;

                if (STORE_ID != 0)
                    item = item.Where(c => c.STORE_ID == STORE_ID);

                if (item != null && item.Any())
                    amount = item.Select(c => c.CONTRACT_AMOUNT).DefaultIfEmpty(0).Sum();
            }
            return amount;
        }
    }
}