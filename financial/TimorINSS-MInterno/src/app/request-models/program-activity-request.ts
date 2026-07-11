export interface SaveProgramActivityRequest {
  id: number;
  codigo: string;
  designacao: string;
  nivel: number;
  parentFk?: number;
  orcamentoConfigFk: number;
}

export interface DeactivateProgramActivityRequest {
  id: number;
}

export interface CopyProgramActivityYearRequest {
  sourceOrcamentoConfigFk: number;
  targetOrcamentoConfigFk: number;
}
