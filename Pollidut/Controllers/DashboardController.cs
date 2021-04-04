using Pollidut.DataAccess;
using Pollidut.Models;
using Newtonsoft.Json;
using sCommonLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Pollidut.ViewModels;

namespace Pollidut.Controllers
{
    [HandleErrors]
    public class DashboardController : Controller
    {
        public ActionResult ReportPosition()
        {
            return View("ReportPosition");
        }
        public ActionResult Test()
        {
            using (PollidutEntities db = new PollidutEntities())
            {
                var complainViewModel = db.Notes.Where(m => m.Active == true).Select(x => new ComplainViewModel
                {
                    ID = x.ID,
                    Content = x.ContentText,
                    User = x.UserName,
                    Left = (int)x.Leftpx,
                    Top = (int)x.Toppx,
                    Zindex = (int)x.Zindexps
                }).ToList();
                return View(complainViewModel);
            }
        }
        public ActionResult AddNotes()
        {
            return View("add_note");
        }
        [HttpPost]
        public ActionResult UpdatePosition(int Id,int x,int y,int z)
        {
            using (PollidutEntities db = new PollidutEntities())
            {
                var utComplain = db.Notes.Where(m => m.ID == Id).FirstOrDefault();
                utComplain.Leftpx = x;
                utComplain.Toppx = y;
                utComplain.Zindexps = z;
                db.SaveChanges();
            }
            return Json(new {status="success" });
        }
        [HttpPost]
        public ActionResult PostComplain(int zindex, string author, string body,string color)
        {
            int Id = 0;
            using (PollidutEntities db = new PollidutEntities())
            {
                DateTime createDate = DateTime.Now.Date;
                DateTime dt = DateTime.Now;

                var complain = new Note
                {
                    ContentText = body,
                    UserName = author,
                    Color = color,
                    Leftpx=0,
                    Toppx=0,
                    Zindexps = zindex,
                    CreatedDate=createDate,
                    StatusId=1,
                    Active=true
                };
                db.Notes.Add(complain);
                db.SaveChanges();
                Id = complain.ID;
            }

            return Json(new {status="success",ID=Id });
        }
        public ActionResult Index()
        {

//            var context = GlobalHost.ConnectionManager.GetHubContext<BATB.Utils.TotalMemoAmountHub>();
//            //context.Clients.All.increaseTotalMemoAmount("10.45"); TODO: Tell Forkan

            string CurrentURL = Request.Url.AbsoluteUri;

            String LoginName = Session["LoginName"].ToString();
            String UserId = Session["UserId"].ToString();
            String FullName = Session["FullName"].ToString();
            String UserTypeId = Session["UserTypeId"].ToString();
            Int32 EmployeeId = Convert.ToInt32(Session["EmployeeId"]);

            String IsMobile = Request.QueryString["m"]; //if comes from mobile value=y, otherwise value=n 

            ViewBag.LoginName = LoginName;
            ViewBag.UserId = UserId;
            ViewBag.FullName = FullName;

            String DashboardTitle = String.Empty;
            String UserTypeName = String.Empty;
            Int32 DistributionHouseId = 0;
            String DistributionHouseName = "N/A";

            var db = new PollidutEntities();

            switch (UserTypeId)
            {
                case "1": UserTypeName = "SuperAdmin"; DashboardTitle = "Super Admin"; break;
                case "2": UserTypeName = "Pallydut"; DashboardTitle = "Pallydut"; break;
                case "3": UserTypeName = "PallydutSupervisor"; DashboardTitle = "Pollidut Supervisor"; break; //for CE project
                default:
                    break;
            }

            ViewBag.DashboardTitle = DashboardTitle; ViewBag.UserTypeName = UserTypeName;
            ViewBag.DistributionHouseId = DistributionHouseId; ViewBag.DistributionHouseName = DistributionHouseName;
            
            Session["DistributionHouseId"] = DistributionHouseId.ToString();
            Session["UserTypeName"] = UserTypeName;
            Session["IsMobile"] = IsMobile; //if from mobile, value=y, otherwise value=n 

            if (IsMobile=="y") //comes from mobile
            {
                //desktop-only user may come from mobile, so double check here.
                switch (UserTypeId)
                {
                    case "8": //CE Admin
                    case "13": //Photo collector
                        return View("Dashboard.desktop"); //force to desktop version
                      

                    default:
                        return View("Dashboard.desktop"); //all other to mobile version by default
                       
                }
            }
            else //comes from desktop
            {
                //mobile-only user may come from desktop, so double check here.
                switch (UserTypeId)
                {
                    case "12": //SR
                        return View("Dashboard.desktop");
                    
                    case "4": //BR Supervisor (BRS)
                    case "6": //CM Supervisor (CMS)
                    case "14": //TSA

                        if (Request.QueryString["switch"] == null)
                        {
                            return View("Dashboard.desktop");
                        }
                        else
                        {
                            return View("Dashboard.desktop"); //intentionally going to desktop
                        }
                    default:
                        return View("Dashboard.desktop");
                }


            }
           
        }

        public ActionResult CreateNewEmployee()
        {
            List<DistributionHouse> houses = DistributionHouseManager.GetDistributionHouses();
            List<PersonCurrentStatus> personcurrentstatuses = PersonCurrentStatusManager.GetStatuses();
            List<Pollidut.Models.EmployeeType> employeeTypes = Pollidut.Models.EmployeeTypeManager.GetEmployeeTypes();

            ViewBag.Houses = houses;
            ViewBag.PersonCurrentStatuses = personcurrentstatuses;
            ViewBag.EmployeeTypes = employeeTypes;

           
            return View("CreateNewEmployee");
        }

