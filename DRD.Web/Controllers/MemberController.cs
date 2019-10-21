using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Core;

using DRD.Domain;

namespace DRD.Web.Controllers
{
    public class MemberController : Controller
    {
        private DtoMemberLogin getUserLogin()
        {
            LoginController login = new LoginController();
            return login.GetUser(this);
        }

        public ActionResult MemberList(string mid)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            // begin decription menu
            DtoMemberLogin user = login.GetUser(this);
            var strmenu = login.ManipulateMenu(this, user, mid);
            // end decription menu

            JsonLayout layout = new JsonLayout();
            layout.activeId = int.Parse(strmenu);
            layout.key = mid;
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = login.GetUser(this);
            
            return View(layout);
        }

        public ActionResult Member(string mid)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            // begin decription menu
            DtoMemberLogin user = login.GetUser(this);
            var strmenu = login.ManipulateSubMenu(this, user, mid);
            // end decription menu

            DtoMemberLogin memlogin = login.GetUser(this);
            DtoMember product = new DtoMember();
            string[] ids = strmenu.Split(',');
            if (ids.Length > 1 && !ids[1].Equals("0"))
            {
                MemberService psvr = new MemberService();
                product = psvr.GetById(int.Parse(ids[1]), memlogin.Id);
            }

            JsonLayout layout = new JsonLayout();
            layout.activeId = int.Parse(ids[0]);
            layout.key = mid.Split(',')[0];
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = memlogin;
            layout.obj = product;
            
            return View(layout);
        }

        public ActionResult MyProfile()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            DtoMemberLogin user = login.GetUser(this);
            MemberService msvr = new MemberService();
            JsonLayout layout = new JsonLayout();
            layout.menus = login.GetMenus(this, 0);
            layout.user = user;
            layout.activeId = 0;
            layout.obj = msvr.GetById(user.Id, user.Id);
            return View(layout);
        }

        public ActionResult MyPlan()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            DtoMemberLogin user = login.GetUser(this);
            MemberPlanService msvr = new MemberPlanService();
            JsonLayout layout = new JsonLayout();
            layout.menus = login.GetMenus(this, 0);
            layout.user = user;
            layout.activeId = 0;
            layout.obj = msvr.GetByMemberId(user.Id);
            return View(layout);
        }
        
        public ActionResult MyDeposit()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            DtoMemberLogin user = login.GetUser(this);
            MemberDepositTrxService msvr = new MemberDepositTrxService();
            JsonLayout layout = new JsonLayout();
            layout.menus = login.GetMenus(this, 0);
            layout.user = user;
            layout.activeId = 0;
            layout.obj = msvr.GetDepositBalance(user.Id);
            return View(layout);
        }
        public ActionResult UserPermissionList(string mid)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            JsonLayout layout = new JsonLayout();
            layout.activeId = int.Parse(mid);
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = login.GetUser(this);

            UserMenuService umsvr = new UserMenuService();
            if (!umsvr.ValidGroupMenu(layout.user.UserGroup, layout.activeId))
                Response.Redirect("/");

            return View(layout);
        }

        public ActionResult EncryptAllPassword()
        {
            var srv = new MemberService();
            var data = srv.EncryptAllPassword();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// 
        public ActionResult GetPlan()
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new MemberPlanService();
            var data = srv.GetByMemberId(user.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLiteAll(string topCriteria, int page, int pageSize)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new MemberService();
            var data = srv.GetLiteAll((long)user.CompanyId, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLiteAllCount(string topCriteria)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new MemberService();
            var data = srv.GetLiteAllCount((long)user.CompanyId, topCriteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult GetLiteAll2(string topCriteria, int page, int pageSize)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new MemberService();
            var data = srv.GetLiteAll(user.Id, topCriteria, page, pageSize, null);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLiteAllCount2(string topCriteria)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new MemberService();
            var data = srv.GetLiteAllCount(user.Id, topCriteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult GetLiteGroupAll(string topCriteria, int page, int pageSize)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new MemberService();
            var data = srv.GetLiteGroupAll(user.Id, topCriteria, page, pageSize, null);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLiteGroupAllCount(string topCriteria)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new MemberService();
            var data = srv.GetLiteGroupAllCount(user.Id, topCriteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>

        public ActionResult GetChatContacts(string topCriteria, int page, int pageSize)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new MemberService();
            var data = srv.GetChatContacts(user.Id, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetChatContactsCount(string topCriteria)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new MemberService();
            var data = srv.GetChatContactsCount(user.Id, topCriteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMaster()
        {
            var srv = new MemberService();
            var data = srv.GetMaster();
            return Json(data, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="prod"></param>
        /// <returns></returns>

        public ActionResult Save(DtoMember prod)
        {
            DtoMemberLogin user = getUserLogin();
            prod.UserId = user.Email;
            prod.CompanyId = user.CompanyId;
            var srv = new MemberService();
            var data = srv.Save(prod);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveProfile(DtoMember prod)
        {
            DtoMemberLogin user = getUserLogin();
            prod.UserId = user.Email;
            prod.CompanyId = user.CompanyId;
            var srv = new MemberService();
            var mem = srv.GetById(user.Id, user.Id);
            prod.IsActive = mem.IsActive;
            prod.UserGroup = mem.UserGroup;
            var data = srv.Save(prod);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Registration(JsonMemberRegister member)
        {
            var srv = new MemberService();
            var data = srv.SaveRegistration(member);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpgradePlan(int planId)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            user.SubscriptTypeId = planId;
            login.SetLogin(this, user);

            var srv = new MemberService();
            var data = srv.UpgradePlan(user.Id, planId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpgradeExtraPlan(int planId)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            user.SubscriptTypeId = planId;
            login.SetLogin(this, user);

            var srv = new MemberService();
            var data = srv.UpgradeExtraPlan(user.Id, planId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpgradeDrDrivePlan(int driveId)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            //user.SubscriptType. = planId;
            login.SetLogin(this, user);

            var srv = new MemberService();
            var data = srv.UpgradeDrDrivePlan(user.Id, driveId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpgradePlanRequest(int planId)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);

            var srv = new MemberService();
            var data = srv.UpgradePlanRequest(user.Id, planId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpgradeNode(int nodeQty, int rotationQty, int sizeQty, int drdriveQty)
        {
            var srv = new MemberService();
            DtoMemberLogin user = getUserLogin();
            var data = srv.UpgradeNode(user.Id, nodeQty, rotationQty, sizeQty, drdriveQty);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ValidationPassword(long memberId, string password)
        {
            var srv = new MemberService();
            if (memberId == 0)
            {
                DtoMemberLogin user = getUserLogin();
                memberId = user.Id;
            }
            var data = srv.ValidationPassword(memberId, password);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}