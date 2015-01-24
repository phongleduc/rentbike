//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RentBike
{
    using System;
    using System.Collections.Generic;
    
    public partial class ContractHistory
    {
        public int ID { get; set; }
        public int CONTRACT_ID { get; set; }
        public int RENT_TYPE_ID { get; set; }
        public int STORE_ID { get; set; }
        public decimal FEE_PER_DAY { get; set; }
        public decimal CONTRACT_AMOUNT { get; set; }
        public System.DateTime RENT_DATE { get; set; }
        public System.DateTime END_DATE { get; set; }
        public string PAY_FEE_MESSAGE { get; set; }
        public string NOTE { get; set; }
        public int REFERENCE_ID { get; set; }
        public string REFERENCE_NAME { get; set; }
        public string ITEM_TYPE { get; set; }
        public string ITEM_LICENSE_NO { get; set; }
        public string SERIAL_1 { get; set; }
        public string SERIAL_2 { get; set; }
        public string DETAIL { get; set; }
        public int CUSTOMER_ID { get; set; }
        public string SEARCH_TEXT { get; set; }
        public bool CONTRACT_STATUS { get; set; }
        public System.DateTime CLOSE_CONTRACT_DATE { get; set; }
        public string REFERENCE_PHONE { get; set; }
        public string SCHOOL_NAME { get; set; }
        public string CLASS_NAME { get; set; }
        public string AUTO_CONTRACT_NO { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
    }
}
