using DS.BusinessLogic;
using DS.BusinessObject.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaxReport.Controllers
{
    public class BaseController : Controller
    {
        protected AccountBO objUser = AccountBO.Current.CurrentUser();

        public BaseController()
        {
        }

        public string ErrorMsg { get; protected set; }


        public string GetFullErrorMsg(string strErrorMsg)
        {
            return string.Format("{0} (cs)", strErrorMsg);
        }

        protected string GetErrorMsgByServer(string strErrorMsg)
        {
            return GetFullErrorMsg(strErrorMsg);
        }

    }
}