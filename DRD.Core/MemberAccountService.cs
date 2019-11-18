using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using DRD.Domain;
using System.Based.Core.Entity;
using System.Based.Core;

namespace DRD.Core
{
    public class MemberAccountService
    {
        private readonly string _connString;

        public MemberAccountService()
        {
            _connString = ConfigConstant.CONSTRING;
        }
        public MemberAccountService(string connString)
        {
            _connString = connString;
        }
        public DtoMemberAccount GetById(string keyId)
        {
            long id = long.Parse(XEncryptionHelper.Decrypt(keyId));
            return GetById(id);
        }
        public DtoMemberAccount GetById(long Id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.MemberAccounts
                     where c.Id == Id
                     select new DtoMemberAccount
                     {
                         Id = c.Id,
                         MemberId = c.MemberId,
                         Title = c.Title,
                         AccountNo = c.AccountNo,
                         AccountName = c.AccountName,
                         BankId = c.BankId,
                         Branch = c.Branch,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                         Bank = new DtoBank
                         {
                             Name = c.Bank.Name,
                             Logo = c.Bank.Logo,
                         }

                     }).FirstOrDefault();


                return result;
            }
        }

        public IEnumerable<DtoMemberAccount> GetByMemberId(long Id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.MemberAccounts
                     where c.MemberId == Id
                     select new DtoMemberAccount
                     {
                         Id = c.Id,
                         MemberId = c.MemberId,
                         Title = c.Title,
                         AccountNo = c.AccountNo,
                         AccountName = c.AccountName,
                         BankId = c.BankId,
                         Branch = c.Branch,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,

                         Bank = new DtoBank
                         {
                             Name = c.Bank.Name,
                             Logo = c.Bank.Logo,
                         }
                     }).ToList();
                return result;
            }
        }

        public DtoMemberAccount Save(DtoMemberAccount memberacc)
        {
            MemberAccount data = new MemberAccount();
            using (var db = new DrdContext(_connString))
            {
                if (memberacc.Id != 0)
                    data = db.MemberAccounts.FirstOrDefault(c => c.Id == memberacc.Id);

                data.MemberId = memberacc.MemberId;
                data.Title = memberacc.Title;
                data.AccountNo = memberacc.AccountNo;
                data.AccountName = memberacc.AccountName;
                data.BankId = memberacc.BankId;
                data.Branch = memberacc.Branch;
                if (memberacc.Id == 0)
                {
                    data.DateCreated = DateTime.Now;
                    db.MemberAccounts.Add(data);
                }else
                    data.DateUpdated = DateTime.Now;

                var result = db.SaveChanges();
                memberacc.Id = data.Id;
                memberacc.DateCreated = data.DateCreated;
                return memberacc;
            }
        }

        //public int Update(DtoMemberAccount memberacc)
        //{
        //    using (var db = new DrdContext(_connString))
        //    {
        //        var entity = db.MemberAccounts.FirstOrDefault(c => c.Id == memberacc.Id);
        //        if (entity == null) return 0;

        //        entity.MemberId = memberacc.MemberId;
        //        entity.Title = memberacc.Title;
        //        entity.AccountNo = memberacc.AccountNo;
        //        entity.AccountName = memberacc.AccountName;
        //        entity.BankId = memberacc.BankId;
        //        entity.Branch = memberacc.Branch;
        //        entity.DateUpdated = DateTime.Now;

        //        var result = db.SaveChanges();
        //        return result;
        //    }
        //}

        public IEnumerable<DtoMemberAccount> GetLiteAll(long memberId, string topCriteria, int page, int pageSize, string order, string criteria)
        {
            int skip = pageSize * (page - 1);
            string ordering = "Title";

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
                    (from c in db.MemberAccounts
                     where c.MemberId == memberId && (topCriteria == null || tops.All(x => (c.Bank.Name).Contains(x)))
                     select new DtoMemberAccount
                     {
                         Id = c.Id,
                         Title = c.Title,
                         AccountNo = c.AccountNo,
                         AccountName = c.AccountName,
                         Branch = c.Branch,
                         BankId = c.BankId,
                         Bank = new DtoBank
                         {
                             Code = c.Bank.Code,
                             Name = c.Bank.Name,
                             Logo = c.Bank.Logo,

                         },
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                if (result != null)
                {
                    foreach (DtoMemberAccount dl in result)
                    {
                        dl.KeyId = XEncryptionHelper.Encrypt(dl.Id.ToString());
                    }
                }

                return result;

            }
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
                    (from c in db.MemberAccounts
                     where c.MemberId == memberId && (topCriteria == null || tops.All(x => (c.Bank.Name).Contains(x)))
                     select new DtoMemberAccount
                     {
                         Id = c.Id,
                     }).Where(criteria).Count();

                return result;

            }
        }

        public JsonSubscriptionRegistryResult SaveRegistration(JsonSubscriptionRegistry registry)
        {
            ApplConfigService.GenerateUniqueKeyLong();

            var regResult = new JsonSubscriptionRegistryResult();

            string pwd = DateTime.Now.ToString("ffff");

            var db = new DrdContext(_connString);
            var result = db.Members.Where(c => c.Email.Equals(registry.MemberEmail)).ToList();
            if (result.Count != 0)
            {
                regResult.UserNumber = "DBLEMAIL";
                return regResult;
            }

            var result2 = db.Companies.Where(c => c.Email.Equals(registry.CompanyEmail)).ToList();
            if (result2.Count != 0)
            {
                regResult.SubscriptionId = "DBLEMAIL";
                return regResult;
            }

            ApplConfigService appsvr = new ApplConfigService();
            var topaz = appsvr.GetValue("APPL_NAME");
            var admName = appsvr.GetValue("EMAILUSERDISPLAY");
            var groupuser = appsvr.GetValue("DEF_GROUP_USER");

            CompanyService csvr = new CompanyService();
            Company comp = new Company();
            comp.Name = registry.CompanyName;
            comp.Contact = registry.MemberName;
            comp.Phone = registry.CompanyPhone;
            comp.Email = registry.CompanyEmail;
            comp.Address = registry.CompanyAddress;
            comp.PointLocation = registry.CompanyPointLocation;
            comp.Latitude = registry.Latitude;
            comp.Longitude = registry.Longitude;
            comp.CompanyType = 0;
            comp.Image1 = "default_panel.jpg";
            comp.Image2 = "default_logo.png";
            comp.BackColorBar = "#42a5f5";
            comp.BackColorPage = "#eceff1";
            long compId = csvr.Save(comp);

            MemberService svr = new MemberService();
            Member mem = new Member();
            mem.Email = registry.MemberEmail;

            // INI APAAN
            mem.MemberTitleId = registry.MemberTitleId;

            mem.Name = registry.MemberName.Trim();
            mem.Phone = registry.MemberPhone;
            mem.Password = XEncryptionHelper.Encrypt(pwd);
            mem.MemberType = 1;
            mem.CompanyId = compId;
            mem.UserGroup = groupuser;
            long memId = svr.Save(mem);

            MemberPlanService plan = new MemberPlanService();
            plan.Save(memId, registry.SubscriptTypeId);

            JsonMemberRegister reg = new JsonMemberRegister();
            reg.Name = mem.Name;
            reg.Number = mem.Number;
            reg.Email = mem.Email;
            reg.Password = pwd;
            svr.sendEmailRegistration(reg);

            regResult.SubscriptionId = comp.Code;
            regResult.UserNumber = mem.Number;
            regResult.Email = registry.MemberEmail;
            return regResult;
        }

    }
}
