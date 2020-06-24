﻿using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service;
using System.Web.Mvc;


namespace DRD.App.Controllers
{
    public class WorkflowController : Controller
    {
        LoginController login = new LoginController();
        WorkflowService workflowService = new WorkflowService();
        UserSession user;
        Layout layout = new Layout();

        public bool Initialize()
        {
            if (login.CheckLogin(this))
            {
                user = login.GetUser(this);
                layout.menus = login.GetMenus(this, layout.activeId);
                layout.user = login.GetUser(this);
                return true;
            }
            return false;
        }
        public void InitializeAPI()
        {
            user = login.GetUser(this);
            login.CheckLogin(this);
        }

        // GET : Workflow/new
        public ActionResult New()
        {
            if (!Initialize())
                return RedirectToAction("Index", "Login");
            WorkflowItem product = new WorkflowItem();
            layout.obj = product;

            return View(layout);
        }

        public ActionResult Index(long id)
        {
            if (!Initialize())
                return RedirectToAction("Index", "Login");
            WorkflowItem product = new WorkflowItem();
            product = workflowService.GetById(id);
            layout.obj = product;

            return View(layout);
        }

        //GET : Workflow/list
        public ActionResult List()
        {
            if (!Initialize())
                return RedirectToAction("Index", "Login");
            return View(layout);
        }
        public ActionResult GetById(long id)
        {
            var srv = new WorkflowService();// getUserLogin().AppZone.Code);
            var data = srv.GetById(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(WorkflowItem prod)
        {
            Initialize();
            prod.CreatorId = user.Id;
            prod.UserEmail = user.Email;
            prod.Type = 0;
            var srv = new WorkflowService();// user.AppZone.Code);
            var data = srv.Save(prod);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveDraft(WorkflowItem prod)
        {
            Initialize();
            prod.CreatorId = user.Id;
            prod.UserEmail = user.Email;
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
            var data = srv.FindWorkflows(user.Id, topCriteria, page, pageSize, null);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FindWorkflowsCountAll(string topCriteria)
        {
            LoginController login = new LoginController();
            UserSession user = login.GetUser(this);
            var srv = new WorkflowService();// getUserLogin().AppZone.Code);
            var data = srv.FindWorkflowsCountAll(user.Id, topCriteria);
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
                    Expression<Func<WorkflowItem, bool>> criteriaUsed = WorkflowItem => true;
                    if (!criteria.Equals(""))
                        criteriaUsed = WorkflowItem => criteria == "";
                    var data = srv.GetPopupAll(user.Id, topCriteria, page, pageSize, null, criteriaUsed);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }*/

        /*/// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult GetProject(long Id)
        {
            var srv = new WorkflowService();// getUserLogin().AppZone.Code);
            var data = srv.GetById(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }*/

    }
}