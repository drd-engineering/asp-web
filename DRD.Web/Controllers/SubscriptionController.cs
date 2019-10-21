using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Core;

using DRD.Domain;
using System.Dynamic;

namespace DRD.Web.Controllers
{
    public class SubscriptionController : Controller
    {


        public ActionResult Registry()
        {
            int reg = 0;
            SubscriptTypeService type = new SubscriptTypeService();
            dynamic model = new ExpandoObject();
            model.SubscriptType = type.GetById(reg);
            MemberTitleService title = new MemberTitleService();
            model.MemberTitles = title.GetAll();
            return View(model);
        }

        public ActionResult Save(DtoCompany prod)
        {
            var srv = new CompanyService();
            var data = srv.Save(prod);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}