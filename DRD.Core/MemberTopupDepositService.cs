using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using DRD.Domain;
using System.Based.Core.Entity;
using System.Data.Entity.Infrastructure;
using System.Based.Core;

namespace DRD.Service
{
    public class MemberTopupDepositService
    {
        private readonly string _connString;

        public MemberTopupDepositService()
        {
            _connString = ConfigConstant.CONSTRING;
        }
        public MemberTopupDepositService(string connString)
        {
            _connString = connString;
        }

        public IEnumerable<DtoMemberTopupDeposit> GetByQuery(string query, int page, int pageSize, string order, string criteria)
        {
            int skip = pageSize * (page - 1);
            string ordering = "PaymentStatus, DateCreated desc";

            if (order != null)
                ordering = order;

            if (criteria == null)
                criteria = query;
            else
                criteria = query + " and " + criteria;

            using (var db = new DrdContext(_connString))
            {
                //var xxx = db.MemberTopupDeposits.ToList();
                var result =
                    (from c in db.MemberTopupDeposits
                     join s in db.PaymentStatus on c.PaymentStatus equals s.Code
                     //orderby c.PaymentStatus, c.TopupDate descending
                     select new DtoMemberTopupDeposit
                     {
                         Id = c.Id,
                         TopupNo = c.TopupNo,
                         TopupDate = c.TopupDate,
                         MemberId = c.MemberId,
                         PaymentStatus = c.PaymentStatus,
                         Amount = c.Amount,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                         PaymentStatusDescr = s.Descr,
                         MemberDescr = c.Member.Number + " | " + c.Member.Name,
                         Member = new DtoMember
                         {
                             Number = c.Member.Number,
                             Name = c.Member.Name,
                             Email = c.Member.Email,
                             Phone = c.Member.Phone,
                         },
                         MemberTopupPayments =
                             (from tp in c.MemberTopupPayments
                              orderby tp.PaymentStatus, tp.PaymentDate descending, tp.PaymentNo
                              select new DtoMemberTopupPayment
                              {
                                  Id = tp.Id,
                                  Amount = tp.Amount,
                                  MemberAccountId = tp.MemberAccountId,
                                  CompanyBankId = tp.CompanyBankId,
                                  PaymentNo = tp.PaymentNo,
                                  PaymentDate = tp.PaymentDate,
                                  PaymentStatus = tp.PaymentStatus,

                                  CompanyBank = new DtoCompanyBank
                                  {
                                      AccountName = tp.CompanyBank.AccountName,
                                      AccountNo = tp.CompanyBank.AccountNo,
                                      BankId = tp.CompanyBank.BankId,
                                      Branch = tp.CompanyBank.Branch,
                                      Id = tp.CompanyBank.Id,
                                      Bank = new DtoBank
                                      {
                                          Code = tp.CompanyBank.Bank.Code,
                                          Logo = tp.CompanyBank.Bank.Logo,
                                          Name = tp.CompanyBank.Bank.Name,
                                      },
                                      PaymentMethod = new DtoPaymentMethod
                                      {
                                          Code = tp.CompanyBank.PaymentMethod.Code,
                                          Name = tp.CompanyBank.PaymentMethod.Name,
                                          Logo = tp.CompanyBank.PaymentMethod.Logo,
                                      },
                                  },
                                  MemberAccount = (tp.MemberAccountId == null ? null : new DtoMemberAccount
                                  {
                                      Id = tp.MemberAccount.Id,
                                      AccountName = tp.MemberAccount.AccountName,
                                      AccountNo = tp.MemberAccount.AccountNo,
                                      Bank = new DtoBank
                                      {
                                          Code = tp.CompanyBank.Bank.Code,
                                          Logo = tp.CompanyBank.Bank.Logo,
                                          Name = tp.CompanyBank.Bank.Name,
                                      },
                                      //BankCode = tp.MemberAccount.Bank.Code,
                                      //BankName = tp.MemberAccount.Bank.Name,
                                      //BankLogo = tp.MemberAccount.Bank.Logo,
                                  }),
                              }).ToList(),
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                return result;
            }
        }

        public DtoMemberTopupDeposit GetById(long id)
        {
            var result = GetByQuery("Id=" + id, 1, 1, null, null);
            if (result != null)
            {
                DtoMemberTopupDeposit td = result.FirstOrDefault();
                //td.PaymentMethods = (new PaymentMethodService(ConfigConstant.CONSTRING)).GetAll();
                return td;
            }

            return null;
        }

        public DtoMemberTopupDeposit GetById(string id)
        {

            var result = GetByQuery("Id=" + long.Parse(XEncryptionHelper.Decrypt(id)), 1, 1, null, null);
            if (result != null)
            {
                DtoMemberTopupDeposit td = result.FirstOrDefault();
                td.PaymentMethods = (new PaymentMethodService(ConfigConstant.CONSTRING)).GetAll();
                td.KeyId = XEncryptionHelper.Encrypt(td.Id.ToString());
                return td;
            }

            return null;
        }

        public DtoMemberTopupDeposit GetById(string id, string cbId)
        {

            var result = GetByQuery("Id=" + long.Parse(XEncryptionHelper.Decrypt(id)), 1, 1, null, null);
            if (result != null)
            {
                DtoMemberTopupDeposit td = result.FirstOrDefault();
                td.PaymentMethods = (new PaymentMethodService(ConfigConstant.CONSTRING)).GetByCbId(cbId);
                td.KeyId = XEncryptionHelper.Encrypt(td.Id.ToString());
                return td;
            }

            return null;
        }

        public IEnumerable<DtoMemberTopupDeposit> GetById(long MemberId, int page, int pageSize, string order, string criteria)
        {
            var result = GetByQuery("MemberId=" + MemberId, page, pageSize, order, criteria);
            if (result != null)
            {
                foreach (DtoMemberTopupDeposit mtd in result)
                {
                    mtd.KeyId = XEncryptionHelper.Encrypt(mtd.Id.ToString());
                }
            }
            return result;
        }

        public DtoMemberTopupDeposit Save(DtoMemberTopupDeposit topup, string status = "00")
        {
            MemberTopupDeposit data = new MemberTopupDeposit();
            using (var db = new DrdContext(_connString))
            {
                ApplConfigService appl = new ApplConfigService();

                var minTopup = appl.GetValue("MIN_TOPUP");
                if (topup.Amount < decimal.Parse(minTopup))
                {
                    var ret = new DtoMemberTopupDeposit();
                    ret.Id = -1;
                    ret.Amount = decimal.Parse(minTopup);
                    return ret;
                }

                string appCode = "MEM_TU";

                for (int i = 0; i < ConfigConstant.LOOP_TRY_SAVE; i++)
                {
                    try
                    {
                        data.TopupNo = appl.GenerateNumber(appCode, "MemberTopupDeposit", "TopupNo", db.MemberTopupDeposits);
                        data.TopupDate = DateTime.Today;
                        data.MemberId = topup.MemberId;
                        data.Amount = topup.Amount;
                        data.PaymentStatus = status;
                        data.DateCreated = DateTime.Now;

                        db.MemberTopupDeposits.Add(data);
                        var result = db.SaveChanges();
                        topup.Id = data.Id;
                        topup.TopupNo = data.TopupNo;
                        topup.TopupDate = data.TopupDate;
                        topup.DateCreated = data.DateCreated;
                        topup.KeyId = XEncryptionHelper.Encrypt(data.Id.ToString());
                        break;
                    }
                    catch (DbUpdateException x)
                    {
                        if (i > ConfigConstant.LOOP_TRY_SAVE_THROW)
                            throw new Exception(x.Message);
                    }
                }

                return topup;
            }
        }

        public int UpdateStatus(long id, string status)
        {
            using (var db = new DrdContext(_connString))
            {
                var entity = db.MemberTopupDeposits.FirstOrDefault(c => c.Id == id);
                if (entity == null) return 0;

                entity.PaymentStatus = status;
                entity.DateUpdated = DateTime.Now;

                if (status.Equals("02"))
                {
                    MemberDepositTrx trx = new MemberDepositTrx();
                    trx.TrxNo = entity.TopupNo;
                    trx.TrxDate = entity.TopupDate;
                    trx.TrxType = "TU";
                    trx.TrxId = entity.Id;
                    trx.Descr = "Drd payment successful";
                    trx.MemberId = entity.MemberId;
                    trx.Amount = entity.Amount;
                    trx.DbCr = 0;
                    trx.DateCreated = DateTime.Now;
                    db.MemberDepositTrxes.Add(trx);
                }
                var result = db.SaveChanges();
                return result;
            }
        }

        public void GetCount(ref JsonDashboardAdmin dashb)
        {
            using (var db = new DrdContext(_connString))
            {
                dashb.MemberTopupPending = db.MemberTopupDeposits.Count(c => c.PaymentStatus.Equals("00"));
                dashb.MemberTopupConfirmation = db.MemberTopupDeposits.Count(c => c.PaymentStatus.Equals("01"));
                dashb.MemberTopupConfirmed = db.MemberTopupDeposits.Count(c => c.PaymentStatus.Equals("02"));
                dashb.MemberTopupNotConfirmed = db.MemberTopupDeposits.Count(c => c.PaymentStatus.Equals("99"));
            }
        }

        public decimal GetCount(int memberId, ref JsonDashboard dashb)
        {
            MemberDepositTrxService mtrx = new MemberDepositTrxService();
            dashb.DepositBalance = mtrx.GetDepositBalance(memberId);

            return dashb.DepositBalance;
        }

        //public decimal GetDepositBalance(long memberId)
        //{
        //    using (var db = new DrdContext(_connString))
        //    {
        //        // get topup
        //        var masuk = db.MemberTopupDeposits.Where(c => c.MemberId == memberId && c.PaymentStatus.Equals("02")).ToList().Sum(c => c.Amount);
        //        decimal keluar = 0;

        //        // get rotation price
        //        keluar = db.Rotations.Where(c => c.CreatorId == memberId && !("00").Contains(c.Status)).ToList().Sum(c => c.Price);

        //        return masuk - keluar;
        //    }

        //}

        public IEnumerable<DtoMemberTopupDeposit> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize)
        {
            return GetLiteAll(creatorId, topCriteria, page, pageSize, null, null);
        }
        public IEnumerable<DtoMemberTopupDeposit> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize, string order)
        {
            return GetLiteAll(creatorId, topCriteria, page, pageSize, order, null);
        }
        public IEnumerable<DtoMemberTopupDeposit> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize, string order, string criteria)
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
                    (from c in db.MemberTopupDeposits
                     join s in db.PaymentStatus on c.PaymentStatus equals s.Code
                     where c.MemberId == creatorId
                     //orderby c.PaymentStatus, c.TopupDate descending
                     select new DtoMemberTopupDeposit
                     {
                         Id = c.Id,
                         TopupNo = c.TopupNo,
                         TopupDate = c.TopupDate,
                         MemberId = c.MemberId,
                         PaymentStatus = c.PaymentStatus,
                         Amount = c.Amount,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                         PaymentStatusDescr = s.Descr,
                         MemberDescr = c.Member.Number + " | " + c.Member.Name,
                         Member = new DtoMember
                         {
                             Number = c.Member.Number,
                             Name = c.Member.Name,
                             Email = c.Member.Email,
                             Phone = c.Member.Phone,
                         },
                         MemberTopupPayments =
                             (from tp in c.MemberTopupPayments
                              orderby tp.PaymentStatus, tp.PaymentDate descending, tp.PaymentNo
                              select new DtoMemberTopupPayment
                              {
                                  Id = tp.Id,
                                  Amount = tp.Amount,
                                  MemberAccountId = tp.MemberAccountId,
                                  CompanyBankId = tp.CompanyBankId,
                                  PaymentNo = tp.PaymentNo,
                                  PaymentDate = tp.PaymentDate,
                                  PaymentStatus = tp.PaymentStatus,

                                  CompanyBank = new DtoCompanyBank
                                  {
                                      AccountName = tp.CompanyBank.AccountName,
                                      AccountNo = tp.CompanyBank.AccountNo,
                                      BankId = tp.CompanyBank.BankId,
                                      Branch = tp.CompanyBank.Branch,
                                      Id = tp.CompanyBank.Id,
                                      Bank = new DtoBank
                                      {
                                          Code = tp.CompanyBank.Bank.Code,
                                          Logo = tp.CompanyBank.Bank.Logo,
                                          Name = tp.CompanyBank.Bank.Name,
                                      },
                                      PaymentMethod = new DtoPaymentMethod
                                      {
                                          Code = tp.CompanyBank.PaymentMethod.Code,
                                          Name = tp.CompanyBank.PaymentMethod.Name,
                                          Logo = tp.CompanyBank.PaymentMethod.Logo,
                                      },
                                  },
                                  MemberAccount = (tp.MemberAccountId == null ? null : new DtoMemberAccount
                                  {
                                      Id = tp.MemberAccount.Id,
                                      AccountName = tp.MemberAccount.AccountName,
                                      AccountNo = tp.MemberAccount.AccountNo,
                                      Bank = new DtoBank
                                      {
                                          Code = tp.CompanyBank.Bank.Code,
                                          Logo = tp.CompanyBank.Bank.Logo,
                                          Name = tp.CompanyBank.Bank.Name,
                                      },
                                      //BankCode = tp.MemberAccount.Bank.Code,
                                      //BankName = tp.MemberAccount.Bank.Name,
                                      //BankLogo = tp.MemberAccount.Bank.Logo,
                                  }),
                              }).ToList(),
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                if (result != null)
                {
                    foreach (DtoMemberTopupDeposit mtd in result)
                    {
                        mtd.KeyId = XEncryptionHelper.Encrypt(mtd.Id.ToString());
                    }
                }
                return result;

            }
        }

        public long GetLiteAllCount(long creatorId, string topCriteria)
        {
            return GetLiteAllCount(creatorId, topCriteria, null);
        }
        public long GetLiteAllCount(long creatorId, string topCriteria, string criteria)
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
                    (from c in db.MemberTopupDeposits
                     where c.MemberId == creatorId && (topCriteria == null || tops.All(x => (c.Member.Name).Contains(x)))
                     select new DtoMemberTopupDeposit
                     {
                         Id = c.Id,
                     }).Where(criteria).Count();

                return result;

            }
        }




    }
}
