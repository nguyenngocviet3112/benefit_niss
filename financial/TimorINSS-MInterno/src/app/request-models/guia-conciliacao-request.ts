import { FilterRequest } from './utils-request';

export interface GetGuiasPendentesValidacaoRequest {
  filter: FilterRequest;
}

// 2026-07-13: dòng sao kê khả dụng để khớp Guia Pagamento giờ đọc từ
// BankStatementLine (mode mới) thay vì Movimentosbancarios (mode cũ) — xem
// GuiaConciliacaoController.GetLinhasDisponiveis (backend nhận thẳng
// SearchFilterRequest, cùng shape { filter }).
export interface GetLinhasDisponiveisRequest {
  filter: FilterRequest;
}

export interface ConciliarGuiaPagamentoRequest {
  guiaIds: number[];
  bankStatementLineIds: number[];
}

export interface GetReceitasGpReportRequest {
  ano: number;
}

export interface UndoConciliacaoGuiaRequest {
  guiaId: number;
}
