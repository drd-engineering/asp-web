using DRD.Models;
using DRD.Models.API;
using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service;
using System.Web.Mvc;

namespace DRD.App.Controllers
{
    public class RotationController : Controller
    {
        RotationService rotationService;
        RotationProcessService rotationProcessService;

        private LoginController login;
        private UserSession user;
        private Layout layout;

        //helper
        private bool CheckLogin(bool getMenu = false)
        {
            login = new LoginController();
            if (login.CheckLogin(this))
            {
                //instantiate
                user = login.GetUser(this);
                rotationService = new RotationService();
                rotationProcessService = new RotationProcessService();
                if (getMenu)
                {
                    //get menu if user authenticated
                    layout = new Layout
                    {
                        Menus = login.GetMenus(this),
                        User = login.GetUser(this)
                    };
                }
                return true;
            }
            return false;
        }


        // GET : Rotation/new
        public ActionResult New()
        {
            if (!CheckLogin(getMenu: true)) return RedirectToAction("Index", "login", new { redirectUrl = "Rotation/New" });

            return View(layout);
        }

        //GET : Rotation/list
        public ActionResult List()
        {
            if (!CheckLogin(getMenu: true))
                return RedirectToAction("Index", "login", new { redirectUrl = "Rotation/List" });
            return View(layout);
        }

        // GET : Rotation
        public ActionResult Index(long id)
        {
            if (!CheckLogin(getMenu: true))
                return RedirectToAction("Index", "login", new { redirectUrl = "Rotation?id="+id });

            layout.Object =  rotationService.GetRotation(id, user.Id);
            if (layout.Object == null) return RedirectToAction("list", "rotation");

            return View(layout);
        }

        /// <summary>
        /// API save Rotation
        /// </summary>
        /// <param name="updatedRotation"></param>
        /// <returns></returns>
        public ActionResult Save(RotationItem updatedRotation)
        {
            CheckLogin();
            updatedRotation.CreatorId = user.Id;
            updatedRotation.UserId = user.Id;
            var data = rotationService.Save(updatedRotation);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// API to start using the rotation and increase the usage of the subscription
        /// </summary>
        /// <param name="rotationId"></param>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        public ActionResult Start(long rotationId, long subscriptionId)
        {
            CheckLogin();
            var data = rotationProcessService.Start(user.Id, rotationId, subscriptionId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// API to obtain rotation that user logged in have related to criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult GetRotations(string criteria, int page, int totalItemPerPage)
        {
            CheckLogin();
            int skip = totalItemPerPage * (page - 1);
            var data = rotationService.GetRotations(user.Id, criteria, skip, totalItemPerPage);
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
        public ActionResult CountRotations(string criteria)
        {
            CheckLogin();
            var data = rotationService.CountRotations(user.Id, criteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// API save workflow, workflow nodes and workflow links
        /// </summary>
        /// <param name="workflowUpdate"></param>
        /// <returns></returns>
        public ActionResult GetUsersWorkflow(long id)
        {
            CheckLogin();
            var data = rotationService.GetUsersWorkflow(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// API save workflow, workflow nodes and workflow links
        /// </summary>
        /// <param name="workflowUpdate"></param>
        /// <returns></returns>
        public ActionResult ProcessActivity(ProcessActivity param, int bit)
        {
            CheckLogin();
            var data = rotationProcessService.ProcessActivity(param, (Constant.EnumActivityAction)bit);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// API save workflow, workflow nodes and workflow links
        /// </summary>
        /// <param name="workflowUpdate"></param>
        /// <returns></returns>
        public ActionResult DeleteRotation(long id)
        {
            CheckLogin();

            var data = rotationService.Delete(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}