using DS.BusinessObject;
using DS.BusinessObject.Account;
using DS.BusinessObject.Invoice;
using DS.BusinessObject.Product;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.DataObject.Number
{
    public class NumberDAO : BaseDAO
    {
        #region Constructor

        public NumberDAO() : base()
        {
        }

        public NumberDAO(IData objIData)
            : base(objIData)
        {
        }

        #endregion

        #region Methods

        //Lấy danh sách dải hóa đơn chờ
        public List<InvoiceNumberBO> GetNumberWaiting(FormSearchNumber form)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_number_waiting_get");
                objIData.AddParameter("p_comtaxcode", form.COMTAXCODE);
                objIData.AddParameter("p_pagesize", form.ITEMPERPAGE);
                objIData.AddParameter("p_offset", form.OFFSET);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceNumberBO>();
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

        public InvoiceNumberBO GetWaitingNumberByWaitingNumberId(long waitingNumberId)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_number_waiting_get_by_id");
                objIData.AddParameter("p_id", waitingNumberId);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceNumberBO>();
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
        //public Int64 GetWaitingNumberByWaitingNumberId(long waitingNumberId)
        //{
        //    IData objIData = this.CreateIData();
        //    try
        //    {
        //        BeginTransactionIfAny(objIData);
        //        objIData.CreateNewStoredProcedure("ds_masterdata.pm_number_waiting_get_by_id");
        //        objIData.AddParameter("p_id", waitingNumberId);
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

        //Tạo mới hóa đơn cách dải
        public bool AddNumberWaiting(InvoiceNumberBO invoiceNumber)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_number_waiting_add");
                objIData.AddParameter("p_comtaxcode", invoiceNumber.COMTAXCODE);
                objIData.AddParameter("p_formcodeid", invoiceNumber.FORMCODE);
                objIData.AddParameter("p_symbolcodeid", invoiceNumber.SYMBOLCODE);
                objIData.AddParameter("p_fromnumber", invoiceNumber.FROMNUMBER);
                objIData.AddParameter("p_tonumber", invoiceNumber.TONUMBER);
                objIData.AddParameter("p_note", invoiceNumber.NOTE);

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

        //Cập nhật dãy số cách dải từ số, đến số
        public bool UpdateNumberWaiting(InvoiceNumberBO invoiceNumber)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_number_waiting_update");
                objIData.AddParameter("p_id", invoiceNumber.ID);
                objIData.AddParameter("p_comtaxcode", invoiceNumber.COMTAXCODE);
                objIData.AddParameter("p_fromnumber", invoiceNumber.FROMNUMBER);
                objIData.AddParameter("p_tonumber", invoiceNumber.TONUMBER);
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

        //Cập nhật số hiện tại của dãy cách dải
        public bool UpdateCurrentWaittingNumber(long numberId, long nextNumber)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_number_waiting_current_number_update");
                objIData.AddParameter("p_id", numberId);
                objIData.AddParameter("p_nextnumber", nextNumber);
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

        public bool AddReleaseNotice(InvoiceNumberBO invoiceNumber)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_number_add");
                objIData.AddParameter("p_comtaxcode", invoiceNumber.COMTAXCODE);
                objIData.AddParameter("p_formcode", invoiceNumber.FORMCODE);
                objIData.AddParameter("p_symbolcode", invoiceNumber.SYMBOLCODE);
                objIData.AddParameter("p_fromnumber", invoiceNumber.FROMNUMBER);
                objIData.AddParameter("p_tonumber", invoiceNumber.TONUMBER);
                objIData.AddParameter("p_fromtime", invoiceNumber.FROMTIME);
                objIData.AddParameter("p_totime", invoiceNumber.TOTIME);
                objIData.AddParameter("p_templatepath", invoiceNumber.TEMPLATEPATH);
                objIData.AddParameter("p_status", invoiceNumber.STATUS);
                objIData.AddParameter("p_taxrate", invoiceNumber.TAXRATE);
                objIData.AddParameter("p_usinginvoicetype", invoiceNumber.USINGINVOICETYPE);

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

        public bool SaveReleaseNotice(InvoiceNumberBO invoiceNumber)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_number_update");
                objIData.AddParameter("p_comtaxcode", invoiceNumber.COMTAXCODE);
                objIData.AddParameter("p_formcode", invoiceNumber.FORMCODE);
                objIData.AddParameter("p_symbolcode", invoiceNumber.SYMBOLCODE);
                objIData.AddParameter("p_oldfromnumber", invoiceNumber.OLDFROMNUMBER);
                objIData.AddParameter("p_oldtonumber", invoiceNumber.OLDTONUMBER);
                objIData.AddParameter("p_fromnumber", invoiceNumber.FROMNUMBER);
                objIData.AddParameter("p_tonumber", invoiceNumber.TONUMBER);
                objIData.AddParameter("p_fromtime", invoiceNumber.FROMTIME);
                objIData.AddParameter("p_totime", invoiceNumber.TOTIME);
                objIData.AddParameter("p_templatepath", invoiceNumber.TEMPLATEPATH);
                objIData.AddParameter("p_status", invoiceNumber.STATUS);
                objIData.AddParameter("p_charonrow", invoiceNumber.CHARONROW);
                objIData.AddParameter("p_recordperpage", invoiceNumber.RECORDPERPAGE);
                objIData.AddParameter("p_headertemplate", invoiceNumber.HEADERTEMPLATE);
                objIData.AddParameter("p_fottertemplate", invoiceNumber.FOTTERTEMPLATE);

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

        public bool DeleteReleaseNotice(long id)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_number_delete");
                objIData.AddParameter("p_id", id);
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


        public List<InvoiceNumberBO> AddNumberWaitingcheck(InvoiceNumberBO invoiceNumber)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_waiting_check_previous_number");
                objIData.AddParameter("p_comtaxcode", invoiceNumber.COMTAXCODE);
                objIData.AddParameter("p_formcodeid", invoiceNumber.FORMCODE);
                objIData.AddParameter("p_symbolcodeid", invoiceNumber.SYMBOLCODE);
                objIData.AddParameter("p_fromnumber", invoiceNumber.FROMNUMBER);
                objIData.AddParameter("p_tonumber", invoiceNumber.TONUMBER);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceNumberBO>();
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

        //public List<InvoiceNumberBO> GetUsingInvoice(InvoiceNumberBO invoiceNumber)
        //{
        //    IData objIData = this.CreateIData();
        //    try
        //    {
        //        BeginTransactionIfAny(objIData);
        //        objIData.CreateNewStoredProcedure("ds_masterdata.pm_number_get_using_invoice_2");
        //        objIData.AddParameter("p_comtaxcode", invoiceNumber.COMTAXCODE);
        //        objIData.AddParameter("p_fromtime", invoiceNumber.FROMTIME);
        //        objIData.AddParameter("p_totime", invoiceNumber.TOTIME);
        //        var reader = objIData.ExecStoreToDataReader();
        //        var list = new List<InvoiceNumberBO>();
        //        ConvertToObject(reader, list);
        //        reader.Close();
        //        CommitTransactionIfAny(objIData);
        //        return list;
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

        public List<InvoiceBO> GetUsingInvoice(InvoiceNumberBO invoiceNumber)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_number_get_using_invoice_2");
                objIData.AddParameter("p_comtaxcode", invoiceNumber.COMTAXCODE);
                objIData.AddParameter("p_fromtime", invoiceNumber.FROMTIME);
                objIData.AddParameter("p_totime", invoiceNumber.TOTIME);
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
        /// Bảng kê hóa đơn giá trị gia tăng
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        public List<OutputInvoice> GetOutputInvoice(InvoiceNumberBO invoiceNumber)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.get_output_invoice");
                objIData.AddParameter("p_comtaxcode", invoiceNumber.COMTAXCODE);
                objIData.AddParameter("p_fromdate", invoiceNumber.FROMTIME);
                objIData.AddParameter("p_todate", invoiceNumber.TOTIME);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<OutputInvoice>();
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

        public List<InvoiceBO> GetOutputInvoiceExcel(InvoiceNumberBO invoiceNumber)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.get_output_invoice_excel");
                objIData.AddParameter("p_comtaxcode", invoiceNumber.COMTAXCODE);
                objIData.AddParameter("p_fromdate", invoiceNumber.FROMTIME);
                objIData.AddParameter("p_todate", invoiceNumber.TOTIME);
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
        /// Bảng kê hóa đơn bán hàng
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        public List<OutputInvoice> GetOutputSellInvoice(InvoiceNumberBO invoiceNumber)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.get_output_sell_invoice");
                objIData.AddParameter("p_comtaxcode", invoiceNumber.COMTAXCODE);
                objIData.AddParameter("p_fromdate", invoiceNumber.FROMTIME);
                objIData.AddParameter("p_todate", invoiceNumber.TOTIME);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<OutputInvoice>();
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
        public List<InvoiceNumberBO> GetNumber(FormSearchNumber form)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_number_get");
                objIData.AddParameter("p_comtaxcode", form.COMTAXCODE);
                objIData.AddParameter("p_formcode", form.FORMCODE);
                objIData.AddParameter("p_symbolcode", form.SYMBOLCODE);
                //objIData.AddParameter("p_pagesize", form.ITEMPERPAGE);
                //objIData.AddParameter("p_offset", form.OFFSET);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceNumberBO>();
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

        public InvoiceNumberBO GetNumberByInvoiceId(long invoiceId)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_number_get_by_invoice_id");
                objIData.AddParameter("p_id", invoiceId);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<InvoiceNumberBO>();
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

        public Int64 GetNumberByInvoiceId2(long invoiceId)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_number_get_by_invoice_id");
                objIData.AddParameter("p_id", invoiceId);
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

        public List<NextNumber> GetNextNumberByInvoiceIdUSB(long invoiceId, long quantity)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_number_get_by_invoice_id_usb");
                objIData.AddParameter("p_id", invoiceId);
                objIData.AddParameter("p_quantity", quantity);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<NextNumber>();
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

        public Int64 GetNextNumberByInvoiceId(string comtaxcode, string formcode, string symbolcode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_number_get_next_number_by_invoice_id");
                objIData.AddParameter("p_comtaxcode", comtaxcode);
                objIData.AddParameter("p_formcode", formcode);
                objIData.AddParameter("p_symbolcode", symbolcode);
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

        // Update current number in pm_number
        public bool UpdatCurrentNumber(long invoiceId, long nextNumber)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_number_currentnumber_update");
                objIData.AddParameter("p_id", invoiceId);
                objIData.AddParameter("p_nextnumber", nextNumber);
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

        public InvoiceNumberBO GetNumberRecord(long invoiceId)
        {
            string queryStr = $"SELECT ds_masterdata.pm_number.id, ds_masterdata.pm_number.issigning FROM ds_masterdata.pm_number" +
                $" JOIN ds_masterdata.pm_invoice ON pm_invoice.formcode = pm_number.formcode AND pm_invoice.symbolcode = pm_number.symbolcode AND pm_invoice.comtaxcode = pm_number.comtaxcode" +
                $" WHERE pm_invoice.id = {invoiceId} AND pm_number.status = 1";
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                var reader = objIData.ExecQueryToDataReader(queryStr);
                List<InvoiceNumberBO> list = new List<InvoiceNumberBO>();
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
        public string SetSigningStatus(long numberId, bool signingStatus)
        {
            string queryStr = $"UPDATE ds_masterdata.pm_number SET issigning = { signingStatus}" +
                $" WHERE pm_number.id = {numberId}";
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.ExecUpdate(queryStr);
                CommitTransactionIfAny(objIData);
                return string.Empty;
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