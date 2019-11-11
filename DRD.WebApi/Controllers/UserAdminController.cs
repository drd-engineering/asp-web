using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DRD.Service;
using System.Based.Core.Entity;
using DRD.Domain;

namespace DRD.Web.Controllers
{
    public class UserAdminController : ApiController
    {
        [HttpPost]
        [Route("api/useradmin")]
        public int ChangePassword(string email, string oldPassword, string newPassword)
        {
            UserAdminService srv = new UserAdminService();
            var data = srv.ChangePassword(email, oldPassword, newPassword);
            return data;
        }
        [HttpPost]
        [Route("api/useradmin")]
        public DtoUserAdmin Login(string email, string password)
        {
            UserAdminService srv = new UserAdminService();
            var data = srv.Login(email, password, false);
            return data;
        }

        [HttpPost]
        [Route("api/usertopadmin")]
        public DtoUserAdmin Login2(string email, string password)
        {
            UserAdminService srv = new UserAdminService();
            var data = srv.Login(email, password, true);
            return data;
        }

        [HttpPost]
        [Route("api/useradmin")]
        public int Logout(string email)
        {
            UserAdminService srv = new UserAdminService();
            var data = srv.Logout(email);
            return data;
        }
        [HttpPost]
        [Route("api/useradmin")]
        public int Logout(int id)
        {
            UserAdminService srv = new UserAdminService();
            var data = srv.Logout(id);
            return data;
        }

    }
}
