using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class ComponenteOrcamentoValorDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int ComponenteOrcamentoRegistoFk { get; set; }

        [Mapper]
        public int? DepartamentoFk { get; set; }

        [Mapper]
        public int? InstitutionId { get; set; }

        [Mapper]
        public int? CentroCustoFk { get; set; }

        [Mapper]
        public int? AgrupamentoFk { get; set; }
        [Mapper]
        public int? ActidadeFk { get; set; }
        [Mapper]
        public int? EconomicFk { get; set; }
        [Mapper]
        public int? FuncionalFk { get; set; }

        [Mapper]
        public decimal Valor { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        [Mapper]
        public int? TipoContaFk { get; set; }

        public virtual AgrupamentoConfigDto AgrupamentoFkNavigation { get; set; }
        public virtual CentroCustoDto CentroCustoFkNavigation { get; set; }
        public virtual ComponenteOrcamentoRegistoDto ComponenteOrcamentoRegistoFkNavigation { get; set; }
        public virtual DepartamentoDto DepartamentoFkNavigation { get; set; }
        public virtual DominioDto TipoContaFkNavigation { get; set; }
    }

    public class ExcTractOrcamentoValor
    {
        public int AgrupamentoId { get; set; }
        public string CentrosCusto { get; set; }
        public string TipoConta { get; set; }
        public string Departamento { get; set; }
        public string Agrupamento { get; set; }
        public string SubAgrupamento { get; set; }
        public string Rubrica { get; set; }
        public string Alinea { get; set; }
        public string SubAlinea { get; set; }
        public string Designacao { get; set; }
        public decimal Valor { get; set; }
        public int DepartamentoFk { get; set; }
        public int CentroCustoFk { get; set; }
        public int TipoContaFk { get; set; }
    }
}