using DRD.Models;
using DRD.Models.Custom;
using DRD.Models.API;
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
        DocumentService documentService;

        private LoginController login;
        private UserSession user;
        private Layout layout;

        //helper
        private bool CheckLogin(bool getMenu = false)
        {
            login = new LoginController();
            if (login.CheckLogin(this))
            {
                //instantiate
                user = login.GetUser(this);
                documentService = new DocumentService();
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
        /// <summary>
        /// API to check if the user already complete thier profile data or not
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult CheckIsUserProfileComplete(long userId)
        {
            CheckLogin();
            if (userId == 0) userId = user.Id;
            var data = documentService.CheckIsUserProfileComplete(userId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Document(long documentId)
        {
            CheckLogin();
            Document doc = documentService.GetById(documentId);
            layout.Object = doc;

            return View(layout);
        }

        public ActionResult GetAnnotateDocs(string topCriteria, int page, int pageSize)
        {
            CheckLogin();
            var data = documentService.GetAnnotateDocs(user.Id, topCriteria, page, pageSize);
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
            CheckLogin();
            var data = documentService.GetCompanyDocument(user.Id, searchKeyword, page, pageSize, companyId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDocument(long rotationNodeId, long documentId)
        {
            CheckLogin();
            var data = documentService.GetById(documentId);

            //data.DocumentUser.FlagPermission = srv.GetPermission(user.Id, rotationNodeId, documentId);

            var fname = OpenFile(data.FileName);
            data.FileName = fname;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDocumentView(long Id)
        {
            CheckLogin();
            var data = documentService.GetById(Id);
            var fname = OpenFile(data.FileName);
            data.FileName = fname;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSignatureDocs(string topCriteria, int page, int pageSize)
        {
            CheckLogin();
            var data = documentService.GetSignatureDocs(user.Id, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult List()
        {
            CheckLogin();
            layout.Menus = login.GetMenus(this);
            layout.User = user;
            return RedirectToAction("Index", "Dashboard");
        }

        public string OpenFile(string fileName)
        {
            // manupulation here
            return fileName;
        }

        public ActionResult RequestDownloadDocument(string docName)
        {
            CheckLogin();
            var data = documentService.RequestDownloadDocument(docName, user.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RequestPrintDocument(string docName)
        {
            CheckLogin();
            var data = documentService.RequestPrintDocument(docName, user.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="newDocument"></param>
        /// <returns></returns>
        public ActionResult Save(DocumentInboxData newDocument, long companyId, long rotationId)
        {
            CheckLogin();
            newDocument.CreatorId = user.Id;
            newDocument.UserEmail = user.Email;
            //prod.CompanyId = (long)user.CompanyId;
            var fileController = DependencyResolver.Current.GetService<UpDownFileController>();
            fileController.ControllerContext = new ControllerContext(this.Request.RequestContext, fileController);

            var moveDirResult = fileController.MoveFromTemporaryToActual(newDocument, companyId);
            if (moveDirResult.Equals(Constant.DocumentUploadStatus.OK.ToString()))
            {
                var srv = new DocumentService();
                var data = srv.Save(newDocument, companyId, rotationId);
                data.Status = moveDirResult;
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = moveDirResult;
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SetUpDoc(DocumentInboxData newDocument, long companyId, long rotationId)
        {
            CheckLogin();
            newDocument.CreatorId = user.Id;
            newDocument.UserEmail = user.Email;
            //prod.CompanyId = (long)user.CompanyId;

            var srv = new DocumentService();
            var data = srv.SetUpDoc(newDocument, companyId, rotationId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveAnnos(long documentId, long creatorId, IEnumerable<DocumentAnnotationsInboxData> annos)
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
            
            layout.Menus = null;
            layout.Object = doc;
            layout.DataId = type;

            return View(layout);
        }
    }
}