import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { forkJoin, Observable } from 'rxjs';
import { ListagemDominios, ListagemDominiosComGrupos, ListagemRegimesResponse, SingleDominio } from '../response-models/dominios-response';
import { GetTiposDocumentoPorTarefaAtivaRequest } from '../request-models/dominio-request';
import { SelectDescriptionResponse } from '../response-models/utils-response';

@Injectable({
  providedIn: 'root'
})
export class DominiosService {

  constructor(
      private router: Router,
      private http: HttpClient
  ) {}

  public GetAllGruposCamposEditaveis() : Observable<ListagemDominios> {
    return this.http.get<ListagemDominios>(`${environment.apiUrl}/Dominios/GetAllGruposCamposEditaveis`);
  }

  public GetAllTiposDeRegime(): Observable<ListagemDominios>
  {
    return this.http.get<ListagemDominios>(`${environment.apiUrl}/Dominios/GetAllTiposDeRegime`);
  }

  public GetAllTiposDeDocumentoTarefa(): Observable<ListagemDominios>
  {
    return this.http.get<ListagemDominios>(`${environment.apiUrl}/dominios/GetAllTiposDeDocumentoTarefa`);
  }

  public getAllTiposDeContracto(): Observable<ListagemDominios>
  {
    return this.http.get<ListagemDominios>(`${environment.apiUrl}/Dominios/GetAllTiposDeContracto`);
  }

  public getAllNaturezasDeContracto(): Observable<ListagemDominios>
  {
    return this.http.get<ListagemDominios>(`${environment.apiUrl}/Dominios/GetAllNaturezasDeContracto`);
  }

  public getAllLeisLaboraisAplicaveis(): Observable<ListagemDominios>
  {
    return this.http.get<ListagemDominios>(`${environment.apiUrl}/Dominios/GetAllLeisLaboraisAplicaveis`);
  }

  public getAllTiposDeDocumento(): Observable<ListagemDominios>
  {
    return this.http.get<ListagemDominios>(`${environment.apiUrl}/Dominios/GetAllTiposDeDocumento`);
  }

  public GetAllRegimes() : Observable<ListagemRegimesResponse>  {
    return this.http.get<ListagemRegimesResponse>(`${environment.apiUrl}/Dominios/GetAllRegimes`);
  }

  public getAllProfissoes() : Observable<ListagemDominios> {
    return this.http.get<ListagemDominios>(`${environment.apiUrl}/Dominios/GetAllProfissoes`);
  }

  public getAllEstadosCivis(): Observable<ListagemDominios>
  {
    return this.http.get<ListagemDominios>(`${environment.apiUrl}/Dominios/GetAllEstadosCivis`);
  }

  public getAllSexos(): Observable<ListagemDominios>
  {
    return this.http.get<ListagemDominios>(`${environment.apiUrl}/Dominios/GetAllSexos`);
  }

  public getAllNacionalidades(): Observable<ListagemDominios>
  {
    return this.http.get<ListagemDominios>(`${environment.apiUrl}/Dominios/GetAllNacionalidades`);
  }

  public getAllDominiosForNovoTrabalhador(): Observable<ListagemDominios[]> {
        let tiposDocumento = this.getAllTiposDeDocumento();
        let estadosCivis = this.getAllEstadosCivis();
        let sexos = this.getAllSexos();
        let nacionalidades = this.getAllNacionalidades();
        let tiposContratos = this.getAllTiposDeContracto();
        let naturezasContrato = this.getAllNaturezasDeContracto();
        let leisLabAplicaveis = this.getAllLeisLaboraisAplicaveis();
        let profissoes = this.getAllProfissoes();

    // Observable.forkJoin (RxJS 5) changes to just forkJoin() in RxJS 6
    return forkJoin([tiposDocumento,estadosCivis,sexos,nacionalidades,tiposContratos,naturezasContrato,leisLabAplicaveis,profissoes]);
  }

  public getAllFuncoes() : Observable<ListagemDominios> {
    return this.http.get<ListagemDominios>(`${environment.apiUrl}/Dominios/GetAllFuncoes`);
  }

  public GetSalarioMinimo() : Observable<SingleDominio>  {
    return this.http.get<SingleDominio>(`${environment.apiUrl}/Dominios/GetSalarioMinimo`);
  }

  public GetDeclarationDay() : Observable<SingleDominio>  {
    return this.http.get<SingleDominio>(`${environment.apiUrl}/Dominios/GetDeclarationDay`);
  }

  public GetAllTiposPagamento() : Observable<ListagemDominios> {
    return this.http.get<ListagemDominios>(`${environment.apiUrl}/Dominios/GetAllTiposPagamento`);
  }

  public GetAllTiposGuia() : Observable<ListagemDominios> {
    return this.http.get<ListagemDominios>(`${environment.apiUrl}/Dominios/GetAllTiposGuia`);
  }

  public GetTiposDocumentoPorTarefaAtiva(request: GetTiposDocumentoPorTarefaAtivaRequest) : Observable<ListagemDominiosComGrupos> {
    return this.http.post<ListagemDominiosComGrupos>(`${environment.apiUrl}/Dominios/GetTiposDocumentoPorTarefaAtiva`, request);
  }

  public getAllCaixas(): Observable<ListagemDominios>
  {
    return this.http.get<ListagemDominios>(`${environment.apiUrl}/Dominios/getAllCaixas`);
  }

  public getAllMovimentosTypes(): Observable<ListagemDominios>
  {
    return this.http.get<ListagemDominios>(`${environment.apiUrl}/Dominios/getAllMovimentosTypes`);
  }

  public getAllTiposConta(): Observable<ListagemDominios>
  {
    return this.http.get<ListagemDominios>(`${environment.apiUrl}/Dominios/GetAllTiposConta`);
  }

  public getAllEstadosPagamento(): Observable<ListagemDominios>
  {
    return this.http.get<ListagemDominios>(`${environment.apiUrl}/Dominios/GetAllEstadosPagamento`);
  }
}
