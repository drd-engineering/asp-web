using System.Collections.Generic;
using DRD.Domain;

namespace System.Based.Core
{
    public interface IMemberDepositTrxService
    {
        DtoMemberDepositTrx GetById(long id);
        IEnumerable<DtoMemberDepositTrx> GetById(long MemberId, int page, int pageSize, string order, string criteria);
        IEnumerable<DtoMemberDepositTrx> GetByQuery(string query, int page, int pageSize, string order, string criteria);
        long GetByQueryCount(long memberId);
        decimal GetDepositBalance(long memberId);
        int Save(DtoMemberDepositTrx trx);
    }
}