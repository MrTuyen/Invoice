using DS.BusinessLogic.Customer;
using DS.BusinessLogic.Product;
using DS.BusinessObject.Customer;
using DS.BusinessObject;
using DS.BusinessObject.Invoice;
using DS.BusinessObject.Product;
using DS.BusinessObject.Report;
using DS.Common.Helpers;
using DS.DataObject.Invoice;
using iTextSharp.text;
using iTextSharp.text.pdf;
using QRCoder;
using SAB.Library.Data;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml.Serialization;
using static DS.Common.Enums.EnumHelper;
using Font = iTextSharp.text.Font;
using Image = iTextSharp.text.Image;
using Rectangle = iTextSharp.text.Rectangle;
using DS.DataObject.Category;
using DS.DataObject.Receipt;
using DS.BusinessObject.Receipt;

namespace DS.BusinessLogic.Receipt
{
    public class ReceiptBLL : BaseBLL
    {
        #region Fields
        protected IData objDataAccess = null;

        #endregion

        #region Properties

        #endregion

        #region Constructor
        public ReceiptBLL()
        {
        }

        public ReceiptBLL(IData objIData)
        {
            objDataAccess = objIData;
        }
        #endregion

        #region Methods
        public List<InvoiceBO> GetReceiptPaging(FormSearchInvoice form)
        {
            try
            {
                ReceiptDAO oDL = new ReceiptDAO();
                return oDL.GetReceipt(form);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<InvoiceBO>();
            }
        }

        public string AddReceipt(ReceiptBO objReceipt, out long invoiceId)
        {
            string msg = string.Empty;
            invoiceId = 0;
            try
            {
                ReceiptDAO receiptDAO = new ReceiptDAO();
                invoiceId = receiptDAO.AddReceipt(objReceipt);
            }
            catch (Exception objEx)
            {
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                msg = "Không thêm được Biên lai thu phí, lệ phí.";
            }
            return msg;
        }

