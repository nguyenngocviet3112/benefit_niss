using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class SucoDto : BaseDto
    {
        [Mapper]
        public int IdSuco { get; set; }

        [Mapper]
        public int SucoPostoAdminFk { get; set; }

        [Mapper]
        public string Nome { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual PostoadministrativoDto SucoPostoAdminFkNavigation { get; set; }
        public virtual ICollection<AldeiaDto> Aldeia { get; set; }
    }
}