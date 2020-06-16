﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DRD.Models.API;
using DRD.Models.Custom;
using DRD.Service;
using DRD.Models.View;

namespace DRD.App.Controllers
{
    public class UserController : Controller
    {
        private LoginController login = new LoginController();
        private UserSession user;
        private UserService userService = new UserService();
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

        /// <summary>
        /// This function will return all the user's subscriptions even it personal or company
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult GetAllSubscription()
        {
            Initialize();
            var data = userService.GetAllSubscription(user.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ValidationPassword(long userId, string password)
        {
            Initialize();
            if (userId == 0)
            {
                userId = user.Id;
            }
            var data = userService.ValidationPassword(userId, password);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// API to check is the user that login has company (is the user are an owner)
        /// </summary>
        /// <returns></returns>
        public ActionResult HasCompany()
        {
            InitializeAPI();
            var data = userService.HasCompany(user.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// API to check is the user that login a member of some company
        /// </summary>
        /// <returns></returns>
        public ActionResult IsMemberofCompany()
        {
            InitializeAPI();
            var data = userService.IsMemberofCompany(user.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}