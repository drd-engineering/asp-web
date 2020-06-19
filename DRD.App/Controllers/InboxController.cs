    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DRD.Models.API;
using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service;

namespace DRD.App.Controllers
{
    public class InboxController : Controller
    {
        LoginController login = new LoginController();
        InboxService inboxService = new InboxService();
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
        /// Access Page Inbox related to the inbox id that user has
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index(long id)
        {
            if(!Initialize())
                return RedirectToAction("Index", "LoginController");
 
            RotationInboxData product = inboxService.GetInboxItem(id, user.Id);
            //page authorization check if user has no access
            if(product.AccessType.Equals((int)Constant.AccessType.noAccess))
                return RedirectToAction("Index", "Dashboard");
            //user have access
            layout.obj = product;
            return View(layout);
        }

        public ActionResult List()
        {
            if (!Initialize())
                return RedirectToAction("Index", "LoginController");
            return View(layout);
        }

        /// <summary>
        /// API to obtain all user inbox
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult GetInboxList(int page, int pageSize)
        {
            Initialize();
            int skip = pageSize * (page - 1);
            var data = inboxService.GetInboxList(user.Id, skip, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CountAll()
        {
            Initialize();
            var data = inboxService.CountAll(user.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateUnreadInboxCounter()
        {
            Initialize();
            CounterInboxData counter = (CounterInboxData)Session["_COUNTERINBOX_"];
            if (counter == null)
                counter = new CounterInboxData();

            if (user == null)
                return Json(null, JsonRequestBehavior.AllowGet);

            var data = inboxService.CountUnread(user.Id);
            if (counter.New.Unread != data)
            {
                counter.Old.Unread = counter.New.Unread;
                counter.New.Unread = data;
            }
            Session["_COUNTERINBOX_"] = counter;
            return Json(counter, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddDocument(int id) 
        {
            var data = inboxService.GetInboxItemById(id, user);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}