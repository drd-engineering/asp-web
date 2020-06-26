using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service;
using System.Web.Mvc;


namespace DRD.App.Controllers
{
    public class WorkflowController : Controller
    {
        private WorkflowService workflowService;

        private LoginController login;
        private UserSession user;
        private Layout layout;

        private bool CheckLogin(bool getMenu = false)
        {
            login = new LoginController();
            if (login.CheckLogin(this))
            {
                user = login.GetUser(this);
                workflowService = new WorkflowService();
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

        // GET : Workflow/new
        public ActionResult New()
        {
            if (!CheckLogin(getMenu:true))
                return RedirectToAction("Index", "login", new { redirectUrl = "Workflow/New" });
            return View(layout);
        }

        // GET : Workflow
        public ActionResult Index(long id = Constant.ID_NOT_FOUND)
        {
            if (!CheckLogin(getMenu:true))
                return RedirectToAction("Index", "login", new { redirectUrl = "Workflow?id="+id });

            layout.Object = workflowService.GetById(id);
            if (layout.Object == null) return RedirectToAction("list", "workflow");

            return View(layout);
        }

        //GET : Workflow/list
        public ActionResult List()
        {
            if (!CheckLogin(getMenu:true))
                return RedirectToAction("Index", "login", new { redirectUrl = "Workflow/List" });
            return View(layout);
        }

        public ActionResult GetById(long id)
        {
            CheckLogin();
            var data = workflowService.GetById(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(WorkflowItem prod)
        {
            CheckLogin();
            prod.CreatorId = user.Id;
            var data = workflowService.Save(prod);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetWorkflows(string topCriteria, int page, int pageSize)
        {
            CheckLogin();
            var data = workflowService.GetWorkflows(user.Id, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CountWorkflows(string topCriteria)
        {
            CheckLogin();
            var data = workflowService.CountWorkflows(user.Id, topCriteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteWorkflow(long id)
        {
            CheckLogin();
            var data = workflowService.Delete(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        
    }
}