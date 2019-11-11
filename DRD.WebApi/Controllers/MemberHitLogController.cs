using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DRD.Service;
using System.Based.Core.Entity;
using DRD.Domain;
using System.Based.Core;

namespace DRD.Web.Controllers
{
    public class MemberHitLogController : ApiController
    {

        [HttpPost]
        [Route("api/memberhitlog/save")]
        public int Save(long memberId, long dataId,  ConfigConstant.enumDataHit hitType)
        {
            MemberHitLogService srv = new MemberHitLogService();
            var data = srv.Save(memberId, dataId,  hitType);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }


    }
}
