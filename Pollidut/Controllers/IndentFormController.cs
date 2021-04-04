using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Pollidut.Models;

namespace Pollidut.Controllers
{
    public class IndentFormController : Controller
    {
        // GET: IndentForm
        public ActionResult Index()
        {
            int userTypeId = Convert.ToInt32(Session["UserTypeId"].ToString());
            int empId = Convert.ToInt32(Session["EmployeeId"].ToString());
            //pollidut supervisor check
            if (userTypeId == 3)
            {
                List<Pollidut.Models.Pollidut> pdlist = new List<Models.Pollidut>();
                List<Pollidut.Models.DistributionHouse> distributionList = new List<Models.DistributionHouse>();
                List<Pollidut.Models.ProductSKU> pdSKUs = new List<Models.ProductSKU>();
                string SupName="";
                try 
                {
                    using (DataAccess.PollidutEntities context = new DataAccess.PollidutEntities())
                    {
                        pdlist = (from a in context.EMPLOYEES
                                  join b in context.PERSONS on a.PERSON_ID equals b.PERSON_ID
                                  where a.SUPERVISOR_ID == empId
                                  select new Pollidut.Models.Pollidut
                                  {
                                      PollidutId = a.EMPLOYEE_ID,
                                      PollidutName = b.PERSON_NAME,
                                      DistributionHouseId = a.DISTRIBUTION_HOUSE_ID == null ? 0 : (int)a.DISTRIBUTION_HOUSE_ID
                                  }).ToList();
                        distributionList = (from c in context.EMPLOYEE_DISTRIBUTION_HOUSES
                                            join d in context.DISTRIBUTION_HOUSES on c.DistributionHouseId equals d.DISTRIBUTION_HOUSE_ID
                                            where c.EmployeeId == empId
                                            select new Pollidut.Models.DistributionHouse
                                            {
                                                DistributionHouseId = c.DistributionHouseId.ToString(),
                                                DistributionHouseName = d.DISTRIBUTION_HOUSE_NAME
                                            }).ToList();
                        SupName = (from x in context.EMPLOYEES join y in context.PERSONS on x.PERSON_ID equals y.PERSON_ID where x.EMPLOYEE_ID == empId select y.PERSON_NAME).FirstOrDefault();
                        DateTime dt = DateTime.Now.Date;
                        var Offer = (from o in context.JCOffers where o.OfferStartDate <= dt && o.OfferEndDate >= dt select o);
                        //var Product_SKUs = (from psku in context.PRODUCT_SKU where psku.ProductSKUId > 0 select psku).ToList();
                        pdSKUs = (from m in context.PRODUCT_SKU
                                  join ofr in Offer on m.ProductSKUId equals ofr.ProductSKUId
                                  into temp
                                  from j in temp.DefaultIfEmpty()
                                  where m.ProductSKUId > 0
                                  select new Pollidut.Models.ProductSKU
                                  {
                                      ProductSKUId = m.ProductSKUId,
                                      ProductSKUName = m.ProductSKUName,
                                      UnitPrice = m.UnitPrice == null ? 0 : (double)m.UnitPrice,
                                      PiecePerCarton=m.PiecePerCarton==null?0:(int)m.PiecePerCarton,
                                      JCOffer = j.JCOfferValue == null ? 0 : (int)j.JCOfferValue,
                                      JCOfferType = j.JCOfferUnit == null ? 0 : (int)j.JCOfferUnit,
                                     // OutOf=j.
                                      DDAmount = j.JCOfferUnit == null ? 0 : j.JCOfferUnit == 1 ? (int)j.JCOfferValue : (int)m.UnitPrice * (int)j.JCOfferValue
                                  }).ToList();
                    }
                }
                catch (Exception e)
                {
                    throw e;

                }
                
                ViewBag.PDList = pdlist;
                ViewBag.DistributionHouses = distributionList;
                ViewBag.PDSKUs = pdSKUs;
                ViewBag.SupervisorName = SupName;
                ViewBag.SupId = empId;
            }
            return View();
        }

