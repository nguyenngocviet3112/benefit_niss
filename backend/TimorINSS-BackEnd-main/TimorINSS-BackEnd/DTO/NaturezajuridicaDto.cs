using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class NaturezajuridicaDto : BaseDto
    {
        [Mapper]
        public int IdNatJuridica { get; set; }

        [Mapper]
        public string Codigo { get; set; }

        [Mapper]
        public string Descricao { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual ICollection<EntidadeempregadoraDto> Entidadeempregadora { get; set; }
    }
}