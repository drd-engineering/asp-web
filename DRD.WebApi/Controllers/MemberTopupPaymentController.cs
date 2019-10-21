using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DRD.Core;
using System.Based.Core.Entity;
using DRD.Domain;

namespace DRD.Web.Controllers
{
    public class MemberTopupPaymentController : ApiController
    {
        //[HttpPost]
        //[Route("api/memtopuppay")]
        //public IEnumerable<DtoMemberTopupPayment> GetById(long MemberId, int page, int pageSize, string order, string criteria)
        //{
        //    MemberTopupPaymentService srv = new MemberTopupPaymentService();
        //    var data = srv.GetById(MemberId, page, pageSize, order, criteria);
        //    return data;// Json(data, JsonRequestBehavior.AllowGet);
        //}
        //[HttpPost]
        //[Route("api/memtopuppay")]
        //public DtoMemberTopupPayment GetById(long id)
        //{
        //    MemberTopupPaymentService srv = new MemberTopupPaymentService();
        //    var data = srv.GetById(id);
        //    return data;// Json(data, JsonRequestBehavior.AllowGet);
        //}


        [HttpPost]
        [Route("api/memtopuppay/save")]
        public DtoMemberTopupPayment Save(DtoMemberTopupPayment topup)
        {
            MemberTopupPaymentService srv = new MemberTopupPaymentService();
            var data = srv.Save(topup);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/memtopuppay/updatestatus")]
        public int UpdateStatus(long id, string status)
        {
            MemberTopupPaymentService srv = new MemberTopupPaymentService();
            var data = srv.UpdateStatus(id, status);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/memtopuppay/confirmed")]
        public int UpdateConfirmed(long id)
        {
            MemberTopupPaymentService srv = new MemberTopupPaymentService();
            var data = srv.UpdateConfirmed(id);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/memtopuppay/notconfirmed")]
        public int UpdateNotconfirmed(long id)
        {
            MemberTopupPaymentService srv = new MemberTopupPaymentService();
            var data = srv.UpdateNotconfirmed(id);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
