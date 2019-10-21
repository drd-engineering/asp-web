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
    public class PodCastController : ApiController
    {
        [HttpPost]
        [Route("api/podcast")]
        public DtoPodCast GetById(long Id)
        {
            PodCastService srv = new PodCastService();
            var data = srv.GetById(Id);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("api/podcast")]
        public DtoPodCast GetById(long Id, long memberId)
        {
            PodCastService srv = new PodCastService();
            var data = srv.GetById(Id, memberId);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("api/podcast")]
        public IEnumerable<DtoPodCastLite> GetLiteByTopCriteria(string topCriteria, int page, int pageSize, string order, string criteria)
        {
            PodCastService srv = new PodCastService();
            var data = srv.GetLiteByTopCriteria(topCriteria, page, pageSize, order, criteria);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("api/podcast")]
        public IEnumerable<DtoPodCastLite> GetLiteByTopCriteria(long memberId, string topCriteria, int page, int pageSize, string order, string criteria)
        {
            PodCastService srv = new PodCastService();
            var data = srv.GetLiteByTopCriteria(memberId, topCriteria, page, pageSize, order, criteria);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}
