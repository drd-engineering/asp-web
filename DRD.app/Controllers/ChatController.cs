using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Core;

using DRD.Domain;

namespace DRD.Web.Controllers
{
    public class ChatController : Controller
    {
        private DtoMemberLogin getUserLogin()
        {
            LoginController login = new LoginController();
            return login.GetUser(this);
        }

        public ActionResult Chat()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            DtoMemberLogin user = login.GetUser(this);
            MemberService msvr = new MemberService();
            JsonLayout layout = new JsonLayout();
            layout.menus = login.GetMenus(this, 0);
            layout.user = user;
            layout.activeId = 0;
            layout.obj = msvr.GetInvitedContacts(user.Id, null,1,100);
            return View(layout);
        }

    }
}