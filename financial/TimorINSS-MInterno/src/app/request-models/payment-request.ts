export interface CreatePaymentAuthorizationRequest {
  obligationFk: number;
  descritivo?: string;
  valorAutorizado: number;
  codigoContaDebitoFk?: number;
  codigoContaCreditoFk?: number;
  mes: number;
  ano: number;
}

export interface SubmitPaymentAuthorizationRequest {
  id: number;
}

export interface ApprovePaymentAuthorizationRequest {
  id: number;
  approve: boolean;
  comment?: string;
}

export interface ExecutePaymentRequest {
  paymentAuthorizationFk: number;
  dataPagamento: string;
  contaBancariaFk?: number;
  numeroDocumento?: string;
  observacao?: string;
}
