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
    public class PaymentMethodService
    {
        private readonly string _connString;

        public PaymentMethodService()
        {
            _connString = ConfigConstant.CONSTRING;
        }
        public PaymentMethodService(string connString)
        {
            _connString = connString;
        }

        public IEnumerable<DtoPaymentMethod> GetAll()
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.PaymentMethods
                     orderby c.Name
                     where (c.UsingType & 4) != 4 && c.CompanyBanks.Count() > 0
                     select new DtoPaymentMethod
                     {
                         Id = c.Id,
                         Code = c.Code,
                         Name = c.Name,
                         Logo = c.Logo,
                         Descr = c.Descr,
                         UsingType = c.UsingType,
                         ConfirmType = c.ConfirmType,
                         UserId = c.UserId,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                         CompanyBanks =
                            (from x in c.CompanyBanks
                             where x.IsActive
                             select new DtoCompanyBank
                             {
                                 Id = x.Id,
                                 BankId = x.BankId,
                                 AccountName = x.AccountName,
                                 AccountNo = x.AccountNo,
                                 Branch = x.Branch,
                                 PaymentMethodId = x.PaymentMethodId,
                                 IsActive = x.IsActive,

                                 //Code = c.Code,
                                 //ConfirmType = c.ConfirmType,
                                 //UsingType = c.UsingType,
                                 Bank = new DtoBank
                                 {
                                     BankType = x.Bank.BankType,
                                     Logo = x.Bank.Logo,
                                     Name = x.Bank.Name,
                                 },
                             }).ToList(),
                     }).ToList();

                if (result != null)
                {
                    foreach (DtoPaymentMethod pm in result)
                    {
                        foreach (DtoCompanyBank cb in pm.CompanyBanks)
                        {
                            cb.KeyId = XEncryptionHelper.Encrypt(cb.Id.ToString());
                        }
                    }
                }

                return result;
            }
        }

        public DtoPaymentMethod GetById(int id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.PaymentMethods
                     where c.Id == id
                     select new DtoPaymentMethod
                     {
                         Id = c.Id,
                         Code = c.Code,
                         Name = c.Name,
                         Logo = c.Logo,
                         Descr = c.Descr,
                         UsingType = c.UsingType,
                         ConfirmType = c.ConfirmType,
                         UserId = c.UserId,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                         CompanyBanks =
                            (from x in c.CompanyBanks
                             select new DtoCompanyBank
                             {
                                 Id = x.Id,
                                 BankId = x.BankId,
                                 AccountName = x.AccountName,
                                 AccountNo = x.AccountNo,
                                 Branch = x.Branch,
                                 PaymentMethodId = x.PaymentMethodId,
                                 IsActive = x.IsActive,

                                 //Code = c.Code,
                                 //ConfirmType = c.ConfirmType,
                                 //UsingType = c.UsingType,
                                 Bank = new DtoBank
                                 {
                                     BankType = x.Bank.BankType,
                                     Logo = x.Bank.Logo,
                                     Name = x.Bank.Name,
                                 },

                             }).ToList(),
                     }).FirstOrDefault();

                if (result != null)
                {

                    foreach (DtoCompanyBank cb in result.CompanyBanks)
                    {
                        cb.KeyId = XEncryptionHelper.Encrypt(cb.Id.ToString());
                    }

                }
                return result;
            }
        }

        public IEnumerable<DtoPaymentMethod> GetByCbId(string cbId)
        {
            using (var db = new DrdContext(_connString))
            {
                long realCbId = long.Parse(XEncryptionHelper.Decrypt(cbId));
                var comapnyBank = db.CompanyBanks.FirstOrDefault(c => c.Id == realCbId);
                long realId = comapnyBank.PaymentMethodId;

                var result =
                    (from c in db.PaymentMethods
                     where c.Id == realId
                     select new DtoPaymentMethod
                     {
                         Id = c.Id,
                         Code = c.Code,
                         Name = c.Name,
                         Logo = c.Logo,
                         Descr = c.Descr,
                         UsingType = c.UsingType,
                         ConfirmType = c.ConfirmType,
                         UserId = c.UserId,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                         CompanyBanks =
                            (from x in c.CompanyBanks
                             where x.Id == realCbId
                             select new DtoCompanyBank
                             {
                                 Id = x.Id,
                                 BankId = x.BankId,
                                 AccountName = x.AccountName,
                                 AccountNo = x.AccountNo,
                                 Branch = x.Branch,
                                 PaymentMethodId = x.PaymentMethodId,

                                 //Code = c.Code,
                                 //ConfirmType = c.ConfirmType,
                                 //UsingType = c.UsingType,
                                 IsActive = x.IsActive,
                                 Bank = new DtoBank
                                 {
                                     BankType = x.Bank.BankType,
                                     Logo = x.Bank.Logo,
                                     Name = x.Bank.Name,
                                 },
                             }).ToList(),
                     }).ToList();

                if (result != null)
                {
                    foreach (DtoPaymentMethod pm in result)
                    {
                        foreach (DtoCompanyBank cb in pm.CompanyBanks)
                        {
                            cb.KeyId = XEncryptionHelper.Encrypt(cb.Id.ToString());
                        }
                    }
                }
                return result;
            }
        }

        public DtoPaymentMethod GetByCode(string code)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.PaymentMethods
                     where c.Code == code
                     select new DtoPaymentMethod
                     {
                         Id = c.Id,
                         Code = c.Code,
                         Name = c.Name,
                         Logo = c.Logo,
                         Descr = c.Descr,
                         UsingType = c.UsingType,
                         ConfirmType = c.ConfirmType,
                         UserId = c.UserId,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                         CompanyBanks =
                            (from x in c.CompanyBanks
                             select new DtoCompanyBank
                             {
                                 Id = x.Id,
                                 BankId = x.BankId,
                                 AccountName = x.AccountName,
                                 AccountNo = x.AccountNo,
                                 Branch = x.Branch,
                                 PaymentMethodId = x.PaymentMethodId,
                                 IsActive = x.IsActive,

                                 //Code = c.Code,
                                 //ConfirmType = c.ConfirmType,
                                 //UsingType = c.UsingType,
                                 Bank = new DtoBank
                                 {
                                     BankType = x.Bank.BankType,
                                     Logo = x.Bank.Logo,
                                     Name = x.Bank.Name,
                                 },
                             }).ToList(),
                     }).FirstOrDefault();

                if (result != null)
                {

                    foreach (DtoCompanyBank cb in result.CompanyBanks)
                    {
                        cb.KeyId = XEncryptionHelper.Encrypt(cb.Id.ToString());
                    }

                }
                return result;
            }
        }

        public int GetIdByCode(string code)
        {
            using (var db = new DrdContext(_connString))
            {
                string[] ar = code.Split('|');
                code = ar[0];

                var result =
                    (from c in db.PaymentMethods
                     where c.Code == code
                     select new DtoPaymentMethod
                     {
                         Id = c.Id,
                     }).FirstOrDefault();

                if (result == null)
                    return 0;

                return result.Id;
            }
        }

        public DtoPaymentMethod Save(DtoPaymentMethod paymethod)
        {
            PaymentMethod data = new PaymentMethod();
            using (var db = new DrdContext(_connString))
            {
                data.Code = paymethod.Code;
                data.Name = paymethod.Name;
                data.Logo = paymethod.Logo;
                data.Descr = paymethod.Descr;
                data.UsingType = paymethod.UsingType;
                data.ConfirmType = paymethod.ConfirmType;
                data.UserId = paymethod.UserId;
                data.DateCreated = DateTime.Now;

                db.PaymentMethods.Add(data);
                var result = db.SaveChanges();
                paymethod.Id = data.Id;
                paymethod.DateCreated = data.DateCreated;
                return paymethod;
            }
        }

        public int Update(DtoPaymentMethod paymethod)
        {
            using (var db = new DrdContext(_connString))
            {
                var entity = db.PaymentMethods.FirstOrDefault(c => c.Id == paymethod.Id);
                if (entity == null) return 0;

                entity.Code = paymethod.Code;
                entity.Name = paymethod.Name;
                entity.Logo = paymethod.Logo;
                entity.Descr = paymethod.Descr;
                entity.UsingType = paymethod.UsingType;
                entity.ConfirmType = paymethod.ConfirmType;
                entity.UserId = paymethod.UserId;
                entity.DateUpdated = DateTime.Now;

                var result = db.SaveChanges();
                return result;
            }
        }
    }
}
