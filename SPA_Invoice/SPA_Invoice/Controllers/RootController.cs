using DS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace SPA_Invoice.Controllers
{
    public class RootController : BaseController
    {
        public ActionResult GetCategory()
        {
            try
            {
                var result = globalBLL.GetCategory(objUser.COMTAXCODE);
                if (globalBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(globalBLL.ResultMessageBO.Message, globalBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetCategory");
                    return Json(new { rs = false, msg = globalBLL.ResultMessageBO.Message });
                }
                ViewBag.Category = result;
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy danh sách danh mục", ex, MethodBase.GetCurrentMethod().Name, "GetCategory");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetCurrencyUnit()
        {
            try
            {
                var result = globalBLL.GetCurrencyUnit(objUser.COMTAXCODE);
                if (globalBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(globalBLL.ResultMessageBO.Message, globalBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetCurrencyUnit");
                    return Json(new { rs = false, msg = globalBLL.ResultMessageBO.Message });
                }
                ViewBag.Currency = result;
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy danh sách đơn vị tiền tệ", ex, MethodBase.GetCurrentMethod().Name, "GetCurrencyUnit");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetInvoiceStatus()
        {
            try
            {
                var result = globalBLL.GetInvoiceStatus();
                if (globalBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(globalBLL.ResultMessageBO.Message, globalBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetInvoiceStatus");
                    return Json(new { rs = false, msg = globalBLL.ResultMessageBO.Message });
                }
                ViewBag.InvoiceStatus = result;
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy danh sách trạng thái hóa đơn", ex, MethodBase.GetCurrentMethod().Name, "GetInvoiceStatus");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetInvoiceType()
        {
            try
            {
                var result = globalBLL.GetInvoiceType();
                if (globalBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(globalBLL.ResultMessageBO.Message, globalBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetInvoiceType");
                    return Json(new { rs = false, msg = globalBLL.ResultMessageBO.Message });
                }
                ViewBag.InvoiceType = result;
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy danh sách loại hóa đơn", ex, MethodBase.GetCurrentMethod().Name, "GetInvoiceType");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetPaymentMethod()
        {
            try
            {
                var result = globalBLL.GetPaymentMethod(objUser.COMTAXCODE);
                if (globalBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(globalBLL.ResultMessageBO.Message, globalBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetPaymentMethod");
                    return Json(new { rs = false, msg = globalBLL.ResultMessageBO.Message });
                }
                ViewBag.PaymentMethod = result;
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy danh sách hình thức thanh toán", ex, MethodBase.GetCurrentMethod().Name, "GetPaymentMethod");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetPaymentStatus()
        {
            try
            {
                var result = globalBLL.GetPaymentStatus();
                if (globalBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(globalBLL.ResultMessageBO.Message, globalBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetPaymentStatus");
                    return Json(new { rs = false, msg = globalBLL.ResultMessageBO.Message });
                }
                ViewBag.PaymentStatus = result;
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy danh sách trạng thái thanh toán", ex, MethodBase.GetCurrentMethod().Name, "GetPaymentStatus");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetQuantityUnit()
        {
            try
            {
                var result = globalBLL.GetQuantityUnit(objUser.COMTAXCODE);
                if (globalBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(globalBLL.ResultMessageBO.Message, globalBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetQuantityUnit");
                    return Json(new { rs = false, msg = globalBLL.ResultMessageBO.Message });
                }
                ViewBag.QuantityUnit = result;
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy danh sách đơn vị tính", ex, MethodBase.GetCurrentMethod().Name, "GetQuantityUnit");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetFormCode()
        {
            try
            {
                var result = globalBLL.GetFormCode(objUser.COMTAXCODE, objUser.USINGINVOICETYPE);
                if (globalBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(globalBLL.ResultMessageBO.Message, globalBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetFormCode");
                    return Json(new { rs = false, msg = globalBLL.ResultMessageBO.Message });
                }
                var sss = result.GroupBy(item => item.FORMCODE, (key, group) => new { FORMCODE = key, Items = group.ToList() }).ToList();
                

                ViewBag.FormCode = result;
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy danh sách mẫu hóa đơn", ex, MethodBase.GetCurrentMethod().Name, "GetFormCode");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetSymbolCode()
        {
            try
            {
                var result = globalBLL.GetSymbolCode(null, objUser.COMTAXCODE);
                if (globalBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(globalBLL.ResultMessageBO.Message, globalBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetSymbolCode");
                    return Json(new { rs = false, msg = globalBLL.ResultMessageBO.Message });
                }
                ViewBag.SymbolCode = result;
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy danh sách ký hiệu hóa đơn", ex, MethodBase.GetCurrentMethod().Name, "GetSymbolCode");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}