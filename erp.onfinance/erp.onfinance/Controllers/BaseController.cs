using DS.BusinessObject.Account;
using DS.Common;
using System.Web.Mvc;

namespace erp.onfinance.Controllers
{
    public class BaseController : Controller
    {
        protected AccountBO objUser = AccountBO.Current.CurrentUser();
        public string ErrorMsg { get; protected set; }


        public string GetFullErrorMsg(string strErrorMsg)
        {
            return string.Format("{0} (cs)", strErrorMsg);
        }

        protected string GetErrorMsgByServer(string strErrorMsg)
        {
            return GetFullErrorMsg(strErrorMsg);
        }

        protected string GetErrorMsgAndITSupport(string strErrorMsg)
        {
            return string.Format("{0}{1}", strErrorMsg, Constants.SUFFIX_IT_SUPPORT);
        }
    }
}