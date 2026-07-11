import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { DeactivateEconomicClassificationRequest, SaveEconomicClassificationRequest } from '../request-models/economic-classification-request';
import { EconomicClassificationTreeResponse } from '../response-models/economic-classification-response';
import { ResponseBase } from '../response-models/utils-response';

@Injectable({
  providedIn: 'root'
})
export class EconomicClassificationService {

  constructor(private http: HttpClient) { }

  public getTree(orcamentoConfigFk: number): Observable<EconomicClassificationTreeResponse> {
    return this.http.get<EconomicClassificationTreeResponse>(`${environment.apiUrl}/economicclassification/GetTree/${orcamentoConfigFk}`);
  }

  public save(request: SaveEconomicClassificationRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/economicclassification/Save`, request);
  }

  public deactivate(request: DeactivateEconomicClassificationRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/economicclassification/Deactivate`, request);
  }
}
