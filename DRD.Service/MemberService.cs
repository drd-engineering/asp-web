using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DRD.Models;

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

    }
}
