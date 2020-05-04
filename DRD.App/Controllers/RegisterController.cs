﻿using System;
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
using DRD.Models.API;

namespace DRD.App.Controllers
{
    public class RegisterController : Controller
    {
        private ServiceContext db = new ServiceContext();

        // GET: Register
        // Index of register page
        public ActionResult Index()
        {
            return View();
        }

        // GET: Register/Save
        // Register a new User, save it to database
        public ActionResult Save(Register register)
        {
            var service = new UserService();
            var data = service.SaveRegistration(register);
            var registrationResponse = new RegisterResponse();
            if (data.Id < 0)
            {
                registrationResponse.Id = "DBLEMAIL";
                return Json(registrationResponse, JsonRequestBehavior.AllowGet);
            }
            var encryptedUserId = Utilities.Encrypt(data.Id.ToString());
            string userFolder = "Images/Member/" + encryptedUserId;
            var targetDir = "/" + userFolder + "/";
            bool exists = System.IO.Directory.Exists(Server.MapPath(targetDir));
            if (!exists)
            {
                System.IO.Directory.CreateDirectory(Server.MapPath(targetDir));
                var Tranfiles = Server.MapPath("/" + "Images/Member" + "/") + "user.png";
                if (System.IO.File.Exists(Tranfiles))
                {
                    //Need to mention any file,so that to overwrite this newly created with the actual file,other wise will get 2 errors like
                    //1)Cannot create a file when that file already exists
                    //2)The path....is a folder not a file.
                    //ProcessedFiles = Server.MapPath(@"~\ProcessedFiles"); //Wrong
                    var ProcessedFiles = Server.MapPath("/" + userFolder + "/") + "user.png";

                    //Need to move or overwrite the new file with actual file.
                    System.IO.File.Copy(Tranfiles, ProcessedFiles);
                }
                registrationResponse.Id = "" + data.Id;
                registrationResponse.Email = data.Email;
                service.SendEmailRegistration(data);
            }
            else
                registrationResponse.Id = "ERROR";
            return Json(registrationResponse, JsonRequestBehavior.AllowGet);
        }

        // GET: Register/GetAllCompany
        // Retrieve all companny as list
        public ActionResult GetAllCompany()
        {
            var service = new CompanyService();
            var data = service.GetAllCompany();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        // GET: Register/CompanySelected?id=
        // retrieve a single company by company id
        public ActionResult CompanySelected(int id)
        {
            var service = new CompanyService();
            var data = service.GetAllCompany();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: Register/CheckEmail
        // Check whenever an email is already used
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
