using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class InssestrangeiroDto : BaseDto
    {
        [Mapper]
        public int IdInssestrang { get; set; }

        [Mapper]
        public int? EstrangeiroEntidadeFk { get; set; }

        [Mapper]
        public int? EstrangeiroTrabalhadorFk { get; set; }

        [Mapper]
        public string NomeSsestrangeiro { get; set; }

        [Mapper]
        public int EstrangeiroPaisFk { get; set; }

        [Mapper]
        public bool IndDecontAtualmente { get; set; }

        [Mapper]
        public bool IndBenfAtualmente { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        [Mapper]
        public byte[] Documento { get; set; }

        [Mapper]
        public string NomeDocumento { get; set; }

        [Mapper]
        public string Nissestrangeiro { get; set; }

        public virtual Entidadeempregadora EstrangeiroEntidade { get; set; }
        public virtual Pais EstrangeiroPais { get; set; }
        public virtual Trabalhador EstrangeiroTrabalhador { get; set; }
    }
}