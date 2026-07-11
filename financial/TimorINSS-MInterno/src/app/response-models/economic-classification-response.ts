export interface EconomicClassificationDataContract {
  id: number;
  codigo: string;
  designacao: string;
  nivel: number;
  parentFk?: number;
  orcamentoConfigFk: number;
  indActivo: boolean;
  hasKids: boolean;
  tipo?: string;
}

export interface EconomicClassificationTreeResponse {
  items: EconomicClassificationDataContract[];
}
