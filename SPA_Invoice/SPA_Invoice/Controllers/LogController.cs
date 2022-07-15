using DocumentFormat.OpenXml.Drawing.Charts;
using DS.BusinessLogic.Logs;
using DS.BusinessObject.Logs;
using DS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using DataTable = System.Data.DataTable;

namespace SPA_Invoice.Controllers
{
    public class LogController : BaseController
    {
        // GET: Log
        public ActionResult Index()
        {
            return PartialView();
        }
        //lấy ra danh sách log
        public ActionResult GetLogs(FormSearchLogs form, int currentPage, int itemPerPage)
        {
            try
            {
                form.CURRENTPAGE = currentPage;
                form.ITEMPERPAGE = itemPerPage;
                form.COMTAXCODE = objUser.COMTAXCODE;
                form.OFFSET = (form.CURRENTPAGE - 1) * form.ITEMPERPAGE;

                LogBLL logBLL = new LogBLL();
                List<LogBO> result = logBLL.GetLogs(form);

                long TotalPages = 0;
                var TotalRow = result.Count == 0 ? 0 : result[0].TOTALROW;
                if (TotalRow % itemPerPage == 0)
                    TotalPages = TotalRow == 0 ? 1 : TotalRow / itemPerPage;
                else
                    TotalPages = TotalRow / itemPerPage + 1;

                if (logBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(logBLL.ResultMessageBO.Message, logBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetLog");
                    return Json(new { rs = false, msg = logBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result, TotalPages, TotalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy thông tin người dùng", ex, MethodBase.GetCurrentMethod().Name, "GetLog");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ExportExcell(string keyword, int currentPage, int itemPerPage)
        {
            try
            {
                Response.SetCookie(new HttpCookie("fileDownload", "true") { Path = "/" });
                LogBLL logBLL = new LogBLL();
                FormSearchLogs search = new FormSearchLogs
                {
                    COMTAXCODE = objUser.COMTAXCODE,
                    KEYWORD = keyword,
                    CURRENTPAGE = currentPage,
                    ITEMPERPAGE = itemPerPage,
                    OFFSET = (currentPage - 1) * itemPerPage
                };
                List<LogBO> result = logBLL.GetLogs(search);

                // tạo nhãn dữ liệu
                DataTable dtReport = new DataTable();
                dtReport.Columns.Add("Người dùng");
                dtReport.Columns.Add("Hành động");
                dtReport.Columns.Add("Bản ghi thao tác");
                dtReport.Columns.Add("Đại chỉ IP");
                dtReport.Columns.Add("Nội dung");
                dtReport.Columns.Add("Thời gian");

                //add dòng dữ liệu vào
                foreach (var row in result)
                {
                    DataRow dr = dtReport.NewRow();
                    dr["Người dùng"] = row.USERNAME;
                    dr["Hành động"] = row.ACTIONNAME;
                    dr["Bản ghi thao tác"] = row.OBJECTNAME;
                    dr["Đại chỉ IP"] = row.IPADDRESS;
                    dr["Nội dung"] = row.DESCRIPTION;
                    dr["Thời gian"] = row.LOGTIME;
                    dtReport.Rows.Add(dr);
                }
                // đặt tên file xuất ra file theo tên 
                string strFileName = "Nhat_ky_truy_cap" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm");
                return PushFile(dtReport, strFileName);
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi khi xuất excel danh sách nhật ký truy cập.", objEx, MethodBase.GetCurrentMethod().Name, "ExportExcell");
                return Json(new { success = false, responseText = "Lỗi khi xuất excel danh sách nhật ký truy cập." });
            }
        }
    }
}