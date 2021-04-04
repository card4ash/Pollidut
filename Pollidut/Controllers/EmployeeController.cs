using Pollidut.DataAccess;
using Newtonsoft.Json;
using sCommonLib;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Pollidut.Models;
using System.Web.Script.Serialization;
using Pollidut.ViewModels;

namespace Pollidut.Controllers
{
    public class EmployeeController : Controller
    {
        //
        // GET: /Employee/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult UpdatePD(FormCollection data)
        {
            try
            {
                var serializer = new JavaScriptSerializer();
                var familyDetail = data["family"].ToString();
                var workDetail = data["works"].ToString();
                var distributionDetail = data["distributions"].ToString();
                var deserializedFamily = serializer.Deserialize<List<FamilyDetail>>(familyDetail);
                var deserializedWork = serializer.Deserialize<List<WorkDetail>>(workDetail);
                var deserializedDistribution = serializer.Deserialize<List<DistributionModel>>(distributionDetail);

                int personId = Convert.ToInt32(data["personId"].ToString());
                int employeeId = Convert.ToInt32(data["empId"].ToString());
                string personname = data["personname"].ToString().Trim();
                string empcode = data["empcode"].ToString().Trim();
                string fathername = data["fathername"].ToString().Trim();
                string mothername = data["mothername"].ToString().Trim();
                string mobileno = data["mobileno"].ToString().Trim();
                string education = data["education"].ToString().Trim();
                string dateofbirth = data["dateofbirth"].ToString().Trim();
                string nidno = data["nidno"].ToString().Trim();
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
                    using (var binaryReader = new BinaryReader(Request.Files["photo"].InputStream))
                    {
                        imagefile = binaryReader.ReadBytes(Request.Files["photo"].ContentLength);//image
                    }
                    BinaryWriter bw = new BinaryWriter(fs);
                    bw.Write(imagefile);
                    bw.Close();
                }
                /****************************************Update************/
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
                            int empId = Convert.ToInt32(Session["EmployeeId"].ToString());
                            int firstDisId = deserializedDistribution.FirstOrDefault().Id == null ? 0 : deserializedDistribution.FirstOrDefault().Id;
                            var person = (from a in db.PERSONS where a.PERSON_ID == personId select a).FirstOrDefault();
                            person.PERSON_NAME = personname;
                            person.FATHERS_NAME = fathername;
                            person.MOTHERS_NAME = mothername;
                            person.MOBILE = mobileno;
                            person.EDUCATIONAL_QUALIFICATION = education;
                            person.DATE_OF_BIRTH = birthDate;
                            person.NID = nidno;
                            person.NATIONALITY = nationality;
                            person.RELIGION_ID = religionid;
                            person.MAILING_ADDRESS = presentaddress;
                            person.PERMANENT_ADDRESS = permanentaddress;
                            person.MARITAL_STATUS_ID = relationship;
                            person.FAMILY_MEMBER_NO = memberno;
                            person.MALE_NO = malemember;
                            person.FEMALE_NO = femalemember;
                            person.ACTIVE = true;
                            person.UPDATE_BY = empId;
                            person.UPDATE_DATE = todaty;
                            person.DISTRIBUTION_HOUSE_ID = firstDisId;
                            if (Request.Files["photo"] != null)
                            {
                                person.PERSON_IMAGE = imageName;
                            }
                            db.SaveChanges();
                            foreach (var fmember in deserializedFamily)
                            {
                                //insert data
                                if (fmember.id == 0)
                                {
                                    var eachMember = new DataAccess.PERSON_FAMILY
                                    {
                                        PERSON_ID = person.PERSON_ID,
                                        MEMBER_NAME = fmember.memberName,
                                        RELATION = fmember.relation,
                                        AGE = fmember.age,
                                        OCCUPATION = fmember.occupation,
                                        ACTIVE = true,

                                    };
                                    db.PERSON_FAMILY.Add(eachMember);
                                    db.SaveChanges();
                                }
                                //remove the data
                                else if (fmember.id < 0)
                                {
                                    int memberId = fmember.id * (-1);
                                    var rmvFmember = (from a in db.PERSON_FAMILY where a.FAMILY_ID == memberId select a).FirstOrDefault();
                                    rmvFmember.ACTIVE = false;
                                    db.SaveChanges();
                                }
                                //update data
                                else
                                {
                                    var rmvFmember = (from a in db.PERSON_FAMILY where a.FAMILY_ID == fmember.id select a).FirstOrDefault();
                                    rmvFmember.MEMBER_NAME = fmember.memberName;
                                    rmvFmember.OCCUPATION = fmember.occupation;
                                    rmvFmember.RELATION = fmember.relation;
                                    rmvFmember.AGE = fmember.age;
                                    db.SaveChanges();
                                }
                                
                            }
                            db.SaveChanges();

                            var emp = (from e in db.EMPLOYEES where e.EMPLOYEE_ID==employeeId select e).FirstOrDefault();
                            emp.PERSON_ID = person.PERSON_ID;
                            emp.EMPLOYEE_CODE_ = empcode;
                            emp.EMPLOYEE_CODE = empcode;
                            emp.EMPLOYEE_TYPE_ID = employeetypeid;
                            emp.JOINING_DATE = DateOfJoin;
                            emp.MINIMUM_INVESTMENT = minimumInvestment;
                            emp.OTHER_FINANCIAL = otherFinace;
                            emp.DISTRIBUTION_HOUSE_ID = firstDisId;
                            emp.PREVIOUS_PD_DETAIL = previouspd;
                            emp.SERVICE_DESCRIPTION = pdservice;
                            emp.UPDATED_BY = empId;
                            emp.UPDATED_DATE = todaty;
                            db.SaveChanges();
                            foreach (var eachWork in deserializedWork)
                            {
                                if (eachWork.id == 0)
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
                                else if (eachWork.id < 0)
                                {
                                    int workId = eachWork.id * (-1);
                                    var rmvwork = (from w in db.EMPLOYEE_WORK where w.WORK_ID == workId select w).FirstOrDefault();
                                    rmvwork.ACTIVE = false;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    var rmvwork = (from w in db.EMPLOYEE_WORK where w.WORK_ID == eachWork.id select w).FirstOrDefault();
                                    rmvwork.WORK_AREA = eachWork.areaname;
                                    rmvwork.THANA_NAME = eachWork.thananame;
                                    rmvwork.UNION_NAME = eachWork.unionname;
                                    db.SaveChanges();
                                }
                                
                            }
                            db.SaveChanges();
                            return Json(new { savestatus = "Success", employeeid = employeeId });
                            //foreach (var distribution in deserializedDistribution)
                            //{
                            //    var dis = new DataAccess.EMPLOYEE_DISTRIBUTION_HOUSES
                            //    {
                            //        EmployeeId = emp.EMPLOYEE_ID,
                            //        DistributionHouseId = distribution.Id,
                            //        Active = true
                            //    };
                            //    db.EMPLOYEE_DISTRIBUTION_HOUSES.Add(dis);
                            //}
                            //db.SaveChanges();
                        }
                        return Json(new { savestatus = "Success", employeeid = employeeId });

                    }
                catch (Exception Ex)
                {
                    return Json(Ex.Message, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception Ex)
            {
                return Json(Ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult getEmployeeDetails(int id)
        {
            String LoginName = Session["LoginName"].ToString();
            String UserId = Session["UserId"].ToString();
            String FullName = Session["FullName"].ToString();
            String UserTypeId = Session["UserTypeId"].ToString();
            Int32 EmployeeId = Convert.ToInt32(Session["EmployeeId"]);
            ViewBag.LoginName = LoginName;
            ViewBag.UserId = UserId;
            ViewBag.FullName = FullName;
            String DashboardTitle = String.Empty;
            String UserTypeName = String.Empty;
            Int32 DistributionHouseId = 0;
            String DistributionHouseName = "N/A";
            switch (UserTypeId)
            {
                case "1": UserTypeName = "SuperAdmin"; DashboardTitle = "Super Admin"; break;
                case "2": UserTypeName = "Pollidut"; DashboardTitle = "Pollidut"; break;
                case "3": UserTypeName = "PollidutSupervisor"; DashboardTitle = "Pollidut Supervisor"; break; //for CE project
                default:
                    break;
            }

            ViewBag.DashboardTitle = DashboardTitle; ViewBag.UserTypeName = UserTypeName;
            ViewBag.DistributionHouseId = DistributionHouseId; ViewBag.DistributionHouseName = DistributionHouseName;





            int empId = Convert.ToInt32(Session["EmployeeId"].ToString());
            List<DistributionHouse> houses = DistributionHouseManager.GetSupervisorHouse(empId);
            List<PersonCurrentStatus> personcurrentstatuses = PersonCurrentStatusManager.GetStatuses();
            List<Pollidut.Models.EmployeeType> employeeTypes = Pollidut.Models.EmployeeTypeManager.GetEmployeeTypes();
            List<Religion> religions = Pollidut.Models.ReligionManager.GetReligions();
            List<MaritalStatus> mstatus = Pollidut.Models.MaritalStatusManager.GetMaritalStatuses();
            EMPLOYEE employee = new EMPLOYEE();
            using(PollidutEntities db=new PollidutEntities())
            {
                employee = db.EMPLOYEES.Include("PERSON").Include("PERSON.PERSON_FAMILY").Include("EMPLOYEE_WORK").Where(e => e.EMPLOYEE_ID == id).FirstOrDefault();
                //ViewBag.Test = employee.EMPLOYEE_ID;
                //ViewBag.Test2=employee.PERSON_ID
                //foreach(var item in employee.EMPLOYEE_WORK)
                //{
                //    string x=item.WORK_ID
                //}
                ViewBag.Employee = employee;
            }
            ViewBag.RelationShip = mstatus;
            ViewBag.Religions = religions;
            ViewBag.Houses = houses;
            ViewBag.PersonCurrentStatuses = personcurrentstatuses;
            ViewBag.EmployeeTypes = employeeTypes;
            return View();
        }

        public ActionResult EmployeeProfile()
        {
            Int32 id = 0;
            try
            {
                id = Convert.ToInt32(Request.QueryString["id"].ToString());
            }
            catch (Exception)
            {
                id = 0;
            }


            try
            {
                //String connectionString = ConfigurationManager.ConnectionStrings[""].ConnectionString;
                using (var dbEntities = new PollidutEntities())
                {
                    var employee = dbEntities.View_EMPLOYEES.Where(e => e.EMPLOYEE_ID == id).FirstOrDefault();
                    ViewBag.Employee = employee;

                    var supervisorID = employee.SUPERVISOR_ID;
                    var supervisorPersonID = dbEntities.EMPLOYEES.Where(e => e.EMPLOYEE_ID == supervisorID).FirstOrDefault().PERSON_ID;
                    var supervisorName = dbEntities.PERSONS.Where(p => p.PERSON_ID == supervisorPersonID).FirstOrDefault().PERSON_NAME;
                    ViewBag.SupervisorName = supervisorName;

                    var areaId = dbEntities.DISTRIBUTION_HOUSES.Where(d => d.DISTRIBUTION_HOUSE_ID == dbEntities.EMPLOYEES.Where(e => e.EMPLOYEE_ID == employee.EMPLOYEE_ID).FirstOrDefault().DISTRIBUTION_HOUSE_ID).FirstOrDefault().AREA_ID;
                    var areaName = dbEntities.AREAS.Where(a => a.AREA_ID == areaId).FirstOrDefault().AREA_NAME;
                    ViewBag.AreaName = areaName;

                    var regionName = dbEntities.REGIONS.Where(r => r.REGION_ID == (dbEntities.AREAS.Where(a => a.AREA_ID == areaId).FirstOrDefault().REGION_ID)).FirstOrDefault().REGION_NAME;
                    ViewBag.RegionName = regionName;
                }

                return View();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult EmployeeEdit()
        {
            int personId = 8;
            using (var _context = new PollidutEntities())
            {
                var person = _context.PERSONS.Where(p => p.PERSON_ID == personId).FirstOrDefault();
                ViewBag.PersonName = person.PERSON_NAME;
            }
            return View();
        }

        public ActionResult EmployeeDetail()
        {
            
            Int32 id = 0;
            try
            {
                id = Convert.ToInt32(Request.QueryString["id"].ToString());
            }
            catch (Exception)
            {
                id = 0;
            }


            try
            {
                //String connectionString = ConfigurationManager.ConnectionStrings[""].ConnectionString;
                using (var dbEntities = new PollidutEntities())
                {
                    var employee = dbEntities.View_EMPLOYEES.Where(e => e.EMPLOYEE_ID == id).FirstOrDefault();
                    if (employee != null)
                    {
                        var employeeId = employee.EMPLOYEE_ID == null ? 0 : employee.EMPLOYEE_ID;
                        var x = dbEntities.Database.Connection.ConnectionString;
                        var person = dbEntities.View_PERSON.Where(p => p.PERSON_ID == employee.PERSON_ID).FirstOrDefault();
                        ViewBag.Person = person;
                        ViewBag.Employee = employee;

                        var familyDetail = dbEntities.PERSON_FAMILY.Where(f => f.PERSON_ID == employee.PERSON_ID).ToList();
                        ViewBag.FamilyDetail = familyDetail;
                        var workArea = dbEntities.EMPLOYEE_WORK.Where(w => w.EMPLOYEE_ID == employee.EMPLOYEE_ID).ToList();
                        ViewBag.WorkAra = workArea;
                        

                        var supervisorID = employee.SUPERVISOR_ID == null ? 0 : employee.SUPERVISOR_ID;
                        var supervisor = dbEntities.View_EMPLOYEES.Where(e => e.EMPLOYEE_ID == supervisorID).FirstOrDefault();
                        var supervisorPerson = dbEntities.PERSONS.Where(e => e.PERSON_ID == supervisor.PERSON_ID).FirstOrDefault();
                        var supervisorPersonID = supervisorPerson.PERSON_ID == null ? 0 : supervisorPerson.PERSON_ID;
                        var supervisorName = supervisorPerson.PERSON_NAME;
                        ViewBag.SupervisorName = supervisorName;

                        var areaId = dbEntities.DISTRIBUTION_HOUSES.Where(d => d.DISTRIBUTION_HOUSE_ID == dbEntities.EMPLOYEES.Where(e => e.EMPLOYEE_ID == employee.EMPLOYEE_ID).FirstOrDefault().DISTRIBUTION_HOUSE_ID).FirstOrDefault().AREA_ID;
                        var areaName = dbEntities.AREAS.Where(a => a.AREA_ID == areaId).FirstOrDefault().AREA_NAME;
                        ViewBag.AreaName = areaName;

                        var regionName = dbEntities.REGIONS.Where(r => r.REGION_ID == (dbEntities.AREAS.Where(a => a.AREA_ID == areaId).FirstOrDefault().REGION_ID)).FirstOrDefault().REGION_NAME;
                        ViewBag.RegionName = regionName;

                        List<DataAccess.PERSON_DEGREES> personDegrees = dbEntities.PERSON_DEGREES.Where(pd => pd.PERSON_ID == employee.PERSON_ID && pd.DEGREE_ID != 0).ToList();
                        ViewBag.PersonDegrees = personDegrees;
                        List<DataAccess.PERSON_JOB_EXPERIENCES> personExperiences = dbEntities.PERSON_JOB_EXPERIENCES.Where(pd => pd.PERSON_ID == employee.PERSON_ID && pd.EXPERIENCE_ID != 0).ToList();
                        ViewBag.PersonExperiences = personExperiences;
                        List<DataAccess.PERSONS_REFERENCES> personReferences = dbEntities.PERSONS_REFERENCES.Where(pd => pd.PERSON_ID == employee.PERSON_ID && pd.REFERENCE_ID != 0).ToList();
                        ViewBag.PersonReferences = personReferences;
                    }
                    //Basic Info
                    List<Pollidut.Models.Gender> genders = Pollidut.Models.GenderManager.GetGenders();
                    ViewBag.Genders = genders;
                    List<Pollidut.Models.Religion> religions = Pollidut.Models.ReligionManager.GetReligions();
                    ViewBag.Religions = religions;
                    List<Pollidut.Models.MaritalStatus> maritalStatuses = Pollidut.Models.MaritalStatusManager.GetMaritalStatuses();
                    ViewBag.MaritalStatuses = maritalStatuses;
                    List<Pollidut.Models.District> districts = Pollidut.Models.DistrictManager.GetDistricts();
                    ViewBag.Districts = districts;
                    List<Pollidut.Models.Bank> banks = Pollidut.Models.BankManager.GetBanks();
                    ViewBag.Banks = banks;

                    //Employee Info
                    List<Pollidut.Models.Brand> brands = Pollidut.Models.BrandManager.GetBrands();
                    ViewBag.Brands = brands;
                    List<Pollidut.Models.DistributionHouse> distributionHouses = Pollidut.Models.DistributionHouseManager.GetDistributionHouses();
                    ViewBag.DistributionHouses = distributionHouses;
                    List<Pollidut.Models.EmployeeRoleStatus> employeeRoleStatuses = Pollidut.Models.EmployeeRoleStatusManager.GetEmployeeRoleStatuses();
                    ViewBag.EmployeeRoleStatuses = employeeRoleStatuses;
                    List<Pollidut.Models.PersonCurrentStatus> personCurrentStatuses = Pollidut.Models.PersonCurrentStatusManager.GetStatuses();
                    ViewBag.PersonCurrentStatuses = personCurrentStatuses;
                    List<Pollidut.Models.EmployeeType> employeeTypes = Pollidut.Models.EmployeeTypeManager.GetEmployeeTypes();
                    ViewBag.EmployeeTypes = employeeTypes;

                    //Education
                    List<Pollidut.Models.EducationalDegree> educationalDegrees = Pollidut.Models.EducationalDegreeManager.GetEducationalDegrees();
                    ViewBag.EducationalDegrees = educationalDegrees;
                    List<Pollidut.Models.EducationalGrade> educationalGrades = Pollidut.Models.EducationalGradeManager.GetGrades();
                    ViewBag.EducationalGrades = educationalGrades;
                    List<Pollidut.Models.EducationalDivision> educationalDivisions = Pollidut.Models.EducationalDivisionManager.GetDivisions();
                    ViewBag.EducationalDivisions = educationalDivisions;

                    //Experience
                    List<Pollidut.Models.ExperienceType> experienceTypes = Pollidut.Models.ExperienceTypeManager.GetExperienceTypes();
                    ViewBag.ExperienceTypes = experienceTypes;
                    //Reference
                    List<Pollidut.Models.ReferenceType> referenceTypes = Pollidut.Models.ReferenceTypeManager.GetReferenceTypes();
                    ViewBag.ReferenceTypes = referenceTypes;
                }

                return View();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

       

        public Nullable<System.DateTime> nullDate { get; set; }

        public ActionResult saveEmployee(System.Web.HttpPostedFileBase file, Employee employee)
        {
            try
            {
                string educationsErrorString = validateEducations(employee.educations);
                if (educationsErrorString != "")
                {
                    return Json(educationsErrorString, JsonRequestBehavior.AllowGet);
                }

                string experencesErrorString = validateExperience(employee.experiences);
                if (experencesErrorString != "")
                {
                    return Json(experencesErrorString, JsonRequestBehavior.AllowGet);
                }
                string referencesErrorString = validateReference(employee.references);
                if (referencesErrorString != "")
                {
                    return Json(referencesErrorString, JsonRequestBehavior.AllowGet);
                }

                #region Coments Image Handle
                //foreach (var photo in file)
                //{
                //    if (photo != null)
                //    {
                //        var fileName = GeneratePrefixFileName() + Path.GetFileName(photo.FileName);
                //        var path = Path.Combine(Server.MapPath("~/App_Data/uploads/cluster"), fileName);
                //        photo.SaveAs(path);
                //        //var imageCluster = new IMAGES_CLUSTERS()
                //        //{
                //        //    CLUSTER_ID = (formdata["clusterList"] == string.Empty) ? (int?)null : Convert.ToInt32(formdata["clusterList"]),
                //        //    CLUSTER_IMAGE_NAME = fileName,
                //        //    ACTIVE = true,
                //        //    USER_ID = 1
                //        //};
                //        dataContext.IMAGES_CLUSTERS.Add(imageCluster);
                //        dataContext.SaveChanges();
                //    }
                //}
                #endregion

                Byte[] imagefile;
                String imageName = String.Empty;

                if (file != null)
                {
                    //if (employee.imageName != null)
                    //{
                    //    imageName = employee.imageName.Split('.')[0] + ".png";
                    //}
                    //else
                    //{
                    imageName = Guid.NewGuid().ToString() + ".png";
                    //}
                }
                else
                {
                    if (employee.imageName != null)
                    {
                        imageName = employee.imageName.Split('.')[0] + ".png";
                    }
                }


                using (DataAccess.PollidutEntities db = new PollidutEntities())
                {
                    int personId = 0;
                    if (employee.personId > 0)
                    {
                        personId = employee.personId;
                        //var modify = (from p in db.PERSONS where p.PERSON_ID == employee.personId 
                        //              select new {ID=p.PERSON_ID,Name=p.FATHERS_NAME}).FirstOrDefault();

                        #region Person Modify
                        var modifyPerson = db.PERSONS.Where(p => p.PERSON_ID == employee.personId).FirstOrDefault();
                        if (modifyPerson != null)
                        {
                            modifyPerson.PERSON_NAME = employee.employeeName;
                            modifyPerson.FATHERS_NAME = employee.fatherName;
                            modifyPerson.MOTHERS_NAME = employee.motherName;
                            DateTime birthDate;
                            Boolean IsDateValid = DateTime.TryParseExact(employee.dateOfBirth, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out birthDate);

                            if (IsDateValid)
                            {
                                modifyPerson.DATE_OF_BIRTH = birthDate;
                            }
                            else
                            {
                                throw new ArgumentException("Birth date is invalid, it must be in dd-mm-yyyy format.");
                            }


                            modifyPerson.RELIGION_ID = (employee.religionId > 0 ? employee.religionId : 0);
                            modifyPerson.RELIGION_SPECIFY = employee.religionSpecify;
                            modifyPerson.NATIONALITY = employee.nationality;
                            modifyPerson.GENDER_ID = (employee.genderId > 0 ? employee.genderId : 0);
                            modifyPerson.MARITAL_STATUS_ID = (employee.maritalStatusId > 0 ? employee.maritalStatusId : 0);
                            modifyPerson.PERMANENT_ADDRESS = employee.permanentAddress;
                            modifyPerson.MAILING_ADDRESS = employee.mailingAddress;
                            modifyPerson.DISTRICT_ID = (employee.districtId > 0 ? employee.districtId : 0);
                            modifyPerson.THANA_ID = (employee.thanaId > 0 ? employee.thanaId : 0);
                            modifyPerson.MOBILE = employee.mobile;
                            modifyPerson.PHONE = employee.phone;
                            modifyPerson.EMAIL = employee.email;
                            modifyPerson.NID = employee.nid;
                            modifyPerson.PERSON_CURRENT_STATUS_ID = (employee.personCurrentStatusId > 0 ? employee.personCurrentStatusId : 0);
                            modifyPerson.ACTIVE = (employee.isActive == 1 ? true : false);
                            modifyPerson.DISTRIBUTION_HOUSE_ID = (employee.houseId > 0 ? employee.houseId : 0);
                            modifyPerson.PERSON_IMAGE = imageName;
                            //if(Session["UserId"]!=null)
                            //{
                            modifyPerson.UPDATE_BY = Convert.ToInt32(Session["UserId"].ToString());
                            //}
                            modifyPerson.UPDATE_DATE = DateTime.Now.Date;
                            db.SaveChanges();
                        }
                        #endregion

                        #region Employee Modify
                        var modifyEmployee = db.EMPLOYEES.Where(e => e.PERSON_ID == employee.personId).FirstOrDefault();
                        if (modifyEmployee != null)
                        {
                            modifyEmployee.EMPLOYEE_TYPE_ID = (employee.employeeTypeId > 0 ? employee.employeeTypeId : 0);
                            modifyEmployee.EMPLOYEE_ROLE_STATUS_ID = (employee.employeeRoleStatusId > 0 ? employee.employeeRoleStatusId : 0);
                            modifyEmployee.EMPLOYEE_CODE = employee.employeeCode.ToString();
                            modifyEmployee.DISTRIBUTION_HOUSE_ID = (employee.houseId > 0 ? employee.houseId : 0);
                            modifyEmployee.BRAND_ID = (employee.brandId > 0 ? employee.brandId : 0);
                            modifyEmployee.BANK_ID = (employee.bankId > 0 ? employee.bankId : 0);
                            modifyEmployee.BANK_BRANCH = employee.bankbranch;
                            modifyEmployee.BANK_ACCOUNT_NO = employee.accountno;
                            modifyEmployee.REMARKS = employee.remark;
                            modifyEmployee.ACTIVE = (employee.isActive == 1 ? true : false);
                            modifyEmployee.UPDATED_BY = Convert.ToInt32(Session["UserId"].ToString());
                            modifyEmployee.UPDATED_DATE = DateTime.Now.Date;
                            db.SaveChanges();
                        }
                        #endregion

                        #region Person Degrees
                        var deleteDegree = db.PERSON_DEGREES.Where(d => d.PERSON_ID == employee.personId & d.DEGREE_ID != 0).ToList();
                        foreach (DataAccess.PERSON_DEGREES degree in deleteDegree)
                        {
                            db.PERSON_DEGREES.Remove(degree);
                        }
                        if (employee.educations != null)
                        {
                            foreach (Education education in employee.educations)
                            {
                                //if (education.educationDegreeId != null)
                                //{
                                var insertDegree = new DataAccess.PERSON_DEGREES
                                {
                                    PERSON_ID = employee.personId,
                                    EDUCATIONAL_DEGREE_ID = (education.educationDegreeId > 0 ? education.educationDegreeId : 0),
                                    INSTITUTE = education.institute,
                                    PASSING_YEAR = education.passingYear,
                                    SUBJECT = education.subject,
                                    DURATION = education.duration,
                                    GRADE_ID = (education.gradeId > 0 ? education.gradeId : 0),
                                    CGPA = education.cgpa,
                                    DIVISION_ID = (education.divisionId > 0 ? education.divisionId : 0),
                                    MARK = education.mark,
                                    ACTIVE = true

                                };
                                db.PERSON_DEGREES.Add(insertDegree);
                                //}
                            }
                            db.SaveChanges();
                        }
                        #endregion

                        #region Person Experiences
                        var deleteExperiences = db.PERSON_JOB_EXPERIENCES.Where(je => je.PERSON_ID == employee.personId && je.EXPERIENCE_ID != 0).ToList();
                        foreach (DataAccess.PERSON_JOB_EXPERIENCES experience in deleteExperiences)
                        {
                            db.PERSON_JOB_EXPERIENCES.Remove(experience);
                        }
                        if (employee.experiences != null)
                        {
                            foreach (Experience experience in employee.experiences)
                            {
                                var insertExperience = new DataAccess.PERSON_JOB_EXPERIENCES
                                {
                                    PERSON_ID = employee.personId,
                                    EXPERIENCE_TYPE_ID = (experience.experienceTypeId > 0 ? experience.experienceTypeId : 0),
                                    ORGANIZATION = experience.organization,
                                    DESIGNATION = experience.designation,
                                    //START_DATE = startDate, //string.IsNullOrEmpty(experience.startDate) ? (DateTime?)null : DateTime.Parse(experience.startDate)
                                    //END_DATE = endDate,//DateTime.ParseExact(experience.endDate, "dd-mm-yyyy", null, System.Globalization.DateTimeStyles.None)

                                    DURATION = experience.expDuration,
                                    JOB_DESCRIPTION = experience.jobDescription,
                                    ACHIEVEMENT = experience.achievement
                                };

                                DateTime startDate;
                                Boolean IsStartDateValid = DateTime.TryParseExact(experience.startDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out startDate);

                                if (IsStartDateValid)
                                {
                                    insertExperience.START_DATE = startDate;
                                }
                                else
                                {
                                    throw new ArgumentException("Experience start date is invalid, it must be in dd-mm-yyyy format.");
                                }

                                DateTime endDate;
                                //DateTime? ddt;
                                Boolean IsEndDateValid = DateTime.TryParseExact(experience.endDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out endDate);

                                if (IsEndDateValid)
                                {
                                    insertExperience.END_DATE = endDate;
                                }
                                else
                                {
                                    insertExperience.END_DATE = nullDate;
                                }

                                db.PERSON_JOB_EXPERIENCES.Add(insertExperience);
                            }
                            db.SaveChanges();
                        }
                        #endregion

                        #region Person References
                        var deleteReferences = db.PERSONS_REFERENCES.Where(r => r.PERSON_ID == employee.personId && r.REFERENCE_ID != 0).ToList();
                        foreach (DataAccess.PERSONS_REFERENCES reference in deleteReferences)
                        {
                            db.PERSONS_REFERENCES.Remove(reference);
                        }
                        if (employee.references != null)
                        {
                            foreach (Reference reference in employee.references)
                            {
                                var insertReference = new DataAccess.PERSONS_REFERENCES
                                {
                                    PERSON_ID = employee.personId,
                                    REFERENCE_TYPE_ID = (reference.referenceTypeId > 0 ? reference.referenceTypeId : 0),
                                    REFERENCE_NAME = reference.refName,
                                    ORGANAIZATION = reference.refOrganization,
                                    DESIGNATION = reference.refDesignation,
                                    MOBILE = reference.refMobile,
                                    PHONE = reference.refPhone,
                                    EMAIL = reference.refEmail,
                                    MAILING_ADDRESS = reference.refAddress,
                                    RELATIONSHIP = reference.refRelation
                                };
                                db.PERSONS_REFERENCES.Add(insertReference);
                            }
                            db.SaveChanges();
                        }
                        #endregion

                    }
                    else
                    {

                    }

                    #region Emage Handle
                    if (personId > 0 && file != null)
                    {
                        var directory = HttpContext.Server.MapPath("~/Photos");
                        string imagePath = Path.Combine(directory, "EmployeePhotos", imageName);
                        FileStream fs = new FileStream(imagePath, FileMode.Create);

                        using (var binaryReader = new BinaryReader(file.InputStream))
                        {
                            imagefile = binaryReader.ReadBytes(file.ContentLength);//image
                        }
                        BinaryWriter bw = new BinaryWriter(fs);
                        bw.Write(imagefile);
                        bw.Close();

                        //var fileName = Path.GetFileName(file.FileName);
                        //var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                        //file.SaveAs(imagePath);
                    }
                    #endregion
                }

                #region comments
                //foreach (Education education in employee.educations)
                //{
                //    int personEducationId = education.personEducationId;
                //    int educationDegreeId = education.educationDegreeId;
                //    string institute = education.institute;
                //    string passingYear = education.passingYear;
                //    string subject = education.subject;
                //    string duration = education.duration;
                //    int gradeId = education.gradeId;
                //    string cgpa = education.cgpa;
                //    int divisionId = education.divisionId;
                //    string mark = education.mark;
                //}
                //foreach (Experience eperience in employee.experiences)
                //{
                //    int personExperienceId = eperience.personExperienceId;
                //    int experienceTypeId = eperience.experienceTypeId;
                //    string organization = eperience.organization;
                //    string designation = eperience.designation;
                //    DateTime startDate = eperience.startDate;
                //    DateTime endDate = eperience.endDate;
                //    string expDuration = eperience.expDuration;
                //    string jobDescription = eperience.jobDescription;
                //    int achievement = eperience.achievement;
                //}
                //foreach (Reference reference in employee.references)
                //{
                //    int personReferenceId = reference.personReferenceId;
                //    int referenceTypeId = reference.referenceTypeId;
                //    string refName = reference.refName;
                //    string refOrganization = reference.refOrganization;
                //    string refDesignation = reference.refDesignation;
                //    string refMobile = reference.refMobile;
                //    string refPhone = reference.refPhone;
                //    string refEmail = reference.refEmail;
                //    string refAddress = reference.refAddress;
                //    string refRelation = reference.refRelation;
                //}
                #endregion
                return Json("success", JsonRequestBehavior.AllowGet);

            }

            catch (DbEntityValidationException e)
            {
                var errorPath = HttpContext.Server.MapPath("~/App_Data");
                ExceptionLogger.CreateLog(e, errorPath);
                return Json(e.Message, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                var errorPath = HttpContext.Server.MapPath("~/App_Data");
                ExceptionLogger.CreateLog(Ex, errorPath);
                return Json(Ex.Message, JsonRequestBehavior.AllowGet);
            }

        }
        public class Employee
        {
            public int personId { get; set; }
            public string employeeName { get; set; }
            public string fatherName { get; set; }
            public string mobile { get; set; }
            public string motherName { get; set; }
            public string mailingAddress { get; set; }
            public string permanentAddress { get; set; }
            public string dateOfBirth { get; set; }
            public int genderId { get; set; }
            public int religionId { get; set; }
            public string religionSpecify { get; set; }
            public string nationality { get; set; }
            public int maritalStatusId { get; set; }
            public int districtId { get; set; }
            public int thanaId { get; set; }
            public string phone { get; set; }
            public string email { get; set; }
            public int bankId { get; set; }
            public string bankbranch { get; set; }
            public string accountno { get; set; }
            public string remark { get; set; }
            public string nid { get; set; }
            public string imageName { get; set; }

            public int personCurrentStatusId { get; set; }
            public int employeeTypeId { get; set; }
            public int employeeRoleStatusId { get; set; }
            public int employeeCode { get; set; }
            public int houseId { get; set; }
            public int brandId { get; set; }
            public int isActive { get; set; }

            public List<Education> educations { get; set; }
            public List<Experience> experiences { get; set; }
            public List<Reference> references { get; set; }
        }

        public class Education
        {
            public int personEducationId { get; set; }
            public int educationDegreeId { get; set; }
            public string institute { get; set; }
            public string passingYear { get; set; }
            public string subject { get; set; }
            public string duration { get; set; }
            public int gradeId { get; set; }
            public decimal cgpa { get; set; }
            public int divisionId { get; set; }
            public int mark { get; set; }
        }
        public class Experience
        {
            public int personExperienceId { get; set; }
            public int experienceTypeId { get; set; }
            public string organization { get; set; }
            public string designation { get; set; }
            public string startDate { get; set; }
            public string endDate { get; set; }
            public string expDuration { get; set; }
            public string jobDescription { get; set; }
            public string achievement { get; set; }
        }
        public class Reference
        {
            public int personReferenceId { get; set; }
            public int referenceTypeId { get; set; }
            public string refName { get; set; }
            public string refOrganization { get; set; }
            public string refDesignation { get; set; }
            public string refMobile { get; set; }
            public string refPhone { get; set; }
            public string refEmail { get; set; }
            public string refAddress { get; set; }
            public string refRelation { get; set; }
        }

        public string validateEducations(List<Education> educations)
        {
            if (educations == null)
            {
                return "";// "Educations not found!";
            }
            else
            {
                foreach (Education education in educations)
                {
                    if (education.educationDegreeId == null)
                    {
                        return "Education degree not found!";
                    }
                    if (education.institute == "" || education.institute == null)
                    {
                        return "Educational institute not found!";
                    }
                }
                return "";
            }
        }

        private string validateExperience(List<Experience> experiences)
        {
            if (experiences == null)
            {
                return "";// "Experiences not found!";
            }
            else
            {
                foreach (Experience experience in experiences)
                {
                    if (experience.experienceTypeId == null)
                    {
                        return "Experience type not found!";
                    }
                    if (experience.organization == "" || experience.organization == null)
                    {
                        return "Experience organization not found!";
                    }
                }
                return "";
            }
        }
        private string validateReference(List<Reference> references)
        {
            if (references == null)
            {
                return "";// "References not found!";
            }
            else
            {
                foreach (Reference reference in references)
                {
                    if (reference.referenceTypeId == null)
                    {
                        return "Reference type not found!";
                    }
                    if (reference.refName == "" || reference.refName == null)
                    {
                        return "Reference name not found!";
                    }
                }
                return "";
            }
        }

        //public JsonResult addEducation()
        //{
        //    return Json(
        //}

        public ActionResult getThanas(int districtId)
        {
            List<Pollidut.Models.Thana> thanas = Pollidut.Models.ThanaManager.GetThanas(districtId);
            ViewBag.Thanas = thanas;

            return Json(thanas, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AssignEmployeeToParent()
        {
            try
            {
                #region UserInfo
                //String LoginName = Session["LoginName"].ToString();
                //String UserId = Session["UserId"].ToString();
                //String FullName = Session["FullName"].ToString();
                //String UserTypeId = Session["UserTypeId"].ToString();
                //String EmployeeId = Session["EmployeeId"].ToString();

                //ViewBag.LoginName = LoginName;
                //ViewBag.UserId = UserId;
                //ViewBag.FullName = FullName;

                //String DashboardTitle = String.Empty;
                //String UserTypeName = String.Empty;

                //switch (UserTypeId)
                //{
                //    case "1": UserTypeName = "webadmin"; DashboardTitle = "Web Admin"; break;
                //    case "2": UserTypeName = "superadmin"; DashboardTitle = "Super Admin"; break;
                //    case "3": UserTypeName = "ac"; DashboardTitle = "AC"; break; //for CE project
                //    case "4": UserTypeName = "brs"; DashboardTitle = "BRS"; break; //for CE project 
                //    case "5": UserTypeName = "br"; DashboardTitle = "BR"; break; //for CE project
                //    case "6": UserTypeName = "cms"; DashboardTitle = "CMS"; break; //CM project, CM Supervisor or Merchandizing Supervisor or MS
                //    case "7": UserTypeName = "mc"; DashboardTitle = "MC"; break; //CM project
                //    case "8": UserTypeName = "ceadmin"; DashboardTitle = "CE Admin"; break;
                //    case "9": UserTypeName = "cmadmin"; DashboardTitle = "CM Admin"; break;
                //    case "10": UserTypeName = "cm"; DashboardTitle = "CM"; break;
                //    case "11": UserTypeName = "bat"; DashboardTitle = "BATB Admin"; break;

                //    default:
                //        break;
                //}

                //ViewBag.DashboardTitle = DashboardTitle; ViewBag.UserTypeName = UserTypeName;

                //Session["UserTypeName"] = UserTypeName;
                #endregion
                using (Pollidut.DataAccess.PollidutEntities db = new PollidutEntities())
                {
                    List<Pollidut.Models.Area> areas = Pollidut.Models.AreaManager.GetComboList();
                    ViewBag.Areas = areas;
                    //List<BATB.Models.DistributionHouse> distributionHouses = BATB.Models.DistributionHouseManager.GetDistributionHouses();
                    //ViewBag.Houses = distributionHouses;
                }

                return View();
            }
            catch (Exception e)
            {
                var errorPath = HttpContext.Server.MapPath("~/App_Data");
                ExceptionLogger.CreateLog(e, errorPath);
                return Json("Error:" + e.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AssignEmployeeToParent(int ParentEmployeeId, int EmployeeId, string ActionMode)
        {
            try
            {
                using (Pollidut.DataAccess.PollidutEntities db = new PollidutEntities())
                {
                    if (ActionMode == "add")
                    {
                        var assignParentEmployee = db.EMPLOYEES.Where(e => e.EMPLOYEE_ID == EmployeeId).FirstOrDefault();
                        if (assignParentEmployee != null)
                        {
                            assignParentEmployee.SUPERVISOR_ID = ParentEmployeeId;
                        }
                    }
                    db.SaveChanges();
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                var errorPath = HttpContext.Server.MapPath("~/App_Data");
                ExceptionLogger.CreateLog(e, errorPath);
                return Json("Action failed!", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult getEmployeeTypes()
        {
            List<Pollidut.Models.EmployeeType> employeeTypes = Pollidut.Models.EmployeeTypeManager.GetEmployeeTypes();
            ViewBag.EmployeeTypes = employeeTypes;
            return Json("employees", JsonRequestBehavior.AllowGet);
        }

        public ActionResult getHouseList(int areaId)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            int UserTypeId = Convert.ToInt32(Session["UserTypeId"]);
            int EmployeeId = Convert.ToInt32(Session["EmployeeId"]);
            using (Pollidut.DataAccess.PollidutEntities db = new PollidutEntities())
            {
                List<Pollidut.Models.DistributionHouse> houses = Pollidut.Models.DistributionHouseManager.GetAreaSpecificDistHouses(areaId);
                ViewBag.Houses = houses;
                return Json(houses, JsonRequestBehavior.AllowGet);

                //List<BATB.Models.Thana> thanas = BATB.Models.ThanaManager.GetThanas(districtId);
                //ViewBag.Thanas = thanas;

                //return Json(thanas, JsonRequestBehavior.AllowGet);
            }
        }
        //public ActionResult getHouses(int areaId)
        //{
        //    List<BATB.Models.DistributionHouse> thanas = BATB.Models.DistributionHouseManager.GetAreaSpecificDistHouses(areaId);
        //    ViewBag.Thanas = thanas;

        //    return Json(thanas, JsonRequestBehavior.AllowGet);
        //}
        public List<Pollidut.DataAccess.AREA> getAreas()
        {
            using (Pollidut.DataAccess.PollidutEntities db = new PollidutEntities())
            {
                return db.AREAS.ToList();
            }
        }

        public ActionResult getEmployeeListForAssignToParent(int houseId)
        {
            try
            {
                using (var context = new PollidutEntities())
                {
                    var UserTypeId = Convert.ToInt32(Session["UserTypeId"]);

                    var projectId = context.USER_TYPES.Where(et => et.USER_TYPE_ID == UserTypeId).FirstOrDefault().PROJECT_ID;
                    List<View_EmployeeListForAssignToParent> data = null;
                    if (projectId > 0)
                    {
                        data = new PollidutEntities().View_EmployeeListForAssignToParent.Where(e => e.DistributionHouseId == houseId && e.PROJECT_ID == projectId).ToList();
                    }
                    else if (projectId == 0)
                    {
                        data = new PollidutEntities().View_EmployeeListForAssignToParent.Where(e => e.DistributionHouseId == houseId).ToList();
                    }
                    String SerializedData = JsonConvert.SerializeObject(data, Formatting.None);
                    return Json(SerializedData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                var errorPath = HttpContext.Server.MapPath("~/App_Data");
                ExceptionLogger.CreateLog(e, errorPath);
                return Json("Error:" + e.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public string getProjectEmployeeTypes(int userId)
        {
            using (Pollidut.DataAccess.PollidutEntities db = new PollidutEntities())
            {
                var employeeId = db.USERS.Where(u => u.USER_ID == userId).FirstOrDefault().EMPLOYEE_ID;
                var employeeTypeId = db.EMPLOYEES.Where(e => e.EMPLOYEE_ID == employeeId).FirstOrDefault().EMPLOYEE_TYPE_ID;
                int UserTypeId = Convert.ToInt32(Session["UserTypeId"]);
                var projectId = db.EMPLOYEE_TYPES.Where(et => et.EMPLOYEE_TYPE_ID == employeeTypeId).FirstOrDefault().PROJECT_ID;

                var projectEmplyeeTypes = db.EMPLOYEE_TYPES.ToList();
                //switch (projectId)
                //{
                //    case 1: //ce


                //    default:
                //        break;
                //}



                if (projectId < 1)
                {
                    projectEmplyeeTypes = db.EMPLOYEE_TYPES.ToList();
                }
                else
                {
                    projectEmplyeeTypes = db.EMPLOYEE_TYPES.Where(et => et.PROJECT_ID == projectId).ToList();
                }

                string employeeTypeIds = "";
                if (projectEmplyeeTypes != null)
                {
                    foreach (Pollidut.DataAccess.EMPLOYEE_TYPES employeeType in projectEmplyeeTypes)
                    {
                        if (string.IsNullOrEmpty(employeeTypeIds))
                        {
                            employeeTypeIds = employeeType.EMPLOYEE_TYPE_ID.ToString();
                        }
                        else
                        {
                            employeeTypeIds += "," + employeeType.EMPLOYEE_TYPE_ID.ToString();
                        }
                    }
                }
                return employeeTypeIds;
            }
        }
    }
}
