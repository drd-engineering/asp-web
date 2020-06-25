using DRD.Models;
using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace DRD.App.Controllers
{
    public class DocumentController : Controller
    {
        private Layout layout = new Layout();
        private LoginController login = new LoginController();
        private UserSession user;

        public ActionResult CheckingPrivateStamp(long memberId)
        {
            if (memberId == 0)
            {
                UserSession user = getUserLogin();
                memberId = user.Id;
            }
            var srv = new DocumentService();
            var data = srv.CheckingPrivateStamp(memberId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckingSignature(long memberId)
        {
            if (memberId == 0)
            {
                UserSession user = getUserLogin();
                memberId = user.Id;
            }
            var srv = new DocumentService();
            var data = srv.CheckingSignature(memberId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Document(long documentId)
        {
            Document doc = new Document();

            DocumentService psvr = new DocumentService();
            doc = psvr.GetById(documentId);
            layout.obj = doc;

            return View(layout);
        }

        public ActionResult GetAnnotateDocs(string topCriteria, int page, int pageSize)
        {
            LoginController login = new LoginController();
            UserSession user = login.GetUser(this);
            var srv = new DocumentService();
            var data = srv.GetAnnotateDocs(user.Id, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

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
            catch (Exception)
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

        public ActionResult GetCompanyDocument(string searchKeyword, int page, int pageSize, long companyId)
        {
            LoginController login = new LoginController();
            UserSession user = login.GetUser(this);
            var srv = new DocumentService();
            var data = srv.GetCompanyDocument(user.Id, searchKeyword, page, pageSize, companyId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDocument(long rotationNodeId, long documentId)
        {
            LoginController login = new LoginController();
            UserSession user = login.GetUser(this);
            var srv = new DocumentService();
            var data = srv.GetById(documentId);

            //data.DocumentUser.FlagPermission = srv.GetPermission(user.Id, rotationNodeId, documentId);

            var fname = OpenFile(data.FileName);
            data.FileName = fname;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDocumentView(long Id)
        {
            LoginController login = new LoginController();
            UserSession user = login.GetUser(this);
            var srv = new DocumentService();
            var data = srv.GetById(Id);
            var fname = OpenFile(data.FileName);
            data.FileName = fname;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLiteAll(string searchKeyword, int page, int pageSize)
        {
            LoginController login = new LoginController();
            UserSession user = login.GetUser(this);
            var srv = new DocumentService();
            var data = srv.GetLiteAll(user.Id, searchKeyword, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLiteAll3(string topCriteria, int page, int pageSize, string criteria)
        {
            LoginController login = new LoginController();
            UserSession user = login.GetUser(this);
            var srv = new DocumentService();
            var data = srv.GetLiteByCreator(user.Id, topCriteria, page, pageSize, null, criteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult GetLiteAllCount(string topCriteria)
        //{
        //    LoginController login = new LoginController();
        //    UserSession user = login.GetUser(this);
        //    var srv = new DocumentService();
        //    var data = srv.GetLiteAllCount(user.Id, topCriteria);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult GetLiteAllCount(string topCriteria)
        {
            LoginController login = new LoginController();
            UserSession user = login.GetUser(this);
            var srv = new DocumentService();
            var data = srv.GetAllCount(user.Id, topCriteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult GetLiteAll(string topCriteria, int page, int pageSize)
        //{
        //    LoginController login = new LoginController();
        //    UserSession user = login.GetUser(this);
        //    var srv = new DocumentService();
        //    var data = srv.GetLiteAll(user.Id, topCriteria, page, pageSize);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult GetLiteAllCount3(string topCriteria, string criteria)
        {
            LoginController login = new LoginController();
            UserSession user = login.GetUser(this);
            var srv = new DocumentService();
            var data = srv.GetLiteByCreatorCount(user.Id, topCriteria, criteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSignatureDocs(string topCriteria, int page, int pageSize)
        {
            LoginController login = new LoginController();
            UserSession user = login.GetUser(this);
            var srv = new DocumentService();
            var data = srv.GetSignatureDocs(user.Id, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
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

        public ActionResult List()
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            // begin decription menu
            UserSession user = login.GetUser(this);

            // end decription menu

            Layout layout = new Layout();
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = user;
            return RedirectToAction("Index", "Dashboard");

            //return View(layout);
        }

        public string OpenFile(string fileName)
        {
            // manupulation here
            return fileName;
        }

        public ActionResult RequestDownloadDocument(string docName)
        {
            UserSession user = getUserLogin();
            var srv = new DocumentService();
            var data = srv.RequestDownloadDocument(docName, user.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RequestPrintDocument(string docName)
        {
            UserSession user = getUserLogin();
            var srv = new DocumentService();
            var data = srv.RequestPrintDocument(docName, user.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult GetPermission(long rotationNodeId, long documentId)
        //{
        //    var srv = new DocumentService();
        //    LoginController login = new LoginController();
        //    UserSession user = login.GetUser(this);
        //    var data = srv.GetPermission(user.Id, rotationNodeId, documentId);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        /// <summary>
        ///
        /// </summary>
        /// <param name="prod"></param>
        /// <returns></returns>
        ///
        public ActionResult Save(DocumentInboxData prod, long companyId, long rotationId)
        {
            Initialize();
            prod.CreatorId = user.Id;
            prod.UserEmail = user.Email;
            //prod.CompanyId = (long)user.CompanyId;
            var fileController = DependencyResolver.Current.GetService<UpDownFileController>();
            fileController.ControllerContext = new ControllerContext(this.Request.RequestContext, fileController);

            var moveDirResult = fileController.MoveFromTemporaryToActual(prod, companyId);
            if (moveDirResult.Equals(Constant.DocumentUploadStatus.OK.ToString()))
            {
                var srv = new DocumentService();
                var data = srv.Save(prod, companyId, rotationId);
                data.status = moveDirResult;
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = moveDirResult;
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        //public ActionResult Signature(long documentId)
        //{
        //    UserSession user = getUserLogin();
        //    var srv = new DocumentService();
        //    var data = srv.Signature(documentId, user.Id);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult SaveAnnos(long documentId, long creatorId, IEnumerable<DocumentElementInboxData> annos)
        {
            var srv = new DocumentService();
            var data = srv.SaveAnnos(documentId, creatorId, "ANDRO", annos);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult XPdfViewer(long documentId, long memberId, int type)
        {
            Layout layout = new Layout();
            DocumentService dsvr = new DocumentService();
            Document doc = dsvr.GetById(documentId);
            UserService msvr = new UserService();
            //UserSession user = msvr.GetById(memberId);
            LoginController login = new LoginController();
            //login.SetLogin(this, user);
            layout.activeId = 0;
            layout.menus = null;
            //layout.user = user;
            layout.obj = doc;
            layout.dataId = type;

            return View(layout);
        }

        private UserSession getUserLogin()
        {
            LoginController login = new LoginController();
            return login.GetUser(this);
        }
        //public ActionResult Move(string docIds, long folderId)
        //{
        //    LoginController login = new LoginController();
        //    UserSession user = login.GetUser(this);
        //    var srv = new DocumentService();
        //    var data = srv.Move(user.Id, docIds, folderId);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public JsonResult UploadDocument(int idx, int fileType)
        //{
        //    string folder = "doc/mgn"; // fileType = 0

        //    if (fileType == 100)
        //        folder = "doc/drdrive";

        //    string _imgname = string.Empty;
        //    string _ext = "";
        //    if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
        //    {
        //        var pic = System.Web.HttpContext.Current.Request.Files["MyImages"];
        //        if (pic == null || pic.ContentLength <= 0)
        //            pic = System.Web.HttpContext.Current.Request.Files[0];

        //        if (pic.ContentLength > 0)
        //        {
        //            var fileName = Path.GetFileName(pic.FileName);
        //            _ext = Path.GetExtension(pic.FileName);
        //            if (String.IsNullOrEmpty(_ext))
        //                _ext = ".png";

        //            _imgname = Guid.NewGuid().ToString();
        //            var _comPath = Server.MapPath("/" + folder + "/") + _imgname + _ext;
        //            _imgname = _imgname + _ext;

        //            ViewBag.Msg = _comPath;
        //            var path = _comPath;

        //            XFEncryptionHelper xf = new XFEncryptionHelper();
        //            var xresult = xf.FileEncryptRequest(Request, path);

        //        }
        //    }
        //    JsonUploadResult result = new JsonUploadResult();
        //    result.idx = idx;
        //    result.filename = _imgname;
        //    result.fileext = _ext.Replace(".", "");
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        //public FileResult DownloadDocument(string ufileName, bool isDocument)
        //{
        //    LoginController login = new LoginController();
        //    login.CheckLogin(this);

        //    DocumentService docsvr = new DocumentService();
        //    DocumentLite doc = docsvr.GetByUniqFileName(ufileName, isDocument);
        //    byte[] pdfByte = new byte[] { };
        //    if (doc.Id != 0)
        //    {
        //        string filepath = Server.MapPath("/doc/mgn/" + doc.FileName);

        //        XFEncryptionHelper xf = new XFEncryptionHelper();
        //        var xresult = xf.FileDecryptRequest(ref pdfByte, filepath);
        //    }
        //    return File(pdfByte, "application/" + doc.ExtFile, doc.FileNameOri);
        //}

        //public FileResult DownloadDocumentX(string ufileName, bool isDocument)
        //{
        //    /*  in cshtml using jquery (not secure)

        //        var name = "/doc/mgn/" + fname;
        //        var link = document.createElement("a");
        //        link.download = fnameori;
        //        link.href = name;
        //        link.click();
        //    */
        //    LoginController login = new LoginController();
        //    login.CheckLogin(this);

        //    DocumentService docsvr = new DocumentService();
        //    DocumentLite doc = docsvr.GetByUniqFileName(ufileName, isDocument);
        //    byte[] pdfByte = new byte[] { };
        //    if (doc.Id != 0)
        //    {
        //        string filepath = Server.MapPath("/doc/mgn/" + doc.FileName);

        //        XFEncryptionHelper xf = new XFEncryptionHelper();
        //        var xresult = xf.FileDecrypt(filepath);
        //        pdfByte = GetBytesFromFile(filepath);
        //    }
        //    return File(pdfByte, "application/" + doc.ExtFile, doc.FileNameOri);
        //}
    }
}