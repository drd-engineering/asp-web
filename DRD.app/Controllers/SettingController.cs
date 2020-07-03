using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using DRD.Models;
using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service;

namespace DRD.App.Controllers
{
    public class SettingController : Controller
    {
        SettingService settingService = new SettingService();

        private LoginController login;
        private UserSession user;
        private Layout layout;

        //helper
        private bool CheckLogin(bool getMenu = false)
        {
            login = new LoginController();
            if (login.CheckLogin(this))
            {
                //instantiate
                user = login.GetUser(this);
                settingService = new SettingService();
                if (getMenu)
                {
                    //get menu if user authenticated
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

        // GET : Setting
        public ActionResult Index()
        {
            if (!CheckLogin(getMenu : true))
                return RedirectToAction("Index", "login", new { redirectUrl = "setting" });
            return View(layout);
        }

        // GET : Setting/Account
        public ActionResult Account()
        {
            return View();
        }

        // GET : Setting/Company
        public ActionResult Company()
        {
            return View();
        }

        // GET : Setting/Notification
        public ActionResult Notification()
        {
            return View();
        }

        /// <summary>
        /// API save workflow, workflow nodes and workflow links
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCompanySetting()
        {
            if (!CheckLogin())
                return Json(HttpStatusCode.Unauthorized, JsonRequestBehavior.AllowGet);
            var data = settingService.GetCompanySetting(user.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// API save workflow, workflow nodes and workflow links
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public ActionResult AcceptCompanyInvitation(long companyId)
        {
            if (!CheckLogin())
                return Json(HttpStatusCode.Unauthorized, JsonRequestBehavior.AllowGet);
            var data = settingService.AcceptCompany(companyId, user.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// API save workflow, workflow nodes and workflow links
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public ActionResult RejectCompanyInvitation(long companyId)
        {
            if (!CheckLogin())
                return Json(HttpStatusCode.Unauthorized, JsonRequestBehavior.AllowGet);
            var data = settingService.ResetState(companyId, user.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}