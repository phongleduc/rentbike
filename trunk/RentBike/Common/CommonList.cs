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
        public const string ACTION_LOGIN = "Đăng nhập";
        public const string ACTION_LOGOUT = "Đăng xuất";

        public const string ACTION_CREATE_STORE = "Tạo cửa hàng";
        public const string ACTION_UPDATE_STORE = "Cập nhật cửa hàng";

        public const string ACTION_CREATE_CONTRACT = "Làm hợp đồng";
        public const string ACTION_UPDATE_CONTRACT = "Cập nhật hợp đồng";
        public const string ACTION_CLOSE_CONTRACT = "Cập nhật hợp đồng";

        public const string ACTION_CREATE_ACCOUNT = "Tạo tài khoản";
        public const string ACTION_UPDATE_ACCOUNT = "Cập nhật tài khoản";

        public const string ACTION_CREATE_TYPE = "Tạo danh mục";

        public const string ACTION_CREATE_INOUT = "Thu/Chi";

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

        public static string FormatedAsCurrency(int input)
        {
            return String.Format("{0:#,0.#} VNÐ", input);
        }

        public static string EncryptPassword(string strPassword)
        {
            string encrypted = string.Empty;
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strPassword));

            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            encrypted = strBuilder.ToString();

            return encrypted;
        }

        public static string ConvertByteImageToBase64String(byte[] data)
        {
            if (data != null && data.Length > 0)
            {
                StringBuilder base64Buidler = new StringBuilder();
                base64Buidler.Append("data:");

                base64Buidler.Append("image/png");
                base64Buidler.Append(";base64,");
                base64Buidler.Append(Convert.ToBase64String(data));
                return base64Buidler.ToString();
            }
            return string.Empty;
        }
        public static void AutoExtendContract(RentBikeEntities db, Contract contract)
        {
            if (contract != null)
            {
                DateTime endDate = contract.END_DATE;
                DateTime extendEndDate = contract.EXTEND_END_DATE == null ? contract.END_DATE.AddDays(-10) : contract.EXTEND_END_DATE.Value.AddDays(-10);

                int overDate = DateTime.Today.Subtract(extendEndDate).Days;
                if (overDate > 0)
                {
                    PayPeriod pp1;
                    PayPeriod pp2;
                    PayPeriod pp3;

                    int percentDate = overDate / 30;
                    int multipleFee = Convert.ToInt32(Decimal.Floor(contract.CONTRACT_AMOUNT / 100000));

                    DateTime endDateUpdated = extendEndDate.AddDays(30 * (percentDate + 1));
                    contract.EXTEND_END_DATE = endDateUpdated;
                    db.SaveChanges();

                    var listPayPeriod = db.PayPeriods.Where(c => c.CONTRACT_ID == contract.ID);
                    PayPeriod firstPayPeriod = listPayPeriod.OrderBy(c => c.PAY_DATE).FirstOrDefault();
                    PayPeriod lastPayPeriod = listPayPeriod.OrderByDescending(c => c.PAY_DATE).FirstOrDefault();
                    decimal increateFeeCar = firstPayPeriod.AMOUNT_PER_PERIOD + (multipleFee * 50 * 10);
                    decimal increateFeeEquip = firstPayPeriod.AMOUNT_PER_PERIOD + (multipleFee * 100 * 10);
                    decimal increateFeeOther = firstPayPeriod.AMOUNT_PER_PERIOD;

                    for (int i = 0; i <= percentDate; i++)
                    {
                        if (lastPayPeriod.PAY_DATE.Subtract(contract.EXTEND_END_DATE.Value.AddDays(-10)).Days >= 0)
                        {
                            break;
                        }

                        pp1 = new PayPeriod();
                        pp1.CONTRACT_ID = contract.ID;
                        pp1.PAY_DATE = lastPayPeriod.PAY_DATE.AddDays(10);
                        switch (contract.RENT_TYPE_ID)
                        {
                            case 1:
                                if (((contract.FEE_PER_DAY / multipleFee) * 10) < 4000)
                                    pp1.AMOUNT_PER_PERIOD = increateFeeCar;
                                else
                                    pp1.AMOUNT_PER_PERIOD = firstPayPeriod.AMOUNT_PER_PERIOD;
                                break;
                            case 2:
                                if (((contract.FEE_PER_DAY / multipleFee) * 10) < 6000)
                                    pp1.AMOUNT_PER_PERIOD = increateFeeEquip;
                                else
                                    pp1.AMOUNT_PER_PERIOD = firstPayPeriod.AMOUNT_PER_PERIOD;
                                break;
                            default:
                                pp1.AMOUNT_PER_PERIOD = increateFeeOther;
                                break;
                        }
                        pp1.STATUS = true;
                        pp1.ACTUAL_PAY = 0;

                        pp2 = new PayPeriod();
                        pp2.CONTRACT_ID = contract.ID;
                        pp2.PAY_DATE = pp1.PAY_DATE.AddDays(10);
                        pp2.AMOUNT_PER_PERIOD = pp1.AMOUNT_PER_PERIOD;
                        pp2.STATUS = true;
                        pp2.ACTUAL_PAY = 0;

                        pp3 = new PayPeriod();
                        pp3.CONTRACT_ID = contract.ID;
                        pp3.PAY_DATE = pp2.PAY_DATE.AddDays(10);
                        pp3.AMOUNT_PER_PERIOD = pp1.AMOUNT_PER_PERIOD;
                        pp3.STATUS = true;
                        pp3.ACTUAL_PAY = 0;

                        db.PayPeriods.Add(pp1);
                        db.PayPeriods.Add(pp2);
                        db.PayPeriods.Add(pp3);

                        db.SaveChanges();

                        lastPayPeriod = pp3;
                    }
                }
            }
        }
    }
}