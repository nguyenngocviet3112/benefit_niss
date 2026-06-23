using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class DestinatarioDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int? EntidadeFk { get; set; }

        [Mapper]
        public int? TrabalhadorFk { get; set; }

        [Mapper]
        public string Nome { get; set; }

        [Mapper]
        public string Niss { get; set; }

        [Mapper]
        public string Tin { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        [Mapper]
        public string Morada { get; set; }

        public virtual EntidadeempregadoraDto EntidadeFkNavigation { get; set; }
        public virtual TrabalhadorDto TrabalhadorFkNavigation { get; set; }
        public virtual ICollection<PagamentosExecutadosDto> Pagamentosexecutados { get; set; }
    }
}