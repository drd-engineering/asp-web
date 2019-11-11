using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using DRD.Domain;
using System.Configuration;
using System.Data.Entity.Infrastructure;

namespace DRD.Service
{
    public class DocumentService
    {
        private readonly string _connString;
        public DocumentService()
        {
            _connString = Constant.CONSTRING;
        }
        // public DtoDocumentLite GetByUniqFileName(string uniqFileName, bool isDocument)
        // {
        //     DtoDocumentLite doc = new DtoDocumentLite();
        //     using (var db = new DrdContext(_connString))
        //     {
        //         if (isDocument)
        //         {
        //             var result = db.Documents.FirstOrDefault(c => c.FileName.Contains(uniqFileName));
        //             if (result != null)
        //             {
        //                 doc.Id = result.Id;
        //                 doc.FileName = result.FileName;
        //                 doc.FileNameOri = result.FileNameOri;
        //                 doc.ExtFile = result.ExtFile;
        //             }
        //         }
        //         else
        //         {
        //             var result = db.RotationNodeUpDocs.FirstOrDefault(c => c.DocumentUpload.FileName.Contains(uniqFileName));
        //             if (result != null)
        //             {
        //                 doc.Id = result.Id;
        //                 doc.FileName = result.DocumentUpload.FileName;
        //                 doc.FileNameOri = result.DocumentUpload.FileNameOri;
        //                 doc.ExtFile = result.DocumentUpload.ExtFile;
        //             }
        //         }
        //     }
        //     return doc;
        // }

        public DtoDocument GetById(long id)
        {

            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Documents
                     where c.Id == id
                     select new DtoDocument
                     {
                         Id = c.Id,
                         Title = c.Title,
                         Descr = c.Descr,
                         FileName = c.FileName,
                         FileNameOri = c.FileNameOri,
                         ExtFile = c.ExtFile,
                         FileSize = c.FileSize,
                         MaxPrint = c.MaxPrint,
                         MaxDownload = c.MaxDownload,
                         ExpiryDay = c.ExpiryDay,
                         FileFlag = c.FileFlag,
                         Version = c.Version,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                         CreatorId = c.CreatorId,
                         UserId = c.UserId,
                         //DocumentMembers = (
                         //   from x in c.DocumentMembers
                         //   select new DtoDocumentMember
                         //   {
                         //       Id = x.Id,
                         //       MemberId = x.MemberId,
                         //       MemberNumber = x.Member.Number,
                         //       MemberName = x.Member.Name,
                         //       //FlagPermission = x.FlagPermission,
                         //   }).ToList(),
                         DocumentAnnotates = (
                            from x in c.DocumentAnnotates
                            select new DtoDocumentAnnotate
                            {
                                Id = x.Id,
                                DocumentId = x.DocumentId,
                                Page = x.Page,
                                AnnotateTypeId = x.AnnotateTypeId,
                                LeftPos = x.LeftPos,
                                TopPos = x.TopPos,
                                WidthPos = x.WidthPos,
                                HeightPos = x.HeightPos,
                                Color = x.Color,
                                BackColor = x.BackColor,
                                Data = x.Data,
                                Data2 = x.Data2,
                                Rotation = x.Rotation,
                                ScaleX = x.ScaleX,
                                ScaleY = x.ScaleY,
                                TransX = x.TransX,
                                TransY = x.TransY,
                                StrokeWidth = x.StrokeWidth,
                                Opacity = x.Opacity,
                                Flag = x.Flag,
                                FlagCode = x.FlagCode,
                                FlagDate = x.FlagDate,
                                FlagImage = x.FlagImage,
                                CreatorId = x.CreatorId,
                                AnnotateId = x.AnnotateId,
                                UserId = x.UserId,
                                DateCreated = x.DateCreated,
                                DateUpdated = x.DateUpdated,
                                AnnotateType = new DtoAnnotateType
                                {
                                    Id = x.AnnotateType.Id,
                                    Code = x.AnnotateType.Code,
                                }
                            }).ToList(),
                     }).FirstOrDefault();

