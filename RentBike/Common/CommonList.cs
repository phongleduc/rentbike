using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace RentBike.Common
{
    public class CommonList
    {
        public static void LoadCity(DropDownList ddlCt)
        {
            List<City> lst;
            using (var db = new RentBikeEntities())
            {
                var ct = from s in db.Cities
                         select s;

                lst = ct.ToList<City>();
            }

            ddlCt.DataSource = lst;
            ddlCt.DataTextField = "NAME";
            ddlCt.DataValueField = "ID";
            ddlCt.DataBind();
        }

        public static void LoadRentType(DropDownList ddlRt)
        {
            List<RentType> lst;
            using (var db = new RentBikeEntities())
            {
                var rt = from s in db.RentTypes
                         select s;

                lst = rt.ToList<RentType>();
            }

            ddlRt.DataSource = lst;
            ddlRt.DataTextField = "NAME";
            ddlRt.DataValueField = "ID";
            ddlRt.DataBind();
        }

        public static void LoadStore(DropDownList ddlSt)
        {
            using (var db = new RentBikeEntities())
            {
                var rt = from s in db.Stores
                         select s;

                foreach (Store store in rt)
                {
                    ddlSt.Items.Add(new ListItem(store.NAME, store.ID.ToString()));
                }
            }
        }

        public static int GetInoutTypeFromRentType(int rentTypeId)
        {
            int inOutType = 0;
            switch (rentTypeId)
            {
                case 1:
                    inOutType = 17;
                    break;
                case 2:
                    inOutType = 22;
                    break;
                case 3:
                    inOutType = 23;
                    break;
                default:
                    break;
            }
            return inOutType;
        }

        public static Contract GetContractByLicenseNo(string licenseNo)
        {
            using (var db = new RentBikeEntities())
            {
                var customer = db.Customers.FirstOrDefault(c =>c.LICENSE_NO == licenseNo);
                if (customer != null)
                {
                    Contract contract = db.Contracts.FirstOrDefault(c =>c.CUSTOMER_ID == customer.ID && c.CONTRACT_STATUS == true);
                    if(contract != null)
                    {
                        return contract;
                    }
                }
            }
            return null;
        }

        private static void CalculatePeriodFee(Contract contract, out int multipleFee, out decimal increateFeeCar, out decimal increateFeeEquip, out decimal increateFeeOther, bool bFirstCreate)
        {
            multipleFee = Convert.ToInt32(Decimal.Floor(contract.CONTRACT_AMOUNT / 100000));
            if (bFirstCreate)
            {
                increateFeeCar = increateFeeEquip = increateFeeOther = contract.FEE_PER_DAY * 10;
            }
            else
            {
                increateFeeCar = (contract.FEE_PER_DAY * 10) + (multipleFee * 50 * 10);
                increateFeeEquip = (contract.FEE_PER_DAY * 10) + (multipleFee * 100 * 10);
                increateFeeOther = (contract.FEE_PER_DAY * 10);
            }
        }

        public static void AutoExtendPeriod(RentBikeEntities db, int contractId)
        {
            Contract contract = db.Contracts.FirstOrDefault(c =>c.ID == contractId && c.CONTRACT_STATUS == true);
            if (contract != null)
            {
                var listPayPeriod = db.PayPeriods.Where(c => c.CONTRACT_ID == contract.ID).ToList();
                PayPeriod lastPayPeriod = listPayPeriod.LastOrDefault();

                DateTime extendEndDate = (contract.EXTEND_END_DATE == null || contract.EXTEND_END_DATE == contract.END_DATE) ? contract.END_DATE.AddDays(-10) : lastPayPeriod.PAY_DATE;

                int overDate = DateTime.Today.Subtract(extendEndDate).Days;
                if (overDate >= 0)
                {
                    int percentDate = overDate / 30;
                    DateTime endDateUpdated = extendEndDate.AddDays(30 * (percentDate + 1));
                    contract.EXTEND_END_DATE = endDateUpdated;

                    int multipleFee;
                    decimal increateFeeCar;
                    decimal increateFeeEquip;
                    decimal increateFeeOther;
                    CalculatePeriodFee(contract, out multipleFee, out increateFeeCar, out increateFeeEquip, out increateFeeOther, false);

                    for (int i = 0; i <= percentDate; i++)
                    {
                        if (lastPayPeriod.PAY_DATE.Subtract(contract.EXTEND_END_DATE.Value.AddDays(-10)).Days > 0)
                        {
                            break;
                        }
                        lastPayPeriod = CreateOneMorePayPeriod(db, contract, lastPayPeriod.PAY_DATE, multipleFee, increateFeeCar, increateFeeEquip, increateFeeOther, false);
                    }

                    contract.EXTEND_END_DATE = lastPayPeriod.PAY_DATE;
                    db.SaveChanges();
                }
            }
        }

        private static PayPeriod CreateOneMorePayPeriod(RentBikeEntities db, Contract contract, DateTime lastPeriodDate, decimal multipleFee, decimal increateFeeCar, decimal increateFeeEquip, decimal increateFeeOther, bool bFirstCreated)
        {
            PayPeriod pp1 = new PayPeriod();
            pp1.CONTRACT_ID = contract.ID;
            if (bFirstCreated)
            {
                pp1.PAY_DATE = lastPeriodDate;
                pp1.AMOUNT_PER_PERIOD = increateFeeCar;
            }
            else
            {
                pp1.PAY_DATE = lastPeriodDate.AddDays(10);
                switch (contract.RENT_TYPE_ID)
                {
                    case 1:
                        if (((contract.FEE_PER_DAY / multipleFee) * 10) < 4000)
                            pp1.AMOUNT_PER_PERIOD = increateFeeCar;
                        else
                            pp1.AMOUNT_PER_PERIOD = increateFeeOther;
                        break;
                    case 2:
                        if (((contract.FEE_PER_DAY / multipleFee) * 10) < 6000)
                            pp1.AMOUNT_PER_PERIOD = increateFeeEquip;
                        else
                            pp1.AMOUNT_PER_PERIOD = increateFeeOther;
                        break;
                    default:
                        pp1.AMOUNT_PER_PERIOD = increateFeeOther;
                        break;
                }
            }
            pp1.STATUS = true;
            pp1.ACTUAL_PAY = 0;

            PayPeriod pp2 = new PayPeriod();
            pp2.CONTRACT_ID = contract.ID;
            if (bFirstCreated)
                pp2.PAY_DATE = pp1.PAY_DATE.AddDays(9);
            else
                pp2.PAY_DATE = pp1.PAY_DATE.AddDays(10);
            pp2.AMOUNT_PER_PERIOD = pp1.AMOUNT_PER_PERIOD;
            pp2.STATUS = true;
            pp2.ACTUAL_PAY = 0;

            PayPeriod pp3 = new PayPeriod();
            pp3.CONTRACT_ID = contract.ID;
            pp3.PAY_DATE = pp2.PAY_DATE.AddDays(10);
            pp3.AMOUNT_PER_PERIOD = pp1.AMOUNT_PER_PERIOD;
            pp3.STATUS = true;
            pp3.ACTUAL_PAY = 0;

            db.PayPeriods.Add(pp1);
            db.PayPeriods.Add(pp2);
            db.PayPeriods.Add(pp3);

            db.SaveChanges();

            return pp3;
        }
        public static void CreatePayPeriod(RentBikeEntities db, int contractId, DateTime lastPeriodDate, bool bFirstCreate)
        {
            var contract = db.Contracts.FirstOrDefault(c => c.CONTRACT_STATUS == true && c.ID == contractId);
            if (contract != null)
            {
                int multipleFee;
                decimal increateFeeCar;
                decimal increateFeeEquip;
                decimal increateFeeOther;

                CalculatePeriodFee(contract, out multipleFee, out increateFeeCar, out increateFeeEquip, out increateFeeOther, bFirstCreate);
                CreateOneMorePayPeriod(db, contract, lastPeriodDate, multipleFee, increateFeeCar, increateFeeEquip, increateFeeOther, bFirstCreate); 
            }
        }

        #region The funtions for batch processing
        public static void AutoUpdateAmountPayPeriod()
        {
            using (var db = new RentBikeEntities())
            {
                var contracts = db.Contracts.Where(c => c.CONTRACT_STATUS == true).ToList();
                foreach (var contract in contracts)
                {
                    List<PayPeriod> listPayPeriod = db.PayPeriods.Where(c => c.CONTRACT_ID == contract.ID).ToList();
                    foreach (PayPeriod pp in listPayPeriod)
                    {
                        var inoutList = db.InOuts.Where(s => s.PERIOD_ID == pp.ID);
                        var sumPay = inoutList.Select(s => s.IN_AMOUNT).DefaultIfEmpty(0).Sum();

                        pp.ACTUAL_PAY = sumPay;
                    }
                }
                db.SaveChanges();
            }
        }
        public static void AutoExtendContract()
        {
            using (var db = new RentBikeEntities())
            {
                //db.Configuration.AutoDetectChangesEnabled = false;
                //db.Configuration.ValidateOnSaveEnabled = false;

                var contracts = db.Contracts.Where(c => c.CONTRACT_STATUS == true).ToList();
                foreach (var contract in contracts)
                {
                    CommonList.AutoExtendPeriod(db, contract.ID);
                }
            }
        }

        public static void AutoUpdateAndRemovePeriod()
        {
            using (var db = new RentBikeEntities())
            {
                var contracts = db.Contracts.Where(c => c.CONTRACT_STATUS == true).ToList();
                foreach (var contract in contracts)
                {
                    List<PayPeriod> lstPay = db.PayPeriods.Where(c => c.CONTRACT_ID == contract.ID).ToList();
                    DateTime extendDate = contract.END_DATE.AddDays(-10);

                    if (!contract.EXTEND_END_DATE.HasValue || contract.EXTEND_END_DATE == contract.END_DATE)
                    {
                        contract.EXTEND_END_DATE = contract.END_DATE;
                        extendDate = contract.EXTEND_END_DATE.Value.AddDays(-10);
                    }
                    else
                    {
                        extendDate = contract.EXTEND_END_DATE.Value; 
                    }
                    db.PayPeriods.RemoveRange(lstPay.Where(c => c.PAY_DATE > extendDate));
                }
                db.SaveChanges();
            }
        }

        public static void ReCalculatePeriod()
        {
            using (var db = new RentBikeEntities())
            {
                var contracts = db.Contracts.Where(c => c.CONTRACT_STATUS == true).ToList();
                foreach (var contract in contracts)
                {
                    List<PayPeriod> lstPay = db.PayPeriods.Where(c => c.CONTRACT_ID == contract.ID).ToList();
                    int div = lstPay.Count % 3;
                    if (div > 0)
                    {
                        div = 3 - div;
                        PayPeriod lastPay = lstPay.LastOrDefault();
                        for (int i = 1; i <= div; i++)
                        {
                            PayPeriod pp = new PayPeriod();
                            pp.CONTRACT_ID = contract.ID;
                            pp.PAY_DATE = lastPay.PAY_DATE.AddDays(i * 10);
                            pp.AMOUNT_PER_PERIOD = lastPay.AMOUNT_PER_PERIOD;
                            pp.STATUS = true;
                            pp.ACTUAL_PAY = 0;

                            db.PayPeriods.Add(pp);
                        }
                    }
                }
                db.SaveChanges();
            }
        }

        public static void BackUp()
        {
            using (var db = new RentBikeEntities())
            {
                string dataTime = db.Database.Connection.Database + "_" + DateTime.Now.ToString("yyyyMMddHHmm");
                string directory = HttpContext.Current.Server.MapPath("~/") + "/backups/";
                string fileName = directory + dataTime + ".bak";

                if (!System.IO.Directory.Exists(directory))
                    System.IO.Directory.CreateDirectory(directory);

                // Here the procedure is called and executes successfully
                db.Database.ExecuteSqlCommand(System.Data.Entity.TransactionalBehavior.DoNotEnsureTransaction, "EXEC [dbo].[BackUp] @path = N'" + fileName + "'");
            }

        }
        #endregion
    }
}