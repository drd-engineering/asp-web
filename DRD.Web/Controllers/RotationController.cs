using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Core;

using DRD.Domain;
using System.Based.Core;

namespace DRD.Web.Controllers
{
    public class RotationController : Controller
    {
        private DtoMemberLogin getUserLogin()
        {
            LoginController login = new LoginController();
            return login.GetUser(this);
        }

        public ActionResult Rotation(string mid)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            // begin decription menu
            DtoMemberLogin user = login.GetUser(this);
            var strmenu = login.ManipulateSubMenu(this, user, mid);
            // end decription menu

            DtoRotation product = new DtoRotation();
            string[] ids = strmenu.Split(',');
            if (ids.Length > 1 && !ids[1].Equals("0"))
            {
                RotationService psvr = new RotationService();// getUserLogin().AppZone.Code);
                product = psvr.GetHeaderById(int.Parse(ids[1]));
            }

            JsonLayout layout = new JsonLayout();
            layout.activeId = int.Parse(ids[0]);
            layout.key = mid.Split(',')[0];
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = login.GetUser(this);
            layout.obj = product;
            layout.dbmenus = login.GetDashbordMenus(this, layout.activeId);
            return View(layout);
        }

        public ActionResult RotationList(string mid)
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
            layout.dbmenus = login.GetDashbordMenus(this, layout.activeId);
            UserMenuService umsvr = new UserMenuService();
            if (!umsvr.ValidGroupMenu(layout.user.UserGroup, layout.activeId))
                Response.Redirect("/");

            return View(layout);
        }
        
        public ActionResult GetById(long id)
        {
            var srv = new RotationService();// getUserLogin().AppZone.Code);
            var data = srv.GetById(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(DtoRotation prod)
        {
            DtoMemberLogin user = getUserLogin();
            prod.CreatorId = user.Id;
            prod.MemberId = user.Id;

            var srv = new RotationService();// user.AppZone.Code);
            var data = srv.Save(prod);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Start(long id)
        {
            DtoMemberLogin user = getUserLogin();
            //prod.AppZone = user.AppZone.Code;
            //prod.UserId = user.UserId;

            var srv = new RotationService();// user.AppZone.Code);
            var data = srv.Start(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

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
            DtoMemberLogin user = getUserLogin();
            var data = srv.GetList(user.Id, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetListCount(string topCriteria)
        {
            var srv = new RotationService();// getUserLogin().AppZone.Code);
            DtoMemberLogin user = getUserLogin();
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
            DtoMemberLogin user = getUserLogin();
            var data = srv.GetLiteAll(user.Id, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLiteAllCount(string topCriteria)
        {
            var srv = new RotationService();// getUserLogin().AppZone.Code);
            DtoMemberLogin user = getUserLogin();
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
            DtoMemberLogin user = getUserLogin();
            var data = srv.GetLiteStatusAll(user.Id, status, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLiteStatusAllCount(string topCriteria, string status)
        {
            var srv = new RotationService();// getUserLogin().AppZone.Code);
            DtoMemberLogin user = getUserLogin();
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
            DtoMemberLogin user = getUserLogin();
            var data = srv.GetNodeLiteAll(user.Id, status, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetNodeLiteAllCount(string status, string topCriteria)
        {
            var srv = new RotationService();// getUserLogin().AppZone.Code);
            DtoMemberLogin user = getUserLogin();
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

        public ActionResult ProcessActivity(JsonProcessActivity param, int bit)
        {
            var srv = new RotationService();// getUserLogin().AppZone.Code);
            DtoMemberLogin user = getUserLogin();

            var data = srv.ProcessActivity(param, (ConfigConstant.EnumActivityAction)bit);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}