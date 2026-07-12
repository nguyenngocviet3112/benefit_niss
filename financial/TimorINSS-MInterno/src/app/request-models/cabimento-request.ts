export interface CreateCabimentoRequest {
  expenditureAuthorizationFk: number;
  descritivo: string;
  valorCabimentado: number;
  processoAprovisionamentoPrevio?: boolean;
  mes: number;
  ano: number;
}

export interface SaveCabimentoRequest {
  id: number;
  descritivo: string;
  valorCabimentado: number;
  processoAprovisionamentoPrevio?: boolean;
}

export interface SubmitCabimentoRequest {
  id: number;
}

export interface ApproveCabimentoRequest {
  id: number;
  approve: boolean;
  comment?: string;
}
