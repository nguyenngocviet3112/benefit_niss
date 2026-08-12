export interface EntidadeRelatorioListagem {
  id: number;
  nome: string;
  niss: string;
  tin: string;
  dtInscricao: Date;
  ativo: boolean;
  totalTrabalhadores: number;
  totalMasculino: number;
  totalFeminino: number;
}

export interface EntidadesRelatorioResponse {
  rows: number;
  entidades: EntidadeRelatorioListagem[];
  errors?: any[];
}

export interface SituacaoContributivaEmpresaListagem {
  idEntidade: number;
  nomeEmpregador: string;
  niss: string;
  ultimoMesPago?: Date;
  mesesEmDivida: Date[];
  totalDivida: number;
}

export interface SituacaoContributivaEmpresasRelatorioResponse {
  rows: number;
  empresas: SituacaoContributivaEmpresaListagem[];
  errors?: any[];
}

export interface ContribuicoesTrendMes {
  mesAno: Date;
  valorPago: number;
  valorDivida: number;
  novosRegistos: number;
}

export interface ContribuicoesTrendsRelatorioResponse {
  meses: ContribuicoesTrendMes[];
  errors?: any[];
}

export interface RubricaOrcamentoListagem {
  agrupamentoConfigFk: number;
  descricao: string;
  departamento: string;
  valorOrcado: number;
  valorReservado: number;
  saldoDisponivel: number;
}

export interface BudgetExecutionRelatorioResponse {
  totalOrcado: number;
  totalCabimentado: number;
  totalCompromissado: number;
  totalPago: number;
  rubricasEmRisco: RubricaOrcamentoListagem[];
  errors?: any[];
}

export interface DespesaPipelineEstagio {
  estagio: string;
  ordem: number;
  total: number;
}

export interface DespesaPipelineRelatorioResponse {
  estagios: DespesaPipelineEstagio[];
  errors?: any[];
}
