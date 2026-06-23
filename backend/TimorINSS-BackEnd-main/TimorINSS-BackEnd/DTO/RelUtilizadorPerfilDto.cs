using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class RelUtilizadorPerfilDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int UtilizadorFk { get; set; }

        [Mapper]
        public int PerfilFk { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual PerfilDto PerfilFkNavigation { get; set; }
        public virtual UtilizadorDto UtilizadorFkNavigation { get; set; }
    }
}