using DS.BusinessLogic;
using DS.BusinessLogic.Customer;
using DS.BusinessLogic.EmailSender;
using DS.BusinessLogic.Invoice;
using DS.BusinessLogic.Meter;
using DS.BusinessObject.Account;
using DS.BusinessObject.EmailSender;
using DS.BusinessObject.Invoice;
using DS.BusinessObject.Output;
using DS.Common;
using DS.Common.Enums;
using DS.Common.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SPA_Invoice.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using static DS.Common.Enums.EnumHelper;

namespace SPA_Invoice.Controllers
{
    public class BaseController : Controller
    {

        protected AccountBO objUser = AccountBO.Current.CurrentUser();
        protected GlobalBLL globalBLL = new GlobalBLL();
        private static readonly object _locker = new object(); // lock object for multithreading
        public BaseController()
        {
        }

        public string ErrorMsg { get; protected set; }

        public string GetFullErrorMsg(string strErrorMsg)
        {
            return string.Format("{0} (cs)", strErrorMsg);
        }

        protected string GetErrorMsgByServer(string strErrorMsg)
        {
            return GetFullErrorMsg(strErrorMsg);
        }

        protected string GetErrorMsgAndITSupport(string strErrorMsg)
        {
            return string.Format("{0}{1}", strErrorMsg, Constants.SUFFIX_IT_SUPPORT);
        }

        public void CalTaskNumber(int totalRow, ref int numberPerTask, ref int taskNumDefault)
        {
            if (totalRow <= taskNumDefault)
            {
                taskNumDefault = 1;
                numberPerTask = totalRow;
            }
            else
            {
                numberPerTask = totalRow / taskNumDefault;
                if (numberPerTask % taskNumDefault != 0)
                    numberPerTask++;
            }
        }

        public void CalTaskNumberInvoice(int totalRow, ref int numberPerTask, ref int taskNumDefault)
        {
            if (totalRow <= taskNumDefault)
            {
                taskNumDefault = 1;
                numberPerTask = totalRow;
            }
            else
            {
                numberPerTask = totalRow / taskNumDefault;
                if (numberPerTask % taskNumDefault != 0)
                    taskNumDefault++;
            }
        }

        public FileResult PushFile(DataTable dt, string fileName)
        {
            return File(GlobalFunction.Export2XLS(dt), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
        }

        public FileResult PushFileCustomizationForInvoice(List<InvoiceBO> lstInvoice, string fileName)
        {
            byte[] res = null;
            if (objUser.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTIENNUOC)
                res = GlobalFunction.Export2XLSInvoice(lstInvoice);
            else
                res = GlobalFunction.Export2XLSCustomizationForInvoice(lstInvoice);

            return File(res, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
        }

        public string RenderViewToString(ControllerContext context, string viewPath, object model = null, bool partial = false)
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
                throw new System.IO.FileNotFoundException("View cannot be found.");
            }

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            context.Controller.ViewData.Model = model;

            string result = null;

            using (var sw = new System.IO.StringWriter())
            {
                var ctx = new ViewContext(context, view, context.Controller.ViewData, context.Controller.TempData, sw);
                view.Render(ctx, sw);
                result = sw.ToString();
            }

            return result.Trim();
        }

        public string RenderViewMultiplePageToString(ControllerContext context, string viewPath, int index, object model = null, bool partial = false)
        {
            try
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
                    throw new System.IO.FileNotFoundException("View cannot be found.");
                }

                // get the view and attach the model to view data
                var view = viewEngineResult.View;
                context.Controller.ViewData.Model = model;
                context.Controller.ViewData["Index"] = index;

                string result = null;

                using (var sw = new System.IO.StringWriter())
                {
                    var ctx = new ViewContext(context, view, context.Controller.ViewData, context.Controller.TempData, sw);
                    view.Render(ctx, sw);
                    result = sw.ToString();
                }

                return result.Trim();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// Tạo file PDF khi người dùng tạo hoặc sửa hóa đơn
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nextNumber"></param>
        /// <returns></returns>
        public byte[] GetInvoiceContent(InvoiceBO invoice, long nextNumber)
        {
            try
            {
                var invoiceBLL = new InvoiceBLL();
                // Tạo mã QR code gán vào thuộc tính QRCODEBASE64 = src hình ảnh bên template view
                invoice.QRCODEBASE64 = QRCodeHelper.CreateQRCode("https://onfinance.asia/tracuu/?referencecode=" + invoice.REFERENCECODE);
                //Lấy detail
                var lstProducts = invoiceBLL.GetInvoiceDetail(invoice.ID);
                // Kiểm tra khuyến mãi nếu có thì gán RETAILPRICE = 0
                foreach (var item in lstProducts)
                {
                    if (item.ISPROMOTION)
                    {
                        item.RETAILPRICE = 0;
                    }
                    item.QUANTITY = Math.Round(item.QUANTITY, 2);
                }
                invoice.LISTPRODUCT = lstProducts;
                //Lấy đường dấn View
                var templatePath = "~" + invoice.TEMPLATEPATH;

                if (string.IsNullOrWhiteSpace(invoice.TEMPLATEPATH))
                {
                    ConfigHelper.Instance.WriteLog($"Không lấy được TEMPLATEPATH. KH: {invoice.COMTAXCODE}.", string.Empty, MethodBase.GetCurrentMethod().Name, "GetInvoiceContent");
                    return null;
                }
                //Convert View to string
                var content = new List<string>();
                content = GetContentMultiplePage(invoice, lstProducts);

                string pdfSize = "A4";
                switch (invoice.USINGINVOICETYPE)
                {
                    case (int)EnumHelper.AccountObjectType.HOADONGTGT:
                    case (int)EnumHelper.AccountObjectType.HOADONTIENDIEN:
                    case (int)EnumHelper.AccountObjectType.HOADONBANHANG:
                        pdfSize = "A4";
                        break;
                    case (int)EnumHelper.AccountObjectType.HOADONTRUONGHOC:
                    case (int)EnumHelper.AccountObjectType.HOADONTIENNUOC:
                        pdfSize = "A5";
                        break;
                }
                //Tạo file pdf từ string
                var dataBuffer = invoiceBLL.BtnCreatePdf(content, null, pdfSize);
                return dataBuffer;
            }
            catch (System.Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy template path", ex, MethodBase.GetCurrentMethod().Name, "UpdateInvoice");
                return null;
            }
        }

        /// <summary>
        /// Tính toán số lượng sản phẩm trên từng page
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="lstProducts"></param>
        /// <returns></returns>

        //public List<string> GetContentMultiplePage(InvoiceBO invoice, List<InvoiceDetailBO> lstProducts)
        //{
        //    //int itemOnPage = 17;
        //    //int itemOnLastPage = 10;

        //    //int totalPage = lstProducts.Count / itemOnPage; 
        //    //if (lstProducts.Count % itemOnPage > 0)
        //    //{

        //    //}

        //    var firstPage = ConfigHelper.FirstPage.Split(';');
        //    var nPage = ConfigHelper.NPage.Split(';');
        //    var lastPage = ConfigHelper.LastPage;
        //    var heightLine = 1.61M;

        //    MeterBLL meterBLL = new MeterBLL();
        //    var templatePath = "~" + invoice.TEMPLATEPATH;
        //    // Áp dụng với hóa đơn nhiều thuế
        //    var lstKCT = lstProducts.Where(x => x.TAXRATE == -1).Count(); // số lượng sản phẩm không chịu thuế
        //    var lstProductsCount = lstProducts.Count(); // số lượng sản phẩm so sánh với lstKCT để hiển thị / thay vì số tiền nếu lstKCT = lstProductsCount
        //    invoice.TOTALQUANTITY = lstProducts.Sum(x => x.QUANTITY); // hóa đơn điện. Tổng sản lượng điện tiêu thụ
        //    List<InvoiceDetailBO> firstData = new List<InvoiceDetailBO>();
        //    List<InvoiceDetailBO> middleData = new List<InvoiceDetailBO>();
        //    List<InvoiceDetailBO> lastData = new List<InvoiceDetailBO>();
        //    Dictionary<int, int> listFirstContentData = new Dictionary<int, int>(); // mảng chứa danh sách số lượng hàng hóa trang đầu tiên (key, value) = (1, số bản ghi hàng hóa)
        //    Dictionary<int, int> listMidContentData = new Dictionary<int, int>(); // mảng chứa danh sách số lượng hàng hóa trang tiếp theo (key, value) = (số thứ tự trang, số bản ghi hàng hóa)
        //    Dictionary<decimal, int> listLastContentData = new Dictionary<decimal, int>(); // mảng chứa danh sách số lượng hàng hóa trang cuối (key, value) = (số thứ tự trang, số bản ghi hàng hóa)
        //    int index = 1;
        //    // Tính số lượng trang
        //    int numberOfCharactersPerRow = invoice.M_ISTAXRATE == 1 ? 70 : 27; // số lượng ký tự trên 1 dòng hóa đơn 1 thuế / nhiều thuế
        //    decimal totalRows = 0; // tổng số hàng của toàn bộ sản phẩm

        //    //remove khoảng trắng để đảm bảo tính toán số dòng chuẩn xác
        //    lstProducts.ForEach(x => x.PRODUCTNAME = x.PRODUCTNAME.Trim());

        //    if (invoice.USINGINVOICETYPE != (int)AccountObjectType.HOADONTIENDIEN)
        //    {
        //        invoice.M_LSTKCT = lstKCT;
        //        invoice.M_LSTPRODUCTSCOUNT = lstProductsCount;
        //        List<string> content = new List<string>(); // mảng byte truyền vào tạo file PDF
        //        decimal pixelPerline = 20;                                  //Số pixel trên mỗi dòng sản phẩm (sản phẩm có n dòng thì nhân với n)
        //        decimal paddingYOneProduct = 20;                            //Khoảng trống phía trên và dưới mỗi sản phẩm
        //        decimal pixelOnePage = 1446 - invoice.HEADERTEMPLATE - 100; //1446: chiều cao toàn trang, 100: chiều cao fotter của mỗi trang phải trừ ra
        //        decimal numCharOnRow = invoice.CHARONROW;                                  //Số ký tự có trên 1 dòng
        //        //decimal numCharOnRow = 40;
        //        int pageIndex = 0;

        //        //tính tổng số pixel
        //        var totalPixel = lstProducts.Sum(p => Math.Ceiling(p.PRODUCTNAME.Length / numCharOnRow) * pixelPerline + paddingYOneProduct);
        //        invoice.M_TOTALPAGES = Math.Ceiling(totalPixel / pixelOnePage);
        //        //Kiểm tra xem số pixel trang cuối (theo lý thuyết) mà lớn hơn số pixel của trang cuối đó thì tách ra làm 2 page, vì trang cuối phải chừa chỗ cho show chữ ký
        //        if (totalPixel - (invoice.M_TOTALPAGES - 1) * pixelOnePage > pixelOnePage - invoice.FOTTERTEMPLATE)
        //            invoice.M_TOTALPAGES++;

        //        if (invoice.M_TOTALPAGES > 1)
        //            invoice.M_ISMULTIPLEPAGE = true;

        //        decimal tempPagePixel = 0;
        //        invoice.LISTPRODUCT = new List<InvoiceDetailBO>();
        //        for (var i = 0; i < lstProducts.Count; i++)
        //        {
        //            invoice.LISTPRODUCT.Add(lstProducts[i]);
        //            tempPagePixel += Math.Ceiling(lstProducts[i].PRODUCTNAME.Length / numCharOnRow) * pixelPerline + paddingYOneProduct;

