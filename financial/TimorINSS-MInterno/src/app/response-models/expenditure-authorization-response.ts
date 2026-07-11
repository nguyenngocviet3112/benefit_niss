import { ResponseBase } from './utils-response';

export interface ExpenditureAuthorizationPlurianualidadeDataContract {
  id: number;
  ano: number;
  valor: number;
}

export interface ExpenditureAuthorizationDataContract {
  id: number;
  numero: number;
  mes: number;
  ano: number;
  orcamentoLinhaFk: number;
  atividadeCodigo: string;
  atividadeDesignacao: string;
  economicClassificationCodigo: string;
  economicClassificationDesignacao: string;
  organizationNome: string;
  rubricaValor: number;
  descritivo: string;
  valorAutorizado: number;
  regularizacao: number;
  valorRevisto: number;
  estado: 'DRAFT' | 'PENDING_REVIEW' | 'PENDING_APPROVAL' | 'APPROVED';
  submittedAt?: string;
  reviewedAt?: string;
  approvedAt?: string;
  lastRejectComment?: string;
  lastRejectAt?: string;
  plurianualidade: ExpenditureAuthorizationPlurianualidadeDataContract[];
}

export interface RubricaDisponivelDataContract {
  orcamentoLinhaId: number;
  atividadeCodigo: string;
  atividadeDesignacao: string;
  economicClassificationCodigo: string;
  economicClassificationDesignacao: string;
  organizationNome: string;
  valor: number;
}

export interface ExpenditureAuthorizationListResponse extends ResponseBase {
  items: ExpenditureAuthorizationDataContract[];
}

export interface ExpenditureAuthorizationResponse extends ResponseBase {
  item: ExpenditureAuthorizationDataContract;
}

export interface AvailableRubricasResponse extends ResponseBase {
  items: RubricaDisponivelDataContract[];
}
