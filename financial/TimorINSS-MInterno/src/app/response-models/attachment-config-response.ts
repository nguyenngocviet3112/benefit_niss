import { ResponseBase } from './utils-response';

export interface AttachmentConfigItem {
  maxFileSizeMb: number;
  adObrigatorio: boolean;
  cabimentoObrigatorio: boolean;
  compromissoObrigatorio: boolean;
  obrigacaoObrigatorio: boolean;
  pagamentoObrigatorio: boolean;
}

export interface AttachmentConfigResponse extends ResponseBase {
  item: AttachmentConfigItem;
}
