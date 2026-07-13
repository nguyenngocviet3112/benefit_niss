using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;

namespace TimorINSSBackEnd.Extensions
{
    // Helper dùng chung để xuất Excel cho các báo cáo mode mới — 1 sheet, dòng
    // tiêu đề in đậm, cột tự co giãn theo nội dung. Không dùng lớp "ExcelDocument"
    // cũ (gắn với IStringLocalizer resource-based, chỉ old-mode dùng) — mode mới
    // đã dùng ngx-translate, tiêu đề cột truyền thẳng bằng chuỗi đã dịch sẵn.
    public static class ExcelExportHelper
    {
        public static string BuildXlsxBase64(string sheetName, string[] headers, IEnumerable<object[]> rows)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(sheetName);

            for (int c = 0; c < headers.Length; c++)
            {
                ws.Cell(1, c + 1).Value = headers[c];
            }
            ws.Row(1).Style.Font.Bold = true;
            ws.Row(1).Style.Fill.BackgroundColor = XLColor.FromHtml("#F5F5F5");

            int r = 2;
            foreach (var row in rows)
            {
                for (int c = 0; c < row.Length; c++)
                {
                    var cell = ws.Cell(r, c + 1);
                    switch (row[c])
                    {
                        case null:
                            break;
                        case decimal dec:
                            cell.Value = dec;
                            cell.Style.NumberFormat.Format = "#,##0.00";
                            break;
                        case int i:
                            cell.Value = i;
                            break;
                        case DateTime dt:
                            cell.Value = dt;
                            cell.Style.DateFormat.Format = "dd/MM/yyyy";
                            break;
                        default:
                            cell.Value = row[c].ToString();
                            break;
                    }
                }
                r++;
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return Convert.ToBase64String(ms.ToArray());
        }
    }
}
