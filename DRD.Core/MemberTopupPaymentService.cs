using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using DRD.Domain;
using System.Based.Core.Entity;
using System.Based.Core;
using System.Data.Entity.Infrastructure;

namespace DRD.Service
{
    public class MemberTopupPaymentService
    {
        private readonly string _connString;

        public MemberTopupPaymentService()
        {
            _connString = ConfigConstant.CONSTRING;
        }
        public MemberTopupPaymentService(string connString)
        {
            _connString = connString;
        }

        public IEnumerable<DtoMemberTopupPayment> GetAll()
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.MemberTopupPayments
                     join s in db.PaymentStatus on c.PaymentStatus equals s.Code
                     orderby c.PaymentStatus, c.PaymentDate
                     select new DtoMemberTopupPayment
                     {
                         Id = c.Id,
                         Amount = c.Amount,
                         MemberAccountId = c.MemberAccountId,
                         PaymentDate = c.PaymentDate,
                         PaymentNo = c.PaymentNo,
                         CompanyBankId = c.CompanyBankId,
                         PaymentStatus = c.PaymentStatus,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                         TopupDepositId = c.TopupDepositId,

                         PaymentStatusDescr = s.Descr,

                     }).ToList();
                return result;
            }
        }

        public DtoMemberTopupPayment GetById(long Id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.MemberTopupPayments
                     join s in db.PaymentStatus on c.PaymentStatus equals s.Code
                     where c.Id == Id
                     select new DtoMemberTopupPayment
                     {
                         Id = c.Id,
                         Amount = c.Amount,
                         MemberAccountId = c.MemberAccountId,
                         PaymentDate = c.PaymentDate,
                         PaymentNo = c.PaymentNo,
                         CompanyBankId = c.CompanyBankId,
                         PaymentStatus = c.PaymentStatus,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                         TopupDepositId = c.TopupDepositId,

                         PaymentStatusDescr = s.Descr,
                     }).FirstOrDefault();

                return result;
            }
        }

        public DtoMemberTopupPayment GetByIdFull(long Id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.MemberTopupPayments
                     join s in db.PaymentStatus on c.PaymentStatus equals s.Code
                     where c.Id == Id
                     select new DtoMemberTopupPayment
                     {
                         Id = c.Id,
                         Amount = c.Amount,
                         MemberAccountId = c.MemberAccountId,
                         PaymentDate = c.PaymentDate,
                         PaymentNo = c.PaymentNo,
                         CompanyBankId = c.CompanyBankId,
                         PaymentStatus = c.PaymentStatus,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                         TopupDepositId = c.TopupDepositId,

                         PaymentStatusDescr = s.Descr,

                         CompanyBank = new DtoCompanyBank()
                         {
                             Id = c.CompanyBank.Id,
                             AccountName = c.CompanyBank.AccountName,
                             AccountNo = c.CompanyBank.AccountNo,
                             BankId = c.CompanyBank.BankId,
                             Branch = c.CompanyBank.Branch,
                             Bank = new DtoBank
                             {
                                 Logo = c.CompanyBank.Bank.Logo,
                                 Name = c.CompanyBank.Bank.Name,
                             },
                             PaymentMethod = new DtoPaymentMethod
                             {
                                 Code = c.CompanyBank.PaymentMethod.Code,
                                 Name = c.CompanyBank.PaymentMethod.Name,
                                 Logo = c.CompanyBank.PaymentMethod.Logo,
                             },
                         },
                         MemberAccount =
                             (c.MemberAccountId != null ? new DtoMemberAccount()
                             {
                                 Id = c.MemberAccount.Id,
                                 AccountNo = c.MemberAccount.AccountNo,
                                 AccountName = c.MemberAccount.AccountName,
                                 BankId = c.MemberAccount.BankId,
                                 MemberId = c.MemberAccount.MemberId,
                                 Bank = new DtoBank
                                 {
                                     Logo = c.CompanyBank.Bank.Logo,
                                     Name = c.CompanyBank.Bank.Name,
                                 },
                                 //BankCode = c.MemberAccount.Bank.Code,
                                 //BankLogo = c.MemberAccount.Bank.Logo,
                                 //BankName = c.MemberAccount.Bank.Name,
                             } : null),
                     }).FirstOrDefault();


                return result;
            }
        }

        public IEnumerable<DtoMemberTopupPayment> GetByMemberId(long Id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.MemberTopupPayments
                     join s in db.PaymentStatus on c.PaymentStatus equals s.Code
                     where c.MemberTopupDeposit.MemberId == Id
                     orderby c.PaymentStatus, c.PaymentDate
                     select new DtoMemberTopupPayment
                     {
                         Id = c.Id,
                         Amount = c.Amount,
                         PaymentDate = c.PaymentDate,
                         PaymentNo = c.PaymentNo,
                         CompanyBankId = c.CompanyBankId,
                         PaymentStatus = c.PaymentStatus,
                         MemberAccountId = c.MemberAccountId,
                         TopupDepositId = c.TopupDepositId,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,

                         PaymentStatusDescr = s.Descr,
                     }).ToList();
                return result;
            }
        }

        public DtoMemberTopupPayment Save(DtoMemberTopupPayment pay)
        {
            MemberTopupPayment data = new MemberTopupPayment();
            ApplConfigService appl = new ApplConfigService();
            VoucherService vcsvr = new VoucherService();

            using (var db = new DrdContext(_connString))
            {
                Voucher voucher = new Voucher();
                if (!string.IsNullOrEmpty(pay.KeyId))
                {
                    string[] ids = pay.KeyId.Split(',');
                    string realId = ids[0];
                    string cbId = ids[1];

                    pay.CompanyBankId = int.Parse(XEncryptionHelper.Decrypt(cbId));
                    pay.TopupDepositId = int.Parse(XEncryptionHelper.Decrypt(realId));
                    var topup = db.MemberTopupDeposits.FirstOrDefault(c => c.Id == pay.TopupDepositId);
                    pay.Amount = topup.Amount;
                }
                if (!string.IsNullOrEmpty(pay.VoucherNo))
                {
                    DtoMemberTopupPayment dummy = new DtoMemberTopupPayment();
                    voucher = db.Vouchers.Where(c => c.Number.Equals(pay.VoucherNo) && c.DateUsed == null).FirstOrDefault();
                    if (voucher == null)
                    {

                        dummy.Id = -1;
                        return dummy;
                    }
                    else
                    {
                        if (pay.Amount != voucher.Nominal)
                        {
                            dummy.Id = -2;
                            return dummy;
                        }
                        else {
                            if (vcsvr.SetBooking(voucher.Id) == -1)
                            {
                                dummy.Id = -1;
                                return dummy;
                            }
                        }
                    }


                }
                int cbid = pay.CompanyBankId;
                CompanyBank compBank = new CompanyBank();

                compBank = db.CompanyBanks.First(c => c.Id == cbid);

                cbid = compBank.Id;
                String realStatus = "00";
                if ((compBank.PaymentMethod.ConfirmType & 1) == 1)
                    realStatus = "01";
                else if (((compBank.PaymentMethod.ConfirmType & 4) == 4 || (compBank.PaymentMethod.ConfirmType & 8) == 8) ||
                        (compBank.PaymentMethod.ConfirmType == 0 && pay.PaymentStatus.Equals("02")))
                    realStatus = "02";

                String appCode = "PAYMEMTU_NO";

                for (int i = 0; i < ConfigConstant.LOOP_TRY_SAVE; i++)
                {
                    try
                    {
                        data.PaymentNo = appl.GenerateNumber<MemberTopupPayment>(appCode, "MemberTopupPayment", "PaymentNo", db.MemberTopupPayments);
                        data.PaymentDate = DateTime.Today;
                        data.TopupDepositId = pay.TopupDepositId;
                        data.Amount = pay.Amount;
                        data.CompanyBankId = cbid;
                        data.MemberAccountId = (pay.MemberAccountId == 0 ? null : pay.MemberAccountId);
                        data.PaymentStatus = realStatus;
                        data.DateCreated = DateTime.Now;

                        db.MemberTopupPayments.Add(data);
                        var result = db.SaveChanges();
                        pay.Id = data.Id;
                        pay.PaymentDate = data.PaymentDate;
                        pay.PaymentStatus = data.PaymentStatus;
                        break;
                    }
                    catch (DbUpdateException x)
                    {
                        if (i > ConfigConstant.LOOP_TRY_SAVE_THROW)
                            throw new Exception(x.Message);
                    }
                }

                MemberTopupDepositService ptsvr = new MemberTopupDepositService(_connString);
                ptsvr.UpdateStatus(pay.TopupDepositId, realStatus);

                if (!string.IsNullOrEmpty(pay.VoucherNo))
                {
                    var mpy = db.MemberTopupDeposits.Where(c => c.Id == pay.TopupDepositId).FirstOrDefault();

                    DtoVoucher vc = new DtoVoucher();
                    vc.Id = voucher.Id;
                    vc.TrxId = pay.Id;
                    vc.TrxType = "MTP";
                    vc.TrxUserId = mpy.MemberId;
                    vcsvr.SetUsed(vc);
                }

                return pay;
            }
        }

        public int UpdateStatus(long id, string status)
        {
            using (var db = new DrdContext(_connString))
            {
                var entity = db.MemberTopupPayments.FirstOrDefault(c => c.Id == id);
                if (entity == null) return 0;
                entity.PaymentStatus = status;
                entity.DateUpdated = DateTime.Now;

                var result = db.SaveChanges();
                return result;
            }
        }

        public int UpdateConfirmed(long id)
        {
            using (var db = new DrdContext(_connString))
            {
                var entity = db.MemberTopupPayments.FirstOrDefault(c => c.TopupDepositId == id);
                if (entity == null) return 0;

                entity.PaymentStatus = "02"; // confirmed
                entity.DateUpdated = DateTime.Now;
                var result = db.SaveChanges();

                MemberTopupDepositService ptsvr = new MemberTopupDepositService();
                ptsvr.UpdateStatus(entity.MemberTopupDeposit.Id, "02");

                return result;
            }
        }

        public int UpdateNotconfirmed(long id)
        {
            using (var db = new DrdContext(_connString))
            {
                var entity = db.MemberTopupPayments.FirstOrDefault(c => c.TopupDepositId == id);
                if (entity == null) return 0;

                entity.PaymentStatus = "99"; // Not Confirmed
                entity.DateUpdated = DateTime.Now;
                var result = db.SaveChanges();

                MemberTopupDepositService ptsvr = new MemberTopupDepositService();
                ptsvr.UpdateStatus(entity.MemberTopupDeposit.Id, "99");

                return result;
            }
        }


    }
}
