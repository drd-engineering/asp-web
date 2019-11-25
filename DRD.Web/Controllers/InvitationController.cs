using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Core;

using DRD.Domain;
using System.Based.Core;

namespace DRD.Web.Controllers
{
    public class InvitationController : Controller
    {
        private DtoMemberLogin getUserLogin()
        {
            LoginController login = new LoginController();
            return login.GetUser(this);
        }

        public ActionResult InvitationList(string mid)
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
        public ActionResult Member(string mid)
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

        public ActionResult Invitation(string mid)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            // begin decription menu
            DtoMemberLogin user = login.GetUser(this);
            var strmenu = login.ManipulateSubMenu(this, user, mid);
            // end decription menu

            string[] ids = strmenu.Split(',');
            JsonLayout layout = new JsonLayout();
            layout.activeId = int.Parse(ids[0]);
            layout.key = mid.Split(',')[0];
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = login.GetUser(this);

            return View(layout);
        }

        public ActionResult Agreement(string key)
        {
            JsonInvitationResult result = null;
            string dkey = "";
            try
            {
                dkey = XEncryptionHelper.Decrypt(key);
                string[] values = dkey.Split(',');
                if (values.Length == 2)
                {
                    MemberService ms = new MemberService();
                    result = ms.checkInvitation(long.Parse(values[0]));
                }
            }
            catch (Exception x) { }

            return View(result);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>

        public ActionResult GetInvitedLiteAll(string topCriteria, int page, int pageSize)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new MemberService();
            var data = srv.GetInvitedLiteAll(user.Id, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetInvitedLiteAllCount(string topCriteria)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new MemberService();
            var data = srv.GetInvitedLiteAllCount(user.Id, topCriteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Check(string email)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new MemberService();
            var data = srv.CheckInvitation(user.Id, email);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(string email, int expiryDay)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            string domain = Request.Url.OriginalString.Replace(Request.Url.LocalPath, "");
            var srv = new MemberService();
            var data = srv.SaveInvitation(user.Id, email, expiryDay, domain);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}