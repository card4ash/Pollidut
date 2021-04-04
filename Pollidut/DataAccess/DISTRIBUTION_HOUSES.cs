//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Pollidut.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class DISTRIBUTION_HOUSES
    {
        public DISTRIBUTION_HOUSES()
        {
            this.EMPLOYEE_DISTRIBUTION_HOUSES = new HashSet<EMPLOYEE_DISTRIBUTION_HOUSES>();
            this.EMPLOYEES = new HashSet<EMPLOYEE>();
        }
    
        public Nullable<int> DISTRIBUTION_HOUSE_MAIN_ID { get; set; }
        public Nullable<int> TERRITORY_ID { get; set; }
        public int AREA_ID { get; set; }
        public int DISTRIBUTION_HOUSE_ID { get; set; }
        public string DISTRIBUTION_HOUSE_NAME { get; set; }
        public Nullable<int> DISTRIBUTION_HOUSE_TYPE_ID { get; set; }
        public Nullable<int> AC_ID { get; set; }
        public Nullable<int> LOCATION_ID { get; set; }
        public string ADDRESS { get; set; }
        public Nullable<int> DISTRICT_ID { get; set; }
        public Nullable<int> THANA_ID { get; set; }
        public string PHONE { get; set; }
        public string DISTRIBUTOR_NAME { get; set; }
        public string DISTRIBUTOR_MOBILE { get; set; }
        public string KCP_NAME { get; set; }
        public string KCP_MOBILE { get; set; }
        public string KCP_EMAIL { get; set; }
        public Nullable<int> DESIGNATION_ID { get; set; }
        public Nullable<bool> ACTIVE { get; set; }
        public string REMARKS { get; set; }
        public Nullable<decimal> LAT { get; set; }
        public Nullable<decimal> LON { get; set; }
    
        public virtual AREA AREA { get; set; }
        public virtual ICollection<EMPLOYEE_DISTRIBUTION_HOUSES> EMPLOYEE_DISTRIBUTION_HOUSES { get; set; }
        public virtual ICollection<EMPLOYEE> EMPLOYEES { get; set; }
    }
}
