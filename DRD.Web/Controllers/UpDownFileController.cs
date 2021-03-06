﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Core;

using DRD.Domain;
using System.Net;
using System.IO;
using System.Based.Core;
using System.Text;

namespace DRD.Web.Controllers
{
    public class UpDownFileController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private DtoMemberLogin getUserLogin()
        {
            LoginController login = new LoginController();
            return login.GetUser(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult XUpload(int idx, int fileType)
        {
            string folder = "doc/mgn"; // fileType = 0
            JsonUploadResult result = new JsonUploadResult();
            string _imgname = string.Empty;
            string _ext = "";
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var pic = System.Web.HttpContext.Current.Request.Files["MyImages"];
                if (pic == null || pic.ContentLength <= 0)
                    pic = System.Web.HttpContext.Current.Request.Files[0];

                if (pic.ContentLength > 0)
                {
                    // check doc quota balance
                    LoginController login = new LoginController();
                    DtoMemberLogin user = login.GetUser(this);
                    MemberPlanService mpsvr = new MemberPlanService();
                    DtoMemberPlan plan = mpsvr.GetByMemberId(user.Id);
                    if (plan == null)
                    {
                        result.idx = -2;
                        return Json(result, JsonRequestBehavior.AllowGet); //Invalid member plan
                    }
                    if (plan.StorageSize + plan.StorageSizeAdd - plan.StorageSizeUsed < pic.ContentLength)
                    {
                        result.idx = -4;
                        return Json(result, JsonRequestBehavior.AllowGet); //The used rotation exceeds the data packet quota number
                    }
                    MemberPlanService plansvr = new MemberPlanService();
                    plansvr.DeductPlan(user.Id, pic.ContentLength, "DOCUMENT");
                    //

                    var fileName = Path.GetFileName(pic.FileName);
                    _ext = Path.GetExtension(pic.FileName);
                    if (String.IsNullOrEmpty(_ext))
                        _ext = ".png";

                    _imgname = Guid.NewGuid().ToString();
                    var _comPath = Server.MapPath("/" + folder + "/") + _imgname + _ext;
                    _imgname = _imgname + _ext;

                    ViewBag.Msg = _comPath;
                    var path = _comPath;

                    XFEncryptionHelper xf = new XFEncryptionHelper();
                    var xresult = xf.FileEncryptRequest(Request, path);

                }
            }

            result.idx = idx;
            result.filename = _imgname;
            result.fileext = _ext.Replace(".", "");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult XUploadFree(int idx, int fileType)
        {
            string folder = "doc/mgn"; // fileType = 0
            JsonUploadResult result = new JsonUploadResult();
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

                    XFEncryptionHelper xf = new XFEncryptionHelper();
                    var xresult = xf.FileEncryptRequest(Request, path);

                }
            }

            result.idx = idx;
            result.filename = _imgname;
            result.fileext = _ext.Replace(".", "");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult XUploadDrDrive(long folderId)
        {
            string folder = "doc/drdrive";
            JsonUploadResult result = new JsonUploadResult();
            string imgname = string.Empty;
            string fileName = string.Empty;
            string ext = "";
            HttpPostedFile pic = null;
            int fileSize = 0;
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                pic = System.Web.HttpContext.Current.Request.Files["MyFile"];
                if (pic == null || pic.ContentLength <= 0)
                    pic = System.Web.HttpContext.Current.Request.Files[0];
                fileSize = pic.ContentLength;
                if (pic.ContentLength > 0)
                {
                    // check doc quota balance
                    LoginController login = new LoginController();
                    DtoMemberLogin user = login.GetUser(this);
                    MemberPlanService mpsvr = new MemberPlanService();
                    DtoMemberPlan plan = mpsvr.GetByMemberId(user.Id);
                    if (plan == null)
                    {
                        result.idx = -2;
                        return Json(result, JsonRequestBehavior.AllowGet); //Invalid member plan
                    }
                    if (plan.DrDriveSize + plan.DrDriveSizeAdd - plan.DrDriveSizeUsed < fileSize)
                    {
                        result.idx = -4;
                        return Json(result, JsonRequestBehavior.AllowGet); //The used rotation exceeds the data packet quota number
                    }
                    MemberPlanService plansvr = new MemberPlanService();
                    plansvr.DeductPlan(user.Id, fileSize, "DRDRIVE");
                    //


                    fileName = Path.GetFileName(pic.FileName);
                    ext = Path.GetExtension(pic.FileName);
                    if (String.IsNullOrEmpty(ext))
                        ext = ".png";

                    imgname = Guid.NewGuid().ToString();
                    var _comPath = Server.MapPath("/" + folder + "/") + imgname + ext;
                    imgname = imgname + ext;

                    ViewBag.Msg = _comPath;
                    var path = _comPath;

                    XFEncryptionHelper xf = new XFEncryptionHelper();
                    var xresult = xf.FileEncryptRequest(Request, path);

                }
            }

