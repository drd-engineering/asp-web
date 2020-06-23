using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public void InitializeAPI()
        {
            user = login.GetUser(this);
            login.CheckLogin(this);
        }

        /// <summary>
        /// Access Page Inbox related to the inbox id that user has
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (!Initialize())
                return RedirectToAction("Index", "LoginController");

            layout.obj = login.GetUser(this);
            layout.activeId = 0;

            System.Diagnostics.Debug.WriteLine("THIS IS LAYOUT :: "+ layout);
            if(layout != null)
            System.Diagnostics.Debug.WriteLine("THIS IS LAYOUT not null :: "+ layout.user.ImageProfile + layout.user.EncryptedId );

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

        // redundant with the login method
        public ActionResult GetUserLogin()
        {
            UserService userService = new UserService();
            long id = login.GetUser(this).Id;
            var data = userService.GetById(id, id);
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