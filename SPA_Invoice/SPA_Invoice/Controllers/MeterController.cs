using DS.BusinessLogic.Meter;
using DS.BusinessObject.Invoice;
using DS.BusinessObject.Meter;
using DS.BusinessObject.Output;
using DS.Common.Enums;
using DS.Common.Helpers;
using DS.DataObject.Meter;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace SPA_Invoice.Controllers
{
    public class MeterController : BaseController
    {
        // GET: Meter
        public ActionResult Index()
        {
            return PartialView();
        }

        /// <summary>
        /// truongnv 20200318
        /// Lấy danh sách bộ chỉ số công tơ điện
        /// </summary>
        /// <param name="custaxcode"></param>
        /// <returns></returns>
        public JsonResult GetMeterListByCustaxcode(string custaxcode)
        {
            try
            {
                MeterBLL oBL = new MeterBLL();
                List<MeterBO> obj = oBL.GetMeterListByCustaxcode(custaxcode);
                if (obj != null)
                    return Json(new { rs = true, msg = obj });
            }
            catch
            {
                return Json(new { rs = false, msg = "Không lấy được thông tin bộ chỉ số công tơ" });
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// truongnv 20200318
        /// Lấy danh sách bộ chỉ số công tơ điện
        /// </summary>
        /// <param name="comtaxcode"></param>
        /// <returns></returns>
        public JsonResult GetMeterListByComtaxcode(string comtaxcode)
        {
            try
            {
                MeterBLL oBL = new MeterBLL();
                if (string.IsNullOrEmpty(comtaxcode)) comtaxcode = objUser.COMTAXCODE;

                List<MeterBO> obj = oBL.GetMeterListByComtaxcode(comtaxcode);
                if (obj != null)
                    return Json(new { rs = true, msg = obj });
            }
            catch
            {
                return Json(new { rs = false, msg = "Không lấy được thông tin bộ chỉ số công tơ" });
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// truongnv 20200318
        /// Lấy danh sách bộ chỉ số công tơ điện
        /// </summary>
        /// <param name="comtaxcode"></param>
        /// <returns></returns>
        public JsonResult GetListMeterCodeByInvoiceID(long invoiceId)
        {
            try
            {
                MeterBLL oBL = new MeterBLL();
                List<MeterBO> obj = oBL.GetListMeterCodeByInvoiceID(invoiceId);
                if (obj != null)
                    return Json(new { rs = true, msg = obj });
            }
            catch
            {
                return Json(new { rs = false, msg = "Không lấy được thông tin bộ chỉ số công tơ" });
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMeter(FormSearchMeter form, int currentPage, int itemPerPage)
        {
            try
            {
                form.CURRENTPAGE = currentPage;
                form.ITEMPERPAGE = itemPerPage;
                form.COMTAXCODE = objUser.COMTAXCODE;
                form.OFFSET = (form.CURRENTPAGE - 1) * form.ITEMPERPAGE;

                MeterBLL MeterBLL = new MeterBLL();
                List<MeterBO> result = MeterBLL.GetMeter(form);

                long TotalPages = 0;
                var TotalRow = result.Count == 0 ? 0 : result[0].TOTALROW;
                if (TotalRow % itemPerPage == 0)
                    TotalPages = TotalRow == 0 ? 1 : TotalRow / itemPerPage;
                else
                    TotalPages = TotalRow / itemPerPage + 1;

                if (MeterBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(MeterBLL.ResultMessageBO.Message, MeterBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetMeter");
                    return Json(new { rs = false, msg = MeterBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result, TotalPages, TotalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy thông tin công tơ", ex, MethodBase.GetCurrentMethod().Name, "GetMeter");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AddMeter(MeterBO meter)
        {
            string msg = string.Empty;
            try
            {
                if (meter.PRODUCTCODELIST != null)
                {
                    var arrProductCode = meter.PRODUCTCODELIST.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (string.IsNullOrWhiteSpace(meter.CODE))
                        meter.CODE = CommonFunction.GenerateCode();
                    foreach (var item in arrProductCode)
                    {
                        meter.COMTAXCODE = objUser.COMTAXCODE;
                        meter.PRODUCTCODE = item;
                        MeterBLL oBL = new MeterBLL();
                        msg = oBL.AddCommonMeter(meter);
                        if (msg.Length > 0)
                        {
                            return Json(new { rs = false, msg = msg });
                        }
                    }
                    return Json(new { rs = true, msg = "Thêm mới thành công." }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { rs = false, msg = msg });
            }
            catch(Exception ex)
            {
                ConfigHelper.Instance.WriteLog($"Thêm mới không thành công.", ex, MethodBase.GetCurrentMethod().Name, "AddMeter");
                return Json(new { rs = false, msg = $"Thêm mới không thành công." }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult UpdateMeter(MeterBO meter)
        {
            try
            {
                meter.COMTAXCODE = objUser.COMTAXCODE;
                MeterBLL productBLL = new MeterBLL();
                var result = productBLL.UpdateMeter(meter);
                if (productBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(productBLL.ResultMessageBO.Message, productBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "UpdateProduct");
                    return Json(new { rs = false, msg = productBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, meter }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi cập nhật công tơ", ex, MethodBase.GetCurrentMethod().Name, "UpdateMeter");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult RemoveMeter(string metercode)
        {
            try
            {
                string[] lstMetercode = metercode.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                MeterBLL meterBLL = new MeterBLL();

                List<string> arr = new List<string>();
                List<string> arr1 = new List<string>();
                
                for (int i = 0; i < lstMetercode.Length; i++)
                {
                    var vVal = CommonFunction.NullSafeString(lstMetercode[i], "");
                    arr1.Add($"'{vVal}'");
                }
                int countcode = 0;
                foreach (var i in arr1)
                {
                    countcode = meterBLL.CheckInvoice(i);
                    
                    if(countcode == 0)
                    {
                        string msg = meterBLL.DeleteMeter(i);
                        arr.Add(i);
                    }
                    else
                    {
                        countcode = countcode ++;
                    }
                }
              
                if(countcode>0)
                {
                    return Json(new { rs = false, msg = $"Công tơ này đang được sử dụng bạn không được xóa " });
                }
                else
                {
                    return Json(new { rs = true, msg = $"Xóa thành công {arr.Count}/{arr1.Count} công tơ." });
                }
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "RemoveMeter");
            }

            return Json(new { rs = true, msg = "Xóa thành công công tơ" });
        }
        public ActionResult SaveMeterList(List<MeterBO> meters)
        {
            try
            {
                meters.ForEach(x => x.COMTAXCODE = objUser.COMTAXCODE);
                MeterBLL oBL = new MeterBLL();
                var result = oBL.ImportMeter(meters);
                if (oBL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(oBL.ResultMessageBO.Message, oBL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "SaveMeterList");
                    return Json(new { rs = false, msg = oBL.ResultMessageBO.Message });
                }
                if (result)
                    return Json(new { rs = true }, JsonRequestBehavior.AllowGet);
                else return Json(new { rs = false, msg = "Lối lưu danh sách công tơ." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lưu công tơ từ file excel", ex, MethodBase.GetCurrentMethod().Name, "SaveMeterList");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ExportExcell(string keyword, int currentPage, int itemPerPage)
        {
            try
            {
                Response.SetCookie(new HttpCookie("fileDownload", "true") { Path = "/" });
                MeterBLL productBLL = new MeterBLL();
                FormSearchMeter search = new FormSearchMeter
                {
                    COMTAXCODE = objUser.COMTAXCODE,
                    KEYWORD = keyword,
                    CURRENTPAGE = currentPage,
                    ITEMPERPAGE = itemPerPage,
                    OFFSET = (currentPage - 1) * itemPerPage
                };
                List<MeterBO> result = productBLL.GetMeter(search);

                // tạo nhãn dữ liệu
                DataTable dtReport = new DataTable();
                dtReport.Columns.Add("MÃ CÔNG TƠ");
                dtReport.Columns.Add("TÊN CÔNG TƠ");
                dtReport.Columns.Add("MST DOANH NGHIỆP");
                dtReport.Columns.Add("MST CÁ NHÂN");
                dtReport.Columns.Add("BIỂU GIÁ ĐIỆN");
                dtReport.Columns.Add("HỆ SỐ NHÂN");
                dtReport.Columns.Add("SỐ HỘ");

                //add dòng dữ liệu vào
                foreach (var row in result)
                {
                    DataRow dr = dtReport.NewRow();
                    dr["MÃ CÔNG TƠ"] = row.CODE;
                    dr["TÊN CÔNG TƠ"] = row.METERNAME;
                    dr["MST DOANH NGHIỆP"] = row.COMTAXCODE;
                    dr["MST CÁ NHÂN"] = row.CUSTAXCODE;
                    dr["BIỂU GIÁ ĐIỆN"] = row.PRODUCTCODELIST;
                    dr["HỆ SỐ NHÂN"] = row.FACTOR;
                    dr["SỐ HỘ"] = row.APARTMENTNO;
                    dtReport.Rows.Add(dr);
                }
                // đặt tên file xuất ra file theo tên 
                string strFileName = "Danh_sach_cong_to" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm");
                return PushFile(dtReport, strFileName);
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi khi xuất excel danh sách công tơ.", objEx, MethodBase.GetCurrentMethod().Name, "ExportExcell");
                return Json(new { success = false, responseText = "Lỗi khi xuất excel danh sách sản phẩm." });
            }
        }
        public ActionResult ImportMeterFromExcel(FormCollection collection)
        {
            try
            {
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        Stream stream = fileContent.InputStream;
                        using (var package = new ExcelPackage(stream))
                        {
                            var workBook = package.Workbook;
                            if (workBook != null)
                            {
                                if (workBook.Worksheets.Count > 0)
                                {
                                    Regex regex = new Regex(@"^\d$");

                                    var sheet = workBook.Worksheets[1];
                                    List<ImportMeterTitleBO> lstColumn = null;
                                    if (objUser.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTIENDIEN)
                                    {
                                        lstColumn = new List<ImportMeterTitleBO>
                                        {
                                            new ImportMeterTitleBO { MYFIELD = "Mã khách/Mã số thuế" },
                                            new ImportMeterTitleBO { MYFIELD = "Mã công tơ" },
                                            new ImportMeterTitleBO { MYFIELD = "Tên công tơ" },
                                            new ImportMeterTitleBO { MYFIELD = "Hệ số" },
                                            new ImportMeterTitleBO { MYFIELD = "Mã giờ" },
                                            new ImportMeterTitleBO { MYFIELD = "Số hộ" },
                                        };
                                    }
                                    else
                                    {
                                        lstColumn = new List<ImportMeterTitleBO>
                                        {
                                            new ImportMeterTitleBO { MYFIELD = "Mã khách/Mã số thuế" },
                                            new ImportMeterTitleBO { MYFIELD = "Mã công tơ" },
                                            new ImportMeterTitleBO { MYFIELD = "Tên công tơ" },
                                            new ImportMeterTitleBO { MYFIELD = "Hệ số" },
                                            new ImportMeterTitleBO { MYFIELD = "Mã giờ" },
                                            new ImportMeterTitleBO { MYFIELD = "Số hộ" },
                                        };
                                    }

                                    for (var columnNum = 1; columnNum <= sheet.Dimension.End.Column; columnNum++)
                                    {
                                        if (sheet.Cells[1, columnNum].Value == null || sheet.Cells[1, columnNum].Value.ToString().Trim() == "")
                                            continue;
                                        try
                                        {
                                            if (lstColumn[columnNum - 1] == null)
                                                lstColumn.Add(new ImportMeterTitleBO());
                                        }
                                        catch
                                        {
                                            lstColumn.Add(new ImportMeterTitleBO());
                                        }
                                        lstColumn[columnNum - 1].YOURFIELD = sheet.Cells[1, columnNum].Value.ToString();
                                        if (lstColumn[columnNum - 1].MYFIELD.ToLower() != lstColumn[columnNum - 1].YOURFIELD.ToLower())
                                            lstColumn[columnNum - 1].ISDIFFERENT = true;
                                    }
                                    int countDiffent = lstColumn.Where(x => x.ISDIFFERENT).Count();
                                    if (countDiffent > 0)
                                        return Json(new { rs = true, isDiffent = true, data = lstColumn });

                                    List<ImportMeterBO> importMeters = new List<ImportMeterBO>();
                                    for (var rowNum = 2; rowNum <= sheet.Dimension.End.Row; rowNum++)
                                    {
                                        ImportMeterBO importMeter = new ImportMeterBO();
                                        if (sheet.Cells[rowNum, 1].Value == null || string.IsNullOrEmpty(sheet.Cells[rowNum, 1].Value.ToString().Trim()))
                                            continue;
                                        string code = sheet.Cells[rowNum, 2].Value == null ? "" : sheet.Cells[rowNum, 2].Value.ToString().Trim();
                                        string name = sheet.Cells[rowNum, 3].Value == null ? "" : sheet.Cells[rowNum, 3].Value.ToString().Trim();
                                        if (string.IsNullOrWhiteSpace(name))
                                            name = code;
                                        importMeter.CUSTAXCODE = sheet.Cells[rowNum, 1].Value == null ? "" : sheet.Cells[rowNum, 1].Value.ToString().Trim();
                                        importMeter.CODE = code;
                                        importMeter.METERNAME = name;
                                        importMeter.FACTOR = CommonFunction.NullSafeInteger(sheet.Cells[rowNum, 4].Value == null ? "" : sheet.Cells[rowNum, 4].Value.ToString().Trim(), 0);
                                        importMeter.PRODUCTCODE = sheet.Cells[rowNum, 5].Value == null ? "" : sheet.Cells[rowNum, 5].Value.ToString().Trim();
                                        importMeter.APARTMENTNO = CommonFunction.NullSafeInteger(sheet.Cells[rowNum, 6].Value == null ? "" : sheet.Cells[rowNum, 6].Value.ToString().Trim(), 0);

                                        importMeters.Add(importMeter);
                                    }

                                    return Json(new { rs = true, isDiffent = false, data = lstColumn, importMeters });
                                }
                            }
                        }
                    }
                }
                return Json(new { rs = false, reponseText = "File không đúng chuẩn, vui lòng tải file Excel mẫu kế bên" });
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi đọc file excel", objEx, MethodBase.GetCurrentMethod().Name, "ImportProductFromExcel");
                return Json(new { rs = false, reponseText = "File không đúng chuẩn, vui lòng tải file Excel mẫu kế bên." });
            }
        }

        #region Mapping meter

        /// <summary>
        /// tuyennv 20200706
        /// Đọc dữ liệu file excel lấy ra thông tin các sheet trong file
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public ActionResult ReadFileExcel(FormCollection collection)
        {
            try
            {
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        string path = Server.MapPath("~/Uploads/");
                        string extension = Path.GetExtension(path + fileContent.FileName);
                        if (extension != ".xls" && extension != ".xlsx") return Json(new { rs = false, msg = "File không đúng định dạng, Hỗ trợ định dạng .xls và .xlsx." });

                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        foreach (string key in Request.Files)
                        {
                            HttpPostedFileBase postedFile = Request.Files[key];
                            postedFile.SaveAs(path + fileContent.FileName);
                        }
                        string fullPath = path + fileContent.FileName;
                        CacheHelper.Set("FullPath", fullPath);
                        List<ExWorksheet> objSheets;
                        string msg = ExcelHelper.GetAllWorksheets(fullPath, out objSheets);
                        if (msg.Length > 0) return Json(new { rs = false, msg = msg });
                        return Json(new { rs = true, fileName = fileContent.FileName, listSheet = JsonConvert.SerializeObject(objSheets), msg = "Tệp dữ liệu đã tải lên thành công." }, JsonRequestBehavior.AllowGet);
                    }
                }
                ConfigHelper.Instance.WriteLog($"File không đúng định dạng theo quy định mẫu cung cấp.", string.Empty, MethodBase.GetCurrentMethod().Name, "ReadFileExcel");
                return Json(new { rs = false, msg = "File không đúng định dạng theo quy định mẫu cung cấp." });
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi đọc file excel", objEx, MethodBase.GetCurrentMethod().Name, "ReadFileExcel");
                return Json(new { rs = false, msg = "Lỗi đọc file excel." });
            }
        }

        /// <summary>
        /// tuyennv 20200706
        /// Thực hiện ghép cột dữ liệu của kh vs onfinance
        /// </summary>
        /// <param name="headerRow">dòng để lấy tiêu đề các cột và dữ liệu</param>
        /// <param name="selectedSheet">Sheet để lấy dữ liệu</param>
        /// <returns></returns>
        public ActionResult MappingColumnExcel(int headerRow, int selectedSheet)
        {
            string msg = string.Empty;
            try
            {
                var fullPath = CacheHelper.Get("FullPath");
                if (fullPath == null) return Json(new { rs = false, msg = "Đã hết phiên làm việc. Bạn bấm <b>Đăng nhập lại</b> để trở về trang đăng nhập." });

                int indexSheet = selectedSheet; // Sheet dùng để lấy dữ liệu
                int rowIndex = headerRow;// dòng lấy ra tiêu đề các cột
                DataTable dt;
                msg = ExcelHelper.GetDataTableFromExcelFile(fullPath.ToString(), indexSheet, rowIndex, out dt);
                if (msg.Length > 0) return Json(new { rs = false, msg = "Dòng tiêu đề bạn chọn không có dữ liệu." });

                string[] arrCol;
                msg = ExcelHelper.GetColumnNamesFromDatable(dt, out arrCol);
                if (msg.Length > 0) return Json(new { rs = false, msg = "Dòng tiêu đề bạn chọn không có dữ liệu." });

                if (dt == null || dt.Rows.Count == 0) return Json(new { rs = false, msg = "Dòng tiêu đề bạn chọn không có dữ liệu." });
                CacheHelper.Set("dtMeter", dt);

                List<ImportMeterTitleBO> lstColumn = null;
                if (objUser.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTIENDIEN)
                {
                    lstColumn = new List<ImportMeterTitleBO>
                                {
                                    new ImportMeterTitleBO { MYFIELD = "Mã khách/Mã số thuế" },
                                    new ImportMeterTitleBO { MYFIELD = "Mã công tơ" },
                                    new ImportMeterTitleBO { MYFIELD = "Tên công tơ" },
                                    new ImportMeterTitleBO { MYFIELD = "Hệ số" },
                                    new ImportMeterTitleBO { MYFIELD = "Mã giờ" },
                                    new ImportMeterTitleBO { MYFIELD = "Số hộ" },
                                };
                }
                else
                {
                    lstColumn = new List<ImportMeterTitleBO>
                                {
                                    new ImportMeterTitleBO { MYFIELD = "Mã khách/Mã số thuế" },
                                    new ImportMeterTitleBO { MYFIELD = "Mã công tơ" },
                                    new ImportMeterTitleBO { MYFIELD = "Tên công tơ" },
                                    new ImportMeterTitleBO { MYFIELD = "Hệ số" },
                                    new ImportMeterTitleBO { MYFIELD = "Mã giờ" },
                                    new ImportMeterTitleBO { MYFIELD = "Số hộ" },
                                };
                }
                CacheHelper.Set("ColumnMeterFinance", lstColumn);

                List<ImportInvoiceColumnMaping> objColMaps = new List<ImportInvoiceColumnMaping>();

                for (var j = 0; j < arrCol.Length; j++)
                {
                    ImportInvoiceColumnMaping objMap = new ImportInvoiceColumnMaping();
                    objMap.Index = j;
                    objMap.ColName = arrCol[j];
                    objColMaps.Add(objMap);
                }

                for (int i = 0; i < lstColumn.Count; i++)
                {
                    for (var j = 0; j < arrCol.Length; j++)
                    {
                        if (lstColumn[i].MYFIELD.Equals(arrCol[j], StringComparison.OrdinalIgnoreCase))
                        {
                            CommonFunction.SetPropertyValue(lstColumn[i], "YOURFIELD", arrCol[j]);
                        }
                        else if (lstColumn[i].ConfigFields != null && lstColumn[i].ConfigFields.Contains(arrCol[j].ToString()))
                        {
                            CommonFunction.SetPropertyValue(lstColumn[i], "YOURFIELD", arrCol[j]);
                        }
                    }
                }

                return Json(new
                {
                    rs = true,
                    data = lstColumn,
                    lstYourField = objColMaps.Distinct(),
                    clientData = JsonConvert.SerializeObject(arrCol)
                });
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi ghép cột dữ liệu.", objEx, MethodBase.GetCurrentMethod().Name, "MappingColumnExcel");
                return Json(new { rs = false, msg = "Lỗi ghép cột dữ liệu." });
            }
        }

        /// <summary>
        /// tuyennv 20200706
        /// Xem trước dữ liệu sau khi mapping xong
        /// </summary>
        /// <param name="listMap"></param>
        /// <param name="formCode"></param>
        /// <param name="symbolCode"></param>
        /// <returns></returns>
        public ActionResult PreviewMeterData(string listMap)
        {
            try
            {
                var dt = CacheHelper.Get("dtMeter");
                if (dt == null) return Json(new { rs = false, msg = "Đã hết phiên làm việc. Bạn bấm <b>Đăng nhập lại</b> để trở về trang đăng nhập." });
                var dtMeter = (DataTable)dt;
                if (dtMeter != null && dtMeter.Rows.Count > 0)
                {
                    var columnInvoiceFinance = CacheHelper.Get("ColumnMeterFinance");
                    if (columnInvoiceFinance == null) return Json(new { rs = false, msg = "Đã hết phiên làm việc. Bạn bấm <b>Đăng nhập lại</b> để trở về trang đăng nhập." });
                    List<ImportMeterTitleBO> lstColumn = (List<ImportMeterTitleBO>)columnInvoiceFinance;
                    var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(listMap);
                    List<MeterBO> importMeters = new List<MeterBO>();
                    // rowNum: start row to get data from excel file.
                    MeterBO importMeter = null;
                    try
                    {
                        foreach (DataRow row in dtMeter.Rows)
                        {
                            importMeter = new MeterBO();
                            if (dic[lstColumn[0].MYFIELD] == null || string.IsNullOrEmpty(row[dic[lstColumn[0].MYFIELD]].ToString().Trim()))
                                continue;
                            string code = dic[lstColumn[1].MYFIELD] == null ? "" : row[dic[lstColumn[1].MYFIELD]].ToString().Trim();
                            string name = dic[lstColumn[2].MYFIELD] == null ? "" : row[dic[lstColumn[2].MYFIELD]].ToString().Trim();
                            if (string.IsNullOrWhiteSpace(name))
                                name = code;
                            importMeter.CUSTAXCODE = dic[lstColumn[0].MYFIELD] == null ? "" : row[dic[lstColumn[0].MYFIELD]].ToString().Trim();
                            importMeter.CODE = code;
                            importMeter.METERNAME = name;
                            importMeter.FACTOR = CommonFunction.NullSafeInteger(dic[lstColumn[3].MYFIELD] == null ? "" : row[dic[lstColumn[3].MYFIELD]].ToString().Trim(), 0);
                            importMeter.PRODUCTCODE = dic[lstColumn[4].MYFIELD] == null ? "" : row[dic[lstColumn[4].MYFIELD]].ToString().Trim();
                            importMeter.APARTMENTNO = CommonFunction.NullSafeInteger(dic[lstColumn[5].MYFIELD] == null ? "" : row[dic[lstColumn[5].MYFIELD]].ToString().Trim(), 0);

                            importMeters.Add(importMeter);
                        }
                    }
                    catch (Exception objEx)
                    {
                        ConfigHelper.Instance.WriteLog("Lỗi lấy dữ liệu để thêm mới.", objEx, MethodBase.GetCurrentMethod().Name, "PreviewMeterData");
                        return Json(new { rs = false, msg = "Lỗi lấy dữ liệu để thêm mới." });
                    }
                    CacheHelper.Set("ListMeter", importMeters);
                    return new JsonResult()
                    {
                        Data = new { ListMeters = importMeters },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        MaxJsonLength = Int32.MaxValue
                    };
                }
                return Json(new { rs = false, msg = "File không đúng chuẩn, vui lòng tải file Excel mẫu kế bên" });
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy dữ liệu để thêm mới.", objEx, MethodBase.GetCurrentMethod().Name, "PreviewMeterData");
                return Json(new { rs = false, msg = "Lỗi lấy dữ liệu để thêm mới." });
            }
        }

        /// <summary>
        /// tuyennv 20200706
        /// Nhập khẩu khách hàng
        /// </summary>
        /// <param name="invoices"></param>
        /// <returns></returns>
        public ActionResult ImportDataMeter()
        {
            var lstMeters = CacheHelper.Get("ListMeter");
            if (lstMeters == null) Json(new { rs = false, msg = "Đã hết phiên làm việc. Bạn bấm <b>Đăng nhập lại</b> để trở về trang đăng nhập." });
            List<MeterBO> meters = (List<MeterBO>)lstMeters;
            if (meters == null) Json(new { rs = false, msg = "Đã hết phiên làm việc. Bạn bấm <b>Đăng nhập lại</b> để trở về trang đăng nhập." });

            string msg = SaveDataImport(meters);
            if (msg.Length > 0)
                return Json(new { rs = true, msg = msg });
            return Json(new { rs = false, msg = "Nhập khẩu hàng hóa, dịch vụ không thành công, liên hệ với quản trị để được hỗ trợ." });
        }

        /// <summary>
        /// tuyennv 20200706
        /// Lưu thông tin khách hàng
        /// </summary>
        /// <param name="invoices"></param>
        /// <returns></returns>
        public string SaveDataImport(List<MeterBO> meters)
        {
            string msg = string.Empty;
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                foreach (var item in meters)
                {
                    item.COMTAXCODE = objUser.COMTAXCODE;
                }
                //Thêm công tơ
                Dictionary<string, MeterBO> dicResultProduct;
                Dictionary<int, MeterBO> dicImportProductError;
                serviceResult = SaveDataMeter(meters, out dicResultProduct, out dicImportProductError);
                msg = serviceResult.Message;
                return msg;
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "SaveDataImport");
                msg = "Lỗi thêm mới hàng hóa, dịch vụ.";
            }
            return msg;
        }

        /// <summary>
        /// tuyennv 20200706
        /// Cất dữ liệu hóa đơn
        /// </summary>
        /// <returns></returns>
        private ServiceResult SaveDataMeter(List<MeterBO> meters, out Dictionary<string, MeterBO> dicResultMeter, out Dictionary<int, MeterBO> dicImportMeterError)
        {
            ServiceResult serviceResult = new ServiceResult();
            string msg = string.Empty;
            dicResultMeter = new Dictionary<string, MeterBO>();
            dicImportMeterError = new Dictionary<int, MeterBO>();
            int totalCount = 0;
            int insertRowcount = 0;
            try
            {
                MeterBLL meterBLL = new MeterBLL();
                totalCount = meters.Count;
                int startRow = 0;
                foreach (var meter in meters)
                {
                    var result = meterBLL.AddCommonMeter(meter);
                    startRow++;
                    if (!string.IsNullOrEmpty(result))
                        dicImportMeterError.Add(startRow, meter);
                    else
                    {
                        insertRowcount++;
                        msg = $"Thêm mới thành công <b>{insertRowcount}/{totalCount}</b> công tơ.";
                        //dicResultProduct.Add(product., product);
                    }
                }
                serviceResult.ErrorCode = 0;
                serviceResult.Message = msg;
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "SaveDataProduct");
                msg = "Lỗi khi thêm mới công tơ.";
                serviceResult.ErrorCode = 1000;
                serviceResult.Message = msg;
            }
            return serviceResult;
        }
        #endregion
    }
}