import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';

import { Observable } from 'rxjs';
import { AgrupamentoConfigResponse } from '../response-models/agrupamentoConfig-response';
import { GetAgrupamentoConfigRequest } from '../request-models/agrupamentoConfig-request';
import { ApiHelperService } from './api-helper.service';




@Injectable({
  providedIn: 'root'
})
export class AgrupamentoConfigService {

  constructor(
    private http: HttpClient,
    private api: ApiHelperService
  ) { }


  public getAgrupamentoConfigByIdCodigoContaTipoConta(request: GetAgrupamentoConfigRequest): Observable<AgrupamentoConfigResponse>
  {
    return this.api.post<AgrupamentoConfigResponse>('agrupamentoConfig/GetAgrupamentoConfigByIdCodigoContaTipoConta', request);
  }
}
