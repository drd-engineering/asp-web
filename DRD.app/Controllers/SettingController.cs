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
        LoginController login = new LoginController();
        SettingService settingService = new SettingService();
        UserSession user;
        Layout layout = new Layout();

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
        public bool InitializeAPI()
        {
            user = login.GetUser(this);
             return login.CheckLogin(this);
        }

        /// <summary>
        /// Access Page Inbox related to the inbox id that user has
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (!Initialize())
                return RedirectToAction("Index", "Login");

            layout.obj = login.GetUser(this);
            layout.activeId = 0;

            return View(layout);
        }

        public ActionResult GetAccountSetting(long id)
        {
            if (!Initialize())
                return Json(-1, JsonRequestBehavior.AllowGet);

            return Json(layout.user, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Account()
        {
            return View();
        }

        public ActionResult Company()
        {
            return View();
        }

        public ActionResult Notification()
        {
            return View();
        }

        public ActionResult getCompanySetting()
        {
            if (!InitializeAPI())
                return Json(HttpStatusCode.Unauthorized, JsonRequestBehavior.AllowGet);
            var data = settingService.getCompanySetting(user.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // accept company invitation
        public ActionResult AcceptCompanyInvitation(long companyId)
        {
            if (!InitializeAPI())
                return Json(HttpStatusCode.Unauthorized, JsonRequestBehavior.AllowGet);
            var data = settingService.acceptCompany(companyId, user.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RejectCompanyInvitation(long companyId)
        {
            if (!InitializeAPI())
                return Json(HttpStatusCode.Unauthorized, JsonRequestBehavior.AllowGet);
            var data = settingService.resetState(companyId, user.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET Profile/GetData
        public ActionResult GetData()
        {
            login.CheckLogin(this);

            UserSession userSession = login.GetUser(this);

            UserService userService = new UserService();

            UserProfile user = userService.GetById(userSession.Id, userSession.Id);
            user.EncryptedId = Utilities.Encrypt(user.Id.ToString());
            var data = user;

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}