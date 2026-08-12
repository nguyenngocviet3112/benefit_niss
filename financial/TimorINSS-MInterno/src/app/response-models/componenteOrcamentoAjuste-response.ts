import { ComponenteOrcamentoAjuste, RubricaDisponivel } from "../models/componenteOrcamentoAjuste";

export interface SolicitarAjusteOrcamentoResponse {
  id: number;
}

export interface GetAjustesOrcamentoResponse {
  ajustes: ComponenteOrcamentoAjuste[];
}

export interface GetRubricasDisponiveisResponse {
  componenteOrcamentoRegistoFk?: number;
  rubricas: RubricaDisponivel[];
}
