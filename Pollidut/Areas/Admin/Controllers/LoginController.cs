using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Pollidut.DataAccess;

namespace Pollidut.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View();
        }

        //Submits login info
        [HttpPost]
        public JsonResult Index(FormCollection data)
        {
            String LoginName = data["loginname"];
            String LoginPassword = data["password"];
            using (PollidutEntities db = new PollidutEntities())
            {
                var user = (from a in db.USERS where a.LOGIN_NAME == LoginName select a).FirstOrDefault();
                if (user != null)
                {
                    if (user.PASSWORD.Equals(LoginPassword))
                    {
                        Session["UserName"] = user.USER_NAME;
                        return Json(new { result = "Redirect", url = "/Admin/Home?user=" + LoginName + "" });
                    }
                    else
                    {
                        return Json(new { result = "InvalidPassword" });
                    }
                }
                else
                {
                    return Json(new { result = "InvalidLoginName" });
                }
            }
        }
    }
}