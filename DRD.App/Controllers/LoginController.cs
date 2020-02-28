using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DRD.Models;
using DRD.Models.View;
using DRD.Models.Custom;
using DRD.Service;
using System.Web.UI;

namespace DRD.App.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
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

        public void Logout()
        {
            Session["_USER_LOGIN_"] = null;
            Session["_COUNTER_"] = null;
            Response.Redirect("/Login");
        }

        [OutputCache(Duration = 1800, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        public void CheckLogin(Controller controller)
        {
            if (controller.Session["_USER_LOGIN_"] == null) controller.Response.Redirect("/Login");
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

            return menuService.GetMenus(activeId);
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