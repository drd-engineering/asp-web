using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service;
using System.Web.Mvc;


namespace DRD.App.Controllers
{
    public class WorkflowController : Controller
    {
        LoginController login = new LoginController();
        WorkflowService workflowService = new WorkflowService();
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
        public bool InitializeAPI()
        {
            if (login.CheckLogin(this))
            {
                user = login.GetUser(this);
                return true;
            }
            return false;
        }

        // GET : Workflow/new
        public ActionResult New()
        {
            if (!Initialize())
                return RedirectToAction("Index", "login", new { redirectUrl = "Workflow/New" });
            WorkflowItem product = new WorkflowItem();
            layout.obj = product;

            return View(layout);
        }

        public ActionResult Index(long id)
        {
            if (!Initialize())
                return RedirectToAction("Index", "login", new { redirectUrl = "Workflow?id="+id });

            WorkflowItem product = new WorkflowItem();
            product = workflowService.GetById(id);
            layout.obj = product;

            return View(layout);
        }

        //GET : Workflow/list
        public ActionResult List()
        {
            if (!Initialize())
                return RedirectToAction("Index", "login", new { redirectUrl = "Workflow/List" });
            return View(layout);
        }

        public ActionResult GetById(long id)
        {
            InitializeAPI();

            var data = workflowService.GetById(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(WorkflowItem prod)
        {
            InitializeAPI();

            prod.CreatorId = user.Id;
            prod.UserEmail = user.Email;
            prod.Type = 0;

            var data = workflowService.Save(prod);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveDraft(WorkflowItem prod)
        {
            InitializeAPI();

            prod.CreatorId = user.Id;
            prod.UserEmail = user.Email;
            prod.Type = 1;

            var data = workflowService.Save(prod);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FindWorkflows(string topCriteria, int page, int pageSize)
        {
            InitializeAPI();

            var data = workflowService.FindWorkflows(user.Id, topCriteria, page, pageSize, null);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FindWorkflowsCountAll(string topCriteria)
        {
            InitializeAPI();

            var data = workflowService.FindWorkflowsCountAll(user.Id, topCriteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteWorkflow(long id)
        {
            InitializeAPI();
            
            var data = workflowService.Delete(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        
    }
}