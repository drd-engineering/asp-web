using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Based.Core.Entity;
using System.Based.Core.Entity.User;
using DRD.Domain;
using System.Configuration;
using DRD.Core;
using System.Based.Core;

namespace DRD.Core
{
    public class UserService
    {
        private readonly string _connString;

        public UserService(string connString)
        {
            _connString = connString;
        }

        public UserService()
        {
            _connString = ConfigConstant.CONSTRING_USER;
        }

        public JsonUser Login(string username, string password)
        {
            using (var db = new DrdUserContext(_connString))
            {
                string pwd = XEncryptionHelper.Encrypt(password);

                var user =
                    (from c in db.UserMasters
                     where c.UserId.Equals(username) && c.Password.Equals(pwd)
                     select new JsonUser
                     {
                         Id = c.Id,
                         UserId = c.UserId,
                         Name = c.UserName,
                         CompanyCode = c.CompanyCode,
                     }).FirstOrDefault();

                if (user != null)
                {
                    user.ShortName = user.Name.Split(' ')[0];
                    if (!string.IsNullOrEmpty(user.CompanyCode))
                    {
                        CompanyService azsvr = new CompanyService();
                        user.Company = azsvr.GetByCode(user.CompanyCode);

                        user.UserType = 0; // user
                    }
                    else
                        user.UserType = 1; // admin

                    return user;
                }
            }

            return null;
        }

        public int ChangePassword(int id, string oldPassword, string newPassword)
        {
            int result = 0;

            using (var db = new DrdUserContext(_connString))
            {
                var user = db.UserMasters.FirstOrDefault(c => c.Id == id);
                if (user == null)
                    return -2;  // invalid user

                string pwd = XEncryptionHelper.Encrypt(oldPassword);

                if (!user.Password.Equals(pwd))
                    return -1;  // old password not same

                user.Password = XEncryptionHelper.Encrypt(newPassword);

                result = db.SaveChanges();
                return result;
            }
        }

    }
}
