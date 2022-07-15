using DS.BusinessLogic.CurrencyUnit;
using DS.BusinessObject.CurrencyUnit;
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
    public class CurrencyUnitController : BaseController
    {
        // GET: CurrencyUnit
        public ActionResult Index()
        {
            return PartialView();
        }
        //lấy ra danh sách tiền thanh toán
        public ActionResult GetCurrencyUnit(string keyword, int pagesize, int currentpage)
        {
            try
            {
                string comtaxcode = objUser.COMTAXCODE;
                int offset = (currentpage - 1) * pagesize;
                CurrencyUnitBLL currencyBLL = new CurrencyUnitBLL();
                List<CurrencyUnitBO> result = currencyBLL.GetAllCurrencyUnit(keyword, pagesize, offset,comtaxcode);
                long TotalPages = 0;
                var TotalRow = result.Count == 0 ? 0 : result[0].TOTALROW;
                if (TotalRow % pagesize == 0)
                    TotalPages = TotalRow == 0 ? 1 : TotalRow / pagesize;
                else
                    TotalPages = TotalRow / pagesize + 1;
                if (currencyBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(currencyBLL.ResultMessageBO.Message, currencyBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetCurrencyUint");
                    return Json(new { rs = false, msg = currencyBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result, TotalPages, TotalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy thông tin tiền thanh toán", ex, MethodBase.GetCurrentMethod().Name, "GetCurrencyUint");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult SaveCurrencyUnit(CurrencyUnitBO currency)
        {
            try
            {
                currency.COMTAXCODE = objUser.COMTAXCODE;
                CurrencyUnitBLL currencyBLL = new CurrencyUnitBLL();
                var result = currencyBLL.SaveCurrencyUnit(currency);
                if (currencyBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(currencyBLL.ResultMessageBO.Message, currencyBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "SaveCurrencyUnit");
                    return Json(new { rs = false, msg = currencyBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lưu thông tin đơn vị tính", ex, MethodBase.GetCurrentMethod().Name, "SaveCurrencyUnit");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }
        //xóa tiền thanh toán
        public ActionResult RemoveCurrencyUnit(string id,int count)
        {
            try
            {
                string[] lstCurrencyid = id.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                CurrencyUnitBLL currencyBLL = new CurrencyUnitBLL();
                List<string> arr = new List<string>();
                for (int i = 0; i < lstCurrencyid.Length; i++)
                {
                    var vVal = CommonFunction.NullSafeInteger(lstCurrencyid[i], 0);
                    arr.Add($"'{vVal}'");
                }
                string ids = string.Join(",", arr);
                string msg = currencyBLL.RemoveCurrencyUnit(ids);
                if (msg.Length > 0)
                    return Json(new { rs = false, msg = msg });
                else
                {
                    return Json(new { rs = true, msg = $"Xóa thành công {count}/{lstCurrencyid.Length} tiền thanh toán." });
                }
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "RemoveCurrencyUnit");
            }

            return Json(new { rs = true, msg = "Xóa thành công tiền thanh toán" });
        }
        public ActionResult ExportExcell(string keyword, int pagesize, int currentpage)
        {
            try
            {
                string comtaxcode = objUser.COMTAXCODE;
                Response.SetCookie(new HttpCookie("fileDownload", "true") { Path = "/" });
                CurrencyUnitBLL categoryBLL = new CurrencyUnitBLL();

                int offset = (currentpage - 1) * pagesize;
                List<CurrencyUnitBO> result = categoryBLL.GetAllCurrencyUnit(keyword, pagesize, offset,comtaxcode);

                //tạo nhãn dữ liệu
                DataTable dtReport = new DataTable();
                dtReport.Columns.Add("MÃ ĐƠN VỊ");
                dtReport.Columns.Add("TÊN ĐƠN VỊ TÍNH");
                dtReport.Columns.Add("TRẠNG THÁI");

                //add dòng dữ liệu vào
                foreach (var row in result)
                {
                    DataRow dr = dtReport.NewRow();
                    dr["MÃ ĐƠN VỊ"] = row.ID;
                    dr["TÊN ĐƠN VỊ TÍNH"] = row.CURRENCYUNIT;
                    if(row.ISACTIVED==true)
                    {
                        dr["TRẠNG THÁI"] = "Hoạt động";
                    }
                    else
                    {
                        dr["TRẠNG THÁI"] = "Ngừng hoạt động";
                    } 
                    dtReport.Rows.Add(dr);
                }
                //đặt tên file xuất ra file theo tên
                string strFileName = "Danh_sach_don_vi_tinh" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm");
                return PushFile(dtReport, strFileName);
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi khi xuất excel danh sách đơn vị tính.", objEx, MethodBase.GetCurrentMethod().Name, "ExportExcell");
                return Json(new { success = false, responseText = "Lỗi khi xuất excel danh sách đơn vị tính." });
            }
        }
    }
}