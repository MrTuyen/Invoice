﻿using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Reflection;

namespace erp.onfinance
{
    public class GlobalFunction
    {

        public static byte[] Export2XLS(DataTable dtData)
        {
            using (ExcelPackage pck = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet1");

                //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                ws.Cells["A1"].LoadFromDataTable(dtData, true);

                //Format the header for column 1-3
                using (ExcelRange rng = ws.Cells["A1:BZ1"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;   //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.WhiteSmoke);  //Set color to dark blue
                    rng.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                }

                for (int i = 0; i < dtData.Columns.Count; i++)
                {
                    if (dtData.Columns[i].DataType == typeof(DateTime))
                    {
                        using (ExcelRange col = ws.Cells[2, i + 1, 2 + dtData.Rows.Count, i + 1])
                        {
                            //col.Style.Numberformat.Format = "MM/dd/yyyy HH:mm";
                            col.Style.Numberformat.Format = "dd/MM/yyyy HH:mm";
                            //col.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        }
                    }
                    if (dtData.Columns[i].DataType == typeof(TimeSpan))
                    {
                        using (ExcelRange col = ws.Cells[2, i + 1, 2 + dtData.Rows.Count, i + 1])
                        {
                            col.Style.Numberformat.Format = "d.hh:mm";
                            col.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        }
                    }
                }

                return pck.GetAsByteArray();
            } // end using
        }
    }
}