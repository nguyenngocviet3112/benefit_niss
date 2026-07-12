export interface SaveAgrupamentoRubricaRequest {
  id: number;
  codigo: string;
  designacao: string;
  parentFk?: number;
  orcamentoConfigFk: number;
  tipoConta: string;
}

export interface DeactivateAgrupamentoRubricaRequest {
  id: number;
}
