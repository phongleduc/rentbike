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
    
    public partial class Account
    {
        public int ID { get; set; }
        public string ACC { get; set; }
        public string PASSWORD { get; set; }
        public int PERMISSION_ID { get; set; }
        public int STORE_ID { get; set; }
        public string NAME { get; set; }
        public string PHONE { get; set; }
        public int CITY_ID { get; set; }
        public System.DateTime REGISTER_DATE { get; set; }
        public bool ACTIVE { get; set; }
        public string NOTE { get; set; }
        public string SEARCH_TEXT { get; set; }
    }
}
