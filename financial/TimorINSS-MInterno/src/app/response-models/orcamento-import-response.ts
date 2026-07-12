import { ResponseBase } from './utils-response';

export type OrcamentoImportRowStatus = 'New' | 'Exists' | 'Error';
export type OrcamentoImportRowAction = 'Insert' | 'Overwrite' | 'Skip';

export interface OrcamentoImportRow {
  rowNum: number;
  atividadeCodigo: string;
  atividadeDesignacao?: string;
  economicClassificationCodigo: string;
  economicClassificationDesignacao?: string;
  organizationNome: string;
  valor: number;
  status: OrcamentoImportRowStatus;
  errorMessage?: string;
  existingOrcamentoLinhaId?: number;
  existingValor?: number;
  atividadeFk?: number;
  economicClassificationFk?: number;
  organizationFk?: number;
  // Chỉ dùng ở FE — quyết định của người dùng cho dòng Exists (mặc định Skip
  // theo CLAUDE.md §6), dòng New luôn ngầm định Insert.
  action?: OrcamentoImportRowAction;
}

export interface ImportOrcamentoPreviewResponse extends ResponseBase {
  rows: OrcamentoImportRow[];
  totalNew: number;
  totalExists: number;
  totalErrors: number;
}
