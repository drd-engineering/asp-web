using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Core;

using DRD.Domain;

namespace DRD.Web.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            JsonLayout layout = new JsonLayout();
            layout.menus = login.GetMenus(this, 0);
            layout.user = login.GetUser(this);
            layout.dbmenus = login.GetDashbordMenus(this, 0);
            layout.activeId = 0;
            return View(layout);
        }

        public ActionResult GetActivityCounter()
        {
            LoginController login = new LoginController();
            JsonCounter counter = (JsonCounter)Session["_COUNTER_"];
            if (counter == null)
                counter = new JsonCounter();

            var user = login.GetUser(this);
            if (user == null)
                return Json(null, JsonRequestBehavior.AllowGet);

            var srv = new DashboardService();
            var data = srv.GetActivityCounter(user.Id, counter);
            Session["_COUNTER_"] = data;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public int SendEmail()
        {
            EmailTools tools = new EmailTools();
            string body = tools.CreateHtmlBody(Server.MapPath("~/doc/emailtemplate/Registration.html"));

            EmailTools emailtools = new EmailTools();
            ApplConfigService acsvr = new ApplConfigService();
            var emailfrom = acsvr.GetValue("EMAILUSER"); 
            var emailfromdisplay = acsvr.GetValue("EMAILUSERDISPLAY");

            var task = emailtools.Send(emailfrom, emailfromdisplay + " Administrator", "xbudi@yahoo.com", "DRD Member Reset Password", body, false, new string[] { });

            return 1;
        }
    }
}