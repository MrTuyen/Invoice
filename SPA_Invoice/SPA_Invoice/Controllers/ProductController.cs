using DS.BusinessLogic;
using DS.BusinessLogic.Product;
using DS.BusinessObject.Invoice;
using DS.BusinessObject.Output;
using DS.BusinessObject.Product;
using DS.Common.Enums;
using DS.Common.Helpers;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using OfficeOpenXml;
using SPA_Invoice.Common;
using SPA_Invoice.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SPA_Invoice.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IHubContext _hubContext;
        public ProductController()
        {
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<SignlRConf>();
        }

        public ActionResult Index()
        {
            //if (string.IsNullOrEmpty(objUser.COMTAXCODE))
            //return PartialView("");

            return PartialView();
        }

        public ActionResult GetProduct(FormSearchProduct form, int currentPage, int itemPerPage)
        {
            try
            {
                form.CURRENTPAGE = currentPage;
                form.ITEMPERPAGE = itemPerPage;
                form.COMTAXCODE = objUser.COMTAXCODE;
                form.OFFSET = (form.CURRENTPAGE - 1) * form.ITEMPERPAGE;
                ProductBLL productBLL = new ProductBLL();
                List<ProductBO> result = productBLL.GetProduct(form);

                foreach (var item in result)
                {
                    if (string.IsNullOrEmpty(item.IMAGE))
                    {
                        item.IMAGE = "/Images/64px_image-add.png";
                    }
                    else
                    {
                        item.IMAGE = "/NOVAON_FOLDER" + item.IMAGE;
                    }
                }
                long TotalPages = 0;
                var TotalRow = result.Count == 0 ? 0 : result[0].TOTALROW;
                if (TotalRow % itemPerPage == 0)
                    TotalPages = TotalRow == 0 ? 1 : TotalRow / itemPerPage;
                else
                    TotalPages = TotalRow / itemPerPage + 1;

                if (productBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(productBLL.ResultMessageBO.Message, productBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetProduct");
                    return Json(new { rs = false, msg = productBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result, TotalPages, TotalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy danh sách sản phẩm", ex, MethodBase.GetCurrentMethod().Name, "GetProduct");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddProduct(ProductBO product)
        {
            try
            {
                product.COMTAXCODE = objUser.COMTAXCODE;
                ProductBLL productBLL = new ProductBLL();

                if (product.IMAGE != null)
                {
                    string[] image = product.IMAGE.Split(',');
                    if (image.Count() > 1)
                    {
                        product.IMAGE = SaveFile(image[1], product);
                    }
                    else
                    {
                        product.IMAGE = image[0].Replace("/" + ConfigurationManager.AppSettings["ImageFolder"], "");
                    }
                }
                var result = productBLL.AddProduct(product);
                if (productBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(productBLL.ResultMessageBO.Message, productBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "AddProduct");
                    return Json(new { rs = false, msg = productBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, product }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi thêm sản phẩm", ex, MethodBase.GetCurrentMethod().Name, "AddProduct");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public string SaveFile(string base64String, ProductBO product)
        {
            try
            {
                string root = ConfigurationManager.AppSettings["ImageFolder"]; // physical folder in local or virtual path on cloud server by getting the appconfig 
                string branchComTaxCode = "/" + (string.IsNullOrEmpty(product.COMTAXCODE) ? "COMTAXCODE/" : product.COMTAXCODE + "/");
                string path = string.Format($"{root}{branchComTaxCode}") + "Images/";
                string title = ElaHelper.FormatKeywordSearch(ElaHelper.FilterVietkey(product.PRODUCTNAME));

                // checking path is exist if not create the folder to download file 
                DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/" + path));
                if (!dir.Exists)
                {
                    dir.Create();
                }

                string fileName = dir + title.Replace(" ", "-") + "-" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + ".jpg";

                // save file to folder
                byte[] dataBuffer = Convert.FromBase64String(base64String);
                using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    if (dataBuffer.Length > 0)
                    {
                        fileStream.Write(dataBuffer, 0, dataBuffer.Length);
                    }
                }

                // return file name to save into database
                return fileName.Replace(Server.MapPath("~/" + root), "");
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lưu hình ảnh sản phẩm", ex, MethodBase.GetCurrentMethod().Name, "SaveFile");
                return null;
            }
        }

        public ActionResult UpdateProduct(ProductBO product)
        {
            try
            {
                product.COMTAXCODE = objUser.COMTAXCODE;
                ProductBLL productBLL = new ProductBLL();

                if (product.IMAGE != null)
                {
                    string[] image = product.IMAGE.Split(',');
                    if (image.Count() > 1)
                    {
                        product.IMAGE = SaveFile(image[1], product);
                    }
                    else
                    {
                        product.IMAGE = image[0].Replace("/" + ConfigurationManager.AppSettings["ImageFolder"], "");
                    }
                }
                var result = productBLL.UpdateProduct(product);
                if (productBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(productBLL.ResultMessageBO.Message, productBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "UpdateProduct");
                    return Json(new { rs = false, msg = productBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, product }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi cập nhật sản phẩm", ex, MethodBase.GetCurrentMethod().Name, "UpdateProduct");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveProductList(List<ProductBO> products)
        {
            try
            {
                products.ForEach(x => x.COMTAXCODE = objUser.COMTAXCODE);
                ProductBLL productBLL = new ProductBLL();
                var result = productBLL.ImportProduct(products);
                if (productBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(productBLL.ResultMessageBO.Message, productBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "SaveProductList");
                    return Json(new { rs = false, msg = productBLL.ResultMessageBO.Message });
                }
                if (result)
                    return Json(new { rs = true }, JsonRequestBehavior.AllowGet);
                else return Json(new { rs = false, msg = "Lối lưu danh sách hàng hóa dịch vụ." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lưu sản phẩm từ file excel", ex, MethodBase.GetCurrentMethod().Name, "SaveProductList");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ChangeProductType(List<ProductBO> products, int productType)
        {
            try
            {
                string productIds = string.Join(",", products.Where(z => z.ISSELECTED).Select(x => x.ID).ToList());
                ProductBLL productBLL = new ProductBLL();
                bool result = productBLL.UpdateChangeType(productIds, productType);
                if (productBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(productBLL.ResultMessageBO.Message, productBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "ChangeProductType");
                    return Json(new { rs = false, msg = productBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi thay đổi loại sản phẩm", ex, MethodBase.GetCurrentMethod().Name, "ChangeProductType");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Lấy danh sách sản phẩm từ Elasticsearch của dược: 10.1.4.79:9200
        /// </summary>
        /// <param name="strKeyword"></param>
        /// <param name="intPageSize"></param>
        /// <returns></returns>
        public ActionResult SuggestByObject(string strKeyword, int intPageSize)
        {
            try
            {
                var objProductInfo = new ProductBLL();
                var listResult = new List<SuggestModel>();

                //Ưu tiên tìm sản phẩm
                var ES_List = objProductInfo.ES_ProductByKeyword(strKeyword, objUser.COMTAXCODE, intPageSize, 0);

                if (objProductInfo.ResultMessageBO.IsError)
                    return Json(new { rs = false, msg = objProductInfo.ResultMessageBO.Message }, JsonRequestBehavior.AllowGet);
                //var tempListProducts = ES_List.Where(x => x.COMTAXCODE.Trim() == objUser.COMTAXCODE).ToList();
                return Json(new { rs = true, listResult = ES_List });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi không thể lấy thông tin sản phẩm từ ES.", ex, "SearchProductSuggestByWeb", "Common");
                return Json(new { rs = false, msg = "Lỗi không thể lấy thông tin sản phẩm từ ES." }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ExportExcell(string keyword, int productType, string category, int currentPage, int itemPerPage)
        {
            try
            {
                Response.SetCookie(new HttpCookie("fileDownload", "true") { Path = "/" });
                ProductBLL productBLL = new ProductBLL();
                FormSearchProduct search = new FormSearchProduct
                {
                    COMTAXCODE = objUser.COMTAXCODE,
                    KEYWORD = keyword,
                    PRODUCTTYPE = productType,
                    CATEGORY = category == "" ? null : category,
                    CURRENTPAGE = currentPage,
                    ITEMPERPAGE = itemPerPage,
                    OFFSET = (currentPage - 1) * itemPerPage
                };
                List<ProductBO> result = productBLL.GetProduct(search);

                // tạo nhãn dữ liệu
                DataTable dtReport = new DataTable();
                dtReport.Columns.Add("TÊN SẢN PHẨM");
                dtReport.Columns.Add("SKU");
                dtReport.Columns.Add("LOẠI");
                dtReport.Columns.Add("DANH MỤC");
                dtReport.Columns.Add("MÔ TẢ");
                dtReport.Columns.Add("ĐƠN VỊ TÍNH");
                dtReport.Columns.Add("ĐƠN GIÁ");

                //add dòng dữ liệu vào
                foreach (var row in result)
                {
                    DataRow dr = dtReport.NewRow();
                    dr["TÊN SẢN PHẨM"] = row.PRODUCTNAME;
                    dr["SKU"] = row.SKU;
                    dr["LOẠI"] = Convert.ToInt32(row.PRODUCTTYPE) == 1 ? "Hàng hóa" : "Dịch vụ";
                    dr["DANH MỤC"] = row.CATEGORYNAME;
                    dr["MÔ TẢ"] = row.DESCRIPTION;
                    dr["ĐƠN VỊ TÍNH"] = row.QUANTITYUNIT;
                    dr["ĐƠN GIÁ"] = row.PRICE;
                    dtReport.Rows.Add(dr);
                }
                // đặt tên file xuất ra file theo tên 
                string strFileName = "Danh_sach_san_pham" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm");
                return PushFile(dtReport, strFileName);
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi khi xuất excel danh sách sản phẩm.", objEx, MethodBase.GetCurrentMethod().Name, "ExportExcell");
                return Json(new { success = false, responseText = "Lỗi khi xuất excel danh sách sản phẩm." });
            }
        }

        public ActionResult ImportProductFromExcel(FormCollection collection)
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
                                    List<ImportProductTitleBO> lstColumn = null;
                                    switch (objUser.USINGINVOICETYPE)
                                    {
                                        case (int)EnumHelper.AccountObjectType.HOADONTIENDIEN:
                                        case (int)EnumHelper.AccountObjectType.HOADONTIENNUOC:
                                            lstColumn = new List<ImportProductTitleBO>
                                            {
                                                new ImportProductTitleBO { MYFIELD = "Mã giờ/Mức" },
                                                new ImportProductTitleBO { MYFIELD = "Tên giờ/Tên mức" },
                                                new ImportProductTitleBO { MYFIELD = "Từ mức" },
                                                new ImportProductTitleBO { MYFIELD = "Đến mức" },
                                                new ImportProductTitleBO { MYFIELD = "Giá" },
                                                new ImportProductTitleBO { MYFIELD = "Biểu giá" },
                                            };
                                            break;
                                        default:
                                            lstColumn = new List<ImportProductTitleBO>
                                            {
                                                new ImportProductTitleBO { MYFIELD = "Mã hàng hóa, dịch vụ" },
                                                new ImportProductTitleBO { MYFIELD = "Tên hàng hóa, dịch vụ" },
                                                new ImportProductTitleBO { MYFIELD = "Đơn vị tính" },
                                                new ImportProductTitleBO { MYFIELD = "Giá bán" },
                                            };
                                            break;
                                    }
                                    for (var columnNum = 1; columnNum <= sheet.Dimension.End.Column; columnNum++)
                                    {
                                        if (sheet.Cells[1, columnNum].Value == null || sheet.Cells[1, columnNum].Value.ToString().Trim() == "")
                                            continue;
                                        try
                                        {
                                            if (lstColumn[columnNum - 1] == null)
                                                lstColumn.Add(new ImportProductTitleBO());
                                        }
                                        catch
                                        {
                                            lstColumn.Add(new ImportProductTitleBO());
                                        }
                                        lstColumn[columnNum - 1].YOURFIELD = sheet.Cells[1, columnNum].Value.ToString();
                                        if (lstColumn[columnNum - 1].MYFIELD.ToLower() != lstColumn[columnNum - 1].YOURFIELD.ToLower())
                                            lstColumn[columnNum - 1].ISDIFFERENT = true;
                                    }
                                    int countDiffent = lstColumn.Where(x => x.ISDIFFERENT).Count();
                                    if (countDiffent > 0)
                                        return Json(new { rs = true, isDiffent = true, data = lstColumn });

                                    List<ImportProductBO> importProducts = new List<ImportProductBO>();

                                    if (objUser.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTIENDIEN || objUser.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTIENNUOC)
                                    {
                                        for (var rowNum = 2; rowNum <= sheet.Dimension.End.Row; rowNum++)
                                        {
                                            ImportProductBO importProduct = new ImportProductBO();
                                            if (sheet.Cells[rowNum, 1].Value == null || string.IsNullOrEmpty(sheet.Cells[rowNum, 1].Value.ToString().Trim()))
                                                continue;

                                            int groupid = CommonFunction.NullSafeInteger(sheet.Cells[rowNum, 6].Value == null ? "" : sheet.Cells[rowNum, 6].Value.ToString().Trim(), 0);
                                            int tolevel = CommonFunction.NullSafeInteger(sheet.Cells[rowNum, 4].Value == null ? "" : sheet.Cells[rowNum, 4].Value.ToString().Trim(), 0);
                                            importProduct.SKU = sheet.Cells[rowNum, 1].Value == null ? "" : sheet.Cells[rowNum, 1].Value.ToString().Trim();
                                            importProduct.PRODUCTNAME = sheet.Cells[rowNum, 2].Value == null ? "" : sheet.Cells[rowNum, 2].Value.ToString().Trim();
                                            importProduct.FROMLEVEL = CommonFunction.NullSafeInteger(sheet.Cells[rowNum, 3].Value == null ? "" : sheet.Cells[rowNum, 3].Value.ToString().Trim(), 0);
                                            importProduct.TOLEVEL = tolevel;
                                            importProduct.PRODUCTTYPE = 1;
                                            importProduct.QUANTITYUNIT = "Khác";
                                            importProduct.PRICE = sheet.Cells[rowNum, 5].Value == null ? 0 :
                                                string.IsNullOrEmpty(sheet.Cells[rowNum, 5].Value.ToString().Trim()) ? 0 : Single.Parse(sheet.Cells[rowNum, 5].Value.ToString().Trim());
                                            importProduct.GROUPID = groupid;

                                            importProducts.Add(importProduct);
                                        }
                                    }
                                    else
                                    {
                                        for (var rowNum = 2; rowNum <= sheet.Dimension.End.Row; rowNum++)
                                        {
                                            ImportProductBO importProduct = new ImportProductBO();
                                            if (sheet.Cells[rowNum, 1].Value == null || string.IsNullOrEmpty(sheet.Cells[rowNum, 1].Value.ToString().Trim()))
                                                continue;
                                            importProduct.SKU = sheet.Cells[rowNum, 1].Value == null ? "" : sheet.Cells[rowNum, 1].Value.ToString().Trim();
                                            importProduct.PRODUCTNAME = sheet.Cells[rowNum, 2].Value == null ? "" : sheet.Cells[rowNum, 2].Value.ToString().Trim();
                                            importProduct.QUANTITYUNIT = sheet.Cells[rowNum, 3].Value == null ? "" : sheet.Cells[rowNum, 3].Value.ToString().Trim();
                                            importProduct.PRICE = sheet.Cells[rowNum, 4].Value == null ? 0 :
                                                string.IsNullOrEmpty(sheet.Cells[rowNum, 4].Value.ToString().Trim()) ? 0 : Single.Parse(sheet.Cells[rowNum, 4].Value.ToString().Trim());
                                            importProduct.GROUPID = 0;
                                            importProducts.Add(importProduct);
                                        }
                                    }

                                    return Json(new { rs = true, isDiffent = false, data = lstColumn, importProducts });
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

        /// <summary>
        /// truongnv 20200318
        /// Lấy danh sách bộ chỉ số công tơ điện
        /// </summary>
        /// <param name="custaxcode"></param>
        /// <returns></returns>
        public JsonResult GetProductListByMeterCode(string meterCode, string custaxcode)
        {
            try
            {
                ProductBLL oBL = new ProductBLL();
                List<ProductBO> obj = oBL.GetProductListByMeterCode(meterCode, custaxcode, objUser.COMTAXCODE);
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
        /// <param name="custaxcode"></param>
        /// <returns></returns>
        public JsonResult GetListProductByComtaxCode(string comtaxcode)
        {
            try
            {
                ProductBLL oBL = new ProductBLL();
                List<ProductBO> obj = oBL.GetListProductByComtaxCode(comtaxcode);
                if (obj != null)
                    return Json(new { rs = true, msg = obj });
            }
            catch
            {
                return Json(new { rs = false, msg = "Không lấy được thông tin bộ chỉ số công tơ" });
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RemoveProduct(string idProducts,string proname,string sku)
        {
            try
            {
                string[] lstProductid = idProducts.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                string[] lstProductname = proname.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                string[] lstProductsku = sku.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                ProductBLL productBLL = new ProductBLL();

                List<string> arr = new List<string>();
                List<string> arrname = new List<string>();
                List<string> arrsku = new List<string>();
                for (int i = 0; i < lstProductid.Length; i++)
                {
                    var vVal = CommonFunction.NullSafeInteger(lstProductid[i], 0);
                    arr.Add($"'{vVal}'");
                }
                for (int i = 0; i < lstProductname.Length; i++)
                {
                    var vVal = CommonFunction.NullSafeString(lstProductname[i], "");
                    arrname.Add($"'{vVal}'");
                }
                for (int i = 0; i < lstProductsku.Length; i++)
                {
                    var vVal = CommonFunction.NullSafeString(lstProductsku[i], "");
                    arrsku.Add($"'{vVal}'");
                }
                string ids = string.Join(",", arr);
                string names = string.Join(",", arrname);
                string skus = string.Join(",", arrsku);
                int count = productBLL.CheckPro(names);
                if(objUser.USINGINVOICETYPE == 2 || objUser.USINGINVOICETYPE == 4)
                {
                    int countMeter = productBLL.CheckMeter(skus);
                    if(countMeter > 0)
                    {
                        return Json(new { rs = false, msg = $"sản phẩm này đang được sử dụng bạn không được xóa !" });
                    }
                }
                if(count > 0)
                {
                    return Json(new { rs = false, msg = $"sản phẩm này đang được sử dụng bạn không được xóa !" });
                }
                else
                {
                    string msg = productBLL.DeleteProduct(ids);
                    if (msg.Length > 0)
                        return Json(new { rs = false, msg = msg });

                    return Json(new { rs = true, msg = $"Xóa thành công {lstProductid.Length}/{lstProductid.Length} sản phẩm." });
                }
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "RemoveProduct");
            }

            return Json(new { rs = true, msg = "Xóa sản phẩm" });
        }

        #region Mapping hàng hóa, dịch vụ

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
                CacheHelper.Set("dtProduct", dt);

                List<ImportProductTitleBO> lstColumn = null;
                switch (objUser.USINGINVOICETYPE)
                {
                    case (int)EnumHelper.AccountObjectType.HOADONTIENDIEN:
                    case (int)EnumHelper.AccountObjectType.HOADONTIENNUOC:
                        lstColumn = new List<ImportProductTitleBO>
                                    {
                                        new ImportProductTitleBO { MYFIELD = "Mã giờ/Mức" },
                                        new ImportProductTitleBO { MYFIELD = "Tên giờ/Tên mức" },
                                        new ImportProductTitleBO { MYFIELD = "Từ mức" },
                                        new ImportProductTitleBO { MYFIELD = "Đến mức" },
                                        new ImportProductTitleBO { MYFIELD = "Giá" },
                                        new ImportProductTitleBO { MYFIELD = "Biểu giá" },
                                    };
                        break;
                    default:
                        lstColumn = new List<ImportProductTitleBO>
                                    {
                                        new ImportProductTitleBO { MYFIELD = "Mã hàng hóa, dịch vụ" },
                                        new ImportProductTitleBO { MYFIELD = "Tên hàng hóa, dịch vụ" },
                                        new ImportProductTitleBO { MYFIELD = "Đơn vị tính" },
                                        new ImportProductTitleBO { MYFIELD = "Giá bán" },
                                    };
                        break;
                }
                CacheHelper.Set("ColumnProductFinance", lstColumn);

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
        /// truongnv 20200706
        /// Xem trước dữ liệu sau khi mapping xong
        /// </summary>
        /// <param name="listMap"></param>
        /// <param name="formCode"></param>
        /// <param name="symbolCode"></param>
        /// <returns></returns>
        public ActionResult PreviewProductData(string listMap)
        {
            try
            {
                var dt = CacheHelper.Get("dtProduct");
                if (dt == null) return Json(new { rs = false, msg = "Đã hết phiên làm việc. Bạn bấm <b>Đăng nhập lại</b> để trở về trang đăng nhập." });
                var dtProduct = (DataTable)dt;
                if (dtProduct != null && dtProduct.Rows.Count > 0)
                {
                    var columnInvoiceFinance = CacheHelper.Get("ColumnProductFinance");
                    if (columnInvoiceFinance == null) return Json(new { rs = false, msg = "Đã hết phiên làm việc. Bạn bấm <b>Đăng nhập lại</b> để trở về trang đăng nhập." });
                    List<ImportProductTitleBO> lstColumn = (List<ImportProductTitleBO>)columnInvoiceFinance;
                    var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(listMap);
                    List<ProductBO> importProducts = new List<ProductBO>();
                    // rowNum: start row to get data from excel file.
                    ProductBO importProduct = null;
                    try
                    {
                        foreach (DataRow row in dtProduct.Rows)
                        {
                            if (objUser.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTIENDIEN || objUser.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTIENNUOC)
                            {
                                if (dic[lstColumn[0].MYFIELD] == null || string.IsNullOrEmpty(row[dic[lstColumn[0].MYFIELD]].ToString().Trim()))
                                    continue;
                                int groupid = CommonFunction.NullSafeInteger(dic[lstColumn[5].MYFIELD] == null ? "" : row[dic[lstColumn[5].MYFIELD]].ToString().Trim(), 0);
                                int tolevel = CommonFunction.NullSafeInteger(dic[lstColumn[3].MYFIELD] == null ? "" : row[dic[lstColumn[3].MYFIELD]].ToString().Trim(), 0);
                                importProduct = new ProductBO()
                                {
                                    SKU = dic[lstColumn[0].MYFIELD] == null ? "" : row[dic[lstColumn[0].MYFIELD]].ToString().Trim(),
                                    PRODUCTNAME = dic[lstColumn[1].MYFIELD] == null ? "" : row[dic[lstColumn[1].MYFIELD]].ToString().Trim(),
                                    FROMLEVEL = CommonFunction.NullSafeInteger(dic[lstColumn[2].MYFIELD] == null ? "" : row[dic[lstColumn[2].MYFIELD]].ToString().Trim(), 0),
                                    TOLEVEL = tolevel,
                                    PRODUCTTYPE = 1,
                                    QUANTITYUNIT = "Khác",
                                    PRICE = dic[lstColumn[4].MYFIELD] == null ? 0 : string.IsNullOrEmpty(row[dic[lstColumn[4].MYFIELD]].ToString().Trim()) ? 0 : decimal.Parse(row[dic[lstColumn[4].MYFIELD]].ToString().Trim()),
                                    GROUPID = groupid
                                };
                                importProducts.Add(importProduct);
                            }
                            else
                            {
                                if (dic[lstColumn[0].MYFIELD] == null || string.IsNullOrEmpty(row[dic[lstColumn[0].MYFIELD]].ToString().Trim()))
                                    continue;
                                importProduct = new ProductBO()
                                {
                                    SKU = dic[lstColumn[0].MYFIELD] != null ? row[dic[lstColumn[0].MYFIELD]].ToString() : string.Empty,
                                    PRODUCTNAME = dic[lstColumn[1].MYFIELD] != null ? row[dic[lstColumn[1].MYFIELD]].ToString() : string.Empty,
                                    QUANTITYUNIT = dic[lstColumn[2].MYFIELD] != null ? row[dic[lstColumn[2].MYFIELD]].ToString() : string.Empty,
                                    PRICE = dic[lstColumn[3].MYFIELD] == null ? 0 : string.IsNullOrEmpty(row[dic[lstColumn[3].MYFIELD]].ToString().Trim()) ? 0 : decimal.Parse(row[dic[lstColumn[3].MYFIELD]].ToString().Trim()),
                                    GROUPID = 0
                                };
                                importProducts.Add(importProduct);
                            }
                        }
                    }
                    catch (Exception objEx)
                    {
                        ConfigHelper.Instance.WriteLog("Lỗi lấy dữ liệu để thêm mới.", objEx, MethodBase.GetCurrentMethod().Name, "PreviewProductData");
                        return Json(new { rs = false, msg = "Lỗi lấy dữ liệu để thêm mới." });
                    }
                    CacheHelper.Set("ListProduct", importProducts);
                    return new JsonResult()
                    {
                        Data = new { ListProducts = importProducts },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        MaxJsonLength = Int32.MaxValue
                    };
                }
                return Json(new { rs = false, msg = "File không đúng chuẩn, vui lòng tải file Excel mẫu kế bên" });
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy dữ liệu để thêm mới.", objEx, MethodBase.GetCurrentMethod().Name, "PreviewProductData");
                return Json(new { rs = false, msg = "Lỗi lấy dữ liệu để thêm mới." });
            }
        }

        /// <summary>
        /// tuyennv 20200706
        /// Nhập khẩu khách hàng
        /// </summary>
        /// <param name="invoices"></param>
        /// <returns></returns>
        public ActionResult ImportDataProduct()
         {
            var lstProducts = CacheHelper.Get("ListProduct");
            if (lstProducts == null) Json(new { rs = false, msg = "Đã hết phiên làm việc. Bạn bấm <b>Đăng nhập lại</b> để trở về trang đăng nhập." });
            List<ProductBO> products = (List<ProductBO>)lstProducts;
            if (products == null) Json(new { rs = false, msg = "Đã hết phiên làm việc. Bạn bấm <b>Đăng nhập lại</b> để trở về trang đăng nhập." });

            string msg = SaveDataImport(products);
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
        public string SaveDataImport(List<ProductBO> products)
        {
            string msg = string.Empty;
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                foreach (var item in products)
                {
                    item.COMTAXCODE = objUser.COMTAXCODE;
                }
                //Thêm sản phẩm, hàng hóa, dịch vụ
                Dictionary<string, ProductBO> dicResultProduct;
                Dictionary<int, ProductBO> dicImportProductError;
                serviceResult = SaveDataProduct(products, out dicResultProduct, out dicImportProductError);
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
        /// truongnv 20200304
        /// Cất dữ liệu hóa đơn
        /// </summary>
        /// <returns></returns>
        private ServiceResult SaveDataProduct(List<ProductBO> products, out Dictionary<string, ProductBO> dicResultProduct, out Dictionary<int, ProductBO> dicImportProductError)
        {
            ProductBLL productBLL = new ProductBLL();
            ServiceResult serviceResult = new ServiceResult();
            string msg = string.Empty;
            dicResultProduct = new Dictionary<string, ProductBO>();
            dicImportProductError = new Dictionary<int, ProductBO>();
            int totalCount = products.Count;
            int insertRowcount = 0;
            try
            {
                int startRow = 0;
                ProgressViewModel model = new ProgressViewModel();
                model.TotalRow = totalCount;
                int taskNum = 20;
                int qtyperTask = 0;
                CalTaskNumber(totalCount, ref qtyperTask, ref taskNum);
                for (int i = 0; i < taskNum; i++)
                {
                    int tempI = i;
                    var items = products.Skip(tempI * qtyperTask).Take(qtyperTask).ToList();
                    var task = Task.Run(() =>
                    {
                        foreach (var item in items)
                        {
                            bool result = productBLL.AddProduct(item);
                            if (result)
                                startRow++;
                            insertRowcount = insertRowcount + 1;
                            model.CurrentRow = startRow;
                            model.IsSuccess = startRow == model.TotalRow;
                            _hubContext.Clients.Group(objUser.USERNAME).newMessageReceivedProduct(model);
                        }
                    });
                }
                //foreach (var product in products)
                //{
                //    bool result = productBLL.AddProduct(product);
                //    startRow++;
                //    if (!result)
                //        dicImportProductError.Add(startRow, product);
                //    else
                //    {
                //        insertRowcount++;
                //        msg = $"Thêm mới thành công <b>{insertRowcount}/{totalCount}</b> sản phẩm, hàng hóa, dịch vụ.";
                //        //dicResultProduct.Add(product., product);
                //    }
                //}
                msg = $"Thêm mới thành công <b>{insertRowcount}/{totalCount}</b> sản phẩm, hàng hóa, dịch vụ.";
                serviceResult.ErrorCode = 0;
                serviceResult.Message = msg;
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "SaveDataProduct");
                msg = "Lỗi khi thêm mới hàng hóa, dịch vụ.";
                serviceResult.ErrorCode = 1000;
                serviceResult.Message = msg;
            }
            return serviceResult;
        }
        #endregion
    }
}