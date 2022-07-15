using DS.BusinessLogic.Company;
using DS.BusinessLogic.Customer;
using DS.BusinessLogic.Invoice;
using DS.BusinessLogic.Number;
using DS.BusinessLogic.Product;
using DS.BusinessObject.API;
using DS.BusinessObject.Customer;
using DS.BusinessObject.Invoice;
using DS.BusinessObject.Output;
using DS.BusinessObject.Product;
using DS.Common.Helpers;
using Newtonsoft.Json;
using SPA_Invoice.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using static DS.Common.Enums.EnumHelper;
using static Invoice.API.Enumeration.EnumCommon;

namespace Invoice.API.Controllers
{
    [System.Web.Http.RoutePrefix("api/onfinance")]
    public class InvoiceController : BaseApiController
    {
        /// <summary>
        /// Khai báo biến dùng chung chứa ID trả về
        /// </summary>
        private object objectId = null;

        /// <summary>
        /// Danh sách column không kiểm tra giá trị
        /// </summary>
        private List<string> igonreColumns = new List<string>() { "payment_method", "TAXRATE", "currency_unit", "DISCOUNTRATE", "producttype" };

        /// <summary>
        /// Danh mục tỷ giá loại tiền tệ
        /// </summary>
        private Dictionary<string, decimal> dicCurrency = new Dictionary<string, decimal>() { { "VND", 1 }, { "USD", 23220 } };

        /// <summary>
        /// URL: https://api.onfinance.asia/invoice/v21/createinvoice
        /// Tạo hóa đơn 
        /// truongnv 20200220
        /// </summary>
        /// <returns>trả ra object thông tin</returns>
        [System.Web.Http.HttpPost]
        public ServiceResult TaoHoadon()
        {
            string msg = string.Empty;
            //Cấu hình các thông tin bắt buộc cần kiểm tra kiểu dữ liệu đầu vào
            List<string> columnRequired = new List<string>() { "secret", "partner_id", "formcode", "symbolcode", "payment_method", "products" };
            InvoiceAPIBO objInvoice = null;
            ServiceResult oResult = new ServiceResult();
            InvoiceBO objInvoiceInsert = new InvoiceBO();
            try
            {
                var jsonInput = new StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd();

                objInvoice = JsonConvert.DeserializeObject<InvoiceAPIBO>(jsonInput);

                jsonInput = MethodHelper.ReadDataFile("Invoicetest.json");
                // Chuyển đổi json to object
                objInvoice = JsonConvert.DeserializeObject<InvoiceAPIBO>(jsonInput);
                if (objInvoice != null)
                {
                    //Kiểm tra xem có tồn tại đối tác hay không
                    msg = CheckExitsPartner(objInvoice.partner_id, objInvoice.secret);
                    if (msg.Length > 0)
                    {
                        msg = $"Đối tác không tồn tại trên hệ thống Onfinance với PartnerID {objInvoice.partner_id}.";
                        SetMessageResult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                        return oResult;
                    }

                    // Kiểm tra tính hợp lệ của dữ liệu
                    msg = CheckValueByType(objInvoice, columnRequired);
                    if (msg.Length > 0)
                    {
                        SetMessageResult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                        return oResult;
                    }

                    var inv = new InvoiceBO();

                    //Mapping data trước khi save
                    msg = BeforeSave(objInvoice, ref inv);
                    if (msg.Length > 0)
                    {
                        SetMessageResult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                        return oResult;
                    }

                    // Lấy thông tin doanh nghiệp
                    CompanyBLL companyBLL = new CompanyBLL();
                    var objCompany = companyBLL.GetCompanyInfoByID(objInvoice.partner_id, objInvoice.company);
                    if (objCompany == null)
                    {
                        SetMessageResult(objectId, (int)ErrorCode.Fail, "Không tìm thấy thông tin doanh nghiệp trong hệ thống.", ref oResult);
                        return oResult;
                    }

                    inv.COMTAXCODE = objCompany.COMTAXCODE;
                    inv.COMNAME = objCompany.COMNAME;
                    inv.COMADDRESS = objCompany.COMADDRESS;
                    inv.COMPHONENUMBER = objCompany.COMPHONENUMBER;

                    //kiểm tra dữ liệu trước khi save
                    msg = ValidateBeforSave(inv, ref oResult);
                    if (msg.Length > 0)
                    {
                        SetMessageResult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                        return oResult;
                    }

                    InvoiceBLL invoiceBLL = new InvoiceBLL();
                    long invoiceId = invoiceBLL.AddInvoice(inv);
                    if (invoiceId <= 0)
                    {
                        msg = $"Không tạo được hóa đơn với PartnerID {objInvoice.partner_id}.";
                        SetMessageResult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                        return oResult;
                    }
                    else
                    {
                        SetMessageResult(invoiceId, (int)ErrorCode.Success, msg, ref oResult);
                    }
                }
                else
                {
                    msg = $"Không lấy được thông tin đầu vào của hóa đơn với PartnerID {objInvoice.partner_id}.";
                    SetMessageResult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                    return oResult;
                }
            }
            catch (Exception ex)
            {
                //msg = $"Không thêm được hóa đơn với PartnerID {objInvoice.partner_id}";
                //SetMessageReult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                //ConfigHelper.Instance.WriteLog(msg, ex, MethodBase.GetCurrentMethod().Name, "Createinvoice");
            }
            return oResult;
        }

        /// <summary>
        /// URL: https://api.onfinance.asia/invoice/v21/signeinvoice
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ProductBO Signeinvoice()
        {
            var json = new StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd();
            return new ProductBO()
            {

            };
        }

        /// <summary>
        /// URL: https://api.onfinance.asia/invoice/v21/sendmaileinvoice
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ProductBO Sendmaileinvoice()
        {
            var json = new StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd();
            return new ProductBO()
            {

            };
        }

