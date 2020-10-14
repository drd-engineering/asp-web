using DRD.Service.Context;
using DRD.Models.API;
using DRD.Models.View;
using DRD.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DRD.Service
{
    public class DocumentService 
    {
        /// <summary>
        /// GET status user complete their profile or not
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int CheckIsUserProfileComplete(long userId)
        {
            using var db = new Connection();
            var user = db.Users.FirstOrDefault(c => c.Id == userId);
            if (user.SignatureImageFileName == null || user.InitialImageFileName == null || user.KTPImageFileName == null || user.KTPVerificationImageFileName == null || string.IsNullOrEmpty("" + user.OfficialIdNo))
                return (int)Constant.UserProfileStatus.NotComplete;
            return (int)Constant.UserProfileStatus.Complete;
        }
        /// <summary>
        /// GET user permission from rotation user
        /// </summary>
        /// <param name="usrId"></param>
        /// <param name="rotationNodeId"></param>
        /// <param name="documentId"></param>
        /// <returns></returns>
        public int GetPermission(long usrId, long rotationNodeId, long documentId)
        {
            var db = new Connection();
            if (rotationNodeId == 0)
                return 0;

            // if the value of rotationNodeId is minus it means the rotation node id still not found, so it should be searched first
            if (rotationNodeId < 0)
            {
                var rid = Math.Abs(rotationNodeId);
                var rnx = db.RotationNodeDocs.FirstOrDefault(c => c.Document.Id == documentId && c.RotationNode.UserId == usrId && !c.RotationNode.Status.Equals((int)Constant.RotationStatus.Open) && c.RotationNode.Rotation.Id == rid);
                if (rnx == null) return 0;
                // get rotation node id
                rotationNodeId = rnx.RotationNode.Id;
            }
            var rn = db.RotationNodes.FirstOrDefault(c => c.Id == rotationNodeId);
            var rm = db.RotationUsers.FirstOrDefault(c => c.UserId == usrId && c.Rotation.Id == rn.Rotation.Id && c.WorkflowNodeId == rn.WorkflowNode.Id);
            if (rm == null) return 0;
            return rm.ActionPermission;
        }
        /// <summary>
        /// SAVE document data db from uploaded document
        /// </summary>
        /// <param name="prod"></param>
        /// <param name="companyId"></param>
        /// <param name="rotationId"></param>
        /// <returns></returns>
        public DocumentInboxData Save(DocumentInboxData prod, long companyId, long rotationId)
        {
            long result = 0;
            DocumentInboxData document;
            using (var db = new Connection())
            {
                if (prod.Id == 0)
                    document = Create(prod, companyId, rotationId);
                // SHOULD BE REVIEWED THIS UPDATE IS NOT USED YET
                else document = Update(prod, companyId, rotationId);
                if (document.Id != 0)
                    result = document.Id;
                document.DocumentAnnotations = SaveAnnos(document.Id, (long)document.CreatorId, document.UserEmail, prod.DocumentAnnotations);
                document.DocumentUsers = CreateDocumentUserFromAnnotations(document.Id);
                document.DocumentUser = document.DocumentUsers.FirstOrDefault(docusr => docusr.UserId == document.CreatorId);
                if (document.DocumentUser == null) document.DocumentUser = new DocumentUserInboxData(CreateDocumentUser(document.Id, document.CreatorId));
            }
            return document;
        }
        /// <summary>
        /// CREATE new document data in db after uploading document for a company
        /// </summary>
        /// <param name="newDocument"></param>
        /// <param name="companyId"></param>
        /// <param name="rotationId"></param>
        /// <returns></returns>
        private DocumentInboxData Create(DocumentInboxData newDocument, long companyId, long rotationId)
        {
            using var db = new Connection();
            Document document = new Document
            {
                // mapping value
                Extension = newDocument.Extension,
                FileName = newDocument.FileName,
                FileSize = newDocument.FileSize,

                UploaderId = newDocument.CreatorId, // harusnya current user bukan? diinject ke newDocument pas di-controller
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,

                // NEW
                MaximumDownloadPerUser = newDocument.MaximumDownloadPerUser,
                MaximumPrintPerUser = newDocument.MaximumPrintPerUser,
                IsCurrent = true, //

                // upload, get file directory, controller atau di service?
                FileUrl = newDocument.FileUrl,

                RotationId = rotationId,
                CompanyId = companyId,
                LatestVersion = "0.0",
            };
            db.Documents.Add(document);
            DocumentHistory documentHistory = new DocumentHistory
            {
                Version = "0.0",
                Latest = true,
                DocumentId = document.Id,
                Details = "Document uploaded by user " + document.UploaderId+ " and stored in company " + document.CompanyId
            };
            document.DocumentHistories.Add(documentHistory);
            db.Documents.Add(document);
            db.DocumentHistories.Add(documentHistory);
            db.SaveChanges();
            newDocument.Id = document.Id;
            newDocument.LatestVersion = document.LatestVersion;

            return newDocument;
        }
        /// <summary>
        /// UPDATE document data in db after uploading document for a company
        /// </summary>
        /// <param name="newDocument"></param>
        /// <param name="companyId"></param>
        /// <param name="rotationId"></param>
        /// <returns></returns>
        private DocumentInboxData Update(DocumentInboxData newDocument, long companyId, long rotationId)
        {
            using var db = new Connection();
            Document document = db.Documents.FirstOrDefault(c => c.Id == newDocument.Id);

            // mapping value
            document.Extension = newDocument.Extension;
            document.FileName = newDocument.FileName;

            document.UploaderId = newDocument.CreatorId; // harusnya current user bukan? diinject ke newDocument pas di-controller

            // NEW
            document.MaximumDownloadPerUser = newDocument.MaximumDownloadPerUser;
            document.MaximumPrintPerUser = newDocument.MaximumPrintPerUser;

            // upload, get file directory, controller atau di service?
            document.FileUrl = newDocument.FileUrl;

            // update file size
            document.FileSize = newDocument.FileSize;
            document.UpdatedAt = DateTime.Now;

            db.SaveChanges();
            newDocument.Id = document.Id;

            return newDocument;
        }
        /// <summary>
        /// SAVE all annottation list, it can be update the latest annotations in document too
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="creatorId"></param>
        /// <param name="userEmail"></param>
        /// <param name="annos"></param>
        /// <returns></returns>
        public ICollection<DocumentAnnotationsInboxData> SaveAnnos(long documentId, long creatorId, string userEmail, IEnumerable<DocumentAnnotationsInboxData> annos)
        {
            using var db = new Connection();
            Document item = GetById(documentId);
            // prepare data 
            var cxold = db.DocumentElements.Count(c => c.DocumentId == documentId);
            var cxnew = annos.Count();
            // nambah annotasi baru
            if (cxold < cxnew)
            {
                var ep = annos.ElementAt(0); // get 1 data for sample
                for (var x = cxold; x < cxnew; x++)
                {
                    DocumentAnnotation da = new DocumentAnnotation();
                    da.DocumentId = documentId;
                    da.Page = ep.Page;
                    da.ElementTypeId = ep.ElementTypeId;

                    da.EmailOfUserAssigned = da.EmailOfUserAssigned = string.IsNullOrEmpty(userEmail) ? da.EmailOfUserAssigned : userEmail;
                    da.CreatedAt = DateTime.Now;
                    db.DocumentElements.Add(da);
                }
                WriteDocumentHistory(document: item, details: "Some annotation added to document by User " + creatorId.ToString());
                db.SaveChanges();
            }
            else if (cxold > cxnew)
            {
                var dremove = db.DocumentElements.Where(c => c.Document.Id == documentId).Take(cxold - cxnew).ToList();
                db.DocumentElements.RemoveRange(dremove);
                WriteDocumentHistory(document: item, details: "Some annotation in document removed by User " + creatorId.ToString());
                db.SaveChanges();
            }
            //
            // save data (update)
            //
            var dnew = db.DocumentElements.Where(c => c.Document.Id == documentId).ToList();
            int v = 0;
            var retval = new List<DocumentAnnotationsInboxData>();
            foreach (DocumentAnnotation da in dnew)
            {
                var epos = annos.ElementAt(v);
                da.DocumentId = documentId;
                da.Page = epos.Page;
                da.ElementTypeId = epos.ElementTypeId;
                da.LeftPosition = epos.LeftPosition;
                da.TopPosition = epos.TopPosition;
                da.WidthPosition = epos.WidthPosition;
                da.HeightPosition = epos.HeightPosition;
                da.Color = epos.Color;
                da.BackColor = epos.BackColor;
                da.Text = epos.Text;
                da.Unknown = epos.Unknown;
                da.Rotation = epos.Rotation;
                da.ScaleX = epos.ScaleX;
                da.ScaleY = epos.ScaleY;
                da.TransitionX = epos.TransitionX;
                da.TransitionY = epos.TransitionY;
                da.StrokeWidth = epos.StrokeWidth;
                da.Opacity = epos.Opacity;
                da.Flag = epos.Flag;
                da.AssignedAnnotationCode = epos.AssignedAnnotationCode;
                da.AssignedAt = epos.AssignedAt;
                da.AssignedAnnotationImageFileName = epos.AssignedAnnotationImageFileName;
                da.CreatorId = (epos.CreatorId == null ? creatorId : epos.CreatorId);
                da.UserId = epos.UserId;
                da.Element = epos.Element;
                da.CreatedAt = DateTime.Now;
                v++;
            }
            db.SaveChanges();
            foreach (DocumentAnnotation da in dnew)
            {
                retval.Add(new DocumentAnnotationsInboxData(da));
            }
            return retval;
        }
        /// <summary>
        /// SAVE document user from annotations of the document saved
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        public ICollection<DocumentUserInboxData> CreateDocumentUserFromAnnotations(long documentId)
        {
            using var db = new Connection();
            var returnValue = new List<DocumentUserInboxData>();
            var createdOrUpdated = new List<DocumentUser>();
            var docs = db.Documents.FirstOrDefault(doc => doc.Id == documentId);
            if (docs == null) return null;
            foreach (DocumentAnnotation el in docs.DocumentAnnotations)
            {
                if (el.UserId == null) continue;
                var docUser = db.DocumentUsers.FirstOrDefault(du => du.UserId == el.UserId.Value && du.DocumentId == el.DocumentId);
                if (docUser == null)
                {
                    docUser = new DocumentUser();
                    docs.DocumentUsers.Add(docUser);
                    db.DocumentUsers.Add(docUser);
                }
                docUser.UserId = el.UserId.Value;
                docUser.DocumentId = el.DocumentId;

                string eltypename = Enum.GetName(typeof(Models.Constant.EnumElementTypeId), el.ElementTypeId);

                if (("SIGNATURE,INITIAL").Contains(eltypename))
                    docUser.ActionPermission |= (int)Models.Constant.EnumDocumentAction.SIGN;

                if (("PRIVATESTAMP").Contains(eltypename))
                    docUser.ActionPermission |= (int)Models.Constant.EnumDocumentAction.PRIVATESTAMP;

                db.SaveChanges();
                returnValue.Add(new DocumentUserInboxData(docUser));
            }
            // after save the value, then return value as api response data
            return returnValue;
        }
        /// <summary>
        /// CREATE default document user
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DocumentUser CreateDocumentUser(long documentId, long userId)
        {
            using var db = new Connection();
            var documentUserDb = db.DocumentUsers.FirstOrDefault(u => u.UserId == userId && u.DocumentId == documentId);
            if (documentUserDb != null) return documentUserDb;
            var newDocumentUser = new DocumentUser
            {
                UserId = userId,
                DocumentId = documentId,
                ActionPermission = 0,
                ActionStatus = 0,
            };
            db.DocumentUsers.Add(newDocumentUser);
            db.SaveChanges();
            // after save the value, then return value as api response data
            return newDocumentUser;
        }

        public IEnumerable<DocumentItem> GetAllCompanyDocument(long companyId)
        {
            using var db = new Connection();
            var result =
                        (from doc in db.Documents
                        where doc.CompanyId == companyId
                        orderby doc.CreatedAt descending
                        select new DocumentItem
                        {
                        Id = doc.Id,
                        Extension = doc.Extension,
                        FileName = doc.FileUrl,
                        FileNameOri = doc.FileName,
                        FileSize = doc.FileSize,
                        CreatorId = doc.UploaderId,
                        MaxDownload = doc.MaximumDownloadPerUser,
                        MaxPrint = doc.MaximumPrintPerUser
                        }).ToList();
            return result;
        }

        public long GetAllCount(long memberId, string searchKeyword)
        {
            string[] keywords = new string[] { };
            if (!string.IsNullOrEmpty(searchKeyword))
                keywords = searchKeyword.Split(' ');

            using (var db = new Connection())
            {
                var result =
                    (from c in db.Documents
                     where c.UploaderId == memberId && (keywords.All(x => (c.FileName).Contains(x)))
                     select new DocumentItem
                     {
                         Id = c.Id,
                     }).Count();

                return result;

            }
        }
        /// <summary>
        /// SEND email about user giving a signature or initial to a document
        /// </summary>
        /// <param name="user"></param>
        /// <param name="rotName"></param>
        /// <param name="docName"></param>
        /// <param name="numbers"></param>
        public void SendEmailSignature(User user, string rotName, string docName, string numbers)
        {
            AppConfigGenerator appsvr = new AppConfigGenerator();
            var topaz = appsvr.GetConstant("APPLICATION_NAME")["value"];
            var admName = appsvr.GetConstant("EMAIL_USER_DISPLAY")["value"];
            EmailService emailService = new EmailService();
            string body =
                "Dear " + user.Name + ",<br/><br/>" +
                "You have signed rotation <b>" + rotName + "</b> in document <b>" + docName + "</b>, the signature number generated: <b>" + numbers + "</b>.<br/>";

            body += "<br/><br/> " + admName + " Administrator<br/>";

            var emailfrom = appsvr.GetConstant("EMAIL_USER")["value"];


            _ = emailService.Send(emailfrom, admName, user.Email, admName + " User Signature", body, false, new string[] { });
        }
        /// <summary>
        /// SEND email about user giving a stamp to a document
        /// </summary>
        /// <param name="user"></param>
        /// <param name="rotName"></param>
        /// <param name="docName"></param>
        /// <param name="numbers"></param>
        public void SendEmailStamp(User user, string rotName, string docName, string numbers)
        {
            AppConfigGenerator appsvr = new AppConfigGenerator();
            var topaz = appsvr.GetConstant("APPLICATION_NAME")["value"];
            var admName = appsvr.GetConstant("EMAIL_USER_DISPLAY")["value"];
            EmailService emailService = new EmailService();
            string body =
                "Dear " + user.Name + ",<br/><br/>" +
                "You have stamped document <b>" + docName + "</b> in rotation <b>" + rotName + "</b>, the stamp number generated: <b>" + numbers + "</b>.<br/>";

            body += "<br/><br/> " + admName + " Administrator<br/>";

            var emailfrom = appsvr.GetConstant("EMAIL_USER")["value"];


            _ = emailService.Send(emailfrom, admName, user.Email, admName + " User Stamp", body, false, new string[] { });
        }
        /// <summary>
        /// CREATE sign data on annotation
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="userId"></param>
        /// <param name="rotationId"></param>
        /// <returns></returns>
        public int Signature(long documentId, long userId, long rotationId)
        {
            using var db = new Connection();
            var datas = db.DocumentElements.Where(c => c.Document.Id == documentId
                && (c.ElementTypeId == (int)Constant.EnumElementTypeId.SIGNATURE || c.ElementTypeId == (int)Constant.EnumElementTypeId.INITIAL)
                && c.UserId == userId && (c.Flag & 1) != 1).ToList();
            if (datas == null)
                return 0;
            var user = db.Users.FirstOrDefault(c => c.Id == userId);
            var rot = db.Rotations.FirstOrDefault(c => c.Id == rotationId);
            var doc = db.Documents.FirstOrDefault(c => c.Id == documentId);
            int cx = 0;
            string numbers = "";
            foreach (DocumentAnnotation da in datas)
            {
                var dt = DateTime.Now;
                da.Flag = 1;
                da.AssignedAt = dt;
                string uniqueCode = dt.ToString("yyMMddHHmmssfff");
                string additionalUniqueRandom = Utilities.RandomString(5);
                string codeUsed = "DRD-" + Utilities.CombineStringRandomPosition(uniqueCode, additionalUniqueRandom);
                da.AssignedAnnotationCode = codeUsed;
                da.AssignedAnnotationImageFileName = (da.ElementTypeId == (int)Models.Constant.EnumElementTypeId.SIGNATURE ? user.SignatureImageFileName : user.InitialImageFileName);
                if (!numbers.Equals(""))
                    numbers += ", ";
                numbers += da.AssignedAnnotationCode;
                cx++;
            }
            if (cx > 0)
            {
                db.SaveChanges();
                SendEmailSignature(user, rot.Name, doc.FileName, numbers);
            }
            WriteDocumentHistory(document: doc, details: "Document was Signed by User " + user.Id.ToString() + "("+user.Name+")");
            return cx;
        }
        /// <summary>
        /// CREATE stamp data on annotation
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="userId"></param>
        /// <param name="rotationId"></param>
        /// <returns></returns>
        public int Stamp(long documentId, long userId, long rotationId)
        {
            using var db = new Connection();
            var datas = db.DocumentElements.Where(c => c.Document.Id == documentId
                && c.ElementTypeId == (int)Constant.EnumElementTypeId.PRIVATESTAMP
                && c.UserId == userId && (c.Flag & 1) != 1).ToList();

            if (datas == null)
                return 0;
            var user = db.Users.FirstOrDefault(c => c.Id == userId);
            var rot = db.Rotations.FirstOrDefault(c => c.Id == rotationId);
            var doc = db.Documents.FirstOrDefault(c => c.Id == documentId);
            int cx = 0;
            string numbers = "";
            foreach (DocumentAnnotation da in datas)
            {
                var dt = DateTime.Now;
                da.Flag = 1;
                da.AssignedAt = dt;
                string uniqueCode = dt.ToString("yyMMddHHmmssfff");
                string additionalUniqueRandom = Utilities.RandomString(5);
                string codeUsed = "DRD-" + Utilities.CombineStringRandomPosition(uniqueCode, additionalUniqueRandom);
                da.AssignedAnnotationCode = codeUsed;
                da.AssignedAnnotationImageFileName = user.StampImageFileName;
                if (!numbers.Equals(""))
                    numbers += ", ";
                numbers += da.AssignedAnnotationCode;
                cx++;
            }
            if (cx > 0)
            {
                db.SaveChanges();
                SendEmailStamp(user, rot.Name, doc.FileName, numbers);
            }
            WriteDocumentHistory(document: doc, details: "Document was Stamped by User " + user.Id.ToString() + "(" + user.Name + ")");
            return cx;
        }
        /// <summary>
        /// UPDATE the status updatedat of document 
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns>status db save</returns>
        public int DocumentUpdatedByRotation(long documentId)
        {
            using (var db = new Connection())
            {
                var docitem = db.Documents.FirstOrDefault(d => d.Id == documentId);
                docitem.UpdatedAt = DateTime.Now;
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// CHANGE status document on rotation is current or not
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns>status db save</returns>
        public int DocumentRemovedorRevisedFromRotation(long documentId)
        {
            using var db = new Connection();
            var docitem = db.Documents.FirstOrDefault(d => d.Id == documentId);
            docitem.IsCurrent = false;
            docitem.UpdatedAt = DateTime.Now;
            WriteDocumentHistory(document: docitem, details: "Document was Removed from rotation ");
            return db.SaveChanges();
        }
        public IEnumerable<DocumentSign> GetAnnotateDocs(long memberId, string topCriteria, int page, int pageSize, string order, string criteria)
        {
            int skip = pageSize * (page - 1);
            string ordering = "DateCreated desc";

            if (!string.IsNullOrEmpty(order))
                ordering = order;

            if (string.IsNullOrEmpty(criteria))
                criteria = "1=1";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

            using (var db = new Connection())
            {
                var tmps =
                    (from c in db.DocumentElements
                     where c.CreatorId == memberId && !("SIGNATURE,INITIAL").Contains(Enum.GetName(typeof(Models.Constant.EnumElementTypeId), c.ElementTypeId)) &&
                            (topCriteria == null || tops.All(x => (c.Document.FileName).Contains(x)))
                     orderby c.AssignedAt descending
                     select new DocumentSign
                     {
                         Id = c.Document.Id,
                         CxAnnotate = 1,
                         CreatedAt = c.AssignedAt,
                     }).ToList();

                var signs =
                    (from c in tmps
                     group c by c.Id into g
                     select new DocumentSign
                     {
                         Id = g.Key,
                         CxAnnotate = g.Sum(c => c.CxAnnotate),
                         CreatedAt = g.Max(c => c.CreatedAt),
                     }).ToList();

                var rowCount = signs.Count();

                var result =
                (from c in signs
                 join d in db.Documents on c.Id equals d.Id
                 select new DocumentSign
                 {
                     Id = c.Id,
                     FileName = d.FileName,
                     CxAnnotate = c.CxAnnotate,
                     RowCount = rowCount,
                     CreatedAt = c.CreatedAt,
                 }).Skip(skip).Take(pageSize).ToList();

                return result;

            }
        }

        public IEnumerable<DocumentSign> GetAnnotateDocs(long memberId, string topCriteria, int page, int pageSize, string order)
        {
            return GetAnnotateDocs(memberId, topCriteria, page, pageSize, order, null);
        }

        public IEnumerable<DocumentSign> GetAnnotateDocs(long memberId, string topCriteria, int page, int pageSize)
        {
            return GetAnnotateDocs(memberId, topCriteria, page, pageSize, null, null);
        }

        public Document GetById(long id)
        {
            using var db = new Connection();
            var result = db.Documents.Where(doc => doc.Id == id).FirstOrDefault();
            if (result != null)
            {
                foreach (DocumentAnnotation da in result.DocumentAnnotations)
                {
                    if (da.UserId == null) continue;
                    if (da.ElementTypeId == (int)Models.Constant.EnumElementTypeId.SIGNATURE || da.ElementTypeId == (int)Models.Constant.EnumElementTypeId.INITIAL
                        || da.ElementTypeId == (int)Models.Constant.EnumElementTypeId.PRIVATESTAMP)
                    {
                        var mem = db.Users.FirstOrDefault(c => c.Id == da.UserId);
                        da.Element = new Element();
                        da.Element.UserId = mem.Id;
                        da.Element.Name = mem.Name;
                        da.Element.Foto = mem.ProfileImageFileName;
                    }
                }
            }
            return result;
        }

        public DocumentItem GetByUniqFileName(string uniqFileName, bool isDocument, bool isTemp)
        {
            DocumentItem doc = new DocumentItem();
            using (var db = new Connection())
            {
                if (isTemp)
                {
                    doc.FileName = uniqFileName;
                }
                else if (isDocument)
                {
                    var result = db.Documents.FirstOrDefault(c => c.FileUrl.Contains(uniqFileName));
                    if (result != null)
                    {
                        doc.Id = result.Id;
                        doc.FileName = result.FileUrl;
                        doc.EncryptedId = Utilities.Encrypt(result.CompanyId.ToString());
                        doc.LatestVersion = result.LatestVersion;
                    }
                }
            }
            return doc;
        }
        public IEnumerable<DocumentItem> GetCompanyDocument(long creatorId, string searchKeyword, int page, int pageSize, long companyId)
        {
            int skip = pageSize * (page - 1);

            // Search keywords
            string[] keywords = new string[] { };
            if (!string.IsNullOrEmpty(searchKeyword))
                keywords = searchKeyword.Split(' ');
            //else
            //    keywords = null;

            using (var db = new Connection())
            {
                var result =
                (from doc in db.Documents
                 where (doc.UploaderId == creatorId || doc.CompanyId == companyId)
                 && (keywords.All(x => (doc.FileName).Contains(x)))
                 orderby doc.CreatedAt descending
                 select new DocumentItem
                 {
                     Id = doc.Id,
                     Extension = doc.Extension,
                     FileNameOri = doc.FileName,
                     FileName = doc.FileUrl,
                     FileSize = doc.FileSize,
                     CreatorId = doc.UploaderId,
                     MaxDownload = doc.MaximumDownloadPerUser,
                     MaxPrint = doc.MaximumPrintPerUser
                 }).Skip(skip).Take(pageSize).ToList();

                // ini ngapain?
                if (result != null)
                {
                    MenuService musvr = new MenuService();
                    foreach (DocumentItem dl in result)
                    {
                        dl.Key = musvr.EncryptData(dl.Id);
                    }
                }

                return result;
            }
        }
        
        public IEnumerable<DocumentSign> GetSignatureDocs(long memberId, string topCriteria, int page, int pageSize, string order, string criteria)
        {

            int skip = pageSize * (page - 1);
            string ordering = "DateCreated desc";

            if (!string.IsNullOrEmpty(order))
                ordering = order;

            if (string.IsNullOrEmpty(criteria))
                criteria = "1=1";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

            using (var db = new Connection())
            {
                var tmps =
                    (from c in db.DocumentElements
                     where c.UserId == memberId && ("SIGNATURE,INITIAL").Contains(Enum.GetName(typeof(Models.Constant.EnumElementTypeId), c.ElementTypeId)) && (c.Flag & 1) == 1 &&
                        (topCriteria == null || tops.All(x => (c.Document.FileName).Contains(x)))
                     orderby c.AssignedAt descending
                     select new DocumentSign
                     {
                         Id = c.Document.Id,
                         CxSignature = (c.ElementTypeId == (int)Models.Constant.EnumElementTypeId.SIGNATURE ? 1 : 0),
                         CxInitial = (c.ElementTypeId == (int)Models.Constant.EnumElementTypeId.INITIAL ? 1 : 0),
                         CreatedAt = c.AssignedAt,
                     }).ToList();

                var signs =
                    (from c in tmps
                     group c by c.Id into g
                     select new DocumentSign
                     {
                         Id = g.Key,
                         CxSignature = g.Sum(c => c.CxSignature),
                         CxInitial = g.Sum(c => c.CxInitial),
                         CreatedAt = g.Max(c => c.CreatedAt),
                     }).ToList();

                var rowCount = signs.Count();

                var result =
                (from c in signs
                 join d in db.Documents on c.Id equals d.Id
                 select new DocumentSign
                 {
                     Id = c.Id,
                     FileName = d.FileName,
                     CxSignature = c.CxSignature,
                     CxInitial = c.CxInitial,
                     RowCount = rowCount,
                     CreatedAt = c.CreatedAt,
                 }).Skip(skip).Take(pageSize).ToList();

                return result;

            }
        }
        public IEnumerable<DocumentSign> GetSignatureDocs(long memberId, string topCriteria, int page, int pageSize, string order)
        {
            return GetSignatureDocs(memberId, topCriteria, page, pageSize, order, null);
        }
        public IEnumerable<DocumentSign> GetSignatureDocs(long memberId, string topCriteria, int page, int pageSize)
        {
            return GetSignatureDocs(memberId, topCriteria, page, pageSize, null, null);
        }

        public int RequestDownloadDocument(string docName, long userId)
        {
            using var db = new Connection();
            var documentDb = db.Documents.FirstOrDefault(c => c.FileUrl.Contains(docName) && c.IsCurrent);
            if (documentDb == null)
                return (int)Constant.DocumentPrintOrDownloadStatus.NOT_FOUND;
            // check if user login is creator of rotation
            var rotationDb = db.Rotations.FirstOrDefault(r => r.Id == documentDb.RotationId);
            if (rotationDb != null)
            {
                if (rotationDb.CreatorId == userId)
                    return (int)Constant.DocumentPrintOrDownloadStatus.OK;
            }
            var rotationUserDb = db.RotationUsers.FirstOrDefault(r => r.UserId == userId && r.RotationId == documentDb.RotationId);
            if (rotationUserDb == null)
                return (int)Constant.DocumentPrintOrDownloadStatus.USER_HAS_NO_ACCESS;
            // calc from rotation is completed
            // if (!rot.RotationNode.Rotation.Status.Equals("90") || (docMem.FlagPermission & (int)ConstantModel.EnumDocumentAction.PRINT) == (int)ConstantModel.EnumDocumentAction.PRINT)
            //    return -3;
            if ((rotationUserDb.ActionPermission & (int)Models.Constant.EnumDocumentAction.DOWNLOAD) != (int)Models.Constant.EnumDocumentAction.DOWNLOAD)
                return (int)Constant.DocumentPrintOrDownloadStatus.USER_HAS_NO_ACCESS;

            // out of max // There may be a race condition
            var documentUserDb = db.DocumentUsers.FirstOrDefault(d => d.DocumentId == documentDb.Id && d.UserId == userId);
            if ((documentDb.MaximumDownloadPerUser!= 0) && (documentUserDb.DownloadCount+ 1 > documentDb.MaximumDownloadPerUser))
                return (int)Constant.DocumentPrintOrDownloadStatus.EXCEED_LIMIT;

            documentUserDb.DownloadCount++;
            db.SaveChanges();
            WriteDocumentHistory(document: documentDb, details: "Document downloaded by User " + rotationUserDb.Id.ToString() + "(" + rotationUserDb.User.Name + ")");
            return (int)Constant.DocumentPrintOrDownloadStatus.OK;
        }

        /// <summary>
        /// REQUEST to print document, if user have permission and not out of limit, so the request return int 1 as status OK, also will save print counter
        /// </summary>
        /// <param name="docName"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int RequestPrintDocument(string docName, long userId)
        {
            using var db = new Connection();
            var documentDb = db.Documents.FirstOrDefault(c => c.FileUrl.Contains(docName) && c.IsCurrent);
            if (documentDb == null)
                return (int)Constant.DocumentPrintOrDownloadStatus.NOT_FOUND;
            // check if user login is creator of rotation
            var rotationDb = db.Rotations.FirstOrDefault(r => r.Id == documentDb.RotationId);
            if (rotationDb != null)
            {
                if (rotationDb.CreatorId == userId)
                    return (int)Constant.DocumentPrintOrDownloadStatus.OK;
            }
            var rotationUserDb = db.RotationUsers.FirstOrDefault(r => r.UserId == userId && r.RotationId == documentDb.RotationId);
            if (rotationUserDb == null)
                return (int)Constant.DocumentPrintOrDownloadStatus.USER_HAS_NO_ACCESS;
            // calc from rotation is completed
            // if (!rot.RotationNode.Rotation.Status.Equals("90") || (docMem.FlagPermission & (int)ConstantModel.EnumDocumentAction.PRINT) == (int)ConstantModel.EnumDocumentAction.PRINT)
            //    return -3;
            System.Diagnostics.Debug.WriteLine(rotationUserDb.ActionPermission);
            if ((rotationUserDb.ActionPermission & (int)Models.Constant.EnumDocumentAction.PRINT) != (int)Models.Constant.EnumDocumentAction.PRINT)
                return (int)Constant.DocumentPrintOrDownloadStatus.USER_HAS_NO_ACCESS;

            // out of max // There may be a race condition
            var documentUserDb = db.DocumentUsers.FirstOrDefault(d => d.DocumentId == documentDb.Id && d.UserId == userId);
            if ((documentDb.MaximumPrintPerUser != 0) && (documentUserDb.PrintCount + 1 > documentDb.MaximumPrintPerUser))
                return (int)Constant.DocumentPrintOrDownloadStatus.EXCEED_LIMIT;

            documentUserDb.PrintCount++;
            db.SaveChanges();
            WriteDocumentHistory(document: documentDb, details: "Document printed by User " + rotationUserDb.Id.ToString() + "(" + rotationUserDb.User.Name + ")");
            return (int)Constant.DocumentPrintOrDownloadStatus.OK;
        }
        /// <summary>
        /// GET document data as inbox data
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        public DocumentInboxData GetDocumentInboxData(long documentId)
        {
            using var db = new Connection();
            var documentDb = db.Documents.Where(d => d.Id == documentId).First();
            var returnVal = new DocumentInboxData(documentDb);
            foreach (DocumentAnnotationsInboxData documentElement in returnVal.DocumentAnnotations)
            {
                if (documentElement.UserId == null || documentElement.UserId == 0) continue;
                if (documentElement.ElementTypeId == (int)Models.Constant.EnumElementTypeId.SIGNATURE
                    || documentElement.ElementTypeId == (int)Models.Constant.EnumElementTypeId.INITIAL
                    || documentElement.ElementTypeId == (int)Models.Constant.EnumElementTypeId.PRIVATESTAMP)
                {
                    var user = db.Users.FirstOrDefault(c => c.Id == documentElement.UserId);
                    Element newElement = new Element();
                    newElement.EncryptedUserId = Utilities.Encrypt(user.Id.ToString());
                    newElement.UserId = user.Id;
                    newElement.Name = user.Name;
                    newElement.Foto = user.ProfileImageFileName;
                    documentElement.Element = newElement;
                }
            }
            return returnVal;
        }
        /// <summary>
        /// UPDATE version of document to new one, its used after saving new document to directory
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="newVersion"></param>
        /// <returns></returns>
        public int UpdateVersion(long documentId, string newVersion)
        {
            var db = new Connection();
            var doc = db.Documents.First(d => d.Id == documentId);
            doc.LatestVersion = newVersion;
            db.SaveChanges();
            return WriteDocumentHistory(doc, "Create pdf annotated and update document version");
        }
        /// <summary>
        /// Upadate history of Document everytime there is a change
        /// </summary>
        /// <param name="document"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public int WriteDocumentHistory(Document document, string details)
        {
            using var db = new Connection();
            if (db.DocumentHistories != null)
            {
                DocumentHistory old = db.DocumentHistories.Where(h => h.DocumentId == document.Id && h.Latest).First();
                old.Latest = false;
            }
            DocumentHistory newHistory = new DocumentHistory
            {
                DocumentId = document.Id,
                Details = details,
                Version = document.LatestVersion,
                Latest = true,
            };
            db.DocumentHistories.Add(newHistory);
            return db.SaveChanges();
        }

    }
}
