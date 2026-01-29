using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Movimentosporconciliar
    {
        public Movimentosporconciliar()
        {
            RelMovimentosporconciliarMovimentos = new HashSet<RelMovimentosporconciliarMovimentos>();
        }

        public int Id { get; set; }
        public int TarefaAtivoFk { get; set; }
        public int? MovimentoBancarioFk { get; set; }
        public bool IsReceita { get; set; }
        public decimal Valor { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public byte[] Comprovativo { get; set; }
        public string NomeComprovativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public bool? IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public int? CodigoContaDebitoFk { get; set; }
        public int? CodigoContaCreditoFk { get; set; }
        public int? DepartamentoFk { get; set; }
        public int? InstitutionId { get; set; }
        public int? CentroCustoFk { get; set; }
        public int? TipoContaFk { get; set; }
        public int? AgrupamentoConfigFk { get; set; }
        public int? GuiapagamentoId { get; set; }
        public int? ReservaCreditoId { get; set; }

        public virtual Agrupamentoconfig AgrupamentoConfigFkNavigation { get; set; }
        public virtual Centrocusto CentroCustoFkNavigation { get; set; }
        public virtual Codigoconta CodigoContaCreditoFkNavigation { get; set; }
        public virtual Codigoconta CodigoContaDebitoFkNavigation { get; set; }
        public virtual Departamento DepartamentoFkNavigation { get; set; }
        public virtual Guiapagamento Guiapagamento { get; set; }
        public virtual Movimentobancario MovimentoBancarioFkNavigation { get; set; }
        public virtual Reservacredito ReservaCredito { get; set; }
        public virtual Tarefaativo TarefaAtivoFkNavigation { get; set; }
        public virtual Dominio TipoContaFkNavigation { get; set; }
        public virtual ICollection<RelMovimentosporconciliarMovimentos> RelMovimentosporconciliarMovimentos { get; set; }
    }
}
