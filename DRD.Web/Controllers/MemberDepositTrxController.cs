using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Service;

using DRD.Domain;

namespace DRD.Web.Controllers
{
    public class MemberDepositTrxController : Controller
    {
        public ActionResult GetById(int page, int pageSize)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);

            MemberDepositTrxService srv = new MemberDepositTrxService();
            var data = srv.GetById(user.Id, page, pageSize, null, null);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetByIdCount()
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new MemberDepositTrxService();//
            var data = srv.GetByQueryCount(user.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
