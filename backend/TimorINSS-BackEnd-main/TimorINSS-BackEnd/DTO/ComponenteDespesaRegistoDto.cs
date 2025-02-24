using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class ComponenteDespesaRegistoDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int TarefaActivoFk { get; set; }

        [Mapper]
        public int? DepartamentoFk { get; set; }

        [Mapper]
        public int CentroCustoFk { get; set; }

        [Mapper]
        public int TipoContaFk { get; set; }

        [Mapper]
        public int CodigoContaFk { get; set; }

        [Mapper]
        public int AgrupamentoConfigFk { get; set; }

        [Mapper]
        public string Descricao { get; set; }

        [Mapper]
        public decimal Valor { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        [Mapper]
        public int Estado { get; set; }

        public virtual AgrupamentoConfigDto AgrupamentoConfigFkNavigation { get; set; }
        public virtual CentroCustoDto CentroCustoFkNavigation { get; set; }
        public virtual CodigoContaDto CodigoContaFkNavigation { get; set; }
        public virtual DepartamentoDto DepartamentoFkNavigation { get; set; }
        public virtual TarefaativoDto TarefaActivoFkNavigation { get; set; }
        public virtual DominioDto TipoContaFkNavigation { get; set; }
    }
}