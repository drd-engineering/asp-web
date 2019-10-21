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
    public class CompanyBankController : ApiController
    {
        [HttpPost]
        public IEnumerable<DtoCompanyBank> GetAll()
        {
            CompanyBankService srv = new CompanyBankService();
            var data = srv.GetAll();
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public DtoCompanyBank GetById(int Id)
        {
            CompanyBankService srv = new CompanyBankService();
            var data = srv.GetById(Id);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("api/companybank/save")]
        public DtoCompanyBank Save(DtoCompanyBank compBank)
        {
            CompanyBankService srv = new CompanyBankService();
            var data = srv.Save(compBank);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/companybank/update")]
        public int Update(DtoCompanyBank compBank)
        {
            CompanyBankService srv = new CompanyBankService();
            var data = srv.Update(compBank);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