            result.idx = folderId;
            result.filename = imgname;
            result.fileext = ext.Replace(".", "");


            MemberFolderService fsvr = new MemberFolderService();
            var df = fsvr.GetById(folderId);

            DrDriveService drdsvr = new DrDriveService();
            DtoDrDrive data = new DtoDrDrive();
            data.FileName = result.filename;
            data.FileNameOri = fileName;
            data.ExtFile = result.fileext;
            data.FileFlag = 0;
            data.FileSize = fileSize;
            data.CxDownload = 0;
            data.MemberFolderId = folderId;
            data.MemberId = df.MemberId;

            drdsvr.Save(data);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Upload(int idx, int fileType)
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
                }
            }
            JsonUploadResult result = new JsonUploadResult();
            result.idx = idx;
            result.filename = _imgname;
            result.fileext = _ext.Replace(".", "");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UploadImage(int idx, int fileType)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ufileName"></param>
        /// <param name="isDocument"></param>
        /// <returns></returns>
        public FileResult XDownload(string ufileName, bool isDocument)
        {
            //LoginController login = new LoginController();
            //login.CheckLogin(this);

            DocumentService docsvr = new DocumentService();
            DtoDocumentLite doc = docsvr.GetByUniqFileName(ufileName, isDocument);
            byte[] pdfByte = new byte[] { };
            if (doc.Id != 0)
            {
                string filepath = Server.MapPath("/doc/mgn/" + doc.FileName);

                XFEncryptionHelper xf = new XFEncryptionHelper();
                var xresult = xf.FileDecryptRequest(ref pdfByte, filepath);
            }
            return File(pdfByte, "application/" + doc.ExtFile, doc.FileNameOri);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyf"></param>
        /// <returns></returns>
        public string XGetPdfData(string keyf)
        {
            ////LoginController login = new LoginController();
            ////login.CheckLogin(this);

            //DocumentService docsvr = new DocumentService();
            //DtoDocumentLite doc = docsvr.GetByUniqFileName(keyf, true);
            //byte[] pdfByte = new byte[] { };
            //if (doc.Id != 0)
            //{
            //    string filepath = Server.MapPath("/doc/mgn/" + doc.FileName);

            //    XFEncryptionHelper xf = new XFEncryptionHelper();
            //    var xresult = xf.FileDecryptRequest(ref pdfByte, filepath);
            //}
            //return Convert.ToBase64String(pdfByte);

            //LoginController login = new LoginController();
            //login.CheckLogin(this);

            byte[] pdfByte = new byte[] { };

            string filepath = Server.MapPath("/doc/mgn/" + keyf);

            XFEncryptionHelper xf = new XFEncryptionHelper();
            var xresult = xf.FileDecryptRequest(ref pdfByte, filepath);

            return Convert.ToBase64String(pdfByte);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public FileResult XDownloadDrDrive(string key)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);

            DrDriveService docsvr = new DrDriveService();
            DtoDrDrive doc = docsvr.GetByKeyId(key);
            byte[] pdfByte = new byte[] { };
            if (doc.Id != 0)
            {
                string filepath = Server.MapPath("/doc/drdrive/" + doc.FileName);
                XFEncryptionHelper xf = new XFEncryptionHelper();
                var xresult = xf.FileDecryptRequest(ref pdfByte, filepath);

                docsvr.SaveCxDownload(doc.Id);
            }
            return File(pdfByte, "application/" + doc.ExtFile, doc.FileNameOri);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ufileName"></param>
        /// <param name="isDocument"></param>
        /// <returns></returns>
        public FileResult Download(string ufileName, bool isDocument)
        {
            /*  in cshtml using jquery (not secure)

                var name = "/doc/mgn/" + fname;
                var link = document.createElement("a");
                link.download = fnameori;
                link.href = name;
                link.click();
            */
            //LoginController login = new LoginController();
            //login.CheckLogin(this);

            DocumentService docsvr = new DocumentService();
            DtoDocumentLite doc = docsvr.GetByUniqFileName(ufileName, isDocument);
            byte[] pdfByte = new byte[] { };
            if (doc.Id != 0)
            {
                string filepath = Server.MapPath("/doc/mgn/" + doc.FileName);

                XFEncryptionHelper xf = new XFEncryptionHelper();
                var xresult = xf.FileDecrypt(filepath);
                pdfByte = GetBytesFromFile(filepath);
            }
            return File(pdfByte, "application/" + doc.ExtFile, doc.FileNameOri);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullFilePath"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
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