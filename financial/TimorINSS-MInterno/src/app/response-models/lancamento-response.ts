export interface LancamentoDataContract {
  id: number;
  data: string;
  descricao: string;
  codigoContaDebitoFk: number;
  codigoContaDebitoCodigo: string;
  codigoContaDebitoDesignacao: string;
  codigoContaCreditoFk: number;
  codigoContaCreditoCodigo: string;
  codigoContaCreditoDesignacao: string;
  valor: number;
  origemTipo: string;
  origemId?: number;
}

export interface LancamentoListResponse {
  items: LancamentoDataContract[];
}
