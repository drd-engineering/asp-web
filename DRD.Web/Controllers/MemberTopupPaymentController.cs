using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DRD.Service;

using DRD.Domain;

namespace DRD.Web.Controllers
{
    public class MemberTopupPaymentController : Controller
    {
        
        public ActionResult Save(DtoMemberTopupPayment topup)
        {
            MemberTopupPaymentService srv = new MemberTopupPaymentService();
            var data = srv.Save(topup);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public int UpdateStatus(long id, string status)
        {
            MemberTopupPaymentService srv = new MemberTopupPaymentService();
            var data = srv.UpdateStatus(id, status);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        public int UpdateConfirmed(long id)
        {
            MemberTopupPaymentService srv = new MemberTopupPaymentService();
            var data = srv.UpdateConfirmed(id);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        public int UpdateNotconfirmed(long id)
        {
            MemberTopupPaymentService srv = new MemberTopupPaymentService();
            var data = srv.UpdateNotconfirmed(id);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