        //            //Nếu số pixel đã tính cộng với số pixel sản phẩm tiếp theo mà lớn hơn số pixel cho phép của trang thì phải tách trang
        //            if (((i < lstProducts.Count - 1) && (tempPagePixel + Math.Ceiling(lstProducts[i].PRODUCTNAME.Length / numCharOnRow) * pixelPerline + paddingYOneProduct) >= pixelOnePage) || i == lstProducts.Count - 1)
        //            {
        //                pageIndex++;
        //                invoice.M_INDEX = i - invoice.LISTPRODUCT.Count + 2;
        //                var midContent = RenderViewMultiplePageToString(ControllerContext, templatePath, pageIndex, invoice, true);
        //                content.Add(midContent);

        //                //Clear
        //                invoice.LISTPRODUCT = new List<InvoiceDetailBO>();
        //                tempPagePixel = 0;
        //            }
        //        }

        //        //Nếu số lượng sản phẩm vừa đủ fill hết các trang => trang cuối chưa được tạo trong vòng for ở trên, cần tạo ra trang cuối
        //        if (pageIndex < invoice.M_TOTALPAGES)
        //        {
        //            var endContent = RenderViewMultiplePageToString(ControllerContext, templatePath, pageIndex + 1, invoice, true);
        //            content.Add(endContent);
        //        }

        //        return content;
        //    }
        //    else
        //    {
        //        lstProducts = lstProducts.OrderBy(x => x.METERCODE).ToList();
        //        for (var i = 0; i < lstProducts.Count; i++)
        //        {
        //            var item = lstProducts[i];
        //            if (item.PRODUCTNAME.Length < numberOfCharactersPerRow)
        //            {
        //                totalRows += heightLine; // 3.7 / 2.3
        //            }
        //            else
        //            {
        //                var tempRows = (decimal)(item.PRODUCTNAME.Length) / numberOfCharactersPerRow;
        //                if (tempRows.ToString().Contains("."))
        //                {
        //                    tempRows = int.Parse(tempRows.ToString().Substring(0, tempRows.ToString().IndexOf("."))) + 1;
        //                }
        //                totalRows += tempRows;
        //            }
        //        }

        //        // kiểm tra hóa đơn tiền điện thì tính thêm dòng tên công tơ điện
        //        decimal electricExtraRows = 0;
        //        var electricMeterList = meterBLL.GetListMeterCodeByInvoiceID(invoice.ID);
        //        electricExtraRows = (meterBLL.GetListMeterCodeByInvoiceID(invoice.ID) == null ? 0 : meterBLL.GetListMeterCodeByInvoiceID(invoice.ID).Count) * heightLine;
        //        totalRows = totalRows + electricExtraRows;
        //        List<string> content = new List<string>(); // mảng byte truyền vào tạo file PDF
        //                                                   // Kiểm tra tổng số dòng sản phẩm. Nếu lớn hơn 18 => Có 2 trang hóa đơn trở lên. Ngược lại là hóa đơn 1 trang
        //        decimal tempTotalRows = 0;
        //        if (totalRows / 18 < 1) // tổng hàng > số lượng dòng tối đa của trang đầu tiên thì thêm vào danh sách sản phẩm trang đầu tiên
        //        {
        //            invoice.M_ISMULTIPLEPAGE = false; // biến cờ xác định nhiều trang
        //            invoice.M_INDEX = 1;
        //            var contentMain = RenderViewMultiplePageToString(ControllerContext, templatePath, 1, invoice, true);
        //            content.Add(contentMain);
        //        }
        //        else
        //        {
        //            invoice.M_ISMULTIPLEPAGE = true; // biến cờ xác định nhiều trang
        //            List<string> meterCodeList = new List<string>();
        //            var extraRows = 0M;
        //            for (int i = 0; i < lstProducts.Count; i++)
        //            {
        //                if (meterCodeList.Where(x => x == lstProducts[i].METERCODE).Count() == 0)
        //                {
        //                    meterCodeList.Add(lstProducts[i].METERCODE);
        //                    extraRows += heightLine;
        //                }


        //                if (lstProducts[i].PRODUCTNAME.Length < numberOfCharactersPerRow)
        //                {
        //                    tempTotalRows += heightLine;
        //                }
        //                else
        //                {
        //                    var tempRows = (decimal)(lstProducts[i].PRODUCTNAME.Length) / numberOfCharactersPerRow;
        //                    if (tempRows.ToString().Contains("."))
        //                    {
        //                        tempRows = int.Parse(tempRows.ToString().Substring(0, tempRows.ToString().IndexOf("."))) + 1;
        //                    }
        //                    tempTotalRows += tempRows;
        //                }

        //                //if (tempTotalRows >= (objUser.USINGINVOICETYPE == (int)AccountObjectType.HOADONTIENDIEN ? (18 - electricExtraRows) : 18) && tempTotalRows <= (objUser.USINGINVOICETYPE == (int)AccountObjectType.HOADONTIENDIEN ? (30 - electricExtraRows) : (invoice.M_ISTAXRATE == 1 ? int.Parse(firstPage[0]) : int.Parse(firstPage[1]))))
        //                if (tempTotalRows >= (objUser.USINGINVOICETYPE == (int)AccountObjectType.HOADONTIENDIEN ? (18 - extraRows) : 18) && tempTotalRows <= (objUser.USINGINVOICETYPE == (int)AccountObjectType.HOADONTIENDIEN ? (30 - extraRows) : (invoice.M_ISTAXRATE == 1 ? int.Parse(firstPage[0]) : int.Parse(firstPage[1]))))
        //                {
        //                    if (listFirstContentData.ContainsKey(index))
        //                    {
        //                        listFirstContentData[index] = i;
        //                    }
        //                    else
        //                    {
        //                        listFirstContentData.Add(index, i);
        //                    }
        //                }
        //            }
        //            index = 2;
        //            firstData = lstProducts.Take(listFirstContentData.First().Value + 1).ToList();
        //            // remove first page data => keep remain to calculate
        //            lstProducts.RemoveRange(0, listFirstContentData.First().Value + 1);
        //            if (lstProducts.Count > 0)
        //            {
        //                //index = 2;
        //                tempTotalRows = 0;
        //                for (int i = 0; i < lstProducts.Count; i++)
        //                {
        //                    if (lstProducts[i].PRODUCTNAME.Length < numberOfCharactersPerRow)
        //                    {
        //                        tempTotalRows += heightLine;
        //                    }
        //                    else
        //                    {
        //                        var tempRows = (decimal)(lstProducts[i].PRODUCTNAME.Length) / numberOfCharactersPerRow;
        //                        if (tempRows.ToString().Contains("."))
        //                        {
        //                            tempRows = int.Parse(tempRows.ToString().Substring(0, tempRows.ToString().IndexOf("."))) + 1;
        //                        }
        //                        tempTotalRows += tempRows;
        //                    }
        //                    if (tempTotalRows >= (invoice.M_ISTAXRATE == 1 ? int.Parse(nPage[0]) : int.Parse(nPage[1])))
        //                    {
        //                        tempTotalRows = 0;
        //                        listMidContentData.Add(index, i);
        //                        index++;
        //                        continue;
        //                    }
        //                }
        //            }
        //            middleData = lstProducts.Take(listMidContentData.Count > 0 ? listMidContentData.Last().Value + 1 : 0).ToList();
        //            // remove middle page data => keep remain to calculate
        //            lstProducts.RemoveRange(0, listMidContentData.Count > 0 ? listMidContentData.Last().Value + 1 : 0);
        //            if (lstProducts.Count > 0)
        //            {
        //                tempTotalRows = 0;
        //                for (int i = 0; i < lstProducts.Count; i++)
        //                {
        //                    if (lstProducts[i].PRODUCTNAME.Length < numberOfCharactersPerRow)
        //                    {
        //                        tempTotalRows += heightLine;
        //                    }
        //                    else
        //                    {
        //                        var tempRows = (decimal)(lstProducts[i].PRODUCTNAME.Length) / numberOfCharactersPerRow;
        //                        if (tempRows.ToString().Contains("."))
        //                        {
        //                            tempRows = int.Parse(tempRows.ToString().Substring(0, tempRows.ToString().IndexOf("."))) + 1;
        //                        }
        //                        tempTotalRows += tempRows;
        //                    }
        //                    if (tempTotalRows > int.Parse(lastPage))
        //                    {
        //                        middleData.AddRange(lstProducts);
        //                        if (listMidContentData.Count == 0)
        //                        {
        //                            listMidContentData.Add(index, firstData.Count + 1);
        //                        }
        //                        else
        //                        {
        //                            listMidContentData.Add(index, middleData.Count + lstProducts.Count);
        //                        }
        //                        lstProducts = new List<InvoiceDetailBO>();
        //                        index++;
        //                        break;
        //                    }
        //                }
        //            }
        //            //lastData = lstProducts;
        //            lastData = lstProducts;
        //            invoice.M_TOTALPAGES = index;
        //            // bind data to content
        //            if (firstData != null && listFirstContentData != null) // first page
        //            {
        //                invoice.LISTPRODUCT = firstData;
        //                invoice.M_INDEX = 1;
        //                var contentMain = RenderViewMultiplePageToString(ControllerContext, templatePath, 1, invoice, true);
        //                content.Add(contentMain);
        //            }

        //            if (middleData != null && listMidContentData != null) // the next page but not last page
        //            {
        //                for (int i = 0; i < listMidContentData.Count; i++)
        //                {
        //                    //invoice.M_INDEX = listMidContentData.Values.ElementAt(i);
        //                    if ((i + 2) == 2)
        //                    {
        //                        invoice.M_INDEX = firstData.Count + 1;
        //                        invoice.LISTPRODUCT = (from k in middleData select k).Take(listMidContentData[i + 2] + 1).ToList();
        //                    }
        //                    else
        //                    {
        //                        invoice.LISTPRODUCT = (from k in middleData select k).Skip(listMidContentData[i + 1] + 1).Take(listMidContentData[i + 2] - listMidContentData[i + 1]).ToList();
        //                    }
        //                    var midContent = RenderViewMultiplePageToString(ControllerContext, templatePath, i + 2, invoice, true);
        //                    content.Add(midContent);
        //                }
        //            }

        //            // last page
        //            invoice.M_LSTKCT = lstKCT;
        //            invoice.M_LSTPRODUCTSCOUNT = lstProductsCount;
        //            invoice.LISTPRODUCT = lastData;
        //            invoice.M_INDEX = lstProductsCount - lastData.Count() + 1;
        //            var lastContent = RenderViewMultiplePageToString(ControllerContext, templatePath, (int)index, invoice, true);
        //            content.Add(lastContent);
        //        }
        //        return content;
        //    }
        //}

