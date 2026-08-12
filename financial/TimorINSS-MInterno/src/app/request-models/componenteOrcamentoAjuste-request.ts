export interface SolicitarAjusteOrcamentoRequest {
  rubricaOrigemFk?: number;
  rubricaDestinoFk: number;
  valor: number;
  motivo?: string;
}

export interface AprovarAjusteOrcamentoRequest {
  id: number;
}

export interface RejeitarAjusteOrcamentoRequest {
  id: number;
  motivoRejeicao?: string;
}

export interface GetAjustesOrcamentoRequest {
  componenteOrcamentoRegistoFk: number;
}
