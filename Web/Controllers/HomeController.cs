using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Domain;
using System.Based.Core;
using System.Runtime.InteropServices;

namespace DRD.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        // upload yang dipake di web dan android
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UploadDocFile(int idx, int fileType)
        {
            string folder = "doc/mgn"; // fileType = 0
            if (fileType == 1)
                folder = "images/member";
            else if (fileType == 2)
                folder = "images/stamp";
            else if (fileType == 100)
                folder = "doc/drdrive";

            string _imgname = string.Empty;
            string _ext = "";
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var pic = System.Web.HttpContext.Current.Request.Files["MyImages"];
                if (pic == null || pic.ContentLength <= 0)
                    pic = System.Web.HttpContext.Current.Request.Files[0];

                if (pic.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(pic.FileName);
                    _ext = Path.GetExtension(pic.FileName);
                    if (String.IsNullOrEmpty(_ext))
                        _ext = ".png";

                    _imgname = Guid.NewGuid().ToString();
                    var _comPath = Server.MapPath("/" + folder + "/") + _imgname + _ext;
                    _imgname = _imgname + _ext;

                    ViewBag.Msg = _comPath;
                    var path = _comPath;

                    //// Saving Image in Original Mode
                    //pic.SaveAs(path);

                    //var binaryReader = new BinaryReader(Request.Files[0].InputStream);
                    //var datas = binaryReader.ReadBytes(Request.Files[0].ContentLength);

                    // encryption
                    XFEncryptionHelper xf = new XFEncryptionHelper();
                    var xresult = xf.FileEncryptRequest(Request, path);

                }
            }
            JsonUploadResult result = new JsonUploadResult();
            result.idx = idx;
            result.filename = _imgname;
            result.fileext = _ext.Replace(".", "");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // upload yang dipake di web dan android
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UploadDocFileX(int idx, int fileType)
        {
            string folder = "doc/mgn"; // fileType = 0
            if (fileType == 1)
                folder = "images/member";
            else if (fileType == 2)
                folder = "images/stamp";
            else if (fileType == 100)
                folder = "doc/drdrive";

            string _imgname = string.Empty;
            string _ext = "";
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var pic = System.Web.HttpContext.Current.Request.Files["MyImages"];
                if (pic == null || pic.ContentLength <= 0)
                    pic = System.Web.HttpContext.Current.Request.Files[0];

                if (pic.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(pic.FileName);
                    _ext = Path.GetExtension(pic.FileName);
                    if (String.IsNullOrEmpty(_ext))
                        _ext = ".png";

                    _imgname = Guid.NewGuid().ToString();
                    var _comPath = Server.MapPath("/" + folder + "/") + _imgname + _ext;
                    _imgname = _imgname + _ext;

                    ViewBag.Msg = _comPath;
                    var path = _comPath;

                    // Saving Image in Original Mode
                    pic.SaveAs(path);


                    // encryption
                    XFEncryptionHelper xf = new XFEncryptionHelper();
                    var xresult = xf.FileEncrypt(path);

                }
            }
            JsonUploadResult result = new JsonUploadResult();
            result.idx = idx;
            result.filename = _imgname;
            result.fileext = _ext.Replace(".", "");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UploadFileImage(int idx, int fileType)
        {
            string folder = getUploadFolder(fileType);

            string _imgname = string.Empty;
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var pic = System.Web.HttpContext.Current.Request.Files["MyImages"];
                if (pic.ContentLength <= 0)
                    pic = System.Web.HttpContext.Current.Request.Files[0];

                if (pic.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(pic.FileName);
                    var _ext = Path.GetExtension(pic.FileName);

                    _imgname = Guid.NewGuid().ToString();
                    var _comPath = Server.MapPath("/Images/" + folder + "/") + _imgname + _ext;
                    _imgname = _imgname + _ext;

                    ViewBag.Msg = _comPath;
                    var path = _comPath;

                    // Saving Image in Original Mode
                    pic.SaveAs(path);

                }
            }
            JsonUploadResult result = new JsonUploadResult();
            result.idx = idx;
            result.filename = _imgname;
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UploadImageFromAndroid(int idx, int fileType)
        {
            string folder = getUploadFolder(fileType);

            string imagedata = Request.Form["image"];

            var fileName = "";
            if (imagedata != null)
            {

                fileName = Guid.NewGuid().ToString() + ".jpg";
                var path = Server.MapPath("/Images/" + folder + "/") + fileName;
                byte[] data = Convert.FromBase64String(imagedata);

                // Saving Image in Original Mode
                System.IO.File.WriteAllBytes(path, data);
            }
            JsonUploadResult result = new JsonUploadResult();
            result.idx = idx;
            result.filename = fileName;
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UploadImagesFromAndroid()
        {
            var results = new List<JsonUploadResult>();

            for (int i = 0; ; i++)
            {
                string fileType = Request.Form["fileType" + i];
                if (fileType == null)
                    break;

                string idx = Request.Form["idx" + i];
                string imagedata = Request.Form["image" + i];
                string folder = getUploadFolder(int.Parse(fileType));


                var fileName = "";
                if (imagedata != null)
                {

                    fileName = Guid.NewGuid().ToString() + ".jpg";
                    var path = Server.MapPath("/Images/" + folder + "/") + fileName;
                    byte[] data = Convert.FromBase64String(imagedata);

                    // Saving Image in Original Mode
                    System.IO.File.WriteAllBytes(path, data);
                }
                JsonUploadResult result = new JsonUploadResult();
                result.idx = int.Parse(idx);
                result.filename = fileName;
                results.Add(result);
            }
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UploadFileFromAndroid(int idx, int fileType, string ext)
        {
            string folder = getUploadFolder(fileType);

            string imagedata = Request.Form["image"];

            var fileName = "";
            if (imagedata != null)
            {

                fileName = Guid.NewGuid().ToString() + "." + ext;
                var path = Server.MapPath("/doc/" + folder + "/") + fileName;
                byte[] data = Convert.FromBase64String(imagedata);

                // Saving Image in Original Mode
                System.IO.File.WriteAllBytes(path, data);
            }
            JsonUploadResult result = new JsonUploadResult();
            result.idx = idx;
            result.filename = fileName;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private string getUploadFolder(int fileType)
        {
            string folder = "mgn"; // fileType = 0
            //if (fileType == 1)
            //    folder = "Promo";
            //else if (fileType == 2)
            //    folder = "Member";
            //else if (fileType == 3)
            //    folder = "Asset";
            //else if (fileType == 4)
            //    folder = "Employee";
            //else if (fileType == 5)
            //    folder = "Partner";
            //else if (fileType == 6)
            //    folder = "MemberIdentity";
            if (fileType == 7)
                folder = "News";
            else if (fileType == 8)
                folder = "PodCast";
            else if (fileType == 9)
                folder = "company";
            //else if (fileType == 10)
            //    folder = "banner";
            //else if (fileType == 11)
            //    folder = "donation";
            //else if (fileType == 12)
            //    folder = "registration";
            //else if (fileType == 13)
            //    folder = "student";

            return folder;
        }
    }
}