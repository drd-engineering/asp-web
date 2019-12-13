using System;
using System.Collections.Generic;
using System.Linq;
using DRD.Models;
using DRD.Models.API.List;
using DRD.Models.Custom;
using DRD.Service.Context;

namespace DRD.Service
{
    public class DocumentService : IDocumentService
    {
        private readonly string _connString;
        public DocumentService()
        {
            _connString = Constant.CONSTRING;
        }
        public DocumentItem GetByUniqFileName(string uniqFileName, bool isDocument)
        {
            DocumentItem doc = new DocumentItem();
            using (var db = new ServiceContext())
            {
                if (isDocument)
                {
                    var result = db.Documents.FirstOrDefault(c => c.FileName.Contains(uniqFileName));
                    if (result != null)
                    {
                        doc.Id = result.Id;
                        doc.FileName = result.FileName;
                    }
                }
                else
                {
                    var result = db.RotationNodeUpDocs.FirstOrDefault(c => c.Document.FileName.Contains(uniqFileName));
                    if (result != null)
                    {
                        doc.Id = result.Id;
                        doc.FileName = result.Document.FileName;
                    }
                }
            }
            return doc;
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
                         Title = c.Title,
                         Description = c.Description,
                         FileName = c.FileName,
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
                                ElementType = new ElementType
                                {
                                    Id = x.ElementType.Id,
                                    Code = x.ElementType.Code,
                                }
                            }).ToList(),
                     }).FirstOrDefault();

                if (result != null)
                {
                    foreach (DocumentElement da in result.DocumentElements)
                    {
                        if (da.ElementId == null) continue;
                        if (da.ElementType.Code.Equals("SIGNATURE") || da.ElementType.Code.Equals("INITIAL") || da.ElementType.Code.Equals("PRIVATESTAMP"))
                        {
                            var mem = db.Users.FirstOrDefault(c => c.Id == da.ElementId);
                            da.Element.UserId = mem.Id;
                            da.Element.Name = mem.Name;
                            da.Element.Foto = mem.ImageProfile;
                        }
                        else if (da.ElementType.Code.Equals("STAMP"))
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
                 where c.CreatorId == creatorId  && (topCriteria == null || tops.All(x => (c.Title).Contains(x)))
                 orderby c.CreatedAt descending
                 select new DocumentItem
                 {
                     Id = c.Id,
                     Title = c.Title,
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
        public IEnumerable<DocumentItem> GetAll(long creatorId, string searchKeyword, int page, int pageSize)
        {
            int skip = pageSize * (page - 1);

            // Search keywords
            string[] keywords = new string[] { };
            if (!string.IsNullOrEmpty(searchKeyword))
                keywords = searchKeyword.Split(' ');
            else
                searchKeyword = null;

            using (var db = new ServiceContext())
            {
                var result =
                (from doc in db.Documents
                 where doc.CreatorId == creatorId && (searchKeyword == null || keywords.All(x => (doc.Title).Contains(x)))
                 orderby doc.CreatedAt descending
                 select new DocumentItem
                 {
                     Id = doc.Id,
                     Title = doc.Title,
                     FileName = doc.FileName,
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

        public long GetAllCount(long memberId, string searchKeyword)
        {
            string[] keywords = new string[] { };
            if (!string.IsNullOrEmpty(searchKeyword))
                keywords = searchKeyword.Split(' ');
            else
                searchKeyword = null;

            using (var db = new ServiceContext())
            {
                var result =
                    (from c in db.Documents
                     where c.CreatorId == memberId && (searchKeyword == null || keywords.All(x => (c.Title).Contains(x)))
                     select new DocumentItem
                     {
                         Id = c.Id,
                     }).Count();

                return result;

            }
        }



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
                     where c.CreatorId == memberId  && (topCriteria == null || tops.All(x => (c.Title).Contains(x)))
                     select new DocumentItem
                     {
                         Id = c.Id,
                     }).Count();

                return result;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="order"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
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
                     where (topCriteria == null || tops.All(x => c.Title.Contains(x)))
                     select new DocumentItem
                     {
                         Id = c.Id,
                         Description = c.Description,
                         Title = c.Title,
                         FileName = c.FileName,
                         
                     }).Skip(skip).Take(pageSize).ToList();

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
                    (topCriteria == null || tops.All(x => (c.Title ).Contains(x)))
                 select new DocumentItem
                 {
                     Id = c.Id,
                     Title = c.Title,
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
                    (topCriteria == null || tops.All(x => (c.Title ).Contains(x)))
                 select new DocumentItem
                 {
                     Id = c.Id,
                 }).Count();

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

            using (var db = new ServiceContext())
            {
                var tmps =
                    (from c in db.DocumentElements
                     where c.ElementId == memberId && ("SIGNATURE,INITIAL").Contains(c.ElementType.Code) && (c.Flag & 1) == 1 &&
                        (topCriteria == null || tops.All(x => (c.Document.Title ).Contains(x)))
                     orderby c.FlagDate descending
                     select new DocumentSign
                     {
                         Id = c.Document.Id,
                         CxSignature = (c.ElementType.Code.Equals("SIGNATURE") ? 1 : 0),
                         CxInitial = (c.ElementType.Code.Equals("INITIAL") ? 1 : 0),
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
                     Title = d.Title,
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
                     where c.CreatorId == memberId && !("SIGNATURE,INITIAL").Contains(c.ElementType.Code) &&
                            (topCriteria == null || tops.All(x => (c.Document.Title ).Contains(x)))
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
                     Title = d.Title,
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
        public int Save(Document prod)
        {
            int result;
            Document document;
            using (var db = new ServiceContext())
            {
                if (prod.Id == 0)
                    document = Create(prod);
                else document = Update(prod);
                result = db.SaveChanges();
            }

            SaveAnnos(document.Id, (long)document.CreatorId, document.UserEmail, document.DocumentElements);
            return result;

        }

        public Document Create(Document newDocument)
        {
            using (var db = new ServiceContext())
            {
                Document document = new Document();

                // validate first
                long companyId = db.Members.Where(m => m.UserId == newDocument.CreatorId).FirstOrDefault().CompanyId;
                PlanBusiness plan = db.PlanBusinesses.Where(c => c.CompanyId == companyId).FirstOrDefault();
                ValidateWithPlan(document, newDocument, plan);

                // mapping value
                document.Title = newDocument.Title;
                document.Description = newDocument.Description;
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
                plan.StorageUsedinByte = plan.StorageUsedinByte - document.FileSize; // update storage limit
                

                db.Documents.Add(document);

                db.SaveChanges();

                return document;
            }
            
        }

        public Document Update(Document newDocument) {
            

            using (var db = new ServiceContext())
            {
                Document document = db.Documents.FirstOrDefault(c => c.Id == newDocument.Id);
                
                // GET Plan
                long companyId = db.Members.Where(m => m.UserId == document.CreatorId).FirstOrDefault().CompanyId;
                PlanBusiness plan = db.PlanBusinesses.Where(c => c.CompanyId == companyId).FirstOrDefault();
                
                // validation
                ValidateWithPlan(document, newDocument, plan);
                Validate(newDocument);


                // mapping value
                document.Title = newDocument.Title;
                document.Description = newDocument.Description;
                document.FileName = newDocument.FileName;

                document.CreatorId = newDocument.CreatorId; // harusnya current user bukan? diinject ke newDocument pas di-controller
                document.UserEmail = newDocument.UserEmail;
                document.CreatedAt = DateTime.Now;

                // NEW
                document.ExpiryDay = newDocument.ExpiryDay;
                document.MaxDownloadPerActivity = newDocument.MaxDownloadPerActivity;
                document.MaxPrintPerActivity = newDocument.MaxPrintPerActivity;

                // upload, get file directory, controller atau di service?
                document.FileUrl = newDocument.FileUrl;

                // update subscription storage
                plan.StorageUsedinByte = (plan.StorageUsedinByte - document.FileSize) - newDocument.FileSize; // update storage limit

                // update file size
                document.FileSize = newDocument.FileSize;
                document.UpdatedAt = DateTime.Now;
                db.SaveChanges();

                return document;
            }

        }

        // Validate Document with Company's or Personal's Plan
        public bool ValidateWithPlan(Document oldDocument, Document newDocument, PlanBusiness plan) {

            if (!plan.IsActive) throw new NotImplementedException();
            
            // TANYAIN
            //if (plan.SubscriptionName != "Business") throw new NotImplementedException();

            // reach out the storage limit
            if (plan.StorageUsedinByte < (newDocument.FileSize - oldDocument.FileSize)) 
                throw new NotImplementedException();
            return true;
        }

        // Currently, this method only used when Update Document
        public bool Validate(Document document)
        {
            if (document.ExpiryDay < 0) throw new NotImplementedException();
            
            // kalo isCurrent salah gimana? pindahin ke orang pertama itu di document service???
            return true;
        }

        public void DoRevision(long documentId) {

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

        public int SaveAnnos(long documentId, long creatorId, string userEmail, IEnumerable<DocumentElement> annos)
        {
            using (var db = new ServiceContext())
            {
                //
                // prepare data 
                //
                var cxold = db.DocumentElements.Count(c => c.Document.Id == documentId);
                var cxnew = annos.Count();
                //nambah old document
                if (cxold < cxnew)
                {
                    var ep = annos.ElementAt(0); // get 1 data for sample
                    for (var x = cxold; x < cxnew; x++)
                    {
                        DocumentElement da = new DocumentElement();
                        da.Document.Id = documentId;
                        da.Page = ep.Page;
                        da.ElementType.Id = ep.ElementType.Id;
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
                foreach (DocumentElement da in dnew)
                {
                    var epos = annos.ElementAt(v);
                    da.Document.Id = documentId;
                    da.Page = epos.Page;
                    da.ElementType.Id = epos.ElementType.Id;
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
                    //da.UserId = userEmail;
                    da.CreatedAt = DateTime.Now;
                    v++;
                }
                return db.SaveChanges();

            }
        }

        public int CheckingSignature(long memberId)
        {
            int ret = 1;
            using (var db = new ServiceContext())
            {
                var mem = db.Users.FirstOrDefault(c => c.Id == memberId);
                if (mem.ImageSignature == null || mem.ImageInitials == null || mem.ImageKtp1 == null || mem.ImageKtp2 == null || string.IsNullOrEmpty(""+mem.OfficialIdNo))
                    ret = -1;
            }
            return ret;
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

        public int Signature(long documentId, long memberId, long rotationId)
        {
            using (var db = new ServiceContext())
            {
                var datas = db.DocumentElements.Where(c => c.Document.Id == documentId && ("SIGNATURE, INITIAL").Contains(c.ElementType.Code) && c.ElementId == memberId && (c.Flag & 1) != 1).ToList();
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
                    da.FlagImage = (da.ElementType.Code.Equals("SIGNATURE") ? member.ImageSignature : member.ImageInitials);
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
                    sendEmailSignature(xmem, rotnod.Subject, doc.Title, numbers);
                }
                return cx;
            }
        }

        public void sendEmailSignature(User member, string rotName, string docName, string numbers)
        {
            AppConfigGenerator appsvr = new AppConfigGenerator();
            var topaz = appsvr.GetConstant("APPL_NAME")["value"];
            var admName = appsvr.GetConstant("EMAILUSERDISPLAY")["value"];
            EmailService emailtools = new EmailService();
            string body =
                "Dear " + member.Name + ",<br/><br/>" +
                "You have signed rotation <b>" + rotName + "</b> in document <b>" + docName + "</b>, the signature number generated: <b>" + numbers + "</b>.<br/>";

            body += "<br/><br/> " + admName + " Administrator<br/>";

            var dbx = new ServiceContext();
            var emailfrom = appsvr.GetConstant("EMAIL_USER")["value"];


            var task = emailtools.Send(emailfrom, admName + " Administrator", member.Email, admName + " User Signature", body, false, new string[] { });
        }

        public int SaveCxPrint(string docName, long memberId)
        {
            using (var db = new ServiceContext())
            {
                var doc = db.Documents.FirstOrDefault(c => c.FileName.Equals(docName));
                var rot = db.RotationNodeDocs.FirstOrDefault(c => c.Document.Id == doc.Id);
                var docmem = db.DocumentUsers.FirstOrDefault(c => c.DocumentId == doc.Id && c.UserId == memberId);
                if (docmem == null)
                    return -4;

                // calc from rotation is completed
                if (!rot.RotationNode.Rotation.Status.Equals("90"))
                    return -3;


                // out of max
                //if (docmem.CxPrint + 1 > doc.MaxPrint)
                //    return -1;

                //docmem.CxPrint++;
                db.SaveChanges();

                return 1;
            }
        }

        public int SaveCxDownload(string docName, long memberId)
        {
            using (var db = new ServiceContext())
            {
                var doc = db.Documents.FirstOrDefault(c => c.FileName.Equals(docName));
                var rot = db.RotationNodeDocs.FirstOrDefault(c => c.Document.Id == doc.Id);
                //var docmem = db.DocumentUsers.FirstOrDefault(c => c.DocumentId == doc.Id && c.MemberId == memberId);
                //if (docmem == null)
                //    return -4;

                //// calc from rotation is completed
                //if (!rot.RotationNode.Rotation.Status.Equals("90"))
                //    return -3;

                //// expired
                //if (rot.RotationNode.Rotation.DateUpdated.Value.AddDays(doc.ExpiryDay) < DateTime.Now)
                //    return -2;

                //// out of max
                //if (docmem.CxDownload + 1 > doc.MaxDownload)
                //    return -1;

                //docmem.CxDownload++;
                db.SaveChanges();

                return 1;
            }
        }

        public int Stamp(long documentId, long memberId, long rotationId)
        {
            using (var db = new ServiceContext())
            {
                var datas = db.DocumentElements.Where(c => c.Document.Id == documentId && c.ElementType.Code.Equals("PRIVATESTAMP") && c.ElementId == memberId && (c.Flag & 1) != 1).ToList();
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
                    sendEmailStamp(xmem, rotnod.Subject, doc.Title, numbers);
                }
                return cx;
            }
        }

        public void sendEmailStamp(User member, string rotName, string docName, string numbers)
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

                    var docs = db.DocumentElements.Where(c => c.Document.Id == documentId && c.ElementId == memberId && ("SIGNATURE,INITIAL,PRIVATESTAMP").Contains(c.ElementType.Code)).ToList();
                    foreach (DocumentElement doc in docs)
                    {
                        if (("SIGNATURE,INITIAL").Contains(doc.ElementType.Code))
                            ret |= 1;
                        else if (doc.ElementType.Code.Equals("PRIVATESTAMP"))
                            ret |= 32;
                    }
                }
            }
            return ret;
        }

        public void sendEmailSignature(Member member, string rotName, string docName, string numbers)
        {
            throw new NotImplementedException();
        }

        public void sendEmailStamp(Member member, string rotName, string docName, string numbers)
        {
            throw new NotImplementedException();
        }

        
    }
}
