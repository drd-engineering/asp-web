using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Models.API;
using DRD.Models.API.Register;
using DRD.Service;
 
namespace DRD.App.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        //User Registration
        public ActionResult Registration(Register register)
        {
            var service = new UserService();
            var data = service.SaveRegistration(register);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}