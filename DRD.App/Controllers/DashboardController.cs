using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Models.API;
using DRD.Service;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace DRD.App.Controllers
{
    public class DashboardController : Controller
    {
        private DashboardService dashboardService = new DashboardService();
        private UserService userService = new UserService();
        private CompanyService companyService = new CompanyService();
        private RotationService rotationService = new RotationService();

        private LoginController login = new LoginController();
        private UserSession user;
        private Layout layout = new Layout();

        public bool Initialize()
        {
            if (login.CheckLogin(this))
            {
                user = login.GetUser(this);
                layout.Menus = login.GetMenus(this);
                layout.User = login.GetUser(this);
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
                return RedirectToAction("Index", "Login");
            if (user == null)
                return RedirectToAction("Index", "Login");
            Layout layout = new Layout();
            layout.Menus = login.GetMenus(this);
            layout.User = user;
            
            var hasCompany = userService.HasCompany(user.Id);
            if (!hasCompany) return RedirectToAction("List", "Inbox"); 
            
            return View(layout);
        }
        /// <summary>
        /// API to obtain how many rotation that user logged in has, devided by status of the rotation
        /// </summary>
        /// <returns></returns>
        public ActionResult GetActivityCounterUser()
        {
            Initialize();
            CounterRotation counter = (CounterRotation)Session["_COUNTERACTIVITY_"];
            if (counter == null)
                counter = new CounterRotation();

            if (user == null)
                return Json(null, JsonRequestBehavior.AllowGet);

            var data = dashboardService.GetActivityCounter(user.Id);
            if (counter.New != data.New)
            {
                counter.Old = counter.New;
                counter.New = data.New;
            }

            Session["_COUNTERACTIVITY_"] = counter;
            return Json(counter, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// API to obtain how many rotation that company has, devided by status of the rotation
        /// </summary>
        /// <param name="CompanyId">Company Id related to user Id as the owner</param>
        /// <returns></returns>
        public ActionResult GetActivityCounterCompany(long companyId)
        {
            Initialize();
            CounterRotation counter = (CounterRotation)Session["_COUNTERACTIVITY_"];
            if (counter == null)
                counter = new CounterRotation();

            if (user == null)
                return Json(null, JsonRequestBehavior.AllowGet);

            var data = dashboardService.GetActivityCounter(user.Id, companyId);
            if (counter.New != data.New)
            {
                counter.Old = counter.New;
                counter.New = data.New;
            }
            Session["_COUNTERACTIVITY_"] = counter;
            return Json(counter, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// API to obtain how the status of subscription a company has including usage and remainings
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public ActionResult GetCompanySubscriptionLimit(long companyId)
        {
            Initialize();
            SubscriptionLimit counter = (SubscriptionLimit)Session["_SUBSCRIPTIONLIMIT_"];
            if (counter == null)
                counter = new SubscriptionLimit();

            if (user == null)
                return Json(null, JsonRequestBehavior.AllowGet);

            var data = dashboardService.GetCompanySubscriptionLimit(companyId);
            if (counter.New != data.New)
            {
                counter.Old = counter.New;
                counter.New = data.New;
            }
            Session["_SUBSCRIPTIONLIMIT_"] = counter;
            return Json(counter, JsonRequestBehavior.AllowGet);
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
            var data = rotationService.GetRotationsByCompany(companyId, Tags, skip, pageSize);
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
            var data = rotationService.CountRotationsByCompany(companyId, Tags);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// API to obtain CSV file of All Rotation Status of a Company
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public ActionResult ExportAllRotationStatusToCSV(long companyId, string companyName)
        {
            InitializeAPI();
            var data = rotationService.GetRotationsByCompany(companyId, null, 0, -1);
            StringBuilder sb = new StringBuilder();
            sb.Append("Id,Subject,Status,Date Created,Date Started,Date Updated,Created by,Workflow Name,Tags,Done,Ongoing,Waiting");
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
                sb.Append(rtd.Workflow.Name + ',');
                foreach (var tagname in rtd.Tags)
                {
                    sb.Append(tagname + " | ");
                }
                sb.Append(",");
                var users = new List<RotationDashboard.UserDashboard>(rtd.RotationUsers);
                for (var i = 0; i < users.Count; i++)
                {
                    if (i == 0)
                    {
                        if (users[i].InboxStatus == -99)
                        {
                            sb.Append(",," + users[i].Name + " | ");
                            continue;
                        }
                    }
                    if (users[i].InboxStatus == 0)
                    {
                        sb.Append("," + users[i].Name + ",");
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