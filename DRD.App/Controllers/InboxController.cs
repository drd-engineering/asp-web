using DRD.Models.API;
using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service;
using System.Web.Mvc;

namespace DRD.App.Controllers
{
    public class InboxController : Controller
    {
        LoginController login = new LoginController();
        InboxService inboxService = new InboxService();
        UserSession user;
        Layout layout = new Layout();

        public bool Initialize()
        {
            if (login.CheckLogin(this))
            {
                user = login.GetUser(this);
                layout.menus = login.GetMenus(this, layout.activeId);
                layout.user = login.GetUser(this);
                return true;
            }
            return false;
        }
        public void InitializeAPI()
        {
            user = login.GetUser(this);
            login.CheckLogin(this);
        }
        /// <summary>
        /// Access Page Inbox related to the inbox id that user has
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index(long id)
        {
            if (!Initialize())
                return RedirectToAction("Index", "LoginController");

            RotationInboxData product = inboxService.GetInboxItem(id, user.Id);
            //page authorization check if user has no access
            if (product.AccessType.Equals((int)Constant.AccessType.noAccess))
                return RedirectToAction("Index", "Dashboard");
            //user have access
            layout.obj = product;
            return View(layout);
        }
        
        public ActionResult GetInboxDetail(long id)
        {
            if(!Initialize())
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
            if (!Initialize())
                return RedirectToAction("Index", "LoginController");
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
            Initialize();
            int skip = pageSize * (page - 1);
            var data = inboxService.GetInboxList(user.Id, criteria, skip, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CountAll(string criteria)
        {
            Initialize();
            var data = inboxService.CountAll(user.Id, criteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateUnreadInboxCounter()
        {
            Initialize();
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
    }
}