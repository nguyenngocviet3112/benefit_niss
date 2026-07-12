import { ResponseBase } from './utils-response';

export interface CicloDespesaRow {
  adId: number;
  numeroAd: number;
  regimeCodigo: string;
  regimeDesignacao: string;
  atividadeCodigo: string;
  atividadeDesignacao: string;
  classificacaoEconomicaCodigo: string;
  classificacaoEconomicaDesignacao: string;
  classificacaoFuncionalCodigo: string | null;
  classificacaoFuncionalDesignacao: string | null;
  cabimentos: number;
  compromissos: number;
  saldo1: number;
  obrigacoes: number;
  saldo2: number;
  pagamentos: number;
  saldo3: number;
}

export interface CicloDespesaListResponse extends ResponseBase {
  items: CicloDespesaRow[];
}