                if (result != null)
                {
                    foreach (DtoDocumentAnnotate da in result.DocumentAnnotates)
                    {
                        if (da.AnnotateId == null) continue;
                        if (da.AnnotateType.Code.Equals("SIGNATURE") || da.AnnotateType.Code.Equals("INITIAL") || da.AnnotateType.Code.Equals("PRIVATESTAMP"))
                        {
                            var mem = db.Members.FirstOrDefault(c => c.Id == da.AnnotateId);
                            da.Annotate.Number = mem.Number;
                            da.Annotate.Name = mem.Name;
                            da.Annotate.Foto = mem.ImageProfile;
                            //if (da.Flag == 1)
                            //{
                            //    if (da.AnnotateType.Code.Equals("SIGNATURE"))
                            //        da.Annotate.Foto = mem.ImageSignature;
                            //    else if (da.AnnotateType.Code.Equals("INITIAL"))
                            //        da.Annotate.Foto = mem.ImageInitials;
                            //    else if (da.AnnotateType.Code.Equals("PRIVATESTAMP"))
                            //        da.Annotate.Foto = mem.ImageStamp;
                            //}
                            //da.Annotate.Signature = (mem.ImageSignature == null ? "nosignature.jpg" : mem.ImageSignature);
                            //da.Annotate.Initial = (mem.ImageInitials == null ? "nosignature.jpg" : mem.ImageInitials);
                        }
                        else if (da.AnnotateType.Code.Equals("STAMP"))
                        {
                            var stmp = db.Stamps.FirstOrDefault(c => c.Id == da.AnnotateId);
                            da.Annotate.Name = stmp.Descr;
                            da.Annotate.Foto = stmp.StampFile;
                        }
                    }


                }

