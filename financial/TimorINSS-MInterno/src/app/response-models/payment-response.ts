import { ResponseBase } from './utils-response';
import { ObligationBeneficiaryDataContract } from './obligation-response';

export interface PaymentExecutionDataContract {
  id: number;
  dataPagamento: string;
  // Tài khoản ngân hàng NỘI BỘ của INSS (nguồn chi) — khác với tài khoản
  // người thụ hưởng (xem beneficiario* trên PaymentAuthorizationDataContract).
  contaBancariaFk?: number;
  contaBancariaNome?: string;
  numeroDocumento?: string;
  observacao?: string;
  executedAt: string;
}

export interface PaymentAuthorizationDataContract {
  id: number;
  numero: number;
  mes: number;
  ano: number;
  obligationFk: number;
  obligationNumero: number;
  obligationMes: number;
  obligationDescritivo?: string;
  valorObrigacao: number;
  // Thông tin người thụ hưởng — echo read-only từ Obligation (không nhập lại
  // ở Pagamento). Với các Obligation dùng danh sách (Beneficiário/Pessoal),
  // các field đơn lẻ dưới đây rỗng, dùng beneficiaryList thay thế.
  beneficiarioNome?: string;
  beneficiarioCategoria?: string;
  beneficiarioNomeConta?: string;
  beneficiarioNumeroConta?: string;
  beneficiarioIban?: string;
  beneficiarioSwift?: string;
  beneficiarioBanco?: string;
  beneficiarioMontanteAPagar?: number;
  beneficiaryList: ObligationBeneficiaryDataContract[];
  descritivo?: string;
  valorAutorizado: number;
  codigoContaDebitoFk?: number;
  codigoContaDebitoDesignacao?: string;
  codigoContaCreditoFk?: number;
  codigoContaCreditoDesignacao?: string;
  estado: 'DRAFT' | 'PENDING_APPROVAL' | 'APPROVED';
  submittedAt?: string;
  approvedAt?: string;
  lastRejectComment?: string;
  lastRejectAt?: string;
  execution?: PaymentExecutionDataContract;
  // true = bút toán tương ứng bị bỏ qua lúc Approve/Execute vì thiếu Tài
  // khoản Nợ/Có — cho phép bổ sung ngay tại màn này (nút "Ghi bù bút toán").
  liquidacaoFaltaConfiguracao: boolean;
  execucaoFaltaConfiguracao: boolean;
}

export interface ObligacaoDisponivelParaPagamentoDataContract {
  obligationId: number;
  numero: number;
  mes: number;
  descritivoObrigacao?: string;
  valorObrigacao: number;
}

export interface CodigoContaOptionDataContract {
  id: number;
  designacao: string;
}

export interface ContaBancariaOptionDataContract {
  id: number;
  entidadeBancaria: string;
  numero: string;
}

export interface PaymentAuthorizationListResponse extends ResponseBase {
  items: PaymentAuthorizationDataContract[];
}

export interface PaymentAuthorizationResponse extends ResponseBase {
  item: PaymentAuthorizationDataContract;
}

export interface ObligacoesDisponiveisParaPagamentoResponse extends ResponseBase {
  items: ObligacaoDisponivelParaPagamentoDataContract[];
}

export interface CodigoContaOptionsResponse extends ResponseBase {
  items: CodigoContaOptionDataContract[];
}

export interface ContaBancariaOptionsResponse extends ResponseBase {
  items: ContaBancariaOptionDataContract[];
}
