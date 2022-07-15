using DS.BusinessLogic.Customer;
using DS.BusinessLogic.Product;
using DS.BusinessObject;
using DS.BusinessObject.Invoice;
using DS.BusinessObject.Product;
using DS.BusinessObject.Report;
using DS.Common.Helpers;
using DS.DataObject.Invoice;
using SAB.Library.Data;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml.Serialization;
using DS.DataObject.Category;
using System.Web.UI.WebControls;
using System.Web.UI;
using DS.Common.Enums;
using System.Collections;
using System.Data;
using DS.BusinessObject.Output;
using static DS.Common.Enums.EnumHelper;
using iTextSharp.text.html.simpleparser;
using System.Windows.Automation;
using System.Threading;
using System.Threading.Tasks;

namespace DS.BusinessLogic.Invoice
{
    public class InvoiceBLL : BaseBLL
    {
        #region Fields
        protected IData objDataAccess = null;

        #endregion

        #region Properties

        #endregion

        #region Constructor
        public InvoiceBLL()
        {
        }

        public InvoiceBLL(IData objIData)
        {
            objDataAccess = objIData;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Lưu hóa đơn
        /// </summary>
        /// <returns></returns>
        public long AddInvoice(InvoiceBO invoice)
        {
            long invoiceId = 0;
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                CustomerBLL customerBLL = new CustomerBLL();
                invoiceId = invoiceDAO.AddInvoice(invoice);
                if (invoiceId > 0)
                {
                    var listDetail = invoice.LISTPRODUCT.GroupBy(x => new { x.PRODUCTNAME, x.TAXRATE, x.RETAILPRICE, x.ISPROMOTION }).Select(y => y.ToList()).ToList();
                    List<InvoiceDetailBO> lstInvoiceDetail = new List<InvoiceDetailBO>();
                    foreach (var item in listDetail)
                    {
                        item[0].QUANTITY = item.Sum(x => x.QUANTITY);
                        lstInvoiceDetail.Add(item[0]);
                    }
                    invoice.LISTPRODUCT = lstInvoiceDetail;
                    foreach (var item in invoice.LISTPRODUCT)
                    {
                        item.INVOICEID = invoiceId;
                        CalculateInvoiceDetailData(item, invoice);
                        invoiceDAO.SaveInvoiceDetail(item);
                        ProductBLL productBLL = new ProductBLL();
                        var result = productBLL.AddProduct(new ProductBO()
                        {
                            PRODUCTNAME = item.PRODUCTNAME.Replace("  ", "").Trim(),
                            COMTAXCODE = invoice.COMTAXCODE,
                            PRODUCTTYPE = 0,
                            PRICE = item.RETAILPRICE,
                            QUANTITYUNIT = item.QUANTITYUNIT
                        });

                        if (!string.IsNullOrEmpty(item.QUANTITYUNIT))
                        {
                            CategoryDAO categoryDAO = new CategoryDAO();
                            categoryDAO.SaveQuantityUnit(new MethodHelper().ToUpperFirstLetter(item.QUANTITYUNIT.ToLower().Trim()),0,invoice.COMTAXCODE);
                        }
                    }
                    return invoiceId;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return -1;
            }
        }

        public long AddInvoiceUpdate(InvoiceBO invoice)
        {
            long invoiceId = 0;
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                CustomerBLL customerBLL = new CustomerBLL();
                invoiceId = invoiceDAO.AddInvoice(invoice);
                if (invoiceId > 0)
                {
                    var listDetail = invoice.LISTPRODUCT.GroupBy(x => new { x.PRODUCTNAME, x.TAXRATE, x.RETAILPRICE, x.ISPROMOTION }).Select(y => y.ToList()).ToList();
                    List<InvoiceDetailBO> lstInvoiceDetail = new List<InvoiceDetailBO>();
                    foreach (var item in listDetail)
                    {
                        item[0].QUANTITY = item.Sum(x => x.QUANTITY);
                        lstInvoiceDetail.Add(item[0]);
                    }
                    invoice.LISTPRODUCT = lstInvoiceDetail;
                    foreach (var item in invoice.LISTPRODUCT)
                    {
                        item.INVOICEID = invoiceId;
                        CalculateInvoiceDetailData(item, invoice);
                        invoiceDAO.SaveInvoiceDetail(item);
                        ProductBLL productBLL = new ProductBLL();
                        var result = productBLL.AddProduct(new ProductBO()
                        {
                            PRODUCTNAME = item.PRODUCTNAME.Replace("  ", "").Trim(),
                            COMTAXCODE = invoice.COMTAXCODE,
                            PRODUCTTYPE = 0,
                            PRICE = item.RETAILPRICE,
                            QUANTITYUNIT = item.QUANTITYUNIT,

                        });

                        if (!string.IsNullOrEmpty(item.QUANTITYUNIT))
                        {
                            CategoryDAO categoryDAO = new CategoryDAO();
                            categoryDAO.SaveQuantityUnit(new MethodHelper().ToUpperFirstLetter(item.QUANTITYUNIT.ToLower().Trim()),0,invoice.COMTAXCODE);
                        }
                    }
                    return invoiceId;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return -1;
            }
        }

        public List<InvoiceDetailBO> GetInvoiceDetailByIds(string iDs)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.GetInvoiceDetailByIds(iDs);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy chi tiết hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        private void CalculateInvoiceDetailData(InvoiceDetailBO invoiceDetailBO, InvoiceBO invoice)
        {
            // Fix bug một số chi tiết hóa đơn tổng tiền trước thuế = 0
            if (invoice.USINGINVOICETYPE == (int)AccountObjectType.HOADONGTGT)
            {
                if (invoiceDetailBO.QUANTITY != 0 || invoiceDetailBO.RETAILPRICE != 0)
                {
                    invoiceDetailBO.TOTALMONEY = invoiceDetailBO.QUANTITY * invoiceDetailBO.RETAILPRICE;
                }
            }
            else if (invoice.USINGINVOICETYPE == (int)AccountObjectType.HOADONTIENDIEN)
            {
                invoiceDetailBO.TOTALMONEY = invoiceDetailBO.QUANTITY * invoiceDetailBO.RETAILPRICE;
            }
            if (invoice.DISCOUNTTYPE == "CHIET_KHAU_THEO_HANG_HOA")
            {
                if (invoiceDetailBO.DISCOUNTRATE > 0 && invoiceDetailBO.TOTALDISCOUNT <= 0)
                {
                    invoiceDetailBO.TOTALDISCOUNT = invoiceDetailBO.TOTALMONEY * (decimal)(invoiceDetailBO.DISCOUNTRATE / 100);
                }
                else if(invoiceDetailBO.DISCOUNTRATE <= 0 && invoiceDetailBO.TOTALDISCOUNT > 0)
                {
                    invoiceDetailBO.DISCOUNTRATE = (invoiceDetailBO.TOTALDISCOUNT / invoiceDetailBO.TOTALMONEY) * 100;
                }
            }
            //Nếu có truyền trực tiếp tiền VAT thì k cần tính lại, lấy trực tiếp số tiền mà user gán
            if (invoiceDetailBO.TOTALTAX == 0)
            {
                int tempTax = invoiceDetailBO.TAXRATE == -1 ? 0 : invoiceDetailBO.TAXRATE;
                invoiceDetailBO.TOTALTAX = (invoiceDetailBO.TOTALMONEY - invoiceDetailBO.TOTALDISCOUNT) * (decimal)(tempTax / 100.00);
            }
            invoiceDetailBO.TOTALPAYMENT = invoiceDetailBO.TOTALMONEY - invoiceDetailBO.TOTALDISCOUNT + invoiceDetailBO.TOTALTAX + invoiceDetailBO.OTHERTAXFEE - invoiceDetailBO.REFUNDFEE;
        }
        public long InsertWithTask(InvoiceBO invoice)
        {
            InvoiceDAO invoiceDAO = new InvoiceDAO();
            long invoiceId = 0;
            try
            {
                //Tính toán thêm 1 số trường trong invoice_detail(tax, discount, totalmoney ) và invoice(totaldiscount, totaltax)
                switch (invoice.USINGINVOICETYPE)
                {
                    case (int)EnumHelper.AccountObjectType.HOADONGTGT:
                    case (int)EnumHelper.AccountObjectType.PHIEUXUATKHO:
                    case (int)EnumHelper.AccountObjectType.HOADONTIENDIEN:
                        invoice.TAXMONEY = 0;
                        invoice.DISCOUNTMONEY = 0;
                        invoice.TOTALMONEY = 0;
                        invoice.TOTALPAYMENT = 0;
                        invoice.OTHERTAXFEE = 0;
                        invoice.REFUNDFEE = 0;
                        foreach (var item in invoice.LISTPRODUCT)
                        {
                            CalculateInvoiceDetailData(item, invoice);
                            invoice.TAXMONEY += item.TOTALTAX;
                            invoice.DISCOUNTMONEY += item.TOTALDISCOUNT;
                            invoice.TOTALMONEY += item.TOTALMONEY - item.TOTALDISCOUNT;
                            invoice.OTHERTAXFEE += item.OTHERTAXFEE;
                            invoice.REFUNDFEE += item.REFUNDFEE;
                            invoice.TOTALPAYMENT += item.TOTALPAYMENT;
                        }
                        break;
                    case (int)EnumHelper.AccountObjectType.HOADONBANHANG:
                        invoice.TAXMONEY = 0;
                        invoice.DISCOUNTMONEY = 0;
                        invoice.TOTALMONEY = 0;
                        invoice.TOTALPAYMENT = 0;
                        foreach (var item in invoice.LISTPRODUCT)
                        {
                            CalculateInvoiceDetailData(item, invoice);
                            invoice.DISCOUNTMONEY += item.TOTALDISCOUNT;
                            invoice.TOTALMONEY += item.TOTALMONEY - item.TOTALDISCOUNT;
                            invoice.TOTALPAYMENT += item.TOTALMONEY;
                        }
                        break;
                }
                invoiceId = invoiceDAO.AddInvoiceNoTrans(invoice);

                if (invoice.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTIENDIEN)
                {
                    if (invoiceId > 0)
                    {
                        invoice.LISTPRODUCT = invoice.LISTPRODUCT != null ? invoice.LISTPRODUCT.Where(x => !string.IsNullOrEmpty(x.PRODUCTNAME)).ToList() : new List<InvoiceDetailBO>();
                        invoice.LISTPRODUCT.ForEach(x => x.INVOICEID = invoiceId);

                        foreach (var item in invoice.LISTPRODUCT)
                        {
                            invoiceDAO.AddInvoiceDetailNoTrans(item);
                        }
                    }

                }
                else
                {
                    if (invoiceId > 0)
                    {
                        invoice.LISTPRODUCT = invoice.LISTPRODUCT != null ? invoice.LISTPRODUCT.Where(x => !string.IsNullOrEmpty(x.PRODUCTNAME)).ToList() : new List<InvoiceDetailBO>();
                        invoice.LISTPRODUCT.ForEach(x => x.INVOICEID = invoiceId);

                        foreach (var item in invoice.LISTPRODUCT)
                        {
                            invoiceDAO.AddInvoiceDetailNoTrans(item);

                        }

                    }

                }
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog(objEx.ToString(), objEx, MethodBase.GetCurrentMethod().Name, "AddInvoiceNoTrans BLL");
                //throw objEx;
            }
            return invoiceId;
        }
        
        
        public  void RollBackInvoice(List<long> invoices)
        {
            
            InvoiceDAO invoiceDAO = new InvoiceDAO();
            invoiceDAO.DeleteInvoices(invoices);
        }
        /// <summary>
        /// Tạo  mới hóa đơn
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>

        public long CreateInvoice(InvoiceBO invoice)
        {
            long invoiceId = 0;
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                CustomerBLL customerBLL = new CustomerBLL();

                if (invoice.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONGTGT
                    || invoice.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.PHIEUXUATKHO
                    || invoice.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTIENDIEN)
                {
                    invoice.TAXMONEY = 0;
                    invoice.DISCOUNTMONEY = 0;
                    invoice.TOTALMONEY = 0;
                    invoice.TOTALPAYMENT = 0;
                    invoice.OTHERTAXFEE = 0;
                    invoice.REFUNDFEE = 0;
                    foreach (var item in invoice.LISTPRODUCT)
                    {
                        CalculateInvoiceDetailData(item, invoice);
                        invoice.TAXMONEY += item.TOTALTAX;
                        invoice.DISCOUNTMONEY += item.TOTALDISCOUNT;
                        invoice.TOTALMONEY += item.TOTALMONEY - item.TOTALDISCOUNT;
                        invoice.OTHERTAXFEE += item.OTHERTAXFEE;
                        invoice.REFUNDFEE += item.REFUNDFEE;
                        invoice.TOTALPAYMENT += item.TOTALPAYMENT;
                    }
                }
                else if (invoice.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONBANHANG)
                {
                    invoice.TAXMONEY = 0;
                    invoice.DISCOUNTMONEY = 0;
                    invoice.TOTALMONEY = 0;
                    invoice.TOTALPAYMENT = 0;
                    foreach (var item in invoice.LISTPRODUCT)
                    {
                        CalculateInvoiceDetailData(item, invoice);
                        invoice.DISCOUNTMONEY += item.TOTALDISCOUNT;
                        invoice.TOTALMONEY += item.TOTALMONEY - item.TOTALDISCOUNT;
                        invoice.TOTALPAYMENT += item.TOTALMONEY;
                    }
                }

                invoiceId = invoiceDAO.AddInvoice(invoice);
                if (invoice.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTIENDIEN)
                {
                    if (invoiceId > 0)
                    {
                        invoice.LISTPRODUCT = invoice.LISTPRODUCT != null ? invoice.LISTPRODUCT.Where(x => !string.IsNullOrEmpty(x.PRODUCTNAME)).ToList() : new List<InvoiceDetailBO>();
                        invoice.LISTPRODUCT.ForEach(x => x.INVOICEID = invoiceId);

                        foreach (var item in invoice.LISTPRODUCT)
                        {
                            invoiceDAO.SaveInvoiceDetail(item);
                        }
                        return invoiceId;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    if (invoiceId > 0)
                    {
                        invoice.LISTPRODUCT = invoice.LISTPRODUCT != null ? invoice.LISTPRODUCT.Where(x => !string.IsNullOrEmpty(x.PRODUCTNAME)).ToList() : new List<InvoiceDetailBO>();
                        invoice.LISTPRODUCT.ForEach(x => x.INVOICEID = invoiceId);

                        foreach (var item in invoice.LISTPRODUCT)
                        {
                            invoiceDAO.SaveInvoiceDetail(item);
                            //Hạn chế tạo product đối với đối tượng khách hàng dùng import từ excel số lượng lớn
                            if (invoice.LISTPRODUCT.Count < 50)
                            {
                                ProductBLL productBLL = new ProductBLL();
                                var result = productBLL.AddProduct(new ProductBO()
                                {
                                    PRODUCTNAME = item.PRODUCTNAME.Replace("  ", " ").Trim(),
                                    COMTAXCODE = invoice.COMTAXCODE,
                                    PRODUCTTYPE = 0,
                                    PRICE = item.RETAILPRICE,
                                    QUANTITYUNIT = item.QUANTITYUNIT,
                                    METERCODE = item.METERCODE,
                                    FACTOR = item.FACTOR
                                });
                            }
                        }
                        return invoiceId;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return -1;
            }
        }

        public CompanySummary GetSumany()
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.GetSumany();
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy báo cáo tổng quan");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        //public long CreateInvoice(InvoiceBO invoice)
        //{
        //    long invoiceId = 0;
        //    try
        //    {
        //        InvoiceDAO invoiceDAO = new InvoiceDAO();
        //        CustomerBLL customerBLL = new CustomerBLL();
        //        //Tính toán thêm 1 số trường trong invoice_detail(tax, discount, totalmoney ) và invoice(totaldiscount, totaltax)
        //        if (invoice.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONGTGT
        //            || invoice.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.PHIEUXUATKHO
        //            || invoice.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTIENDIEN)
        //        {
        //            invoice.TAXMONEY = 0;
        //            invoice.DISCOUNTMONEY = 0;
        //            invoice.TOTALMONEY = 0;
        //            invoice.TOTALPAYMENT = 0;
        //            invoice.OTHERTAXFEE = 0;
        //            foreach (var item in invoice.LISTPRODUCT)
        //            {
        //                CalculateInvoiceDetailData(item, invoice);
        //                invoice.TAXMONEY += item.TOTALTAX;
        //                invoice.DISCOUNTMONEY += item.TOTALDISCOUNT;
        //                invoice.TOTALMONEY += item.TOTALMONEY - item.TOTALDISCOUNT;
        //                invoice.OTHERTAXFEE += item.OTHERTAXFEE;
        //                invoice.TOTALPAYMENT += item.TOTALPAYMENT;
        //            }
        //        }

        //        invoiceId = invoiceDAO.AddInvoice(invoice);
        //        if (invoice.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTIENDIEN)
        //        {
        //            if (invoiceId > 0)
        //            {
        //                invoice.LISTPRODUCT = invoice.LISTPRODUCT != null ? invoice.LISTPRODUCT.Where(x => !string.IsNullOrEmpty(x.PRODUCTNAME)).ToList() : new List<InvoiceDetailBO>();
        //                invoice.LISTPRODUCT.ForEach(x => x.INVOICEID = invoiceId);

        //                foreach (var item in invoice.LISTPRODUCT)
        //                {
        //                    invoiceDAO.SaveInvoiceDetail(item);
        //                }
        //                return invoiceId;
        //            }
        //            else
        //            {
        //                return -1;
        //            }
        //        }
        //        else
        //        {
        //            if (invoiceId > 0)
        //            {
        //                invoice.LISTPRODUCT = invoice.LISTPRODUCT != null ? invoice.LISTPRODUCT.Where(x => !string.IsNullOrEmpty(x.PRODUCTNAME)).ToList() : new List<InvoiceDetailBO>();
        //                invoice.LISTPRODUCT.ForEach(x => x.INVOICEID = invoiceId);

        //                foreach (var item in invoice.LISTPRODUCT)
        //                {
        //                    invoiceDAO.SaveInvoiceDetail(item);
        //                    //Hạn chế tạo product đối với đối tượng khách hàng dùng import từ excel số lượng lớn
        //                    if (invoice.LISTPRODUCT.Count < 50)
        //                    {
        //                        ProductBLL productBLL = new ProductBLL();
        //                        var result = productBLL.AddProduct(new ProductBO()
        //                        {
        //                            PRODUCTNAME = item.PRODUCTNAME.Replace("  ", " ").Trim(),
        //                            COMTAXCODE = invoice.COMTAXCODE,
        //                            PRODUCTTYPE = 0,
        //                            PRICE = item.RETAILPRICE,
        //                            QUANTITYUNIT = item.QUANTITYUNIT,
        //                            METERCODE = item.METERCODE,
        //                            FACTOR = item.FACTOR
        //                        });
        //                    }
        //                }
        //                return invoiceId;
        //            }
        //            else
        //            {
        //                return -1;
        //            }
        //        }
        //    }
        //    catch (Exception objEx)
        //    {
        //        this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật hóa đơn");
        //        objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
        //        return -1;
        //    }
        //}

        private void SaveInvoice(object invoiceDetail)
        {
            InvoiceDetailBO item = (InvoiceDetailBO)invoiceDetail;
            InvoiceDAO invoiceDAO = new InvoiceDAO();
            invoiceDAO.SaveInvoiceDetail(item);
        }

        /// <summary>
        /// Tạo  mới hóa đơn API
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        /// 
        public long CreateInvoiceAPI(InvoiceBO invoice)
        {
            InvoiceDAO invoiceDAO = new InvoiceDAO();
            long invoiceId = 0;
            try
            {
                //Tính toán thêm 1 số trường trong invoice_detail(tax, discount, totalmoney ) và invoice(totaldiscount, totaltax)
                switch (invoice.USINGINVOICETYPE)
                {
                    case (int)EnumHelper.AccountObjectType.HOADONGTGT:
                    case (int)EnumHelper.AccountObjectType.PHIEUXUATKHO:
                    case (int)EnumHelper.AccountObjectType.HOADONTIENDIEN:
                        invoice.TAXMONEY = 0;
                        invoice.DISCOUNTMONEY = 0;
                        invoice.TOTALMONEY = 0;
                        invoice.TOTALPAYMENT = 0;
                        invoice.OTHERTAXFEE = 0;
                        invoice.REFUNDFEE = 0;
                        foreach (var item in invoice.LISTPRODUCT)
                        {
                            CalculateInvoiceDetailData(item, invoice);
                            invoice.TAXMONEY += item.TOTALTAX;
                            invoice.DISCOUNTMONEY += item.TOTALDISCOUNT;
                            invoice.TOTALMONEY += item.TOTALMONEY - item.TOTALDISCOUNT;
                            invoice.OTHERTAXFEE += item.OTHERTAXFEE;
                            invoice.REFUNDFEE += item.REFUNDFEE;
                            invoice.TOTALPAYMENT += item.TOTALPAYMENT;
                        }
                        break;
                    case (int)EnumHelper.AccountObjectType.HOADONBANHANG:
                        invoice.TAXMONEY = 0;
                        invoice.DISCOUNTMONEY = 0;
                        invoice.TOTALMONEY = 0;
                        invoice.TOTALPAYMENT = 0;
                        foreach (var item in invoice.LISTPRODUCT)
                        {
                            CalculateInvoiceDetailData(item, invoice);
                            invoice.DISCOUNTMONEY += item.TOTALDISCOUNT;
                            invoice.TOTALMONEY += item.TOTALMONEY - item.TOTALDISCOUNT;
                            invoice.TOTALPAYMENT += item.TOTALMONEY;
                        }
                        break;
                }
                invoiceId = invoiceDAO.AddInvoiceAPI(invoice);

                if (invoice.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTIENDIEN)
                {
                    if (invoiceId > 0)
                    {
                        invoice.LISTPRODUCT = invoice.LISTPRODUCT != null ? invoice.LISTPRODUCT.Where(x => !string.IsNullOrEmpty(x.PRODUCTNAME)).ToList() : new List<InvoiceDetailBO>();
                        invoice.LISTPRODUCT.ForEach(x => x.INVOICEID = invoiceId);

                        foreach (var item in invoice.LISTPRODUCT)
                        {
                            invoiceDAO.SaveInvoiceDetail(item);
                        }
                    }

                }
                else
                {
                    if (invoiceId > 0)
                    {
                        invoice.LISTPRODUCT = invoice.LISTPRODUCT != null ? invoice.LISTPRODUCT.Where(x => !string.IsNullOrEmpty(x.PRODUCTNAME)).ToList() : new List<InvoiceDetailBO>();
                        invoice.LISTPRODUCT.ForEach(x => x.INVOICEID = invoiceId);

                        foreach (var item in invoice.LISTPRODUCT)
                        {
                            invoiceDAO.SaveInvoiceDetail(item);

                        }

                    }

                }
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog(objEx.ToString(), objEx, MethodBase.GetCurrentMethod().Name, "AddInvoiceNoTrans BLL");
                //throw objEx;
            }
            return invoiceId;
        }

        //public long CreateInvoiceAPI(InvoiceBO invoice)
        //{
        //    long invoiceId = 0;
        //    try
        //    {
        //        InvoiceDAO invoiceDAO = new InvoiceDAO();
        //        CustomerBLL customerBLL = new CustomerBLL();

        //        //Tính toán thêm 1 số trường trong invoice_detail(tax, discount, totalmoney ) và invoice(totaldiscount, totaltax)
        //        invoice.TAXMONEY = 0;
        //        invoice.DISCOUNTMONEY = 0;
        //        invoice.TOTALMONEY = 0;
        //        invoice.TOTALPAYMENT = 0;
        //        foreach (var item in invoice.LISTPRODUCT)
        //        {
        //            CalculateInvoiceDetailData(item, invoice);
        //            invoice.TAXMONEY += item.TOTALTAX;
        //            invoice.DISCOUNTMONEY += item.TOTALDISCOUNT;
        //            invoice.TOTALMONEY += item.TOTALMONEY - item.TOTALDISCOUNT;
        //            invoice.TOTALPAYMENT += item.TOTALPAYMENT;
        //        }

        //        invoiceId = invoiceDAO.AddInvoiceAPI(invoice);
        //        if (invoiceId > 0)
        //        {
        //            invoice.LISTPRODUCT = invoice.LISTPRODUCT != null ? invoice.LISTPRODUCT.Where(x => !string.IsNullOrEmpty(x.PRODUCTNAME)).ToList() : new List<InvoiceDetailBO>();

        //            foreach (var item in invoice.LISTPRODUCT)
        //            {
        //                item.INVOICEID = invoiceId;
        //                invoiceDAO.SaveInvoiceDetail(item);
        //            }
        //            return invoiceId;
        //        }
        //        else
        //        {
        //            return -1;
        //        }
        //    }
        //    catch (Exception objEx)
        //    {
        //        this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật hóa đơn");
        //        objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
        //        return -1;
        //    }
        //}

        public long UpdateInvoice(InvoiceBO invoice)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                invoice.LISTPRODUCT = invoice.LISTPRODUCT.Where(x => x.PRODUCTNAME != null).ToList();
                // check list products if null set all money fields to 0 to avoid get TotalPayment from InvoiceBO passed from client still have totalpayment value lead to logic error - update totalpayment to invoice table
                if (invoice.LISTPRODUCT.Count == 0)
                {
                    invoice.TOTALPAYMENT = 0;
                    invoice.TOTALMONEY = 0;
                    invoice.TAXMONEY = 0;
                }
                //
                invoice.ID = invoiceDAO.UpdateInvoice(invoice);
                foreach (var item in invoice.LISTPRODUCT)
                {
                    item.INVOICEID = invoice.ID;
                    CalculateInvoiceDetailData(item, invoice);
                    invoiceDAO.SaveInvoiceDetail(item);
                }

                return invoice.ID;
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return -1;
            }
        }

        public long UpdateInvoiceAPI(InvoiceBO invoice)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                invoice.LISTPRODUCT = invoice.LISTPRODUCT.Where(x => x.PRODUCTNAME != null).ToList();
                if (invoice.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONGTGT
                    || invoice.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.PHIEUXUATKHO
                    || invoice.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTIENDIEN)
                {
                    invoice.TAXMONEY = 0;
                    invoice.DISCOUNTMONEY = 0;
                    invoice.TOTALMONEY = 0;
                    invoice.TOTALPAYMENT = 0;
                    foreach (var item in invoice.LISTPRODUCT)
                    {
                        CalculateInvoiceDetailData(item, invoice);
                        invoice.TAXMONEY += item.TOTALTAX;
                        invoice.DISCOUNTMONEY += item.TOTALDISCOUNT;
                        invoice.TOTALMONEY += item.TOTALMONEY - item.TOTALDISCOUNT;
                        invoice.TOTALPAYMENT += item.TOTALPAYMENT;
                    }
                }
                var invoiceID = invoiceDAO.UpdateInvoiceAPI(invoice);
                foreach (var item in invoice.LISTPRODUCT)
                {
                    item.INVOICEID = invoice.ID;
                    invoiceDAO.SaveInvoiceDetail(item);
                }
                return invoiceID;
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật hóa đơn API");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return -1;
            }
        }

