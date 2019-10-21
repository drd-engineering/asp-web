using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRD.Core;

using DRD.Domain;
using System.IO;

namespace DRD.Web.Controllers
{
    public class TestingController : Controller
    {

        public ActionResult ViewPdf()
        {

            //string fileName = "test.pdf";
            //string filePath = "~/doc/mgn/" + fileName;
            //Response.AppendHeader("Content-Disposition", "inline; filename=" + fileName);

            //return File(filePath, "application/pdf");
            return View();
        }
        public ActionResult pdfannotate()
        {

            return View();
        }
        public ActionResult PdfViewer()
        {

            return View();
        }
        public ActionResult SimplePdfViewer()
        {

            return View();
        }
        public ActionResult Pen()
        {

            return View();
        }
    }
}