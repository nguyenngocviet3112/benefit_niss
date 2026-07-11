import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { DeactivateFunctionalClassificationRequest, SaveFunctionalClassificationRequest } from '../request-models/functional-classification-request';
import { FunctionalClassificationTreeResponse } from '../response-models/functional-classification-response';
import { ResponseBase } from '../response-models/utils-response';

@Injectable({
  providedIn: 'root'
})
export class FunctionalClassificationService {

  constructor(private http: HttpClient) { }

  public getAllActive(): Observable<FunctionalClassificationTreeResponse> {
    return this.http.get<FunctionalClassificationTreeResponse>(`${environment.apiUrl}/functionalclassification/GetAllActive`);
  }

  public save(request: SaveFunctionalClassificationRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/functionalclassification/Save`, request);
  }

  public deactivate(request: DeactivateFunctionalClassificationRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/functionalclassification/Deactivate`, request);
  }
}
