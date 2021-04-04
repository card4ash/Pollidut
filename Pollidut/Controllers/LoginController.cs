using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Pollidut.DataAccess;
using System.IO;

namespace Pollidut.Controllers
{
    public class LoginController : Controller
    {

        //Entering into the application
        public ActionResult Index()
        {
            String Mode = Request.QueryString["mode"]; //login or logout
            String IsMobile = Request.QueryString["m"];

            if (Request.Browser.IsMobileDevice)
            {
                IsMobile = "y";
            }

            if (Mode != null) //user is logging out
            {
                ViewBag.WelcomeText = "You are successfully logout";
            }
            else
            {
                ViewBag.WelcomeText = "Login"; //user is logging in
            }

            if (IsMobile != null) //something is there
            {

                if (IsMobile == "y")
                {
                    return View("Login.mobile");
                }
                else
                {
                    return View("Login.desktop");
                }

            }
            else //comes from desktop without any doubt.
            {
                return View("Login.desktop");
            }

        }

        //Submits login info
        [HttpPost]
        public JsonResult Index(FormCollection data)
        {
            String LoginName = data["username"];
            String LoginPassword = data["password"];
            String IsMobile=data["ismobile"]; //if comes from mobile value=y, otherwise value=n 
            Decimal Lat = Convert.ToDecimal( data["lat"]);
            Decimal Lon= Convert.ToDecimal(data["lon"]);
            

            Byte[] imagefile;
            String imageName = String.Empty;

            if (Request.Files["files"] != null)
            {
                imageName = Guid.NewGuid().ToString() + ".png";
            }

            using (var context = new PollidutEntities())
            {

                var login = context.View_USERS.Where(m => m.LOGIN_NAME == LoginName && m.PASSWORD == LoginPassword).FirstOrDefault();

                if (login == null)
                {
                    return Json(new { result = "InvalidLogin" }, JsonRequestBehavior.AllowGet);
                }

                Int32 UserId = login.USER_ID;
                String FullName = login.USER_NAME;
                Int32 UserTypeId = login.USER_TYPE_ID;
                Int32 EmployeeId = Convert.ToInt32(login.EMPLOYEE_ID);



                try
                {
                    context.USERS_LOGIN_INFO.Add(new USERS_LOGIN_INFO() { USER_ID = UserId, EMPLOYEE_ID=EmployeeId, IMAGE_ID = imageName, LOGIN_DATE = DateTime.Now,LAT=Lat,LON=Lon });
                    context.SaveChanges();
                }
                catch (Exception Ex)
                {
                    return Json("Error", JsonRequestBehavior.AllowGet);
                }

                if (Request.Files["files"] != null)
                {
                    var directory = HttpContext.Server.MapPath("~/Photos");
                    string imagePath = Path.Combine(directory, "LoginPhotos", imageName);
                    FileStream fs = new FileStream(imagePath, FileMode.CreateNew);

                    using (var binaryReader = new BinaryReader(Request.Files["files"].InputStream))
                    {
                        imagefile = binaryReader.ReadBytes(Request.Files["files"].ContentLength);//image
                    }

                    BinaryWriter bw = new BinaryWriter(fs);
                    bw.Write(imagefile);
                    bw.Close();
                }

                Session.Timeout = 24000;
                Session["LoginName"] = LoginName.ToString();
                Session["UserId"] = UserId.ToString();
                Session["FullName"] = FullName.ToString();
                Session["UserTypeId"] = UserTypeId.ToString();
                Session["EmployeeId"] = EmployeeId.ToString();

               

              //  return Json(new { result = "Redirect", url = Url.Action("Dashboard", "Dashboard", new { user = LoginName, supervisorname = FullName }) });

                if (UserTypeId==5) //if BR
                {
                    return Json(new { result = "Redirect", url = Url.Action("Index", "BR/BasicPosition", new { user = LoginName, name = FullName }) });
                }
                else
                {
                    switch (UserTypeId)
                    {
                        case 4:  // BR Supervisor (BRS)
                        case 6:  //user type 'CMS' (CM Supervisor)
                        case 12: //SR
                            IsMobile = "y"; //force to use mobile interface
                            break;
                        case  13: //photo collector
                             IsMobile = "n"; //force to use desktop interface
                            break;
                        default:
                            IsMobile = "n"; //desktop by default
                            break;
                    }

                    return Json(new { result = "Redirect", url = "/Dashboard?m=" + IsMobile + "&user=" + LoginName + "" });
                }
            }

           
        }

        public ActionResult MobileLogin()
        {
            return RedirectToAction("Index", "Login", new { m = "y" });
        }

    }
}
