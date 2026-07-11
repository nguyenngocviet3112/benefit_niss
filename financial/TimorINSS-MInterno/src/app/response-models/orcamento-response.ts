import { ResponseBase } from './utils-response';

export interface OrcamentoLinhaDataContract {
  id: number;
  atividadeFk: number;
  atividadeCodigo: string;
  atividadeDesignacao: string;
  economicClassificationFk: number;
  economicClassificationCodigo: string;
  economicClassificationDesignacao: string;
  functionalClassificationFk?: number;
  functionalClassificationCodigo?: string;
  functionalClassificationDesignacao?: string;
  organizationFk: number;
  organizationNome: string;
  valor: number;
  indActivo: boolean;
}

export interface OrcamentoBatchDataContract {
  id: number;
  orcamentoConfigFk: number;
  estado: 'DRAFT' | 'PENDING_REVIEW' | 'PENDING_APPROVAL' | 'APPROVED';
  submittedAt?: string;
  reviewedAt?: string;
  approvedAt?: string;
  lastRejectComment?: string;
  lastRejectAt?: string;
  totalValor: number;
  linhas: OrcamentoLinhaDataContract[];
}

export interface OrcamentoBatchResponse extends ResponseBase {
  batch: OrcamentoBatchDataContract;
}
