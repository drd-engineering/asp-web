using System.Collections.Generic;
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

            CompanyService companyService = new CompanyService();
            CompanyList companyList = companyService.GetAllCompanyOwnedbyUser(user.Id);
            if (companyList.companies.Count > 0 ) return View(layout);
            return RedirectToAction("List", "Inbox");
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

        // untuk tampilan front dari dashboard.
        public ActionResult GetDashboardRotationStatus(long companyId, ICollection<string> Tags, int page, int pageSize)
        {
            Initialize();
            int skip = (page - 1) * pageSize;
            var data = dashboardService.GetDashboardRotationStatus(companyId, user.Id, Tags, skip, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult sendEmailNotifikasiRotasi(long rotationId, long userId)
        {
            Initialize();
            var data = dashboardService.sendEmailNotifikasiRotasi(rotationId, userId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}