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
    public class StampController : ApiController
    {
        [HttpPost]
        [Route("api/Stamp")]
        public DtoStamp GetById(int Id)
        {
            StampService srv = new StampService();
            var data = srv.GetById(Id);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("api/Stamp/lite")]
        public IEnumerable<DtoStampLite> GetLite(long userId, string topCriteria, int page, int pageSize)
        {
            StampService srv = new StampService();
            var data = srv.GetLite(userId, topCriteria, page, pageSize, null, null);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
