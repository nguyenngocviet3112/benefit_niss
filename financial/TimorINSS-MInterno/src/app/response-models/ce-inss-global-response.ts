import { ResponseBase } from './utils-response';

export interface CeInssGlobalRow {
  agrupamentoId: number;
  codigo: string;
  designacao: string;
  valorOrcamentoInicial: number;
  valorOrcamentado: number;
  cabimentos: number;
  compromissos: number;
  totalExecucao: number;
  taxaExecucao: number;
  saldoExecucao: number;
  saldoComprometidoNaoLiquidado: number;
  saldoCabimentadoNaoComprometido: number;
}

export interface CeInssGlobalResponse extends ResponseBase {
  organizationLabel: string;
  receitas: CeInssGlobalRow[];
  despesas: CeInssGlobalRow[];
  totalReceitaInicial: number;
  totalReceitaCorrigido: number;
  totalDespesaInicial: number;
  totalDespesaCorrigido: number;
  saldoOrcamental: number;
}
