using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class DispensaContributivaDto : BaseDto
    {
        [Mapper]
        public int IdDispContributiva { get; set; }

        [Mapper]
        public int Ano { get; set; }

        [Mapper]
        public decimal Percentagem { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }
    }
}