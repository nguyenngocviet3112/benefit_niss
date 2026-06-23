using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class ContaBancariaDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public string Swift { get; set; }

        [Mapper]
        public string EntidadeBancaria { get; set; }

        [Mapper]
        public string Descricao { get; set; }

        [Mapper]
        public string Iban { get; set; }

        [Mapper]
        public string Numero { get; set; }

        public decimal? Saldo { get; set; }
    }
}