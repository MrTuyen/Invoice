using DS.BusinessLogic.Invoice;
using DS.BusinessObject.Invoice;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SPA_Invoice.Common
{
    public class GlobalFunction
    {
        public static byte[] Export2XLS(System.Data.DataTable dtData)
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
            }
        }

        //public static byte[] Export2XLSCustomizationForInvoice(List<InvoiceBO> lstInvoice)
        //{
        //    InvoiceBO invoice = new InvoiceBO();
        //    InvoiceBLL invoiceBLL = new InvoiceBLL();
        //    if (lstInvoice.Count > 0)
        //    {
        //        invoice = lstInvoice[0];
        //    }
        //    var rowStart = 7;
        //    // var totalQuantity = 0;
        //    decimal totalQuantity = 0;
        //    decimal totalAmount = 0;
        //    decimal totalTaxAmount = 0;
        //    decimal totalOtherTaxFee = 0;

        //    using (ExcelPackage pck = new ExcelPackage())
        //    {
        //        //Create the worksheet
        //        ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet1");

        //        ws.Column(8).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //        ws.Column(8).Style.Numberformat.Format = "#,##0.00";
        //        ws.Column(10).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //        ws.Column(10).Style.Numberformat.Format = "#,##0.00";
        //        ws.Column(12).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //        ws.Column(12).Style.Numberformat.Format = "#,##0.00";
        //        ws.Column(13).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //        ws.Column(13).Style.Numberformat.Format = "#,##0.00";

        //        ws.Column(13).Style.WrapText = true;
        //        ws.Column(13).Style.VerticalAlignment = ExcelVerticalAlignment.Top;
        //        ws.Column(13).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
        //        ws.Column(14).Style.WrapText = true;

        //        ws.Cells["A1"].Value = "Công ty: " + invoice.COMNAME;
        //        ws.Cells["A2"].Value = "MST: " + invoice.COMTAXCODE;
        //        ws.Cells["A3"].Value = "Địa chỉ: " + invoice.COMADDRESS;

        //        ws.Cells["A4:O4"].Value = "BẢNG KÊ HÓA ĐƠN ĐẦU RA";
        //        ws.Cells["A4:O4"].Merge = true;
        //        ws.Cells["A4:O4"].Style.Font.Bold = true;
        //        ws.Cells["A4:O4"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //        ws.Cells["A4:O4"].Style.Font.Size = 12;

        //        ws.Cells["A6:O6"].Style.Font.Bold = true;
        //        ws.Cells["A6:O6"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //        ws.Cells["A6:O6"].Style.WrapText = true;

        //        ws.Cells["A6"].Value = "Mã khách hàng";
        //        ws.Cells["B6"].Value = "MST";
        //        ws.Cells["C6"].Value = "Tên đơn vị";
        //        ws.Cells["D6"].Value = "Số hóa đơn";
        //        ws.Cells["E6"].Value = "Ngày hóa đơn";
        //        ws.Cells["F6"].Value = "Nội dung hóa đơn";
        //        ws.Cells["G6"].Value = "ĐVT";
        //        ws.Cells["H6"].Value = "Số lượng";
        //        ws.Cells["I6"].Value = "Đơn giá";
        //        ws.Cells["J6"].Value = "Thành tiền";
        //        ws.Cells["K6"].Value = "Thuế suất";
        //        ws.Cells["L6"].Value = "Thuế suất GTGT";
        //        ws.Cells["M6"].Value = "Thuế, phí khác";
        //        ws.Cells["N6"].Value = "Ghi chú";
        //        ws.Cells["O6"].Value = "Loại hóa đơn";
        //        // loop through product list to add new line
        //        foreach (InvoiceBO tempInvoice in lstInvoice)
        //        {
        //            //var tempInvoice = lstInvoice[i];
        //            var lstProduct = invoiceBLL.GetInvoiceDetail(tempInvoice.ID);

        //            for (int j = 0; j < lstProduct.Count; j++)
        //            {
        //                var temProduct = lstProduct[j];
        //                var amount = temProduct.RETAILPRICE * temProduct.QUANTITY;
        //                if (temProduct.RETAILPRICE == 0 || temProduct.QUANTITY == 0) // Trường hợp đơn giá, số lượng = 0
        //                {
        //                    amount = temProduct.TOTALMONEY;
        //                }
        //                var taxRate = temProduct.TAXRATE == -1 ? 0 : temProduct.TAXRATE;

        //                ws.Cells[string.Format("A{0}", rowStart)].Value = tempInvoice.CUSID;
        //                ws.Cells[string.Format("B{0}", rowStart)].Value = tempInvoice.CUSTAXCODE;
        //                ws.Cells[string.Format("C{0}", rowStart)].Value = tempInvoice.CUSNAME;
        //                ws.Cells[string.Format("D{0}", rowStart)].Value = tempInvoice.NUMBER == 0 ? "" : tempInvoice.NUMBER.ToString("D7");
        //                ws.Cells[string.Format("E{0}", rowStart)].Value = tempInvoice.SIGNEDTIME.ToString("dd/MM/yyyy") == "01/01/0001" ? "" : tempInvoice.SIGNEDTIME.ToString("dd/MM/yyyy");
        //                ws.Cells[string.Format("F{0}", rowStart)].Value = temProduct.PRODUCTNAME + Environment.NewLine + temProduct.CONSIGNMENTID;
        //                ws.Cells[string.Format("G{0}", rowStart)].Value = temProduct.QUANTITYUNIT;
        //                ws.Cells[string.Format("H{0}", rowStart)].Value = temProduct.QUANTITY;
        //                ws.Cells[string.Format("I{0}", rowStart)].Value = temProduct.RETAILPRICE;
        //                ws.Cells[string.Format("J{0}", rowStart)].Value = amount;
        //                ws.Cells[string.Format("K{0}", rowStart)].Value = temProduct.TAXRATE == -1 ? "\\" : temProduct.TAXRATE.ToString();
        //                ws.Cells[string.Format("L{0}", rowStart)].Value = (amount * (decimal)(taxRate) / 100);
        //                ws.Cells[string.Format("M{0}", rowStart)].Value = temProduct.OTHERTAXFEE;
        //                if (!string.IsNullOrEmpty(tempInvoice.NOTE))
        //                {
        //                    ws.Cells[string.Format("N{0}", rowStart)].Value = (string.IsNullOrEmpty(tempInvoice.NOTE) ? "" : tempInvoice.NOTE.Replace("<br />", "\n"));
        //                }
        //                ws.Cells[string.Format("O{0}", rowStart)].Value = tempInvoice.INVOICETYPENAME;

        //                totalQuantity += temProduct.QUANTITY;
        //                totalAmount += amount;
        //                totalTaxAmount += (amount * (decimal)(taxRate) / 100);
        //                totalOtherTaxFee += temProduct.OTHERTAXFEE;
        //                rowStart++;
        //            }
        //        }
        //        // Total
        //        ws.Cells[string.Format("A{0}", rowStart + 1)].Value = "TỔNG CỘNG";
        //        ws.Cells[string.Format("A{0}:F{0}", rowStart + 1)].Merge = true;
        //        ws.Cells[string.Format("A{0}:F{0}", rowStart + 1)].Style.Font.Bold = true;
        //        ws.Cells[string.Format("A{0}:F{0}", rowStart + 1)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

        //        ws.Cells[string.Format("H{0}", rowStart + 1)].Value = totalQuantity;
        //        ws.Cells[string.Format("J{0}", rowStart + 1)].Value = totalAmount;
        //        ws.Cells[string.Format("L{0}", rowStart + 1)].Value = totalTaxAmount;
        //        ws.Cells[string.Format("M{0}", rowStart + 1)].Value = totalOtherTaxFee;

        //        // custom style for excel file
        //        using (ExcelRange rng = ws.Cells[$"A1:N{(rowStart + 1)}"])
        //        {
        //            rng.Style.Font.Name = "Times New Roman";
        //        }
        //        using (ExcelRange rng = ws.Cells[$"A6:O{(rowStart + 1)}"])
        //        {
        //            rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //            rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        //            rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //            rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //            rng.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
        //            rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
        //            rng.Style.WrapText = true;
        //        }
        //        //ws.Cells[$"B6:B{(rowStart + 1)}"].Style.WrapText = true;

        //        return pck.GetAsByteArray();
        //    }
        //}

        public static byte[] Export2XLSCustomizationForInvoice(List<InvoiceBO> lstInvoice)
        {
            InvoiceBO invoice = new InvoiceBO();
            InvoiceBLL invoiceBLL = new InvoiceBLL();
            if (lstInvoice.Count > 0)
            {
                invoice = lstInvoice[0];
            }
            var rowStart = 7;
            // var totalQuantity = 0;
            decimal totalQuantity = 0;
            decimal totalAmount = 0;
            decimal totalTaxAmount = 0;
            decimal totalOtherTaxFee = 0;

            using (ExcelPackage pck = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet1");
 
                ws.Column(8).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                //ws.Column(8).Style.Numberformat.Format = "#,##0.00";
                ws.Column(10).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                //ws.Column(10).Style.Numberformat.Format = "#,##0.00";
                ws.Column(12).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                //ws.Column(12).Style.Numberformat.Format = "#,##0.00";
                ws.Column(13).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                //ws.Column(13).Style.Numberformat.Format = "#,##0.00";

                ws.Column(13).Style.WrapText = true;
                ws.Column(13).Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                ws.Column(13).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Column(14).Style.WrapText = true;

                ws.Cells["A1"].Value = "Công ty: " + invoice.COMNAME;
                ws.Cells["A2"].Value = "MST: " + invoice.COMTAXCODE;
                ws.Cells["A3"].Value = "Địa chỉ: " + invoice.COMADDRESS;

                ws.Cells["A4:O4"].Value = "BẢNG KÊ HÓA ĐƠN ĐẦU RA";
                ws.Cells["A4:O4"].Merge = true;
                ws.Cells["A4:O4"].Style.Font.Bold = true;
                ws.Cells["A4:O4"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ws.Cells["A4:O4"].Style.Font.Size = 12;

                ws.Cells["A6:O6"].Style.Font.Bold = true;
                ws.Cells["A6:O6"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ws.Cells["A6:O6"].Style.WrapText = true;

                ws.Cells["A6"].Value = "Mã khách hàng";
                ws.Cells["B6"].Value = "MST";
                ws.Cells["C6"].Value = "Tên đơn vị";
                ws.Cells["D6"].Value = "Số hóa đơn";
                ws.Cells["E6"].Value = "Ngày hóa đơn";
                ws.Cells["F6"].Value = "Nội dung hóa đơn";
                ws.Cells["G6"].Value = "ĐVT";
                ws.Cells["H6"].Value = "Số lượng";
                ws.Cells["I6"].Value = "Đơn giá";
                ws.Cells["J6"].Value = "Thành tiền";
                ws.Cells["K6"].Value = "Thuế suất";
                ws.Cells["L6"].Value = "Thuế suất GTGT";
                ws.Cells["M6"].Value = "Thuế, phí khác";
                ws.Cells["N6"].Value = "Ghi chú";
                ws.Cells["O6"].Value = "Loại hóa đơn";
                // loop through product list to add new line
                foreach (InvoiceBO tempInvoice in lstInvoice)
                {
                    //var tempInvoice = lstInvoice[i];
                    var lstProduct = invoiceBLL.GetInvoiceDetail(tempInvoice.ID);

                    for (int j = 0; j < lstProduct.Count; j++)
                    {
                        var temProduct = lstProduct[j];
                        var amount = temProduct.RETAILPRICE * temProduct.QUANTITY;
                        if (temProduct.RETAILPRICE == 0 || temProduct.QUANTITY == 0) // Trường hợp đơn giá, số lượng = 0
                        {
                            amount = temProduct.TOTALMONEY;
                        }
                        var taxRate = temProduct.TAXRATE == -1 ? 0 : temProduct.TAXRATE;

                        ws.Cells[string.Format("A{0}", rowStart)].Value = tempInvoice.CUSID;
                        ws.Cells[string.Format("B{0}", rowStart)].Value = tempInvoice.CUSTAXCODE;
                        ws.Cells[string.Format("C{0}", rowStart)].Value = tempInvoice.CUSNAME;
                        ws.Cells[string.Format("D{0}", rowStart)].Value = tempInvoice.NUMBER == 0 ? "" : tempInvoice.NUMBER.ToString("D7");
                        ws.Cells[string.Format("E{0}", rowStart)].Value = tempInvoice.SIGNEDTIME.ToString("dd/MM/yyyy") == "01/01/0001" ? "" : tempInvoice.SIGNEDTIME.ToString("dd/MM/yyyy");
                        ws.Cells[string.Format("F{0}", rowStart)].Value = temProduct.PRODUCTNAME + Environment.NewLine + temProduct.CONSIGNMENTID;
                        ws.Cells[string.Format("G{0}", rowStart)].Value = temProduct.QUANTITYUNIT;
                        ws.Cells[string.Format("H{0}", rowStart)].Value = temProduct.QUANTITY;
                        ws.Cells[string.Format("I{0}", rowStart)].Value = temProduct.RETAILPRICE;
                        ws.Cells[string.Format("J{0}", rowStart)].Value = amount;
                        ws.Cells[string.Format("K{0}", rowStart)].Value = temProduct.TAXRATE == -1 ? "\\" : temProduct.TAXRATE.ToString();
                        ws.Cells[string.Format("L{0}", rowStart)].Value = (amount * (decimal)(taxRate) / 100);
                        ws.Cells[string.Format("M{0}", rowStart)].Value = temProduct.OTHERTAXFEE;
                        if (!string.IsNullOrEmpty(tempInvoice.NOTE))
                        {
                            ws.Cells[string.Format("N{0}", rowStart)].Value = (string.IsNullOrEmpty(tempInvoice.NOTE) ? "" : tempInvoice.NOTE.Replace("<br />", "\n"));
                        }
                        ws.Cells[string.Format("O{0}", rowStart)].Value = tempInvoice.INVOICETYPENAME;

                        totalQuantity += temProduct.QUANTITY;
                        totalAmount += amount;
                        totalTaxAmount += (amount * (decimal)(taxRate)/ 100);
                        totalOtherTaxFee += temProduct.OTHERTAXFEE;
                        rowStart++;
                    }
                }
                // Total
                ws.Cells[string.Format("A{0}", rowStart + 1)].Value = "TỔNG CỘNG";
                ws.Cells[string.Format("A{0}:F{0}", rowStart + 1)].Merge = true;
                ws.Cells[string.Format("A{0}:F{0}", rowStart + 1)].Style.Font.Bold = true;
                ws.Cells[string.Format("A{0}:F{0}", rowStart + 1)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                ws.Cells[string.Format("H{0}", rowStart + 1)].Value = totalQuantity;
                ws.Cells[string.Format("J{0}", rowStart + 1)].Value = totalAmount;
                ws.Cells[string.Format("L{0}", rowStart + 1)].Value = totalTaxAmount;
                ws.Cells[string.Format("M{0}", rowStart + 1)].Value = totalOtherTaxFee;

                // custom style for excel file
                using (ExcelRange rng = ws.Cells[$"A1:N{(rowStart + 1)}"])
                {
                    rng.Style.Font.Name = "Times New Roman";
                }
                using (ExcelRange rng = ws.Cells[$"A6:O{(rowStart + 1)}"])
                {
                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    rng.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    rng.Style.WrapText = true;
                }
                //ws.Cells[$"B6:B{(rowStart + 1)}"].Style.WrapText = true;

                return pck.GetAsByteArray();
            }
        }

        public static byte[] Export2XLSInvoice(List<InvoiceBO> lstInvoice)
        {
            InvoiceBO invoice = new InvoiceBO();
            InvoiceBLL invoiceBLL = new InvoiceBLL();
            if (lstInvoice.Count > 0)
            {
                invoice = lstInvoice[0];
            }
            var rowStart = 7;
            // var totalQuantity = 0;
            decimal totalQuantity = 0;
            decimal totalAmount = 0;
            decimal totalTaxAmount = 0;
            decimal totalTaxWaterAmount = 0;

            var lstInvoiceExport = lstInvoice.OrderBy(x => x.CUSTOMERCODE).ToList();

            using (ExcelPackage pck = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet1");

                ws.Column(8).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ws.Column(8).Style.Numberformat.Format = "#,##0.00";
                ws.Column(9).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ws.Column(9).Style.Numberformat.Format = "#,##0.00";
                ws.Column(11).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ws.Column(11).Style.Numberformat.Format = "#,##0.00";
                ws.Column(12).Style.WrapText = true;
                ws.Column(12).Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                ws.Column(12).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Column(13).Style.WrapText = true;

                //ws.Cells["A1"].Comment
                ws.Cells["A1"].Value = "Công ty: " + invoice.COMNAME;
                ws.Cells["A2"].Value = "MST: " + invoice.COMTAXCODE;
                ws.Cells["A3"].Value = "Địa chỉ: " + invoice.COMADDRESS;

                ws.Cells["A4:K4"].Value = "BẢNG KÊ HÓA ĐƠN ĐẦU RA";
                ws.Cells["A4:K4"].Merge = true;
                ws.Cells["A4:K4"].Style.Font.Bold = true;
                ws.Cells["A4:K4"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ws.Cells["A4:K4"].Style.Font.Size = 12;

                ws.Cells["A6:M6"].Style.Font.Bold = true;
                ws.Cells["A6:M6"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ws.Cells["A6:M6"].Style.WrapText = true;

                ws.Cells["A6"].Value = "Mã khách";
                ws.Cells["B6"].Value = "Tên đơn vị";
                ws.Cells["C6"].Value = "Số hóa đơn";
                ws.Cells["D6"].Value = "Ngày hóa đơn";
                ws.Cells["E6"].Value = "Nội dung hóa đơn";
                ws.Cells["F6"].Value = "ĐVT";
                ws.Cells["G6"].Value = "Số lượng";
                ws.Cells["H6"].Value = "Đơn giá";
                ws.Cells["I6"].Value = "Thành tiền";
                ws.Cells["J6"].Value = "Thuế suất";
                ws.Cells["K6"].Value = "Thuế suất GTGT";
                ws.Cells["L6"].Value = "Phí BVMT (%)";
                ws.Cells["M6"].Value = "Tổng tiền phí BVMT";
                ws.Cells["N6"].Value = "Ghi chú";
                ws.Cells["O6"].Value = "Loại hóa đơn";
                // loop through product list to add new line
                foreach (InvoiceBO tempInvoice in lstInvoiceExport)
                {
                    //var tempInvoice = lstInvoice[i];
                    var lstProduct = invoiceBLL.GetInvoiceDetail(tempInvoice.ID);

                    for (int j = 0; j < lstProduct.Count; j++)
                    {
                        var temProduct = lstProduct[j];
                        var amount = temProduct.RETAILPRICE * temProduct.QUANTITY;
                        var taxRate = temProduct.TAXRATE == -1 ? 0 : temProduct.TAXRATE;
                        var taxRateWater = temProduct.TAXRATEWATER == -1 ? 0 : temProduct.TAXRATEWATER;

                        ws.Cells[string.Format("A{0}", rowStart)].Value = tempInvoice.CUSTOMERCODE;
                        ws.Cells[string.Format("B{0}", rowStart)].Value = tempInvoice.CUSNAME;
                        ws.Cells[string.Format("C{0}", rowStart)].Value = tempInvoice.NUMBER.ToString("D7");
                        ws.Cells[string.Format("D{0}", rowStart)].Value = tempInvoice.SIGNEDTIME.ToString("dd/MM/yyyy");
                        ws.Cells[string.Format("E{0}", rowStart)].Value = temProduct.PRODUCTNAME;
                        ws.Cells[string.Format("F{0}", rowStart)].Value = temProduct.QUANTITYUNIT;
                        ws.Cells[string.Format("G{0}", rowStart)].Value = temProduct.QUANTITY;
                        ws.Cells[string.Format("H{0}", rowStart)].Value = temProduct.RETAILPRICE;
                        ws.Cells[string.Format("I{0}", rowStart)].Value = amount;
                        ws.Cells[string.Format("J{0}", rowStart)].Value = taxRate;
                        ws.Cells[string.Format("K{0}", rowStart)].Value = (amount * (decimal)(taxRate) / 100);

                        ws.Cells[string.Format("L{0}", rowStart)].Value = taxRateWater;
                        ws.Cells[string.Format("M{0}", rowStart)].Value = (amount * (decimal)(taxRateWater) / 100);

                        if (!string.IsNullOrEmpty(tempInvoice.NOTE))
                        {
                            ws.Cells[string.Format("N{0}", rowStart)].Value = (string.IsNullOrEmpty(tempInvoice.NOTE) ? "" : tempInvoice.NOTE.Replace("<br />", "\n"));
                        }
                        ws.Cells[string.Format("O{0}", rowStart)].Value = tempInvoice.INVOICETYPENAME;

                        totalQuantity += temProduct.QUANTITY;
                        totalAmount += amount;
                        totalTaxAmount += (amount * (decimal)(taxRate) / 100);
                        totalTaxWaterAmount += (amount * (decimal)(taxRateWater) / 100);
                        rowStart++;
                    }
                }
                // Total
                ws.Cells[string.Format("A{0}", rowStart + 1)].Value = "TỔNG CỘNG";
                ws.Cells[string.Format("A{0}:F{0}", rowStart + 1)].Merge = true;
                ws.Cells[string.Format("A{0}:F{0}", rowStart + 1)].Style.Font.Bold = true;
                ws.Cells[string.Format("A{0}:F{0}", rowStart + 1)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                ws.Cells[string.Format("G{0}", rowStart + 1)].Value = totalQuantity;
                ws.Cells[string.Format("I{0}", rowStart + 1)].Value = totalAmount;
                ws.Cells[string.Format("K{0}", rowStart + 1)].Value = totalTaxAmount;
                ws.Cells[string.Format("M{0}", rowStart + 1)].Value = totalTaxWaterAmount;

                // custom style for excel file
                using (ExcelRange rng = ws.Cells[$"A1:L{(rowStart + 1)}"])
                {
                    rng.Style.Font.Name = "Times New Roman";
                }
                using (ExcelRange rng = ws.Cells[$"A6:O{(rowStart + 1)}"])
                {
                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                }

                return pck.GetAsByteArray();
            }
        }
    }
}