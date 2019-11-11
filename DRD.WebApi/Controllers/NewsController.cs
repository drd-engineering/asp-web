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
    public class NewsController : ApiController
    {
        [HttpPost]
        [Route("api/news")]
        public DtoNews GetById(int Id)
        {
            NewsService srv = new NewsService();
            var data = srv.GetById(Id);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/news")]
        public DtoNews GetById(int Id, long memberId)
        {
            NewsService srv = new NewsService();
            var data = srv.GetById(Id, memberId);
            return data;// Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="order"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("api/newslite")]
        public IEnumerable<DtoNewsLite> GetLiteAll(int type, string topCriteria, int page, int pageSize, string order, string criteria)
        {
            NewsService srv = new NewsService();
            var data = srv.GetLiteByTopCriteria(type, topCriteria, page, pageSize, order, criteria);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/newslite2")]
        public IEnumerable<DtoNewsLite> GetLiteAll(string types, string topCriteria, int page, int pageSize, string order, string criteria)
        {
            NewsService srv = new NewsService();
            var data = srv.GetLiteByTopCriteria(types, topCriteria, page, pageSize, order, criteria);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="type"></param>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="order"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("api/newslite")]
        public IEnumerable<DtoNewsLite> GetLiteAll(long memberId, int type, string topCriteria, int page, int pageSize, string order, string criteria)
        {
            NewsService srv = new NewsService();
            var data = srv.GetLiteByTopCriteria(memberId, type, topCriteria, page, pageSize, order, criteria);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("api/newslite2")]
        public IEnumerable<DtoNewsLite> GetLiteAll(long memberId, string types, string topCriteria, int page, int pageSize, string order, string criteria)
        {
            NewsService srv = new NewsService();
            var data = srv.GetLiteByTopCriteria(memberId, types, topCriteria, page, pageSize, order, criteria);
            return data; // Json(data, JsonRequestBehavior.AllowGet);
        }


    }
}
