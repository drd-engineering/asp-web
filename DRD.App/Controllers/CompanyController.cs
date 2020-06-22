using DRD.Models;
using DRD.Models.API;
using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace DRD.App.Controllers
{
    public class CompanyController : Controller
    {
        private LoginController login = new LoginController();
        private UserSession user;
        private CompanyService companyService = new CompanyService();
        private MemberService memberService = new MemberService();
        private UserService userService = new UserService();
        private SubscriptionService subscriptionService = new SubscriptionService();
        private Layout layout = new Layout();

        public bool Initialize()
        {
            if (login.CheckLogin(this))
            {
                user = login.GetUser(this);
                layout.menus = login.GetMenus(this, layout.activeId);
                layout.user = login.GetUser(this);
                return true;
            }
            return false;
        }
        public void InitializeAPI()
        {
            user = login.GetUser(this);
            login.CheckLogin(this);
        }

        public ActionResult Index()
        {
            if (!Initialize())
                return RedirectToAction("Index", "LoginController");
            var isAdminandHasCompany = userService.IsAdminOrOwnerofAnyCompany(user.Id);
            if (isAdminandHasCompany)
            {
                return View(layout);
            }
            return RedirectToAction("List", "Inbox");
        }

        public ActionResult GetAllCompanyOwnedbyUser()
        {
            Initialize();
            var data = companyService.GetAllCompanyOwnedbyUser(user.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Member(long id)
        {
            if (!Initialize())
                return RedirectToAction("Index", "LoginController");
            var company = companyService.GetCompany(id);
            if (company == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            if (!memberService.checkIsAdmin(user.Id, id))
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
            foreach (AddMemberResponse item in data)
            {
                companyService.SendEmailAddMember(item);
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// API to get all the company Owned and Managed by the user logged in
        /// </summary>
        /// <returns></returns>
        public ActionResult GetOwnedandManagedCompany()
        {
            InitializeAPI();
            var data = companyService.GetOwnedandManagedCompany(user.Id);
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