using System;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    // Ajuste ao orçamento de uma rubrica já Aprovada, sem passar pela Retificação (que substitui
    // o envelope inteiro e fica bloqueada assim que existam despesas executadas no período -- ver
    // bug/limitação confirmada em 2026-08-11). Cobre 2 casos com a mesma tabela:
    //  - RubricaOrigemFk == null  -> Bonificação/reforço de orçamento (soma directo à RubricaDestinoFk)
    //  - RubricaOrigemFk != null  -> Transferência entre rubricas (subtrai da origem, soma ao destino)
    // Fluxo em 2 passos: Solicitar (Estado = Pendente) -> Aprovar/Rejeitar.
    public partial class ComponenteOrcamentoAjuste
    {
        public int Id { get; set; }
        public int ComponenteOrcamentoRegistoFk { get; set; }
        public int? RubricaOrigemFk { get; set; }
        public int RubricaDestinoFk { get; set; }
        public decimal Valor { get; set; }
        public string Estado { get; set; }
        public string Motivo { get; set; }
        public string MotivoRejeicao { get; set; }
        public int UtilizadorSolicitacao { get; set; }
        public DateTime DataSolicitacao { get; set; }
        public int? UtilizadorAprovacao { get; set; }
        public DateTime? DataAprovacao { get; set; }
        public bool IndActivo { get; set; }
    }
}
