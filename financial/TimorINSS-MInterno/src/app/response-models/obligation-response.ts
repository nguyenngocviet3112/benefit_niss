import { ResponseBase } from './utils-response';

export interface ObligationItemDataContract {
  id: number;
  compromissoDespesaFk: number;
  compromissoDespesaNumero: number;
  atividadeCodigo: string;
  economicClassificationCodigo: string;
  economicClassificationDesignacao: string;
  compromissoValorRevisto: number;
  compromissoSaldoDisponivel: number;
  value: number;
}

export interface ObligationBeneficiaryDataContract {
  id: number;
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

export type LiquidacaoTipo = 'SALARIOS' | 'PENSOES' | 'SUBSIDIOS_IMEDIATOS' | 'DESPESAS_SEM_CONTRATO' | 'OUTRAS_DESPESAS_CONTRATO';
export type BeneficiarioCategoria = 'FORNECEDOR' | 'BENEFICIARIO' | 'CONTRIBUINTE_EE' | 'PESSOAL' | 'OUTRO';

export interface ObligationDataContract {
  id: number;
  numero: number;
  mes: number;
  ano: number;
  descritivoObrigacao: string;
  valorObrigacao: number;
  liquidacaoTipo?: LiquidacaoTipo;
  beneficiarioNome?: string;
  beneficiarioNiss?: string;
  beneficiarioCategoria?: BeneficiarioCategoria;
  beneficiarioNomeConta?: string;
  beneficiarioNumeroConta?: string;
  beneficiarioIban?: string;
  beneficiarioSwift?: string;
  beneficiarioBanco?: string;
  beneficiarioMontanteAPagar?: number;
  estado: 'DRAFT' | 'PENDING_APPROVAL' | 'APPROVED';
  submittedAt?: string;
  approvedAt?: string;
  lastRejectComment?: string;
  lastRejectAt?: string;
  items: ObligationItemDataContract[];
  beneficiaries: ObligationBeneficiaryDataContract[];
}

export interface CompromissoComSaldoDataContract {
  compromissoDespesaId: number;
  numero: number;
  atividadeCodigo: string;
  atividadeDesignacao: string;
  economicClassificationCodigo: string;
  economicClassificationDesignacao: string;
  organizationNome: string;
  valorRevisto: number;
  saldoDisponivel: number;
}

export interface ObligationListResponse extends ResponseBase {
  items: ObligationDataContract[];
}

export interface ObligationResponse extends ResponseBase {
  item: ObligationDataContract;
}

export interface CompromissosComSaldoResponse extends ResponseBase {
  items: CompromissoComSaldoDataContract[];
}
