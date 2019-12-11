using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DRD.App.Controllers
{
    public class ErrorController : Controller
    {
        LoginController login = new LoginController();
        UserSession user;
        Layout layout = new Layout();

        public void Initialize()
        {
            user = login.GetUser(this);
            login.CheckLogin(this);
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = login.GetUser(this);
        }
        public ActionResult Error()
        {
            Initialize();
            ErrorInfo errorInfo = new ErrorInfo();
            errorInfo.Message = "An Error Has Occured";
            errorInfo.Description = "An unexpected error occured on our website. The website administrator has been notified.";
            layout.errorInfo = errorInfo;

            return View("Error", layout);
        }
        public ActionResult BadRequest()
        {
            Initialize();
            ErrorInfo errorInfo = new ErrorInfo();
            errorInfo.Message = "Bad Request";
            errorInfo.Description = "The request cannot be fulfilled due to bad syntax.";
            layout.errorInfo = errorInfo;

            return View("Error", layout);
        }
        public ActionResult NotFound()
        {
            Initialize();
            ErrorInfo errorInfo = new ErrorInfo();
            errorInfo.Message = "404 We are sorry, the page you requested cannot be found.";
            errorInfo.Description = "The URL may be misspelled or the page you're looking for is no longer available.";
            layout.errorInfo = errorInfo;

            return View("Error", layout);
        }

        public ActionResult Forbidden()
        {
            Initialize();
            ErrorInfo errorInfo = new ErrorInfo();
            errorInfo.Message = "403 Forbidden";
            errorInfo.Description = "Forbidden: You don't have permission to access [directory] on this server.";
            layout.errorInfo = errorInfo;

            return View("Error", layout);
        }
        public ActionResult URLTooLong()
        {
            Initialize();
            ErrorInfo errorInfo = new ErrorInfo();
            errorInfo.Message = "URL Too Long";
            errorInfo.Description = "The requested URL is too large to process. That’s all we know.";
            layout.errorInfo = errorInfo;

            return View("Error", layout);
        }
        public ActionResult ServiceUnavailable()
        {
            Initialize();
            ErrorInfo errorInfo = new ErrorInfo();
            errorInfo.Message = "Service Unavailable";
            errorInfo.Description = "Our apologies for the temporary inconvenience. This is due to overloading or maintenance of the server.";
            layout.errorInfo = errorInfo;

            return View("Error", layout);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}