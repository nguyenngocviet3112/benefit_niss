import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { DeactivateOrcamentoConfigRequest, SaveOrcamentoConfigRequest } from '../request-models/orcamento-config-request';
import { OrcamentoConfigListResponse } from '../response-models/orcamento-config-response';
import { ResponseBase } from '../response-models/utils-response';

@Injectable({
  providedIn: 'root'
})
export class OrcamentoConfigService {

  constructor(private http: HttpClient) { }

  public getAll(): Observable<OrcamentoConfigListResponse> {
    return this.http.get<OrcamentoConfigListResponse>(`${environment.apiUrl}/orcamentoconfig/GetAll`);
  }

  public save(request: SaveOrcamentoConfigRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/orcamentoconfig/Save`, request);
  }

  public deactivate(request: DeactivateOrcamentoConfigRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/orcamentoconfig/Deactivate`, request);
  }
}
