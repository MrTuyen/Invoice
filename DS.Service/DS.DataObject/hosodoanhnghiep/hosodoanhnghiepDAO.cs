using DS.BusinessObject.hosodoanhnghiep;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.DataObject.hosodoanhnghiep
{
    public class hosodoanhnghiepDAO : BaseDAO
    {
        #region Constructor

        public hosodoanhnghiepDAO() : base()
        {
        }

        public hosodoanhnghiepDAO(IData objIData)
            : base(objIData)
        {
        }

        #endregion

        #region Methods
        public List<hosodoanhnghiepBO> GetHSDN(hosodoanhnghiepBO hsdn)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_hosodoanhnghiep_get");
                objIData.AddParameter("p_keyword", hsdn.KEYWORD);
                objIData.AddParameter("p_pagesize", hsdn.ITEMPERPAGE);
                objIData.AddParameter("p_offset", hsdn.OFFSET);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<hosodoanhnghiepBO>();
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
        public bool AddHSDN(hosodoanhnghiepBO item)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_hosodoanhnghiep_add");
                objIData.AddParameter("p_comname", item.COMNAME);
                objIData.AddParameter("p_comtaxcode", item.COMTAXCODE);
                objIData.AddParameter("p_comaddress", item.COMADDRESS);
                objIData.AddParameter("p_comlegal", item.COMLEGAL);
                objIData.AddParameter("p_comphone", item.COMPHONE);
                objIData.AddParameter("p_comactivetime", item.COMACTIVETIME);
                objIData.AddParameter("p_comactivitytime", item.COMACTIVITYTIME);
                objIData.AddParameter("p_status", item.STATUS);
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