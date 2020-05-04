using DRD.Models;
using DRD.Models.API;
using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web.Mvc;

namespace DRD.App.Controllers
{
    public class UpDownFileController : Controller
    {
        private Layout layout = new Layout();
        private LoginController login = new LoginController();
        private UserSession user;
        public ActionResult DownloadAllDocumentfromCompany(long companyId)
        {
            var companyServ = new CompanyService();
            var companyItem = companyServ.GetCompany(companyId);
            if (companyItem != null)
            {
                var folder = "doc/company";
                var encryptedId = Utilities.Encrypt(companyItem.Id.ToString());
                var pathTarget = "/" + folder + "/" + encryptedId + "/";
                var docServ = new DocumentService();
                var allDocument = docServ.GetAllCompanyDocument(companyItem.Id);
                using (var memoryStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var doc in allDocument)
                        {
                            byte[] pdfByte = new byte[] { };
                            if (doc.Id != 0)
                            {
                                string filepath = Server.MapPath(pathTarget) + doc.FileName;

                                XFEncryptionHelper xf = new XFEncryptionHelper();
                                var xresult = xf.FileDecryptRequest(ref pdfByte, filepath);
                            }
                            var inerFile = archive.CreateEntry(doc.FileNameOri + doc.Extention, CompressionLevel.Fastest);
                            using (var entryStream = inerFile.Open())
                            using (var streamWriter = new StreamWriter(entryStream))
                            {
                                streamWriter.BaseStream.Write(pdfByte, 0, pdfByte.Length);
                            }
                        }
                    }
                    return File(memoryStream.ToArray(), "application/zip", companyItem.Name + ".zip");
                }
            }
            else return null;
        }

        public void Initialize()
        {
            user = login.GetUser(this);
            login.CheckLogin(this);
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = login.GetUser(this);
        }

        public void InitializeAPI()
        {
            user = login.GetUser(this);
            login.CheckLogin(this);
        }

        public int MoveFromTemporaryToActual(DocumentInboxData newDocument, long companyId)
        {
            string Tranfiles, ProcessedFiles;
            //Tranfiles = Server.MapPath(@"~\godurian\sth100\transfiles\" + Filename);
            var tempFolder = "doc/company/temp";
            var encryptedCompanyId = Utilities.Encrypt(companyId.ToString());
            var actualPath = "doc/company/" + encryptedCompanyId;
            var targetDir = "/" + actualPath + "/";
            bool exists = System.IO.Directory.Exists(Server.MapPath(targetDir));
            if (!exists)
                System.IO.Directory.CreateDirectory(Server.MapPath(targetDir));
            try
            {
                Tranfiles = Server.MapPath("/" + tempFolder + "/") + newDocument.FileUrl + ".drd";
                System.Diagnostics.Debug.WriteLine("LOCATION FILE " + Tranfiles);
                if (System.IO.File.Exists(Tranfiles))
                {
                    //Need to mention any file,so that to overwrite this newly created with the actual file,other wise will get 2 errors like
                    //1)Cannot create a file when that file already exists
                    //2)The path....is a folder not a file.
                    //ProcessedFiles = Server.MapPath(@"~\ProcessedFiles"); //Wrong
                    ProcessedFiles = Server.MapPath("/" + actualPath + "/") + newDocument.FileUrl + ".drd";//Need to mention any file in dest folder,even though it doesnt contain this file.

                    //Need to move or overwrite the new file with actual file.
                    System.IO.File.Move(Tranfiles, ProcessedFiles);
                    System.IO.File.Delete(Tranfiles);
                    return 1;
                }
                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
        }

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
            DocUploadResult result = new DocUploadResult();
            result.idx = idx;
            result.filename = _imgname;
            result.fileext = _ext.Replace(".", "");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="keyf"></param>
        /// <returns></returns>
        public string XGetPdfData(string keyf, bool isNew)
        {
            Initialize();

            DocumentService docsvr = new DocumentService();
            DocumentItem doc = docsvr.GetByUniqFileName(keyf, true, isNew);

            byte[] pdfByte = new byte[] { };
            string filepath = "";
            if (isNew)
                filepath = Server.MapPath("/doc/company/temp/" + doc.FileName);
            else
                filepath = Server.MapPath("/doc/company/" + doc.EncryptedId + "/" + doc.FileName);

            XFEncryptionHelper xf = new XFEncryptionHelper();
            var xresult = xf.FileDecryptRequest(ref pdfByte, filepath);
            if (xresult.Equals("OK"))
            {
                System.Diagnostics.Debug.WriteLine("[[FILEPATH]] " + filepath + " [[OPEN DOC PDF]] " + xresult);

                return Convert.ToBase64String(pdfByte);
            }
            else
                return Convert.ToBase64String(new byte[] { });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult XUploadAsTemporary(int idx, int fileType, long companyId)
        {
            //check user session for login status
            Initialize();

            //initiating variable folder and doc
            DocUploadResult result = new DocUploadResult();
            string folder = "doc/company/temp"; // fileType = 0

            var targetdir = "/" + folder + "/";
            bool exists = System.IO.Directory.Exists(Server.MapPath(targetdir));
            if (!exists)
                System.IO.Directory.CreateDirectory(Server.MapPath(targetdir));

            string filenameori = string.Empty;
            string _filename = string.Empty;
            string _ext = "";

            //checking uploaded files through request
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                //get uploaded file
                var file = System.Web.HttpContext.Current.Request.Files["FileUploaded"];
                if (file == null || file.ContentLength <= 0)
                    file = System.Web.HttpContext.Current.Request.Files[0];

                //check file exist
                if (file.ContentLength > 0)
                {
                    // check storage quota
                    var subscriptionService = new SubscriptionService();
                    var usage = subscriptionService.GetActiveBusinessSubscriptionByCompany(companyId);
                    if (usage == null)
                    {
                        result.idx = -2;
                        return Json(result, JsonRequestBehavior.AllowGet); //Invalid member plan
                    }
                    if (usage.StorageLimit - usage.TotalStorage < file.ContentLength)
                    {
                        result.idx = -4;
                        return Json(result, JsonRequestBehavior.AllowGet); //The used rotation exceeds the data packet quota number
                    }

                    var companyService = new CompanyService();
                    _ext = Path.GetExtension(file.FileName);
                    var companyofUser = companyService.GetCompanyItem(companyId);
                    var fileName = Path.GetFileName(file.FileName);
                    filenameori = fileName;

                    //meant for uploaded signature and initial
                    if (String.IsNullOrEmpty(_ext))
                        _ext = ".png";

                    //generate unique identity for each file
                    _filename = Guid.NewGuid().ToString();
                    var _comPath = Server.MapPath("/" + folder + "/") + _filename + _ext;
                    _filename = _filename + _ext;

                    System.Diagnostics.Debug.WriteLine("[[FILE]] " + _filename + " [[EXT]] " + _ext + "  [[PATH]]" + _comPath);

                    ViewBag.Msg = _comPath;
                    var path = _comPath;

                    XFEncryptionHelper xf = new XFEncryptionHelper();
                    var xresult = xf.FileEncryptRequest(Request, path);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Not exist file");
                    result.idx = -5;
                    return Json(result, JsonRequestBehavior.AllowGet); //Invalid member plan
                }
            }

            result.idx = idx;
            result.filename = _filename;
            result.filenameori = filenameori;
            result.fileext = _ext.Replace(".", "");
            return Json(result, JsonRequestBehavior.AllowGet);
            // quota upload belum dipotong
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult XUploadFree(int idx, int fileType)
        {
            string folder = "doc/mgn"; // fileType = 0
            DocUploadResult result = new DocUploadResult();
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
        /// <param name="fileType"></param>
        /// <returns></returns>
        private string GetUploadFolder(int fileType)
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

        private UserSession getUserLogin()
        {
            LoginController login = new LoginController();
            return login.GetUser(this);
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        /*[AcceptVerbs(HttpVerbs.Post)]
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
        }*/
        /*
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
                */
        /*
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
            *//*  in cshtml using jquery (not secure)

                var name = "/doc/mgn/" + fname;
                var link = document.createElement("a");
                link.download = fnameori;
                link.href = name;
                link.click();
            *//*
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
        }*/
    }
}