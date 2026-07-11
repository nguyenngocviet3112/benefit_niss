export interface DepartamentoConfigDataContract {
  id: number;
  nome: string;
  institutionId?: number;
  institutionNome?: string;
  indActivo: boolean;
}

export interface DepartamentoConfigListResponse {
  items: DepartamentoConfigDataContract[];
}

export interface DepartamentoConfigResponse {
  item: DepartamentoConfigDataContract;
  errors?: { errorCode: string; errorMessage: string }[];
}
