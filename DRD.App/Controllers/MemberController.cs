using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Models;
using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Models.View.Member;

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
        /// POST: Member / FindMembers
        /// </summary>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult FindMembers(string topCriteria, int page, int pageSize)
        {
            InitializeAPI();
            var data = service.FindMembers(user.Id, topCriteria, page, pageSize, null);return Json(data, JsonRequestBehavior.AllowGet);
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
            System.Diagnostics.Debug.WriteLine("USER MEMBER :: " + companyId +"  "+isAdmin);

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

        public ActionResult addCompanyToMember(long companyId, long userId)
        {
            InitializeAPI();

            bool data = memberService.addCompanyToMember(userId, companyId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}