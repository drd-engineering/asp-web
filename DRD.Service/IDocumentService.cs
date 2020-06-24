using DRD.Models;
using DRD.Models.View;
using System.Collections.Generic;

namespace DRD.Service
{
    public interface IDocumentService
    {
        DocumentInboxData Create(DocumentInboxData document, long companyId, long rotationId);
        DocumentInboxData Update(DocumentInboxData document, long companyId, long rotationId);
        int DocumentUpdatedByRotation(long documentId);
        int DocumentRemovedofRevisedFromRotation(long documentId);
        //void DoRevision(Document document);
        int CheckingPrivateStamp(long memberId);
        int CheckingSignature(long memberId);
        ICollection<DocumentElement> FillAnnos(Document doc);
        IEnumerable<DocumentSign> GetAnnotateDocs(long memberId, string topCriteria, int page, int pageSize);
        IEnumerable<DocumentSign> GetAnnotateDocs(long memberId, string topCriteria, int page, int pageSize, string order);
        IEnumerable<DocumentSign> GetAnnotateDocs(long memberId, string topCriteria, int page, int pageSize, string order, string criteria);
        Document GetById(long id);
        DocumentItem GetByUniqFileName(string uniqFileName, bool isDocument, bool isNew);
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
        DocumentInboxData Save(DocumentInboxData prod, long companyId, long rotationId);
        ICollection<DocumentElementInboxData> SaveAnnos(long documentId, long creatorId, string userEmail, IEnumerable<DocumentElementInboxData> annos);
        ICollection<DocumentUserInboxData> CreateDocumentUser(long documentId);
        int RequestDownloadDocument(string docName, long userId);
        int RequestPrintDocument(string docName, long userId);
        void SendEmailSignature(Member member, string rotName, string docName, string numbers);
        void SendEmailStamp(Member member, string rotName, string docName, string numbers);
        int Signature(long documentId, long memberId, long rotationId);
        int Stamp(long documentId, long memberId, long rotationId);
    }
}
