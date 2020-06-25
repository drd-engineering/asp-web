﻿using DRD.Models;
using DRD.Models.API;
using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service;
using System.Web.Mvc;

namespace DRD.App.Controllers
{
    public class RotationController : Controller
    {
        LoginController login = new LoginController();
        RotationService rotationService = new RotationService();
        RotationProcessService rotationProcessService = new RotationProcessService();
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
        /// <summary>
        /// New Rotation item Page
        /// </summary>
        /// <returns></returns>
        public ActionResult New()
        {
            if (!Initialize())
                return RedirectToAction("Index", "login", new { redirectUrl = "Rotation/New" });

            Rotation product = new Rotation();
            layout.obj = product;
            return View(layout);
        }

        /// <summary>
        /// Rotation List Page
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            if (!Initialize())
                return RedirectToAction("Index", "login", new { redirectUrl = "Rotation/List" });
            return View(layout);
        }

        /// <summary>
        /// Rotation Details Page by Id and user created
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index(long id)
        {
            if (!Initialize())
                return RedirectToAction("Index", "login", new { redirectUrl = "Rotation?id="+id });
            
            var product =  rotationService.GetRotationById(id, user.Id);
            if (product == null) return RedirectToAction("list", "rotation");

            layout.obj = product;
            return View(layout);
        }

        public ActionResult GetById(long id)
        {
            var rotationService = new RotationService();// getUserLogin().AppZone.Code);
            var data = rotationService.GetById(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(RotationItem prod)
        {
            InitializeAPI();
            prod.CreatorId = user.Id;
            prod.UserId = user.Id;
            var data = rotationService.Save(prod);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Start(long rotationId, long subscriptionId)
        {
            InitializeAPI();

            //TODO make response status work and using the object instead of the code variable
            var data = rotationProcessService.Start(user.Id, rotationId, subscriptionId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// API to obtain rotation that user logged in have related to criteria
        /// </summary>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult FindRotations(string criteria, int page, int pageSize)
        {
            InitializeAPI();
            int skip = pageSize * (page - 1);
            var data = rotationService.FindRotations(user.Id, criteria, skip, pageSize);
            if (data != null)
            {
                MenuService menuService = new MenuService();
                foreach (var item in data)
                {
                    item.Key = menuService.EncryptData(item.Id);
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// API to count all the rotation that user logged in have related to criteria
        /// </summary>
        /// <param name="topCriteria"></param>
        /// <returns></returns>
        public ActionResult FindRotationCountAll(string criteria)
        {
            InitializeAPI();
            var data = rotationService.FindRotationCountAll(user.Id, criteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUsersWorkflow(long id)
        {
            var data = rotationService.GetUsersWorkflow(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProcessActivity(ProcessActivity param, int bit)
        {
            InitializeAPI();
            var data = rotationProcessService.ProcessActivity(param, (Constant.EnumActivityAction)bit);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteRotation(long id)
        {
            InitializeAPI();

            var data = rotationService.Delete(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}