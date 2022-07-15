using DS.BusinessObject.Invoice;
using DS.BusinessObject.Output;
using DS.Common.Helpers;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace DS.DataObject.Invoice
{
    public class InvoiceDAO : BaseDAO
    {
        #region Constructor
        public InvoiceDAO() : base()
        {
        }

        public InvoiceDAO(IData objIData)
            : base(objIData)
        {
        }

        #endregion

        #region Methods

        //Tạo mới hóa đơn
        public Int64 AddInvoice(InvoiceBO Invoice)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_add");
                objIData.AddParameter("p_comname", Invoice.COMNAME);
                objIData.AddParameter("p_comtaxcode", Invoice.COMTAXCODE);
                objIData.AddParameter("p_comaddress", Invoice.COMADDRESS);
                objIData.AddParameter("p_formcode", Invoice.FORMCODE);
                objIData.AddParameter("p_symbolcode", Invoice.SYMBOLCODE);
                objIData.AddParameter("p_number", Invoice.NUMBER);
                objIData.AddParameter("p_cusname", Invoice.CUSNAME);
                objIData.AddParameter("p_cusaddress", Invoice.CUSADDRESS);
                objIData.AddParameter("p_cusbuyer", Invoice.CUSBUYER);
                objIData.AddParameter("p_cusemail", Invoice.CUSEMAIL);
                objIData.AddParameter("p_cusphonenumber", Invoice.CUSPHONENUMBER);
                objIData.AddParameter("p_custaxcode", Invoice.CUSTAXCODE);
                objIData.AddParameter("p_cuspaymentmethod", Invoice.CUSPAYMENTMETHOD);
                objIData.AddParameter("p_cusaccountnumber", Invoice.CUSACCOUNTNUMBER);
                objIData.AddParameter("p_cusbankname", Invoice.CUSBANKNAME);
                objIData.AddParameter("p_termid", Invoice.TERMID);
                objIData.AddParameter("p_duedate", Invoice.DUEDATE);
                objIData.AddParameter("p_discounttype", Invoice.DISCOUNTTYPE);
                objIData.AddParameter("p_totalmoney", Invoice.TOTALMONEY);
                objIData.AddParameter("p_taxmoney", Invoice.TAXMONEY);
                objIData.AddParameter("p_discountmoney", Invoice.DISCOUNTMONEY);
                objIData.AddParameter("p_totalpayment", Invoice.TOTALPAYMENT);
                objIData.AddParameter("p_invoicestatus", Invoice.INVOICESTATUS);
                objIData.AddParameter("p_paymentstatus", Invoice.PAYMENTSTATUS);
                objIData.AddParameter("p_invoicetype", Invoice.INVOICETYPE);
                objIData.AddParameter("p_reference", Invoice.REFERENCE);
                objIData.AddParameter("p_isinvoicewaiting", Invoice.ISINVOICEWAITING);
                objIData.AddParameter("p_invoicewaitingtime", Invoice.INVOICEWAITINGTIME);
                objIData.AddParameter("p_changereason", Invoice.CHANGEREASON);
                objIData.AddParameter("p_note", Invoice.NOTE);
                objIData.AddParameter("p_currency", Invoice.CURRENCY);
                objIData.AddParameter("p_exchangerate", Invoice.EXCHANGERATE);
                objIData.AddParameter("p_referencecode", Invoice.REFERENCECODE);
                objIData.AddParameter("p_signlink", Invoice.SIGNLINK);
                objIData.AddParameter("p_invoicemethod", Invoice.INVOICEMETHOD);
                objIData.AddParameter("p_fromdate", Invoice.FROMDATE);
                objIData.AddParameter("p_todate", Invoice.TODATE);
                objIData.AddParameter("p_apartmentno", Invoice.APARTMENTNO);
                objIData.AddParameter("p_customercode", Invoice.CUSTOMERCODE);
                objIData.AddParameter("p_sortorder", Invoice.SORTORDER);
                objIData.AddParameter("p_customfieldexchangerate", Invoice.CUSTOMFIELDEXCHANGERATE);
                objIData.AddParameter("p_customfieldexchange", Invoice.CUSTOMFIELDEXCHANGE);
                objIData.AddParameter("p_usinginvoicetype", Invoice.USINGINVOICETYPE);

                objIData.AddParameter("p_createdby", Invoice.CREATEDBY);
                objIData.AddParameter("p_modifiedby", Invoice.MODIFIEDBY);
                objIData.AddParameter("p_modifieddate", Invoice.MODIFIEDDATE);
                objIData.AddParameter("p_ipaddress", Invoice.IPADDRESS);

                //Phiếu xuất kho
                objIData.AddParameter("p_deliveryordernumber", Invoice.DELIVERYORDERNUMBER);
                objIData.AddParameter("p_deliveryordercontent", Invoice.DELIVERYORDERCONTENT);
                objIData.AddParameter("p_transportationmethod", Invoice.TRANSPORTATIONMETHOD);
                objIData.AddParameter("p_fromwarehousename", Invoice.FROMWAREHOUSENAME);
                objIData.AddParameter("p_towarehousename", Invoice.TOWAREHOUSENAME);
                objIData.AddParameter("p_deliveryorderdate", Invoice.DELIVERYORDERDATE);
                objIData.AddParameter("p_contractnumber", Invoice.CONTRACTNUMBER);

                // Số lượng số thập phân lấy sau dấy phẩy
                objIData.AddParameter("p_quantityplace", Invoice.QUANTITYPLACE);
                objIData.AddParameter("p_priceplace", Invoice.PRICEPLACE);
                objIData.AddParameter("p_moneyplace", Invoice.MONEYPLACE);

                // Phân quyền cộng tác viên theo email CTV
                objIData.AddParameter("p_partneremail", Invoice.PARTNEREMAIL);

                // Thêm thuế, phí khác
                objIData.AddParameter("p_othertaxfee", Invoice.OTHERTAXFEE);
                // Phí hoàn, phí dịch vụ
                objIData.AddParameter("p_refundfee", Invoice.REFUNDFEE);
                objIData.AddParameter("p_servicefee", Invoice.SERVICEFEE);
                objIData.AddParameter("p_servicefeetaxrate", Invoice.SERVICEFEETAXRATE);
                objIData.AddParameter("p_servicefeetax", Invoice.SERVICEFEETAX);
                objIData.AddParameter("p_totalservicefee", Invoice.TOTALSERVICEFEE);
                // Ký theo thứ tự import excel
                objIData.AddParameter("p_signindex", Invoice.SIGNINDEX);

                var id = objIData.ExecStoreToString();
                CommitTransactionIfAny(objIData);
                return Convert.ToInt64(id);
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }
        public bool DeleteInvoices(List<long> invoiceIds)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                foreach (var invId in invoiceIds)
                {
                    objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_deleted");
                    objIData.AddParameter("p_id", invId);
                    var id = objIData.ExecNonQuery();
                    CommitTransactionIfAny(objIData);
                }
                return true;
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog(objEx.ToString(), objEx, MethodBase.GetCurrentMethod().Name, "DeleteInvoices DAO");
                RollBackTransactionIfAny(objIData);
                //throw objEx;
                return false;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }
        public Int64 AddInvoiceNoTrans(InvoiceBO Invoice)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_add");
                objIData.AddParameter("p_comname", Invoice.COMNAME);
                objIData.AddParameter("p_comtaxcode", Invoice.COMTAXCODE);
                objIData.AddParameter("p_comaddress", Invoice.COMADDRESS);
                objIData.AddParameter("p_formcode", Invoice.FORMCODE);
                objIData.AddParameter("p_symbolcode", Invoice.SYMBOLCODE);
                objIData.AddParameter("p_number", Invoice.NUMBER);
                objIData.AddParameter("p_cusname", Invoice.CUSNAME);
                objIData.AddParameter("p_cusaddress", Invoice.CUSADDRESS);
                objIData.AddParameter("p_cusbuyer", Invoice.CUSBUYER);
                objIData.AddParameter("p_cusemail", Invoice.CUSEMAIL);
                objIData.AddParameter("p_cusphonenumber", Invoice.CUSPHONENUMBER);
                objIData.AddParameter("p_custaxcode", Invoice.CUSTAXCODE);
                objIData.AddParameter("p_cuspaymentmethod", Invoice.CUSPAYMENTMETHOD);
                objIData.AddParameter("p_cusaccountnumber", Invoice.CUSACCOUNTNUMBER);
                objIData.AddParameter("p_cusbankname", Invoice.CUSBANKNAME);
                objIData.AddParameter("p_termid", Invoice.TERMID);
                objIData.AddParameter("p_duedate", Invoice.DUEDATE);
                objIData.AddParameter("p_discounttype", Invoice.DISCOUNTTYPE);
                objIData.AddParameter("p_totalmoney", Invoice.TOTALMONEY);
                objIData.AddParameter("p_taxmoney", Invoice.TAXMONEY);
                objIData.AddParameter("p_discountmoney", Invoice.DISCOUNTMONEY);
                objIData.AddParameter("p_totalpayment", Invoice.TOTALPAYMENT);
                objIData.AddParameter("p_invoicestatus", Invoice.INVOICESTATUS);
                objIData.AddParameter("p_paymentstatus", Invoice.PAYMENTSTATUS);
                objIData.AddParameter("p_invoicetype", Invoice.INVOICETYPE);
                objIData.AddParameter("p_reference", Invoice.REFERENCE);
                objIData.AddParameter("p_isinvoicewaiting", Invoice.ISINVOICEWAITING);
                objIData.AddParameter("p_invoicewaitingtime", Invoice.INVOICEWAITINGTIME);
                objIData.AddParameter("p_changereason", Invoice.CHANGEREASON);
                objIData.AddParameter("p_note", Invoice.NOTE);
                objIData.AddParameter("p_currency", Invoice.CURRENCY);
                objIData.AddParameter("p_exchangerate", Invoice.EXCHANGERATE);
                objIData.AddParameter("p_referencecode", Invoice.REFERENCECODE);
                objIData.AddParameter("p_signlink", Invoice.SIGNLINK);
                objIData.AddParameter("p_invoicemethod", Invoice.INVOICEMETHOD);
                objIData.AddParameter("p_fromdate", Invoice.FROMDATE);
                objIData.AddParameter("p_todate", Invoice.TODATE);
                objIData.AddParameter("p_apartmentno", Invoice.APARTMENTNO);
                objIData.AddParameter("p_customercode", Invoice.CUSTOMERCODE);
                objIData.AddParameter("p_sortorder", Invoice.SORTORDER);
                objIData.AddParameter("p_customfieldexchangerate", Invoice.CUSTOMFIELDEXCHANGERATE);
                objIData.AddParameter("p_customfieldexchange", Invoice.CUSTOMFIELDEXCHANGE);
                objIData.AddParameter("p_usinginvoicetype", Invoice.USINGINVOICETYPE);

                objIData.AddParameter("p_createdby", Invoice.CREATEDBY);
                objIData.AddParameter("p_modifiedby", Invoice.MODIFIEDBY);
                objIData.AddParameter("p_modifieddate", Invoice.MODIFIEDDATE);
                objIData.AddParameter("p_ipaddress", Invoice.IPADDRESS);

                //Phiếu xuất kho
                objIData.AddParameter("p_deliveryordernumber", Invoice.DELIVERYORDERNUMBER);
                objIData.AddParameter("p_deliveryordercontent", Invoice.DELIVERYORDERCONTENT);
                objIData.AddParameter("p_transportationmethod", Invoice.TRANSPORTATIONMETHOD);
                objIData.AddParameter("p_fromwarehousename", Invoice.FROMWAREHOUSENAME);
                objIData.AddParameter("p_towarehousename", Invoice.TOWAREHOUSENAME);
                objIData.AddParameter("p_deliveryorderdate", Invoice.DELIVERYORDERDATE);
                objIData.AddParameter("p_contractnumber", Invoice.CONTRACTNUMBER);

                // Số lượng số thập phân lấy sau dấy phẩy
                objIData.AddParameter("p_quantityplace", Invoice.QUANTITYPLACE);
                objIData.AddParameter("p_priceplace", Invoice.PRICEPLACE);
                objIData.AddParameter("p_moneyplace", Invoice.MONEYPLACE);

                // Phân quyền cộng tác viên theo email CTV
                objIData.AddParameter("p_partneremail", Invoice.PARTNEREMAIL);

                // Thêm thuế, phí khác
                objIData.AddParameter("p_othertaxfee", Invoice.OTHERTAXFEE);
                // Phí hoàn, phí dịch vụ
                objIData.AddParameter("p_refundfee", Invoice.REFUNDFEE);
                objIData.AddParameter("p_servicefee", Invoice.SERVICEFEE);
                objIData.AddParameter("p_servicefeetaxrate", Invoice.SERVICEFEETAXRATE);
                objIData.AddParameter("p_servicefeetax", Invoice.SERVICEFEETAX);
                objIData.AddParameter("p_totalservicefee", Invoice.TOTALSERVICEFEE);

                // Ký theo thứ tự import excel
                objIData.AddParameter("p_signindex", Invoice.SIGNINDEX);
                var id = objIData.ExecStoreToString();
                //
                int.TryParse(id.ToString(), out int invoiceId);
                string refCode = ReferenceCode.GenerateReferenceCode(invoiceId);
                objIData.ExecQueryToString($"update ds_masterdata.pm_invoice set referencecode='{refCode}' WHERE id={invoiceId}");
                //
                CommitTransactionIfAny(objIData);
                return Convert.ToInt64(id);
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog(objEx.ToString(), objEx, MethodBase.GetCurrentMethod().Name, "AddInvoiceNoTrans DAO");
                RollBackTransactionIfAny(objIData);
                //throw objEx;
                return -1;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public CompanySummary GetSumany()
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_company_sumany");
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<CompanySummary>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list.FirstOrDefault();
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        //public Int64 AddInvoice(InvoiceBO Invoice)
        //{
        //    int usinginvoiceTypeId = 0;
        //    if (System.Web.HttpContext.Current.Session["USINGINVOICETYPEID"] != null)
        //        usinginvoiceTypeId = (int)System.Web.HttpContext.Current.Session["USINGINVOICETYPEID"];

        //    IData objIData = this.CreateIData();
        //    try
        //    {
        //        BeginTransactionIfAny(objIData);
        //        objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_add");
        //        objIData.AddParameter("p_comname", Invoice.COMNAME);
        //        objIData.AddParameter("p_comtaxcode", Invoice.COMTAXCODE);
        //        objIData.AddParameter("p_comaddress", Invoice.COMADDRESS);
        //        objIData.AddParameter("p_formcode", Invoice.FORMCODE);
        //        objIData.AddParameter("p_symbolcode", Invoice.SYMBOLCODE);
        //        objIData.AddParameter("p_number", Invoice.NUMBER);
        //        objIData.AddParameter("p_cusname", Invoice.CUSNAME);
        //        objIData.AddParameter("p_cusaddress", Invoice.CUSADDRESS);
        //        objIData.AddParameter("p_cusbuyer", Invoice.CUSBUYER);
        //        objIData.AddParameter("p_cusemail", Invoice.CUSEMAIL);
        //        objIData.AddParameter("p_cusphonenumber", Invoice.CUSPHONENUMBER);
        //        objIData.AddParameter("p_custaxcode", Invoice.CUSTAXCODE);
        //        objIData.AddParameter("p_cuspaymentmethod", Invoice.CUSPAYMENTMETHOD);
        //        objIData.AddParameter("p_cusaccountnumber", Invoice.CUSACCOUNTNUMBER);
        //        objIData.AddParameter("p_cusbankname", Invoice.CUSBANKNAME);
        //        objIData.AddParameter("p_termid", Invoice.TERMID);
        //        objIData.AddParameter("p_duedate", Invoice.DUEDATE);
        //        objIData.AddParameter("p_discounttype", Invoice.DISCOUNTTYPE);
        //        objIData.AddParameter("p_totalmoney", Invoice.TOTALMONEY);
        //        objIData.AddParameter("p_taxmoney", Invoice.TAXMONEY);
        //        objIData.AddParameter("p_discountmoney", Invoice.DISCOUNTMONEY);
        //        objIData.AddParameter("p_totalpayment", Invoice.TOTALPAYMENT);
        //        objIData.AddParameter("p_invoicestatus", Invoice.INVOICESTATUS);
        //        objIData.AddParameter("p_paymentstatus", Invoice.PAYMENTSTATUS);
        //        objIData.AddParameter("p_invoicetype", Invoice.INVOICETYPE);
        //        objIData.AddParameter("p_reference", Invoice.REFERENCE);
        //        objIData.AddParameter("p_isinvoicewaiting", Invoice.ISINVOICEWAITING);
        //        objIData.AddParameter("p_invoicewaitingtime", Invoice.INVOICEWAITINGTIME);
        //        objIData.AddParameter("p_changereason", Invoice.CHANGEREASON);
        //        objIData.AddParameter("p_note", Invoice.NOTE);
        //        objIData.AddParameter("p_currency", Invoice.CURRENCY);
        //        objIData.AddParameter("p_exchangerate", Invoice.EXCHANGERATE);
        //        objIData.AddParameter("p_referencecode", Invoice.REFERENCECODE);
        //        objIData.AddParameter("p_signlink", Invoice.SIGNLINK);
        //        objIData.AddParameter("p_invoicemethod", Invoice.INVOICEMETHOD);
        //        objIData.AddParameter("p_fromdate", Invoice.FROMDATE);
        //        objIData.AddParameter("p_todate", Invoice.TODATE);
        //        objIData.AddParameter("p_apartmentno", Invoice.APARTMENTNO);
        //        objIData.AddParameter("p_customercode", Invoice.CUSTOMERCODE);
        //        objIData.AddParameter("p_sortorder", Invoice.SORTORDER);
        //        objIData.AddParameter("p_customfieldexchangerate", Invoice.CUSTOMFIELDEXCHANGERATE);
        //        objIData.AddParameter("p_customfieldexchange", Invoice.CUSTOMFIELDEXCHANGE);
        //        objIData.AddParameter("p_usinginvoicetype", usinginvoiceTypeId);

        //        objIData.AddParameter("p_createdby", Invoice.CREATEDBY);
        //        objIData.AddParameter("p_modifiedby", Invoice.MODIFIEDBY);
        //        objIData.AddParameter("p_modifieddate", Invoice.MODIFIEDDATE);
        //        objIData.AddParameter("p_ipaddress", Invoice.IPADDRESS);

        //        //Phiếu xuất kho
        //        objIData.AddParameter("p_deliveryordernumber", Invoice.DELIVERYORDERNUMBER);
        //        objIData.AddParameter("p_deliveryordercontent", Invoice.DELIVERYORDERCONTENT);
        //        objIData.AddParameter("p_transportationmethod", Invoice.TRANSPORTATIONMETHOD);
        //        objIData.AddParameter("p_fromwarehousename", Invoice.FROMWAREHOUSENAME);
        //        objIData.AddParameter("p_towarehousename", Invoice.TOWAREHOUSENAME);
        //        objIData.AddParameter("p_deliveryorderdate", Invoice.DELIVERYORDERDATE);
        //        objIData.AddParameter("p_contractnumber", Invoice.CONTRACTNUMBER);

        //        // Số lượng số thập phân lấy sau dấy phẩy
        //        objIData.AddParameter("p_quantityplace", Invoice.QUANTITYPLACE);
        //        objIData.AddParameter("p_priceplace", Invoice.PRICEPLACE);
        //        objIData.AddParameter("p_moneyplace", Invoice.MONEYPLACE);

        //        // Phân quyền cộng tác viên theo email CTV
        //        objIData.AddParameter("p_partneremail", Invoice.PARTNEREMAIL);

        //        // Thêm thuế, phí khác
        //        objIData.AddParameter("p_othertaxfee", Invoice.OTHERTAXFEE);

        //        var id = objIData.ExecStoreToString();
        //        CommitTransactionIfAny(objIData);
        //        return Convert.ToInt64(id);
        //    }
        //    catch (Exception objEx)
        //    {
        //        RollBackTransactionIfAny(objIData);
        //        throw objEx;
        //    }
        //    finally
        //    {
        //        this.DisconnectIData(objIData);
        //    }
        //}

        public Int64 AddInvoiceAPI(InvoiceBO Invoice)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_add_api");
                objIData.AddParameter("p_comname", Invoice.COMNAME);
                objIData.AddParameter("p_comtaxcode", Invoice.COMTAXCODE);
                objIData.AddParameter("p_comaddress", Invoice.COMADDRESS);
                objIData.AddParameter("p_formcode", Invoice.FORMCODE);
                objIData.AddParameter("p_symbolcode", Invoice.SYMBOLCODE);
                objIData.AddParameter("p_number", Invoice.NUMBER);
                objIData.AddParameter("p_cusname", Invoice.CUSNAME != null ? Invoice.CUSNAME.Trim() : Invoice.CUSNAME);
                objIData.AddParameter("p_cusaddress", Invoice.CUSADDRESS != null ? Invoice.CUSADDRESS.Trim() : Invoice.CUSADDRESS);
                objIData.AddParameter("p_cusbuyer", Invoice.CUSBUYER != null ? Invoice.CUSBUYER.Trim() : Invoice.CUSBUYER);
                objIData.AddParameter("p_cusemail", Invoice.CUSEMAIL != null ? Invoice.CUSEMAIL.Trim() : Invoice.CUSEMAIL);
                objIData.AddParameter("p_cusphonenumber", Invoice.CUSPHONENUMBER != null ? Invoice.CUSPHONENUMBER.Trim() : Invoice.CUSPHONENUMBER);
                objIData.AddParameter("p_custaxcode", Invoice.CUSTAXCODE != null ? Invoice.CUSTAXCODE.Trim() : Invoice.CUSTAXCODE);
                objIData.AddParameter("p_cuspaymentmethod", Invoice.CUSPAYMENTMETHOD != null ? Invoice.CUSPAYMENTMETHOD.Trim() : Invoice.CUSPAYMENTMETHOD);
                objIData.AddParameter("p_cusaccountnumber", Invoice.CUSACCOUNTNUMBER != null ? Invoice.CUSACCOUNTNUMBER.Trim() : Invoice.CUSACCOUNTNUMBER);
                objIData.AddParameter("p_cusbankname", Invoice.CUSBANKNAME != null ? Invoice.CUSBANKNAME.Trim() : Invoice.CUSBANKNAME);
                objIData.AddParameter("p_termid", Invoice.TERMID);
                objIData.AddParameter("p_duedate", Invoice.DUEDATE);
                objIData.AddParameter("p_discounttype", Invoice.DISCOUNTTYPE);
                objIData.AddParameter("p_totalmoney", Invoice.TOTALMONEY);
                objIData.AddParameter("p_taxmoney", Invoice.TAXMONEY);
                objIData.AddParameter("p_discountmoney", Invoice.DISCOUNTMONEY);
                objIData.AddParameter("p_totalpayment", Invoice.TOTALPAYMENT);
                objIData.AddParameter("p_invoicestatus", Invoice.INVOICESTATUS);
                objIData.AddParameter("p_paymentstatus", Invoice.PAYMENTSTATUS);
                objIData.AddParameter("p_invoicetype", Invoice.INVOICETYPE);
                objIData.AddParameter("p_reference", Invoice.REFERENCE);
                objIData.AddParameter("p_isinvoicewaiting", Invoice.ISINVOICEWAITING);
                objIData.AddParameter("p_invoicewaitingtime", Invoice.INVOICEWAITINGTIME);
                objIData.AddParameter("p_changereason", Invoice.CHANGEREASON);
                objIData.AddParameter("p_note", Invoice.NOTE);
                objIData.AddParameter("p_currency", Invoice.CURRENCY);
                objIData.AddParameter("p_exchangerate", Invoice.EXCHANGERATE);
                objIData.AddParameter("p_referencecode", Invoice.REFERENCECODE);
                objIData.AddParameter("p_signlink", Invoice.SIGNLINK);
                objIData.AddParameter("p_invoicemethod", Invoice.INVOICEMETHOD);
                objIData.AddParameter("p_fromdate", Invoice.FROMDATE);
                objIData.AddParameter("p_todate", Invoice.TODATE);
                objIData.AddParameter("p_apartmentno", Invoice.APARTMENTNO);
                objIData.AddParameter("p_customercode", Invoice.CUSTOMERCODE);
                objIData.AddParameter("p_sortorder", Invoice.SORTORDER);
                objIData.AddParameter("p_customfieldexchangerate", Invoice.CUSTOMFIELDEXCHANGERATE);
                objIData.AddParameter("p_customfieldexchange", Invoice.CUSTOMFIELDEXCHANGE);
                objIData.AddParameter("p_usinginvoicetype", Invoice.USINGINVOICETYPE);

                objIData.AddParameter("p_createdby", Invoice.CREATEDBY);
                objIData.AddParameter("p_modifiedby", Invoice.MODIFIEDBY);
                objIData.AddParameter("p_modifieddate", Invoice.MODIFIEDDATE);
                objIData.AddParameter("p_ipaddress", Invoice.IPADDRESS);

                //Phiếu xuất kho
                objIData.AddParameter("p_deliveryordernumber", Invoice.DELIVERYORDERNUMBER);
                objIData.AddParameter("p_deliveryordercontent", Invoice.DELIVERYORDERCONTENT);
                objIData.AddParameter("p_transportationmethod", Invoice.TRANSPORTATIONMETHOD);
                objIData.AddParameter("p_fromwarehousename", Invoice.FROMWAREHOUSENAME);
                objIData.AddParameter("p_towarehousename", Invoice.TOWAREHOUSENAME);
                objIData.AddParameter("p_deliveryorderdate", Invoice.DELIVERYORDERDATE);
                objIData.AddParameter("p_contractnumber", Invoice.CONTRACTNUMBER);

                objIData.AddParameter("p_signedtime", Invoice.SIGNEDTIME.ToLocalTime());
                objIData.AddParameter("p_inittime", Invoice.INITTIME.ToLocalTime());
                objIData.AddParameter("p_invoicecode", Invoice.INVOICECODE);

                // Số lượng số thập phân lấy sau dấy phẩy
                objIData.AddParameter("p_quantityplace", Invoice.QUANTITYPLACE);
                objIData.AddParameter("p_priceplace", Invoice.PRICEPLACE);
                objIData.AddParameter("p_moneyplace", Invoice.MONEYPLACE);

                // Phân quyền cộng tác viên theo email CTV
                objIData.AddParameter("p_partneremail", Invoice.PARTNEREMAIL);

                // Thêm thuế, phí khác
                objIData.AddParameter("p_othertaxfee", Invoice.OTHERTAXFEE);
                // Phí hoàn, phí dịch vụ
                objIData.AddParameter("p_refundfee", Invoice.REFUNDFEE);
                objIData.AddParameter("p_servicefee", Invoice.SERVICEFEE);
                objIData.AddParameter("p_servicefeetaxrate", Invoice.SERVICEFEETAXRATE);
                objIData.AddParameter("p_servicefeetax", Invoice.SERVICEFEETAX);
                objIData.AddParameter("p_totalservicefee", Invoice.TOTALSERVICEFEE);
                // Ký theo thứ tự import excel
                objIData.AddParameter("p_signindex", Invoice.SIGNINDEX);

                var id = objIData.ExecStoreToString();
                CommitTransactionIfAny(objIData);
                return Convert.ToInt64(id);
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog(objEx.ToString(), objEx.InnerException.ToString(), MethodBase.GetCurrentMethod().Name, "AddInvoiceAPI");
                ConfigHelper.Instance.WriteLog(objEx.ToString(), objEx, MethodBase.GetCurrentMethod().Name, "AddInvoiceAPI");
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public List<InvoiceBO> GetforGDT(string taxnumber, string companyname, DateTime fromDate, DateTime toDate, int? itemPerPage, int? page)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_getfor_gdt");
                objIData.AddParameter("p_comtaxcode", taxnumber);
                objIData.AddParameter("p_customer", companyname);
                objIData.AddParameter("p_fromdate", fromDate);
                objIData.AddParameter("p_todate", toDate);
                objIData.AddParameter("p_pagesize", itemPerPage);
                objIData.AddParameter("p_offset", page);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        //Cập nhật hóa đơn
        public Int64 UpdateInvoice(InvoiceBO Invoice)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_update");
                objIData.AddParameter("p_id", Invoice.ID); //string
                objIData.AddParameter("p_comtaxcode", Invoice.COMTAXCODE); //string
                objIData.AddParameter("p_formcode", Invoice.FORMCODE); //string
                objIData.AddParameter("p_symbolcode", Invoice.SYMBOLCODE); //string
                objIData.AddParameter("p_number", Invoice.NUMBER); //int
                objIData.AddParameter("p_cusname", Invoice.CUSNAME != null ? Invoice.CUSNAME.Trim() : Invoice.CUSNAME);
                objIData.AddParameter("p_cusaddress", Invoice.CUSADDRESS != null ? Invoice.CUSADDRESS.Trim() : Invoice.CUSADDRESS);
                objIData.AddParameter("p_cusbuyer", Invoice.CUSBUYER != null ? Invoice.CUSBUYER.Trim() : Invoice.CUSBUYER);
                objIData.AddParameter("p_cusemail", Invoice.CUSEMAIL != null ? Invoice.CUSEMAIL.Trim() : Invoice.CUSEMAIL);
                objIData.AddParameter("p_cusphonenumber", Invoice.CUSPHONENUMBER != null ? Invoice.CUSPHONENUMBER.Trim() : Invoice.CUSPHONENUMBER);
                objIData.AddParameter("p_custaxcode", Invoice.CUSTAXCODE != null ? Invoice.CUSTAXCODE.Trim() : Invoice.CUSTAXCODE);
                objIData.AddParameter("p_cuspaymentmethod", Invoice.CUSPAYMENTMETHOD != null ? Invoice.CUSPAYMENTMETHOD.Trim() : Invoice.CUSPAYMENTMETHOD);
                objIData.AddParameter("p_cusaccountnumber", Invoice.CUSACCOUNTNUMBER != null ? Invoice.CUSACCOUNTNUMBER.Trim() : Invoice.CUSACCOUNTNUMBER);
                objIData.AddParameter("p_cusbankname", Invoice.CUSBANKNAME != null ? Invoice.CUSBANKNAME.Trim() : Invoice.CUSBANKNAME);
                objIData.AddParameter("p_termid", Invoice.TERMID); //int
                objIData.AddParameter("p_duedate", Invoice.DUEDATE); //datetime
                objIData.AddParameter("p_discounttype", Invoice.DISCOUNTTYPE); //int
                objIData.AddParameter("p_totalmoney", Invoice.TOTALMONEY); //decimal
                objIData.AddParameter("p_taxmoney", Invoice.TAXMONEY); //decimal
                objIData.AddParameter("p_discountmoney", Invoice.DISCOUNTMONEY); //decimal
                objIData.AddParameter("p_totalpayment", Invoice.TOTALPAYMENT); //decimal
                objIData.AddParameter("p_invoicestatus", Invoice.INVOICESTATUS); //int
                objIData.AddParameter("p_paymentstatus", Invoice.PAYMENTSTATUS); //int
                objIData.AddParameter("p_invoicetype", Invoice.INVOICETYPE); //int
                objIData.AddParameter("p_isinvoicewaiting", Invoice.ISINVOICEWAITING);
                objIData.AddParameter("p_invoicewaitingtime", Invoice.INVOICEWAITINGTIME);
                objIData.AddParameter("p_changereason", Invoice.CHANGEREASON);
                objIData.AddParameter("p_note", Invoice.NOTE);
                objIData.AddParameter("p_currency", Invoice.CURRENCY);
                objIData.AddParameter("p_exchangerate", Invoice.EXCHANGERATE);
                objIData.AddParameter("p_invoicemethod", Invoice.INVOICEMETHOD);
                objIData.AddParameter("p_fromdate", Invoice.FROMDATE);
                objIData.AddParameter("p_todate", Invoice.TODATE);
                objIData.AddParameter("p_apartmentno", Invoice.APARTMENTNO);
                objIData.AddParameter("p_customercode", Invoice.CUSTOMERCODE);
                objIData.AddParameter("p_customfieldexchangerate", Invoice.CUSTOMFIELDEXCHANGERATE);
                objIData.AddParameter("p_customfieldexchange", Invoice.CUSTOMFIELDEXCHANGE);

                objIData.AddParameter("p_modifiedby", Invoice.MODIFIEDBY);
                objIData.AddParameter("p_modifieddate", Invoice.MODIFIEDDATE);
                objIData.AddParameter("p_ipaddress", Invoice.IPADDRESS);

                //Phiếu xuất kho
                objIData.AddParameter("p_deliveryordernumber", Invoice.DELIVERYORDERNUMBER);
                objIData.AddParameter("p_deliveryordercontent", Invoice.DELIVERYORDERCONTENT);
                objIData.AddParameter("p_transportationmethod", Invoice.TRANSPORTATIONMETHOD);
                objIData.AddParameter("p_fromwarehousename", Invoice.FROMWAREHOUSENAME);
                objIData.AddParameter("p_towarehousename", Invoice.TOWAREHOUSENAME);
                objIData.AddParameter("p_deliveryorderdate", Invoice.DELIVERYORDERDATE);
                objIData.AddParameter("p_contractnumber", Invoice.CONTRACTNUMBER);

                // Số lượng số thập phân lấy sau dấy phẩy
                objIData.AddParameter("p_quantityplace", Invoice.QUANTITYPLACE);
                objIData.AddParameter("p_priceplace", Invoice.PRICEPLACE);
                objIData.AddParameter("p_moneyplace", Invoice.MONEYPLACE);

                // Thêm thuế, phí khác
                objIData.AddParameter("p_othertaxfee", Invoice.OTHERTAXFEE);
                // Phí hoàn, phí dịch vụ
                objIData.AddParameter("p_refundfee", Invoice.REFUNDFEE);
                objIData.AddParameter("p_servicefee", Invoice.SERVICEFEE);
                objIData.AddParameter("p_servicefeetaxrate", Invoice.SERVICEFEETAXRATE);
                objIData.AddParameter("p_servicefeetax", Invoice.SERVICEFEETAX);
                objIData.AddParameter("p_totalservicefee", Invoice.TOTALSERVICEFEE);

                var id = objIData.ExecStoreToString();
                CommitTransactionIfAny(objIData);
                return Convert.ToInt64(id);
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public Int64 UpdateInvoiceAPI(InvoiceBO Invoice)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_update_api");
                objIData.AddParameter("p_id", Invoice.ID); //string
                objIData.AddParameter("p_comtaxcode", Invoice.COMTAXCODE); //string
                objIData.AddParameter("p_formcode", Invoice.FORMCODE); //string
                objIData.AddParameter("p_symbolcode", Invoice.SYMBOLCODE); //string
                objIData.AddParameter("p_number", Invoice.NUMBER); //int
                objIData.AddParameter("p_cusname", Invoice.CUSNAME != null ? Invoice.CUSNAME.Trim() : Invoice.CUSNAME);
                objIData.AddParameter("p_cusaddress", Invoice.CUSADDRESS != null ? Invoice.CUSADDRESS.Trim() : Invoice.CUSADDRESS);
                objIData.AddParameter("p_cusbuyer", Invoice.CUSBUYER != null ? Invoice.CUSBUYER.Trim() : Invoice.CUSBUYER);
                objIData.AddParameter("p_cusemail", Invoice.CUSEMAIL != null ? Invoice.CUSEMAIL.Trim() : Invoice.CUSEMAIL);
                objIData.AddParameter("p_cusphonenumber", Invoice.CUSPHONENUMBER != null ? Invoice.CUSPHONENUMBER.Trim() : Invoice.CUSPHONENUMBER);
                objIData.AddParameter("p_custaxcode", Invoice.CUSTAXCODE != null ? Invoice.CUSTAXCODE.Trim() : Invoice.CUSTAXCODE);
                objIData.AddParameter("p_cuspaymentmethod", Invoice.CUSPAYMENTMETHOD != null ? Invoice.CUSPAYMENTMETHOD.Trim() : Invoice.CUSPAYMENTMETHOD);
                objIData.AddParameter("p_cusaccountnumber", Invoice.CUSACCOUNTNUMBER != null ? Invoice.CUSACCOUNTNUMBER.Trim() : Invoice.CUSACCOUNTNUMBER);
                objIData.AddParameter("p_cusbankname", Invoice.CUSBANKNAME != null ? Invoice.CUSBANKNAME.Trim() : Invoice.CUSBANKNAME);
                objIData.AddParameter("p_termid", Invoice.TERMID); //int
                objIData.AddParameter("p_duedate", Invoice.DUEDATE); //datetime
                objIData.AddParameter("p_discounttype", Invoice.DISCOUNTTYPE); //int
                objIData.AddParameter("p_totalmoney", Invoice.TOTALMONEY); //decimal
                objIData.AddParameter("p_taxmoney", Invoice.TAXMONEY); //decimal
                objIData.AddParameter("p_discountmoney", Invoice.DISCOUNTMONEY); //decimal
                objIData.AddParameter("p_totalpayment", Invoice.TOTALPAYMENT); //decimal
                objIData.AddParameter("p_invoicestatus", Invoice.INVOICESTATUS); //int
                objIData.AddParameter("p_paymentstatus", Invoice.PAYMENTSTATUS); //int
                objIData.AddParameter("p_invoicetype", Invoice.INVOICETYPE); //int
                objIData.AddParameter("p_isinvoicewaiting", Invoice.ISINVOICEWAITING);
                objIData.AddParameter("p_invoicewaitingtime", Invoice.INVOICEWAITINGTIME);
                objIData.AddParameter("p_changereason", Invoice.CHANGEREASON);
                objIData.AddParameter("p_note", Invoice.NOTE);
                objIData.AddParameter("p_currency", Invoice.CURRENCY);
                objIData.AddParameter("p_exchangerate", Invoice.EXCHANGERATE);
                objIData.AddParameter("p_invoicemethod", Invoice.INVOICEMETHOD);
                objIData.AddParameter("p_fromdate", Invoice.FROMDATE);
                objIData.AddParameter("p_todate", Invoice.TODATE);
                objIData.AddParameter("p_apartmentno", Invoice.APARTMENTNO);
                objIData.AddParameter("p_customercode", Invoice.CUSTOMERCODE);
                objIData.AddParameter("p_customfieldexchangerate", Invoice.CUSTOMFIELDEXCHANGERATE);
                objIData.AddParameter("p_customfieldexchange", Invoice.CUSTOMFIELDEXCHANGE);

                objIData.AddParameter("p_modifiedby", Invoice.MODIFIEDBY);
                objIData.AddParameter("p_modifieddate", Invoice.MODIFIEDDATE);
                objIData.AddParameter("p_ipaddress", Invoice.IPADDRESS);

                //Phiếu xuất kho
                objIData.AddParameter("p_deliveryordernumber", Invoice.DELIVERYORDERNUMBER);
                objIData.AddParameter("p_deliveryordercontent", Invoice.DELIVERYORDERCONTENT);
                objIData.AddParameter("p_transportationmethod", Invoice.TRANSPORTATIONMETHOD);
                objIData.AddParameter("p_fromwarehousename", Invoice.FROMWAREHOUSENAME);
                objIData.AddParameter("p_towarehousename", Invoice.TOWAREHOUSENAME);
                objIData.AddParameter("p_deliveryorderdate", Invoice.DELIVERYORDERDATE);
                objIData.AddParameter("p_contractnumber", Invoice.CONTRACTNUMBER);

                // Số lượng số thập phân lấy sau dấy phẩy
                objIData.AddParameter("p_quantityplace", Invoice.QUANTITYPLACE);
                objIData.AddParameter("p_priceplace", Invoice.PRICEPLACE);
                objIData.AddParameter("p_moneyplace", Invoice.MONEYPLACE);
                // 
                objIData.AddParameter("p_inittime", Invoice.INITTIME);

                var id = objIData.ExecStoreToString();
                CommitTransactionIfAny(objIData);
                return Convert.ToInt64(id);
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public InvoiceBO GetInvoiceByChecksum(string checksumCode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_get_checksum");
                objIData.AddParameter("p_checksum", checksumCode);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list.FirstOrDefault();
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        // Lấy về hóa đơn với số hóa đơn để kiểm tra ngày ký trong dải chờ 
        public List<InvoiceBO> CheckDateOfPreviousInvoice(InvoiceBO form, long number)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_waiting_check_previous_invoice");
                objIData.AddParameter("p_comtaxcode", form.COMTAXCODE);
                objIData.AddParameter("p_formcode", form.FORMCODE);
                objIData.AddParameter("p_symbolcode", form.SYMBOLCODE);
                objIData.AddParameter("p_number", number);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        /// <summary>
        /// Cập nhật biên bản hóa đơn
        /// truongnv 20200220
        /// </summary>
        /// <param name="invoiceId">ID hóa đơn</param>
        /// <param name="reason">lý do</param>
        /// <param name="reportType">loại biên bản: điều chỉnh hay hủy bỏ</param>
        /// <returns></returns>
        public bool UpdateReportInvoice(long invoiceId, string reason, int reportType)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_report_update");
                objIData.AddParameter("p_id", invoiceId);
                objIData.AddParameter("p_reason", reason);
                objIData.AddParameter("p_reporttype", reportType);
                var id = objIData.ExecNonQuery();
                CommitTransactionIfAny(objIData);
                return true;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        //Cập nhật hóa đơn hủy
        public bool UpdateCancelInvoice(InvoiceBO Invoice)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_update_cancel");
                objIData.AddParameter("p_id", Invoice.ID);
                objIData.AddParameter("p_comtaxcode", Invoice.COMTAXCODE);
                objIData.AddParameter("p_invoicetype", Invoice.INVOICETYPE);
                objIData.AddParameter("p_cancelreason", Invoice.CANCELREASON);
                objIData.AddParameter("p_canceltime", Invoice.CANCELTIME);
                var id = objIData.ExecNonQuery();
                CommitTransactionIfAny(objIData);
                return true;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        //Cập nhật lý do điều chỉnh
        public bool UpdateModifiedInvoice(InvoiceBO Invoice)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_modified_update");
                objIData.AddParameter("p_id", Invoice.ID);
                objIData.AddParameter("p_comtaxcode", Invoice.COMTAXCODE);
                objIData.AddParameter("p_changereason", Invoice.CHANGEREASON);
                var id = objIData.ExecNonQuery();
                CommitTransactionIfAny(objIData);
                return true;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        //Cập nhật hóa đơn chuyển đổi
        public Int64 UpdateConvertInvoice(long invoice_id, string fullname, int invoiceType, string signlink)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_update_convert");
                objIData.AddParameter("p_id", invoice_id);
                objIData.AddParameter("p_invoicetype", invoiceType);
                objIData.AddParameter("p_convertusername", fullname);
                objIData.AddParameter("p_signlink", signlink);
                var id = objIData.ExecStoreToString();
                CommitTransactionIfAny(objIData);
                return Convert.ToInt64(id);
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        //Cập nhật hóa đơn thay thế
        public Int64 UpdateReplaceInvoice(InvoiceBO Invoice)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_update_convert");
                objIData.AddParameter("p_id", Invoice.ID);
                objIData.AddParameter("p_comtaxcode", Invoice.COMTAXCODE);
                objIData.AddParameter("p_invoicetype", Invoice.INVOICETYPE);
                var id = objIData.ExecStoreToString();
                CommitTransactionIfAny(objIData);
                return Convert.ToInt64(id);
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public List<InvoiceBO> SearchByInvoiceCode(string iNVOICECODE, string cOMTAXCODE)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_getby_invoicecode");
                objIData.AddParameter("p_invoicecode", iNVOICECODE);
                objIData.AddParameter("p_comtaxcode", cOMTAXCODE);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public List<InvoiceBO> GetInvoiceByDate(string comTaxcode, DateTime fromDate, DateTime toDate)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_getall_bydate");
                objIData.AddParameter("p_comtaxcode", comTaxcode);
                objIData.AddParameter("p_fromdate", fromDate);
                objIData.AddParameter("p_todate", toDate);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public List<InvoiceBO> GetInvoiceAll(string comTaxcode, DateTime fromDate, DateTime toDate, int usingInvoiceType)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_getall");
                objIData.AddParameter("p_comtaxcode", comTaxcode);
                objIData.AddParameter("p_fromdate", fromDate);
                objIData.AddParameter("p_todate", toDate);
                objIData.AddParameter("p_usinginvoicetype", usingInvoiceType);

                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
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
        public string GetInvoiceCountSigned(string comTaxcode, DateTime fromDate, DateTime toDate, int usinginvocietype)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_count_signed_in_by_year");
                objIData.AddParameter("p_comtaxcode", comTaxcode);
                objIData.AddParameter("p_fromdate", fromDate);
                objIData.AddParameter("p_todate", toDate);
                objIData.AddParameter("p_usinginvoicetype", usinginvocietype);
                var total = objIData.ExecStoreToString();
                CommitTransactionIfAny(objIData);
                return total;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        //Lưu chi tiết danh sách sản phẩm trong hóa đơn
        public bool AddInvoiceDetailNoTrans(InvoiceDetailBO invoiceDetail)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_detail_add");
                objIData.AddParameter("p_invoiceid", invoiceDetail.INVOICEID);
                objIData.AddParameter("p_sku", invoiceDetail.SKU);
                objIData.AddParameter("p_productname", invoiceDetail.PRODUCTNAME.Trim());
                objIData.AddParameter("p_quantity", invoiceDetail.QUANTITY);
                objIData.AddParameter("p_quantityunit", invoiceDetail.QUANTITYUNIT);
                objIData.AddParameter("p_retailprice", invoiceDetail.RETAILPRICE);
                objIData.AddParameter("p_saleprice", invoiceDetail.SALEPRICE);
                objIData.AddParameter("p_ispromotion", invoiceDetail.ISPROMOTION);
                objIData.AddParameter("p_taxrate", invoiceDetail.TAXRATE);
                objIData.AddParameter("p_discountrate", invoiceDetail.DISCOUNTRATE);
                objIData.AddParameter("p_totalmoney", invoiceDetail.TOTALMONEY);
                objIData.AddParameter("p_totaldiscount", invoiceDetail.TOTALDISCOUNT);
                objIData.AddParameter("p_totaltax", invoiceDetail.TOTALTAX);
                objIData.AddParameter("p_totalpayment", invoiceDetail.TOTALPAYMENT);
                objIData.AddParameter("p_isdeleted", invoiceDetail.ISDELETED);

                objIData.AddParameter("p_oldno", invoiceDetail.OLDNO);
                objIData.AddParameter("p_newno", invoiceDetail.NEWNO);
                objIData.AddParameter("p_factor", invoiceDetail.FACTOR);
                objIData.AddParameter("p_metercode", invoiceDetail.METERCODE);
                objIData.AddParameter("p_metername", invoiceDetail.METERNAME);
                objIData.AddParameter("p_apartno", invoiceDetail.APARTMENTNO);
                objIData.AddParameter("p_taxratewater", invoiceDetail.TAXRATEWATER);
                objIData.AddParameter("p_consignmentid", invoiceDetail.CONSIGNMENTID);
                objIData.AddParameter("p_description", invoiceDetail.DESCRIPTION);
                objIData.AddParameter("p_groupid", invoiceDetail.GROUPID);

                // Phiếu xuất kho
                objIData.AddParameter("p_inquantity", invoiceDetail.INQUANTITY);

                // Thuế, phí khác
                objIData.AddParameter("p_othertaxfee", invoiceDetail.OTHERTAXFEE);

                // Phí hoàn
                objIData.AddParameter("p_refundfee", invoiceDetail.REFUNDFEE);

                var reader = objIData.ExecNonQuery();
                CommitTransactionIfAny(objIData);
                return true;
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog(objEx.ToString(), objEx, MethodBase.GetCurrentMethod().Name, "AddInvoiceDetailNoTrans DAO");
                RollBackTransactionIfAny(objIData);
                //throw objEx;
                return false;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }

        }
        public bool SaveInvoiceDetail(InvoiceDetailBO invoiceDetail)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_detail_add");
                objIData.AddParameter("p_invoiceid", invoiceDetail.INVOICEID);
                objIData.AddParameter("p_sku", invoiceDetail.SKU);
                objIData.AddParameter("p_productname", invoiceDetail.PRODUCTNAME.Trim());
                objIData.AddParameter("p_quantity", invoiceDetail.QUANTITY);
                objIData.AddParameter("p_quantityunit", invoiceDetail.QUANTITYUNIT);
                objIData.AddParameter("p_retailprice", invoiceDetail.RETAILPRICE);
                objIData.AddParameter("p_saleprice", invoiceDetail.SALEPRICE);
                objIData.AddParameter("p_ispromotion", invoiceDetail.ISPROMOTION);
                objIData.AddParameter("p_taxrate", invoiceDetail.TAXRATE);
                objIData.AddParameter("p_discountrate", invoiceDetail.DISCOUNTRATE);
                objIData.AddParameter("p_totalmoney", invoiceDetail.TOTALMONEY);
                objIData.AddParameter("p_totaldiscount", invoiceDetail.TOTALDISCOUNT);
                objIData.AddParameter("p_totaltax", invoiceDetail.TOTALTAX);
                objIData.AddParameter("p_totalpayment", invoiceDetail.TOTALPAYMENT);
                objIData.AddParameter("p_isdeleted", invoiceDetail.ISDELETED);

                objIData.AddParameter("p_oldno", invoiceDetail.OLDNO);
                objIData.AddParameter("p_newno", invoiceDetail.NEWNO);
                objIData.AddParameter("p_factor", invoiceDetail.FACTOR);
                objIData.AddParameter("p_metercode", invoiceDetail.METERCODE);
                objIData.AddParameter("p_metername", invoiceDetail.METERNAME);
                objIData.AddParameter("p_apartno", invoiceDetail.APARTMENTNO);
                objIData.AddParameter("p_taxratewater", invoiceDetail.TAXRATEWATER);
                objIData.AddParameter("p_consignmentid", invoiceDetail.CONSIGNMENTID);
                objIData.AddParameter("p_description", invoiceDetail.DESCRIPTION);
                objIData.AddParameter("p_groupid", invoiceDetail.GROUPID);

                // Phiếu xuất kho
                objIData.AddParameter("p_inquantity", invoiceDetail.INQUANTITY);

                // Thuế, phí khác
                objIData.AddParameter("p_othertaxfee", invoiceDetail.OTHERTAXFEE);

                // Phí hoàn
                objIData.AddParameter("p_refundfee", invoiceDetail.REFUNDFEE);

                var reader = objIData.ExecNonQuery();
                CommitTransactionIfAny(objIData);

                return true;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        //Lấy danh sách Hóa đơn
        public List<InvoiceBO> GetInvoice(FormSearchInvoice form)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_get");
                objIData.AddParameter("p_comtaxcode", form.COMTAXCODE);
                objIData.AddParameter("p_invoicestatus", form.INVOICESTATUS);
                objIData.AddParameter("p_paymentstatus", form.PAYMENTSTATUS);
                objIData.AddParameter("p_formcode", form.FORMCODE);
                objIData.AddParameter("p_symbolcode", form.SYMBOLCODE);
                objIData.AddParameter("p_number", form.NUMBER);
                objIData.AddParameter("p_customer", form.CUSTOMER);
                objIData.AddParameter("p_fromdate", form.FROMDATE);
                objIData.AddParameter("p_todate", form.TODATE);
                objIData.AddParameter("p_pagesize", form.ITEMPERPAGE);
                objIData.AddParameter("p_offset", form.OFFSET);
                objIData.AddParameter("p_invoicetype", form.INVOICETYPE);
                objIData.AddParameter("p_status", form.STATUS);
                objIData.AddParameter("p_customercode", form.CUSTOMERCODE);
                objIData.AddParameter("p_fromnumber", form.FROMNUMBER);
                objIData.AddParameter("p_tonumber", form.TONUMBER);
                objIData.AddParameter("p_usinginvoicetype", form.USINGINVOICETYPE);
                objIData.AddParameter("p_custaxcode", form.CUSTAXCODE);
                objIData.AddParameter("p_partneremail", form.PARTNEREMAIL);

                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceBO>();
                
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        //lấy ra hóa đơn phát hành
        public List<InvoiceBO> GetInvoiceSigned(FormSearchInvoice form)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_get_signed");
                objIData.AddParameter("p_comtaxcode", form.COMTAXCODE);
                objIData.AddParameter("p_paymentstatus", form.PAYMENTSTATUS);
                objIData.AddParameter("p_formcode", form.FORMCODE);
                objIData.AddParameter("p_symbolcode", form.SYMBOLCODE);
                objIData.AddParameter("p_number", form.NUMBER);
                objIData.AddParameter("p_customer", form.CUSTOMER);
                objIData.AddParameter("p_fromdate", form.FROMDATE);
                objIData.AddParameter("p_todate", form.TODATE);
                objIData.AddParameter("p_pagesize", form.ITEMPERPAGE);
                objIData.AddParameter("p_offset", form.OFFSET);
                objIData.AddParameter("p_invoicetype", form.INVOICETYPE);
                objIData.AddParameter("p_status", form.STATUS);
                objIData.AddParameter("p_customercode", form.CUSTOMERCODE);
                objIData.AddParameter("p_fromnumber", form.FROMNUMBER);
                objIData.AddParameter("p_tonumber", form.TONUMBER);
                objIData.AddParameter("p_usinginvoicetype", form.USINGINVOICETYPE);

                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        //Lấy danh sách Hóa đơn API
        public List<InvoiceBO> GetInvoiceAPI(FormSearchInvoice form)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_get_api");
                objIData.AddParameter("p_comtaxcode", form.COMTAXCODE);
                objIData.AddParameter("p_invoicestatus", form.INVOICESTATUS);
                objIData.AddParameter("p_paymentstatus", form.PAYMENTSTATUS);
                objIData.AddParameter("p_formcode", form.FORMCODE);
                objIData.AddParameter("p_symbolcode", form.SYMBOLCODE);
                objIData.AddParameter("p_number", form.NUMBER);
                objIData.AddParameter("p_customer", form.CUSTOMER);
                objIData.AddParameter("p_fromdate", form.FROMDATE);
                objIData.AddParameter("p_todate", form.TODATE);
                objIData.AddParameter("p_pagesize", form.ITEMPERPAGE);
                objIData.AddParameter("p_offset", form.OFFSET);
                objIData.AddParameter("p_invoicetype", form.INVOICETYPE);

                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        /// <summary>
        /// Lấy danh sách Hóa đơn phát hành lỗi
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public List<InvoiceBO> GetInvoiceReleaseError(FormSearchInvoice form)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_get_release_error");
                objIData.AddParameter("p_comtaxcode", form.COMTAXCODE);
                objIData.AddParameter("p_invoicestatus", form.INVOICESTATUS);
                objIData.AddParameter("p_paymentstatus", form.PAYMENTSTATUS);
                objIData.AddParameter("p_formcode", form.FORMCODE);
                objIData.AddParameter("p_symbolcode", form.SYMBOLCODE);
                objIData.AddParameter("p_number", form.NUMBER);
                objIData.AddParameter("p_customer", form.CUSTOMER);
                objIData.AddParameter("p_fromdate", form.FROMDATE);
                objIData.AddParameter("p_todate", form.TODATE);
                objIData.AddParameter("p_pagesize", form.ITEMPERPAGE);
                objIData.AddParameter("p_offset", form.OFFSET);
                objIData.AddParameter("p_invoicetype", form.INVOICETYPE);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public List<InvoiceBO> GetInvoiceByStatus(FormSearchInvoice form)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_get_by_status");
                objIData.AddParameter("p_comtaxcode", form.COMTAXCODE);
                objIData.AddParameter("p_invoicetype", form.INVOICETYPE);
                objIData.AddParameter("p_fromdate", form.FROMDATE);
                objIData.AddParameter("p_todate", form.TODATE);
                objIData.AddParameter("p_pagesize", form.ITEMPERPAGE);
                objIData.AddParameter("p_offset", form.OFFSET);
                objIData.AddParameter("p_reportType", form.REPORTYPE.ToString());
                objIData.AddParameter("p_number", form.NUMBER);
                objIData.AddParameter("p_formcode", form.FORMCODE);
                objIData.AddParameter("p_symbolcode", form.SYMBOLCODE);
                objIData.AddParameter("p_cusname", form.CUSTOMER);
                objIData.AddParameter("p_partneremail", form.PARTNEREMAIL);
                objIData.AddParameter("p_usinginvoicetype", form.USINGINVOICETYPE);

                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        //danh sách HĐ xóa bỏ
        public List<InvoiceBO> GetInvoiceDelete(FormSearchInvoice form)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_get_delete");
                objIData.AddParameter("p_comtaxcode", form.COMTAXCODE);
                objIData.AddParameter("p_paymentstatus", form.PAYMENTSTATUS);
                objIData.AddParameter("p_formcode", form.FORMCODE);
                objIData.AddParameter("p_symbolcode", form.SYMBOLCODE);
                objIData.AddParameter("p_number", form.NUMBER);
                objIData.AddParameter("p_customer", form.CUSTOMER);
                objIData.AddParameter("p_fromdate", form.FROMDATE);
                objIData.AddParameter("p_todate", form.TODATE);
                objIData.AddParameter("p_pagesize", form.ITEMPERPAGE);
                objIData.AddParameter("p_offset", form.OFFSET);
                objIData.AddParameter("p_usinginvoicetype", form.USINGINVOICETYPE);
                objIData.AddParameter("p_partneremail", form.PARTNEREMAIL);

                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }
        //danh sách HĐ chờ duyệt
        public List<InvoiceBO> GetInvoiceWating(FormSearchInvoice form)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_get_wating");
                objIData.AddParameter("p_comtaxcode", form.COMTAXCODE);
                objIData.AddParameter("p_invoicestatus", form.INVOICESTATUS);
                objIData.AddParameter("p_paymentstatus", form.PAYMENTSTATUS);
                objIData.AddParameter("p_formcode", form.FORMCODE);
                objIData.AddParameter("p_symbolcode", form.SYMBOLCODE);
                objIData.AddParameter("p_number", form.NUMBER);
                objIData.AddParameter("p_customer", form.CUSTOMER);
                objIData.AddParameter("p_fromdate", form.FROMDATE);
                objIData.AddParameter("p_todate", form.TODATE);
                objIData.AddParameter("p_pagesize", form.ITEMPERPAGE);
                objIData.AddParameter("p_offset", form.OFFSET);
                objIData.AddParameter("p_invoicetype", form.INVOICETYPE);
                objIData.AddParameter("p_status", form.STATUS);
                objIData.AddParameter("p_usinginvoicetype", form.USINGINVOICETYPE);
                objIData.AddParameter("p_customercode", form.CUSTOMERCODE);
                objIData.AddParameter("p_partneremail", form.PARTNEREMAIL);

                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        /// <summary>
        /// Lấy dữ liệu bảng danh mục
        /// </summary>
        /// <param name="propertyVal">Giá trị</param>
        /// <param name="storedName">Tên stored</param>
        /// <returns></returns>
        public object GetDataCategories(int propertyVal, string storedName)
        {
            object vVal;
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure(storedName);
                objIData.AddParameter("p_id", propertyVal);
                vVal = objIData.ExecStoreToString();
                CommitTransactionIfAny(objIData);
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
            return vVal;
        }

        public List<InvoiceBO> GetListInvoiceByCustomer(FormSearchInvoice form)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_get_by_customer");
                objIData.AddParameter("p_comtaxcode", form.COMTAXCODE);
                objIData.AddParameter("p_invoicestatus", form.INVOICESTATUS);
                objIData.AddParameter("p_customer", form.CUSTOMER);
                objIData.AddParameter("p_phonenumber", form.PHONENUMBER);
                objIData.AddParameter("p_taxcode", form.TAXCODE);
                objIData.AddParameter("p_fromdate", form.FROMDATE);
                objIData.AddParameter("p_todate", form.TODATE);
                objIData.AddParameter("p_pagesize", form.ITEMPERPAGE);
                objIData.AddParameter("p_offset", form.OFFSET);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        //Lấy danh sách Hóa đơn
        public InvoiceBO GetInvoiceById(long invoiceId, int usingInvoiceType = -1)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_get_by_id");
                objIData.AddParameter("p_invoiceid", invoiceId);
                objIData.AddParameter("p_usinginvoicetype", usingInvoiceType);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list.FirstOrDefault();
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        //Lấy danh sách Hóa đơn
        public List<InvoiceBO> GetMultiInvoice(string lstid, string comtaxcode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_get_multi_id");
                objIData.AddParameter("p_ids", lstid);
                objIData.AddParameter("p_comtaxcode", comtaxcode);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }


        public InvoiceBO GetInvoiceById(long invoiceId, System.Web.HttpContext context, int usingInvoiceType = -1)
        {
            if (usingInvoiceType == -1)
            {
                if (context.Session["USINGINVOICETYPEID"] != null)
                    usingInvoiceType = (int)context.Session["USINGINVOICETYPEID"];
            }

            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_get_by_id");
                objIData.AddParameter("p_invoiceid", invoiceId);
                objIData.AddParameter("p_usinginvoicetype", usingInvoiceType);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list.FirstOrDefault();
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        // Update sign link
        public bool UpdateInvoiceSignLink(long invoiceId, string signLink, string referenceCode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_signlink_update");
                objIData.AddParameter("p_id", invoiceId);
                objIData.AddParameter("p_signlink", signLink);
                objIData.AddParameter("p_referencecode", referenceCode);
                var reader = objIData.ExecNonQuery();
                CommitTransactionIfAny(objIData);
                return true;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public bool UpdateSignLink(long invoiceId, string signLink)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_signlink_import_update");
                objIData.AddParameter("p_id", invoiceId);
                objIData.AddParameter("p_signlink", signLink);
                var reader = objIData.ExecNonQuery();
                CommitTransactionIfAny(objIData);
                return true;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public string DeleteInvoice(string invoiceIds)
        {
            IData objIData = this.CreateIData();
            string msg = string.Empty;
            string query = $"UPDATE ds_masterdata.pm_invoice SET active = true, modifieddate = CURRENT_TIMESTAMP WHERE pm_invoice.id IN({invoiceIds}); " +
                           $"UPDATE ds_masterdata.pm_invoice_detail SET isdeleted = true WHERE pm_invoice_detail.invoiceid IN({invoiceIds});";
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.ExecUpdate(query);
                CommitTransactionIfAny(objIData);
                return string.Empty;
            }
            catch (Exception objEx)
            {
                msg = objEx.Message;
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        //xóa hẳn HĐ
        public string DeletedInvoice(string ids)
        {
            IData objIData = this.CreateIData();
            string msg = string.Empty;
            string query = $"DELETE FROM ds_masterdata.pm_invoice WHERE pm_invoice.id IN({ids}); " +
                           $"DELETE FROM ds_masterdata.pm_invoice_detail WHERE pm_invoice_detail.invoiceid IN ({ids})";
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.ExecQueryToString(query);
                CommitTransactionIfAny(objIData);
                return string.Empty;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                RollBackTransactionIfAny(objIData);
                throw ex;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        //xóa hẳn HĐ
        public List<InvoiceBO> DeletedAllInvoice(string comTaxCode, int usingInvoiceType)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_deleted_all");
                objIData.AddParameter("p_comtaxcode", comTaxCode);
                objIData.AddParameter("p_usinginvoicetype", usingInvoiceType);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception ex)
            {
                RollBackTransactionIfAny(objIData);
                throw ex;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public string UpdatePaymentStatus(string invoiceIds, int paymentStatus)
        {
            IData objIData = this.CreateIData();
            string msg = string.Empty;
            string query = $"UPDATE ds_masterdata.pm_invoice" +
                $" SET paymentstatus = {paymentStatus}, modifieddate = CURRENT_TIMESTAMP" +
                $" WHERE pm_invoice.id IN({invoiceIds});";
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.ExecUpdate(query);
                CommitTransactionIfAny(objIData);
                return string.Empty;
            }
            catch (Exception objEx)
            {
                msg = objEx.Message;
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public string UpdatePartner(string invoiceIds, string email)
        {
            IData objIData = this.CreateIData();
            string msg = string.Empty;
            string query = $"UPDATE ds_masterdata.pm_invoice" +
                $" SET partneremail = '{email}', modifieddate = CURRENT_TIMESTAMP" +
                $" WHERE pm_invoice.id IN({invoiceIds});";
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.ExecUpdate(query);
                CommitTransactionIfAny(objIData);
                return string.Empty;
            }
            catch (Exception objEx)
            {
                msg = objEx.Message;
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        // Update invoice status and signed link
        public bool UpdateInvoiceStatusSignedLink(long invoiceId, string signedLink)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_status_signedlink_update");
                objIData.AddParameter("p_id", invoiceId);
                objIData.AddParameter("p_signedlink", signedLink);
                var reader = objIData.ExecNonQuery();
                CommitTransactionIfAny(objIData);
                return true;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        // Update invoice status and checksumxml link
        public bool UpdateInvoiceCheckSumXml(long invoiceId, string checkSumXml)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_checksum_update");
                objIData.AddParameter("p_id", invoiceId);
                objIData.AddParameter("p_checksumxml", checkSumXml);
                var reader = objIData.ExecNonQuery();
                CommitTransactionIfAny(objIData);
                return true;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        // Update canceled link
        public bool UpdateInvoiceCanceledLink(long invoiceId, string canceledLink)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_canceledlink_update");
                objIData.AddParameter("p_id", invoiceId);
                objIData.AddParameter("p_canceledlink", canceledLink);
                var reader = objIData.ExecNonQuery();
                CommitTransactionIfAny(objIData);
                return true;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        // Update invoice no
        public long UpdateInvoiceNumber(long invoiceId, long nextNumber, DateTime dtSignTime, string checkSum, int invoiceStatus)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_number_update");
                objIData.AddParameter("p_id", invoiceId);
                objIData.AddParameter("p_nextnumber", nextNumber);
                objIData.AddParameter("p_signedtime", dtSignTime);
                objIData.AddParameter("p_invoicestatus", invoiceStatus);
                objIData.AddParameter("p_checksum", checkSum);
                var id = objIData.ExecStoreToString();
                long number = Convert.ToInt64(id);

                CommitTransactionIfAny(objIData);
                return number;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        // Update waitting invoice no
        public long UpdateInvoiceWaittingNumber(long invoiceid, long numberid, DateTime signedTime)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_number_update");
                objIData.AddParameter("p_id", invoiceid);
                objIData.AddParameter("p_numberid", numberid);
                objIData.AddParameter("p_signedtime", signedTime);
                var id = objIData.ExecStoreToString();
                long number = Convert.ToInt64(id);

                CommitTransactionIfAny(objIData);
                return number;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        //Lấy chi tiết Hóa đơn
        public List<InvoiceDetailBO> GetInvoiceDetail(long invoiceid)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_detail_get");
                objIData.AddParameter("p_invoiceid", invoiceid);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceDetailBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list.OrderBy(x => x.INITTIME).ToList();
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        //Lấy chi tiết Hóa đơn theo danh sách ID
        public List<InvoiceDetailBO> GetInvoiceDetailByIds(string ids)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_detail_getids");
                objIData.AddParameter("p_ids", ids);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceDetailBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        /// <summary>
        /// Lấy hóa đơn theo mã code có template path
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public InvoiceBO GetInvoiceByCode(string code)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_get_by_code");
                objIData.AddParameter("p_code", code);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list.FirstOrDefault();
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public InvoiceBO GetInvoiceByReferenceCode(string referencecode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_get_referencecode");
                objIData.AddParameter("p_referencecode", referencecode);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list.FirstOrDefault();
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }


        //Cập nhật link file đính kèm 
        public bool UpdateAttachFileLink(InvoiceBO invoice)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_update_attachmentfilelink");
                objIData.AddParameter("p_comtaxcode", invoice.COMTAXCODE);
                objIData.AddParameter("p_id", invoice.ID);
                objIData.AddParameter("p_attachmentfilelink", invoice.ATTACHMENTFILELINK);
                var reader = objIData.ExecNonQuery();
                CommitTransactionIfAny(objIData);
                return true;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
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
            IData objIData = this.CreateIData();
            long number = 0;
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_update_signed");
                objIData.AddParameter("p_id", invoiceId);
                objIData.AddParameter("p_nextnumber", tempNextNumber);
                objIData.AddParameter("p_signedtime", dtSignTime);
                objIData.AddParameter("p_checksumxml", checkSumXml);
                objIData.AddParameter("p_signedlink", signLink);
                objIData.AddParameter("p_source", source);
                var id = objIData.ExecStoreToString();
                number = Convert.ToInt64(id);
                CommitTransactionIfAny(objIData);
                return number;
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog($"Lỗi gì đó khi cập nhật sau ký. number: {number}. invoiceId: {invoiceId}. tempNextNumber: {tempNextNumber}. dtSignTime: {dtSignTime}. checkSumXml: {checkSumXml}. ", objEx, MethodBase.GetCurrentMethod().Name, "UpdateDataInvoice DAO");
                RollBackTransactionIfAny(objIData);
                return -1;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }
        public List<InvoiceBO> GetAllInvoive(InvoiceSearchFormBO invoiceSearch)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_getby_taxcode");
                objIData.AddParameter("p_comtaxcode", invoiceSearch.COMTAXCODE);
                objIData.AddParameter("p_companyname", invoiceSearch.COMPANYNAME);
                objIData.AddParameter("p_keyword", invoiceSearch.KEYWORD);
                objIData.AddParameter("p_pagesize", invoiceSearch.ITEMPERPAGE);
                objIData.AddParameter("p_offset", invoiceSearch.OFFSET);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }
        public bool UpdateInvoiceStatus(InvoiceBO invoice)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_update_status");
                objIData.AddParameter("p_comtaxcode", invoice.COMTAXCODE);
                objIData.AddParameter("p_formcode", invoice.FORMCODE);
                objIData.AddParameter("p_symbolcode", invoice.SYMBOLCODE);
                objIData.AddParameter("p_currentnumber", invoice.NUMBERTEMP);
                objIData.AddParameter("p_invoicewating", invoice.INVOICEWATING);
                objIData.AddParameter("p_invoicestatus", invoice.INVOICESTATUS);
                objIData.AddParameter("p_number", invoice.NUMBER);
                var reader = objIData.ExecNonQuery();
                CommitTransactionIfAny(objIData);

                return true;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public List<InvoiceBO> ExportInvoiceExcelBySignDate(FormSearchInvoice form)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_get_by_signtime");
                objIData.AddParameter("p_comtaxcode", form.COMTAXCODE);
                objIData.AddParameter("p_invoicestatus", form.INVOICESTATUS);
                objIData.AddParameter("p_paymentstatus", form.PAYMENTSTATUS);
                objIData.AddParameter("p_formcode", form.FORMCODE);
                objIData.AddParameter("p_symbolcode", form.SYMBOLCODE);
                objIData.AddParameter("p_number", form.NUMBER);
                objIData.AddParameter("p_customer", form.CUSTOMER);
                objIData.AddParameter("p_fromdate", form.FROMDATE);
                objIData.AddParameter("p_todate", form.TODATE);
                objIData.AddParameter("p_invoicetype", form.INVOICETYPE);
                objIData.AddParameter("p_status", form.STATUS);
                objIData.AddParameter("p_usinginvoicetype", form.USINGINVOICETYPE);

                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public List<InvoiceBO> ExportInvoiceExcelByInitDate(FormSearchInvoice form)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_get_by_inittime");
                objIData.AddParameter("p_comtaxcode", form.COMTAXCODE);
                objIData.AddParameter("p_invoicestatus", form.INVOICESTATUS);
                objIData.AddParameter("p_paymentstatus", form.PAYMENTSTATUS);
                objIData.AddParameter("p_formcode", form.FORMCODE);
                objIData.AddParameter("p_symbolcode", form.SYMBOLCODE);
                objIData.AddParameter("p_number", form.NUMBER);
                objIData.AddParameter("p_customer", form.CUSTOMER);
                objIData.AddParameter("p_fromdate", form.FROMDATE);
                objIData.AddParameter("p_todate", form.TODATE);
                objIData.AddParameter("p_invoicetype", form.INVOICETYPE);
                objIData.AddParameter("p_status", form.STATUS);
                objIData.AddParameter("p_usinginvoicetype", form.USINGINVOICETYPE);

                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public List<InvoiceBO> ExportInvoiceExcelByIds(string ids)
        {
            string query =
                $" SELECT pm_invoice.*," +
                $" common_invoice_type.invoicetypename," +
                $" COALESCE" +
                $" ( pm_invoice.customercode, (" +
                    $" SELECT pm_customer.cusid" +
                    $" FROM ds_masterdata.pm_customer" +
                    $" WHERE pm_customer.custaxcode = pm_invoice.custaxcode" +
                    $" LIMIT 1" +
                $" ) ) as cusid" +
                $" FROM ds_masterdata.pm_invoice " +
                $" LEFT JOIN ds_masterdata.common_invoice_type ON common_invoice_type.invoicetypeid = pm_invoice.invoicetype" +
                $" WHERE id IN ({ids})";
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                var reader = objIData.ExecQueryToDataReader(query);
                var list = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public List<InvoiceOutput> GetListIDInvoce(string comtaxcode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_get_list_invoice_ids");
                objIData.AddParameter("p_comtaxcode", comtaxcode);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceOutput>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }
        public List<InvoiceBO> GetSearchCustomerID(string cusPhoneNumber, string customerCode, int page, int offset, DateTime fromDate, DateTime toDate, string comTaxCode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_customer_get_customerid");
                objIData.AddParameter("p_comtaxcode", comTaxCode);
                objIData.AddParameter("p_fromdate", fromDate);
                objIData.AddParameter("p_todate", toDate);
                objIData.AddParameter("p_pagesize", page);
                objIData.AddParameter("p_offset", offset);
                objIData.AddParameter("p_cusphonenumber", cusPhoneNumber);
                objIData.AddParameter("p_customercode", customerCode);

                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public string RecoverInvoice(string invoiceIds)
        {
            IData objIData = this.CreateIData();
            string msg = string.Empty;
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.ExecUpdate($"UPDATE ds_masterdata.pm_invoice SET active = false WHERE pm_invoice.id IN({invoiceIds}) ;UPDATE ds_masterdata.pm_invoice_detail SET isdeleted=false Where pm_invoice_detail.invoiceid IN({invoiceIds})");
                CommitTransactionIfAny(objIData);
                return string.Empty;
            }
            catch (Exception objEx)
            {
                msg = objEx.Message;
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }
        //báo cáo tình hình sử dụng hóa đơn

        public List<InvoiceSummary> Getforstatistics(InvoiceSearchFormBO invoiceSearch)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_statistics_get");
                objIData.AddParameter("p_keyword", invoiceSearch.KEYWORD);
                objIData.AddParameter("p_status", invoiceSearch.STATUS);
                objIData.AddParameter("p_pagesize", invoiceSearch.ITEMPERPAGE);
                objIData.AddParameter("p_offset", invoiceSearch.OFFSET);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceSummary>();
                var listBO = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public List<InvoiceSummary> GetStatisticTopTen(InvoiceSearchFormBO invoiceSearch)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_statistics_get_bytime");
                objIData.AddParameter("p_keyword", invoiceSearch.KEYWORD);
                objIData.AddParameter("p_status", invoiceSearch.STATUS);
                objIData.AddParameter("p_pagesize", invoiceSearch.ITEMPERPAGE);
                objIData.AddParameter("p_offset", invoiceSearch.OFFSET);
                objIData.AddParameter("p_fromtime", invoiceSearch.FROMTIME);
                objIData.AddParameter("p_totime", invoiceSearch.TOTIME);

                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceSummary>();
                var listBO = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        //số hóa đơn theo từng doanh nghiệp
        public string GetInvoiceCount(string comname, int usingtype, int usingtypeinvoice)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_count_all_invoice");
                objIData.AddParameter("p_comname", comname);
                objIData.AddParameter("p_usingtype", usingtype);
                objIData.AddParameter("p_usingtypeinvoice", usingtypeinvoice);
                var total = objIData.ExecStoreToString();
                if (string.IsNullOrEmpty(total))
                {
                    total = "0";
                }
                CommitTransactionIfAny(objIData);
                return total;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }
        //số khách hàng theo trạng thái
        public string GetCompanyCount(string status, string keyword)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_company_count");
                objIData.AddParameter("p_status", status);
                objIData.AddParameter("p_keyword", keyword);
                var total = objIData.ExecStoreToString();
                if (string.IsNullOrEmpty(total))
                {
                    total = "0";
                }
                CommitTransactionIfAny(objIData);
                return total;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public InvoiceBO GetInvoiceByIdAPI(long invoiceId)
        {
            int usinginvoiceTypeId = 0;
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_get_by_id");
                objIData.AddParameter("p_invoiceid", invoiceId);
                objIData.AddParameter("p_usinginvoicetype", usinginvoiceTypeId);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list.FirstOrDefault();
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public InvoiceBO GetMaxNumberInvoice(FormSearchInvoice form)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_get_max_number_invoice");
                objIData.AddParameter("p_comtaxcode", form.COMTAXCODE);
                objIData.AddParameter("p_formcode", form.FORMCODE);
                objIData.AddParameter("p_symbolcode", form.SYMBOLCODE);

                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list.FirstOrDefault();
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public InvoiceBO GetInvoiceByNumber(FormSearchInvoice form)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_get_by_number");
                objIData.AddParameter("p_comtaxcode", form.COMTAXCODE);
                objIData.AddParameter("p_formcode", form.FORMCODE);
                objIData.AddParameter("p_symbolcode", form.SYMBOLCODE);
                objIData.AddParameter("p_number", form.NUMBER);

                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list.FirstOrDefault();
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        #endregion
    }
}