﻿using System;
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
        public static void AutoExtendContract(RentBikeEntities db, Contract contract)
        {
            if (contract != null)
            {
                DateTime endDate = contract.END_DATE;
                DateTime extendEndDate = contract.EXTEND_END_DATE == null ? contract.END_DATE.AddDays(-10) : contract.EXTEND_END_DATE.Value.AddDays(-10);

                int overDate = DateTime.Today.Subtract(extendEndDate).Days;
                if (overDate >= 0)
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
                        if (lastPayPeriod.PAY_DATE.Subtract(contract.EXTEND_END_DATE.Value.AddDays(-10)).Days > 0)
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