        public List<string> GetContentMultiplePage(InvoiceBO invoice, List<InvoiceDetailBO> lstProducts)
        {
            //int itemOnPage = 17;
            //int itemOnLastPage = 10;

            //int totalPage = lstProducts.Count / itemOnPage; 
            //if (lstProducts.Count % itemOnPage > 0)
            //{

            //}

            var firstPage = ConfigHelper.FirstPage.Split(';');
            var nPage = ConfigHelper.NPage.Split(';');
            var lastPage = ConfigHelper.LastPage;
            var heightLine = 1.61M;

            MeterBLL meterBLL = new MeterBLL();
            var templatePath = "~" + invoice.TEMPLATEPATH;
            // Áp dụng với hóa đơn nhiều thuế
            var lstKCT = lstProducts.Where(x => x.TAXRATE == -1).Count(); // số lượng sản phẩm không chịu thuế
            var lstProductsCount = lstProducts.Count(); // số lượng sản phẩm so sánh với lstKCT để hiển thị / thay vì số tiền nếu lstKCT = lstProductsCount
            invoice.TOTALQUANTITY = lstProducts.Sum(x => x.QUANTITY); // hóa đơn điện. Tổng sản lượng điện tiêu thụ
            List<InvoiceDetailBO> firstData = new List<InvoiceDetailBO>();
            List<InvoiceDetailBO> middleData = new List<InvoiceDetailBO>();
            List<InvoiceDetailBO> lastData = new List<InvoiceDetailBO>();
            Dictionary<int, int> listFirstContentData = new Dictionary<int, int>(); // mảng chứa danh sách số lượng hàng hóa trang đầu tiên (key, value) = (1, số bản ghi hàng hóa)
            Dictionary<int, int> listMidContentData = new Dictionary<int, int>(); // mảng chứa danh sách số lượng hàng hóa trang tiếp theo (key, value) = (số thứ tự trang, số bản ghi hàng hóa)
            Dictionary<decimal, int> listLastContentData = new Dictionary<decimal, int>(); // mảng chứa danh sách số lượng hàng hóa trang cuối (key, value) = (số thứ tự trang, số bản ghi hàng hóa)
            int index = 1;
            // Tính số lượng trang
            int numberOfCharactersPerRow = invoice.M_ISTAXRATE == 1 ? 70 : 27; // số lượng ký tự trên 1 dòng hóa đơn 1 thuế / nhiều thuế
            decimal totalRows = 0; // tổng số hàng của toàn bộ sản phẩm

            //remove khoảng trắng để đảm bảo tính toán số dòng chuẩn xác
            lstProducts.ForEach(x => x.PRODUCTNAME = x.PRODUCTNAME.Trim());

            if (invoice.USINGINVOICETYPE != (int)AccountObjectType.HOADONTIENDIEN)
            {
                invoice.M_LSTKCT = lstKCT;
                invoice.M_LSTPRODUCTSCOUNT = lstProductsCount;
                List<string> content = new List<string>(); // mảng byte truyền vào tạo file PDF
                decimal pixelPerline = 20;                                  //Số pixel trên mỗi dòng sản phẩm (sản phẩm có n dòng thì nhân với n)
                decimal paddingYOneProduct = 20;                            //Khoảng trống phía trên và dưới mỗi sản phẩm
                decimal pixelOnePage = 1446 - invoice.HEADERTEMPLATE - 100; //1446: chiều cao toàn trang, 100: chiều cao fotter của mỗi trang phải trừ ra
                decimal numCharOnRow = invoice.CHARONROW;                                  //Số ký tự có trên 1 dòng
                //decimal numCharOnRow = 40;
                int pageIndex = 0;
                bool isFixRecordPerPage = invoice.RECORDPERPAGE > 0;

                //tính tổng số pixel
                if (isFixRecordPerPage)
                {
                    invoice.M_TOTALPAGES = lstProducts.Count / invoice.RECORDPERPAGE;
                    //Kiểm tra xem số pixel trang cuối (theo lý thuyết) mà lớn hơn số pixel của trang cuối đó thì tách ra làm 2 page, vì trang cuối phải chừa chỗ cho show chữ ký
                    if (lstProducts.Count % invoice.RECORDPERPAGE != 0)
                        invoice.M_TOTALPAGES++;
                }
                else
                {
                    var totalPixel = lstProducts.Sum(p => Math.Ceiling(p.PRODUCTNAME.Length / numCharOnRow) * pixelPerline + paddingYOneProduct);
                    invoice.M_TOTALPAGES = Math.Ceiling(totalPixel / pixelOnePage);

                    if (totalPixel - (invoice.M_TOTALPAGES - 1) * pixelOnePage > pixelOnePage - invoice.FOTTERTEMPLATE)
                        invoice.M_TOTALPAGES++;
                }

                if (invoice.M_TOTALPAGES > 1)
                    invoice.M_ISMULTIPLEPAGE = true;
                else
                    invoice.M_INDEX = 1;

                decimal tempPagePixel = 0;
                invoice.LISTPRODUCT = new List<InvoiceDetailBO>();
                for (var i = 0; i < lstProducts.Count; i++)
                {
                    invoice.LISTPRODUCT.Add(lstProducts[i]);
                    tempPagePixel += Math.Ceiling(lstProducts[i].PRODUCTNAME.Length / numCharOnRow) * pixelPerline + paddingYOneProduct;

                    //Nếu số pixel đã tính cộng với số pixel sản phẩm tiếp theo mà lớn hơn số pixel cho phép của trang thì phải tách trang
                    if (isFixRecordPerPage)
                    {
                        if (invoice.LISTPRODUCT.Count > invoice.RECORDPERPAGE - 1)
                        {
                            pageIndex++;
                            invoice.M_INDEX = i - invoice.LISTPRODUCT.Count + 2;
                            var midContent = RenderViewMultiplePageToString(ControllerContext, templatePath, pageIndex, invoice, true);
                            content.Add(midContent);

                            //Clear
                            invoice.LISTPRODUCT = new List<InvoiceDetailBO>();
                            tempPagePixel = 0;
                        }
                    }
                    else
                    {
                        if (((i < lstProducts.Count - 1) && (tempPagePixel + Math.Ceiling(lstProducts[i].PRODUCTNAME.Length / numCharOnRow) * pixelPerline + paddingYOneProduct) >= pixelOnePage) || i == lstProducts.Count - 1)
                        {
                            pageIndex++;
                            invoice.M_INDEX = i - invoice.LISTPRODUCT.Count + 2;
                            var midContent = RenderViewMultiplePageToString(ControllerContext, templatePath, pageIndex, invoice, true);
                            content.Add(midContent);

                            //Clear
                            invoice.LISTPRODUCT = new List<InvoiceDetailBO>();
                            tempPagePixel = 0;
                        }
                    }
                }

                //Nếu số lượng sản phẩm vừa đủ fill hết các trang => trang cuối chưa được tạo trong vòng for ở trên, cần tạo ra trang cuối
                if (pageIndex < invoice.M_TOTALPAGES)
                {
                    if (isFixRecordPerPage)
                        invoice.M_INDEX = pageIndex * invoice.RECORDPERPAGE + 1;
                    var endContent = RenderViewMultiplePageToString(ControllerContext, templatePath, pageIndex + 1, invoice, true);
                    content.Add(endContent);
                }

                return content;
            }
            else
            {
                lstProducts = lstProducts.OrderBy(x => x.METERCODE).ToList();
                for (var i = 0; i < lstProducts.Count; i++)
                {
                    var item = lstProducts[i];
                    if (item.PRODUCTNAME.Length < numberOfCharactersPerRow)
                    {
                        totalRows += heightLine; // 3.7 / 2.3
                    }
                    else
                    {
                        var tempRows = (decimal)(item.PRODUCTNAME.Length) / numberOfCharactersPerRow;
                        if (tempRows.ToString().Contains("."))
                        {
                            tempRows = int.Parse(tempRows.ToString().Substring(0, tempRows.ToString().IndexOf("."))) + 1;
                        }
                        totalRows += tempRows;
                    }
                }

                // kiểm tra hóa đơn tiền điện thì tính thêm dòng tên công tơ điện
                decimal electricExtraRows = 0;
                var electricMeterList = meterBLL.GetListMeterCodeByInvoiceID(invoice.ID);
                electricExtraRows = (meterBLL.GetListMeterCodeByInvoiceID(invoice.ID) == null ? 0 : meterBLL.GetListMeterCodeByInvoiceID(invoice.ID).Count) * heightLine;
                totalRows = totalRows + electricExtraRows;
                List<string> content = new List<string>(); // mảng byte truyền vào tạo file PDF
                                                           // Kiểm tra tổng số dòng sản phẩm. Nếu lớn hơn 18 => Có 2 trang hóa đơn trở lên. Ngược lại là hóa đơn 1 trang
                decimal tempTotalRows = 0;
                if (totalRows / 18 < 1) // tổng hàng > số lượng dòng tối đa của trang đầu tiên thì thêm vào danh sách sản phẩm trang đầu tiên
                {
                    invoice.M_ISMULTIPLEPAGE = false; // biến cờ xác định nhiều trang
                    invoice.M_INDEX = 1;
                    var contentMain = RenderViewMultiplePageToString(ControllerContext, templatePath, 1, invoice, true);
                    content.Add(contentMain);
                }
                else
                {
                    invoice.M_ISMULTIPLEPAGE = true; // biến cờ xác định nhiều trang
                    List<string> meterCodeList = new List<string>();
                    var extraRows = 0M;
                    for (int i = 0; i < lstProducts.Count; i++)
                    {
                        if (meterCodeList.Where(x => x == lstProducts[i].METERCODE).Count() == 0)
                        {
                            meterCodeList.Add(lstProducts[i].METERCODE);
                            extraRows += heightLine;
                        }


                        if (lstProducts[i].PRODUCTNAME.Length < numberOfCharactersPerRow)
                        {
                            tempTotalRows += heightLine;
                        }
                        else
                        {
                            var tempRows = (decimal)(lstProducts[i].PRODUCTNAME.Length) / numberOfCharactersPerRow;
                            if (tempRows.ToString().Contains("."))
                            {
                                tempRows = int.Parse(tempRows.ToString().Substring(0, tempRows.ToString().IndexOf("."))) + 1;
                            }
                            tempTotalRows += tempRows;
                        }

                        //if (tempTotalRows >= (objUser.USINGINVOICETYPE == (int)AccountObjectType.HOADONTIENDIEN ? (18 - electricExtraRows) : 18) && tempTotalRows <= (objUser.USINGINVOICETYPE == (int)AccountObjectType.HOADONTIENDIEN ? (30 - electricExtraRows) : (invoice.M_ISTAXRATE == 1 ? int.Parse(firstPage[0]) : int.Parse(firstPage[1]))))
                        if (tempTotalRows >= (objUser.USINGINVOICETYPE == (int)AccountObjectType.HOADONTIENDIEN ? (18 - extraRows) : 18) && tempTotalRows <= (objUser.USINGINVOICETYPE == (int)AccountObjectType.HOADONTIENDIEN ? (30 - extraRows) : (invoice.M_ISTAXRATE == 1 ? int.Parse(firstPage[0]) : int.Parse(firstPage[1]))))
                        {
                            if (listFirstContentData.ContainsKey(index))
                            {
                                listFirstContentData[index] = i;
                            }
                            else
                            {
                                listFirstContentData.Add(index, i);
                            }
                        }
                    }
                    index = 2;
                    firstData = lstProducts.Take(listFirstContentData.First().Value + 1).ToList();
                    // remove first page data => keep remain to calculate
                    lstProducts.RemoveRange(0, listFirstContentData.First().Value + 1);
                    if (lstProducts.Count > 0)
                    {
                        //index = 2;
                        tempTotalRows = 0;
                        for (int i = 0; i < lstProducts.Count; i++)
                        {
                            if (lstProducts[i].PRODUCTNAME.Length < numberOfCharactersPerRow)
                            {
                                tempTotalRows += heightLine;
                            }
                            else
                            {
                                var tempRows = (decimal)(lstProducts[i].PRODUCTNAME.Length) / numberOfCharactersPerRow;
                                if (tempRows.ToString().Contains("."))
                                {
                                    tempRows = int.Parse(tempRows.ToString().Substring(0, tempRows.ToString().IndexOf("."))) + 1;
                                }
                                tempTotalRows += tempRows;
                            }
                            if (tempTotalRows >= (invoice.M_ISTAXRATE == 1 ? int.Parse(nPage[0]) : int.Parse(nPage[1])))
                            {
                                tempTotalRows = 0;
                                listMidContentData.Add(index, i);
                                index++;
                                continue;
                            }
                        }
                    }
                    middleData = lstProducts.Take(listMidContentData.Count > 0 ? listMidContentData.Last().Value + 1 : 0).ToList();
                    // remove middle page data => keep remain to calculate
                    lstProducts.RemoveRange(0, listMidContentData.Count > 0 ? listMidContentData.Last().Value + 1 : 0);
                    if (lstProducts.Count > 0)
                    {
                        tempTotalRows = 0;
                        for (int i = 0; i < lstProducts.Count; i++)
                        {
                            if (lstProducts[i].PRODUCTNAME.Length < numberOfCharactersPerRow)
                            {
                                tempTotalRows += heightLine;
                            }
                            else
                            {
                                var tempRows = (decimal)(lstProducts[i].PRODUCTNAME.Length) / numberOfCharactersPerRow;
                                if (tempRows.ToString().Contains("."))
                                {
                                    tempRows = int.Parse(tempRows.ToString().Substring(0, tempRows.ToString().IndexOf("."))) + 1;
                                }
                                tempTotalRows += tempRows;
                            }
                            if (tempTotalRows > int.Parse(lastPage))
                            {
                                middleData.AddRange(lstProducts);
                                if (listMidContentData.Count == 0)
                                {
                                    listMidContentData.Add(index, firstData.Count + 1);
                                }
                                else
                                {
                                    listMidContentData.Add(index, middleData.Count + lstProducts.Count);
                                }
                                lstProducts = new List<InvoiceDetailBO>();
                                index++;
                                break;
                            }
                        }
                    }
                    //lastData = lstProducts;
                    lastData = lstProducts;
                    invoice.M_TOTALPAGES = index;
                    // bind data to content
                    if (firstData != null && listFirstContentData != null) // first page
                    {
                        invoice.LISTPRODUCT = firstData;
                        invoice.M_INDEX = 1;
                        var contentMain = RenderViewMultiplePageToString(ControllerContext, templatePath, 1, invoice, true);
                        content.Add(contentMain);
                    }

                    if (middleData != null && listMidContentData != null) // the next page but not last page
                    {
                        for (int i = 0; i < listMidContentData.Count; i++)
                        {
                            //invoice.M_INDEX = listMidContentData.Values.ElementAt(i);
                            if ((i + 2) == 2)
                            {
                                invoice.M_INDEX = firstData.Count + 1;
                                invoice.LISTPRODUCT = (from k in middleData select k).Take(listMidContentData[i + 2] + 1).ToList();
                            }
                            else
                            {
                                invoice.LISTPRODUCT = (from k in middleData select k).Skip(listMidContentData[i + 1] + 1).Take(listMidContentData[i + 2] - listMidContentData[i + 1]).ToList();
                            }
                            var midContent = RenderViewMultiplePageToString(ControllerContext, templatePath, i + 2, invoice, true);
                            content.Add(midContent);
                        }
                    }

                    // last page
                    invoice.M_LSTKCT = lstKCT;
                    invoice.M_LSTPRODUCTSCOUNT = lstProductsCount;
                    invoice.LISTPRODUCT = lastData;
                    invoice.M_INDEX = lstProductsCount - lastData.Count() + 1;
                    var lastContent = RenderViewMultiplePageToString(ControllerContext, templatePath, (int)index, invoice, true);
                    content.Add(lastContent);
                }
                return content;
            }
        }
        //public List<string> GetContentMultiplePage(InvoiceBO invoice, List<InvoiceDetailBO> lstProducts)
        //{
        //    var firstPage = ConfigHelper.FirstPage.Split(';');
        //    var nPage = ConfigHelper.NPage.Split(';');
        //    var lastPage = ConfigHelper.LastPage;

