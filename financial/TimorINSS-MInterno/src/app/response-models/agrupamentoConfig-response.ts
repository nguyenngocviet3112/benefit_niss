import { AgrupamentosConfig } from '../models/agrupamentosConfig';



export interface AgrupamentoConfigResponse {
  agrupamentos: AgrupamentosConfig[];
}



export interface RelatoriosExecucaoOrcamentalListagemResponse {
  rows: number;
  lista: RelatoriosExecucaoOrcamentalListagem[];
}

export interface RelatoriosExecucaoOrcamentalListagem {
  instiutiton: string;
  contaOGE: string;
  centrosCusto: string[];
  rubricas: string[];
  valorOrcamentoInicial: number;
  valorOrcamentado: number;
  cabimentos: number;
  compromissos: number;
  obrigacoes: number;
  valorAnoAnterior: number;
  janeiro: number;
  fevereiro: number;
  marco: number;
  abril: number;
  maio: number;
  junho: number;
  julho: number;
  agosto: number;
  setembro: number;
  outubro: number;
  novembro: number;
  dezembro: number;
  totalExecucao: number;
  taxaExecucao: number;
  variacaoExecucao: number;
}

export interface ClassificacaoEconomicaExecucaoListagemResponse {
  lista: ClassificacaoEconomicaExecucaoListagem[];
}

export interface ClassificacaoEconomicaExecucaoListagem {
  codigoCE: string;
  designacaoCE: string;
  nivel: number;
  valorOrcamentoInicial: number;
  valorOrcamentado: number;
  janeiro: number;
  fevereiro: number;
  marco: number;
  abril: number;
  maio: number;
  junho: number;
  julho: number;
  agosto: number;
  setembro: number;
  outubro: number;
  novembro: number;
  dezembro: number;
  totalExecucao: number;
  taxaExecucao: number;
  cabimentos: number;
  compromissos: number;
  obrigacoes: number;
  saldoDisponivel: number;
  saldoNaoComprometido: number;
  valorCabimentadoNaoComprometido: number;
  valorComprometidoNaoLiquidado: number;
  valorLiquidadoNaoPago: number;
  receitaLiquidada: number;
  saldoReceitaLiquidadaNaoCobrada: number;
  saldoExecucao: number;
}

