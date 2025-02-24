import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Observable, forkJoin } from 'rxjs';
import { ListagemDominios, ListagemRegimesResponse, SingleDominio } from '../response-models/dominios-response';

@Injectable({
  providedIn: 'root'
})
export class DominiosService {

  constructor(
      private router: Router,
      private http: HttpClient
  ) {}

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

  public getAllTipoDivida(): Observable<ListagemDominios>
  {
    return this.http.get<ListagemDominios>(`${environment.apiUrl}/Dominios/GetAllTipoDivida`);
  }

  public getAllSituacaoPagamento(): Observable<ListagemDominios>
  {
    return this.http.get<ListagemDominios>(`${environment.apiUrl}/Dominios/GetAllSituacaoPagamento`);
  }

  public GetAllRegimes() : Observable<ListagemRegimesResponse>  {
    return this.http.get<ListagemRegimesResponse>(`${environment.apiUrl}/Dominios/GetAllRegimes`);
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

  public getAllProfissoes() : Observable<ListagemDominios> {
    return this.http.get<ListagemDominios>(`${environment.apiUrl}/Dominios/GetAllProfissoes`);
  }

  public getAllFuncoes() : Observable<ListagemDominios> {
    return this.http.get<ListagemDominios>(`${environment.apiUrl}/Dominios/GetAllFuncoes`);
  }
}
