using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Core;

using DRD.Domain;


namespace DRD.Web.Controllers
{
    public class BankController : Controller
    {
        public ActionResult GetLiteAll(string topCriteria, int page, int pageSize)
        {
            var srv = new BankService();// getUserLogin().AppZone.Code);
            var data = srv.GetLiteAll(topCriteria, page, pageSize, null, null);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLiteAllCount(string topCriteria)
        {
            var srv = new BankService();// getUserLogin().AppZone.Code);
            var data = srv.GetLiteAllCount(topCriteria, null);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}
