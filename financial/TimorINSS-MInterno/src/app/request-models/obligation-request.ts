export interface CreateObligationRequest {
  descritivoObrigacao: string;
  mes: number;
  ano: number;
}

export interface AddObligationItemRequest {
  obligationFk: number;
  compromissoDespesaFk: number;
  value: number;
}

export interface RemoveObligationItemRequest {
  id: number;
}

export interface SubmitObligationRequest {
  id: number;
}

export interface ApproveObligationRequest {
  id: number;
  approve: boolean;
  comment?: string;
}
