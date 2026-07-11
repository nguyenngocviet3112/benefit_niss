using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;

namespace TimorINSSBackEnd.ExcelReaderService
{
    // Đọc workbook mẫu master-data: sheet "Instructions" (bỏ qua) + sheet dữ liệu
    // (sheet thứ 2), dòng 1 = header, dòng 2 = caption in nghiêng (bỏ qua), dữ
    // liệu thật từ dòng 3. Trả về danh sách dòng dạng Dictionary theo header đã
    // trim, để mỗi entity tự map cột theo tên riêng của nó.
    public static class MasterDataTreeExcelReader
    {
        public static List<Dictionary<string, string>> ReadDataSheet(IFormFile file)
        {
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();

            using (Stream stream = file.OpenReadStream())
            using (XLWorkbook workbook = new XLWorkbook(stream))
            {
                IXLWorksheet worksheet = workbook.Worksheets.Count > 1
                    ? workbook.Worksheet(2)
                    : workbook.Worksheet(1);

                IXLRow headerRow = worksheet.Row(1);
                int lastColumn = worksheet.LastColumnUsed().ColumnNumber();
                List<string> headers = new List<string>();
                for (int col = 1; col <= lastColumn; col++)
                {
                    headers.Add(headerRow.Cell(col).GetString().Trim());
                }

                int lastRow = worksheet.LastRowUsed().RowNumber();
                // Dòng 2 là caption in nghiêng (mô tả cột) — bỏ qua, dữ liệu bắt đầu dòng 3.
                for (int rowNum = 3; rowNum <= lastRow; rowNum++)
                {
                    IXLRow row = worksheet.Row(rowNum);
                    if (row.IsEmpty())
                        continue;

                    Dictionary<string, string> rowDict = new Dictionary<string, string>();
                    for (int col = 1; col <= lastColumn; col++)
                    {
                        string header = headers[col - 1];
                        if (string.IsNullOrEmpty(header))
                            continue;
                        rowDict[header] = row.Cell(col).GetString().Trim();
                    }

                    if (rowDict.Count == 0)
                        continue;

                    rows.Add(rowDict);
                }
            }

            return rows;
        }
    }
}
