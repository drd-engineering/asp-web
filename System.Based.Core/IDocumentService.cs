using System.Collections.Generic;
using DRD.Domain;

namespace System.Based.Core
{
    public interface IDocumentService
    {
        int CheckingPrivateStamp(long memberId);
        int CheckingSignature(long memberId);
        ICollection<DtoDocumentAnnotate> FillAnnos(DtoDocument doc);
        IEnumerable<DtoDocumentSign> GetAnnotateDocs(long memberId, string topCriteria, int page, int pageSize);
        IEnumerable<DtoDocumentSign> GetAnnotateDocs(long memberId, string topCriteria, int page, int pageSize, string order);
        IEnumerable<DtoDocumentSign> GetAnnotateDocs(long memberId, string topCriteria, int page, int pageSize, string order, string criteria);
        DtoDocument GetById(long id);
        DtoDocumentLite GetByUniqFileName(string uniqFileName, bool isDocument);
        IEnumerable<DtoDocumentLite> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize);
        IEnumerable<DtoDocumentLite> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize, string order);
        IEnumerable<DtoDocumentLite> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize, string order, string criteria);
        long GetLiteAllCount(long memberId, string topCriteria);
        long GetLiteAllCount(long memberId, string topCriteria, string criteria);
        IEnumerable<DtoDocumentLite> GetLiteByCreator(long memberId, string topCriteria, int page, int pageSize);
        IEnumerable<DtoDocumentLite> GetLiteByCreator(long memberId, string topCriteria, int page, int pageSize, string order);
        IEnumerable<DtoDocumentLite> GetLiteByCreator(long memberId, string topCriteria, int page, int pageSize, string order, string criteria);
        long GetLiteByCreatorCount(long memberId, string topCriteria);
        long GetLiteByCreatorCount(long memberId, string topCriteria, string criteria);
        IEnumerable<DtoDocumentLite> GetLiteByTopCriteria(long companyId, string topCriteria, int page, int pageSize, string order, string criteria);
        int GetPermission(long memberId, long rotationNodeId, long documentId);
        IEnumerable<DtoDocumentSign> GetSignatureDocs(long memberId, string topCriteria, int page, int pageSize);
        IEnumerable<DtoDocumentSign> GetSignatureDocs(long memberId, string topCriteria, int page, int pageSize, string order);
        IEnumerable<DtoDocumentSign> GetSignatureDocs(long memberId, string topCriteria, int page, int pageSize, string order, string criteria);
        int Save(DtoDocument prod);
        int SaveAnnos(long documentId, long creatorId, string userId, IEnumerable<DtoDocumentAnnotate> annos);
        int SaveCxDownload(string docName, long memberId);
        int SaveCxPrint(string docName, long memberId);
        void sendEmailSignature(DtoMember member, string rotName, string docName, string numbers);
        void sendEmailStamp(DtoMember member, string rotName, string docName, string numbers);
        int Signature(long documentId, long memberId, long rotationId);
        int Stamp(long documentId, long memberId, long rotationId);
    }
}