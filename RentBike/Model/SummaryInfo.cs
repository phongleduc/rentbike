using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RentBike.Model
{
    public class SummaryInfo
    {
        public SummaryInfo()
        { }

        public int StoreId { get; set; }
        public DateTime InOutDate { get; set; }
        public string RentTypeName { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public decimal BeginAmount { get; set; }
        public decimal EndAmount { get; set; }
        public decimal TotalIn { get; set; }
        public decimal TotalOut { get; set; }
        public decimal ContractFee { get; set; }
        public decimal ContractFeeCar { get; set; }
        public decimal RentFee { get; set; }
        public decimal RentFeeCar { get; set; }
        public decimal CloseFee { get; set; }
        public decimal CloseFeeCar { get; set; }
        public decimal ContractFeeEquip { get; set; }
        public decimal RentFeeEquip { get; set; }
        public decimal CloseFeeEquip { get; set; }
        public decimal ContractFeeOther { get; set; }
        public decimal RentFeeOther { get; set; }
        public decimal CloseFeeOther { get; set; }
        public decimal RemainEndOfDay { get; set; }
        public decimal InCapital { get; set; }
        public decimal OutCapital { get; set; }
        public decimal InOther { get; set; }
        public decimal OutOther { get; set; }
        public decimal RedundantFee { get; set; }
        public decimal RedundantFeeCar { get; set; }
        public decimal RedundantFeeEquip { get; set; }
        public decimal RedundantFeeOther { get; set; }


    }
}