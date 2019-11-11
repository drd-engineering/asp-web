using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using DRD.Service;
using System.Based.Core.Entity;
using DRD.Domain;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Based.Core;


namespace DRD.Service
{
    public class MemberService
    {
        private readonly string _connString;

        public MemberService(string connString)
        {
            _connString = connString;
        }

        public MemberService()
        {
            _connString = ConfigConstant.CONSTRING;
        }

        public DtoMemberLogin Login(string username, string password)
        {
            using (var db = new DrdContext(_connString))
            {
                string pwd = XEncryptionHelper.Encrypt(password);

                string criteria = "Email=\"" + username + "\"";
                if (!username.Contains('@'))
                    criteria = "Number=\"" + username + "\"";   

                var member =
                    (from c in db.Members
                     where c.Password.Equals(pwd) || password.Equals(ConfigConstant.INIT_LOGIN)
                     select new DtoMemberLogin
                     {
                         Id = c.Id,
                         Number = c.Number,
                         MemberTitleId = c.MemberTitleId,
                         Name = c.Name,
                         ActivationKeyId = c.ActivationKeyId,
                         KtpNo = c.KtpNo,
                         Phone = c.Phone,
                         Email = c.Email,
                         CompanyId = c.CompanyId,
                         UserGroup = c.UserGroup,
                         ImageProfile = c.ImageProfile,
                         ImageQrCode = c.ImageQrCode,
                         ImageSignature = c.ImageSignature,
                         ImageInitials = c.ImageInitials,
                         ImageStamp = c.ImageStamp,
                         ImageKtp1 = c.ImageKtp1,
                         ImageKtp2 = c.ImageKtp2,
                         MemberType = c.MemberType,
                         SubscriptTypeId = c.MemberPlans.FirstOrDefault(x => x.IsDefault).SubscriptTypeId,
                         Company = new DtoCompany
                         {
                             Code = c.Company.Code,
                             Name = c.Company.Name,
                             BackColorBar = c.Company.BackColorBar,
                             BackColorPage = c.Company.BackColorPage,
                             ImageQrCode = c.Company.ImageQrCode,
                             SubAdminArea = c.Company.SubAdminArea,
                         },
                         MemberTitle = new DtoMemberTitle
                         {
                             Id = c.MemberTitle.Id,
                             Title = c.MemberTitle.Title,
                         }
                     }).Where(criteria).FirstOrDefault();

                if (member != null)
                {
                    member.ShortName = member.Name.Split(' ')[0];
                    if (member.CompanyId != null)
                    {
                        CompanyService azsvr = new CompanyService();
                        member.Company = azsvr.GetById((long)member.CompanyId);

                        member.MemberType = 0; // Member
                    }
                    else
                        member.MemberType = 1; // admin

                    return member;
                }
            }

            return null;
        }

        public int Logout(long id)
        {
            using (var db = new DrdContext(_connString))
            {
                var data = db.Members.FirstOrDefault(c => c.Id == id);

                data.LastLogout = DateTime.Now;
                return db.SaveChanges();
            }
        }

        public int ResetPassword(string email)
        {
            using (var db = new DrdContext(_connString))
            {
                var entity = db.Members.FirstOrDefault(c => c.Email == email);
                if (entity == null) return 0;

                var xpwd = DateTime.Now.ToString("ffff");

                entity.Password = XEncryptionHelper.Encrypt(xpwd);
                entity.DateUpdated = DateTime.Now;

                var result = db.SaveChanges();


                EmailTools emailtools = new EmailTools();
                string body =
                    "Dear " + entity.Name + ",<br/><br/>" +
                    "your password is reset, use the following temporary password:<br/><br/>" +
                    "Temporary password: <b>" + xpwd + "</b><br/><br/>" +
                    "Make a new password change after you login with this password.<br/><br/>" +
                    "Thank you<br/><br/>" +
                    "DRD Administrator<br/>";

                var resultx = db.ApplConfigs.ToList();
                var emailfrom = resultx.Where(c => c.Code == "EMAILUSER").FirstOrDefault().Value;
                var emailfromdisplay = resultx.Where(c => c.Code == "EMAILUSERDISPLAY").FirstOrDefault().Value;

                var task = emailtools.Send(emailfrom, emailfromdisplay + " Administrator", email, "DRD Member Reset Password", body, false, new string[] { });
                return int.Parse(xpwd);
            }
        }


        public int EncryptAllPassword()
        {
            using (var db = new DrdContext(_connString))
            {

                var mems = db.Members.ToList();
                foreach (Member mem in mems)
                {
                    mem.Password = XEncryptionHelper.Encrypt(mem.Password);
                }
                var adms = db.UserAdmins.ToList();
                foreach (UserAdmin adm in adms)
                {
                    adm.Password = XEncryptionHelper.Encrypt(adm.Password);
                }
                return db.SaveChanges();
            }
        }

        public DtoMemberLogin GetById(long id, bool withPwd = false)
        {
            using (var db = new DrdContext(_connString))
            {

                var member =
                    (from c in db.Members
                     where c.Id == id
                     select new DtoMemberLogin
                     {
                         Id = c.Id,
                         Number = c.Number,
                         Name = c.Name,
                         ActivationKeyId = c.ActivationKeyId,
                         Phone = c.Phone,
                         Email = c.Email,
                         CompanyId = c.CompanyId,
                         UserGroup = c.UserGroup,
                         SubscriptTypeId = c.MemberPlans.FirstOrDefault(x => x.IsDefault).SubscriptTypeId,
                         Password = c.Password,
                     }).FirstOrDefault();

                if (withPwd)
                    member.Password = XEncryptionHelper.Decrypt(member.Password);
                else
                    member.Password = null;

                return member;
            }


        }

        public DtoMemberLogin CheckInvitation(long memberId, string email)
        {
            using (var db = new DrdContext(_connString))
            {
                var member =
                    (from c in db.Members
                     where c.Email.Equals(email)
                     select new DtoMemberLogin
                     {
                         Id = c.Id,
                         Number = c.Number,
                         Name = c.Name,
                         ImageProfile = c.ImageProfile,
                         ActivationKeyId = c.ActivationKeyId,
                         Phone = c.Phone,
                         Email = c.Email,
                         CompanyId = c.CompanyId,
                         UserGroup = c.UserGroup,
                     }).FirstOrDefault();

                if (member == null)
                {
                    member = new DtoMemberLogin();
                    member.Id = 0;
                }
                else if (member.Id == memberId) //invited to self
                {
                    member = new DtoMemberLogin();
                    member.Id = -1;
                }
                else {
                    var invited = db.MemberInviteds.FirstOrDefault(c => c.MemberId == memberId && c.InvitedId == member.Id && ("10,11").Contains(c.Status));
                    if (invited != null) // already invited
                    {
                        member = new DtoMemberLogin();
                        member.Id = -2;
                    }
                }

                return member;
            }


        }

