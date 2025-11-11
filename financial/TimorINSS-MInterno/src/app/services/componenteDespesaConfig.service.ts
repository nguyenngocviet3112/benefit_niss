import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { GetComponenteDespesaConfigRequest } from '../request-models/componenteDespesaConfig-request';
import { GetComponenteDespesaConfigReponse } from '../response-models/componenteDespesaConfig-response';
import { ApiHelperService } from './api-helper.service';

@Injectable({
  providedIn: 'root'
})
export class ComponenteDespesaConfigService {

  constructor(
    private router: Router,
    private http: HttpClient,
    private api: ApiHelperService
  ) { }


  public getComponenteDespesaConfigByTarefaAtivoId(request: GetComponenteDespesaConfigRequest): Observable<GetComponenteDespesaConfigReponse>
  {
    return this.api.post<GetComponenteDespesaConfigReponse>('componenteDespesaConfig/GetComponenteDespesaConfigByTarefaAtivoId', request);
  }

}
