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
    public class NewsVideoController : ApiController
    {
        [HttpPost]
        [Route("api/newsvideo")]
        public DtoNewsVideo GetById(long Id)
        {
            NewsVideoService srv = new NewsVideoService();
            var data = srv.GetById(Id);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("api/newsvideo")]
        public DtoNewsVideo GetById(long Id, long memberId)
        {
            NewsVideoService srv = new NewsVideoService();
            var data = srv.GetById(Id, memberId);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("api/newsvideo")]
        public IEnumerable<DtoNewsVideoLite> GetLiteByTopCriteria(string topCriteria, int page, int pageSize, string order, string criteria)
        {
            NewsVideoService srv = new NewsVideoService();
            var data = srv.GetLiteByTopCriteria(topCriteria, page, pageSize, order, criteria);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("api/newsvideo")]
        public IEnumerable<DtoNewsVideoLite> GetLiteByTopCriteria(long memberId, string topCriteria, int page, int pageSize, string order, string criteria)
        {
            NewsVideoService srv = new NewsVideoService();
            var data = srv.GetLiteByTopCriteria(memberId, topCriteria, page, pageSize, order, criteria);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
