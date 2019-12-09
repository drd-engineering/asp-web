using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DRD.Models;
using DRD.Models.API;
using DRD.Models.API.Register;
using DRD.Models.Custom;
using DRD.Models.API;

using DRD.Service;

namespace DRD.App.Controllers
{
    public class CompanyController : Controller
    {
        // GET: Company
        public ActionResult Index()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            UserSession user = login.GetUser(this);

            Layout layout = new Layout();
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = login.GetUser(this);

            CompanyService companyService = new CompanyService();
            CompanyList companyList = companyService.GetAllCompanyDetails(user.Id);

            ViewBag.companies = companyList;

            return View(layout);
        }
        public ActionResult Member(long id)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            UserSession user = login.GetUser(this);

            Layout layout = new Layout();
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = login.GetUser(this);

            return View(layout);
        }
        // GET: Contact/GetPersonalContact?searhKey=[]&page=1&size=20
        public ActionResult GetCompanyList()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);
            UserSession user = login.GetUser(this);

            CompanyService companyService = new CompanyService();
            CompanyList data = companyService.GetAllCompanyDetails(user.Id);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSubscriptionList()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);
            UserSession user = login.GetUser(this);

            SubscriptionService subscriptionService = new SubscriptionService();
            List<BusinessSubscription> data = subscriptionService.getSubscription();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}