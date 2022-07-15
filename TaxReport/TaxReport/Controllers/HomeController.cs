using DS.BusinessLogic.Company;
using DS.BusinessLogic.Invoice;
using DS.BusinessObject.Invoice;
using DS.Common.Helpers;
using Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace TaxReport.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadInfo(string taxnumber)
        {
            try
            {
                CompanyBLL objCompanyBLL = new CompanyBLL();
                var obj = objCompanyBLL.GetTaxCode(taxnumber);
                if (objCompanyBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(objCompanyBLL.ResultMessageBO.Message, objCompanyBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "Search");
                    return Json(new { rs = false, msg = objCompanyBLL.ResultMessageBO.Message });
                }

                return Json(new { obj });
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy thông tin doanh nghiệp", objEx, MethodBase.GetCurrentMethod().Name, "LoadInfo");
                return Json(new { msg = "Lỗi tìm kiếm." });
            }
        }

        public ActionResult Search(string taxnumber, string companyname, string daterange, int page)
        {
            try
            {
                string[] d = daterange.Replace(" ", "").Split('-');
                DateTime fromDate = DateTime.ParseExact(d[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.ParseExact(d[1], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                int itemPerPage = 20;
                int offset = (page - 1) * itemPerPage;

                InvoiceBLL objInvoiceBLL = new InvoiceBLL();
                List<InvoiceBO> lstInvoice = objInvoiceBLL.GetforGDT(taxnumber, companyname, fromDate, toDate, itemPerPage, offset);
                if (objInvoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(objInvoiceBLL.ResultMessageBO.Message, objInvoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "Search");
                    return Json(new { rs = false, msg = objInvoiceBLL.ResultMessageBO.Message });
                }

                long TotalPages = 0;
                if (lstInvoice.Count > 0)
                {
                    var intRowCount = lstInvoice.First().TOTALROW;
                    if (intRowCount % itemPerPage == 0)
                        TotalPages = intRowCount / itemPerPage;
                    else
                        TotalPages = intRowCount / itemPerPage + 1;
                }

                ViewBag.ListInvoice = lstInvoice;
                ViewBag.TotalPages = TotalPages;
                ViewBag.CurrentPage = page;

                var list = RenderViewToString(ControllerContext, "ListInvoice", null);
                var pages = RenderViewToString(ControllerContext, "Pages", null);

                return Json(new { pages, list });
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi tìm kiếm", objEx, MethodBase.GetCurrentMethod().Name, "Search");
                return Json(new { msg = "Lỗi tìm kiếm." });
            }
        }

        string RenderViewToString(ControllerContext context,
                                    string viewPath,
                                    object model = null,
                                    bool partial = false)
        {
            // first find the ViewEngine for this view
            ViewEngineResult viewEngineResult = null;
            if (partial)
                viewEngineResult = ViewEngines.Engines.FindPartialView(context, viewPath);
            else
                viewEngineResult = ViewEngines.Engines.FindView(context, viewPath, null);

            if (viewEngineResult == null)
                throw new FileNotFoundException("View cannot be found.");

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            context.Controller.ViewData.Model = model;

            string result = null;

            using (var sw = new StringWriter())
            {
                var ctx = new ViewContext(context, view,
                                            context.Controller.ViewData,
                                            context.Controller.TempData,
                                            sw);
                view.Render(ctx, sw);
                result = sw.ToString();
            }

            return result;
        }

        public ActionResult ExportExcel(string taxnumber, string companyname, string daterange)
        {
            try
            {
            string[] d = daterange.Replace(" ", "").Split('-');
            DateTime fromDate = DateTime.ParseExact(d[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime toDate = DateTime.ParseExact(d[1], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            InvoiceBLL objInvoiceBLL = new InvoiceBLL();
            List<InvoiceBO> lstInvoice = objInvoiceBLL.GetforGDT(taxnumber, companyname, fromDate, toDate, null, null);
            if (objInvoiceBLL.ResultMessageBO.IsError)
            {
                ConfigHelper.Instance.WriteLog(objInvoiceBLL.ResultMessageBO.Message, objInvoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "Search");
                return Json(new { rs = false, msg = objInvoiceBLL.ResultMessageBO.Message });
            }
                string strFileName = "Onfinance.asia.InvoiceList_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss");
                return PushFileInvoice(lstInvoice, strFileName);
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi khi lấy thông tin chi tiết khách hàng", objEx, MethodBase.GetCurrentMethod().Name, "Search Customer");
                return Json(new { success = false, responseText = "Lỗi khi lấy thông tin chi tiết khách hàng" });
            }
        }

        public FileResult PushFileInvoice(List<InvoiceBO> lstInvoice, string fileName)
        {
            var  res = Export2XLSInvoice(lstInvoice);
            return File(res, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
        }
        public static byte[] Export2XLSInvoice(List<InvoiceBO> lstInvoice)
        {
            InvoiceBO invoice = new InvoiceBO();
            InvoiceBLL invoiceBLL = new InvoiceBLL();
            if (lstInvoice.Count > 0)
            {
                invoice = lstInvoice[0];
            }
            // var totalQuantity = 0;
            using (ExcelPackage pck = new ExcelPackage())
            {
                List<string> SheetName = new List<string>() { "Sheet1", "Sheet2" };
                for(int a = 0; a <SheetName.Count; a++)
                {
                    switch(a)
                    {
                        //Write data on Sheet1
                        case 0:
                            {
                                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("HOADON_MASTER");
                                ws.Row(1).Style.Font.Bold = true;
                                ws.Cells["A1:AH1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                ws.Cells["A1:AH1"].Style.WrapText = true;
                                ws.Cells["A1:AH1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells["A1:AH1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);
                                ws.Column(20).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Column(20).Style.Numberformat.Format = "#,##0.00";
                                ws.Column(21).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Column(21).Style.Numberformat.Format = "#,##0.00";
                                ws.Column(22).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Column(22).Style.Numberformat.Format = "#,##0.00";
                                ws.Column(23).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Column(23).Style.Numberformat.Format = "#,##0.00";
                                ws.Column(1).Width = 20;
                                ws.Column(2).Width = 20;
                                ws.Column(3).Width = 20;
                                ws.Column(4).Width = 20;
                                ws.Column(5).Width = 20;
                                ws.Column(6).Width = 20;
                                ws.Column(7).Width = 20;
                                ws.Column(8).Width = 20;
                                ws.Column(9).Width = 20;
                                ws.Column(10).Width = 20;
                                ws.Column(11).Width = 20;
                                ws.Column(12).Width = 30;
                                ws.Column(13).Width = 20;
                                ws.Column(14).Width = 20;
                                ws.Column(15).Width = 20;
                                ws.Column(16).Width = 50;
                                ws.Column(17).Width = 20;
                                ws.Column(18).Width = 20;
                                ws.Column(19).Width = 100;
                                ws.Column(20).Width = 20;
                                ws.Column(21).Width = 20;
                                ws.Column(22).Width = 20;
                                ws.Column(23).Width = 20;
                                ws.Column(24).Width = 20;
                                ws.Column(25).Width = 20;
                                ws.Column(26).Width = 20;
                                ws.Column(27).Width = 20;
                                ws.Column(28).Width = 20;
                                ws.Column(29).Width = 20;
                                ws.Column(30).Width = 20;
                                ws.Column(31).Width = 20;
                                ws.Column(32).Width = 20;
                                ws.Column(33).Width = 20;
                                ws.Column(34).Width = 40;
                                var rowStart = 2;

                                ws.Cells["A1"].Value = "Mã hóa đơn";
                                ws.Cells["B1"].Value = "Mã tra cứu hóa đơn";
                                ws.Cells["C1"].Value = "Loại hóa đơn";
                                ws.Cells["D1"].Value = "Tên hóa đơn";
                                ws.Cells["E1"].Value = "Mẫu số hóa đơn";
                                ws.Cells["F1"].Value = "Kí hiệu hóa đơn";
                                ws.Cells["G1"].Value = "Số hóa đơn";
                                ws.Cells["H1"].Value = "Trạng thái";
                                ws.Cells["I1"].Value = "Ngày tháng năm lập hóa đơn";
                                ws.Cells["J1"].Value = "Hóa đơn đã in chuyển đổi";
                                ws.Cells["K1"].Value = "Thời điểm in chuyển đổi";
                                ws.Cells["L1"].Value = "Đơn vị tiền tệ";
                                ws.Cells["M1"].Value = "Tỷ giá";
                                ws.Cells["N1"].Value = "Tên người bán";
                                ws.Cells["O1"].Value = "Mã số thuế(Người bán)";
                                ws.Cells["P1"].Value = "Địa chỉ";
                                ws.Cells["Q1"].Value = "Tên người mua";
                                ws.Cells["R1"].Value = "Mã số thuế(Người mua)";
                                ws.Cells["S1"].Value = "Địa chỉ";
                                ws.Cells["T1"].Value = "Tổng tiền hàng(chưa có thuế GTGT)";
                                ws.Cells["U1"].Value = "Tổng tiền thuế(Tổng cộng tiền thuế GTGT)";
                                ws.Cells["V1"].Value = "Tổng tiền phí";
                                ws.Cells["W1"].Value = "Tổng tiền thanh toán bằng số";
                                ws.Cells["X1"].Value = "Tổng tiền thanh toán bằng chữ";
                                ws.Cells["Y1"].Value = "Chữ ký số người bán";
                                ws.Cells["Z1"].Value = "Ngày người bán ký";
                                ws.Cells["AA1"].Value = "Chữ ký số người mua";
                                ws.Cells["AB1"].Value = "Ngày người mua ký";
                                ws.Cells["AC1"].Value = "Nhà cung cấp giải pháp phần mềm(MST NCC)";
                                ws.Cells["AD1"].Value = "Mã hiệu phần mềm tạo hóa đơn";
                                ws.Cells["AE1"].Value = "Mã hóa đơn sau xử lý";
                                ws.Cells["AF1"].Value = "Ngày sau xử lý hóa đơn";
                                ws.Cells["AG1"].Value = "Lý do xử lý";
                                ws.Cells["AH1"].Value = "Cổng tra cứu hóa đơn";
                                int i = 1;
                                
                                foreach (InvoiceBO tempInvoice in lstInvoice)
                                {
                                    ws.Cells[string.Format("A{0}", rowStart)].Value = tempInvoice.ID;
                                    ws.Cells[string.Format("B{0}", rowStart)].Value = tempInvoice.REFERENCECODE;
                                    if (tempInvoice.USINGINVOICETYPE == 0)
                                    {
                                        ws.Cells[string.Format("C{0}", rowStart)].Value = "GTGT";
                                    }
                                    else if (tempInvoice.USINGINVOICETYPE == 1)
                                    {
                                        ws.Cells[string.Format("C{0}", rowStart)].Value = "HĐTH";
                                    }
                                    else if (tempInvoice.USINGINVOICETYPE == 2)
                                    {
                                        ws.Cells[string.Format("C{0}", rowStart)].Value = "HĐTĐ";
                                    }
                                    else if (tempInvoice.USINGINVOICETYPE == 3)
                                    {
                                        ws.Cells[string.Format("C{0}", rowStart)].Value = "HĐBH";
                                    }
                                    else if (tempInvoice.USINGINVOICETYPE == 4)
                                    {
                                        ws.Cells[string.Format("C{0}", rowStart)].Value = "HĐTN";
                                    }
                                    else if (tempInvoice.USINGINVOICETYPE == 5)
                                    {
                                        ws.Cells[string.Format("C{0}", rowStart)].Value = "PXK";
                                    }
                                    if (tempInvoice.USINGINVOICETYPE == 0)
                                    {
                                        ws.Cells[string.Format("D{0}", rowStart)].Value = "Hóa đơn GTGT";
                                    }
                                    else if (tempInvoice.USINGINVOICETYPE == 1)
                                    {
                                        ws.Cells[string.Format("D{0}", rowStart)].Value = "Hóa đơn trường học";
                                    }
                                    else if (tempInvoice.USINGINVOICETYPE == 2)
                                    {
                                        ws.Cells[string.Format("D{0}", rowStart)].Value = "Hóa đơn tiền điện";
                                    }
                                    else if (tempInvoice.USINGINVOICETYPE == 3)
                                    {
                                        ws.Cells[string.Format("D{0}", rowStart)].Value = "Hóa đơn bán hàng";
                                    }
                                    else if (tempInvoice.USINGINVOICETYPE == 4)
                                    {
                                        ws.Cells[string.Format("D{0}", rowStart)].Value = "Hóa đơn tiền nước";
                                    }
                                    else if (tempInvoice.USINGINVOICETYPE == 5)
                                    {
                                        ws.Cells[string.Format("D{0}", rowStart)].Value = "Phiếu xuất kho";
                                    }
                                    ws.Cells[string.Format("E{0}", rowStart)].Value = tempInvoice.FORMCODE;
                                    ws.Cells[string.Format("F{0}", rowStart)].Value = tempInvoice.SYMBOLCODE;
                                    ws.Cells[string.Format("G{0}", rowStart)].Value = tempInvoice.NUMBER.ToString("D7");
                                    if (tempInvoice.INVOICESTATUS == 1)
                                    {
                                        ws.Cells[string.Format("H{0}", rowStart)].Value = "Chờ duyệt";
                                    }
                                    else if (tempInvoice.INVOICESTATUS == 2)
                                    {
                                        ws.Cells[string.Format("H{0}", rowStart)].Value = "Đã phát hành";
                                    }
                                    else if (tempInvoice.INVOICESTATUS == 3)
                                    {
                                        ws.Cells[string.Format("H{0}", rowStart)].Value = "Đang ký";
                                    }
                                    ws.Cells[string.Format("I{0}", rowStart)].Value = tempInvoice.INITTIME.Date.ToString("dd/MM/yyyy");
                                    ws.Cells[string.Format("J{0}", rowStart)].Value = "";
                                    ws.Cells[string.Format("K{0}", rowStart)].Value = "";
                                    ws.Cells[string.Format("L{0}", rowStart)].Value = tempInvoice.CURRENCY;
                                    ws.Cells[string.Format("M{0}", rowStart)].Value = tempInvoice.EXCHANGERATE;
                                    ws.Cells[string.Format("N{0}", rowStart)].Value = tempInvoice.COMNAME;
                                    ws.Cells[string.Format("O{0}", rowStart)].Value = tempInvoice.COMTAXCODE;
                                    ws.Cells[string.Format("P{0}", rowStart)].Value = tempInvoice.COMADDRESS;
                                    ws.Cells[string.Format("Q{0}", rowStart)].Value = tempInvoice.CUSNAME;
                                    ws.Cells[string.Format("R{0}", rowStart)].Value = tempInvoice.CUSTAXCODE;
                                    ws.Cells[string.Format("S{0}", rowStart)].Value = tempInvoice.CUSADDRESS;
                                    ws.Cells[string.Format("T{0}", rowStart)].Value = tempInvoice.TOTALMONEY;
                                    ws.Cells[string.Format("U{0}", rowStart)].Value = tempInvoice.TAXMONEY;
                                    ws.Cells[string.Format("V{0}", rowStart)].Value = "";
                                    ws.Cells[string.Format("W{0}", rowStart)].Value = tempInvoice.TOTALPAYMENT;
                                    ws.Cells[string.Format("X{0}", rowStart)].Value = "";
                                    ws.Cells[string.Format("Y{0}", rowStart)].Value = "";
                                    ws.Cells[string.Format("Z{0}", rowStart)].Value = tempInvoice.SIGNEDTIME.Date.ToString("dd/MM/yyyy");
                                    ws.Cells[string.Format("AA{0}", rowStart)].Value = "";
                                    ws.Cells[string.Format("AB{0}", rowStart)].Value = "";
                                    ws.Cells[string.Format("AC{0}", rowStart)].Value = "0106507946";
                                    ws.Cells[string.Format("AD{0}", rowStart)].Value = "ONFINANCE INVOICE";
                                    ws.Cells[string.Format("AE{0}", rowStart)].Value = "";
                                    ws.Cells[string.Format("AF{0}", rowStart)].Value = "";
                                    ws.Cells[string.Format("AG{0}", rowStart)].Value = "";
                                    ws.Cells[string.Format("AH{0}", rowStart)].Value = "https://onfinance.asia/tracuu";
                                   
                                    rowStart++;
                                    i++;
                                }
                                using (ExcelRange rng = ws.Cells[$"A1:O{(rowStart + 1)}"])
                                {
                                    rng.Style.Font.Name = "Times New Roman";
                                }
                                using (ExcelRange rng = ws.Cells[$"A1:AH{(rowStart + 1)}"])
                                {
                                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                break;
                            }
                        //Write data on Sheet1
                        case 1:
                            {
                                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("HOADON_DETAIL");
                                ws.Column(12).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Column(12).Style.Numberformat.Format = "#,##0.00";
                                ws.Column(13).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Column(13).Style.Numberformat.Format = "#,##0.00";
                                ws.Column(15).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Column(15).Style.Numberformat.Format = "#,##0.00";
                                ws.Column(16).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Column(16).Style.Numberformat.Format = "#,##0.00";
                                ws.Column(18).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Column(18).Style.Numberformat.Format = "#,##0.00";
                                ws.Column(19).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Column(19).Style.Numberformat.Format = "#,##0.00";
                                ws.Column(21).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Column(21).Style.Numberformat.Format = "#,##0.00";
                                ws.Row(1).Style.Font.Bold=true;
                                ws.Cells["A1:T1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                ws.Cells["A1:T1"].Style.WrapText = true;
                                ws.Cells["A1:T1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells["A1:T1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);
                                ws.Column(1).Width = 20;
                                ws.Column(2).Width = 30;
                                ws.Column(3).Width = 20;
                                ws.Column(4).Width = 20;
                                ws.Column(5).Width = 20;
                                ws.Column(6).Width = 20;
                                ws.Column(7).Width = 20;
                                ws.Column(8).Width = 20;
                                ws.Column(9).Width = 20;
                                ws.Column(10).Width = 20;
                                ws.Column(11).Width = 20;
                                ws.Column(12).Width = 20;
                                ws.Column(13).Width = 30;
                                ws.Column(14).Width = 20;
                                ws.Column(15).Width = 20;
                                ws.Column(16).Width = 30;
                                ws.Column(17).Width = 20;
                                ws.Column(18).Width = 20;
                                ws.Column(19).Width = 20;
                                ws.Column(20).Width = 20;
                                
                                var rowStart = 2;

                                //ws.Cells["A1"].Value = "STT";
                                ws.Cells["A1"].Value = "Mã hóa đơn";
                                ws.Cells["B1"].Value = "Mã phần mềm hóa đơn ĐT";
                                ws.Cells["C1"].Value = "Loại hóa đơn";
                                ws.Cells["D1"].Value = "Mẫu số hóa đơn";
                                ws.Cells["E1"].Value = "Kí hiệu hóa đơn";
                                ws.Cells["F1"].Value = "Số hóa đơn";
                                ws.Cells["G1"].Value = "Mã số thuế(Người bán)";
                                ws.Cells["H1"].Value = "STT";
                                ws.Cells["I1"].Value = "Tên hàng hóa dịch vụ";
                                ws.Cells["J1"].Value = "Đơn vị tính";
                                ws.Cells["K1"].Value = "Số lượng";
                                ws.Cells["L1"].Value = "Đơn giá";
                                ws.Cells["M1"].Value = "Tỉ lệ % chiết khấu";
                                ws.Cells["N1"].Value = "Số tiền chiết khấu";
                                ws.Cells["O1"].Value = "Thành tiền(Chưa có thuế GTGT)";
                                ws.Cells["P1"].Value = "Thuế suất";
                                ws.Cells["Q1"].Value = "Tiền thuế";
                                ws.Cells["R1"].Value = "Tiền phải thanh toán";
                                ws.Cells["S1"].Value = "Tên loại phí";
                                ws.Cells["T1"].Value = "Tiền phí";
                                int i = 1;
                                // loop through product list to add new line
                                foreach (InvoiceBO tempInvoice in lstInvoice)
                                {
                                    var lstProduct = invoiceBLL.GetInvoiceDetail(tempInvoice.ID);
                                    //ws.Cells[string.Format("A{0}", rowStart)].Value = i;
                                    for (int j = 0 ; j < lstProduct.Count ; j++)
                                    {
                                        var temProduct = lstProduct[j];
                                        var amount = temProduct.RETAILPRICE * temProduct.QUANTITY;
                                        var taxRate = temProduct.TAXRATE == -1 ? 0 : temProduct.TAXRATE;
                                        
                                        ws.Cells[string.Format("A{0}", rowStart)].Value = tempInvoice.ID;
                                        ws.Cells[string.Format("B{0}", rowStart)].Value = "ONFINANCE INVOICE";

                                        if (tempInvoice.USINGINVOICETYPE == 0)
                                        {
                                            ws.Cells[string.Format("C{0}", rowStart)].Value = "Hóa đơn GTGT";
                                        }
                                        else if (tempInvoice.USINGINVOICETYPE == 1)
                                        {
                                            ws.Cells[string.Format("C{0}", rowStart)].Value = "Hóa đơn trường học";
                                        }
                                        else if (tempInvoice.USINGINVOICETYPE == 2)
                                        {
                                            ws.Cells[string.Format("C{0}", rowStart)].Value = "Hóa đơn tiền điện";
                                        }
                                        else if (tempInvoice.USINGINVOICETYPE == 3)
                                        {
                                            ws.Cells[string.Format("C{0}", rowStart)].Value = "Hóa đơn bán hàng";
                                        }
                                        else if (tempInvoice.USINGINVOICETYPE == 4)
                                        {
                                            ws.Cells[string.Format("C{0}", rowStart)].Value = "Hóa đơn tiền nước";
                                        }
                                        else if (tempInvoice.USINGINVOICETYPE == 5)
                                        {
                                            ws.Cells[string.Format("C{0}", rowStart)].Value = "Phiếu xuất kho";
                                        }
                                        ws.Cells[string.Format("D{0}", rowStart)].Value = tempInvoice.FORMCODE;
                                        ws.Cells[string.Format("E{0}", rowStart)].Value = tempInvoice.SYMBOLCODE;
                                        ws.Cells[string.Format("F{0}", rowStart)].Value = tempInvoice.NUMBER.ToString("D7");
                                        ws.Cells[string.Format("G{0}", rowStart)].Value = tempInvoice.CUSTAXCODE;
                                        ws.Cells[string.Format("H{0}", rowStart)].Value = i;
                                        ws.Cells[string.Format("I{0}", rowStart)].Value = temProduct.PRODUCTNAME;
                                        ws.Cells[string.Format("J{0}", rowStart)].Value = temProduct.QUANTITYUNIT;
                                        ws.Cells[string.Format("K{0}", rowStart)].Value = temProduct.QUANTITY;
                                        ws.Cells[string.Format("L{0}", rowStart)].Value = temProduct.RETAILPRICE;
                                        ws.Cells[string.Format("M{0}", rowStart)].Value = tempInvoice.DISCOUNTTYPE;
                                        ws.Cells[string.Format("N{0}", rowStart)].Value = tempInvoice.DISCOUNTMONEY;
                                        ws.Cells[string.Format("O{0}", rowStart)].Value = amount;
                                        ws.Cells[string.Format("P{0}", rowStart)].Value = taxRate;
                                        ws.Cells[string.Format("Q{0}", rowStart)].Value = tempInvoice.TAXMONEY;
                                        ws.Cells[string.Format("R{0}", rowStart)].Value = tempInvoice.TOTALMONEY;
                                        ws.Cells[string.Format("S{0}", rowStart)].Value = "";
                                        ws.Cells[string.Format("T{0}", rowStart)].Value = "";
                                        rowStart++;
                                        i++;
                                    }
                                   
                                }
                                using (ExcelRange rng = ws.Cells[$"A1:T{(rowStart + 1)}"])
                                {
                                    rng.Style.Font.Name = "Times New Roman";
                                    
                                }
                                using (ExcelRange rng = ws.Cells[$"A1:T{(rowStart + 1)}"])
                                {
                                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                break;
                            }
                    }
                }
                return pck.GetAsByteArray();
            }
        }
    }
}