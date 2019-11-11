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
    public class MemberTopupDepositController : ApiController
    {
        [HttpPost]
        [Route("api/memtopup")]
        public IEnumerable<DtoMemberTopupDeposit> GetById(long memberId, int page, int pageSize, string order, string criteria)
        {
            MemberTopupDepositService srv = new MemberTopupDepositService();
            var data = srv.GetById(memberId, page, pageSize, order, criteria);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/memtopup")]
        public DtoMemberTopupDeposit GetById(long id)
        {
            MemberTopupDepositService srv = new MemberTopupDepositService();
            var data = srv.GetById(id);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [Route("api/memtopup/save")]
        public DtoMemberTopupDeposit Save(DtoMemberTopupDeposit topup)
        {
            MemberTopupDepositService srv = new MemberTopupDepositService();
            var data = srv.Save(topup);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/memtopup/updatestatus")]
        public int UpdateStatus(long id, string status)
        {
            MemberTopupDepositService srv = new MemberTopupDepositService();
            var data = srv.UpdateStatus(id, status);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("api/memtopup/countdetail")]
        public IEnumerable<DtoMemberTopupDeposit> GetCountDetail(string command,
            int page, int pageSize, string order, string criteria)
        {
            MemberTopupDepositService srv = new MemberTopupDepositService();
            string query = "";
            if (command.Equals("Pending"))
                query = "PaymentStatus=\"00\"";
            else if (command.Equals("Confirmation"))
                query = "PaymentStatus=\"01\"";
            else if (command.Equals("Confirmed"))
                query = "PaymentStatus=\"02\"";
            else if (command.Equals("Notconfirmed"))
                query = "PaymentStatus=\"99\"";
            return srv.GetByQuery(query, page, pageSize, order, criteria);
        }

    }
}
