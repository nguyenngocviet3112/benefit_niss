using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace TimorINSSBackEnd.ExcelReaderService
{
    public static class ExcelReader
    {
        public static List<T> Read<T>(Stream file)
        {
            // Ficheiros .xlsx começam com a assinatura ZIP "PK\x03\x04" — detectamos pelo
            // conteúdo (não pela extensão) para não depender do nome enviado pelo cliente.
            var signature = new byte[4];
            var bytesRead = file.Read(signature, 0, 4);
            file.Position = 0;
            bool isXlsx = bytesRead == 4 && signature[0] == 0x50 && signature[1] == 0x4B && signature[2] == 0x03 && signature[3] == 0x04;

            Stream sourceStream = isXlsx ? ConvertXlsxToCsvStream(file) : file;

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
                PrepareHeaderForMatch = args => args.Header.Trim().ToLower(),
                Encoding = System.Text.Encoding.UTF8,
                ShouldSkipRecord = args => args.Row.Parser.Record.All(string.IsNullOrWhiteSpace)
            };

            using var reader = new StreamReader(sourceStream, false);
            using var csv = new CsvReader(reader, config);
            var records = csv.GetRecords<T>().ToList();

            return records;
        }

        private static Stream ConvertXlsxToCsvStream(Stream xlsxFile)
        {
            using var workbook = new XLWorkbook(xlsxFile);
            var worksheet = workbook.Worksheets.First();
            var usedRange = worksheet.RangeUsed();

            var csvBuilder = new StringBuilder();
            if (usedRange != null)
            {
                foreach (var row in usedRange.Rows())
                {
                    var fields = row.Cells(usedRange.FirstColumn().ColumnNumber(), usedRange.LastColumn().ColumnNumber())
                        .Select(cell => EscapeCsvField(GetCellText(cell)));
                    csvBuilder.AppendLine(string.Join(",", fields));
                }
            }

            return new MemoryStream(Encoding.UTF8.GetBytes(csvBuilder.ToString()));
        }

        private static string GetCellText(IXLCell cell)
        {
            return cell.DataType switch
            {
                XLDataType.Number => cell.GetDouble().ToString(CultureInfo.InvariantCulture),
                XLDataType.DateTime => cell.GetDateTime().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                _ => cell.GetString().Trim()
            };
        }

        private static string EscapeCsvField(string value)
        {
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }
            return value;
        }
    }
}
