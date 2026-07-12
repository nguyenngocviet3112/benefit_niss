import { ResponseBase } from './utils-response';

export interface CodigoContaOpeningBalanceDataContract {
  id: number;
  codigo: string;
  designacao: string;
  initialValue?: number;
  isCredit?: boolean;
  initialValueDate?: string;
}

export interface CodigoContaOpeningBalanceListResponse extends ResponseBase {
  items: CodigoContaOpeningBalanceDataContract[];
}
