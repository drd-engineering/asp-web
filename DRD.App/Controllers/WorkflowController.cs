using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

using DRD.Models;
using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Models.API;
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

        // GET : Workflow/new
        public ActionResult New()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);
            UserSession user = login.GetUser(this);
            ListWorkflowData product = new ListWorkflowData();
            Layout layout = new Layout();
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.objItems = login.GetMenuObjectItems(layout.menus, layout.activeId);
            layout.user = login.GetUser(this);
            layout.obj = product;

            return View(layout);
        }

        //GET : Workflow/list
        public ActionResult List()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);
            UserSession user = login.GetUser(this);
            Layout layout = new Layout();
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

        public ActionResult FindWorkflows(string topCriteria, int page, int pageSize)
        {
            LoginController login = new LoginController();
            UserSession user = login.GetUser(this);
            var srv = new WorkflowService();// getUserLogin().AppZone.Code);
            var data = srv.FindWorkflows(user.Id, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
/*
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
            Expression<Func<WorkflowData, bool>> criteriaUsed = WorkflowData => true;
            if (!criteria.Equals(""))
                criteriaUsed = WorkflowData => criteria == "";
            var data = srv.GetPopupAll(user.Id, topCriteria, page, pageSize, null, criteriaUsed);
            return Json(data, JsonRequestBehavior.AllowGet);
        }*/

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