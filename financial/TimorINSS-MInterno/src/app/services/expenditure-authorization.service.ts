import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import {
  ApproveExpenditureAuthorizationRequest,
  CreateExpenditureAuthorizationRequest,
  DeletePlurianualidadeRequest,
  ReviewExpenditureAuthorizationRequest,
  SaveExpenditureAuthorizationRequest,
  SavePlurianualidadeRequest,
  SubmitExpenditureAuthorizationRequest
} from '../request-models/expenditure-authorization-request';
import {
  AvailableRubricasResponse,
  ExpenditureAuthorizationListResponse,
  ExpenditureAuthorizationResponse
} from '../response-models/expenditure-authorization-response';
import { ResponseBase, StringFileResponse } from '../response-models/utils-response';

@Injectable({
  providedIn: 'root'
})
export class ExpenditureAuthorizationService {

  constructor(private http: HttpClient) { }

  public getByAno(ano: number): Observable<ExpenditureAuthorizationListResponse> {
    return this.http.get<ExpenditureAuthorizationListResponse>(`${environment.apiUrl}/expenditureauthorization/GetByAno/${ano}`);
  }

  public getByAnoExcel(ano: number): Observable<StringFileResponse> {
    return this.http.get<StringFileResponse>(`${environment.apiUrl}/expenditureauthorization/GetByAnoExcel/${ano}`);
  }

  public getAvailableRubricas(orcamentoConfigFk: number): Observable<AvailableRubricasResponse> {
    return this.http.get<AvailableRubricasResponse>(`${environment.apiUrl}/expenditureauthorization/GetAvailableRubricas/${orcamentoConfigFk}`);
  }

  public create(request: CreateExpenditureAuthorizationRequest): Observable<ExpenditureAuthorizationResponse> {
    return this.http.post<ExpenditureAuthorizationResponse>(`${environment.apiUrl}/expenditureauthorization/Create`, request);
  }

  public save(request: SaveExpenditureAuthorizationRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/expenditureauthorization/Save`, request);
  }

  public savePlurianualidade(request: SavePlurianualidadeRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/expenditureauthorization/SavePlurianualidade`, request);
  }

  public deletePlurianualidade(request: DeletePlurianualidadeRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/expenditureauthorization/DeletePlurianualidade`, request);
  }

  public submit(request: SubmitExpenditureAuthorizationRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/expenditureauthorization/Submit`, request);
  }

  public review(request: ReviewExpenditureAuthorizationRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/expenditureauthorization/Review`, request);
  }

  public approve(request: ApproveExpenditureAuthorizationRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/expenditureauthorization/Approve`, request);
  }
}
