import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { SelectDescriptionResponse } from '../response-models/utils-response';
import { GetAllSubClassificacaoByTarefaAtivaIdRequest } from '../request-models/tarefa-request';
import { ApiHelperService } from './api-helper.service';


@Injectable({
  providedIn: 'root'
})
export class SubClassificacaoService {

  constructor(
    private router: Router,
    private http: HttpClient,
    private api: ApiHelperService
  ) { }


  public getAllSubClassificacao(): Observable<SelectDescriptionResponse>
  {
    return this.api.get<SelectDescriptionResponse>('subClassificacao/GetAllSubClassificacao');
  }
  
  public GetAllSubClassificacaoByTarefaAtivaId(request: GetAllSubClassificacaoByTarefaAtivaIdRequest): Observable<SelectDescriptionResponse>
  {
    return this.api.post<SelectDescriptionResponse>('subClassificacao/GetAllSubClassificacaoByTarefaAtivaId', request);
  }
}
