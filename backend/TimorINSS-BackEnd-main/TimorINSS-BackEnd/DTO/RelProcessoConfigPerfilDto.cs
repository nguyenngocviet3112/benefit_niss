using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class RelprocessoconfigperfilDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int ProcessoConfigFk { get; set; }

        [Mapper]
        public int PerfilFk { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual PerfilDto PerfilFkNavigation { get; set; }
        public virtual ProcessoconfigDto ProcessoConfigFkNavigation { get; set; }
    }
}