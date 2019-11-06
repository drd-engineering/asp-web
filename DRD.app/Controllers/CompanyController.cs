using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Core;

using DRD.Domain;

namespace DRD.Web.Controllers
{
    public class CompanyController : Controller
    {
        //public ActionResult Company(string mid)
        //{
        //    LoginController login = new LoginController();
        //    login.CheckLogin(this);

        //    DtoCompany product = new DtoCompany();
        //    string[] ids = mid.Split(',');
        //    if (!ids[1].Equals("0"))
        //    {
        //        CompanyService psvr = new CompanyService();// getUserLogin().AppZone.Code);
        //        product = psvr.GetById(int.Parse(ids[1]));
        //    }

        //    JsonLayout layout = new JsonLayout();
        //    layout.activeId = int.Parse(ids[0]);
        //    layout.menus = login.GetMenus(this, layout.activeId);
        //    layout.user = login.GetUser(this);
        //    layout.obj = product;

        //    UserMenuService umsvr = new UserMenuService();
        //    if (!umsvr.ValidUserMenu(layout.user.Id, layout.activeId))
        //        Response.Redirect("/");

        //    return View(layout);
        //}

        public ActionResult Company(string mid)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            // begin decription menu
            DtoMemberLogin user = login.GetUser(this);
            var strmenu = login.ManipulateMenu(this, user, mid);
            // end decription menu
            
            DtoCompany product = new DtoCompany();
            CompanyService psvr = new CompanyService();
            product = psvr.GetById((long)user.CompanyId);

            JsonLayout layout = new JsonLayout();
            layout.activeId = int.Parse(strmenu);
            layout.key = mid.Split(',')[0];
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = user;
            layout.obj = product;

            UserMenuService umsvr = new UserMenuService();
            if (!umsvr.ValidGroupMenu(layout.user.UserGroup, layout.activeId))
                Response.Redirect("/");

            return View(layout);
        }

        //public ActionResult Company(string mid)
        //{
        //    LoginController login = new LoginController();
        //    login.CheckLogin(this);

        //    DtoMemberLogin user = login.GetUser(this);

        //    DtoCompany product = new DtoCompany();
        //    CompanyService psvr = new CompanyService();
        //    product = psvr.GetById((long)user.CompanyId);

        //    JsonLayout layout = new JsonLayout();
        //    layout.activeId = 0;
        //    layout.menus = login.GetMenus(this, layout.activeId);
        //    layout.user = user;
        //    layout.obj = product;

        //    return View(layout);
        //}

        public ActionResult CompanyList(string mid)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            // begin decription menu
            DtoMemberLogin user = login.GetUser(this);
            var strmenu = login.ManipulateMenu(this, user, mid);
            // end decription menu

            JsonLayout layout = new JsonLayout();
            layout.activeId = int.Parse(strmenu);
            layout.key = mid;
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = login.GetUser(this);
            
            return View(layout);
        }

        //public ActionResult SelectCompany(string code)
        //{
        //    LoginController login = new LoginController();
        //    login.CheckLogin(this);

        //    CompanyService azsvr = new CompanyService();
        //    JsonUser user = (JsonUser)Session["_USER_LOGIN_"];
        //    user.CompanyCode = code;
        //    user.Company = azsvr.GetByCode(code);
        //    Session["_USER_LOGIN_"] = user;
        //    Response.Redirect("/");

        //    return View();
        //}


        public ActionResult GetById(long id)
        {
            var srv = new CompanyService();
            var data = srv.GetById(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(DtoCompany prod)
        {
            LoginController login = new LoginController();
            login.UpdateCompanyProfile(this, prod);
            var srv = new CompanyService();
            var data = srv.Save(prod);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Registration(JsonSubscriptionRegistry registry)
        {
            var srv = new CompanyService();
            var data = srv.SaveRegistration(registry);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult GetLiteAll(string topCriteria, int page, int pageSize)
        {
            var srv = new CompanyService();
            var data = srv.GetLiteAll(topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLiteAllCount(string topCriteria)
        {
            var srv = new CompanyService();
            var data = srv.GetLiteAllCount(topCriteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public ActionResult GetLiteAll2(string topCriteria, int page, int pageSize, string criteria)
        {
            var srv = new CompanyService();// getUserLogin().AppZone.Code);
            var data = srv.GetLiteAll(topCriteria, page, pageSize, null, criteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLiteAllCount2(string topCriteria, string criteria)
        {
            var srv = new CompanyService();// getUserLogin().AppZone.Code);
            var data = srv.GetLiteAllCount(topCriteria, criteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public ActionResult GetLitePermissionAll(string topCriteria, int page, int pageSize, string criteria)
        {
            var srv = new CompanyService();// getUserLogin().AppZone.Code);
            var data = srv.GetLitePermissionAll(topCriteria, page, pageSize, null, criteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLitePermissionAllCount(string topCriteria, string criteria)
        {
            var srv = new CompanyService();// getUserLogin().AppZone.Code);
            var data = srv.GetLitePermissionAllCount(topCriteria, criteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}