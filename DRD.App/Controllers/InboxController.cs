using System;
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
    public class InboxController : Controller
    {
        LoginController login = new LoginController();
        InboxService inboxService = new InboxService();
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
      
        public ActionResult List()
        {
            Initialize();
            var data = inboxService.GetInboxList(user);
            layout.obj = data;
            return View(layout);
        }


        public ActionResult GetInboxList()
        {
            Initialize();
            var data = inboxService.GetInboxList(user);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Inbox(int id)
        {
            Initialize();

            System.Diagnostics.Debug.WriteLine("INBOX ID " + id);
            
            var data = inboxService.GetInboxItem(user, id);
            layout.dataId = id;
            layout.obj = data;

            return View(layout);
        }

        public ActionResult AddDocument(int id) 
        {
            var data = inboxService.GetInboxItem(user, id);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}