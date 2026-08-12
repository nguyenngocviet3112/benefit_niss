import { FilterRequest } from './utils-request';

export interface EntidadesRelatorioRequest {
  filter: FilterRequest;
  Ativo?: boolean;
}

export interface SituacaoContributivaEmpresasRelatorioRequest {
  filter: FilterRequest;
  search?: string;
  apenasComDivida?: boolean;
}

export interface ContribuicoesTrendsRelatorioRequest {
  filter: FilterRequest;
}

export interface BudgetExecutionRelatorioRequest {
  filter: FilterRequest;
}

export interface DespesaPipelineRelatorioRequest {
  filter: FilterRequest;
}
