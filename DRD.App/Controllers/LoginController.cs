using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DRD.Models;
using DRD.Models.View;
using DRD.Models.Custom;
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

        public string ManipulateMenu(Controller controller, UserSession user, string data)
        {
            MenuService msvr = new MenuService();
            string decx = msvr.Decrypt(data);
            if (decx == null)
            {
                controller.Response.Redirect("/error/invalidpage");
                return null;
            }
            string[] datas = decx.Split(',');
            if (long.Parse(datas[0]) != user.Id)
            {
                controller.Response.Redirect("/error/invalidpage");
                return null;
            }
            return datas[1];
        }

        public string ManipulateSubMenu(Controller controller, UserSession user, string data)
        {
            string[] xdatas = data.Split(',');
            string data1 = ManipulateMenu(controller, user, xdatas[0]);

            if (xdatas.Length == 1)
                return data1 + ",0";

            MenuService msvr = new MenuService();
            string decx = msvr.Decrypt(xdatas[1]);
            if (decx == null)
                controller.Response.Redirect("/error/invalidpage");
            string[] datas = decx.Split(',');

            return data1 + "," + datas[1];
        }
    }
}