export interface CreateObligationRequest {
  descritivoObrigacao: string;
  liquidacaoTipo?: string;
  beneficiarioNome?: string;
  beneficiarioNiss?: string;
  beneficiarioCategoria?: string;
  beneficiarioNomeConta?: string;
  beneficiarioNumeroConta?: string;
  beneficiarioIban?: string;
  beneficiarioSwift?: string;
  beneficiarioBanco?: string;
  beneficiarioMontanteAPagar?: number;
  mes: number;
  ano: number;
}

export interface AddObligationBeneficiaryRequest {
  obligationFk: number;
  niss?: string;
  nomeContribuinte?: string;
  nomeBeneficiario?: string;
  nomeConta?: string;
  numeroConta?: string;
  iban?: string;
  swift?: string;
  banco?: string;
  salarioIliquido?: number;
  cotizacao4?: number;
  imposto10?: number;
  salarioLiquido?: number;
  outrosSuplementos?: number;
  montanteAPagar: number;
}

export interface RemoveObligationBeneficiaryRequest {
  id: number;
}

export interface AddObligationItemRequest {
  obligationFk: number;
  compromissoDespesaFk: number;
  value: number;
}

export interface RemoveObligationItemRequest {
  id: number;
}

export interface SubmitObligationRequest {
  id: number;
}

export interface ApproveObligationRequest {
  id: number;
  approve: boolean;
  comment?: string;
}