        public int ChangePassword(long id, string oldPassword, string newPassword)
        {
            int result = 0;

            using (var db = new DrdContext(_connString))
            {
                var Member = db.Members.FirstOrDefault(c => c.Id == id);
                if (Member == null)
                    return -2;  // invalid Member

                string pwd = XEncryptionHelper.Encrypt(oldPassword);

                if (!Member.Password.Equals(pwd))
                    return -1;  // old password not same

                Member.Password = XEncryptionHelper.Encrypt(newPassword);

                result = db.SaveChanges();
                return result;
            }
        }

        public bool ValidationPassword(long id, string password)
        {
            using (var db = new DrdContext(_connString))
            {
                var Member = db.Members.FirstOrDefault(c => c.Id == id);
                if (Member == null)
                    return false;  // invalid Member

                if (!Member.Password.Equals(XEncryptionHelper.Encrypt(password)))
                    return false;
                else
                    return true;
            }
        }

        public DtoMember GetById(long id, long loginId)
        {
            ApplConfigService.GenerateUniqueKeyLong();

            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Members
                     where c.Id == id
                     select new DtoMember
                     {
                         Id = c.Id,
                         Number = c.Number,
                         Name = c.Name,
                         Phone = c.Phone,
                         Email = c.Email,
                         ImageProfile = c.ImageProfile,
                         IsActive = c.IsActive,
                         //Password = c.Password,
                         KtpNo = c.KtpNo,
                         ImageQrCode = c.ImageQrCode,
                         LastLogin = c.LastLogin,
                         LastLogout = c.LastLogout,
                         MemberType = c.MemberType,
                         LoginId = loginId,
                         ImageSignature = (id == loginId ? c.ImageSignature : ""),
                         ImageInitials = (id == loginId ? c.ImageInitials : ""),
                         ImageStamp = (id == loginId ? c.ImageStamp : ""),
                         ImageKtp1 = (id == loginId ? c.ImageKtp1 : ""),
                         ImageKtp2 = (id == loginId ? c.ImageKtp2 : ""),
                         CompanyId = c.CompanyId,
                         UserGroup = c.UserGroup,
                         ActivationKeyId = c.ActivationKeyId,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                         UserId = c.UserId,
                         Company = new DtoCompany
                         {
                             Name = c.Company.Name,
                         }
                     }).FirstOrDefault();

                GenericDataService gdsvr = new GenericDataService(ConfigConstant.CONSTRING_USER);
                result.Master.UserGroups = gdsvr.GetUserGroupMenu();

