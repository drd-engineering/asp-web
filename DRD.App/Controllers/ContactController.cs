using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service;
using System.Web.Mvc;

namespace DRD.App.Controllers
{
    public class ContactController : Controller
    {
        private LoginController login;
        private ContactService contactService;
        private UserSession user;
        private Layout layout;

        private bool CheckLogin(bool getMenu = false)
        {
            login = new LoginController();
            if (login.CheckLogin(this))
            {
                user = login.GetUser(this);
                contactService = new ContactService();
                if (getMenu)
                {
                    layout = new Layout
                    {
                        Menus = login.GetMenus(this),
                        User = login.GetUser(this)
                    };
                }
                return true;
            }
            return false;
        }

        // GET: Contact
        public ActionResult Index()
        {
            CheckLogin();
            layout.Menus = login.GetMenus(this);
            
            return RedirectToAction("Index", "Dashboard");
            //return View(layout);
        }

        // GET: Contact/GetPersonalContact
        public ActionResult GetPersonalContact(string topCriteria, int page, int pageSize)
        {
            CheckLogin();

            ContactList data = contactService.GetPersonalContact(login.GetUser(this), topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: Contact/GetContactFromCompany/Id
        public ActionResult GetContactFromCompany(long CompanyId, string topCriteria, int page, int pageSize)
        {
            CheckLogin();
            ContactList data = contactService.GetContactFromCompany(login.GetUser(this), CompanyId, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCompaniesData()
        {
            CheckLogin();
            var data = contactService.GetListOfCompany(login.GetUser(this));
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}