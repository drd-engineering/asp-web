using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DRD.Models;
using DRD.Service.Context;
using DRD.Service;
using DRD.Models.API.Register;

namespace DRD.App.Controllers
{
    public class RegistrationController : Controller
    {
        private ServiceContext db = new ServiceContext();

        // GET: Registration
        public ActionResult Index()
        {
            return View();
        }

        // GET: Registration/Save
        //User Registration
        public ActionResult Save(Register register)
        {
            var service = new UserService();
            var data = service.SaveRegistration(register);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