        /// <summary>
        /// URL: https://api.onfinance.asia/invoice/v21/addcustomers
        /// Thêm khách hàng vào hệ thống
        /// truongnv 20200221
        /// </summary>
        /// <returns>trả về thông tin thành công/thất bại</returns>
        [System.Web.Http.HttpPost]
        public ServiceResult Addcustomers()
        {
            string msg = string.Empty;

            CustomerAPIBO objCustomer = null;
            ServiceResult oResult = new ServiceResult();
            try
            {
                var jsonInput = new StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd();
                jsonInput = MethodHelper.ReadDataFile("CustomerTest.json");
                // Chuyển đổi json to object
                objCustomer = JsonConvert.DeserializeObject<CustomerAPIBO>(jsonInput);
                if (objCustomer != null)
                {
                    //Kiểm tra xem có tồn tại đối tác hay không
                    msg = CheckExitsPartner(objCustomer.partner_id, objCustomer.secret);
                    if (msg.Length > 0)
                    {
                        msg = $"Đối tác không tồn tại trên hệ thống Onfinance với PartnerID {objCustomer.partner_id}.";
                        SetMessageResult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                        return oResult;
                    }

                    // Nếu không có danh sách khách hàng
                    if (objCustomer.customers == null || objCustomer.customers.Count == 0)
                    {
                        SetMessageResult(objectId, (int)ErrorCode.Fail, "Thông tin khách hàng không được trống (customers is null).", ref oResult);
                        return oResult;
                    }

                    // Lấy thông tin doanh nghiệp
                    CompanyBLL companyBLL = new CompanyBLL();
                    var objCompany = companyBLL.GetDataCompanyByID(objCustomer.partner_id);
                    if (objCompany == null)
                    {
                        SetMessageResult(objectId, (int)ErrorCode.Fail, "Không tìm thấy thông tin doanh nghiệp trong hệ thống.", ref oResult);
                        return oResult;
                    }

                    //Danh sách column yêu cầu bắt buộc nhập dữ liệu
                    List<string> columnRequired = new List<string>() { "secret", "partner_id", "cusname" };
                    CustomerBO customer = null;
                    int insertRowCount = 0;
                    int totalRowCount = objCustomer.customers.Count;
                    for (int i = 0; i < totalRowCount; i++)
                    {
                        customer = new CustomerBO();
                        // Kiểm tra tính hợp lệ của dữ liệu khách hàng
                        msg = CheckValueByType(objCustomer.customers[i], columnRequired);
                        if (msg.Length > 0)
                        {
                            SetMessageResult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                            return oResult;
                        }

                        //Mapping data trước khi save
                        msg = BeforeSave(objCustomer.customers[i], ref customer);
                        if (msg.Length > 0)
                        {
                            SetMessageResult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                            return oResult;
                        }

                        customer.COMTAXCODE = objCompany.COMTAXCODE;
                        customer.COMADDRESS = objCompany.COMADDRESS;

                        //kiểm tra dữ liệu trước khi save
                        msg = ValidateBeforSave(customer, ref oResult);
                        if (msg.Length > 0)
                        {
                            SetMessageResult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                            return oResult;
                        }

                        CustomerBLL customerBLL = new CustomerBLL();
                        bool success = customerBLL.AddCustomer(customer);
                        if (!success)
                        {
                            oResult.Message = $"Không thêm được khách hàng với PartnerID {objCustomer.partner_id}";
                            SetMessageResult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                            return oResult;
                        }
                        else
                        {
                            insertRowCount++;
                            msg = $"Tạo mới khách hàng thành công {insertRowCount} / {totalRowCount}.";
                            SetMessageResult(objectId, (int)ErrorCode.Success, msg, ref oResult);
                        }
                    }
                }
                else
                {
                    msg = $"Không lấy được thông tin đầu vào của khách hàng với PartnerID {objCustomer.partner_id}.";
                    SetMessageResult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                }
            }
            catch (Exception ex)
            {
                //msg = $"Không thêm được khách hàng với PartnerID {objCustomer.partner_id}";
                //SetMessageReult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                //ConfigHelper.Instance.WriteLog(msg, ex, MethodBase.GetCurrentMethod().Name, "AddCustomer");
            }
            return oResult;
        }

        /// <summary>
        /// URL: https://api.onfinance.asia/invoice/v21/addproducts
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ServiceResult Addproducts()
        {
            string msg = string.Empty;

            ProductAPIBO objProduct = null;
            ServiceResult oResult = new ServiceResult();
            try
            {
                CurrencyBO objCurrency = CurrencyConversion();

                var jsonInput = new StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd();
                jsonInput = MethodHelper.ReadDataFile("Product.json");
                // Chuyển đổi json to object
                objProduct = JsonConvert.DeserializeObject<ProductAPIBO>(jsonInput);
                if (objProduct != null)
                {
                    //Kiểm tra xem có tồn tại đối tác hay không
                    msg = CheckExitsPartner(objProduct.partner_id, objProduct.secret);
                    if (msg.Length > 0)
                    {
                        msg = $"Đối tác không tồn tại trên hệ thống Onfinance với PartnerID {objProduct.partner_id}.";
                        SetMessageResult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                        return oResult;
                    }

                    // Nếu không có danh sách khách hàng
                    if (objProduct.products == null || objProduct.products.Count == 0)
                    {
                        SetMessageResult(objectId, (int)ErrorCode.Fail, "Thông tin hàng hóa không được trống (products is null).", ref oResult);
                        return oResult;
                    }

                    // Lấy thông tin doanh nghiệp
                    CompanyBLL companyBLL = new CompanyBLL();
                    var objCompany = companyBLL.GetDataCompanyByID(objProduct.partner_id);
                    if (objCompany == null)
                    {
                        SetMessageResult(objectId, (int)ErrorCode.Fail, "Không tìm thấy thông tin doanh nghiệp trong hệ thống.", ref oResult);
                        return oResult;
                    }

                    //Danh sách column yêu cầu bắt buộc nhập dữ liệu
                    List<string> columnRequired = new List<string>() { "secret", "partner_id", "productname", "price" };
                    ProductBO product = null;
                    int insertRowCount = 0;
                    int totalRowCount = objProduct.products.Count;
                    for (int i = 0; i < totalRowCount; i++)
                    {
                        product = new ProductBO();
                        // Kiểm tra tính hợp lệ của dữ liệu khách hàng
                        msg = CheckValueByType(objProduct.products[i], columnRequired);
                        if (msg.Length > 0)
                        {
                            SetMessageResult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                            return oResult;
                        }

                        //Mapping data trước khi save
                        msg = BeforeSave(objProduct.products[i], ref product);
                        if (msg.Length > 0)
                        {
                            SetMessageResult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                            return oResult;
                        }

                        product.COMTAXCODE = objCompany.COMTAXCODE;
                        product.COMNAME = objCompany.COMNAME;
                        product.COMADDRESS = objCompany.COMADDRESS;

                        //kiểm tra dữ liệu trước khi save
                        msg = ValidateBeforSave(product, ref oResult);
                        if (msg.Length > 0)
                        {
                            SetMessageResult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                            return oResult;
                        }

                        ProductBLL productBLL = new ProductBLL();
                        bool success = productBLL.AddProduct(product);
                        if (!success)
                        {
                            oResult.Message = $"Không thêm được hàng hóa với PartnerID {objProduct.partner_id}";
                            SetMessageResult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                            return oResult;
                        }
                        else
                        {
                            insertRowCount++;
                            msg = $"Tạo mới hàng hóa thành công {insertRowCount} / {totalRowCount}.";
                            SetMessageResult(objectId, (int)ErrorCode.Success, msg, ref oResult);
                        }
                    }
                }
                else
                {
                    msg = $"Không lấy được thông tin đầu vào của hàng hóa với PartnerID {objProduct.partner_id}.";
                    SetMessageResult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                }
            }
            catch (Exception ex)
            {
                //msg = $"Không thêm được hàng hóa với PartnerID {objProduct.partner_id}";
                //SetMessageReult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                //ConfigHelper.Instance.WriteLog(msg, ex, MethodBase.GetCurrentMethod().Name, "AddProduct");
            }
            return oResult;
        }
        #region Sumary total invoice
        /// <summary>
        /// Lấy ra danh sách tình hình sử dụng hóa đơn
        /// </summary>
        /// <param name="comtaxCode"></param>
        /// <param name="usingInvoiceType"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("getStatusInvoices")]
        public HttpResponseMessage GetStatusInvoices(string comtaxCode, int? usingInvoiceType)
        {
            InvoiceSearchFormBO obj = new InvoiceSearchFormBO();
            obj.COMPANYNAME = comtaxCode;
            obj.STATUS = usingInvoiceType;
            InvoiceBLL invoiceBLL = new InvoiceBLL();
            
            var invoices = invoiceBLL.Getforstatistics(obj);//còn số
            
            return Request.CreateResponse(HttpStatusCode.OK, new { message = invoices });
        }
        #endregion
        #region Close Invoice
        /// <summary>
        /// Chuyển hóa đơn thành hóa đơn hủy
        /// { "id": 1, "cancelreason": "cancel", "canceltime": "01/01/2020" }
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("cancelInvoice")]
        public HttpResponseMessage CancelInvoice(HttpRequestMessage request)
        {
            InvoiceBLL invoiceBLL = new InvoiceBLL();
            var requestData = request.Content.ReadAsStringAsync().Result;
            var objData = JsonConvert.DeserializeObject<InvoiceBO>(requestData);

            var invoice = invoiceBLL.GetInvoiceByIdAPI(objData.ID);
            if (invoice == null)
                return Request.CreateResponse(HttpStatusCode.OK, new { message = $"Không tìm thấy hóa đơn {objData.ID}." });
            invoice.CANCELREASON = objData.CANCELREASON;
            invoice.CANCELTIME = objData.CANCELTIME;
            invoice.INVOICETYPE = (int)INVOICE_TYPE.CANCEL;
            bool result = invoiceBLL.UpdateCancelInvoice(invoice);
            string message = result == false ? $"Hệ thống không hủy được hóa đơn {objData.ID}. Xin vui lòng liên hệ với admin" : $"Hủy thành công hóa đơn {objData.ID}";
            return Request.CreateResponse(HttpStatusCode.OK, new { message = message });
        }

