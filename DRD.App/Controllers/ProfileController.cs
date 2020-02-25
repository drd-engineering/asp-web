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
            user.EncryptedId = Utilities.Encrypt(user.Id.ToString());
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
                    System.Diagnostics.Debug.WriteLine("IMAGE UPLOADED :::" + path);
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

            System.Diagnostics.Debug.WriteLine("PROFILE CONTROLLER, UPDATE RESULT" + ret1);
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
            string fromFolder = "images/member/temp";
            string toFolder = "images/member/" + user.EncryptedId.ToString();
            var targetdir = "/" + toFolder + "/";
            bool exists = System.IO.Directory.Exists(targetdir);
            if (!exists)
                System.IO.Directory.CreateDirectory(targetdir);
            try
            {
                int returnval = 0;
                string Tranfiles, ProcessedFiles;
                Tranfiles = Server.MapPath("/" + fromFolder + "/") + location;
                System.Diagnostics.Debug.WriteLine("[[ IMAGE NAME ]] " + Tranfiles);
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
                System.Diagnostics.Debug.WriteLine("[[ERROR HAPPEN]] " + ex.Message);
                return 0;
            }
        }
    }
}