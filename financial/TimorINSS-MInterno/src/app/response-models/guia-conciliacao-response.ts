import { ResponseBase } from './utils-response';

export interface GuiaComprovativoDataContract {
  idGuia: number;
  numDocumento: string;
  dataUpload?: string;
  dataPagamento?: string;
  valorPago?: number;
  bankCode: string;
  comprovativoPag?: string;
}

export interface GuiaComprovativoResponse extends ResponseBase {
  guia: GuiaComprovativoDataContract;
}
