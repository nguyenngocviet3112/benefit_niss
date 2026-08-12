using System;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class ComponenteOrcamentoAjusteDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int ComponenteOrcamentoRegistoFk { get; set; }

        [Mapper]
        public int? RubricaOrigemFk { get; set; }

        [Mapper]
        public int RubricaDestinoFk { get; set; }

        [Mapper]
        public decimal Valor { get; set; }

        [Mapper]
        public string Estado { get; set; }

        [Mapper]
        public string Motivo { get; set; }

        [Mapper]
        public string MotivoRejeicao { get; set; }

        [Mapper]
        public int UtilizadorSolicitacao { get; set; }

        [Mapper]
        public DateTime DataSolicitacao { get; set; }

        [Mapper]
        public int? UtilizadorAprovacao { get; set; }

        [Mapper]
        public DateTime? DataAprovacao { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }
    }
}
