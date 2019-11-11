using System;
using System.Linq;
using DRD.Models;
using DRD.Service.Context;
using DRD.Models.API.Register;
using System.Data.Entity.Infrastructure;

namespace DRD.Service
{
    public class UserService
    {
        private readonly string _connString;

        public UserService()
        {
            _connString = "ServiceContext";
        }

        public UserService(string connString)
        {
            _connString = connString;
        }
        /// ini ga usah gasih??

        public RegisterResponse SaveRegistration(Register register)
        {
            using (var db = new ServiceContext())
            {
                ApplicationConfig.GenerateUniqueKeyLong();

                var resgistrationResponse = new RegisterResponse();

                string pwd = DateTime.Now.ToString("ffff");

                var result = db.Users.Where(c => c.Email.Equals(register.Email)).ToList();

                if (result.Count != 0)
                {
                    resgistrationResponse.Id = "DBLEMAIL";
                    return resgistrationResponse;
                }

                UserService svr = new UserService();
                User user = new User();
                user.Email = register.Email;
                user.Name = register.Name.Trim();
                user.Phone = register.Phone;
                user.Password = System.Web.Security.Membership.GeneratePassword(length : 8, numberOfNonAlphanumericCharacters : 5);
                long userId = svr.Save(user);
                user.Id = userId;

                if(register.CompanyId != null){
                    Member member = new Member();
                    member.UserId = userId;
                    member.CompanyId = register.CompanyId;
                    long memberId = svr.Save(member);
                }

                svr.sendEmailRegistration(user);

                resgistrationResponse.Id = "" + user.Id;
                resgistrationResponse.Email = register.Email;
                return resgistrationResponse;
            }
        }
        public void sendEmailRegistration(User user)
        {
            var configGenerator = new AppConfigGenerator();
            var topaz = configGenerator.GetConstant("APPLICATION_NAME")["value"];
            var senderName = configGenerator.GetConstant("EMAIL_USER_DISPLAY")["value"];
            EmailService emailService = new EmailService();

            string body = emailService.CreateHtmlBody(System.Web.HttpContext.Current.Server.MapPath("/doc/emailtemplate/Registration.html"));
            String strPathAndQuery = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
            String strUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");

            body = body.Replace("{_URL_}", strUrl);
            body = body.Replace("{_NAME_}", user.Name);
            body = body.Replace("{_NUMBER_}", "" + user.Id);
            body = body.Replace("{_PASSWORD_}", user.Password);

            body = body.Replace("//images", "/images");

            var senderEmail = configGenerator.GetConstant("EMAIL_USER")["value"];
            //var emailfromdisplay = resultx.Where(c => c.Code == "EMAILUSERDISPLAY").FirstOrDefault().Value;

            var task = emailService.Send(senderEmail, senderName + " Administrator", user.Email, senderName + " User Registration", body, false, new string[] { });
        }
        
        public long Save(User user)
        {
            ApplicationConfig.GenerateUniqueKeyLong();

            ApplicationConfig asvr = new ApplicationConfig();
                        int result = 0;
            using (var db = new ServiceContext())
            {
                for (int i = 0; i < Constant.TEST_DUPLICATION_COUNT; i++)
                {
                    try
                    {
                        user.Id = asvr.LongRandom(1000000000,10000000000, new Random());
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
            ApplicationConfig.GenerateUniqueKeyLong();

            ApplicationConfig asvr = new ApplicationConfig();
                        int result = 0;
            using (var db = new ServiceContext())
            {
                for (int i = 0; i < Constant.TEST_DUPLICATION_COUNT; i++)
                {
                    try
                    {
                        member.Id = asvr.LongRandom(1000000000,10000000000, new Random());
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
    }
}
