using System;
using System.Linq;
using DRD.Models;
using DRD.Models.Custom;
using DRD.Service.Context;
using DRD.Models.API.Register;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;

namespace DRD.Service
{
    public class UserService
    {
        public RegisterResponse SaveRegistration(Register register)
        {
            using (var db = new ServiceContext())
            {
                var resgistrationResponse = new RegisterResponse();
                var result = db.Users.Where(userItem => userItem.Email.Equals(register.Email)).ToList();

                if (result.Count != 0)
                {
                    resgistrationResponse.Id = "DBLEMAIL";
                    return resgistrationResponse;
                }

                UserService userService = new UserService();
                User user = new User();
                user.Email = register.Email;
                user.Name = register.Name;
                user.Phone = register.Phone;
                user.IsActive = true;
                //user.Password = Utilities.Encrypt(System.Web.Security.Membership.GeneratePassword(length: 8, numberOfNonAlphanumericCharacters: 1));
                user.Password = System.Web.Security.Membership.GeneratePassword(length: 6, numberOfNonAlphanumericCharacters: 1);
                long userId = userService.Save(user);
                user.Id = userId;

                if (register.CompanyId != null)
                {
                    Member member = new Member();
                    member.UserId = userId;
                    member.CompanyId = register.CompanyId.Value;
                    long memberId = userService.Save(member);
                }

                //TODO: remove these lines when production
                System.Diagnostics.Debug.WriteLine("[[USERSERVICE]]Will sent email of : " + user.Id + " - " + user.Name);

                userService.sendEmailRegistration(user);

                resgistrationResponse.Id = "" + user.Id;
                resgistrationResponse.Email = register.Email;
                return resgistrationResponse;
            }
        }

        public UserSession Login(string username, string password)
        {
            using (var db = new ServiceContext())
            {
                //string encryptedPassword = Utilities.Encrypt(password);
                string encryptedPassword = password;

                Expression<Func<UserSession, bool>> findUsername = s => s.Email == username;
                if (!username.Contains('@'))
                {
                    long userId = Convert.ToInt64(username);
                    findUsername = s => s.Id == userId;
                }

                UserSession loginUser =
                    (from user in db.Users
                     where user.Password.Equals(encryptedPassword)
                     select new UserSession
                     {
                         Id = user.Id,
                         Name = user.Name,
                         OfficialIdNo = user.OfficialIdNo,
                         Phone = user.Phone,
                         Email = user.Email,
                         ImageProfile = user.ImageProfile,
                         ImageSignature = user.ImageSignature,
                         ImageInitials = user.ImageInitials,
                         ImageStamp = user.ImageStamp,
                         ImageKtp1 = user.ImageKtp1,
                         ImageKtp2 = user.ImageKtp2
                     }).Where(findUsername).FirstOrDefault();

                if (loginUser != null)
                {
                    loginUser.Name = loginUser.Name.Split(' ')[0];
                    return loginUser;
                }
            }

            return null;
        }

        public bool CheckEmailAvailability(string email)
        {
            using (var db = new ServiceContext())
            {
             var result = db.Users.Where(userItem => userItem.Email.Equals(email)).ToList();
                if (result.Count != 0) { return false; }
                else { return true; }
            }
        }
        public int Logout(long id)
        {
            using (var db = new ServiceContext())
            {
                var data = db.Users.FirstOrDefault(user => user.Id == id);

                //data.LastLogout = DateTime.Now;
                //return db.SaveChanges();
                return 0;
            }
        }

