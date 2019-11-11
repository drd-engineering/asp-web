using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Based.Core.Entity;
using DRD.Domain;
using System.Configuration;
using DRD.Service;
using System.Based.Core;

namespace DRD.Service
{
    public class MemberTitleService
    {
        private readonly string _connString;

        public MemberTitleService(string connString)
        {
            _connString = connString;
        }

        public MemberTitleService()
        {
            _connString = ConfigConstant.CONSTRING;
        }

        public IEnumerable<DtoMemberTitle> GetAll()
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

        public DtoMemberTitle GetById(int id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.MemberTitles
                     where c.Id == id
                     select new DtoMemberTitle
                     {
                         Id = c.Id,
                         Title = c.Title,
                     }).FirstOrDefault();

                return result;
            }

        }


    }
}
