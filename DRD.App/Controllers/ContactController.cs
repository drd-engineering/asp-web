using System.Web.Mvc;
using DRD.Service;
using DRD.Models.View;
using DRD.Models.Custom;
using System.Collections.Generic;

namespace DRD.App.Controllers
{
    public class ContactController : Controller
    {
        LoginController login = new LoginController();
        ContactService contactService = new ContactService();
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

        // GET: Contact
        public ActionResult Index()
        {
            Initialize();
            layout.menus = login.GetMenus(this, 0);
            layout.activeId = 0;
            return RedirectToAction("Index", "Dashboard");
            //return View(layout);
        }

        // GET: Contact/GetPersonalContact
        public ActionResult GetPersonalContact(string topCriteria, int page, int pageSize)
        {
            System.Diagnostics.Debug.WriteLine("GET PERSONAL CONTACT:");
            InitializeAPI();
            
            ContactList data = contactService.GetPersonalContact(login.GetUser(this), topCriteria, page, pageSize);
            return  Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: Contact/GetContactFromCompany/Id
        public ActionResult GetContactFromCompany(long CompanyId, string topCriteria, int page, int pageSize)
        {
            System.Diagnostics.Debug.WriteLine("GET CONTACT FROM COMPANY: "+ CompanyId);
            InitializeAPI();
            ContactList data = contactService.GetContactFromCompany(login.GetUser(this), CompanyId, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCompaniesData() {
            InitializeAPI();
            var data = contactService.GetListOfCompany(login.GetUser(this));
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}