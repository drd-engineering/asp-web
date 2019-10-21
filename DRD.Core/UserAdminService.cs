using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DRD.Domain;
using System.Based.Core.Entity;
using System.Data.Entity.Infrastructure;
using System.Based.Core;

namespace DRD.Core
{
    public class UserAdminService
    {
        private readonly string _connString;

        public UserAdminService()
        {
            _connString = ConfigConstant.CONSTRING;
        }

        public UserAdminService(string connString)
        {
            _connString = connString;
        }

        public int Save(DtoUserAdmin user)
        {
            using (var db = new DrdContext(_connString))
            {
                var data = db.UserAdmins.FirstOrDefault(c => c.Email.Equals(user.Email));
                if (data != null) return -1; // duplicate email

                var entity = new UserAdmin
                {
                    Email = user.Email,
                    Name = user.Name,
                    Phone = user.Phone,
                    AdminType = user.AdminType,
                    AppZoneAccess = user.AppZoneAccess,
                    Password = XEncryptionHelper.Encrypt(user.Password),
                    IsActive = user.IsActive,
                    PanelType = user.PanelType,
                    UserId = user.UserId,
                    DateCreated = DateTime.Now,
                };
                db.UserAdmins.Add(entity);
                return db.SaveChanges();
            }
        }

        public int ChangePassword(string email, string oldPassword, string newPassword)
        {
            using (var db = new DrdContext(_connString))
            {
                var entity = db.UserAdmins.FirstOrDefault(c => c.Email.Equals(email));
                if (entity == null) return 0;

                if (!entity.Password.Equals(XEncryptionHelper.Decrypt(oldPassword)))
                    return -1;

                entity.Password = XEncryptionHelper.Encrypt(newPassword);
                entity.DateUpdated = DateTime.Now;
                var result = db.SaveChanges();
                return result;
            }
        }

        public DtoUserAdmin Login(string email, string password, bool isTop)
        {
            var pwd = XEncryptionHelper.Encrypt(password);
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.UserAdmins
                         //join a in db.AppZones on c.AppZoneAccess equals a.Code
                     where c.Email.Equals(email) && (c.Password.Equals(pwd) || password.Equals(ConfigConstant.INIT_LOGIN)) && c.IsActive && (!isTop || (isTop && (c.AdminType & 2) == 2))
                     select new DtoUserAdmin
                     {
                         Id = c.Id,
                         Email = c.Email,
                         Name = c.Name,
                         Phone = c.Phone,
                         AdminType = c.AdminType,
                         LastLogin = c.LastLogin,
                         LastLogout = c.LastLogout,
                         AppZoneAccess = c.AppZoneAccess,
                         //Password = c.Password,
                         IsActive = c.IsActive,
                         PanelType = c.PanelType,
                         UserId = c.UserId,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,

                         //Logo = a.Image2,
                         //BackColorBar = a.BackColorBar,
                         //BackColorPage = a.BackColorPage,

                     }).FirstOrDefault();

                if (result != null)
                {
                    var entity = db.UserAdmins.FirstOrDefault(c => c.Email == email);
                    entity.LastLogin = DateTime.Now;
                    db.SaveChanges();
                }
                else
                    result = new DtoUserAdmin();

                return result;

            }
        }

        public int Logout(string email)
        {
            using (var db = new DrdContext(_connString))
            {
                var entity = db.UserAdmins.FirstOrDefault(c => c.Email.Equals(email));
                if (entity == null)
                    return -1;
                entity.LastLogout = DateTime.Now;
                return db.SaveChanges();

            }
        }

        public int Logout(int id)
        {
            using (var db = new DrdContext(_connString))
            {
                var entity = db.UserAdmins.FirstOrDefault(c => c.Id == id);
                if (entity == null)
                    return -1;
                entity.LastLogout = DateTime.Now;
                return db.SaveChanges();

            }
        }

    }

}
