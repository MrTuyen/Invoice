using SAB.Library.Core.FileService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessLogic
{
    public class BaseBLL
    {
        #region Fields
        protected ResultMessageBO objResultMessageBO = new ResultMessageBO();
        #endregion

        #region Properties

        public ResultMessageBO ResultMessageBO
        {
            get { return objResultMessageBO; }
            set { objResultMessageBO = value; }
        }

        public string ErrorMsg { get; protected set; }

        public string NameSpace
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Namespace; }
        }
        #endregion

        #region Method

        #endregion
    }
}
