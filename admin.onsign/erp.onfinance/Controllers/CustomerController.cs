using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace erp.onfinance.Controllers
{
    public class CustomerController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}