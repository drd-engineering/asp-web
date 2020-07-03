using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace DRD.App.Controllers
{
    public class MemberController : Controller
    {
        LoginController login;
        MemberService memberService;
        UserSession user;

        private bool CheckLogin(bool getMenu = false)
        {
            login = new LoginController();
            if (login.CheckLogin(this))
            {
                user = login.GetUser(this);
                memberService = new MemberService();
                user = login.GetUser(this);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Request to get member data based on search query and page list requested
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="page"></param>
        /// <param name="totalItemPerPage"></param>
        /// <returns></returns>
        public ActionResult GetMembers(string criteria, int page, int totalItemPerPage)
        {
            CheckLogin();
            var data = memberService.GetMembers(user.Id, criteria, page, totalItemPerPage, null);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Request to get all the member listed in search query, to help paging for searching member
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public ActionResult CountMembers(string criteria)
        {
            CheckLogin();
            var data = memberService.CountMembers(user.Id, criteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Request to get member data that are participate in Rotation based on search query and page list requested
        /// </summary>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult FindMembersRotation(string topCriteria, int page, int pageSize, long rotationId)
        {
            CheckLogin();
            var data = memberService.FindMembersRotation(user.Id, topCriteria, page, pageSize, rotationId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Request to get count of all the member that are participate in Rotation listed in search query, to help paging for searching member
        /// </summary>
        /// <param name="topCriteria"></param>
        /// <returns></returns>
        public ActionResult FindMembersRotationCountAll(string topCriteria, long rotationId)
        {
            CheckLogin();
            var data = memberService.FindMembersRotationCountAll(user.Id, topCriteria, rotationId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AcceptInvitation(long memberId)
        {
            CheckLogin();
            if (user == null)
            {
                return RedirectToAction("Index", "LoginController");
            }
            var data = memberService.AcceptInvitation(user.Id, memberId);
            if (!data) return RedirectToAction("Index", "LoginController");
            
            return RedirectToAction("Index", "Setting");
        }
        public ActionResult GetAcceptedMember(long companyId)
        {
            CheckLogin();

            MemberList data = memberService.GetAcceptedMember(companyId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAcceptedMemberOrAdmin(long companyId, bool isAdmin)
        {
            CheckLogin();
            MemberList data = memberService.GetAcceptedMember(companyId, isAdmin);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetWaitingMember(long companyId)
        {
            CheckLogin();

            MemberList data = memberService.GetWaitingMember(companyId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddMemberToCompany(long companyId, long userId)
        {
            CheckLogin();

            long data = memberService.AddMemberToCompany(userId, companyId);
            bool returnValue = false;
            if (data > 0)
                returnValue = true;
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(long memberId)
        {
            CheckLogin();

            long data = memberService.Delete(memberId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BecomeAdmin(long companyId, ICollection<MemberItem> adminCandidate)
        {
            CheckLogin();

            var data = memberService.BecomeAdmin(companyId, adminCandidate);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BecomeMember(long companyId, ICollection<MemberItem> memberCandidate)
        {
            CheckLogin();

            var data = memberService.BecomeMember(companyId, memberCandidate);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}