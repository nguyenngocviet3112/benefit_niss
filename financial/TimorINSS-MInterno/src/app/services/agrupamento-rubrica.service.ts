import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { DeactivateAgrupamentoRubricaRequest, SaveAgrupamentoRubricaRequest } from '../request-models/agrupamento-rubrica-request';
import { AgrupamentoRubricaTreeResponse } from '../response-models/agrupamento-rubrica-response';
import { ResponseBase } from '../response-models/utils-response';

@Injectable({
  providedIn: 'root'
})
export class AgrupamentoRubricaService {

  constructor(private http: HttpClient) { }

  public getTree(orcamentoConfigFk: number, tipoConta: string): Observable<AgrupamentoRubricaTreeResponse> {
    return this.http.get<AgrupamentoRubricaTreeResponse>(
      `${environment.apiUrl}/agrupamentorubrica/GetTree/${orcamentoConfigFk}/${encodeURIComponent(tipoConta)}`
    );
  }

  public save(request: SaveAgrupamentoRubricaRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/agrupamentorubrica/Save`, request);
  }

  public deactivate(request: DeactivateAgrupamentoRubricaRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/agrupamentorubrica/Deactivate`, request);
  }
}
