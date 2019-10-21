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
    public class InvitationController : ApiController
    {

        [HttpPost]
        [Route("api/invitation/list")]
        public IEnumerable<DtoMemberInvited> GetInvitedLiteAll(long userId, string topCriteria, int page, int pageSize)
        {
            var srv = new MemberService();
            var data = srv.GetInvitedLiteAll(userId, topCriteria, page, pageSize, null, null);
            return data;
        }
        [HttpPost]
        [Route("api/invitation/invitation")]
        public IEnumerable<DtoMemberInvited> GetInvitationLiteAll(long userId, string topCriteria, int page, int pageSize)
        {
            var srv = new MemberService();
            var data = srv.GetInvitationLiteAll(userId, topCriteria, page, pageSize);
            return data;
        }
        [HttpPost]
        [Route("api/invitation/accept")]
        public JsonInvitationResult checkInvitation(long id)
        {
            var srv = new MemberService();
            var data = srv.checkInvitation(id);
            return data;
        }
        [HttpPost]
        [Route("api/invitation/check")]
        public DtoMemberLogin Check(long userId, string email)
        {
            var srv = new MemberService();
            var data = srv.CheckInvitation(userId, email);
            return data;
        }
        [HttpPost]
        [Route("api/invitation/save")]
        public long Save(long userId, string email, int expiryDay, string domain)
        {
            //string domain = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            var srv = new MemberService();
            var data = srv.SaveInvitation(userId, email, expiryDay, domain);
            return data;
        }
    }
}
