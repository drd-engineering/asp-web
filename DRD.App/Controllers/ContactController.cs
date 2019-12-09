using System.Web.Mvc;
using DRD.Service;
using DRD.Models.API;
using DRD.Models.API.Contact;
using DRD.Models;
using System.Collections.Generic;

namespace DRD.App.Controllers
{
    public class ContactController : Controller
    {
        // GET: Contact
        public ActionResult Index()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            Layout layout = new Layout();
            layout.menus = login.GetMenus(this, 0);
            layout.user = login.GetUser(this);
            layout.activeId = 0;

            return View(layout);
        }
            

        // GET: Contact/GetPersonalContact
        public ActionResult GetPersonalContact()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            ContactService ContactServiceInstance = new ContactService();
            
            ContactList data = ContactServiceInstance.GetPersonalContact(login.GetUser(this));
            return  Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: Contact/GetContactFromCompany/Id
        public ActionResult GetContactFromCompany(long CompanyId)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            ContactService ContactServiceInstance = new ContactService();

            ContactList data = ContactServiceInstance.GetContactFromCompany(login.GetUser(this), CompanyId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult GetCompaniesData() {

            LoginController login = new LoginController();
            login.CheckLogin(this);

            ContactService service = new ContactService();
            var data = service.GetListOfCompany(login.GetUser(this));
            

            return Json(data, JsonRequestBehavior.AllowGet);
        }


    }
}