using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class ContactoDto : BaseDto
    {
        [Mapper]
        public int IdContacto { get; set; }

        [Mapper]
        public int? ContactoTrabalhadorFk { get; set; }

        [Mapper]
        public int? ContactoEntidadeFk { get; set; }

        [Mapper]
        public string Telemovel { get; set; }

        [Mapper]
        public string Email { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        [Mapper]
        public bool FlagImportado { get; set; }

        public virtual EntidadeempregadoraDto ContactoEntidadeFkNavigation { get; set; }
        public virtual TrabalhadorDto ContactoTrabalhadorFkNavigation { get; set; }
    }
}