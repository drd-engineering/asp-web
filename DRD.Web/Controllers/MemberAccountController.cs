using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Service;

using DRD.Domain;

namespace DRD.Web.Controllers
{
    public class MemberAccountController : Controller
    {
        private DtoMemberLogin getUserLogin()
        {
            LoginController login = new LoginController();
            return login.GetUser(this);
        }

        public ActionResult MemberAccountList()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            DtoMemberLogin user = login.GetUser(this);
            MemberDepositTrxService msvr = new MemberDepositTrxService();
            JsonLayout layout = new JsonLayout();
            layout.menus = login.GetMenus(this, 0);
            layout.user = user;
            layout.activeId = 0;
            layout.obj = msvr.GetDepositBalance(user.Id);
            return View(layout);
        }

        public ActionResult MemberAccount(string id)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            DtoMemberLogin user = login.GetUser(this);
            MemberAccountService msvr = new MemberAccountService();
            JsonLayout layout = new JsonLayout();
            layout.menus = login.GetMenus(this, 0);
            layout.user = user;
            layout.activeId = 0;
            if (id == null)
                layout.obj = new DtoMemberAccount();
            else
                layout.obj = msvr.GetById(id);
            return View(layout);
        }

        public ActionResult GetLiteAll(string topCriteria, int page, int pageSize)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new MemberAccountService();// getUserLogin().AppZone.Code);
            var data = srv.GetLiteAll(user.Id, topCriteria, page, pageSize, null, null);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLiteAllCount(string topCriteria)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new MemberAccountService();// getUserLogin().AppZone.Code);
            var data = srv.GetLiteAllCount(user.Id, topCriteria, null);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetMemberAccount(long Id)
        {
            var srv = new MemberAccountService();// getUserLogin().AppZone.Code);
            var data = srv.GetById(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(DtoMemberAccount prod)
        {
            DtoMemberLogin user = getUserLogin();
            prod.MemberId = user.Id;
            var srv = new MemberAccountService();// user.AppZone.Code);
            var data = srv.Save(prod);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


    }
}