using DS.BusinessLogic.QuantityUnit;
using DS.BusinessObject.QuantityUnit;
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
    public class QuantityUnitController : BaseController
    {
        // GET: QuantityUnit
        public ActionResult Index()
        {
            return PartialView();
        }
        public ActionResult GetQuantityUnit(string keyword, int pagesize, int currentpage)
        {
            try
            {
                string comtaxcode = objUser.COMTAXCODE;
                int offset = (currentpage - 1) * pagesize;
                QuantityUnitBLL quantityBLL = new QuantityUnitBLL();
                List<QuantityUnitBO> result = quantityBLL.GetAllQuantityUnit(keyword, pagesize, offset,comtaxcode);
                long TotalPages = 0;
                var TotalRow = result.Count == 0 ? 0 : result[0].TOTALROW;
                if (TotalRow % pagesize == 0)
                    TotalPages = TotalRow == 0 ? 1 : TotalRow / pagesize;
                else
                    TotalPages = TotalRow / pagesize + 1;
                if (quantityBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(quantityBLL.ResultMessageBO.Message, quantityBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetQuantityUnit");
                    return Json(new { rs = false, msg = quantityBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result, TotalPages, TotalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy thông tin đơn vị tính", ex, MethodBase.GetCurrentMethod().Name, "GetQuantityUnit");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult SaveQuantityUnit(QuantityUnitBO quantityunit)
        {
            try
            {
                quantityunit.COMTAXCODE = objUser.COMTAXCODE;
                QuantityUnitBLL quantityBLL = new QuantityUnitBLL();
                var result = quantityBLL.SaveQuantityUnit(quantityunit);
                if (quantityBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(quantityBLL.ResultMessageBO.Message, quantityBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "SaveQuantityUnit");
                    return Json(new { rs = false, msg = quantityBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lưu thông tin đơn vị tính", ex, MethodBase.GetCurrentMethod().Name, "SaveQuantityUnit");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }
        //xóa đơn vị tính
        public ActionResult RemoveQuantityUnit(string id, string count)
        {
            try
            {
                string[] lstQuantityid = id.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                QuantityUnitBLL quantityBLL = new QuantityUnitBLL();
                List<string> arr = new List<string>();
                for (int i = 0; i < lstQuantityid.Length; i++)
                {
                    var vVal = CommonFunction.NullSafeInteger(lstQuantityid[i], 0);
                    arr.Add($"'{vVal}'");
                }
                string ids = string.Join(",", arr);
                string msg = quantityBLL.RemoveQuantityUnit(ids);
                if (msg.Length > 0)
                    return Json(new { rs = false, msg = msg });
                else
                {
                    return Json(new { rs = true, msg = $"Xóa thành công {count}/{lstQuantityid.Length} đơn vị tính." });
                }
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "RemoveQuantityUnit");
            }

            return Json(new { rs = true, msg = "Xóa thành công đơn vị tính" });
        }
        public ActionResult ExportExcell(string keyword, int pagesize, int currentpage)
        {
            try
            {
                string comtaxcode = objUser.COMTAXCODE;
                Response.SetCookie(new HttpCookie("fileDownload", "true") { Path = "/" });
                QuantityUnitBLL currencyBLL = new QuantityUnitBLL();

                int offset = (currentpage - 1) * pagesize;
                List<QuantityUnitBO> result = currencyBLL.GetAllQuantityUnit(keyword, pagesize, offset,comtaxcode);

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
                    dr["TÊN ĐƠN VỊ TÍNH"] = row.QUANTITYUNIT;
                    if(row.ISACTIVED==true)
                    {
                        dr["TRẠNG THÁI"] = "Hoạt động";
                    }else
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