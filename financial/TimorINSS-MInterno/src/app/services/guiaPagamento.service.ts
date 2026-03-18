import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import {
  approveComprovativoPagamentoRequest,
  GetAllGuiasStatesFromYearByFilterRequest, GetGuiasDetailsRequest,
  GetGuiasRelatoriosRequest,
  GuiaPagamentoRequest,
  insertComprovativoPagamentoRequest,
  useCreditInGuiaPagamentoRequest
} from '../request-models/guiaPagamento-request';
import { GuiaListagemResponse, RelatoriosGuiasListagemResponse } from '../response-models/guiaPagamento-response';
import { ApiHelperService } from './api-helper.service';



@Injectable({
  providedIn: 'root'
})
export class GuiaPagamentoService {

  constructor(
    private router: Router,
    private http: HttpClient,
    private api: ApiHelperService
  ) { }


  public saveGuiaPagamento(entity: GuiaPagamentoRequest) {
    return this.api.post<boolean>('guiaPagamento/SaveGuiaPagamento', entity);
  }

  public getAllGuiasByEntidade(entity: GetAllGuiasStatesFromYearByFilterRequest) {
    const encodedId = this.api.encodeId(entity.idEntidade);
    const payload = { ...entity, idEntidadeStr: encodedId , idEntidade:0};
    return this.api.post<GuiaListagemResponse>('guiaPagamento/listGuiasByEntidade', payload);
  }

  public getGuiasDetailByEntidade(entity: GetGuiasDetailsRequest) {
    const encodedId = this.api.encodeId(entity.idGuiaPagamento);
    const payload = { ...entity, idGuiaPagamentoStr: encodedId };
    return this.api.post<GuiaListagemResponse>('guiaPagamento/guiaPagamentoDetail', payload);
  }

  public getAllGuiasByEntidadeApprove(entity: GetAllGuiasStatesFromYearByFilterRequest) {
    const encodedId = this.api.encodeId(entity.idEntidade);
    const payload = { ...entity, idEntidadeStr: encodedId , idEntidade:0};
    return this.api.post<GuiaListagemResponse>('guiaPagamento/listGuiasByEntidadeApprove', payload);
  }

  public useCreditInGuiaPagamento(request: useCreditInGuiaPagamentoRequest) {
    return this.api.post('guiaPagamento/useCreditInGuiaPagamento', request);
  }

  public insertComprovativoPagamento(request: insertComprovativoPagamentoRequest) {
    return this.api.post('guiaPagamento/insertComprovativoPagamento', request);
  }

  public approveComprovativoPagamento(request: approveComprovativoPagamentoRequest) {
    return this.api.post('guiaPagamento/approveComprovativoPagamento', request);
  }

  public GetGuiasRelatorios(request: GetGuiasRelatoriosRequest): Observable<RelatoriosGuiasListagemResponse> {
    return this.api.post<RelatoriosGuiasListagemResponse>('guiaPagamento/GetGuiasPagamentoRelatorios', request);
}
}
