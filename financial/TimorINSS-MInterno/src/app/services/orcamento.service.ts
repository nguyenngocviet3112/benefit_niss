import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import {
  ApproveOrcamentoBatchRequest,
  DeleteOrcamentoLinhaRequest,
  ReviewOrcamentoBatchRequest,
  SaveOrcamentoLinhaRequest,
  SubmitOrcamentoBatchRequest
} from '../request-models/orcamento-request';
import { OrcamentoBatchResponse } from '../response-models/orcamento-response';
import { ResponseBase } from '../response-models/utils-response';

@Injectable({
  providedIn: 'root'
})
export class OrcamentoService {

  constructor(private http: HttpClient) { }

  public getActiveBatch(orcamentoConfigFk: number): Observable<OrcamentoBatchResponse> {
    return this.http.get<OrcamentoBatchResponse>(`${environment.apiUrl}/orcamento/GetActiveBatch/${orcamentoConfigFk}`);
  }

  public startNewBatch(orcamentoConfigFk: number): Observable<OrcamentoBatchResponse> {
    return this.http.post<OrcamentoBatchResponse>(`${environment.apiUrl}/orcamento/StartNewBatch`, { OrcamentoConfigFk: orcamentoConfigFk });
  }

  public saveLinha(request: SaveOrcamentoLinhaRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/orcamento/SaveLinha`, request);
  }

  public deleteLinha(request: DeleteOrcamentoLinhaRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/orcamento/DeleteLinha`, request);
  }

  public submit(request: SubmitOrcamentoBatchRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/orcamento/Submit`, request);
  }

  public review(request: ReviewOrcamentoBatchRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/orcamento/Review`, request);
  }

  public approve(request: ApproveOrcamentoBatchRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/orcamento/Approve`, request);
  }
}
