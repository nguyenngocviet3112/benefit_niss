import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { UpdateCodigoContaOpeningBalanceRequest } from '../request-models/codigo-conta-opening-balance-request';
import { CodigoContaOpeningBalanceListResponse } from '../response-models/codigo-conta-opening-balance-response';
import { ResponseBase } from '../response-models/utils-response';

@Injectable({
  providedIn: 'root'
})
export class CodigoContaOpeningBalanceService {

  constructor(private http: HttpClient) { }

  public getAll(): Observable<CodigoContaOpeningBalanceListResponse> {
    return this.http.get<CodigoContaOpeningBalanceListResponse>(`${environment.apiUrl}/codigocontaopeningbalance/GetAll`);
  }

  public update(request: UpdateCodigoContaOpeningBalanceRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/codigocontaopeningbalance/Update`, request);
  }
}
