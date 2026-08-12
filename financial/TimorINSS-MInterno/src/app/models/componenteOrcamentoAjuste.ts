export interface ComponenteOrcamentoAjuste {
  id: number;
  componenteOrcamentoRegistoFk: number;
  rubricaOrigemFk?: number;
  rubricaOrigemDescricao?: string;
  rubricaDestinoFk: number;
  rubricaDestinoDescricao?: string;
  valor: number;
  estado: string;
  motivo?: string;
  motivoRejeicao?: string;
  utilizadorSolicitacao: number;
  utilizadorSolicitacaoNome?: string;
  dataSolicitacao: Date;
  utilizadorAprovacao?: number;
  utilizadorAprovacaoNome?: string;
  dataAprovacao?: Date;
}

export interface RubricaDisponivel {
  id: number;
  descricao: string;
  valor: number;
  saldoDisponivel: number;
}
