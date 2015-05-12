using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RentBike.Common;

namespace RentBike
{
    public partial class FormBadContractReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["store_id"] == null)
            {
                Response.Redirect("FormLogin.aspx");
            }
            using (var db = new RentBikeEntities())
            {
                List<CONTRACT_FULL_VW> result = GetResultList(db);
                LoadGeneralInfo(result);
                LoadData(result, db);
            }
        }

        private List<CONTRACT_FULL_VW> GetResultList(RentBikeEntities db)
        {
            var data = new List<CONTRACT_FULL_VW>();
            if (CheckAdminPermission())
            {
                DropDownList ddlStore = Master.FindControl("ddlStore") as DropDownList;
                if (ddlStore != null && !string.IsNullOrEmpty(ddlStore.SelectedValue))
                {
                    int storeid = Helper.parseInt(ddlStore.SelectedValue);
                    data = db.CONTRACT_FULL_VW.ToList().Where(c => c.CONTRACT_STATUS == true && c.STORE_ID == storeid)
                   .OrderByDescending(c => c.ID).ToList();
                }
                else
                {
                    data = db.CONTRACT_FULL_VW.ToList().Where(c => c.CONTRACT_STATUS == true)
                        .OrderByDescending(c => c.ID).ToList();
                }
            }
            else
            {
                int storeid = Helper.parseInt(Session["store_id"].ToString());
                data = db.CONTRACT_FULL_VW.ToList().Where(c => c.CONTRACT_STATUS == true && c.STORE_ID == storeid)
                    .OrderByDescending(c => c.ID).ToList();
            }

            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                data = data.Where(s => s.SEARCH_TEXT.Contains(txtSearch.Text)).ToList();
            }

            var result = new List<CONTRACT_FULL_VW>();
            var lstPeriod = db.PayPeriods.Where(s => s.STATUS == true).ToList();
            foreach (CONTRACT_FULL_VW c in data)
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

        private void LoadData(List<CONTRACT_FULL_VW> data, RentBikeEntities db)
        {
            rptContract.DataSource = data;
            rptContract.DataBind();

        }
        protected void btnNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("FormContractUpdate.aspx");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            //LoadData(txtSearch.Text.Trim(), 0);
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