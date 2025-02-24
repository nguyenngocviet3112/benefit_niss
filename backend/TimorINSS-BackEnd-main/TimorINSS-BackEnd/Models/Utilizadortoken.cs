using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Utilizadortoken
    {
        public int Id { get; set; }
        public int? IdUtilizador { get; set; }
        public string TokenString { get; set; }
        public string Salt { get; set; }
        public bool? IsRecover { get; set; }
        public int? EntidadeId { get; set; }
    }
}
