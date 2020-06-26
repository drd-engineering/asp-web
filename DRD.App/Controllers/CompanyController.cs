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
        private CompanyService companyService;
        private MemberService memberService;
        private SubscriptionService subscriptionService;
        private UserService userService;

        private LoginController login;
        private UserSession user;
        private Layout layout;

        private bool CheckLogin(bool getMenu = false)
        {
            login = new LoginController();
            if (login.CheckLogin(this))
            {
                user = login.GetUser(this);
                companyService = new CompanyService();
                memberService = new MemberService();
                subscriptionService = new SubscriptionService();
                userService = new UserService();

                if (getMenu)
                {
                    layout = new Layout
                    {
                        Menus = login.GetMenus(this),
                        User = login.GetUser(this)
                    };
                }
                return true;
            }
            return false;
        }

        public ActionResult Index()
        {
            if (!CheckLogin(getMenu:true))
                return RedirectToAction("Index", "login", new { redirectUrl = "company" });
            
            if (!userService.IsAdminOrOwnerofAnyCompany(user.Id)) return RedirectToAction("List", "Inbox");
            
            return View(layout);
            
            
        }

        public ActionResult GetAllCompanyOwnedbyUser()
        {
            CheckLogin();
            var data = companyService.GetAllCompanyOwnedbyUser(user.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Member(long id = Constant.ID_NOT_FOUND)
        {
            if (!CheckLogin(getMenu: true))
                return RedirectToAction("Index", "login", new { redirectUrl = "Company/Member?id"+id });
            var company = companyService.GetCompany(id);
            if (company == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            if (!memberService.checkIsAdmin(user.Id, id) && !memberService.checkIsOwner(user.Id, id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            ViewBag.Company = company.Name;
            return View(layout);
        }

        public ActionResult AddMembers(long companyId, string emails)
        {
            CheckLogin();
            var data = memberService.AddMembers(companyId, user.Id, emails);
            foreach (AddMemberResponse item in data)
            {
                memberService.SendEmailAddMember(item);
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// API to get all the company Owned and Managed by the user logged in
        /// </summary>
        /// <returns></returns>
        public ActionResult GetOwnedandManagedCompany()
        {
            CheckLogin();
            var data = companyService.GetOwnedandManagedCompany(user.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSubscriptionList()
        {
            CheckLogin();
            List<BusinessPackage> data = subscriptionService.GetAllPublicSubscription();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AcceptMember(long memberId)
        {
            CheckLogin();
            var data = companyService.AcceptMember(memberId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RejectMember(long memberId)
        {
            CheckLogin();
            var data = companyService.RejectMember(memberId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}