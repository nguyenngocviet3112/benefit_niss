using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Inssestrangeiro
    {
        public int IdInssestrang { get; set; }
        public int? EstrangeiroEntidadeFk { get; set; }
        public int? EstrangeiroTrabalhadorFk { get; set; }
        public string NomeSsestrangeiro { get; set; }
        public int EstrangeiroPaisFk { get; set; }
        public bool IndDecontAtualmente { get; set; }
        public bool IndBenfAtualmente { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public byte[] Documento { get; set; }
        public string NomeDocumento { get; set; }
        public string Nissestrangeiro { get; set; }

        public virtual Entidadeempregadora EstrangeiroEntidadeFkNavigation { get; set; }
        public virtual Pais EstrangeiroPaisFkNavigation { get; set; }
        public virtual Trabalhador EstrangeiroTrabalhadorFkNavigation { get; set; }
    }
}
