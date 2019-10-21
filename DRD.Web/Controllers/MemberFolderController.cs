using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Core;

using DRD.Domain;

namespace DRD.Web.Controllers
{
    public class MemberFolderController : Controller
    {
        private DtoMemberLogin getUserLogin()
        {
            LoginController login = new LoginController();
            return login.GetUser(this);
        }

        public ActionResult Save(DtoMemberFolder folder)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            folder.MemberId = user.Id;
            var srv = new MemberFolderService();
            var data = srv.Save(folder);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Remove(long folderId)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new MemberFolderService();
            var data = srv.Remove(user.Id, folderId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAll(long excludeId)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new MemberFolderService();
            var data = srv.GetAll(user.Id, excludeId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDashboard()
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new MemberFolderService();
            var data = srv.GetDashboard(user.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        
    }
}