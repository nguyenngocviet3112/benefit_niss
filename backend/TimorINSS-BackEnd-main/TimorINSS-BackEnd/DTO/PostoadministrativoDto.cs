using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class PostoadministrativoDto : BaseDto
    {
        [Mapper]
        public int IdPostoAdmin { get; set; }

        [Mapper]
        public int PostoAdminMunicipioFk { get; set; }

        [Mapper]
        public string Nome { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual MunicipioDto PostoAdminMunicipioFkNavigation { get; set; }
        public virtual ICollection<SucoDto> Suco { get; set; }
    }
}