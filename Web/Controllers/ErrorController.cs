using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Core;

using DRD.Domain;

namespace DRD.Web.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult InvalidPage()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            JsonLayout layout = new JsonLayout();
            layout.menus = login.GetMenus(this, 0);
            layout.user = login.GetUser(this);
            layout.activeId = 0;
            return View(layout);
        }

        
    }
}