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
    public class RegisterController : Controller
    {
        private ServiceContext db = new ServiceContext();

        // GET: Register
        public ActionResult Index()
        {
            //var service = new CompanyService();
            //var data = service.GetAllCompany();
            //CompanyDropDown.DataSource = data;
            //CompanyDropDown.DataBind();
            return View();
        }

        // GET: Register/Save
        //User Registration
        public ActionResult Save(Register register)
        {
            var service = new UserService();
            var data = service.SaveRegistration(register);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: Register/GetAllCompaniesRegistered
        //User Registration
        public ActionResult GetAllCompaniesRegistered()
        {
            var service = new CompanyService();
            var data = service.GetAllCompany();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: Register/CheckEmail
        //User Registration
        public ActionResult CheckEmail(string email)
        {
            var service = new UserService();
            var data = service.CheckEmailAvailability(email);
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
