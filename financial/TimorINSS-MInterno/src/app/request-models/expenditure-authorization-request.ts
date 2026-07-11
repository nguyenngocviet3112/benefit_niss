export interface CreateExpenditureAuthorizationRequest {
  orcamentoLinhaFk: number;
  descritivo: string;
  valorAutorizado: number;
  mes: number;
  ano: number;
}

export interface SaveExpenditureAuthorizationRequest {
  id: number;
  descritivo: string;
  valorAutorizado: number;
  regularizacao: number;
}

export interface SavePlurianualidadeRequest {
  id: number;
  expenditureAuthorizationFk: number;
  ano: number;
  valor: number;
}

export interface DeletePlurianualidadeRequest {
  id: number;
}

export interface SubmitExpenditureAuthorizationRequest {
  id: number;
}

export interface ReviewExpenditureAuthorizationRequest {
  id: number;
  approve: boolean;
  comment?: string;
}

export interface ApproveExpenditureAuthorizationRequest {
  id: number;
  approve: boolean;
  comment?: string;
}
