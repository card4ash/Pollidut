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
    
    public partial class Note
    {
        public int ID { get; set; }
        public string ContentText { get; set; }
        public Nullable<int> Ticket { get; set; }
        public Nullable<int> StatusId { get; set; }
        public string Reply { get; set; }
        public Nullable<System.DateTime> OpenDate { get; set; }
        public Nullable<System.DateTime> CloseingDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ReplyBy { get; set; }
        public Nullable<System.DateTime> ReplyDate { get; set; }
        public string UserName { get; set; }
        public string Color { get; set; }
        public Nullable<int> Leftpx { get; set; }
        public Nullable<int> Toppx { get; set; }
        public Nullable<int> Zindexps { get; set; }
        public Nullable<bool> Active { get; set; }
        public string Remarks { get; set; }
    }
}
