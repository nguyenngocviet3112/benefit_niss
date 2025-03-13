using System;
using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class KhachHangDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public string HoTen { get; set; }

        [Mapper]
        public string Email { get; set; }

        [Mapper]
        public string DiemTichLuy { get; set; }
        [Mapper]
        public DateTime NgaySinh { get; set; }

        public virtual ICollection<KhachHangDto> Entidadeempregadora { get; set; }
    }
}