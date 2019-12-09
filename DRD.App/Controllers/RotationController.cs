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
    public class RotationController : Controller
    {
        private UserSession getUserLogin()
        {
            LoginController login = new LoginController();
            return login.GetUser(this);
        }

        public ActionResult New()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);
            Rotation product = new Rotation();
            Layout layout = new Layout();
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = login.GetUser(this);
            layout.obj = product;
            return View(layout);
        }

        public ActionResult List()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            // begin decription menu
            UserSession user = login.GetUser(this);
            var strmenu = login.ManipulateMenu(this, user);
            // end decription menu

            Layout layout = new Layout();
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = login.GetUser(this);
            //layout.dbmenus = login.GetDashbordMenus(this, layout.activeId);
            //UserMenuService umsvr = new UserMenuService();
            //if (!umsvr.ValidGroupMenu(layout.user.UserGroup, layout.activeId))
            //    Response.Redirect("/");

            return View(layout);
        }

        public ActionResult GetById(long id)
        {
            var srv = new RotationService();// getUserLogin().AppZone.Code);
            var data = srv.GetById(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(Rotation prod)
        {
            UserSession user = getUserLogin();
            prod.CreatorId = user.Id;

            var srv = new RotationService();// user.AppZone.Code);
            var data = srv.Save(prod);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult Start(long id)
        //{
            //UserSession user = getUserLogin();
            //prod.AppZone = user.AppZone.Code;
            //prod.UserId = user.UserId;

           // var srv = new RotationService();// user.AppZone.Code);
            //var data = srv.Start(id);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>

        public ActionResult GetList(string topCriteria, int page, int pageSize)
        {
            var srv = new RotationService();// getUserLogin().AppZone.Code);
            UserSession user = getUserLogin();
            var data = srv.GetList(user.Id, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetListCount(string topCriteria)
        {
            var srv = new RotationService();// getUserLogin().AppZone.Code);
            UserSession user = getUserLogin();
            var data = srv.GetListCount(user.Id, topCriteria);
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
            var srv = new RotationService();// getUserLogin().AppZone.Code);
            UserSession user = getUserLogin();
            var data = srv.GetLiteAll(user.Id, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLiteAllCount(string topCriteria)
        {
            var srv = new RotationService();// getUserLogin().AppZone.Code);
            UserSession user = getUserLogin();
            var data = srv.GetLiteAllCount(user.Id, topCriteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// 
        public ActionResult GetLiteStatusAll(string topCriteria, string status, int page, int pageSize)
        {
            var srv = new RotationService();// getUserLogin().AppZone.Code);
            UserSession user = getUserLogin();
            var data = srv.GetLiteStatusAll(user.Id, status, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLiteStatusAllCount(string topCriteria, string status)
        {
            var srv = new RotationService();// getUserLogin().AppZone.Code);
            UserSession user = getUserLogin();
            var data = srv.GetLiteStatusAllCount(user.Id, status, topCriteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>

        public ActionResult GetNodeLiteAll(string status, string topCriteria, int page, int pageSize)
        {
            var srv = new RotationService();// getUserLogin().AppZone.Code);
            UserSession user = getUserLogin();
            var data = srv.GetNodeLiteAll(user.Id, status, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetNodeLiteAllCount(string status, string topCriteria)
        {
            var srv = new RotationService();// getUserLogin().AppZone.Code);
            UserSession user = getUserLogin();
            var data = srv.GetNodeLiteAllCount(user.Id, status, topCriteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public ActionResult GetUsersWorkflow(long id)
        {
            var srv = new RotationService();// user.AppZone.Code);
            var data = srv.GetUsersWorkflow(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="bit"></param>
        /// <returns></returns>

        //public ActionResult ProcessActivity(ProcessActivity param, int bit)
        //{
        //    var srv = new RotationService();// getUserLogin().AppZone.Code);
        //    UserSession user = getUserLogin();

            //var data = srv.ProcessActivity(param, (Constant.EnumActivityAction)bit);
          //  return Json(data, JsonRequestBehavior.AllowGet);
        //}
    }
}