        //public HttpResponseMessage CancelInvoice(long invoiceId)
        //{
        //    InvoiceBLL invoiceBLL = new InvoiceBLL();
        //    var invoice=invoiceBLL.GetInvoiceById(invoiceId);
        //    if(invoice==null)
        //        return Request.CreateResponse(HttpStatusCode.OK, new { message = $"Không tìm thấy hóa đơn {invoiceId}." });
        //    invoice.INVOICETYPE = (int)INVOICE_TYPE.CANCEL;
        //    bool result= invoiceBLL.UpdateCancelInvoice(invoice);
        //    string message = result == false ? $"Hệ thống không hủy được hóa đơn {invoiceId}. Xin vui lòng liên hệ với admin" : $"Hủy thành công hóa đơn {invoiceId}";
        //    return Request.CreateResponse(HttpStatusCode.OK, new { message = message });
        //}
        #endregion

        #region Save Invoice
        /// <summary>
        /// Kiểm tra dữ liệu trước khi cất
        /// </summary>
        /// <param name="objInvoice"></param>
        /// <param name="oResult"></param>
        /// <returns></returns>
        private string ValidateBeforSave(InvoiceBO objInvoice, ref ServiceResult oResult)
        {
            string msg = string.Empty;
            try
            {
                // Kiểm tra khách hàng có tồn tại không
                CustomerBLL customerBLL = new CustomerBLL();
                msg = customerBLL.CheckCustomerDuplicateTaxCode(new CustomerBO() { CUSTAXCODE = objInvoice.CUSTAXCODE, CUSNAME = objInvoice.CUSNAME, COMTAXCODE = objInvoice.COMTAXCODE });
                if (msg.Length == 0)
                {
                    SetMessageResult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                    return msg;
                }

                CompanyBLL companyBLL = new CompanyBLL();
                //Kiểm tra xem hóa đơn đã phát hành chưa? 
                msg = companyBLL.CheckInvoiceRelease(objInvoice.COMTAXCODE, objInvoice.FORMCODE, objInvoice.SYMBOLCODE);
                if (msg.Length > 0)
                {
                    //B3: Kiểm tra xem hóa đơn còn số phát hành không?
                    msg = companyBLL.CheckInvoiceNumberRelease(objInvoice.COMTAXCODE, objInvoice.FORMCODE, objInvoice.SYMBOLCODE);
                    if (msg.Length > 0)
                    {
                        SetMessageResult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                        return msg;
                    }
                }

                // Kiểm tra Email đúng định dạng chưa
                msg = CommonFunction.ValidateEmail(objInvoice.CUSEMAIL);
                if (msg.Length > 0)
                {
                    SetMessageResult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                    return msg;
                }
            }
            catch (Exception ex)
            {
                msg = $"Lỗi kiểm tra dữ liệu trước khi cất. methodName (ValidateBeforSave).";
            }
            return msg;
        }

