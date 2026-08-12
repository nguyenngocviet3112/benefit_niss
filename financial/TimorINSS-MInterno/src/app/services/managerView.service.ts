import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiHelperService } from './api-helper.service';
import {
  EntidadesRelatorioRequest,
  SituacaoContributivaEmpresasRelatorioRequest,
  ContribuicoesTrendsRelatorioRequest,
  BudgetExecutionRelatorioRequest,
  DespesaPipelineRelatorioRequest
} from '../request-models/managerView-request';
import {
  EntidadesRelatorioResponse,
  SituacaoContributivaEmpresasRelatorioResponse,
  ContribuicoesTrendsRelatorioResponse,
  BudgetExecutionRelatorioResponse,
  DespesaPipelineRelatorioResponse
} from '../response-models/managerView-response';

@Injectable({
  providedIn: 'root'
})
export class ManagerViewService {

  constructor(private api: ApiHelperService) { }

  public GetEntidadesRelatorio(request: EntidadesRelatorioRequest): Observable<EntidadesRelatorioResponse> {
    return this.api.post<EntidadesRelatorioResponse>('entidadeEmpregadora/GetEntidadesRelatorio', request);
  }

  public GetSituacaoContributivaEmpresasRelatorio(request: SituacaoContributivaEmpresasRelatorioRequest): Observable<SituacaoContributivaEmpresasRelatorioResponse> {
    return this.api.post<SituacaoContributivaEmpresasRelatorioResponse>('declaracao/GetSituacaoContributivaEmpresasRelatorio', request);
  }

  public GetContribuicoesTrendsRelatorio(request: ContribuicoesTrendsRelatorioRequest): Observable<ContribuicoesTrendsRelatorioResponse> {
    return this.api.post<ContribuicoesTrendsRelatorioResponse>('declaracao/GetContribuicoesTrendsRelatorio', request);
  }

  public GetBudgetExecutionRelatorio(request: BudgetExecutionRelatorioRequest): Observable<BudgetExecutionRelatorioResponse> {
    return this.api.post<BudgetExecutionRelatorioResponse>('componenteDespesaRegisto/GetBudgetExecutionRelatorio', request);
  }

  public GetDespesaPipelineRelatorio(request: DespesaPipelineRelatorioRequest): Observable<DespesaPipelineRelatorioResponse> {
    return this.api.post<DespesaPipelineRelatorioResponse>('componenteDespesaRegisto/GetDespesaPipelineRelatorio', request);
  }
}
