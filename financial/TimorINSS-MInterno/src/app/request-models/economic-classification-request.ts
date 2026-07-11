export interface SaveEconomicClassificationRequest {
  id: number;
  codigo: string;
  designacao: string;
  nivel: number;
  parentFk?: number;
  orcamentoConfigFk: number;
  tipo?: string;
}

export interface DeactivateEconomicClassificationRequest {
  id: number;
}
