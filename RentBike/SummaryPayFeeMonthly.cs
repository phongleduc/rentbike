//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RentBike
{
    using System;
    using System.Collections.Generic;
    
    public partial class SummaryPayFeeMonthly
    {
        public int ID { get; set; }
        public decimal SUMMURY_FEE { get; set; }
        public int SUMMURY_FEE_TYPE { get; set; }
        public System.DateTime SUMMURY_DATE { get; set; }
        public int STORE_ID { get; set; }
        public string STORE_NAME { get; set; }
        public string NOTE { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
    }
}