using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DRD.Core;

using DRD.Domain;

namespace DRD.Web.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ChangePassword()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            MenuService svr = new MenuService();
            JsonLayout layout = new JsonLayout();
            layout.menus = login.GetMenus(this, 0);// int.Parse(mid));
            layout.user = login.GetUser(this);
            layout.activeId = 0;// int.Parse(0);
            return View(layout);
        }

        public ActionResult Login(string username, string password)
        {
            int ret = -1;
            MemberService svr = new MemberService();
            var user = svr.Login(username, password);
            if (user != null)
            {
                if (user.CompanyId == null)
                    ret = 2;
                else {

                    CompanyService azsvr = new CompanyService();
                    //if (azsvr.GetById((long)user.CompanyId) == null)
                    if (user.UserGroup == null)
                        ret = -1;
                    else
                        ret = 1;
                }
                if (ret != -1)
                    Session["_USER_LOGIN_"] = user;
            }
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        public void SetLogin(Controller controller, DtoMemberLogin user)
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

        public void UpdateCompanyProfile(Controller controller, DtoCompany company)
        {
            DtoMemberLogin user = (DtoMemberLogin)controller.Session["_USER_LOGIN_"];
            if (user == null) return;

            user.Company = company;
            controller.Session["_USER_LOGIN_"] = user;
        }

        public DtoMemberLogin GetUser(Controller controller)
        {
            DtoMemberLogin user = (DtoMemberLogin)controller.Session["_USER_LOGIN_"];
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

        public List<JsonMenu> GetMenus(Controller controller, int activeId)
        {
            DtoMemberLogin user = (DtoMemberLogin)controller.Session["_USER_LOGIN_"];
            if (user == null)
                return null;
            UserMenuService svr = new UserMenuService();

            return svr.GetMenus(user.Id, user.UserGroup, activeId);
        }
        public List<JsonMenu> GetDashbordMenus(Controller controller, int activeId)
        {
            DtoMemberLogin user = (DtoMemberLogin)controller.Session["_USER_LOGIN_"];
            if (user == null)
                return null;
            UserMenuService svr = new UserMenuService();

            return svr.GetDashboardMenus(user.Id, user.UserGroup, activeId);
        }
        public List<string> GetMenuObjectItems(List<JsonMenu> menus, int parentId)
        {
            List<string> items = new List<string>();
            foreach (JsonMenu m in menus)
            {
                if (int.Parse(m.ParentCode) == parentId && m.ItemType == 1)
                {
                    items.Add(m.ObjectName);
                }
            }
            return items;
        }

        //public int UpdatePassword(string oldPassword, string newPassword)
        //{
        //    UserService svr = new UserService();
        //    JsonUser user = (JsonUser)Session["_USER_LOGIN_"];
        //    return svr.ChangePassword(user.Id, oldPassword, newPassword);
        //}
        public int UpdatePassword(string oldPassword, string newPassword)
        {
            MemberService svr = new MemberService();
            DtoMemberLogin user = (DtoMemberLogin)Session["_USER_LOGIN_"];
            return svr.ChangePassword(user.Id, oldPassword, newPassword);
        }

        public int ResetPassword(string email)
        {
            MemberService svr = new MemberService();
            return svr.ResetPassword(email);
        }

        public string ManipulateMenu(Controller controller, DtoMemberLogin user, string data)
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
            UserMenuService umsvr = new UserMenuService();
            if (!umsvr.ValidGroupMenu(user.UserGroup, int.Parse(datas[1])))
                controller.Response.Redirect("/error/invalidpage");

            return datas[1];
        }

        public string ManipulateSubMenu(Controller controller, DtoMemberLogin user, string data)
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