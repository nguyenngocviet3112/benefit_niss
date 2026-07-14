import { ResponseBase } from './utils-response';

export interface BankStatementLineDataContract {
  id: number;
  contaBancariaFk: number;
  contaBancariaNome: string;
  entidadeBancaria?: string;
  dataValor: string;
  dataTransacao?: string;
  codigoTransacaoBancaria?: string;
  descricao?: string;
  credito: number;
  debito: number;
  isConciliado: boolean;
  receitaPacFk?: number;
  receitaPacDescricao?: string;
  paymentExecutionFk?: number;
  paymentExecutionDescricao?: string;
  conciliadoAt?: string;
}

export interface ReceitaDisponivelParaConciliacaoDataContract {
  receitaPacId: number;
  numero: number;
  mes: number;
  ano: number;
  descritivo: string;
  valorPac: number;
  valorCobradoBanco: number;
}

export interface PagamentoDisponivelParaConciliacaoDataContract {
  paymentExecutionId: number;
  obligationNumero: number;
  obligationDescritivo?: string;
  dataPagamento: string;
  numeroDocumento?: string;
  valorAutorizado: number;
}

export interface BankStatementLineListResponse extends ResponseBase {
  items: BankStatementLineDataContract[];
}

export interface ReceitasDisponiveisParaConciliacaoResponse extends ResponseBase {
  items: ReceitaDisponivelParaConciliacaoDataContract[];
}

export interface PagamentosDisponiveisParaConciliacaoResponse extends ResponseBase {
  items: PagamentoDisponivelParaConciliacaoDataContract[];
}

export interface BankStatementLineImportRow {
  rowNum: number;
  dataValor: string;
  descricao: string;
  credito: number;
  debito: number;
  isDuplicate: boolean;
  // Chỉ dùng ở frontend (không tới từ backend) — người dùng chọn cho từng dòng.
  action?: 'Insert' | 'Skip';
}

export interface ImportBankStatementLinePreviewResponse extends ResponseBase {
  rows: BankStatementLineImportRow[];
}
