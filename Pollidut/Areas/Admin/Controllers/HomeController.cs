using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.OleDb;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Collections;
using Newtonsoft.Json;
using System.Configuration;
using sCommonLib;
using System.Web.UI;
using System.IO;
using System.Web.UI.WebControls;
using System.Reflection;
using System.ComponentModel;

namespace Pollidut.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /RobiAdmin/Home/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SrAtField()
        {
            return View("sratfield");
        }


        public ActionResult AdvanceSearch() 
        {
            return View("advanceSearch");
        }
	}
}