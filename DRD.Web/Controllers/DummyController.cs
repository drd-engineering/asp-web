﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DRD.Web.Controllers
{
    public class DummyController : Controller
    {
        // GET: Dummy
        public ActionResult DesktopMember()
        {
            return View();
        }
        public ActionResult Company()
        {
            return View();
        }
    }
}