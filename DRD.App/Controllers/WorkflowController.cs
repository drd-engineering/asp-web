﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DRD.Models;
using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service;


namespace DRD.App.Controllers
{
    public class WorkflowController : Controller
    {
        private UserSession getUserLogin()
        {
            LoginController login = new LoginController();
            return login.GetUser(this);
        }

        public ActionResult Workflow(string mid)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            // begin decription menu
            UserSession user = login.GetUser(this);
           // var strmenu = login.ManipulateSubMenu(this, user, mid);
            // end decription menu

            WorkflowData product = new WorkflowData();
            //string[] ids = strmenu.Split(',');
            //if (ids.Length > 1 && !ids[1].Equals("0"))
            //{
            //    WorkflowService psvr = new WorkflowService();// getUserLogin().AppZone.Code);
              //  product = psvr.GetById(int.Parse(ids[1]));
            //}

            Layout layout = new Layout();
            //layout.activeId = int.Parse(ids[0]);
            layout.key = mid.Split(',')[0];
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.objItems = login.GetMenuObjectItems(layout.menus, layout.activeId);
            layout.user = login.GetUser(this);
            layout.obj = product;

            return View(layout);
        }

        public ActionResult WorkflowList(string mid)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            // begin decription menu
            UserSession user = login.GetUser(this);
            //var strmenu = login.ManipulateMenu(this, user, mid);
            // end decription menu

            Layout layout = new Layout();
            //layout.activeId = int.Parse(strmenu);
            layout.key = mid;
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = login.GetUser(this);

            return View(layout);
        }
        public ActionResult GetById(long id)
        {
            var srv = new WorkflowService();// getUserLogin().AppZone.Code);
            var data = srv.GetById(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(WorkflowData prod)
        {
            UserSession user = getUserLogin();
            prod.CreatorId = user.Id;
            prod.UserId = user.Email;
            prod.Type = 0;
            var srv = new WorkflowService();// user.AppZone.Code);
            var data = srv.Save(prod);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveDraft(WorkflowData prod)
        {
            UserSession user = getUserLogin();
            prod.CreatorId = user.Id;
            prod.UserId = user.Email;
            prod.Type = 1;
            var srv = new WorkflowService();// user.AppZone.Code);
            var data = srv.Save(prod);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLiteAll(string topCriteria, int page, int pageSize)
        {
            LoginController login = new LoginController();
            UserSession user = login.GetUser(this);
            var srv = new WorkflowService();// getUserLogin().AppZone.Code);
            var data = srv.GetLiteAll(user.Id, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLiteAllCount(string topCriteria)
        {
            LoginController login = new LoginController();
            UserSession user = login.GetUser(this);
            var srv = new WorkflowService();// getUserLogin().AppZone.Code);
            var data = srv.GetLiteAllCount(user.Id, topCriteria);
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
            LoginController login = new LoginController();
            UserSession user = login.GetUser(this);
            var srv = new WorkflowService();// getUserLogin().AppZone.Code);
            var data = srv.GetLiteAll(user.Id, topCriteria, page, pageSize, null, null);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLiteAllCount2(string topCriteria, string criteria)
        {
            LoginController login = new LoginController();
            UserSession user = login.GetUser(this);
            var srv = new WorkflowService();// getUserLogin().AppZone.Code);
            var data = srv.GetLiteAllCount(user.Id, topCriteria, null);
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
        public ActionResult GetPopupAll(string topCriteria, int page, int pageSize, string criteria)
        {
            LoginController login = new LoginController();
            UserSession user = login.GetUser(this);
            var srv = new WorkflowService();// getUserLogin().AppZone.Code);
            var data = srv.GetPopupAll(user.Id, topCriteria, page, pageSize, null, criteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPopupAllCount(string topCriteria, string criteria)
        {
            LoginController login = new LoginController();
            UserSession user = login.GetUser(this);
            var srv = new WorkflowService();// getUserLogin().AppZone.Code);
            var data = srv.GetPopupAllCount(user.Id, topCriteria, criteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult GetProject(long Id)
        {
            var srv = new WorkflowService();// getUserLogin().AppZone.Code);
            var data = srv.GetById(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}