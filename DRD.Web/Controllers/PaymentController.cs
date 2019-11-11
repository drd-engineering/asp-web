using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Service;

using DRD.Domain;

namespace DRD.Web.Controllers
{
    public class PaymentController : Controller
    {
        private DtoMemberLogin getUserLogin()
        {
            LoginController login = new LoginController();
            return login.GetUser(this);
        }


        public ActionResult Payment(string id)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            DtoMemberLogin user = login.GetUser(this);
            MemberTopupDepositService msvr = new MemberTopupDepositService();
            JsonLayout layout = new JsonLayout();
            layout.menus = login.GetMenus(this, 0);
            layout.user = user;
            layout.activeId = 0;
            layout.obj = msvr.GetById(id);
            return View(layout);
        }
        public ActionResult Confirmation(string id)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);
            string[] ids = id.Split(',');
            string realId = ids[0];
            string cbId = ids[1];

            DtoMemberLogin user = login.GetUser(this);
            MemberTopupDepositService msvr = new MemberTopupDepositService();
            JsonLayout layout = new JsonLayout();
            layout.menus = login.GetMenus(this, 0);
            layout.user = user;
            layout.activeId = 0;
            layout.obj = msvr.GetById(realId, cbId);
            layout.key = id;
            return View(layout);
        }
        public ActionResult Aggregator(string id)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);
            string[] ids = id.Split(',');
            string realId = ids[0];
            string cbId = ids[1];

            DtoMemberLogin user = login.GetUser(this);
            MemberTopupDepositService msvr = new MemberTopupDepositService();
            JsonLayout layout = new JsonLayout();
            layout.menus = login.GetMenus(this, 0);
            layout.user = user;
            layout.activeId = 0;
            layout.obj = msvr.GetById(realId, cbId);
            layout.url = Request.Url.OriginalString.Replace(Request.Url.PathAndQuery, "") + "/Faspay/Payment?key=" + realId + ",MTU," + cbId;

            return View(layout);

        }

        public ActionResult Aggregator2(string id)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);
            string[] ids = id.Split(',');
            string realId = ids[0];
            string cbId = ids[1];

            DtoMemberLogin user = login.GetUser(this);
            MemberTopupDepositService msvr = new MemberTopupDepositService();
            JsonLayout layout = new JsonLayout();
            layout.menus = login.GetMenus(this, 0);
            layout.user = user;
            layout.activeId = 0;
            layout.obj = msvr.GetById(realId, cbId);
            layout.url = Request.Url.OriginalString.Replace(Request.Url.PathAndQuery, "") + "/Faspay/Payment?key=" + realId + ",MTU," + cbId;


            return View(layout);

        }

        public ActionResult XAggregator(string id)
        {
            string[] ids = id.Split(',');
            string realId = ids[0];
            string cbId = ids[1];

            MemberTopupDepositService mtdsvr = new MemberTopupDepositService();
            var xobj = mtdsvr.GetById(realId, cbId); ;

            MemberService msvr = new MemberService();
            DtoMemberLogin user = msvr.GetById(xobj.MemberId);
            LoginController login = new LoginController();
            login.SetLogin(this, user);
            
            JsonLayout layout = new JsonLayout();

            layout.menus = login.GetMenus(this, 0);
            layout.user = user;
            layout.activeId = 0;
            layout.obj = xobj;
            layout.url = Request.Url.OriginalString.Replace(Request.Url.PathAndQuery, "") + "/Faspay/Payment?key=" + realId + ",MTU," + cbId;


            return View(layout);

        }

        public ActionResult Gerai(string id)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);
            string[] ids = id.Split(',');
            string realId = ids[0];
            string cbId = ids[1];

            DtoMemberLogin user = login.GetUser(this);
            MemberTopupDepositService msvr = new MemberTopupDepositService();
            JsonLayout layout = new JsonLayout();
            layout.menus = login.GetMenus(this, 0);
            layout.user = user;
            layout.activeId = 0;
            layout.obj = msvr.GetById(realId, cbId);
            return View(layout);
        }
        public ActionResult Deposit(string id)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);
            string[] ids = id.Split(',');
            string realId = ids[0];
            string cbId = ids[1];

            DtoMemberLogin user = login.GetUser(this);
            MemberTopupDepositService msvr = new MemberTopupDepositService();
            JsonLayout layout = new JsonLayout();
            layout.menus = login.GetMenus(this, 0);
            layout.user = user;
            layout.activeId = 0;
            layout.obj = msvr.GetById(realId, cbId);
            return View(layout);
        }
        public ActionResult Voucher(string id)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);
            string[] ids = id.Split(',');
            string realId = ids[0];
            string cbId = ids[1];

            DtoMemberLogin user = login.GetUser(this);
            MemberTopupDepositService msvr = new MemberTopupDepositService();
            JsonLayout layout = new JsonLayout();
            layout.menus = login.GetMenus(this, 0);
            layout.user = user;
            layout.activeId = 0;
            layout.obj = msvr.GetById(realId, cbId);
            layout.key = id;
            return View(layout);
        }

        public DtoMemberTopupPayment SaveConfirmation(string id)
        {
            MemberTopupPaymentService paysvr = new MemberTopupPaymentService();
            string[] ids = id.Split(',');
            string realId = ids[0];
            string cbId = ids[1];
            DtoMemberTopupPayment pay = new DtoMemberTopupPayment();
            return paysvr.Save(pay);
        }

    }
}