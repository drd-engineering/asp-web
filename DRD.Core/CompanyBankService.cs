using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DRD.Domain;
using System.Based.Core.Entity;
using System.Based.Core;


namespace DRD.Service
{
    public class CompanyBankService
    {
        private readonly string _connString;

        public CompanyBankService()
        {
            _connString = ConfigConstant.CONSTRING;
        }
        public CompanyBankService(string connString)
        {
            _connString = connString;
        }

        public IEnumerable<DtoCompanyBank> GetAll()
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.CompanyBanks
                     orderby c.Bank.Name
                     select new DtoCompanyBank
                     {
                         Id = c.Id,
                         BankId = c.BankId,
                         AccountName = c.AccountName,
                         AccountNo = c.AccountNo,
                         Branch = c.Branch,
                         PaymentMethodId = c.PaymentMethodId,
                         IsActive = c.IsActive,
                         UserId = c.UserId,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                         Bank = new DtoBank
                         {
                             BankType = c.Bank.BankType,
                             Code = c.Bank.Code,
                             Logo = c.Bank.Logo,
                             Name = c.Bank.Name,
                         },
                         PaymentMethod = new DtoPaymentMethod
                         {
                             Code = c.PaymentMethod.Code,
                             Name = c.PaymentMethod.Name,
                             Logo = c.PaymentMethod.Logo,
                             ConfirmType = c.PaymentMethod.ConfirmType,
                             UsingType = c.PaymentMethod.UsingType,
                         },
                     }).ToList();

                return result;
            }
        }

        public DtoCompanyBank GetById(int id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.CompanyBanks
                     where c.Id == id
                     select new DtoCompanyBank
                     {
                         Id = c.Id,
                         BankId = c.BankId,
                         AccountName = c.AccountName,
                         AccountNo = c.AccountNo,
                         Branch = c.Branch,
                         PaymentMethodId = c.PaymentMethodId,
                         IsActive = c.IsActive,
                         UserId = c.UserId,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,

                         Bank = new DtoBank
                         {
                             BankType = c.Bank.BankType,
                             Code = c.Bank.Code,
                             Logo = c.Bank.Logo,
                             Name = c.Bank.Name,
                         },
                         PaymentMethod = new DtoPaymentMethod
                         {
                             Code = c.PaymentMethod.Code,
                             Name = c.PaymentMethod.Name,
                             Logo = c.PaymentMethod.Logo,
                             ConfirmType = c.PaymentMethod.ConfirmType,
                             UsingType = c.PaymentMethod.UsingType,
                         },
                     }).FirstOrDefault();

                return result;
            }
        }

        public DtoCompanyBank GetById(string id)
        {
            long realId = long.Parse(XEncryptionHelper.Decrypt(id));
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.CompanyBanks
                     where c.Id == realId
                     select new DtoCompanyBank
                     {
                         Id = c.Id,
                         BankId = c.BankId,
                         AccountName = c.AccountName,
                         AccountNo = c.AccountNo,
                         Branch = c.Branch,
                         PaymentMethodId = c.PaymentMethodId,
                         IsActive = c.IsActive,
                         UserId = c.UserId,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,

                         Bank = new DtoBank
                         {
                             BankType = c.Bank.BankType,
                             Code = c.Bank.Code,
                             Logo = c.Bank.Logo,
                             Name = c.Bank.Name,
                         },
                         PaymentMethod = new DtoPaymentMethod
                         {
                             Code = c.PaymentMethod.Code,
                             Name = c.PaymentMethod.Name,
                             Logo = c.PaymentMethod.Logo,
                             ConfirmType = c.PaymentMethod.ConfirmType,
                             UsingType = c.PaymentMethod.UsingType,
                         },
                     }).FirstOrDefault();

                return result;
            }
        }

        public DtoCompanyBank Save(DtoCompanyBank compBank)
        {
            CompanyBank data = new CompanyBank();
            using (var db = new DrdContext(_connString))
            {
                data.AccountName = compBank.AccountName;
                data.AccountNo = compBank.AccountNo;
                data.BankId = compBank.BankId;
                data.Branch = compBank.Branch;
                data.PaymentMethodId = compBank.PaymentMethodId;
                data.IsActive = true;
                data.UserId = compBank.UserId;
                data.DateCreated = DateTime.Now;

                db.CompanyBanks.Add(data);
                var result = db.SaveChanges();
                compBank.Id = data.Id;
                compBank.DateCreated = data.DateCreated;
                return compBank;
            }
        }

        public int Update(DtoCompanyBank compBank)
        {
            using (var db = new DrdContext(_connString))
            {
                var entity = db.CompanyBanks.FirstOrDefault(c => c.Id == compBank.Id);
                if (entity == null) return 0;

                entity.AccountName = compBank.AccountName;
                entity.AccountNo = compBank.AccountNo;
                entity.BankId = compBank.BankId;
                entity.Branch = compBank.Branch;
                entity.PaymentMethodId = compBank.PaymentMethodId;
                entity.IsActive = compBank.IsActive;
                entity.UserId = compBank.UserId;
                entity.DateUpdated = DateTime.Now;

                var result = db.SaveChanges();
                return result;
            }
        }
    }
}
