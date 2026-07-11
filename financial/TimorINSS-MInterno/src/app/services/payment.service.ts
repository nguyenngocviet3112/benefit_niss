import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import {
  ApprovePaymentAuthorizationRequest,
  CreatePaymentAuthorizationRequest,
  ExecutePaymentRequest,
  SubmitPaymentAuthorizationRequest
} from '../request-models/payment-request';
import {
  CodigoContaOptionsResponse,
  ContaBancariaOptionsResponse,
  ObligacoesDisponiveisParaPagamentoResponse,
  PaymentAuthorizationListResponse,
  PaymentAuthorizationResponse
} from '../response-models/payment-response';
import { ResponseBase } from '../response-models/utils-response';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {

  constructor(private http: HttpClient) { }

  public getByAno(ano: number): Observable<PaymentAuthorizationListResponse> {
    return this.http.get<PaymentAuthorizationListResponse>(`${environment.apiUrl}/pagamento/GetByAno/${ano}`);
  }

  public getObligacoesDisponiveis(ano: number): Observable<ObligacoesDisponiveisParaPagamentoResponse> {
    return this.http.get<ObligacoesDisponiveisParaPagamentoResponse>(`${environment.apiUrl}/pagamento/GetObligacoesDisponiveis/${ano}`);
  }

  public getCodigoContaOptions(): Observable<CodigoContaOptionsResponse> {
    return this.http.get<CodigoContaOptionsResponse>(`${environment.apiUrl}/pagamento/GetCodigoContaOptions`);
  }

  public getContaBancariaOptions(): Observable<ContaBancariaOptionsResponse> {
    return this.http.get<ContaBancariaOptionsResponse>(`${environment.apiUrl}/pagamento/GetContaBancariaOptions`);
  }

  public create(request: CreatePaymentAuthorizationRequest): Observable<PaymentAuthorizationResponse> {
    return this.http.post<PaymentAuthorizationResponse>(`${environment.apiUrl}/pagamento/Create`, request);
  }

  public submit(request: SubmitPaymentAuthorizationRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/pagamento/Submit`, request);
  }

  public approve(request: ApprovePaymentAuthorizationRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/pagamento/Approve`, request);
  }

  public execute(request: ExecutePaymentRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/pagamento/Execute`, request);
  }
}
