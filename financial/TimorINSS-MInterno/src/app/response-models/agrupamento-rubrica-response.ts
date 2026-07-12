export interface AgrupamentoRubricaItemDataContract {
  id: number;
  codigo: string;
  designacao: string;
  parentFk?: number;
  orcamentoConfigFk: number;
  tipoConta: string;
  indActivo: boolean;
  hasKids: boolean;
}

export interface AgrupamentoRubricaTreeResponse {
  items: AgrupamentoRubricaItemDataContract[];
}
