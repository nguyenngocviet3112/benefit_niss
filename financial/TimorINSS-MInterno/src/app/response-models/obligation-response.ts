import { ResponseBase } from './utils-response';

export interface ObligationItemDataContract {
  id: number;
  compromissoDespesaFk: number;
  compromissoDespesaNumero: number;
  atividadeCodigo: string;
  economicClassificationCodigo: string;
  economicClassificationDesignacao: string;
  compromissoValorRevisto: number;
  compromissoSaldoDisponivel: number;
  value: number;
}

export interface ObligationDataContract {
  id: number;
  numero: number;
  mes: number;
  ano: number;
  descritivoObrigacao: string;
  valorObrigacao: number;
  estado: 'DRAFT' | 'PENDING_APPROVAL' | 'APPROVED';
  submittedAt?: string;
  approvedAt?: string;
  lastRejectComment?: string;
  lastRejectAt?: string;
  items: ObligationItemDataContract[];
}

export interface CompromissoComSaldoDataContract {
  compromissoDespesaId: number;
  numero: number;
  atividadeCodigo: string;
  atividadeDesignacao: string;
  economicClassificationCodigo: string;
  economicClassificationDesignacao: string;
  organizationNome: string;
  valorRevisto: number;
  saldoDisponivel: number;
}

export interface ObligationListResponse extends ResponseBase {
  items: ObligationDataContract[];
}

export interface ObligationResponse extends ResponseBase {
  item: ObligationDataContract;
}

export interface CompromissosComSaldoResponse extends ResponseBase {
  items: CompromissoComSaldoDataContract[];
}