        //    MeterBLL meterBLL = new MeterBLL();
        //    var templatePath = "~" + invoice.TEMPLATEPATH;
        //    // Áp dụng với hóa đơn nhiều thuế
        //    // Tính tiền tổng cho trang cuối hóa đơn nhiều trang
        //    decimal totalMoneyWithoutTax = 0; // tổng tiền toàn hóa đơn trước thuế
        //    decimal totalDiscount = 0; // tổng tiền chiết khấu
        //    decimal totalVatAmount = 0; // tổng tiền thuế
        //    decimal totalMoney = 0; // tổng tiền thanh toán
        //    var lstKCT = lstProducts.Where(x => x.TAXRATE == -1).Count(); // số lượng sản phẩm không chịu thuế
        //    var lstProductsCount = lstProducts.Count(); // số lượng sản phẩm so sánh với lstKCT để hiển thị / thay vì số tiền nếu lstKCT = lstProductsCount
        //    invoice.TOTALQUANTITY = lstProducts.Sum(x => x.QUANTITY);
        //    List<InvoiceDetailBO> firstData = new List<InvoiceDetailBO>();
        //    List<InvoiceDetailBO> middleData = new List<InvoiceDetailBO>();
        //    List<InvoiceDetailBO> lastData = new List<InvoiceDetailBO>();
        //    Dictionary<int, int> listFirstContentData = new Dictionary<int, int>(); // mảng chứa danh sách số lượng hàng hóa trang đầu tiên (key, value) = (1, số bản ghi hàng hóa)
        //    Dictionary<int, int> listMidContentData = new Dictionary<int, int>(); // mảng chứa danh sách số lượng hàng hóa trang tiếp theo (key, value) = (số thứ tự trang, số bản ghi hàng hóa)
        //    Dictionary<decimal, int> listLastContentData = new Dictionary<decimal, int>(); // mảng chứa danh sách số lượng hàng hóa trang cuối (key, value) = (số thứ tự trang, số bản ghi hàng hóa)
        //    int index = 1;
        //    // Tính số lượng trang
        //    int numberOfCharactersPerRow = invoice.M_ISTAXRATE == 1 ? 70 : 27; // số lượng ký tự trên 1 dòng hóa đơn 1 thuế / nhiều thuế
        //    decimal totalRows = 0; // tổng số hàng của toàn bộ sản phẩm
        //    for (var i = 0; i < lstProducts.Count; i++)
        //    {
        //        var item = lstProducts[i];
        //        if (item.PRODUCTNAME.Length < numberOfCharactersPerRow)
        //        {
        //            totalRows += 1.61M; // 3.7 / 2.3
        //        }
        //        else
        //        {
        //            var tempRows = (decimal)(item.PRODUCTNAME.Length) / numberOfCharactersPerRow;
        //            if (tempRows.ToString().Contains("."))
        //            {
        //                tempRows = int.Parse(tempRows.ToString().Substring(0, tempRows.ToString().IndexOf("."))) + 1;
        //            }
        //            totalRows += tempRows;
        //        }
        //        // Tính tiền tổng cho trang cuối hóa đơn nhiều trang
        //        var retailPrice = item.RETAILPRICE;
        //        var amountWihtoutVAT = (item.RETAILPRICE * item.QUANTITY);
        //        decimal discountAmount = 0;
        //        if (item.DISCOUNTRATE != 0)
        //        {
        //            discountAmount = amountWihtoutVAT * (((decimal)item.DISCOUNTRATE) / 100);
        //            amountWihtoutVAT = amountWihtoutVAT - discountAmount;
        //        }
        //        totalMoneyWithoutTax += (item.RETAILPRICE * item.QUANTITY);
        //        totalDiscount += discountAmount;
        //        totalVatAmount += (amountWihtoutVAT * ((decimal)(item.TAXRATE == -1 ? 0 : item.TAXRATE) / 100));
        //    }

