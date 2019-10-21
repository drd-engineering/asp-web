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
    public class MemberAccountController : ApiController
    {
        [HttpPost]
        [Route("api/memberacc/member")]
        public IEnumerable<DtoMemberAccount> GetByMemberId(long id)
        {
            MemberAccountService srv = new MemberAccountService();
            var data = srv.GetByMemberId(id);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("api/memberacc")]
        public DtoMemberAccount GetById(long Id)
        {
            MemberAccountService srv = new MemberAccountService();
            var data = srv.GetById(Id);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("api/memberacc/save")]
        public DtoMemberAccount Save(DtoMemberAccount memberacc)
        {
            MemberAccountService srv = new MemberAccountService();
            var data = srv.Save(memberacc);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //[Route("api/memberacc/update")]
        //public int Update(DtoMemberAccount memberacc)
        //{
        //    MemberAccountService srv = new MemberAccountService();
        //    var data = srv.Update(memberacc);
        //    return data; // Json(data, JsonRequestBehavior.AllowGet);
        //}
    }
}
