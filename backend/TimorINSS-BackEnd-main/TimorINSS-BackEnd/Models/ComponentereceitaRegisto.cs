using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class ComponentereceitaRegisto
    {
        public ComponentereceitaRegisto()
        {
            ComponentereceitaRegistoMovimentos = new HashSet<ComponentereceitaRegistoMovimentos>();
        }

        public int Id { get; set; }
        public int TarefaActivoFk { get; set; }
        public int? DepartamentoFk { get; set; }
        public int CentroCustoFk { get; set; }
        public int TipoContaFk { get; set; }
        public int CodigoContaFk { get; set; }
        public int AgrupamentoConfigFk { get; set; }
        public int? InstitutionId { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public bool IndActivo { get; set; }
        public int ComponenteOrcamentoRegistoFk { get; set; }
        public int? CodigoContaDebitoFk { get; set; }

        public virtual Agrupamentoconfig AgrupamentoConfigFkNavigation { get; set; }
        public virtual Institution InstitutionFkNavigation { get; set; }
        public virtual Centrocusto CentroCustoFkNavigation { get; set; }
        public virtual Codigoconta CodigoContaDebitoFkNavigation { get; set; }
        public virtual Codigoconta CodigoContaFkNavigation { get; set; }
        public virtual ComponenteorcamentoRegisto ComponenteOrcamentoRegistoFkNavigation { get; set; }
        public virtual Departamento DepartamentoFkNavigation { get; set; }
        public virtual Tarefaativo TarefaActivoFkNavigation { get; set; }
        public virtual Dominio TipoContaFkNavigation { get; set; }
        public virtual ICollection<ComponentereceitaRegistoMovimentos> ComponentereceitaRegistoMovimentos { get; set; }
    }
}
