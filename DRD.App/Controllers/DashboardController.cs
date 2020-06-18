using DRD.Models.API;
using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace DRD.App.Controllers
{
    public class DashboardController : Controller
    {
        private LoginController login = new LoginController();
        private DashboardService dashboardService = new DashboardService();
        private CompanyService companyService = new CompanyService();
        private RotationService rotationService = new RotationService();
        private UserSession user;
        private Layout layout = new Layout();

        public bool Initialize()
        {
            if (login.CheckLogin(this)) { 
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

        // GET: Dashboard
        public ActionResult Index()
        {
            if (!Initialize())
                return RedirectToAction("Index", "LoginController");
            Layout layout = new Layout();
            layout.menus = login.GetMenus(this, 0);
            layout.user = user;
            layout.activeId = 0;

            CompanyService companyService = new CompanyService();
            if (user != null)
            {
            CompanyList companyList = companyService.GetAllCompanyOwnedbyUser(user.Id);
            if (companyList.companies.Count > 0) return View(layout);
            }
            return RedirectToAction("List", "Inbox");
        }

        public ActionResult GetRotationFromAllCompany()
        {
            Initialize();
            CounterItem counter = (CounterItem)Session["_COUNTER_"];
            if (counter == null)
                counter = new CounterItem();

            if (user == null)
                return Json(null, JsonRequestBehavior.AllowGet);

            var data = dashboardService.GetActivityCounter(user.Id, counter);
            Session["_COUNTER_"] = data;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetActivityCounter()
        {
            Initialize();
            CounterItem counter = (CounterItem)Session["_COUNTER_"];
            if (counter == null)
                counter = new CounterItem();

            if (user == null)
                return Json(null, JsonRequestBehavior.AllowGet);

            var data = dashboardService.GetActivityCounter(user.Id, counter);
            Session["_COUNTER_"] = data;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetActivityCounterCompany(long CompanyId)
        {
            Initialize();
            CounterItem counter = (CounterItem)Session["_COUNTER_"];
            if (counter == null)
                counter = new CounterItem();

            if (user == null)
                return Json(null, JsonRequestBehavior.AllowGet);

            var data = dashboardService.GetActivityCounter(user.Id, counter, CompanyId);
            Session["_COUNTER_"] = data;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// API to obtain Rotation status of a Company 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="Tags"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult GetDashboardRotationStatus(long companyId, ICollection<string> Tags, int page, int pageSize)
        {
            Initialize();
            int skip = (page - 1) * pageSize;
            var data = rotationService.GetRelatedToCompany(companyId, Tags, skip, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// API to count all Rotation Status of a company
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="Tags"></param>
        /// <returns></returns>
        public ActionResult DashboardRotationStatusCountAll(long companyId, ICollection<string> Tags)
        {
            Initialize();
            var data = rotationService.CountAllRelatedToCompany(companyId, Tags);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// API to obtain CSV file of All Rotation Status of a Company
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public ActionResult ExportAllRotationStatusToCSV(long companyId, string companyName) {
            InitializeAPI();
            var data = rotationService.GetRelatedToCompany(companyId, null, 0, -1);
            StringBuilder sb = new StringBuilder();
            sb.Append("Id,Subject,Status,Date Created,Date Started,Date Updated,Created by,Workflow Name,Tags,Reviewed Done by,Reviewed by,Not Yet Review");
            sb.Append("\r\n");
            foreach (RotationDashboard rtd in data)
            {
                sb.Append(rtd.Id.ToString() + ',');
                sb.Append(rtd.Subject + ',');
                sb.Append(rtd.Status.ToString() + ',');
                sb.Append(rtd.DateCreated.ToString() + ',');
                sb.Append(rtd.DateStarted.ToString() + ',');
                sb.Append(rtd.DateUpdated.ToString() + ',');
                sb.Append(rtd.Creator.Name + ',');
                sb.Append(rtd.Workflow.Name+ ',');
                foreach (var tagname in rtd.Tags)
                {
                    sb.Append(tagname + " | ");
                }
                sb.Append(",");
                var users = new List<RotationDashboard.UserDashboard>(rtd.RotationUsers);
                for (var i = 0; i<users.Count;  i++)
                {
                    if (i == 0)
                    {
                        if (users[i].InboxStatus == -99)
                        {
                            sb.Append(",,"+ users[i].Name + " | ");
                            continue;
                        }
                    }
                    if (users[i].InboxStatus == 0)
                    {
                        sb.Append(","+ users[i].Name + ",");
                    }
                    sb.Append(users[i].Name + " | ");
                }
                //Append new line character.
                sb.Append("\r\n");
            }
            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "RotationStatus" + companyName + ".csv");
        }
        public ActionResult SendEmailNotifikasiRotasi(long rotationId, long userId)
        {
            Initialize();
            var data = dashboardService.SendEmailNotifikasiRotasi(rotationId, userId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}