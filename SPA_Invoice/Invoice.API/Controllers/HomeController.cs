using DS.BusinessLogic.Account;
using DS.BusinessLogic.Company;
using DS.BusinessLogic.Invoice;
using DS.BusinessLogic.Number;
using DS.BusinessObject.Invoice;
using DS.BusinessObject.Output;
using DS.Common.Helpers;
using Invoice.API.DTO.Invoice;
using Invoice.API.Filter;
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
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using static DS.Common.Enums.EnumHelper;

namespace Invoice.API.Controllers
{
    [RoutePrefix("api/onfinance")]
    //[JwtAuthentication(Realm = "Onfinance")]
    public class HomeController : BaseApiController
    {
        #region Biến dùng chung
        SPA_Invoice.Controllers.HomeController homeController = new SPA_Invoice.Controllers.HomeController();// Biến homeController bên Invoice => hỗ trợ dùng hàm chung bên Invoice
        string outputInputInvoiceFolder = ConfigurationManager.AppSettings["OutputInvoice"]; // Thư mục lưu hóa đơn bên Invoice
        string uriFolder = ConfigurationManager.AppSettings["UriFolder"].Replace("\\", "/");
        //string appIds = ConfigurationManager.AppSettings["appIds"];
        string partnerFilePath = ConfigurationManager.AppSettings["PartnerFilePath"]; // Thư mục lưu danh sách đối tác dùng api
        #endregion

        #region Các phương thức APIs
        /// <summary>
        /// URL: https://api.onfinance.asia/home/v21/gettoken
        /// jsonData = {"comtaxcode": 0336692460, "appId": "fafkssdfksdfioasdfoiwroiqw3riowereu"}
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("home/v21/gettoken")]
        public HttpResponseMessage GetToken([FromBody] TokenModel tokenModel)
        {
            var partner = GetListPartner().Where(x => x.appId == tokenModel.appId).FirstOrDefault();
            if (partner == null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { code = 100, msg = "App ID không hợp lệ!" });

            var token = JwtManager.GenerateToken(tokenModel.appId);

            var tk = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(token);


            return Request.CreateResponse(HttpStatusCode.OK, new { token = tk });
        }

