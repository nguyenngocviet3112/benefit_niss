import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import {
  AddObligationBeneficiaryRequest,
  AddObligationItemRequest,
  ApproveObligationRequest,
  CreateObligationRequest,
  RemoveObligationBeneficiaryRequest,
  RemoveObligationItemRequest,
  SubmitObligationRequest
} from '../request-models/obligation-request';
import {
  CompromissosComSaldoResponse,
  ObligationListResponse,
  ObligationResponse
} from '../response-models/obligation-response';
import { ResponseBase } from '../response-models/utils-response';

@Injectable({
  providedIn: 'root'
})
export class ObligationService {

  constructor(private http: HttpClient) { }

  public getByAno(ano: number): Observable<ObligationListResponse> {
    return this.http.get<ObligationListResponse>(`${environment.apiUrl}/obligation/GetByAno/${ano}`);
  }

  public getCompromissosComSaldo(ano: number): Observable<CompromissosComSaldoResponse> {
    return this.http.get<CompromissosComSaldoResponse>(`${environment.apiUrl}/obligation/GetCompromissosComSaldo/${ano}`);
  }

  public create(request: CreateObligationRequest): Observable<ObligationResponse> {
    return this.http.post<ObligationResponse>(`${environment.apiUrl}/obligation/Create`, request);
  }

  public addItem(request: AddObligationItemRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/obligation/AddItem`, request);
  }

  public removeItem(request: RemoveObligationItemRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/obligation/RemoveItem`, request);
  }

  public addBeneficiary(request: AddObligationBeneficiaryRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/obligation/AddBeneficiary`, request);
  }

  public removeBeneficiary(request: RemoveObligationBeneficiaryRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/obligation/RemoveBeneficiary`, request);
  }

  public submit(request: SubmitObligationRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/obligation/Submit`, request);
  }

  public approve(request: ApproveObligationRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/obligation/Approve`, request);
  }
}
