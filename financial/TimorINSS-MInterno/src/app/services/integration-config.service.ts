import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { IntegrationConfigItem, IntegrationConfigResponse } from '../response-models/integration-config-response';
import { ResponseBase } from '../response-models/utils-response';

@Injectable({
  providedIn: 'root'
})
export class IntegrationConfigService {

  constructor(private http: HttpClient) { }

  public getConfig(): Observable<IntegrationConfigResponse> {
    return this.http.get<IntegrationConfigResponse>(`${environment.apiUrl}/integrationconfig/Get`);
  }

  public saveConfig(request: IntegrationConfigItem): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/integrationconfig/Save`, request);
  }
}
