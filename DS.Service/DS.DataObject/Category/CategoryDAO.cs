using DS.BusinessObject.Account;
using DS.BusinessObject.Category;
using DS.BusinessObject.Invoice;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.DataObject.Category
{
    public class CategoryDAO : BaseDAO
    {
        #region Constructor

        public CategoryDAO() : base()
        {
        }

        public CategoryDAO(IData objIData)
            : base(objIData)
        {
        }

        #endregion
        //lấy ra danh sách danh mục
        #region Methods
        public List<CategoryBO> GetAllCategory(string keyword,int pagesize,int offset,string Comtaxcode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.common_category_getall");
                objIData.AddParameter("p_keyword",keyword );
                objIData.AddParameter("p_pagesize", pagesize);
                objIData.AddParameter("p_offset", offset);
                objIData.AddParameter("p_comataxcode", Comtaxcode);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<CategoryBO>();
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
        //Tạo mới hoặc cập nhật danh mục

        public bool SaveCategory(CategoryBO category)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.common_category_add");
                objIData.AddParameter("p_id", category.ID);
                objIData.AddParameter("p_categoryname", category.CATEGORY);
                objIData.AddParameter("p_comtaxcode", category.COMTAXCODE);
                objIData.AddParameter("p_isactived", category.ISACTIVE);
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

        public bool SaveQuantityUnit(string quantityunit, long ID, string comtaxcode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.common_quantity_unit_add");
                objIData.AddParameter("p_quantityunit", quantityunit);
                objIData.AddParameter("p_comtaxcode", comtaxcode);
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
        //xóa dịch vụ
        public string RemoveCategory(string id)
        {
            IData objData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objData);
                objData.ExecQueryToString($"DELETE FROM ds_masterdata.common_category WHERE id IN ({id}) AND comtaxcode != '1' AND isactive = false");
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