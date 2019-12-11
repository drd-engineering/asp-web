using System.Collections.Generic;

using DRD.Models;
using DRD.Models.API.List;
using DRD.Models.Custom;

namespace DRD.Service
{
    public interface IDocumentService
    {
        Document Create(Document document);
        Document Update(Document document);
        //void DoRevision(Document document);


        int CheckingPrivateStamp(long memberId);
        int CheckingSignature(long memberId);
        ICollection<DocumentElement> FillAnnos(Document doc);
        IEnumerable<DocumentSign> GetAnnotateDocs(long memberId, string topCriteria, int page, int pageSize);
        IEnumerable<DocumentSign> GetAnnotateDocs(long memberId, string topCriteria, int page, int pageSize, string order);
        IEnumerable<DocumentSign> GetAnnotateDocs(long memberId, string topCriteria, int page, int pageSize, string order, string criteria);
        Document GetById(long id);
        DocumentItem GetByUniqFileName(string uniqFileName, bool isDocument);
        IEnumerable<DocumentItem> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize);
        IEnumerable<DocumentItem> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize, string order);
        IEnumerable<DocumentItem> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize, string order, string criteria);
        long GetLiteAllCount(long memberId, string topCriteria);
        long GetLiteAllCount(long memberId, string topCriteria, string criteria);
        IEnumerable<DocumentItem> GetLiteByCreator(long memberId, string topCriteria, int page, int pageSize);
        IEnumerable<DocumentItem> GetLiteByCreator(long memberId, string topCriteria, int page, int pageSize, string order);
        IEnumerable<DocumentItem> GetLiteByCreator(long memberId, string topCriteria, int page, int pageSize, string order, string criteria);
        long GetLiteByCreatorCount(long memberId, string topCriteria);
        long GetLiteByCreatorCount(long memberId, string topCriteria, string criteria);
        IEnumerable<DocumentItem> GetLiteByTopCriteria(long companyId, string topCriteria, int page, int pageSize, string order, string criteria);
        int GetPermission(long memberId, long rotationNodeId, long documentId);
        IEnumerable<DocumentSign> GetSignatureDocs(long memberId, string topCriteria, int page, int pageSize);
        IEnumerable<DocumentSign> GetSignatureDocs(long memberId, string topCriteria, int page, int pageSize, string order);
        IEnumerable<DocumentSign> GetSignatureDocs(long memberId, string topCriteria, int page, int pageSize, string order, string criteria);
        int Save(Document prod);
        int SaveAnnos(long documentId, long creatorId, string userEmail, IEnumerable<DocumentElement> annos);
        int SaveCxDownload(string docName, long memberId);
        int SaveCxPrint(string docName, long memberId);
        void sendEmailSignature(Member member, string rotName, string docName, string numbers);
        void sendEmailStamp(Member member, string rotName, string docName, string numbers);
        int Signature(long documentId, long memberId, long rotationId);
        int Stamp(long documentId, long memberId, long rotationId);
    }
}
