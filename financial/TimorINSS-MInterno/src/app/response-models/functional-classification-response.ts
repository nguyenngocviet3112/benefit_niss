export interface FunctionalClassificationDataContract {
  id: number;
  codigo: string;
  designacao: string;
  parentFk?: number;
  indActivo: boolean;
  hasKids: boolean;
}

export interface FunctionalClassificationTreeResponse {
  items: FunctionalClassificationDataContract[];
}
