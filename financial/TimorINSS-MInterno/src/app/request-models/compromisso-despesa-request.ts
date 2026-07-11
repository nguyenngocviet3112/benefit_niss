export interface CreateCompromissoDespesaRequest {
  cabimentoFk: number;
  descritivo: string;
  valorCompromissoGlobal: number;
  valorCompromissoAno: number;
  assumidoCom?: 'CONTRATO' | 'LISTA_BENEFICIARIOS' | 'OBRIGACAO';
  mes: number;
  ano: number;
}

export interface SaveCompromissoDespesaPlurianualidadeRequest {
  id: number;
  compromissoDespesaFk: number;
  ano: number;
  valor: number;
}

export interface SubmitCompromissoDespesaRequest {
  id: number;
}

export interface ReviewCompromissoDespesaRequest {
  id: number;
  approve: boolean;
  comment?: string;
}

export interface ApproveCompromissoDespesaRequest {
  id: number;
  approve: boolean;
  comment?: string;
}
