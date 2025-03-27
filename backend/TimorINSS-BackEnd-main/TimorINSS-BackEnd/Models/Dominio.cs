using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Dominio
    {
        public Dominio()
        {
            Camposeditaveis = new HashSet<Camposeditaveis>();
            Componentecarregardocumento = new HashSet<Componentecarregardocumento>();
            ComponentedespesaRegistoEstadoNavigation = new HashSet<ComponentedespesaRegisto>();
            ComponentedespesaRegistoTipoContaFkNavigation = new HashSet<ComponentedespesaRegisto>();
            ComponentedocumentoRegisto = new HashSet<ComponentedocumentoRegisto>();
            Componenteorcamentovalor = new HashSet<Componenteorcamentovalor>();
            ComponentereceitaRegisto = new HashSet<ComponentereceitaRegisto>();
            ContacorrenteSituacaoPagamentoNavigation = new HashSet<Contacorrente>();
            ContacorrenteTipoDividaNavigation = new HashSet<Contacorrente>();
            DeclaracaoremuneracaoNacionalidadeFkNavigation = new HashSet<Declaracaoremuneracao>();
            DeclaracaoremuneracaoSexoFkNavigation = new HashSet<Declaracaoremuneracao>();
            Documentoidentificacao = new HashSet<Documentoidentificacao>();
            Escalao = new HashSet<Escalao>();
            GuiapagamentoIndPagoNavigation = new HashSet<Guiapagamento>();
            GuiapagamentoTipoGuiaNavigation = new HashSet<Guiapagamento>();
            Movimentobancario = new HashSet<Movimentobancario>();
            Movimentosbancarios = new HashSet<Movimentosbancarios>();
            Movimentosporconciliar = new HashSet<Movimentosporconciliar>();
            Pagamentosexecutados = new HashSet<Pagamentosexecutados>();
            RegimeRegimePaiNavigation = new HashSet<Regime>();
            RegimeTipoRegimeNavigation = new HashSet<Regime>();
            RelentidadetrabalhadorLeiLabAplicavelNavigation = new HashSet<Relentidadetrabalhador>();
            RelentidadetrabalhadorNaturezaContratoNavigation = new HashSet<Relentidadetrabalhador>();
            RelentidadetrabalhadorProfissaoNavigation = new HashSet<Relentidadetrabalhador>();
            RelentidadetrabalhadorRegimeFkNavigation = new HashSet<Relentidadetrabalhador>();
            RelentidadetrabalhadorTipoContratoNavigation = new HashSet<Relentidadetrabalhador>();
            Reltipodecontaorcamentoconfig = new HashSet<Reltipodecontaorcamentoconfig>();
            ResponsavellegalFuncaoNavigation = new HashSet<Responsavellegal>();
            ResponsavellegalNacionalidadeNavigation = new HashSet<Responsavellegal>();
            ResponsavellegalSexoNavigation = new HashSet<Responsavellegal>();
            TrabalhadorEstadoCivilNavigation = new HashSet<Trabalhador>();
            TrabalhadorNacionalidadeTrabalhadorNavigation = new HashSet<Trabalhador>();
            TrabalhadorSexoTrabalhadorNavigation = new HashSet<Trabalhador>();
        }

        public int IdDominio { get; set; }
        public string Dominio1 { get; set; }
        public int Valor { get; set; }
        public string Descricao { get; set; }
        public string DescricaoEn { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual ICollection<Camposeditaveis> Camposeditaveis { get; set; }
        public virtual ICollection<Componentecarregardocumento> Componentecarregardocumento { get; set; }
        public virtual ICollection<ComponentedespesaRegisto> ComponentedespesaRegistoEstadoNavigation { get; set; }
        public virtual ICollection<ComponentedespesaRegisto> ComponentedespesaRegistoTipoContaFkNavigation { get; set; }
        public virtual ICollection<ComponentedocumentoRegisto> ComponentedocumentoRegisto { get; set; }
        public virtual ICollection<Componenteorcamentovalor> Componenteorcamentovalor { get; set; }
        public virtual ICollection<ComponentereceitaRegisto> ComponentereceitaRegisto { get; set; }
        public virtual ICollection<Contacorrente> ContacorrenteSituacaoPagamentoNavigation { get; set; }
        public virtual ICollection<Contacorrente> ContacorrenteTipoDividaNavigation { get; set; }
        public virtual ICollection<Declaracaoremuneracao> DeclaracaoremuneracaoNacionalidadeFkNavigation { get; set; }
        public virtual ICollection<Declaracaoremuneracao> DeclaracaoremuneracaoSexoFkNavigation { get; set; }
        public virtual ICollection<Documentoidentificacao> Documentoidentificacao { get; set; }
        public virtual ICollection<Escalao> Escalao { get; set; }
        public virtual ICollection<Guiapagamento> GuiapagamentoIndPagoNavigation { get; set; }
        public virtual ICollection<Guiapagamento> GuiapagamentoTipoGuiaNavigation { get; set; }
        public virtual ICollection<Movimentobancario> Movimentobancario { get; set; }
        public virtual ICollection<Movimentosbancarios> Movimentosbancarios { get; set; }
        public virtual ICollection<Movimentosporconciliar> Movimentosporconciliar { get; set; }
        public virtual ICollection<Pagamentosexecutados> Pagamentosexecutados { get; set; }
        public virtual ICollection<Regime> RegimeRegimePaiNavigation { get; set; }
        public virtual ICollection<Regime> RegimeTipoRegimeNavigation { get; set; }
        public virtual ICollection<Relentidadetrabalhador> RelentidadetrabalhadorLeiLabAplicavelNavigation { get; set; }
        public virtual ICollection<Relentidadetrabalhador> RelentidadetrabalhadorNaturezaContratoNavigation { get; set; }
        public virtual ICollection<Relentidadetrabalhador> RelentidadetrabalhadorProfissaoNavigation { get; set; }
        public virtual ICollection<Relentidadetrabalhador> RelentidadetrabalhadorRegimeFkNavigation { get; set; }
        public virtual ICollection<Relentidadetrabalhador> RelentidadetrabalhadorTipoContratoNavigation { get; set; }
        public virtual ICollection<Reltipodecontaorcamentoconfig> Reltipodecontaorcamentoconfig { get; set; }
        public virtual ICollection<Responsavellegal> ResponsavellegalFuncaoNavigation { get; set; }
        public virtual ICollection<Responsavellegal> ResponsavellegalNacionalidadeNavigation { get; set; }
        public virtual ICollection<Responsavellegal> ResponsavellegalSexoNavigation { get; set; }
        public virtual ICollection<Trabalhador> TrabalhadorEstadoCivilNavigation { get; set; }
        public virtual ICollection<Trabalhador> TrabalhadorNacionalidadeTrabalhadorNavigation { get; set; }
        public virtual ICollection<Trabalhador> TrabalhadorSexoTrabalhadorNavigation { get; set; }
    }
}
