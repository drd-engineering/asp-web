using DRD.Models;
using DRD.Models.API;
using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service.Context;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace DRD.Service
{
    public class UserService
    {
        /// <summary>
        /// Save User Ragistration as a new user
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public User SaveRegistration(RegistrationData register)
        {
            using var db = new ServiceContext();
            var result = db.Users.Where(userItem => userItem.Email.Equals(register.Email)).Count();
            if (result != 0)
            {
                User retVal = new User();
                retVal.Id = -1;
                return retVal;
            }
            User user = new User(register.Email, register.Name, register.Phone)
            {
                Password = Utilities.Encrypt(System.Web.Security.Membership.GeneratePassword(length: 8, numberOfNonAlphanumericCharacters: 1))
            };
            long userId = Save(user);
            user.Id = userId;
            if (register.CompanyId != null)
            {
                MemberService memberService = new MemberService();
                long memberId = memberService.AddMemberRequestToJoinCompany(userId, register.CompanyId.Value);
            }
            return user;
        }
        /// <summary>
        /// Helper method to Save user with valid and unique user Id long value
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private long Save(User user)
        {
            using var db = new ServiceContext();
            var encryptedUserId = Utilities.Encrypt(user.Id.ToString());
            while(Constant.RESTRICTED_FOLDER_NAME.Any(w => encryptedUserId.Contains(w)) && CheckIdIsExist(user.Id))
            {
                user.Id = Utilities.RandomLongGenerator(minimumValue: Constant.MINIMUM_VALUE_ID, maximumValue: Constant.MAXIMUM_VALUE_ID);
                user.Username = user.Id.ToString();
                encryptedUserId = Utilities.Encrypt(user.Id.ToString());
            }
            db.Users.Add(user);
            db.SaveChanges();
            return user.Id;
        }
        /// <summary>
        /// Check if User Id generated is exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool CheckIdIsExist(long id)
        {
            using var db = new ServiceContext();
            return db.Users.Any(i => i.Id == id);
        }
        /// <summary>
        /// Registration notification via email, for new user
        /// </summary>
        /// <param name="user"></param>
        public void SendEmailRegistration(User user)
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
            body = body.Replace("{_PASSWORD_}", Utilities.Decrypt(user.Password));

            var senderEmail = configGenerator.GetConstant("EMAIL_USER")["value"];

            var task = emailService.Send(senderEmail, senderName, user.Email, "User Registration", body, false, new string[] { });
        }
        /// <summary>
        /// Login to DRD account using username and password
        /// </summary>
        /// <param name="username">you can use ID user and email user to login.</param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UserSession Login(string username, string password)
        {
            using (var db = new ServiceContext())
            {
                string encryptedPassword = Utilities.Encrypt(password);

                Expression<Func<User, bool>> findUsername = s => s.Email == username;
                if (!username.Contains('@'))
                {
                    long userId = Convert.ToInt64(username);
                    if (userId < 0) encryptedPassword = password;
                    findUsername = s => s.Id == userId;
                }
                User userGet = db.Users.Where(user => user.Password.Equals(encryptedPassword)).Where(findUsername).FirstOrDefault();
                if (userGet == null) return null;
                if (userGet.IsActive == false)
                {
                    userGet.IsActive = true;
                    db.SaveChanges();
                }
                UserSession loginUser = new UserSession(userGet);
                return loginUser;
            }
        }
        /// <summary>
        /// CHECK is email is never used by other user
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool CheckIsEmailAvailable(string email)
        {
            using var db = new ServiceContext();
            return !db.Users.Any(userItem => userItem.Email.Equals(email));
        }

        public UserProfile Update(UserProfile userProfile)
        {
            User user;
            using (var db = new ServiceContext())
            {
                user = db.Users.Where(u => u.Id == userProfile.Id).FirstOrDefault();
                user.InitialImageFileName = (userProfile.ImageInitials == null ? null : RemovePrefixLocation(userProfile.ImageInitials));
                user.KTPImageFileName = (userProfile.ImageKtp1 == null ? null : RemovePrefixLocation(userProfile.ImageKtp1));
                user.KTPVerificationImageFileName = (userProfile.ImageKtp2 == null ? null : RemovePrefixLocation(userProfile.ImageKtp2));
                user.ProfileImageFileName = (userProfile.ImageProfile == null ? null : RemovePrefixLocation(userProfile.ImageProfile));
                user.SignatureImageFileName = (userProfile.ImageSignature == null ? null : RemovePrefixLocation(userProfile.ImageSignature));
                user.StampImageFileName = (userProfile.ImageStamp == null ? null : RemovePrefixLocation(userProfile.ImageStamp));
                user.OfficialIdNo = userProfile.OfficialIdNo;
                db.SaveChanges();
            }
            userProfile.ImageInitials = user.InitialImageFileName;
            userProfile.ImageKtp1 = user.KTPImageFileName;
            userProfile.ImageKtp2 = user.KTPVerificationImageFileName;
            userProfile.ImageProfile = user.ProfileImageFileName;
            userProfile.ImageSignature = user.SignatureImageFileName;
            userProfile.ImageStamp = user.StampImageFileName;
            userProfile.OfficialIdNo = user.OfficialIdNo;

            return userProfile;
        }

        public string RemovePrefixLocation(string location)
        {
            if (location == null)
                return location;
            string pattern = @"(^\/.*\/)";
            location = "/" + location;
            string removedPrefix = System.Text.RegularExpressions.Regex.Replace(location, pattern, string.Empty);
            return removedPrefix;
        }
        public String GetName(long id)
        {
            using var db = new ServiceContext();
            return db.Users.Find(id).Name;
        }

        public UserProfile GetById(long id, long loginId)
        {
            using (var db = new ServiceContext())
            {
                var result =
                    (from c in db.Users
                     where c.Id == id
                     select new UserProfile
                     {
                         Id = c.Id,
                         Name = c.Name,
                         Phone = c.Phone,
                         Email = c.Email,
                         ImageProfile = c.ProfileImageFileName,
                         IsActive = c.IsActive,
                         ImageSignature = (id == loginId ? c.SignatureImageFileName : ""),
                         ImageInitials = (id == loginId ? c.InitialImageFileName : ""),
                         ImageStamp = (id == loginId ? c.StampImageFileName : ""),
                         ImageKtp1 = (id == loginId ? c.KTPImageFileName : ""),
                         ImageKtp2 = (id == loginId ? c.KTPVerificationImageFileName : ""),
                         OfficialIdNo = c.OfficialIdNo,
                         CreatedAt = c.CreatedAt
                     }).ToList().FirstOrDefault();
                return result;
            }
        }

        /// <summary>
        /// this function will return all the list Subscription if the user is admin and company have subscription, or the user have subscription.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ListSubscription GetAllSubscription(long userId)
        {
            using (var db = new ServiceContext())
            {
                var returnValue = new ListSubscription();
                //var userHasSubscription = db.PlanPersonal.Where(p => p.UserId.equals(userId));
                var userBusinessPlan = (from member in db.Members
                                        join company in db.Companies on member.CompanyId equals company.Id
                                        join usage in db.BusinessUsages on company.Id equals usage.CompanyId
                                        where member.UserId == userId
                                        && member.IsAdministrator
                                        && usage.IsActive
                                        select new SubscriptionData
                                        {
                                            id = usage.Id,
                                            type = "company",
                                            companyId = company.Id,
                                            companyName = company == null ? null : company.Name
                                        }).ToList();
                if (userBusinessPlan.Count != 0)
                {
                    foreach (var item in userBusinessPlan)
                    {
                        returnValue.items.Add(item);
                    }
                }
                return returnValue;
            }
        }

        /// <summary>
        /// Change password of specify user that loged in to application
        /// </summary>
        /// <param name="user"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public int UpdatePassword(UserSession user, String oldPassword, String newPassword)
        {
            using (var db = new ServiceContext())
            {
                var encryptedPassword = Utilities.Encrypt(oldPassword);
                User getUser = db.Users.Where(userdb => userdb.Id == user.Id && userdb.Password.Equals(encryptedPassword)).FirstOrDefault();
                if (getUser == null)
                    return -1;
                getUser.Password = Utilities.Encrypt(newPassword);
                db.SaveChanges();
                return 1;
            }
        }

        /// <summary>
        /// SAVE new user password by generator and send the password to user's email
        /// </summary>
        /// <param name="emailUser"></param>
        /// <returns></returns>
        public User ResetPassword(string emailUser)
        {
            using var db = new ServiceContext();
            var userGet = db.Users.FirstOrDefault(c => c.Email.Equals(emailUser));
            if (userGet == null) return userGet;

            var xpwd = System.Web.Security.Membership.GeneratePassword(length: 8, numberOfNonAlphanumericCharacters: 1);

            userGet.Password = Utilities.Encrypt(xpwd);
            var result = db.SaveChanges();
            
            return userGet;
        }
        /// <summary>
        /// EMAIL reset password result user
        /// </summary>
        /// <param name="user"></param>
        public void SendEmailResetPassword(User user)
        {
            var configGenerator = new AppConfigGenerator();
            var topaz = configGenerator.GetConstant("APPLICATION_NAME")["value"];
            var senderName = configGenerator.GetConstant("EMAIL_USER_DISPLAY")["value"];
            EmailService emailService = new EmailService();

            string body = emailService.CreateHtmlBody(System.Web.HttpContext.Current.Server.MapPath("/doc/emailtemplate/ResetPassword.html"));
            String strPathAndQuery = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
            String strUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");

            body = body.Replace("{_URL_}", strUrl);
            body = body.Replace("{_NAME_}", user.Name);
            body = body.Replace("{_PASSWORD_}", Utilities.Decrypt(user.Password));

            var senderEmail = configGenerator.GetConstant("EMAIL_USER")["value"];

            var task = emailService.Send(senderEmail, senderName, user.Email, "User Reset Password", body, false, new string[] { });
        }
        /// <summary>
        /// Validate the password provided is the user's password
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool ValidationPassword(long id, string password)
        {
            var equals = false;
            using (var db = new ServiceContext())
            {
                var User = db.Users.FirstOrDefault(c => c.Id == id);
                if (User == null)
                    return equals;  // invalid User

                // for test case, can be deprecated if needed
                if (User.Id < 0 && User.Password.Equals(password))
                {
                        equals = true;
                }else if (User.Password.Equals(Utilities.Encrypt(password)))
                        equals = true;

                return equals;
            }
        }
        /// <summary>
        /// Checking if the user is owner of a company or more
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool HasCompany(long userId)
        {
            var has = true;
            using (var db = new ServiceContext())
            {
                var countCompany = db.Companies.Count(c => c.OwnerId == userId && c.IsActive);
                has = countCompany > 0;
            }
            return has;
        }
        /// <summary>
        /// Checking if the user is a member of some company
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsMemberofCompany(long userId)
        {
            var isMember = true;
            using (var db = new ServiceContext())
            {
                var countMember = db.Members.Count(m => m.UserId == userId && m.IsCompanyAccept && m.IsMemberAccept);
                isMember = countMember > 0;
            }
            return isMember;
        }
        /// <summary>
        /// Checking if the user is admin of a company or more
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsAdminOfCompany(long userId)
        {
            var usrIsAdmin = false;
            using (var db = new ServiceContext())
            {
                var countAdminCompany = db.Members.Count(m => m.UserId == userId && m.IsAdministrator && m.IsActive && m.IsCompanyAccept && m.IsMemberAccept);
                usrIsAdmin = countAdminCompany > 0;
            }
            return usrIsAdmin;
        }
        /// <summary>
        /// Check if the user is an admin or has company
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsAdminOrOwnerofAnyCompany(long userId)
        {
            using (var db = new ServiceContext())
            {
                var countAsAdmin = db.Members.Count(memberItem => memberItem.UserId == userId
                    && memberItem.IsActive && memberItem.IsAdministrator && memberItem.IsActive
                    && memberItem.IsCompanyAccept && memberItem.IsMemberAccept);
                var countAsOwner = db.Companies.Count(companyItem => companyItem.OwnerId == userId && companyItem.IsActive);

                return countAsAdmin > 0 || countAsOwner > 0;
            }
        }
        public bool IsAdminOrOwnerofSpecificCompany(long userId, long companyId)
        {
            using (var db = new ServiceContext())
            {
                var countAsAdmin = db.Members.Count(memberItem => memberItem.UserId == userId
                    && memberItem.IsActive && memberItem.IsAdministrator && memberItem.IsActive
                    && memberItem.IsCompanyAccept && memberItem.IsMemberAccept && memberItem.CompanyId==companyId);
                var countAsOwner = db.Companies.Count(companyItem => companyItem.OwnerId == userId && companyItem.Id==companyId && companyItem.IsActive);

                return countAsAdmin > 0 || countAsOwner > 0;
            }
        }
    }
}