        [HttpPost]
        public JsonResult CreateNewEmployee(FormCollection data)
        {
            String PERSON_NAME = data["personname"].ToString().Trim();
            String FATHERS_NAME = data["fathername"].ToString().Trim();
            String MOTHERS_NAME = data["mothername"].ToString().Trim();
            String dateofbirth = data["dateofbirth"].ToString().Trim();
            String MAILING_ADDRESS = data["presentaddress"].ToString().Trim();
            String PERMANENT_ADDRESS = data["permanentaddress"].ToString().Trim();
            String MOBILE = data["mobileno"].ToString().Trim();
            String DISTRIBUTION_HOUSE_ID = data["distributionhouseid"].ToString();
            String EMPLOYEE_TYPE_ID = data["employeetypeid"].ToString();


            Byte[] imagefile;
            String imageName = String.Empty;

            if (Request.Files["photo"] != null)
            {
                imageName = Guid.NewGuid().ToString() + ".png";
            }

            if (Request.Files["photo"] != null)
            {
                var directory = HttpContext.Server.MapPath("~/Photos");
                string imagePath = Path.Combine(directory, "EmployeePhotos", imageName);
                FileStream fs = new FileStream(imagePath, FileMode.CreateNew);

                using (var binaryReader = new BinaryReader(Request.Files["photo"].InputStream))
                {
                    imagefile = binaryReader.ReadBytes(Request.Files["photo"].ContentLength);//image
                }


                String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
                SqlConnection connection = new SqlConnection(ConnectionString);
                SqlCommand command = new SqlCommand(); command.Connection = connection;
                SqlTransaction transaction = null;


                try
                {
                    DateTime DATE_OF_BIRTH;
                    DateTime.TryParseExact(dateofbirth, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out DATE_OF_BIRTH);

                    connection.Open();
                    transaction = connection.BeginTransaction();
                    command.Transaction = transaction;


                    //insert into Person table
                    String sqlInsertPerson = "insert into PERSONS(PERSON_NAME,FATHERS_NAME,MOTHERS_NAME,DATE_OF_BIRTH,MAILING_ADDRESS,PERMANENT_ADDRESS,MOBILE,DISTRIBUTION_HOUSE_ID,PERSON_IMAGE) Values(@PERSON_NAME,@FATHERS_NAME,@MOTHERS_NAME,@DATE_OF_BIRTH,@MAILING_ADDRESS,@PERMANENT_ADDRESS,@MOBILE,@DISTRIBUTION_HOUSE_ID,@PERSON_IMAGE)";
                    sqlInsertPerson = sqlInsertPerson + ";select SCOPE_IDENTITY();";
                    command.CommandText = sqlInsertPerson;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@PERSON_NAME", PERSON_NAME);
                    command.Parameters.AddWithValue("@FATHERS_NAME", FATHERS_NAME);
                    command.Parameters.AddWithValue("@MOTHERS_NAME", MOTHERS_NAME);
                    command.Parameters.AddWithValue("@DATE_OF_BIRTH", DATE_OF_BIRTH);
                    command.Parameters.AddWithValue("@MAILING_ADDRESS", MAILING_ADDRESS);
                    command.Parameters.AddWithValue("@PERMANENT_ADDRESS", PERMANENT_ADDRESS);
                    command.Parameters.AddWithValue("@MOBILE", MOBILE);
                    command.Parameters.AddWithValue("@DISTRIBUTION_HOUSE_ID", DISTRIBUTION_HOUSE_ID);
                    command.Parameters.AddWithValue("@PERSON_IMAGE", imageName);

                    Int32 PERSON_ID =  Convert.ToInt32(command.ExecuteScalar()); 

                    //-->insert into Employee table
                    String sqlInsertEmployee = "insert into EMPLOYEES(PERSON_ID,DISTRIBUTION_HOUSE_ID,EMPLOYEE_TYPE_ID) Values(@PERSON_ID,@DISTRIBUTION_HOUSE_ID,@EMPLOYEE_TYPE_ID)";
                    sqlInsertEmployee = sqlInsertEmployee + ";select SCOPE_IDENTITY();";
                    command.CommandText = sqlInsertEmployee;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@PERSON_ID", PERSON_ID);
                    command.Parameters.AddWithValue("@DISTRIBUTION_HOUSE_ID", DISTRIBUTION_HOUSE_ID);
                    command.Parameters.AddWithValue("@EMPLOYEE_TYPE_ID", EMPLOYEE_TYPE_ID);

                    Int32 EmployeeId =  Convert.ToInt32(command.ExecuteScalar()); 

                    String sqlUpdateEmployeeCode = "Update EMPLOYEES set EMPLOYEE_CODE=@EMPLOYEE_CODE Where EMPLOYEE_ID=@EMPLOYEE_ID";
                    command.CommandText = sqlUpdateEmployeeCode; command.Parameters.Clear();
                    command.Parameters.AddWithValue("@EMPLOYEE_CODE", EmployeeId);
                    command.Parameters.AddWithValue("@EMPLOYEE_ID", EmployeeId);
                    command.ExecuteNonQuery(); 
                    //<--insert into employee table

                    transaction.Commit();
                    connection.Close();
                    command.Dispose();
                    transaction.Dispose();
                    connection.Dispose();

                    BinaryWriter bw = new BinaryWriter(fs);
                    bw.Write(imagefile); 
                    bw.Close();

                    return Json(new { savestatus = "Success", employeeid = EmployeeId.ToString() });

                }

                catch (Exception Ex)
                {
                    if (transaction != null) { transaction.Rollback(); transaction.Dispose(); }
                    if (connection != null)
                    {
                        if (connection.State == ConnectionState.Open) { connection.Close(); }
                        connection.Dispose();
                    }

                    return Json(Ex.Message, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("Please select a photo.", JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult CreatePD()
        {
            int empId = Convert.ToInt32(Session["EmployeeId"].ToString());
            List<DistributionHouse> houses = DistributionHouseManager.GetSupervisorHouse(empId);
            List<PersonCurrentStatus> personcurrentstatuses = PersonCurrentStatusManager.GetStatuses();
            List<Pollidut.Models.EmployeeType> employeeTypes = Pollidut.Models.EmployeeTypeManager.GetEmployeeTypes();
            List<Religion> religions = Pollidut.Models.ReligionManager.GetReligions();
            List<MaritalStatus> mstatus = Pollidut.Models.MaritalStatusManager.GetMaritalStatuses();

            ViewBag.RelationShip = mstatus;
            ViewBag.Religions = religions;
            ViewBag.Houses = houses;
            ViewBag.PersonCurrentStatuses = personcurrentstatuses;
            ViewBag.EmployeeTypes = employeeTypes;


            return View("CreatePD");
        }
        

        [HttpPost]
        public JsonResult CreatePD(FormCollection data)
        {
            var serializer = new JavaScriptSerializer();
            var familyDetail = data["family"].ToString();
            var workDetail = data["works"].ToString();
            var distributionDetail = data["distributions"].ToString();
            var deserializedFamily = serializer.Deserialize<List<FamilyDetail>>(familyDetail);
            var deserializedWork = serializer.Deserialize<List<WorkDetail>>(workDetail);
            var deserializedDistribution = serializer.Deserialize<List<DistributionModel>>(distributionDetail);

            string personname = data["personname"].ToString().Trim();
            string empcode = data["empcode"].ToString().Trim();
            string fathername = data["fathername"].ToString().Trim();
            string mothername = data["mothername"].ToString().Trim();
            string mobileno = data["mobileno"].ToString().Trim();
            string education = data["education"].ToString().Trim();
            string dateofbirth = data["dateofbirth"].ToString().Trim();
            string nidno =data["nidno"].ToString().Trim();
            string nationality = data["nationality"].ToString().Trim();
            int religionid = Convert.ToInt32(data["religionid"].ToString().Trim());
            string presentaddress = data["presentaddress"].ToString().Trim();
            string permanentaddress = data["permanentaddress"].ToString().Trim();
            int employeetypeid = Convert.ToInt32(data["employeetypeid"].ToString().Trim());
            string joinDate = data["joinDate"].ToString().Trim();
            int minimumInvestment = Convert.ToInt32(data["minimumInvestment"].ToString().Trim());
            string otherFinace = data["otherFinace"].ToString().Trim();
            int relationship = Convert.ToInt32(data["relationship"].ToString().Trim());
            int memberno = Convert.ToInt32(data["memberno"].ToString().Trim());
            int malemember = Convert.ToInt32(data["malemember"].ToString().Trim());
            int femalemember = Convert.ToInt32(data["femalemember"].ToString().Trim());
            string pdservice = data["pdservice"].ToString().Trim();
            string previouspd = data["previouspd"].ToString().Trim();


            Byte[] imagefile;
            String imageName = String.Empty;

            if (Request.Files["photo"] != null)
            {
                imageName = Guid.NewGuid().ToString() + ".png";
            }

            if (Request.Files["photo"] != null)
            {
                var directory = HttpContext.Server.MapPath("~/Photos");
                string imagePath = Path.Combine(directory, "EmployeePhotos", imageName);
                FileStream fs = new FileStream(imagePath, FileMode.CreateNew);
                int empId = Convert.ToInt32(Session["EmployeeId"].ToString());
                int retEmpId=0;
                using (var binaryReader = new BinaryReader(Request.Files["photo"].InputStream))
                {
                    imagefile = binaryReader.ReadBytes(Request.Files["photo"].ContentLength);//image
                }
                try
                {
                    DateTime todaty = DateTime.Now;
                    DateTime bdate = DateTime.Now.Date; DateTime jdate = DateTime.Now.Date;
                    DateTime? birthDate = null;
                    DateTime? DateOfJoin = null;
                    DateTime CreatedDate = DateTime.Now;
                    if (dateofbirth != "")
                    {
                        Boolean isEndDateValid = DateTime.TryParseExact(dateofbirth, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out bdate);
                        if (!isEndDateValid)
                        {
                            return Json(new { status = "error", details = "Birthdate is invalid. It must be in dd/MM/yyyy format." }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            birthDate = bdate;
                        }
                    }
                    else
                    {
                        birthDate = null;
                    }
                    if (joinDate != "")
                    {
                        Boolean isEndDateValid = DateTime.TryParseExact(joinDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out jdate);
                        if (!isEndDateValid)
                        {
                            return Json(new { status = "error", details = "Join date is invalid. It must be in dd/MM/yyyy format." }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            DateOfJoin = jdate;
                        }
                    }
                    else
                    {
                        DateOfJoin = null;
                    }

                    using (DataAccess.PollidutEntities db = new PollidutEntities())
                    {
                        int firstDisId = deserializedDistribution.FirstOrDefault().Id == null ? 0 : deserializedDistribution.FirstOrDefault().Id;
                        var person = new DataAccess.PERSON
                        {
                            PERSON_NAME = personname,
                            FATHERS_NAME=fathername,
                            MOTHERS_NAME=mothername,
                            MOBILE=mobileno,
                            EDUCATIONAL_QUALIFICATION=education,
                            DATE_OF_BIRTH=birthDate,
                            NID=nidno,
                            NATIONALITY=nationality,
                            RELIGION_ID=religionid,
                            MAILING_ADDRESS=presentaddress,
                            PERMANENT_ADDRESS=permanentaddress,
                            MARITAL_STATUS_ID=relationship,
                            FAMILY_MEMBER_NO=memberno,
                            MALE_NO=malemember,
                            FEMALE_NO=femalemember,
                            ACTIVE=true,
                            ENTERED_BY = empId,
                            ENTERED_DATE = todaty,
                            GENDER_ID=0,
                            THANA_ID=0,
                            PERSON_CURRENT_STATUS_ID=0,
                            DISTRIBUTION_HOUSE_ID = firstDisId,
                            PERSON_IMAGE = imageName
                        };
                        db.PERSONS.Add(person);
                        db.SaveChanges();
                        foreach(var fmember in deserializedFamily)
                        {
                            var eachMember = new DataAccess.PERSON_FAMILY
                            {
                                PERSON_ID = person.PERSON_ID,
                                MEMBER_NAME=fmember.memberName,
                                RELATION=fmember.relation,
                                AGE=fmember.age,
                                OCCUPATION=fmember.occupation,
                                ACTIVE=true,
                                
                            };
                            db.PERSON_FAMILY.Add(eachMember);
                        }
                        db.SaveChanges();
                        
                        var emp = new DataAccess.EMPLOYEE
                        {
                            PERSON_ID = person.PERSON_ID,
                            EMPLOYEE_CODE_ = empcode,
                            EMPLOYEE_CODE=empcode,
                            EMPLOYEE_TYPE_ID = employeetypeid,
                            JOINING_DATE=DateOfJoin,
                            MINIMUM_INVESTMENT=minimumInvestment,
                            OTHER_FINANCIAL=otherFinace,
                            DISTRIBUTION_HOUSE_ID = firstDisId,
                            PREVIOUS_PD_DETAIL=previouspd,
                            SERVICE_DESCRIPTION=pdservice,
                            CREATED_BY=empId,
                            CRAETED_DATE = todaty,
                            DESIGNATION_ID=0,
                            BANK_ID=0,
                            BRAND_ID=0,
                            LOCATION_ID=0,
                            OFFICE_ID=0,
                            EMPLOYEE_ROLE_STATUS_ID=0,
                            SUPERVISOR_ID = empId,
                            ACTIVE=true
                        };
                        db.EMPLOYEES.Add(emp);
                        db.SaveChanges();
                        retEmpId=emp.EMPLOYEE_ID;
                        foreach (var eachWork in deserializedWork)
                        {
                            var work = new DataAccess.EMPLOYEE_WORK
                            {
                                EMPLOYEE_ID = emp.EMPLOYEE_ID,
                                WORK_AREA = eachWork.areaname,
                                UNION_NAME = eachWork.unionname,
                                THANA_NAME = eachWork.thananame,
                                ACTIVE = true
                            };
                            db.EMPLOYEE_WORK.Add(work);
                        }
                        db.SaveChanges();
                        foreach (var distribution in deserializedDistribution)
                        {
                            var dis = new DataAccess.EMPLOYEE_DISTRIBUTION_HOUSES
                            {
                                EmployeeId = emp.EMPLOYEE_ID,
                                DistributionHouseId = distribution.Id,
                                Active=true
                            };
                            db.EMPLOYEE_DISTRIBUTION_HOUSES.Add(dis);
                        }
                        db.SaveChanges();
                    }
                    

                    BinaryWriter bw = new BinaryWriter(fs);
                    bw.Write(imagefile);
                    bw.Close();

                    return Json(new { savestatus = "Success", employeeid=retEmpId});

                }

                catch (Exception Ex)
                {
                    

                    return Json(Ex.Message, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("Please select a photo.", JsonRequestBehavior.AllowGet);
            }
        }

        
       
        public ActionResult SearchEmployee()
        {
            try
            {
                

                List<DistributionHouse> houses = DistributionHouseManager.GetDistributionHouses();
                ViewBag.Houses = houses;

                List<Pollidut.Models.District> districts = Pollidut.Models.DistrictManager.GetDistricts();
                ViewBag.Districts = districts;

                List<Pollidut.Models.EmployeeType> employeeTypes = Pollidut.Models.EmployeeTypeManager.GetEmployeeTypes();
                ViewBag.EmployeeTypes = employeeTypes;

                List<Pollidut.Models.Designation> designations = Pollidut.Models.DesignationManager.GetDesignations();
                ViewBag.Designations = designations;

                return View("SearchEmployee");
            }
            catch (Exception Ex)
            {
                var errorPath = HttpContext.Server.MapPath("~/App_Data");
                ExceptionLogger.CreateLog(Ex, errorPath);
                return Json(new { result = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditEmployee()
        {
            try
            {
                int empId = Convert.ToInt32(Session["EmployeeId"].ToString());
                List<DistributionHouse> houses = DistributionHouseManager.GetSupervisorHouse(empId);
                ViewBag.Houses = houses;
                List<Pollidut.Models.District> districts = Pollidut.Models.DistrictManager.GetDistricts();
                ViewBag.Districts = districts;
                return View("EditEmployee");
            }
            catch (Exception Ex)
            {
                var errorPath = HttpContext.Server.MapPath("~/App_Data");
                ExceptionLogger.CreateLog(Ex, errorPath);
                return Json(new { result = "error" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HandleSessionTimeout]
        [HttpPost]
        public JsonResult EditEmployeeResult(FormCollection data)
        {
            try
            {

                String DistributionHouseId = data["distributionhouseid"].ToString();//
                String IsActive = data["active"].ToString();//
                String EmployeeCode = data["employeecode"].ToString();//
                String PersonName = data["personname"].ToString();//
                String MobileNo = data["mobileno"].ToString();//
                String NID = data["nid"].ToString();//
                String FromDateOfBirth = data["fromdateofbirth"].ToString();
                String ToDateOfBirth = data["todateofbirth"].ToString();
                String GenderId = data["genderid"].ToString();
                String MarritalStatusId = data["marritalstatusid"].ToString();
                String DistrictId = data["districtid"].ToString();
                String FromStartDate = data["fromstartdate"].ToString();
                String ToStartDate = data["tostartdate"].ToString();

                String GreaterInvest = data["greaterInvest"].ToString();
                String LessInvest = data["lessInverst"].ToString();
                int empId = Convert.ToInt32(Session["EmployeeId"].ToString());

                using(PollidutEntities db=new PollidutEntities())
                {
                    var pdList = (from e in db.EMPLOYEES
                                  join p in db.PERSONS on e.PERSON_ID equals p.PERSON_ID
                                  join d in db.DISTRIBUTION_HOUSES on e.DISTRIBUTION_HOUSE_ID equals d.DISTRIBUTION_HOUSE_ID
                                  where e.SUPERVISOR_ID == empId
                                  select new
                                      {
                                          EmployeeId=e.EMPLOYEE_ID,
                                          DistributionId = e.DISTRIBUTION_HOUSE_ID,
                                          DistributionHouse = d.DISTRIBUTION_HOUSE_NAME,
                                          EmpCode = e.EMPLOYEE_CODE,
                                          EmpName = p.PERSON_NAME,
                                          MobileNo = p.MOBILE,
                                          NID = p.NID,
                                          Photo = p.PERSON_IMAGE,
                                          MiminumInvest = e.MINIMUM_INVESTMENT,
                                          DOB = p.DATE_OF_BIRTH,
                                          JoinDate = e.JOINING_DATE,
                                          GenderId = p.GENDER_ID,
                                          MaritalStatusId = p.MARITAL_STATUS_ID,
                                          DistrictId = p.DISTRICT_ID,
                                          Active = e.ACTIVE
                                      });
                    if (!String.IsNullOrEmpty(DistributionHouseId))
                    {
                        int disId = Convert.ToInt32(DistributionHouseId.ToString());
                        if (disId > 0)
                        {
                            pdList = (from pd in pdList where pd.DistributionId == disId select pd);
                        }
                    }
                    if (!String.IsNullOrEmpty(GreaterInvest))
                    {
                        int gInvest = Convert.ToInt32(GreaterInvest.ToString());
                        if (gInvest > 0)
                        {
                            pdList = (from pd in pdList where pd.MiminumInvest >gInvest select pd);
                        }
                    }
                    if (!String.IsNullOrEmpty(LessInvest))
                    {
                        int lInvest = Convert.ToInt32(LessInvest.ToString());
                        if (lInvest > 0)
                        {
                            pdList = (from pd in pdList where pd.MiminumInvest < lInvest select pd);
                        }
                    }



                    if (!String.IsNullOrEmpty(IsActive))
                    {
                        pdList = (from pd in pdList where pd.Active == true select pd);
                    }

                    if (!String.IsNullOrEmpty(EmployeeCode))
                    {
                        pdList = (from pd in pdList where pd.EmpCode.Contains(EmployeeCode) select pd);
                    }


                    if (!String.IsNullOrEmpty(PersonName))
                    {
                        pdList = (from pd in pdList where pd.EmpName == PersonName select pd);
                    }

                    if (!String.IsNullOrEmpty(MobileNo))
                    {
                        pdList = (from pd in pdList where pd.MobileNo == MobileNo select pd);
                    }
                    //NID
                    if (!String.IsNullOrEmpty(NID))
                    {
                        pdList = (from pd in pdList where pd.NID == NID select pd);
                    }

                    if (!String.IsNullOrEmpty(FromDateOfBirth))
                    {
                        DateTime fromDateOfBirth;
                        if (DateTime.TryParseExact(FromDateOfBirth, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out fromDateOfBirth))
                        {
                            pdList = (from pd in pdList where pd.DOB < fromDateOfBirth select pd);
                        }
                    }
                    if (!String.IsNullOrEmpty(ToDateOfBirth))
                    {
                        DateTime afterDateOfBirth;

                        if (DateTime.TryParseExact(ToDateOfBirth, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out afterDateOfBirth))
                        {
                            pdList = (from pd in pdList where pd.DOB > afterDateOfBirth select pd);
                        }
                    }



                    //GenderId
                    if (!String.IsNullOrEmpty(GenderId))
                    {
                        int genId = Convert.ToInt16(GenderId);
                        pdList = (from pd in pdList where pd.GenderId == genId select pd);
                    }


                    //MarritalStatusId
                    if (!String.IsNullOrEmpty(MarritalStatusId))
                    {
                        int MaritalId = Convert.ToInt16(MarritalStatusId);
                        pdList = (from pd in pdList where pd.MaritalStatusId == MaritalId select pd);
                    }
                    // DistrictId 
                    if (!String.IsNullOrEmpty(DistrictId))
                    {
                        int districtId = Convert.ToInt16(DistrictId);
                        pdList = (from pd in pdList where pd.DistrictId == districtId select pd);
                    }
                    if (!String.IsNullOrEmpty(FromStartDate))
                    {
                        DateTime beforeJoin;
                        if (DateTime.TryParseExact(FromDateOfBirth, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out beforeJoin))
                        {
                            pdList = (from pd in pdList where pd.JoinDate < beforeJoin select pd);
                        }
                    }
                    if (!String.IsNullOrEmpty(ToStartDate))
                    {
                        DateTime afterJoin;
                        if (DateTime.TryParseExact(ToStartDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out afterJoin))
                        {
                            pdList = (from pd in pdList where pd.JoinDate > afterJoin select pd);
                        }
                    }
                    var rdata = pdList.ToList();
                    return Json(new {data=rdata,msg="success" });
                }
            }
            catch (Exception Ex)
            {
                var errorPath = HttpContext.Server.MapPath("~/App_Data");
                ExceptionLogger.CreateLog(Ex, errorPath);
                return Json(new { result = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HandleSessionTimeout]
        [HttpPost]
        public JsonResult SearchEmployeeResult(FormCollection data)
        {
            try
            {

            String DistributionHouseId = data["distributionhouseid"].ToString();//
            String PersonCurrentStatusId = data["personcurrentstatusid"].ToString();//
            String IsActive = data["active"].ToString();//
            String EmployeeCode = data["employeecode"].ToString();//
            String PersonName = data["personname"].ToString();//
            String MobileNo = data["mobileno"].ToString();//
            String Email = data["email"].ToString();//
            String NID = data["nid"].ToString();//
          //  String DesignationId = data["designationid"].ToString();//
            String FromDateOfBirth = data["fromdateofbirth"].ToString();
            String ToDateOfBirth = data["todateofbirth"].ToString();
            String GenderId = data["genderid"].ToString();
            String MarritalStatusId = data["marritalstatusid"].ToString();
            String DistrictId = data["districtid"].ToString();
            String EmployeeTypeId = data["employeetypeid"].ToString();

            String FromStartDate = data["fromstartdate"].ToString();
            String ToStartDate = data["tostartdate"].ToString();

            String FromEndDate = data["fromenddate"].ToString();
            String ToEndDate = data["toenddate"].ToString();


            String WhereClause = String.Empty;

            if (!String.IsNullOrEmpty(DistributionHouseId))
            {
                WhereClause = "DistributionHouseId=" + DistributionHouseId;
            }

            if (!String.IsNullOrEmpty(PersonCurrentStatusId))
            {
                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "PersonCurrentStatusId=" + PersonCurrentStatusId;
                }
                else
                {
                    WhereClause += " AND PersonCurrentStatusId=" + PersonCurrentStatusId;
                }
            }

            if (!String.IsNullOrEmpty(IsActive))
            {
                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "IsActive=" + IsActive;
                }
                else
                {
                    WhereClause += " AND IsActive=" + IsActive;

                }
            }

            if (!String.IsNullOrEmpty(EmployeeCode))
            {
                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "EmployeeCode=" + EmployeeCode;
                }
                else
                {
                    WhereClause += " AND EmployeeCode=" + EmployeeCode;

                }
            }


            if (!String.IsNullOrEmpty(PersonName))
            {
                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "PersonName Like '%" + PersonName + "%'";
                }
                else
                {
                    WhereClause += " AND PersonName Like'%" + PersonName + "%'";

                }
            }

            if (!String.IsNullOrEmpty(MobileNo))
            {
                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "Mobile Like '%" + MobileNo + "%'";
                }
                else
                {
                    WhereClause += " AND Mobile Like'%" + MobileNo + "%'";

                }
            }

            //Email
            if (!String.IsNullOrEmpty(Email))
            {
                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "Email='%" + Email + "%'";
                }
                else
                {
                    WhereClause += " AND Email='%" + Email + "%'";

                }
            }

            //NID
            if (!String.IsNullOrEmpty(NID))
            {
                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "NID='%" + NID + "%'";
                }
                else
                {
                    WhereClause += " AND NID='%" + NID + "%'";

                }
            }


            //DesignationId
            //if (!String.IsNullOrEmpty(DesignationId))
            //{
            //    if (String.IsNullOrEmpty(WhereClause))
            //    {
            //        WhereClause = "DesignationId=" + DesignationId;
            //    }
            //    else
            //    {
            //        WhereClause += " AND DesignationId=" + DesignationId;

            //    }
            //}

            if (!String.IsNullOrEmpty(FromDateOfBirth) && !String.IsNullOrEmpty(ToDateOfBirth))
            {
                DateTime fromDateOfBirth; DateTime.TryParseExact(FromDateOfBirth, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out fromDateOfBirth);
                DateTime toDateOfBirth; DateTime.TryParseExact(ToDateOfBirth, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out toDateOfBirth);

                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "DateOfBirth Between '" + fromDateOfBirth.ToString("yyyy-MM-dd") + "' AND '" + toDateOfBirth.ToString("yyyy-MM-dd") + "'";
                }
                else
                {
                    WhereClause = " AND DateOfBirth Between '" + fromDateOfBirth.ToString("yyyy-MM-dd") + "' AND '" + toDateOfBirth.ToString("yyyy-MM-dd") + "'";

                }
            }
            else if (!String.IsNullOrEmpty(FromDateOfBirth) && String.IsNullOrEmpty(ToDateOfBirth))
            {
                DateTime fromDateOfBirth; DateTime.TryParseExact(FromDateOfBirth, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out fromDateOfBirth);

                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "DateOfBirth >= '" + fromDateOfBirth.ToString("yyyy-MM-dd") + "'";
                }
                else
                {
                    WhereClause = "DateOfBirth >= '" + fromDateOfBirth.ToString("yyyy-MM-dd") + "'";
                }
            }
            else if (String.IsNullOrEmpty(FromDateOfBirth) && !String.IsNullOrEmpty(ToDateOfBirth))
            {
                DateTime toDateOfBirth; DateTime.TryParseExact(FromDateOfBirth, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out toDateOfBirth);

                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "DateOfBirth <= '" + toDateOfBirth.ToString("yyyy-MM-dd") + "'";
                }
                else
                {
                    WhereClause = "DateOfBirth <= '" + toDateOfBirth.ToString("yyyy-MM-dd") + "'";
                }
            }

            //GenderId
            if (!String.IsNullOrEmpty(GenderId))
            {
                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "GenderId=" + GenderId;
                }
                else
                {
                    WhereClause += " AND GenderId=" + GenderId;
                }
            }


            //MarritalStatusId
            if (!String.IsNullOrEmpty(MarritalStatusId))
            {
                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "MarritalStatusId=" + MarritalStatusId;
                }
                else
                {
                    WhereClause += " AND MarritalStatusId=" + MarritalStatusId;
                }
            }

            // DistrictId 
            if (!String.IsNullOrEmpty(DistrictId))
            {
                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "DistrictId=" + DistrictId;
                }
                else
                {
                    WhereClause += " AND DistrictId=" + DistrictId;
                }
            }

            //EmployeeTypeId
            if (!String.IsNullOrEmpty(EmployeeTypeId))
            {
                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "EmployeeTypeId=" + EmployeeTypeId;
                }
                else
                {
                    WhereClause += " AND EmployeeTypeId=" + EmployeeTypeId;
                }
            }


            if (!String.IsNullOrEmpty(FromStartDate) && !String.IsNullOrEmpty(ToStartDate))
            {
                DateTime fromStartDate; DateTime.TryParseExact(FromStartDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out fromStartDate);
                DateTime toStartDate; DateTime.TryParseExact(ToStartDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out toStartDate);

                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "StartDate Between '" + fromStartDate.ToString("yyyy-MM-dd") + "' AND '" + toStartDate.ToString("yyyy-MM-dd") + "'";
                }
                else
                {
                    WhereClause += " AND StartDate Between '" + fromStartDate.ToString("yyyy-MM-dd") + "' AND '" + toStartDate.ToString("yyyy-MM-dd") + "'";

                }
            }
            else if (!String.IsNullOrEmpty(FromStartDate) && String.IsNullOrEmpty(ToStartDate))
            {
                DateTime fromStartDate; DateTime.TryParseExact(FromStartDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out fromStartDate);

                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "StartDate >= '" + fromStartDate.ToString("yyyy-MM-dd") + "'";
                }
                else
                {
                    WhereClause += " AND StartDate >= '" + fromStartDate.ToString("yyyy-MM-dd") + "'";
                }
            }
            else if (String.IsNullOrEmpty(FromStartDate) && !String.IsNullOrEmpty(ToStartDate))
            {
                DateTime toStartDate; DateTime.TryParseExact(ToStartDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out toStartDate);

                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "StartDate <= '" + toStartDate.ToString("yyyy-MM-dd") + "'";
                }
                else
                {
                    WhereClause += " AND StartDate <= '" + toStartDate.ToString("yyyy-MM-dd") + "'";
                }
            }

            if (!String.IsNullOrEmpty(FromEndDate) && !String.IsNullOrEmpty(ToEndDate))
            {
                DateTime fromEndDate; DateTime.TryParseExact(FromEndDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out fromEndDate);
                DateTime toEndDate; DateTime.TryParseExact(ToEndDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out toEndDate);

                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "EndDate Between '" + fromEndDate.ToString("yyyy-MM-dd") + "' AND '" + toEndDate.ToString("yyyy-MM-dd") + "'";
                }
                else
                {
                    WhereClause += " AND EndDate Between '" + fromEndDate.ToString("yyyy-MM-dd") + "' AND '" + toEndDate.ToString("yyyy-MM-dd") + "'";

                }
            }
            else if (!String.IsNullOrEmpty(FromEndDate) && String.IsNullOrEmpty(ToEndDate))
            {
                DateTime fromEndDate; DateTime.TryParseExact(FromEndDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out fromEndDate);

                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "EndDate >= '" + fromEndDate.ToString("yyyy-MM-dd") + "'";
                }
                else
                {
                    WhereClause += " AND EndDate >= '" + fromEndDate.ToString("yyyy-MM-dd") + "'";
                }
            }
            else if (String.IsNullOrEmpty(FromEndDate) && !String.IsNullOrEmpty(ToEndDate))
            {
                DateTime toEndDate; DateTime.TryParseExact(ToEndDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out toEndDate);

                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "EndDate <= '" + toEndDate.ToString("yyyy-MM-dd") + "'";
                }
                else
                {
                    WhereClause += " AND EndDate <= '" + toEndDate.ToString("yyyy-MM-dd") + "'";
                }
            }

            if (!String.IsNullOrEmpty(WhereClause))
            {
                WhereClause = " Where " + WhereClause;
            }

            String sqlSelect = "select  DistributionHouseName,EmployeeId , EmployeeCode, PersonName, EmployeeTypeName, IsActive,Mobile,Photo from viewSearchEmployee " + WhereClause;

            DataTable dtSearchResult = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(sqlSelect, WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString))
            {
                da.Fill(dtSearchResult);
            }

