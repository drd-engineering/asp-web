using DRD.Models.API;
using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service;
using System.Web.Mvc;
using System.Web.Routing;

namespace DRD.App.Controllers
{
    public class InboxController : Controller
    {
        InboxService inboxService;
        RotationProcessService rotationProcessService;

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
                rotationProcessService = new RotationProcessService();
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
        /// <summary>
        /// Access Page Inbox related to the inbox id that user has
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index(long id)
        {
            if (!CheckLogin(getMenu: true))
                return RedirectToAction("Index", "login", new { redirectUrl = "Inbox?id="+id });

            RotationInboxData product = inboxService.GetInboxItem(id, user.Id);

            //page authorization check if user has no access
            if (product == null || product.AccessType.Equals((int)Constant.AccessType.noAccess))
                return RedirectToAction("Index", "Inbox");

            //user have access
            layout.Object = product;
            return View(layout);
        }
        
        public ActionResult GetInboxDetail(long id)
        {
            if(!CheckLogin())
                return Json(-1, JsonRequestBehavior.AllowGet);
 
            var data = inboxService.GetInboxItem(id, user.Id);

            //page authorization check if user has no access
            if(data.AccessType.Equals((int)Constant.AccessType.noAccess))
                return Json(-2, JsonRequestBehavior.AllowGet);
            
            return Json(data, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// Page inbox list
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            if (!CheckLogin(getMenu: true))
            {
                return RedirectToAction("Index", "login", new { redirectUrl = "Inbox/List"});
            }
            return View(layout);
        }

        /// <summary>
        /// API to obtain all user inbox
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult GetInboxList(string criteria, int page, int pageSize)
        {
            CheckLogin();
            int skip = pageSize * (page - 1);
            var data = inboxService.GetInboxList(user.Id, criteria, skip, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CountAll(string criteria)
        {
            CheckLogin();
            var data = inboxService.CountAll(user.Id, criteria);
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

            var data = inboxService.CountUnread(user.Id);
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
            var data = rotationProcessService.ProcessActivity(param, (Constant.EnumActivityAction)bit);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}