        public ActionResult SearchForm()
        {
            int userTypeId = Convert.ToInt32(Session["UserTypeId"].ToString());
            int empId = Convert.ToInt32(Session["EmployeeId"].ToString());
            //pollidut supervisor check
            if (userTypeId == 3)
            {
                List<Pollidut.Models.Pollidut> pdlist = new List<Models.Pollidut>();
                List<Pollidut.Models.DistributionHouse> distributionList = new List<Models.DistributionHouse>();
                List<Pollidut.Models.ProductSKU> pdSKUs = new List<Models.ProductSKU>();
                string SupName = "";
                try
                {
                    using (DataAccess.PollidutEntities context = new DataAccess.PollidutEntities())
                    {
                        pdlist = (from a in context.EMPLOYEES
                                  join b in context.PERSONS on a.PERSON_ID equals b.PERSON_ID
                                  where a.SUPERVISOR_ID == empId
                                  select new Pollidut.Models.Pollidut
                                  {
                                      PollidutId = a.EMPLOYEE_ID,
                                      PollidutName = b.PERSON_NAME,
                                      DistributionHouseId = a.DISTRIBUTION_HOUSE_ID == null ? 0 : (int)a.DISTRIBUTION_HOUSE_ID
                                  }).ToList();
                        distributionList = (from c in context.EMPLOYEE_DISTRIBUTION_HOUSES
                                            join d in context.DISTRIBUTION_HOUSES on c.DistributionHouseId equals d.DISTRIBUTION_HOUSE_ID
                                            where c.EmployeeId == empId
                                            select new Pollidut.Models.DistributionHouse
                                            {
                                                DistributionHouseId = c.DistributionHouseId.ToString(),
                                                DistributionHouseName = d.DISTRIBUTION_HOUSE_NAME
                                            }).ToList();
                        SupName = (from x in context.EMPLOYEES join y in context.PERSONS on x.PERSON_ID equals y.PERSON_ID where x.EMPLOYEE_ID == empId select y.PERSON_NAME).FirstOrDefault();
                        DateTime dt = DateTime.Now.Date;
                        var Offer = (from o in context.JCOffers where o.OfferStartDate <= dt && o.OfferEndDate >= dt select o);
                        pdSKUs = (from m in context.PRODUCT_SKU
                                  join ofr in Offer on m.ProductSKUId equals ofr.ProductSKUId
                                  into temp
                                  from j in temp.DefaultIfEmpty()
                                  where m.ProductSKUId > 0
                                  select new Pollidut.Models.ProductSKU
                                  {
                                      ProductSKUId = m.ProductSKUId,
                                      ProductSKUName = m.ProductSKUName,
                                      UnitPrice = m.UnitPrice == null ? 0 : (double)m.UnitPrice,
                                      PiecePerCarton = m.PiecePerCarton == null ? 0 : (int)m.PiecePerCarton,
                                      JCOffer = j.JCOfferValue == null ? 0 : (int)j.JCOfferValue,
                                      JCOfferType = j.JCOfferUnit == null ? 0 : (int)j.JCOfferUnit,
                                      OutOf = j.OutOf == null ? 0 : (int)j.OutOf,
                                      OutOfType = j.OutOfType == null ? 0 : (int)j.OutOfType,
                                  }).ToList();
                    }
                }
                catch (Exception e)
                {
                    throw e;

                }
                ViewBag.PDSKUs = pdSKUs;
                ViewBag.PDList = pdlist;
                ViewBag.DistributionHouses = distributionList;
                ViewBag.SupervisorName = SupName;
                ViewBag.SupId = empId;
            }
            return View();
        }
        public ActionResult DeliveryDtail(int formId)
        {
            List<Pollidut.Models.ProductSKU> pdSKUs = new List<Models.ProductSKU>();
            using (DataAccess.PollidutEntities context = new DataAccess.PollidutEntities())
            {
                var prdSkuDeliveryList = (from p in context.FORM_ROW_ITEM where p.FormId == formId select p).ToList();
                DateTime dt = DateTime.Now.Date;
                var Offer = (from o in context.JCOffers where o.OfferStartDate <= dt && o.OfferEndDate >= dt select o);
                pdSKUs = (from m in context.PRODUCT_SKU
                          join ofr in Offer on m.ProductSKUId equals ofr.ProductSKUId
                          into temp
                          from j in temp.DefaultIfEmpty()
                          where m.ProductSKUId > 0
                          select new Pollidut.Models.ProductSKU
                          {
                              ProductSKUId = m.ProductSKUId,
                              ProductSKUName = m.ProductSKUName,
                              UnitPrice = m.UnitPrice == null ? 0 : (double)m.UnitPrice,
                              PiecePerCarton = m.PiecePerCarton == null ? 0 : (int)m.PiecePerCarton,
                              JCOffer = j.JCOfferValue == null ? 0 : (int)j.JCOfferValue,
                              JCOfferType = j.JCOfferUnit == null ? 0 : (int)j.JCOfferUnit,
                              OutOf = j.OutOf == null ? 0 : (int)j.OutOf,
                              OutOfType = j.OutOfType == null ? 0 : (int)j.OutOfType,
                          }).ToList();
                var data = (from x in prdSkuDeliveryList
                            join y in pdSKUs on x.ProductSKUId equals y.ProductSKUId
                            select new
                                {
                                    Id = x.RowId,
                                    ProductSKUId = x.ProductSKUId,
                                    ProductSKUName = y.ProductSKUName,
                                    Icarton=x.IndentCartonQty,
                                    Ipiece=x.IndentPieceQty,
                                    Ioffer=x.JCOffer,
                                    Dcarton = x.DeliveryCartoonQty == null ? "" : x.DeliveryCartoonQty.ToString(),
                                    Dpiece=x.DeliveryPieceQty==null?"":x.DeliveryPieceQty.ToString(),
                                    DOffer=x.DeliveryOffer==null?"":x.DeliveryOffer.ToString(),
                                    Dtprice = x.DTotalPrice == null ? "" : x.DTotalPrice.ToString(),
                                    DddAmount = x.DDDAmount == null ? "" : x.DDDAmount.ToString(),
                                    Dnprice = x.DNetPrice == null ? "" : x.DNetPrice.ToString(),
                                    IddAmount=x.IDDAmount,
                                    UnitPrice = y.UnitPrice,
                                    PiecePerCarton = y.PiecePerCarton,
                                    JCOffer = y.JCOffer,
                                    JCOfferType = y.JCOfferType,
                                    OutOf = y.OutOf,
                                    OutOfType = y.OutOfType,
                                }).ToList();
                return Json(new { msg = "success",data=data });
            }
        }
        [HttpPost]
        public ActionResult Search(int pdId,int supId, int distributionId,string orderDatefrom,string orderDateto,string formCode)
        {
            DateTime OrderDateFrom = DateTime.Now.Date;
            DateTime OrderDateTo = DateTime.Now.Date;
            if (orderDatefrom != "")
            {
                Boolean isStartDateValid = DateTime.TryParseExact(orderDatefrom, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out OrderDateFrom);
                if (!isStartDateValid)
                {
                    return Json(new { status = "error", details = "Order date (From) is invalid. It must be in dd/MM/yyyy format." }, JsonRequestBehavior.AllowGet);
                }
            }
            if (orderDateto != "")
            {
                Boolean isStartDateValid = DateTime.TryParseExact(orderDateto, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out OrderDateTo);
                if (!isStartDateValid)
                {
                    return Json(new { status = "error", details = "Order date (To) is invalid. It must be in dd/MM/yyyy format." }, JsonRequestBehavior.AllowGet);
                }
            }
            TimeSpan ts = new TimeSpan(23, 59, 59);
            OrderDateTo=OrderDateTo.Add(ts);
            using (DataAccess.PollidutEntities db = new DataAccess.PollidutEntities())
            {
                var result = from x in db.PD_PRODUCT_INDENT_FORM  where x.OrderDate>=OrderDateFrom && x.OrderDate<=OrderDateTo select x;
                if (pdId > 0) 
                {
                    result = from p in result where p.PollidutId == pdId select p;
                }
                if (supId > 0)
                {
                    result = from s in result where s.SupervisorId == supId select s;
                }
                if (distributionId > 0)
                {
                    result = from d in result where d.DistributionHouseId == distributionId select d;
                }
                if(!string.IsNullOrEmpty(formCode))
                {
                    result = from f in result where f.FormCode.Contains(@"/" + formCode + "/") select f;
                }
                var data = (from r in result
                            join h in db.DISTRIBUTION_HOUSES on r.DistributionHouseId equals h.DISTRIBUTION_HOUSE_ID
                            join e in db.EMPLOYEES on r.PollidutId equals e.EMPLOYEE_ID
                            join p in db.PERSONS on e.PERSON_ID equals p.PERSON_ID
                            select new
                                {
                                    ID=r.FormId,
                                    Name = p.PERSON_NAME,
                                    Distribution = h.DISTRIBUTION_HOUSE_NAME,
                                    OrderDate = r.OrderDate,
                                    DeliveryDate = r.DeliveryDate,
                                    FormCode = r.FormCode
                                }).ToList().Select(x => new SearchResult
                                {
                                    ID=x.ID,
                                    Name = x.Name,
                                    Distribution = x.Distribution,
                                    OrderDate = GetFormattedString(x.OrderDate),
                                    DeliveryDate = GetFormattedString(x.DeliveryDate),
                                    FormCode = x.FormCode
                                }).ToList();
                return Json(new { msg = "success", data = data });
            }
            
        }
        public string GetFormattedString(DateTime? dt)
        {
            if (dt == null) return "";
            else
            {
                DateTime ndt = (DateTime)dt;
                return ndt.ToString("dd-MM-yyyy");
            }
                
        }
        public class SearchResult
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Distribution { get; set; }
            public string OrderDate { get; set; }
            public string DeliveryDate { get; set; }
            public string FormCode { get; set; }
        }
        [HttpPost]
        public ActionResult SaveForm(IndentForm iForm, List<RowItem> rowItems)
        {
            if (iForm != null)
            {
                DateTime OrderDate = DateTime.Now.Date; 
                DateTime CreatedDate = DateTime.Now;
                if (iForm.OrderDate != "")
                {
                    Boolean isStartDateValid = DateTime.TryParseExact(iForm.OrderDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out OrderDate);
                    if (!isStartDateValid)
                    {
                        return Json(new { status = "error", details = "Start date is invalid. It must be in dd/MM/yyyy format." }, JsonRequestBehavior.AllowGet);
                    }
                }
                try
                {
                    using (DataAccess.PollidutEntities db = new DataAccess.PollidutEntities())
                    {
                        var iform = new DataAccess.PD_PRODUCT_INDENT_FORM
                        {
                            PollidutId = iForm.PollidutId,
                            SupervisorId = iForm.SupervisorId,
                            DistributionHouseId = iForm.DistributionHouseId,
                            FormCode = iForm.FormCode,
                            TotalPrice=(decimal)iForm.TotalPrice,
                            OrderDate = OrderDate,
                            CreatedBy = iForm.SupervisorId,
                            CreatedDate = CreatedDate,
                            Active = true
                        };
                        db.PD_PRODUCT_INDENT_FORM.Add(iform);
                        db.SaveChanges();
                        foreach (var row in rowItems)
                        {
                            var iformRow = new DataAccess.FORM_ROW_ITEM
                            {
                                ProductSKUId = row.ProductSKUId,
                                FormId = iform.FormId,
                                UnitPrice = (decimal)row.UnitPrice,
                                JCOffer = row.JCOffer,
                                IndentCartonQty = row.IndentCartonQty,
                                IndentPieceQty = row.IndentPieceQty,
                                ITotalPrice = (decimal)row.TotalPrice,
                                INetPrice = (decimal)row.NetPrice,
                                IDDAmount = (decimal)row.DDAmount
                            };
                            db.FORM_ROW_ITEM.Add(iformRow);
                        }
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return Json(new { msg = "success" });
        }

        [HttpPost]
        public ActionResult SaveDelivery(IndentForm dForm,List<RowItem> rowItems)
        {
            
            DateTime DeliveryDate = DateTime.Now.Date;
            DateTime CreatedDate = DateTime.Now;
            if (dForm.DeliveryDate != "")
            {
                Boolean isStartDateValid = DateTime.TryParseExact(dForm.DeliveryDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out DeliveryDate);
                if (!isStartDateValid)
                {
                    return Json(new { status = "error", details = "Delivery Date is invalid. It must be in dd/MM/yyyy format." }, JsonRequestBehavior.AllowGet);
                }
            }
            using (DataAccess.PollidutEntities db = new DataAccess.PollidutEntities())
            {
                var uform = (from f in db.PD_PRODUCT_INDENT_FORM where f.FormId == dForm.FormId select f).FirstOrDefault();
                if (uform != null)
                {
                    uform.DeliveryDate = DeliveryDate;
                    uform.DeliveryTotalPrice =(decimal)dForm.DTotalPrice;
                    db.SaveChanges();
                    foreach(var item in rowItems)
                    {
                        if (item.RowId > 0)
                        {
                            //update
                            var tblRow = (from r in db.FORM_ROW_ITEM where r.RowId == item.RowId select r).FirstOrDefault();
                            if (tblRow != null)
                            {
                                tblRow.DeliveryUnitPrice = (decimal)item.DUnitPrice;
                                tblRow.DeliveryCartoonQty = item.DeliveryCartonQty;
                                tblRow.DeliveryPieceQty = item.DeliveryPieceQty;
                                tblRow.DeliveryOffer = item.DJCOffer;
                                tblRow.DTotalPrice = (decimal)item.DTotalPrice;
                                tblRow.DDDAmount = (decimal)item.DDDAmount;
                                tblRow.DNetPrice = (decimal)item.DNetPrice;
                                db.SaveChanges();
                            }
                        }
                        else if(item.RowId==0)
                        {
                            //insert
                            var formRownew = new DataAccess.FORM_ROW_ITEM
                            {
                                FormId=uform.FormId,
                                ProductSKUId = item.ProductSKUId,
                                UnitPrice = (decimal)item.UnitPrice,
                                DeliveryUnitPrice = (decimal)item.DUnitPrice,
                                JCOffer=0,
                                IndentCartonQty=0,
                                IndentPieceQty=0,
                                DeliveryCartoonQty=item.DeliveryCartonQty,
                                DeliveryPieceQty=item.DeliveryPieceQty,
                                DeliveryOffer=item.DJCOffer,
                                ITotalPrice=0,
                                DTotalPrice = (decimal)item.DTotalPrice,
                                IDDAmount=0,
                                DDDAmount = (decimal)item.DDDAmount,
                                INetPrice=0,
                                DNetPrice = (decimal)item.DNetPrice,
                                Active = true,
                                CreatedDate = CreatedDate,
                                ModifiedDate=CreatedDate
                            };
                            db.FORM_ROW_ITEM.Add(formRownew);
                            db.SaveChanges();
                        }
                        else
                        {
                            //something goes wrong.
                        }
                    }
                }
            }

            return Json(new {msg="success" });
        }

        public ActionResult Summary()
        {
            int userTypeId = Convert.ToInt32(Session["UserTypeId"].ToString());
            int empId = Convert.ToInt32(Session["EmployeeId"].ToString());
            if(userTypeId==3)
            {
                List<Pollidut.Models.Pollidut> pdlist = new List<Models.Pollidut>();
                List<Pollidut.Models.DistributionHouse> distributionList = new List<Models.DistributionHouse>();

                string SupName = "";
                try
                {
                    using (DataAccess.PollidutEntities context = new DataAccess.PollidutEntities())
                    {
                        pdlist = (from a in context.EMPLOYEES
                                  join b in context.PERSONS on a.PERSON_ID equals b.PERSON_ID
                                  where a.SUPERVISOR_ID == empId
                                  select new Pollidut.Models.Pollidut
                                  {
                                      PollidutId = a.EMPLOYEE_ID,
                                      PollidutName = b.PERSON_NAME,
                                      DistributionHouseId = a.DISTRIBUTION_HOUSE_ID == null ? 0 : (int)a.DISTRIBUTION_HOUSE_ID
                                  }).ToList();
                        DateTime dt = DateTime.Now.Date;

                        distributionList = (from c in context.EMPLOYEE_DISTRIBUTION_HOUSES
                                            join d in context.DISTRIBUTION_HOUSES on c.DistributionHouseId equals d.DISTRIBUTION_HOUSE_ID
                                            where c.EmployeeId == empId
                                            select new Pollidut.Models.DistributionHouse
                                            {
                                                DistributionHouseId = c.DistributionHouseId.ToString(),
                                                DistributionHouseName = d.DISTRIBUTION_HOUSE_NAME
                                            }).ToList();
                        SupName = (from x in context.EMPLOYEES join y in context.PERSONS on x.PERSON_ID equals y.PERSON_ID where x.EMPLOYEE_ID == empId select y.PERSON_NAME).FirstOrDefault();
                        var Offer = (from o in context.JCOffers where o.OfferStartDate <= dt && o.OfferEndDate >= dt select o);
                        
                    }
                }
                catch (Exception e)
                {
                    throw e;

                }

                ViewBag.PDList = pdlist;
                ViewBag.DistributionHouses = distributionList;
                ViewBag.SupervisorName = SupName;
                ViewBag.SupId = empId;
            }
            return View("Summery");
        }
        [HttpPost]
        public ActionResult GetSummary(int pdId, int supId, int distributionId, string startDate, string endDate)
        {
            DateTime Start = DateTime.Now.Date;
            DateTime End = DateTime.Now.Date;
            if (startDate != "")
            {
                Boolean isStartDateValid = DateTime.TryParseExact(startDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out Start);
                if (!isStartDateValid)
                {
                    return Json(new { status = "error", details = "Order date (From) is invalid. It must be in dd/MM/yyyy format." }, JsonRequestBehavior.AllowGet);
                }
            }
            if (endDate != "")
            {
                Boolean isStartDateValid = DateTime.TryParseExact(endDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out End);
                if (!isStartDateValid)
                {
                    return Json(new { status = "error", details = "Order date (To) is invalid. It must be in dd/MM/yyyy format." }, JsonRequestBehavior.AllowGet);
                }
            }
            TimeSpan ts = new TimeSpan(23, 59, 59);
            End = End.Add(ts);
            using (DataAccess.PollidutEntities db = new DataAccess.PollidutEntities())
            {
                var result = (from a in db.FormPDEmplyeeViews where a.OrderDate >= Start && a.OrderDate <= End && a.Active == true select a);
                if (supId > 0)
                {
                    result = (from b in result where b.SupervisorId == supId select b);
                }
                if (pdId > 0)
                {
                    result = (from c in result where c.PollidutId == pdId select c);
                }
                if (distributionId > 0)
                {
                    result = (from d in result where d.DISTRIBUTION_HOUSE_ID == distributionId select d);
                }
                DateTime dt = DateTime.Now.Date;

                var pdTargets = (from p in db.PALLYDUT_TARGET
                                 where p.Active == true && p.StartDate <= dt && p.EndDate >= dt
                                 group p by p.PallydutId into g
                                 select new
                                 {
                                     PollidutId = g.Key,
                                     Start = g.Select(x => x.StartDate).Min(),
                                     End = g.Select(y => y.EndDate).Max(),
                                     Target = g.Select(z => z.Target).Sum()
                                 }).ToList();
                var groupresult = (from r in result
                            group r by new { r.PollidutId, r.PERSON_NAME, r.DISTRIBUTION_HOUSE_ID, r.DISTRIBUTION_HOUSE_NAME } into g
                            select new
                                {
                                    PollidutId = g.Key.PollidutId,
                                    PollidutName = g.Key.PERSON_NAME,
                                    DistributionId = g.Key.DISTRIBUTION_HOUSE_ID,
                                    DistributionName = g.Key.DISTRIBUTION_HOUSE_NAME,
                                    TotalIndent = g.Select(x => x.TotalPrice).Sum(),
                                    TotalDelivery = g.Select(x => x.DeliveryTotalPrice).Sum(),
                                }).ToList();
                var data = (from a in groupresult
                            join b in pdTargets on a.PollidutId equals b.PollidutId into t
                            from c in t.DefaultIfEmpty()
                            select new
                                {
                                    a.PollidutId,
                                    a.PollidutName,
                                    a.DistributionId,
                                    a.DistributionName,
                                    a.TotalIndent,
                                    a.TotalDelivery,
                                    Target = c == null ? 0 : c.Target == null ? 0 : c.Target
                                });

                return Json(new { msg = "success",data=data });
            }
            //return Json(new { msg = "success" });
        }
        public ActionResult SummaryDetail(int pdId, int supId, int distributionId, string startDate, string endDate)
        {
            DateTime Start = DateTime.Now.Date;
            DateTime End = DateTime.Now.Date;
            if (startDate != "")
            {
                Boolean isStartDateValid = DateTime.TryParseExact(startDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out Start);
                if (!isStartDateValid)
                {
                    return Json(new { status = "error", details = "Order date (From) is invalid. It must be in dd/MM/yyyy format." }, JsonRequestBehavior.AllowGet);
                }
            }
            if (endDate != "")
            {
                Boolean isStartDateValid = DateTime.TryParseExact(endDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out End);
                if (!isStartDateValid)
                {
                    return Json(new { status = "error", details = "Order date (To) is invalid. It must be in dd/MM/yyyy format." }, JsonRequestBehavior.AllowGet);
                }
            }
            TimeSpan ts = new TimeSpan(23, 59, 59);
            End = End.Add(ts);
            using(DataAccess.PollidutEntities db=new DataAccess.PollidutEntities())
            {
                var result = (from r in db.PD_PRODUCT_INDENT_FORM where r.OrderDate >= Start && r.OrderDate <= End && r.Active == true select r);
                if (supId > 0)
                {
                    result = (from r in result where r.SupervisorId == supId select r);
                }
                if(pdId>0)
                {
                    result = (from r in result where r.PollidutId == pdId select r);
                }
                if (distributionId > 0)
                {
                    result = (from r in result where r.DistributionHouseId==distributionId select r);
                }
                var items = (from a in db.FORM_ROW_ITEM
                            where (from b in result select b.FormId).Contains(a.FormId)
                            group a by a.PRODUCT_SKU into g
                            select new
                                {
                                    ProductId = g.Key.ProductId,
                                    ProductName = g.Key.ProductSKUName,
                                    ICarton = g.Select(x => x.IndentCartonQty).Sum(),
                                    IPiece = g.Select(x => x.IndentPieceQty).Sum(),
                                    DCarton = g.Select(x => x.DeliveryCartoonQty).Sum(),
                                    DPiece = g.Select(x => x.DeliveryPieceQty).Sum(),
                                    ITotal = g.Select(x => x.INetPrice).Sum(),
                                    DTotal = g.Select(x => x.DNetPrice).Sum()
                                }).ToList();
                return Json(new { msg = "success",data=items});
            }
            
        }

        public ActionResult OrderForm()
        {
            int userTypeId = Convert.ToInt32(Session["UserTypeId"].ToString());
            int empId = Convert.ToInt32(Session["EmployeeId"].ToString());
            //pollidut supervisor check
            if (userTypeId == 3)
            {
                List<Pollidut.Models.Pollidut> PollydutList = new List<Models.Pollidut>();
                List<Pollidut.Models.DistributionHouse> distributionList = new List<Models.DistributionHouse>();
                List<Pollidut.Models.ProductSKU> pdSKUs = new List<Models.ProductSKU>();
                
                string SupName = "";
                try
                {
                    using (DataAccess.PollidutEntities context = new DataAccess.PollidutEntities())
                    {
                        var pdlist = (from a in context.EMPLOYEES
                                  join b in context.PERSONS on a.PERSON_ID equals b.PERSON_ID
                                  where a.SUPERVISOR_ID == empId
                                  select new 
                                  {
                                      PollidutId = a.EMPLOYEE_ID,
                                      PollidutName = b.PERSON_NAME,
                                      DistributionHouseId = a.DISTRIBUTION_HOUSE_ID == null ? 0 : (int)a.DISTRIBUTION_HOUSE_ID
                                  }).ToList();
                        DateTime dt = DateTime.Now.Date;

                        var pdTargets = (from p in context.PALLYDUT_TARGET
                                         where p.Active == true && p.StartDate <= dt && p.EndDate >= dt
                                         group p by p.PallydutId into g
                                         select new
                                             {
                                                 PollidutId = g.Key,
                                                 Start = g.Select(x => x.StartDate).Min(),
                                                 End = g.Select(y => y.EndDate).Max(),
                                                 Target = g.Select(z => z.Target).Sum()
                                             }).ToList();

                        var PdTargetsList = (from m in pdlist
                                             join n in pdTargets on m.PollidutId equals n.PollidutId into t
                                             from l in t.DefaultIfEmpty()
                                             select new
                                                 {
                                                     PallydutId = m.PollidutId,
                                                     PallydutName = m.PollidutName,
                                                     DistributionId = m.DistributionHouseId,
                                                     StartDate = l == null ? dt :l.Start==null?dt: l.Start,
                                                     EndDate = l==null?dt: l.End == null ? dt : l.End,
                                                     Target =l==null?0: l.Target == null ? 0 : l.Target
                                                 }).ToList();

                        foreach (var Target in PdTargetsList)
                        {
                            var PdOrderQty = (from p in context.PD_PRODUCT_INDENT_FORM
                                              where p.PollidutId == Target.PallydutId && p.OrderDate >= Target.StartDate
                                                  && p.OrderDate <= Target.EndDate
                                              group p by p.PollidutId into g
                                              select new
                                                  {
                                                      ID = g.Key,
                                                      OrderQty = g.Select(x => x.TotalPrice).Sum()
                                                  }).FirstOrDefault();
                            var PdDeliveryQty = (from p in context.PD_PRODUCT_INDENT_FORM
                                                 where p.PollidutId == Target.PallydutId && p.DeliveryDate >= Target.StartDate && p.DeliveryDate <= Target.EndDate
                                                 group p by p.PollidutId into g
                                                 select new
                                                     {
                                                         ID = g.Key,
                                                         DeliveryQty = g.Select(x => x.DeliveryTotalPrice).Sum()
                                                     }).FirstOrDefault();
                            
                            Pollidut.Models.Pollidut PD = new Models.Pollidut();
                            PD.PollidutId = Target.PallydutId;
                            PD.PollidutName = Target.PallydutName;
                            PD.DistributionHouseId = Target.DistributionId;
                            PD.StartDate = (DateTime)Target.StartDate;
                            PD.EndDate = (DateTime)Target.EndDate;
                            PD.Target =(double) Target.Target;
                            PD.OrderQty = PdOrderQty==null?0:(double)PdOrderQty.OrderQty;
                            PD.Acheivement = PdDeliveryQty==null?0:(double)PdDeliveryQty.DeliveryQty;
                            PollydutList.Add(PD);
                        }


                        distributionList = (from c in context.EMPLOYEE_DISTRIBUTION_HOUSES
                                            join d in context.DISTRIBUTION_HOUSES on c.DistributionHouseId equals d.DISTRIBUTION_HOUSE_ID
                                            where c.EmployeeId == empId
                                            select new Pollidut.Models.DistributionHouse
                                            {
                                                DistributionHouseId = c.DistributionHouseId.ToString(),
                                                DistributionHouseName = d.DISTRIBUTION_HOUSE_NAME
                                            }).ToList();
                        SupName = (from x in context.EMPLOYEES join y in context.PERSONS on x.PERSON_ID equals y.PERSON_ID where x.EMPLOYEE_ID == empId select y.PERSON_NAME).FirstOrDefault();
                        var Offer = (from o in context.JCOffers where o.OfferStartDate <= dt && o.OfferEndDate >= dt select o);
                        //var Product_SKUs = (from psku in context.PRODUCT_SKU where psku.ProductSKUId > 0 select psku).ToList();
                        pdSKUs = (from m in context.PRODUCT_SKU
                                  join ofr in Offer on m.ProductSKUId equals ofr.ProductSKUId
                                  into temp
                                  from j in temp.DefaultIfEmpty()
                                  where m.ProductSKUId > 0
                                  select new Pollidut.Models.ProductSKU
                                  {
                                      ProductSKUId = m.ProductSKUId,
                                      ProductSKUName = m.ProductSKUName,
                                      UnitPrice = m.UnitPrice == null ? 0 : (double)m.UnitPrice,
                                      PiecePerCarton = m.PiecePerCarton == null ? 0 : (int)m.PiecePerCarton,
                                      JCOffer = j.JCOfferValue == null ? 0 : (int)j.JCOfferValue,
                                      JCOfferType = j.JCOfferUnit == null ? 0 : (int)j.JCOfferUnit,
                                      OutOf=j.OutOf==null?0:(int)j.OutOf,
                                      OutOfType=j.OutOfType==null?0:(int)j.OutOfType,
                                  }).ToList();
                    }
                }
                catch (Exception e)
                {
                    throw e;

                }

                ViewBag.PDList = PollydutList;
                ViewBag.DistributionHouses = distributionList;
                ViewBag.PDSKUs = pdSKUs;
                ViewBag.SupervisorName = SupName;
                ViewBag.SupId = empId;
            }
            return View();
        }
    }
}