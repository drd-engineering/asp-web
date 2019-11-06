using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Core;

using DRD.Domain;
using System.Net;
using System.IO;

namespace DRD.Web.Controllers
{
    public class EmailHtmlController : Controller
    {

        public ActionResult InboxNotif(long memberId)
        {
            MemberService msvr = new MemberService();
            JsonLayout layout = new JsonLayout();
            layout.user = msvr.GetById(memberId);

            return View(layout);
        }
        public ActionResult Registration(long memberId)
        {
            MemberService msvr = new MemberService();
            JsonLayout layout = new JsonLayout();
            layout.user = msvr.GetById(memberId, true);
            return View(layout);
        }

        
    }
}