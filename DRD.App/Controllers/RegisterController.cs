using DRD.Models.API;
using DRD.Service;
using DRD.Service.Context;
using System.Web.Mvc;

namespace DRD.App.Controllers
{
    public class RegisterController : Controller
    {
        private CompanyService companyService = new CompanyService();
        private UserService userService = new UserService();
        private ServiceContext db = new ServiceContext();

        // GET: Register
        // Index of register page
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// API to SAVE user's registration data 
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public ActionResult Save(RegistrationData register)
        {
            var data = userService.SaveRegistration(register);
            var registrationResponse = new RegistrationResponse();
            if (data.Id < 0)
            {
                registrationResponse.Id = "DBLEMAIL";
                return Json(registrationResponse, JsonRequestBehavior.AllowGet);
            }
            var encryptedUserId = Utilities.Encrypt(data.Id.ToString());
            string userFolder = "Images/Member/" + encryptedUserId;
            var targetDir = "/" + userFolder + "/";
            bool exists = System.IO.Directory.Exists(Server.MapPath(targetDir));
            if (!exists)
            {
                System.IO.Directory.CreateDirectory(Server.MapPath(targetDir));
                var Tranfiles = Server.MapPath("/" + "Images/Member" + "/") + "user.png";
                if (System.IO.File.Exists(Tranfiles))
                {
                    var ProcessedFiles = Server.MapPath("/" + userFolder + "/") + "user.png";
                    System.IO.File.Copy(Tranfiles, ProcessedFiles);
                }
                registrationResponse.Id = data.Id.ToString();
                registrationResponse.Email = data.Email;
                userService.SendEmailRegistration(data);
            }
            else
                registrationResponse.Id = "ERROR";
            return Json(registrationResponse, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// API to GET all company that available in DRD
        /// </summary>
        /// <returns>company details only contain name, code and id</returns>
        public ActionResult GetCompanies()
        {
            var data = companyService.GetCompanies();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// API to CHECK is email is never used by other user
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public ActionResult CheckIsEmailAvailable(string email)
        {
            var data = userService.CheckIsEmailAvailable(email);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
