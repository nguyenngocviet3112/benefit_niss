export interface LanguageConfigDataContract {
  id: number;
  codigo: string;
  nome: string;
  ordem: number;
  indActivo: boolean;
}

export interface LanguageConfigListResponse {
  items: LanguageConfigDataContract[];
}
