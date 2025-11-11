import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { GetHistoricoTextoRequest, GetProcessoDataRequest, GetProcessosRelatoriosRequest, ListProcessoConfiRequest, ProcessoConfigRequest, ProcessoUpdateRequest } from '../request-models/processo-request';
import { TarefaListagemRequest } from '../request-models/tarefa-request';
import { ComponenteHistoricoTextoListagemResponse } from '../response-models/componenteTextoRegisto-response';
import { ProcessoConfigResponse, ProcessoDataResponse, ProcessosArquivadosListagemResponse, ProcessosListagemResponse, RelatoriosProcessosListagemResponse, TipoProcessosRelatoriosResponse } from '../response-models/processo-response';
import { SelectDescriptionResponse } from '../response-models/utils-response';
import { ApiHelperService } from './api-helper.service';

@Injectable({
    providedIn: 'root'
})
export class ProcessoService {

    constructor(
        private http: HttpClient,
        private api: ApiHelperService
    ) { }

    public GetAllProcessos(request: TarefaListagemRequest): Observable<ProcessosListagemResponse> {
        return this.api.post<ProcessosListagemResponse>('processos/GetAllProcessos', request);
    }

    public SwitchProcessoState(request: ProcessoUpdateRequest) {
        return this.api.post('processos/SwitchProcessoState', request);
    }

    public CreateProcessoConfig(request: ProcessoConfigRequest) {
        return this.api.post('processos/CreateProcessoConfig', request);
    }

    public GetProcessoConfig(request: ListProcessoConfiRequest): Observable<ProcessoConfigResponse> {
        return this.api.post<ProcessoConfigResponse>('processos/GetProcessoConfig', request);
    }

    public UpdateProcessoConfig(request: ProcessoConfigRequest) {
        return this.api.post('processos/UpdateProcessoConfig', request);
    }

    
    public ListIniciarProcessos(request: any): Observable<SelectDescriptionResponse> {
        return this.api.post<SelectDescriptionResponse>('processos/ListIniciarProcessosApprove', request);
    }

    public GetAllProcessosArquivados(request: any): Observable<ProcessosArquivadosListagemResponse> {
        return this.api.post<ProcessosArquivadosListagemResponse>('processos/GetAllProcessosArquivados', request);
    }

    public GetProcessoData(request: GetProcessoDataRequest): Observable<ProcessoDataResponse> {
        return this.api.post<ProcessoDataResponse>('processos/GetProcessoData', request);
    }

    public StartProcess(request: ProcessoUpdateRequest) {
        return this.api.post('processos/StartProcess', request);
    }

    public GetHistoricoTexto(request: GetHistoricoTextoRequest): Observable<ComponenteHistoricoTextoListagemResponse> {
        return this.api.post<ComponenteHistoricoTextoListagemResponse>('processos/GetHistoricoTexto', request);
    }

    public GetProcessosRelatorios(request: GetProcessosRelatoriosRequest): Observable<RelatoriosProcessosListagemResponse> {
        return this.api.post<RelatoriosProcessosListagemResponse>('processos/GetProcessosRelatorios', request);
    }

    public GetTipoProcessosRelatorios(request: any): Observable<TipoProcessosRelatoriosResponse> {
        return this.api.post<TipoProcessosRelatoriosResponse>('processos/GetTipoProcessosRelatorios', request);
    }

    public ExtractToExcelRelatorios(request: GetProcessosRelatoriosRequest): Observable<any> {
        return this.api.post<any>('processos/ExtractToExcelRelatorios', request);
    }
}
