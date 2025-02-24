using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class RelUtilizadorDepartamentoDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int UtilizadorFk { get; set; }

        [Mapper]
        public int DepartamentoFk { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual DepartamentoDto DepartamentoFkNavigation { get; set; }
        public virtual UtilizadorDto UtilizadorFkNavigation { get; set; }
    }
}