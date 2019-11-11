using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DRD.Service;
    
using DRD.Domain;


namespace DRD.Web.Controllers
{
    public class BankController : ApiController
    {
        [HttpPost]
        public IEnumerable<DtoBank> GetAll()
        {
            BankService srv = new BankService();
            var data = srv.GetAll();
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public DtoBank GetById(int Id)
        {
            BankService srv = new BankService();
            var data = srv.GetById(Id);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //[Route("api/bank/save")]
        //public DtoBank Save(DtoBank bank)
        //{
        //    BankService srv = new BankService();
        //    var data = srv.Save(bank);
        //    return data;// Json(data, JsonRequestBehavior.AllowGet);
        //}
        //[HttpPost]
        //[Route("api/bank/update")]
        //public int Update(DtoBank bank)
        //{
        //    BankService srv = new BankService();
        //    var data = srv.Update(bank);
        //    return data;// Json(data, JsonRequestBehavior.AllowGet);
        //}
    }
}