        [HttpPost]
        [JwtAuthentication]
        [Route("home/v21/getdetailinvoice")]
        public HttpResponseMessage GetDetailInvoice([FromBody] InvoiceBO inv)
        {
            InvoiceBLL objInvoice = new InvoiceBLL();
            //Lấy master
            var obj = objInvoice.GetInvoiceById(inv.ID, inv.USINGINVOICETYPE);
            if (obj == null || obj.ID == 0)
                return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 405, msg = "Không tìm thấy hóa đơn." });

            //Lấy detail
            obj.LISTPRODUCT = objInvoice.GetInvoiceDetail(obj.ID);
            if (obj.LISTPRODUCT == null)
                return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 406, msg = "Không tìm thấy chi tiết hóa đơn." });

            //Kiểm tra hóa đơn của doanh nghiệp đang yêu cầu
            if (obj.COMTAXCODE != inv.COMTAXCODE)
                return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 407, msg = "Không tìm thấy hóa đơn của doanh nghiệp." });

            string UriFolder = ConfigurationManager.AppSettings["UriFolder"];
            obj.SIGNEDXML = string.IsNullOrEmpty(obj.SIGNLINK) ? null : (UriFolder + obj.SIGNLINK.Replace(".pdf", ".xml"));
            obj.SIGNEDLINK = string.Format("https://api.onfinance.asia/api/onfinance/home/v21/viewpdf?comtaxcode={0}&id={1}", obj.COMTAXCODE, obj.ID); // string.IsNullOrEmpty(obj.SIGNLINK) ? null : (UriFolder + obj.SIGNLINK.Replace(".xml",".pdf"));
            return Request.CreateResponse(HttpStatusCode.OK, new { rs = true, code = 0, obj });

        }


        /// <summary>
        /// URL: https://api.onfinance.asia/invoice/v21/postsignmultipleinvoice
        /// jsonData = {"ids": "1;2;3;4;5;6;7"}
        /// </summary>
        /// <returns></returns>
        [JwtAuthentication]
        [Route("home/v21/postsignmultipleinvoice")]
        [HttpPost]
        public HttpResponseMessage PostSignMultipleInvoice([FromBody] ListInvoiceIdRequest objInvoice)
        {
            var signLinkXml = string.Empty;
            long nextNumber;
            string msgSignSuccess = string.Empty;
            int totalRowSign = 0;
            string msg = string.Empty;

            var dicImportInvoiceError = new Dictionary<long, long>();
            //var dicResultInvoices = new Dictionary<InvoiceBO, string>();
            List<ResponseInvoiceInfo> signInvoiceOutputs = new List<ResponseInvoiceInfo>();
            List<SignInvoiceData> signInvoiceDatas = new List<SignInvoiceData>();
            InvoiceBLL invoiceBLL = new InvoiceBLL();
            try
            {
                AccountBLL accountBLL = new AccountBLL();
                UserModel userModel = accountBLL.CheckUserNameUsedKysoHSM(objInvoice.COMUSERNAME, objInvoice.COMTAXCODE);
                if (userModel == null || string.IsNullOrEmpty(userModel.APIID) || string.IsNullOrEmpty(userModel.SECRET) || string.IsNullOrEmpty(userModel.APIURL))
                {
                    msg = "Tài khoản không được đăng ký HSM.";
                    return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, msg = msg });
                }

                string[] lstInvoiceid = objInvoice.IDS.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                totalRowSign = lstInvoiceid.Length;

                // Lấy ra chi tiết hóa đơn
                List<InvoiceBO> lstInvoice = invoiceBLL.GetMultiInvoice(objInvoice.IDS, objInvoice.COMTAXCODE);
                var lstProducts = invoiceBLL.GetInvoiceDetailByIds(objInvoice.IDS);

                foreach(var inv in lstInvoice)
                {
                    var lstdetail = lstProducts.Where(x => x.INVOICEID == inv.ID).ToList();
                    if (lstdetail != null && lstdetail.Count > 0) {
                        SignInvoiceData obj = new SignInvoiceData
                        {
                            CusCode = inv.CUSTOMERCODE,
                            InvoiceData = inv,
                            lstDetails = lstdetail
                        };
                        signInvoiceDatas.Add(obj);
                    }
                }


                if (signInvoiceDatas.Count > 0)
                {
                    //Sắp xếp lại dữ liệu
                    var oDatas = signInvoiceDatas.OrderBy(x => x.CusCode).ToList();

                    //Thực hiện ký
                    foreach (var item in oDatas)
                    {
                        InvoiceBO invoice = item.InvoiceData;
                        msg = homeController.CheckNumberWaiting(invoice.COMTAXCODE, invoice.FORMCODE, invoice.SYMBOLCODE, out nextNumber);
                        if (msg.Length > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, msg = msg });
                        }

                        //Template Xml
                        signLinkXml = invoiceBLL.CreateFileInvoiceXMLAPI(invoice, item.lstDetails, nextNumber, outputInputInvoiceFolder, GetInvoiceNameXmlFile(invoice.USINGINVOICETYPE));
                        if (signLinkXml.Length == 0)
                        {
                            msg = "Không tạo được file xml.";
                            return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, msg = msg });
                        }
                        invoice.SIGNLINK = signLinkXml;
                        string dstFilePathXml = outputInputInvoiceFolder + signLinkXml;


                        //Thông tin xml cần ký
                        string base64Xml = Convert.ToBase64String(System.IO.File.ReadAllBytes(dstFilePathXml));
                        if (base64Xml == null)
                        {
                            msg = "Lỗi chuyển đổi sang xml base64.";
                            return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, msg = msg });
                        }

                        //Tích hợp ký số bên Cyber HSM
                        var task = Task.Run(() => CommonFunction.SignInvoice(base64Xml, userModel.APIURL, userModel.APIID, userModel.SECRET));
                        if (task.Result.status == 1000) // Thành công
                        {
                            string source = "OnSign CyberLotusHSM";
                            //Gán thời gian ký hóa đơn
                            DateTime dtSigntime = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            msg = SaveSignedInvoice(invoice.ID, nextNumber, task.Result.base64xmlSigned, invoice, dtSigntime, source);

                            //Ghi nhận ID ký thành công
                            signInvoiceOutputs.Add(new ResponseInvoiceInfo() { ID = invoice.ID, INVOICECODE = invoice.INVOICECODE, REFERENCECODE = invoice.REFERENCECODE, NUMBER = nextNumber, SIGNEDTIME = dtSigntime, SIGNLINK = uriFolder + invoice.SIGNLINK.Replace(".xml", ".pdf")});
                        }
                        else
                        {
                            dicImportInvoiceError.Add(invoice.ID, invoice.ID);
                            msgSignSuccess = "không tạo được chữ ký HSM.";
                            ConfigHelper.Instance.WriteLog($"Phát hành hóa đơn không thành công. Kết quả trả về từ Cyber: {task.Result.status}. Des: {task.Result.description}", string.Empty, MethodBase.GetCurrentMethod().Name, "signXmlHSMApiMultiple");
                        }
                    }

                    if (signInvoiceOutputs.Count > 0)
                        return Request.CreateResponse(HttpStatusCode.OK, new { rs = true, msg = $"Phát hành hóa đơn thành công {signInvoiceOutputs.Count}/{totalRowSign} hóa đơn.", rowData = JsonConvert.SerializeObject(signInvoiceOutputs) });
                    else
                        return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, msg = $"Phát hành hóa đơn không thành công. Chi tiết {msgSignSuccess}", rowData = JsonConvert.SerializeObject(dicImportInvoiceError) });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 109, msg = "Không tìm thấy hóa đơn để ký." });
                }
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Phát hành hóa đơn không thành công.", ex, MethodBase.GetCurrentMethod().Name, "signXmlHSMApiMultiple");
                msg = "Phát hành hóa đơn không thành công.";
                return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, msg = msg });
            }
        }

        /// <summary>
        /// URL: https://api.onfinance.asia/home/v21/getsigninvoicelist
        /// jsonData = {"comtaxcode": 0336692460, "currentpage": 1, "itemperpage": 10}
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        //[HttpPost]
        //[JwtAuthentication]
        //[Route("home/v21/getsigninvoicelist")]
        //public HttpResponseMessage GetSignInvoiceList([FromBody] FormSearchInvoice form)
        //{
        //    // Taking invoice list. INVOICESTATUS = 1
        //    try
        //    {
        //        InvoiceBLL invoiceBLL = new InvoiceBLL();
        //        var timeSearch = DateTime.Now;
        //        form.FROMDATE = new DateTime(timeSearch.Year - 1, timeSearch.Month, timeSearch.Day);
        //        form.TODATE = timeSearch;
        //        form.INVOICESTATUS = (int)INVOICE_STATUS.WAITING;
        //        form.OFFSET = (form.CURRENTPAGE - 1) * form.ITEMPERPAGE;
        //        List<InvoiceBO> result = invoiceBLL.GetInvoiceAPI(form);
        //        long TotalPages = 0;
        //        var TotalRow = result.Count == 0 ? 0 : result[0].TOTALROW;
        //        TotalPages = TotalRow / form.ITEMPERPAGE.Value + 1;
        //        if (TotalRow % form.ITEMPERPAGE.Value == 0)
        //        {
        //            TotalPages = TotalRow == 0 ? 1 : TotalRow / form.ITEMPERPAGE.Value;
        //        }
        //        var newResult = result.Select(item => new
        //        {
        //            item.ID,
        //            item.FORMCODE,
        //            item.SYMBOLCODE,
        //            item.CREATEDTIME,
        //            item.CUSNAME,
        //            item.NUMBER,
        //            item.LINKVIEW,
        //            item.CUSTAXCODE,
        //            item.TOTALPAYMENT
        //        }).ToList();

        //        ResultDataInvoice data = new ResultDataInvoice()
        //        {
        //            rs = true,
        //            result = newResult.Distinct(),
        //            TotalPages = TotalPages,
        //            TotalRow = TotalRow,
        //            CurrentPage = form.CURRENTPAGE.Value
        //        };
        //        return Request.CreateResponse(HttpStatusCode.OK, data);
        //    }
        //    catch (Exception ex)
        //    {
        //        //ConfigHelper.Instance.WriteLog("Lỗi lấy danh sách hóa", ex, MethodBase.GetCurrentMethod().Name, "GetInvoice");
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, new ResultDataInvoice() { });
        //    }
        //}



        /// <summary>
        /// URL: https://api.onfinance.asia/invoice/v21/postsigninvoice
        /// jsonData = {"id": 1}
        /// </summary>
        /// <returns></returns>
        //[JwtAuthentication(Realm = "Onfinance")]
        //[Route("home/v21/postsigninvoice")]
        //[HttpPost]
        //public HttpResponseMessage PostSignInvoice([FromBody] InvoiceBO inv)
        //{
        //    //UserModel userModel = new UserModel
        //    //{
        //    //    APIID = "a8ff044d4cff44e5bf044d4cff34e526",
        //    //    SECRET = "NzRmMTg5NGEzNDQwYTMzYzBkNjY2ODc1N2YyN2FjNjM2NzhmMGYxYTI1MjBkMTQ2NmMyMzIzZjdkNjY0YjA3NQ==",
        //    //    APIURL = "http://demoapi.cyberhsm.vn/api/xml/sign/invoicedata"
        //    //};

        //    // Do signing with specificied invoice id
        //    string msg = string.Empty;
        //    try
        //    {
        //        //Kiểm tra xem khách hàng có sử dụng chữ ký số hsm không
        //        AccountBLL accountBLL = new AccountBLL();
        //        UserModel userModel = accountBLL.CheckUserNameUsedKysoHSM(inv.COMUSERNAME, inv.COMTAXCODE);
        //        if (userModel == null || string.IsNullOrEmpty(userModel.APIID) || string.IsNullOrEmpty(userModel.SECRET) || string.IsNullOrEmpty(userModel.APIURL))
        //        {
        //            msg = "Tài khoản không được đăng ký HSM.";
        //            return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, msg = msg });
        //        }

        //        long idInvoice = long.Parse(inv.ID.ToString());
        //        var signLinkXml = string.Empty;
        //        long nextNumber = 0;

        //        // Lấy thông tin hóa đơn
        //        InvoiceBLL invoiceBLL = new InvoiceBLL();
        //        InvoiceBO invoice = invoiceBLL.GetInvoiceByIdAPI(idInvoice);
        //        if (invoice == null)
        //        {
        //            msg = "Không lấy được thông tin hóa đơn.";
        //            return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, msg = msg });
        //        }
        //        if (invoice.NUMBER > 0)
        //        {
        //            msg = "Hóa đơn đã được phát hành bởi 1 người khác.";
        //            return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, msg = msg });

        //        }
        //        msg = homeController.CheckNumberWaiting(idInvoice, ref nextNumber);
        //        if (msg.Length > 0)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, msg = msg });
        //        }

        //        //Template XmlH
        //        //signLinkXml = invoiceBLL.GenerateInvoiceXMLAPI(invoice, nextNumber, outputInputInvoiceFolder);
        //        signLinkXml = invoiceBLL.CreateFileInvoiceXMLAPI(invoice, invoiceBLL.GetInvoiceDetail(invoice.ID), nextNumber, outputInputInvoiceFolder, GetInvoiceNameXmlFile(invoice.USINGINVOICETYPE));
        //        if (signLinkXml.Length == 0)
        //        {
        //            msg = "Không tạo được file xml.";
        //            return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, msg = msg });
        //        }

        //        //Đường dẫn thư mục chứa file
        //        var dstFilePathXml = outputInputInvoiceFolder + signLinkXml;

        //        //Thông tin xml cần ký
        //        string base64Xml = Convert.ToBase64String(File.ReadAllBytes(dstFilePathXml));
        //        if (base64Xml == null)
        //        {
        //            msg = "Lỗi chuyển đổi sang xml base64.";
        //            return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, msg = msg });
        //        }

        //        //Tích hợp ký số bên Cyber HSM
        //        var task = Task.Run(() => CommonFunction.SignInvoice(base64Xml, userModel.APIURL, userModel.APIID, userModel.SECRET));
        //        task.Wait();
        //        if (task.Result.status == 1000) // Thành công
        //        {
        //            string source = "OnSign CyberLotusHSM";
        //            //Gán thời gian ký hóa đơn
        //            DateTime dtSigntime = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        //            msg = SaveSignedInvoice(idInvoice, nextNumber, task.Result.base64xmlSigned, invoice, dtSigntime, source);
        //            invoice.NUMBER = nextNumber;
        //            if (msg.Length > 0)
        //                return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, msg = "Lỗi cập nhật thông tin hóa đơn." });
        //            return Request.CreateResponse(HttpStatusCode.OK, new { rs = true, msg = $"Phát hành hóa đơn thành công."/*, info = invoice*/ });
        //        }
        //        else
        //            return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, msg = "Phát hành hóa đơn không thành công." });
        //    }
        //    catch (Exception ex)
        //    {
        //        //ConfigHelper.Instance.WriteLog("Lỗi đọc file hóa đơn", ex, MethodBase.GetCurrentMethod().Name, "GetFile");
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, new { rs = false, msg = "Không thể phát hành hóa đơn." });
        //    }
        //}



        /// <summary>
        /// URL: https://api.onfinance.asia/invoice/v21/postcreatelistinvoice
        /// jsonData = {"ids": "1;2;3;4;5;6;7"}
        /// </summary>
        /// <returns></returns>
        [JwtAuthentication(Realm = "Onfinance")]
        [Route("home/v21/postcreatelistinvoice")]
        [HttpPost]
        public HttpResponseMessage PostCreateListInvoice(HttpRequestMessage request)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                var response = new InvoiceResponse();

                // Không dùng được [FromBody] nên phải dùng cách này để lấy json string.
                var requestData = request.Content.ReadAsStringAsync().Result;
                var listInvoiceData = JsonConvert.DeserializeObject<List<InvoiceBO>>(requestData);

                if (listInvoiceData == null)
                    return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 101, msg = "Không có dữ liệu hóa đơn cần tạo" });

                //Tạo hóa đơn từ list hóa đơn ở trên listInvoiceData
                List<int> lstCreated = new List<int>();
                List<long> invoiceIds = new List<long>();
                string msg = "";
                for (int i = 0; i< listInvoiceData.Count;i++)
                {
                    InvoiceBO invoice = listInvoiceData[i];

                    // Kiểm tra nếu là hóa đơn thay thế
                    if (invoice.INVOICETYPE == (int)INVOICE_TYPE.ALTERNATIVE)
                    {
                        var refInvoice = invoiceBLL.GetInvoiceByIdAPI(invoice.REFERENCE); // Hóa đơn tham chiếu khi thay thế hoặc điều chỉnh.
                        if (refInvoice == null)
                            return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 101, msg = $"Không tồn tại hóa đơn tham chiếu {invoice.REFERENCE}." });

                        if (refInvoice.INVOICESTATUS != (int)INVOICE_STATUS.CONFIRMED)
                            return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 101, msg = $"Hóa đơn tham chiếu {invoice.REFERENCE} chưa phát hành." });

                        if (refInvoice.INVOICETYPE != (int)INVOICE_TYPE.CANCEL)
                            return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 101, msg = $"Hóa đơn tham chiếu {invoice.REFERENCE} chưa được hủy bỏ." });
                    }

                    if (invoice.LISTPRODUCT == null || invoice.LISTPRODUCT.Count == 0)
                    {
                        msg += string.Format("Hóa đơn thứ {0} không có sản phẩm; ", i);
                        continue;
                    }
                    invoice.REFERENCECODE = ReferenceCode.GenerateReferenceCode();
                    invoice.INVOICETYPE = invoice.INVOICETYPE == 0 ? 1 : invoice.INVOICETYPE;
                    invoice.INVOICESTATUS = 1;
                    invoice.PAYMENTSTATUS = invoice.PAYMENTSTATUS == 0 ? 1 : invoice.PAYMENTSTATUS;
                    invoice.CURRENCY = string.IsNullOrWhiteSpace(invoice.CURRENCY) ? "VND" : invoice.CURRENCY;
                    invoice.EXCHANGERATE = invoice.EXCHANGERATE == 0 ? 1 : invoice.EXCHANGERATE;
                    long result = invoiceBLL.CreateInvoiceAPI(invoice);

                    if (result != -1)
                    {
                        lstCreated.Add(i);
                        invoiceIds.Add(result);
                    }
                }

                if (lstCreated.Count == 0)
                {
                    response = new InvoiceResponse()
                    {
                        Code = "102",
                        Message = msg + invoiceBLL.ResultMessageBO.Message,
                        Success = false,
                        Time = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")
                    };
                }
                else
                {
                    response = new InvoiceResponse()
                    {
                        Code = lstCreated.Count == listInvoiceData.Count ? "0" : "01",
                        Message = msg + (lstCreated.Count == listInvoiceData.Count ? "Tất cả hóa đơn được tạo thành công." : "Một (vài) hóa đơn đã được tạo thành công"),
                        Success = true,
                        IndexCreated = lstCreated,
                        InvoiceCreated = invoiceIds,
                        Time = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"),
                        data = new List<InvoiceBO>() { new InvoiceBO() { COMTAXCODE = listInvoiceData[0].COMTAXCODE } }
                    };
                }

                
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 104, msg = ex.ToString() });
            }
        }

        /// <summary>
        /// URL: https://api.onfinance.asia/invoice/v21/postcreateinvoiceame
        /// jsonData = {"ids": "1;2;3;4;5;6;7"}
        /// </summary>
        /// <returns></returns>
        [JwtAuthentication(Realm = "Onfinance")]
        [Route("home/v21/postcreatelistinvoiceame")]
        [HttpPost]
        public HttpResponseMessage PostCreateListInvoiceAME(HttpRequestMessage request)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                var response = new InvoiceResponse();

                // Không dùng được [FromBody] nên phải dùng cách này để lấy json string.
                var requestData = request.Content.ReadAsStringAsync().Result;
                var listInvoiceData = JsonConvert.DeserializeObject<List<InvoiceBO>>(requestData);

                if (listInvoiceData == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 101, msg = "Không có dữ liệu hóa đơn cần tạo" });
                }

                //Tạo hóa đơn từ list hóa đơn ở trên listInvoiceData
                List<int> lstCreated = new List<int>();
                List<long> invoiceIds = new List<long>();
                List<string> listInvoiceId = new List<string>();
                string msg = "";
                for (int i = 0; i < listInvoiceData.Count; i++)
                {
                    InvoiceBO invoice = listInvoiceData[i];

                    var lstInvoice = invoiceBLL.SearchByInvoiceCode(invoice.INVOICECODE, invoice.COMTAXCODE);
                    if (lstInvoice.Count > 0)
                    {
                        foreach (var item in lstInvoice)
                        {
                            if (invoice.AME_DATETIME > invoice.AME_DATETIME0 && item.INVOICESTATUS == (int)INVOICE_STATUS.WAITING)
                            {
                                invoice.ID = item.ID;
                                invoice.INVOICETYPE = 1;
                                invoice.INVOICESTATUS = 1;
                                invoice.PAYMENTSTATUS = 1;
                                invoice.CURRENCY = string.IsNullOrWhiteSpace(invoice.CURRENCY) ? "VND" : invoice.CURRENCY;
                                invoice.EXCHANGERATE = invoice.EXCHANGERATE == 0 ? 1 : invoice.EXCHANGERATE;
                                invoice.INITTIME = invoice.INITTIME.AddHours(7); // Lệch múi giờ nên phải cộng thêm 7 tiếng với những hóa đơn cập nhật

                                long result = invoiceBLL.UpdateInvoiceAPI(invoice);
                                if (result != -1)
                                {
                                    lstCreated.Add(i);
                                    invoiceIds.Add(result);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (invoice.LISTPRODUCT == null || invoice.LISTPRODUCT.Count == 0)
                        {
                            msg += string.Format("Hóa đơn thứ {0} không có sản phẩm; ", i);
                            continue;
                        }
                        invoice.REFERENCECODE = ReferenceCode.GenerateReferenceCode();
                        invoice.INVOICETYPE = 1;
                        invoice.INVOICESTATUS = 1;
                        invoice.PAYMENTSTATUS = 1;
                        invoice.CURRENCY = string.IsNullOrWhiteSpace(invoice.CURRENCY) ? "VND" : invoice.CURRENCY;
                        invoice.EXCHANGERATE = invoice.EXCHANGERATE == 0 ? 1 : invoice.EXCHANGERATE;

                        long result = invoiceBLL.CreateInvoiceAPI(invoice);
                        if (result != -1)
                        {
                            lstCreated.Add(i);
                            invoiceIds.Add(result);
                        }
                    }
                }

                if (lstCreated.Count == 0)
                {
                    response = new InvoiceResponse()
                    {
                        Code = "102",
                        Message = msg + invoiceBLL.ResultMessageBO.Message,
                        Success = false,
                        Time = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")
                    };
                }
                else
                {
                    response = new InvoiceResponse()
                    {
                        Code = lstCreated.Count == listInvoiceData.Count ? "0" : "01",
                        Message = msg + (lstCreated.Count == listInvoiceData.Count ? "Tất cả hóa đơn được tạo thành công." : "Một (vài) hóa đơn đã được tạo thành công"),
                        Success = true,
                        IndexCreated = lstCreated,
                        InvoiceCreated = invoiceIds,
                        Time = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")
                    };
                }


                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 104, msg = ex.ToString() });
            }
        }

        /// <summary>
        /// URL: https://api.onfinance.asia/invoice/v21/createandsigninvoices
        /// jsonData like create method
        /// </summary>
        /// <returns></returns>
        [JwtAuthentication]
        [Route("home/v21/invoices/createandsign")]
        [HttpPost]
        public HttpResponseMessage CreateAndSignInvoices(HttpRequestMessage request)
        {
            try
            {
                // Tạo hóa đơn. Trả về hóa đơn thông tin hóa đơn tạo thành công.
                var objInvoice = PostCreateListInvoice(request);

                // Danh sách id hóa đơn cần ký
                objInvoice.TryGetContentValue(out InvoiceResponse listInvoiceIds);
                if (listInvoiceIds.InvoiceCreated.Count <= 0)
                    return objInvoice;
                else
                {
                    var comTaxCode = listInvoiceIds.data[0].COMTAXCODE;
                    var result = PostSignMultipleInvoice(new ListInvoiceIdRequest() { COMUSERNAME = comTaxCode, COMTAXCODE = comTaxCode, IDS = string.Join(";", listInvoiceIds.InvoiceCreated) });

                    return result;
                }
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Phát hành hóa đơn không thành công.", ex, MethodBase.GetCurrentMethod().Name, "CreateAndSignInvoices");
                return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, msg = "Internal Error." });
            }
        }

        /// <summary>
        /// Lấy ra danh sách tình hình sử dụng hóa đơn
        /// { "comtaxCode": "0336692463"}
        /// </summary> 
        /// <param name="comtaxCode"></param>
        /// <returns></returns>
        [JwtAuthentication(Realm = "Onfinance")]
        [Route("home/v21/invoices/statistic")]
        [HttpPost]
        public HttpResponseMessage GetStatusInvoices(HttpRequestMessage request)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                var requestData = request.Content.ReadAsStringAsync().Result;
                var objData = JsonConvert.DeserializeObject<InvoiceSearchFormBO>(requestData);

                InvoiceSearchFormBO obj = new InvoiceSearchFormBO();
                obj.KEYWORD = objData.COMTAXCODE;

                var result = invoiceBLL.Getforstatistics(obj);//còn số
                var lst = result.GroupBy(x => x.ID).Select(x => x.ToList()).ToList();
                var lstInvSumDetail = new List<InvoiceSummaryDetail>();
                foreach (var l in lst)
                {
                    var item = l.Select(x => new InvoiceSummaryDetail()
                    {
                        FORMCODE = x.FORMCODE,
                        SYMBOLCODE = x.SYMBOLCODE,
                        FROMNUMBER = x.FROMNUMBER,
                        TONUMBER = x.TONUMBER,
                        TOTALNUMBER = x.TOTALNUMBER,
                        USEDNUMBER = x.USEDNUMBER,
                        NUMBERSTATUSNAME = x.NUMBERSTATUSNAME,
                        NUMBERSTATUSID = x.NUMBERSTATUSID
                    }).ToList();

                    lstInvSumDetail.AddRange(item);
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { rs = true, msg = "Thành công.", data = lstInvSumDetail });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 104, msg = ex.ToString() });
            }
        }

        /// <summary>
        /// Chuyển hóa đơn thành hóa đơn hủy
        /// { "id": 1, "cancelReason": "cancel", "cancelTime": "01/01/2020" }
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancelReason"></param>
        /// <param name="cancelTime"></param>
        /// <returns></returns>
        [JwtAuthentication(Realm = "Onfinance")]
        [Route("home/v21/invoices/cancel")]
        [HttpPost]
        public HttpResponseMessage CancelInvoice(HttpRequestMessage request)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                var requestData = request.Content.ReadAsStringAsync().Result;
                var objData = JsonConvert.DeserializeObject<InvoiceBO>(requestData);
                DateTime calcelTime = DateTime.ParseExact(objData.CANCELTIME.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                var invoice = invoiceBLL.GetInvoiceByIdAPI(objData.ID);
                if (invoice == null)
                    return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, msg = $"Không tìm thấy hóa đơn {objData.ID}."});

                var result = new ResponseInvoiceInfo() { ID = invoice.ID, SIGNEDTIME = invoice.SIGNEDTIME, NUMBER = invoice.NUMBER, INVOICECODE = objData.INVOICECODE, SIGNLINK = uriFolder + invoice.SIGNLINK, REFERENCECODE = invoice.REFERENCECODE };
                if (invoice.INVOICESTATUS != (int)DS.Common.Enums.EnumHelper.INVOICE_STATUS.CONFIRMED)
                    return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 200, msg = $"Hóa đơn chưa phát hành {objData.ID}.", data = result });
                if (invoice.INVOICETYPE == (int)DS.Common.Enums.EnumHelper.INVOICE_TYPE.CANCEL)
                    return Request.CreateResponse(HttpStatusCode.OK, new { rs = true, code = 200, msg = $"Hóa đơn đã được hủy {objData.INVOICECODE}.", data = result });
                if (calcelTime < invoice.SIGNEDTIME)
                    return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 200, msg = $"Thời gian hủy {objData.CANCELTIME} không nhỏ hơn thời gian phát hành {invoice.SIGNEDTIME.ToString("dd/MM/yyyy")} hóa đơn {objData.ID}.", data = result });

                invoice.CANCELREASON = objData.CANCELREASON;
                invoice.CANCELTIME = calcelTime;
                invoice.INVOICETYPE = (int)INVOICE_TYPE.CANCEL;

                bool isSuccess = invoiceBLL.UpdateCancelInvoice(invoice);
                string message = isSuccess == false ? $"Hệ thống không hủy được hóa đơn {objData.ID}. Xin vui lòng liên hệ với admin" : $"Hủy thành công hóa đơn {objData.ID}";
                return Request.CreateResponse(HttpStatusCode.OK, new { rs = true, code = 200, msg = message, data = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 104, msg = ex.ToString() });
            }
        }

        /// <summary>
        /// Chuyển hóa đơn thành hóa đơn hủy
        /// { "invoicecode": "MLK001", "comtaxcode": "0336692463", "cancelReason": "cancel", "cancelTime": "01/01/2020"}
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancelReason"></param>
        /// <param name="cancelTime"></param>
        /// <returns></returns>
        [JwtAuthentication(Realm = "Onfinance")]
        [Route("home/v21/invoices/cancelbyinvoicecode")]
        [HttpPost]
        public HttpResponseMessage CancelInvoiceByInvoiceCode(HttpRequestMessage request)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                var requestData = request.Content.ReadAsStringAsync().Result;
                var objData = JsonConvert.DeserializeObject<InvoiceBO>(requestData);

                DateTime calcelTime = DateTime.ParseExact(objData.CANCELTIME.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                var invoice = invoiceBLL.SearchByInvoiceCode(objData.INVOICECODE, objData.COMTAXCODE).FirstOrDefault();
                if (invoice == null)
                    return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 404, msg = $"Không tìm thấy hóa đơn có mã liên kết {objData.INVOICECODE}." });
                var result = new ResponseInvoiceInfo() { ID = invoice.ID, SIGNEDTIME = invoice.SIGNEDTIME, NUMBER = invoice.NUMBER, INVOICECODE = objData.INVOICECODE, SIGNLINK = uriFolder + invoice.SIGNLINK, REFERENCECODE = invoice.REFERENCECODE };
                if (invoice.INVOICESTATUS != (int)DS.Common.Enums.EnumHelper.INVOICE_STATUS.CONFIRMED)
                    return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 200, msg = $"Hóa đơn chưa phát hành {objData.INVOICECODE}.", data = result });
                if (invoice.INVOICETYPE == (int)DS.Common.Enums.EnumHelper.INVOICE_TYPE.CANCEL)
                    return Request.CreateResponse(HttpStatusCode.OK, new { rs = true, code = 200, msg = $"Hóa đơn đã được hủy {objData.INVOICECODE}.", data = result });
                if (calcelTime < invoice.SIGNEDTIME)
                    return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 200, msg = $"Thời gian hủy {objData.CANCELTIME} không nhỏ hơn thời gian phát hành {invoice.SIGNEDTIME.ToString("dd/MM/yyyy")} hóa đơn {objData.INVOICECODE}.", data = result });

                invoice.CANCELREASON = objData.CANCELREASON;
                invoice.CANCELTIME = calcelTime;
                invoice.INVOICETYPE = (int)INVOICE_TYPE.CANCEL;

                bool isSuccess = invoiceBLL.UpdateCancelInvoice(invoice);
                string message = isSuccess == false ? $"Hệ thống không hủy được hóa đơn {objData.INVOICECODE}. Xin vui lòng liên hệ với admin" : $"Hủy thành công hóa đơn {objData.INVOICECODE}";
                return Request.CreateResponse(HttpStatusCode.OK, new { rs = true, code = 200, msg = message, data = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 500, msg = "Internal Error \n" + ex.ToString() });
            }
        }


        /// <summary>
        /// Xem hóa đơn
        /// { "comTaxCode": "0336692463", "id": 1}
        /// </summary>
        /// <param name="comTaxCode"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [JwtAuthentication(Realm = "Onfinance")]
        [HttpPost]
        [Route("home/v21/invoices/view")]
        public HttpResponseMessage ViewPDF(HttpRequestMessage request)
        {
            InvoiceBLL invoiceBLL = new InvoiceBLL();
            var requestData = request.Content.ReadAsStringAsync().Result;
            var objData = JsonConvert.DeserializeObject<InvoiceBO>(requestData);
            try
            {
                if (objData.COMTAXCODE.Contains("-"))
                {
                    var tempObj = objData.COMTAXCODE.Split('-');
                    objData.COMTAXCODE = tempObj[0].Substring(0, 10) + "-" + tempObj[1];
                }

                var invoice = invoiceBLL.GetInvoiceByIdAPI(objData.ID);
                if (invoice == null || invoice.COMTAXCODE != objData.COMTAXCODE)
                    return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 404, message = $"Không tìm thấy hóa đơn {objData.ID}." });

                string inputJson = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(invoice);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["CreateFileUri"]);
                httpWebRequest.ContentType = "application/json;charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentLength = inputJson.Length;
                httpWebRequest.AllowWriteStreamBuffering = false;
                httpWebRequest.SendChunked = true;

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(inputJson);
                }
                var finalResult = "";
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    finalResult = streamReader.ReadToEnd();
                    streamReader.Close();
                }
                dynamic result = new System.Web.Script.Serialization.JavaScriptSerializer().DeserializeObject(finalResult);
                return Request.CreateResponse(HttpStatusCode.OK, new { rs = true, code = 200, msg = $"Thành công.", data = new { link = uriFolder + result["msg"] } });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 500, msg = "Internal Error \n" + ex.ToString() });
            }
        }

        /// <summary>
        /// Xem hóa đơn
        /// { "comTaxCode": "0336692463", "invoiceCode": "MLK001"}
        /// </summary>
        /// <param name="comTaxCode"></param>
        /// <param name="invoiceCode"></param>
        /// <returns></returns>
        [JwtAuthentication(Realm = "Onfinance")]
        [HttpPost]
        [Route("home/v21/invoices/viewinvoicecode")]
        public HttpResponseMessage ViewPDFByInvoiceCode(HttpRequestMessage request)
        {
            InvoiceBLL invoiceBLL = new InvoiceBLL();
            var requestData = request.Content.ReadAsStringAsync().Result;
            var objData = JsonConvert.DeserializeObject<InvoiceBO>(requestData);
            try
            {
                if (objData.COMTAXCODE.Contains("-"))
                {
                    var tempObj = objData.COMTAXCODE.Split('-');
                    objData.COMTAXCODE = tempObj[0].Substring(0, 10) + "-" + tempObj[1];
                }

                var invoice = invoiceBLL.SearchByInvoiceCode(objData.INVOICECODE, objData.COMTAXCODE).FirstOrDefault();
                if (invoice == null || invoice.COMTAXCODE != objData.COMTAXCODE)
                    return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 404, message = $"Không tìm thấy hóa đơn {objData.INVOICECODE}." });

                string inputJson = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(invoice);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["CreateFileUri"]);
                httpWebRequest.ContentType = "application/json;charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentLength = inputJson.Length;
                httpWebRequest.AllowWriteStreamBuffering = false;
                httpWebRequest.SendChunked = true;

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(inputJson);
                }
                var finalResult = "";
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    finalResult = streamReader.ReadToEnd();
                    streamReader.Close();
                }
                dynamic result = new System.Web.Script.Serialization.JavaScriptSerializer().DeserializeObject(finalResult);
                return Request.CreateResponse(HttpStatusCode.OK, new { rs = true, code = 200, msg = $"Thành công.", data = new { link = uriFolder + result["msg"] } });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 500, msg = "Internal Error \n" + ex.ToString() });
            }
        }

        /// <summary>
        /// Kiểm tra hóa đơn đã phát hành chưa theo mã liên kết nếu rồi thì trả về còn không thì ký và trả về thông tin
        /// </summary>
        /// { "comtaxcode": "0336692463", "invoicecode": "ma lien ket" }
        /// <returns></returns>
        [HttpPost]
        [JwtAuthentication]
        [Route("home/v21/invoices/checkreleased")]
        public HttpResponseMessage CheckReleasedByInvoiceCode([FromBody] InvoiceBO inv)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                var invoice = invoiceBLL.SearchByInvoiceCode(inv.INVOICECODE, inv.COMTAXCODE).FirstOrDefault();
                if (invoice == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 405, msg = "Không tìm thấy hóa đơn." });
                }

                if (invoice.INVOICESTATUS == (int)INVOICE_STATUS.CONFIRMED)
                {
                    var result = new ResponseInvoiceInfo() { ID = invoice.ID, INVOICECODE = invoice.INVOICECODE, REFERENCECODE = invoice.REFERENCECODE, NUMBER = invoice.NUMBER, SIGNEDTIME = invoice.SIGNEDTIME, SIGNLINK = uriFolder + invoice.SIGNLINK.Replace(".xml", ".pdf") };

                    return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 200, msg = "Hóa đơn đã được phát hành.", data = result });
                }
                else
                {
                    var result = PostSignMultipleInvoice(new ListInvoiceIdRequest() { COMUSERNAME = invoice.COMTAXCODE, COMTAXCODE = invoice.COMTAXCODE, IDS = string.Join(";", invoice.ID) });

                    return result;
                }
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 104, msg = "Lỗi dữ liệu đầu vào không đúng chuẩn." });
            }
        }

        /// <summary>
        /// Kiểm tra hóa đơn đã phát hành chưa theo mã liên kết nếu rồi thì trả về còn không thì ký và trả về thông tin
        /// </summary>
        /// { "comtaxcode": "0336692463", "invoicecode": "1;2;3;4;5" }
        /// <returns></returns>
        [HttpPost]
        [JwtAuthentication]
        [Route("home/v21/invoices/checkmanyreleased")]
        public HttpResponseMessage CheckManyReleasedByInvoiceCode([FromBody] InvoiceBO inv)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                var invoices = invoiceBLL.SearchByInvoiceCode(inv.INVOICECODE, inv.COMTAXCODE);
                if (invoices == null || invoices.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 404, msg = "Không tìm thấy hóa đơn." });
                }

                List<ResponseInvoiceInfo> listSignedInvoices = new List<ResponseInvoiceInfo>();
                List<ResponseInvoiceInfo> listWaitingInvoices = new List<ResponseInvoiceInfo>();
                List<Task> listTasks = new List<Task>();
                int taskNum = 20;
                int qtyperTask = 0;
                CalTaskNumber(invoices.Count, ref qtyperTask, ref taskNum);

                for (int i = 0; i < taskNum; i++)
                {
                    int tempI = i;
                    var items = invoices.Skip(tempI * qtyperTask).Take(qtyperTask).ToList();
                    var task = Task.Run(() => {
                        foreach (var invoice in items)
                        {
                            if (invoice.INVOICESTATUS == (int)INVOICE_STATUS.CONFIRMED)
                            {
                                var item = new ResponseInvoiceInfo() { ID = invoice.ID, INVOICECODE = invoice.INVOICECODE, REFERENCECODE = invoice.REFERENCECODE, NUMBER = invoice.NUMBER, SIGNEDTIME = invoice.SIGNEDTIME, SIGNLINK = uriFolder + invoice.SIGNLINK.Replace(".xml", ".pdf") };
                                listSignedInvoices.Add(item);
                            }
                            else
                            {
                                var item = new ResponseInvoiceInfo() { ID = invoice.ID, INVOICECODE = invoice.INVOICECODE, REFERENCECODE = invoice.REFERENCECODE, NUMBER = invoice.NUMBER, SIGNEDTIME = invoice.SIGNEDTIME };
                                listWaitingInvoices.Add(item);
                            }
                        }
                    });
                    listTasks.Add(task);
                }
                Task.WaitAll(listTasks.ToArray());
                return Request.CreateResponse(HttpStatusCode.OK, new { rs = true, code = 200, msg = "", data = new { LISTSIGNEDINVOICES = listSignedInvoices, LISTWAITTINGINVOICES = listWaitingInvoices } });
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 104, msg = "Lỗi dữ liệu đầu vào không đúng chuẩn." });
            }
        }

        [HttpPost]
        [JwtAuthentication]
        [Route("home/v21/searchbyinvoicecode")]
        public HttpResponseMessage SearchByInvoiceCode([FromBody] InvoiceBO inv)
        {
            try
            {
                InvoiceBLL objInvoice = new InvoiceBLL();
                //Lấy master
                var lstInvoice = objInvoice.SearchByInvoiceCode(inv.INVOICECODE, inv.COMTAXCODE);
                if (lstInvoice == null || lstInvoice.Count == 0)
                    return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 405, msg = "Không tìm thấy hóa đơn." });

                //Lấy detail
                var lstIds = string.Join(";", lstInvoice.Select(x => x.ID));
                var lstProducts = objInvoice.GetInvoiceDetailByIds(lstIds);

                if (lstProducts == null || lstProducts.Count == 0)
                    return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 406, msg = "Không tìm thấy chi tiết hóa đơn." });

                lstInvoice.ForEach(obj =>
                {
                    obj.SIGNEDXML = string.IsNullOrEmpty(obj.SIGNLINK) ? null : (uriFolder + obj.SIGNLINK.Replace(".pdf", ".xml"));
                    obj.SIGNEDLINK = string.Format("https://api.onfinance.asia/api/onfinance/home/v21/viewpdf?comtaxcode={0}&id={1}", obj.COMTAXCODE, obj.ID); 
                    //obj.SIGNEDLINK = string.IsNullOrEmpty(obj.SIGNLINK) ? null : (UriFolder + obj.SIGNLINK.Replace(".xml", ".pdf"));
                    obj.LISTPRODUCT = lstProducts.Where(x => x.INVOICEID == obj.ID).ToList();
                });

                return Request.CreateResponse(HttpStatusCode.OK, new { rs = true, code = 0, lstInvoice });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 104, msg = "Lỗi dữ liệu đầu vào không đúng chuẩn." });
            }
        }

        [HttpGet]
        [Route("home/v21/viewpdf")]
        public RedirectResult ViewPDF(string comtaxcode, string id)
        {
            Uri urlNoteFound = new Uri("notefoundpdf", UriKind.RelativeOrAbsolute);

            try
            {
                string UriFolder = ConfigurationManager.AppSettings["UriFolder"];
                var uriPdf = string.Format("/{0}/{1}/hoa-don-goc/{0}_{1}_1.pdf", comtaxcode, id);
                var filePathPdf = outputInputInvoiceFolder + uriPdf;
                if (!System.IO.File.Exists(filePathPdf))
                {
                    ////var bController = new SPA_Invoice.Controllers.BaseController();
                    //ServiceResult serviceResult = SaveTemplateInvoice(Convert.ToInt64(id), 0);
                    //if (serviceResult.ErrorCode == 1000)
                    //    return Redirect(urlNoteFound);
                    InvoiceBLL invoiceBLL = new InvoiceBLL();
                    var invoice = invoiceBLL.GetInvoiceById(Convert.ToInt64(id), 0);
                    if (invoice == null || invoice.COMTAXCODE != comtaxcode)
                    {
                        return Redirect(urlNoteFound);
                    }

                    string inputJson = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(invoice);

                    var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://e.onfinance.asia/InvoiceConvert/CreatePdfInvoice");
                    httpWebRequest.ContentType = "application/json;charset=utf-8";
                    httpWebRequest.Method = "POST";
                    httpWebRequest.ContentLength = inputJson.Length;
                    httpWebRequest.AllowWriteStreamBuffering = false;
                    httpWebRequest.SendChunked = true;

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        streamWriter.Write(inputJson);
                    }
                    var finalResult = "";
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        finalResult = streamReader.ReadToEnd();
                        streamReader.Close();
                    }

                }

                return Redirect(UriFolder + uriPdf);
            }
            catch (Exception ex)
            {
                
                return Redirect(urlNoteFound);
            }
        }

        [HttpGet]
        [Route("home/v21/notefoundpdf")]
        public HttpResponseMessage NoteFoundPdf()
        {
            return Request.CreateResponse(HttpStatusCode.OK, new { rs = true, code = 0 });
        }

        #endregion

        #region Hàm dùng chung
        //private string CheckNumberWaiting(long invoiceId, ref long nextNumber)
        //{
        //    string msg = string.Empty;
        //    try
        //    {
        //        NumberBLL numberBLL = new NumberBLL();
        //        var tempNextNumber = numberBLL.GetNumberByInvoiceId2(invoiceId);
        //        if (tempNextNumber == 0)
        //            msg = "Dải hóa đơn này đã hết số.";
        //        else
        //            nextNumber = tempNextNumber;
        //    }
        //    catch (Exception ex)
        //    {
        //        msg = "Lỗi kiểm tra dải hóa đơn chờ.";
        //        throw ex;
        //    }
        //    return msg;
        //}

        public ServiceResult SaveTemplateInvoice(long invoiceId, int usingInvoiceTypeID)
        {
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                //var invoice = invoiceBLL.GetInvoiceById(invoiceId, (int)Session["USINGINVOICETYPEID"]);
                var invoice = invoiceBLL.GetInvoiceById(invoiceId, usingInvoiceTypeID);
                if (invoice == null)
                {
                    serviceResult.ErrorCode = 1000;
                    serviceResult.Message = "Không tạo được file Pdf.";
                    return serviceResult;
                }

                var baseController = new SPA_Invoice.Controllers.BaseController();
                string htmlContent = baseController.GetContentViewString(invoice, invoice.NUMBER);

                baseController.RenderHtmlToPdf(invoice, htmlContent, out string filePath, out long invoiceid);
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    serviceResult.ErrorCode = 1000;
                    serviceResult.Message = "Lỗi lấy nội dung file và đơn và tạo file Pdf.";
                    return serviceResult;
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

        private string SaveSignedInvoice(long invoiceId, long tempNextNumber, string base64xmlSigned, InvoiceBO invoice, DateTime dtSignTime, string source = null)
        {
            string msg = string.Empty;
            string filePathXml = string.Empty;
            string fileName = invoice.COMTAXCODE + "_" + invoice.ID + "_" + invoice.INVOICETYPE + ".pdf";
            try
            {
                string signLinkRef = invoice.SIGNLINK;
                if (string.IsNullOrWhiteSpace(invoice.SIGNLINK))
                    signLinkRef = CreateFileInvoicePath(invoice, fileName);

                invoice.SIGNLINK = signLinkRef;
                string pathXml = outputInputInvoiceFolder + invoice.SIGNLINK.Substring(0, invoice.SIGNLINK.Length - 4) + ".xml";

                //Lưu file XML đã ký thành công
                var outputFileNameXml = pathXml;
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
                long nextNumber = invoiceBLL.UpdateDataInvoice(invoiceId, sizeXml, invoice.SIGNLINK, tempNextNumber, dtSignTime, source);
                if (nextNumber <= 0)
                {
                    msg = "Số hóa đơn hiện tại đã hết số.";
                    return msg;
                }

                //Cập nhật số hóa đơn hiện tại
                bool isSuccess = false;
                NumberBLL numberBLL = new NumberBLL();
                //if (numberWaitingId == 0)

                isSuccess = numberBLL.UpdateCurrentNumber(invoiceId, tempNextNumber);
                //Không ký hóa đơn trong dải chờ
                //else
                //    isSuccess = numberBLL.UpdateCurrentWaittingNumber(numberWaitingId, tempNextNumber);

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
                var root = outputInputInvoiceFolder;
                var branchComTaxCode = "/" + (string.IsNullOrEmpty(invoice.COMTAXCODE) ? "COMTAXCODE/" : invoice.COMTAXCODE + "/");
                var branchInvoiceID = string.IsNullOrEmpty(invoice.ID.ToString()) ? "INVOICEID/" : invoice.ID.ToString() + "/";
                var branchInvoiceType = string.IsNullOrEmpty(invoice.INVOICETYPENAME.ToString()) ? "INVOICETYPENAME/" : ElaHelper.FormatKeywordSearch(ElaHelper.FilterVietkey(invoice.INVOICETYPENAME.ToString())).Replace(" ", "-");
                var branchDate = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                var path = string.Format($"{root}{branchComTaxCode}{branchInvoiceID}{branchInvoiceType}");

                DirectoryInfo dir = new DirectoryInfo(path);
                if (!dir.Exists)
                {
                    dir.Create();
                }

                string filePath = dir + "\\" + fileName;

                fileName = filePath.Replace(root, "").Replace('\\', '/');

                return fileName;
            }
            catch (Exception objEx)
            {
                //ConfigHelper.Instance.WriteLog("Không tạo được đường dẫn file.", objEx, MethodBase.GetCurrentMethod().Name, "CreateFileInvoicePath");
                fileName = string.Empty;
                return fileName;
            }
        }
        private List<TokenModel> GetListPartner()
        {
            try
            {
                var listPartners = new List<TokenModel>();
                var contentFile = "";
                using (StreamReader sr = new StreamReader(partnerFilePath))
                {
                    contentFile = sr.ReadToEnd();
                }
                listPartners = JsonConvert.DeserializeObject<List<TokenModel>>(contentFile);
                return listPartners;
            }
            catch (Exception ex)
            {
                return new List<TokenModel>();
            }
        }

        public string GetInvoiceNameXmlFile(int usingInvoiceType)
        {
            string invoiceName = "";
            switch (usingInvoiceType)
            {
                case (int)AccountObjectType.HOADONGTGT:
                    {
                        invoiceName = "Hóa đơn giá trị gia tăng";
                    }
                    break;
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
        #endregion
    }
}
