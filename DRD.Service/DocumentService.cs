using DRD.Models;
using DRD.Models.View;
using DRD.Service.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DRD.Service
{
    public class DocumentService : IDocumentService
    {
        public static ElementType GetElementTypeFromCsvByCode(string code)
        {
            var root = System.Web.HttpContext.Current.Server.MapPath("~");
            var path = Path.Combine(root, @"ElementType.csv");
            ElementType values = File.ReadAllLines(path)
                                           .Select(v => ElementType.fromCsv(v))
                                           .Where(c => c.Code.Equals(code)).FirstOrDefault();

            return values;
        }

        public static ElementType GetElementTypeFromCsvById(int id)
        {
            var root = System.Web.HttpContext.Current.Server.MapPath("~");
            var path = Path.Combine(root, @"ElementType.csv");
            ElementType values = File.ReadAllLines(path)
                                           .Select(v => ElementType.fromCsv(v))
                                           .Where(c => c.Id == id).FirstOrDefault();

            return values;
        }

        public int CheckingPrivateStamp(long memberId)
        {
            int ret = 1;
            using (var db = new ServiceContext())
            {
                var mem = db.Users.FirstOrDefault(c => c.Id == memberId);
                if (mem.ImageStamp == null)
                    ret = -1;
            }
            return ret;
        }

        public int CheckingSignature(long memberId)
        {
            int ret = 1;
            using (var db = new ServiceContext())
            {
                var mem = db.Users.FirstOrDefault(c => c.Id == memberId);
                if (mem.ImageSignature == null || mem.ImageInitials == null || mem.ImageKtp1 == null || mem.ImageKtp2 == null || string.IsNullOrEmpty("" + mem.OfficialIdNo))
                    ret = -1;
            }
            return ret;
        }

        public DocumentInboxData Create(DocumentInboxData newDocument, long companyId, long rotationId)
        {
            using (var db = new ServiceContext())
            {
                Document document = new Document();
                SubscriptionService subscriptionService = new SubscriptionService();
                BusinessPackage package = subscriptionService.getCompanyPackageByCompany(companyId);
                Usage usage = subscriptionService.GetCompanyUsage(companyId);
                // validate first
                ValidateWithPlan(document, newDocument, package);

                // mapping value
                document.Extention = newDocument.Extention;
                document.Description = newDocument.Description;
                document.Description = newDocument.FileUrl;
                document.FileName = newDocument.FileName;
                document.FileSize = newDocument.FileSize;

                document.CreatorId = newDocument.CreatorId; // harusnya current user bukan? diinject ke newDocument pas di-controller
                document.UserEmail = newDocument.UserEmail;
                document.CreatedAt = DateTime.Now;

                // NEW
                document.ExpiryDay = newDocument.ExpiryDay;
                document.MaxDownloadPerActivity = newDocument.MaxDownloadPerActivity;
                document.MaxPrintPerActivity = newDocument.MaxPrintPerActivity;
                document.IsCurrent = true; // ??

                // upload, get file directory, controller atau di service?
                document.FileUrl = newDocument.FileUrl;

                // update subscription storage
                usage.Storage = package.Storage - document.FileSize; // update storage limit

                document.RotationId = rotationId;
                document.CompanyId = companyId;

                db.Documents.Add(document);
                db.SaveChanges();
                newDocument.Id = document.Id;

                return newDocument;
            }

        }

        public ICollection<DocumentUserInboxData> CreateDocumentUser(long documentId)
        {
            using (var db = new ServiceContext())
            {
                var returnValue = new List<DocumentUserInboxData>();
                var createdOrUpdated = new List<DocumentUser>();
                var docs = db.Documents.FirstOrDefault(doc => doc.Id == documentId);
                if (docs == null) return null;
                foreach (DocumentElement el in docs.DocumentElements)
                {
                    if (el.ElementId == null) continue;
                    var docUser = db.DocumentUsers.FirstOrDefault(du => du.UserId == el.ElementId.Value && du.DocumentId == el.DocumentId);
                    if (docUser != null) continue;
                    docUser = new DocumentUser();
                    docUser.UserId = el.ElementId.Value;
                    docUser.DocumentId = el.DocumentId;
                    docUser.FlagPermission = 6; // view, add annotate
                    if (("SIGNATURE,INITIAL").Contains(GetElementTypeFromCsvById(el.ElementTypeId).Code)) docUser.FlagPermission |= 1;
                    if (("PRIVATESTAMP").Contains(GetElementTypeFromCsvById(el.ElementTypeId).Code)) docUser.FlagPermission |= 32;
                    db.DocumentUsers.Add(docUser);
                }
                db.SaveChanges();

                // after save the value, then return value as api response data
                foreach (DocumentUser du in createdOrUpdated)
                {
                    var item = new DocumentUserInboxData();
                    item.DocumentId = du.DocumentId;
                    item.Document = du.Document;
                    item.FlagAction = du.FlagAction;
                    item.FlagPermission = du.FlagPermission;
                    item.UserId = du.UserId;
                    item.User = du.User;
                    item.UserName = du.User.Name;
                    returnValue.Add(item);
                }
                return returnValue;
            }
        }

        public void DoRevision(long documentId)
        {

            using (var db = new ServiceContext())
            {
                //document.IsCurrent = false;
            }

            throw new NotImplementedException();
        }

        public ICollection<DocumentElement> FillAnnos(Document doc)
        {
            using (var db = new ServiceContext())
            {
                var annos =
                    (from x in db.DocumentElements
                     where x.Document.Id == doc.Id
                     select new DocumentElement
                     {
                         Page = x.Page,
                         LeftPosition = x.LeftPosition,
                         TopPosition = x.TopPosition,
                         WidthPosition = x.WidthPosition,
                         HeightPosition = x.HeightPosition,
                         Color = x.Color,
                         BackColor = x.BackColor,
                         Data = x.Data,
                         Data2 = x.Data2,
                         Rotation = x.Rotation,
                         ScaleX = x.ScaleX,
                         ScaleY = x.ScaleY,
                         TransitionX = x.TransitionX,
                         TransitionY = x.TransitionY,
                         StrokeWidth = x.StrokeWidth,
                         Opacity = x.Opacity,
                         Flag = x.Flag,
                         FlagCode = x.FlagCode,
                         FlagDate = x.FlagDate,
                         FlagImage = x.FlagImage,
                         CreatorId = x.CreatorId,
                         ElementId = x.ElementId,

                     }).ToList();

                return annos;
            }
        }

        public IEnumerable<DocumentItem> GetAllCompanyDocument(long companyId)
        {
            using (var db = new ServiceContext())
            {
                var result =
                (from doc in db.Documents
                 where doc.CompanyId == companyId
                 orderby doc.CreatedAt descending
                 select new DocumentItem
                 {
                     Id = doc.Id,
                     Extention = doc.Extention,
                     FileName = doc.FileUrl,
                     FileNameOri = doc.FileName,
                     FileSize = doc.FileSize,
                     CreatorId = doc.CreatorId,
                     CreatedAt = doc.CreatedAt,
                     Description = doc.Description,
                     MaxDownload = doc.MaxDownloadPerActivity,
                     MaxPrint = doc.MaxPrintPerActivity,
                     ExpiryDay = doc.ExpiryDay
                 }).ToList();
                return result;
            }
        }

        public long GetAllCount(long memberId, string searchKeyword)
        {
            string[] keywords = new string[] { };
            if (!string.IsNullOrEmpty(searchKeyword))
                keywords = searchKeyword.Split(' ');

            using (var db = new ServiceContext())
            {
                var result =
                    (from c in db.Documents
                     where c.CreatorId == memberId && (keywords.All(x => (c.FileName).Contains(x)))
                     select new DocumentItem
                     {
                         Id = c.Id,
                     }).Count();

                return result;

            }
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

            using (var db = new ServiceContext())
            {
                var tmps =
                    (from c in db.DocumentElements
                     where c.CreatorId == memberId && !("SIGNATURE,INITIAL").Contains(GetElementTypeFromCsvById(c.ElementTypeId).Code) &&
                            (topCriteria == null || tops.All(x => (c.Document.FileName).Contains(x)))
                     orderby c.FlagDate descending
                     select new DocumentSign
                     {
                         Id = c.Document.Id,
                         CxAnnotate = 1,
                         DateCreated = c.FlagDate,
                     }).ToList();

                var signs =
                    (from c in tmps
                     group c by c.Id into g
                     select new DocumentSign
                     {
                         Id = g.Key,
                         CxAnnotate = g.Sum(c => c.CxAnnotate),
                         DateCreated = g.Max(c => c.DateCreated),
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
                     DateCreated = c.DateCreated,
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

            using (var db = new ServiceContext())
            {
                var result =
                    (from c in db.Documents
                     where c.Id == id
                     select new Document
                     {
                         Id = c.Id,
                         Extention = c.Extention,
                         Description = c.Description,
                         FileName = c.FileName,
                         FileUrl = c.FileUrl,
                         FileSize = c.FileSize,
                         CreatedAt = c.CreatedAt,
                         UpdatedAt = c.UpdatedAt,
                         CreatorId = c.CreatorId,
                         UserEmail = c.UserEmail,
                         DocumentElements = (
                            from x in c.DocumentElements
                            select new DocumentElement
                            {
                                Id = x.Id,
                                Document = x.Document,
                                Page = x.Page,
                                LeftPosition = x.LeftPosition,
                                TopPosition = x.TopPosition,
                                WidthPosition = x.WidthPosition,
                                HeightPosition = x.HeightPosition,
                                Color = x.Color,
                                BackColor = x.BackColor,
                                Data = x.Data,
                                Data2 = x.Data2,
                                Rotation = x.Rotation,
                                ScaleX = x.ScaleX,
                                ScaleY = x.ScaleY,
                                TransitionX = x.TransitionX,
                                TransitionY = x.TransitionY,
                                StrokeWidth = x.StrokeWidth,
                                Opacity = x.Opacity,
                                Flag = x.Flag,
                                FlagCode = x.FlagCode,
                                FlagDate = x.FlagDate,
                                FlagImage = x.FlagImage,
                                CreatorId = x.CreatorId,
                                ElementId = x.ElementId,
                                UserId = x.UserId,
                                CreatedAt = x.CreatedAt,
                                UpdatedAt = x.UpdatedAt,
                                ElementTypeId = x.ElementTypeId
                            }).ToList(),
                     }).FirstOrDefault();

                if (result != null)
                {
                    foreach (DocumentElement da in result.DocumentElements)
                    {
                        if (da.ElementId == null) continue;
                        if (da.ElementTypeId == GetElementTypeFromCsvByCode("SIGNATURE").Id || da.ElementTypeId == GetElementTypeFromCsvByCode("INITIAL").Id
                            || da.ElementTypeId == GetElementTypeFromCsvByCode("PRIVATESTAMP").Id)
                        {
                            var mem = db.Users.FirstOrDefault(c => c.Id == da.ElementId);
                            da.Element.UserId = mem.Id;
                            da.Element.Name = mem.Name;
                            da.Element.Foto = mem.ImageProfile;
                        }
                        else if (da.ElementTypeId == GetElementTypeFromCsvByCode("STAMP").Id)
                        {
                            var stmp = db.Stamps.FirstOrDefault(c => c.Id == da.ElementId);
                            da.Element.Name = stmp.Descr;
                            da.Element.Foto = stmp.StampFile;
                        }
                    }


                }

                return result;
            }
        }

        public DocumentItem GetByUniqFileName(string uniqFileName, bool isDocument, bool isTemp)
        {
            DocumentItem doc = new DocumentItem();
            using (var db = new ServiceContext())
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
                    }
                }
                else
                {
                    var result = db.RotationNodeUpDocs.FirstOrDefault(c => c.Document.FileUrl.Contains(uniqFileName));
                    if (result != null)
                    {
                        doc.Id = result.Id;
                        doc.FileName = result.Document.FileUrl;
                        doc.EncryptedId = Utilities.Encrypt(result.Document.CompanyId.ToString());
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

            using (var db = new ServiceContext())
            {
                var result =
                (from doc in db.Documents
                 where (doc.CreatorId == creatorId || doc.CompanyId == companyId)
                 && (keywords.All(x => (doc.FileName).Contains(x)))
                 orderby doc.CreatedAt descending
                 select new DocumentItem
                 {
                     Id = doc.Id,
                     Extention = doc.Extention,
                     FileNameOri = doc.FileName,
                     FileName = doc.FileUrl,
                     FileSize = doc.FileSize,
                     CreatorId = doc.CreatorId,
                     CreatedAt = doc.CreatedAt,
                     Description = doc.Description,
                     MaxDownload = doc.MaxDownloadPerActivity,
                     MaxPrint = doc.MaxPrintPerActivity,
                     ExpiryDay = doc.ExpiryDay
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

        public IEnumerable<DocumentItem> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize)
        {
            return GetLiteAll(creatorId, topCriteria, page, pageSize, null, null);
        }
        public IEnumerable<DocumentItem> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize, string order)
        {
            return GetLiteAll(creatorId, topCriteria, page, pageSize, order, null);
        }
        public IEnumerable<DocumentItem> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize, string order, string criteria)
        {
            int skip = pageSize * (page - 1);
            string ordering = "CreatedAt desc";

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

            using (var db = new ServiceContext())
            {
                var result =
                (from c in db.Documents
                 where c.CreatorId == creatorId && (topCriteria == null || tops.All(x => (c.FileName).Contains(x)))
                 orderby c.CreatedAt descending
                 select new DocumentItem
                 {
                     Id = c.Id,
                     Extention = c.Extention,
                     FileName = c.FileName,
                     FileSize = c.FileSize,
                     CreatorId = c.CreatorId,
                     CreatedAt = c.CreatedAt,
                     //DocumentUsers =
                     //    (from x in c.DocumentUsers
                     //     select new DocumentUser
                     //     {
                     //         Id = x.Id,
                     //         DocumentId = x.DocumentId,
                     //        UserId = x.UserId,
                     //         FlagAction = x.FlagAction,
                     //     }).ToList(),
                 }).Skip(skip).Take(pageSize).ToList();

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
        // Author: Rani
        /*
         Changes: 
            Sorting criteria and order is fixed.
        */
        public long GetLiteAllCount(long memberId, string topCriteria)
        {
            return GetLiteAllCount(memberId, topCriteria, null);
        }
        public long GetLiteAllCount(long memberId, string topCriteria, string criteria)
        {

            if (string.IsNullOrEmpty(criteria))
                criteria = "1=1";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

            using (var db = new ServiceContext())
            {
                var result =
                    (from c in db.Documents
                     where c.CreatorId == memberId && (topCriteria == null || tops.All(x => (c.FileName).Contains(x)))
                     select new DocumentItem
                     {
                         Id = c.Id,
                     }).Count();

                return result;

            }
        }

        public IEnumerable<DocumentItem> GetLiteByCreator(long memberId, string topCriteria, int page, int pageSize)
        {
            return GetLiteByCreator(memberId, topCriteria, page, pageSize, null, null);
        }

        public IEnumerable<DocumentItem> GetLiteByCreator(long memberId, string topCriteria, int page, int pageSize, string order)
        {
            return GetLiteByCreator(memberId, topCriteria, page, pageSize, order, null);
        }

        public IEnumerable<DocumentItem> GetLiteByCreator(long memberId, string topCriteria, int page, int pageSize, string order, string criteria)
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

            using (var db = new ServiceContext())
            {
                var result =
                (from c in db.Documents
                 where c.CreatorId == memberId &&
                    (topCriteria == null || tops.All(x => (c.FileName).Contains(x)))
                 select new DocumentItem
                 {
                     Id = c.Id,
                     Extention = c.Extention,
                     FileName = c.FileName,
                     FileSize = c.FileSize,
                     CreatorId = c.CreatorId,
                     CreatedAt = c.CreatedAt,

                 }).Skip(skip).Take(pageSize).ToList();

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

        public long GetLiteByCreatorCount(long memberId, string topCriteria)
        {
            return GetLiteByCreatorCount(memberId, topCriteria, null);
        }

        public long GetLiteByCreatorCount(long memberId, string topCriteria, string criteria)
        {

            if (string.IsNullOrEmpty(criteria))
                criteria = "1=1";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

            using (var db = new ServiceContext())
            {
                var result =
                (from c in db.Documents
                 where c.CreatorId == memberId &&
                    (topCriteria == null || tops.All(x => (c.FileName).Contains(x)))
                 select new DocumentItem
                 {
                     Id = c.Id,
                 }).Count();

                return result;

            }
        }

        public IEnumerable<DocumentItem> GetLiteByTopCriteria(long companyId, string topCriteria, int page, int pageSize, string order, string criteria)
        {
            int skip = pageSize * (page - 1);
            string ordering = "Title";

            if (order != null)
                ordering = order;

            if (criteria == null)
                criteria = "1=1";

            string[] tops = new string[] { };
            if (topCriteria != null)
                tops = topCriteria.Split(' ');

            using (var db = new ServiceContext())
            {
                var result =
                    (from c in db.Documents
                     where (topCriteria == null || tops.All(x => c.FileName.Contains(x)))
                     select new DocumentItem
                     {
                         Id = c.Id,
                         Description = c.Description,
                         Extention = c.Extention,
                         FileName = c.FileName,

                     }).Skip(skip).Take(pageSize).ToList();

                return result;

            }
        }
        public int GetPermission(long memberId, long rotationNodeId, long documentId)
        {
            int ret = 0;
            using (var db = new ServiceContext())
            {
                if (rotationNodeId == 0)
                    return 0;

                if (rotationNodeId < 0)
                {
                    var rid = Math.Abs(rotationNodeId);
                    var rnx = db.RotationNodeDocs.FirstOrDefault(c => c.Document.Id == documentId && c.RotationNode.UserId == memberId && !c.RotationNode.Status.Equals((int)Constant.RotationStatus.Open) && c.RotationNode.Rotation.Id == rid);
                    if (rnx == null)
                        return 0;

                    rotationNodeId = rnx.RotationNode.Id;
                }
                var rn = db.RotationNodes.FirstOrDefault(c => c.Id == rotationNodeId);
                var rm = db.RotationUsers.FirstOrDefault(c => c.UserId == memberId && c.Rotation.Id == rn.Rotation.Id && c.WorkflowNodeId == rn.WorkflowNode.Id);
                if (rm != null)
                {
                    ret = rm.FlagPermission;
                }
            }
            return ret;
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

            using (var db = new ServiceContext())
            {
                var tmps =
                    (from c in db.DocumentElements
                     where c.ElementId == memberId && ("SIGNATURE,INITIAL").Contains(GetElementTypeFromCsvById(c.ElementTypeId).Code) && (c.Flag & 1) == 1 &&
                        (topCriteria == null || tops.All(x => (c.Document.FileName).Contains(x)))
                     orderby c.FlagDate descending
                     select new DocumentSign
                     {
                         Id = c.Document.Id,
                         CxSignature = (c.ElementTypeId == GetElementTypeFromCsvByCode("SIGNATURE").Id ? 1 : 0),
                         CxInitial = (c.ElementTypeId == GetElementTypeFromCsvByCode("INITIAL").Id ? 1 : 0),
                         DateCreated = c.FlagDate,
                     }).ToList();

                var signs =
                    (from c in tmps
                     group c by c.Id into g
                     select new DocumentSign
                     {
                         Id = g.Key,
                         CxSignature = g.Sum(c => c.CxSignature),
                         CxInitial = g.Sum(c => c.CxInitial),
                         DateCreated = g.Max(c => c.DateCreated),
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
                     DateCreated = c.DateCreated,
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
            using (var db = new ServiceContext())
            {
                var doc = db.Documents.FirstOrDefault(c => c.FileUrl.Equals(docName));
                if (doc == null)
                    return -4;
                // Harus di improve lagi untuk tau status saat ini
                // var rot = db.RotationNodeDocs.FirstOrDefault(c => c.DocumentId == doc.Id);
                var docMem = db.DocumentUsers.FirstOrDefault(c => c.DocumentId == doc.Id && c.UserId == userId);
                if (docMem == null)
                    return -4;
                // calc from rotation is completed
                // if (!rot.RotationNode.Rotation.Status.Equals("90") || (docMem.FlagPermission & (int)Constant.EnumDocumentAction.PRINT) == (int)Constant.EnumDocumentAction.PRINT)
                //    return -3;
                if ((docMem.FlagPermission & (int)Constant.EnumDocumentAction.DOWNLOAD) == (int)Constant.EnumDocumentAction.DOWNLOAD)
                    return -3;

                // out of max // There may be a race condition
                if (docMem.PrintCount + 1 > doc.MaxPrintPerActivity)
                    return -1;

                docMem.PrintCount++;
                db.SaveChanges();

                return 1;
            }
        }

        /// <summary>
        /// Requesting to print document, if user have permission and not out of limit, so the request return int 1 as status OK, also will save print counter
        /// </summary>
        /// <param name="docName"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public int RequestPrintDocument(string docName, long userId)
        {
            using (var db = new ServiceContext())
            {
                var doc = db.Documents.FirstOrDefault(c => c.FileUrl.Equals(docName));
                if (doc == null)
                    return -4;
                // Harus di improve lagi untuk tau status saat ini
                // var rot = db.RotationNodeDocs.FirstOrDefault(c => c.DocumentId == doc.Id);
                var docMem = db.DocumentUsers.FirstOrDefault(c => c.DocumentId == doc.Id && c.UserId == userId);
                if (docMem == null)
                    return -4;
                // calc from rotation is completed
                // if (!rot.RotationNode.Rotation.Status.Equals("90") || (docMem.FlagPermission & (int)Constant.EnumDocumentAction.PRINT) == (int)Constant.EnumDocumentAction.PRINT)
                //    return -3;
                if ((docMem.FlagPermission & (int)Constant.EnumDocumentAction.PRINT) == (int)Constant.EnumDocumentAction.PRINT)
                    return -3;

                // out of max // There may be a race condition
                if (docMem.PrintCount + 1 > doc.MaxPrintPerActivity)
                    return -1;

                docMem.PrintCount++;
                db.SaveChanges();

                return 1;
            }
        }

        public DocumentInboxData Save(DocumentInboxData prod, long companyId, long rotationId)
        {
            long result = 0;
            DocumentInboxData document;
            using (var db = new ServiceContext())
            {
                if (prod.Id == 0)
                    document = Create(prod, companyId, rotationId);
                else document = Update(prod, companyId, rotationId);
                if (document.Id != 0)
                    result = document.Id;
                document.DocumentElements = SaveAnnos(document.Id, (long)document.CreatorId, document.UserEmail, prod.DocumentElements);
                document.DocumentUsers = CreateDocumentUser(document.Id);
                document.DocumentUser = document.DocumentUsers.FirstOrDefault(docusr => docusr.UserId == document.CreatorId);
                if (document.DocumentUser == null) document.DocumentUser = new DocumentUserInboxData() { UserId = document.CreatorId, DocumentId = document.Id, FlagPermission = 6 };
            }
            return document;
        }
        public ICollection<DocumentElementInboxData> SaveAnnos(long documentId, long creatorId, string userEmail, IEnumerable<DocumentElementInboxData> annos)
        {
            using (var db = new ServiceContext())
            {
                //
                // prepare data 
                var cxold = db.DocumentElements.Count(c => c.DocumentId == documentId);
                var cxnew = annos.Count();
                //nambah old document
                if (cxold < cxnew)
                {
                    var ep = annos.ElementAt(0); // get 1 data for sample
                    for (var x = cxold; x < cxnew; x++)
                    {
                        DocumentElement da = new DocumentElement();
                        da.DocumentId = documentId;
                        da.Page = ep.Page;
                        da.ElementTypeId = ep.ElementTypeId;

                        da.UserId = userEmail;
                        da.CreatedAt = DateTime.Now;
                        db.DocumentElements.Add(da);
                    }
                    db.SaveChanges();
                }
                else if (cxold > cxnew)
                {
                    var dremove = db.DocumentElements.Where(c => c.Document.Id == documentId).Take(cxold - cxnew).ToList();
                    db.DocumentElements.RemoveRange(dremove);
                    db.SaveChanges();
                }
                //
                // save data (update)
                //
                var dnew = db.DocumentElements.Where(c => c.Document.Id == documentId).ToList();
                int v = 0;
                var retval = new List<DocumentElementInboxData>();
                foreach (DocumentElement da in dnew)
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
                    da.Data = epos.Data;
                    da.Data2 = epos.Data2;
                    da.Rotation = epos.Rotation;
                    da.ScaleX = epos.ScaleX;
                    da.ScaleY = epos.ScaleY;
                    da.TransitionX = epos.TransitionX;
                    da.TransitionY = epos.TransitionY;
                    da.StrokeWidth = epos.StrokeWidth;
                    da.Opacity = epos.Opacity;
                    da.Flag = epos.Flag;
                    da.FlagCode = epos.FlagCode;
                    da.FlagDate = epos.FlagDate;
                    da.FlagImage = epos.FlagImage;
                    da.CreatorId = (epos.CreatorId == null ? creatorId : epos.CreatorId);
                    da.ElementId = epos.ElementId;
                    da.Element = epos.Element;
                    da.CreatedAt = DateTime.Now;
                    v++;
                }
                db.SaveChanges();
                foreach (DocumentElement da in dnew)
                {
                    retval.Add(new DocumentElementInboxData(da));
                }
                return retval;
            }
        }

        public void SendEmailSignature(User member, string rotName, string docName, string numbers)
        {
            AppConfigGenerator appsvr = new AppConfigGenerator();
            var topaz = appsvr.GetConstant("APPLICATION_NAME")["value"];
            var admName = appsvr.GetConstant("EMAIL_USER_DISPLAY")["value"];
            EmailService emailtools = new EmailService();
            string body =
                "Dear " + member.Name + ",<br/><br/>" +
                "You have signed rotation <b>" + rotName + "</b> in document <b>" + docName + "</b>, the signature number generated: <b>" + numbers + "</b>.<br/>";

            body += "<br/><br/> " + admName + " Administrator<br/>";

            var dbx = new ServiceContext();
            var emailfrom = appsvr.GetConstant("EMAIL_USER")["value"];


            var task = emailtools.Send(emailfrom, admName + " Administrator", member.Email, admName + " User Signature", body, false, new string[] { });
        }

        public void SendEmailSignature(Member member, string rotName, string docName, string numbers)
        {
            throw new NotImplementedException();
        }

        public void SendEmailStamp(User member, string rotName, string docName, string numbers)
        {
            AppConfigGenerator appsvr = new AppConfigGenerator();
            var topaz = appsvr.GetConstant("APPL_NAME")["value"];
            var admName = appsvr.GetConstant("EMAILUSERDISPLAY")["value"];
            EmailService emailtools = new EmailService();
            string body =
                "Dear " + member.Name + ",<br/><br/>" +
                "You have stamped rotation <b>" + rotName + "</b> in document <b>" + docName + "</b>, the stamp number generated: <b>" + numbers + "</b>.<br/>";

            body += "<br/><br/> " + admName + " Administrator<br/>";

            var dbx = new ServiceContext();
            var emailfrom = appsvr.GetConstant("EMAIL_USER")["value"];

            var task = emailtools.Send(emailfrom, admName + " Administrator", member.Email, admName + " User Stamping", body, false, new string[] { });
        }

        public void SendEmailStamp(Member member, string rotName, string docName, string numbers)
        {
            throw new NotImplementedException();
        }

        public int Signature(long documentId, long memberId, long rotationId)
        {
            using (var db = new ServiceContext())
            {
                var datas = db.DocumentElements.Where(c => c.Document.Id == documentId && (c.ElementTypeId == 4 || c.ElementTypeId == 5) && c.ElementId == memberId && (c.Flag & 1) != 1).ToList();
                if (datas == null)
                    return 0;
                var user = db.Users.FirstOrDefault(c => c.Id == memberId);
                var rotnod = db.Rotations.FirstOrDefault(c => c.Id == rotationId);
                var doc = db.Documents.FirstOrDefault(c => c.Id == documentId);
                int cx = 0;
                string numbers = "";
                foreach (DocumentElement da in datas)
                {
                    var dt = DateTime.Now;
                    da.Flag = 1;
                    da.FlagDate = dt;
                    da.FlagCode = "DRD-" + dt.ToString("yyMMddHHmmssfff");
                    da.FlagImage = (da.ElementTypeId == GetElementTypeFromCsvByCode("SIGNATURE").Id ? user.ImageSignature : user.ImageInitials);
                    if (!numbers.Equals(""))
                        numbers += ", ";
                    numbers += da.FlagCode;
                    cx++;
                }
                if (cx > 0)
                {
                    db.SaveChanges();
                    User xmem = new User();
                    xmem.Id = user.Id;
                    xmem.Name = user.Name;
                    xmem.Email = user.Email;
                    SendEmailSignature(xmem, rotnod.Subject, doc.FileName, numbers);
                }
                return cx;
            }
        }

        public int Stamp(long documentId, long memberId, long rotationId)
        {
            using (var db = new ServiceContext())
            {
                var datas = db.DocumentElements.Where(c => c.Document.Id == documentId && c.ElementTypeId == GetElementTypeFromCsvByCode("PRIVATESTAMP").Id && c.ElementId == memberId && (c.Flag & 1) != 1).ToList();
                if (datas == null)
                    return 0;
                var member = db.Users.FirstOrDefault(c => c.Id == memberId);
                var rotnod = db.Rotations.FirstOrDefault(c => c.Id == rotationId);
                var doc = db.Documents.FirstOrDefault(c => c.Id == documentId);
                int cx = 0;
                string numbers = "";
                foreach (DocumentElement da in datas)
                {
                    var dt = DateTime.Now;
                    da.Flag = 1;
                    da.FlagDate = dt;
                    da.FlagCode = "DRD-" + dt.ToString("yyMMddHHmmssfff");
                    da.FlagImage = member.ImageStamp;
                    if (!numbers.Equals(""))
                        numbers += ", ";
                    numbers += da.FlagCode;
                    cx++;
                }
                if (cx > 0)
                {
                    db.SaveChanges();
                    User xmem = new User();
                    xmem.Id = member.Id;
                    xmem.Name = member.Name;
                    xmem.Email = member.Email;
                    SendEmailStamp(xmem, rotnod.Subject, doc.FileName, numbers);
                }
                return cx;
            }
        }

        public DocumentInboxData Update(DocumentInboxData newDocument, long companyId, long rotationId)
        {
            using (var db = new ServiceContext())
            {
                Document document = db.Documents.FirstOrDefault(c => c.Id == newDocument.Id);
                SubscriptionService subscriptionService = new SubscriptionService();
                BusinessPackage package = subscriptionService.getCompanyPackageByCompany(companyId);
                Usage usage = subscriptionService.GetCompanyUsage(companyId);

                // validation
                ValidateWithPlan(document, newDocument, package);
                Validate(newDocument);


                // mapping value
                document.Extention = newDocument.Extention;
                document.Description = newDocument.Description;
                document.FileName = newDocument.FileName;

                document.CreatorId = newDocument.CreatorId; // harusnya current user bukan? diinject ke newDocument pas di-controller
                document.UserEmail = newDocument.UserEmail;

                // NEW
                document.ExpiryDay = newDocument.ExpiryDay;
                document.MaxDownloadPerActivity = newDocument.MaxDownloadPerActivity;
                document.MaxPrintPerActivity = newDocument.MaxPrintPerActivity;

                // upload, get file directory, controller atau di service?
                document.FileUrl = newDocument.FileUrl;

                // update subscription storage
                usage.Storage = (usage.Storage - document.FileSize) - newDocument.FileSize; // update storage limit

                // update file size
                document.FileSize = newDocument.FileSize;
                document.UpdatedAt = DateTime.Now;

                db.SaveChanges();
                newDocument.UpdatedAt = document.UpdatedAt;
                newDocument.Id = document.Id;

                return newDocument;
            }

        }

        // Currently, this method only used when Update Document
        public bool Validate(DocumentInboxData document)
        {
            if (document.ExpiryDay < 0) throw new NotImplementedException();

            // kalo isCurrent salah gimana? pindahin ke orang pertama itu di document service???
            return true;
        }

        // Validate Document with Company's or Personal's Plan
        public bool ValidateWithPlan(Document oldDocument, DocumentInboxData newDocument, BusinessPackage package)
        {

            if (!package.IsActive) throw new NotImplementedException();

            // TANYAIN
            //if (plan.SubscriptionName != "Business") throw new NotImplementedException();

            // reach out the storage limit
            if (package.Storage < (newDocument.FileSize - oldDocument.FileSize))
                throw new NotImplementedException();
            return true;
        }
        /// <summary>
        /// Update the status updatedat of document 
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns>status db save</returns>
        public int DocumentUpdatedByRotation(long documentId)
        {
            using (var db = new ServiceContext())
            {
                var docitem = db.Documents.FirstOrDefault(d => d.Id == documentId);
                docitem.UpdatedAt = DateTime.Now;
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// Make the document not currently in rotation
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns>status db save</returns>
        public int DocumentRemovedofRevisedFromRotation(long documentId)
        {
            using (var db = new ServiceContext())
            {
                var docitem = db.Documents.FirstOrDefault(d => d.Id == documentId);
                docitem.IsCurrent = false;
                docitem.UpdatedAt = DateTime.Now;
                return db.SaveChanges();
            }
        }
    }
}
