import { ResponseBase } from './utils-response';

export interface MeuPerfilResponse extends ResponseBase {
  username: string;
  nome: string;
  email: string;
  departamentoFk?: number;
  departamentoNome: string;
}
