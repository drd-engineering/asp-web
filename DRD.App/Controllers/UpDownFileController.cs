using DRD.Models;
using DRD.Models.API;
using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace DRD.App.Controllers
{
    public class UpDownFileController : Controller
    {
        private Layout layout = new Layout();
        private LoginController login = new LoginController();
        private UserSession user;

        private SubscriptionService subscriptionService = new SubscriptionService();
        private DocumentService documentService = new DocumentService();
        
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
                            var inerFile = archive.CreateEntry(doc.FileNameOri + doc.Extension, CompressionLevel.Fastest);
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
            layout.Menus = login.GetMenus(this);
            layout.User = login.GetUser(this);
        }

        public void InitializeAPI()
        {
            user = login.GetUser(this);
            login.CheckLogin(this);
        }

        public String MoveFromTemporaryToActual(DocumentInboxData newDocument, long companyId)
        {
            string Tranfiles, ProcessedFiles;
            //Tranfiles = Server.MapPath(@"~\godurian\sth100\transfiles\" + Filename);
            var tempFolder = "doc/company/temp";
            var encryptedCompanyId = Utilities.Encrypt(companyId.ToString());
            // FirstTime create the directory
            var actualPath = "doc/company/" + encryptedCompanyId;
            var targetDir = "/" + actualPath + "/";
            bool exists = System.IO.Directory.Exists(Server.MapPath(targetDir));
            if (!exists)
                System.IO.Directory.CreateDirectory(Server.MapPath(targetDir));

            // always start with version 0.0
            actualPath = "doc/company/" + encryptedCompanyId + "/v0.0";
            targetDir = "/" + actualPath + "/";
            exists = System.IO.Directory.Exists(Server.MapPath(targetDir));
            if (!exists)
                System.IO.Directory.CreateDirectory(Server.MapPath(targetDir));
            try
            {
                Tranfiles = Server.MapPath("/" + tempFolder + "/") + newDocument.FileUrl + ".drd";

                // check storage quota
                Constant.BusinessUsageStatus SubscriptionStatus = subscriptionService.CheckOrAddSpecificUsage(DRD.Models.Constant.BusinessPackageItem.Storage, companyId, newDocument.FileSize, true);
                var status = SubscriptionStatus.ToString();

                if (!SubscriptionStatus.Equals(Constant.BusinessUsageStatus.OK))
                {
                    return status; //The used rotation exceeds the data packet quota number
                }
                
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
                    return Constant.DocumentUploadStatus.OK.ToString();
                }
                return Constant.DocumentUploadStatus.NOT_FOUND.ToString();
            }
            catch (Exception)
            {
                return Constant.DocumentUploadStatus.SERVER_ERROR.ToString();
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
            result.Id = idx;
            result.FileUrl = _imgname;
            result.FileExtension = _ext.Replace(".", "");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// API service to get pdfstring data using unique filename
        /// </summary>
        /// <param name="keyf"></param>
        /// <returns></returns>
        public string XGetPdfData(string fileUrlName, bool isNew)
        {
            Initialize();

            DocumentService docsvr = new DocumentService();
            DocumentItem doc = docsvr.GetByUniqFileName(fileUrlName, true, isNew);

            string filePath = "";
            if (isNew)
                filePath = Server.MapPath("/doc/company/temp/" + doc.FileName);
            else
                filePath = Server.MapPath("/doc/company/" + doc.EncryptedId + "/v" + doc.LatestVersion + "/" + doc.FileName);

            string result = OpenPdfFileAsString(filePath);
            return result;
        }
        private string GetPdfData(string fileUrlName, bool isNew)
        {
            DocumentService docsvr = new DocumentService();
            DocumentItem doc = docsvr.GetByUniqFileName(fileUrlName, true, isNew);

            string filePath = "";
            if (isNew)
                filePath = Server.MapPath("/doc/company/temp/" + doc.FileName);
            else
                filePath = Server.MapPath("/doc/company/" + doc.EncryptedId + "/v" + doc.LatestVersion + "/" + doc.FileName);

            string result = OpenPdfFileAsString(filePath);
            return result;
        }
        private string OpenPdfFileAsString(string filePath)
        {
            byte[] pdfByte = new byte[] { };
            XFEncryptionHelper xf = new XFEncryptionHelper();
            var xresult = xf.FileDecryptRequest(ref pdfByte, filePath);
            if (xresult.Equals("OK"))
            {
                return Convert.ToBase64String(pdfByte);
            }
            else
                return Convert.ToBase64String(new byte[] { });
        }
        async public Task<bool> CreatePdfAnnotated(string pdfString, DocumentInboxData doc)
        {
            RequestToAnnotatePdf requestData = new RequestToAnnotatePdf(pdfString, doc.DocumentAnnotations);
            HttpClient client = new HttpClient();

            var stringPayload = JsonConvert.SerializeObject(requestData);

            var content = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://127.0.0.1:8000/makepdf/", content);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                System.Diagnostics.Debug.WriteLine(response.Content);
                return false;
            }
            var value = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(value);
            var newPdfString = result["result"];

            var ok = await Task.Run(()=>SavePdfToDirectory(newPdfString, doc));
            if (!ok)
                return false;
            var saved = await Task.Run(() => documentService.UpdateVersion(doc.Id, "1.0"));
            if (saved == 0)
                return false;
            return true;
        }
        /// <summary>
        /// save pdfstring to directory
        /// </summary>
        /// <param name="pdfString"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public bool SavePdfToDirectory(string pdfString, DocumentInboxData document)
        {
            //initiating variable folder and doc
            string folder = "doc/company/" + document.EncryptedCompanyId;

            var targetdir = "/" + folder + "/";
            bool exists = Directory.Exists(Server.MapPath(targetdir));
            if (!exists)
                Directory.CreateDirectory(Server.MapPath(targetdir));

            // Save to version 1.0 this will need to be reviewed
            folder = "doc/company/" + document.EncryptedCompanyId + "/v1.0";

            targetdir = "/" + folder + "/";
            exists = Directory.Exists(Server.MapPath(targetdir));
            if (!exists)
                Directory.CreateDirectory(Server.MapPath(targetdir));

            //checking pdf string
            if (pdfString != null || pdfString != "")
            {
                byte[] bytes = Convert.FromBase64String(pdfString);
                Stream fileStream = new MemoryStream(bytes);

                //generate unique identity for each file
                var _comPath = Server.MapPath("/" + folder + "/") + document.FileUrl;
                var path = _comPath;

                XFEncryptionHelper xf = new XFEncryptionHelper();
                var xresult = xf.FileStreamEncrypt(fileStream, path);
                if (xresult == "OK")
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        /// <summary>
        /// This is asyncronous task to creating all final document in rotation
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public async Task<bool> FinalizeDocuments(IEnumerable<RotationNodeDoc> items)
        {
            List<Task> taskToCreateDocumentWithAnnotation = new List<Task>();
            foreach (RotationNodeDoc item in items)
            {
                var doc = documentService.GetDocumentInboxData(item.DocumentId);
                if (!doc.IsCurrent) { continue; }
                var pdfString = GetPdfData(doc.FileUrl, false);
                var anno = doc.DocumentAnnotations;
                Task pdfNew = CreatePdfAnnotated(pdfString, doc);
                taskToCreateDocumentWithAnnotation.Add(pdfNew);
            }
            while (taskToCreateDocumentWithAnnotation.Count > 0)
            {
                Task finishedTask = await Task.WhenAny(taskToCreateDocumentWithAnnotation);
                taskToCreateDocumentWithAnnotation.Remove(finishedTask);
            }
            return true;
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
            bool exists = Directory.Exists(Server.MapPath(targetdir));
            if (!exists)
                Directory.CreateDirectory(Server.MapPath(targetdir));

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
                    var SubscriptionStatus = subscriptionService.CheckOrAddSpecificUsage(DRD.Models.Constant.BusinessPackageItem.Storage, companyId, file.ContentLength);
                    result.Status = SubscriptionStatus.ToString();

                    if (!SubscriptionStatus.Equals(Constant.BusinessUsageStatus.OK))
                    {
                        result.Id = (int)SubscriptionStatus;
                        return Json(result, JsonRequestBehavior.AllowGet); //The used rotation exceeds the data packet quota number
                    }

                    _ext = Path.GetExtension(file.FileName);
                    var fileName = Path.GetFileName(file.FileName);
                    filenameori = fileName;

                    //meant for uploaded signature and initial
                    if (String.IsNullOrEmpty(_ext))
                        _ext = ".png";

                    //generate unique identity for each file
                    _filename = Guid.NewGuid().ToString();
                    var _comPath = Server.MapPath("/" + folder + "/") + _filename + _ext;
                    _filename = _filename + _ext;
                    ViewBag.Msg = _comPath;
                    var path = _comPath;

                    XFEncryptionHelper xf = new XFEncryptionHelper();
                    var xresult = xf.FileEncryptRequest(Request, path);
                }
                else
                {
                    result.Id = -5;
                    return Json(result, JsonRequestBehavior.AllowGet); //Invalid member plan
                }
            }
            result.Id = idx;
            result.FileUrl = _filename;
            result.FileName = filenameori;
            result.FileExtension = _ext.Replace(".", "");
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

            result.Id = idx;
            result.FileUrl = _imgname;
            result.FileExtension = _ext.Replace(".", "");
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