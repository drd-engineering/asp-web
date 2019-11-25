﻿using System;
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
    public class ProfileController : Controller
    {
        // GET: Profile
        public ActionResult Index()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            return View();
        }

        // GET: Profile/MemberList/
        public ActionResult UserList(string mid)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            // begin decription menu
            UserSession userSession = login.GetUser(this);
            var strmenu = login.ManipulateMenu(this, userSession, mid);
            // end decription menu

            Layout layout = new Layout();
            layout.activeId = int.Parse(strmenu);
            layout.key = mid;
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = login.GetUser(this);

            return View(layout);
        }

        public ActionResult User(string mid)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            // begin decription menu
            UserSession userSession = login.GetUser(this);
            var strmenu = login.ManipulateSubMenu(this, userSession, mid);
            // end decription menu

            User user = new User();
            string[] ids = strmenu.Split(',');
            if (ids.Length > 1 && !ids[1].Equals("0"))
            {
                UserService userService = new UserService();
                user = userService.GetById(int.Parse(ids[1]), userSession.Id);
            }

            Layout layout = new Layout();
            layout.activeId = int.Parse(ids[0]);
            layout.key = mid.Split(',')[0];
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = userSession;
            layout.obj = user;

            return View(layout);
        }
        public UserSession GetUserLogin()
        {
            LoginController login = new LoginController();
            return login.GetUser(this);
        }
    }
}