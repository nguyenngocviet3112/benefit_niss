import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { forkJoin, Observable } from 'rxjs';
import { ListagemDominios, ListagemDominiosComGrupos, ListagemRegimesResponse, SingleDominio } from '../response-models/dominios-response';
import { GetTiposDocumentoPorTarefaAtivaRequest } from '../request-models/dominio-request';
import { SelectDescriptionResponse } from '../response-models/utils-response';
import { ApiHelperService } from './api-helper.service';

@Injectable({
  providedIn: 'root'
})
export class DominiosService {

  constructor(
      private router: Router,
      private http: HttpClient,
      private api: ApiHelperService
  ) {}

  public GetAllGruposCamposEditaveis() : Observable<ListagemDominios> {
    return this.api.get<ListagemDominios>('Dominios/GetAllGruposCamposEditaveis/'+ localStorage.getItem('selectedLanguage'));
  }

  public GetAllTiposDeRegime(): Observable<ListagemDominios>
  {
    return this.api.get<ListagemDominios>('Dominios/GetAllTiposDeRegime/'+ localStorage.getItem('selectedLanguage'));
  }

  public GetAllTiposDeDocumentoTarefa(): Observable<ListagemDominios>
  {
    return this.api.get<ListagemDominios>('Dominios/GetAllTiposDeDocumentoTarefa/'+ localStorage.getItem('selectedLanguage'));
  }

  public getAllTiposDeContracto(): Observable<ListagemDominios>
  {
    return this.api.get<ListagemDominios>('Dominios/GetAllTiposDeContracto/'+ localStorage.getItem('selectedLanguage'));
  }

  public getAllNaturezasDeContracto(): Observable<ListagemDominios>
  {
    return this.api.get<ListagemDominios>('Dominios/GetAllNaturezasDeContracto/'+ localStorage.getItem('selectedLanguage'));
  }

  public getAllLeisLaboraisAplicaveis(): Observable<ListagemDominios>
  {
    return this.api.get<ListagemDominios>('Dominios/GetAllLeisLaboraisAplicaveis/'+ localStorage.getItem('selectedLanguage'));
  }

  public getAllTiposDeDocumento(): Observable<ListagemDominios>
  {
    return this.api.get<ListagemDominios>('Dominios/GetAllTiposDeDocumento/'+ localStorage.getItem('selectedLanguage'));
  }

  public GetAllRegimes() : Observable<ListagemRegimesResponse>  {
    return this.api.get<ListagemRegimesResponse>('Dominios/GetAllRegimes/'+ localStorage.getItem('selectedLanguage'));
  }

  public getAllProfissoes() : Observable<ListagemDominios> {
    return this.api.get<ListagemDominios>('Dominios/GetAllProfissoes/'+ localStorage.getItem('selectedLanguage'));
  }

  public getAllEstadosCivis(): Observable<ListagemDominios>
  {
    return this.api.get<ListagemDominios>('Dominios/GetAllEstadosCivis/'+ localStorage.getItem('selectedLanguage'));
  }

  public getAllSexos(): Observable<ListagemDominios>
  {
    return this.api.get<ListagemDominios>('Dominios/GetAllSexos/'+ localStorage.getItem('selectedLanguage'));
  }

  public getAllNacionalidades(): Observable<ListagemDominios>
  {
    return this.api.get<ListagemDominios>('Dominios/GetAllNacionalidades/'+ localStorage.getItem('selectedLanguage'));
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
    return this.api.get<ListagemDominios>('Dominios/GetAllFuncoes/'+ localStorage.getItem('selectedLanguage'));
  }

  public GetSalarioMinimo() : Observable<SingleDominio>  {
    return this.api.get<SingleDominio>('Dominios/GetSalarioMinimo/'+ localStorage.getItem('selectedLanguage'));
  }

  public GetDeclarationDay() : Observable<SingleDominio>  {
    return this.api.get<SingleDominio>('Dominios/GetDeclarationDay/'+ localStorage.getItem('selectedLanguage'));
  }

  public GetAllTiposPagamento() : Observable<ListagemDominios> {
    return this.api.get<ListagemDominios>('Dominios/GetAllTiposPagamento/'+ localStorage.getItem('selectedLanguage'));
  }

  public GetAllTiposGuia() : Observable<ListagemDominios> {
    return this.api.get<ListagemDominios>('Dominios/GetAllTiposGuia/'+ localStorage.getItem('selectedLanguage'));
  }

  public GetTiposDocumentoPorTarefaAtiva(request: GetTiposDocumentoPorTarefaAtivaRequest) : Observable<ListagemDominiosComGrupos> {
    return this.api.post<ListagemDominiosComGrupos>('Dominios/GetTiposDocumentoPorTarefaAtiva', request);
  }

  public getAllCaixas(): Observable<ListagemDominios>
  {
    return this.api.get<ListagemDominios>('Dominios/getAllCaixas/'+ localStorage.getItem('selectedLanguage'));
  }

  public getAllMovimentosTypes(): Observable<ListagemDominios>
  {
    return this.api.get<ListagemDominios>('Dominios/getAllMovimentosTypes/'+ localStorage.getItem('selectedLanguage'));
  }

  public getAllTiposConta(): Observable<ListagemDominios>
  {
    return this.api.get<ListagemDominios>('Dominios/GetAllTiposConta/'+ localStorage.getItem('selectedLanguage'));
  }

  public getAllEstadosPagamento(): Observable<ListagemDominios>
  {
    return this.api.get<ListagemDominios>('Dominios/GetAllEstadosPagamento/'+ localStorage.getItem('selectedLanguage'));
  }
}
