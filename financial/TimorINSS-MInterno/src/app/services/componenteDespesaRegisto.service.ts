import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {CompromissoUpsertRequest, DeleteDespesaRequest, DeleteListaDespesaRequest, DeleteRequest, DespesasRelatorioRequest, GetAllDespesaRegistadaRequest, GetDespesasCompromissoRequest, GetValoresDespesaByIdCodigoOrcamentoRequest, RegistoDespesaRequest, UpdateDespesaCabimentadaRequest, UpdateDespesaRequest } from '../request-models/componenteDespesaRegisto-request';
import { GetComponenteDespesaCabimentadaParaExecucaoReponse, GetComponenteDespesaRegistoReponse, GetDespesasCompromissoResponse, GetDespesasRelatorioReponse, GetValoresDespesaByIdCodigoOrcamentoResponse } from '../response-models/componenteDespesaRegisto-response';
import { ApiHelperService } from './api-helper.service';

@Injectable({
  providedIn: 'root'
})
export class ComponenteDespesaRegistoService {

  constructor(
    private router: Router,
    private http: HttpClient,
    private api: ApiHelperService
  ) { }

  public addEditComponenteDespesaRegisto(entity: RegistoDespesaRequest) {
    return this.api.post('componenteDespesaRegisto/AddEditComponenteDespesaRegisto', entity);
  }

  public GetAllDespesaRegistadaByTarefaAtivoId(request: GetAllDespesaRegistadaRequest): Observable<GetComponenteDespesaRegistoReponse>
  {
    return this.api.post<GetComponenteDespesaRegistoReponse>('componenteDespesaRegisto/GetAllDespesaRegistadaByTarefaAtivoId', request);
  }

  public deleteDespesa(request: DeleteDespesaRequest) {
    return this.api.post('componenteDespesaRegisto/DeleteDespesa', request);
  }

  public GetValoresDespesaByIdCodigoOrcamento(request: GetValoresDespesaByIdCodigoOrcamentoRequest): Observable<GetValoresDespesaByIdCodigoOrcamentoResponse>
  {
    return this.api.post<GetValoresDespesaByIdCodigoOrcamentoResponse>('componenteDespesaRegisto/GetValoresDespesaByIdCodigoOrcamento', request);
  }

  public deleteAllDespesasRegistadas(request: DeleteListaDespesaRequest) {
    return this.api.post('componenteDespesaRegisto/DeleteAllDespesasRegistadas', request);
  }

  public UpdateDespesa(request: UpdateDespesaRequest) {
    return this.api.post('componenteDespesaRegisto/UpdateDespesa', request);
  }


  public GetAllDespesaCabimentadasParaExecucaoByTarefaAtivoId(request: GetAllDespesaRegistadaRequest): Observable<GetComponenteDespesaCabimentadaParaExecucaoReponse>
  {
    return this.api.post<GetComponenteDespesaCabimentadaParaExecucaoReponse>('componenteDespesaRegisto/GetAllDespesaCabimentadasParaExecucaoByTarefaAtivoId', request);
  }


  public GetDespesasRelatorio(request: DespesasRelatorioRequest): Observable<GetDespesasRelatorioReponse>
  {
    return this.api.post<GetDespesasRelatorioReponse>('componenteDespesaRegisto/GetDespesasRelatorios', request);
  }


  public GetDespesasRelatorioExcel(request: DespesasRelatorioRequest): Observable<any>
  {
    return this.api.post<any>('componenteDespesaRegisto/GetDespesasRelatoriosExcel', request);
  }

  public GetDespesasCompromissoByTarefaAtivoId(request: GetDespesasCompromissoRequest): Observable<GetDespesasCompromissoResponse>
  {
    return this.api.post<GetDespesasCompromissoResponse>('componenteDespesaRegisto/GetDespesasCompromissoByTarefaAtivoId', request);
  }

  public deleteCompromisso(request: DeleteRequest) {
    return this.api.post('componenteDespesaRegisto/DeleteCompromisso', request);
  }

  public upsertCompromisso(request: CompromissoUpsertRequest) {
    return this.api.post('componenteDespesaRegisto/UpsertCompromisso', request);
  }

  public GetAllDespesaCompromissosParaExecucaoByTarefaAtivoId(request: GetAllDespesaRegistadaRequest): Observable<GetComponenteDespesaCabimentadaParaExecucaoReponse>
  {
    return this.api.post<GetComponenteDespesaCabimentadaParaExecucaoReponse>('componenteDespesaRegisto/GetAllDespesaCompromissosParaExecucaoByTarefaAtivoId', request);
  }

  public UpdateDespesaCabimentada(request: UpdateDespesaCabimentadaRequest) {
    return this.api.post('componenteDespesaRegisto/UpdateDespesaCabimentada', request);
  }
}
