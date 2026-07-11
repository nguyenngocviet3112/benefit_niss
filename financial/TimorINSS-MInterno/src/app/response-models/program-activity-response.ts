export interface ProgramActivityDataContract {
  id: number;
  codigo: string;
  designacao: string;
  nivel: number;
  parentFk?: number;
  orcamentoConfigFk: number;
  indActivo: boolean;
  hasKids: boolean;
}

export interface ProgramActivityTreeResponse {
  items: ProgramActivityDataContract[];
}
