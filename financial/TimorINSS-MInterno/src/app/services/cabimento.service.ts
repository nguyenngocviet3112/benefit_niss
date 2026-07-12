import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ApproveCabimentoRequest, CreateCabimentoRequest, SaveCabimentoRequest, SubmitCabimentoRequest } from '../request-models/cabimento-request';
import { AdsDisponiveisParaCabimentoResponse, CabimentoListResponse, CabimentoResponse } from '../response-models/cabimento-response';
import { ResponseBase } from '../response-models/utils-response';

@Injectable({
  providedIn: 'root'
})
export class CabimentoService {

  constructor(private http: HttpClient) { }

  public getByAno(ano: number): Observable<CabimentoListResponse> {
    return this.http.get<CabimentoListResponse>(`${environment.apiUrl}/cabimento/GetByAno/${ano}`);
  }

  public getAdsDisponiveis(ano: number): Observable<AdsDisponiveisParaCabimentoResponse> {
    return this.http.get<AdsDisponiveisParaCabimentoResponse>(`${environment.apiUrl}/cabimento/GetAdsDisponiveis/${ano}`);
  }

  public create(request: CreateCabimentoRequest): Observable<CabimentoResponse> {
    return this.http.post<CabimentoResponse>(`${environment.apiUrl}/cabimento/Create`, request);
  }

  public save(request: SaveCabimentoRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/cabimento/Save`, request);
  }

  public submit(request: SubmitCabimentoRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/cabimento/Submit`, request);
  }

  public approve(request: ApproveCabimentoRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/cabimento/Approve`, request);
  }
}
