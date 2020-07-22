using DRD.Models.API;
using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service;
using System.Web.Mvc;
using DRD.Models;

namespace DRD.App.Controllers
{
    public class InboxController : Controller
    {
        InboxService inboxService;

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
                inboxService = new InboxService();
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

        // GET : Workflow
        public ActionResult Index(long id = Constant.ID_NOT_FOUND)
        {
            if (!CheckLogin(getMenu: true))
                return RedirectToAction("Index", "login", new { redirectUrl = "Inbox?id="+id });

            RotationInboxData inbox = inboxService.GetInbox(id, user.Id);

            //page authorization check if user has no access
            if (inbox == null || inbox.AccessType.Equals((int)DRD.Models.Constant.AccessType.noAccess))
                return RedirectToAction("Index", "Inbox");

            //user have access
            layout.Object = inbox;
            return View(layout);
        }

        // GET : Workflow/List
        public ActionResult List()
        {
            if (!CheckLogin(getMenu: true))
                return RedirectToAction("Index", "login", new { redirectUrl = "Inbox/List"});
            return View(layout);
        }

        /// <summary>
        /// API get detail of Inbox to update the latest inbox info after updating/process inbox
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetInbox(long id)
        {
            if(!CheckLogin())
                return Json(-1, JsonRequestBehavior.AllowGet);
 
            var data = inboxService.GetInbox(id, user.Id);

            //page authorization check if user has no access
            if(data.AccessType.Equals((int)DRD.Models.Constant.AccessType.noAccess))
                return Json(-2, JsonRequestBehavior.AllowGet);
            
            return Json(data, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// API to obtain all user inbox
        /// </summary>
        /// <param name="page"></param>
        /// <param name="totalItemPerPage"></param>
        /// <returns></returns>
        public ActionResult GetInboxes(string criteria, int page, int totalItemPerPage)
        {
            CheckLogin();
            int skip = totalItemPerPage * (page - 1);
            var data = inboxService.GetInboxes(user.Id, criteria, skip, totalItemPerPage);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CountInboxes(string criteria)
        {
            CheckLogin();
            var data = inboxService.CountInboxes(user.Id, criteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateUnreadInboxCounter()
        {
            CheckLogin();
            CounterInboxData counter = (CounterInboxData)Session["_COUNTERINBOX_"];
            if (counter == null)
                counter = new CounterInboxData();

            if (user == null)
                return Json(null, JsonRequestBehavior.AllowGet);

            var data = inboxService.CountUnreadInboxes(user.Id);
            if (counter.New.Unread != data)
            {
                counter.Old.Unread = counter.New.Unread;
                counter.New.Unread = data;
            }
            Session["_COUNTERINBOX_"] = counter;
            return Json(counter, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// API save workflow, workflow nodes and workflow links
        /// </summary>
        /// <param name="workflowUpdate"></param>
        /// <returns></returns>
        public ActionResult ProcessActivity(ProcessActivity param, int bit)
        {
            CheckLogin();
            var data = inboxService.ProcessActivity(param, (DRD.Models.Constant.EnumActivityAction)bit);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}