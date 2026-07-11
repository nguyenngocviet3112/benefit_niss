import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { DeactivateDepartamentoConfigRequest, SaveDepartamentoConfigRequest } from '../request-models/departamento-config-request';
import { DepartamentoConfigListResponse, DepartamentoConfigResponse } from '../response-models/departamento-config-response';
import { ResponseBase } from '../response-models/utils-response';

@Injectable({
  providedIn: 'root'
})
export class DepartamentoConfigService {

  constructor(private http: HttpClient) { }

  public getAll(): Observable<DepartamentoConfigListResponse> {
    return this.http.get<DepartamentoConfigListResponse>(`${environment.apiUrl}/departamento/GetAllConfig`);
  }

  public save(request: SaveDepartamentoConfigRequest): Observable<DepartamentoConfigResponse> {
    return this.http.post<DepartamentoConfigResponse>(`${environment.apiUrl}/departamento/SaveConfig`, request);
  }

  public deactivate(request: DeactivateDepartamentoConfigRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/departamento/DeactivateConfig`, request);
  }
}
