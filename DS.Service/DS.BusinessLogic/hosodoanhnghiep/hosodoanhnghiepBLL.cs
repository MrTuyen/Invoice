using DS.BusinessObject.hosodoanhnghiep;
using DS.Common.Helpers;
using DS.DataObject.hosodoanhnghiep;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessLogic.hosodoanhnghiep
{
    public class hosodoanhnghiepBLL : BaseBLL
    {
        #region Fields
        protected IData objDataAccess = null;

        #endregion

        #region Properties

        #endregion

        #region Constructor
        public hosodoanhnghiepBLL()
        {
        }

        public hosodoanhnghiepBLL(IData objIData)
        {
            objDataAccess = objIData;
        }
        #endregion

        #region Methods

        public List<hosodoanhnghiepBO> GetHSDN(hosodoanhnghiepBO hsdn)
        {
            try
            {
                hosodoanhnghiepDAO hosodoanhnghiepDAO = new hosodoanhnghiepDAO();
                return hosodoanhnghiepDAO.GetHSDN(hsdn);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi thêm doanh nghiệp hosodoanhnghiep.vn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                throw objEx;
            }
        }


        public bool AddHSDN(hosodoanhnghiepBO invoiceNumber)
        {
            try
            {
                hosodoanhnghiepDAO hosodoanhnghiepDAO = new hosodoanhnghiepDAO();
                return hosodoanhnghiepDAO.AddHSDN(invoiceNumber);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi thêm doanh nghiệp hosodoanhnghiep.vn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                throw objEx;
            }
        }

        #endregion
    }
}
