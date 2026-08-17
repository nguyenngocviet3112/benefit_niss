import { MyMaxNumberStateMatcher } from "../matcher";
import { Compromisso } from "../models/compromisso";
import { Despesa } from "../models/despesa";
import { FilterRequest } from "./utils-request";

export interface RegistoDespesaRequest {
  despesa: Despesa;
  // [PT] true = o utilizador já viu a lista de despesas em curso para os mesmos 5 parâmetros e
  // confirmou que quer mesmo registar. Na 1ª tentativa vai sempre por confirmar (false/undefined).
  // [VI] true = người dùng đã xem danh sách despesa đang mở cùng 5 tham số và xác nhận vẫn muốn
  // đăng ký. Lần gửi đầu tiên luôn để chưa xác nhận (false/undefined).
  confirmarDespesasEmCurso?: boolean;
}

export interface GetAllDespesaRegistadaRequest{
  tarefaAtivoId: number;
}

export interface DeleteDespesaRequest{
  id: number;
}

export interface DeleteRequest{
  id: number;
}

export interface CompromissoUpsertRequest {
  compromisso: Compromisso;
}
export interface GetValoresDespesaByIdCodigoOrcamentoRequest{
  agrupamentoFk: number;
  orcamentoRegistoFk: number;
  institutionId: number;
  actidadeFk: number;
  funcionalFk: number;
  // [PT] 5.º parâmetro: sem ele os valores mostrados somavam todos os centros de custo da rubrica.
  // [VI] Tham số thứ 5: thiếu nó thì các giá trị hiển thị gộp mọi centro de custo của rubrica.
  centroCustoFk: number;
}

export interface DeleteListaDespesaRequest{
  ids: number[];
}

export interface UpdateDespesaRequest{
  id: number;
  estado: string;
}

export interface DespesasRelatorioRequest {
  EstadoDespesa?: number;
  filter: FilterRequest;
}

export interface GetDespesasCompromissoRequest {
  tarefaAtivoId: number;
}

export interface CompromissoUpsertRequest {
  compromisso: Compromisso;
  tarefaAtivoId: number;
}

export interface UpdateDespesaCabimentadaRequest {
  id: number;
  valor: number;
}