        /// <summary>
        /// Kiểm tra thông tin trước khi cất hóa đơn vào hệ thống
        /// </summary>
        /// <param name="objInvoice">Đối tượng hóa đơn</param>
        /// <param name="invoice">Đối tượng hóa đơn trả ra</param>
        /// <returns></returns>
        private string BeforeSave(object objectType, ref InvoiceBO invoice)
        {
            string msg = string.Empty;
            //Cấu hình các cột thông tin lấy dạng danh mục
            List<string> lstColumnDic = new List<string>() { "payment_method" };
            try
            {
                // Lấy thông tin mapping property
                string data = CommonFunction.ReadDataFile("DictionaryInvoice.json");
                if (!string.IsNullOrWhiteSpace(data))
                {
                    Dictionary<string, object> keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                    PropertyInfo[] propertyInfos = objectType.GetType().GetProperties();
                    foreach (var pi in propertyInfos)
                    {
                        string columnName = pi.Name;
                        if (keyValuePairs.ContainsKey(columnName))
                        {
                            var propertyType = pi.PropertyType;
                            TypeCode typeCode = Type.GetTypeCode(propertyType);
                            object value = pi.GetValue(objectType);

                            //Nếu là object
                            if (typeCode == TypeCode.Object && value != null)
                            {
                                List<InvoiceDetailBO> objProductDetails = new List<InvoiceDetailBO>();
                                msg = SetDataDetail(value, keyValuePairs, ref objProductDetails);
                                if (msg.Length > 0)
                                    return msg;

                                invoice.LISTPRODUCT = objProductDetails;
                            }
                            else
                            {
                                if (lstColumnDic.Contains(columnName.ToLower()))
                                {
                                    object vVal = GetDataCategories(value, columnName.ToLower());
                                    if (vVal == null || vVal.ToString() == "")
                                    {
                                        msg = $"Không tìm thấy thông tin danh mục trong hệ thống. Column {columnName}.";
                                        return msg;
                                    }

                                    CommonFunction.SetPropertyValue(invoice, keyValuePairs[columnName].ToString(), vVal);
                                }
                                else
                                    CommonFunction.SetPropertyValue(invoice, keyValuePairs[columnName].ToString(), value);
                            }
                        }
                    }
                    //Gán thông tin mặc định cho bảng pm_invoice
                    invoice.INVOICESTATUS = 1; //mặc định hóa đơn chưa phát hành
                    invoice.INVOICETYPE = 1; // mặc định hóa đơn gốc
                    invoice.INVOICETYPENAME = "Hóa đơn gốc";
                    invoice.INITTIME = DateTime.Now;
                    invoice.CURRENCY = invoice.CURRENCY == "1" ? "VND" : "USD";
                    invoice.EXCHANGERATE = dicCurrency[invoice.CURRENCY]; //Tỉ lệ giá so với tiền VND tại thời điểm tạo

                    decimal taxmoney = 0;
                    decimal totalmoney = 0;
                    decimal discountmoney = 0;
                    decimal totalpayment = 0;
                    msg = CalculatorMoney(invoice.LISTPRODUCT, invoice.CURRENCY, ref taxmoney, ref totalmoney, ref discountmoney, ref totalpayment);
                    if (msg.Length > 0)
                        return msg;

                    invoice.DISCOUNTTYPE = "KHONG_CO_CHIET_KHAU";
                    invoice.TAXMONEY = taxmoney;//Tiền thuế = Đơn giá * số lượng * thuế suất;
                    invoice.DISCOUNTMONEY = discountmoney; //Tiền chiết khấu = Tổng tiền * % chiết khấu
                    invoice.TOTALMONEY = totalmoney; //Tổng tiền = Đơn giá * số lượng
                    invoice.TOTALPAYMENT = totalpayment; //Tổng tiền thanh toán = Tổng tiền - Tiền chiết khấu + Tiền thuế
                    invoice.REFERENCECODE = ReferenceCode.GenerateReferenceCode();
                    string fileName = invoice.COMTAXCODE + "_" + invoice.ID + "_" + invoice.INVOICETYPE + ".pdf";
                    invoice.SIGNLINK = GetFilePath(invoice, fileName);
                }
            }
            catch (Exception ex)
            {
                msg = $"Lỗi tạo hóa đơn. methodName (BeforeSave).";
            }
            return msg;
        }

        /// <summary>
        ///Lấy fileName 
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetFilePath(InvoiceBO invoice, string fileName)
        {
            string pathName = string.Empty;
            try
            {
                var root = ConfigurationManager.AppSettings["InputInvoiceFolder"];
                var branchComTaxCode = "/" + (string.IsNullOrEmpty(invoice.COMTAXCODE) ? "COMTAXCODE/" : invoice.COMTAXCODE + "/");
                var branchInvoiceID = string.IsNullOrEmpty(invoice.ID.ToString()) ? "INVOICEID/" : invoice.ID.ToString() + "/";
                var branchInvoiceType = string.IsNullOrEmpty(invoice.INVOICETYPENAME.ToString()) ? "INVOICETYPENAME/" : ElaHelper.FormatKeywordSearch(ElaHelper.FilterVietkey(invoice.INVOICETYPENAME.ToString())).Replace(" ", "-");
                var branchDate = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                var path = string.Format($"{root}{branchComTaxCode}{branchInvoiceID}{branchInvoiceType}");

                DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/" + path));
                if (!dir.Exists)
                {
                    dir.Create();
                }

                string filePath = dir + "\\" + fileName;
                pathName = filePath.Replace(HttpContext.Current.Server.MapPath("~/" + root), "").Replace('\\', '/');
            }
            catch
            {
                pathName = string.Empty;
            }
            return pathName;
        }

        /// <summary>
        /// Tính tiền hóa đơn
        /// </summary>
        /// <param name="invoiceDetails"></param>
        /// <param name="taxmoney"></param>
        /// <param name="totalmoney"></param>
        /// <param name="discountmoney"></param>
        /// <param name="totalpayment"></param>
        /// <returns></returns>
        private string CalculatorMoney(List<InvoiceDetailBO> invoiceDetails, string curencyStr, ref decimal taxmoney, ref decimal totalmoney, ref decimal discountmoney, ref decimal totalpayment)
        {
            string msg = string.Empty;
            decimal txmoney = 0;
            decimal ttmoney = 0;
            decimal dscountmoney = 0;
            decimal ttpayment = 0;
            decimal exchangerate = dicCurrency[curencyStr];
            int globalTaxtate = 0;
            try
            {
                foreach (var item in invoiceDetails)
                {
                    var totalMoney = item.QUANTITY * item.RETAILPRICE * exchangerate;
                    var discountMoney = item.DISCOUNTRATE * totalMoney / 100;

                    if (!item.ISPROMOTION)
                    {
                        ttmoney += item.QUANTITY * item.RETAILPRICE;
                        dscountmoney += item.DISCOUNTRATE * (item.QUANTITY * item.RETAILPRICE * exchangerate / 100);
                        if (item.TAXRATE == 1)
                        {
                            txmoney += (totalMoney - discountMoney) * globalTaxtate / 100;
                        }
                        else
                        {
                            txmoney += (totalMoney - discountMoney) * (item.TAXRATE == -1 ? 0 : item.TAXRATE) / 100;
                        }
                    }
                }
                ttpayment = ttmoney - dscountmoney + txmoney;
                taxmoney = txmoney;
                totalmoney = ttmoney;
                discountmoney = dscountmoney;
                totalpayment = ttpayment;
            }
            catch (Exception ex)
            {
                msg = "Lỗi tính Tiền thuế và tổng tiền cho bảng hóa đơn.";
            }
            return msg;
        }

