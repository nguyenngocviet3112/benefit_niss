import { ResponseBase } from './utils-response';

export interface CompromissoDespesaPlurianualidadeDataContract {
  id: number;
  ano: number;
  valor: number;
}

export interface CompromissoDespesaDataContract {
  id: number;
  numero: number;
  mes: number;
  ano: number;
  cabimentoFk: number;
  cabimentoNumero: number;
  expenditureAuthorizationNumero: number;
  atividadeCodigo: string;
  atividadeDesignacao: string;
  economicClassificationCodigo: string;
  economicClassificationDesignacao: string;
  organizationNome: string;
  valorCabimentado: number;
  descritivo: string;
  valorCompromissoGlobal: number;
  valorCompromissoAno: number;
  regularizacao: number;
  valorRevisto: number;
  valorObrigado: number;
  saldoDisponivel: number;
  assumidoCom?: 'CONTRATO' | 'LISTA_BENEFICIARIOS' | 'OBRIGACAO';
  estado: 'DRAFT' | 'PENDING_REVIEW' | 'PENDING_APPROVAL' | 'APPROVED';
  submittedAt?: string;
  reviewedAt?: string;
  approvedAt?: string;
  lastRejectComment?: string;
  lastRejectAt?: string;
  plurianualidade: CompromissoDespesaPlurianualidadeDataContract[];
}

export interface CabimentoDisponivelParaCompromissoDataContract {
  cabimentoId: number;
  numero: number;
  atividadeCodigo: string;
  atividadeDesignacao: string;
  economicClassificationCodigo: string;
  economicClassificationDesignacao: string;
  organizationNome: string;
  valorCabimentado: number;
}

export interface CompromissoDespesaListResponse extends ResponseBase {
  items: CompromissoDespesaDataContract[];
}

export interface CompromissoDespesaResponse extends ResponseBase {
  item: CompromissoDespesaDataContract;
}

export interface CabimentosDisponiveisResponse extends ResponseBase {
  items: CabimentoDisponivelParaCompromissoDataContract[];
}
