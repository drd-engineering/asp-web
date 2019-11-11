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
    public class VersioningController : ApiController
    {
        [HttpPost]
        public IEnumerable<DtoVersioning> GetAll()
        {
            VersioningService srv = new VersioningService();
            var data = srv.GetAll();
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public DtoVersioning GetById(int Id)
        {
            VersioningService srv = new VersioningService();
            var data = srv.GetById(Id);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public DtoVersioning GetByPackageName(string package)
        {
            VersioningService srv = new VersioningService();
            var data = srv.GetByPackageName(package);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("api/versioning/save")]
        public DtoVersioning Save(DtoVersioning bank)
        {
            VersioningService srv = new VersioningService();
            var data = srv.Save(bank);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/versioning/update")]
        public int Update(DtoVersioning bank)
        {
            VersioningService srv = new VersioningService();
            var data = srv.Update(bank);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
