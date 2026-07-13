import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { CeInssGlobalRequest } from '../request-models/ce-inss-global-request';
import { CeInssGlobalResponse } from '../response-models/ce-inss-global-response';
import { StringFileResponse } from '../response-models/utils-response';

@Injectable({
  providedIn: 'root'
})
export class CeInssGlobalService {

  constructor(private http: HttpClient) { }

  public getReport(request: CeInssGlobalRequest): Observable<CeInssGlobalResponse> {
    return this.http.post<CeInssGlobalResponse>(`${environment.apiUrl}/ceInssGlobal/GetReport`, request);
  }

  public getReportExcel(request: CeInssGlobalRequest): Observable<StringFileResponse> {
    return this.http.post<StringFileResponse>(`${environment.apiUrl}/ceInssGlobal/GetReportExcel`, request);
  }
}
