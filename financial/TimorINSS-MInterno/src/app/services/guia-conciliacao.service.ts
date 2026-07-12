import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ConciliarGuiaPagamentoRequest, GetGuiasPendentesValidacaoRequest } from '../request-models/guia-conciliacao-request';
import { MovimentosBancariosConciliacaoFilterRequest } from '../request-models/movimentosBancarios-request';
import { GuiaListagemResponse } from '../response-models/guiaPagamento-response';
import { MovimentosListagemResponse } from '../response-models/movimentosBancarios-response';
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

  public getMovimentosBancariosDisponiveis(request: MovimentosBancariosConciliacaoFilterRequest): Observable<MovimentosListagemResponse> {
    return this.http.post<MovimentosListagemResponse>(`${environment.apiUrl}/movimentosBancarios/ListMovimentosConciliacao`, request);
  }

  public conciliarGuiaPagamento(request: ConciliarGuiaPagamentoRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/guiaConciliacao/ConciliarGuiaPagamento`, request);
  }

  public getComprovativo(idGuia: number): Observable<GuiaComprovativoResponse> {
    return this.http.get<GuiaComprovativoResponse>(`${environment.apiUrl}/guiaConciliacao/GetComprovativo/${idGuia}`);
  }
}
