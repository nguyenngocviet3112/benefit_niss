import { ValoresDespesaRegistada } from '../models/despesa';
import { DespesaCabimentadasParaExecucao, DespesaCompromisso, DespesaRegistada, DespesaRelatorio } from '../models/despesaRegistada';
import { ValorCamposEditaveisSaveRequest } from '../request-models/camposEditaveis-request';


export interface GetComponenteDespesaRegistoReponse {
  componenteDespesaRegisto: DespesaRegistada[];
}

// [PT] Uma despesa que já existe para a mesma combinação dos 5 parâmetros e ainda está em curso.
// [VI] Một despesa đã tồn tại cùng tổ hợp 5 tham số và vẫn đang mở.
export interface DespesaEmCurso {
  id: number;
  numeroProcesso: string;
  descricao: string;
  valor: number;
  // [PT] Código do estado ESTADODESPESA (1 = Registada, 2 = Autorizada). A etiqueta legível é
  // resolvida no ecrã: a descrição em DOMINIO é só a letra "R"/"A".
  // [VI] Mã trạng thái ESTADODESPESA (1 = Registada, 2 = Autorizada). Nhãn đọc được do màn hình
  // dịch: descricao trong DOMINIO chỉ là chữ "R"/"A".
  estadoValor: number;
}

// [PT] Se despesasEmCurso vier preenchida, NADA foi gravado: é o aviso para o utilizador confirmar.
// [VI] Nếu despesasEmCurso có dữ liệu nghĩa là CHƯA lưu gì: đó là cảnh báo chờ người dùng xác nhận.
export interface AddEditDespesaRegistoResponse {
  despesasEmCurso: DespesaEmCurso[];
}


export interface GetValoresDespesaByIdCodigoOrcamentoResponse {
  valoresDespesa: ValoresDespesaRegistada;
}

export interface GetComponenteDespesaCabimentadaParaExecucaoReponse {
  despesasParaExecucao: DespesaCabimentadasParaExecucao[];
}

export interface GetDespesasRelatorioReponse {
  rows: number;
  despesas: DespesaRelatorio[];
}

export interface GetDespesasCompromissoResponse {
    componenteDespesaObrigacao: DespesaCompromisso[];
}