        //    // kiểm tra hóa đơn tiền điện thì tính thêm dòng tên công tơ điện
        //    decimal electricExtraRows = 0;
        //    if (objUser.USINGINVOICETYPE == (int)AccountObjectType.HOADONTIENDIEN)
        //    {
        //        var electricMeterList = meterBLL.GetListMeterCodeByInvoiceID(invoice.ID);
        //        electricExtraRows = (meterBLL.GetListMeterCodeByInvoiceID(invoice.ID) == null ? 0 : meterBLL.GetListMeterCodeByInvoiceID(invoice.ID).Count) * 1.5M;
        //        totalRows = totalRows + electricExtraRows;
        //    }
        //    if (objUser.USINGINVOICETYPE == (int)AccountObjectType.HOADONBANHANG)
        //    {
        //        totalMoney = totalMoneyWithoutTax - totalDiscount;
        //    }
        //    else
        //    {
        //        totalMoney = (totalMoneyWithoutTax + totalVatAmount) - totalDiscount;
        //    }
        //    List<string> content = new List<string>(); // mảng byte truyền vào tạo file PDF
        //                                               // Kiểm tra tổng số dòng sản phẩm. Nếu lớn hơn 18 => Có 2 trang hóa đơn trở lên. Ngược lại là hóa đơn 1 trang
        //    decimal tempTotalRows = 0;
        //    if (totalRows / 18 < 1) // tổng hàng > số lượng dòng tối đa của trang đầu tiên thì thêm vào danh sách sản phẩm trang đầu tiên
        //    {
        //        invoice.M_ISMULTIPLEPAGE = false; // biến cờ xác định nhiều trang
        //        invoice.M_INDEX = 1;
        //        var contentMain = RenderViewMultiplePageToString(ControllerContext, templatePath, 1, invoice, true);
        //        content.Add(contentMain);
        //    }
        //    else
        //    {
        //        invoice.M_ISMULTIPLEPAGE = true; // biến cờ xác định nhiều trang
        //        for (int i = 0; i < lstProducts.Count; i++)
        //        {
        //            if (lstProducts[i].PRODUCTNAME.Length < numberOfCharactersPerRow)
        //            {
        //                tempTotalRows += 1.61M;
        //            }
        //            else
        //            {
        //                var tempRows = (decimal)(lstProducts[i].PRODUCTNAME.Length) / numberOfCharactersPerRow;
        //                if (tempRows.ToString().Contains("."))
        //                {
        //                    tempRows = int.Parse(tempRows.ToString().Substring(0, tempRows.ToString().IndexOf("."))) + 1;
        //                }
        //                tempTotalRows += tempRows;
        //            }
        //            //if (tempTotalRows >= 18 && tempTotalRows <= (invoice.M_ISTAXRATE == 1 ? 30 : 36))
        //            //if (tempTotalRows >= (objUser.USINGINVOICETYPE == 2 ? 15 : 18) && tempTotalRows <= (objUser.USINGINVOICETYPE == 2 ? 25 : (invoice.M_ISTAXRATE == 1 ? 30 : 36)))
        //            //if (tempTotalRows >= (objUser.USINGINVOICETYPE == (int)AccountObjectType.HOADONTIENDIEN ? (18 - electricExtraRows) : 18) && tempTotalRows <= (objUser.USINGINVOICETYPE == (int)AccountObjectType.HOADONTIENDIEN ? (30 - electricExtraRows) : (invoice.M_ISTAXRATE == 1 ? 30 : 36)))
        //            if (tempTotalRows >= (objUser.USINGINVOICETYPE == (int)AccountObjectType.HOADONTIENDIEN ? (18 - electricExtraRows) : 18) && tempTotalRows <= (objUser.USINGINVOICETYPE == (int)AccountObjectType.HOADONTIENDIEN ? (30 - electricExtraRows) : (invoice.M_ISTAXRATE == 1 ? int.Parse(firstPage[0]) : int.Parse(firstPage[1]))))
        //            {
        //                if (listFirstContentData.ContainsKey(index))
        //                {
        //                    listFirstContentData[index] = i;
        //                }
        //                else
        //                {
        //                    listFirstContentData.Add(index, i);
        //                }
        //            }
        //        }
        //        index = 2;
        //        firstData = lstProducts.Take(listFirstContentData.First().Value + 1).ToList();
        //        // remove first page data => keep remain to calculate
        //        lstProducts.RemoveRange(0, listFirstContentData.First().Value + 1);
        //        if (lstProducts.Count > 0)
        //        {
        //            //index = 2;
        //            tempTotalRows = 0;
        //            for (int i = 0; i < lstProducts.Count; i++)
        //            {
        //                if (lstProducts[i].PRODUCTNAME.Length < numberOfCharactersPerRow)
        //                {
        //                    tempTotalRows += 1.61M;
        //                }
        //                else
        //                {
        //                    var tempRows = (decimal)(lstProducts[i].PRODUCTNAME.Length) / numberOfCharactersPerRow;
        //                    if (tempRows.ToString().Contains("."))
        //                    {
        //                        tempRows = int.Parse(tempRows.ToString().Substring(0, tempRows.ToString().IndexOf("."))) + 1;
        //                    }
        //                    tempTotalRows += tempRows;
        //                }
        //                if (tempTotalRows >= (invoice.M_ISTAXRATE == 1 ? int.Parse(nPage[0]) : int.Parse(nPage[1])))
        //                {
        //                    tempTotalRows = 0;
        //                    listMidContentData.Add(index, i);
        //                    index++;
        //                    continue;
        //                }
        //            }
        //        }
        //        middleData = lstProducts.Take(listMidContentData.Count > 0 ? listMidContentData.Last().Value + 1 : 0).ToList();
        //        // remove middle page data => keep remain to calculate
        //        lstProducts.RemoveRange(0, listMidContentData.Count > 0 ? listMidContentData.Last().Value + 1 : 0);
        //        if (lstProducts.Count > 0)
        //        {
        //            tempTotalRows = 0;
        //            for (int i = 0; i < lstProducts.Count; i++)
        //            {
        //                if (lstProducts[i].PRODUCTNAME.Length < numberOfCharactersPerRow)
        //                {
        //                    tempTotalRows += 1.61M;
        //                }
        //                else
        //                {
        //                    var tempRows = (decimal)(lstProducts[i].PRODUCTNAME.Length) / numberOfCharactersPerRow;
        //                    if (tempRows.ToString().Contains("."))
        //                    {
        //                        tempRows = int.Parse(tempRows.ToString().Substring(0, tempRows.ToString().IndexOf("."))) + 1;
        //                    }
        //                    tempTotalRows += tempRows;
        //                }
        //                if (tempTotalRows > int.Parse(lastPage))
        //                {
        //                    middleData.AddRange(lstProducts);
        //                    if (listMidContentData.Count == 0)
        //                    {
        //                        listMidContentData.Add(index, firstData.Count + 1);
        //                    }
        //                    else
        //                    {
        //                        listMidContentData.Add(index, middleData.Count + lstProducts.Count);
        //                    }

        //                    index++;
        //                    //lastData = new List<InvoiceDetailBO>();
        //                    lstProducts = new List<InvoiceDetailBO>();
        //                    break;
        //                }
        //            }
        //        }
        //        //lastData = lstProducts;
        //        lastData = lstProducts;
        //        foreach (var item in listMidContentData)
        //        {
        //            lastData.RemoveRange(0, listMidContentData.Count > 0 ? listMidContentData.Last().Value + 1 : 0);
        //        }
        //        invoice.M_TOTALPAGES = index;
        //        // bind data to content
        //        if (firstData != null && listFirstContentData != null) // first page
        //        {
        //            invoice.LISTPRODUCT = firstData;
        //            invoice.M_INDEX = 1;
        //            var contentMain = RenderViewMultiplePageToString(ControllerContext, templatePath, 1, invoice, true);
        //            content.Add(contentMain);
        //        }

        //        if (middleData != null && listMidContentData != null) // the next page but not last page
        //        {
        //            for (int i = 0; i < listMidContentData.Count; i++)
        //            {
        //                //invoice.M_INDEX = listMidContentData.Values.ElementAt(i);
        //                if ((i + 2) == 2)
        //                {
        //                    invoice.M_INDEX = firstData.Count + 1;
        //                    invoice.LISTPRODUCT = (from k in middleData select k).Take(listMidContentData[i + 2] + 1).ToList();
        //                }
        //                else
        //                {
        //                    invoice.LISTPRODUCT = (from k in middleData select k).Skip(listMidContentData[i + 1] + 1).Take(listMidContentData[i + 2] - listMidContentData[i + 1]).ToList();
        //                }
        //                var midContent = RenderViewMultiplePageToString(ControllerContext, templatePath, i + 2, invoice, true);
        //                content.Add(midContent);
        //            }
        //        }

        //        invoice.M_TOTALMONEYWITHOUTTAX = totalMoneyWithoutTax; // last page
        //        invoice.M_TOTALDISCOUNT = totalDiscount;
        //        invoice.M_TOTALVATAMOUNT = totalVatAmount;
        //        invoice.M_TOTALMONEY = totalMoney;
        //        invoice.M_LSTKCT = lstKCT;
        //        invoice.M_LSTPRODUCTSCOUNT = lstProductsCount;
        //        invoice.LISTPRODUCT = lastData;
        //        invoice.M_INDEX = lstProductsCount - lastData.Count() + 1;
        //        var lastContent = RenderViewMultiplePageToString(ControllerContext, templatePath, (int)index, invoice, true);
        //        content.Add(lastContent);
        //    }
        //    return content;
        //}

        /// <summary>
        /// Tạo file PDF khi người dùng tạo hoặc sửa hóa đơn
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nextNumber"></param>
        /// <returns></returns>
        public byte[] GetInvoiceImportContent(InvoiceBO invoice, long nextNumber)
        {
            try
            {
                var invoiceBLL = new InvoiceBLL();
                // Tạo mã QR code gán vào thuộc tính QRCODEBASE64 = src hình ảnh bên template view
                invoice.QRCODEBASE64 = QRCodeHelper.CreateQRCode("https://onfinance.asia/tracuu/?referencecode=" + invoice.REFERENCECODE);
                //Lấy detail
                var lstProducts = invoiceBLL.GetInvoiceDetail(invoice.ID);
                invoice.LISTPRODUCT = lstProducts;
                invoice.NUMBER = nextNumber != 0 ? nextNumber : 0;
                //Lấy đường dấn View
                var templatePath = "~" + invoice.TEMPLATEPATH;
                //Convert View to string
                var content = RenderViewToString(ControllerContext, templatePath, invoice, true);
                string pdfSize = "A4";
                switch (invoice.USINGINVOICETYPE)
                {
                    case (int)EnumHelper.AccountObjectType.HOADONGTGT:
                    case (int)EnumHelper.AccountObjectType.HOADONTIENDIEN:
                    case (int)EnumHelper.AccountObjectType.HOADONBANHANG:
                        pdfSize = "A4";
                        break;
                    case (int)EnumHelper.AccountObjectType.HOADONTRUONGHOC:
                    case (int)EnumHelper.AccountObjectType.HOADONTIENNUOC:
                        pdfSize = "A5";
                        break;
                }
                //Tạo file pdf từ string
                var dataBuffer = invoiceBLL.BtnCreatePdf(content, null, pdfSize);
                return dataBuffer;
            }
            catch (System.Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy template path", ex, MethodBase.GetCurrentMethod().Name, "GetInvoiceImportContent");
                return null;
            }
        }

        /// <summary>
        /// Tạo file PDF khi người dùng tạo hoặc sửa hóa đơn
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nextNumber"></param>
        /// <returns></returns>
        public string GetTemplateInvoiceContent(InvoiceBO invoice, List<InvoiceDetailBO> lstProducts, long nextNumber, out string htmlContent)
        {
            string msg = string.Empty;
            htmlContent = string.Empty;

            try
            {
                var invoiceBLL = new InvoiceBLL();
                // Tạo mã QR code gán vào thuộc tính QRCODEBASE64 = src hình ảnh bên template view
                invoice.QRCODEBASE64 = QRCodeHelper.CreateQRCode("https://onfinance.asia/tracuu/?referencecode=" + invoice.REFERENCECODE);
                //Lấy detail
                invoice.LISTPRODUCT = lstProducts;
                invoice.NUMBER = nextNumber;
                //Lấy đường dấn View
                var templatePath = "~" + invoice.TEMPLATEPATH == null ? "~/NOVAON_FOLDER/0700252468/TemplateInvoice_01GTKT0001_AA20E.cshtml" : invoice.TEMPLATEPATH;
                //Convert View to string
                htmlContent = RenderViewToString(ControllerContext, templatePath, invoice, true);
            }
            catch (System.Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy template path", ex, MethodBase.GetCurrentMethod().Name, "GetInvoiceImportContent");
                msg = ex.ToString();
            }
            return msg;
        }

        /// <summary>
        /// Tạo file PDF khi người dùng tạo hoặc sửa hóa đơn
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nextNumber"></param>
        /// <returns></returns>
        public string GetTemplateInvoice(InvoiceBO invoice, List<InvoiceDetailBO> lstProducts, long nextNumber, out string htmlContent)
        {
            string msg = string.Empty;
            htmlContent = string.Empty;

            try
            {
                var invoiceBLL = new InvoiceBLL();
                // Tạo mã QR code gán vào thuộc tính QRCODEBASE64 = src hình ảnh bên template view
                invoice.QRCODEBASE64 = QRCodeHelper.CreateQRCode("https://onfinance.asia/tracuu/?referencecode=" + invoice.REFERENCECODE);
                //Lấy detail
                invoice.LISTPRODUCT = lstProducts;
                invoice.NUMBER = nextNumber;
                //Lấy đường dấn View
                var templatePath = invoice.TEMPLATEPATH;
                //Convert View to string
                htmlContent = RenderViewToString(ControllerContext, templatePath, invoice, true);
            }
            catch (System.Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy template path", ex, MethodBase.GetCurrentMethod().Name, "GetInvoiceImportContent");
                msg = ex.ToString();
            }
            return msg;
        }