        public void sendEmailRegistration(User user)
        {
            //TODO: remove these lines when production
            System.Diagnostics.Debug.WriteLine("[[USERSERVICE]]Send Email Trigered");
            var configGenerator = new AppConfigGenerator();
            var topaz = configGenerator.GetConstant("APPLICATION_NAME")["value"];
            var senderName = configGenerator.GetConstant("EMAIL_USER_DISPLAY")["value"];
            EmailService emailService = new EmailService();

            string body = emailService.CreateHtmlBody(System.Web.HttpContext.Current.Server.MapPath("/doc/emailtemplate/Registration.html"));
            String strPathAndQuery = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
            String strUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");

            //TODO: remove these lines when production
            System.Diagnostics.Debug.WriteLine("[[USERSERVICE]]This is the pathquery of Email Registration");
            System.Diagnostics.Debug.WriteLine(strPathAndQuery);

            body = body.Replace("{_URL_}", strUrl);
            body = body.Replace("{_NAME_}", user.Name);
            body = body.Replace("{_NUMBER_}", "" + user.Id);
            body = body.Replace("{_PASSWORD_}", user.Password);

            body = body.Replace("//images", "/images");

            var senderEmail = configGenerator.GetConstant("EMAIL_USER")["value"];

            //TODO: remove these lines when production
            System.Diagnostics.Debug.WriteLine("[[USERSERVICE]]This is the sender of Email Registration");
            System.Diagnostics.Debug.WriteLine(senderEmail);

            var task = emailService.Send(senderEmail, senderName + " Administrator", user.Email, senderName + " User Registration", body, false, new string[] { });
        }

        public long Save(User user)
        {

            int result = 0;
            using (var db = new ServiceContext())
            {
                for (int i = 0; i < Constant.TEST_DUPLICATION_COUNT; i++)
                {
                    try
                    {
                        user.Id = Utilities.RandomLongGenerator(minimumValue: 1000000000, maximumValue: 10000000000);

                        //TODO: remove these lines when production
                        System.Diagnostics.Debug.WriteLine("[[USERSERVICE]]User ID expected when saving : " + user.Id);

                        user.ImageProfile = "icon_user.png";
                        user.CreatedAt = DateTime.Now;
                        db.Users.Add(user);
                        result = db.SaveChanges();
                        break;
                    }
                    catch (DbUpdateException x)
                    {
                        if (i > Constant.TEST_DUPLICATION_COUNT)
                            throw new Exception(x.Message);
                    }
                }

                return user.Id;
            }
        }
        public long Save(Member member)
        {
            int result = 0;
            using (var db = new ServiceContext())
            {
                for (int i = 0; i < Constant.TEST_DUPLICATION_COUNT; i++)
                {
                    try
                    {
                        member.Id = ((long)member.CompanyId * 10000) + Utilities.RandomLongGenerator(1000, 10000);
                        member.JoinedAt = DateTime.Now;
                        db.Members.Add(member);
                        result = db.SaveChanges();
                        break;
                    }
                    catch (DbUpdateException x)
                    {
                        if (i > Constant.TEST_DUPLICATION_COUNT)
                            throw new Exception(x.Message);
                    }
                }

                return member.Id;
            }

        }

        public String GetName(long id)
        { 
            using (var db = new ServiceContext())
            {
                var result =
                    (from c in db.Users
                     where c.Id == id
                     select new UserSession
                     {
                         Id = c.Id,
                         Name = c.Name
                     }).FirstOrDefault();
                return result.Name;
            }
        }
        public User GetById(long id, long loginId)
        {
            using (var db = new ServiceContext())
            {
                var result =
                    (from c in db.Users
                     where c.Id == id
                     select new User
                     {
                         Id = c.Id,
                         Name = c.Name,
                         Phone = c.Phone,
                         Email = c.Email,
                         ImageProfile = c.ImageProfile,
                         IsActive = c.IsActive,
                         ImageSignature = (id == loginId ? c.ImageSignature : ""),
                         ImageInitials = (id == loginId ? c.ImageInitials : ""),
                         ImageStamp = (id == loginId ? c.ImageStamp : ""),
                         ImageKtp1 = (id == loginId ? c.ImageKtp1 : ""),
                         ImageKtp2 = (id == loginId ? c.ImageKtp2 : ""),
                         CreatedAt = c.CreatedAt,
                     }).FirstOrDefault();
                return result;
            }
        }

    }

}