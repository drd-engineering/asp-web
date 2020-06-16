using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DRD.Models.API;
using DRD.Models.Custom;
using DRD.Service;
 
namespace DRD.App.Controllers
{
    public class UserController : Controller
    {
        /// <summary>
        /// This function will return all the user's subscriptions even it personal or company
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult GetAllSubscription()
        {
            var logincontroller = new LoginController();
            UserSession user = logincontroller.GetUser(this);
            var service = new UserService();
            var data = service.GetAllSubscription(user.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ValidationPassword(long userId, string password)
        {
            var srv = new UserService();
            if (userId == 0)
            {
                var logincontroller = new LoginController();
                UserSession user = logincontroller.GetUser(this);
                userId = user.Id;
            }
            var data = srv.ValidationPassword(userId, password);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}