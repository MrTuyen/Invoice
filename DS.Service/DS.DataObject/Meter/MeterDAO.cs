using DS.BusinessObject.Account;
using DS.BusinessObject.Meter;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.DataObject.Meter
{
    public class MeterDAO : BaseDAO
    {
        #region Constructor
        public MeterDAO() : base()
        {
        }

        public MeterDAO(IData objIData)
            : base(objIData)
        {
        }
        #endregion

        #region Function
        public List<MeterBO> GetMeterListByCustaxcode(string custaxcode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.common_meter_get_by_custaxcode");
                objIData.AddParameter("p_custaxcode", custaxcode);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<MeterBO>();
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

        public List<MeterBO> GetMeterListByComtaxcode(string comtaxcode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.common_meter_get_by_comtaxcode");
                objIData.AddParameter("p_comtaxcode", comtaxcode);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<MeterBO>();
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

        public List<MeterBO> GetListMeterCodeByInvoiceID(long invoiceId)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_detail_get_by_invoceid");
                objIData.AddParameter("p_invoiceid", invoiceId);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<MeterBO>();
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
        //Lấy ra danh sách công tơ
        public List<MeterBO> GetMeter(FormSearchMeter form)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.common_meter_get");
                objIData.AddParameter("p_comtaxcode", form.COMTAXCODE);
                objIData.AddParameter("p_keyword", form.KEYWORD);
                objIData.AddParameter("p_pagesize", form.ITEMPERPAGE);
                objIData.AddParameter("p_offset", form.OFFSET);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<MeterBO>();
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
        //Tạo mới hoặc cập nhật sản phẩm
        public string AddCommonMeter(MeterBO meter)
        {
            IData objIData = this.CreateIData();

            string msg = string.Empty;
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.common_meter_add");
                objIData.AddParameter("p_code", meter.CODE);
                objIData.AddParameter("p_metername", meter.METERNAME);
                objIData.AddParameter("p_custaxcode", meter.CUSTAXCODE);
                objIData.AddParameter("p_comtaxcode", meter.COMTAXCODE);
                objIData.AddParameter("p_productcode", meter.PRODUCTCODE);
                objIData.AddParameter("p_factor", meter.FACTOR);
                objIData.AddParameter("p_productcodelist", meter.PRODUCTCODELIST);
                objIData.AddParameter("p_apartmentno", meter.APARTMENTNO); 
                var reader = objIData.ExecNonQuery();
                CommitTransactionIfAny(objIData);
            }
            catch (Exception objEx)
            {
                msg = "Lỗi thêm mới danh mục công tơ.";
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
            return msg;
        }
        public bool UpdateMeter(MeterBO meter)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.common_meter_update");
                objIData.AddParameter("p_id", meter.ID);
                objIData.AddParameter("p_code", meter.CODE);
                objIData.AddParameter("p_namemeter", meter.METERNAME);
                objIData.AddParameter("p_comtaxcode", meter.COMTAXCODE);
                objIData.AddParameter("p_custaxcode", meter.CUSTAXCODE);
                objIData.AddParameter("p_productcode", meter.PRODUCTCODE);
                objIData.AddParameter("p_factor", meter.FACTOR);
                objIData.AddParameter("p_apartmentno", meter.APARTMENTNO);
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
        public string DeleteMeter(string meterIds)
        {
            IData objIData = this.CreateIData();

            string msg = string.Empty;
            try
            {
                BeginTransactionIfAny(objIData);
                //objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_delete_invocie");
                //objIData.AddParameter("p_ids", invoiceIds);
                objIData.ExecUpdate($"UPDATE ds_masterdata.common_meter SET isactive = false WHERE common_meter.code = {meterIds}");
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

        public int CkeckInvoice(string metercode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                var reader = objIData.ExecQueryToString($"SELECT COUNT(*) FROM ds_masterdata.pm_invoice WHERE id IN (SELECT invoiceid FROM ds_masterdata.pm_invoice_detail WHERE metercode = {metercode} AND isdeleted=FALSE) AND active=FALSE");
                CommitTransactionIfAny(objIData);
                return Convert.ToInt32(reader);
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
