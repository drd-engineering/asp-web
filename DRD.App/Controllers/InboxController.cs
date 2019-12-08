using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DRD.Models;
using DRD.Models.Custom;
using DRD.Models.View;

namespace DRD.App.Controllers
{
    /// <summary>
    ///  TODO UNFINISHED INBOX!! some commented lines are all important
    /// </summary>
    public class InboxController : Controller
    {
        private UserSession getUserLogin()
        {
            LoginController login = new LoginController();
            return login.GetUser(this);
        }


        public ActionResult List()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            // begin decription menu
            UserSession user = login.GetUser(this);
            //var strmenu = login.ManipulateMenu(this, user, mid);
            // end decription menu

            Layout layout = new Layout();
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = login.GetUser(this);

            return View(layout);
        }

        public ActionResult Inbox()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            // begin decription menu
            UserSession user = login.GetUser(this);
           // var strmenu = login.ManipulateSubMenu(this, user, mid);
            // end decription menu

            Rotation product = new Rotation();
            //string[] ids = strmenu.Split(',');
            //if (ids.Length > 1 && !ids[1].Equals("0"))
            //{
               // RotationService psvr = new RotationService();// getUserLogin().AppZone.Code);
                //product = psvr.GetNodeById(int.Parse(ids[1]));
            //}

            Layout layout = new Layout();
            //layout.activeId = int.Parse(ids[0]);
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = login.GetUser(this);
            layout.obj = product;

            //layout.dbmenus = login.GetDashbordMenus(this, layout.activeId);
            return View(layout);
        }
    }
}