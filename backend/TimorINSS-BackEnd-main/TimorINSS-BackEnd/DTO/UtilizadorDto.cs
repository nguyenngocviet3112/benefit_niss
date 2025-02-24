using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class UtilizadorDto : BaseDto
    {
        [Mapper]
        public int IdUtilizador { get; set; }

        [Mapper]
        public int UtilizadorEntidadeFk { get; set; }

        [Mapper]
        public string Username { get; set; }

        [Mapper]
        public string Password { get; set; }

        [Mapper]
        public string Salt { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        [Mapper]
        public int LoginAttempts { get; set; }

        [Mapper]
        public bool Locked { get; set; }

        public EntidadeempregadoraDto EntidadeEmpregadora { get; set; }
    }
}