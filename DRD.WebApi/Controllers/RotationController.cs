using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DRD.Core;
using System.Based.Core.Entity;
using DRD.Domain;
using System.Based.Core;

namespace DRD.Web.Controllers
{
    public class RotationController : ApiController
    {

        [HttpPost]
        [Route("api/rotation")]
        public DtoRotation GetById(long id, long memberId)
        {
            RotationService srv = new RotationService();
            var data = srv.GetById(id, memberId);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }

        // ga kepake, hapus nanti
        [HttpPost]
        [Route("api/rotation")]
        public IEnumerable<DtoRotation> GetByMemberId(long memberId)
        {
            RotationService srv = new RotationService();
            var data = srv.GetByMemberId(memberId);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }
        // ga kepake, hapus nanti
        [HttpPost]
        [Route("api/rotation/status")]
        public IEnumerable<DtoRotation> GetByMemberId(long memberId, string status)
        {
            RotationService srv = new RotationService();
            var data = srv.GetByMemberId(memberId, status);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("api/rotation/status")]
        public IEnumerable<DtoRotationLite> GetLiteStatusAll(long memberId, string status, string topCriteria, int page, int pageSize)
        {
            RotationService srv = new RotationService();
            var data = srv.GetLiteStatusAll(memberId, status, topCriteria, page, pageSize);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }

        // ga kepake, hapus nanti 
        [HttpPost]
        [Route("api/rotation/nodestatus")]
        public IEnumerable<DtoRotation> GetNodeByMemberId(long memberId, string status)
        {
            RotationService srv = new RotationService();
            var data = srv.GetNodeByMemberId(memberId, status);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/rotation/nodestatus")]
        public IEnumerable<DtoRotationLite> GetNodeByMemberId(long memberId, string status, string topCriteria, int page, int pageSize)
        {
            RotationService srv = new RotationService();
            var data = srv.GetNodeByMemberId(memberId, status, topCriteria, page, pageSize);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("api/rotation/inbox")]
        public IEnumerable<DtoRotation> GetInboxByMemberId(long memberId)
        {
            RotationService srv = new RotationService();
            var data = srv.GetInboxByMemberId(memberId);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/rotation/node")]
        public DtoRotation GetNodeById(long id)
        {
            RotationService srv = new RotationService();
            var data = srv.GetNodeById(id);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("api/rotation/process")]
        public int ProcessActivity(JsonProcessActivity param, int bit)
        {
            var srv = new RotationService();
            var docsvr = new DocumentService();
            foreach (DtoRotationNodeDoc rotdoc in param.RotationNodeDocs)
            {
                rotdoc.Document.DocumentAnnotates = docsvr.FillAnnos(rotdoc.Document);
            }

            var data = srv.ProcessActivity(param, (ConfigConstant.EnumActivityAction)bit);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
