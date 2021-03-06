﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Core;

using DRD.Domain;

namespace DRD.Web.Controllers
{
    public class NewsVideoController : Controller
    {
        private DtoMemberLogin getUserLogin()
        {
            LoginController login = new LoginController();
            return login.GetUser(this);
        }

        public ActionResult NewsVideoList(string mid)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            // begin decription menu
            DtoMemberLogin user = login.GetUser(this);
            var strmenu = login.ManipulateMenu(this, user, mid);
            // end decription menu

            JsonLayout layout = new JsonLayout();
            layout.activeId = int.Parse(strmenu);
            layout.key = mid;
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = login.GetUser(this);

            return View(layout);
        }

        public ActionResult NewsVideo(string mid)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            // begin decription menu
            DtoMemberLogin user = login.GetUser(this);
            var strmenu = login.ManipulateSubMenu(this, user, mid);
            // end decription menu

            DtoNewsVideo product = new DtoNewsVideo();
            string[] ids = strmenu.Split(',');
            if (ids.Length > 1 && !ids[1].Equals("0"))
            {
                NewsVideoService psvr = new NewsVideoService();
                product = psvr.GetById(int.Parse(ids[1]));
            }

            JsonLayout layout = new JsonLayout();
            layout.activeId = int.Parse(ids[0]);
            layout.key = mid.Split(',')[0];
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = login.GetUser(this);
            layout.obj = product;

            return View(layout);
        }

        public ActionResult GetLiteAll(string topCriteria, int page, int pageSize)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);

            var srv = new NewsVideoService();
            var data = srv.GetLiteAll(user.Id, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLiteAllCount(string topCriteria)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);

            var srv = new NewsVideoService();
            var data = srv.GetLiteAllCount(user.Id, topCriteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLiteAll2(string topCriteria, int page, int pageSize, string criteria)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);

            var srv = new NewsVideoService();
            var data = srv.GetLiteAll(user.Id, topCriteria, page, pageSize, null, criteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLiteAllCount2(string topCriteria, string criteria)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);

            var srv = new NewsVideoService();
            var data = srv.GetLiteAllCount(user.Id, topCriteria, criteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNews(long Id)
        {
            var srv = new NewsVideoService();
            var data = srv.GetById(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(DtoNewsVideo news)
        {
            DtoMemberLogin user = getUserLogin();
            news.CreatorId = user.Id;

            var srv = new NewsVideoService();
            var data = srv.Save(news);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}