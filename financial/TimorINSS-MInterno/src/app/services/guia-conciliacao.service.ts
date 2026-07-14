import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ConciliarGuiaPagamentoRequest, GetGuiasPendentesValidacaoRequest, GetLinhasDisponiveisRequest, GetReceitasGpReportRequest, UndoConciliacaoGuiaRequest } from '../request-models/guia-conciliacao-request';
import { GuiaListagemResponse } from '../response-models/guiaPagamento-response';
import { BankStatementLineListResponse } from '../response-models/bank-statement-line-response';
import { ResponseBase } from '../response-models/utils-response';
import { GuiaComprovativoResponse } from '../response-models/guia-conciliacao-response';

@Injectable({
  providedIn: 'root'
})
export class GuiaConciliacaoService {

  constructor(private http: HttpClient) { }

  public getGuiasPendentesValidacao(request: GetGuiasPendentesValidacaoRequest): Observable<GuiaListagemResponse> {
    return this.http.post<GuiaListagemResponse>(`${environment.apiUrl}/guiaPagamento/listGuiasByEntidadeApprove`, request);
  }

  // 2026-07-13: chuyển từ movimentosBancarios/ListMovimentosConciliacao (bảng cũ
  // Movimentosbancarios) sang guiaConciliacao/GetLinhasDisponiveis (BankStatementLine,
  // cùng bảng với Conciliação de Movimentos) — xem CLAUDE.md/memory
  // bank-statement-line-guia-pagamento-unification.
  public getLinhasDisponiveis(request: GetLinhasDisponiveisRequest): Observable<BankStatementLineListResponse> {
    return this.http.post<BankStatementLineListResponse>(`${environment.apiUrl}/guiaConciliacao/GetLinhasDisponiveis`, request);
  }

  public conciliarGuiaPagamento(request: ConciliarGuiaPagamentoRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/guiaConciliacao/ConciliarGuiaPagamento`, request);
  }

  public getComprovativo(idGuia: number): Observable<GuiaComprovativoResponse> {
    return this.http.get<GuiaComprovativoResponse>(`${environment.apiUrl}/guiaConciliacao/GetComprovativo/${idGuia}`);
  }

  public getReceitasGpReport(request: GetReceitasGpReportRequest): Observable<GuiaListagemResponse> {
    return this.http.post<GuiaListagemResponse>(`${environment.apiUrl}/guiaConciliacao/GetReceitasGpReport`, request);
  }

  public undoConciliacao(request: UndoConciliacaoGuiaRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/guiaConciliacao/UndoConciliacao`, request);
  }
}
