import { OrcamentoImportRowAction } from '../response-models/orcamento-import-response';

export interface OrcamentoImportRowConfirm {
  rowNum: number;
  atividadeFk: number;
  economicClassificationFk: number;
  organizationFk: number;
  valor: number;
  action: OrcamentoImportRowAction;
  existingOrcamentoLinhaId?: number;
}

export interface ConfirmOrcamentoImportRequest {
  orcamentoConfigFk: number;
  rows: OrcamentoImportRowConfirm[];
}
