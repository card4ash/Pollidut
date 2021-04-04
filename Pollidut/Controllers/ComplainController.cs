using Pollidut.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Pollidut.ViewModels;

namespace Pollidut.Controllers
{
    public class ComplainController : Controller
    {
        // GET: Complain
        public ActionResult Index()
        {
            return View();
        }
        //Ticket Creation By Client
        public ActionResult CreateTicket()
        {
            int empId = Convert.ToInt32(Session["EmployeeId"].ToString());
            using (DataAccess.PollidutEntities db = new PollidutEntities())
            {
                var Category = (from b in db.TICKET_CATEGORY select b).ToList();
                ViewBag.Category = Category;
            }
            return View("CreateTicket");
        }
        [HttpPost]
        public JsonResult CreateNewComplain(ComplainTicket ticket)
        {
            using(PollidutEntities db=new PollidutEntities())
            {
                int empId = Convert.ToInt32(Session["EmployeeId"].ToString());
                DateTime dt=DateTime.Now;
                var tkt=new ComplainTicket()
                {
                    StatusId=1,
                    CategoryId=ticket.CategoryId,
                    SubCategoryId=ticket.SubCategoryId,
                    Mobile=ticket.Mobile,
                    Title=ticket.Title,
                    ContentText=ticket.ContentText,
                    isNotifyByEmail=ticket.isNotifyByEmail,
                    CreatedBy=empId,
                    Active=true,
                    OpenDate=dt
                };
                db.ComplainTickets.Add(tkt);
                db.SaveChanges();
            }
            
            return Json("success");
        }
        //Search Ticket By Support Center
        public ActionResult SearchTicket()
        {
            using (DataAccess.PollidutEntities db = new PollidutEntities())
            {
                var TicketStatus = (from b in db.TICKET_STATUS select b).ToList();
                ViewBag.TicketStatus = TicketStatus;
                var Category = (from b in db.TICKET_CATEGORY select b).ToList();
                ViewBag.Category = Category;
            }
            return View("SearchTicket");
        }
        public JsonResult getTickets(int statusId=0,int CategoryId=0,int SubCategoryId=0)
        {
            int empId = Convert.ToInt32(Session["EmployeeId"].ToString());
            List<SearchedTicket> tickets = new List<SearchedTicket>();
            using (PollidutEntities db = new PollidutEntities())
            {
                var searched = (from a in db.ComplainTickets where a.CreatedBy == empId select a);
                if (statusId > 0)
                {
                    searched = (from b in searched where b.StatusId == statusId select b);
                }
                if (CategoryId > 0)
                {
                    searched = (from b in searched where b.CategoryId == CategoryId select b);
                }
                if (SubCategoryId > 0)
                {
                    searched = (from b in searched where b.SubCategoryId ==SubCategoryId select b);
                }
                var Filterddata = (from d in searched
                                   select new
                                       {
                                           d.TicketId,
                                           d.TICKET_STATUS.StatusName,
                                           d.OpenDate,
                                           d.CloseDate
                                       }).ToList();
                foreach (var eachTicket in Filterddata)
                {
                    try
                    {
                        SearchedTicket ticket = new SearchedTicket();
                        int TotalConversation = (from a in db.TICKET_REPLIED where a.TicketId == eachTicket.TicketId select a).Count();
                        DateTime dt = DateTime.Now;
                        DateTime d1 = eachTicket.OpenDate == null ? dt : (DateTime)eachTicket.OpenDate;
                        DateTime d2 = eachTicket.CloseDate == null ? dt : (DateTime)eachTicket.CloseDate;
                        TimeSpan ts = d1.Subtract(d2);
                        string Difference = ts.Days.ToString() + " D. " + ts.Hours + " H. " + ts.Minutes + " M.";
                        var lastReplied = (from a in db.TICKET_REPLIED join b in db.EMPLOYEES on a.RepliedBy equals b.EMPLOYEE_ID join c in db.PERSONS on b.PERSON_ID equals c.PERSON_ID where a.TicketId == eachTicket.TicketId orderby a.RepliedTime descending select c.PERSON_NAME ).FirstOrDefault();

                        string LastRepliedFrom = "Mr. Nazmul";
                        if (lastReplied != null)
                        {
                            LastRepliedFrom = lastReplied;
                        }
                        ticket.TicketId = eachTicket.TicketId;
                        ticket.Status = eachTicket.StatusName;
                        ticket.TotalConversation = TotalConversation;
                        ticket.Duration = Difference;
                        ticket.LastRepliedFrom = LastRepliedFrom;
                        tickets.Add(ticket);
                    }
                    catch (Exception ex)
                    {
                        return Json(new { msg="failure"});
                    }
                    
                }
            }
            return Json(tickets);
        }
        //Ticket Details Open By Support Center
        public ActionResult TicketDetails(int ticketId)
        {
            return View();
        }
        public JsonResult getSubCategory(int categoryId)
        {
            using (PollidutEntities db = new PollidutEntities())
            {
                var data = (from a in db.TICKET_SUB_CATEGORY where a.CATEGORY_ID == categoryId select new
                {
                    SUB_CATEGORY_ID=a.SUB_CATEGORY_ID,
                    SUB_CATEGORY_NAME=a.SUB_CATEGORY_NAME
                }).ToList();
                return Json(data);
            }
        }
    }
}