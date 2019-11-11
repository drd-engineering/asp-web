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
    public class DocumentController : ApiController
    {

        [HttpPost]
        [Route("api/document/lite")]
        public IEnumerable<DtoDocumentLite> GetLiteSelectedAll(long memberId, string topCriteria, int page, int pageSize, string order, string criteria)
        {
            DocumentService srv = new DocumentService();
            var data = srv.GetLiteByCreator(memberId, topCriteria, page, pageSize, order, criteria);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("api/document")]
        public DtoDocument GetDocument(long memberId, long rotationNodeId, long documentId)
        {
            var srv = new DocumentService();
            var data = srv.GetById(documentId);
            data.DocumentMember.FlagPermission = srv.GetPermission(memberId, rotationNodeId, documentId);

            //var fname = OpenFile(data.FileName);
            //data.FileName = fname;
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("api/document/permission")]
        public int GetPermission(long memberId, long rotationNodeId, long documentId)
        {
            var srv = new DocumentService();
            return srv.GetPermission(memberId, rotationNodeId, documentId);
        }
    }
}
