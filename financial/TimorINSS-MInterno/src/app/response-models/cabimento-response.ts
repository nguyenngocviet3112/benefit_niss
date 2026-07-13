import { ResponseBase } from './utils-response';

export interface CabimentoDataContract {
  id: number;
  numero: number;
  mes: number;
  ano: number;
  expenditureAuthorizationFk: number;
  expenditureAuthorizationNumero: number;
  expenditureAuthorizationMes: number;
  atividadeCodigo: string;
  atividadeDesignacao: string;
  economicClassificationCodigo: string;
  economicClassificationDesignacao: string;
  functionalClassificationCodigo?: string;
  functionalClassificationDesignacao?: string;
  organizationNome: string;
  valorAutorizadoAd: number;
  descritivo: string;
  valorCabimentado: number;
  valorComprometido: number;
  saldoDisponivel: number;
  processoAprovisionamentoPrevio?: boolean;
  proposta?: string;
  fundamentacaoLegal?: string;
  estado: 'DRAFT' | 'PENDING_APPROVAL' | 'APPROVED';
  submittedAt?: string;
  approvedAt?: string;
  approveComment?: string;
  lastRejectComment?: string;
  lastRejectAt?: string;
}

export interface AdDisponivelParaCabimentoDataContract {
  expenditureAuthorizationId: number;
  numero: number;
  mes: number;
  atividadeCodigo: string;
  atividadeDesignacao: string;
  economicClassificationCodigo: string;
  economicClassificationDesignacao: string;
  organizationNome: string;
  valorRevisto: number;
}

export interface CabimentoListResponse extends ResponseBase {
  items: CabimentoDataContract[];
}

export interface CabimentoResponse extends ResponseBase {
  item: CabimentoDataContract;
}

export interface AdsDisponiveisParaCabimentoResponse extends ResponseBase {
  items: AdDisponivelParaCabimentoDataContract[];
}
