import { FilterRequest } from './utils-request';

export interface GetGuiasPendentesValidacaoRequest {
  filter: FilterRequest;
}

export interface ConciliarGuiaPagamentoRequest {
  guiaIds: number[];
  movimentosBancarios: number[];
}
