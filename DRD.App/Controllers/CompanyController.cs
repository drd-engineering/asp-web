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
        private CompanyService companyService = new CompanyService();
        private MemberService memberService = new MemberService();
        private SubscriptionService subscriptionService = new SubscriptionService();
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
            return View(layout);
        }

        public ActionResult GetAllCompanyOwnedbyUser()
        {
            Initialize();
            var data = companyService.GetAllCompanyOwnedbyUser(user.Id);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Member(long id)
        {
            Initialize();
            var company = companyService.GetCompany(id);
            if(company == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            
            if(!companyService.CheckIsOwner(user.Id, id) && !memberService.checkIsAdmin(user.Id, id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            ViewBag.Company = company.Name;
            return View(layout);
        }

        public ActionResult AddMembers(long companyId, string emails)
        {
            InitializeAPI();
            var data = companyService.AddMembers(companyId, user.Id, emails);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: Contact/GetPersonalContact?searhKey=[]&page=1&size=20
        public ActionResult GetCompanyList()
        {
            InitializeAPI();
            CompanyList data = companyService.GetAllCompanyDetails(user.Id);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSubscriptionList()
        {
            InitializeAPI();
            List<BusinessPackage> data = subscriptionService.GetAllPublicSubscription();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AcceptMember(long memberId)
        {
            InitializeAPI();
            var data = companyService.AcceptMember(memberId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RejectMember(long memberId)
        {
            InitializeAPI();
            var data = companyService.RejectMember(memberId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}