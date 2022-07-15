using DS.BusinessLogic;
using DS.BusinessLogic.Customer;
using DS.BusinessObject.Customer;
using DS.BusinessObject.Invoice;
using DS.Common.Helpers;
using SPA_Invoice.Common;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using DS.BusinessLogic.Invoice;
using DS.Common.Enums;
using DS.BusinessLogic.Meter;
using DS.BusinessObject.Meter;
using Newtonsoft.Json;
using DS.BusinessObject.Output;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using SPA_Invoice.Models;

namespace SPA_Invoice.Controllers
{

    public class CustomerController : BaseController
    {
        private readonly IHubContext _hubContext;
        public CustomerController()
        {
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<SignlRConf>();
        }

        public ActionResult Index()
        {
            return PartialView();
        }

        public ActionResult Detail()
        {
            return PartialView();
        }

        public ActionResult GetCustomer(FormSearchCustomer form, int currentPage, int itemPerPage)
        {
            try
            {
                form.CURRENTPAGE = currentPage;
                form.ITEMPERPAGE = itemPerPage;
                form.COMTAXCODE = objUser.COMTAXCODE;
                form.OFFSET = (form.CURRENTPAGE - 1) * form.ITEMPERPAGE;

                CustomerBLL customerBLL = new CustomerBLL();
                List<CustomerBO> result = customerBLL.GetCustomer(form);

                long TotalPages = 0;
                var TotalRow = result.Count == 0 ? 0 : result[0].TOTALROW;
                if (TotalRow % itemPerPage == 0)
                    TotalPages = TotalRow == 0 ? 1 : TotalRow / itemPerPage;
                else
                    TotalPages = TotalRow / itemPerPage + 1;

                if (customerBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(customerBLL.ResultMessageBO.Message, customerBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetCustomer");
                    return Json(new { rs = false, msg = customerBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result, TotalPages, TotalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy thông tin khách hàng", ex, MethodBase.GetCurrentMethod().Name, "GetCustomer");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddCustomer(CustomerBO customer)
        {
            try
            {
                CustomerBLL customerBLL = new CustomerBLL();
                customer.COMTAXCODE = objUser.COMTAXCODE;

                //if (string.IsNullOrWhiteSpace(customer.CUSTAXCODE) && !string.IsNullOrWhiteSpace(customer.CUSID))
                //    customer.CUSTAXCODE = customer.CUSID;// 
                //if (string.IsNullOrWhiteSpace(customer.CUSID) && !string.IsNullOrWhiteSpace(customer.CUSTAXCODE))
                //    customer.CUSID = customer.CUSTAXCODE;
                if (string.IsNullOrWhiteSpace(customer.CUSNAME) && !string.IsNullOrWhiteSpace(customer.CUSBUYER))
                    customer.CUSNAME = customer.CUSBUYER;
                else if (string.IsNullOrWhiteSpace(customer.CUSBUYER) && !string.IsNullOrWhiteSpace(customer.CUSNAME))
                    customer.CUSBUYER = customer.CUSNAME;

                // kiểm tra trùng dữ liệu KH bằng MST
                string msg = customerBLL.CheckCustomerDuplicateTaxCode(customer);
                string msgid = customerBLL.CheckCustomerByCusId(customer);
                if((objUser.USINGINVOICETYPE != (int)EnumHelper.AccountObjectType.HOADONTRUONGHOC))
                {
                    if (msg.Length > 0)
                        return Json(new { rs = false, msg = msg + "  " + msgid + ". Vui lòng kiểm tra lại." });
                }
                if (msgid.Length > 0)
                    return Json(new { rs = false, msg = msgid +"  "+ msg + ". Vui lòng kiểm tra lại." });
                bool result = customerBLL.AddCustomer(customer);
                if (customerBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(customerBLL.ResultMessageBO.Message, customerBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "SaveCustomer");
                    return Json(new { rs = false, msg = customerBLL.ResultMessageBO.Message });
                }

                return Json(new { rs = result, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi thêm khách hàng", ex, MethodBase.GetCurrentMethod().Name, "AddCustomer");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UpdateCustomer(CustomerBO customer)
        {
            try
            {
                CustomerBLL customerBLL = new CustomerBLL();

                customer.COMTAXCODE = objUser.COMTAXCODE;
                string msg = customerBLL.CheckCustomerDuplicateTaxCode(customer);
                string msgid = customerBLL.CheckCustomerByCusId(customer);
                
                var result = customerBLL.UpdateCustomer(customer);
                if (customerBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(customerBLL.ResultMessageBO.Message, customerBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "SaveCustomer");
                    return Json(new { rs = false, msg = customerBLL.ResultMessageBO.Message });
                }

                return Json(new { rs = result, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi cập nhật khách hàng", ex, MethodBase.GetCurrentMethod().Name, "UpdateCustomer");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetInvoice(CustomerBO customer, int currentPage)
        {
            try
            {
                FormSearchInvoice form = new FormSearchInvoice
                {
                    COMTAXCODE = objUser.COMTAXCODE,
                    CUSTOMER = customer.CUSBUYER,
                    PHONENUMBER = customer.CUSPHONENUMBER,
                    TAXCODE = customer.CUSTAXCODE,
                    ITEMPERPAGE = 10,
                    CURRENTPAGE = currentPage,
                };

                form.FROMDATE = string.IsNullOrEmpty(form.STRFROMDATE) ? new DateTime(2000, 1, 1) : DateTime.ParseExact(form.STRFROMDATE, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                form.TODATE = string.IsNullOrEmpty(form.STRTODATE) ? DateTime.Now : DateTime.ParseExact(form.STRTODATE, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                form.OFFSET = (form.CURRENTPAGE - 1) * form.ITEMPERPAGE;

                InvoiceBLL invoiceBLL = new InvoiceBLL();
                var result = invoiceBLL.GetListInvoiceByCustomer(form);
                if (invoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetInvoice");
                    return Json(new { rs = false, msg = invoiceBLL.ResultMessageBO.Message });
                }

                long TotalPages = 0;
                long TotalRow = result.Count == 0 ? 0 : result[0].TOTALROW;
                if (TotalRow % 10 == 0)
                    TotalPages = TotalRow == 0 ? 1 : TotalRow / 10;
                else
                    TotalPages = TotalRow / 10 + 1;

                return Json(new { rs = true, result, TotalPages }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy danh sách hóa đơn của khách hàng", ex, MethodBase.GetCurrentMethod().Name, "GetInvoice");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ExportExcell(string keyword, int currentPage)
        {
            try
            {
                CustomerBLL customerBLL = new CustomerBLL();
                FormSearchCustomer search = new FormSearchCustomer
                {
                    COMTAXCODE = objUser.COMTAXCODE,
                    KEYWORD = keyword,
                    CURRENTPAGE = currentPage
                };
                List<CustomerBO> result = customerBLL.GetCustomer(search);

                // tạo nhãn dữ liệu
                DataTable dtReport = new DataTable();
                dtReport.Columns.Add("TÊN KHÁCH HÀNG");
                dtReport.Columns.Add("TÊN DOANH NGHIỆP");
                dtReport.Columns.Add("MÃ SỐ THUẾ");
                dtReport.Columns.Add("SỐ ĐIỆN THOẠI");
                dtReport.Columns.Add("EMAIL");
                dtReport.Columns.Add("WEBSITE");
                dtReport.Columns.Add("ĐỊA CHỈ");
                dtReport.Columns.Add("HÌNH THỨC THANH TOÁN");
                dtReport.Columns.Add("NGÂN HÀNG");
                dtReport.Columns.Add("SỐ TÀI KHOẢN");
                dtReport.Columns.Add("ĐƠN VỊ TIỀN TỆ");
                dtReport.Columns.Add("GHI CHÚ");

                //add dòng dữ liệu vào
                foreach (var row in result)
                {
                    DataRow dr = dtReport.NewRow();
                    dr["TÊN KHÁCH HÀNG"] = row.CUSBUYER;
                    dr["TÊN DOANH NGHIỆP"] = row.CUSNAME;
                    dr["MÃ SỐ THUẾ"] = row.CUSTAXCODE;
                    dr["SỐ ĐIỆN THOẠI"] = row.CUSPHONENUMBER;
                    dr["EMAIL"] = row.CUSEMAIL;
                    dr["WEBSITE"] = row.CUSWEBSITE;
                    dr["ĐỊA CHỈ"] = row.CUSADDRESS;
                    dr["HÌNH THỨC THANH TOÁN"] = row.CUSPAYMENTMETHOD;
                    dr["NGÂN HÀNG"] = row.CUSBANKNAME;
                    dr["SỐ TÀI KHOẢN"] = row.CUSACCOUNTNUMBER;
                    dr["ĐƠN VỊ TIỀN TỆ"] = row.CUSCURRENCYUNIT;
                    dr["GHI CHÚ"] = row.CUSNOTE;
                    dtReport.Rows.Add(dr);
                }
                // đặt tên file xuất ra file theo tên 
                string strFileName = "Danh_sach_khach_hang" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm");
                return PushFile(dtReport, strFileName);
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi khi xuất excel danh sách khách hàng.", objEx, MethodBase.GetCurrentMethod().Name, "ExportExcell");
                return Json(new { success = false, responseText = "Lỗi khi xuất excel danh sách khách hàng." });
            }
        }

        public ActionResult DeactiveCustomer(List<CustomerBO> customers)
        {
            try
            {
                string customerIds = string.Join(",", customers.Where(x => x.ISSELECTED).Select(y => y.ID).ToList());

                CustomerBLL customerBLL = new CustomerBLL();
                var result = customerBLL.DeactiveCustomer(customerIds);
                if (customerBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(customerBLL.ResultMessageBO.Message, customerBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "DeactiveCustomer");
                    return Json(new { rs = false, msg = customerBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi ngừng kích hoạt khách hàng", ex, MethodBase.GetCurrentMethod().Name, "DeactiveCustomer");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveCustomerList(List<CustomerBO> customers)
        {
            try
            {
                customers.ForEach(x => x.COMTAXCODE = objUser.COMTAXCODE);
                CustomerBLL customerBLL = new CustomerBLL();
                var result = customerBLL.SaveCustomerList(customers);
                if (customerBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(customerBLL.ResultMessageBO.Message, customerBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "SaveCustomer");
                    return Json(new { rs = false, msg = customerBLL.ResultMessageBO.Message });
                }
                if (!result)
                    return Json(new { rs = false, msg = "Tải lên file khách hàng thất bại, vui lòng thử lại" });
                return Json(new { rs = result, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lưu khách hàng được tải lên bằng file excel", ex, MethodBase.GetCurrentMethod().Name, "SaveCustomerList");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ChangeYourField(List<ImportCustomerTitleBO> listdatafield)
        {
            try
            {
                return Json(new { rs = true, ListData = listdatafield }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ImportCustomerFromExcel(FormCollection collection)
        {
            try
            {
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        Stream stream = fileContent.InputStream;
                        using (var package = new ExcelPackage(stream))
                        {
                            var workBook = package.Workbook;
                            if (workBook != null)
                            {
                                if (workBook.Worksheets.Count > 0)
                                {
                                    var sheet = workBook.Worksheets[1];
                                    //int countYourColumns = 0;
                                    List<ImportCustomerTitleBO> lstColumn = null;
                                    //if (objUser.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONGTGT || objUser.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONBANHANG) // TH không phải là trường học
                                    //{
                                    //    lstColumn = new List<ImportCustomerTitleBO>
                                    //                {
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Mã KH" },
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Doanh nghiệp" },
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Mã số thuế" },
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Địa chỉ" },
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Người mua hàng" },
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Email" },
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Số điện thoại" },
                                    //                };
                                    //}
                                    //else if (objUser.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTRUONGHOC) 
                                    //{
                                    //    lstColumn = new List<ImportCustomerTitleBO>
                                    //                {
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Mã HS" },
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Phụ huynh" },
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Mã số thuế/Mã HS" },
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Lớp" },
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Tên học sinh" },
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Email" },
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Số điện thoại" },
                                    //                };
                                    //}
                                    //else if (objUser.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTIENDIEN)
                                    //{
                                    //    lstColumn = new List<ImportCustomerTitleBO>
                                    //                {
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Mã khách hàng" },
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Tên khách hàng" },
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Mã số thuế" },
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Địa chỉ" },
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Điện thoại" },
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Email" },
                                    //                    new ImportCustomerTitleBO { MYFIELD = "TK ngân hàng" },
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Tên NH" },
                                    //                };
                                    //}
                                    //else if (objUser.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTIENNUOC)
                                    //{
                                    //    lstColumn = new List<ImportCustomerTitleBO>
                                    //                {
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Mã khách hàng" },
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Tên khách hàng" },
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Mã số thuế" },
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Địa chỉ" },
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Điện thoại" },
                                    //                     new ImportCustomerTitleBO { MYFIELD = "Người mua hàng" },
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Email" },
                                    //                    new ImportCustomerTitleBO { MYFIELD = "TK ngân hàng" },
                                    //                    new ImportCustomerTitleBO { MYFIELD = "Tên NH" },
                                    //                };
                                    //}

                                    lstColumn = new List<ImportCustomerTitleBO>
                                                    {
                                                        new ImportCustomerTitleBO { MYFIELD = "Mã khách hàng" },
                                                        new ImportCustomerTitleBO { MYFIELD = "Tên khách hàng" },
                                                        new ImportCustomerTitleBO { MYFIELD = "Mã số thuế" },
                                                        new ImportCustomerTitleBO { MYFIELD = "Địa chỉ" },
                                                        new ImportCustomerTitleBO { MYFIELD = "Điện thoại" },
                                                         new ImportCustomerTitleBO { MYFIELD = "Người mua hàng" },
                                                        new ImportCustomerTitleBO { MYFIELD = "Email" },
                                                        new ImportCustomerTitleBO { MYFIELD = "TK ngân hàng" },
                                                        new ImportCustomerTitleBO { MYFIELD = "Tên NH" },
                                                        new ImportCustomerTitleBO { MYFIELD = "Hình thức thanh toán" },
                                                    };

                                    for (var columnNum = 1; columnNum <= sheet.Dimension.End.Column; columnNum++)
                                    {
                                        //countYourColumns++;
                                        if (sheet.Cells[1, columnNum].Value == null || sheet.Cells[1, columnNum].Value.ToString().Trim() == "")

                                            continue;
                                        try
                                        {
                                            if (lstColumn[columnNum - 1] == null)
                                                lstColumn.Add(new ImportCustomerTitleBO());
                                        }
                                        catch
                                        {
                                            lstColumn.Add(new ImportCustomerTitleBO());
                                        }
                                        lstColumn[columnNum - 1].YOURFIELD = sheet.Cells[1, columnNum].Value.ToString();
                                        if (lstColumn[columnNum - 1].MYFIELD.ToLower() != lstColumn[columnNum - 1].YOURFIELD.ToLower())
                                            lstColumn[columnNum - 1].ISDIFFERENT = true;
                                    }
                                    int countDiffent = lstColumn.Where(x => x.ISDIFFERENT).Count();
                                    //int countMissingColumns = lstColumn.Count - countYourColumns;
                                    if (countDiffent > 0)
                                        return Json(new { rs = true, IsDiffent = true, data = lstColumn });

                                    List<ImportCustomerBO> importCustomers = new List<ImportCustomerBO>();
                                    for (var rowNum = 2; rowNum <= sheet.Dimension.End.Row; rowNum++)
                                    {
                                        ImportCustomerBO importCustomer = new ImportCustomerBO();
                                        if (sheet.Cells[rowNum, 2].Value == null || sheet.Cells[rowNum, 2].Value.ToString().Trim() == "")
                                            continue;

                                        string cusid = sheet.Cells[rowNum, 1].Value == null ? "" : sheet.Cells[rowNum, 1].Value.ToString().Trim();
                                        string custaxcode = sheet.Cells[rowNum, 3].Value == null ? "" : sheet.Cells[rowNum, 3].Value.ToString().Trim();

                                        if (string.IsNullOrEmpty(cusid))
                                            cusid = custaxcode;
                                        else if (string.IsNullOrEmpty(custaxcode))
                                            custaxcode = cusid;

                                        importCustomer.CUSID = cusid;
                                        importCustomer.CUSNAME = sheet.Cells[rowNum, 2].Value == null ? "" : sheet.Cells[rowNum, 2].Value.ToString().Trim();
                                        importCustomer.CUSTAXCODE = custaxcode;
                                        importCustomer.CUSADDRESS = sheet.Cells[rowNum, 4].Value == null ? "" : sheet.Cells[rowNum, 4].Value.ToString().Trim();
                                        importCustomer.CUSPHONENUMBER = sheet.Cells[rowNum, 5].Value == null ? "" : sheet.Cells[rowNum, 5].Value.ToString().Trim();
                                        importCustomer.CUSBUYER = sheet.Cells[rowNum, 6].Value == null ? "" : sheet.Cells[rowNum, 6].Value.ToString().Trim();
                                        importCustomer.CUSEMAIL = sheet.Cells[rowNum, 7].Value == null ? "" : sheet.Cells[rowNum, 7].Value.ToString().Trim();
                                        importCustomer.CUSACCOUNTNUMBER = sheet.Cells[rowNum, 8].Value == null ? "" : sheet.Cells[rowNum, 8].Value.ToString().Trim();
                                        importCustomer.CUSBANKNAME = sheet.Cells[rowNum, 9].Value == null ? "" : sheet.Cells[rowNum, 9].Value.ToString().Trim();
                                        importCustomer.CUSPAYMENTMETHOD = sheet.Cells[rowNum, 10].Value == null ? "" : sheet.Cells[rowNum, 10].Value.ToString().Trim();
                                        importCustomers.Add(importCustomer);
                                    }

                                    return Json(new { rs = true, isDiffent = false, data = lstColumn, importCustomers });
                                }
                            }
                        }
                    }
                }
                return Json(new { rs = false, reponseText = "File không đúng chuẩn, vui lòng tải file Excel mẫu kế bên" });
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi đọc file excel", objEx, MethodBase.GetCurrentMethod().Name, "ImportCustomerFromExcel");
                return Json(new { rs = false, reponseText = "File không đúng chuẩn, vui lòng tải file Excel mẫu kế bên." });
            }
        }

        public ActionResult SuggestByObject(string strKeyword, int intPageSize)
        {
            try
            {
                var objInfo = new CustomerBLL();

                //Ưu tiên tìm sản phẩm
                var ES_List = objInfo.ES_CustomerByKeyword(strKeyword, objUser.COMTAXCODE, intPageSize, 0);

                if (objInfo.ResultMessageBO.IsError)
                    return Json(new { rs = false, msg = objInfo.ResultMessageBO.Message }, JsonRequestBehavior.AllowGet);

                //var tempListCustomers = ES_List.Where(x => x.COMTAXCODE == objUser.COMTAXCODE).ToList();
                return Json(new { rs = true, listResult = ES_List });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi không thể lấy thông tin khách hàng từ ES.", ex, "SuggestByObject", "Common");
                return Json(new { rs = false, msg = "Lỗi không thể lấy thông tin khách hàng từ ES." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Tìm kiếm khách hàng
        /// </summary>
        /// <param name="strKeyword"></param>
        /// <param name="intPageSize"></param>
        /// <returns></returns>
        public ActionResult SuggestCusByObject(string strKeyword, int intPageSize)
        {
            try
            {
                FormSearchCustomer form = new FormSearchCustomer();

                form.CURRENTPAGE = 1;
                form.ITEMPERPAGE = 10;
                form.COMTAXCODE = objUser.COMTAXCODE;
                form.KEYWORD = strKeyword;
                form.OFFSET = (form.CURRENTPAGE - 1) * form.ITEMPERPAGE;

                CustomerBLL oBL = new CustomerBLL();

                List<CustomerBO> tempListCustomers = oBL.SearchCustomer(form);

                return Json(new { rs = true, listResult = tempListCustomers });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi không thể lấy thông tin khách hàng từ ES.", ex, "SuggestCusByObject", "Common");
                return Json(new { rs = false, msg = "Lỗi không thể lấy thông tin khách hàng từ ES." }, JsonRequestBehavior.AllowGet);
            }
        }
        //xóa khách hàng
        public ActionResult RemoveCustomer(string idCustomers, CustomerBO customer, string codeCus)
        {
            try
            {
                string[] lstCustomerid = idCustomers.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                string[] lstCusCode = codeCus.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                customer.COMTAXCODE = objUser.COMTAXCODE;
                CustomerBLL customerBLL = new CustomerBLL();
                List<string> arr = new List<string>();
                List<string> arr1 = new List<string>();
                for (int i = 0; i < lstCustomerid.Length; i++)
                {
                    var vVal = CommonFunction.NullSafeInteger(lstCustomerid[i], 0);
                    arr.Add($"'{vVal}'");
                }
                for (int i = 0; i < lstCusCode.Length; i++)
                {
                    var vVal = CommonFunction.NullSafeString(lstCusCode[i], "");
                    arr1.Add($"'{vVal}'");
                }
                string ids = string.Join(",", arr);
                string codes = string.Join(",", arr1);
                int listCus = Convert.ToInt32(customerBLL.CheckCustomer(codes, customer.COMTAXCODE));
                if(objUser.USINGINVOICETYPE == 2 || objUser.USINGINVOICETYPE == 4)
                {
                    int listMeter = Convert.ToInt32(customerBLL.CheckMeter(codes, customer.COMTAXCODE));
                    if (listCus > 0 || listMeter> 0)
                    {
                        return Json(new { rs = false, msg = $"Khách hàng này đang được sử dụng bạn không được xóa " });
                    }
                }
                if (listCus > 0)
                {
                    return Json(new { rs = false, msg = $"Khách hàng này đang được sử dụng bạn không được xóa " });
                }
                else
                {
                    string msg = customerBLL.DeleteCustomer(ids);
                    if (msg.Length > 0)
                        return Json(new { rs = false, msg = msg });
                    else
                    {
                        return Json(new { rs = true, msg = $"Xóa thành công {lstCustomerid.Length}/{lstCustomerid.Length} khách hàng." });
                    }
                }
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "RemoveCustomer");
            }

            return Json(new { rs = true, msg = "Xóa thành công khách hàng" });
        }

        #region Mapping khách hàng

        /// <summary>
        /// tuyennv 20200706
        /// Đọc dữ liệu file excel lấy ra thông tin các sheet trong file
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public ActionResult ReadFileExcel(FormCollection collection)
        {
            try
            {
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        string path = Server.MapPath("~/Uploads/");
                        string extension = Path.GetExtension(path + fileContent.FileName);
                        if (extension != ".xls" && extension != ".xlsx") return Json(new { rs = false, msg = "File không đúng định dạng, Hỗ trợ định dạng .xls và .xlsx." });

                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        foreach (string key in Request.Files)
                        {
                            HttpPostedFileBase postedFile = Request.Files[key];
                            postedFile.SaveAs(path + fileContent.FileName);
                        }
                        string fullPath = path + fileContent.FileName;
                        CacheHelper.Set("FullPath", fullPath);
                        List<ExWorksheet> objSheets;
                        string msg = ExcelHelper.GetAllWorksheets(fullPath, out objSheets);
                        if (msg.Length > 0) return Json(new { rs = false, msg = msg });
                        return Json(new { rs = true, fileName = fileContent.FileName, listSheet = JsonConvert.SerializeObject(objSheets), msg = "Tệp dữ liệu đã tải lên thành công." }, JsonRequestBehavior.AllowGet);
                    }
                }
                ConfigHelper.Instance.WriteLog($"File không đúng định dạng theo quy định mẫu cung cấp.", string.Empty, MethodBase.GetCurrentMethod().Name, "ReadFileExcel");
                return Json(new { rs = false, msg = "File không đúng định dạng theo quy định mẫu cung cấp." });
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi đọc file excel", objEx, MethodBase.GetCurrentMethod().Name, "ReadFileExcel");
                return Json(new { rs = false, msg = "Lỗi đọc file excel." });
            }
        }

        /// <summary>
        /// tuyennv 20200706
        /// Thực hiện ghép cột dữ liệu của kh vs onfinance
        /// </summary>
        /// <param name="headerRow">dòng để lấy tiêu đề các cột và dữ liệu</param>
        /// <param name="selectedSheet">Sheet để lấy dữ liệu</param>
        /// <returns></returns>
        public ActionResult MappingColumnExcel(int headerRow, int selectedSheet)
        {
            string msg = string.Empty;
            try
            {
                var fullPath = CacheHelper.Get("FullPath");
                if (fullPath == null) return Json(new { rs = false, msg = "Đã hết phiên làm việc. Bạn bấm <b>Đăng nhập lại</b> để trở về trang đăng nhập." });

                int indexSheet = selectedSheet; // Sheet dùng để lấy dữ liệu
                int rowIndex = headerRow;// dòng lấy ra tiêu đề các cột
                DataTable dt;
                msg = ExcelHelper.GetDataTableFromExcelFile(fullPath.ToString(), indexSheet, rowIndex, out dt);
                if (msg.Length > 0) return Json(new { rs = false, msg = "Dòng tiêu đề bạn chọn không có dữ liệu." });

                string[] arrCol;
                msg = ExcelHelper.GetColumnNamesFromDatable(dt, out arrCol);
                if (msg.Length > 0) return Json(new { rs = false, msg = "Dòng tiêu đề bạn chọn không có dữ liệu." });

                if (dt == null || dt.Rows.Count == 0) return Json(new { rs = false, msg = "Dòng tiêu đề bạn chọn không có dữ liệu." });
                CacheHelper.Set("dtCustomer", dt);

                List<ImportInvoiceTitleBO> lstColumn = null;
                lstColumn = new List<ImportInvoiceTitleBO>
                {
                    new ImportInvoiceTitleBO { MYFIELD = "Mã khách hàng", EXPLAINT = "Mã khác hàng", ISMANDATORY = false, ConfigFields= new List<string>(){ "Mã khách hàng","Mã KH" } },
                    new ImportInvoiceTitleBO { MYFIELD = "Tên khách hàng", EXPLAINT = "Tên khách hàng" },
                    new ImportInvoiceTitleBO { MYFIELD = "MST", EXPLAINT = "Mã số thuế, kiểu ký tự số", ConfigFields= new List<string>(){ "Mã số thuế","MST" } },
                    new ImportInvoiceTitleBO { MYFIELD = "Địa chỉ" },
                    new ImportInvoiceTitleBO { MYFIELD = "Số điện thoại", ConfigFields = new List<string>(){ "Số điện thoại", "Điện thoại" } },
                    new ImportInvoiceTitleBO { MYFIELD = "Người mua hàng" },
                    new ImportInvoiceTitleBO { MYFIELD = "Email" },
                    new ImportInvoiceTitleBO { MYFIELD = "TK ngân hàng", ConfigFields = new List<string>(){ "Số tài khoản", "TK ngân hàng" } },
                    new ImportInvoiceTitleBO { MYFIELD = "Ngân hàng", ConfigFields = new List<string>(){ "Ngân hàng", "Tên NH", "Tên ngân hàng" } },
                    new ImportInvoiceTitleBO { MYFIELD = "Hình thức thanh toán" }
                };
                CacheHelper.Set("ColumnCustomerFinance", lstColumn);

                List<ImportInvoiceColumnMaping> objColMaps = new List<ImportInvoiceColumnMaping>();

                for (var j = 0; j < arrCol.Length; j++)
                {
                    ImportInvoiceColumnMaping objMap = new ImportInvoiceColumnMaping();
                    objMap.Index = j;
                    objMap.ColName = arrCol[j];
                    objColMaps.Add(objMap);
                }

                for (int i = 0; i < lstColumn.Count; i++)
                {
                    for (var j = 0; j < arrCol.Length; j++)
                    {
                        if (lstColumn[i].MYFIELD.Equals(arrCol[j], StringComparison.OrdinalIgnoreCase))
                        {
                            CommonFunction.SetPropertyValue(lstColumn[i], "YOURFIELD", arrCol[j]);
                        }
                        else if (lstColumn[i].ConfigFields != null && lstColumn[i].ConfigFields.Contains(arrCol[j].ToString()))
                        {
                            CommonFunction.SetPropertyValue(lstColumn[i], "YOURFIELD", arrCol[j]);
                        }
                    }
                }

                return Json(new
                {
                    rs = true,
                    data = lstColumn,
                    lstYourField = objColMaps.Distinct(),
                    clientData = JsonConvert.SerializeObject(arrCol)
                });
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi ghép cột dữ liệu.", objEx, MethodBase.GetCurrentMethod().Name, "MappingColumnExcel");
                return Json(new { rs = false, msg = "Lỗi ghép cột dữ liệu." });
            }
        }

        /// <summary>
        /// truongnv 20200706
        /// Xem trước dữ liệu sau khi mapping xong
        /// </summary>
        /// <param name="listMap"></param>
        /// <param name="formCode"></param>
        /// <param name="symbolCode"></param>
        /// <returns></returns>
        public ActionResult PreviewCustomerData(string listMap, string formCode, string symbolCode)
        {
            try
            {
                var dt = CacheHelper.Get("dtCustomer");
                if (dt == null) return Json(new { rs = false, msg = "Đã hết phiên làm việc. Bạn bấm <b>Đăng nhập lại</b> để trở về trang đăng nhập." });
                var dtInvoice = (DataTable)dt;
                if (dtInvoice != null && dtInvoice.Rows.Count > 0)
                {
                    var columnInvoiceFinance = CacheHelper.Get("ColumnCustomerFinance");
                    if (columnInvoiceFinance == null) return Json(new { rs = false, msg = "Đã hết phiên làm việc. Bạn bấm <b>Đăng nhập lại</b> để trở về trang đăng nhập." });
                    List<ImportInvoiceTitleBO> lstColumn = (List<ImportInvoiceTitleBO>)columnInvoiceFinance;
                    var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(listMap);
                    List<CustomerBO> importCustomers = new List<CustomerBO>();
                    // rowNum: start row to get data from excel file.
                    CustomerBO importCustomer = null;
                    try
                    {
                        foreach (DataRow row in dtInvoice.Rows)
                        {
                            if (dic[lstColumn[1].MYFIELD] == null || row[dic[lstColumn[1].MYFIELD]].ToString().Trim() == "")
                                continue;

                            string cusid = dic[lstColumn[0].MYFIELD] == null ? "" : row[dic[lstColumn[0].MYFIELD]].ToString().Trim();
                            string custaxcode = dic[lstColumn[2].MYFIELD] == null ? "" : row[dic[lstColumn[2].MYFIELD]].ToString().Trim();

                            //if (string.IsNullOrEmpty(cusid))
                            //    cusid = custaxcode;
                            //else if (string.IsNullOrEmpty(custaxcode))
                            //    custaxcode = cusid;

                            importCustomer = new CustomerBO
                            {
                                CUSID = cusid,
                                CUSNAME = dic[lstColumn[1].MYFIELD] != null ? row[dic[lstColumn[1].MYFIELD]].ToString() : string.Empty,
                                CUSTAXCODE = custaxcode,
                                CUSADDRESS = dic[lstColumn[3].MYFIELD] != null ? row[dic[lstColumn[3].MYFIELD]].ToString() : string.Empty,
                                CUSPHONENUMBER = dic[lstColumn[4].MYFIELD] != null ? row[dic[lstColumn[4].MYFIELD]].ToString() : string.Empty,
                                CUSBUYER = dic[lstColumn[5].MYFIELD] != null ? row[dic[lstColumn[5].MYFIELD]].ToString() : string.Empty,
                                CUSEMAIL = dic[lstColumn[6].MYFIELD] != null ? row[dic[lstColumn[6].MYFIELD]].ToString() : string.Empty,
                                CUSACCOUNTNUMBER = dic[lstColumn[7].MYFIELD] != null ? row[dic[lstColumn[7].MYFIELD]].ToString() : string.Empty,
                                CUSBANKNAME = dic[lstColumn[8].MYFIELD] != null ? row[dic[lstColumn[8].MYFIELD]].ToString() : string.Empty,
                                CUSPAYMENTMETHOD = dic[lstColumn[9].MYFIELD] != null ? row[dic[lstColumn[9].MYFIELD]].ToString() : string.Empty,
                            };
                            importCustomers.Add(importCustomer);
                        }
                    }
                    catch (Exception objEx)
                    {
                        ConfigHelper.Instance.WriteLog("Lỗi lấy dữ liệu để thêm mới.", objEx, MethodBase.GetCurrentMethod().Name, "PreviewCustomerData");
                        return Json(new { rs = false, msg = "Lỗi lấy dữ liệu để thêm mới." });
                    }
                    CacheHelper.Set("ListCustomers", importCustomers);
                    return new JsonResult()
                    {
                        Data = new { ListCustomers = importCustomers},
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        MaxJsonLength = Int32.MaxValue
                    };
                }
                return Json(new { rs = false, msg = "File không đúng chuẩn, vui lòng tải file Excel mẫu kế bên" });
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy dữ liệu để thêm mới.", objEx, MethodBase.GetCurrentMethod().Name, "PreiviewInoiceData");
                return Json(new { rs = false, msg = "Lỗi lấy dữ liệu để thêm mới." });
            }
        }

        /// <summary>
        /// tuyennv 20200706
        /// Nhập khẩu khách hàng
        /// </summary>
        /// <param name="invoices"></param>
        /// <returns></returns>
        public ActionResult ImportDataCustomer()
        {
            var lstCustomers = CacheHelper.Get("ListCustomers");
            if (lstCustomers == null)
                Json(new { rs = false, msg = "Đã hết phiên làm việc. Bạn bấm <b>Đăng nhập lại</b> để trở về trang đăng nhập." });
            List<CustomerBO> customers = (List<CustomerBO>)lstCustomers;
            if (customers == null)
                Json(new { rs = false, msg = "Đã hết phiên làm việc. Bạn bấm <b>Đăng nhập lại</b> để trở về trang đăng nhập." });

            string msg = SaveDataImport(customers);
            if (msg.Length > 0)
                return Json(new { rs = true, msg = msg });
            return Json(new { rs = false, msg = "Nhập khẩu khách hàng không thành công, liên hệ với quản trị để được hỗ trợ." });
        }

        /// <summary>
        /// tuyennv 20200706
        /// Lưu thông tin khách hàng
        /// </summary>
        /// <param name="invoices"></param>
        /// <returns></returns>
        public string SaveDataImport(List<CustomerBO> customers)
        {
            string msg = string.Empty;
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                foreach (var item in customers)
                    item.COMTAXCODE = objUser.COMTAXCODE;

                //Thêm khách hàng
                serviceResult = SaveDataCustomer(customers, out Dictionary<string, CustomerBO> dicResultInvoices, out Dictionary<int, CustomerBO> dicImportInvoiceError);
                msg = serviceResult.Message;
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "SaveDataImport");
                msg = "Lỗi thêm mới khách hàng.";
            }
            return msg;
        }

        /// <summary>
        /// tuyennv 20201208
        /// Cất dữ liệu hóa đơn
        /// </summary>
        /// <returns></returns>
        private ServiceResult SaveDataCustomer(List<CustomerBO> customers, out Dictionary<string, CustomerBO> dicResultCustomer, out Dictionary<int, CustomerBO> dicImportCustomerError)
        {
            ServiceResult serviceResult = new ServiceResult();
            CustomerBLL customerBLL = new CustomerBLL();
            string msg = string.Empty;
            dicResultCustomer = new Dictionary<string, CustomerBO>();
            dicImportCustomerError = new Dictionary<int, CustomerBO>();
            int totalCount = customers.Count;
            int insertRowcount = 0;
            try
            {
                ProgressViewModel model = new ProgressViewModel();
                model.TotalRow = totalCount;
                int startRow = 0;
                int taskNum = 20;
                int qtyperTask = 0;
                CalTaskNumber(totalCount, ref qtyperTask, ref taskNum);
                for (int i = 0; i < taskNum; i++)
                {
                    int tempI = i;
                    var items = customers.Skip(tempI * qtyperTask).Take(qtyperTask).ToList();
                    var task = Task.Run(() =>
                    {
                        foreach (var item in items)
                        {
                            bool result = customerBLL.AddCustomer(item);
                            if (result)
                                startRow++;
                            insertRowcount = insertRowcount + 1;
                            model.CurrentRow = startRow;
                            model.IsSuccess = startRow == model.TotalRow;
                            _hubContext.Clients.Group(objUser.USERNAME).newMessageReceivedCustomer(model);
                        }
                    });
                }

                //foreach (var cus in customers)
                //{
                //    bool result = customerBLL.AddCustomer(cus);
                //    startRow++;
                //    if (!result)
                //        dicImportCustomerError.Add(startRow, cus);
                //    else
                //    {
                //        insertRowcount++;
                //        msg = $"Thêm mới thành công <b>{insertRowcount}/{totalCount}</b> khách hàng.";
                //    }
                //}
                msg = $"Thêm mới thành công <b>{insertRowcount}/{totalCount}</b> khách hàng.";
                serviceResult.ErrorCode = 0;
                serviceResult.Message = msg;
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "SaveDataCustomer");
                msg = "Lỗi khi thêm mới khách hàng.";
                serviceResult.ErrorCode = 1000;
                serviceResult.Message = msg;
            }
            return serviceResult;
        }
        #endregion
    }
}