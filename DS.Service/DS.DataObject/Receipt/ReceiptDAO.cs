using DS.BusinessObject.Account;
using DS.BusinessObject.Invoice;
using DS.BusinessObject.Receipt;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.DataObject.Receipt
{
    public class ReceiptDAO : BaseDAO
    {
        #region Constructor
        public ReceiptDAO() : base()
        {
        }

        public ReceiptDAO(IData objIData)
            : base(objIData)
        {
        }

        #endregion

        #region Methods
        public List<InvoiceBO> GetReceipt(FormSearchInvoice form)
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

        public Int64 AddReceipt(ReceiptBO objReceipt)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_add");
                objIData.AddParameter("p_comname", objReceipt.COMNAME);
                objIData.AddParameter("p_comtaxcode", objReceipt.COMTAXCODE);
                objIData.AddParameter("p_comaddress", objReceipt.COMADDRESS);
                objIData.AddParameter("p_formcode", objReceipt.FORMCODE);
                objIData.AddParameter("p_symbolcode", objReceipt.SYMBOLCODE);
                objIData.AddParameter("p_number", objReceipt.NUMBER);
                objIData.AddParameter("p_cusname", objReceipt.CUSNAME);
                objIData.AddParameter("p_cusaddress", objReceipt.CUSADDRESS);
                objIData.AddParameter("p_cusbuyer", objReceipt.CUSBUYER);
                objIData.AddParameter("p_cusemail", objReceipt.CUSEMAIL);
                objIData.AddParameter("p_cusphonenumber", objReceipt.CUSPHONENUMBER);
                objIData.AddParameter("p_custaxcode", objReceipt.CUSTAXCODE);
                objIData.AddParameter("p_cuspaymentmethod", objReceipt.CUSPAYMENTMETHOD);
                objIData.AddParameter("p_cusaccountnumber", objReceipt.CUSACCOUNTNUMBER);
                objIData.AddParameter("p_cusbankname", objReceipt.CUSBANKNAME);
                objIData.AddParameter("p_termid", objReceipt.TERMID);
                objIData.AddParameter("p_duedate", objReceipt.DUEDATE);
                objIData.AddParameter("p_discounttype", objReceipt.DISCOUNTTYPE);
                objIData.AddParameter("p_totalmoney", objReceipt.TOTALMONEY);
                objIData.AddParameter("p_taxmoney", objReceipt.TAXMONEY);
                objIData.AddParameter("p_discountmoney", objReceipt.DISCOUNTMONEY);
                objIData.AddParameter("p_totalpayment", objReceipt.TOTALPAYMENT);
                objIData.AddParameter("p_invoicestatus", objReceipt.INVOICESTATUS);
                objIData.AddParameter("p_paymentstatus", objReceipt.PAYMENTSTATUS);
                objIData.AddParameter("p_invoicetype", objReceipt.INVOICETYPE);
                objIData.AddParameter("p_reference", objReceipt.REFERENCE);
                objIData.AddParameter("p_isinvoicewaiting", objReceipt.ISINVOICEWAITING);
                objIData.AddParameter("p_invoicewaitingtime", objReceipt.INVOICEWAITINGTIME);
                objIData.AddParameter("p_changereason", objReceipt.CHANGEREASON);
                objIData.AddParameter("p_note", objReceipt.NOTE);
                objIData.AddParameter("p_currency", objReceipt.CURRENCY);
                objIData.AddParameter("p_exchangerate", objReceipt.EXCHANGERATE);
                objIData.AddParameter("p_referencecode", objReceipt.REFERENCECODE);
                objIData.AddParameter("p_signlink", objReceipt.SIGNLINK);
                objIData.AddParameter("p_invoicemethod", 0);
                objIData.AddParameter("p_fromdate", DateTime.Now);
                objIData.AddParameter("p_todate", DateTime.Now);
                objIData.AddParameter("p_apartmentno", 0);
                objIData.AddParameter("p_customercode", objReceipt.CUSTOMERCODE);
                objIData.AddParameter("p_sortorder", 0);
                objIData.AddParameter("p_customfieldexchangerate", 0);
                objIData.AddParameter("p_customfieldexchange", 0);
                objIData.AddParameter("p_usinginvoicetype", objReceipt.USINGINVOICETYPE);

                objIData.AddParameter("p_createdby", objReceipt.CREATEDBY);
                objIData.AddParameter("p_modifiedby", objReceipt.MODIFIEDBY);
                objIData.AddParameter("p_modifieddate", objReceipt.MODIFIEDDATE);
                objIData.AddParameter("p_ipaddress", objReceipt.IPADDRESS);

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

        public ReceiptBO GetReceiptById(long invoiceId)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_get_by_id");
                objIData.AddParameter("p_invoiceid", invoiceId);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<ReceiptBO>();
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

        public Int64 UpdateReceipt(ReceiptBO Invoice)
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
                objIData.AddParameter("p_cusname", Invoice.CUSNAME); //string
                objIData.AddParameter("p_cusaddress", Invoice.CUSADDRESS); //string
                objIData.AddParameter("p_cusbuyer", Invoice.CUSBUYER); //string
                objIData.AddParameter("p_cusemail", Invoice.CUSEMAIL); //string
                objIData.AddParameter("p_cusphonenumber", Invoice.CUSPHONENUMBER); //int
                objIData.AddParameter("p_custaxcode", Invoice.CUSTAXCODE); //string
                objIData.AddParameter("p_cuspaymentmethod", Invoice.CUSPAYMENTMETHOD); //string
                objIData.AddParameter("p_cusaccountnumber", Invoice.CUSACCOUNTNUMBER); //string
                objIData.AddParameter("p_cusbankname", Invoice.CUSBANKNAME); //string
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
                objIData.AddParameter("p_invoicemethod", 0);
                objIData.AddParameter("p_fromdate", DateTime.Now);
                objIData.AddParameter("p_todate", DateTime.Now);
                objIData.AddParameter("p_apartmentno", 0);
                objIData.AddParameter("p_customercode", Invoice.CUSTOMERCODE);
                objIData.AddParameter("p_customfieldexchangerate", 0);
                objIData.AddParameter("p_customfieldexchange", 0);

                objIData.AddParameter("p_modifiedby", Invoice.MODIFIEDBY);
                objIData.AddParameter("p_modifieddate", Invoice.MODIFIEDDATE);
                objIData.AddParameter("p_ipaddress", Invoice.IPADDRESS);

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
        #endregion
    }
}