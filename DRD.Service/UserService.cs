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
        public User SaveRegistration(Register register)
        {
            using (var db = new ServiceContext())
            {
                var result = db.Users.Where(userItem => userItem.Email.Equals(register.Email)).ToList();

                if (result.Count != 0)
                {
                    User retVal = new User();
                    retVal.Id = -1;
                    return retVal;
                }
                User user = new User();
                user.Email = register.Email;
                user.Name = register.Name;
                user.Phone = register.Phone;
                user.IsActive = true;
                user.Password = Utilities.Encrypt(System.Web.Security.Membership.GeneratePassword(length: 8, numberOfNonAlphanumericCharacters: 1));
                // user.Password = System.Web.Security.Membership.GeneratePassword(length: 6, numberOfNonAlphanumericCharacters: 1);
                long userId = Save(user);
                user.Id = userId;
                if (register.CompanyId != null)
                {
                    Member member = new Member();
                    member.UserId = userId;
                    member.isMemberAccept = true;
                    member.CompanyId = register.CompanyId.Value;
                    long memberId = Save(member);
                }
                return user;
            }
        }
        /// <summary>
        /// Save user with valid user Id long value
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public long Save(User user)
        {
            int result = 0;
            using (var db = new ServiceContext())
            {
                for (int i = 0; i < Constant.TEST_DUPLICATION_COUNT; i++)
                {
                    for (int j = 0; j < Constant.TEST_DUPLICATION_COUNT; j++)
                    {
                        user.Id = Utilities.RandomLongGenerator(minimumValue: 1000000000, maximumValue: 10000000000);
                        var encryptedUserId = Utilities.Encrypt(user.Id.ToString());
                        if (!Constant.RESTRICTED_FOLDER_NAME.Any(w => encryptedUserId.Contains(w)))
                            break;
                    }
                    try
                    {
                        user.ImageProfile = "user.png";
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
        /// <summary>
        /// Save member with valid user Id long value
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
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
        public UserSession Login(string username, string password)
        {
            using (var db = new ServiceContext())
            {
                string encryptedPassword = Utilities.Encrypt(password);
                /*string encryptedPassword = password;*/

                Expression<Func<User, bool>> findUsername = s => s.Email == username;
                if (!username.Contains('@'))
                {
                    long userId = Convert.ToInt64(username);
                    if (userId < 0)
                        encryptedPassword = password;
                    findUsername = s => s.Id == userId;
                }
                User userGet = db.Users.Where(user => user.Password.Equals(encryptedPassword)).Where(findUsername).FirstOrDefault();

                if (userGet != null)
                {
                    if (userGet.IsActive == false)
                    {
                        userGet.IsActive = true;
                        db.SaveChanges();
                    }
                    UserSession loginUser = new UserSession();
                    loginUser.Id = userGet.Id;
                    loginUser.EncryptedId = Utilities.Encrypt(userGet.Id.ToString());
                    loginUser.Name = userGet.Name;
                    loginUser.OfficialIdNo = userGet.OfficialIdNo;
                    loginUser.Phone = userGet.Phone;
                    loginUser.Email = userGet.Email;
                    loginUser.ImageProfile = userGet.ImageProfile;
                    loginUser.ImageSignature = userGet.ImageSignature;
                    loginUser.ImageInitials = userGet.ImageInitials;
                    loginUser.ImageStamp = userGet.ImageStamp;
                    loginUser.ImageKtp1 = userGet.ImageKtp1;
                    loginUser.ImageKtp2 = userGet.ImageKtp2;

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
        public UserProfile Update(UserProfile userProfile)
        {
            User user;
            using (var db = new ServiceContext())
            {
                user = db.Users.Where(u => u.Id == userProfile.Id).FirstOrDefault();
                user.ImageInitials = (userProfile.ImageInitials == null ? null : RemovePrefixLocation(userProfile.ImageInitials));
                user.ImageKtp1 = (userProfile.ImageKtp1 == null ? null : RemovePrefixLocation(userProfile.ImageKtp1));
                user.ImageKtp2 = (userProfile.ImageKtp2 == null ? null : RemovePrefixLocation(userProfile.ImageKtp2));
                user.ImageProfile = (userProfile.ImageProfile == null ? null : RemovePrefixLocation(userProfile.ImageProfile));
                user.ImageSignature = (userProfile.ImageSignature == null ? null : RemovePrefixLocation(userProfile.ImageSignature));
                user.ImageStamp = (userProfile.ImageStamp == null ? null : RemovePrefixLocation(userProfile.ImageStamp));
                user.OfficialIdNo = userProfile.OfficialIdNo;
                db.SaveChanges();
            }
            userProfile.ImageInitials = user.ImageInitials;
            userProfile.ImageKtp1 = user.ImageKtp1;
            userProfile.ImageKtp2 = user.ImageKtp2;
            userProfile.ImageProfile = user.ImageProfile;
            userProfile.ImageSignature = user.ImageSignature;
            userProfile.ImageStamp = user.ImageStamp;
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
                         ImageProfile = c.ImageProfile,
                         IsActive = c.IsActive,
                         ImageSignature = (id == loginId ? c.ImageSignature : ""),
                         ImageInitials = (id == loginId ? c.ImageInitials : ""),
                         ImageStamp = (id == loginId ? c.ImageStamp : ""),
                         ImageKtp1 = (id == loginId ? c.ImageKtp1 : ""),
                         ImageKtp2 = (id == loginId ? c.ImageKtp2 : ""),
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
                                        join usage in db.Usages on company.Id equals usage.CompanyId
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
        /// User for reset user password, the password will be sent to user email
        /// </summary>
        /// <param name="emailUser"></param>
        /// <returns></returns>
        public int ResetPassword(string emailUser)
        {
            using (var db = new ServiceContext())
            {
                var userGet = db.Users.FirstOrDefault(c => c.Email.Equals(emailUser));
                if (userGet == null) return 0;

                var xpwd = System.Web.Security.Membership.GeneratePassword(length: 8, numberOfNonAlphanumericCharacters: 1);

                userGet.Password = Utilities.Encrypt(xpwd);
                var result = db.SaveChanges();

                EmailService emailService = new EmailService();
                string body =
                    "Dear " + userGet.Name + ",<br/><br/>" +
                    "your password is reset, use the following temporary password:<br/><br/>" +
                    "Temporary password: <b>" + xpwd + "</b><br/><br/>" +
                    "Make a new password change after you login with this password.<br/><br/>" +
                    "Thank you<br/><br/>" +
                    "DRD<br/>";
                var configGenerator = new AppConfigGenerator();
                var emailfrom = configGenerator.GetConstant("EMAIL_USER")["value"];
                var emailfromdisplay = configGenerator.GetConstant("EMAIL_USER_DISPLAY")["value"];

                var task = emailService.Send(emailfrom, emailfromdisplay, emailUser, "DRD User Reset Password", body, false, new string[] { });
                return 1;
            }
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
                if (User.Id < 0)
                    if (User.Password.Equals(password))
                        equals = true;
                    else
                    if (User.Password.Equals(Utilities.Encrypt(password)))
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
                var countMember = db.Members.Count(m => m.UserId == userId && m.isCompanyAccept && m.isMemberAccept);
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
                var countAdminCompany = db.Members.Count(m => m.UserId == userId && m.IsAdministrator && m.IsActive && m.isCompanyAccept && m.isMemberAccept);
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
                    && memberItem.isCompanyAccept && memberItem.isMemberAccept);
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
                    && memberItem.isCompanyAccept && memberItem.isMemberAccept && memberItem.CompanyId==companyId);
                var countAsOwner = db.Companies.Count(companyItem => companyItem.OwnerId == userId && companyItem.Id==companyId && companyItem.IsActive);

                return countAsAdmin > 0 || countAsOwner > 0;
            }
        }
    }
}