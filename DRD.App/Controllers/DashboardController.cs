using System.Web.Mvc;
using DRD.Service;
using DRD.Models.View;
using DRD.Models.API;
using DRD.Models.Custom;

namespace DRD.App.Controllers
{
    public class DashboardController : Controller
    {
        LoginController login = new LoginController();
        DashboardService dashboardService = new DashboardService();
        CompanyService companyService = new CompanyService();
        UserSession user;
        Layout layout = new Layout();

        public void Initialize()
        {
            user = login.GetUser(this);
            login.CheckLogin(this);
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = login.GetUser(this);
        }
        public void InitializeAPI()
        {
            user = login.GetUser(this);
            login.CheckLogin(this);
        }

        // GET: Dashboard
        public ActionResult Index()
        {
            Initialize();
            Layout layout = new Layout();
            layout.menus = login.GetMenus(this, 0);
            layout.user = user;
            layout.activeId = 0;
            return View(layout);
        }

        public ActionResult GetRotationFromAllCompany()
        {
            Initialize();
            CounterItem counter = (CounterItem)Session["_COUNTER_"];
            if (counter == null)
                counter = new CounterItem();

            if (user == null)
                return Json(null, JsonRequestBehavior.AllowGet);

            var data = dashboardService.GetActivityCounter(user.Id, counter);
            Session["_COUNTER_"] = data;
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetActivityCounter()
        {
            Initialize();
            CounterItem counter = (CounterItem)Session["_COUNTER_"];
            if (counter == null)
                counter = new CounterItem();

            if (user == null)
                return Json(null, JsonRequestBehavior.AllowGet);

            var data = dashboardService.GetActivityCounter(user.Id, counter);
            Session["_COUNTER_"] = data;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetActivityCounterCompany(long CompanyId)
        {
            Initialize();
            CounterItem counter = (CounterItem)Session["_COUNTER_"];
            if (counter == null)
                counter = new CounterItem();

            if (user == null)
                return Json(null, JsonRequestBehavior.AllowGet);

            var data = dashboardService.GetActivityCounter(user.Id, counter, CompanyId);
            Session["_COUNTER_"] = data;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetListCompanyOwned()
        {
            Initialize();
            var data = companyService.getCompanyListByOwnerId(user.Id);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // public ActionResult Sendemail()
        // {
        //     Initialize();
        //     var data = dashboardService.SendEmail("test");
        //     return Json(data, JsonRequestBehavior.AllowGet);
        // }
    }
}