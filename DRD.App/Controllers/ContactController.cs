using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service;
using System.Web.Mvc;

namespace DRD.App.Controllers
{
    public class ContactController : Controller
    {
        private ContactService contactService;

        private LoginController login;
        private UserSession user;
        private Layout layout;

        //helper
        private bool CheckLogin(bool getMenu = false)
        {
            login = new LoginController();
            if (login.CheckLogin(this))
            {
                //instantiate
                user = login.GetUser(this);
                contactService = new ContactService();
                if (getMenu)
                {
                    //get menu if user authenticated
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
            
            if (!CheckLogin(getMenu: true))
                return RedirectToAction("Index", "login", new { redirectUrl = "Contact"});
            
            return View(layout);
        }

        // GET: Contact/GetPersonalContact
        public ActionResult GetPersonalContact(string topCriteria, int page, int pageSize)
        {
            CheckLogin();

            ContactList data = contactService.GetPersonalContact(login.GetUser(this), topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: Contact/GetEmailContact
        public ActionResult GetEmailContact(string email)
        {
            CheckLogin();

            MemberData data = contactService.GetEmailContact(user.Id, email);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: Contact/AddPersonalContact
        public ActionResult AddPersonalContact(long userId)
        {
            CheckLogin();

            long data = contactService.AddPersonalContact(user.Id, userId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult InviteEmail(string criteria)
        {
            CheckLogin();

            //TODO invite by email

            return Json(null, JsonRequestBehavior.AllowGet);
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