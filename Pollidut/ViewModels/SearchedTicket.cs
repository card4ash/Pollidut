using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pollidut.ViewModels
{
    public class SearchedTicket
    {
        public int TicketId { get; set; }
        public string Status { get; set; }
        public string Duration { get; set; }
        public int TotalConversation { get; set; }
        public string LastRepliedFrom { get; set; }
    }
}