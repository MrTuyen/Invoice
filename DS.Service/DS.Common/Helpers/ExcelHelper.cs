using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DS.Common.Helpers
{
    /// <summary>
    /// truongnv 20200406
    /// 
    /// </summary>
    static public class ExcelHelper
    {
        static public string GetDataTableFromExcelFile(string fullPath, int sheetIndex, int rowIndex, out DataTable dt)
        {
            string msg = "";
            dt = null;

            string extension = Path.GetExtension(fullPath).ToLower();
            if (extension == ".xls")
                msg = GetDataTableFromXls(fullPath, sheetIndex, rowIndex, out dt);
            else if (extension == ".xlsx")
                msg = GetDataTableFromXlsx(fullPath, sheetIndex, rowIndex, out dt);
            else
                return "Phần mở rộng của File không phải là Excel (.xls hoặc .xlsx)";
            if (msg.Length > 0)
                return msg;

            return msg;
        }

        /// <summary>
        /// Hàm get data từ file VTV Cab
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="sheetIndex"></param>
        /// <param name="rowIndex"></param>
        /// <param name="fromCode"></param>
        /// <param name="symbolCode"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        static public string GetHorizontalTableFromXls(string fullPath, int sheetIndex, int rowIndex, string formCode, string symbolCode, out DataTable dt)
        {
            dt = null;
            var erRow = 0;
            try
            {
                FileInfo fi = new FileInfo(fullPath);
                if (!fi.Exists)
                    return "File không tồn tại: " + fullPath;

                HSSFWorkbook hssfwb;
                using (FileStream file = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                {
                    hssfwb = new HSSFWorkbook(file);
                }

                ISheet sheet = hssfwb.GetSheetAt(sheetIndex);
                dt = new DataTable(sheet.SheetName);

                //Tạo column cho table
                dt.Columns.Add(Constants.COL_SIGNEDTIME);
                dt.Columns.Add(Constants.COL_FORMCODE);
                dt.Columns.Add(Constants.COL_SYMBOLCODE);
                dt.Columns.Add(Constants.COL_CUSNAME);
                dt.Columns.Add(Constants.COL_CUSTAXCODE);
                dt.Columns.Add(Constants.COL_CUSBUYER);
                dt.Columns.Add(Constants.COL_CUSADDRESS);
                dt.Columns.Add(Constants.COL_CUSPHONENUMBER);
                dt.Columns.Add(Constants.COL_CUSEMAIL);
                dt.Columns.Add(Constants.COL_CUSPAYMENTMETHOD);
                dt.Columns.Add(Constants.COL_CUSACCOUNTNUMBER);
                dt.Columns.Add(Constants.COL_CUSBANKNAME);
                dt.Columns.Add(Constants.COL_INVOICEID);
                dt.Columns.Add(Constants.COL_PRODUCTNAME);
                dt.Columns.Add(Constants.COL_QUANTITYUNIT);
                dt.Columns.Add(Constants.COL_SALEPRICE);
                dt.Columns.Add(Constants.COL_QUANTITY);
                dt.Columns.Add(Constants.COL_TAXRATE);
                dt.Columns.Add(Constants.COL_TOTALPAYMENT);

                //var lstCol = sheet.GetRow(rowIndex - 1)
                //    //.Where(x => !string.IsNullOrEmpty(x.ToString()))
                //    .Select(x => new DataColumn
                //    {
                //        Caption = x.ToString(),
                //        ColumnName = x.ToString()
                //    }).ToArray();

                //dt.Columns.AddRange(lstCol);
                IRow hRow = sheet.GetRow(rowIndex - 1);

                for (int i = rowIndex; i <= sheet.LastRowNum; i++)
                {
                    erRow = i;
                    IRow row = sheet.GetRow(i);
                    if (row == null || string.IsNullOrEmpty(row.Cells[2].ToString())) continue;

                    if (!string.IsNullOrEmpty(row.Cells[54].ToString())) AddProductHorizontalTable(formCode, symbolCode, hRow.Cells[54].ToString(), row.Cells[54].ToString(), row, ref dt);
                    if (!string.IsNullOrEmpty(row.Cells[55].ToString())) AddProductHorizontalTable(formCode, symbolCode, hRow.Cells[55].ToString(), row.Cells[55].ToString(), row, ref dt);
                    if (!string.IsNullOrEmpty(row.Cells[56].ToString())) AddProductHorizontalTable(formCode, symbolCode, hRow.Cells[56].ToString(), row.Cells[56].ToString(), row, ref dt);
                    if (!string.IsNullOrEmpty(row.Cells[57].ToString())) AddProductHorizontalTable(formCode, symbolCode, hRow.Cells[57].ToString(), row.Cells[57].ToString(), row, ref dt);
                    if (!string.IsNullOrEmpty(row.Cells[58].ToString())) AddProductHorizontalTable(formCode, symbolCode, hRow.Cells[58].ToString(), row.Cells[58].ToString(), row, ref dt);
                    if (!string.IsNullOrEmpty(row.Cells[59].ToString())) AddProductHorizontalTable(formCode, symbolCode, hRow.Cells[59].ToString(), row.Cells[59].ToString(), row, ref dt);
                    if (!string.IsNullOrEmpty(row.Cells[60].ToString())) AddProductHorizontalTable(formCode, symbolCode, hRow.Cells[60].ToString(), row.Cells[60].ToString(), row, ref dt);
                    if (!string.IsNullOrEmpty(row.Cells[61].ToString())) AddProductHorizontalTable(formCode, symbolCode, hRow.Cells[61].ToString(), row.Cells[61].ToString(), row, ref dt);
                    if (!string.IsNullOrEmpty(row.Cells[62].ToString())) AddProductHorizontalTable(formCode, symbolCode, hRow.Cells[62].ToString(), row.Cells[62].ToString(), row, ref dt);
                    if (!string.IsNullOrEmpty(row.Cells[63].ToString())) AddProductHorizontalTable(formCode, symbolCode, hRow.Cells[63].ToString(), row.Cells[63].ToString(), row, ref dt);
                    if (!string.IsNullOrEmpty(row.Cells[64].ToString())) AddProductHorizontalTable(formCode, symbolCode, hRow.Cells[64].ToString(), row.Cells[64].ToString(), row, ref dt);
                    if (!string.IsNullOrEmpty(row.Cells[65].ToString())) AddProductHorizontalTable(formCode, symbolCode, hRow.Cells[65].ToString(), row.Cells[65].ToString(), row, ref dt);
                    if (!string.IsNullOrEmpty(row.Cells[66].ToString())) AddProductHorizontalTable(formCode, symbolCode, hRow.Cells[66].ToString(), row.Cells[66].ToString(), row, ref dt);
                    if (!string.IsNullOrEmpty(row.Cells[67].ToString())) AddProductHorizontalTable(formCode, symbolCode, hRow.Cells[67].ToString(), row.Cells[67].ToString(), row, ref dt);
                    if (!string.IsNullOrEmpty(row.Cells[68].ToString())) AddProductHorizontalTable(formCode, symbolCode, hRow.Cells[68].ToString(), row.Cells[68].ToString(), row, ref dt);
                    if (!string.IsNullOrEmpty(row.Cells[69].ToString())) AddProductHorizontalTable(formCode, symbolCode, hRow.Cells[69].ToString(), row.Cells[69].ToString(), row, ref dt);
                    if (!string.IsNullOrEmpty(row.Cells[70].ToString())) AddProductHorizontalTable(formCode, symbolCode, hRow.Cells[70].ToString(), row.Cells[70].ToString(), row, ref dt);

                }

            }
            catch (Exception ex)
            {
                var msg = "Lỗi import file";
                if (erRow != 0)
                    msg = "Lỗi import file. Vui lòng xem lại dòng " + erRow + " theo thứ tự file excel.";

                ConfigHelper.Instance.WriteLog(ex.ToString() + " @Row: #" + rowIndex, ex, MethodBase.GetCurrentMethod().Name, "GetHorizontalTableFromXls");

                return msg;
            }

            return null;
        }

        static private void AddProductHorizontalTable(string fromCode, string symbolCode, string productName, string salePrice, IRow row, ref DataTable dt)
        {
            DataRow dataRow = dt.NewRow();

            dataRow[Constants.COL_SIGNEDTIME] = row.Cells[20].ToString();
            dataRow[Constants.COL_FORMCODE] = fromCode;
            dataRow[Constants.COL_SYMBOLCODE] = symbolCode;
            //dataRow[Constants.COL_CUSNAME] = row.Cells[2].ToString();
            //dataRow[Constants.COL_CUSTAXCODE] = row.Cells[1].ToString();
            dataRow[Constants.COL_CUSBUYER] = row.Cells[2].ToString();
            dataRow[Constants.COL_CUSADDRESS] = row.Cells[4].ToString();
            dataRow[Constants.COL_CUSPHONENUMBER] = row.Cells[3].ToString();
            //dataRow[Constants.COL_CUSEMAIL] = row.Cells[1].ToString();
            dataRow[Constants.COL_CUSPAYMENTMETHOD] = row.Cells[36].ToString();
            //dataRow[Constants.COL_CUSACCOUNTNUMBER] = row.Cells[1].ToString();
            //dataRow[Constants.COL_CUSBANKNAME] = row.Cells[1].ToString();
            dataRow[Constants.COL_INVOICEID] = row.Cells[1].ToString();
            dataRow[Constants.COL_PRODUCTNAME] = productName;
            dataRow[Constants.COL_QUANTITYUNIT] = "Gói";
            dataRow[Constants.COL_SALEPRICE] = salePrice.Replace(".", "").Replace(",", "");
            dataRow[Constants.COL_QUANTITY] = 1;
            //dataRow[Constants.COL_TAXRATE] = row.Cells[1].ToString();
            dataRow[Constants.COL_TOTALPAYMENT] = salePrice.Replace(".", "").Replace(",", "");

            dt.Rows.Add(dataRow);
        }

        static public string GetDataTableFromXls(string fullPath, int sheetIndex, int rowIndex, out DataTable dt)
        {
            string msg = "";
            dt = null;
            try
            {
                FileInfo fi = new FileInfo(fullPath);
                if (!fi.Exists) return "File không tồn tại: " + fullPath;

                HSSFWorkbook hssfwb;
                using (FileStream file = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                {
                    hssfwb = new HSSFWorkbook(file);
                }

                ISheet sheet = hssfwb.GetSheetAt(sheetIndex);
                dt = new DataTable(sheet.SheetName);

                int maxCols = 0;
                foreach (IRow row in sheet)
                {
                    if (row.LastCellNum > maxCols)
                    {
                        maxCols = row.LastCellNum;
                        for (int i = 0; i < maxCols; i++)
                            dt.Columns.Add(row.Cells[i].StringCellValue.Replace("\n", string.Empty));
                    }
                    break;
                }

                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;
                    rowIndex++;
                    DataRow dataRow = dt.NewRow();
                    foreach (var c in row.Cells)
                    {
                        switch (c.CellType)
                        {
                            case CellType.Blank: dataRow[c.ColumnIndex] = string.Empty; break;
                            case CellType.Boolean: dataRow[c.ColumnIndex] = c.BooleanCellValue; break;
                            case CellType.Error: dataRow[c.ColumnIndex] = c.ErrorCellValue; break;
                            case CellType.Formula: dataRow[c.ColumnIndex] = c; break;
                            case CellType.String: dataRow[c.ColumnIndex] = c.StringCellValue; break;
                            case CellType.Unknown: dataRow[c.ColumnIndex] = c; break;
                            case CellType.Numeric:
                                if (DateUtil.IsCellDateFormatted(c)) dataRow[c.ColumnIndex] = c.DateCellValue.ToString("dd/MM/yyyy");
                                else dataRow[c.ColumnIndex] = c.NumericCellValue.ToString();

                                break;
                        }
                    }

                    dt.Rows.Add(dataRow);
                }
            }
            catch (Exception ex)
            {
                dt = null;
                msg = ex.ToString() + " @Row: #" + rowIndex;
                ConfigHelper.Instance.WriteLog(msg, ex, MethodBase.GetCurrentMethod().Name, "GetDataTableFromXls");
            }
            return msg;
        }
        static public string GetDataTableFromXlsx(string fullPath, int sheetIndex, int rowIndex, out DataTable dt)
        {
            string msg = "";
            dt = null;
            int rowNum = rowIndex + 1; // dòng xuất phát để lấy dữ liệu vào các cột = dòng tiêu đề + 1
            int colNum = 1; // cột bắt đầu lấy dữ liệu
            //int rowStart = 1; // dòng header chứa tên các cột dữ liệu
            try
            {
                FileInfo fi = new FileInfo(fullPath);
                if (!fi.Exists) return "File không tồn tại: " + fullPath;

                using (var pck = new ExcelPackage())
                {
                    using (var stream = File.OpenRead(fullPath))
                    {
                        pck.Load(stream);
                    }
                    var ws = pck.Workbook.Worksheets[sheetIndex];

                    dt = new DataTable();
                    for (colNum = 1; colNum <= ws.Dimension.End.Column; colNum++)
                    {
                        if (ws.Cells[rowIndex, colNum].Value == null || ws.Cells[rowIndex, colNum].Value.ToString().Trim() == "")
                            continue;

                        string colName = ws.Cells[rowIndex, colNum].Value.ToString().Replace("\n", string.Empty);
                        dt.Columns.Add(colName);
                    }

                    for (; rowNum <= ws.Dimension.End.Row; rowNum++)
                    {
                        var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                        DataRow row = dt.Rows.Add();
                        foreach (var cell in wsRow)
                        {
                            colNum = 1;

                            string format = cell.Style.Numberformat.Format;
                            object value = cell.Value;
                            if (value == null)
                            {
                                //ConfigHelper.Instance.WriteLog($"Lỗi đọc dữ liệu từ file Excel. cell.{cell.Columns.ToString()}. Value: {cell.Value}", string.Empty, MethodBase.GetCurrentMethod().Name, "GetDataTableFromXlsx");
                            }
                            if (format.Contains("yyyy") || format.Contains("yy"))
                            {
                                var v = cell.Value;
                                DateTime date = DateTime.MinValue;
                                if (v is double) date = DateTime.FromOADate((double)v);
                                else if (v is DateTime) date = (DateTime)v;
                                else
                                {
                                    try
                                    {
                                        date = DateTime.ParseExact(v.ToString().Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                    }
                                    catch { }
                                }
                                if (date != DateTime.MinValue) value = date.ToString("dd/MM/yyyy");
                            }
                            row[cell.Start.Column - 1] = value == null ? DBNull.Value : value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dt = null;
                msg = ex.ToString() + " @Row-Col: #" + rowNum + "-" + colNum;
                ConfigHelper.Instance.WriteLog(msg, ex, MethodBase.GetCurrentMethod().Name, "GetDataTableFromXlsx");
            }
            return msg;
        }

        static public string GetColumnNamesFromDatable(DataTable dt, out string[] arrCol)
        {
            string msg = string.Empty;
            arrCol = null;
            try
            {
                arrCol = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray(); //.Replace("\n", "")
            }
            catch (Exception ex)
            {
                arrCol = null;
                msg = ex.ToString();
                ConfigHelper.Instance.WriteLog(msg, ex, MethodBase.GetCurrentMethod().Name, "GetColumnNamesFromDatable");
            }
            return msg;
        }

        static public string GetAllWorksheets(string fullPath, out List<ExWorksheet> objSheets)
        {
            string msg = string.Empty;
            objSheets = new List<ExWorksheet>();
            try
            {
                FileInfo fi = new FileInfo(fullPath);
                if (!fi.Exists) return "File không tồn tại: " + fullPath;

                string extension = Path.GetExtension(fullPath).ToLower();
                if (extension == ".xls")
                {
                    HSSFWorkbook hssfwb;
                    using (FileStream file = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                    {
                        hssfwb = new HSSFWorkbook(file);
                    }
                    for (int i = 0; i < hssfwb.NumberOfSheets; i++)
                    {
                        ExWorksheet obj = new ExWorksheet();
                        ISheet sheet = hssfwb.GetSheetAt(i);
                        obj.Index = i;
                        obj.SheetName = sheet.SheetName;
                        objSheets.Add(obj);
                    }
                }
                else if (extension == ".xlsx")
                {
                    using (var pck = new ExcelPackage())
                    {
                        using (var stream = File.OpenRead(fullPath))
                        {
                            pck.Load(stream);
                        }
                        var ws = pck.Workbook.Worksheets;
                        foreach (var item in ws)
                        {
                            ExWorksheet obj = new ExWorksheet();
                            obj.Index = item.Index;
                            obj.SheetName = item.Name;
                            objSheets.Add(obj);
                        }
                    }
                }
                else return "Phần mở rộng của File không phải là Excel (.xls hoặc .xlsx)";
                if (msg.Length > 0) return msg;
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog($"Lỗi lấy thông tin sheet: {ex.ToString()}", ex, MethodBase.GetCurrentMethod().Name, "GetAllWorksheets");
                msg = ex.ToString();
            }
            return msg;
        }
    }
}
