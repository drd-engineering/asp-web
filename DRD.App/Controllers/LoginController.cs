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
        public ActionResult Index(string redirectUrl = "/")
        {
            ViewBag.redirectUrl = redirectUrl;
            if (this.Session["_USER_LOGIN_"] != null)
                return RedirectToAction("Index", "Dashboard");
            return View();
        }

        public ActionResult ChangePassword()
        {
            if (!CheckLogin(this))
                return RedirectToAction("Index", "login", new { redirectUrl = "login/changepassword" });
            return View();
        }
        public ActionResult Ubhsppchngnds(string token)
        {
            UserService userService = new UserService();
            System.Diagnostics.Debug.WriteLine(userService.GenerateToken(3543151354,512));
            System.Diagnostics.Debug.WriteLine(userService.CheckTokenValidity(token));
            System.Diagnostics.Debug.WriteLine("TESTING");
            System.Diagnostics.Debug.WriteLine(userService.GenerateToken(52988479423338, 512));
            System.Diagnostics.Debug.WriteLine(userService.CheckTokenUserValidity(token));
            System.Diagnostics.Debug.WriteLine("TESTING");
            var data = userService.CheckTokenUserValidity(token);
            if (data == null)
                return RedirectToAction("Index", "login");
            return View(data);
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }
        /// <summary>
        /// API for login to DRD account using username and password
        /// </summary>
        /// <param name="username">there are many options for username, use id or email</param>
        /// <param name="password"></param>
        /// <returns></returns>
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

        /// <summary>
        /// API for login to DRD account using username and password
        /// </summary>
        /// <param name="username">there are many options for username, use id or email</param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ActionResult ResetPassword(string username, string password)
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

        /// <summary>
        /// LOGOUT Clear session of a user logged in
        /// </summary>
        public void Logout()
        {
            Session["_USER_LOGIN_"] = null;
            Session["_COUNTERACTIVITY_"] = null;
            Session["_SUBSCRIPTIONLIMIT_"] = null;
            Session["_COUNTERINBOX_"] = null;
            Response.Redirect("/Login");
        }

        /// <summary>
        /// CHECK is there any user logged in to DRD from the given controller
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        [OutputCache(Duration = 1800, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        public bool CheckLogin(Controller controller)
        {
            return controller.Session["_USER_LOGIN_"] != null;
        }

        /// <summary>
        /// GET user logged in details from sessions
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public UserSession GetUser(Controller controller)
        {
            UserSession user = (UserSession)controller.Session["_USER_LOGIN_"];
            return user;
        }
        /// <summary>
        /// GET menu list for layout
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public List<Menu> GetMenus(Controller controller)
        {
            UserSession user = (UserSession)controller.Session["_USER_LOGIN_"];
            if (user == null)
                return null;
            MenuService menuService = new MenuService();

            return menuService.GetMenus(user.Id);
        }
        /// <summary>
        /// API to change password after authorized login
        /// </summary>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public ActionResult UpdatePassword(String oldPassword, String newPassword)
        {
            CheckLogin(this);
            UserSession user = GetUser(this);
            UserService usrService = new UserService();
            var data = usrService.UpdatePassword(user, oldPassword, newPassword);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// API to change password after authorized login
        /// </summary>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public ActionResult ReplacePassword(String token, String newPassword)
        {
            UserService usrService = new UserService();
            var data = usrService.UpdatePassword(token, newPassword);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// API to request for reset password of user
        /// </summary>
        /// <param name="emailUser"></param>
        /// <returns></returns>
        public ActionResult ResetPassword(String emailUser)
        {
            UserService userService = new UserService();
            var data = userService.GetUser(emailUser);
            if (data != null)
                userService.SendEmailResetPassword(data);
            var result = data == null ? 0 : 1;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}