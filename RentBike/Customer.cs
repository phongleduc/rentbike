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
    
    public partial class Customer
    {
        public int ID { get; set; }
        public string NAME { get; set; }
        public string LICENSE_NO { get; set; }
        public Nullable<System.DateTime> LICENSE_RANGE_DATE { get; set; }
        public string LICENSE_RANGE_PLACE { get; set; }
        public string PERMANENT_RESIDENCE { get; set; }
        public string CURRENT_RESIDENCE { get; set; }
        public Nullable<System.DateTime> BIRTH_DAY { get; set; }
        public string EMAIL { get; set; }
        public string PHONE { get; set; }
        public string ADDRESS { get; set; }
        public int CITY_ID { get; set; }
        public byte[] PHOTO { get; set; }
    }
}