        ///// <summary>
        ///// Tạo file PDF khi người dùng tạo hoặc sửa hóa đơn
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="nextNumber"></param>
        ///// <returns></returns>
        //public byte[] GetInvoiceContent(string id, long nextNumber)
        //{
        //    try
        //    {
        //        var invoiceId = int.Parse(id);
        //        var invoiceBLL = new InvoiceBLL();
        //        //Lấy master
        //        var invoice = invoiceBLL.GetInvoiceById(invoiceId);
        //        //Lấy detail
        //        invoice.LISTPRODUCT = invoiceBLL.GetInvoiceDetail(invoiceId);
        //        //Gán số hóa đơn nếu đã ký
        //        invoice.NUMBER = nextNumber != 0 ? nextNumber : 0;
        //        //Lấy đường dấn View
        //        var templatePath = "~" + invoice.TEMPLATEPATH;
        //        //Convert View to string
        //        var content = RenderViewToString(ControllerContext, templatePath, invoice, true);
        //        //Tạo file pdf từ string
        //        var dataBuffer = invoiceBLL.BtnCreatePdf(content, null);

        //        return dataBuffer;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        ConfigHelper.Instance.WriteLog("Lỗi lấy template path", ex, MethodBase.GetCurrentMethod().Name, "UpdateInvoice");
        //        return null;
        //    }
        //}

        /// <summary>
        /// Lấy tên hóa đơn ghi vào file xml theo loại hóa đơn của người dùng hiện tại
        /// </summary>
        /// <returns></returns>
        public string GetInvoiceNameXmlFile(int usingInvoiceType)
        {
            string invoiceName = "";
            switch (usingInvoiceType)
            {
                case (int)AccountObjectType.HOADONGTGT:
                    {
                        invoiceName = "Hóa đơn giá trị gia tăng";
                    } break;
                case (int)AccountObjectType.HOADONTIENNUOC:
                    {
                        invoiceName = "Hóa đơn giá trị gia tăng (tiền nước)";
                    }
                    break;
                case (int)AccountObjectType.HOADONTIENDIEN:
                    {
                        invoiceName = "Hóa đơn giá trị gia tăng (tiền điện)";
                    }
                    break;
                case (int)AccountObjectType.HOADONBANHANG:
                    {
                        invoiceName = "Hóa đơn bán hàng";
                    }
                    break;
                case (int)AccountObjectType.HOADONTRUONGHOC:
                    {
                        invoiceName = "Hóa đơn trường học";
                    }
                    break;
                case (int)AccountObjectType.PHIEUXUATKHO:
                    {
                        invoiceName = "Phiếu xuất kho kiêm vận chuyển nội bộ";
                    }
                    break;
                default:
                    invoiceName = "";
                    break;
            }

            return invoiceName;
        }

        public bool SaveFile(string fileName, byte[] dataBuffer)
        {
            try
            {
                using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    if (dataBuffer.Length > 0)
                    {
                        fileStream.Write(dataBuffer, 0, dataBuffer.Length);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Chưa lưu được file vào thư mục của bạn", ex, MethodBase.GetCurrentMethod().Name, "SaveFile2Disk");
                return false;
            }
        }

        #region SendEmail without context
        public ActionResult SendEmail(InvoiceBO invoice, string imgBase64, string fileNames)
        {
            List<Stream> lstFileStream = new List<Stream>();
            List<string> lstFileNames = new List<string>();
            string msg = string.Empty;
            string email = invoice.CUSEMAIL;
            try
            {
                // Tạo Pdf
                ServiceResult serviceResult = SaveTemplateInvoice(invoice.ID);

                if (serviceResult.ErrorCode == 1000)
                    return Json(new { rs = false, msg = serviceResult.Message }, JsonRequestBehavior.AllowGet);

                InvoiceBLL invoiceBLL = new InvoiceBLL();
                invoice = invoiceBLL.GetInvoiceById(invoice.ID, (int)Session["USINGINVOICETYPEID"]);
                if (invoice == null) return Json(new { rs = false, msg = "Lỗi lấy thông tin hóa đơn gửi mail." }, JsonRequestBehavior.AllowGet);
                invoice.CUSEMAIL = email;
                string signLink = invoice.SIGNLINK == null ? "" : invoice.SIGNLINK;

                //msg = AddDataEmail(invoice, signLink);
                //if (msg.Length > 0)
                //    return Json(new { rs = false, msg = "Không tạo được thông tin email vào hệ thống." }, JsonRequestBehavior.AllowGet);

                string inputFolder = ConfigurationManager.AppSettings["OutputSignedInvoiceFolder"];
                var dstFilePathPdf = Server.MapPath("~/" + inputFolder + signLink);
                var dstFilePathXml = Server.MapPath("~/" + inputFolder + signLink.Substring(0, signLink.Length - 4) + ".xml");
                string fileName = "Hoa_Don_Dien_Tu_" + invoice.NUMBER.ToString("D7") + ".pdf";
                msg = GetListAttactmentFile(dstFilePathPdf, fileName, ref lstFileStream, ref lstFileNames);
                if (msg.Length > 0)
                    return Json(new { rs = false, msg = msg }, JsonRequestBehavior.AllowGet);

                fileName = "Hoa_Don_Dien_Tu_" + invoice.NUMBER.ToString("D7") + ".xml";
                msg = GetListAttactmentFile(dstFilePathXml, fileName, ref lstFileStream, ref lstFileNames);
                if (msg.Length > 0)
                    return Json(new { rs = false, msg = msg }, JsonRequestBehavior.AllowGet);

                //Convert ra objectJson
                var data = (JArray)JsonConvert.DeserializeObject(imgBase64);
                var fileNameStr = fileNames.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (data != null)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        // Thực hiện save file người dùng đính kèm
                        string error = SaveAttmentFile(invoice.ID, invoice.COMTAXCODE, data[i].ToString(), fileNameStr[i]);
                        if (error.Length > 0)
                        {
                            return Json(new { rs = false, msg = error }, JsonRequestBehavior.AllowGet);
                        }

                        // gán file người dùng đính kèm lên
                        fileName = fileNameStr[i].ToString();
                        string filePath = Server.MapPath("~/Temp/" + fileName);
                        msg = GetListAttactmentFile(filePath, fileName, ref lstFileStream, ref lstFileNames);
                        if (msg.Length > 0)
                            return Json(new { rs = false, msg = msg }, JsonRequestBehavior.AllowGet);
                    }
                }

                EmailData emailData = new EmailData()
                {
                    Username = objUser.MAILSERVICEID == (int)EMAIL_SERVICE_TYPE.GMAIL ? objUser.MAILSERVICEACCOUNT : ConfigurationManager.AppSettings["UsernameEmail"],
                    Password = objUser.MAILSERVICEID == (int)EMAIL_SERVICE_TYPE.GMAIL ? objUser.MAILSERVICEPASSWORD : ConfigurationManager.AppSettings["PasswordEmail"],
                    Host = "smtp.gmail.com",
                    Port = 587,
                    MailTo = invoice.CUSEMAIL,
                    Subject = "Thông báo phát hành Hóa đơn điện tử - " + invoice.COMNAME,
                    CC = "onfinance@novaon.asia",
                    Content = "",
                    StreamAttachment = lstFileStream,
                    FileName = lstFileNames
                };
                msg = AddDataEmail(invoice, signLink);
                //if (msg.Length > 0)
                //    return Json(new { rs = false, msg = "Không tạo được thông tin email vào hệ thống." }, JsonRequestBehavior.AllowGet);
                EmailSender.MailInvoiceSender(emailData, invoice, 1);
                if (msg.Length > 0)
                    return Json(new { rs = false, msg = "Không tạo được thông tin email vào hệ thống." }, JsonRequestBehavior.AllowGet);
                //Xóa file trong thư mục temp
                MethodHelper.RemoveFileInDirectory(Server.MapPath("~/Temp"));

                return Json(new { rs = true, msg = "Gửi mail thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi gửi email!", ex, MethodBase.GetCurrentMethod().Name, "SendEmail");
                return Json(new { rs = false, msg = "Gửi mail không thành công!" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Thêm thông tin email vào hệ thống
        /// tuyennv 20200220
        /// </summary>
        /// <param name="invoice">đối tượng hóa đơn</param>
        /// <param name="signLink">đường dẫn</param>
        /// <returns></returns>
        protected string AddDataEmail(InvoiceBO invoice, string signLink)
        {
            string msg = string.Empty;
            try
            {
                EmailBO email = new EmailBO()
                {
                    INVOICEID = invoice.ID,
                    STATUS = 1,
                    MAILTO = invoice.CUSEMAIL,
                    RECIEVERNAME = "Thông báo phát hành Hóa đơn điện tử - " + invoice.COMNAME,
                    MAILTYPE = "Phat-hanh",
                    ATTACHEMENTLINK = signLink + "," + signLink.Substring(0, signLink.Length - 4) + ".xml",
                    COMTAXCODE = invoice.COMTAXCODE,
                };

                EmailBLL emailBLL = new EmailBLL();
                invoice.EMAILID = emailBLL.AddEmail(email);
            }
            catch (Exception ex)
            {
                msg = $"Không thêm được thông tin Email vào hệ thống. Chi tiết: {ex.ToString()}";
            }
            return msg;
        }

        /// <summary>
        /// Lấy ra thông tin file đính kèm đẻ gửi mail
        /// tuyennv 20200908
        /// </summary>
        /// <param name="lstFileStreams">thông tin file trả ra</param>
        /// <param name="lstFileNames">tên file gửi mail</param>
        /// <param name="filePath">đường dẫn</param>
        /// <param name="fileNBame">tên file tạo</param>
        /// <returns></returns>
        protected string GetListAttactmentFile(string filePath, string fileNBame, ref List<Stream> lstFileStreams, ref List<string> lstFileNames)
        {
            string msg = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    FileStream fileStreamPdf = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    lstFileStreams.Add(fileStreamPdf);
                    lstFileNames.Add(fileNBame);

                }
            }
            catch (Exception ex)
            {
                msg = $"Không tạo được file để gửi mail.";
            }
            return msg;
        }

        /// <summary>
        /// Đính kèm file khi gửi mail cho khách hàng
        /// Yêu cầu bug: Khi gửi email. Có thêm chức năng đính kèm file gửi đi => Áp dụng cho trường hợp, 
        /// KH gửi hóa đơn cho KH thì gửi kèm thêm bảng kê chi tiết.
        /// Nếu có thể đính kèm lên luôn thì sẽ không cần gửi thêm mail khác nữa
        /// </summary>
        /// <param name="id"></param>
        /// <param name="comTaxCode"></param>
        /// <param name="base64String"></param>
        /// <returns></returns>
        protected string SaveAttmentFile(Int64 id, string comTaxCode, string base64String, string fileName)
        {
            string msg = string.Empty;
            try
            {
                var tempPath = Server.MapPath("~/Temp");
                DirectoryInfo dir = new DirectoryInfo(tempPath);
                if (!dir.Exists)
                {
                    dir.Create();
                }

                string path = Path.Combine(tempPath, fileName);

                byte[] dataBuffer = Convert.FromBase64String(base64String);
                using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    if (dataBuffer.Length > 0)
                    {
                        fileStream.Write(dataBuffer, 0, dataBuffer.Length);
                    }
                }
            }
            catch (Exception objEx)
            {
                msg = "Lỗi không tải lên được file.";
                ConfigHelper.Instance.WriteLog(msg, objEx, MethodBase.GetCurrentMethod().Name, "UploadFileReport");
            }
            return msg;
        }

        //public ServiceResult SaveTemplateInvoice(long invoiceId)
        //{
        //    ServiceResult serviceResult = new ServiceResult();
        //    try
        //    {
        //        InvoiceBLL invoiceBLL = new InvoiceBLL();
        //        //var invoice = invoiceBLL.GetInvoiceById(invoiceId, (int)Session["USINGINVOICETYPEID"]);
        //        var invoice = invoiceBLL.GetInvoiceById(invoiceId, objUser.USINGINVOICETYPE);
        //        if (invoice == null)
        //        {
        //            serviceResult.ErrorCode = 1000;
        //            serviceResult.Message = "Không tạo được file Pdf.";
        //            return serviceResult;
        //        }

        //        bool pageSizeA5 = true;
        //        switch (objUser.USINGINVOICETYPE)
        //        {
        //            case (int)EnumHelper.AccountObjectType.HOADONTIENNUOC:
        //            //case (int)EnumHelper.AccountObjectType.HOADONTRUONGHOC:
        //                pageSizeA5 = false;
        //                break;
        //        }

        //        string filePath;
        //        if (pageSizeA5)
        //        {
        //            var temp = GetInvoiceContent(invoice, invoice.NUMBER); // GetInvoiceImportContent(invoice, invoice.NUMBER);
        //            if (temp == null)
        //            {
        //                serviceResult.ErrorCode = 1000;
        //                serviceResult.Message = "Lỗi lấy nội dung file và đơn và tạo file Pdf.";
        //                return serviceResult;
        //            }

        //            //Lưu file vào thư mục
        //            string fileName = invoice.COMTAXCODE + "_" + invoice.ID + "_" + invoice.INVOICETYPE + ".pdf";
        //            filePath = SaveInvoiceFile(temp, invoice, fileName);
        //            if (string.IsNullOrEmpty(filePath))
        //            {
        //                serviceResult.ErrorCode = 1000;
        //                serviceResult.Message = "Lỗi lưu file hóa đơn vào thư mục.";
        //                return serviceResult;
        //            }
        //        }
        //        else
        //        {
        //            string htmlContent = GetContentViewString(invoice, invoice.NUMBER);
        //            long invoiceid;
        //            RenderHtmlToPdf(invoice, htmlContent, out filePath, out invoiceid);
        //            if (string.IsNullOrWhiteSpace(filePath))
        //            {
        //                serviceResult.ErrorCode = 1000;
        //                serviceResult.Message = "Lỗi lấy nội dung file và đơn và tạo file Pdf.";
        //                return serviceResult;
        //            }
        //        }

        //        bool b = invoiceBLL.UpdateSignLink(invoiceId, filePath);
        //        if (!b)
        //        {
        //            serviceResult.ErrorCode = 1000;
        //            serviceResult.Message = "Lỗi cập nhật đường dẫn vào bản ghi hóa đơn.";
        //            return serviceResult;
        //        }

        //        serviceResult.ErrorCode = 0;
        //        serviceResult.Message = filePath;
        //    }
        //    catch (Exception ex)
        //    {
        //        ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "SaveTemplateInvoice");
        //        serviceResult.ErrorCode = 1000;
        //        serviceResult.Message = "Lỗi tạo mẫ hóa đơn try-catch.";
        //    }
        //    return serviceResult;
        //}
        public ServiceResult SaveTemplateInvoice(long invoiceId)
        {
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                var invoice = invoiceBLL.GetInvoiceById(invoiceId);
                if (invoice == null || invoice.ID == 0)
                {
                    serviceResult.ErrorCode = 1000;
                    serviceResult.Message = "Không tạo được file Pdf.";
                    return serviceResult;
                }
                bool pageSizeA5 = true;
                switch (invoice.USINGINVOICETYPE)
                {
                    case (int)EnumHelper.AccountObjectType.HOADONTIENNUOC:
                        //case (int)EnumHelper.AccountObjectType.HOADONTRUONGHOC:
                        pageSizeA5 = false;
                        break;
                }

                string filePath = "";
                if (pageSizeA5)
                {
                    var temp = GetInvoiceContent(invoice, invoice.NUMBER); // GetInvoiceImportContent(invoice, invoice.NUMBER);
                    if (temp == null)
                    {
                        serviceResult.ErrorCode = 1000;
                        serviceResult.Message = "Lỗi lấy nội dung file và đơn và tạo file Pdf.";
                        return serviceResult;
                    }

                    //Lưu file vào thư mục
                    string fileName = invoice.COMTAXCODE + "_" + invoice.ID + "_" + invoice.INVOICETYPE + ".pdf";
                    filePath = SaveInvoiceFile(temp, invoice, fileName);
                    if (string.IsNullOrEmpty(filePath))
                    {
                        serviceResult.ErrorCode = 1000;
                        serviceResult.Message = "Lỗi lưu file hóa đơn vào thư mục.";
                        return serviceResult;
                    }
                }
                else
                {
                    string htmlContent = GetContentViewString(invoice, invoice.NUMBER);
                    long invoiceid;
                    RenderHtmlToPdf(invoice, htmlContent, out filePath, out invoiceid);
                    if (string.IsNullOrWhiteSpace(filePath))
                    {
                        serviceResult.ErrorCode = 1000;
                        serviceResult.Message = "Lỗi lấy nội dung file và đơn và tạo file Pdf.";
                        return serviceResult;
                    }
                }

                bool b = invoiceBLL.UpdateSignLink(invoiceId, filePath);
                if (!b)
                {
                    serviceResult.ErrorCode = 1000;
                    serviceResult.Message = "Lỗi cập nhật đường dẫn vào bản ghi hóa đơn.";
                    return serviceResult;
                }

                serviceResult.ErrorCode = 0;
                serviceResult.Message = filePath;
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "SaveTemplateInvoice");
                serviceResult.ErrorCode = 1000;
                serviceResult.Message = "Lỗi tạo mẫ hóa đơn try-catch.";
            }
            return serviceResult;
        }

