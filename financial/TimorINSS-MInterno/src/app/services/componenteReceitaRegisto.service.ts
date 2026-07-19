import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { DeleteReceitaRequest, GetComponenteReceitaRegistoByIdContaOSSRequest, ReceitasNaoConciliadasRelatorioRequest, RegistoReceitaRequest } from '../request-models/componenteReceitaRegisto-request';
import { ComponenteReceitaRegistoResponse } from '../response-models/componenteReceitaRegisto-response';
import { GetExecucaoOrcamentalRelatoriosRequest, GetExecucaoOrcamentalClassificacaoEconomicaRequest } from '../request-models/agrupamentoConfig-request';
import { RelatoriosExecucaoOrcamentalListagemResponse, ClassificacaoEconomicaExecucaoListagemResponse } from '../response-models/agrupamentoConfig-response';
import { FilterRequest } from '../request-models/utils-request';
import { ReceitaNaoConciliadaRelatorioResponse, ReceitaRelatorioResponse } from '../models/componenteReceitaRegisto';
import { ApiHelperService } from './api-helper.service';

@Injectable({
  providedIn: 'root'
})
export class ComponenteReceitaRegistoService {

  constructor(
    private router: Router,
    private http: HttpClient,
    private api: ApiHelperService
  ) { }

  public addEditComponenteReceitaRegisto(entity: RegistoReceitaRequest) {
    return this.api.post('componenteReceitaRegisto/AddEditComponenteReceitaRegisto', entity);
  }

  public GetComponenteReceitaRegistoByContaOSSId(request: GetComponenteReceitaRegistoByIdContaOSSRequest): Observable<ComponenteReceitaRegistoResponse>
  {
    return this.api.post<ComponenteReceitaRegistoResponse>('componenteReceitaRegisto/GetComponenteReceitaRegistoByContaOSSId', request);
  }

  public deleteReceita(request: DeleteReceitaRequest) {
    return this.api.post('componenteReceitaRegisto/DeleteReceita', request);
  }

  public GetExecucaoOrcamental(request: GetExecucaoOrcamentalRelatoriosRequest): Observable<RelatoriosExecucaoOrcamentalListagemResponse> {
    return this.api.post<RelatoriosExecucaoOrcamentalListagemResponse>('componenteReceitaRegisto/GetExecucaoOrcamental', request);
  }

  public GetExecucaoOrcamentalExcel(request: GetExecucaoOrcamentalRelatoriosRequest): Observable<any> {
    return this.api.post<any>('componenteReceitaRegisto/GetExecucaoOrcamentalExcel', request);
  }

  public GetExecucaoOrcamentalPorClassificacaoEconomica(request: GetExecucaoOrcamentalClassificacaoEconomicaRequest): Observable<ClassificacaoEconomicaExecucaoListagemResponse> {
    return this.api.post<ClassificacaoEconomicaExecucaoListagemResponse>('componenteReceitaRegisto/GetExecucaoOrcamentalPorClassificacaoEconomica', request);
  }

  public GetReceitasRelatorio(request: FilterRequest): Observable<ReceitaRelatorioResponse> {
    return this.api.post<ReceitaRelatorioResponse>('componenteReceitaRegisto/ReceitasRelatorios', request);
  }

  public GetReceitasRelatorioExcel(request: FilterRequest): Observable<any> {
    return this.api.post<any>('componenteReceitaRegisto/ReceitasRelatoriosExcel', request);
  }

  public GetReceitasNaoConciliadasRelatorio(request: ReceitasNaoConciliadasRelatorioRequest): Observable<ReceitaNaoConciliadaRelatorioResponse> {
    return this.api.post<ReceitaNaoConciliadaRelatorioResponse>('componenteReceitaRegisto/ReceitasNaoConciliadasRelatorios', request);
  }

  public GetReceitasNaoConciliadasRelatorioExcel(request: ReceitasNaoConciliadasRelatorioRequest): Observable<any> {
    return this.api.post<any>('componenteReceitaRegisto/ReceitasNaoConciliadasRelatoriosExcel', request);
  }



}
