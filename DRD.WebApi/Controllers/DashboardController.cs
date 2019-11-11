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
    public class DashboardController : ApiController
    {
        [HttpPost]
        [Route("api/dashboard/count")]
        public JsonDashboard GetCount(long memberId)
        {
            DashboardService srv = new DashboardService();
            var data = srv.GetCount(memberId);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/dashboard/compare")]
        public bool Compare(long memberId, JsonDashboard dash)
        {
            DashboardService srv = new DashboardService();
            var data = srv.Compare(memberId, dash);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/dashboard/admin/count")]
        public JsonDashboardAdmin GetAdminCount()
        {
            DashboardService srv = new DashboardService();
            var data = srv.GetAdminCount();
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/dashboard/admin/compare")]
        public bool AdminCompare(JsonDashboardAdmin dash)
        {
            DashboardService srv = new DashboardService();
            var data = srv.AdminCompare(dash);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
