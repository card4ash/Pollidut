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
    
    public partial class TICKET_CATEGORY
    {
        public TICKET_CATEGORY()
        {
            this.ComplainTickets = new HashSet<ComplainTicket>();
        }
    
        public int CATEGORY_ID { get; set; }
        public string CATEGORY_NAME { get; set; }
    
        public virtual ICollection<ComplainTicket> ComplainTickets { get; set; }
    }
}