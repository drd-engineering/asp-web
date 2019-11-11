using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Service;

using DRD.Domain;

namespace DRD.Web.Controllers
{
    public class SignedController : Controller
    {
        private DtoMemberLogin getUserLogin()
        {
            LoginController login = new LoginController();
            return login.GetUser(this);
        }


        public ActionResult SignedList(string mid)
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

        public ActionResult Signed(string mid)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            // begin decription menu
            DtoMemberLogin user = login.GetUser(this);
            var strmenu = login.ManipulateSubMenu(this, user, mid);
            // end decription menu

            DtoRotation product = new DtoRotation();
            string[] ids = strmenu.Split(',');
            if (ids.Length > 1 && !ids[1].Equals("0"))
            {
                RotationService psvr = new RotationService();// getUserLogin().AppZone.Code);
                product = psvr.GetNodeById(int.Parse(ids[1]));
            }

            JsonLayout layout = new JsonLayout();
            layout.activeId = int.Parse(ids[0]);
            layout.key = mid.Split(',')[0];
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = login.GetUser(this);
            layout.obj = product;
            
            return View(layout);
        }

        //public ActionResult GetLiteAll(string topCriteria, int page, int pageSize)
        //{
        //    var srv = new NewsService(getUserLogin().AppZone.Code);
        //    var data = srv.GetLiteAll(topCriteria, page, pageSize);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetLiteAllCount(string topCriteria)
        //{
        //    var srv = new NewsService(getUserLogin().AppZone.Code);
        //    var data = srv.GetLiteAllCount(topCriteria);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetLiteAll2(string topCriteria, int page, int pageSize, string criteria)
        //{
        //    var srv = new NewsService(getUserLogin().AppZone.Code);
        //    var data = srv.GetLiteAll(topCriteria, page, pageSize, null, criteria);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetLiteAllCount2(string topCriteria, string criteria)
        //{
        //    var srv = new NewsService(getUserLogin().AppZone.Code);
        //    var data = srv.GetLiteAllCount(topCriteria, criteria);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetNews(long Id)
        //{
        //    var srv = new NewsService(getUserLogin().AppZone.Code);
        //    var data = srv.GetById(Id);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult Save(DtoNews news)
        //{
        //    JsonUser user = getUserLogin();
        //    news.AppZone = user.AppZone.Code;
        //    news.UserId = user.UserId;

        //    var srv = new NewsService(user.AppZone.Code);
        //    var data = srv.Save(news);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetMaster()
        //{
        //    var srv = new NewsService(getUserLogin().AppZone.Code);
        //    var data = srv.GetMaster();
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}


    }
}