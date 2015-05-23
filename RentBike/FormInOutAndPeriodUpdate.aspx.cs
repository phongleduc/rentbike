﻿using RentBike.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RentBike
{
    public partial class FormInOutAndPeriodUpdate : FormBase
    {
        public DateTime InOutDate { get; set; }
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!IsPostBack)
            {
                // LOAD PAYPERIOD
                int inOutId = Helper.parseInt(Request.QueryString["id"]);
                if (inOutId != 0)
                {
                    using (var db = new RentBikeEntities())
                    {
                        var io = db.InOuts.FirstOrDefault(c =>c.ID == inOutId);
                        InOutDate = io.INOUT_DATE.Value;
                        // DISPLAY SREEN
                        txtIncome.Text = Convert.ToString(io.IN_AMOUNT.ToString());
                        txtMoreInfo.Text = io.MORE_INFO;
                        hplContract.NavigateUrl = string.Format("FormContractUpdate.aspx?ID={0}", io.CONTRACT_ID);
                        hplContract.Text = "Xem chi tiết hợp đồng";

                        var store = db.Stores.FirstOrDefault(s =>s.ID == io.STORE_ID && s.ACTIVE == true);
                        if (store != null)
                        {
                            txtStore.Text = store.NAME;
                            txtStore.Enabled = false;
                        }

                        var inouttypelist = db.InOutTypes.Where(s =>s.IS_CONTRACT == true && s.ACTIVE == true).ToList();
                        ddInOutType.DataSource = inouttypelist;
                        ddInOutType.DataTextField = "NAME";
                        ddInOutType.DataValueField = "ID";
                        ddInOutType.DataBind();
                        ddInOutType.SelectedValue = io.INOUT_TYPE_ID.ToString();
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            using (var db = new RentBikeEntities())
            {
                int inOutId = Helper.parseInt(Request.QueryString["id"]);
                if (inOutId != 0)
                {
                    var io = db.InOuts.FirstOrDefault(c =>c.ID == inOutId);
                    // SAVE INOUT
                    io.IN_AMOUNT = Convert.ToDecimal(txtIncome.Text);
                    io.MORE_INFO = txtMoreInfo.Text.Trim();
                    io.UPDATED_BY = Session["username"].ToString();
                    io.UPDATED_DATE = DateTime.Now;
                    db.SaveChanges();
                }

                int periodId = Helper.parseInt(Request.QueryString["pid"]);
                if (periodId != 0)
                {
                    // SAVE PERIOD
                    var pp = db.PayPeriods.FirstOrDefault(s =>s.ID == periodId);
                    pp.ACTUAL_PAY = Convert.ToDecimal(txtIncome.Text);
                    db.SaveChanges();

                    Response.Redirect(string.Format("FormInOutUpdate.aspx?ID={0}", periodId), false);
                }
                Response.Redirect("FormDailyIncomeOutcome.aspx", false);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("FormInOutUpdate.aspx?ID={0}", Request.QueryString["pid"]), false);
        }
    }
}