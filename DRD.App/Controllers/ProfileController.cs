using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DRD.Models;
using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service;

namespace DRD.App.Controllers
{

    public class ProfileController : Controller
    {
        // GET: Profile
        public ActionResult Index()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            Layout layout = new Layout();
            layout.menus = login.GetMenus(this, 0);
            layout.user = login.GetUser(this);
            layout.obj = login.GetUser(this);
            layout.activeId = 0;

            return View(layout);
        }

        // GET: Profile/MemberList/
        public ActionResult UserList()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            // begin decription menu
            UserSession userSession = login.GetUser(this);
            var strmenu = login.ManipulateMenu(this, userSession);
            // end decription menu

            Layout layout = new Layout();
            layout.activeId = int.Parse(strmenu);
            layout.menus = login.GetMenus(this, layout.activeId);
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
            var data = user;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult User()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            // begin decription menu
            UserSession userSession = login.GetUser(this);
            var strmenu = login.ManipulateSubMenu(this, userSession);
            // end decription menu

            UserProfile user = new UserProfile();
            string[] ids = strmenu.Split(',');
            if (ids.Length > 1 && !ids[1].Equals("0"))
            {
                UserService userService = new UserService();
                user = userService.GetById(int.Parse(ids[1]), userSession.Id);
            }

            Layout layout = new Layout();
            layout.activeId = int.Parse(ids[0]);
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
            string folder = "images/member";
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
                    var imagePath = Server.MapPath("/" + folder + "/") + imageName + imageExtension;
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
            UserProfile oldUser = userService.GetById(user.Id, userSession.Id);


            //user.IsActive = oldUser.IsActive;
            
            oldUser.ImageInitials = user.ImageInitials;
            oldUser.ImageKtp1 = user.ImageKtp1;
            oldUser.ImageKtp2 = user.ImageKtp2;
            oldUser.ImageProfile = user.ImageProfile;
            oldUser.ImageSignature = user.ImageSignature;
            oldUser.ImageStamp = user.ImageStamp;
            oldUser.OfficialIdNo = user.OfficialIdNo;

            var data = userService.Update(oldUser);

            System.Diagnostics.Debug.WriteLine("PROFILE CONTROLLER, UPDATE RESULT" + data);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}