        /// <summary>
        /// Gán giá trị cho thuộc tính của danh sách detail
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="keyValuePairs"></param>
        /// <param name="invoiceDetails"></param>
        /// <returns></returns>
        private string SetDataDetail(object objectType, Dictionary<string, object> keyValuePairs, ref List<InvoiceDetailBO> invoiceDetails)
        {
            string msg = string.Empty;
            InvoiceDetailBO invoiceDetail = new InvoiceDetailBO();
            try
            {
                Dictionary<string, string> pairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(keyValuePairs["products"].ToString());
                List<InvoiceDetailAPIBO> detailBOs = (List<InvoiceDetailAPIBO>)objectType;
                for (int i = 0; i < detailBOs.Count; i++)
                {
                    invoiceDetail = new InvoiceDetailBO();
                    // Lấy thông tin mapping property
                    PropertyInfo[] propertyInfos = detailBOs[i].GetType().GetProperties();
                    foreach (var pi in propertyInfos)
                    {
                        string columnName = pi.Name;
                        if (pairs.ContainsKey(columnName))
                        {
                            var propertyType = pi.PropertyType;
                            object value = pi.GetValue(detailBOs[i]);
                            CommonFunction.SetPropertyValue(invoiceDetail, pairs[columnName], value);
                        }
                    }
                    //Gán thêm các trường thông tin 
                    invoiceDetail.INITTIME = DateTime.Now;
                    invoiceDetail.TAXRATE = -1; // mặc định không chịu thuế
                    invoiceDetail.DISCOUNTRATE = 0; // mặc định không chịu thuế => % chiết khấu = 0;
                                                    //Add object to list
                    invoiceDetails.Add(invoiceDetail);
                }
            }
            catch (Exception ex)
            {
                msg = $"Lỗi tạo hóa đơn. methodName (SetDataDetail).";
            }
            return msg;
        }

        /// <summary>
        /// Lấy giá trị trong danh mục
        /// </summary>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private object GetDataCategories(object value, string propertyName)
        {
            object oResult = null;
            InvoiceBLL invoiceBLL = new InvoiceBLL();
            try
            {
                int outValue = CommonFunction.NullSafeInteger(value, 0);
                switch (propertyName)
                {
                    case "payment_method":
                    case "cuspaymentmethod":
                        oResult = invoiceBLL.GetDataCategories(outValue, "ds_masterdata.pm_common_payment_method_getcategories_by_id");
                        break;
                    case "currency_unit":
                        oResult = invoiceBLL.GetDataCategories(outValue, "ds_masterdata.pm_common_currency_unit_getcategories_by_id");
                        break;
                    case "quantityunit":
                        oResult = invoiceBLL.GetDataCategories(outValue, "ds_masterdata.pm_common_quantity_unit_getcategories_by_id");
                        break;
                }
            }
            catch (Exception ex)
            {
                oResult = null;
            }
            return oResult;
        }

        /// <summary>
        /// Kiểm tra thông tin giá trị hợp lệ của các property trong object
        /// </summary>
        /// <param name="value"></param>
        /// <param name="typeCode"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        private string CheckValueByType(object obj, List<string> columnRequired)
        {
            object defaultValue = 0;
            string msg = string.Empty;
            List<string> columnDetailRequired = new List<string>() { "price" };
            try
            {
                PropertyInfo[] propertyInfos = obj.GetType().GetProperties();
                foreach (var pi in propertyInfos)
                {
                    string columnName = pi.Name;
                    Type propertyType = pi.PropertyType;
                    TypeCode typeCode = Type.GetTypeCode(propertyType);
                    object value = pi.GetValue(obj);

                    //Kiểm tra trường thông tin bắt buộc
                    if (columnRequired.Contains(columnName.ToLower()))
                    {
                        if (value == null)
                        {
                            msg = $"Thông tin cột {columnName} với kiểu dữ liệu {typeCode} không được để trống.";
                            return msg;
                        }
                    }

                    msg = CheckDataByTypeCode(typeCode, columnName, value, columnDetailRequired);
                    if (msg.Length > 0)
                        return msg;
                }
            }
            catch (Exception ex)
            {
                msg = $"Lỗi kiểm tra dữ liệu đầu vào master. methodName (CheckValueByType).";
            }

            return msg;
        }

        /// <summary>
        /// Kiểm tra thông tin giá trị hợp lệ của các property trong object
        /// </summary>
        /// <param name="value"></param>
        /// <param name="typeCode"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        private string CheckDetailValueByType(object obj, List<string> columnRequired)
        {
            string msg = string.Empty;
            List<string> columnDetailRequired = new List<string>() { };
            try
            {
                List<InvoiceDetailAPIBO> detailBOs = (List<InvoiceDetailAPIBO>)obj;
                for (int i = 0; i < detailBOs.Count; i++)
                {
                    PropertyInfo[] propertyInfos = detailBOs[i].GetType().GetProperties();
                    foreach (var pi in propertyInfos)
                    {
                        string columnName = pi.Name;
                        Type propertyType = pi.PropertyType;
                        TypeCode typeCode = Type.GetTypeCode(propertyType);
                        object value = pi.GetValue(detailBOs[i]);

                        //Kiểm tra trường thông tin bắt buộc
                        if (columnRequired.Contains(columnName.ToLower()))
                        {
                            if (value == null)
                            {
                                msg = $"Thông tin cột {columnName} với kiểu dữ liệu {typeCode} không được để trống.";
                                return msg;
                            }
                        }

                        msg = CheckDataByTypeCode(typeCode, columnName, value, columnDetailRequired);
                        if (msg.Length > 0)
                            return msg;
                    }
                }
            }
            catch (Exception ex)
            {
                msg = $"Lỗi kiểm tra dữ liệu đầu vào master. methodName (CheckDetailValueByType).";
            }
            return msg;
        }

        /// <summary>
        /// Kiểm tra dữ liệu đầu vào bằng TypeCode
        /// </summary>
        /// <param name="typeCode">kiểu dữ liệu</param>
        /// <param name="columnName">tên cột</param>
        /// <param name="value">giá trị</param>
        /// <param name="columnDetailRequired">Cấu hình các cột thông tin cần yêu cầu bắt buộc</param>
        /// <returns></returns>
        private string CheckDataByTypeCode(TypeCode typeCode, string columnName, object value, List<string> columnDetailRequired)
        {
            string msg = string.Empty;
            switch (typeCode)
            {
                case TypeCode.Object:
                    if (value != null)
                    {
                        msg = CheckDetailValueByType(value, columnDetailRequired);
                        if (msg.Length > 0)
                            return msg;
                    }
                    break;
                case TypeCode.Int32:
                    int outValue = CommonFunction.NullSafeInteger(value, -1);
                    if (outValue <= 0 && !igonreColumns.Contains(columnName))
                    {
                        msg = $"Giá trị cột {columnName} với kiểu dữ liệu {typeCode} phải lớn hơn 0 và là số nguyên dương.";
                        return msg;
                    }
                    break;
                case TypeCode.Double:
                    double dValue = CommonFunction.NullSafeDouble(value, -1);
                    if (dValue < 0)
                    {
                        msg = $"Giá trị cột {columnName} với kiểu dữ liệu {typeCode} phải lớn hơn hoặc bằng 0.";
                        return msg;
                    }
                    break;
                case TypeCode.Decimal:
                    decimal dcValue = CommonFunction.NullSafeDecimal(value, -1);
                    if (dcValue < 0)
                    {
                        msg = $"Giá trị cột {columnName} với kiểu dữ liệu {typeCode} phải lớn hơn hoặc bằng 0.";
                        return msg;
                    }
                    break;
                case TypeCode.DateTime:
                    DateTime dTime = CommonFunction.NullSafeDateTime(value, DateTime.MinValue);
                    if (dTime == DateTime.MinValue)
                    {
                        msg = $"Giá trị cột {columnName} với kiểu dữ liệu {typeCode} sai định dạng kiểu ngày tháng.";
                        return msg;
                    }
                    break;
            }
            return msg;
        }

