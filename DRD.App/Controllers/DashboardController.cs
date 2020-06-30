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
        LoginController login;
        UserSession user;
        DashboardService dashboardService;
        Layout layout;
        bool hasCompany;
        // HELPER
        private bool CheckLogin(bool getMenu = false)
        {
            login = new LoginController();
            if (login.CheckLogin(this))
            {
                //instantiate
                user = login.GetUser(this);
                dashboardService = new DashboardService();
                if (!hasCompany)
                    hasCompany = dashboardService.CheckIsUserHasCompany(user.Id);
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
        // GET: Dashboard
        public ActionResult Index()
        {
            if (!CheckLogin(getMenu: true))
                return RedirectToAction("Index", "login", new { redirectUrl = "dashboard" });
            layout.User = user;
            // prevent user to go do dashboard page if they are not an owner
            if (!hasCompany) return RedirectToAction("List", "Inbox"); 
            return View(layout);
        }
        /// <summary>
        /// API to obtain how many rotation that company has, devided by status of the rotation
        /// </summary>
        /// <param name="CompanyId">Company Id related to user Id as the owner</param>
        /// <returns></returns>
        public ActionResult GetActivityCounterCompany(long companyId)
        {
            if (!CheckLogin(getMenu: true))
                return RedirectToAction("Index", "login", new { redirectUrl = "dashboard" });
            // prevent user to go do dashboard page if they are not an owner
            if (!hasCompany) return RedirectToAction("List", "Inbox");
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
            if (!CheckLogin(getMenu: true))
                return RedirectToAction("Index", "login", new { redirectUrl = "dashboard" });
            // prevent user to go do dashboard page if they are not an owner
            if (!hasCompany) return RedirectToAction("List", "Inbox");
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
        public ActionResult GetRotationsByCompany(long companyId, ICollection<string> Tags, int page, int pageSize)
        {
            if (!CheckLogin(getMenu: true))
                return RedirectToAction("Index", "login", new { redirectUrl = "profile" });
            int skip = (page - 1) * pageSize;
            var data = dashboardService.GetRotationsByCompany(companyId, Tags, skip, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// API to count all Rotation Status of a company
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="Tags"></param>
        /// <returns></returns>
        public ActionResult CountRotationsByCompany(long companyId, ICollection<string> Tags)
        {
            if (!CheckLogin(getMenu: true))
                return RedirectToAction("Index", "login", new { redirectUrl = "profile" });
            var data = dashboardService.CountRotationsByCompany(companyId, Tags);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// API to obtain CSV file of All Rotation Status of a Company
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public ActionResult ExportAllRotationStatusToCSV(long companyId, string companyName)
        {
            if (!CheckLogin(getMenu: true))
                return RedirectToAction("Index", "login", new { redirectUrl = "dashboard" });
            // prevent user to go do dashboard page if they are not an owner
            if (!hasCompany) return RedirectToAction("List", "Inbox");
            var data = dashboardService.GetRotationsByCompany(companyId, null, 0, -1);
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
            if (!CheckLogin(getMenu: true))
                return RedirectToAction("Index", "login", new { redirectUrl = "dashboard" });
            // prevent user to go do dashboard page if they are not an owner
            if (!hasCompany) return RedirectToAction("List", "Inbox");
            var data = dashboardService.SendEmailNotifikasiRotasi(rotationId, userId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}