using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Core;

using DRD.Domain;
using System.Net;
using System.IO;

namespace DRD.Web.Controllers
{
    public class DrDriveController : Controller
    {

        private DtoMemberLogin getUserLogin()
        {
            LoginController login = new LoginController();
            return login.GetUser(this);
        }

        public ActionResult Index(string mid)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            // begin decription menu
            DtoMemberLogin user = login.GetUser(this);
            var strmenu = login.ManipulateMenu(this, user, mid);
            // end decription menu

            JsonLayout layout = new JsonLayout();
            layout.activeId = int.Parse(strmenu);
            layout.key = mid;
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.objItems = login.GetMenuObjectItems(layout.menus, layout.activeId);
            layout.user = user;

            return View(layout);
        }

        public ActionResult GetLiteAll(long memberId, long folderId, string topCriteria, int page, int pageSize)
        {
            //LoginController login = new LoginController();
            //DtoMemberLogin user = login.GetUser(this);
            var srv = new DrDriveService();
            var data = srv.GetLiteAll(memberId, folderId, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLiteAllCount(long memberId, long folderId, string topCriteria)
        {
            //LoginController login = new LoginController();
            //DtoMemberLogin user = login.GetUser(this);
            var srv = new DrDriveService();
            var data = srv.GetLiteAllCount(memberId, folderId, topCriteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCounting()
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new DrDriveService();
            var data = srv.GetCounting(user.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Move(string docIds, long folderId)
        {
            //LoginController login = new LoginController();
            //DtoMemberLogin user = login.GetUser(this);
            var srv = new DrDriveService();
            var data = srv.Move(docIds, folderId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult Delete(long id)
        //{
        //    //LoginController login = new LoginController();
        //    //DtoMemberLogin user = login.GetUser(this);
        //    var srv = new DrDriveService();
        //    var data = srv.Delete(id);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult Delete(string docIds)
        {
            //LoginController login = new LoginController();
            //DtoMemberLogin user = login.GetUser(this);
            var srv = new DrDriveService();
            var data = srv.Delete(docIds);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        // upload yang dipake di web dan android
        //[AcceptVerbs(HttpVerbs.Post)]
        //public JsonResult UploadFile(long folderId)
        //{

        //    string folder = "doc/drdrive";

        //    string imgname = string.Empty;
        //    string fileName = string.Empty;
        //    string ext = "";
        //    HttpPostedFile pic = null;
        //    if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
        //    {
        //        pic = System.Web.HttpContext.Current.Request.Files["MyFile"];
        //        if (pic == null || pic.ContentLength <= 0)
        //            pic = System.Web.HttpContext.Current.Request.Files[0];

        //        if (pic.ContentLength > 0)
        //        {
        //            fileName = Path.GetFileName(pic.FileName);
        //            ext = Path.GetExtension(pic.FileName);
        //            if (String.IsNullOrEmpty(ext))
        //                ext = ".png";

        //            imgname = Guid.NewGuid().ToString();
        //            var _comPath = Server.MapPath("/" + folder + "/") + imgname + ext;
        //            imgname = imgname + ext;

        //            ViewBag.Msg = _comPath;
        //            var path = _comPath;

        //            // Saving Image in Original Mode
        //            pic.SaveAs(path);

        //        }
        //    }
        //    JsonUploadResult result = new JsonUploadResult();
        //    result.idx = folderId;
        //    result.filename = imgname;
        //    result.fileext = ext.Replace(".", "");


        //    MemberFolderService fsvr = new MemberFolderService();
        //    var df = fsvr.GetById(folderId);

        //    DrDriveService drdsvr = new DrDriveService();
        //    DtoDrDrive data = new DtoDrDrive();
        //    data.FileName = result.filename;
        //    data.FileNameOri = fileName;
        //    data.ExtFile = result.fileext;
        //    data.FileFlag = 0;
        //    data.FileSize = pic.ContentLength;
        //    data.CxDownload = 0;
        //    data.MemberFolderId = folderId;
        //    data.MemberId = df.MemberId;
            
        //    drdsvr.Save(data);

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}


        //public FileResult DownloadFile(string key)
        //{
        //    /*  in cshtml using jquery (not secure)

        //        var name = "/doc/mgn/" + fname;
        //        var link = document.createElement("a");
        //        link.download = fnameori;
        //        link.href = name;
        //        link.click();
        //    */
        //    LoginController login = new LoginController();
        //    DtoMemberLogin user = login.GetUser(this);

        //    DrDriveService docsvr = new DrDriveService();
        //    DtoDrDrive doc = docsvr.GetByKeyId(key);
        //    byte[] pdfByte = new byte[] { };
        //    if (doc.Id != 0)
        //    {
        //        string filepath = Server.MapPath("/doc/drdrive/" + doc.FileName);
        //        pdfByte = GetBytesFromFile(filepath);

        //        docsvr.SaveCxDownload(doc.Id);
        //    }
        //    return File(pdfByte, "application/" + doc.ExtFile, doc.FileNameOri);
        //}

        public byte[] GetBytesFromFile(string fullFilePath)
        {
            // this method is limited to 2^32 byte files (4.2 GB)
            FileStream fs = null;
            try
            {
                fs = System.IO.File.OpenRead(fullFilePath);
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                return bytes;
            }
            catch (Exception x)
            {
                return new byte[] { };
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }
    }
}