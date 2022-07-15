using DS.BusinessLogic.PaymentMethod;
using DS.BusinessObject.PaymentMethod;
using DS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace SPA_Invoice.Controllers
{
    public class PaymentMethodController : BaseController
    {
        // GET: PaymentMethod
        public ActionResult Index()
        {
            return PartialView();
        }
        //lấy ra danh sách HTTT
        public ActionResult GetPaymentMethod(string keyword, int pagesize, int currentpage)
        {
            try
            {
                string comtaxcode = objUser.COMTAXCODE;
                int offset = (currentpage - 1) * pagesize;
                PaymentMethodBLL paymentBLL = new PaymentMethodBLL();
                List<PaymentMethodBO> result = paymentBLL.GetAllPaymentMethod(keyword, pagesize, offset,comtaxcode);
                long TotalPages = 0;
                var TotalRow = result.Count == 0 ? 0 : result[0].TOTALROW;
                if (TotalRow % pagesize == 0)
                    TotalPages = TotalRow == 0 ? 1 : TotalRow / pagesize;
                else
                    TotalPages = TotalRow / pagesize + 1;
                if (paymentBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(paymentBLL.ResultMessageBO.Message, paymentBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetPaymentMethod");
                    return Json(new { rs = false, msg = paymentBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result, TotalPages, TotalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy thông tin HTTT", ex, MethodBase.GetCurrentMethod().Name, "GetPaymentMethod");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult SavePaymentMethod(PaymentMethodBO payment)
        {
            try
            {
                payment.COMTAXCODE = objUser.COMTAXCODE;
                PaymentMethodBLL paymentBLL = new PaymentMethodBLL();
                var result = paymentBLL.SavePaymentMethod(payment);
                if (paymentBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(paymentBLL.ResultMessageBO.Message, paymentBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "SavePaymentMethod");
                    return Json(new { rs = false, msg = paymentBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lưu thông tin HTTT", ex, MethodBase.GetCurrentMethod().Name, "SavePaymentMethod");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }
        //xóa HTTT 
        public ActionResult RemovePaymentMethod(string id, int count)
        {
            try
            {
                string[] lstPaymentid = id.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                PaymentMethodBLL paymentBLL = new PaymentMethodBLL();
                List<string> arr = new List<string>();
                for (int i = 0; i < lstPaymentid.Length; i++)
                {
                    var vVal = CommonFunction.NullSafeInteger(lstPaymentid[i], 0);
                    arr.Add($"'{vVal}'");
                }
                string ids = string.Join(",", arr);                
                string msg = paymentBLL.RemovePaymentMethod(ids);
                    if (msg.Length > 0)
                        return Json(new { rs = false, msg = msg });
                    else
                    {
                        return Json(new { rs = true, msg = $"Xóa thành công {count}/{lstPaymentid.Length} HTTT." });
                    }
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "RemovePaymentMethod");
            }

            return Json(new { rs = true, msg = "Xóa thành công HTTT" });
        }
        public ActionResult ExportExcell(string keyword, int pagesize, int currentpage)
        {
            try
            {
                string comtaxcode = objUser.COMTAXCODE;
                Response.SetCookie(new HttpCookie("fileDownload", "true") { Path = "/" });
                PaymentMethodBLL paymentBLL = new PaymentMethodBLL();

                int offset = (currentpage - 1) * pagesize;
                List<PaymentMethodBO> result = paymentBLL.GetAllPaymentMethod(keyword, pagesize, offset,comtaxcode);

                // tạo nhãn dữ liệu
                DataTable dtReport = new DataTable();
                dtReport.Columns.Add("MÃ HTTT");
                dtReport.Columns.Add("TÊN HTTT");
                dtReport.Columns.Add("TRẠNG THÁI");

                //add dòng dữ liệu vào
                foreach (var row in result)
                {
                    DataRow dr = dtReport.NewRow();
                    dr["MÃ HTTT"] = row.ID;
                    dr["TÊN HTTT"] = row.PAYMENTMETHOD;
                    if(row.ISACTIVED==true)
                    {
                        dr["TRẠNG THÁI"] = "Hoạt động";
                    }else
                    {
                        dr["TRẠNG THÁI"] = "Ngừng hoạt động";
                    }
                    dtReport.Rows.Add(dr);
                }
                // đặt tên file xuất ra file theo tên 
                string strFileName = "Danh_sach_HTTT" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm");
                return PushFile(dtReport, strFileName);
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi khi xuất excel danh sách HTTT.", objEx, MethodBase.GetCurrentMethod().Name, "ExportExcell");
                return Json(new { success = false, responseText = "Lỗi khi xuất excel danh sách HTTT." });
            }
        }
    }
}