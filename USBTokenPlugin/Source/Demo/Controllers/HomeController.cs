using Demo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Demo.Controllers
{
    public class HomeController : Controller
    {
        public JsonResult GetFile()
        {
            try
            {
                Data data = new Data()
                {
                    Base64Pdf = Convert.ToBase64String(System.IO.File.ReadAllBytes(Server.MapPath("/Inputs/template.pdf"))),
                    Base64Xml = Convert.ToBase64String(System.IO.File.ReadAllBytes(Server.MapPath("/Inputs/template.xml")))
                };
                var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
                //return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveFile(string base64Pdf, string base64Xml, string subject)
        {
            try
            {
                string pdfFileName = "/Outputs/signed.pdf";
                SaveFile2Disk(pdfFileName, base64Pdf);
                string xmlFileName = "/Outputs/signed.xml";
                SaveFile2Disk(xmlFileName, base64Xml);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        private void SaveFile2Disk(string fileName, string base64String)
        {
            try
            {
                string dstFilePath = Server.MapPath(fileName);
                byte[] dataBuffer = Convert.FromBase64String(base64String);
                using (FileStream fileStream = new FileStream(dstFilePath, FileMode.Create, FileAccess.Write))
                {
                    if (dataBuffer.Length > 0)
                    {
                        fileStream.Write(dataBuffer, 0, dataBuffer.Length);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}