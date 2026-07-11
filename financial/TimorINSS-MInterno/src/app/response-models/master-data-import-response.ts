import { ResponseBase } from './utils-response';

export interface ImportRowError {
  row: number;
  codigo: string;
  message: string;
}

export interface ImportMasterDataTreeResponse extends ResponseBase {
  total: number;
  success: number;
  failed: number;
  rowErrors: ImportRowError[];
}
