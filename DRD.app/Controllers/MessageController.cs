using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Core;

using DRD.Domain;

namespace DRD.Web.Controllers
{
    public class MessageController : Controller
    {
        private DtoMemberLogin getUserLogin()
        {
            LoginController login = new LoginController();
            return login.GetUser(this);
        }

        public ActionResult GetById(int Id)
        {
            MessageService srv = new MessageService();
            var data = srv.GetById(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(DtoMessage rocc)
        {
            MessageService srv = new MessageService();
            var data = srv.Save(rocc);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetByFrom(long Id)
        {
            MessageService srv = new MessageService();
            var data = srv.GetByFrom(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetByTo(long Id)
        {
            MessageService srv = new MessageService();
            var data = srv.GetByTo(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
       

        public ActionResult UpdateDateOpened(long fromId, long toId)
        {
            MessageService srv = new MessageService();
            var data = srv.UpdateDateOpened(fromId, toId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCount(long Id)
        {
            MessageService srv = new MessageService();
            var data = srv.GetCount(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSum(long maxId, string topCriteria, int page, int pageSize)
        {
            MessageService srv = new MessageService();
            LoginController login = new LoginController();
            login.CheckLogin(this);
            DtoMemberLogin user = login.GetUser(this);
            var data = srv.GetSum(user.Id, maxId, topCriteria, page, pageSize);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSumDetail(long myId, long yourId)
        {
            MessageService srv = new MessageService();
            var data = srv.GetSumDetail(myId, yourId);
            data = data.OrderBy(c=>c.DateMessage);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //public IEnumerable<DtoMessageSumDetail> GetSumDetail(long myId, long yourId, int page, int pageSize)
        //{
        //    MessageService srv = new MessageService();
        //    var data = srv.GetSumDetail(myId, yourId, page, pageSize);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult GetCountDetail(long Id, String command)
        {
            MessageService srv = new MessageService();
            var data = srv.GetCountDetail(Id, command);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNewMessage(long myId, long yourId)
        {
            MessageService srv = new MessageService();
            var data = srv.GetNewMessage(myId, yourId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}
