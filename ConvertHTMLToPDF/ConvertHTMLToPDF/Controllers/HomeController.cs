using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SelectPdf;

namespace ConvertHTMLToPDF.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //OK
            var content = RenderViewToString(ControllerContext, "~/Views/Home/HDGoc.cshtml", null, true);


            FileResult fileResult = new FileContentResult(BtnCreatePdf(content, null), "application/pdf");
            fileResult.FileDownloadName = "OnfinanceInvoice.pdf";
            return fileResult;
            //End OK
            ///

            // get parameters
            string headerUrl = RenderViewToString(ControllerContext, "~/Views/Home/header.cshtml", null);
            string footerUrl = RenderViewToString(ControllerContext, "~/Views/Home/footer.cshtml", null);

            bool showHeaderOnFirstPage = true;
            bool showHeaderOnOddPages = true;
            bool showHeaderOnEvenPages = true;

            int headerHeight = 50;
            try
            {
                headerHeight = 50;
            }
            catch { }


            bool showFooterOnFirstPage = true;
            bool showFooterOnOddPages = true;
            bool showFooterOnEvenPages = true;

            int footerHeight = 50;
            try
            {
                footerHeight = 50;
            }
            catch { }

            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();

            // header settings
            converter.Options.DisplayHeader = showHeaderOnFirstPage || showHeaderOnOddPages || showHeaderOnEvenPages;
            converter.Header.DisplayOnFirstPage = showHeaderOnFirstPage;
            converter.Header.DisplayOnOddPages = showHeaderOnOddPages;
            converter.Header.DisplayOnEvenPages = showHeaderOnEvenPages;
            converter.Header.Height = headerHeight;

            PdfHtmlSection headerHtml = new PdfHtmlSection(headerUrl);
            headerHtml.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
            converter.Header.Add(headerHtml);

            // footer settings
            converter.Options.DisplayFooter = showFooterOnFirstPage || showFooterOnOddPages || showFooterOnEvenPages;
            converter.Footer.DisplayOnFirstPage = showFooterOnFirstPage;
            converter.Footer.DisplayOnOddPages = showFooterOnOddPages;
            converter.Footer.DisplayOnEvenPages = showFooterOnEvenPages;
            converter.Footer.Height = footerHeight;

            PdfHtmlSection footerHtml = new PdfHtmlSection(footerUrl);
            footerHtml.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
            converter.Footer.Add(footerHtml);

            // add page numbering element to the footer
            //if (ChkPageNumbering.Checked)
            //{
                // page numbers can be added using a PdfTextSection object
                PdfTextSection text = new PdfTextSection(0, 10, "Page: {page_number} of {total_pages}  ", new System.Drawing.Font("Arial", 8));
                text.HorizontalAlign = PdfTextHorizontalAlign.Right;
                converter.Footer.Add(text);
            //}

            // create a new pdf document converting an url
            PdfDocument doc = converter.ConvertHtmlString(content, null);

            // save pdf document
            Byte[] res = null;
            res = doc.Save();

            // close pdf document
            doc.Close();

            FileResult file = new FileContentResult(res, "application/pdf");
            file.FileDownloadName = "OnfinanceInvoice.pdf";
            return fileResult;
        }

        public ActionResult HDGoc()
        {
            return View();
        }

        public ActionResult HDChdoi()
        {
            return View();
        }

        protected Byte[] BtnCreatePdf(string htmlString, string baseUrl)
        {
            PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize), "A4", true);
            PdfPageOrientation pdfOrientation = (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation), "Portrait", true);

            int webPageWidth = 1024;
            try
            {
                webPageWidth = Convert.ToInt32(1024);
            }
            catch { }

            int webPageHeight = 0;
            try
            {
                webPageHeight = Convert.ToInt32(null);
            }
            catch { }

            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();

            // set converter options
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            converter.Options.WebPageWidth = webPageWidth;
            converter.Options.WebPageHeight = webPageHeight;

            // create a new pdf document converting an url
            PdfDocument doc = converter.ConvertHtmlString(htmlString, baseUrl);

            // save pdf document
            //doc.Save("Sample.pdf");

            Byte[] res = null;
            res = doc.Save();

            // close pdf document
            doc.Close();

            return res;
        }

        public string RenderViewToString(System.Web.Mvc.ControllerContext context, string viewPath, object model = null, bool partial = false)
        {
            // first find the ViewEngine for this view
            ViewEngineResult viewEngineResult = null;
            if (partial)
            {
                viewEngineResult = ViewEngines.Engines.FindPartialView(context, viewPath);
            }
            else
            {
                viewEngineResult = ViewEngines.Engines.FindView(context, viewPath, null);
            }

            if (viewEngineResult == null)
            {
                throw new FileNotFoundException("View cannot be found.");
            }

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            context.Controller.ViewData.Model = model;

            string result = null;

            using (var sw = new StringWriter())
            {
                var ctx = new ViewContext(context, view, context.Controller.ViewData, context.Controller.TempData, sw);
                view.Render(ctx, sw);
                result = sw.ToString();
            }

            return result.Trim();
        }
    }
}