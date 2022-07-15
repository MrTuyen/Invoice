using DS.BusinessObject.API;
using DS.Common.Helpers;
using DS.DataObject.API;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessLogic.API
{
    public class PartnerOAuthBLL : BaseBLL
    {
        #region Fields
        protected IData objDataAccess = null;

        #endregion

        #region Properties

        #endregion

        #region Constructor
        public PartnerOAuthBLL()
        {
        }

        public PartnerOAuthBLL(IData objIData)
        {
            objDataAccess = objIData;
        }
        #endregion

        #region methods

        public List<PartnerOAuthBO> GetPartner(PartnerOAuthBO partnerOAuthBO)
        {
            try
            {
                PartnerOAuthDAO partnerOAuthDAO = new PartnerOAuthDAO();
                return partnerOAuthDAO.GetPartner(partnerOAuthBO);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách đối tác");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }
        #endregion
    }
}
