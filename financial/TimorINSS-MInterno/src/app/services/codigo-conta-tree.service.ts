import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { DeactivateCodigoContaRequest, SaveCodigoContaRequest } from '../request-models/codigo-conta-tree-request';
import { CodigoContaTreeResponse } from '../response-models/codigo-conta-tree-response';
import { ResponseBase } from '../response-models/utils-response';

@Injectable({
  providedIn: 'root'
})
export class CodigoContaTreeService {

  constructor(private http: HttpClient) { }

  public getTree(orcamentoConfigFk: number): Observable<CodigoContaTreeResponse> {
    return this.http.get<CodigoContaTreeResponse>(`${environment.apiUrl}/codigocontatree/GetTree/${orcamentoConfigFk}`);
  }

  public save(request: SaveCodigoContaRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/codigocontatree/Save`, request);
  }

  public deactivate(request: DeactivateCodigoContaRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/codigocontatree/Deactivate`, request);
  }
}
