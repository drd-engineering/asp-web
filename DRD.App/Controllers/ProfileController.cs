﻿using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace DRD.App.Controllers
{

    public class ProfileController : Controller
    {
        LoginController login = new LoginController();
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

        public void InitializeAPI()
        {
            user = login.GetUser(this);
            login.CheckLogin(this);
        }

        // GET: Profile
        public ActionResult Index()
        {
            if (!Initialize())
                return RedirectToAction("Index", "Login");
            
            layout.obj = login.GetUser(this);
            layout.activeId = 0;

            return View(layout);
        }

        // GET: Profile/MemberList/
        public ActionResult UserList()
        {
            InitializeAPI();

            // begin decription menu
            UserSession userSession = login.GetUser(this);
            // end decription menu

            layout.user = login.GetUser(this);

            return View(layout);
        }

        // GET Profile/GetData
        public ActionResult GetData()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            UserSession userSession = login.GetUser(this);

            UserService userService = new UserService();

            UserProfile user = userService.GetById(userSession.Id, userSession.Id);
            user.EncryptedId = Utilities.Encrypt(user.Id.ToString());
            var data = user;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserMenu()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            // begin decription menu
            UserSession userSession = login.GetUser(this);
            // end decription menu

            UserProfile user = new UserProfile();
           
            Layout layout = new Layout();
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = userSession;
            layout.obj = user;

            return View(layout);
        }

        // redundant with the login method
        public ActionResult GetUserLogin()
        {
            LoginController login = new LoginController();
            UserService userService = new UserService();
            long id = login.GetUser(this).Id;
            var data = userService.GetById(id, id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Upload(int idx)
        {
            string folder = "images/member/temp";
            string imageName = "";
            string imageExtension = "";

            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                // get file from request body
                var file = System.Web.HttpContext.Current.Request.Files["MyImages"];

                if (file == null || file.ContentLength <= 0)
                    file = System.Web.HttpContext.Current.Request.Files[0];

                // if uploaded image exist
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    imageExtension = Path.GetExtension(file.FileName);

                    if (String.IsNullOrEmpty(imageExtension))
                        imageExtension = ".png";

                    imageName = Guid.NewGuid().ToString();
                    var targetdir = "/" + folder + "/";
                    bool exists = System.IO.Directory.Exists(Server.MapPath(targetdir));
                    if (!exists)
                        System.IO.Directory.CreateDirectory(Server.MapPath(targetdir));

                    var imagePath = Server.MapPath(targetdir) + imageName + imageExtension;
                    imageName = imageName + imageExtension;

                    ViewBag.Msg = imagePath;
                    var path = imagePath;

                    // Saving Image in Original Mode

                    file.SaveAs(path);
                }
            }
            UploadResponse result = new UploadResponse();
            result.Idx = idx;
            result.Filename = imageName;
            result.Fileext = imageExtension.Replace(".", "");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(UserProfile user)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            // begin decription menu
            UserSession userSession = login.GetUser(this);

            // assign uneditable data
            //user.Email = userSession.Email;

            //user.CompanyId = userSession.CompanyId;

            UserService userService = new UserService();
            /*UserProfile oldUser = userService.GetById(user.Id, userSession.Id);*/ // no need to validate who is log in
            var data = new List<int>();

            var ret1 = userService.Update(user);
            data.Add(1);
            data.Add(MoveFromTemp(user, ret1.ImageInitials));
            data.Add(MoveFromTemp(user, ret1.ImageKtp1));
            data.Add(MoveFromTemp(user, ret1.ImageKtp2));
            data.Add(MoveFromTemp(user, ret1.ImageProfile));
            data.Add(MoveFromTemp(user, ret1.ImageSignature));
            data.Add(MoveFromTemp(user, ret1.ImageStamp));

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        private int MoveFromTemp(UserProfile user, string location)
        {
            if (location == null)
                return -2;
            string fromFolder = "Images/Member/temp";
            string toFolder = "Images/Member/" + user.EncryptedId.ToString();
            var targetDir = "/" + toFolder + "/";
            bool exists = System.IO.Directory.Exists(Server.MapPath(targetDir));
            if (!exists)
                System.IO.Directory.CreateDirectory(Server.MapPath(targetDir));
            try
            {

                string Tranfiles, ProcessedFiles;
                Tranfiles = Server.MapPath("/" + fromFolder + "/") + location;
                if (System.IO.File.Exists(Tranfiles))
                {
                    ProcessedFiles = Server.MapPath("/" + toFolder + "/") + location;
                    //Need to move or overwrite the new file with actual file.
                    System.IO.File.Move(Tranfiles, ProcessedFiles);
                    System.IO.File.Delete(Tranfiles);
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}