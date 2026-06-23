using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Declaracaoremuneracao
    {
        public int IdDeclaracao { get; set; }
        public decimal DiasContrato { get; set; }
        public decimal DiasEfecTrabalhados { get; set; }
        public int FaltasInjustific { get; set; }
        public int DiasParentalidade { get; set; }
        public decimal DiasTrabcontabSegSocial { get; set; }
        public decimal RemunDeclarada { get; set; }
        public decimal DecimoTerceiro { get; set; }
        public DateTime MesAno { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public bool FlagImportado { get; set; }
        public int ContaCorrenteFk { get; set; }
        public int DeclaracaoRelEntidadeTrabalhadorFk { get; set; }
        public bool Oficioso { get; set; }
        public int NacionalidadeFk { get; set; }
        public int RegimeFk { get; set; }
        public int SexoFk { get; set; }

        public virtual Contacorrente ContaCorrenteFkNavigation { get; set; }
        public virtual Relentidadetrabalhador DeclaracaoRelEntidadeTrabalhadorFkNavigation { get; set; }
        public virtual Dominio NacionalidadeFkNavigation { get; set; }
        public virtual Regime RegimeFkNavigation { get; set; }
        public virtual Dominio SexoFkNavigation { get; set; }
    }
}
