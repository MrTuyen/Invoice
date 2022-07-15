using DS.BusinessLogic;
using DS.BusinessLogic.Category;
using DS.BusinessObject;
using DS.BusinessObject.Category;
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
    public class CategoryController : BaseController
    {
        public ActionResult Index()
        {
            return PartialView();
        }

        // GET: Category
        //lấy ra danh sách danh mục
        public ActionResult GetCategory(string keyword, int pagesize,int currentpage)
        {
            try
            {
                string comtaxcode = objUser.COMTAXCODE;
                int offset = (currentpage - 1) * pagesize;
                CategoryBLL categoryBLL = new CategoryBLL();
                List<CategoryBO> result = categoryBLL.GetAllCategory(keyword, pagesize, offset, comtaxcode);
                long TotalPages = 0;
                var TotalRow = result.Count == 0 ? 0 : result[0].TOTALROW;
                if (TotalRow % pagesize == 0)
                    TotalPages = TotalRow == 0 ? 1 : TotalRow / pagesize;
                else
                    TotalPages = TotalRow / pagesize + 1;
                if (categoryBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(categoryBLL.ResultMessageBO.Message, categoryBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetCategory");
                    return Json(new { rs = false, msg = categoryBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result,TotalPages,TotalRow}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy thông tin khách hàng", ex, MethodBase.GetCurrentMethod().Name, "GetCategory");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult SaveCategory(CategoryBO category)
        {
            try
            {
                category.COMTAXCODE = objUser.COMTAXCODE;
                CategoryBLL categoryBLL = new CategoryBLL();
                var result = categoryBLL.SaveProduct(category);
                if (categoryBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(categoryBLL.ResultMessageBO.Message, categoryBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "SaveCategory");
                    return Json(new { rs = false, msg = categoryBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Tên danh mục bị trùng vui lòng thử với tên khác", ex, MethodBase.GetCurrentMethod().Name, "SaveCategory");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }
        //xóa dịch vụ
        public ActionResult RemoveCategory(string id,int count)
        {
            try
            {
                string[] lstCategoryid = id.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                CategoryBLL categoryBLL = new CategoryBLL();
                List<string> arr = new List<string>();
                List<string> arrActive = new List<string>();
                for (int i = 0; i < lstCategoryid.Length; i++)
                {
                    var vVal = CommonFunction.NullSafeInteger(lstCategoryid[i], 0);
                    arr.Add($"'{vVal}'");
                }
                string ids = string.Join(",", arr);
                string msg = categoryBLL.RemoveCategory(ids);
                if (msg.Length > 0)
                    return Json(new { rs = false, msg = msg });
                else
                {
                    return Json(new { rs = true, msg = $"Xóa thành công {count}/{lstCategoryid.Length} dịch vụ." });
                }
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "RemoveCategory");
            }

            return Json(new { rs = true, msg = "Xóa thành công dịch vụ" });
        }
        public ActionResult ExportExcell(string keyword, int pagesize, int currentpage)
        {
            try
            {
                string comtaxcode = objUser.COMTAXCODE;
                Response.SetCookie(new HttpCookie("fileDownload", "true") { Path = "/" });
                CategoryBLL categoryBLL = new CategoryBLL();

                int offset = (currentpage - 1) * pagesize;
                List<CategoryBO> result = categoryBLL.GetAllCategory(keyword,pagesize,offset,comtaxcode);

                // tạo nhãn dữ liệu
                DataTable dtReport = new DataTable();
                dtReport.Columns.Add("MÃ DANH MỤC");
                dtReport.Columns.Add("TÊN DANH MỤC");
                dtReport.Columns.Add("TRẠNG THÁI");

                //add dòng dữ liệu vào
                foreach (var row in result)
                {
                    DataRow dr = dtReport.NewRow();
                    dr["MÃ DANH MỤC"] = row.ID;
                    dr["TÊN DANH MỤC"] = row.CATEGORY;
                    if(row.ISACTIVE==true)
                    {
                        dr["TRẠNG THÁI"] = "Hoạt động";
                    }else
                    {
                        dr["TRẠNG THÁI"] = "Ngừng hoạt động";
                    }
                    dtReport.Rows.Add(dr);
                }
                // đặt tên file xuất ra file theo tên 
                string strFileName = "Danh_sach_danh_muc" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm");
                return PushFile(dtReport, strFileName);
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi khi xuất excel danh sách danh mục.", objEx, MethodBase.GetCurrentMethod().Name, "ExportExcell");
                return Json(new { success = false, responseText = "Lỗi khi xuất excel danh sách danh mục." });
            }
        }
    }
}