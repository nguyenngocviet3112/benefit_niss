import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import {
  ApproveCompromissoDespesaRequest,
  CreateCompromissoDespesaRequest,
  ReviewCompromissoDespesaRequest,
  SaveCompromissoDespesaPlurianualidadeRequest,
  SaveCompromissoDespesaRequest,
  SubmitCompromissoDespesaRequest
} from '../request-models/compromisso-despesa-request';
import {
  CabimentosDisponiveisResponse,
  CompromissoDespesaListResponse,
  CompromissoDespesaResponse
} from '../response-models/compromisso-despesa-response';
import { ResponseBase } from '../response-models/utils-response';

@Injectable({
  providedIn: 'root'
})
export class CompromissoDespesaService {

  constructor(private http: HttpClient) { }

  public getByAno(ano: number): Observable<CompromissoDespesaListResponse> {
    return this.http.get<CompromissoDespesaListResponse>(`${environment.apiUrl}/compromissodespesa/GetByAno/${ano}`);
  }

  public getCabimentosDisponiveis(ano: number): Observable<CabimentosDisponiveisResponse> {
    return this.http.get<CabimentosDisponiveisResponse>(`${environment.apiUrl}/compromissodespesa/GetCabimentosDisponiveis/${ano}`);
  }

  public create(request: CreateCompromissoDespesaRequest): Observable<CompromissoDespesaResponse> {
    return this.http.post<CompromissoDespesaResponse>(`${environment.apiUrl}/compromissodespesa/Create`, request);
  }

  public save(request: SaveCompromissoDespesaRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/compromissodespesa/Save`, request);
  }

  public savePlurianualidade(request: SaveCompromissoDespesaPlurianualidadeRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/compromissodespesa/SavePlurianualidade`, request);
  }

  public submit(request: SubmitCompromissoDespesaRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/compromissodespesa/Submit`, request);
  }

  public review(request: ReviewCompromissoDespesaRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/compromissodespesa/Review`, request);
  }

  public approve(request: ApproveCompromissoDespesaRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/compromissodespesa/Approve`, request);
  }
}
