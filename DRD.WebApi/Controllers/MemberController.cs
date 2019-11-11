using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DRD.Service;
using System.Based.Core.Entity;
using DRD.Domain;

namespace DRD.Web.Controllers
{
    public class MemberController : ApiController
    {

        [HttpPost]
        [Route("api/member/login")]
        public DtoMemberLogin Login(string username, string password)
        {
            MemberService srv = new MemberService();
            var data = srv.Login(username, password);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("api/member/logout")]
        public int Logout(long id)
        {
            MemberService srv = new MemberService();
            var data = srv.Logout(id);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/member/changepwd")]
        public int ChangePassword(long memberId, string oldpassword, string newpassword)
        {
            MemberService srv = new MemberService();
            var data = srv.ChangePassword(memberId, oldpassword, newpassword);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/member/resetpwd")]
        public int ResetPassword(string email)
        {
            MemberService srv = new MemberService();
            return srv.ResetPassword(email);
        }

        [HttpPost]
        [Route("api/member/photoprofile")]
        public int UpdatePhotoProfile(long memberId, string fileName)
        {
            MemberService srv = new MemberService();
            var data = srv.UpdatePhotoProfile(memberId, fileName);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/member/image")]
        public int UpdateImage(long memberId, string fileName, int imageType)
        {
            MemberService srv = new MemberService();
            var data = srv.UpdateImage(memberId, fileName, imageType);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/member/ktpno")]
        public int UpdateKtpNo(long memberId, string ktpNo)
        {
            MemberService srv = new MemberService();
            var data = srv.UpdateKtpNo(memberId, ktpNo);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/member/title")]
        public IEnumerable<DtoMemberTitle> GetMemberTitles()
        {
            MemberService srv = new MemberService();
            var data = srv.GetMemberTitles();
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/member/plan")]
        public DtoMemberPlan GetPlan(long memberId)
        {
            MemberPlanService srv = new MemberPlanService();
            var data = srv.GetByMemberId(memberId);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/member/advisor")]
        public IEnumerable<DtoMemberLite> GetAdvisors()
        {
            MemberService srv = new MemberService();
            var data = srv.GetAdvisors();
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/member/invitedcontact")]
        public IEnumerable<DtoMemberLite> GetInvitedContacts(long memberId, string topCriteria, int page, int pageSize)
        {
            MemberService srv = new MemberService();
            var data = srv.GetInvitedContacts(memberId, topCriteria, page, pageSize);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("api/member/validpwd")]
        public bool ValidationPassword(long memberId, string password)
        {
            var srv = new MemberService();
            var data = srv.ValidationPassword(memberId, password);
            return data;
        }
    }
}