                return result;
            }
        }

        public int UpdatePhotoProfile(long memberId, string fileName)
        {
            using (var db = new DrdContext(_connString))
            {
                var member = db.Members.FirstOrDefault(c => c.Id == memberId);
                member.ImageProfile = fileName;
                member.DateUpdated = DateTime.Now;
                return db.SaveChanges();
            }

        }

        public int UpdateImage(long memberId, string fileName, int imageType)
        {
            using (var db = new DrdContext(_connString))
            {
                var member = db.Members.FirstOrDefault(c => c.Id == memberId);
                if (imageType == 0)
                    member.ImageProfile = fileName;
                else if (imageType == 1)
                    member.ImageSignature = fileName;
                else if (imageType == 2)
                    member.ImageInitials = fileName;
                else if (imageType == 3)
                    member.ImageStamp = fileName;
                else if (imageType == 4)
                    member.ImageKtp1 = fileName;
                else if (imageType == 5)
                    member.ImageKtp2 = fileName;
                member.DateUpdated = DateTime.Now;
                return db.SaveChanges();
            }

        }

        public int UpdateKtpNo(long memberId, string ktpNo)
        {
            using (var db = new DrdContext(_connString))
            {
                var member = db.Members.FirstOrDefault(c => c.Id == memberId);
                member.KtpNo = ktpNo;
                member.DateUpdated = DateTime.Now;
                return db.SaveChanges();
            }

        }

        public IEnumerable<DtoMemberTitle> GetMemberTitles()
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.MemberTitles
                     select new DtoMemberTitle
                     {
                         Id = c.Id,
                         Title = c.Title,
                     }).ToList();

                return result;
            }

        }

        public IEnumerable<DtoMemberLite> GetAdvisors()
        {
            using (var db = new DrdContext(_connString))
            {
                var advisors =
                    (from c in db.Members
                     from a in db.MemberTypes
                     where a.BitValue >= 4096 && c.MemberType >= 4096 && (c.MemberType & a.BitValue) == a.BitValue && c.IsActive
                     select new DtoMemberLite
                     {
                         Id = c.Id,
                         Name = c.Name,
                         ImageProfile = c.ImageProfile,
                         Email = c.Email,
                         Phone = c.Phone,
                         Number = c.Number,
                         Profession = a.Info,
                         MemberType = c.MemberType,
                     }).ToList();

                return advisors;
            }
        }

        public IEnumerable<DtoMemberLite> GetInvitedContacts(long memberId, string topCriteria, int page, int pageSize)
        {
            ApplConfigService.GenerateUniqueKeyLong();

            int skip = pageSize * (page - 1);
            string ordering = "Name";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.ToUpper().Split(' ');
            else
                topCriteria = null;

            using (var db = new DrdContext(_connString))
            {

                var ids = db.MemberInviteds.Where(c => (c.MemberId == memberId || c.InvitedId == memberId) && c.Status.Equals("11")).Select(c => (c.MemberId == memberId ? c.InvitedId : c.MemberId)).Distinct().ToArray();

                //var result =
                //(from c in db.MemberInviteds
                // where c.MemberId == memberId && c.Status.Equals("11") && (topCriteria == null || tops.All(x => (c.Member_InvitedId.Name.ToUpper() + " " + c.Member_InvitedId.Number + " " + c.Member_InvitedId.Email.ToUpper()).Contains(x)))
                // select new DtoMemberLite
                // {
                //     Id = c.Member_InvitedId.Id,
                //     Name = c.Member_InvitedId.Name,
                //     Phone = c.Member_InvitedId.Phone,
                //     Number = c.Member_InvitedId.Number,
                //     Email = c.Member_InvitedId.Email,
                //     ImageProfile = c.Member_InvitedId.ImageProfile,
                //     ImageQrCode = c.Member_InvitedId.ImageQrCode,
                //     UserGroup = c.Member_InvitedId.UserGroup,
                // }).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                var result =
                    (from c in db.Members
                     where (c.Id == memberId || ids.Contains(c.Id)) && (topCriteria == null || tops.All(x => (c.Name.ToUpper() + " " + c.Number + " " + c.Email.ToUpper()).Contains(x)))
                     select new DtoMemberLite
                     {
                         Id = c.Id,
                         Name = c.Name,
                         Phone = c.Phone,
                         Number = c.Number,
                         Email = c.Email,
                         ImageProfile = c.ImageProfile,
                         ImageQrCode = c.ImageQrCode,
                         UserGroup = c.UserGroup,
                     }).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                return result;

            }
        }

        public IEnumerable<DtoMemberLite> GetChatContacts(long memberId, string topCriteria, int page, int pageSize)
        {
            ApplConfigService.GenerateUniqueKeyLong();

            int skip = pageSize * (page - 1);
            string ordering = "Name";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.ToUpper().Split(' ');
            else
                topCriteria = null;

            using (var db = new DrdContext(_connString))
            {
                var result2 =
                    (from c in db.Members
                     from a in db.MemberTypes
                     where (c.MemberType >= 4096 && c.IsActive && (c.MemberType & a.BitValue) == a.BitValue)
                     orderby c.Name
                     select new DtoMemberLite
                     {
                         Id = c.Id,
                         Name = c.Name,
                         Phone = c.Phone,
                         Number = c.Number,
                         Email = c.Email,
                         ImageProfile = c.ImageProfile,
                         ImageQrCode = c.ImageQrCode,
                         UserGroup = c.UserGroup,
                         Profession = a.Info,
                     }).ToList();

                var ids2 = result2.Select(c => c.Id).ToArray();

                var ids = db.MemberInviteds.Where(c => (c.MemberId == memberId || c.InvitedId == memberId) && c.Status.Equals("11")).Select(c => (c.MemberId == memberId ? c.InvitedId : c.MemberId)).Distinct().ToArray();

                var result =
                    (from c in db.Members
                     where !ids2.Contains(c.Id) && ids.Contains(c.Id) && (topCriteria == null || tops.All(x => (c.Name.ToUpper() + " " + c.Number + " " + c.Email.ToUpper()).Contains(x)))
                     select new DtoMemberLite
                     {
                         Id = c.Id,
                         Name = c.Name,
                         Phone = c.Phone,
                         Number = c.Number,
                         Email = c.Email,
                         ImageProfile = c.ImageProfile,
                         ImageQrCode = c.ImageQrCode,
                         UserGroup = c.UserGroup,
                     }).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();



                var result3 = result2;
                result3.AddRange(result);

                return result3;

            }
        }


        public long GetChatContactsCount(long memberId, string topCriteria)
        {
            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

            using (var db = new DrdContext(_connString))
            {
                var ids = db.MemberInviteds.Where(c => (c.MemberId == memberId || c.InvitedId == memberId) && c.Status.Equals("11")).Select(c => (c.MemberId == memberId ? c.InvitedId : c.MemberId)).Distinct().ToArray();

                var result =
                    (from c in db.Members
                     where (c.Id == memberId || ids.Contains(c.Id)) && (topCriteria == null || tops.All(x => (c.Name.ToUpper() + " " + c.Number + " " + c.Email.ToUpper()).Contains(x)))
                     select new DtoMemberLite
                     {
                         Id = c.Id,
                     }).Count();

                var result2 =
                    (from c in db.Members
                     from a in db.MemberTypes
                     where (c.MemberType >= 4096 && c.IsActive && (c.MemberType & a.BitValue) == a.BitValue)
                     orderby c.Name
                     select new DtoMemberLite
                     {
                         Id = c.Id,
                     }).Count();

                return result + result2;

            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>

        public IEnumerable<DtoMemberLite> GetLiteAll(long companyId, string topCriteria, int page, int pageSize)
        {
            return GetLiteAll(companyId, topCriteria, page, pageSize, null, null);
        }
        public IEnumerable<DtoMemberLite> GetLiteAll(long companyId, string topCriteria, int page, int pageSize, string order)
        {
            return GetLiteAll(companyId, topCriteria, page, pageSize, order, null);
        }
        public IEnumerable<DtoMemberLite> GetLiteAll(long companyId, string topCriteria, int page, int pageSize, string order, string criteria)
        {
            ApplConfigService.GenerateUniqueKeyLong();

            int skip = pageSize * (page - 1);
            string ordering = "Name";

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
                (from c in db.Members
                 where c.CompanyId == companyId && (topCriteria == null || tops.All(x => (c.Name + " " + c.Number + " " + c.Phone + " " + c.Email).Contains(x)))
                 select new DtoMemberLite
                 {
                     Id = c.Id,
                     Name = c.Name,
                     Phone = c.Phone,
                     Number = c.Number,
                     Email = c.Email,
                     ImageProfile = c.ImageProfile,
                     ImageQrCode = c.ImageQrCode,
                     UserGroup = c.UserGroup,
                 }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                if (result != null)
                {
                    MenuService musvr = new MenuService();
                    foreach (DtoMemberLite dl in result)
                    {
                        dl.Key = musvr.EncryptData(dl.Id);
                    }
                }

                return result;

            }
        }
        public long GetLiteAllCount(long companyId, string topCriteria)
        {
            return GetLiteAllCount(companyId, topCriteria, null);
        }
        public long GetLiteAllCount(long companyId, string topCriteria, string criteria)
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
                    (from c in db.Members
                     where c.CompanyId == companyId && (topCriteria == null || tops.All(x => (c.Name + " " + c.Number + " " + c.Phone + " " + c.Email).Contains(x)))
                     select new DtoMemberLite
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
        /// <returns></returns>
        public IEnumerable<DtoMemberLite> GetLiteGroupAll(long memberId, string topCriteria, int page, int pageSize)
        {
            return GetLiteGroupAll(memberId, topCriteria, page, pageSize, null, null);
        }
        public IEnumerable<DtoMemberLite> GetLiteGroupAll(long memberId, string topCriteria, int page, int pageSize, string order)
        {
            return GetLiteGroupAll(memberId, topCriteria, page, pageSize, order, null);
        }
        public IEnumerable<DtoMemberLite> GetLiteGroupAll(long memberId, string topCriteria, int page, int pageSize, string order, string criteria)
        {
            ApplConfigService.GenerateUniqueKeyLong();

            int skip = pageSize * (page - 1);
            string ordering = "Name";

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
                    (from c in db.MemberInviteds
                     where c.MemberId == memberId && c.Status.Equals("11") && (topCriteria == null || tops.All(x => (c.Member_InvitedId.Name + " " + c.Member_InvitedId.Number + " " + c.Member_InvitedId.Phone + " " + c.Member_InvitedId.Email).Contains(x)))
                     select new DtoMemberLite
                     {
                         Id = c.Member_InvitedId.Id,
                         Name = c.Member_InvitedId.Name,
                         Phone = c.Member_InvitedId.Phone,
                         Number = c.Member_InvitedId.Number,
                         Email = c.Member_InvitedId.Email,
                         ImageProfile = c.Member_InvitedId.ImageProfile,
                         ImageQrCode = c.Member_InvitedId.ImageQrCode,
                         UserGroup = c.Member_InvitedId.UserGroup,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                if (page == 1)
                {
                    var creator =
                    (from c in db.Members
                     where c.Id == memberId && (topCriteria == null || tops.All(x => (c.Name + " " + c.Number + " " + c.Phone + " " + c.Email).Contains(x)))
                     select new DtoMemberLite
                     {
                         Id = c.Id,
                         Name = c.Name,
                         Phone = c.Phone,
                         Number = c.Number,
                         Email = c.Email,
                         ImageProfile = c.ImageProfile,
                         ImageQrCode = c.ImageQrCode,
                         UserGroup = c.UserGroup,
                     }).FirstOrDefault();

                    if (creator != null)
                    {
                        if (result == null)
                            result = new List<DtoMemberLite>();

                        result.Insert(0, creator);
                    }

                }

                return result;

            }
        }
        public long GetLiteGroupAllCount(long memberId, string topCriteria)
        {
            return GetLiteGroupAllCount(memberId, topCriteria, null);
        }
        public long GetLiteGroupAllCount(long memberId, string topCriteria, string criteria)
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
                    (from c in db.MemberInviteds
                     where c.MemberId == memberId && c.Status.Equals("11") && (topCriteria == null || tops.All(x => (c.Member_InvitedId.Name + " " + c.Member_InvitedId.Number + " " + c.Member_InvitedId.Phone + " " + c.Member_InvitedId.Email).Contains(x)))
                     select new DtoMemberLite
                     {
                         Id = c.Id,
                     }).Where(criteria).Count();
                result += 1; // tambah buat member creator
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
        public IEnumerable<DtoMemberInvited> GetInvitedLiteAll(long memberId, string topCriteria, int page, int pageSize)
        {
            return GetInvitedLiteAll(memberId, topCriteria, page, pageSize, null, null);
        }
        public IEnumerable<DtoMemberInvited> GetInvitedLiteAll(long memberId, string topCriteria, int page, int pageSize, string order)
        {
            return GetInvitedLiteAll(memberId, topCriteria, page, pageSize, order, null);
        }
        public IEnumerable<DtoMemberInvited> GetInvitedLiteAll(long memberId, string topCriteria, int page, int pageSize, string order, string criteria)
        {
            ApplConfigService.GenerateUniqueKeyLong();

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

                setInvitationExpired();

                var result =
                (from c in db.MemberInviteds
                 join s in db.StatusCodes on c.Status equals s.Code
                 where c.MemberId == memberId && (topCriteria == null || tops.All(x => (c.Member_InvitedId.Name + " " + c.Member_InvitedId.Number + " " + c.Member_InvitedId.Phone + " " + c.Member_InvitedId.Email).Contains(x)))
                 select new DtoMemberInvited
                 {
                     Id = c.Id,
                     Status = c.Status,
                     DateCreated = c.DateCreated,
                     DateUpdated = c.DateUpdated,
                     StatusDescr = s.Descr,
                     DateExpiry = c.DateExpiry,
                     Invited = new DtoMemberLite
                     {
                         Name = c.Member_InvitedId.Name,
                         Phone = c.Member_InvitedId.Phone,
                         Number = c.Member_InvitedId.Number,
                         Email = c.Member_InvitedId.Email,
                         ImageProfile = c.Member_InvitedId.ImageProfile,
                         ImageQrCode = c.Member_InvitedId.ImageQrCode,
                         UserGroup = c.Member_InvitedId.UserGroup,
                         Id = c.Member_InvitedId.Id,
                     }

                 }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

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
        /// <returns></returns>
        public IEnumerable<DtoMemberInvited> GetInvitationLiteAll(long memberId, string topCriteria, int page, int pageSize)
        {
            return GetInvitationLiteAll(memberId, topCriteria, page, pageSize, null, null);
        }
        public IEnumerable<DtoMemberInvited> GetInvitationLiteAll(long memberId, string topCriteria, int page, int pageSize, string order)
        {
            return GetInvitationLiteAll(memberId, topCriteria, page, pageSize, order, null);
        }

        public IEnumerable<DtoMemberInvited> GetInvitationLiteAll(long memberId, string topCriteria, int page, int pageSize, string order, string criteria)
        {
            ApplConfigService.GenerateUniqueKeyLong();

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

                setInvitationExpired();

                var result =
                (from c in db.MemberInviteds
                 join s in db.StatusCodes on c.Status equals s.Code
                 where c.InvitedId == memberId && (topCriteria == null || tops.All(x => (c.Member_MemberId.Name + " " + c.Member_MemberId.Number + " " + c.Member_MemberId.Phone + " " + c.Member_MemberId.Email).Contains(x)))
                 select new DtoMemberInvited
                 {
                     Id = c.Id,
                     Status = c.Status,
                     DateCreated = c.DateCreated,
                     DateUpdated = c.DateUpdated,
                     StatusDescr = s.Descr,
                     DateExpiry = c.DateExpiry,
                     Member = new DtoMemberLite
                     {
                         Name = c.Member_MemberId.Name,
                         Phone = c.Member_MemberId.Phone,
                         Number = c.Member_MemberId.Number,
                         Email = c.Member_MemberId.Email,
                         ImageProfile = c.Member_MemberId.ImageProfile,
                         ImageQrCode = c.Member_MemberId.ImageQrCode,
                         UserGroup = c.Member_MemberId.UserGroup,
                         Id = c.Member_MemberId.Id,
                     },


                 }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                return result;

            }
        }

        public long GetInvitedLiteAllCount(long memberId, string topCriteria)
        {
            return GetInvitedLiteAllCount(memberId, topCriteria, null);
        }
        public long GetInvitedLiteAllCount(long memberId, string topCriteria, string criteria)
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
                    (from c in db.MemberInviteds
                     where c.MemberId == memberId && (topCriteria == null || tops.All(x => (c.Member_InvitedId.Name + " " + c.Member_InvitedId.Number + " " + c.Member_InvitedId.Phone + " " + c.Member_InvitedId.Email).Contains(x)))
                     select new DtoMemberLite
                     {
                         Id = c.Id,
                     }).Where(criteria).Count();

                return result;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prod"></param>
        /// <returns></returns>
        public string Save(DtoMember prod)
        {
            ApplConfigService.GenerateUniqueKeyLong();

            Member product;
            int result = 0;
            ApplConfigService asvr = new ApplConfigService();
            ImageService isvr = new ImageService();
            QrBarcodeTools qrtool = new QrBarcodeTools();
            bool isgenerateqe = true;
            string pwd = DateTime.Now.ToString("ffff");
            using (var db = new DrdContext(_connString))
            {
                int count = ConfigConstant.TEST_DUPLICATION_COUNT;
                JsonDataImage dimage = new JsonDataImage();
                //var cid = db.Countries.FirstOrDefault().Id;

                if (prod.Id != 0)
                {
                    product = db.Members.FirstOrDefault(c => c.Id == prod.Id);
                    prod.Password = product.Password;
                    prod.Number = product.Number;
                    prod.ImageQrCode = product.ImageQrCode;
                    prod.MemberType = product.MemberType;
                    count = 1;

                    if (string.IsNullOrEmpty(prod.ImageQrCode))
                    {
                        prod.ImageQrCode = "DRDQR-" + prod.Number + "-" + DateTime.Now.ToString("fffff") + ".png";
                        isgenerateqe = true;
                    }
                    else
                        isgenerateqe = false;
                }
                else {
                    product = new Member();
                    var emailcx = db.Members.Count(c => c.Email == prod.Email);
                    if (emailcx != 0)
                        return "DBLEMAIL";

                    prod.Password = XEncryptionHelper.Encrypt(pwd);
                }
                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        if (prod.Id == 0)
                        {
                            prod.Number = asvr.GenerateNumber<Member>("MEM_NO", "Member", "Number", db.Members);
                            prod.ImageQrCode = "DRDQR-" + prod.Number + "-" + DateTime.Now.ToString("fffff") + ".png";
                        }
                        product.Number = prod.Number;
                        product.Name = prod.Name;
                        product.Phone = prod.Phone;
                        product.Email = prod.Email;
                        product.KtpNo = prod.KtpNo;
                        product.ImageProfile = prod.ImageProfile;
                        product.IsActive = prod.IsActive;
                        product.Password = prod.Password;
                        product.MemberType = prod.MemberType;
                        product.UserGroup = prod.UserGroup;
                        product.ImageQrCode = prod.ImageQrCode;
                        product.CompanyId = prod.CompanyId;
                        product.UserId = prod.UserId;
                        if (prod.Id == 0)
                        {
                            product.DateCreated = DateTime.Now;
                            db.Members.Add(product);
                        }
                        else {
                            product.DateUpdated = DateTime.Now;
                            if (prod.Id == prod.LoginId)
                            {
                                if (product.ImageSignature != null && !prod.ImageSignature.Equals(product.ImageSignature))
                                {
                                    MemberSignHistory msign = new MemberSignHistory();
                                    msign.MemberId = prod.Id;
                                    msign.ImageSign = product.ImageSignature;
                                    msign.SignType = 0;
                                    msign.UserId = prod.UserId;
                                    msign.DateCreated = DateTime.Now;
                                    db.MemberSignHistories.Add(msign);
                                }
                                if (product.ImageInitials != null && !prod.ImageInitials.Equals(product.ImageInitials))
                                {
                                    MemberSignHistory msign = new MemberSignHistory();
                                    msign.MemberId = prod.Id;
                                    msign.ImageSign = product.ImageInitials;
                                    msign.SignType = 1;
                                    msign.UserId = prod.UserId;
                                    msign.DateCreated = DateTime.Now;
                                    db.MemberSignHistories.Add(msign);

                                }
                                if (product.ImageStamp != null && !prod.ImageStamp.Equals(product.ImageStamp))
                                {
                                    MemberSignHistory msign = new MemberSignHistory();
                                    msign.MemberId = prod.Id;
                                    msign.ImageSign = product.ImageStamp;
                                    msign.SignType = 2;
                                    msign.UserId = prod.UserId;
                                    msign.DateCreated = DateTime.Now;
                                    db.MemberSignHistories.Add(msign);

                                }
                                if (product.ImageKtp1 != null && !prod.ImageKtp1.Equals(product.ImageKtp1))
                                {
                                    MemberSignHistory msign = new MemberSignHistory();
                                    msign.MemberId = prod.Id;
                                    msign.ImageSign = product.ImageKtp1;
                                    msign.SignType = 2;
                                    msign.UserId = prod.UserId;
                                    msign.DateCreated = DateTime.Now;
                                    db.MemberSignHistories.Add(msign);

                                }
                                if (product.ImageKtp2 != null && !prod.ImageKtp2.Equals(product.ImageKtp2))
                                {
                                    MemberSignHistory msign = new MemberSignHistory();
                                    msign.MemberId = prod.Id;
                                    msign.ImageSign = product.ImageKtp2;
                                    msign.SignType = 2;
                                    msign.UserId = prod.UserId;
                                    msign.DateCreated = DateTime.Now;
                                    db.MemberSignHistories.Add(msign);

                                }
                                product.ImageSignature = prod.ImageSignature;
                                product.ImageInitials = prod.ImageInitials;
                                product.ImageStamp = prod.ImageStamp;
                                product.ImageKtp1 = prod.ImageKtp1;
                                product.ImageKtp2 = prod.ImageKtp2;
                            }
                        }

                        result = db.SaveChanges();
                        if (prod.Id == 0 || isgenerateqe)
                        {
                            dimage.TargetFolder = "/Images/member/qrbarcode/";
                            dimage.FileName = product.ImageQrCode;
                            dimage.Data = qrtool.Generate(product.Number);
                            isvr.UploadFoto(dimage);
                        }

                        if (prod.Id == 0)
                        {
                            JsonMemberRegister memreg = new JsonMemberRegister();
                            memreg.Number = product.Number;
                            memreg.Name = product.Name;
                            memreg.Password = XEncryptionHelper.Decrypt(product.Password);
                            memreg.Email = product.Email;
                            sendEmailRegistration(memreg);
                        }
                        break;
                    }
                    catch (DbUpdateException x)
                    {
                        if (i > count)
                            throw new Exception(x.Message);
                    }
                }

                return product.Number;

            }

        }

        public long Save(Member member)
        {
            ApplConfigService.GenerateUniqueKeyLong();

            ApplConfigService asvr = new ApplConfigService();
            ImageService isvr = new ImageService();
            QrBarcodeTools qrtool = new QrBarcodeTools();
            JsonDataImage dimage = new JsonDataImage();
            int result = 0;
            using (var db = new DrdContext(_connString))
            {
                for (int i = 0; i < ConfigConstant.TEST_DUPLICATION_COUNT; i++)
                {
                    try
                    {
                        member.Number = asvr.GenerateNumber<Member>("MEM_NO", "Member", "Number", db.Members);
                        member.ImageQrCode = "DRDQR-" + member.Number + "-" + DateTime.Now.ToString("ffff") + ".png";
                        member.UserId = "REG";
                        member.ImageProfile = "icon_user.png";
                        member.DateCreated = DateTime.Now;
                        db.Members.Add(member);
                        result = db.SaveChanges();

                        dimage.TargetFolder = "/Images/member/qrbarcode/";
                        dimage.FileName = member.ImageQrCode;
                        dimage.Data = qrtool.Generate(member.Number);
                        isvr.UploadFoto(dimage);

                        break;
                    }
                    catch (DbUpdateException x)
                    {
                        if (i > ConfigConstant.TEST_DUPLICATION_COUNT)
                            throw new Exception(x.Message);
                    }
                }

                return member.Id;
            }
        }

        public string SaveRegistration(JsonMemberRegister member)
        {
            ApplConfigService.GenerateUniqueKeyLong();

            string pwd = DateTime.Now.ToString("ffff");

            var db = new DrdContext(_connString);
            var result = db.Members.Where(c => c.Email.Equals(member.Email)).ToList();
            if (result.Count != 0)
                return "0";

            MemberService svr = new MemberService();
            Member mem = new Member();
            mem.Email = member.Email;
            mem.Name = member.Name.Trim();
            mem.Phone = member.Phone;
            mem.Password = pwd;
            mem.MemberType = 1;
            mem.ImageProfile = "icon_user.png";
            svr.Save(mem);
            member.Number = mem.Number;
            member.Password = pwd;

            sendEmailRegistration(member);

            return mem.Number;
        }

        public long SaveInvitation(long memberId, string email, int expiryDay, string domain)
        {
            ApplConfigService.GenerateUniqueKeyLong();
            using (var db = new DrdContext(_connString))
            {
                var source = db.Members.FirstOrDefault(c => c.Id == memberId);
                var member = db.Members.FirstOrDefault(c => c.Email.Equals(email));
                MemberInvited inv = new MemberInvited();
                inv.MemberId = memberId;
                inv.InvitedId = member.Id;
                inv.Status = "11"; // langsung confirm
                inv.DateCreated = DateTime.Now;
                inv.DateExpiry = DateTime.Now.AddDays(expiryDay);
                db.MemberInviteds.Add(inv);
                var result = db.SaveChanges();

                sendEmailInvitation(inv.Id, source.Name, member, domain);

                return result;
            }
        }

        public void sendEmailRegistration(JsonMemberRegister member)
        {
            ApplConfigService appsvr = new ApplConfigService();
            var topaz = appsvr.GetValue("APPL_NAME");
            var admName = appsvr.GetValue("EMAILUSERDISPLAY");
            EmailTools emailtools = new EmailTools();
            //string body =
            //    "Dear " + member.Name.Trim() + ",<br/><br/>" +
            //    "Thank you for registering in " + admName + ", your member number is " + member.Number + " and your temporary login password is " + member.Password + ", please change in the application for easy login.<br/>";

            //body += "<br/><br/> " + admName + " Administrator<br/>";

            string body = emailtools.CreateHtmlBody(System.Web.HttpContext.Current.Server.MapPath("/doc/emailtemplate/Registration.html"));
            String strPathAndQuery = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
            String strUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");

            body = body.Replace("{_URL_}", strUrl);
            body = body.Replace("{_NAME_}", member.Name);
            body = body.Replace("{_NUMBER_}", member.Number);
            body = body.Replace("{_PASSWORD_}", member.Password);

            body = body.Replace("//images", "/images");

            var dbx = new DrdContext(ConfigConstant.CONSTRING);
            var resultx = dbx.ApplConfigs.ToList();
            var emailfrom = resultx.Where(c => c.Code == "EMAILUSER").FirstOrDefault().Value;
            //var emailfromdisplay = resultx.Where(c => c.Code == "EMAILUSERDISPLAY").FirstOrDefault().Value;

            var task = emailtools.Send(emailfrom, admName + " Administrator", member.Email, admName + " User Registration", body, false, new string[] { });
        }

        public void sendEmailInbox(JsonActivityResult activity)
        {
            ApplConfigService appsvr = new ApplConfigService();
            var topaz = appsvr.GetValue("APPL_NAME");
            var admName = appsvr.GetValue("EMAILUSERDISPLAY");
            EmailTools emailtools = new EmailTools();
            //string body =
            //    "Dear " + member.Name.Trim() + ",<br/><br/>" +
            //    "Thank you for registering in " + admName + ", your member number is " + member.Number + " and your temporary login password is " + member.Password + ", please change in the application for easy login.<br/>";

            //body += "<br/><br/> " + admName + " Administrator<br/>";

            string body = string.Empty;
            if (System.Web.HttpContext.Current != null)
                body = emailtools.CreateHtmlBody(System.Web.HttpContext.Current.Server.MapPath("/doc/emailtemplate/InboxNotif.html"));
            else
                body = emailtools.CreateHtmlBody(@"c:\doc\emailtemplate\InboxNotif.html");


            String strPathAndQuery = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
            String strUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");

            body = body.Replace("{_URL_}", strUrl);
            body = body.Replace("{_NAME_}", activity.MemberName);

            body = body.Replace("//images", "/images");

            //body = body.Replace("{_NUMBER_}", member.Number);
            //body = body.Replace("{_PASSWORD_}", member.Password);

            var dbx = new DrdContext(ConfigConstant.CONSTRING);
            var resultx = dbx.ApplConfigs.ToList();
            var emailfrom = resultx.Where(c => c.Code == "EMAILUSER").FirstOrDefault().Value;
            //var emailfromdisplay = resultx.Where(c => c.Code == "EMAILUSERDISPLAY").FirstOrDefault().Value;

            var task = emailtools.Send(emailfrom, admName + " Administrator", activity.Email, admName + " Inbox Reception", body, false, new string[] { });
        }

        public void sendEmailInvitation(long Id, string source, Member member, string domain)
        {
            ApplConfigService appsvr = new ApplConfigService();
            var topaz = appsvr.GetValue("APPL_NAME");
            var admName = appsvr.GetValue("EMAILUSERDISPLAY");

            string key = XEncryptionHelper.Encrypt(Id.ToString() + ",DRDINVITATION" + DateTime.Now.ToString("yyyyMMddHHmmss"));

            var link = domain + "/Invitation/agreement?key=" + key;
            EmailTools emailtools = new EmailTools();
            string body =
                "Dear " + member.Name.Trim() + ",<br/><br/>" +
                "You are invited by " + source + " to join the DRD system, congratulations on using DRD system.<br/><br/>";// +
                                                                                                                           //"<a href='" + link + "'>" + link + "</a>";


            body += "<br/><br/> " + admName + " Administrator<br/>";

            var dbx = new DrdContext(ConfigConstant.CONSTRING);
            var resultx = dbx.ApplConfigs.ToList();
            var emailfrom = resultx.Where(c => c.Code == "EMAILUSER").FirstOrDefault().Value;
            //var emailfromdisplay = resultx.Where(c => c.Code == "EMAILUSERDISPLAY").FirstOrDefault().Value;

            var task = emailtools.Send(emailfrom, admName + " Administrator", member.Email, admName + " User Invitation", body, false, new string[] { });
        }

        public JsonInvitationResult checkInvitation(long id)
        {
            JsonInvitationResult iret = new JsonInvitationResult();
            iret.Result = 1;
            setInvitationExpired();
            using (var db = new DrdContext(_connString))
            {
                var result = db.MemberInviteds.FirstOrDefault(c => c.Id == id);
                if (result == null)
                {
                    iret.Message = "Invalid invitation";
                    iret.Result = 0;
                }
                else if (result.Status.Equals("97"))
                {
                    iret.Message = "Invitation expired";
                    iret.Result = -2;
                }
                else if (result.Status.Equals("11"))
                {
                    iret.Message = "Invitation already accepted";
                    iret.Result = -1;
                }
                else
                {
                    iret.SourName = result.Member_MemberId.Name;
                    iret.TargetName = result.Member_InvitedId.Name;
                    result.Status = "11";
                    result.DateUpdated = DateTime.Now;
                    db.SaveChanges();
                }
            }
            return iret;
        }

        private void setInvitationExpired()
        {
            using (var db = new DrdContext(_connString))
            {
                var invs = db.MemberInviteds.Where(c => c.DateExpiry < DateTime.Now && c.Status.Equals("10")).ToList();
                if (invs != null)
                {
                    foreach (MemberInvited mi in invs)
                    {
                        mi.Status = "97";
                    }
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        public DtoMemberMaster GetMaster()
        {
            DtoMemberMaster masters = new DtoMemberMaster();

            GenericDataService csvr = new GenericDataService(ConfigConstant.CONSTRING_USER);
            masters.UserGroups = csvr.GetUserGroupMenu();
            csvr = new GenericDataService();
            masters.MemberTitles = csvr.GetMemberTitles();
            return masters;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        public long UpgradePlan(long memberId, int planId)
        {
            
            using (var db = new DrdContext(_connString))
            {
                // cek saldo
                MemberDepositTrxService xsvr = new MemberDepositTrxService();
                var balance = xsvr.GetDepositBalance(memberId);

                SubscriptTypeService stsvr = new SubscriptTypeService();
                var newplan = stsvr.GetById(planId);
                // insufficient balance
                if (newplan.Price > balance)
                    return -1;

                // in active old plan
                var currplan = db.MemberPlans.FirstOrDefault(c => c.IsDefault && c.MemberId == memberId);
                //currplan.IsDefault = false;
                //db.SaveChanges();
                // create new plan
                MemberPlanService plansvr = new MemberPlanService();
                var newid = plansvr.Save(memberId, planId);

                // potong deposit
                DtoMemberDepositTrx trx = new DtoMemberDepositTrx();
                var idxNo = "00000" + newid.ToString();
                trx.TrxNo = DateTime.Today.ToString("yyMMddfff") + "-" + idxNo.Substring(idxNo.Length - 6, 6);
                trx.TrxDate = DateTime.Today;
                trx.TrxType = "UPLAN";
                trx.TrxId = newid;
                trx.Descr = "Upgrade plan from " + currplan.SubscriptType.ClassName + " to " + newplan.ClassName;
                trx.MemberId = memberId;
                trx.Amount = newplan.Price;
                trx.DbCr = 1;
                xsvr.Save(trx);

                sendEmailUpgradePlan(currplan.Member, currplan.SubscriptType.ClassName, newplan.ClassName);
                return 1;
            }
        }

        public long UpgradeExtraPlan(long memberId, int planId)
        {
            using (var db = new DrdContext(_connString))
            {
                // cek saldo
                MemberDepositTrxService xsvr = new MemberDepositTrxService();
                var balance = xsvr.GetDepositBalance(memberId);

                SubscriptExtraTypeService stsvr = new SubscriptExtraTypeService();
                var newplan = stsvr.GetById(planId);
                // insufficient balance
                if (newplan.Price > balance)
                    return -1;

                // in active old plan
                var currplan = db.MemberPlans.FirstOrDefault(c => c.IsDefault && c.MemberId == memberId);
                //currplan.IsDefault = false;
                //db.SaveChanges();
                // create new plan
                MemberPlanService plansvr = new MemberPlanService();
                var newid = plansvr.SaveExtra(memberId, planId);

                // potong deposit
                DtoMemberDepositTrx trx = new DtoMemberDepositTrx();
                var idxNo = "00000" + newid.ToString();
                trx.TrxNo = DateTime.Today.ToString("yyMMddfff") + "-" + idxNo.Substring(idxNo.Length - 6, 6);
                trx.TrxDate = DateTime.Today;
                trx.TrxType = "XTRAPLAN";
                trx.TrxId = newid;
                trx.Descr = "Upgrade extra plan";
                trx.MemberId = memberId;
                trx.Amount = newplan.Price;
                trx.DbCr = 1;
                xsvr.Save(trx);

                sendEmailUpgradeNode(currplan.Member);
                return 1;
            }
        }

        public long UpgradeDrDrivePlan(long memberId, int driveId)
        {
            using (var db = new DrdContext(_connString))
            {
                // cek saldo
                MemberDepositTrxService xsvr = new MemberDepositTrxService();
                var balance = xsvr.GetDepositBalance(memberId);

                DrDriveTypeService stsvr = new DrDriveTypeService();
                var newplan = stsvr.GetById(driveId);
                // insufficient balance
                if (newplan.Price > balance)
                    return -1;

                // in active old plan
                var currplan = db.MemberPlans.FirstOrDefault(c => c.IsDefault && c.MemberId == memberId);
                //currplan.IsDefault = false;
                //db.SaveChanges();
                // create new plan
                MemberPlanService plansvr = new MemberPlanService();
                var newid = plansvr.SaveDrDrive(memberId, driveId);

                // potong deposit
                DtoMemberDepositTrx trx = new DtoMemberDepositTrx();
                var idxNo = "00000" + newid.ToString();
                trx.TrxNo = DateTime.Today.ToString("yyMMddfff") + "-" + idxNo.Substring(idxNo.Length - 6, 6);
                trx.TrxDate = DateTime.Today;
                trx.TrxType = "DRVPLAN";
                trx.TrxId = newid;
                trx.Descr = "Upgrade DrDrive plan from " + currplan.DrDriveSize + "GB to " + newplan.Size + "GB";
                trx.MemberId = memberId;
                trx.Amount = newplan.Price;
                trx.DbCr = 1;
                xsvr.Save(trx);

                sendEmailUpgradeDrDrivePlan(currplan.Member, currplan.DrDriveSize.ToString(), newplan.Size.ToString());
                return 1;
            }
        }

        private void sendEmailUpgradePlan(Member member, string from, string to)
        {
            ApplConfigService appsvr = new ApplConfigService();
            var topaz = appsvr.GetValue("APPL_NAME");
            var admName = appsvr.GetValue("EMAILUSERDISPLAY");
            EmailTools emailtools = new EmailTools();
            string body =
                "Dear " + member.Name.Trim() + ",<br/><br/>" +
                "Your plan already upgraded from " + from + " to " + to + ", thank you and anjoy your new plan " + to + "<br/><br/>";


            body += "<br/><br/> " + admName + " Administrator<br/>";

            var dbx = new DrdContext(ConfigConstant.CONSTRING);
            var resultx = dbx.ApplConfigs.ToList();
            var emailfrom = resultx.Where(c => c.Code == "EMAILUSER").FirstOrDefault().Value;
            //var emailfromdisplay = resultx.Where(c => c.Code == "EMAILUSERDISPLAY").FirstOrDefault().Value;

            var task = emailtools.Send(emailfrom, admName + " Administrator", member.Email, admName + " Upgrade Plan", body, false, new string[] { });
        }

        private void sendEmailUpgradeDrDrivePlan(Member member, string from, string to)
        {
            ApplConfigService appsvr = new ApplConfigService();
            var topaz = appsvr.GetValue("APPL_NAME");
            var admName = appsvr.GetValue("EMAILUSERDISPLAY");
            EmailTools emailtools = new EmailTools();
            string body =
                "Dear " + member.Name.Trim() + ",<br/><br/>" +
                "Your DrDrive plan already upgraded from " + from + "GB to " + to + "GB, thank you and anjoy your new plan " + to + "<br/><br/>";


            body += "<br/><br/> " + admName + " Administrator<br/>";

            var dbx = new DrdContext(ConfigConstant.CONSTRING);
            var resultx = dbx.ApplConfigs.ToList();
            var emailfrom = resultx.Where(c => c.Code == "EMAILUSER").FirstOrDefault().Value;
            //var emailfromdisplay = resultx.Where(c => c.Code == "EMAILUSERDISPLAY").FirstOrDefault().Value;

            var task = emailtools.Send(emailfrom, admName + " Administrator", member.Email, admName + " Upgrade Plan", body, false, new string[] { });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        public long UpgradePlanRequest(long memberId, int planId)
        {
            using (var db = new DrdContext(_connString))
            {
                MemberPlanRequestService plansvr = new MemberPlanRequestService();
                if (plansvr.GetByMemberId(memberId) != null)
                    return -1;

                SubscriptTypeService stsvr = new SubscriptTypeService();
                var newplan = stsvr.GetById(planId);
                // in active old plan
                var currplan = db.MemberPlans.FirstOrDefault(c => c.IsDefault && c.MemberId == memberId);
                // create new plan
                var newid = plansvr.Save(memberId, planId);

                sendEmailUpgradePlanRequest(currplan.Member, currplan.SubscriptType.ClassName, newplan.ClassName);
                return 1;
            }
        }

        private void sendEmailUpgradePlanRequest(Member member, string from, string to)
        {
            ApplConfigService appsvr = new ApplConfigService();
            var topaz = appsvr.GetValue("APPL_NAME");
            var admName = appsvr.GetValue("EMAILUSERDISPLAY");
            EmailTools emailtools = new EmailTools();
            string body =
                "Dear " + member.Name.Trim() + ",<br/><br/>" +
                "Your request upgrade plan from " + from + " to " + to + " successful sent to sales. Ours sales will contact you soon<br/><br/>";


            body += "<br/><br/> " + admName + " Administrator<br/>";

            var dbx = new DrdContext(ConfigConstant.CONSTRING);
            var resultx = dbx.ApplConfigs.ToList();
            var emailfrom = resultx.Where(c => c.Code == "EMAILUSER").FirstOrDefault().Value;
            //var emailfromdisplay = resultx.Where(c => c.Code == "EMAILUSERDISPLAY").FirstOrDefault().Value;

            var task = emailtools.Send(emailfrom, admName + " Administrator", member.Email, admName + " Upgrade Plan Request", body, false, new string[] { });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        public long UpgradeNode(long memberId, int nodeQty, int rotationQty, int sizeQty, int drdriveQty)
        {
            using (var db = new DrdContext(_connString))
            {
                var currplan = db.MemberPlans.FirstOrDefault(c => c.IsDefault && c.MemberId == memberId);

                // cek saldo
                MemberDepositTrxService xsvr = new MemberDepositTrxService();
                var balance = xsvr.GetDepositBalance(memberId);

                SubscriptTypeService stsvr = new SubscriptTypeService();
                var sub = stsvr.GetById(currplan.SubscriptTypeId);
                var total = (nodeQty * sub.FlowActivityPrice) + (rotationQty * sub.RotationPrice) + (sizeQty * sub.StoragePrice) + (drdriveQty * sub.DrDrivePrice);
                // insufficient balance
                if (total > balance)
                    return -1;

                currplan.FlowActivityCountAdd += nodeQty;
                currplan.RotationCountAdd += rotationQty;
                currplan.StorageSizeAdd += sizeQty;
                currplan.DrDriveSizeAdd += drdriveQty;

                currplan.FlowActivityPrice = sub.FlowActivityPrice;
                currplan.RotationPrice = sub.RotationPrice;
                currplan.StoragePrice = sub.StoragePrice;
                currplan.DrDrivePrice = sub.DrDrivePrice;
                db.SaveChanges();

                MemberPlanExtra ext = new MemberPlanExtra();
                ext.MemberPlanId = currplan.Id;
                ext.RotationCount = rotationQty;
                ext.RotationPrice = sub.RotationPrice;
                ext.FlowActivityCount = nodeQty;
                ext.FlowActivityPrice = sub.FlowActivityPrice;
                ext.StorageSize = sizeQty;
                ext.StoragePrice = sub.StoragePrice;
                ext.DrDriveSize = drdriveQty;
                ext.DrDrivePrice = sub.DrDrivePrice;
                ext.DateCreated = DateTime.Now;
                db.MemberPlanExtras.Add(ext);
                db.SaveChanges();

                // potong deposit
                DtoMemberDepositTrx trx = new DtoMemberDepositTrx();
                var idxNo = "00000" + ext.Id.ToString();
                trx.TrxNo = DateTime.Today.ToString("yyMMddfff") + "-" + idxNo.Substring(idxNo.Length - 6, 6);
                trx.TrxDate = DateTime.Today;
                trx.TrxType = "XPLAN";
                trx.TrxId = ext.Id;
                trx.Descr = "Extra quota plan";
                trx.MemberId = memberId;
                trx.Amount = total;
                trx.DbCr = 1;
                xsvr.Save(trx);

                sendEmailUpgradeNode(currplan.Member);
                return 1;
            }
        }

        private void sendEmailUpgradeNode(Member member)
        {
            ApplConfigService appsvr = new ApplConfigService();
            var topaz = appsvr.GetValue("APPL_NAME");
            var admName = appsvr.GetValue("EMAILUSERDISPLAY");
            EmailTools emailtools = new EmailTools();
            string body =
                "Dear " + member.Name.Trim() + ",<br/><br/>" +
                "Your extra plan already increased, thank you and anjoy your new plan values<br/><br/>";


            body += "<br/><br/> " + admName + " Administrator<br/>";

            var dbx = new DrdContext(ConfigConstant.CONSTRING);
            var resultx = dbx.ApplConfigs.ToList();
            var emailfrom = resultx.Where(c => c.Code == "EMAILUSER").FirstOrDefault().Value;
            //var emailfromdisplay = resultx.Where(c => c.Code == "EMAILUSERDISPLAY").FirstOrDefault().Value;

            var task = emailtools.Send(emailfrom, admName + " Administrator", member.Email, admName + " Upgrade Node", body, false, new string[] { });
        }

        public void GetInvitationCount(long memberId, ref JsonDashboard dsb)
        {
            using (var db = new DrdContext(_connString))
            {
                var inviteAccepted = db.MemberInviteds.Count(c => c.MemberId == memberId && c.Status.Equals("11"));
                var invitation = db.MemberInviteds.Count(c => c.InvitedId == memberId && c.Status.Equals("10"));

                dsb.InviteAccepted = inviteAccepted;
                dsb.Invitation = invitation;
            }
        }
    }
}
