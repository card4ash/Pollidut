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
    
    public partial class EMPLOYEE_DISTRIBUTION_HOUSES
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int DistributionHouseId { get; set; }
        public Nullable<bool> Active { get; set; }
        public string Remarks { get; set; }
    
        public virtual DISTRIBUTION_HOUSES DISTRIBUTION_HOUSES { get; set; }
        public virtual EMPLOYEE EMPLOYEE { get; set; }
    }
}
