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
    public class DocumentController : Controller
    {

        private DtoMemberLogin getUserLogin()
        {
            LoginController login = new LoginController();
            return login.GetUser(this);
        }

        public ActionResult Test(string mid)
        {
            return View();
        }
        public ActionResult Test6(string mid)
        {
            return View();
        }
        public ActionResult Test7(string mid)
        {
            return View();
        }
        public ActionResult TestX(string mid)
        {
            return View();
        }
        public ActionResult Document(string mid)
        {
            LoginController login = new LoginController();
            login.CheckLogin(this);

            // begin decription menu
            DtoMemberLogin user = login.GetUser(this);
            var strmenu = login.ManipulateSubMenu(this, user, mid);
            // end decription menu

            DtoDocument doc = new DtoDocument();
            string[] ids = strmenu.Split(',');
            if (ids.Length > 1 && !ids[1].Equals("0"))
            {
                DocumentService psvr = new DocumentService();
                doc = psvr.GetById(int.Parse(ids[1]));
            }

            JsonLayout layout = new JsonLayout();
            layout.activeId = int.Parse(ids[0]);
            layout.key = mid.Split(',')[0];
            layout.menus = login.GetMenus(this, layout.activeId);
            layout.user = login.GetUser(this);
            layout.obj = doc;

            return View(layout);
        }

        public ActionResult DocumentList(string mid)
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
            layout.user = user;

            return View(layout);
        }

        public ActionResult XPdfViewer(long documentId, long memberId, int type)
        {
            JsonLayout layout = new JsonLayout();
            DocumentService dsvr = new DocumentService();
            DtoDocument doc = dsvr.GetById(documentId);
            MemberService msvr = new MemberService();
            DtoMemberLogin user = msvr.GetById(memberId);
            LoginController login = new LoginController();
            login.SetLogin(this, user);
            layout.activeId = 0;
            layout.menus = null;
            layout.user = user;
            layout.obj = doc;
            layout.dataId = type;

            return View(layout);
        }


        public ActionResult GetLiteAll(string topCriteria, int page, int pageSize)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new DocumentService();
            var data = srv.GetLiteAll(user.Id, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLiteAllCount(string topCriteria)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new DocumentService();
            var data = srv.GetLiteAllCount(user.Id, topCriteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult GetLiteAll2(string topCriteria, int page, int pageSize, string criteria)
        //{
        //    LoginController login = new LoginController();
        //    DtoMemberLogin user = login.GetUser(this);
        //    var srv = new DocumentService();
        //    var data = srv.GetLiteSelectedAll(user.Id, topCriteria, page, pageSize, null, criteria);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        //public ActionResult GetLiteAllCount2(string topCriteria, string criteria)
        //{
        //    LoginController login = new LoginController();
        //    DtoMemberLogin user = login.GetUser(this);
        //    var srv = new DocumentService();
        //    var data = srv.GetLiteSellectedAllCount(user.Id, topCriteria, criteria);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult GetLiteAll3(string topCriteria, int page, int pageSize, string criteria)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new DocumentService();
            var data = srv.GetLiteByCreator(user.Id, topCriteria, page, pageSize, null, criteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLiteAllCount3(string topCriteria, string criteria)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new DocumentService();
            var data = srv.GetLiteByCreatorCount(user.Id, topCriteria, criteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDocument(long rotationNodeId, long documentId)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new DocumentService();
            var data = srv.GetById(documentId);
            
            data.DocumentMember.FlagPermission = srv.GetPermission(user.Id, rotationNodeId, documentId);

            var fname = OpenFile(data.FileName);
            data.FileName = fname;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDocumentView(long Id)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new DocumentService();
            var data = srv.GetById(Id);
            var fname = OpenFile(data.FileName);
            data.FileName = fname;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult GetPermission(long rotationNodeId, long documentId)
        //{
        //    var srv = new DocumentService();
        //    LoginController login = new LoginController();
        //    DtoMemberLogin user = login.GetUser(this);
        //    var data = srv.GetPermission(user.Id, rotationNodeId, documentId);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult GetSignatureDocs(string topCriteria, int page, int pageSize)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new DocumentService();
            var data = srv.GetSignatureDocs(user.Id, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAnnotateDocs(string topCriteria, int page, int pageSize)
        {
            LoginController login = new LoginController();
            DtoMemberLogin user = login.GetUser(this);
            var srv = new DocumentService();
            var data = srv.GetAnnotateDocs(user.Id, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prod"></param>
        /// <returns></returns>
        /// 

        public ActionResult CheckingSignature(long memberId)
        {
            if (memberId == 0)
            {
                DtoMemberLogin user = getUserLogin();
                memberId = user.Id;
            }
            var srv = new DocumentService();
            var data = srv.CheckingSignature(memberId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckingPrivateStamp(long memberId)
        {
            if (memberId == 0)
            {
                DtoMemberLogin user = getUserLogin();
                memberId = user.Id;
            }
            var srv = new DocumentService();
            var data = srv.CheckingPrivateStamp(memberId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(DtoDocument prod)
        {
            DtoMemberLogin user = getUserLogin();
            prod.CreatorId = user.Id;
            prod.UserId = user.Email;
            prod.CompanyId = (long)user.CompanyId;
            var srv = new DocumentService();
            var data = srv.Save(prod);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult Signature(long documentId)
        //{
        //    DtoMemberLogin user = getUserLogin();
        //    var srv = new DocumentService();
        //    var data = srv.Signature(documentId, user.Id);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult SaveCxPrint(string docName)
        {
            DtoMemberLogin user = getUserLogin();
            var srv = new DocumentService();
            var data = srv.SaveCxPrint(docName, user.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveCxDownload(string docName)
        {
            DtoMemberLogin user = getUserLogin();
            var srv = new DocumentService();
            var data = srv.SaveCxDownload(docName, user.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SaveAnnos(long documentId, long creatorId, IEnumerable<DtoDocumentAnnotate> annos)
        {
            var srv = new DocumentService();
            var data = srv.SaveAnnos(documentId, creatorId, "ANDRO", annos);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        //public ActionResult Move(string docIds, long folderId)
        //{
        //    LoginController login = new LoginController();
        //    DtoMemberLogin user = login.GetUser(this);
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
        //    DtoDocumentLite doc = docsvr.GetByUniqFileName(ufileName, isDocument);
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
        //    DtoDocumentLite doc = docsvr.GetByUniqFileName(ufileName, isDocument);
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

        public string OpenFile(string fileName)
        {
            // manupulation here
            return fileName;
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