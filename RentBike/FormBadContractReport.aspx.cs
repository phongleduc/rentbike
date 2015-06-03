using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RentBike.Common;

namespace RentBike
{
    public partial class FormBadContractReport : FormBase
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!IsPostBack)
            {
                List<CONTRACT_FULL_VW> result = GetResultList(txtSearch.Text);
                LoadGeneralInfo(result);
                LoadData(result);
            }
        }

        private List<CONTRACT_FULL_VW> GetResultList(string strSearch)
        {
            using (var db = new RentBikeEntities())
            {
                IQueryable<CONTRACT_FULL_VW> dataList = db.CONTRACT_FULL_VW.Where(c => c.CONTRACT_STATUS == true && c.ACTIVE == true).OrderByDescending(c => c.ID);

                if (STORE_ID != 0)
                    dataList = dataList.Where(c => c.STORE_ID == STORE_ID);

                if (!string.IsNullOrEmpty(strSearch))
                    dataList = dataList.Where(s => s.SEARCH_TEXT.ToLower().Contains(txtSearch.Text.ToLower())
                        || s.CUSTOMER_NAME.ToLower().Contains(txtSearch.Text.ToLower()));


                var result = new List<CONTRACT_FULL_VW>();
                var lstPeriod = db.PayPeriods.Where(s => s.STATUS == true).ToList();
                foreach (CONTRACT_FULL_VW c in dataList)
                {
                    var lstTempPeriod = lstPeriod.Where(s => s.CONTRACT_ID == c.ID).ToList();
                    decimal totalPayed = lstTempPeriod.Select(s => s.ACTUAL_PAY).DefaultIfEmpty(0).Sum();
                    foreach (PayPeriod pp in lstTempPeriod)
                    {
                        if (pp.AMOUNT_PER_PERIOD > totalPayed)
                        {
                            c.PAY_DATE = pp.PAY_DATE;
                            c.OVER_DATE = DateTime.Now.Subtract(c.PAY_DATE).Days;
                            if (c.OVER_DATE > 50)
                            {
                                result.Add(c);
                            }
                            break;
                        }
                        totalPayed -= pp.AMOUNT_PER_PERIOD;
                    }
                }
                return result.OrderBy(c => c.OVER_DATE).ToList();
            }
        }

        private void LoadData(List<CONTRACT_FULL_VW> data)
        {
            rptBadContract.DataSource = data;
            rptBadContract.DataBind();

        }
        protected void btnNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("FormContractUpdate.aspx");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            List<CONTRACT_FULL_VW> result = GetResultList(txtSearch.Text);
            LoadGeneralInfo(result);
            LoadData(result);
        }

        private void LoadGeneralInfo(List<CONTRACT_FULL_VW> lstContract)
        {

            decimal rentbikeNo = 0;
            decimal rentEquipNo = 0;
            decimal rentOtherNo = 0;
            decimal totalBadContract = 0;

            IEnumerable<CONTRACT_FULL_VW> ieRentBike = lstContract.Where(x => x.RENT_TYPE_ID == 1 && x.CONTRACT_STATUS == true);
            if (ieRentBike.Any())
            {
                rentbikeNo = ieRentBike.Sum(x => x.CONTRACT_AMOUNT);
            }

            IEnumerable<CONTRACT_FULL_VW> ieRentEquiq = lstContract.Where(x => x.RENT_TYPE_ID == 2 && x.CONTRACT_STATUS == true);
            if (ieRentEquiq.Any())
            {
                rentEquipNo = ieRentEquiq.Sum(x => x.CONTRACT_AMOUNT);
            }

            IEnumerable<CONTRACT_FULL_VW> ieRentOther = lstContract.Where(x => x.RENT_TYPE_ID == 3 && x.CONTRACT_STATUS == true);
            if (ieRentBike.Any())
            {
                rentOtherNo = ieRentOther.Sum(x => x.CONTRACT_AMOUNT);
            }

            lblRentBikeCount.Text = ieRentBike.Count().ToString();
            lblRentEquipCount.Text = ieRentEquiq.Count().ToString();
            lblRentOtherCount.Text = ieRentOther.Count().ToString();

            lblTotalFeeBikeContract.Text = rentbikeNo == 0 ? "0" : string.Format("{0:0,0}", rentbikeNo) + " VNĐ";
            lblTotalFeeEquiqContract.Text = rentbikeNo == 0 ? "0" : string.Format("{0:0,0}", rentEquipNo) + " VNĐ";
            lblTotalFeeOtherContract.Text = rentbikeNo == 0 ? "0" : string.Format("{0:0,0}", rentOtherNo) + " VNĐ";
            totalBadContract = lstContract.Sum(x => x.CONTRACT_AMOUNT);
            lblNumberOfBadContract.Text = lstContract.Count() + "/" + lstContract.Count();
            if (lstContract.Count() > 0)
                lblPercentBadContract.Text = String.Format("{0:P2}", lstContract.Count() / lstContract.Count());
            else
                lblPercentBadContract.Text = String.Format("{0:P2}", 0);

            lblTotalBadContract.Text = totalBadContract == 0 ? "0" : string.Format("{0:0,0}", totalBadContract) + " VNĐ";
        }

        protected override void ddlStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<CONTRACT_FULL_VW> result = GetResultList(txtSearch.Text);
            LoadGeneralInfo(result);
            LoadData(result);
        }
    }
}