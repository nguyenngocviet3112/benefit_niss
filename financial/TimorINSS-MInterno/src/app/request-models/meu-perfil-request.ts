export interface AlterarEmailRequest {
  email: string;
}

export interface AlterarSenhaRequest {
  senhaAtual: string;
  senhaNova: string;
  confirmarSenhaNova: string;
}
