using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Mvc;

using DRD.Models;
using DRD.Models.API;
using DRD.Models.Custom;
using DRD.Models.View;

using DRD.Service;

namespace DRD.App.Controllers
{
    public class CompanyController : Controller
    {
        private LoginController login = new LoginController();
        private UserSession user;
        private Layout layout = new Layout();

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

        public ActionResult Index()
        {
            Initialize();
            CompanyService companyService = new CompanyService();

            return View(layout);
        }
        public ActionResult Member(long id)
        {
            Initialize();

            CompanyService companyService = new CompanyService();
            MemberService memberService = new MemberService();

            var company = companyService.GetCompany(id);
            
            if(company == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            
            if(!companyService.checkIsOwner(user.Id, id) && !memberService.checkIsAdmin(user.Id, id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            ViewBag.Company = company.Name;
            return View(layout);
        }
        // GET: Contact/GetPersonalContact?searhKey=[]&page=1&size=20
        public ActionResult GetCompanyList()
        {
            InitializeAPI();

            CompanyService companyService = new CompanyService();
            CompanyList data = companyService.GetAllCompanyDetails(user.Id);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSubscriptionList()
        {
            InitializeAPI();

            SubscriptionService subscriptionService = new SubscriptionService();
            List<BusinessSubscription> data = subscriptionService.getSubscription();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}