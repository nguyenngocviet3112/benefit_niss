export interface SaveOrcamentoConfigRequest {
  id: number;
  ano: number;
  dataInicio: string;
  dataFim?: string;
}

export interface DeactivateOrcamentoConfigRequest {
  id: number;
}
