using DS.BusinessLogic.Account;
using DS.BusinessLogic.Invoice;
using DS.BusinessObject;
using DS.Common.Enums;
using DS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace SPA_Invoice.Controllers
{
    public class SettingTemplateController : BaseController
    {
        // GET: SettingTemplate
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Lấy thông tin mẫu hóa đơn
        /// </summary>
        /// <param name="tempid"></param>
        /// <returns></returns>
        public ActionResult LoadTemplate(string tempid)
        {
            string msg = string.Empty;
            try
            {
                //GetTemplateInvoiceContent
                AccountBLL accountBLL = new AccountBLL();
                string formCode = "01GTKT0/001";
                string symbolCode = "AA/";
                if (objUser.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTRUONGHOC)
                {
                    formCode = "02GTTT0/001";
                    symbolCode = "AA/";
                }
                else if (objUser.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONBANHANG)
                {
                    formCode = "02GTTT1/001";
                    symbolCode = "AB/";
                }

                var objCustomer = accountBLL.GetInfoUserByUserName(objUser.USERNAME, objUser.COMTAXCODE);
                var lstProduct = new List<DS.BusinessObject.Invoice.InvoiceDetailBO>();
                if (objUser.USINGINVOICETYPE != (int)EnumHelper.AccountObjectType.HOADONTIENNUOC)
                {
                    for (var i = 0; i < 10; i++)
                    {
                        var p = new DS.BusinessObject.Invoice.InvoiceDetailBO();
                        lstProduct.Add(p);
                    }
                }
                else
                {
                    for (var i = 0; i < 4; i++)
                    {
                        var p = new DS.BusinessObject.Invoice.InvoiceDetailBO();
                        lstProduct.Add(p);
                    }
                }
                var model = new DS.BusinessObject.Invoice.InvoiceBO()
                {
                    INVOICESTATUS = -1,
                    FORMCODE = formCode,
                    SYMBOLCODE = symbolCode + DateTime.Now.Year.ToString().Substring(2, 2) + "E",
                    COMNAME = objCustomer.COMNAME,
                    COMTAXCODE = objCustomer.COMTAXCODE,
                    COMADDRESS = objCustomer.COMADDRESS,
                    COMPHONENUMBER = objCustomer.COMPHONENUMBER,
                    COMACCOUNTNUMBER = objCustomer.COMACCOUNTNUMBER,
                    COMBANKNAME = objCustomer.COMBANKNAME,
                    LISTPRODUCT = lstProduct
                };

                string htmlContent = string.Empty;
                model.TEMPLATEPATH = $"~/Views/TempInvoice/{tempid}.cshtml";

                msg = GetTemplateInvoice(model, lstProduct, 0, out htmlContent);
                if (msg.Length > 0)
                    return Json(new { rs = false, msg = msg });
                return Json(new { rs = true, res = htmlContent });
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            return Json(new { rs = false, msg = msg });
        }

        /// <summary>
        /// Lưu mới hoặc thay đổi template
        /// </summary>
        /// <param name="objNumberBO"></param>
        /// <param name="editingTemplate"></param>
        /// <param name="cssText"></param>
        /// <param name="templateFile"></param>
        /// <returns></returns>
        public ActionResult SaveTemplate(InvoiceNumberBO objNumberBO, InvoiceNumberBO editingTemplate, string templateFile)
        {
            FormSearchNumber form = new FormSearchNumber()
            {
                COMTAXCODE = objUser.COMTAXCODE
                //FORMCODE = objNumberBO.FORMCODE,
                //SYMBOLCODE = objNumberBO.SYMBOLCODE
            };
            DS.BusinessLogic.Number.NumberBLL ojNumberBLL = new DS.BusinessLogic.Number.NumberBLL();
            //Lấy danh sách mẫu của doanh nghiệp
            List<InvoiceNumberBO> lstNumber = ojNumberBLL.GetNumber(form);
            if (ojNumberBLL.ResultMessageBO.IsError)
            {
                ConfigHelper.Instance.WriteLog(ojNumberBLL.ResultMessageBO.Message, ojNumberBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetNumber");
                return Json(new { rs = false, msg = ojNumberBLL.ResultMessageBO.Message });
            }

            //Kiểm tra tình trạng phát hành
            //Chỉ cho phép user edit Status = 4 -> chưa phát hành, status != 4 là đã hoặc đang sử dụng -> không được phép edit(ghi đè)
            //Nếu cùng mẫu, cùng ký hiệu nhưng khác dải số thì copy trong database và nhập dải số mới
            var lstExist = lstNumber.Where(x => x.STATUS != 4 && x.FORMCODE == objNumberBO.FORMCODE && x.SYMBOLCODE == objNumberBO.SYMBOLCODE).ToList();
            if (lstExist.Count() > 0)
            {
                var lstNum = lstExist.Select(x => Convert.ToInt32(x.FORMCODE.Substring(x.FORMCODE.Length - 3, 3)));
                return Json(new { rs = false, msg = "Mẫu số \"" + objNumberBO.FORMCODE + "\" đã được sử dụng, bạn không được tạo nhiều mẫu cùng tên. <br>Bạn có thể dùng mẫu sau: \"<strong>01GTKT0/" + (lstNum.Max() + 1).ToString("D3") + "</strong>\"" });
            }

            //Những background dạng base64 sẽ được bóc tách lưu dạng file\
            if(objNumberBO.CSS != null)
            {
                int posStart = objNumberBO.CSS.IndexOf("data:image/", 0);
                while (posStart > -1)
                {
                    //Vị trí kết thúc base64
                    int posEnd = objNumberBO.CSS.IndexOf("');", posStart);
                    //Lấy base64 để lưu dạng file
                    string base64 = objNumberBO.CSS.Substring(posStart, posEnd - posStart).Replace("'", "");
                    //Xóa base64
                    objNumberBO.CSS = objNumberBO.CSS.Remove(posStart, posEnd - posStart);
                    //Bóc tách lấy tên class -> đặt làm tên file
                    int posNameStart = objNumberBO.CSS.LastIndexOf('}', posStart);
                    posNameStart = posNameStart == -1 ? 0 : posNameStart;
                    int posNameEnd = objNumberBO.CSS.LastIndexOf('{', posStart);
                    var name = objNumberBO.CSS.Substring(posNameStart, posNameEnd - posNameStart).Replace("}", "").Replace("\n", "").Replace(".", "").Replace(",", "").Replace(" ", "");
                    //Lưu hình
                    var filePath = UploadImage(objNumberBO.FORMCODE, objNumberBO.SYMBOLCODE, name, base64);
                    //Gán đường dẫn hình vào vị trí base64
                    objNumberBO.CSS = objNumberBO.CSS.Insert(posStart, filePath);

                    //Tìm base64 xem còn không?
                    posStart = objNumberBO.CSS.IndexOf("data:image/", posStart);
                }
            }
          
            // Open the file to read 
            templateFile = templateFile.IndexOf("/") > -1 ? "~" + templateFile : "~/Views/TempInvoice/" + templateFile + ".cshtml";
            var path = Server.MapPath(templateFile);
            string cshtml = System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8);

            //THay thế css
            //Lấy vị trí thẻ <style>
            var startIndex = cshtml.IndexOf("<style>") + 7;
            var endIndex = cshtml.IndexOf("</style>");

            //Xóa css mặc định, chèn css mới
            cshtml = cshtml.Remove(startIndex, endIndex - startIndex).Insert(startIndex, "\r\n" + objNumberBO.CSS + "\r\n");


            //Thay thế các biến về giá trị của doanh nghiệp
            cshtml = cshtml.Replace("@obj.COMNAME", objUser.COMNAME).Replace("@obj.COMTAXCODE", objUser.COMTAXCODE).Replace("@obj.COMADDRESS", objUser.COMADDRESS).Replace("@obj.COMPHONENUMBER", objUser.COMPHONENUMBER);
            cshtml = cshtml.Replace("@obj.FORMCODE", objNumberBO.FORMCODE).Replace("@obj.SYMBOLCODE", objNumberBO.SYMBOLCODE);

            InvoiceBLL objInvoiceBLL = new InvoiceBLL();
            objNumberBO.COMTAXCODE = objUser.COMTAXCODE;
            if (objUser.ISFREETRIAL)
            {
                objNumberBO.FROMNUMBER = 1;
                objNumberBO.TONUMBER = 300;
            }
            else
            {
                objNumberBO.FROMNUMBER = 0;
                objNumberBO.TONUMBER = 0;
                objNumberBO.STATUS = 4;
            }
            objNumberBO.FROMTIME = DateTime.Now;
            objNumberBO.TEMPLATEHTML = cshtml;
            objNumberBO.TEMPLATEPATH = null;
            objNumberBO.USINGINVOICETYPE = objUser.USINGINVOICETYPE;
            if (editingTemplate != null && editingTemplate.FORMCODE != null && editingTemplate.FORMCODE != "")
            {
                //Thay thế tên mẫu cũ, ký hiệu cũ bằng tên mẫu mới, ký hiệu mới
                cshtml = cshtml.Replace(editingTemplate.FORMCODE, objNumberBO.FORMCODE).Replace(editingTemplate.SYMBOLCODE, objNumberBO.SYMBOLCODE);
                //Cập nhật mẫu cũ
                objNumberBO.FROMNUMBER = editingTemplate.FROMNUMBER;
                objNumberBO.TONUMBER = editingTemplate.TONUMBER;
                objNumberBO.TEMPLATEHTML = cshtml;
            }
            objInvoiceBLL.SaveInvoiceTemplate(objNumberBO);
            if (objInvoiceBLL.ResultMessageBO.IsError)
            {
                ConfigHelper.Instance.WriteLog(objInvoiceBLL.ResultMessageBO.Message, objInvoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "SaveTemplate");
                return Json(new { rs = false, msg = objInvoiceBLL.ResultMessageBO.Message });
            }

            return Json(new { rs = true, outputTemplate = objNumberBO.TEMPLATEPATH });
        }

        public string UploadImage(string formCode, string symbolCode, string imageName, string dataImg)
        {
            try
            {
                if (dataImg.Contains("image/png;"))
                {
                    imageName += ".png";
                    dataImg = dataImg.Replace("data:image/png;base64,", "");
                }
                else
                {
                    imageName += ".jpg";
                    dataImg = dataImg.Replace("data:image/jpeg;base64,", "");
                }

                //ĐỊnh dạng tên file
                imageName = formCode.Replace("/", "") + "_" + symbolCode.Replace("/", "") + "_" + imageName;

                var folder = System.Configuration.ConfigurationManager.AppSettings["InputInvoiceFolder"];
                folder = string.Format($"/{folder}/{objUser.COMTAXCODE}/Resources/");
                var dir = System.Web.HttpContext.Current.Server.MapPath("~" + folder);
                // checking path is exist if not create the folder to download file 
                Directory.CreateDirectory(dir);

                var imageBytes = Convert.FromBase64String(dataImg);
                using (var imageFile = new FileStream(dir + "\\" + imageName, FileMode.Create))
                {
                    imageFile.Write(imageBytes, 0, imageBytes.Length);
                    imageFile.Flush();
                }

                return Request.UrlReferrer.AbsoluteUri + folder + imageName + "?v=" + DateTime.Now.Ticks;
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Không thể tải file.", objEx, MethodBase.GetCurrentMethod().Name, "Downloadfile");
                return null;
            }
        }
    }
}