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
        private UserService userService;

        private LoginController login;
        private UserSession user;
        private Layout layout;

        //helper
        private bool CheckLogin(bool getMenu = false)
        {
            login = new LoginController();
            if (login.CheckLogin(this))
            {
                user = login.GetUser(this);
                companyService = new CompanyService();
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

        // GET: Company
        public ActionResult Index()
        {
            if (!CheckLogin(getMenu:true)) 
                return RedirectToAction("Index", "login", new { redirectUrl = "company" });
            
            if (!userService.IsAdminOrOwnerofAnyCompany(user.Id)) 
                return RedirectToAction("Index", "Dashboard");
            
            return View(layout);   
        }

        // GET: Company
        public ActionResult Member(long id = Constant.ID_NOT_FOUND)
        {
            if (!CheckLogin(getMenu: true))
                return RedirectToAction("Index", "login", new { redirectUrl = "Company/Member?id=" + id });

            var company = companyService.GetCompany(id, user.Id);
            if (company == null)
                return RedirectToAction("Index", "Dashboard");

            ViewBag.Company = company.Name;
            return View(layout);
        }

        /// <summary>
        /// API to get company owned by user
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCompaniesOwnedByUser()
        {
            if (!CheckLogin(getMenu: false))
                return RedirectToAction("Index", "login", new { redirectUrl = "dashboard" });

            var data = companyService.GetCompaniesOwnedByUser(user.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Invite bulk emails to be company members need to be accepted by the recipient
        /// </summary>
        /// <returns></returns>
        public ActionResult AddMembers(long companyId, string emails)
        {
            CheckLogin();
            var data = companyService.AddMembers(companyId,user.Id, emails, user.Id);
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
        /// <summary>
        /// Accept member request in company member invitation management
        /// </summary>
        /// <returns></returns>
        public ActionResult AcceptMember(long memberId)
        {
            CheckLogin();
            var data = companyService.AcceptMember(memberId, user.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Reject member request in company member invitation management
        /// </summary>
        /// <returns></returns>
        public ActionResult RejectMember(long memberId)
        {
            CheckLogin();
            var data = companyService.RejectMember(memberId, user.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}