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
        /// SAVE User Ragistration as a new user
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public User SaveRegistration(RegistrationData register)
        {
            using var db = new ServiceContext();
            var result = db.Users.Where(userItem => userItem.Email.Equals(register.Email.ToLower())).Count();
            if (result != 0)
            {
                User retVal = new User();
                retVal.Id = -1;
                return retVal;
            }
            User user = new User(register.Email.ToLower(), register.Name, register.Phone)
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
        /// HELPER method to Save user with valid and unique user Id long value
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
        /// CHECK if User Id generated is exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool CheckIdIsExist(long id)
        {
            using var db = new ServiceContext();
            return db.Users.Any(i => i.Id == id);
        }
        /// <summary>
        /// SEND registration notification via email, for new user
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
        /// GET user session to DRD account using username and password
        /// </summary>
        /// <param name="username">you can use ID user and email user to login.</param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UserSession Login(string username, string password)
        {
            using var db = new ServiceContext();
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
        /// <summary>
        /// CHECK is email is never used by other user
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool CheckIsEmailAvailable(string email)
        {
            using var db = new ServiceContext();
            return !db.Users.Any(userItem => userItem.Email.Equals(email.ToLower()));
        }
        /// <summary>
        /// SAVE user profile data to user db
        /// </summary>
        /// <param name="userProfile"></param>
        /// <returns></returns>
        public UserProfile UpdateProfile(UserProfile userProfile)
        {
            User user;
            using var db = new ServiceContext();
            user = db.Users.FirstOrDefault(u => u.Id == userProfile.Id);
            if (user == null) return null;

            user.InitialImageFileName = (userProfile.InitialImageFileName == null ? null : RemovePrefixLocation(userProfile.InitialImageFileName));
            user.KTPImageFileName = (userProfile.KTPImageFileName == null ? null : RemovePrefixLocation(userProfile.KTPImageFileName));
            user.KTPVerificationImageFileName = (userProfile.KTPVerificationImageFileName == null ? null : RemovePrefixLocation(userProfile.KTPVerificationImageFileName));
            user.ProfileImageFileName = (userProfile.ProfileImageFileName == null ? null : RemovePrefixLocation(userProfile.ProfileImageFileName));
            user.SignatureImageFileName = (userProfile.SignatureImageFileName == null ? null : RemovePrefixLocation(userProfile.SignatureImageFileName));
            user.StampImageFileName = (userProfile.StampImageFileName == null ? null : RemovePrefixLocation(userProfile.StampImageFileName));
            user.OfficialIdNo = userProfile.OfficialIdNo;
            user.Name = userProfile.Name;
            user.Phone = userProfile.Phone;
            user.Email = userProfile.Email;
            user.Username = userProfile.Username;
            user.OfficialIdNo = userProfile.OfficialIdNo;
            user.TwoFactorEnabled = userProfile.TwoFactorEnabled;
            db.SaveChanges();

            // update return value
            userProfile.InitialImageFileName = user.InitialImageFileName;
            userProfile.KTPImageFileName = user.KTPImageFileName;
            userProfile.KTPVerificationImageFileName = user.KTPVerificationImageFileName;
            userProfile.ProfileImageFileName = user.ProfileImageFileName;
            userProfile.SignatureImageFileName = user.SignatureImageFileName;
            userProfile.StampImageFileName = user.StampImageFileName;
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
        public string GetName(long id)
        {
            using var db = new ServiceContext();
            return db.Users.Find(id).Name;
        }
        /// <summary>
        /// GET user profile data 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserProfile GetProfile(long id)
        {
            using var db = new ServiceContext();
            var userDb = db.Users.FirstOrDefault(u => u.Id == id);
            UserProfile result = new UserProfile(userDb);
            return result;
        }
        /// <summary>
        /// this function will return all the list Subscription if the user is admin and company have subscription, or the user have subscription.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ListSubscription GetAllSubscription(long userId)
        {
            using var db = new ServiceContext();
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
        /// <summary>
        /// SAVE new password of specify user that loged in to application
        /// </summary>
        /// <param name="user"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public int UpdatePassword(UserSession user, String oldPassword, String newPassword)
        {
            using var db = new ServiceContext();
            var encryptedPassword = Utilities.Encrypt(oldPassword);
            User getUser = db.Users.Where(userdb => userdb.Id == user.Id && userdb.Password.Equals(encryptedPassword)).FirstOrDefault();
            if (getUser == null)
                return -1;
            getUser.Password = Utilities.Encrypt(newPassword);
            db.SaveChanges();
            return 1;
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
        /// EMAIL to user result of the reset password action
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
        /// CHECK if the password provided is the user's password
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool ValidationPassword(long id, string password)
        {
            var equals = false;
            using var db = new ServiceContext();
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
        /// <summary>
        /// CHECK if the user is owner of a company or more
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool HasCompany(long userId)
        {
            using var db = new ServiceContext();
            return db.Companies.Any(c => c.OwnerId == userId && c.IsActive);
        }
        /// <summary>
        /// CHECK if the user is a member of some company
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsMemberofCompany(long userId)
        {
            using var db = new ServiceContext();
            return db.Members.Any(m => m.UserId == userId && m.IsCompanyAccept && m.IsMemberAccept);
        }
        /// <summary>
        /// CHECK if the user is admin of a company or more
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsAdminOfCompany(long userId)
        {
            using var db = new ServiceContext();
            return db.Members.Any(m => m.UserId == userId && 
                m.IsAdministrator && m.IsActive && 
                m.IsCompanyAccept && m.IsMemberAccept);
        }
        /// <summary>
        /// CHECK if the user is an admin at least in one company or has at least one company
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsAdminOrOwnerofAnyCompany(long userId)
        {
            using var db = new ServiceContext();
            return IsAdminOfCompany(userId) || HasCompany(userId);
        }
        /// <summary>
        /// CHECK if the user is an admin or has the company
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public bool IsAdminOrOwnerofSpecificCompany(long userId, long companyId)
        {
            using var db = new ServiceContext();
            return db.Members.Any(memberItem => memberItem.UserId == userId
                && memberItem.IsActive && memberItem.IsAdministrator && memberItem.IsActive
                && memberItem.IsCompanyAccept && memberItem.IsMemberAccept && memberItem.CompanyId==companyId) 
                || 
                db.Companies.Any(companyItem => companyItem.OwnerId == userId && companyItem.Id==companyId && companyItem.IsActive);
        }
    }
}