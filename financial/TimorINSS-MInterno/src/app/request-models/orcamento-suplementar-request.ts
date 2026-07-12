export interface SaveOrcamentoSuplementarLinhaRequest {
  id: number;
  orcamentoConfigFk: number;
  orcamentoLinhaFk: number;
  adjustmentValue: number;
}

export interface DeleteOrcamentoSuplementarLinhaRequest {
  id: number;
}

export interface SubmitOrcamentoSuplementarRequest {
  orcamentoConfigFk: number;
}

export interface ReviewOrcamentoSuplementarRequest {
  batchId: number;
  approve: boolean;
  comment?: string;
}

export interface ApproveOrcamentoSuplementarRequest {
  batchId: number;
  approve: boolean;
  comment?: string;
}
