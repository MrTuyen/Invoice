using DS.BusinessObject.QuantityUnit;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.DataObject.QuantityUnit
{
    public class QuantityUnitDAO : BaseDAO
    {
        #region Constructor

        public QuantityUnitDAO() : base()
        {
        }

        public QuantityUnitDAO(IData objIData)
            : base(objIData)
        {
        }

        #endregion
        #region Methods
        //lấy ra danh sách đơn vị tính
        public List<QuantityUnitBO> GetQuantityUnit(string keyword, int pagesize, int offset,string comtaxcode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.common_quantity_unit_get");
                objIData.AddParameter("p_keyword", keyword);
                objIData.AddParameter("p_pagesize", pagesize);
                objIData.AddParameter("p_offset", offset);
                objIData.AddParameter("p_comtaxcode", comtaxcode);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<QuantityUnitBO>();
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
        //Tạo mới hoặc cập nhật đơn vị tính
        public bool SaveQuantityUnit(QuantityUnitBO quantity)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.common_quantity_unit_add");
                objIData.AddParameter("p_id", quantity.ID);
                objIData.AddParameter("p_quantityunit", quantity.QUANTITYUNIT);
                objIData.AddParameter("p_comtaxcode", quantity.COMTAXCODE);
                objIData.AddParameter("p_isactived", quantity.ISACTIVED);
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
        //xóa đơn vị tính
        public string RemoveQuantityUnit(string id) 
        {
            IData objData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objData);
                objData.ExecQueryToString($"DELETE FROM ds_masterdata.common_quantity_unit WHERE id IN ({id}) AND comtaxcode != '1' AND isactived = false");
                CommitTransactionIfAny(objData);
                return string.Empty;
            }catch(Exception ex)
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
