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
    public class MessageController : ApiController
    {
        [HttpPost]
        [Route("api/message")]
        public DtoMessage GetById(int Id)
        {
            MessageService srv = new MessageService();
            var data = srv.GetById(Id);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("api/message/save")]
        public DtoMessage Save(DtoMessage rocc)
        {
            MessageService srv = new MessageService();
            var data = srv.Save(rocc);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("api/message/from")]
        public IEnumerable<DtoMessage> GetByFrom(long Id)
        {
            MessageService srv = new MessageService();
            var data = srv.GetByFrom(Id);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/message/to")]
        public IEnumerable<DtoMessage> GetByTo(long Id)
        {
            MessageService srv = new MessageService();
            var data = srv.GetByTo(Id);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        //[Route("api/message/setopen")]
        //public int UpdateDateOpened(long Id)
        //{
        //    MessageService srv = new MessageService();
        //    var data = srv.UpdateDateOpened(Id);
        //    return data;// Json(data, JsonRequestBehavior.AllowGet);
        //}
        //[HttpPost]
        //[Route("api/message/setopen")]
        //public int UpdateDateOpened(long yourId)
        //{
        //    MessageService srv = new MessageService();
        //    var data = srv.UpdateDateOpened(yourId);
        //    return data;// Json(data, JsonRequestBehavior.AllowGet);
        //}
        [HttpPost]
        [Route("api/message/setopen")]
        public int UpdateDateOpened(long fromId, long toId)
        {
            MessageService srv = new MessageService();
            var data = srv.UpdateDateOpened(fromId, toId);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/message/count")]
        public JsonMessageCount GetCount(long Id)
        {
            MessageService srv = new MessageService();
            var data = srv.GetCount(Id);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/message/sum")]
        public IEnumerable<DtoMessageSum> GetSum(long Id, long maxId, string topCriteria, int page, int pageSize)
        {
            MessageService srv = new MessageService();
            var data = srv.GetSum(Id, maxId, topCriteria, page, pageSize);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/message/sumdetail")]
        public IEnumerable<DtoMessageSumDetail> GetSumDetail(long myId, long yourId)
        {
            MessageService srv = new MessageService();
            var data = srv.GetSumDetail(myId, yourId);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/message/sumdetail")]
        public IEnumerable<DtoMessageSumDetail> GetSumDetail(long myId, long yourId, int page, int pageSize)
        {
            MessageService srv = new MessageService();
            var data = srv.GetSumDetail(myId, yourId, page, pageSize);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/message/countdetail")]
        public IEnumerable<DtoMessage> GetCountDetail(long Id, String command)
        {
            MessageService srv = new MessageService();
            var data = srv.GetCountDetail(Id, command);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/message/getnewmessage")]
        public IEnumerable<DtoMessageSumDetail> GetNewMessage(long myId, long yourId)
        {
            MessageService srv = new MessageService();
            var data = srv.GetNewMessage(myId, yourId);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
