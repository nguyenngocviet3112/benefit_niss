using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class FuncionalidadeDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public string Descricao { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual ICollection<RelPerfilFuncionalidadeDto> RelPerfilFuncionalidade { get; set; }
    }
}