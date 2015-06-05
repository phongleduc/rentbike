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
                var rt = from s in db.Stores where s.ACTIVE == true
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
                var customer = db.Customers.FirstOrDefault(c => c.LICENSE_NO == licenseNo);
                if (customer != null)
                {
                    Contract contract = db.Contracts.FirstOrDefault(c => c.CUSTOMER_ID == customer.ID && c.CONTRACT_STATUS == true);
                    if (contract != null)
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
            Contract contract = db.Contracts.FirstOrDefault(c => c.ID == contractId && c.CONTRACT_STATUS == true);
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
                if (contract.FEE_PER_DAY > 0)
                {
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
                else
                    pp1.AMOUNT_PER_PERIOD = increateFeeCar;
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

        public static bool IsBadContract(RentBikeEntities db, int contractId)
        {
            var contract = db.Contracts.FirstOrDefault(c => c.ID == contractId && c.CONTRACT_STATUS == true);
            if (contract != null)
            {
                var lstPeriod = db.PayPeriods.Where(c => c.CONTRACT_ID == contractId);
                decimal totalPayed = lstPeriod.Select(c => c.ACTUAL_PAY).DefaultIfEmpty(0).Sum();
                foreach (PayPeriod pp in lstPeriod)
                {
                    if (pp.AMOUNT_PER_PERIOD > totalPayed)
                    {
                        if (DateTime.Today.Subtract(pp.PAY_DATE).Days > 50)
                            return true;
                    }
                    totalPayed -= pp.AMOUNT_PER_PERIOD;
                }
            }
            return false;
        }

        public static List<CONTRACT_FULL_VW> GetWarningData(string date, string searchText, int storeId)
        {
            using (var db = new RentBikeEntities())
            {
                //int totalRecord = 0;
                List<CONTRACT_FULL_VW> dataList = new List<CONTRACT_FULL_VW>();

                var st = db.CONTRACT_FULL_VW.Where(c => c.CONTRACT_STATUS == true && c.ACTIVE == true);

                if (storeId != 0)
                {
                    st = st.Where(c => c.STORE_ID == storeId);
                }
                st = st.OrderByDescending(c => c.ID);

                string searchDate = string.Empty;
                if (!string.IsNullOrEmpty(date))
                {
                    searchDate = Convert.ToDateTime(date).ToString("yyyyMMdd");
                }

                var lstPeriod = db.PayPeriods.Where(s => s.STATUS == true).ToList();
                foreach (CONTRACT_FULL_VW c in st)
                {
                    var inOutList = db.InOuts.Where(s => s.CONTRACT_ID == c.ID).ToList();

                    c.PAYED_TIME = 0;
                    c.PAY_DATE = c.RENT_DATE;
                    c.DAY_DONE = DateTime.Now.Subtract(c.PAY_DATE).Days;

                    DateTime nowDate = DateTime.Today;
                    if (!string.IsNullOrEmpty(date))
                    {
                        nowDate = Convert.ToDateTime(date);
                    }
                    string contactId = c.ID.ToString();
                    var tmpLstPeriod = lstPeriod.Where(s => s.CONTRACT_ID == c.ID).ToList();
                    if (tmpLstPeriod != null)
                    {
                        decimal paidAmount = tmpLstPeriod.Where(s => s.ACTUAL_PAY > 0).Select(s => s.ACTUAL_PAY).DefaultIfEmpty(0).Sum();
                        int paidNumberOfFee = 0;
                        bool paidFull = false;
                        foreach (PayPeriod pp in tmpLstPeriod)
                        {
                            c.PERIOD_MESSAGE = GetPeriodMessage(tmpLstPeriod, nowDate);
                            if (pp.AMOUNT_PER_PERIOD == 0)
                            {
                                c.OVER_DATE = 0;
                                break;
                            }
                            paidAmount -= pp.AMOUNT_PER_PERIOD;
                            if (paidAmount >= 0)
                                paidNumberOfFee += 1;

                            if (paidAmount <= 0)
                            {
                                c.OVER_DATE = nowDate.Subtract(pp.PAY_DATE).Days;
                                if (paidAmount < 0 && tmpLstPeriod.Any(s => s.PAY_DATE == pp.PAY_DATE.AddDays(9)))
                                {
                                    c.OVER_DATE = nowDate.Subtract(pp.PAY_DATE).Days + 2;
                                }
                                c.PAY_DATE = pp.PAY_DATE;
                                c.PERIOD_ID = pp.ID;
                                if (paidAmount == 0 || c.OVER_DATE <= 0)
                                    paidFull = true;
                                break;
                            }
                        }
                        c.PAYED_TIME = paidNumberOfFee;
                        c.DAY_DONE = DateTime.Now.Subtract(c.RENT_DATE).Days + 1;

                        if (string.IsNullOrEmpty(date) || DateTime.Now.Subtract(Convert.ToDateTime(date)).Days <= 0)
                        {
                            if (paidFull && c.OVER_DATE <= 0)
                            {
                                c.CSS_CLASS = "background-green";
                            }
                            else if (c.OVER_DATE > 10)
                            {
                                c.CSS_CLASS = "background-red";
                            }
                        }
                        else
                        {
                            if (c.OVER_DATE <= 0)
                            {
                                var inout = inOutList.Where(s => s.PERIOD_DATE.ToString("yyyyMMdd").Equals(nowDate.ToString("yyyyMMdd"))).OrderByDescending(s => s.INOUT_DATE).FirstOrDefault();
                                if (inout != null && inout.INOUT_DATE.HasValue && inout.INOUT_DATE.Value.Subtract(nowDate).Days > 0)
                                {
                                    c.CSS_CLASS = "background-amber";
                                }
                                else
                                {
                                    c.CSS_CLASS = "background-green";
                                }
                            }
                            else
                            {
                                c.CSS_CLASS = "background-red";
                            }
                        }

                        if (c.FEE_PER_DAY == 0)
                            c.CSS_CLASS = "background-green";

                        c.RENT_TYPE_NAME = ReBuildRentTypeName(c);
                        if (!string.IsNullOrEmpty(searchDate))
                        {
                            if (tmpLstPeriod.Any(s => s.PAY_DATE.ToString("yyyyMMdd").Equals(searchDate)))
                            {
                                c.FEE_PER_DAY = tmpLstPeriod.FirstOrDefault(s => s.PAY_DATE.ToString("yyyyMMdd").Equals(searchDate)).AMOUNT_PER_PERIOD;
                                dataList.Add(c);
                            }
                        }
                        else if (tmpLstPeriod.Any(s => s.PAY_DATE.ToString("yyyyMMdd").Equals(DateTime.Now.ToString("yyyyMMdd"))))
                        {
                            c.FEE_PER_DAY = tmpLstPeriod.FirstOrDefault(s => s.PAY_DATE.ToString("yyyyMMdd").Equals(DateTime.Now.ToString("yyyyMMdd"))).AMOUNT_PER_PERIOD;
                            dataList.Add(c);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(searchText))
                {
                    dataList = dataList.Where(s => s.SEARCH_TEXT.ToLower().Contains(searchText.ToLower())
                        || s.CUSTOMER_NAME.ToLower().Contains(searchText.ToLower())).ToList();
                }
                return dataList.OrderBy(c => c.DAY_DONE).ToList();
            }
        }

        private static string GetPeriodMessage(List<PayPeriod> listPay, DateTime searchDate)
        {
            var index = listPay.FindIndex(c => c.PAY_DATE == searchDate) + 1;
            var periodNum = index;
            var monthNum = 1;

            periodNum = periodNum % 3 == 0 ? 3 : periodNum % 3;

            for (int i = 1; i <= index; i++)
            {
                if (i % 3 == 0)
                    monthNum += 1;
            }

            if (index <= 3)
            {
                return "Kỳ " + periodNum;
            }
            else
            {
                if (periodNum % 3 == 1)
                    return "Hết hạn T" + (monthNum - 1);
                else
                {
                    if (periodNum % 3 == 0)
                        return "Kỳ " + periodNum + " - T" + (monthNum - 1);
                    else
                        return "Kỳ " + periodNum + " - T" + monthNum;
                }
            }
        }

        private static string ReBuildRentTypeName(CONTRACT_FULL_VW con)
        {
            switch (con.RENT_TYPE_ID)
            {
                case 1:
                    return con.RENT_TYPE_NAME = "Thuê xe";
                case 2:
                    return con.RENT_TYPE_NAME = "TBVP";
                case 3:
                    return con.RENT_TYPE_NAME = "Khác";
                default:
                    return string.Empty;
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

        public static void RemoveRedundantPeriod()
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
                        int index = lstPay.Count - 1;
                        for (int i = 1; i <= div; i++)
                        {
                            db.PayPeriods.Remove(lstPay[index]);
                            index -= 1;
                        }
                    }
                }
                db.SaveChanges();
            }
        }

        public static void UpdateAllPeriodHavingOverFee()
        {
            using (var db = new RentBikeEntities())
            {
                var contracts = db.Contracts.Where(c => c.CONTRACT_STATUS == true).ToList();
                foreach (var contract in contracts)
                {
                    List<PayPeriod> payList = db.PayPeriods.Where(c => c.CONTRACT_ID == contract.ID).ToList();
                    decimal totalActualPay = payList.Select(c => c.ACTUAL_PAY).DefaultIfEmpty(0).Sum();
                    decimal totalPlanPay = payList.Select(c => c.AMOUNT_PER_PERIOD).DefaultIfEmpty(0).Sum();

                    if (totalActualPay > totalPlanPay)
                    {
                        CommonList.CreatePayPeriod(db, contract.ID, payList.LastOrDefault().PAY_DATE, false);
                    }
                }
            }
        }

        public static void UpdatePeriodAmount()
        {
            using (var db = new RentBikeEntities())
            {
                var contracts = db.Contracts.Where(c => c.CONTRACT_STATUS == true).ToList();
                foreach (var contract in contracts)
                {
                    List<PayPeriod> payList = db.PayPeriods.Where(c => c.CONTRACT_ID == contract.ID).ToList();
                    foreach (PayPeriod pp in payList)
                    {
                        var inOutList = db.InOuts.Where(c => c.PERIOD_ID == pp.ID);
                        decimal total = inOutList.Select(c => c.IN_AMOUNT).DefaultIfEmpty(0).Sum();
                        pp.ACTUAL_PAY = total;
                    }
                }
                db.SaveChanges();
            }
        }

        public static void UpdatePeriodAmount1()
        {
            using (var db = new RentBikeEntities())
            {
                var contracts = db.Contracts.Where(c => c.CONTRACT_STATUS == true).ToList();
                foreach (var contract in contracts)
                {
                    if (contract.FEE_PER_DAY == 0)
                    {
                        List<PayPeriod> payList = db.PayPeriods.Where(c => c.CONTRACT_ID == contract.ID && c.AMOUNT_PER_PERIOD > 0).ToList();
                        foreach (PayPeriod pp in payList)
                        {
                            pp.AMOUNT_PER_PERIOD = 0;
                        }
                    }
                }
                db.SaveChanges();
            }
        }

        public static void SaveSummaryPayFeeDaily()
        {
            using (var db = new RentBikeEntities())
            {
                try
                {
                    var stores = db.Stores.Where(c => c.ACTIVE == true).ToList();
                    foreach (var store in stores)
                    {
                        List<CONTRACT_FULL_VW> listContract = GetWarningData(DateTime.Today.ToString(), string.Empty, store.ID).Where(c => c.OVER_DATE <= 50).ToList();
                        foreach (var contract in listContract)
                        {
                            if (db.SummaryPayFeeDailies.Any(c => c.PERIOD_DATE == DateTime.Today
                                && c.STORE_ID == store.ID))
                                continue;

                            SummaryPayFeeDaily sum = new SummaryPayFeeDaily();
                            sum.CONTRACT_ID = contract.ID;
                            sum.CONTRACT_NO = contract.CONTRACT_NO;
                            sum.CUSTOMER_NAME = contract.CUSTOMER_NAME;
                            sum.PHONE = contract.PHONE;
                            sum.RENT_TYPE_ID = contract.RENT_TYPE_ID;
                            sum.RENT_TYPE_NAME = contract.RENT_TYPE_NAME;
                            sum.PERIOD_DATE = DateTime.Today;
                            sum.PAY_FEE = contract.FEE_PER_DAY;
                            sum.PAY_TIME = contract.PAYED_TIME;
                            sum.PAY_MESSAGE = contract.PERIOD_MESSAGE;
                            sum.STORE_ID = contract.STORE_ID;
                            sum.STORE_NAME = contract.STORE_NAME;
                            sum.NOTE = contract.NOTE;
                            sum.SEARCH_TEXT = contract.SEARCH_TEXT;
                            sum.CREATED_DATE = DateTime.Now;
                            sum.UPDATED_DATE = DateTime.Now;

                            db.SummaryPayFeeDailies.Add(sum);
                        }
                    }
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Logger.Log(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State));
                        foreach (var ve in eve.ValidationErrors)
                        {
                           Logger.Log(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage));
                        }
                    }
                    throw;
                }
            }
        }

        public static List<SummaryPayFeeDaily> GetSummaryPayFeeDailyData(DateTime startDate, DateTime endDate, string searchText, int storeId)
        {
            using (var db = new RentBikeEntities())
            {
                IQueryable<SummaryPayFeeDaily> sumList = db.SummaryPayFeeDailies;
                if (storeId != 0)
                {
                    sumList = sumList.Where(c => c.STORE_ID == storeId);
                }

                if (startDate != null && endDate > startDate)
                {
                    sumList = sumList.Where(c => c.PERIOD_DATE >= startDate && c.PERIOD_DATE <= endDate);
                }
                else
                {
                    sumList = sumList.Where(c => c.PERIOD_DATE == startDate);
                }

                if (!string.IsNullOrEmpty(searchText))
                {
                    sumList = sumList.Where(s => s.SEARCH_TEXT.ToLower().Contains(searchText.ToLower())
                        || s.CUSTOMER_NAME.ToLower().Contains(searchText.ToLower()));
                }

                return sumList.ToList();
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

                System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(directory);
                directoryInfo.Empty();

                // Here the procedure is called and executes successfully
                db.Database.ExecuteSqlCommand(System.Data.Entity.TransactionalBehavior.DoNotEnsureTransaction, "EXEC [dbo].[BackUp] @path = N'" + fileName + "'");
            }

        }
        #endregion
    }
}