using DS.BusinessObject.API;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.DataObject.API
{
    public class PartnerOAuthDAO : BaseDAO
    {
        #region Constructor

        public PartnerOAuthDAO() : base()
        {
        }

        public PartnerOAuthDAO(IData objIData)
            : base(objIData)
        {
        }

        #endregion

        #region Methods
        //Tạo mới hoặc cập nhật danh mục
        public List<PartnerOAuthBO> GetPartner(PartnerOAuthBO partner)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.system_partner_get");
                objIData.AddParameter("p_partnerid", partner.PARTNERID);
                objIData.AddParameter("p_status", partner.ISACTIVED);
                objIData.AddParameter("p_username", partner.CREATEDBYUSER);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<PartnerOAuthBO>();
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

        #endregion
    }
}