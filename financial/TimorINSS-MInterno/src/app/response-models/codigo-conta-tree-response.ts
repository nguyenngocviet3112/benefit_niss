export interface CodigoContaTreeItemDataContract {
  id: number;
  codigo: string;
  designacao: string;
  parentFk?: number;
  orcamentoConfigFk: number;
  indActivo: boolean;
  hasKids: boolean;
}

export interface CodigoContaTreeResponse {
  items: CodigoContaTreeItemDataContract[];
}
