import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { DeactivateReceitaPacRequest, SaveReceitaPacRequest } from '../request-models/receita-pac-request';
import { ReceitaPacListResponse, ReceitaPacResponse } from '../response-models/receita-pac-response';
import { ResponseBase } from '../response-models/utils-response';

@Injectable({
  providedIn: 'root'
})
export class ReceitaPacService {

  constructor(private http: HttpClient) { }

  public getByAno(ano: number): Observable<ReceitaPacListResponse> {
    return this.http.get<ReceitaPacListResponse>(`${environment.apiUrl}/receitapac/GetByAno/${ano}`);
  }

  public save(request: SaveReceitaPacRequest): Observable<ReceitaPacResponse> {
    return this.http.post<ReceitaPacResponse>(`${environment.apiUrl}/receitapac/Save`, request);
  }

  public deactivate(request: DeactivateReceitaPacRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/receitapac/Deactivate`, request);
  }
}
