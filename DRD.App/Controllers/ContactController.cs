using System.Web.Mvc;
using DRD.Service;
using DRD.Models.View;
using DRD.Models.View.Contact;
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>

        // GET: Contact/GetPersonalContact
        public ActionResult GetPersonalContact(string criteria, int page, int pageSize)
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