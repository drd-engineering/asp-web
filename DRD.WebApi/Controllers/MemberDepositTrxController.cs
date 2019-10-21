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
    public class MemberDepositTrxController : ApiController
    {
        [HttpPost]
        [Route("api/deposittrx")]
        public IEnumerable<DtoMemberDepositTrx> GetById(long memberId, string topCriteria, int page, int pageSize, string order, string criteria)
        {
            MemberDepositTrxService srv = new MemberDepositTrxService();
            var data = srv.GetById(memberId, page, pageSize, order, criteria);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
        

    }
}
