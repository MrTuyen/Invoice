using DS.BusinessLogic.Invoice;
using DS.BusinessObject.Invoice;
using DS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DS.SearchInvoice.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SearchCode(string id)
        {
            try
            {
                InvoiceBLL objInvoiceBLL = new InvoiceBLL();
                var objInvoice = objInvoiceBLL.GetInvoiceByReferenceCode(id.ToUpper().Trim());
                if (objInvoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(objInvoiceBLL.ResultMessageBO.Message, objInvoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetInvoice");
                    return Json(new { rs = false, msg = objInvoiceBLL.ResultMessageBO.Message });
                }
                if (objInvoice != null)
                {
                    CreatePdfInvoice(objInvoice);
                    CreateFilePath(objInvoice);
                    return Json(new { rs = true, msg = objInvoice });
                }

                return Json(new { rs = false, msg = "Không tìm thấy hóa đơn theo mã tra cứu." });
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi khi tìm hóa đơn theo mã tra cứu", objEx, MethodBase.GetCurrentMethod().Name, "SearchCode");
                return Json(new { rs = false, msg = "Không tìm thấy hóa đơn theo mã tra cứu." });
            }
        }

        /// <summary>
        /// Chuyển đổi hóa đơn điện tử sang hóa đơn giấy
        /// </summary>
        /// <param name="id">Mã tra cứu</param>
        /// <param name="fullName">Họ và tên người chuyển đổi</param>
        /// <returns>trả ra object json</returns>
        public async Task<ActionResult> ConvertInvoice(long id, string fullName, InvoiceBO invoice)
        {
            try
            {
                string apiUrl = string.Format(ConfigHelper.UriAppAddress, "InvoiceConvert/ConvertInvoice");
                var input = new { id = id, fullName = fullName, invoice = invoice };
                string inputJson = (new JavaScriptSerializer()).Serialize(input);

                using (WebClient client = new WebClient())
                {
                    client.Headers["Content-type"] = "application/json";
                    client.Encoding = Encoding.UTF8;
                    //string str = client.UploadString(apiUrl, inputJson);
                    string str = await client.UploadStringTaskAsync(apiUrl, inputJson);
                    ConvertResult json = Newtonsoft.Json.JsonConvert.DeserializeObject<ConvertResult>(str);
                    if (json.rs)
                    {
                        return Json(new { rs = true, linkView = ConfigHelper.UriInvoiceFolder + json.msg, linkDown = json.msg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { json.rs, json.msg }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi chuyển hóa đơn sang hóa đơn giấy", objEx, MethodBase.GetCurrentMethod().Name, "ConvertInvoice");
                return Json(new { rs = false, msg = "Không thể chuyển đổi hóa đơn điện tử sang hóa đơn giấy vì hóa đơn này đã được chuyển đổi." });
            }
        }

        public ActionResult CreatePdfInvoice(InvoiceBO invoice)
        {
            try
            {
                //string apiUrl = string.Format(ConfigHelper.UriAppAddress, "InvoiceConvert/CreatePdfInvoice");
                //var input = new { invoice = invoice };
                string apiUrl = string.Format(ConfigHelper.UriAppAddress, "Invoice/CreateFilePdfToView");
                var input = new { invoiceId = invoice.ID };
                string inputJson = (new JavaScriptSerializer()).Serialize(input);

                using (WebClient client = new WebClient())
                {
                    client.Headers["Content-type"] = "application/json";
                    client.Encoding = Encoding.UTF8;
                    //string str = client.UploadString(apiUrl, inputJson);
                    string str = client.UploadString(apiUrl, inputJson);
                    ConvertResult json = Newtonsoft.Json.JsonConvert.DeserializeObject<ConvertResult>(str);
                    if (json.rs)
                    {
                        return Json(new { rs = true, linkView = ConfigHelper.UriInvoiceFolder + json.msg, linkDown = json.msg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { json.rs, json.msg }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi chuyển hóa đơn sang hóa đơn giấy", objEx, MethodBase.GetCurrentMethod().Name, "ConvertInvoice");
                return Json(new { rs = false, msg = "Không thể chuyển đổi hóa đơn điện tử sang hóa đơn giấy vì hóa đơn này đã được chuyển đổi." });
            }
        }

        public ActionResult SearchFile(FormCollection collection)
        {
            try
            {
                string checksumCode = string.Empty;
                //ĐỌc file tải lên
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        //Tạo mã checksum từ file tải lên
                        Stream stream = fileContent.InputStream;
                        checksumCode = Checksum.CreateMD5(stream);
                    }
                }

                //Nếu việc tải lên thành công thì tiến hành kiếm tra
                if (!string.IsNullOrEmpty(checksumCode))
                {
                    //Lấy thông tin từ DB
                    InvoiceBLL objInvoiceBLL = new InvoiceBLL();
                    var objInvoice = objInvoiceBLL.GetInvoiceByChecksum(checksumCode);
                    if (objInvoiceBLL.ResultMessageBO.IsError)
                    {
                        ConfigHelper.Instance.WriteLog(objInvoiceBLL.ResultMessageBO.Message, objInvoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetInvoice");
                        return Json(new { rs = false, msg = objInvoiceBLL.ResultMessageBO.Message });
                    }

                    //Mọi thứ ổn, tiến hành kiểm tra xem có tồn tại file với mã vừa checksum hay không
                    if (objInvoice != null)
                    {
                        ////Có tồn tại, gửi lên obj hóa đơn
                        ////Giữ lại đường dẫn trên DB
                        //objInvoice.PREVIEWLINK = objInvoice.SIGNEDLINK;
                        ////Tạo đường dẫn file xml
                        //objInvoice.SIGNEDXML = objInvoice.PREVIEWLINK != null ? objInvoice.PREVIEWLINK.Substring(0, objInvoice.PREVIEWLINK.Length - 4) + ".xml" : null;
                        ////Tạo url file pdf để view lên web
                        //objInvoice.SIGNEDLINK = objInvoice.PREVIEWLINK != null ? ConfigHelper.UriInvoiceFolder + objInvoice.PREVIEWLINK : null;

                        CreateFilePath(objInvoice);
                        return Json(new { rs = true, msg = objInvoice });
                    }
                    else
                    {
                        //Không tồn tại, file ảo
                        return Json(new { rs = false, msg = "Nội dung file xml không khớp với bất kỳ hóa đơn nào đã phát hành trên Onfinance.asia. <br><br>Bạn vui lòng kiểm tra lại." });
                    }
                }


                return Json(new { rs = false, msg = "Lỗi đọc file, bạn vui lòng kiểm tra lại file vừa chọn có đúng định dạng xml hay không." });
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi đọc file xml", objEx, MethodBase.GetCurrentMethod().Name, "ImportProductFromExcel");
                return Json(new { rs = false, msg = "Lỗi đọc file, bạn vui lòng kiểm tra lại file vừa chọn có đúng định dạng xml hay không." });
            }
        }

        void CreateFilePath(BusinessObject.Invoice.InvoiceBO objInvoice)
        {
            //Giữ lại đường dẫn trên DB
            objInvoice.PREVIEWLINK = objInvoice.SIGNLINK;
            //Tạo đường dẫn file xml, nếu là hóa đơn hủy thì lấy cho tải file hóa đơn gốc
            if (objInvoice.INVOICETYPE == 3)
            {
                objInvoice.SIGNEDXML = objInvoice.PREVIEWLINK != null ? objInvoice.PREVIEWLINK.Substring(0, objInvoice.PREVIEWLINK.Length - 4) + ".xml" : null;
                //objInvoice.SIGNEDXML = objInvoice.SIGNEDXML.Replace("hoa-don-huy", "hoa-don-goc").Replace("_3.xml", "_1.xml");
                objInvoice.SIGNEDXML = objInvoice.SIGNEDXML.Replace("hoa-don-xoa-bo", "hoa-don-goc").Replace("_3.xml", "_1.xml");
            }
            else if (objInvoice.INVOICETYPE == 4)
            {
                objInvoice.SIGNEDXML = objInvoice.PREVIEWLINK != null ? objInvoice.PREVIEWLINK.Substring(0, objInvoice.PREVIEWLINK.Length - 4) + ".xml" : null;
                objInvoice.SIGNEDXML = objInvoice.SIGNEDXML.Replace("hoa-don-chuyen-doi", "hoa-don-goc").Replace("_4.xml", "_1.xml");
            }
            else
                objInvoice.SIGNEDXML = objInvoice.PREVIEWLINK != null ? objInvoice.PREVIEWLINK.Substring(0, objInvoice.PREVIEWLINK.Length - 4) + ".xml" : null;

            //Nếu là hóa đơn chuyển đổi thì vẫn phải tải xml từ folder hóa đơn gốc
            objInvoice.SIGNEDXML = objInvoice.SIGNEDXML.Replace("hoa-don-xoa-bo", "hoa-don-goc").Replace("_3.xml", "_1.xml");

            //Tạo url file pdf để view lên web
            objInvoice.SIGNEDLINK = objInvoice.PREVIEWLINK != null ? ConfigHelper.UriInvoiceFolder + objInvoice.PREVIEWLINK : null;

            //Lấy link file chứng chỉ
            objInvoice.CERTIFICATELINK = $"/{objInvoice.COMTAXCODE}/ChungChiChuKySo_{objInvoice.COMTAXCODE}.cer";
        }

        public ActionResult Downloadfile(string link)
        {
            try
            {
                //Loại bỏ domain
                //link = link.Replace(ConfigHelper.UriInvoiceFolder, "");

                //Tách lấy tên file
                string[] path = link.Split('/');

                //Đưa về đường dẫn vật lý
                link = ConfigHelper.PhysicalInvoiceFolder + link.Replace('/', '\\');

                //Đọc file
                byte[] fileBytes = GetFile(link);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, path[path.Length - 1]);
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi tải file.", objEx, MethodBase.GetCurrentMethod().Name, "Downloadfile");
                return null;
            }
        }

        byte[] GetFile(string s)
        {
            using (FileStream fs = System.IO.File.OpenRead(s))
            {
                //System.IO.FileStream fs = System.IO.File.OpenRead(s);
                byte[] data = new byte[fs.Length];
                int br = fs.Read(data, 0, data.Length);
                if (br != fs.Length)
                    throw new IOException(s);
                return data;
            }
        }
    }

    public class ConvertResult
    {
        public bool rs { get; set; }
        public string msg { get; set; }
    }
}