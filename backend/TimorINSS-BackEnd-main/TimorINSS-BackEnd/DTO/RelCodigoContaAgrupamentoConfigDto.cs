using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class RelCodigoContaAgrupamentoConfigDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int CodigocontaFk { get; set; }

        [Mapper]
        public int AgrupamentoConfigFk { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual AgrupamentoConfigDto AgrupamentoFkNavigation { get; set; }
        public virtual CodigoContaDto CodigocontaFkNavigation { get; set; }
    }
}