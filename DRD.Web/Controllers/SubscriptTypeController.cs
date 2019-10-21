using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Core;

using DRD.Domain;
using System.Dynamic;

namespace DRD.Web.Controllers
{
    public class SubscriptTypeController : Controller
    {

        public ActionResult GetAllExcluded()
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            
            var srv = new SubscriptTypeService();
            var data = srv.GetAllExcluded(user.SubscriptTypeId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetById()
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);

            var srv = new SubscriptTypeService();
            var data = srv.GetById(user.SubscriptTypeId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}