import { ResponseBase } from './utils-response';

export interface PaymentExecutionDataContract {
  id: number;
  dataPagamento: string;
  contaBancariaFk?: number;
  contaBancariaNome?: string;
  numeroDocumento?: string;
  observacao?: string;
  executedAt: string;
}

export interface PaymentAuthorizationDataContract {
  id: number;
  numero: number;
  mes: number;
  ano: number;
  obligationFk: number;
  obligationNumero: number;
  obligationDescritivo?: string;
  valorObrigacao: number;
  descritivo?: string;
  valorAutorizado: number;
  codigoContaDebitoFk?: number;
  codigoContaDebitoDesignacao?: string;
  codigoContaCreditoFk?: number;
  codigoContaCreditoDesignacao?: string;
  estado: 'DRAFT' | 'PENDING_APPROVAL' | 'APPROVED';
  submittedAt?: string;
  approvedAt?: string;
  lastRejectComment?: string;
  lastRejectAt?: string;
  execution?: PaymentExecutionDataContract;
}

export interface ObligacaoDisponivelParaPagamentoDataContract {
  obligationId: number;
  numero: number;
  descritivoObrigacao?: string;
  valorObrigacao: number;
}

export interface CodigoContaOptionDataContract {
  id: number;
  designacao: string;
}

export interface ContaBancariaOptionDataContract {
  id: number;
  entidadeBancaria: string;
  numero: string;
}

export interface PaymentAuthorizationListResponse extends ResponseBase {
  items: PaymentAuthorizationDataContract[];
}

export interface PaymentAuthorizationResponse extends ResponseBase {
  item: PaymentAuthorizationDataContract;
}

export interface ObligacoesDisponiveisParaPagamentoResponse extends ResponseBase {
  items: ObligacaoDisponivelParaPagamentoDataContract[];
}

export interface CodigoContaOptionsResponse extends ResponseBase {
  items: CodigoContaOptionDataContract[];
}

export interface ContaBancariaOptionsResponse extends ResponseBase {
  items: ContaBancariaOptionDataContract[];
}
