using DRD.Models;
using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI;

namespace DRD.App.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            if (this.Session["_USER_LOGIN_"] != null)
                return RedirectToAction("Index", "DashboardController");
            return View();
        }

        public ActionResult ChangePassword()
        {
            CheckLogin(this);
            GetUser(this);
            return View();
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        public ActionResult Login(string username, string password)
        {
            int ret = -1;
            UserService userService = new UserService();
            var user = userService.Login(username, password);
            if (user != null)
            {
                ret = 1;
                Session["_USER_LOGIN_"] = user;
            }
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public void SetLogin(Controller controller, User user)
        {
            controller.Session["_USER_LOGIN_"] = user;
        }

        /// <summary>
        /// API LOGOUT Clear session of a user logged in
        /// </summary>
        public void Logout()
        {
            Session["_USER_LOGIN_"] = null;
            Session["_COUNTERACTIVITY_"] = null;
            Session["_SUBSCRIPTIONLIMIT_"] = null;
            Session["_COUNTERINBOX_"] = null;
            Response.Redirect("/Login");
        }

        [OutputCache(Duration = 1800, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        public bool CheckLogin(Controller controller)
        {
            if (controller.Session["_USER_LOGIN_"] == null)
            {
                controller.Response.Redirect("/Login");
                return false;
            }
            return true;
        }

        public UserSession GetUser(Controller controller)
        {
            UserSession user = (UserSession)controller.Session["_USER_LOGIN_"];
            //user.Id = user.Id;
            //user.UserId = user.UserId;
            //user.Name = user.Name;
            //user.ShortName = user.ShortName;
            //user.Location = user.Location;
            //user.AppZone = user.CompanyCode;

            //AppZoneService azsvr = new AppZoneService();
            //azsvr.GetByCode();

            return user;
        }

        public List<Menu> GetMenus(Controller controller, int activeId)
        {
            UserSession user = (UserSession)controller.Session["_USER_LOGIN_"];
            if (user == null)
                return null;
            MenuService menuService = new MenuService();

            return menuService.GetMenus(user.Id);
        }

        public List<string> GetMenuObjectItems(List<Menu> menus, int parentId)
        {
            List<string> items = new List<string>();
            foreach (Menu m in menus)
            {
                if (int.Parse(m.ParentCode) == parentId && m.ItemType == 1)
                {
                    items.Add(m.ObjectName);
                }
            }
            return items;
        }

        public string ManipulateMenu(Controller controller, UserSession user)
        {
            return "";
        }

        public string ManipulateSubMenu(Controller controller, UserSession user)
        {
            return "";
        }

        // GET FUNCTION for change password
        public ActionResult UpdatePassword(String oldPassword, String newPassword)
        {
            CheckLogin(this);
            UserSession user = GetUser(this);
            UserService usrService = new UserService();
            var data = usrService.UpdatePassword(user, oldPassword, newPassword);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET FUNCTION for reset password
        public ActionResult ResetPassword(String emailUser)
        {
            UserService usrService = new UserService();
            var data = usrService.ResetPassword(emailUser);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}