using DRD.Models.Custom;
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
        LoginController login;
        UserSession user;
        UserService userService;
        Layout layout;
        // HELPER
        private bool CheckLogin(bool getMenu = false)
        {
            login = new LoginController();
            if (login.CheckLogin(this))
            {
                //instantiate
                user = login.GetUser(this);
                userService = new UserService();
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
        // GET: Profile
        public ActionResult Index()
        {
            if (!CheckLogin(getMenu:true))
                return RedirectToAction("Index", "login", new { redirectUrl = "profile" });
            layout.Object = user;
            return View(layout);
        }
        /// <summary>
        /// API get profile data of user logged in
        /// </summary>
        /// <returns></returns>
        public ActionResult GetProfileUser()
        {
            if (!CheckLogin(getMenu: false))
                return RedirectToAction("Index", "login", new { redirectUrl = "profile" });
            UserProfile userProfile = userService.GetProfile(user.Id);
            return Json(userProfile, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// SAVE user logged in profile updated data
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>
        public ActionResult Update(UserProfile newUser)
        {
            if (!CheckLogin(getMenu: false))
                return RedirectToAction("Index", "login", new { redirectUrl = "profile" });
            // List of return status
            var data = new List<int>();
            System.Diagnostics.Debug.WriteLine(newUser.Id);
            var ret1 = userService.UpdateProfile(newUser);
            data.Add(1);
            data.Add(MoveFromTemp(newUser, ret1.InitialImageFileName));
            data.Add(MoveFromTemp(newUser, ret1.KTPImageFileName));
            data.Add(MoveFromTemp(newUser, ret1.KTPVerificationImageFileName));
            data.Add(MoveFromTemp(newUser, ret1.ProfileImageFileName));
            data.Add(MoveFromTemp(newUser, ret1.SignatureImageFileName));
            data.Add(MoveFromTemp(newUser, ret1.StampImageFileName));

            var updatedUser = userService.GetUpdatedUser(newUser.Id);
            {
                Session["_USER_LOGIN_"] = updatedUser;
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// API upload image related to profile like signature image, profile image, etc. to a temporary directory
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
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
        /// <summary>
        /// HELPER move any image file user from folder temporary to user folder
        /// </summary>
        /// <param name="user"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        private int MoveFromTemp(UserProfile user, string filename)
        {
            if (filename == null)
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
                Tranfiles = Server.MapPath("/" + fromFolder + "/") + filename;
                if (System.IO.File.Exists(Tranfiles))
                {
                    ProcessedFiles = Server.MapPath("/" + toFolder + "/") + filename;
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
            catch (Exception)
            {
                return 0;
            }
        }
    }
}