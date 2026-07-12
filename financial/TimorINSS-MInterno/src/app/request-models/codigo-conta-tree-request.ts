export interface SaveCodigoContaRequest {
  id: number;
  codigo: string;
  designacao: string;
  parentFk?: number;
  orcamentoConfigFk: number;
}

export interface DeactivateCodigoContaRequest {
  id: number;
}