        public bool UpdateInvoiceStatus(InvoiceBO invoice)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.UpdateInvoiceStatus(invoice);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật trạng thái");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        public List<InvoiceBO> GetforGDT(string taxnumber, string companyname, DateTime fromDate, DateTime toDate, int? itemPerPage, int? page)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.GetforGDT(taxnumber, companyname, fromDate, toDate, itemPerPage, page);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy hóa đơn trong báo cáo cơ quan thuế");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }
        public InvoiceBO GetInvoiceByChecksum(string checksumCode)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.GetInvoiceByChecksum(checksumCode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy hóa đơn từ mã checksum");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new InvoiceBO();
            }
        }

        public List<InvoiceBO> CheckDateOfPreviousInvoice(InvoiceBO form, long number)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.CheckDateOfPreviousInvoice(form, number);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy hóa đơn từ 1 số hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<InvoiceBO>();
            }
        }

        /// <summary>
        /// Cập nhật biên bản hóa đơn
        /// truongnv 20200220
        /// </summary>
        /// <param name="invoice">đối tượng hóa đơn</param>
        /// <param name="reportType">Loại hóa đơn điều chỉnh hay hủy bỏ</param>
        /// <returns></returns>
        public bool UpdateReportInvoice(long invoiceId, string reason, int reportType)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                var result = invoiceDAO.UpdateReportInvoice(invoiceId, reason, reportType);
                return true;
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        /// <summary>
        /// Lấy dữ liệu bảng danh mục
        /// </summary>
        /// <param name="propertyVal">Giá trị</param>
        /// <param name="storedName">Tên stored</param>
        /// <returns></returns>
        public object GetDataCategories(int value, string storedName)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.GetDataCategories(value, storedName);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return string.Empty;
            }
        }

        public bool UpdateCancelInvoice(InvoiceBO invoice)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                var result = invoiceDAO.UpdateCancelInvoice(invoice);
                return true;
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        public List<InvoiceBO> SearchByInvoiceCode(string iNVOICECODE, string cOMTAXCODE)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.SearchByInvoiceCode(iNVOICECODE, cOMTAXCODE);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public bool UpdateModifiedInvoice(InvoiceBO invoice)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                var result = invoiceDAO.UpdateModifiedInvoice(invoice);
                return true;
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        public long UpdateConvertInvoice(long invoice_id, string fullname, int invoiceType, string signlink)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.UpdateConvertInvoice(invoice_id, fullname, invoiceType, signlink);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return -1;
            }
        }

        public long UpdateReplaceInvoice(InvoiceBO invoice)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                invoice.ID = invoiceDAO.UpdateReplaceInvoice(invoice);
                return invoice.ID;
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return -1;
            }
        }

        public List<InvoiceBO> GetInvoiceByDate(string comTaxcode, DateTime fromDate, DateTime toDate)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.GetInvoiceByDate(comTaxcode, fromDate, toDate);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<InvoiceBO>();
            }
        }

        public List<InvoiceBO> GetInvoiceAll(string comTaxcode, DateTime fromDate, DateTime toDate, int usingInvoiceType)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.GetInvoiceAll(comTaxcode, fromDate, toDate, usingInvoiceType);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<InvoiceBO>();
            }
        }

        /// <summary>
        /// truongnv 070302020
        /// Đếm số lượng hóa đơn đã phát hành theo năm
        /// </summary>
        /// <param name="comTaxcode"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public string GetInvoiceCountSigned(string comTaxcode, DateTime fromDate, DateTime toDate,int usinginvocietype)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.GetInvoiceCountSigned(comTaxcode, fromDate, toDate,usinginvocietype);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return string.Empty;
            }
        }

        public bool ImportInvoice(List<InvoiceBO> invoices)
        {
            try
            {

                var groupInvoice = invoices.GroupBy(x => x.INVOICEID).Select(y => y.ToList()).ToList();

                InvoiceBLL invoiceBLL = new InvoiceBLL();
                foreach (var invoice in groupInvoice)
                {
                    InvoiceBO xxx = invoice[0];
                    xxx.INVOICETYPE = 1;
                    xxx.INVOICESTATUS = 1;
                    xxx.PAYMENTSTATUS = 2;
                    xxx.CURRENCY = "VND";
                    xxx.EXCHANGERATE = 1;
                    List<InvoiceDetailBO> yyy = new List<InvoiceDetailBO>();
                    foreach (var invoiceDetail in invoice)
                    {
                        yyy.Add(invoiceDetail.LISTPRODUCT[0]);
                    }
                    xxx.LISTPRODUCT = yyy;
                    var result = invoiceBLL.AddInvoice(xxx);
                }

                //objIData.CommitTransaction();
                return true;
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        /// <summary>
        /// Lấy danh sách hóa đơn
        /// </summary>
        /// <returns></returns>
        public List<InvoiceBO> GetInvoice(FormSearchInvoice form)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.GetInvoice(form);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<InvoiceBO>();
            }
        }

        //lấy ra hóa đơn đã phát hành
        public List<InvoiceBO> GetInvoiceSigned(FormSearchInvoice form)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.GetInvoiceSigned(form);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<InvoiceBO>();
            }
        }

        public List<InvoiceBO> GetInvoiceAPI(FormSearchInvoice form)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.GetInvoiceAPI(form);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách hóa đơn API");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<InvoiceBO>();
            }
        }

        /// <summary>
        /// Lấy danh sách hóa đơn phát hành lỗi
        /// </summary>
        /// <returns></returns>
        public List<InvoiceBO> GetInvoiceReleaseError(FormSearchInvoice form)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.GetInvoiceReleaseError(form);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<InvoiceBO>();
            }
        }

        public List<InvoiceBO> GetInvoiceByStatus(FormSearchInvoice form)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.GetInvoiceByStatus(form);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<InvoiceBO>();
            }
        }

        public List<InvoiceBO> GetListInvoiceByCustomer(FormSearchInvoice form)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.GetListInvoiceByCustomer(form);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<InvoiceBO>();
            }
        }

        /// <summary>
        /// Lấy hóa đơn theo id
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public InvoiceBO GetInvoiceById(long invoiceid, int usingInvoiceType = -1)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.GetInvoiceById(invoiceid, usingInvoiceType);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new InvoiceBO();
            }
        }

        public List<InvoiceBO> GetMultiInvoice(string lstid, string comtaxcode)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.GetMultiInvoice(lstid, comtaxcode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi ds lấy hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public InvoiceBO GetInvoiceById(long invoiceid, HttpContext context, int usingInvoiceType = -1)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.GetInvoiceById(invoiceid, context, usingInvoiceType);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new InvoiceBO();
            }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceid"></param>
        /// <param name="signLink"></param>
        /// <returns></returns>
        public bool UpdateSignLink(long invoiceid, string signLink)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.UpdateSignLink(invoiceid, signLink);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật sign link hóa đơn, long invoiceid, string signLink, string referenceCode");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        public bool UpdateInvoiceStatusSignedLink(long invoiceid, string signedLink)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.UpdateInvoiceStatusSignedLink(invoiceid, signedLink);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật trạng thái và link hóa đơn đã ký");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        public bool UpdateInvoiceCheckSumXml(long invoiceid, string checkSumXml)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.UpdateInvoiceCheckSumXml(invoiceid, checkSumXml);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật trạng thái và checksumxml hóa đơn đã ký");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        /// <summary>
        /// Cập nhật hóa đơn khi thực hiện ký phát hành hóa đơn
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="checkSumXml"></param>
        /// <param name="signLink"></param>
        /// <param name="tempNextNumber"></param>
        /// <param name="dtSignTime"></param>
        /// <returns></returns>
        public long UpdateDataInvoice(long invoiceId, string checkSumXml, string signLink, long tempNextNumber, DateTime dtSignTime, string source = null)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.UpdateDataInvoice(invoiceId, checkSumXml, signLink, tempNextNumber, dtSignTime, source);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật trạng thái và checksumxml hóa đơn đã ký");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return -1;
            }
        }

        public bool UpdateInvoiceCanceledLink(long invoiceId, string canceledLink)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.UpdateInvoiceCanceledLink(invoiceId, canceledLink);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật link hủy hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        public long UpdateInvoiceNumber(long invoiceId, long nextNumber, DateTime dtSignTime, string checkSum, int invoiceStatus)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.UpdateInvoiceNumber(invoiceId, nextNumber, dtSignTime, checkSum, invoiceStatus);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật số hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return -1;
            }
        }

        public long UpdateInvoiceWaittingNumber(long invoiceid, long numberid, DateTime signedTime)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.UpdateInvoiceWaittingNumber(invoiceid, numberid, signedTime);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật số hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return -1;
            }
        }

        public bool UpdateAttachFileLink(InvoiceBO invoice)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.UpdateAttachFileLink(invoice);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật đường dẫn File đính kèm");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        public string DeleteInvoice(string invoiceids)
        {
            string msg = string.Empty;
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                msg = invoiceDAO.DeleteInvoice(invoiceids);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật sign link hóa đơn, long invoiceid, string signLink, string referenceCode");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return objEx.Message;
            }
            return msg;
        }

        //xóa hẳn HĐ
        public string DeletedInvoice(string ids)
        {
            string msg = string.Empty;
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                msg = invoiceDAO.DeletedInvoice(ids);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật sign link hóa đơn, long invoiceid, string signLink, string referenceCode");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return objEx.Message;
            }
            return msg;
        }

        //xóa hẳn tất cả HĐ
        public List<InvoiceBO> DeletedAllInvoice(string comTaxCode, int usingInvoiceType)
        {
            string msg = string.Empty;
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.DeletedAllInvoice(comTaxCode, usingInvoiceType);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi xóa tất cả hóa đơn.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<InvoiceBO>();
            }
        }

        public string UpdatePaymentStatus(string invoiceids, int paymentStatus)
        {
            string msg = string.Empty;
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                msg = invoiceDAO.UpdatePaymentStatus(invoiceids, paymentStatus);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật trạng thái thanh toán.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return objEx.Message;
            }
            return msg;
        }

        public string UpdatePartner(string invoiceids, string email)
        {
            string msg = string.Empty;
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                msg = invoiceDAO.UpdatePartner(invoiceids, email);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật phân quyền hóa đơn cho cộng tác viên.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return objEx.Message;
            }
            return msg;
        }

        /// <summary>
        /// Lấy chi tiết hóa đơn
        /// </summary>
        /// <returns></returns>
        public List<InvoiceDetailBO> GetInvoiceDetail(long invoiceid)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.GetInvoiceDetail(invoiceid);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy chi tiết hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<InvoiceDetailBO>();
            }
        }

        public InvoiceBO GetInvoiceByReferenceCode(string referencecode)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.GetInvoiceByReferenceCode(referencecode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy hóa đơn từ mã tra cứu");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new InvoiceBO();
            }
        }

        /// <summary>
        /// Lấy hóa đơn theo mã tra cứu có kèm thông tin templatepath
        /// </summary>
        /// <param name="referencecode"></param>
        /// <returns></returns>
        public InvoiceBO GetInvoiceByCode(string referencecode)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.GetInvoiceByCode(referencecode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy hóa đơn từ mã tra cứu");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new InvoiceBO();
            }
        }

        #region Tạo file XML

        /// <summary>
        /// Tạo file XML khi ký hóa đơn thông thường.
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="lstProducts"></param>
        /// <param name="nextNumber"></param>
        /// <param name="invoiceName"></param>
        /// <returns></returns>
        public string CreateFileInvoiceXML(InvoiceBO invoice, List<InvoiceDetailBO> lstProducts, long nextNumber, string invoiceName = "Hóa đơn giá trị gia tăng (Tiền nước)", string signTime = null)
        {
            string filePath = string.Empty;
            try
            {

                DateTime dtSignTime = DateTime.ParseExact(signTime == null ? DateTime.Now.ToString("dd/MM/yyyy") : signTime, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("inv", "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1");
                ns.Add("ds", "http://www.w3.org/2000/09/xmldsig#");
                invoice invoiceInstance = new invoice();

                // Nội dung XML cho Phiếu xuất kho
                if (invoice.USINGINVOICETYPE == (int)AccountObjectType.PHIEUXUATKHO)
                {
                    List<invoiceInvoiceDataItemsItem> lstItem = new List<invoiceInvoiceDataItemsItem>();
                    uint index = 1;
                    decimal totalMoneyWithVAT = 0;
                    decimal totalMoneyWithoutVAT = 0;
                    decimal totalDiscount = 0;
                    decimal totalVatAmount = 0;
                    foreach (var item in lstProducts)
                    {
                        if (item.ISPROMOTION)
                        {
                            item.RETAILPRICE = 0;
                        }
                        decimal amountMoneyWithoutVAT = item.RETAILPRICE * item.QUANTITY;
                        decimal tempAmountMoneyWithoutVAT = amountMoneyWithoutVAT;
                        decimal discountAmount = 0;
                        if (item.DISCOUNTRATE != 0)
                        {
                            discountAmount = amountMoneyWithoutVAT * (((decimal)item.DISCOUNTRATE) / 100);
                            amountMoneyWithoutVAT = amountMoneyWithoutVAT - discountAmount;
                        }
                        lstItem.Add(new invoiceInvoiceDataItemsItem()
                        {
                            lineNumber = index,
                            itemCode = item.SKU,
                            itemName = item.PRODUCTNAME,
                            unitName = item.QUANTITYUNIT,
                            quantity = item.QUANTITY,
                            itemTotalAmountWithoutVat = Math.Round(tempAmountMoneyWithoutVAT, MidpointRounding.AwayFromZero),
                            discountPercentage = item.DISCOUNTRATE,
                            discountAmount = Math.Round(discountAmount, MidpointRounding.AwayFromZero),
                            vatPercentage = item.TAXRATE == -1 ? 0 : item.TAXRATE,
                            vatAmount = Math.Round(amountMoneyWithoutVAT * ((item.TAXRATE == -1 ? 0 : (decimal)item.TAXRATE) / 100), MidpointRounding.AwayFromZero),
                            unitPrice = Math.Round(item.RETAILPRICE, MidpointRounding.AwayFromZero),
                            promotion = item.ISPROMOTION
                        });

                        totalMoneyWithoutVAT += tempAmountMoneyWithoutVAT;
                        totalDiscount += discountAmount;
                        totalVatAmount += (amountMoneyWithoutVAT * ((decimal)(item.TAXRATE == -1 ? 0 : item.TAXRATE) / 100));
                        index++;
                    }
                    totalMoneyWithVAT = (totalMoneyWithoutVAT + totalVatAmount) - totalDiscount;
                    //invoice invoiceInstance = new invoice();
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

                        deliveryOrderBy = invoice.CUSNAME,
                        deliveryOrderDate = invoice.DELIVERYORDERDATE,
                        deliveryBy = invoice.CUSBUYER,
                        fromWarehouseName = invoice.FROMWAREHOUSENAME,
                        transportationMethod = invoice.TRANSPORTATIONMETHOD,
                        toWarehouseName = invoice.TOWAREHOUSENAME,
                        contractNumber = invoice.CONTRACTNUMBER,
                        deliveryOrderContent = invoice.DELIVERYORDERCONTENT,
                        exchangeRate = invoice.EXCHANGERATE,
                        items = new invoiceInvoiceDataItems()
                        {
                            item = lstItem
                        },
                        invoiceTaxBreakdowns = new invoiceInvoiceDataInvoiceTaxBreakdowns()
                        {
                            invoiceTaxBreakdown = new invoiceInvoiceDataInvoiceTaxBreakdownsInvoiceTaxBreakdown()
                            {
                                vatPercentage = 0
                            }
                        },

                        totalAmountWithoutVAT = Math.Round(totalMoneyWithoutVAT, MidpointRounding.AwayFromZero),
                        totalVATAmount = Math.Round(totalVatAmount, MidpointRounding.AwayFromZero),
                        totalAmountWithVAT = invoice.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTIENNUOC ? Math.Round(invoice.TOTALPAYMENT, MidpointRounding.AwayFromZero) : Math.Round(totalMoneyWithVAT, MidpointRounding.AwayFromZero),

                        invoiceNotes = invoice.NOTE,
                        totalAmountWithVATInWords = ReadNumberToCurrencyWords.ConvertToWordsWithPostfix(invoice.USINGINVOICETYPE == 0 ? totalMoneyWithVAT : Math.Round(invoice.TOTALPAYMENT, MidpointRounding.AwayFromZero), invoice.CURRENCY),
                        discountAmount = Math.Round(totalDiscount, MidpointRounding.AwayFromZero),
                        isDiscountAmtPos = 0,
                        totalAmountWithVATFrn = invoice.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTIENNUOC ? Math.Round(invoice.TOTALPAYMENT, MidpointRounding.AwayFromZero) : Math.Round(totalMoneyWithVAT, MidpointRounding.AwayFromZero),
                        userDefines = $"ONFINANCE_Data_id:{invoice.ID}-checksum:{invoice.CHECKSUMXML}-referencecode:{invoice.REFERENCECODE}"
                    };
                    invoiceControlData invControlData = new invoiceControlData()
                    {
                        systemCode = "NOVAON_ONFINANCE"
                    };

                    invoiceInstance.invoiceData = invData;
                    invoiceInstance.controlData = invControlData;
                }
                // Nội dung XML cho Hóa đơn bán hàng
                else if (invoice.USINGINVOICETYPE == (int)AccountObjectType.HOADONBANHANG)
                {
                    List<invoiceInvoiceDataItemsItem> lstItem = new List<invoiceInvoiceDataItemsItem>();
                    uint index = 1;
                    foreach (var item in lstProducts)
                    {
                        if (item.ISPROMOTION)
                        {
                            item.RETAILPRICE = 0;
                        }
                        lstItem.Add(new invoiceInvoiceDataItemsItem()
                        {
                            lineNumber = index,
                            itemCode = item.SKU,
                            itemName = item.PRODUCTNAME,
                            unitName = item.QUANTITYUNIT,
                            quantity = item.QUANTITY,
                            itemTotalAmountWithoutVat = Math.Round(item.TOTALMONEY, MidpointRounding.AwayFromZero),
                            discountPercentage = item.DISCOUNTRATE,
                            discountAmount = Math.Round(item.TOTALDISCOUNT, MidpointRounding.AwayFromZero),
                            vatPercentage = 0,
                            vatAmount = 0,
                            unitPrice = Math.Round(item.RETAILPRICE, MidpointRounding.AwayFromZero),
                            promotion = item.ISPROMOTION
                        });
                        index++;
                    }

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
                        sellerFaxNumber = "",
                        sellerEmail = invoice.CUSEMAIL,
                        sellerBankAccount = "",
                        sellerBankName = "",

                        buyerLegalName = invoice.CUSNAME,
                        buyerTaxCode = invoice.CUSTAXCODE,
                        buyerAddressLine = invoice.CUSADDRESS,
                        deliveryOrderDate = dtSignTime,
                        exchangeRate = invoice.EXCHANGERATE,
                        items = new invoiceInvoiceDataItems()
                        {
                            item = lstItem
                        },
                        invoiceTaxBreakdowns = new invoiceInvoiceDataInvoiceTaxBreakdowns()
                        {
                            invoiceTaxBreakdown = new invoiceInvoiceDataInvoiceTaxBreakdownsInvoiceTaxBreakdown()
                            {
                                vatPercentage = 0
                            }
                        },

                        totalAmountWithoutVAT = Math.Round(invoice.TOTALMONEY, MidpointRounding.AwayFromZero),
                        totalAmountWithVAT = Math.Round(invoice.TOTALPAYMENT, MidpointRounding.AwayFromZero),
                        totalAmountWithVATInWords = ReadNumberToCurrencyWords.ConvertToWordsWithPostfix(Math.Round(invoice.TOTALPAYMENT, MidpointRounding.AwayFromZero), invoice.CURRENCY),
                        discountAmount = Math.Round(invoice.DISCOUNTMONEY, MidpointRounding.AwayFromZero),
                        isDiscountAmtPos = 0,
                        totalAmountWithVATFrn = Math.Round(invoice.TOTALPAYMENT, MidpointRounding.AwayFromZero),
                        userDefines = $"ONFINANCE_Data_id:{invoice.ID}-checksum:{invoice.CHECKSUMXML}-referencecode:{invoice.REFERENCECODE}"
                    };
                    invoiceControlData invControlData = new invoiceControlData()
                    {
                        systemCode = "NOVAON_ONFINANCE"
                    };

                    invoiceInstance.invoiceData = invData;
                    invoiceInstance.controlData = invControlData;
                }
                // Nội dung XML cho hóa đơn trường học
                else if (invoice.USINGINVOICETYPE == (int)AccountObjectType.HOADONTRUONGHOC)
                {
                    List<invoiceInvoiceDataItemsItem> lstItem = new List<invoiceInvoiceDataItemsItem>();
                    uint index = 1;
                    foreach (var item in lstProducts)
                    {
                        if (item.ISPROMOTION)
                        {
                            item.RETAILPRICE = 0;
                        }
                        lstItem.Add(new invoiceInvoiceDataItemsItem()
                        {
                            lineNumber = index,
                            itemCode = item.SKU,
                            itemName = item.PRODUCTNAME,
                            unitName = item.QUANTITYUNIT,
                            quantity = item.QUANTITY,
                            itemTotalAmountWithoutVat = Math.Round(item.TOTALMONEY, MidpointRounding.AwayFromZero),
                            discountPercentage = item.DISCOUNTRATE,
                            discountAmount = Math.Round(item.TOTALDISCOUNT, MidpointRounding.AwayFromZero),
                            vatPercentage = item.TAXRATE == -1 ? 0 : item.TAXRATE,
                            vatAmount = Math.Round(item.TOTALTAX, MidpointRounding.AwayFromZero),
                            unitPrice = Math.Round(item.RETAILPRICE, MidpointRounding.AwayFromZero),
                            promotion = item.ISPROMOTION
                        });
                        index++;
                    }

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
                        items = new invoiceInvoiceDataItems()
                        {
                            item = lstItem
                        },
                        invoiceTaxBreakdowns = new invoiceInvoiceDataInvoiceTaxBreakdowns()
                        {
                            invoiceTaxBreakdown = new invoiceInvoiceDataInvoiceTaxBreakdownsInvoiceTaxBreakdown()
                            {
                                vatPercentage = 0
                            }
                        },

                        totalAmountWithoutVAT = Math.Round(invoice.TOTALMONEY, MidpointRounding.AwayFromZero),
                        totalVATAmount = Math.Round(invoice.TAXMONEY, MidpointRounding.AwayFromZero),
                        totalAmountWithVAT = Math.Round(invoice.TOTALPAYMENT, MidpointRounding.AwayFromZero),

                        invoiceNotes = invoice.NOTE,
                        totalAmountWithVATInWords = ReadNumberToCurrencyWords.ConvertToWordsWithPostfix(Math.Round(invoice.TOTALPAYMENT, MidpointRounding.AwayFromZero), invoice.CURRENCY),
                        discountAmount = Math.Round(invoice.DISCOUNTMONEY, MidpointRounding.AwayFromZero),
                        isDiscountAmtPos = 0,
                        totalAmountWithVATFrn = Math.Round(invoice.TOTALPAYMENT, MidpointRounding.AwayFromZero),
                        userDefines = $"ONFINANCE_Data_id:{invoice.ID}-checksum:{invoice.CHECKSUMXML}-referencecode:{invoice.REFERENCECODE}"
                    };
                    invoiceControlData invControlData = new invoiceControlData()
                    {
                        systemCode = "NOVAON_ONFINANCE"
                    };

                    invoiceInstance.invoiceData = invData;
                    invoiceInstance.controlData = invControlData;
                }
                // Nội dung XML cho các loại hóa đơn khác
                else
                {
                    var isQuantityPriceIsZero = false;
                    foreach (var item in lstProducts)
                    {
                        if (item.QUANTITY == 0 || item.RETAILPRICE == 0)
                            isQuantityPriceIsZero = true;
                    }

                    if (!isQuantityPriceIsZero)
                    {
                        List<invoiceInvoiceDataItemsItem> lstItem = new List<invoiceInvoiceDataItemsItem>();
                        uint index = 1;
                        decimal totalMoneyWithVAT = 0;
                        decimal totalMoneyWithoutVAT = 0;
                        decimal totalDiscount = 0;
                        decimal totalVatAmount = 0;
                        decimal totalEnviromentProtectTaxAmount = 0;
                        foreach (var item in lstProducts)
                        {
                            if (item.ISPROMOTION)
                            {
                                item.RETAILPRICE = 0;
                            }
                            decimal amountMoneyWithoutVAT = item.RETAILPRICE * item.QUANTITY;
                            decimal tempAmountMoneyWithoutVAT = amountMoneyWithoutVAT;
                            decimal discountAmount = 0;
                            if (item.DISCOUNTRATE != 0)
                            {
                                discountAmount = amountMoneyWithoutVAT * (((decimal)item.DISCOUNTRATE) / 100);
                                amountMoneyWithoutVAT = amountMoneyWithoutVAT - discountAmount;
                            }
                            lstItem.Add(new invoiceInvoiceDataItemsItem()
                            {
                                lineNumber = index,
                                itemCode = item.SKU,
                                itemName = item.PRODUCTNAME,
                                unitName = item.QUANTITYUNIT,
                                quantity = Math.Round(item.QUANTITY, invoice.QUANTITYPLACE, MidpointRounding.AwayFromZero),
                                itemTotalAmountWithoutVat = Math.Round(tempAmountMoneyWithoutVAT, invoice.MONEYPLACE, MidpointRounding.AwayFromZero),
                                discountPercentage = item.DISCOUNTRATE,
                                discountAmount = Math.Round(discountAmount, invoice.MONEYPLACE, MidpointRounding.AwayFromZero),
                                vatPercentage = item.TAXRATE == -1 ? 0 : item.TAXRATE,
                                vatAmount = Math.Round(amountMoneyWithoutVAT * ((item.TAXRATE == -1 ? 0 : (decimal)item.TAXRATE) / 100), invoice.MONEYPLACE, MidpointRounding.AwayFromZero),
                                unitPrice = Math.Round(item.RETAILPRICE, invoice.QUANTITYPLACE, MidpointRounding.AwayFromZero),
                                promotion = item.ISPROMOTION
                            });

                            totalMoneyWithoutVAT += tempAmountMoneyWithoutVAT;
                            totalDiscount += discountAmount;
                            totalVatAmount += (amountMoneyWithoutVAT * ((decimal)(item.TAXRATE == -1 ? 0 : item.TAXRATE) / 100));
                            totalEnviromentProtectTaxAmount += (amountMoneyWithoutVAT * ((decimal)(item.TAXRATEWATER == -1 ? 0 : item.TAXRATEWATER) / 100));
                            index++;
                        }
                        totalMoneyWithVAT = (totalMoneyWithoutVAT + totalVatAmount + invoice.OTHERTAXFEE - invoice.REFUNDFEE + invoice.TOTALSERVICEFEE) - totalDiscount;
                        //invoice invoiceInstance = new invoice();
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
                            items = new invoiceInvoiceDataItems()
                            {
                                item = lstItem
                            },
                            invoiceTaxBreakdowns = new invoiceInvoiceDataInvoiceTaxBreakdowns()
                            {
                                invoiceTaxBreakdown = new invoiceInvoiceDataInvoiceTaxBreakdownsInvoiceTaxBreakdown()
                                {
                                    vatPercentage = 0
                                }
                            },

                            totalAmountWithoutVAT = Math.Round(totalMoneyWithoutVAT, invoice.MONEYPLACE, MidpointRounding.AwayFromZero),
                            totalVATAmount = Math.Round(totalVatAmount, invoice.MONEYPLACE, MidpointRounding.AwayFromZero),
                            totalAmountWithVAT = invoice.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTIENNUOC ? Math.Round(invoice.TOTALPAYMENT, MidpointRounding.AwayFromZero) : Math.Round(totalMoneyWithVAT, invoice.MONEYPLACE, MidpointRounding.AwayFromZero),
                            totalEnvironmentProtectTax = Math.Round(totalEnviromentProtectTaxAmount, invoice.MONEYPLACE, MidpointRounding.AwayFromZero),
                            totalOtherTaxesCharges = Math.Round(invoice.OTHERTAXFEE, invoice.MONEYPLACE, MidpointRounding.AwayFromZero),
                            totalRefundFee = Math.Round(invoice.REFUNDFEE, invoice.MONEYPLACE, MidpointRounding.AwayFromZero),
                            totalServiceFee = Math.Round(invoice.TOTALSERVICEFEE, invoice.MONEYPLACE, MidpointRounding.AwayFromZero),
                            invoiceNotes = invoice.NOTE,
                            totalAmountWithVATInWords = ReadNumberToCurrencyWords.ConvertToWordsWithPostfixNumberPlace(invoice.USINGINVOICETYPE == 0 ? totalMoneyWithVAT : Math.Round(invoice.TOTALPAYMENT, MidpointRounding.AwayFromZero), invoice.CURRENCY, invoice.MONEYPLACE),
                            discountAmount = Math.Round(totalDiscount, invoice.MONEYPLACE, MidpointRounding.AwayFromZero),
                            isDiscountAmtPos = 0,
                            totalAmountWithVATFrn = invoice.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTIENNUOC ? Math.Round(invoice.TOTALPAYMENT, MidpointRounding.AwayFromZero) : Math.Round(totalMoneyWithVAT, invoice.MONEYPLACE, MidpointRounding.AwayFromZero),
                            userDefines = $"ONFINANCE_Data_id:{invoice.ID}-checksum:{invoice.CHECKSUMXML}-referencecode:{invoice.REFERENCECODE}"
                        };
                        invoiceControlData invControlData = new invoiceControlData()
                        {
                            systemCode = "NOVAON_ONFINANCE"
                        };

                        invoiceInstance.invoiceData = invData;
                        invoiceInstance.controlData = invControlData;
                    }
                    else
                    {
                        List<invoiceInvoiceDataItemsItem> lstItem = new List<invoiceInvoiceDataItemsItem>();
                        uint index = 1;
                        foreach (var item in lstProducts)
                        {
                            if (item.ISPROMOTION)
                            {
                                item.RETAILPRICE = 0;
                            }
                            lstItem.Add(new invoiceInvoiceDataItemsItem()
                            {
                                lineNumber = index,
                                itemCode = item.SKU,
                                itemName = item.PRODUCTNAME,
                                unitName = item.QUANTITYUNIT,
                                quantity = Math.Round(item.QUANTITY, invoice.QUANTITYPLACE, MidpointRounding.AwayFromZero),
                                itemTotalAmountWithoutVat = Math.Round(item.TOTALMONEY, invoice.MONEYPLACE, MidpointRounding.AwayFromZero),
                                discountPercentage = item.DISCOUNTRATE,
                                discountAmount = Math.Round(item.TOTALDISCOUNT, invoice.MONEYPLACE, MidpointRounding.AwayFromZero),
                                vatPercentage = item.TAXRATE == -1 ? 0 : item.TAXRATE,
                                vatAmount = Math.Round(item.TOTALTAX, invoice.MONEYPLACE, MidpointRounding.AwayFromZero),
                                unitPrice = Math.Round(item.RETAILPRICE, invoice.PRICEPLACE, MidpointRounding.AwayFromZero),
                                promotion = item.ISPROMOTION
                            });
                            index++;
                        }

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
                            items = new invoiceInvoiceDataItems()
                            {
                                item = lstItem
                            },
                            invoiceTaxBreakdowns = new invoiceInvoiceDataInvoiceTaxBreakdowns()
                            {
                                invoiceTaxBreakdown = new invoiceInvoiceDataInvoiceTaxBreakdownsInvoiceTaxBreakdown()
                                {
                                    vatPercentage = 0
                                }
                            },

                            totalAmountWithoutVAT = Math.Round(invoice.TOTALMONEY, invoice.MONEYPLACE, MidpointRounding.AwayFromZero),
                            totalVATAmount = Math.Round(invoice.TAXMONEY, invoice.MONEYPLACE, MidpointRounding.AwayFromZero),
                            totalAmountWithVAT = Math.Round(invoice.TOTALPAYMENT, invoice.MONEYPLACE, MidpointRounding.AwayFromZero),
                            totalOtherTaxesCharges = Math.Round(invoice.OTHERTAXFEE, invoice.MONEYPLACE, MidpointRounding.AwayFromZero),
                            totalRefundFee = Math.Round(invoice.REFUNDFEE, invoice.MONEYPLACE, MidpointRounding.AwayFromZero),
                            totalServiceFee = Math.Round(invoice.TOTALSERVICEFEE, invoice.MONEYPLACE, MidpointRounding.AwayFromZero),
                            invoiceNotes = invoice.NOTE,
                            //totalAmountWithVATInWords = ReadNumberToCurrencyWords.ConvertToWordsWithPostfix(Math.Round(invoice.TOTALPAYMENT, decimalPlaceInvoice, MidpointRounding.AwayFromZero), invoice.CURRENCY),
                            totalAmountWithVATInWords = ReadNumberToCurrencyWords.ConvertToWordsWithPostfixNumberPlace(Math.Round(invoice.TOTALPAYMENT, invoice.MONEYPLACE, MidpointRounding.AwayFromZero), invoice.CURRENCY, invoice.MONEYPLACE),
                            discountAmount = Math.Round(invoice.DISCOUNTMONEY, invoice.MONEYPLACE, MidpointRounding.AwayFromZero),
                            isDiscountAmtPos = 0,
                            totalAmountWithVATFrn = Math.Round(invoice.TOTALPAYMENT, invoice.MONEYPLACE, MidpointRounding.AwayFromZero),
                            userDefines = $"ONFINANCE_Data_id:{invoice.ID}-checksum:{invoice.CHECKSUMXML}-referencecode:{invoice.REFERENCECODE}"
                        };
                        invoiceControlData invControlData = new invoiceControlData()
                        {
                            systemCode = "NOVAON_ONFINANCE"
                        };

                        invoiceInstance.invoiceData = invData;
                        invoiceInstance.controlData = invControlData;
                    }
                }

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

                filePath = fileName.Replace(HttpContext.Current.Server.MapPath("~/" + root), "").Replace('\\', '/');
                return filePath;
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi tạo hóa đơn xml", ex, MethodBase.GetCurrentMethod().Name, "CreateInvoiceXML");
                return string.Empty;
            }
        }

        /// <summary>
        /// Tạo file xml khi ký hóa đơn qua API hỗ trợ bởi ký HSM
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="lstProducts"></param>
        /// <param name="nextNumber"></param>
        /// <param name="invoiceFolder"></param>
        /// <param name="invoiceName"></param>
        /// <returns></returns>
        public string CreateFileInvoiceXMLAPI(InvoiceBO invoice, List<InvoiceDetailBO> lstProducts, long nextNumber, string invoiceFolder, string invoiceName = "Hóa đơn giá trị gia tăng (Tiền nước)")
        {
            string filePath = string.Empty;
            try
            {
                DateTime dtSignTime = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("inv", "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1");
                ns.Add("ds", "http://www.w3.org/2000/09/xmldsig#");


                List<invoiceInvoiceDataItemsItem> lstItem = new List<invoiceInvoiceDataItemsItem>();
                uint index = 1;
                foreach (var item in lstProducts)
                {
                    if (item.ISPROMOTION)
                    {
                        item.RETAILPRICE = 0;
                    }
                    lstItem.Add(new invoiceInvoiceDataItemsItem()
                    {
                        lineNumber = index,
                        itemCode = item.SKU,
                        itemName = item.PRODUCTNAME,
                        unitName = item.QUANTITYUNIT,
                        quantity = item.QUANTITY,
                        itemTotalAmountWithoutVat = Math.Round(item.TOTALMONEY, MidpointRounding.AwayFromZero),
                        discountPercentage = item.DISCOUNTRATE,
                        discountAmount = Math.Round(item.TOTALDISCOUNT, MidpointRounding.AwayFromZero),
                        vatPercentage = item.TAXRATE == -1 ? 0 : item.TAXRATE,
                        vatAmount = Math.Round(item.TOTALTAX, MidpointRounding.AwayFromZero),
                        unitPrice = Math.Round(item.RETAILPRICE, MidpointRounding.AwayFromZero),
                        promotion = item.ISPROMOTION
                    });

                    //totalMoneyWithoutVAT += tempAmountMoneyWithoutVAT;
                    //totalDiscount += discountAmount;
                    //totalVatAmount += (amountMoneyWithoutVAT * ((decimal)(item.TAXRATE == -1 ? 0 : item.TAXRATE) / 100));
                    index++;
                }
                //totalMoneyWithVAT = (totalMoneyWithoutVAT + totalVatAmount) - totalDiscount;

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
                    items = new invoiceInvoiceDataItems()
                    {
                        item = lstItem
                    },
                    invoiceTaxBreakdowns = new invoiceInvoiceDataInvoiceTaxBreakdowns()
                    {
                        invoiceTaxBreakdown = new invoiceInvoiceDataInvoiceTaxBreakdownsInvoiceTaxBreakdown()
                        {
                            vatPercentage = 0
                        }
                    },

                    totalAmountWithoutVAT = Math.Round(invoice.TOTALMONEY, MidpointRounding.AwayFromZero),
                    totalVATAmount = Math.Round(invoice.TAXMONEY, MidpointRounding.AwayFromZero),
                    totalAmountWithVAT = Math.Round(invoice.TOTALPAYMENT, MidpointRounding.AwayFromZero),

                    invoiceNotes = invoice.NOTE,
                    totalAmountWithVATInWords = ReadNumberToCurrencyWords.ConvertToWordsWithPostfix(invoice.USINGINVOICETYPE == 0 ? Math.Round(invoice.TOTALPAYMENT, MidpointRounding.AwayFromZero) : Math.Round(invoice.TOTALPAYMENT, MidpointRounding.AwayFromZero), invoice.CURRENCY),
                    discountAmount = Math.Round(invoice.DISCOUNTMONEY, MidpointRounding.AwayFromZero),
                    isDiscountAmtPos = 0,
                    totalAmountWithVATFrn = Math.Round(invoice.TOTALPAYMENT, MidpointRounding.AwayFromZero),
                    userDefines = $"ONFINANCE_Data_id:{invoice.ID}-checksum:{invoice.CHECKSUMXML}-referencecode:{invoice.REFERENCECODE}"
                };
                invoiceControlData invControlData = new invoiceControlData()
                {
                    systemCode = "NOVAON_ONFINANCE"
                };

                invoiceInstance.invoiceData = invData;
                invoiceInstance.controlData = invControlData;

                string root = invoiceFolder; // physical folder in local or virtual path on cloud server by getting the appconfig 
                string branchComTaxCode = "/" + (string.IsNullOrEmpty(invoice.COMTAXCODE) ? "COMTAXCODE/" : invoice.COMTAXCODE + "/");
                string branchInvoiceID = string.IsNullOrEmpty(invoice.ID.ToString()) ? "INVOICEID/" : invoice.ID.ToString() + "/";
                string branchInvoiceType = string.IsNullOrEmpty(invoice.INVOICETYPENAME.ToString()) ? "INVOICETYPENAME/" : ElaHelper.FormatKeywordSearch(ElaHelper.FilterVietkey(invoice.INVOICETYPENAME.ToString())).Replace(" ", "-");

                string branchDate = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();

                string path = string.Format($"{root}{branchComTaxCode}{branchInvoiceID}{branchInvoiceType}");
                // checking path is exist if not create the folder to download file 
                DirectoryInfo dir = new DirectoryInfo(path);
                if (!dir.Exists)
                {
                    dir.Create();
                }

                // write all the data from memory stream to fileName is created in the folder just created above
                string fileName = dir + "/" + invoice.COMTAXCODE + "_" + invoice.ID + "_" + invoice.INVOICETYPE + ".xml";
                XMLHelper.SerializationXmlWithPrefix(invoiceInstance, fileName, ns);

                filePath = fileName.Replace(root, "").Replace('\\', '/');
                return filePath;
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi tạo hóa đơn xml", ex, MethodBase.GetCurrentMethod().Name, "CreateInvoiceXML");
                return string.Empty;
            }
        }

        #endregion

        public string GenerateInvoiceByTemplate(byte[] dataBuffer, InvoiceBO invoice)
        {
            var filePathDatabase = string.Empty;
            try
            {
                string fileName = invoice.COMTAXCODE + "_" + invoice.ID + "_" + invoice.INVOICETYPE + ".pdf";
                filePathDatabase = SaveInvoiceFile(dataBuffer, invoice, fileName);
                //Update file's link into database (signlink)
                if (!string.IsNullOrEmpty(filePathDatabase))
                {
                    bool iSuccess = UpdateInvoiceSignLink(invoice.ID, filePathDatabase, invoice.REFERENCECODE);
                    if (!iSuccess)
                        filePathDatabase = string.Empty;
                    return filePathDatabase;
                }
                else
                {
                    ConfigHelper.Instance.WriteLog("Lỗi tạo hóa đơn file PDF.", "", MethodBase.GetCurrentMethod().Name, "GenerateInvoiceByTemplate");
                    return null;
                }
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi tạo hóa đơn.", ex, MethodBase.GetCurrentMethod().Name, "GenerateInvoiceByTemplate");
                return null;
            }
        }

        /// <summary>
        /// Tạo và save file mẫu hóa đơn vào thư mục (.pdf)
        /// truongnv 20200226
        /// </summary>
        /// <param name="dataBuffer">mảng file</param>
        /// <param name="invoice">đối tượng hóa đơn</param>
        /// <param name="msg">Thông báo</param>
        /// <returns></returns>
        public string GenerateTemplatePdf(byte[] dataBuffer, InvoiceBO invoice, ref string msg)
        {
            var filePath = string.Empty;
            try
            {
                string fileName = invoice.COMTAXCODE + "_" + invoice.ID + "_" + invoice.INVOICETYPE + ".pdf";
                filePath = SaveInvoiceFile(dataBuffer, invoice, fileName);
                //Update file's link into database (signlink)
                if (string.IsNullOrEmpty(filePath))
                {
                    msg = "Lỗi không lưu được file mẫu hóa đơn vào folder.";
                }
            }
            catch (Exception ex)
            {
                msg = "Lỗi không tạo được file mẫu hóa đơn vào folder.";
                ConfigHelper.Instance.WriteLog("Lỗi tạo hóa đơn.", ex, MethodBase.GetCurrentMethod().Name, "GenerateInvoiceByTemplate");
            }
            return filePath;
        }

        public string SaveInvoiceFile(byte[] dataBuffer, InvoiceBO invoice, string fileName)
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

        public bool SaveInvoiceTemplate(InvoiceNumberBO objNumberBO)
        {
            try
            {
                string path = ConfigurationManager.AppSettings["InputInvoiceFolder"]; // physical folder in local or virtual path on cloud server by getting the appconfig 
                path = string.Format($"/{path}/{objNumberBO.COMTAXCODE}"); ///TemplateInvoice.html

                // checking path is exist if not create the folder to download file 
                DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath("~" + path));
                if (!dir.Exists)
                {
                    dir.Create();
                }

                ////Nếu có thay đổi css
                //if (!string.IsNullOrEmpty(objNumberBO.CSS))
                //{
                //    //Đường dẫn file css
                //    string cssPath = "/style_" + objNumberBO.FORMCODE.Replace("/", "") + "_" + objNumberBO.SYMBOLCODE.Replace("/", "") + ".css";
                //    //Tạo file css
                //    File.WriteAllText(dir + cssPath, objNumberBO.CSS, System.Text.Encoding.UTF8);


                //    //Lấy vị trí thẻ <style>
                //    var startIndex = objNumberBO.TEMPLATEHTML.IndexOf("<style>");
                //    //Chèn file css mà user đã tạo
                //    objNumberBO.TEMPLATEHTML = objNumberBO.TEMPLATEHTML.Insert(startIndex, "<link href=\"" + objNumberBO.CSSPath + path + cssPath + "\" rel=\"stylesheet\" type=\"text/css\" />\r\n");
                //}

                // write all the data from memory stream to fileName is created in the folder just created above
                string fileName = "/" + "TemplateInvoice_" + objNumberBO.FORMCODE.Replace("/", "") + "_" + objNumberBO.SYMBOLCODE.Replace("/", "") + ".cshtml";
                File.WriteAllText(dir + fileName, objNumberBO.TEMPLATEHTML, System.Text.Encoding.UTF8);

                //Lưu đường dẫn vào database để sử dụng
                objNumberBO.TEMPLATEPATH = path + fileName;
                DataObject.Number.NumberDAO numberDAO = new DataObject.Number.NumberDAO();
                return numberDAO.AddReleaseNotice(objNumberBO);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Không thể lưu mẫu hóa đơn, vui lòng kiểm tra lại mẫu và ký hiệu hóa đơn.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        #endregion
        /// <summary>
        /// Tạo file pdf khi lưu biên bản hóa đơn
        /// </summary>
        /// <param name="report"></param>
        /// <param name="isSigned"></param>
        /// <returns></returns>
        public string GenerateReportToPdf(ReportBO report, bool isSigned)
        {
            string filePath = string.Empty;
            string invoiceTypeName = "/hoa-don-xoa-bo/";
            string extensionFile = "_canceled_report.pdf";
            string errorMessage = "Lỗi tạo biên bản hủy hóa đơn.";
            try
            {
                if (report.REPORTTYPE == "1")
                {
                    filePath = HttpContext.Current.Server.MapPath("~/TemplateFiles/CanceledReport.html");
                }
                else
                {
                    extensionFile = "_modified_report.pdf";
                    invoiceTypeName = "/hoa-don-dieu-chinh/";
                    errorMessage = "Lỗi tạo biên bản điểu chỉnh hóa đơn.";
                    filePath = HttpContext.Current.Server.MapPath("~/TemplateFiles/ModifiedReport.html");
                }

                var invoice = GetInvoiceById(report.INVOICEID);
                if (invoice.INVOICETYPE == 5)
                {
                    invoice = GetInvoiceById(invoice.REFERENCE);
                }
                string html = string.Empty;
                using (StreamReader reader = new StreamReader(filePath))
                {
                    html = reader.ReadToEnd();
                }

                html = html.Replace("{Date}", report.REPORTTIME.Day.ToString("D2"));
                html = html.Replace("{Month}", report.REPORTTIME.Month.ToString("D2"));
                html = html.Replace("{Year}", report.REPORTTIME.Year.ToString());
                html = html.Replace("{CURRENTDATE}", invoice.SIGNEDTIME.ToString("dd/MM/yyyy"));

                html = html.Replace("{COMNAME}", report.COMNAME);
                html = html.Replace("{COMADDRESS}", report.COMADDRESS);
                html = html.Replace("{COMTAXCODE}", report.COMTAXCODE);
                html = html.Replace("{COMDELEGATE}", report.COMLEGALNAME);
                html = html.Replace("{COMPHONE}", report.COMPHONENUMBER);
                html = html.Replace("{COMROLE}", report.COMROLE);

                html = html.Replace("{CUSNAME}", report.CUSNAME);
                html = html.Replace("{CUSADDRESS}", report.CUSADDRESS);
                html = html.Replace("{CUSPHONE}", report.CUSPHONENUMBER);
                html = html.Replace("{CUSDELEGATE}", report.CUSDELEGATE);
                html = html.Replace("{CUSTAXCODE}", report.CUSTAXCODE);
                html = html.Replace("{CUSROLE}", report.CUSROLE);

                html = html.Replace("{INVOICENUMBER}", invoice.NUMBER.ToString("D7"));
                html = html.Replace("{INVOICEFORMCODE}", invoice.FORMCODE);
                html = html.Replace("{INVOICESYMBOLCODE}", invoice.SYMBOLCODE);
                html = html.Replace("{REASON}", report.REASON);

                //TH người dùng chọn lưu và ký thì gắn chân chữ ký vào hệ thống
                if (isSigned)
                {
                    string signature = "<div style='font-weight: bold;border: 3px solid red;color: red;text-align: left;padding: 12px;width: 300px;margin: auto;margin-top: 20px; background: url(https://e.onfinance.asia/Images/check-mark-icon-png-11.png) no-repeat center center; background-size: 29%'>"
                    + "Signature Valid<br>"
                    + "Ký bởi: " + report.COMNAME + "<br>"
                    + "Ký ngày: " + report.REPORTTIME.ToString("dd/MM/yyyy")
                    + "</div>";
                    html = html.Replace("{SIGNATURE}", signature);
                }
                else
                {
                    html = html.Replace("{SIGNATURE}", string.Empty);
                }

                string root = ConfigurationManager.AppSettings["InputInvoiceFolder"];
                string reportLink = "/" + invoice.COMTAXCODE + "/" + invoice.ID + invoiceTypeName;
                DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/" + root + reportLink));
                if (!dir.Exists)
                {
                    dir.Create();
                }
                var fileName = HttpContext.Current.Server.MapPath("~/" + root + reportLink + invoice.COMTAXCODE + "_" + invoice.ID + extensionFile);

                List<string> content = new List<string>();
                content.Add(html);
                var dataBuffer = BtnCreatePdf(content, null);
                using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    if (dataBuffer.Length > 0)
                    {
                        fileStream.Write(dataBuffer, 0, dataBuffer.Length);
                    }
                }
                return reportLink + invoice.COMTAXCODE + "_" + invoice.ID + extensionFile;
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(errorMessage, ex, MethodBase.GetCurrentMethod().Name, "CreateCanceledReport");
                return null;
            }
        }

        public byte[] BtnCreatePdf(string htmlString, string baseUrl, string pageSizeText = "A4")
        {
            PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize), pageSizeText, true);
            PdfPageOrientation pdfOrientation = (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation), pageSize == PdfPageSize.A4 ? "Portrait" : "Landscape", true);

            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();
            converter.Options.MaxPageLoadTime = 120;
            int webPageWidth = 1024;
            if (pageSize == PdfPageSize.A4)
            {
                try
                {
                    webPageWidth = Convert.ToInt32(1024);
                }
                catch { }

                int webPageHeight = 768;
                try
                {
                    webPageHeight = Convert.ToInt32(768);
                }
                catch { }

                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
            }

            // set converter options
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;

            // save pdf document
            SelectPdf.PdfDocument doc = converter.ConvertHtmlString(htmlString, baseUrl);

            byte[] res = null;
            res = doc.Save();

            // close pdf document
            doc.Close();

            return res;
        }

        Func<object, PdfDocument> CreatePdfPage = (object obj) =>
        {
            try
            {
                PDFContentPage pdf = (PDFContentPage)obj;
                PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize), pdf.PageSize, true);
                PdfPageOrientation pdfOrientation = (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation), pageSize == PdfPageSize.A4 ? "Portrait" : "Landscape", true);

                // instantiate a html to pdf converter object
                HtmlToPdf converter = new HtmlToPdf();
                //converter.Options.MaxPageLoadTime = 120;
                int webPageWidth = 1024;
                int webPageHeight = 1446;
                if (pageSize == PdfPageSize.A4)
                {
                    converter.Options.WebPageWidth = webPageWidth;
                    converter.Options.WebPageHeight = webPageHeight;
                }

                // set converter options
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;

                // create a new pdf document converting an url
                PdfDocument doc = converter.ConvertHtmlString(pdf.HtmlContent, pdf.BaseUrl);

                return doc;
            }
            catch (Exception ex)
            {
                return null;
            }
        };

        public byte[] BtnCreatePdf(List<string> htmlString, string baseUrl, string pageSizeText = "A4")
        {
            byte[] res = null;
            try
            {
                var tasks = new List<Task<PdfDocument>>();
                for (int i = 0; i < htmlString.Count; i++)
                {
                    var pdfPageContent = new PDFContentPage
                    {
                        BaseUrl = null,
                        HtmlContent = htmlString[i],
                        PageSize = pageSizeText
                    };
                    var task = Task<PdfDocument>.Factory.StartNew(CreatePdfPage, pdfPageContent);
                    tasks.Add(task);

                    //doc.Append(docItem);
                }

                // Wait for all the tasks to finish.
                Task.WaitAll(tasks.ToArray());
                //Get result from task
                PdfDocument doc = new PdfDocument();
                foreach (var t in tasks)
                {
                    if(t.Result!=null)
                        doc.Append(t.Result);
                }

                res = doc.Save();
                doc.Close();

                // close pdf document
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog($"Lỗi tạo file Pdf => {ex.ToString()}", ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "BtnCreatePdf");
            }
            return res;
        }

        public byte[] BtnCreatePdfMultiplePage(List<string> htmlString, string baseUrl)
        {
            byte[] res = null;
            try
            {
                PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize), "A4", true);
                PdfPageOrientation pdfOrientation = (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation), "Portrait", true);

                int webPageWidth = 1024;
                try
                {
                    webPageWidth = Convert.ToInt32(1024);
                }
                catch { }

                int webPageHeight = 768;
                try
                {
                    webPageHeight = Convert.ToInt32(768);
                }
                catch { }

                // instantiate a html to pdf converter object
                HtmlToPdf converter = new HtmlToPdf();

                // set converter options
                converter.Options.PdfPageSize = PdfPageSize.A4; ;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
               

                // create a new pdf document converting an url
                SelectPdf.PdfDocument doc = converter.ConvertHtmlString(htmlString[0], baseUrl);
                if (htmlString.Count > 1)
                {
                    for (int i = 1; i < htmlString.Count; i++)
                    {
                        SelectPdf.PdfDocument docItem = converter.ConvertHtmlString(htmlString[i], baseUrl);
                        doc.Append(docItem);
                    }
                }

                // save pdf document
                //doc.Save("Sample.pdf");

                res = doc.Save();

                // close pdf document
                doc.Close();
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog($"Lỗi tạo file Pdf => {ex.ToString()}", ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "BtnCreatePdfMultiplePage");
            }
            return res;
        }

        public List<InvoiceBO> GetAllInvoice(InvoiceSearchFormBO invoiceSearch)
        {
            try
            {
                InvoiceDAO objCompanyDAO = new InvoiceDAO();
                return objCompanyDAO.GetAllInvoive(invoiceSearch);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public List<InvoiceBO> ExportInvoiceExcelBySignDate(FormSearchInvoice form)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.ExportInvoiceExcelBySignDate(form);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách hóa đơn theo ngày ký.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<InvoiceBO>();
            }
        }

        public List<InvoiceBO> ExportInvoiceExcelByInitDate(FormSearchInvoice form)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.ExportInvoiceExcelByInitDate(form);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách hóa đơn theo ngày ký.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<InvoiceBO>();
            }
        }

        public List<InvoiceBO> ExportInvoiceExcelByIds(string ids)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.ExportInvoiceExcelByIds(ids);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách hóa đơn theo ngày ký.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<InvoiceBO>();
            }
        }

        public List<InvoiceOutput> GetListIDInvoce(string comtaxcode)
        {
            try
            {
                InvoiceDAO objCompanyDAO = new InvoiceDAO();
                return objCompanyDAO.GetListIDInvoce(comtaxcode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public List<InvoiceBO> GetSearchCustomerID(string cusPhoneNumber, string customerCode, int page, int offset, DateTime fromDate, DateTime toDate, string comTaxCode)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.GetSearchCustomerID(cusPhoneNumber, customerCode, page, offset, fromDate, toDate, comTaxCode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách công tơ");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<InvoiceBO>();
            }
        }

        public List<InvoiceBO> GetInvoiceDelete(FormSearchInvoice form)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.GetInvoiceDelete(form);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<InvoiceBO>();
            }
        }

        public List<InvoiceBO> GetInvoiceWating(FormSearchInvoice form)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.GetInvoiceWating(form);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<InvoiceBO>();
            }
        }

        public string RecoverInvoice(string invoiceids)
        {
            string msg = string.Empty;
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                msg = invoiceDAO.RecoverInvoice(invoiceids);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật sign link hóa đơn, long invoiceid, string signLink, string referenceCode");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return objEx.Message;
            }
            return msg;
        }

        // Admin: báo cáo tình hình sử dụng hóa đơn
        public List<InvoiceSummary> Getforstatistics(InvoiceSearchFormBO invoiceSearch)
        {
            try
            {
                InvoiceDAO objCompanyDAO = new InvoiceDAO();
                return objCompanyDAO.Getforstatistics(invoiceSearch);

            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public List<InvoiceSummary> GetStatisticTopTen(InvoiceSearchFormBO invoiceSearch)
        {
            try
            {
                InvoiceDAO objCompanyDAO = new InvoiceDAO();
                return objCompanyDAO.GetStatisticTopTen(invoiceSearch);

            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }


        public int GetInvoiceCount(string comname, int usingtype, int usingtypeinvoice)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return Convert.ToInt32(invoiceDAO.GetInvoiceCount(comname, usingtype, usingtypeinvoice));
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return 0;
            }
        }

        // Admin: lấy ra số khách hàng theo trạng thái
        public int GetCompanyCount(string status, string keyword)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return Convert.ToInt32(invoiceDAO.GetCompanyCount(status, keyword));
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return 0;
            }
        }

        /// <summary>
        /// Lấy hóa đơn theo id cho project API
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public InvoiceBO GetInvoiceByIdAPI(long invoiceid)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.GetInvoiceByIdAPI(invoiceid);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new InvoiceBO();
            }
        }

        /// <summary>
        /// Lấy ra hóa đơn có số hóa đơn lớn nhất trong thông báo phát hành
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public InvoiceBO GetMaxNumberInvoice(FormSearchInvoice form)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.GetMaxNumberInvoice(form);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy hóa đơn có số hóa đơn lớn nhất");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new InvoiceBO();
            }
        }

        /// <summary>
        /// Lấy ra hóa đơn có số hóa đơn cụ thể
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public InvoiceBO GetInvoiceByNumber(FormSearchInvoice form)
        {
            try
            {
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                return invoiceDAO.GetInvoiceByNumber(form);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy hóa đơn có số hóa đơn lớn nhất");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new InvoiceBO();
            }
        }
    }
}
