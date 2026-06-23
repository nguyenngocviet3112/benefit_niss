using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class ExcelImporterRel
    {
        public int Id { get; set; }
        public Guid ImportId { get; set; }
        public int RelId { get; set; }
        public string Data { get; set; }
        public DateTime Date { get; set; }
    }
}
