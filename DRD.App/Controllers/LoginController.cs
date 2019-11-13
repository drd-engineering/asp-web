using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DRD.Models;
using DRD.Service;

namespace DRD.App.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
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

        public ActionResult ForgotPassword()
        {
            return View();
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

        public void CheckLogin(Controller controller)
        {
            if (controller.Session["_USER_LOGIN_"] == null) controller.Response.Redirect("/Login");
        }

        public void UpdateCompanyProfile(Controller controller, Company company)
        {
            User user = (User)controller.Session["_USER_LOGIN_"];
            if (user == null) return;

            //user.Company = company;
            controller.Session["_USER_LOGIN_"] = user;
        }

        public User GetUser(Controller controller)
        {
            User user = (User)controller.Session["_USER_LOGIN_"];
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

        
    }
}