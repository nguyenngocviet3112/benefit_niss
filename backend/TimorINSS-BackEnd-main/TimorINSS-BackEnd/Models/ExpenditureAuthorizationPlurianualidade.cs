using System;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class ExpenditureAuthorizationPlurianualidade
    {
        public int Id { get; set; }
        public int ExpenditureAuthorizationFk { get; set; }
        public int Ano { get; set; }
        public decimal Valor { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual ExpenditureAuthorization ExpenditureAuthorizationFkNavigation { get; set; }
    }
}
