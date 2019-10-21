using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DRD.Core;
using System.Based.Core.Entity;
using DRD.Domain;

namespace DRD.Web.Controllers
{
    public class CompanyController : ApiController
    {
       
        [HttpPost]
        [Route("api/company")]
        public DtoCompany GetByCode(string code)
        {
            CompanyService srv = new CompanyService();
            var data = srv.GetByCode(code);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }
        
    }
}
