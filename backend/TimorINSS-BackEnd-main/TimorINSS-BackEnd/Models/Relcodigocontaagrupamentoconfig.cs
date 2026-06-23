using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Relcodigocontaagrupamentoconfig
    {
        public int Id { get; set; }
        public int CodigocontaFk { get; set; }
        public int AgrupamentoConfigFk { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Agrupamentoconfig AgrupamentoConfigFkNavigation { get; set; }
        public virtual Codigoconta CodigocontaFkNavigation { get; set; }
    }
}