        public string UpdateReceipt(ReceiptBO invoice, out long invoiceId)
        {
            string msg = string.Empty;
            invoiceId = 0;
            try
            {
                ReceiptDAO oDL = new ReceiptDAO();
                invoiceId = oDL.UpdateReceipt(invoice);
            }
            catch (Exception objEx)
            {
                msg = "Lỗi cập nhật hóa đơn";
                objResultMessageBO = ConfigHelper.Instance.WriteLog(msg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
            }
            return msg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceid"></param>
        /// <param name="signLink"></param>
        /// <returns></returns>
        public bool UpdateInvoiceSignLink(long invoiceid, string signLink, string referenceCode)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.UpdateInvoiceSignLink(invoiceid, signLink, referenceCode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật sign link hóa đơn, long invoiceid, string signLink, string referenceCode");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        public ReceiptBO GetReceiptById(long invoiceid)
        {
            try
            {
                ReceiptDAO receiptDAO = new ReceiptDAO();
                return receiptDAO.GetReceiptById(invoiceid);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public string SaveInvoiceFile(byte[] dataBuffer, ReceiptBO invoice, string fileName)
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
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    if (dataBuffer.Length > 0)
                    {
                        fileStream.Write(dataBuffer, 0, dataBuffer.Length);
                    }
                }
                fileName = filePath.Replace(HttpContext.Current.Server.MapPath("~/" + root), "").Replace('\\', '/');

                return fileName;
            }
            catch (Exception objEx)
            {
                fileName = string.Empty;
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Không thể lưu mẫu hóa đơn, vui lòng kiểm tra lại mẫu và ký hiệu hóa đơn.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public string GenerateInvoiceXML(ReceiptBO invoice, long nextNumber, string invoiceName)
        {
            string msg = string.Empty;
            try
            {
                DateTime dtSignTime = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("inv", "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1");
                ns.Add("ds", "http://www.w3.org/2000/09/xmldsig#");

                invoice invoiceInstance = new invoice();
                invoiceInvoiceData invData = new invoiceInvoiceData()
                {
                    id = "data",
                    invoiceType = invoice.INVOICETYPENAME,
                    templateCode = invoice.FORMCODE,
                    invoiceSeries = invoice.SYMBOLCODE,
                    invoiceNumber = nextNumber.ToString("D7"),
                    invoiceName = invoiceName,
                    invoiceIssuedDate = dtSignTime,
                    currencyCode = invoice.CURRENCY,
                    adjustmentType = invoice.INVOICETYPE,
                    signedDate = dtSignTime,
                    payments = new invoiceInvoiceDataPayments()
                    {
                        payment = new invoiceInvoiceDataPaymentsPayment()
                        {
                            paymentMethodName = invoice.CUSPAYMENTMETHOD
                        }
                    },
                    delivery = "",
                    sellerLegalName = invoice.COMNAME,
                    sellerTaxCode = invoice.COMTAXCODE,
                    sellerAddressLine = invoice.COMADDRESS,
                    sellerPhoneNumber = invoice.CUSPHONENUMBER,
                    sellerFaxNumber = "00000",
                    sellerEmail = invoice.CUSEMAIL,
                    sellerBankAccount = null,
                    sellerBankName = "",

                    buyerLegalName = invoice.CUSNAME,
                    buyerTaxCode = invoice.CUSTAXCODE,
                    buyerAddressLine = invoice.CUSADDRESS,
                    deliveryOrderDate = dtSignTime,
                    exchangeRate = invoice.EXCHANGERATE,
                    invoiceTaxBreakdowns = new invoiceInvoiceDataInvoiceTaxBreakdowns()
                    {
                        invoiceTaxBreakdown = new invoiceInvoiceDataInvoiceTaxBreakdownsInvoiceTaxBreakdown()
                        {
                            vatPercentage = 0
                        }
                    },
                    invoiceNotes = invoice.NOTE,
                    totalAmountWithVATInWords = ReadNumberToCurrencyWords.ConvertToWordsWithPostfix(invoice.TOTALMONEY, invoice.CURRENCY),
                    isDiscountAmtPos = 0,
                    totalAmountWithVATFrn = Math.Round(invoice.TOTALMONEY, MidpointRounding.AwayFromZero),
                    userDefines = "<![CDATA[<TransactionID>4ZFJUEKLAE</TransactionID><InvoiceRefID>d2a7ec5f-c008-47c7-a781-67b33cd27083</InvoiceRefID><InvoiceTemplateID>ca6b2dfa-adb6-4360-90b7-d44770120c92</InvoiceTemplateID><MainCurrency>VND</MainCurrency><UnitPriceDecimalDigits>2</UnitPriceDecimalDigits><UnitPriceOCDecimalDigits>2</UnitPriceOCDecimalDigits><QuantityDecimalDigits>2</QuantityDecimalDigits><AmountDecimalDigits>0</AmountDecimalDigits><AmountOCDecimalDigits>2</AmountOCDecimalDigits><ExchangRateDecimalDigits>2</ExchangRateDecimalDigits><CoefficientDecimalDigits>2</CoefficientDecimalDigits><AccountObjectCode>KH08178</AccountObjectCode><UserItems><UserItem><LineNumber>1</LineNumber><DiscountRate>0.00 </DiscountRate><DiscountAmountOC>0 </DiscountAmountOC><DiscountAmount>0 </DiscountAmount><PanelLengthQuantity>0.00 </PanelLengthQuantity><PanelWidthQuantity>0.00 </PanelWidthQuantity><PanelHeightQuantity>0.00 </PanelHeightQuantity><PanelRadiusQuantity>0.00 </PanelRadiusQuantity><PanelQuantity>0.00 </PanelQuantity><InventoryItemType>0</InventoryItemType><RowType>0</RowType><MainUnitName>CÁI</MainUnitName><MainConvertRate>1.00 </MainConvertRate><MainQuantity>2.00 </MainQuantity><MainUnitPrice>863636.36 </MainUnitPrice></UserItem></UserItems>]]>"
                };
                invoiceControlData invControlData = new invoiceControlData()
                {
                    systemCode = "NOVAON_ONFINANCE"
                };

                invoiceInstance.invoiceData = invData;
                invoiceInstance.controlData = invControlData;

                string root = ConfigurationManager.AppSettings["InputInvoiceFolder"]; // physical folder in local or virtual path on cloud server by getting the appconfig 

                string branchComTaxCode = "/" + (string.IsNullOrEmpty(invoice.COMTAXCODE) ? "COMTAXCODE/" : invoice.COMTAXCODE + "/");

                string branchInvoiceID = string.IsNullOrEmpty(invoice.ID.ToString()) ? "INVOICEID/" : invoice.ID.ToString() + "/";

                string branchInvoiceType = string.IsNullOrEmpty(invoice.INVOICETYPENAME.ToString()) ? "INVOICETYPENAME/" : ElaHelper.FormatKeywordSearch(ElaHelper.FilterVietkey(invoice.INVOICETYPENAME.ToString())).Replace(" ", "-");

                string branchDate = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();

                string path = string.Format($"{root}{branchComTaxCode}{branchInvoiceID}{branchInvoiceType}");
                // checking path is exist if not create the folder to download file 
                DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/" + path));
                if (!dir.Exists)
                {
                    dir.Create();
                }
                // write all the data from memory stream to fileName is created in the folder just created above
                string fileName = dir + "/" + invoice.COMTAXCODE + "_" + invoice.ID + "_" + invoice.INVOICETYPE + ".xml";
                XMLHelper.SerializationXmlWithPrefix(invoiceInstance, fileName, ns);

                fileName = fileName.Replace(HttpContext.Current.Server.MapPath("~/" + root), "").Replace('\\', '/');
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi tạo hóa đơn xml", ex, MethodBase.GetCurrentMethod().Name, "GenerateInvoiceXML");
                msg = "Lỗi tạo biên lai ra file xml.";
            }
            return msg;
        }
        #endregion

    }
}
