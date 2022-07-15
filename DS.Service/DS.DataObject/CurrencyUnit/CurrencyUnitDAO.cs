using DS.BusinessObject.CurrencyUnit;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.DataObject.CurrencyUnit
{
    public class CurrencyUnitDAO:BaseDAO
    {
        #region Constructor

        public CurrencyUnitDAO() : base()
        {
        }

        public CurrencyUnitDAO(IData objIData)
            : base(objIData)
        {
        }

        #endregion
        #region Methods
        //lấy ra danh sách tiền thanh toán
        public List<CurrencyUnitBO> GetCurrencyUnit(string keyword, int pagesize, int offset,string comtaxcode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.common_currency_unit_getall");
                objIData.AddParameter("p_keyword", keyword);
                objIData.AddParameter("p_pagesize", pagesize);
                objIData.AddParameter("p_offset", offset);
                objIData.AddParameter("p_comtaxcode", comtaxcode);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<CurrencyUnitBO>();
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
        //Tạo mới hoặc cập nhật tiền thanh toán
        public bool SaveCurrencyUnit(CurrencyUnitBO currency)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.common_currency_unit_add");
                objIData.AddParameter("p_id", currency.ID);
                objIData.AddParameter("p_currencyunit", currency.CURRENCYUNIT);
                objIData.AddParameter("p_comataxcode", currency.COMTAXCODE);
                objIData.AddParameter("p_isactived", currency.ISACTIVED);
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
        //xoá tiền thanh toán
        public string RemoveCurrencyUnit(string id)
        {
            IData objData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objData);
                objData.ExecUpdate($"DELETE FROM ds_masterdata.common_currency_unit WHERE id IN ({id}) AND comtaxcode != '1' AND isactived = false");
                CommitTransactionIfAny(objData);
                return string.Empty;
            }
            catch (Exception ex)
            {
                RollBackTransactionIfAny(objData);
                throw ex;
            }
            finally
            {
                this.DisconnectIData(objData);
            }
        }
        #endregion
    }
}
