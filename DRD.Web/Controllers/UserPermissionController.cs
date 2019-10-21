using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Core;

using DRD.Domain;

namespace DRD.Web.Controllers
{
    public class UserPermissionController : Controller
    {
        private DtoMemberLogin getUserLogin()
        {
            LoginController login = new LoginController();
            return login.GetUser(this);
        }

        public ActionResult UserPermissionList(string mid)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);
            MenuService msvr = new MenuService();
            string decx = msvr.Decrypt(mid);
            if (decx == null)
            {
                Response.Redirect("/error/invalidpage");
            }
            JsonLayout layout = new JsonLayout();
            layout.activeId = int.Parse(decx);
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = login.GetUser(this);

            UserMenuService umsvr = new UserMenuService();
            if (!umsvr.ValidGroupMenu(layout.user.UserGroup, layout.activeId))
                Response.Redirect("/");

            return View(layout);
        }

        public ActionResult UserPermission(string mid)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            JsonPermission product = new JsonPermission();
            string[] ids = mid.Split(',');
            if (!ids[1].Equals("0"))
            {
                UserPermissionService psvr = new UserPermissionService();
                product = psvr.GetByMemberId(int.Parse(ids[1]));
            }

            MenuService svr = new MenuService();
            JsonLayout layout = new JsonLayout();
            layout.activeId = int.Parse(ids[0]);
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = login.GetUser(this);
            layout.obj = product;

            UserMenuService umsvr = new UserMenuService();
            if (!umsvr.ValidGroupMenu(layout.user.UserGroup, layout.activeId))
                Response.Redirect("/");

            return View(layout);
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prod"></param>
        /// <returns></returns>

        public ActionResult Save(JsonPermission permit)
        {
            DtoMemberLogin user = getUserLogin();
            permit.UserId = user.Email;

            var srv = new UserPermissionService();
            var data = srv.Save(permit);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}