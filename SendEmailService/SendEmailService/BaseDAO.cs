using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendEmailService
{
    public class BaseDAO
    {
        #region Fields

        protected IData objDataAccess = null;

        #endregion

        #region Properties

        protected IData DataAccess
        {
            get { return objDataAccess; }
            set { objDataAccess = value; }
        }

        #endregion

        #region Constructor

        protected BaseDAO()
        { }

        protected BaseDAO(IData objIData)
        {
            objDataAccess = objIData;
        }

        #endregion

        #region Methods

        protected IData CreateIData()
        {
            IData objIData;
            if (objDataAccess == null)
            {
                objIData = Data.CreateData(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString, false);
            }
            else
                objIData = objDataAccess;
            return objIData;
        }

        protected void DisconnectIData(IData objIData)
        {
            if (objDataAccess == null)
                objIData.Disconnect();
        }

        protected void BeginTransactionIfAny(IData objIData)
        {
            if (objDataAccess == null)
                objIData.BeginTransaction();
            //else
            //    objDataAccess.Connect();
        }

        protected void CommitTransactionIfAny(IData objIData)
        {
            if (objDataAccess == null)
                objIData.CommitTransaction();
        }

        protected void RollBackTransactionIfAny(IData objIData)
        {
            if (objDataAccess == null)
                objIData.RollBackTransaction();
        }

        protected void ConvertToObject(IDataReader reader, dynamic lstObj)
        {
            MethodHelper objMethodHelper = new MethodHelper();
            objMethodHelper.ConvertToObject(reader, lstObj);
        }
        #endregion
    }
}
