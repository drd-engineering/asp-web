using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Based.Core.Entity;
using System.Based.Core.Entity.User;
using DRD.Domain;
using System.Configuration;
using System.Based.Core;

namespace DRD.Core
{
    public class GenericDataService
    {
        private readonly string _connString;

        public GenericDataService(string connString)
        {
            _connString = connString;
        }

        public GenericDataService()
        {
            _connString = ConfigConstant.CONSTRING;
        }

        public List<JsonUserGroup> GetUserGroupMenu()
        {
            using (var db = new DrdUserContext(_connString))
            {
                var result =
                    (from c in db.GroupMasters
                     select new JsonUserGroup
                     {
                         Name = c.Name,
                         Descr = c.Descr,
                     }).ToList();

                return result;

            }
        }
        public List<DtoMemberTitle> GetMemberTitles()
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

        public List<DtoNewsType> GetNewsTypes()
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.NewsTypes
                     where c.BitValue != 0
                     select new DtoNewsType
                     {
                         Id = c.Id,
                         Descr = c.Descr,
                         Info = c.Info,
                         BitValue = c.BitValue,
                     }).ToList();

                return result;

            }
        }
    }
}