        protected string SaveInvoiceFile(byte[] dataBuffer, InvoiceBO invoice, string fileName)
        {
            try
            {
                var root = ConfigurationManager.AppSettings["InputInvoiceFolder"];
                var branchComTaxCode = "/" + (string.IsNullOrEmpty(invoice.COMTAXCODE) ? "COMTAXCODE/" : invoice.COMTAXCODE + "/");
                var branchInvoiceID = string.IsNullOrEmpty(invoice.ID.ToString()) ? "INVOICEID/" : invoice.ID.ToString() + "/";
                var branchInvoiceType = string.IsNullOrEmpty(invoice.INVOICETYPENAME.ToString()) ? "INVOICETYPENAME/" : ElaHelper.FormatKeywordSearch(ElaHelper.FilterVietkey(invoice.INVOICETYPENAME.ToString())).Replace(" ", "-");
                var branchDate = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                var path = string.Format($"{root}{branchComTaxCode}{branchInvoiceID}{branchInvoiceType}");

                DirectoryInfo dir = new DirectoryInfo(HttpContext.Server.MapPath("~/" + path));
                if (!dir.Exists)
                {
                    dir.Create();
                }

                string filePath = dir + "\\" + fileName;
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    if (dataBuffer.Length > 0)
                    {
                        fileStream.Write(dataBuffer, 0, dataBuffer.Length);
                    }
                }
                fileName = filePath.Replace(HttpContext.Server.MapPath("~/" + root), "").Replace('\\', '/');

                return fileName;
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog(objEx.ToString(), objEx, MethodBase.GetCurrentMethod().Name, "SaveInvoiceFile");

                fileName = string.Empty;
                return null;
            }
        }

        public string RenderHtmlToPdf(InvoiceBO invoice, string htmlContent, out string path, out long invoiceid)
        {
            string msg = string.Empty;
            path = string.Empty;
            invoiceid = 0;
            try
            {
                var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                htmlToPdf.Orientation = NReco.PdfGenerator.PageOrientation.Landscape;
                //The page width and page height values are in mm
                htmlToPdf.PageHeight = 209;
                htmlToPdf.PageWidth = 148;
                var margins = new NReco.PdfGenerator.PageMargins();
                margins.Bottom = 0;
                margins.Top = 0;
                margins.Left = 0;
                margins.Right = 0;
                htmlToPdf.Margins = margins;
                var pdfBytes = htmlToPdf.GeneratePdf(htmlContent);
                path = GetTempFilePath(pdfBytes, invoice);

            }
            catch (Exception ex)
            {
                msg = $"Không tạo được file Pdf. Chi tiết: {ex.ToString()}";
                ConfigHelper.Instance.WriteLog(msg, ex, MethodBase.GetCurrentMethod().Name, "RenderHtmlToPdf");
            }
            return msg;
        }

        public string GetContentViewString(InvoiceBO invoice, long nextNumber)
        {
            string content = string.Empty;
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                // Tạo mã QR code gán vào thuộc tính QRCODEBASE64 = src hình ảnh bên template view
                invoice.QRCODEBASE64 = QRCodeHelper.CreateQRCode("https://onfinance.asia/tracuu/?referencecode=" + invoice.REFERENCECODE);
                //Lấy detail
                var lstProducts = invoiceBLL.GetInvoiceDetail(invoice.ID);
                invoice.LISTPRODUCT = lstProducts;
                invoice.NUMBER = nextNumber != 0 ? nextNumber : 0;
                //Lấy đường dấn View
                var templatePath = "~" + invoice.TEMPLATEPATH;
                //Convert View to string
                content = RenderViewToString(ControllerContext, templatePath, invoice, true);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("", ex, MethodBase.GetCurrentMethod().Name, "GetContentViewString");
                content = string.Empty;
            }
            return content;
        }

        public List<string> GetContentViewStringMuliplePage(InvoiceBO invoice, long nextNumber)
        {
            try
            {
                invoice.USINGINVOICETYPE = objUser.USINGINVOICETYPE;
                var invoiceBLL = new InvoiceBLL();
                // Tạo mã QR code gán vào thuộc tính QRCODEBASE64 = src hình ảnh bên template view
                invoice.QRCODEBASE64 = QRCodeHelper.CreateQRCode("https://onfinance.asia/tracuu/?referencecode=" + invoice.REFERENCECODE);
                //Lấy detail
                var lstProducts = invoiceBLL.GetInvoiceDetail(invoice.ID);
                // Kiểm tra khuyến mãi nếu có thì gán RETAILPRICE = 0
                foreach (var item in lstProducts)
                {
                    if (item.ISPROMOTION)
                    {
                        item.RETAILPRICE = 0;
                    }
                    item.QUANTITY = Math.Round(item.QUANTITY, 2);
                }
                invoice.LISTPRODUCT = lstProducts;
                //Gán số hóa đơn nếu đã ký
                if (invoice.INVOICESTATUS == (int)INVOICE_STATUS.CONFIRMED && nextNumber == 0) // kiểm tra lỗi ký thành công nhưng không có số
                {
                    ConfigHelper.Instance.WriteLog($"{objUser.COMTAXCODE}: Lỗi số tiếp theo khi ký = 0. nextNumber: {nextNumber}", "", MethodBase.GetCurrentMethod().Name, "GetInvoiceContent");
                    return null;
                }
                invoice.NUMBER = nextNumber != 0 ? nextNumber : 0;
                if (invoice.INVOICESTATUS == (int)INVOICE_STATUS.CONFIRMED && invoice.NUMBER == 0) // kiểm tra lỗi ký thành công nhưng không có số
                {
                    ConfigHelper.Instance.WriteLog($"{objUser.COMTAXCODE}: Lỗi số tiếp theo khi ký = 0. InvoiceNumber = 0. nextNumber: {invoice.NUMBER}", "", MethodBase.GetCurrentMethod().Name, "GetInvoiceContent");
                    return null;
                }
                //Lấy đường dấn View
                var templatePath = "~" + invoice.TEMPLATEPATH;

                if (string.IsNullOrWhiteSpace(invoice.TEMPLATEPATH))
                {
                    ConfigHelper.Instance.WriteLog($"Không lấy được TEMPLATEPATH. KH: {invoice.COMTAXCODE}.", string.Empty, MethodBase.GetCurrentMethod().Name, "GetInvoiceContent");
                    return null;
                }
                //Convert View to string
                var content = new List<string>();
                content = GetContentMultiplePage(invoice, lstProducts);

                return content;
            }
            catch (System.Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy template path", ex, MethodBase.GetCurrentMethod().Name, "UpdateInvoice");
                return null;
            }
        }

        protected string GetTempFilePath(byte[] dataBuffer, InvoiceBO invoice)
        {
            string filePaths = string.Empty;
            try
            {
                string fileName = invoice.COMTAXCODE + "_" + invoice.ID + "_" + invoice.INVOICETYPE + ".pdf";

                var root = ConfigurationManager.AppSettings["InputInvoiceFolder"];
                var branchComTaxCode = "/" + (string.IsNullOrEmpty(invoice.COMTAXCODE) ? "COMTAXCODE/" : invoice.COMTAXCODE + "/");
                var branchInvoiceID = string.IsNullOrEmpty(invoice.ID.ToString()) ? "INVOICEID/" : invoice.ID.ToString() + "/";
                var branchInvoiceType = string.IsNullOrEmpty(invoice.INVOICETYPENAME.ToString()) ? "INVOICETYPENAME/" : ElaHelper.FormatKeywordSearch(ElaHelper.FilterVietkey(invoice.INVOICETYPENAME.ToString())).Replace(" ", "-");
                var branchDate = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                var path = string.Format($"{root}{branchComTaxCode}{branchInvoiceID}{branchInvoiceType}");

                DirectoryInfo dir = new DirectoryInfo(HttpContext.Server.MapPath("~/" + path));
                if (!dir.Exists)
                {
                    dir.Create();
                }

                string filePath = dir + "\\" + fileName;
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    if (dataBuffer.Length > 0)
                    {
                        fileStream.Write(dataBuffer, 0, dataBuffer.Length);
                    }
                }

                filePaths = filePath.Replace(HttpContext.Server.MapPath("~/" + root), "").Replace('\\', '/');
            }
            catch (Exception ex)
            {
                filePaths = string.Empty;
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "GetTempFilePath");
            }
            return filePaths;
        }
        
        #endregion

        #region SendEmail with context

        /// <summary>
        /// TuyenNV - 20200608
        /// Gửi email hàng loạt.
        /// </summary>
        /// <param name="idInvoices"></param>
        /// <returns></returns>

        public ActionResult SendMultipleEmail(List<InvoiceBO> invoices)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                HttpContext context = System.Web.HttpContext.Current;
                List<InvoiceBO> listInvoiceAfterGetById = new List<InvoiceBO>();
                bool pageSizeA5 = true;
                switch (objUser.USINGINVOICETYPE)
                {
                    case (int)EnumHelper.AccountObjectType.HOADONTIENNUOC:
                        pageSizeA5 = false;
                        break;
                }

                invoices = invoices.Where(inv => (!string.IsNullOrEmpty(inv.CUSEMAIL) && inv.CUSBUYER != null)).ToList();
                foreach (var item in invoices)
                {
                    string email = string.IsNullOrEmpty(item.CUSEMAIL) ? item.CUSEMAILSEND : item.CUSEMAIL;
                    string filePath = string.Empty;
                    var invoice = invoiceBLL.GetInvoiceById(item.ID);
                    if (invoice == null) continue;
                    invoice.CUSEMAIL = email;
                    listInvoiceAfterGetById.Add(invoice); // cho vào list mới để gửi email

                    string htmlContent = "";
                    List<string> content = new List<string>();
                    if (invoice.USINGINVOICETYPE == (int)AccountObjectType.HOADONTIENNUOC)
                        htmlContent = GetContentViewString(invoice, invoice.NUMBER);
                    else
                        content = GetContentViewStringMuliplePage(invoice, invoice.NUMBER);
                    lock (_locker)
                    {
                        var t = new Task(() =>
                        {
                            Thread.Sleep(100);
                            if (pageSizeA5)
                            {
                                var temp = SendHtmlStringToSelectPDF(invoice, content);
                                string fileName = invoice.COMTAXCODE + "_" + invoice.ID + "_" + invoice.INVOICETYPE + ".pdf";
                                filePath = SaveInvoiceFile(temp, invoice, fileName);
                            }
                            else
                            {
                                long invoiceid;
                                RenderHtmlToPdf(invoice, htmlContent, out filePath, out invoiceid);
                            }
                        });
                        t.ConfigureAwait(false);
                        t.Start();
                    }
                }

                int start_hour = DateTime.Now.Hour;
                int start_minute = DateTime.Now.AddMinutes(2).Minute;
                int start_second = DateTime.Now.AddSeconds(60).Second;
                MyScheduler.IntervalInSeconds(start_hour, start_minute, start_second, () =>
                {
                    SendMultiEmail(listInvoiceAfterGetById, "", "", context);
                    listInvoiceAfterGetById = null;
                });

                return Json(new { rs = true, msg = "Hoàn thành gửi email." });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "SendMultipleEmail");
            }
            return Json(new { rs = true, msg = "Không hoàn thành gửi email." });
        }

        public ActionResult SendMultiEmail(List<InvoiceBO> invoices, string imgBase64, string fileNames, HttpContext context)
        {
            string msg = string.Empty;
            string inputFolder = ConfigurationManager.AppSettings["OutputSignedInvoiceFolder"];
            try
            {
                if (invoices == null)
                    msg = "Không có hóa đơn.";
                if (msg.Length > 0)
                    return Json(new { rs = false, msg = msg }, JsonRequestBehavior.AllowGet);
                Parallel.ForEach(invoices, (invoice) =>
                {
                    Thread.Sleep(100);
                    List<Stream> lstFileStream = new List<Stream>();
                    List<string> lstFileNames = new List<string>();
                    string signLink = invoice.SIGNLINK == null ? "" : invoice.SIGNLINK;
                    var dstFilePathPdf = context.Server.MapPath("~/" + inputFolder + signLink);
                    var dstFilePathXml = context.Server.MapPath("~/" + inputFolder + signLink.Substring(0, signLink.Length - 4) + ".xml");
                    string fileName = "Hoa_Don_Dien_Tu_" + invoice.NUMBER.ToString("D7") + ".pdf";
                    msg = GetListAttactmentFile(dstFilePathPdf, fileName, ref lstFileStream, ref lstFileNames);
                    if (msg.Length > 0)
                        return;

                    fileName = "Hoa_Don_Dien_Tu_" + invoice.NUMBER.ToString("D7") + ".xml";
                    msg = GetListAttactmentFile(dstFilePathXml, fileName, ref lstFileStream, ref lstFileNames);
                    //if (msg.Length > 0)
                    //    return;
                    EmailData emailData = new EmailData()
                    {
                        Username = objUser.MAILSERVICEID == (int)EMAIL_SERVICE_TYPE.GMAIL ? objUser.MAILSERVICEACCOUNT : ConfigurationManager.AppSettings["UsernameEmail"],
                        Password = objUser.MAILSERVICEID == (int)EMAIL_SERVICE_TYPE.GMAIL ? objUser.MAILSERVICEPASSWORD : ConfigurationManager.AppSettings["PasswordEmail"],
                        Host = "smtp.gmail.com",
                        Port = 587,
                        MailTo = invoice.CUSEMAIL,
                        Subject = "Thông báo phát hành Hóa đơn điện tử - " + invoice.COMNAME,
                        CC = "onfinance@novaon.asia",
                        Content = "",
                        StreamAttachment = lstFileStream,
                        FileName = lstFileNames
                    };
                    msg = AddDataEmail(invoice, signLink);
                    EmailSender.MailInvoiceSender(emailData, invoice, 1, context);
                    if (msg.Length > 0)
                        return;
                });
                invoices = null;
                return Json(new { rs = true, msg = "Gửi mail thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi gửi email!", ex, MethodBase.GetCurrentMethod().Name, "SendEmail");
                return Json(new { rs = false, msg = "Gửi mail không thành công!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public byte[] SendHtmlStringToSelectPDF(InvoiceBO invoice, List<string> content)
        {
            try
            {
                invoice.USINGINVOICETYPE = objUser.USINGINVOICETYPE;
                var invoiceBLL = new InvoiceBLL();
                string pdfSize = "A4";
                switch (invoice.USINGINVOICETYPE)
                {
                    case (int)EnumHelper.AccountObjectType.HOADONGTGT:
                    case (int)EnumHelper.AccountObjectType.HOADONTIENDIEN:
                    case (int)EnumHelper.AccountObjectType.HOADONBANHANG:
                        pdfSize = "A4";
                        break;
                    case (int)EnumHelper.AccountObjectType.HOADONTRUONGHOC:
                    case (int)EnumHelper.AccountObjectType.HOADONTIENNUOC:
                        pdfSize = "A5";
                        break;
                }
                //Tạo file pdf từ string
                var dataBuffer = invoiceBLL.BtnCreatePdf(content, null, pdfSize);
                return dataBuffer;
            }
            catch (System.Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy template path", ex, MethodBase.GetCurrentMethod().Name, "GetInvoiceImportContentAsync");
                return null;
            }
        }

        //public async Task<byte[]> SendHtmlStringToSelectPDFAsync(InvoiceBO invoice, List<string> content)
        //{
        //    await System.Threading.Tasks.Task.Delay(100); //Use - when you want a logical delay without blocking the current thread.
        //    try
        //    {
        //        invoice.USINGINVOICETYPE = objUser.USINGINVOICETYPE;
        //        var invoiceBLL = new InvoiceBLL();
        //        string pdfSize = "A4";
        //        switch (invoice.USINGINVOICETYPE)
        //        {
        //            case (int)EnumHelper.AccountObjectType.HOADONGTGT:
        //            case (int)EnumHelper.AccountObjectType.HOADONTIENDIEN:
        //            case (int)EnumHelper.AccountObjectType.HOADONBANHANG:
        //                pdfSize = "A4";
        //                break;
        //            case (int)EnumHelper.AccountObjectType.HOADONTRUONGHOC:
        //            case (int)EnumHelper.AccountObjectType.HOADONTIENNUOC:
        //                pdfSize = "A5";
        //                break;
        //        }
        //        //Tạo file pdf từ string
        //        var dataBuffer = invoiceBLL.BtnCreatePdf(content, null, pdfSize);
        //        return dataBuffer;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        ConfigHelper.Instance.WriteLog("Lỗi lấy template path", ex, MethodBase.GetCurrentMethod().Name, "GetInvoiceImportContentAsync");
        //        return null;
        //    }
        //}
        #endregion

    }
}