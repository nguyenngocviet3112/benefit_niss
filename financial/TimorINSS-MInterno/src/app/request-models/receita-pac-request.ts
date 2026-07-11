export interface SaveReceitaPacRequest {
  id: number;
  mes: number;
  ano: number;
  niss?: string;
  regimeFk: number;
  atividadeFk?: number;
  economicClassificationFk: number;
  organizationFk: number;
  descritivo: string;
  valorPac: number;
  valorCobradoBanco: number;
  valorCobradoCaixa: number;
}

export interface DeactivateReceitaPacRequest {
  id: number;
}
