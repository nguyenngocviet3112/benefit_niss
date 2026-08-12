import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiHelperService } from './api-helper.service';
import { AprovarAjusteOrcamentoRequest, GetAjustesOrcamentoRequest, RejeitarAjusteOrcamentoRequest, SolicitarAjusteOrcamentoRequest } from '../request-models/componenteOrcamentoAjuste-request';
import { GetAjustesOrcamentoResponse, GetRubricasDisponiveisResponse, SolicitarAjusteOrcamentoResponse } from '../response-models/componenteOrcamentoAjuste-response';

@Injectable({
  providedIn: 'root'
})
export class componenteOrcamentoAjusteService {

  constructor(private api: ApiHelperService) { }

  public SolicitarAjuste(request: SolicitarAjusteOrcamentoRequest): Observable<SolicitarAjusteOrcamentoResponse> {
    return this.api.post<SolicitarAjusteOrcamentoResponse>('componenteOrcamentoAjuste/SolicitarAjuste', request);
  }

  public AprovarAjuste(request: AprovarAjusteOrcamentoRequest) {
    return this.api.post('componenteOrcamentoAjuste/AprovarAjuste', request);
  }

  public RejeitarAjuste(request: RejeitarAjusteOrcamentoRequest) {
    return this.api.post('componenteOrcamentoAjuste/RejeitarAjuste', request);
  }

  public GetPendentes(request: GetAjustesOrcamentoRequest): Observable<GetAjustesOrcamentoResponse> {
    return this.api.post<GetAjustesOrcamentoResponse>('componenteOrcamentoAjuste/GetPendentes', request);
  }

  public GetHistorico(request: GetAjustesOrcamentoRequest): Observable<GetAjustesOrcamentoResponse> {
    return this.api.post<GetAjustesOrcamentoResponse>('componenteOrcamentoAjuste/GetHistorico', request);
  }

  public GetRubricasDisponiveis(): Observable<GetRubricasDisponiveisResponse> {
    return this.api.post<GetRubricasDisponiveisResponse>('componenteOrcamentoAjuste/GetRubricasDisponiveis', {});
  }
}
