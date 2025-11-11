import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { GetAllRelTarefaComponenteByIdTarefaActivoRequest, UpdateRelTarefaComponenteRequest } from '../request-models/relTarefaComponente-request';
import { Observable } from 'rxjs';
import { ComponenteListagemResponse } from '../response-models/componente-response';
import { ApiHelperService } from './api-helper.service';

@Injectable({
  providedIn: 'root'
})
export class RelTarefaComponenteService {

  constructor(
    private router: Router,
    private http: HttpClient,
    private api: ApiHelperService
  ) { }


  public updateRelTarefaComponente(entity: UpdateRelTarefaComponenteRequest) {
    return this.api.post('relTarefaComponente/UpdateRelTarefaComponente', entity);
  }

  public getAllRelTarefaComponenteByIdTarefaActivo(request: GetAllRelTarefaComponenteByIdTarefaActivoRequest) : Observable<ComponenteListagemResponse> {
    return this.api.post<ComponenteListagemResponse>('relTarefaComponente/GetAllRelTarefaComponenteByIdTarefaActivo', request);
  }
}
