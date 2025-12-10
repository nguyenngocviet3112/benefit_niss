import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';

import { Observable } from 'rxjs';
import { ConciliarMovimentosPermissionsListRequest, ConciliarMovimentosRequest, DesfazerConciliacaoRequest, GetMovimentosConciliadosRequest, MovimentosAConciliarConciliacaoFilterRequest, MovimentosAConciliarFilterRequest, MovimentosBancariosConciliacaoFilterRequest, MovimentosDespesaReceitaUpsertRequest, MovimentosListagemRequest, MovimentosUpsertRequest } from '../request-models/movimentosBancarios-request';
import { ContasBancariasListagemResponse, MovimentosListagemResponse, MovimentosDespesaReceitaListagemResponse, MovimentoBancarioDropListResponse, MovimentosConciliadosResponse, SaldoMovimentosResponse, ConciliarMovimentosPermissionsListResponse } from '../response-models/movimentosBancarios-response';
import { ComponenteReceitaRegistoResponse } from '../response-models/componenteReceitaRegisto-response';
import { GetGuiaMovimentoRequest } from '../request-models/componenteOrcamentoRegisto-request';
import { ApiHelperService } from './api-helper.service';



@Injectable({
  providedIn: 'root'
})
export class movimentosBancariosService {

  constructor(
    private http: HttpClient,
    private api: ApiHelperService
  ) { }

  public saveMovimentoBancario(entity: MovimentosUpsertRequest) {
    return this.api.post<boolean>('movimentosBancarios/InsertMovimento', entity);
  }

  public updateMovimentoBancario(entity: MovimentosUpsertRequest) {
    return this.api.post<boolean>('movimentosBancarios/UpdateMovimento', entity);
  }

  public saveMovimentoDespesaReceita(entity: MovimentosDespesaReceitaUpsertRequest) {
    return this.api.post<boolean>('movimentosPorConciliar/CreateMovimentoPorConciliar', entity);
  }

  public updateMovimentoDespesaReceita(entity: MovimentosDespesaReceitaUpsertRequest) {
    return this.api.post<boolean>('movimentosPorConciliar/UpdateMovimentoPorConciliar', entity);
  }

  public listMovimentosBancarios(entity: MovimentosListagemRequest) : Observable<MovimentosListagemResponse> {
    return this.api.post<MovimentosListagemResponse>('movimentosBancarios/ListMovimentos', entity);
  }

  public ListContasBancarias(incluirSaldo: boolean = false) : Observable<ContasBancariasListagemResponse> {
    // alert(incluirSaldo);
    // return this.api.get<ContasBancariasListagemResponse>('movimentosBancarios/ListContasBancarias');
    return this.api.get<ContasBancariasListagemResponse>(`movimentosBancarios/ListContasBancarias?incluirSaldo=${incluirSaldo}`);
  }

  public GetMovimentoBancarioDropList(tipoMovimento: number) : Observable<MovimentoBancarioDropListResponse> {
    return this.api.get<MovimentoBancarioDropListResponse>(`movimentosBancarios/getMovimentoBancarioDropList?domainFilterId=${tipoMovimento}`);
  }

  public ListMovimentoDespesaReceita(entity: MovimentosAConciliarFilterRequest) : Observable<MovimentosDespesaReceitaListagemResponse> {
    return this.api.post<MovimentosDespesaReceitaListagemResponse>('movimentosPorConciliar/GetListagem', entity);
  }

  public ListMovimentosBancariosConciliacao(entity: MovimentosBancariosConciliacaoFilterRequest) : Observable<MovimentosListagemResponse> {
    return this.api.post<MovimentosListagemResponse>('movimentosBancarios/ListMovimentosConciliacao', entity);
  }

  public ListMovimentoDespesaReceitaConciliacao(entity: MovimentosAConciliarConciliacaoFilterRequest) : Observable<MovimentosDespesaReceitaListagemResponse> {
    return this.api.post<MovimentosDespesaReceitaListagemResponse>('movimentosPorConciliar/GetListagemConciliacao', entity);
  }

  public DesfazerConciliacao(entity: DesfazerConciliacaoRequest) {
    return this.api.post<DesfazerConciliacaoRequest>('movimentosPorConciliar/DesfazerConciliacao', entity);
  }

  public GetMovimentosConciliados(request: GetMovimentosConciliadosRequest): Observable<MovimentosConciliadosResponse>
  {
    return this.api.post<MovimentosConciliadosResponse>('movimentosPorConciliar/GetMovimentosConciliados', request);
  }

  public ListSaldoMovimentos(request: any = {}) : Observable<SaldoMovimentosResponse> {
    return this.api.post<SaldoMovimentosResponse>('movimentosBancarios/ListSaldoMovimentos', request);
  }

  public ConciliarMovimentos(request: ConciliarMovimentosRequest) {
    return this.api.post('movimentosPorConciliar/ConciliarMovimentos', request);
  }

  public ListPermissions(request: ConciliarMovimentosPermissionsListRequest): Observable<ConciliarMovimentosPermissionsListResponse> {
    return this.api.post<ConciliarMovimentosPermissionsListResponse>('movimentosBancarios/ListPermissions', request);
  }

  public listMovimentosBancariosExcel(entity: MovimentosListagemRequest) : Observable<any> {
    return this.api.post<any>('movimentosBancarios/ListMovimentosExcel', entity);
  }

  public getMovimentoGuia(entity: GetGuiaMovimentoRequest) : Observable<any> {
    return this.api.post<any>('movimentosPorConciliar/GetMovimentoGuia', entity);
  }
}