        /// <summary>
        /// Gán thông tin trả về cho API khi thực hiện các hành động
        /// truongnv 20200221
        /// </summary>
        /// <param name="objectId">Giá trị trả về</param>
        /// <param name="errorCode">Mã lỗi</param>
        /// <param name="message">Thông báo</param>
        /// <param name="oResult">đối tượng trả về</param>
        private void SetMessageResult(object objectId, int errorCode, string message, ref ServiceResult oResult)
        {
            oResult.ObjectID = objectId;
            oResult.ErrorCode = errorCode;
            oResult.Message = message;
        }

        /// <summary>
        /// Kiểm tra xem đối tác có tồn tại trên hệ thống Onfinance không?
        /// truongnv 20200221
        /// </summary>
        /// <param name="partnerId">Mã đối tác</param>
        /// <param name="secret">key kiểm tra</param>
        /// <returns>trả ra thông báo thành công/thất bại</returns>
        private string CheckExitsPartner(int partnerId, string secret)
        {
            string msg = string.Empty;
            try
            {
                //deserialize JSON from file  
                string Json = MethodHelper.ReadDataFile("key.json");
                List<Partner> objPartner = JsonConvert.DeserializeObject<List<Partner>>(Json);
                /*
                 * Kiểm tra xem có tồn tại đối tác hay không
                 */
                var exitsPartner = objPartner.SingleOrDefault(x => x.Partner_id.Equals(partnerId) && x.Secret.Equals(secret, StringComparison.OrdinalIgnoreCase));
                if (exitsPartner == null)
                {
                    msg = $"Đối tác không tồn tại trên hệ thống Onfinance với PartnerID {partnerId}.";
                }
            }
            catch (Exception ex)
            {
                msg = $"Lỗi không kiểm tra được thông tin đối tác. methodName (CheckExitsPartner).";
            }
            return msg;
        }

