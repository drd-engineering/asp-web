using DRD.Models.Custom;
using DRD.Models.View;

using System.Web.Mvc;

namespace DRD.App.Controllers
{
    public class ErrorController : Controller
    {

        public ActionResult Error()
        {

            ErrorInfo errorInfo = new ErrorInfo();
            errorInfo.Message = "An Error Has Occured";
            errorInfo.Description = "An unexpected error occured on our website. The website administrator has been notified.";

            return RedirectToAction("Index", "Login");
        }
        public ActionResult BadRequest()
        {

            ErrorInfo errorInfo = new ErrorInfo();
            errorInfo.Message = "Bad Request";
            errorInfo.Description = "The request cannot be fulfilled due to bad syntax.";

            return RedirectToAction("Index", "Login");
        }
        public ActionResult NotFound()
        {

            ErrorInfo errorInfo = new ErrorInfo();
            errorInfo.Message = "404 We are sorry, the page you requested cannot be found.";
            errorInfo.Description = "The URL may be misspelled or the page you're looking for is no longer available.";
            return RedirectToAction("Index", "Login");
        }

        public ActionResult Forbidden()
        {

            ErrorInfo errorInfo = new ErrorInfo();
            errorInfo.Message = "403 Forbidden";
            errorInfo.Description = "Forbidden: You don't have permission to access [directory] on this server.";

            return RedirectToAction("Index", "Login");
        }
        public ActionResult URLTooLong()
        {

            ErrorInfo errorInfo = new ErrorInfo();
            errorInfo.Message = "URL Too Long";
            errorInfo.Description = "The requested URL is too large to process. That’s all we know.";
            return RedirectToAction("Index", "Login");
        }
        public ActionResult ServiceUnavailable()
        {

            ErrorInfo errorInfo = new ErrorInfo();
            errorInfo.Message = "Service Unavailable";
            errorInfo.Description = "Our apologies for the temporary inconvenience. This is due to overloading or maintenance of the server.";

            return RedirectToAction("Index", "Login");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}