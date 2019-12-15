using System.Web.Mvc;
using DRD.Service;
using DRD.Models.View;
using DRD.Models.API;

namespace DRD.App.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            Layout layout = new Layout();
            layout.menus = login.GetMenus(this, 0);
            layout.user = login.GetUser(this);
            layout.activeId = 0;
            return View(layout);
        }

        public ActionResult GetRotationFromAllCompany()
        {
            LoginController login = new LoginController();
            CounterItem counter = (CounterItem)Session["_COUNTER_"];
            if (counter == null)
                counter = new CounterItem();

            var user = login.GetUser(this);
            if (user == null)
                return Json(null, JsonRequestBehavior.AllowGet);

            var dashboardService = new DashboardService();
            var data = dashboardService.GetActivityCounter(user.Id, counter);
            Session["_COUNTER_"] = data;
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetActivityCounter()
        {
            LoginController login = new LoginController();
            CounterItem counter = (CounterItem)Session["_COUNTER_"];
            if (counter == null)
                counter = new CounterItem();

            var user = login.GetUser(this);
            if (user == null)
                return Json(null, JsonRequestBehavior.AllowGet);

            var dashboardService = new DashboardService();
            var data = dashboardService.GetActivityCounter(user.Id, counter);
            Session["_COUNTER_"] = data;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetActivityCounter(long CompanyId)
        {
            LoginController login = new LoginController();
            CounterItem counter = (CounterItem)Session["_COUNTER_"];
            if (counter == null)
                counter = new CounterItem();

            var user = login.GetUser(this);
            if (user == null)
                return Json(null, JsonRequestBehavior.AllowGet);

            var dashboardService = new DashboardService();
            var data = dashboardService.GetActivityCounter(user.Id, counter, CompanyId);
            Session["_COUNTER_"] = data;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}