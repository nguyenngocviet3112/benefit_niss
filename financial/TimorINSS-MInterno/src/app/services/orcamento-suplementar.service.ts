import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import {
  ApproveOrcamentoSuplementarRequest,
  DeleteOrcamentoSuplementarLinhaRequest,
  ReviewOrcamentoSuplementarRequest,
  SaveOrcamentoSuplementarLinhaRequest,
  SubmitOrcamentoSuplementarRequest
} from '../request-models/orcamento-suplementar-request';
import {
  OrcamentoSuplementarBatchResponse,
  RubricasAprovadasParaSuplementarResponse
} from '../response-models/orcamento-suplementar-response';
import { ResponseBase } from '../response-models/utils-response';

@Injectable({
  providedIn: 'root'
})
export class OrcamentoSuplementarService {

  constructor(private http: HttpClient) { }

  public getActiveBatch(orcamentoConfigFk: number): Observable<OrcamentoSuplementarBatchResponse> {
    return this.http.get<OrcamentoSuplementarBatchResponse>(`${environment.apiUrl}/orcamentosuplementar/GetActiveBatch/${orcamentoConfigFk}`);
  }

  public getRubricasAprovadas(orcamentoConfigFk: number): Observable<RubricasAprovadasParaSuplementarResponse> {
    return this.http.get<RubricasAprovadasParaSuplementarResponse>(`${environment.apiUrl}/orcamentosuplementar/GetRubricasAprovadas/${orcamentoConfigFk}`);
  }

  public saveLinha(request: SaveOrcamentoSuplementarLinhaRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/orcamentosuplementar/SaveLinha`, request);
  }

  public deleteLinha(request: DeleteOrcamentoSuplementarLinhaRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/orcamentosuplementar/DeleteLinha`, request);
  }

  public submit(request: SubmitOrcamentoSuplementarRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/orcamentosuplementar/Submit`, request);
  }

  public review(request: ReviewOrcamentoSuplementarRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/orcamentosuplementar/Review`, request);
  }

  public approve(request: ApproveOrcamentoSuplementarRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/orcamentosuplementar/Approve`, request);
  }
}
