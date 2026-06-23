using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class KhachHang
    {
        //public KhachHang()
        //{
        //    KhachHang = new HashSet<KhachHang>();
        //}

        public int Id { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public int DiemTichLuy { get; set; }
        public DateTime NgaySinh { get; set; }
        public virtual ICollection<KhachHang> vKhachHang { get; set; }
    }
}
