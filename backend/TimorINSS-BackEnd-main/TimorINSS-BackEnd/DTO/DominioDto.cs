using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class DominioDto : BaseDto
    {
        [Mapper]
        public int IdDominio { get; set; }

        [Mapper]
        public string Dominio1 { get; set; }

        [Mapper]
        public int Valor { get; set; }

        [Mapper]
        public string Descricao { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual ICollection<ContacorrenteDto> ContacorrenteSituacaoPagamentoNavigation { get; set; }
        public virtual ICollection<ContacorrenteDto> ContacorrenteTipoDividaNavigation { get; set; }
        public virtual ICollection<DocumentoidentificacaoDto> DocumentoidentificacaoNavigation { get; set; }
        public virtual ICollection<EscalaoDto> EscalaoNavigation { get; set; }
        public virtual ICollection<GuiapagamentoDto> GuiapagamentoIndPagoNavigation { get; set; }
        public virtual ICollection<GuiapagamentoDto> GuiapagamentoTipoGuiaNavigation { get; set; }
        public virtual ICollection<RegimeDto> RegimeRegimePaiNavigation { get; set; }
        public virtual ICollection<RegimeDto> RegimeTipoRegimeNavigation { get; set; }
        public virtual ICollection<RelentidadetrabalhadorDto> RelentidadetrabalhadorLeiLabAplicavelNavigation { get; set; }
        public virtual ICollection<RelentidadetrabalhadorDto> RelentidadetrabalhadorNaturezaContratoNavigation { get; set; }
        public virtual ICollection<RelentidadetrabalhadorDto> RelentidadetrabalhadorRegimeFkNavigation { get; set; }
        public virtual ICollection<RelentidadetrabalhadorDto> RelentidadetrabalhadorTipoContratoNavigation { get; set; }
        public virtual ICollection<RelTipoDeContaOrcamentoConfigDto> ReltipodecontaorcamentoconfigNavigation { get; set; }
        public virtual ICollection<ResponsavellegalDto> ResponsavellegalNacionalidadeNavigation { get; set; }
        public virtual ICollection<ResponsavellegalDto> ResponsavellegalSexoNavigation { get; set; }
        public virtual ICollection<TrabalhadorDto> TrabalhadorEstadoCivilNavigation { get; set; }
        public virtual ICollection<TrabalhadorDto> TrabalhadorNacionalidadeTrabalhadorNavigation { get; set; }
        public virtual ICollection<TrabalhadorDto> TrabalhadorSexoTrabalhadorNavigation { get; set; }

        public virtual ICollection<CamposEditaveisDto> CamposeditaveisNavigation { get; set; }
    }
}