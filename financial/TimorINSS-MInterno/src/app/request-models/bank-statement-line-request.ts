export interface AddBankStatementLineRequest {
  contaBancariaFk: number;
  dataValor: string;
  dataTransacao?: string;
  codigoTransacaoBancaria?: string;
  descricao?: string;
  credito: number;
  debito: number;
}

export interface DeleteBankStatementLineRequest {
  id: number;
}

export interface MatchReceitaRequest {
  id: number;
  receitaPacFk: number;
}

export interface MatchPagamentoRequest {
  id: number;
  paymentExecutionFk: number;
}

export interface UnmatchRequest {
  id: number;
}

export interface BankStatementLineImportRowConfirmRequest {
  rowNum: number;
  dataValor: string;
  descricao: string;
  credito: number;
  debito: number;
  action: 'Insert' | 'Skip';
}

export interface ConfirmBankStatementLineImportRequest {
  contaBancariaFk: number;
  rows: BankStatementLineImportRowConfirmRequest[];
}
