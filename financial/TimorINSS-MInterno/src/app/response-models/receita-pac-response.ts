export interface ReceitaPacDataContract {
  id: number;
  numero: number;
  mes: number;
  ano: number;
  niss?: string;
  regimeFk: number;
  regimeDesignacao?: string;
  atividadeFk?: number;
  atividadeCodigo?: string;
  atividadeDesignacao?: string;
  economicClassificationFk: number;
  economicClassificationCodigo?: string;
  economicClassificationDesignacao?: string;
  organizationFk: number;
  organizationNome?: string;
  descritivo: string;
  valorPac: number;
  valorCobradoBanco: number;
  valorCobradoCaixa: number;
  contaBancariaFk?: number;
  contaBancariaNome?: string;
  codigoContaDebitoFk?: number;
  codigoContaDebitoDesignacao?: string;
  codigoContaCreditoFk?: number;
  codigoContaCreditoDesignacao?: string;
  valorCobradoTotal: number;
  saldoPorCobrar: number;
}

export interface ReceitaPacListResponse {
  items: ReceitaPacDataContract[];
}

export interface ReceitaPacResponse {
  item: ReceitaPacDataContract;
  errors?: { errorCode: string; errorMessage: string }[];
}
