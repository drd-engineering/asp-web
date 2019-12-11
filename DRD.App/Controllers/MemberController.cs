using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Models;
using DRD.Models.API;
using DRD.Models.Custom;
using DRD.Models.View.Member;

using DRD.Service;

namespace DRD.App.Controllers
{
    public class MemberController : Controller
    {

        /// <summary>
        /// POST: Member / Getlitegroupall
        /// </summary>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult GetLiteGroupAll(string topCriteria, int page, int pageSize)
        {
            LoginController login = new LoginController();
            UserSession user = login.GetUser(this);
            var service = new MemberService();
            var data = service.GetLiteGroupAll(user.Id, topCriteria, page, pageSize, null);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAcceptedMember(long companyId)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);
            UserSession user = login.GetUser(this);

            MemberService memberService = new MemberService();
            MemberList data = memberService.getAllAcceptedMember(companyId);

            System.Diagnostics.Debug.WriteLine("COUNT ACCEPTED  : " );
            
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetWaitingMember(long companyId)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);
            UserSession user = login.GetUser(this);

            MemberService memberService = new MemberService();
            MemberList data = memberService.getAllWaitingMember(companyId);
                        
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult addMemberToCompany(long companyId, long userId)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);
            UserSession user = login.GetUser(this);

            MemberService memberService = new MemberService();
            bool data = memberService.addMemberToCompany(userId,companyId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult addCompanyToMember(long companyId, long userId)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);
            UserSession user = login.GetUser(this);

            MemberService memberService = new MemberService();
            bool data = memberService.addCompanyToMember(userId, companyId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}