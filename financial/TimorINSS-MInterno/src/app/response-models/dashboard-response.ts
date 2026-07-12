import { ResponseBase } from './utils-response';

export interface ProcessSummaryDataContract {
  total: number;
  emProcessamento: number;
  aprovado: number;
}

export interface DashboardSummaryDataContract {
  ad: ProcessSummaryDataContract;
  cabimento: ProcessSummaryDataContract;
  compromisso: ProcessSummaryDataContract;
  obrigacao: ProcessSummaryDataContract;
  pagamentoAutorizacao: ProcessSummaryDataContract;
  pagamentoExecutado: number;
  bancoConciliado: number;
  bancoPendente: number;
}

export interface DashboardSummaryResponse extends ResponseBase {
  summary: DashboardSummaryDataContract;
}
