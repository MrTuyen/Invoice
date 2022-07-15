using DS.BusinessObject;
using DS.BusinessObject.Address;
using DS.BusinessObject.Invoice;
using DS.Common.Helpers;
using DS.DataObject;
using SAB.Library.Data;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessLogic
{
    public class GlobalBLL : BaseBLL
    {
        #region Fields
        protected IData objDataAccess = null;

        #endregion

        #region Properties

        #endregion

        #region Constructor
        public GlobalBLL()
        {
        }

        public GlobalBLL(IData objIData)
        {
            objDataAccess = objIData;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Lấy danh sách tỉnh/thành phố
        /// </summary>
        /// <returns></returns>
        public List<ProvinceBO> GetProvince(string provinceIds = null)
        {
            try
            {
                GlobalDAO objAccountDAO = new GlobalDAO();
                return objAccountDAO.GetProvince(provinceIds);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách tỉnh/thành phố");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        /// <summary>
        /// Lấy danh sách huyện/quận
        /// </summary>
        /// <returns></returns>
        public List<DistrictBO> GetDistrict(string provinceId = null)
        {
            try
            {
                GlobalDAO objAccountDAO = new GlobalDAO();
                return objAccountDAO.GetDistrict(provinceId);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách huyện/quận");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        // Lấy danh sách xã/phường
        public List<WardBO> GetWard(string districtId = null)
        {
            try
            {
                GlobalDAO objAccountDAO = new GlobalDAO();
                return objAccountDAO.GetWard(districtId);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách tỉnh/thành phố");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }


        // Lấy danh sách danh mục
        public List<GlobalBO> GetCategory(string comtaxcode)
        {
            try
            {
                GlobalDAO globalDAO = new GlobalDAO();
                return globalDAO.GetCategory(comtaxcode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách danh mục");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        // Lấy danh sách đơn vị tiền tệ
        public List<GlobalBO> GetCurrencyUnit(string comtaxcode)
        {
            try
            {
                GlobalDAO globalDAO = new GlobalDAO();
                return globalDAO.GetCurrencyUnit(comtaxcode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách đơn vị tiền tệ");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        // Lấy danh sách trạng thái hóa hóa đơn
        public List<GlobalBO> GetInvoiceStatus()
        {
            try
            {
                GlobalDAO globalDAO = new GlobalDAO();
                return globalDAO.GetInvoiceStatus();
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách trạng thái hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        // Lấy danh sách loại hóa đơn
        public List<GlobalBO> GetInvoiceType()
        {
            try
            {
                GlobalDAO globalDAO = new GlobalDAO();
                return globalDAO.GetInvoiceType();
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách loại hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public InvoiceTypeBO GetInvoiceTypeByID(int invoicetypeid)
        {
            try
            {
                GlobalDAO globalDAO = new GlobalDAO();
                return globalDAO.GetInvoiceTypeByID(invoicetypeid);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy loại hóa đơn theo ID");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        // Lấy danh sách hình thức thanh toán
        public List<GlobalBO> GetPaymentMethod(string comtaxcode)
        {
            try
            {
                GlobalDAO globalDAO = new GlobalDAO();
                return globalDAO.GetPaymentMethod(comtaxcode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách hình thức thanh toán");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        // Lấy danh sách trạng thái thanh toán
        public List<GlobalBO> GetPaymentStatus()
        {
            try
            {
                GlobalDAO globalDAO = new GlobalDAO();
                return globalDAO.GetPaymentStatus();
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách trạng thái thanh toán");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        // Lấy danh sách đơn vị tính
        public List<GlobalBO> GetQuantityUnit(string comtaxcode)
        {
            try
            {
                GlobalDAO globalDAO = new GlobalDAO();
                return globalDAO.GetQuantityUnit(comtaxcode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách trạng thái thanh toán");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        // Lấy danh sách mẫu hóa đơn
        public List<GlobalBO> GetFormCode(string comtaxcode = null, int usingInvoiceType = 0)
        {
            try
            {
                GlobalDAO globalDAO = new GlobalDAO();
                return globalDAO.GetFormCode(comtaxcode, usingInvoiceType);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách mẫu hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        // Lấy danh sách ký hiệu
        public List<GlobalBO> GetSymbolCode(string formcodeid = null, string comtaxcode = null)
        {
            try
            {
                GlobalDAO globalDAO = new GlobalDAO();
                return globalDAO.GetSymbolCode(formcodeid, comtaxcode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách ký hiệu");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        /// <summary>
        /// Chuyển chuổi string sang định file PDF, đầu ra là mảng Byte dùng để lưu thành file hoặc trả về khi người dùng có nhu cầu download
        /// </summary>
        /// <param name="htmlString"></param>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        public Byte[] ConvertStringToPDF(string htmlString, string baseUrl, string pageSizeText = "A4")
        {
            PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize), pageSizeText, true);

            string orientationPage = "Portrait";
            if (PdfPageSize.A5 == pageSize)
                orientationPage = "landscape";

            PdfPageOrientation pdfOrientation = (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation), orientationPage, true);

            int webPageWidth = 1024;
            int webPageHeight = 0;

            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();

            // set converter options
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            converter.Options.WebPageWidth = webPageWidth;
            converter.Options.WebPageHeight = webPageHeight;
            converter.Options.MarginLeft = 0;
            converter.Options.MarginRight = 0;
            converter.Options.MarginTop = 0;
            converter.Options.MarginBottom = 0;

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

        #endregion
    }
}
