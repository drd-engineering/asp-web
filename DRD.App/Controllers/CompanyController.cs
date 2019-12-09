using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DRD.Models;
using DRD.Models.API;
using DRD.Models.Custom;
using DRD.Models.API;

using DRD.Service;

namespace DRD.App.Controllers
{
    public class CompanyController : Controller
    {
        // GET: Company
        public ActionResult Index()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            // begin decription menu
            UserSession user = login.GetUser(this);
            //var strmenu = login.ManipulateMenu(this, user, mid);
            // end decription menu
            //ViewBag.Title = "Company";
            Layout layout = new Layout();
            //layout.activeId = int.Parse(strmenu);
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = login.GetUser(this);

            CompanyProfile companyProfile = new CompanyProfile();
            
            CompanyService companyService = new CompanyService();
            MemberService memberService = new MemberService();

            

            return View(layout);
        }
        public ActionResult Member(long id)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            // begin decription menu
            UserSession user = login.GetUser(this);
            // end decription menu

            Layout layout = new Layout();
            //layout.activeId = int.Parse(strmenu);
            //layout.key = mid;
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = login.GetUser(this);

            return View(layout);
        }

    }
}