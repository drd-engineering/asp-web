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
    public class VoucherController : ApiController
    {
        [HttpPost]
        public IEnumerable<DtoVoucher> GetAll()
        {
            VoucherService srv = new VoucherService();
            var data = srv.GetAll();
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public DtoVoucher GetById(int Id)
        {
            VoucherService srv = new VoucherService();
            var data = srv.GetById(Id);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public DtoVoucher GetByNumber(string number)
        {
            VoucherService srv = new VoucherService();
            var data = srv.GetByNumber(number);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("api/voucher/save")]
        public DtoVoucher Save(DtoVoucher voucher)
        {
            VoucherService srv = new VoucherService();
            var data = srv.Save(voucher);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/voucher/setused")]
        public int SetUsed(DtoVoucher voucher)
        {
            VoucherService srv = new VoucherService();
            var data = srv.SetUsed(voucher);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        //http://api.risalah.media/api/voucher/generate?qty=10&type=0&nominal=50000&price=50000&appZone=JKT
        [HttpPost]
        [Route("api/voucher/generate")]
        public int Generate(int qty, int type, int nominal, int price, string appZone)
        {
            VoucherService srv = new VoucherService();
            var data = srv.Generate(qty, type, nominal, price);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //[Route("api/voucher/value")]
        //public JsonVoucherValue GetValue(string appZone)
        //{
        //    VoucherService srv = new VoucherService();
        //    var data = srv.GetValue(appZone);
        //    return data;// Json(data, JsonRequestBehavior.AllowGet);
        //}


        [HttpPost]
        [Route("api/vgen/list")]
        public IEnumerable<DtoVoucherGenerator> VgenGetAllPagging(int page, int pageSize, string order, string criteria)
        {
            VoucherGeneratorService srv = new VoucherGeneratorService();
            var data = srv.GetAllPagging(page, pageSize, order, criteria);
            return data;
        }

        [HttpPost]
        [Route("api/vgen")]
        public DtoVoucherGenerator VgenGetById(long id)
        {
            VoucherGeneratorService srv = new VoucherGeneratorService();
            var data = srv.GetById(id);
            return data;
        }

        [HttpPost]
        [Route("api/vgen/save")]
        public int VgenSave(DtoVoucherGenerator vg)
        {
            VoucherGeneratorService srv = new VoucherGeneratorService();
            var data = srv.Save(vg);
            return data;
        }
    }
}