            String SerializedData = JsonConvert.SerializeObject(dtSearchResult, Formatting.None);
            return Json(SerializedData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                var errorPath = HttpContext.Server.MapPath("~/App_Data");
                ExceptionLogger.CreateLog(Ex, errorPath);
                return Json(new { result = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CreateNewEmployeePosition()
        {
            List<DistributionHouse> houses = DistributionHouseManager.GetDistributionHouses();
            List<Pollidut.Models.EmployeeType> empTypes = Pollidut.Models.EmployeeTypeManager.GetEmployeeTypes();

            ViewBag.Houses = houses;
            ViewBag.PositionTypes = empTypes;

            return View("CreateNewEmployeePosition");
        }

        public JsonResult DistributionHouseSpecificPositions(int houseid)
        {

            List<Pollidut.Models.ServerToClientModel.DistributionHouseSpecificPosition> positions =
                Pollidut.Models.ServerToClientModel.DistributionHouseSpecificPositionManager.GetPositions(houseid);


            return Json(positions, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreateNewEmployeePosition(FormCollection data)
        {
            Int32 distributionhouseid = Convert.ToInt32(data["distributionhouseid"]);
            Int32 positiontypeid = Convert.ToInt32(data["positiontypeid"]);
            Int32 positionquantity = Convert.ToInt32(data["positionquantity"]);
            String PositionTypeName = data["typename"].ToString();

           // String PositionTypeName = String.Empty;
            //switch (positiontypeid)
            //{
            //    case 1: PositionTypeName = "BR "; break;
            //    case 2: PositionTypeName = "BR Sup "; break;
            //    case 3: PositionTypeName = "AC "; break;
            //    case 5: PositionTypeName = "CM "; break;
            //    case 6: PositionTypeName = "MS "; break;

            //    default:
            //        break;
            //}


            Int32 startingposition = Convert.ToInt32(data["startingposition"]);
            Int32 endingposition = Convert.ToInt32(data["endingposition"]);

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    con.Open();

                    String sqlCountPositions = "SELECT COUNT(CM_POSITION_ID) AS PositionCount FROM dbo.CM_POSITIONS GROUP BY DISTRIBUTION_HOUSE_ID, POSITION_TYPE_ID HAVING      (DISTRIBUTION_HOUSE_ID = " + distributionhouseid + ") AND (POSITION_TYPE_ID = " + positiontypeid + ")";


                    cmd.CommandText = sqlCountPositions;

                    Object objPositionCount = cmd.ExecuteScalar();
                    Int32 intPositionCount = 0;

                    if (objPositionCount != null)
                    {
                        intPositionCount = Convert.ToInt32(objPositionCount);
                    }


                    for (int i = 0; i < positionquantity; i++)
                    {
                        intPositionCount++;
                        String sqlInsert = "insert into CM_POSITIONS(DISTRIBUTION_HOUSE_ID, CM_POSITION_NAME,ACTIVE,POSITION_TYPE_ID) values(@DISTRIBUTION_HOUSE_ID, @CM_POSITION_NAME,@ACTIVE,@POSITION_TYPE_ID)"; //;select SCOPE_IDENTITY(); @CM_POSITION_ID,


                        cmd.CommandText = sqlInsert;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@DISTRIBUTION_HOUSE_ID", distributionhouseid);
                        cmd.Parameters.AddWithValue("@CM_POSITION_NAME", PositionTypeName + " "  + intPositionCount.ToString());
                        cmd.Parameters.AddWithValue("@ACTIVE", 1);
                        cmd.Parameters.AddWithValue("@POSITION_TYPE_ID", positiontypeid);

                        cmd.ExecuteNonQuery();
                    }

                    con.Close();
                }
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult DeleteCmPosition(int positionid)
        {

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                String sqlInsert = "delete from CM_POSITIONS where CM_POSITION_ID =" + positionid + "";
                using (SqlCommand cmd = new SqlCommand(sqlInsert, con))
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

      
        [HttpPost]
        public JsonResult UpdatePositionName(int positionid, string positionname, int distributionhouseid)
        {

            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            String Status = String.Empty;

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                String sqlCheck = "Select CM_POSITION_NAME from CM_POSITIONS where CM_POSITION_NAME='"+ positionname +"' and DISTRIBUTION_HOUSE_ID=" + distributionhouseid + " and CM_POSITION_ID <>" + positionid + "";

                String sqlInsert = "update CM_POSITIONS set CM_POSITION_NAME='" + positionname + "' where  CM_POSITION_ID =" + positionid + "";

               
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;  con.Open();
                    cmd.CommandText = sqlCheck;
                    object result = cmd.ExecuteScalar();
                    if (result==null)
                    {
                        cmd.CommandText = sqlInsert; cmd.ExecuteNonQuery(); Status = "Success";
                    }
                    else
                    {
                        Status = "Exist";
                    }
                  
                    con.Close();
                }
            }

            return Json(Status, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AssignEmployeePosition()
        {
            List<DistributionHouse> houses = DistributionHouseManager.GetDistributionHouses();
            List<Pollidut.Models.EmployeeType> empTypes = Pollidut.Models.EmployeeTypeManager.GetEmployeeTypes();

            ViewBag.Houses = houses;
            ViewBag.PositionTypes = empTypes;

            return View("AssignEmployeePosition");
        }

        [HttpPost]
        public JsonResult HouseSpecificPositionAndEmployeeList(int houseid)
        {
            List<Pollidut.Models.ServerToClientModel.HouseSpecificPositionAndEmployee> list =
                Pollidut.Models.ServerToClientModel.HouseSpecificPositionAndEmployeeManager.GetPositions(houseid);

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult HouseAndPositionSpecificPositionAndEmployeeList(int houseid, int positiontypeid)
        {
            List<Pollidut.Models.ServerToClientModel.HouseSpecificPositionAndEmployee> list =
                Pollidut.Models.ServerToClientModel.HouseSpecificPositionAndEmployeeManager.GetPositions(houseid, positiontypeid);

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //viewHouseAndTypeSpecificEmployees
        [HttpPost]
        public JsonResult GetHouseAndTypeSpecificUassignedEmployees(int houseid, int employeetypeid)
        {
            List<Pollidut.Models.ServerToClientModel.HouseAndTypeSpecificUnassignedEmployee> list =
                Pollidut.Models.ServerToClientModel.HouseAndTypeSpecificUnassignedEmployeeManager.GetPositions(houseid, employeetypeid);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AssignEmployeePosition(int positionid, int employeeid)
        {
            Pollidut.Models.EmployeePositionAssignment assignment = new Pollidut.Models.EmployeePositionAssignment(positionid, employeeid);

            String result = String.Empty;


            try
            {
                if (assignment.AssignEmployee() == Pollidut.Models.AssignmentResult.Success)
                {
                    result = "Success";
                }
                else
                {
                    if (assignment.AssignEmployee() == Pollidut.Models.AssignmentResult.AlreadyAssigned)
                    {
                        result = "He is already assigned to " + assignment.PreviouslyAssignedPositionName;
                    }
                }
            }
            catch (Exception Ex)
            {
                return Json(Ex.Message, JsonRequestBehavior.AllowGet);
            }


            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult RemoveEmployeePosition(Int32 positionid, Int32 employeeid)
        {
            Pollidut.Models.EmployeePositionAssignment assignment = new Pollidut.Models.EmployeePositionAssignment(positionid, employeeid);

            try
            {
                assignment.RemoveEmployee();
            }
            catch (Exception Ex)
            {
                return Json(Ex.Message, JsonRequestBehavior.AllowGet);
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetHouseSpecificCmList(int houseid)
        //{
        //    var cmlist = HouseSpecificCmManager.GetHouseSpecificCms(houseid);
        //    return Json(cmlist, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetHouseSpecificUnassignedPositions(int houseid)
        //{
        //    var positions = HouseSpecificUnassignedPositionManager.GetHouseSpecificUnassignedPositions(houseid);
        //    return Json(positions, JsonRequestBehavior.AllowGet);
        //}

        ////[HttpPost]
        ////public JsonResult AssignEmployeePosition(FormCollection data)
        ////{

        ////    Int32 distributionhouseid = Convert.ToInt32(data["distributionhouseid"]);
        ////    Int32 cmid = Convert.ToInt32(data["cmid"]);
        ////    Int32 positionid = Convert.ToInt32(data["positionid"]);

        ////    String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        ////    using (SqlConnection con = new SqlConnection(ConnectionString))
        ////    {
        ////        using (SqlCommand cmd = new SqlCommand())
        ////        {
        ////            cmd.Connection = con;
        ////            con.Open();
        ////            String sqlInsert = "insert into CMS(EMPLOYEE_ID,CM_POSITION_ID) values(" + cmid + "," + positionid + ")";
        ////            cmd.CommandText = sqlInsert; cmd.ExecuteNonQuery();
        ////            con.Close();
        ////        }
        ////    }
        ////    return Json("Success", JsonRequestBehavior.AllowGet);
        ////}

//        public ActionResult CreateNewBit()
//        {
//            List<DistributionHouse> houses = DistributionHouseManager.GetDistributionHouses();
//            ViewBag.Houses = houses;

//            return View("CreateNewBit");
//        }

//        [HttpPost]
//        public JsonResult CreateNewBit(FormCollection data)
//        {
//            Int32 distributionhouseid = Convert.ToInt32(data["distributionhouseid"]);

//            String bitQuantity = data["bitquantity"].ToString();

//            if (String.IsNullOrEmpty(bitQuantity)) //entry with specified range
//            {
//                Int32 startingposition = Convert.ToInt32(data["startingposition"]);
//                Int32 endingposition = Convert.ToInt32(data["endingposition"]);

//                var db = new BATB.DataAccess.BATBEntities();

//                try
//                {
//                    for (int i = startingposition; i <= endingposition; i++)
//                    {
//                        BATB.DataAccess.BIT newBit = new DataAccess.BIT();
//                        newBit.DISTRIBUTION_HOUSE_ID = distributionhouseid; newBit.BIT_ID = i; newBit.BIT_NAME = "Bit " + i.ToString();

//                        db.BITS.Add(newBit);
//                    }

//                    db.SaveChanges();

//                    return Json("Success", JsonRequestBehavior.AllowGet);

//                }
//                catch (Exception Ex)
//                {
//                    return Json(Ex.Message, JsonRequestBehavior.AllowGet);
//                }
//            }
//            else //entry by quantity
//            {
//                String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;


//                String sqlCountPositions = "SELECT COUNT(BIT_ID) AS BitCount FROM dbo.BITS GROUP BY DISTRIBUTION_HOUSE_ID HAVING      (DISTRIBUTION_HOUSE_ID = "+ distributionhouseid +")";

//                try
//                {
//                    using (SqlConnection con = new SqlConnection(ConnectionString))
//                    {
//                        using (SqlCommand cmd = new SqlCommand())
//                        {
//                            cmd.Connection = con;
//                            con.Open();
//                            cmd.CommandText = sqlCountPositions;

//                            Object objPositionCount = cmd.ExecuteScalar();
//                            Int32 intPositionCount = 0;

//                            if (objPositionCount != null && objPositionCount != DBNull.Value)
//                            {
//                                intPositionCount = Convert.ToInt32(objPositionCount);
//                            }

//                            String sqlGetLastId = "select MAX(BIT_ID) as LastBitId from BITS";
//                            cmd.CommandText = sqlGetLastId;
//                            Object objLastBitId = cmd.ExecuteScalar();

//                            Int32 FisrtBitId = 1;
//                            if (objLastBitId != null && objLastBitId != DBNull.Value)
//                            {
//                                FisrtBitId = Convert.ToInt32(objLastBitId) + 1;
//                            }
                           
//                            Int32 LastBitId = FisrtBitId + Convert.ToInt32(bitQuantity);
//                            for (int i = FisrtBitId; i < LastBitId; i++)
//                            {
//                                intPositionCount++;
//                                String sqlInsert = "insert into BITS(BIT_NAME,DISTRIBUTION_HOUSE_ID) values('Bit " + intPositionCount.ToString() + "'," + distributionhouseid + ")";
//                                cmd.CommandText = sqlInsert; cmd.ExecuteNonQuery();
//                            }
//                            con.Close();
//                        }
//                    }

//                    return Json("Success", JsonRequestBehavior.AllowGet);
//                }
//                catch (Exception ex)
//                {
//                    return Json(ex.Message, JsonRequestBehavior.AllowGet);
//                }

//            }
//        }

//        [HttpPost]
//        public JsonResult UpdateBitName(int bitid, string bitname, int distributionhouseid)
//        {
//            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
//            try
//            {
//                String result = String.Empty;
//                String sqlCheck = "SELECT BIT_ID FROM BITS where BIT_NAME='"+ bitname +"' and DISTRIBUTION_HOUSE_ID ="+ distributionhouseid +" and BIT_ID<>"+ bitid +"";

//                using (SqlConnection con = new SqlConnection(ConnectionString))
//                {
//                    using (SqlCommand cmd = new SqlCommand())
//                    {
//                        cmd.Connection = con;
//                        con.Open();
//                        cmd.CommandText = sqlCheck;
//                        Object CheckResult = cmd.ExecuteScalar();

//                        if (CheckResult != null && CheckResult != DBNull.Value) //exists
//                        {
//                            result = "This bit name already in use. Please try another name.";
//                        }
//                        else
//                        {
//                            String sqlUpdate = "Update BITS Set BIT_NAME='" + bitname + "' where BIT_ID=" + bitid + "";
//                            cmd.CommandText = sqlUpdate;
//                            cmd.ExecuteNonQuery();
//                            result = "Success";
//                        }
//                        con.Close();
//                    }
//                }

//                return Json(result, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception ex)
//            {
//                return Json(ex.Message, JsonRequestBehavior.AllowGet);
//            }
//        }



//        [HttpPost]
//        public JsonResult DeleteBit(int bitid)
//        {
//            String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
//            try
//            {
//                String result = String.Empty;
//                String sqlCheck = " SELECT     C.CM_POSITION_NAME FROM         dbo.DEFAULT_CM_WISE_PLANS AS P INNER JOIN   dbo.BITS AS B ON P.BIT_ID = B.BIT_ID INNER JOIN dbo.CM_POSITIONS AS C ON P.CM_POSITION_ID = C.CM_POSITION_ID WHERE     (B.BIT_ID = "+ bitid +")";

//                using (SqlConnection con = new SqlConnection(ConnectionString))
//                {
//                    using (SqlCommand cmd = new SqlCommand())
//                    {
//                        cmd.Connection = con;
//                        con.Open();
//                        cmd.CommandText = sqlCheck;
//                        Object CheckResult = cmd.ExecuteScalar();

//                        if (CheckResult != null && CheckResult != DBNull.Value) //exists
//                        {
//                            result = "This bit has been assigned to "+ CheckResult.ToString() +" position. You can not delete it.";
//                        }
//                        else
//                        {
//                            String sqlDelete = "delete from BITS where BIT_ID="+ bitid +"";
//                            cmd.CommandText = sqlDelete;
//                            cmd.ExecuteNonQuery(); 
//                            result = "Success";
//                        }
//                        con.Close();
//                    }
//                }

//                return Json(result, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception ex)
//            {
//                return Json(ex.Message, JsonRequestBehavior.AllowGet);
//            }
//        }


//        public ActionResult CreateDefaultBit()
//        {
//            List<DistributionHouse> houses = DistributionHouseManager.GetDistributionHouses();
//            ViewBag.Houses = houses;

//            List<Bit> bits = BitManager.GetBits();
//            ViewBag.Bits = bits;


//            return View("CreateDefaultBit");
//        }

//        [HttpPost]
//        public JsonResult GetRoutes(int houseid)
//        {
//            var routes = BATB.Models.RouteManager.GetRoutes(houseid);
//            return Json(routes, JsonRequestBehavior.AllowGet);
//        }

//        [HttpPost]
//        public JsonResult GetSpecificHouseBits(int houseid)
//        {
//            List<Bit> bits = BitManager.GetHouseSpecificBits(houseid);

//            return Json(bits, JsonRequestBehavior.AllowGet);
//        }


//        //Get House Specific Bits With Outlet Count
//        [HttpPost]
//        public JsonResult GetHouseSpecificBitsWithOutletCount(int houseid)
//        {
//            DataTable dt = BitManager.GetHouseSpecificBitsWithOutletCount(houseid);
//            String SerializedData = JsonConvert.SerializeObject(dt, Formatting.None);
//            return Json(SerializedData, JsonRequestBehavior.AllowGet);
//        }

//        [HttpPost]
//        public JsonResult GetSections(int routeid)
//        {
//            var sections = BATB.Models.SectionManager.GetRouteSpecificSections(routeid);
//            return Json(sections, JsonRequestBehavior.AllowGet);
//        }

//        [HttpPost]
//        public JsonResult GetClusters(int sectionid)
//        {
//            var clusters = BATB.Models.ClusterManager.GetSectionSpecificClusters(sectionid);
//            return Json(clusters, JsonRequestBehavior.AllowGet);
//        }


//        [HttpPost]
//        public JsonResult GetOutlets(int clusterid)
//        {
//            var outlets = BATB.Models.OutletManager.GetClusterSpecificOutlets(clusterid);
//            return Json(outlets, JsonRequestBehavior.AllowGet);
//        }


//        [HttpPost]
//        public JsonResult AssignOutletToBit(int outletid, int bitid,String mode)
//        {
//            try
//            {
//                if (mode == "Add")
//                {
//                    var db = new BATB.DataAccess.BATBEntities();

//                    BATB.DataAccess.DEFAULT_BIT newBit = new DataAccess.DEFAULT_BIT();
//                    newBit.BIT_ID = bitid; newBit.OUTLET_ID = outletid;
//                    db.DEFAULT_BIT.Add(newBit);
//                    db.SaveChanges();
//                }
//                else //Update
//                {
//                    string connecSting = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
//                    using (SqlConnection connection = new SqlConnection(connecSting))
//                    {
//                        String sqlUpdate = "Update DEFAULT_BIT Set BIT_ID=" + bitid + " Where OUTLET_ID=" + outletid + "";
//                        using (SqlCommand command = new SqlCommand(sqlUpdate, connection))
//                        {
//                            connection.Open(); command.ExecuteNonQuery(); connection.Close();
//                        }
//                    }
//                }
//            }
//            catch (Exception Ex)
//            {
//                 return Json(Ex.Message, JsonRequestBehavior.AllowGet);
//            }

//            return Json("Success", JsonRequestBehavior.AllowGet);
//        }


//        [HttpPost]
//        public JsonResult UpdateOutletToBit()
//        {

//            return Json("success", JsonRequestBehavior.AllowGet);
//        }

//        [HttpPost]
//        public JsonResult CreateDefaultBit(FormCollection data)
//        {
//            Int32 bitid = Convert.ToInt32(data["bitid"]);

//            String strOutlets = data["outletid"];
//            var db = new BATB.DataAccess.BATBEntities();

//            if (strOutlets != null)
//            {
//                if (strOutlets.Contains(","))
//                {
//                    String[] arrOutlets = strOutlets.Split(',');
//                    for (int i = 0; i < arrOutlets.Length; i++)
//                    {
//                        Int32 outletId = Convert.ToInt32(arrOutlets[i]);

//                        BATB.DataAccess.DEFAULT_BIT newBit = new DataAccess.DEFAULT_BIT();
//                        newBit.BIT_ID = bitid; newBit.OUTLET_ID = outletId;
//                        db.DEFAULT_BIT.Add(newBit);
//                    }

//                    db.SaveChanges();
//                }
//                else
//                {
//                    Int32 outletId = Convert.ToInt32(strOutlets);
//                    BATB.DataAccess.DEFAULT_BIT newBit = new DataAccess.DEFAULT_BIT();
//                    newBit.BIT_ID = bitid; newBit.OUTLET_ID = outletId; db.DEFAULT_BIT.Add(newBit); db.SaveChanges();
//                }
//            }

//            return Json("Success", JsonRequestBehavior.AllowGet);
//        }

//        [HttpPost]
//        public JsonResult GetClusterWiseOutletsForCreateDefaultBitForm(int ClusterId)
//        {
//            /* Original Source of viewSearchOutletsForCreateDefaultBit
//            SELECT     TOP (100) PERCENT DH.DISTRIBUTION_HOUSE_ID AS HouseId, DH.DISTRIBUTION_HOUSE_NAME AS HouseName, R.ROUTE_ID AS RouteId, 
//                      R.ROUTE_NAME AS RouteName, S.SECTION_ID AS SectionId, S.SECTION_NAME AS SectionName, C.CLUSTER_ID AS ClusterId, C.CLUSTER_NAME AS ClusterName, 
//                      ISNULL(B.BIT_ID, - 1) AS BitId, ISNULL(B.BIT_NAME, 'N/A') AS BitName, O.OUTLET_ID AS OutletId, O.OUTLET_NAME AS OutletName, 
//                      OT.OUTLET_TYPE_NAME AS OutletType, ISNULL(O.ADS, 0) AS AdsValue, O.GEO_POS_LAT AS Lat, O.GEO_POS_LON AS Lon, I.IMAGE_NAME AS Photo
//FROM         dbo.OUTLET_TYPES AS OT INNER JOIN
//                      dbo.DISTRIBUTION_HOUSES AS DH INNER JOIN
//                      dbo.ROUTES AS R ON DH.DISTRIBUTION_HOUSE_ID = R.DISTRIBUTION_HOUSE_ID INNER JOIN
//                      dbo.SECTIONS AS S ON R.ROUTE_ID = S.ROUTE_ID INNER JOIN
//                      dbo.CLUSTERS AS C ON S.SECTION_ID = C.SECTION_ID INNER JOIN
//                      dbo.OUTLETS AS O ON C.CLUSTER_ID = O.CLUSTER_ID ON OT.OUTLET_TYPE_ID = O.OUTLET_TYPE_ID INNER JOIN
//                      dbo.IMAGES_OUTLET AS I ON O.OUTLET_ID = I.OUTLET_ID LEFT OUTER JOIN
//                      dbo.BITS AS B INNER JOIN
//                      dbo.DEFAULT_BIT AS DB ON B.BIT_ID = DB.BIT_ID ON O.OUTLET_ID = DB.OUTLET_ID
//WHERE     (DH.DISTRIBUTION_HOUSE_ID > 0) AND (O.ACTIVE = 1)
//ORDER BY HouseName, RouteName, SectionName, ClusterName, OutletName

//            */
//            String sqlSelect = "Select * from viewSearchOutletsForCreateDefaultBit Where ClusterId=" + ClusterId + "";

//            DataTable dtSearchResult = new DataTable();
//            using (SqlDataAdapter da = new SqlDataAdapter(sqlSelect, WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString))
//            {
//                da.Fill(dtSearchResult);
//            }

//            String SerializedData = JsonConvert.SerializeObject(dtSearchResult, Formatting.None);

//            // String jsonDatatable = GetJson(dtSearchResult);


//            return Json(SerializedData, JsonRequestBehavior.AllowGet);
//        }

//        [HttpPost]
//        public JsonResult GetBitWiseOutletsForCreateDefaultBitForm(int bitid)
//        {
//            /* Original Source of viewSearchOutletsForCreateDefaultBit
//            SELECT     TOP (100) PERCENT DH.DISTRIBUTION_HOUSE_ID AS HouseId, DH.DISTRIBUTION_HOUSE_NAME AS HouseName, R.ROUTE_ID AS RouteId, 
//                      R.ROUTE_NAME AS RouteName, S.SECTION_ID AS SectionId, S.SECTION_NAME AS SectionName, C.CLUSTER_ID AS ClusterId, C.CLUSTER_NAME AS ClusterName, 
//                      ISNULL(B.BIT_ID, - 1) AS BitId, ISNULL(B.BIT_NAME, 'N/A') AS BitName, O.OUTLET_ID AS OutletId, O.OUTLET_NAME AS OutletName, 
//                      OT.OUTLET_TYPE_NAME AS OutletType, ISNULL(O.ADS, 0) AS AdsValue, O.GEO_POS_LAT AS Lat, O.GEO_POS_LON AS Lon, I.IMAGE_NAME AS Photo
//FROM         dbo.OUTLET_TYPES AS OT INNER JOIN
//                      dbo.DISTRIBUTION_HOUSES AS DH INNER JOIN
//                      dbo.ROUTES AS R ON DH.DISTRIBUTION_HOUSE_ID = R.DISTRIBUTION_HOUSE_ID INNER JOIN
//                      dbo.SECTIONS AS S ON R.ROUTE_ID = S.ROUTE_ID INNER JOIN
//                      dbo.CLUSTERS AS C ON S.SECTION_ID = C.SECTION_ID INNER JOIN
//                      dbo.OUTLETS AS O ON C.CLUSTER_ID = O.CLUSTER_ID ON OT.OUTLET_TYPE_ID = O.OUTLET_TYPE_ID INNER JOIN
//                      dbo.IMAGES_OUTLET AS I ON O.OUTLET_ID = I.OUTLET_ID LEFT OUTER JOIN
//                      dbo.BITS AS B INNER JOIN
//                      dbo.DEFAULT_BIT AS DB ON B.BIT_ID = DB.BIT_ID ON O.OUTLET_ID = DB.OUTLET_ID
//WHERE     (DH.DISTRIBUTION_HOUSE_ID > 0) AND (O.ACTIVE = 1)
//ORDER BY HouseName, RouteName, SectionName, ClusterName, OutletName

//            */
//            String sqlSelect = "Select * from viewSearchOutletsForCreateDefaultBit Where BitId=" + bitid + "";

//            DataTable dtSearchResult = new DataTable();
//            using (SqlDataAdapter da = new SqlDataAdapter(sqlSelect, WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString))
//            {
//                da.Fill(dtSearchResult);
//            }

//            String SerializedData = JsonConvert.SerializeObject(dtSearchResult, Formatting.None);

//            // String jsonDatatable = GetJson(dtSearchResult);


//            return Json(SerializedData, JsonRequestBehavior.AllowGet);
//        }

//        public JsonResult UpdateDefaultBit(int outletid,int previousbitid,int currentbitid)
//        {
//            String result = "Success";
//            try
//            {
//                String ConnectionString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
//                String sqlString = String.Empty;
//                if (previousbitid==-1) //insert
//                {
//                    sqlString = "insert into DEFAULT_BIT(BIT_ID,OUTLET_ID) VALUES("+ currentbitid +","+ outletid +")";
//                }
//                else
//                {
//                    if (currentbitid==-1)
//                    {
//                        sqlString = "delete from DEFAULT_BIT where OUTLET_ID=" + outletid + " and BIT_ID="+ previousbitid +"";
//                    }
//                    else
//                    {
//                        sqlString = "update DEFAULT_BIT set BIT_ID=" + currentbitid + " where  OUTLET_ID=" + outletid + " and BIT_ID="+ previousbitid +"";
//                    }
//                }
//                using (SqlConnection connection=new SqlConnection(ConnectionString))
//                {
//                    using (SqlCommand command=new SqlCommand(sqlString,connection))
//                    {
//                        connection.Open(); command.ExecuteNonQuery(); connection.Close();
//                    }
//                }
//            }
//            catch (Exception Ex)
//            {

//                result =Ex.Message;
//            }

//            return Json(result, JsonRequestBehavior.AllowGet);
//        }


        //#region Tree View

        //public ActionResult GetTreeView()
        //{
        //    return View("GetTreeView");
        //}


        //// [HttpPost]
        //public JsonResult GetRootNodes()
        //{
        //    List<BATB.Models.DynaTreeNode> nodes = BATB.Models.DynaTreeManager.GetRootNodes();
        //    return Json(nodes, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetChildNodes()
        //{
        //    String Key = Request.QueryString["key"].ToString();
        //    List<BATB.Models.DynaTreeNode> nodes = BATB.Models.DynaTreeManager.GetChildNodes(Key);
        //    return Json(nodes, JsonRequestBehavior.AllowGet);
        //}

        //#endregion

        #region User management


        public ActionResult ManageUser()
        {
            return View();
        }

        public JsonResult getUsers()
        {
            using (DataAccess.PollidutEntities db = new DataAccess.PollidutEntities())
            {
                var users = (from u in db.USERS
                             where u.ACTIVE == true
                             select new
                             {
                                 ID = u.USER_ID,
                                 Name = u.USER_NAME
                             }).ToList();
                return Json(users);
            }
        }
        public JsonResult getUserType()
        {
            using (DataAccess.PollidutEntities db = new DataAccess.PollidutEntities())
            {
                var userType = (from ut in db.USER_TYPES
                                where ut.ACTIVE == true
                                select new
                                {
                                    ID = ut.USER_TYPE_ID,
                                    Name = ut.USER_TYPE_NAME
                                }).ToList();
                return Json(userType);
            }
        }
        public JsonResult getLoginName(string longinName)
        {
            using (DataAccess.PollidutEntities db = new DataAccess.PollidutEntities())
            {
                var loginName = (from l in db.USERS where l.LOGIN_NAME == longinName select l).FirstOrDefault();
                if (loginName == null)
                {
                    return Json(false);
                }
                else
                    return Json(true);
            }

        }
        public JsonResult getEmployee()
        {
            return null;
        }

        public class userInfo
        {
            public string userName { get; set; }
            public string password { get; set; }
            public string loginName { get; set; }
            public int userType { get; set; }
            public int? employeeId { get; set; }
        }
        public JsonResult addUser(userInfo user)
        {
            using (DataAccess.PollidutEntities db = new DataAccess.PollidutEntities())
            {
                var loginName = (from l in db.USERS where l.LOGIN_NAME == user.loginName select l).FirstOrDefault();
                var employeeId = (from e in db.USERS where e.EMPLOYEE_ID == user.employeeId select e).FirstOrDefault();
                if (loginName == null)
                {
                    if (employeeId == null)
                    {
                        var userdata = new DataAccess.USER
                        {
                            USER_NAME = user.userName,
                            PASSWORD = user.password,
                            LOGIN_NAME = user.loginName,
                            USER_TYPE_ID = user.userType,
                            EMPLOYEE_ID = user.employeeId,
                            ACTIVE = true
                        };
                        db.USERS.Add(userdata);
                        db.SaveChanges();
                    }
                    else
                    {
                        return Json("Employee already have a Login Name");
                    }
                }
                else
                    return Json("Login Name Already Exist");

                db.SaveChanges();
                return Json("Success");
            }
        }
        public ActionResult DeleteUserProfile()
        {
            return View("DeleteUserProfile");
        }

        public JsonResult deleteUser(int ID)
        {
            using (DataAccess.PollidutEntities db = new DataAccess.PollidutEntities())
            {
                var deletepriv = db.USERS.Where(u => u.USER_ID == ID).FirstOrDefault();
                deletepriv.ACTIVE = false;
                db.SaveChanges();
                return Json("Success");
            }
        }

        public ActionResult GetUserEmployee()
        {
            List<DistributionHouse> houses = DistributionHouseManager.GetDistributionHouses();
            ViewBag.Houses = houses;

            List<Pollidut.Models.District> districts = Pollidut.Models.DistrictManager.GetDistricts();
            ViewBag.Districts = districts;

            List<Pollidut.Models.EmployeeType> employeeTypes = Pollidut.Models.EmployeeTypeManager.GetEmployeeTypes();
            ViewBag.EmployeeTypes = employeeTypes;

            List<Pollidut.Models.Designation> designations = Pollidut.Models.DesignationManager.GetDesignations();
            ViewBag.Designations = designations;


            return View("GetEmployee");
        }

        [HttpPost]
        public JsonResult GetUserEmployee(FormCollection data)
        {

            String DistributionHouseId = data["distributionhouseid"].ToString();//
            String PersonCurrentStatusId = data["personcurrentstatusid"].ToString();//
            String IsActive = data["active"].ToString();//
            String EmployeeCode = data["employeecode"].ToString();//
            String PersonName = data["personname"].ToString();//
            String MobileNo = data["mobileno"].ToString();//
            String Email = data["email"].ToString();//
            String NID = data["nid"].ToString();//
            //  String DesignationId = data["designationid"].ToString();//
            String FromDateOfBirth = data["fromdateofbirth"].ToString();
            String ToDateOfBirth = data["todateofbirth"].ToString();
            String GenderId = data["genderid"].ToString();
            String MarritalStatusId = data["marritalstatusid"].ToString();
            String DistrictId = data["districtid"].ToString();
            String EmployeeTypeId = data["employeetypeid"].ToString();

            String FromStartDate = data["fromstartdate"].ToString();
            String ToStartDate = data["tostartdate"].ToString();

            String FromEndDate = data["fromenddate"].ToString();
            String ToEndDate = data["toenddate"].ToString();


            String WhereClause = String.Empty;

            if (!String.IsNullOrEmpty(DistributionHouseId))
            {
                WhereClause = "DistributionHouseId=" + DistributionHouseId;
            }

            if (!String.IsNullOrEmpty(PersonCurrentStatusId))
            {
                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "PersonCurrentStatusId=" + PersonCurrentStatusId;
                }
                else
                {
                    WhereClause += " AND PersonCurrentStatusId=" + PersonCurrentStatusId;
                }
            }

            if (!String.IsNullOrEmpty(IsActive))
            {
                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "IsActive=" + IsActive;
                }
                else
                {
                    WhereClause += " AND IsActive=" + IsActive;

                }
            }

            if (!String.IsNullOrEmpty(EmployeeCode))
            {
                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "EmployeeCode=" + EmployeeCode;
                }
                else
                {
                    WhereClause += " AND EmployeeCode=" + EmployeeCode;

                }
            }


            if (!String.IsNullOrEmpty(PersonName))
            {
                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "PersonName Like '%" + PersonName + "%'";
                }
                else
                {
                    WhereClause += " AND PersonName Like'%" + PersonName + "%'";

                }
            }

            if (!String.IsNullOrEmpty(MobileNo))
            {
                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "Mobile Like '%" + MobileNo + "%'";
                }
                else
                {
                    WhereClause += " AND Mobile Like'%" + MobileNo + "%'";

                }
            }

            //Email
            if (!String.IsNullOrEmpty(Email))
            {
                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "Email='%" + Email + "%'";
                }
                else
                {
                    WhereClause += " AND Email='%" + Email + "%'";

                }
            }

            //NID
            if (!String.IsNullOrEmpty(NID))
            {
                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "NID='%" + NID + "%'";
                }
                else
                {
                    WhereClause += " AND NID='%" + NID + "%'";

                }
            }


            //DesignationId
            //if (!String.IsNullOrEmpty(DesignationId))
            //{
            //    if (String.IsNullOrEmpty(WhereClause))
            //    {
            //        WhereClause = "DesignationId=" + DesignationId;
            //    }
            //    else
            //    {
            //        WhereClause += " AND DesignationId=" + DesignationId;

            //    }
            //}

            if (!String.IsNullOrEmpty(FromDateOfBirth) && !String.IsNullOrEmpty(ToDateOfBirth))
            {
                DateTime fromDateOfBirth; DateTime.TryParseExact(FromDateOfBirth, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out fromDateOfBirth);
                DateTime toDateOfBirth; DateTime.TryParseExact(ToDateOfBirth, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out toDateOfBirth);

                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "DateOfBirth Between '" + fromDateOfBirth.ToString("yyyy-MM-dd") + "' AND '" + toDateOfBirth.ToString("yyyy-MM-dd") + "'";
                }
                else
                {
                    WhereClause = " AND DateOfBirth Between '" + fromDateOfBirth.ToString("yyyy-MM-dd") + "' AND '" + toDateOfBirth.ToString("yyyy-MM-dd") + "'";

                }
            }
            else if (!String.IsNullOrEmpty(FromDateOfBirth) && String.IsNullOrEmpty(ToDateOfBirth))
            {
                DateTime fromDateOfBirth; DateTime.TryParseExact(FromDateOfBirth, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out fromDateOfBirth);

                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "DateOfBirth >= '" + fromDateOfBirth.ToString("yyyy-MM-dd") + "'";
                }
                else
                {
                    WhereClause = "DateOfBirth >= '" + fromDateOfBirth.ToString("yyyy-MM-dd") + "'";
                }
            }
            else if (String.IsNullOrEmpty(FromDateOfBirth) && !String.IsNullOrEmpty(ToDateOfBirth))
            {
                DateTime toDateOfBirth; DateTime.TryParseExact(FromDateOfBirth, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out toDateOfBirth);

                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "DateOfBirth <= '" + toDateOfBirth.ToString("yyyy-MM-dd") + "'";
                }
                else
                {
                    WhereClause = "DateOfBirth <= '" + toDateOfBirth.ToString("yyyy-MM-dd") + "'";
                }
            }

            //GenderId
            if (!String.IsNullOrEmpty(GenderId))
            {
                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "GenderId=" + GenderId;
                }
                else
                {
                    WhereClause += " AND GenderId=" + GenderId;
                }
            }


            //MarritalStatusId
            if (!String.IsNullOrEmpty(MarritalStatusId))
            {
                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "MarritalStatusId=" + MarritalStatusId;
                }
                else
                {
                    WhereClause += " AND MarritalStatusId=" + MarritalStatusId;
                }
            }

            // DistrictId 
            if (!String.IsNullOrEmpty(DistrictId))
            {
                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "DistrictId=" + DistrictId;
                }
                else
                {
                    WhereClause += " AND DistrictId=" + DistrictId;
                }
            }

