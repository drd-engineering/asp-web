using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Core;

using DRD.Domain;

namespace DRD.Web.Controllers
{
    public class TopupDepositController : Controller
    {
        private DtoMemberLogin getUserLogin()
        {
            LoginController login = new LoginController();
            return login.GetUser(this);
        }

        public ActionResult TopupList()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            DtoMemberLogin user = login.GetUser(this);
            //MemberService msvr = new MemberService();
            JsonLayout layout = new JsonLayout();
            layout.menus = login.GetMenus(this, 0);
            layout.user = user;
            layout.activeId = 0;
            //layout.obj = msvr.GetById(user.Id, user.Id);
            return View(layout);
        }

        public ActionResult Topup()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            DtoMemberLogin user = login.GetUser(this);
            //MemberService msvr = new MemberService();
            ApplConfigService appsvr = new ApplConfigService();
            var minTopup = appsvr.GetValue("MIN_TOPUP");
            JsonLayout layout = new JsonLayout();
            layout.menus = login.GetMenus(this, 0);
            layout.user = user;
            layout.activeId = 0;
            layout.obj = decimal.Parse(minTopup);
            return View(layout);
        }

        public ActionResult GetLiteAll(string topCriteria, int page, int pageSize)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new MemberTopupDepositService();// getUserLogin().AppZone.Code);
            var data = srv.GetLiteAll(user.Id, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLiteAllCount(string topCriteria)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new MemberTopupDepositService();// getUserLogin().AppZone.Code);
            var data = srv.GetLiteAllCount(user.Id, topCriteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTopup(long Id)
        {
            var srv = new MemberTopupDepositService();// getUserLogin().AppZone.Code);
            var data = srv.GetById(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(DtoMemberTopupDeposit prod)
        {
            DtoMemberLogin user = getUserLogin();
            var srv = new MemberTopupDepositService();
            prod.MemberId = user.Id;
            var data = srv.Save(prod);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


    }
}