        /// <summary>
        /// Lấy thông tin tiền tệ
        /// </summary>
        /// <returns></returns>
        private CurrencyBO CurrencyConversion()
        {
            CurrencyBO obj = new CurrencyBO();
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Encoding = System.Text.Encoding.UTF8;
                    client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.108 Safari/537.36");
                    client.Headers.Add("Access-Control-Allow-Origin", "*");
                    client.Headers.Add("Accept-Language", "vi-VN,vi;q=0.9,fr-FR;q=0.8,fr;q=0.7,en-US;q=0.6,en;q=0.5");
                    client.Headers.Add("Accept", " text/html, application/xhtml+xml, */*");
                    string URLTaxCode = ConfigurationManager.AppSettings["URLTaxCode"];
                    var url = ConfigurationManager.AppSettings["URLCurrency"];

                    string content = client.DownloadString(url);
                    content = content.Replace(")", "").Replace("(", "");
                    obj = JsonConvert.DeserializeObject<CurrencyBO>(content);
                }
            }
            catch (Exception ex)
            {
                obj = null;
            }
            return obj;
        }
        #endregion

        #region Save Customer
        private string BeforeSave(object objectType, ref CustomerBO customer)
        {
            string msg = string.Empty;
            List<string> lstColumnDic = new List<string>() { "cuspaymentmethod" };
            try
            {
                // Lấy thông tin mapping property
                string data = CommonFunction.ReadDataFile("DictionaryCustomer.json");
                if (!string.IsNullOrWhiteSpace(data))
                {
                    Dictionary<string, object> keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                    PropertyInfo[] propertyInfos = objectType.GetType().GetProperties();
                    foreach (var pi in propertyInfos)
                    {
                        string columnName = pi.Name;
                        if (keyValuePairs.ContainsKey(columnName))
                        {
                            var propertyType = pi.PropertyType;
                            TypeCode typeCode = Type.GetTypeCode(propertyType);
                            object value = pi.GetValue(objectType);

                            //Nếu là object
                            if (typeCode == TypeCode.Object && value != null)
                            {
                                List<InvoiceDetailBO> objProductDetails = new List<InvoiceDetailBO>();
                                msg = SetDataDetail(value, keyValuePairs, ref objProductDetails);
                                if (msg.Length > 0)
                                    return msg;
                            }
                            else
                            {
                                if (lstColumnDic.Contains(columnName.ToLower()))
                                {
                                    object vVal = GetDataCategories(value, columnName.ToLower());
                                    if (vVal == null || vVal.ToString() == "")
                                    {
                                        msg = $"Không tìm thấy thông tin danh mục trong hệ thống. Column {columnName}.";
                                        return msg;
                                    }

                                    CommonFunction.SetPropertyValue(customer, keyValuePairs[columnName].ToString(), vVal);
                                }
                                else
                                    CommonFunction.SetPropertyValue(customer, keyValuePairs[columnName].ToString(), value);
                            }
                        }
                    }
                    //Gán thông tin mặc định khi insert
                    customer.INITTIME = DateTime.Now;
                    customer.ISDELETED = false;
                }
            }
            catch (Exception ex)
            {
                msg = $"Lỗi tạo khách hàng. methodName (BeforeSave).";
            }
            return msg;
        }

        private string ValidateBeforSave(CustomerBO objCustomer, ref ServiceResult oResult)
        {
            string msg = string.Empty;
            try
            {
                // Kiểm tra xem khách có tồn tại không?
                CustomerBLL customerBLL = new CustomerBLL();
                msg = customerBLL.CheckCustomerDuplicateTaxCode(objCustomer);
                if (msg.Length > 0)
                {
                    SetMessageResult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                    return msg;
                }

                // Kiểm tra mã khách hàng (cusid) đã tồn tại chưa
                msg = customerBLL.CheckCustomerByCusId(objCustomer);
                if (msg.Length > 0)
                {
                    SetMessageResult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                    return msg;
                }

                // Kiểm tra Email đúng định dạng chưa
                msg = CommonFunction.ValidateEmail(objCustomer.CUSEMAIL);
                if (msg.Length > 0)
                {
                    SetMessageResult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                    return msg;
                }
            }
            catch (Exception ex)
            {
                msg = $"Lỗi kiểm tra dữ liệu trước khi cất. methodName (ValidateBeforSave).";
            }
            return msg;
        }
        #endregion

        #region Save Product
        private string BeforeSave(object objectType, ref ProductBO product)
        {
            string msg = string.Empty;
            List<string> lstColumnDic = new List<string>() { "quantityunit" };
            try
            {
                // Lấy thông tin mapping property
                string data = CommonFunction.ReadDataFile("DictionaryProduct.json");
                if (!string.IsNullOrWhiteSpace(data))
                {
                    Dictionary<string, object> keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                    PropertyInfo[] propertyInfos = objectType.GetType().GetProperties();
                    foreach (var pi in propertyInfos)
                    {
                        string columnName = pi.Name;
                        if (keyValuePairs.ContainsKey(columnName))
                        {
                            var propertyType = pi.PropertyType;
                            TypeCode typeCode = Type.GetTypeCode(propertyType);
                            object value = pi.GetValue(objectType);

                            //Nếu là object
                            if (typeCode == TypeCode.Object && value != null)
                            {
                                List<InvoiceDetailBO> objProductDetails = new List<InvoiceDetailBO>();
                                msg = SetDataDetail(value, keyValuePairs, ref objProductDetails);
                                if (msg.Length > 0)
                                    return msg;
                            }
                            else
                            {
                                if (lstColumnDic.Contains(columnName.ToLower()))
                                {
                                    object vVal = GetDataCategories(value, columnName.ToLower());
                                    if (vVal == null || vVal.ToString() == "")
                                    {
                                        msg = $"Không tìm thấy thông tin danh mục trong hệ thống. Column {columnName}.";
                                        return msg;
                                    }

                                    CommonFunction.SetPropertyValue(product, keyValuePairs[columnName].ToString(), vVal);
                                }
                                else
                                    CommonFunction.SetPropertyValue(product, keyValuePairs[columnName].ToString(), value);
                            }
                        }
                    }
                    //Gán thông tin mặc định khi insert
                    product.INITTIME = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                msg = $"Lỗi tạo khách hàng. methodName (BeforeSave).";
            }
            return msg;
        }

        private string ValidateBeforSave(ProductBO objProduct, ref ServiceResult oResult)
        {
            string msg = string.Empty;
            try
            {
                ProductBLL productBLL = new ProductBLL();
                // Kiểm tra tên hàng hóa đã tồn tại chưa
                msg = productBLL.CheckProductDuplicateByTaxCode(objProduct.COMTAXCODE, objProduct.PRODUCTNAME);
                if (msg.Length > 0)
                {
                    SetMessageResult(objectId, (int)ErrorCode.Fail, msg, ref oResult);
                    return msg;
                }

                //kiểm tra productType
                if (objProduct.PRODUCTTYPE != (int)ProductType.Product && objProduct.PRODUCTTYPE != (int)ProductType.Service)
                {
                    SetMessageResult(objectId, (int)ErrorCode.Fail, $"Loại sản phẩm đang không đúng PRODUCTTYPE:(1: Hàng hóa,2: Dịch vụ).", ref oResult);
                    return msg;
                }
            }
            catch (Exception ex)
            {
                msg = $"Lỗi kiểm tra dữ liệu trước khi cất. methodName (ValidateBeforSave).";
            }
            return msg;
        }
        #endregion

        #region E-Sign

        /// <summary>
        /// URL: https://api.onfinance.asia/invoice/v21/getsigninvoicelist
        /// jsonData = {"comtaxcode": 0336692460, "currentpage": 1, "itemperpage": 10}
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [System.Web.Http.Route("api/invoice/v21/getsigninvoicelist")]
        public JsonResult GetSignInvoiceList([FromBody]FormSearchInvoice form)
        {
            string token = string.Empty;
            var headers = Request.Headers;
            if (headers.Contains("Authorization"))
            {
                token = headers.GetValues("Authorization").First();
            }
            // Verify token

            // Do taking invoice list. INVOICESTATUS = 1
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                var timeSearch = DateTime.Now;
                form.FROMDATE = new DateTime(timeSearch.Year - 1, timeSearch.Month, timeSearch.Day);
                form.TODATE = timeSearch;
                form.INVOICESTATUS = (int)INVOICE_STATUS.WAITING;
                List<InvoiceBO> result = invoiceBLL.GetInvoice(form);
                long TotalPages = 0;
                var TotalRow = result.Count == 0 ? 0 : result[0].TOTALROW;
                TotalPages = TotalRow / form.ITEMPERPAGE.Value + 1;
                if (TotalRow % form.ITEMPERPAGE.Value == 0)
                {
                    TotalPages = TotalRow == 0 ? 1 : TotalRow / form.ITEMPERPAGE.Value;
                }
                var newResult = result.Select(item => new {ID = item.ID, FORMCODE = item.FORMCODE, SYMBOLCODE = item.SYMBOLCODE, CREATEDTIME = item.INITTIME, CUSNAME = item.CUSNAME, NUMBER = item.NUMBER, LINKVIEW = item.SIGNLINK }).ToList();
                ResultDataInvoice data = new ResultDataInvoice()
                {
                    rs = true,
                    result = newResult.Distinct(),
                    TotalPages = TotalPages,
                    TotalRow = TotalRow
                };

                return new JsonResult()
                {
                    Data = data,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = Int32.MaxValue
                };
            }
            catch (Exception ex)
            {
                //ConfigHelper.Instance.WriteLog("Lỗi lấy danh sách hóa", ex, MethodBase.GetCurrentMethod().Name, "GetInvoice");
                return new JsonResult(){ Data = new ResultDataInvoice(){ rs = false, result = null, TotalPages = 0, TotalRow = 0 } };
            }
        }

        /// <summary>
        /// URL: https://api.onfinance.asia/invoice/v21/postsigninvoice
        /// jsonData = {"id": 1}
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.Route("api/invoice/v21/postsigninvoice")]
        public JsonResult PostSignInvoice([FromBody]InvoiceBO inv)
        {
            UserModel userModel = new UserModel();
            userModel.APIID = "a8ff044d4cff44e5bf044d4cff34e526";
            userModel.SECRET = "NzRmMTg5NGEzNDQwYTMzYzBkNjY2ODc1N2YyN2FjNjM2NzhmMGYxYTI1MjBkMTQ2NmMyMzIzZjdkNjY0YjA3NQ==";
            userModel.APIURL = "http://demoapi.cyberhsm.vn/api/xml/sign/invoicedata";
            string token = string.Empty;
            var re = Request;
            var headers = re.Headers;
            if (headers.Contains("Authorization"))
            {
                token = headers.GetValues("Authorization").First();
            }
            // Verify token

            // Do signing with specificied invoice id
            long idInvoice = long.Parse(inv.ID.ToString());
            var signLinkXml = string.Empty;
            long nextNumber = 0;
            string msg = string.Empty;
            try
            {
                msg = CheckNumberWaiting(idInvoice, ref nextNumber);
                if (msg.Length > 0)
                {
                    return new JsonResult() { Data = new { rs = false, msg }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }

                // Lấy thông tin hóa đơn
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                InvoiceBO invoice = invoiceBLL.GetInvoiceById(idInvoice);
                if (invoice == null)
                {
                    msg = "Không lấy được thông tin hóa đơn.";
                    return new JsonResult() { Data = new { rs = false, msg }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                //Template Xml
                signLinkXml = invoiceBLL.CreateFileInvoiceXML(invoice, invoiceBLL.GetInvoiceDetail(invoice.ID), nextNumber);
                if (signLinkXml.Length == 0)
                {
                    msg = "Không tạo được file xml.";
                    return new JsonResult() { Data = new { rs = false, msg }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }

                //Đường dẫn thư mục chứa file
                var inputFolder = ConfigurationManager.AppSettings["InputInvoiceFolder"];
                var dstFilePathXml = HttpContext.Current.Server.MapPath("~/" + inputFolder + signLinkXml);

                //Thông tin xml cần ký
                string base64Xml = Convert.ToBase64String(System.IO.File.ReadAllBytes(dstFilePathXml));
                if (base64Xml == null)
                {
                    msg = "Lỗi chuyển đổi sang xml base64.";
                    return new JsonResult() { Data = new { rs = false, msg }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }

                //Tích hợp ký số bên Cyber HSM
                var task = Task.Run(() => CommonFunction.SignInvoice(base64Xml, userModel.APIURL, userModel.APIID, userModel.SECRET));
                task.Wait();
                if (task.Result.status == 1000) // Thành công
                {
                    //Gán thời gian ký hóa đơn
                    DateTime dtSigntime = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    msg = SaveInvoiceSigned(idInvoice, nextNumber, task.Result.base64xmlSigned, invoice, dtSigntime);
                    invoice.NUMBER = nextNumber;
                    if (msg.Length > 0)
                        return new JsonResult() { Data = new { rs = false, msg = "Lỗi cập nhật thông tin hóa đơn." }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    return new JsonResult() { Data = new { rs = true, msg = $"Phát hành hóa đơn thành công.", info = invoice }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                    return new JsonResult() { Data = new { rs = false, msg = "Phát hành hóa đơn không thành công." }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                //ConfigHelper.Instance.WriteLog("Lỗi đọc file hóa đơn", ex, MethodBase.GetCurrentMethod().Name, "GetFile");
                return new JsonResult() { Data = new { rs = false, msg = "Phát hành hóa đơn không thành công." }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        


        private string CheckNumberWaiting(long invoiceId, ref long nextNumber)
        {
            string msg = string.Empty;
            try
            {
                NumberBLL numberBLL = new NumberBLL();
                var tempNextNumber = numberBLL.GetNumberByInvoiceId2(invoiceId);
                if (tempNextNumber == 0)
                    msg = "Dải hóa đơn này đã hết số.";
                else
                    nextNumber = tempNextNumber;
            }
            catch (Exception ex)
            {
                msg = "Lỗi kiểm tra dải hóa đơn chờ.";
                throw ex;
            }
            return msg;
        }
        private string SaveInvoiceSigned(long invoiceId, long tempNextNumber, string base64xmlSigned, InvoiceBO invoice, DateTime dtSignTime, long numberWaitingId = 0)
        {
            string msg = string.Empty;
            string filePathXml = string.Empty;
            string fileName = invoice.COMTAXCODE + "_" + invoice.ID + "_" + invoice.INVOICETYPE + ".pdf";
            try
            {
                var outputFolder = ConfigurationManager.AppSettings["OutputSignedInvoiceFolder"];

                string signLinkRef = invoice.SIGNLINK;
                if (string.IsNullOrWhiteSpace(invoice.SIGNLINK))
                    signLinkRef = CreateFileInvoicePath(invoice, fileName);

                invoice.SIGNLINK = signLinkRef;

                string pathXml = outputFolder + invoice.SIGNLINK.Substring(0, invoice.SIGNLINK.Length - 4) + ".xml";

                //Lưu file XML đã ký thành công
                var outputFileNameXml = HttpContext.Current.Server.MapPath("~/" + pathXml).Replace("Invoice.API" , "SPA_Invoice\\NOVAON_FOLDER");
                msg = CommonFunction.Base64StringToFile(base64xmlSigned, outputFileNameXml);
                if (msg.Length > 0)
                {
                    msg = "Lưu file XML không thành công.";
                    return msg;
                }

                //Sum total size file
                var sizeXml = Checksum.CreateMD5(outputFileNameXml);

                //Cập nhật hóa đơn
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                long nextNumber = invoiceBLL.UpdateDataInvoice(invoiceId, sizeXml, invoice.SIGNLINK, tempNextNumber, dtSignTime);
                if (nextNumber <= 0)
                {
                    msg = "Số hóa đơn hiện tại đã hết số.";
                    return msg;
                }

                //Cập nhật số hóa đơn hiện tại
                bool isSuccess = false;
                NumberBLL numberBLL = new NumberBLL();
                if (numberWaitingId == 0)
                    isSuccess = numberBLL.UpdateCurrentNumber(invoiceId, tempNextNumber);
                else
                    isSuccess = numberBLL.UpdateCurrentWaittingNumber(numberWaitingId, tempNextNumber);

                if (isSuccess == false)
                {
                    msg = "Lỗi cập nhật giá trị số hóa đơn hiện tại.";
                    return msg;
                }
            }
            catch (Exception ex)
            {
                msg = "Lỗi khi cập nhật thông tin ký hóa đơn.";
                throw ex;
            }
            return msg;
        }
        private string CreateFileInvoicePath(InvoiceBO invoice, string fileName)
        {
            try
            {
                var root = ConfigurationManager.AppSettings["InputInvoiceFolder"];
                var branchComTaxCode = "/" + (string.IsNullOrEmpty(invoice.COMTAXCODE) ? "COMTAXCODE/" : invoice.COMTAXCODE + "/");
                var branchInvoiceID = string.IsNullOrEmpty(invoice.ID.ToString()) ? "INVOICEID/" : invoice.ID.ToString() + "/";
                var branchInvoiceType = string.IsNullOrEmpty(invoice.INVOICETYPENAME.ToString()) ? "INVOICETYPENAME/" : ElaHelper.FormatKeywordSearch(ElaHelper.FilterVietkey(invoice.INVOICETYPENAME.ToString())).Replace(" ", "-");
                var branchDate = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                var path = string.Format($"{root}{branchComTaxCode}{branchInvoiceID}{branchInvoiceType}");

                DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/" + path));
                if (!dir.Exists)
                {
                    dir.Create();
                }

                string filePath = dir + "\\" + fileName;

                fileName = filePath.Replace(HttpContext.Current.Server.MapPath("~/" + root), "").Replace('\\', '/');

                return fileName;
            }
            catch (Exception objEx)
            {
                //ConfigHelper.Instance.WriteLog("Không tạo được đường dẫn file.", objEx, MethodBase.GetCurrentMethod().Name, "CreateFileInvoicePath");
                fileName = string.Empty;
                return fileName;
            }
        }
        #endregion
    }
}

class EmptyController : System.Web.Mvc.Controller { }
