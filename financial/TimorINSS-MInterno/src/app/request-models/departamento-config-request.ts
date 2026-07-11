export interface SaveDepartamentoConfigRequest {
  id: number;
  nome: string;
  institutionId?: number;
}

export interface DeactivateDepartamentoConfigRequest {
  id: number;
}
