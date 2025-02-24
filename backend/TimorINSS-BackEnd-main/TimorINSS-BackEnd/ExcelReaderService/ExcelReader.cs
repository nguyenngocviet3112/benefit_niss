using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace TimorINSSBackEnd.ExcelReaderService
{
    public static class ExcelReader
    {
        public static List<T> Read<T>(Stream file)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
                PrepareHeaderForMatch = args => args.Header.Trim().ToLower(),
                Encoding = System.Text.Encoding.UTF8,
                ShouldSkipRecord = args => args.Row.Parser.Record.All(string.IsNullOrWhiteSpace)
            };

            using var reader = new StreamReader(file, false);
            using var csv = new CsvReader(reader, config);
            var records = csv.GetRecords<T>().ToList();

            return records;
        }
    }
}
