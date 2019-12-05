using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Models.API;
using DRD.Models.API.Register;
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
    }
}