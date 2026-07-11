export interface OrcamentoConfigDataContract {
  id: number;
  ano: number;
  tipo: string;
  dataInicio: string;
  dataFim?: string;
  indActivo: boolean;
}

export interface OrcamentoConfigListResponse {
  items: OrcamentoConfigDataContract[];
}
