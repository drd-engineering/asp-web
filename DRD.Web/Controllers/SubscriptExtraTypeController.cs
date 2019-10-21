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
    public class SubscriptExtraTypeController : Controller
    {

        public ActionResult GetAll()
        {
            //LoginController login = new LoginController();
            //DtoMemberLogin user = login.GetUser(this);
            
            var srv = new SubscriptExtraTypeService();
            var data = srv.GetAll();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetById(int id)
        {
            //LoginController login = new LoginController();
            //DtoMemberLogin user = login.GetUser(this);

            var srv = new SubscriptExtraTypeService();
            var data = srv.GetById(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}