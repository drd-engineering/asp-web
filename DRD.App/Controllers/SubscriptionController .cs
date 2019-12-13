using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Mvc;

using DRD.Models;
using DRD.Models.API;
using DRD.Models.API.Register;
using DRD.Models.Custom;
using DRD.Models.View;

using DRD.Service;

namespace DRD.App.Controllers
{
    public class SubscriptionController : Controller
    {
        private LoginController login = new LoginController();
        private UserSession user = new UserSession();
        private Layout layout = new Layout();

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

        public ActionResult GetSubscriptionList()
        {
            Initialize();

            SubscriptionService subscriptionService = new SubscriptionService();
            List<BusinessSubscriptionItem> data = subscriptionService.getBusinessSubscriptionByUser(user.Id);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}