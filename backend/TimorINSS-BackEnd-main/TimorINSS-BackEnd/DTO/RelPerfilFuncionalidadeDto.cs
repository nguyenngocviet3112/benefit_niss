using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class RelPerfilFuncionalidadeDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int PerfilFk { get; set; }

        [Mapper]
        public int FuncionalidadeFk { get; set; }

        [Mapper]
        public bool Create { get; set; }

        [Mapper]
        public bool Read { get; set; }

        [Mapper]
        public bool Update { get; set; }

        [Mapper]
        public bool Delete { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual FuncionalidadeDto FuncionalidadeFkNavigation { get; set; }
        public virtual PerfilDto PerfilFkNavigation { get; set; }
    }
}