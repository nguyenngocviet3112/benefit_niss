import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { SelectDescriptionResponse } from '../response-models/utils-response';
import { ConfigurarTarefaRequest, EditarTarefaRequest, GetAllTarefasASeguirRequest, GetHistoricoTextoRequest, GetTarefaDataRequest, SwitchTarefaAtivoRequest, TarefaDataRequest, TarefaListagemRequest} from '../request-models/tarefa-request';
import { ComponenteTarefaConfiguradaResponse, TarefaAtivoListagemResponse, TarefaDataResponse, TarefaListagemResponse } from '../response-models/tarefa-response';
import { map } from 'rxjs/operators';
import { ComponenteHistoricoTextoListagemResponse } from '../response-models/componenteTextoRegisto-response';
import { ApiHelperService } from './api-helper.service';

@Injectable({
  providedIn: 'root'
})
export class TarefaService {

  constructor(
    private router: Router,
    private http: HttpClient,
    private api: ApiHelperService
  ) { }


  public getAllTarefaAtivo(): Observable<SelectDescriptionResponse>
  {
    return this.api.get<SelectDescriptionResponse>('tarefa/GetAllTarefaAtivo');
  }

  public addTarefaConfigurada(entity: ConfigurarTarefaRequest) {
    return this.api.post('tarefa/AddTarefaConfigurada', entity);
  }

  public getAllTarefas(request: TarefaListagemRequest): Observable<TarefaListagemResponse>
  {
    return this.api.post<TarefaListagemResponse>('tarefa/GetAllTarefas', request);
  }

  public getAllComponentesByIdTarefa(id: number) : Observable<ComponenteTarefaConfiguradaResponse>  {
    return this.api.get<ComponenteTarefaConfiguradaResponse>('tarefa/GetAllComponentesByIdTarefa/'+id)
    .pipe(map(x => {
      let response : ComponenteTarefaConfiguradaResponse = <ComponenteTarefaConfiguradaResponse>{
      };
      if(x == null)
        return response;
      response = x;
      return response;
    }))
  }

  public editarTarefa(entity: EditarTarefaRequest) {
    return this.api.post('tarefa/EditarTarefa', entity);
  }

  public GetAllTarefasAtivas(request: TarefaListagemRequest): Observable<TarefaAtivoListagemResponse>
  {
    return this.api.post<TarefaAtivoListagemResponse>('tarefa/GetAllTarefasAtivas', request);
  }
  
  public LockTarefa(request: SwitchTarefaAtivoRequest) {
    return this.api.post('tarefa/LockTarefa', request);
  }

  public UnlockTarefa(request: SwitchTarefaAtivoRequest) {
    return this.api.post('tarefa/UnlockTarefa', request);
  }

  public GetHistoricoTexto(request: GetHistoricoTextoRequest): Observable<ComponenteHistoricoTextoListagemResponse> {
    return this.api.post<ComponenteHistoricoTextoListagemResponse>('tarefa/GetHistoricoTexto', request);
  }

  public GetAllTarefasASeguir(request: GetAllTarefasASeguirRequest): Observable<SelectDescriptionResponse>
  {
    return this.api.post<SelectDescriptionResponse>('tarefa/GetAllTarefasASeguir',request);
  }

  public GetTarefaData(request: GetTarefaDataRequest): Observable<TarefaDataResponse> {
    return this.api.post<TarefaDataResponse>('tarefa/GetTarefaData',request);
  }

  public SaveTarefaData(entity: TarefaDataRequest) {
    return this.api.post('tarefa/SaveTarefaData', entity);
  }

  public ArquivarTarefa(entity: TarefaDataRequest) {
    return this.api.post('tarefa/ArquivarTarefa', entity);
  }

  public GetTituloListaPagamento(tarefaActivoId: number) {
    return this.api.get(`tarefa/GetTituloListaPagamento/${tarefaActivoId}`);
  }

  public SaveTituloListaPagamento(tarefaAtivoId: number, titulo: string) {
    return this.api.post('tarefa/SaveTituloListaPagamento', { tarefaAtivoId, titulo });
  }
}
