using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Models;
using DRD.Models.Custom;
using DRD.Models.View;

using DRD.Service;

namespace DRD.App.Controllers
{
    public class MemberController : Controller
    {
        LoginController login = new LoginController();
        MemberService memberService = new MemberService();

        UserSession user;
        Layout layout = new Layout();

        public void Initialize()
        {
            user = login.GetUser(this);
            login.CheckLogin(this);
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = login.GetUser(this);
        }
        public void InitializeAPI()
        {
            user = login.GetUser(this);
            login.CheckLogin(this);
        }
        /// <summary>
        /// Request to get member data based on search query and page list requested
        /// </summary>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult FindMembers(string topCriteria, int page, int pageSize)
        {
            InitializeAPI();
            var data = memberService.FindMembers(user.Id, topCriteria, page, pageSize, null);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Request to get all the member listed in search query, to help paging for searching member
        /// </summary>
        /// <param name="topCriteria"></param>
        /// <returns></returns>
        public ActionResult FindMembersCountAll(string topCriteria)
        {
            InitializeAPI();
            var data = memberService.FindMembersCountAll(user.Id, topCriteria);
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
            InitializeAPI();
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
            InitializeAPI();
            var data = memberService.FindMembersRotationCountAll(user.Id, topCriteria, rotationId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAcceptedMember(long companyId)
        {
            InitializeAPI();
            
            MemberList data = memberService.getAcceptedMember(companyId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAcceptedMemberOrAdmin(long companyId, bool isAdmin)
        {
            InitializeAPI();
            MemberList data = memberService.getAcceptedMember(companyId, isAdmin);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetWaitingMember(long companyId)
        {
            InitializeAPI();

            MemberList data = memberService.getWaitingMember(companyId);
                        
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult addMemberToCompany(long companyId, long userId)
        {
            InitializeAPI();

            bool data = memberService.addMemberToCompany(userId,companyId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(long memberId)
        {
            InitializeAPI();

            long data = memberService.Delete(memberId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult addCompanyToMember(long companyId, long userId)
        {
            InitializeAPI();

            bool data = memberService.addCompanyToMember(userId, companyId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BecomeAdmin(long companyId, ICollection<MemberItem> adminCandidate)
        {
            InitializeAPI();

            var data = memberService.BecomeAdmin(companyId, adminCandidate);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BecomeMember(long companyId, ICollection<MemberItem> memberCandidate)
        {
            InitializeAPI();

            var data = memberService.BecomeMember(companyId, memberCandidate);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}