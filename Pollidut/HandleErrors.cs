
using System;
using System.Web;
using System.Web.Mvc;

using System.Reflection;
using sCommonLib;

namespace Pollidut
{
    public class HandleErrors:System.Web.Mvc.HandleErrorAttribute
    {
        public override void OnException(System.Web.Mvc.ExceptionContext filterContext)
        {
            // if the request is AJAX return JSON else view.
            //if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            //{
            //    filterContext.Result = new JsonResult
            //    {
            //        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
            //        Data = new
            //        {
            //            error = true,
            //            message = filterContext.Exception.Message
            //        }
            //    };
            //}


            //If the exeption is already handled we do nothing
            if (filterContext.ExceptionHandled)
            {
                return;
            }
            else
            {
                //get or post?
                var requestType = System.Web.HttpContext.Current.Request.RequestType;

                var ErrorLogPath = HttpContext.Current.Server.MapPath("~/App_Data");
               

                var controllerName = (string)filterContext.RouteData.Values["controller"]; 
                //var actionName = (string)filterContext.RouteData.Values["action"];
                //var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);

                //Determine the return type of the action
                string actionName = filterContext.RouteData.Values["action"].ToString();
                Type controller = filterContext.Controller.GetType();

                //System.Web.Mvc.ExceptionContext.Controller.GetType().GetMethod(actionName);
                MethodInfo method = controller.GetMethod(actionName); //OK Code

                string ErrorMessage = "Created on: " + DateTime.Now.ToString("dd-MMM-yyyy, hh.mm.ss tt");
                ErrorMessage += Environment.NewLine + "Error Description: " + filterContext.Exception.Message;
                ErrorMessage += Environment.NewLine + "Source File: " + controllerName;
                ErrorMessage += Environment.NewLine + "Method: " + actionName;
              //  ErrorMessage += Environment.NewLine + "Line: " + line;
               // ErrorMessage += Environment.NewLine + "Column: " + col;

                ErrorMessage += Environment.NewLine + "________________________________________________________________________" + Environment.NewLine;
                StreamFunctions.CreateExceptionLog(ErrorMessage, ErrorLogPath);
                 

                //Object pattributes = method.GetCustomAttribute(typeof(HttpPostAttribute), true);
              


                var returnType = method.ReturnType;

                //If the action that generated the exception returns JSON
                if (returnType.Equals(typeof(JsonResult)))
                {
                    filterContext.Result = new JsonResult()
                    {
                        JsonRequestBehavior=JsonRequestBehavior.AllowGet,
                        Data = filterContext.Exception.Message
                    };

                    //filterContext.Result = new JsonResult
                    //{
                    //    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    //    Data = new
                    //    {
                    //        error = true,
                    //        message = filterContext.Exception.Message
                    //    }
                    //};
                }


                //If the action that generated the exception returns a view
                //Thank you Sumesh for the comment
                if (returnType.Equals(typeof(ActionResult)) || (returnType).IsSubclassOf(typeof(ActionResult)))
                {
                   
                    if ( filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.Result = new JsonResult()
                        {
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                            Data = filterContext.Exception.Message
                        };
                    }
                    else
                    {
                        filterContext.Result = new ViewResult
                        {
                            ViewName = "URL to the errror page"
                        };
                    }

                }
            }

            //Make sure that we mark the exception as handled
            filterContext.ExceptionHandled = true;
        }
    }
}


