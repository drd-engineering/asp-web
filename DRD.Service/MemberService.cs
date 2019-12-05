using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DRD.Models;
using DRD.Models.API;

using DRD.Service;
using DRD.Service.Context;

namespace DRD.Service
{
    public class MemberService
    {
        public Member getMember(long memberId)
        {
            using (var db = new ServiceContext())
            {
                return db.Members.Where(memberItem => memberItem.Id == memberId).FirstOrDefault();
            }
        }

        public Member getMember(long userId, long companyId)
        {
            using (var db = new ServiceContext())
            {
                return db.Members.Where(memberItem => memberItem.UserId == userId && memberItem.CompanyId == companyId).FirstOrDefault();
            }
        }

        public Member getAdministrator(long CompanyId)
        {
            return null;
        }

        public bool changeAdministratorAccess(long memberId, bool beAdmin)
        {
            using (var db = new ServiceContext())
            {
                Member member = getMember(memberId);
                member.IsAdministrator = beAdmin;

                bool result = db.Members.Add(member).IsAdministrator;
                db.SaveChanges();

                return result;
            }
        }

        public bool addMemberToCompany(long userId, long companyId)
        {
            using (var db = new ServiceContext())
            {
                Member member = getMember(userId, companyId);
                if (member != null)
                {
                    member.IsActive = true;
                }
                else
                {
                    member = new Member()
                    {
                        UserId = userId,
                        CompanyId = userId
                    };
                }
                bool result = db.Members.Add(member).IsAdministrator;
                db.SaveChanges();

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
        public IEnumerable<MemberData> GetLiteGroupAll(long memberId, string topCriteria, int page, int pageSize)
        {
            return GetLiteGroupAll(memberId, topCriteria, page, pageSize, null, null);
        }
        public IEnumerable<MemberData> GetLiteGroupAll(long memberId, string topCriteria, int page, int pageSize, string order)
        {
            return GetLiteGroupAll(memberId, topCriteria, page, pageSize, order, null);
        }
        public IEnumerable<MemberData> GetLiteGroupAll(long memberId, string topCriteria, int page, int pageSize, string order, string criteria)
        {
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
                topCriteria = "";

            using (var db = new ServiceContext())
            {
                var selfCompany = ().ToList();
                var result =
                    (from c in db.MemberInviteds
                     where c.MemberId == memberId && c.Status.Equals("11") && (topCriteria.Equals("") || tops.All(x => (c.Member_InvitedId.Name + " " + c.Member_InvitedId.Number + " " + c.Member_InvitedId.Phone + " " + c.Member_InvitedId.Email).Contains(x)))
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
    }
}