                return result;
            }
        }

        public IEnumerable<DtoDocumentLite> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize)
        {
            return GetLiteAll(creatorId, topCriteria, page, pageSize, null, null);
        }
        public IEnumerable<DtoDocumentLite> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize, string order)
        {
            return GetLiteAll(creatorId, topCriteria, page, pageSize, order, null);
        }
        public IEnumerable<DtoDocumentLite> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize, string order, string criteria)
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

            using (var db = new DrdContext(_connString))
            {
                var result =
                (from c in db.Documents
                 where c.CreatorId == creatorId && c.MemberFolderId == 1 && (topCriteria == null || tops.All(x => (c.Title + " " + c.FileNameOri + " " + c.Version).Contains(x)))
                 select new DtoDocumentLite
                 {
                     Id = c.Id,
                     Title = c.Title,
                     FileName = c.FileName,
                     FileNameOri = c.FileNameOri,
                     ExtFile = c.ExtFile,
                     FileSize = c.FileSize,
                     ExpiryDay = c.ExpiryDay,
                     MaxPrint = c.MaxPrint,
                     MaxDownload = c.MaxDownload,
                     Version = c.Version,
                     CreatorId = c.CreatorId,
                     DateCreated = c.DateCreated,
                     DocumentMembers =
                         (from x in c.DocumentMembers
                          select new DtoDocumentMember
                          {
                              Id = x.Id,
                              DocumentId = x.DocumentId,
                              MemberId = x.MemberId,
                              //FlagPermission = x.FlagPermission,
                              FlagAction = x.FlagAction,
                          }).ToList(),
                     //DocumentAnnotates = (
                     //       from x in c.DocumentAnnotates
                     //       select new DtoDocumentAnnotate
                     //       {
                     //           Id = x.Id,
                     //           DocumentId = x.DocumentId,
                     //           Page = x.Page,
                     //           AnnotateTypeId = x.AnnotateTypeId,
                     //           LeftPos = x.LeftPos,
                     //           TopPos = x.TopPos,
                     //           WidthPos = x.WidthPos,
                     //           HeightPos = x.HeightPos,
                     //           Color = x.Color,
                     //           BackColor = x.BackColor,
                     //           Data = x.Data,
                     //           Data2 = x.Data2,
                     //           Rotation = x.Rotation,
                     //           ScaleX = x.ScaleX,
                     //           ScaleY = x.ScaleY,
                     //           TransX = x.TransX,
                     //           TransY = x.TransY,
                     //           StrokeWidth = x.StrokeWidth,
                     //           Opacity = x.Opacity,
                     //           Flag = x.Flag,
                     //           FlagCode = x.FlagCode,
                     //           CreatorId = x.CreatorId,
                     //           AnnotateId = x.AnnotateId,
                     //           UserId = x.UserId,
                     //           DateCreated = x.DateCreated,
                     //           DateUpdated = x.DateUpdated,
                     //           AnnotateType = new DtoAnnotateType
                     //           {
                     //               Id = x.AnnotateType.Id,
                     //               Code = x.AnnotateType.Code,
                     //           }
                     //       }).ToList(),
                 }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                if (result != null)
                {
                    MenuService musvr = new MenuService();
                    foreach (DtoDocumentLite dl in result)
                    {
                        dl.Key = musvr.EncryptData(dl.Id);
                    }
                }

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

            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Documents
                     where c.CreatorId == memberId && c.MemberFolderId == 1 && (topCriteria == null || tops.All(x => (c.Title + " " + c.FileNameOri + " " + c.Version).Contains(x)))
                     select new DtoDocumentLite
                     {
                         Id = c.Id,
                     }).Where(criteria).Count();

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
        public IEnumerable<DtoDocumentLite> GetLiteByTopCriteria(long companyId, string topCriteria, int page, int pageSize, string order, string criteria)
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

            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Documents
                     where c.CompanyId == companyId && (c.FileFlag & 1) != 1 && (topCriteria == null || tops.All(x => c.Title.Contains(x)))
                     select new DtoDocumentLite
                     {
                         Id = c.Id,
                         Descr = c.Descr,
                         Title = c.Title,
                         CompanyId = c.CompanyId,
                         ExtFile = c.ExtFile,
                         FileName = c.FileName,
                         FileNameOri = c.FileNameOri,
                         Version = c.Version,
                         DocumentMembers =
                         (from x in c.DocumentMembers
                          select new DtoDocumentMember
                          {
                              Id = x.Id,
                              DocumentId = x.DocumentId,
                              MemberId = x.MemberId,
                              //FlagPermission = x.FlagPermission,
                              FlagAction = x.FlagAction,
                          }).ToList(),
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                return result;

            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="creatorId"></param>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        //public IEnumerable<DtoDocumentLite> GetLiteSelectedAll(long memberId, string topCriteria, int page, int pageSize)
        //{
        //    return GetLiteSelectedAll(memberId, topCriteria, page, pageSize, null, null);
        //}
        //public IEnumerable<DtoDocumentLite> GetLiteSelectedAll(long memberId, string topCriteria, int page, int pageSize, string order)
        //{
        //    return GetLiteSelectedAll(memberId, topCriteria, page, pageSize, order, null);
        //}
        //public IEnumerable<DtoDocumentLite> GetLiteSelectedAll(long memberId, string topCriteria, int page, int pageSize, string order, string criteria)
        //{
        //    int skip = pageSize * (page - 1);
        //    string ordering = "DateCreated desc";

        //    if (!string.IsNullOrEmpty(order))
        //        ordering = order;

        //    if (string.IsNullOrEmpty(criteria))
        //        criteria = "1=1";

        //    // top criteria
        //    string[] tops = new string[] { };
        //    if (!string.IsNullOrEmpty(topCriteria))
        //        tops = topCriteria.Split(' ');
        //    else
        //        topCriteria = null;

        //    using (var db = new DrdContext(_connString))
        //    {
        //        var result =
        //        (from c in db.DocumentMembers
        //         where c.MemberId == memberId && (c.FlagPermission & 64) == 64 &&
        //            (topCriteria == null || tops.All(x => (c.Document.Title + " " + c.Document.FileNameOri + " " + c.Document.Version).Contains(x)))
        //         select new DtoDocumentLite
        //         {
        //             Id = c.Document.Id,
        //             Title = c.Document.Title,
        //             FileName = c.Document.FileName,
        //             FileNameOri = c.Document.FileNameOri,
        //             ExtFile = c.Document.ExtFile,
        //             FileSize = c.Document.FileSize,
        //             ExpiryDay = c.Document.ExpiryDay,
        //             MaxPrint = c.Document.MaxPrint,
        //             MaxDownload = c.Document.MaxDownload,
        //             Version = c.Document.Version,
        //             CreatorId = c.Document.CreatorId,
        //             DateCreated = c.Document.DateCreated,
        //             DocumentMembers =
        //                 (from x in c.Document.DocumentMembers
        //                  select new DtoDocumentMember
        //                  {
        //                      Id = x.Id,
        //                      DocumentId = x.DocumentId,
        //                      MemberId = x.MemberId,
        //                      FlagPermission = x.FlagPermission,
        //                      FlagAction = x.FlagAction,
        //                  }).ToList(),

        //         }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

        //        if (result != null)
        //        {
        //            MenuService musvr = new MenuService();
        //            foreach (DtoDocumentLite dl in result)
        //            {
        //                dl.Key = musvr.EncryptData(dl.Id);
        //            }
        //        }

        //        return result;

        //    }
        //}

        //public long GetLiteSellectedAllCount(long memberId, string topCriteria)
        //{
        //    return GetLiteSellectedAllCount(memberId, topCriteria, null);
        //}
        //public long GetLiteSellectedAllCount(long memberId, string topCriteria, string criteria)
        //{

        //    if (string.IsNullOrEmpty(criteria))
        //        criteria = "1=1";

        //    // top criteria
        //    string[] tops = new string[] { };
        //    if (!string.IsNullOrEmpty(topCriteria))
        //        tops = topCriteria.Split(' ');
        //    else
        //        topCriteria = null;

        //    using (var db = new DrdContext(_connString))
        //    {
        //        var result =
        //            (from c in db.DocumentMembers
        //             where c.MemberId == memberId && (c.FlagPermission & 64) == 64 &&
        //                (topCriteria == null || tops.All(x => (c.Document.Title + " " + c.Document.FileNameOri + " " + c.Document.Version).Contains(x)))
        //             select new DtoDocumentLite
        //             {
        //                 Id = c.Id,
        //             }).Where(criteria).Count();

        //        return result;

        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="order"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>

        public IEnumerable<DtoDocumentLite> GetLiteByCreator(long memberId, string topCriteria, int page, int pageSize)
        {
            return GetLiteByCreator(memberId, topCriteria, page, pageSize, null, null);
        }
        public IEnumerable<DtoDocumentLite> GetLiteByCreator(long memberId, string topCriteria, int page, int pageSize, string order)
        {
            return GetLiteByCreator(memberId, topCriteria, page, pageSize, order, null);
        }
        public IEnumerable<DtoDocumentLite> GetLiteByCreator(long memberId, string topCriteria, int page, int pageSize, string order, string criteria)
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

            using (var db = new DrdContext(_connString))
            {
                var result =
                (from c in db.Documents
                 where c.CreatorId == memberId &&
                    (topCriteria == null || tops.All(x => (c.Title + " " + c.FileNameOri + " " + c.Version).Contains(x)))
                 select new DtoDocumentLite
                 {
                     Id = c.Id,
                     Title = c.Title,
                     FileName = c.FileName,
                     FileNameOri = c.FileNameOri,
                     ExtFile = c.ExtFile,
                     FileSize = c.FileSize,
                     ExpiryDay = c.ExpiryDay,
                     MaxPrint = c.MaxPrint,
                     MaxDownload = c.MaxDownload,
                     Version = c.Version,
                     CreatorId = c.CreatorId,
                     DateCreated = c.DateCreated,
                     DocumentMembers =
                         (from x in c.DocumentMembers
                          select new DtoDocumentMember
                          {
                              Id = x.Id,
                              DocumentId = x.DocumentId,
                              MemberId = x.MemberId,
                              //FlagPermission = x.FlagPermission,
                              FlagAction = x.FlagAction,
                          }).ToList(),

                 }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                if (result != null)
                {
                    MenuService musvr = new MenuService();
                    foreach (DtoDocumentLite dl in result)
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

            using (var db = new DrdContext(_connString))
            {
                var result =
                (from c in db.Documents
                 where c.CreatorId == memberId &&
                    (topCriteria == null || tops.All(x => (c.Title + " " + c.FileNameOri + " " + c.Version).Contains(x)))
                 select new DtoDocumentLite
                 {
                     Id = c.Id,
                 }).Where(criteria).Count();

                return result;

            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="order"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public IEnumerable<DtoDocumentSign> GetSignatureDocs(long memberId, string topCriteria, int page, int pageSize, string order, string criteria)
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

            using (var db = new DrdContext(_connString))
            {
                var tmps =
                    (from c in db.DocumentAnnotates
                     where c.AnnotateId == memberId && ("SIGNATURE,INITIAL").Contains(c.AnnotateType.Code) && (c.Flag & 1) == 1 &&
                        (topCriteria == null || tops.All(x => (c.Document.Title + " " + c.Document.FileNameOri + " " + c.Document.Version).Contains(x)))
                     orderby c.FlagDate descending
                     select new DtoDocumentSign
                     {
                         Id = c.DocumentId,
                         CxSignature = (c.AnnotateType.Code.Equals("SIGNATURE") ? 1 : 0),
                         CxInitial = (c.AnnotateType.Code.Equals("INITIAL") ? 1 : 0),
                         DateCreated = c.FlagDate,
                     }).ToList();

                var signs =
                    (from c in tmps
                     group c by c.Id into g
                     select new DtoDocumentSign
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
                 select new DtoDocumentSign
                 {
                     Id = c.Id,
                     Title = d.Title,
                     FileName = d.FileName,
                     FileNameOri = d.FileNameOri,
                     ExtFile = d.ExtFile,
                     Version = d.Version,
                     CxSignature = c.CxSignature,
                     CxInitial = c.CxInitial,
                     RowCount = rowCount,
                     DateCreated = c.DateCreated,
                 }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                return result;

            }
        }
        public IEnumerable<DtoDocumentSign> GetSignatureDocs(long memberId, string topCriteria, int page, int pageSize, string order)
        {
            return GetSignatureDocs(memberId, topCriteria, page, pageSize, order, null);
        }
        public IEnumerable<DtoDocumentSign> GetSignatureDocs(long memberId, string topCriteria, int page, int pageSize)
        {
            return GetSignatureDocs(memberId, topCriteria, page, pageSize, null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="order"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public IEnumerable<DtoDocumentSign> GetAnnotateDocs(long memberId, string topCriteria, int page, int pageSize, string order, string criteria)
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

            using (var db = new DrdContext(_connString))
            {
                var tmps =
                    (from c in db.DocumentAnnotates
                     where c.CreatorId == memberId && !("SIGNATURE,INITIAL").Contains(c.AnnotateType.Code) &&
                            (topCriteria == null || tops.All(x => (c.Document.Title + " " + c.Document.FileNameOri + " " + c.Document.Version).Contains(x)))
                     orderby c.FlagDate descending
                     select new DtoDocumentSign
                     {
                         Id = c.DocumentId,
                         CxAnnotate = 1,
                         DateCreated = c.FlagDate,
                     }).ToList();

                var signs =
                    (from c in tmps
                     group c by c.Id into g
                     select new DtoDocumentSign
                     {
                         Id = g.Key,
                         CxAnnotate = g.Sum(c => c.CxAnnotate),
                         DateCreated = g.Max(c => c.DateCreated),
                     }).ToList();

                var rowCount = signs.Count();

                var result =
                (from c in signs
                 join d in db.Documents on c.Id equals d.Id
                 select new DtoDocumentSign
                 {
                     Id = c.Id,
                     Title = d.Title,
                     FileName = d.FileName,
                     FileNameOri = d.FileNameOri,
                     ExtFile = d.ExtFile,
                     Version = d.Version,
                     CxAnnotate = c.CxAnnotate,
                     RowCount = rowCount,
                     DateCreated = c.DateCreated,
                 }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                return result;

            }
        }
        public IEnumerable<DtoDocumentSign> GetAnnotateDocs(long memberId, string topCriteria, int page, int pageSize, string order)
        {
            return GetAnnotateDocs(memberId, topCriteria, page, pageSize, order, null);
        }
        public IEnumerable<DtoDocumentSign> GetAnnotateDocs(long memberId, string topCriteria, int page, int pageSize)
        {
            return GetAnnotateDocs(memberId, topCriteria, page, pageSize, null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prod"></param>
        /// <returns></returns>
        public int Save(DtoDocument prod)
        {

            Document product;
            int result = 0;

            using (var db = new DrdContext(_connString))
            {
                if (prod.Id != 0)
                    product = db.Documents.FirstOrDefault(c => c.Id == prod.Id);
                else
                    product = new Document();

                product.Title = prod.Title;
                product.Descr = prod.Descr;
                product.FileName = prod.FileName;
                product.FileNameOri = prod.FileNameOri;
                product.ExtFile = prod.ExtFile;
                product.FileSize = prod.FileSize;
                product.MemberFolderId = 1;
                product.MaxPrint = prod.MaxPrint;
                product.MaxDownload = prod.MaxDownload;
                product.ExpiryDay = prod.ExpiryDay;
                product.Version = prod.Version;
                product.CreatorId = prod.CreatorId;
                product.UserId = prod.UserId;
                product.CompanyId = prod.CompanyId;
                if (prod.Id == 0)
                {
                    product.DateCreated = DateTime.Now;
                    db.Documents.Add(product);
                }
                else
                    product.DateUpdated = DateTime.Now;

                result = db.SaveChanges();

                SaveAnnos(product.Id, (long)prod.CreatorId, prod.UserId, prod.DocumentAnnotates);
                return result;
            }

        }

        public ICollection<DtoDocumentAnnotate> FillAnnos(DtoDocument doc)
        {
            using (var db = new DrdContext(_connString))
            {
                var annos =
                    (from x in db.DocumentAnnotates
                     where x.DocumentId == doc.Id
                     select new DtoDocumentAnnotate
                     {
                         Page = x.Page,
                         AnnotateTypeId = x.AnnotateTypeId,
                         LeftPos = x.LeftPos,
                         TopPos = x.TopPos,
                         WidthPos = x.WidthPos,
                         HeightPos = x.HeightPos,
                         Color = x.Color,
                         BackColor = x.BackColor,
                         Data = x.Data,
                         Data2 = x.Data2,
                         Rotation = x.Rotation,
                         ScaleX = x.ScaleX,
                         ScaleY = x.ScaleY,
                         TransX = x.TransX,
                         TransY = x.TransY,
                         StrokeWidth = x.StrokeWidth,
                         Opacity = x.Opacity,
                         Flag = x.Flag,
                         FlagCode = x.FlagCode,
                         FlagDate = x.FlagDate,
                         FlagImage = x.FlagImage,
                         CreatorId = x.CreatorId,
                         AnnotateId = x.AnnotateId,

                     }).ToList();

                return annos;
            }
        }

        public int SaveAnnos(long documentId, long creatorId, string userId, IEnumerable<DtoDocumentAnnotate> annos)
        {
            using (var db = new DrdContext(_connString))
            {
                //
                // prepare data 
                //
                var cxold = db.DocumentAnnotates.Count(c => c.DocumentId == documentId);
                var cxnew = annos.Count();
                if (cxold < cxnew)
                {
                    var ep = annos.ElementAt(0); // get 1 data for sample
                    for (var x = cxold; x < cxnew; x++)
                    {
                        DocumentAnnotate da = new DocumentAnnotate();
                        da.DocumentId = documentId;
                        da.Page = ep.Page;
                        da.AnnotateTypeId = ep.AnnotateTypeId;
                        da.UserId = userId;
                        da.DateCreated = DateTime.Now;
                        db.DocumentAnnotates.Add(da);
                    }
                    db.SaveChanges();
                }
                else if (cxold > cxnew)
                {
                    var dremove = db.DocumentAnnotates.Where(c => c.DocumentId == documentId).Take(cxold - cxnew).ToList();
                    db.DocumentAnnotates.RemoveRange(dremove);
                    db.SaveChanges();
                }
                //
                // save data (update)
                //
                var dnew = db.DocumentAnnotates.Where(c => c.DocumentId == documentId).ToList();
                int v = 0;
                foreach (DocumentAnnotate da in dnew)
                {
                    var epos = annos.ElementAt(v);
                    da.DocumentId = documentId;
                    da.Page = epos.Page;
                    da.AnnotateTypeId = epos.AnnotateTypeId;
                    da.LeftPos = epos.LeftPos;
                    da.TopPos = epos.TopPos;
                    da.WidthPos = epos.WidthPos;
                    da.HeightPos = epos.HeightPos;
                    da.Color = epos.Color;
                    da.BackColor = epos.BackColor;
                    da.Data = epos.Data;
                    da.Data2 = epos.Data2;
                    da.Rotation = epos.Rotation;
                    da.ScaleX = epos.ScaleX;
                    da.ScaleY = epos.ScaleY;
                    da.TransX = epos.TransX;
                    da.TransY = epos.TransY;
                    da.StrokeWidth = epos.StrokeWidth;
                    da.Opacity = epos.Opacity;
                    da.Flag = epos.Flag;
                    da.FlagCode = epos.FlagCode;
                    da.FlagDate = epos.FlagDate;
                    da.FlagImage = epos.FlagImage;
                    da.CreatorId = (epos.CreatorId == null ? creatorId : epos.CreatorId);
                    da.AnnotateId = epos.AnnotateId;
                    da.UserId = userId;
                    da.DateCreated = DateTime.Now;
                    v++;
                }
                return db.SaveChanges();

            }
        }

        public int CheckingSignature(long memberId)
        {
            int ret = 1;
            using (var db = new DrdContext(_connString))
            {
                var mem = db.Members.FirstOrDefault(c => c.Id == memberId);
                if (mem.ImageSignature == null || mem.ImageInitials == null || mem.ImageKtp1 == null || mem.ImageKtp2 == null || string.IsNullOrEmpty(mem.KtpNo))
                    ret = -1;
            }
            return ret;
        }

        public int CheckingPrivateStamp(long memberId)
        {
            int ret = 1;
            using (var db = new DrdContext(_connString))
            {
                var mem = db.Members.FirstOrDefault(c => c.Id == memberId);
                if (mem.ImageStamp == null)
                    ret = -1;
            }
            return ret;
        }

        public int Signature(long documentId, long memberId, long rotationId)
        {
            using (var db = new DrdContext(_connString))
            {
                var datas = db.DocumentAnnotates.Where(c => c.DocumentId == documentId && ("SIGNATURE, INITIAL").Contains(c.AnnotateType.Code) && c.AnnotateId == memberId && (c.Flag & 1) != 1).ToList();
                if (datas == null)
                    return 0;
                var member = db.Members.FirstOrDefault(c => c.Id == memberId);
                var rotnod = db.Rotations.FirstOrDefault(c => c.Id == rotationId);
                var doc = db.Documents.FirstOrDefault(c => c.Id == documentId);
                int cx = 0;
                string numbers = "";
                foreach (DocumentAnnotate da in datas)
                {
                    var dt = DateTime.Now;
                    da.Flag = 1;
                    da.FlagDate = dt;
                    da.FlagCode = "DRD-" + dt.ToString("yyMMddHHmmssfff");
                    da.FlagImage = (da.AnnotateType.Code.Equals("SIGNATURE") ? member.ImageSignature : member.ImageInitials);
                    if (!numbers.Equals(""))
                        numbers += ", ";
                    numbers += da.FlagCode;
                    cx++;
                }
                if (cx > 0)
                {
                    db.SaveChanges();
                    DtoMember xmem = new DtoMember();
                    xmem.Id = member.Id;
                    xmem.Name = member.Name;
                    xmem.Email = member.Email;
                    sendEmailSignature(xmem, rotnod.Subject, doc.Title, numbers);
                }
                return cx;
            }
        }

        public void sendEmailSignature(DtoMember member, string rotName, string docName, string numbers)
        {
            ApplConfigService appsvr = new ApplConfigService();
            var topaz = appsvr.GetValue("APPL_NAME");
            var admName = appsvr.GetValue("EMAILUSERDISPLAY");
            EmailTools emailtools = new EmailTools();
            string body =
                "Dear " + member.Name + ",<br/><br/>" +
                "You have signed rotation <b>" + rotName + "</b> in document <b>" + docName + "</b>, the signature number generated: <b>" + numbers + "</b>.<br/>";

            body += "<br/><br/> " + admName + " Administrator<br/>";

            var dbx = new DrdContext(ConfigConstant.CONSTRING);
            var resultx = dbx.ApplConfigs.ToList();
            var emailfrom = resultx.Where(c => c.Code == "EMAILUSER").FirstOrDefault().Value;
            //var emailfromdisplay = resultx.Where(c => c.Code == "EMAILUSERDISPLAY").FirstOrDefault().Value;

            var task = emailtools.Send(emailfrom, admName + " Administrator", member.Email, admName + " User Signature", body, false, new string[] { });
        }

        public int SaveCxPrint(string docName, long memberId)
        {
            using (var db = new DrdContext(_connString))
            {
                var doc = db.Documents.FirstOrDefault(c => c.FileName.Equals(docName));
                var rot = db.RotationNodeDocs.FirstOrDefault(c => c.DocumentId == doc.Id);
                var docmem = db.DocumentMembers.FirstOrDefault(c => c.DocumentId == doc.Id && c.MemberId == memberId);
                if (docmem == null)
                    return -4;

                // calc from rotation is completed
                if (!rot.RotationNode.Rotation.Status.Equals("90"))
                    return -3;

                // expired
                if (rot.RotationNode.Rotation.DateStatus.Value.AddDays(doc.ExpiryDay) < DateTime.Now)
                    return -2;

                // out of max
                if (docmem.CxPrint + 1 > doc.MaxPrint)
                    return -1;

                docmem.CxPrint++;
                db.SaveChanges();

                return doc.MaxPrint - docmem.CxPrint + 1;
            }
        }

        public int SaveCxDownload(string docName, long memberId)
        {
            using (var db = new DrdContext(_connString))
            {
                var doc = db.Documents.FirstOrDefault(c => c.FileName.Equals(docName));
                var rot = db.RotationNodeDocs.FirstOrDefault(c => c.DocumentId == doc.Id);
                var docmem = db.DocumentMembers.FirstOrDefault(c => c.DocumentId == doc.Id && c.MemberId == memberId);
                if (docmem == null)
                    return -4;

                // calc from rotation is completed
                if (!rot.RotationNode.Rotation.Status.Equals("90"))
                    return -3;

                // expired
                if (rot.RotationNode.Rotation.DateStatus.Value.AddDays(doc.ExpiryDay) < DateTime.Now)
                    return -2;

                // out of max
                if (docmem.CxDownload + 1 > doc.MaxDownload)
                    return -1;

                docmem.CxDownload++;
                db.SaveChanges();

                return doc.MaxDownload - docmem.CxDownload + 1;
            }
        }

        public int Stamp(long documentId, long memberId, long rotationId)
        {
            using (var db = new DrdContext(_connString))
            {
                var datas = db.DocumentAnnotates.Where(c => c.DocumentId == documentId && c.AnnotateType.Code.Equals("PRIVATESTAMP") && c.AnnotateId == memberId && (c.Flag & 1) != 1).ToList();
                if (datas == null)
                    return 0;
                var member = db.Members.FirstOrDefault(c => c.Id == memberId);
                var rotnod = db.Rotations.FirstOrDefault(c => c.Id == rotationId);
                var doc = db.Documents.FirstOrDefault(c => c.Id == documentId);
                int cx = 0;
                string numbers = "";
                foreach (DocumentAnnotate da in datas)
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
                    DtoMember xmem = new DtoMember();
                    xmem.Id = member.Id;
                    xmem.Name = member.Name;
                    xmem.Email = member.Email;
                    sendEmailStamp(xmem, rotnod.Subject, doc.Title, numbers);
                }
                return cx;
            }
        }

        public void sendEmailStamp(DtoMember member, string rotName, string docName, string numbers)
        {
            ApplConfigService appsvr = new ApplConfigService();
            var topaz = appsvr.GetValue("APPL_NAME");
            var admName = appsvr.GetValue("EMAILUSERDISPLAY");
            EmailTools emailtools = new EmailTools();
            string body =
                "Dear " + member.Name + ",<br/><br/>" +
                "You have stamped rotation <b>" + rotName + "</b> in document <b>" + docName + "</b>, the stamp number generated: <b>" + numbers + "</b>.<br/>";

            body += "<br/><br/> " + admName + " Administrator<br/>";

            var dbx = new DrdContext(ConfigConstant.CONSTRING);
            var resultx = dbx.ApplConfigs.ToList();
            var emailfrom = resultx.Where(c => c.Code == "EMAILUSER").FirstOrDefault().Value;
            //var emailfromdisplay = resultx.Where(c => c.Code == "EMAILUSERDISPLAY").FirstOrDefault().Value;

            var task = emailtools.Send(emailfrom, admName + " Administrator", member.Email, admName + " User Stamping", body, false, new string[] { });
        }


        public int GetPermission(long memberId, long rotationNodeId, long documentId)
        {
            int ret = 0;
            using (var db = new DrdContext(_connString))
            {
                if (rotationNodeId == 0)
                    return 0;

                if (rotationNodeId < 0)
                {
                    var rid = Math.Abs(rotationNodeId);
                    var rnx = db.RotationNodeDocs.FirstOrDefault(c => c.DocumentId == documentId && c.RotationNode.MemberId == memberId && !c.RotationNode.Status.Equals("00") && c.RotationNode.RotationId == rid);
                    if (rnx == null)
                        return 0;

                    rotationNodeId = rnx.RotationNodeId;
                }
                var rn = db.RotationNodes.FirstOrDefault(c => c.Id == rotationNodeId);
                var rm = db.RotationMembers.FirstOrDefault(c => c.MemberId == memberId && c.RotationId == rn.RotationId && c.WorkflowNodeId == rn.WorkflowNodeId);
                if (rm != null)
                {
                    ret = rm.FlagPermission;

                    var docs = db.DocumentAnnotates.Where(c => c.DocumentId == documentId && c.AnnotateId == memberId && ("SIGNATURE,INITIAL,PRIVATESTAMP").Contains(c.AnnotateType.Code)).ToList();
                    foreach (DocumentAnnotate doc in docs)
                    {
                        if (("SIGNATURE,INITIAL").Contains(doc.AnnotateType.Code))
                            ret |= 1;
                        else if (doc.AnnotateType.Code.Equals("PRIVATESTAMP"))
                            ret |= 32;
                    }
                }
            }
            return ret;
        }

    }
}
