using CsvHelper.Configuration.Attributes;
using System;
using System.Globalization;

namespace TimorINSSBackEnd.ExcelReaderService.Models
{
    public class MovimentoBancario
    {
        [Name("Description")]
        public string Description { get; set; }

        [Name("Amount")]
        public decimal? Amount { get; set; }

        [Name("Date (dd/mm/yyyy)")]
        public string Date
        {
            set
            {
                _Date = DateTime.ParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
        }
        [Ignore]
        public DateTime? _Date { get; set; }
    }
}
