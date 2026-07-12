import { ResponseBase } from './utils-response';

export interface OrcamentoSuplementarLinhaDataContract {
  id: number;
  orcamentoLinhaFk: number;
  atividadeCodigo: string;
  atividadeDesignacao: string;
  economicClassificationCodigo: string;
  economicClassificationDesignacao: string;
  organizationNome: string;
  oldValue: number;
  adjustmentValue: number;
  finalValue: number;
}

export interface OrcamentoSuplementarBatchDataContract {
  id: number;
  orcamentoConfigFk: number;
  estado: 'DRAFT' | 'PENDING_REVIEW' | 'PENDING_APPROVAL' | 'APPROVED';
  submittedAt?: string;
  reviewedAt?: string;
  approvedAt?: string;
  lastRejectComment?: string;
  lastRejectAt?: string;
  totalAdjustment: number;
  linhas: OrcamentoSuplementarLinhaDataContract[];
}

export interface RubricaAprovadaParaSuplementarDataContract {
  orcamentoLinhaId: number;
  atividadeCodigo: string;
  atividadeDesignacao: string;
  economicClassificationCodigo: string;
  economicClassificationDesignacao: string;
  organizationNome: string;
  valorAtual: number;
}

export interface OrcamentoSuplementarBatchResponse extends ResponseBase {
  batch: OrcamentoSuplementarBatchDataContract;
}

export interface RubricasAprovadasParaSuplementarResponse extends ResponseBase {
  items: RubricaAprovadaParaSuplementarDataContract[];
}
