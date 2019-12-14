using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;

using DRD.Models;
using DRD.Models.View;
using DRD.Models.View.Rotation;
using DRD.Models.Custom;
using DRD.Service;

namespace DRD.App.Controllers
{
    public class RotationController : Controller
    {
        LoginController login = new LoginController();
        MemberService memberService = new MemberService();
        RotationService rotationService = new RotationService();
        UserSession user;
        Layout layout = new Layout();

        public void Initialize()
        {
            user = login.GetUser(this);
            login.CheckLogin(this);
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = login.GetUser(this);
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
            Rotation product = new Rotation();
            
            Initialize();
            layout.obj = product;
            
            return View(layout);
        }
        /// <summary>
        /// Rotation List Page
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            Initialize();
            return View(layout);
        }
        /// <summary>
        /// Rotation Details Page by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index(long id)
        {
            Initialize();
            layout.obj = rotationService.GetRotationById(id);
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
            
            var data = rotationService.Start(user.Id, rotationId, subscriptionId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FindRotations(string topCriteria, int page, int pageSize)
        {
            var rotationService = new RotationService();// getUserLogin().AppZone.Code);
            InitializeAPI();
            var data = rotationService.FindRotations(user.Id, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLiteAll(string topCriteria, int page, int pageSize)
        {
            var rotationService = new RotationService();// getUserLogin().AppZone.Code);
            InitializeAPI();
            var data = rotationService.GetLiteAll(user.Id, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLiteAllCount(string topCriteria)
        {
            var rotationService = new RotationService();// getUserLogin().AppZone.Code);
            InitializeAPI();
            var data = rotationService.GetLiteAllCount(user.Id, topCriteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLiteStatusAll(string topCriteria, string status, int page, int pageSize)
        {
            var rotationService = new RotationService();// getUserLogin().AppZone.Code);
            InitializeAPI();
            var data = rotationService.GetLiteStatusAll(user.Id, status, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLiteStatusAllCount(string topCriteria, string status)
        {
            var rotationService = new RotationService();// getUserLogin().AppZone.Code);
            InitializeAPI();
            var data = rotationService.GetLiteStatusAllCount(user.Id, status, topCriteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNodeLiteAll(string status, string topCriteria, int page, int pageSize)
        {
            var rotationService = new RotationService();// getUserLogin().AppZone.Code);
            InitializeAPI();
            var data = rotationService.GetNodeLiteAll(user.Id, status, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetNodeLiteAllCount(string status, string topCriteria)
        {
            var rotationService = new RotationService();// getUserLogin().AppZone.Code);
            InitializeAPI();
            var data = rotationService.GetNodeLiteAllCount(user.Id, status, topCriteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUsersWorkflow(long id)
        {
            
            var data = rotationService.GetUsersWorkflow(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult ProcessActivity(ProcessActivity param, int bit)
        //{
        //    var rotationService = new RotationService();// getUserLogin().AppZone.Code);
        //    InitializeAPI();

            //var data = rotationService.ProcessActivity(param, (Constant.EnumActivityAction)bit);
          //  return Json(data, JsonRequestBehavior.AllowGet);
        //}
    }
}