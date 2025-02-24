using System;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class SuspensoesDto : BaseDto
    {
        [Mapper]
        public int IdSuspensao { get; set; }

        [Mapper]
        public int? EntidadeSuspensaoFk { get; set; }

        [Mapper]
        public DateTime DataInicioSuspensao { get; set; }

        [Mapper]
        public DateTime? DataFimSuspensao { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        [Mapper]
        public int? TrabalhadorSuspensaoFk { get; set; }

        public virtual EntidadeempregadoraDto EntidadeSuspensao { get; set; }
        public virtual TrabalhadorDto TrabalhadorSuspensao { get; set; }
    }
}