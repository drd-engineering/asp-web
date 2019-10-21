using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Based.Core.Entity.User;
using DRD.Domain;
using DRD.Core;
using System.Based.Core;

namespace DRD.Core
{
    public class UserMasterService
    {
        private readonly string _connString;

        public UserMasterService(string connString)
        {
            _connString = connString;
        }

        public UserMasterService()
        {
            _connString = ConfigConstant.CONSTRING_USER;
        }

        public IEnumerable<JsonUserMaster> GetAll()
        {
            using (var db = new DrdUserContext(_connString))
            {
                var result =
                    (from c in db.UserMasters
                     select new JsonUserMaster
                     {
                         Id = c.Id,
                         UserId = c.UserId,
                         UserName = c.UserName,
                         Password = c.Password,
                         CompanyCode = c.CompanyCode,
                         GroupMasterId = c.GroupMasterId,
                         UserTypeId = c.UserTypeId
                     }).ToList();


                return result;
            }
        }

        public JsonUserMaster GetById(long Id)
        {
            using (var db = new DrdUserContext(_connString))
            {
                var result =
                    (from c in db.UserMasters
                     where c.Id == Id
                     select new JsonUserMaster
                     {
                         Id = c.Id,
                         UserId = c.UserId,
                         UserName = c.UserName,
                         Password = c.Password,
                         CompanyCode = c.CompanyCode,
                         GroupMasterId = c.GroupMasterId,
                         UserTypeId = c.UserTypeId
                     }).FirstOrDefault();


                return result;
            }
        }

        public JsonUserMaster GetByUserLogin(String user, String password)
        {
            String pwd = XEncryptionHelper.Encrypt(password);
            using (var db = new DrdUserContext(_connString))
            {
                var result =
                    (from c in db.UserMasters
                     where c.UserId == user && c.Password == pwd
                     select new JsonUserMaster
                     {
                         Id = c.Id,
                         UserId = c.UserId,
                         UserName = c.UserName,
                         Password = c.Password,
                         CompanyCode = c.CompanyCode,
                         GroupMasterId = c.GroupMasterId,
                         UserTypeId = c.UserTypeId
                     }).FirstOrDefault();

                return result;
            }
        }

        public int ChangePassword(String user, String password, String newPassword)
        {
            String pwd = XEncryptionHelper.Encrypt(password);
            String newpwd = XEncryptionHelper.Decrypt(newPassword);
            using (var db = new DrdUserContext(_connString))
            {
                var entity = db.UserMasters.FirstOrDefault(c => c.UserId == user && c.Password == pwd);
                if (entity == null) return 0;

                entity.Password = newpwd;

                var result = db.SaveChanges();
                return result;
            }
        }

    }
}
