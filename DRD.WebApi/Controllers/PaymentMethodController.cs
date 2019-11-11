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
    public class PaymentMethodController : ApiController
    {
        [HttpPost]
        [Route("api/paymethod")]
        public IEnumerable<DtoPaymentMethod> GetAll()
        {
            PaymentMethodService srv = new PaymentMethodService();
            var data = srv.GetAll();
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("api/paymethod")]
        public DtoPaymentMethod GetById(int Id)
        {
            PaymentMethodService srv = new PaymentMethodService();
            var data = srv.GetById(Id);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("api/paymethod")]
        public DtoPaymentMethod GetByCode(string code)
        {
            PaymentMethodService srv = new PaymentMethodService();
            var data = srv.GetByCode(code);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
