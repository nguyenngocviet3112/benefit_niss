export interface SaveOrcamentoLinhaRequest {
  id: number;
  orcamentoConfigFk: number;
  atividadeFk: number;
  economicClassificationFk: number;
  functionalClassificationFk?: number;
  organizationFk: number;
  valor: number;
}

export interface DeleteOrcamentoLinhaRequest {
  id: number;
}

export interface SubmitOrcamentoBatchRequest {
  orcamentoConfigFk: number;
}

export interface ReviewOrcamentoBatchRequest {
  batchId: number;
  approve: boolean;
  comment?: string;
}

export interface ApproveOrcamentoBatchRequest {
  batchId: number;
  approve: boolean;
  comment?: string;
}
