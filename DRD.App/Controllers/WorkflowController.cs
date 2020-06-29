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

        //helper
        private bool CheckLogin(bool getMenu = false)
        {
            login = new LoginController();
            if (login.CheckLogin(this))
            {
                //instantiate
                user = login.GetUser(this);
                workflowService = new WorkflowService();
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

            layout.Object = workflowService.GetById(id,user.Id);
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

        /// <summary>
        /// API save workflow, workflow nodes and workflow links
        /// </summary>
        /// <param name="workflowUpdate"></param>
        /// <returns></returns>
        public ActionResult Save(WorkflowItem workflowUpdate)
        {
            CheckLogin();
            workflowUpdate.CreatorId = user.Id;
            var data = workflowService.Save(workflowUpdate);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// API Get all workflows that match the params
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="page"></param>
        /// <param name="totalItemPerPage"></param>
        /// <returns></returns>
        public ActionResult GetWorkflows(string criteria, int page, int totalItemPerPage)
        {
            CheckLogin();
            var data = workflowService.GetWorkflows(user.Id, criteria, page, totalItemPerPage);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// API count the total of workflows that match the criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public ActionResult CountWorkflows(string criteria)
        {
            CheckLogin();
            var data = workflowService.CountWorkflows(user.Id, criteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// API delete workflow based on workflow id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteWorkflow(long id)
        {
            CheckLogin();
            var data = workflowService.Delete(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        
    }
}