            //EmployeeTypeId
            if (!String.IsNullOrEmpty(EmployeeTypeId))
            {
                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "EmployeeTypeId=" + EmployeeTypeId;
                }
                else
                {
                    WhereClause += " AND EmployeeTypeId=" + EmployeeTypeId;
                }
            }


            if (!String.IsNullOrEmpty(FromStartDate) && !String.IsNullOrEmpty(ToStartDate))
            {
                DateTime fromStartDate; DateTime.TryParseExact(FromStartDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out fromStartDate);
                DateTime toStartDate; DateTime.TryParseExact(ToStartDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out toStartDate);

                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "StartDate Between '" + fromStartDate.ToString("yyyy-MM-dd") + "' AND '" + toStartDate.ToString("yyyy-MM-dd") + "'";
                }
                else
                {
                    WhereClause += " AND StartDate Between '" + fromStartDate.ToString("yyyy-MM-dd") + "' AND '" + toStartDate.ToString("yyyy-MM-dd") + "'";

                }
            }
            else if (!String.IsNullOrEmpty(FromStartDate) && String.IsNullOrEmpty(ToStartDate))
            {
                DateTime fromStartDate; DateTime.TryParseExact(FromStartDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out fromStartDate);

                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "StartDate >= '" + fromStartDate.ToString("yyyy-MM-dd") + "'";
                }
                else
                {
                    WhereClause += " AND StartDate >= '" + fromStartDate.ToString("yyyy-MM-dd") + "'";
                }
            }
            else if (String.IsNullOrEmpty(FromStartDate) && !String.IsNullOrEmpty(ToStartDate))
            {
                DateTime toStartDate; DateTime.TryParseExact(ToStartDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out toStartDate);

                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "StartDate <= '" + toStartDate.ToString("yyyy-MM-dd") + "'";
                }
                else
                {
                    WhereClause += " AND StartDate <= '" + toStartDate.ToString("yyyy-MM-dd") + "'";
                }
            }

            if (!String.IsNullOrEmpty(FromEndDate) && !String.IsNullOrEmpty(ToEndDate))
            {
                DateTime fromEndDate; DateTime.TryParseExact(FromEndDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out fromEndDate);
                DateTime toEndDate; DateTime.TryParseExact(ToEndDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out toEndDate);

                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "EndDate Between '" + fromEndDate.ToString("yyyy-MM-dd") + "' AND '" + toEndDate.ToString("yyyy-MM-dd") + "'";
                }
                else
                {
                    WhereClause += " AND EndDate Between '" + fromEndDate.ToString("yyyy-MM-dd") + "' AND '" + toEndDate.ToString("yyyy-MM-dd") + "'";

                }
            }
            else if (!String.IsNullOrEmpty(FromEndDate) && String.IsNullOrEmpty(ToEndDate))
            {
                DateTime fromEndDate; DateTime.TryParseExact(FromEndDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out fromEndDate);

                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "EndDate >= '" + fromEndDate.ToString("yyyy-MM-dd") + "'";
                }
                else
                {
                    WhereClause += " AND EndDate >= '" + fromEndDate.ToString("yyyy-MM-dd") + "'";
                }
            }
            else if (String.IsNullOrEmpty(FromEndDate) && !String.IsNullOrEmpty(ToEndDate))
            {
                DateTime toEndDate; DateTime.TryParseExact(ToEndDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out toEndDate);

                if (String.IsNullOrEmpty(WhereClause))
                {
                    WhereClause = "EndDate <= '" + toEndDate.ToString("yyyy-MM-dd") + "'";
                }
                else
                {
                    WhereClause += " AND EndDate <= '" + toEndDate.ToString("yyyy-MM-dd") + "'";
                }
            }

            if (!String.IsNullOrEmpty(WhereClause))
            {
                WhereClause = " Where " + WhereClause;
            }

            String sqlSelect = "select  DistributionHouseName,EmployeeId , EmployeeCode, PersonName, EmployeeTypeName, IsActive,Mobile,Photo from viewSearchEmployee " + WhereClause;

            DataTable dtSearchResult = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(sqlSelect, WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString))
            {
                da.Fill(dtSearchResult);
            }

            String SerializedData = JsonConvert.SerializeObject(dtSearchResult, Formatting.None);
            return Json(SerializedData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        //#region Default CM Wise Plans
        //public ActionResult DefaultCmWisePlansManagement()
        //{
        //    List<DistributionHouse> houses = DistributionHouseManager.GetDistributionHouses();
        //    List<BATB.Models.EmployeeType> empTypes = BATB.Models.EmployeeTypeManager.GetEmployeeTypes();

        //    ViewBag.Houses = houses;
        //    ViewBag.PositionTypes = empTypes;
        //    return View("DefaultCmWisePlansManagement");
        //}

        //[HttpPost]
        //public JsonResult GetHouseSpecificValidPositionAndBitList(int houseid)
        //{
        //    List<BATB.Models.ServerToClientModel.HouseSpecificCmPositionAndBit> list =
        //        BATB.Models.ServerToClientModel.HouseSpecificCmPositionAndBitManager.GetHouseSpecificValidPositionAndBitList(houseid);

        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}


        //[HttpPost]
        //public JsonResult GetHouseSpecificUnassignedPositionList(int houseid)
        //{
        //    List<BATB.Models.ServerToClientModel.HouseSpecificCmPositionAndBit> list =
        //        BATB.Models.ServerToClientModel.HouseSpecificCmPositionAndBitManager.GetHouseSpecificUnassignedPositionList(houseid);

        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public JsonResult GetHouseSpecificUnassignedBitList(int houseid)
        //{
        //    List<BATB.Models.ServerToClientModel.HouseSpecificCmPositionAndBit> list =
        //        BATB.Models.ServerToClientModel.HouseSpecificCmPositionAndBitManager.GetHouseSpecificUnassignedBitList(houseid);

        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public JsonResult GetHouseAndPositionTypeSpecificPositionAndBitList(int houseid, int positiontypeid)
        //{
        //    List<BATB.Models.ServerToClientModel.HouseSpecificCmPositionAndBit> list =
        //        BATB.Models.ServerToClientModel.HouseSpecificCmPositionAndBitManager.GetHouseAndPositionTypeSpecificPositionAndBitList(houseid, positiontypeid);

        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public JsonResult GetPositionSpecificBitList(int positionid)
        //{
        //    List<BATB.Models.ServerToClientModel.EmployeePositionSpecificBit> list =
        //        BATB.Models.ServerToClientModel.EmployeePositionSpecificBitManager.GetPositionSpecificBitList(positionid);

        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public JsonResult GetHouseAndTypeSpecificEmployeePositions(Int32 houseid, Int32 positiontypeid)
        //{
        //    List<EmployeePosition> list = EmployeePositionManager.GetHouseAndTypeSpecificEmployeePositions(houseid, positiontypeid);
        //    return Json(list, JsonRequestBehavior.AllowGet);

        //}

        //[HttpPost]
        //public JsonResult SaveDefaultCmWisePlans(int positionid, int bitid)
        //{

        //    String saveStatus = string.Empty;

        //    String connString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
        //    using (SqlConnection connection = new SqlConnection(connString))
        //    {
        //        String sqlCheck = "select CM_POSITION_ID from DEFAULT_CM_WISE_PLANS where CM_POSITION_ID=" + positionid + " and BIT_ID=" + bitid + "";
        //        String sqlInsert = "insert into DEFAULT_CM_WISE_PLANS (CM_POSITION_ID,BIT_ID) values(" + positionid + "," + bitid + ")";

        //        using (SqlCommand command = new SqlCommand())
        //        {
        //            command.Connection = connection; connection.Open();
        //            command.CommandText = sqlCheck;
        //            object result = command.ExecuteScalar();

        //            if (result == null || result == DBNull.Value)
        //            {
        //                command.CommandText = sqlInsert;
        //                command.ExecuteNonQuery(); saveStatus = "Success";
        //            }
        //            else
        //            {
        //                saveStatus = "This position already assigned to this bit. Please choose another bit.";

        //            }

        //            connection.Close();
        //        }

        //    }
        //    return Json(saveStatus, JsonRequestBehavior.AllowGet);
        //}


        //[HttpPost]
        //public JsonResult RemoveDefaultCmWisePlans(int positionid, int bitid)
        //{

        //    String saveStatus = string.Empty;

        //    String connString = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
        //    using (SqlConnection connection = new SqlConnection(connString))
        //    {
        //        String sqlDelete = "Delete from DEFAULT_CM_WISE_PLANS where CM_POSITION_ID =" + positionid + " and BIT_ID=" + bitid + "";

        //        using (SqlCommand command = new SqlCommand())
        //        {
        //            command.Connection = connection; connection.Open();

        //            command.CommandText = sqlDelete;
        //            command.ExecuteNonQuery(); saveStatus = "Success";
        //            connection.Close();
        //        }

        //    }
        //    return Json(saveStatus, JsonRequestBehavior.AllowGet);
        //}

        //#endregion





    }
}
