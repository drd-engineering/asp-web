using DRD.Models;
using DRD.Models.API;
using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service.Context;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

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
            using var db = new Connection();
            var result = db.Users.Where(userItem => userItem.Email.Equals(register.Email.ToLower())).Count();
            if (result != 0)
            {
                User retVal = new User
                {
                    Id = -1
                };
                return retVal;
            }

            User user = new User(register.Email.ToLower(), register.Name, register.Phone);
            
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
            using var db = new Connection();
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
            using var db = new Connection();
            return db.Users.Any(i => i.Id == id);
        }

        public User CreateVoidUser(string email)
        {
            return null;
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
            String token = GenerateToken(user.Id, (int) Constant.TokenType.firstPassword);

            body = body.Replace("{_URL_}", strUrl);
            body = body.Replace("{_NAME_}", user.Name);
            body = body.Replace("{_NUMBER_}", "" + user.Id);
            body = body.Replace("{_TOKEN_}", token);

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
            using var db = new Connection();
            string encryptedPassword = Utilities.Encrypt(password);
            username = username.ToLower();

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

        public string GenerateToken(long userId, int type)
        {
            //the format will be "type|date|userId"
            String formatedToken = "" + type + "_" + userId + "_" + DateTime.Now;
            
            string token = HttpUtility.UrlEncode(Utilities.Encrypt(formatedToken));
            return token;
        }
        
        public bool CheckTokenValidity(string token)
        {

            if (token == null) return false;

            Constant.TokenType tokenType = GetTokenType(token);
            DateTime tokenDate = GetTokenDate(token);

            if (tokenType.Equals(Constant.TokenType.Error) || tokenDate.Equals(DateTime.MaxValue))
                return false;
            

            using var db = new Connection();
            return tokenType switch
            {
                (Constant.TokenType.firstPassword) => db.Users.Find(GetTokenUserId(token))?.Password == null,
                (Constant.TokenType.resetPassword) => DateTime.Now < tokenDate.AddDays(1),
                _ => false,
            };
            ;
        }

        public ResetPasswordPageData CheckTokenUserValidity(string token)
        {
            if (!CheckTokenValidity(token)) return null;
            using var db = new Connection();
            User user = db.Users.Find(GetTokenUserId(token));

            if (user == null) return null;
            string type = GetTokenType(token).ToString();
            return  new ResetPasswordPageData() { Name= user.Name, Type= type };
        }

        public UserSession LoginWithToken(string token)
        {
            using var db = new Connection();
            if (!CheckTokenValidity(token)) return null;

            long id = GetTokenUserId(token);
           
            User userGet = db.Users.Find(id);
            if (userGet == null) return null;
            if (userGet.IsActive == false)
            {
                userGet.IsActive = true;
                db.SaveChanges();
            }
            UserSession loginUser = new UserSession(userGet);
            return loginUser;
        }

        private string trySplitEncrypt(string token, int index)
        {
            string secret = Utilities.Decrypt(HttpUtility.UrlDecode(token));
            string[] tokenPart = secret.Split('_');
            if (index >= tokenPart.Length) return null;
            return tokenPart[index];
        }

        private Constant.TokenType GetTokenType(string token)
        {

            bool canConvert = int.TryParse(trySplitEncrypt(token,0), out int type);

            return canConvert ? (Constant.TokenType) type : Constant.TokenType.Error;
        }

        private DateTime GetTokenDate(string token)
        {
            bool canConvert = DateTime.TryParse(trySplitEncrypt(token, 2), out DateTime date);

            return canConvert? date : DateTime.MaxValue;
        }

        private long GetTokenUserId(string token)
        {
            bool canConvert = long.TryParse(trySplitEncrypt(token, 1), out long userId);

            return canConvert ? userId : -1;
        }

        public UserSession GetUpdatedUser(long id)
        {
            using var db = new Connection();

            User userGet = db.Users.Where(user => user.Id == id).FirstOrDefault();
            if (userGet == null) return null;

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
            using var db = new Connection();
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
            using var db = new Connection();
            user = db.Users.FirstOrDefault(u => u.Id == userProfile.Id);
            if (user == null) return null;

            user.InitialImageFileName = (userProfile.InitialImageFileName == null || userProfile.InitialImageFileName.Equals("no_picture.png") ? null : RemovePrefixLocation(userProfile.InitialImageFileName));
            user.KTPImageFileName = (userProfile.KTPImageFileName == null || userProfile.KTPImageFileName.Equals("no_picture.png") ? null : RemovePrefixLocation(userProfile.KTPImageFileName));
            user.KTPVerificationImageFileName = (userProfile.KTPVerificationImageFileName == null || userProfile.KTPVerificationImageFileName.Equals("no_picture.png") ? null : RemovePrefixLocation(userProfile.KTPVerificationImageFileName));
            user.ProfileImageFileName = (userProfile.ProfileImageFileName == null || userProfile.ProfileImageFileName.Equals("no_picture.png") ? null : RemovePrefixLocation(userProfile.ProfileImageFileName));
            user.SignatureImageFileName = (userProfile.SignatureImageFileName == null || userProfile.SignatureImageFileName.Equals("no_picture.png") ? null : RemovePrefixLocation(userProfile.SignatureImageFileName));
            user.StampImageFileName = (userProfile.StampImageFileName == null || userProfile.StampImageFileName.Equals("no_picture.png") ? null : RemovePrefixLocation(userProfile.StampImageFileName));
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
            using var db = new Connection();
            return db.Users.Find(id).Name;
        }
        /// <summary>
        /// GET user profile data 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserProfile GetProfile(long id)
        {
            using var db = new Connection();
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
            using var db = new Connection();
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
            using var db = new Connection();
            var encryptedPassword = Utilities.Encrypt(oldPassword);
            User getUser = db.Users.Where(userdb => userdb.Id == user.Id && userdb.Password.Equals(encryptedPassword)).FirstOrDefault();
            if (getUser == null)
                return -1;
            getUser.Password = Utilities.Encrypt(newPassword);
            db.SaveChanges();
            return 1;
        }

        /// <summary>
        /// SAVE new password of specify user that loged in to application
        /// </summary>
        /// <param name="user"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public int UpdatePassword(String token, String newPassword)
        {
            using var db = new Connection();
            if (!CheckTokenValidity(token)) return -1;
            User getUser = db.Users.Find(GetTokenUserId(token));
            if (getUser == null) return -1;
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
            using var db = new Connection();
            var userGet = db.Users.FirstOrDefault(c => c.Email.Equals(emailUser));
            if (userGet == null) return userGet;

            var xpwd = System.Web.Security.Membership.GeneratePassword(length: 8, numberOfNonAlphanumericCharacters: 1);

            userGet.Password = Utilities.Encrypt(xpwd);
            var result = db.SaveChanges();
            
            return userGet;
        }
        /// <summary>
        /// SAVE new user password by generator and send the password to user's email
        /// </summary>
        /// <param name="emailUser"></param>
        /// <returns></returns>
        public User GetUser(string emailUser)
        {
            using var db = new Connection();
            var userGet = db.Users.FirstOrDefault(c => c.Email.Equals(emailUser));
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
            String token = GenerateToken(user.Id,(int)Constant.TokenType.resetPassword);

            body = body.Replace("{_URL_}", strUrl);
            body = body.Replace("{_NAME_}", user.Name);
            body = body.Replace("{_URL_}", strUrl);
            body = body.Replace("{_TOKEN_}", token);

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
            using var db = new Connection();
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
            using var db = new Connection();
            return db.Companies.Any(c => c.OwnerId == userId && c.IsActive);
        }
        /// <summary>
        /// CHECK if the user is a member of some company
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsMemberofCompany(long userId)
        {
            using var db = new Connection();
            return db.Members.Any(m => m.UserId == userId && m.IsCompanyAccept && m.IsMemberAccept);
        }
        /// <summary>
        /// CHECK if the user is admin of a company or more
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsAdminOfCompany(long userId)
        {
            using var db = new Connection();
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
            using var db = new Connection();
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
            using var db = new Connection();
            return db.Members.Any(memberItem => memberItem.UserId == userId
                && memberItem.IsActive && memberItem.IsAdministrator && memberItem.IsActive
                && memberItem.IsCompanyAccept && memberItem.IsMemberAccept && memberItem.CompanyId==companyId) 
                || 
                db.Companies.Any(companyItem => companyItem.OwnerId == userId && companyItem.Id==companyId && companyItem.IsActive);
        }
    }
}