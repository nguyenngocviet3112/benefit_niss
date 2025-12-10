using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Componenteorcamentovalor
    {
        public int Id { get; set; }
        public int ComponenteOrcamentoRegistoFk { get; set; }
        public int? DepartamentoFk { get; set; }
        public int? InstitutionId { get; set; }

        public int? CentroCustoFk { get; set; }
        public int? AgrupamentoFk { get; set; }
        public int? ActidadeFk { get; set; }
        public int? EconomicFk { get; set; }
        public int? FuncionalFk { get; set; }

        public decimal Valor { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public int? TipoContaFk { get; set; }



        public virtual Agrupamentoconfig AgrupamentoFkNavigation { get; set; }
        public virtual Centrocusto CentroCustoFkNavigation { get; set; }
        public virtual ComponenteorcamentoRegisto ComponenteOrcamentoRegistoFkNavigation { get; set; }
        public virtual Departamento DepartamentoFkNavigation { get; set; }
        public virtual Institution InstitutionFkNavigation { get; set; }
        public virtual Dominio TipoContaFkNavigation { get; set; }
    }
}
