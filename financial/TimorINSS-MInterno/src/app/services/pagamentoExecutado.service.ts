import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { BalancoRelatoriosRequest, DeletePagamentoRequest, EditPagamentoExecutadoRequest, FornecedoresRelatoriosRequest, GetDestinatarioPagamentoRequest, GetOrdensPagamentoRelatoriosRequest, GetPagamentoExecutadoRequest, ListagemPagamentosProcessoRequest, ReportsDropdownContasOGERequest, SaveClassificacaoContabilisticaExecucaoRequest, SavePagamentoExecutadoRequest } from '../request-models/pagamentoExecutado-request';
import { DestinatarioPagamentoExecutadosResponse, ListagemPagamentosProcessoResponse, RelatoriosOrdensPagamentoListagemResponse } from '../response-models/pagamentosExecutados-response';
import { FilterRequest } from '../request-models/utils-request';
import { SelectDescriptionResponse } from '../response-models/utils-response';
import { GetExecucaoOrcamentalRelatoriosRequest, GetExecucaoOrcamentalClassificacaoEconomicaRequest } from '../request-models/agrupamentoConfig-request';
import { RelatoriosExecucaoOrcamentalListagemResponse, ClassificacaoEconomicaExecucaoListagemResponse } from '../response-models/agrupamentoConfig-response';
import { BalancoResponse, ClassificacaoContabilisticaResponse, FornecedoresResponse } from '../models/pagamentos_executados';
import { ApiHelperService } from './api-helper.service';

@Injectable({
  providedIn: 'root'
})
export class PagamentoExecutadoService {

  constructor(
      private http: HttpClient,
      private api: ApiHelperService
  ) {}

  public SavePagamentoExecutado(entity: SavePagamentoExecutadoRequest){
    return this.api.post('pagamentoExecutado/SavePagamentoExecutado', entity);
  }

  public GetDestinatariosPagamentoByNumPagamento(request: GetDestinatarioPagamentoRequest): Observable<DestinatarioPagamentoExecutadosResponse>
  {
    return this.api.post<DestinatarioPagamentoExecutadosResponse>('pagamentoExecutado/GetDestinatariosPagamentoByNumPagamento', request);
  }

  public DeletePagamento(request: DeletePagamentoRequest) {
    return this.api.post('pagamentoExecutado/DeletePagamento', request);
  }

  public GetPagamentosExecutadosByIdDestinatario(request: GetPagamentoExecutadoRequest): Observable<DestinatarioPagamentoExecutadosResponse>
  {
    return this.api.post<DestinatarioPagamentoExecutadosResponse>('pagamentoExecutado/GetPagamentosExecutadosByIdDestinatario', request);
  }

  public GetListaPagamentoExcel(request: ListagemPagamentosProcessoRequest): Observable<DestinatarioPagamentoExecutadosResponse>
  {
    return this.api.post<DestinatarioPagamentoExecutadosResponse>('pagamentoExecutado/GetListaPagamentoExcel', request);
  }

  public EditPagamentoExecutado(entity: EditPagamentoExecutadoRequest){
    return this.api.post('pagamentoExecutado/EditPagamentoExecutado', entity);
  }

  public GetOrdensPagamentoRelatoriosGrouped(request: GetOrdensPagamentoRelatoriosRequest): Observable<RelatoriosOrdensPagamentoListagemResponse> {
    return this.api.post<RelatoriosOrdensPagamentoListagemResponse>('pagamentoExecutado/GetPagamentosRelatoriosGrouped', request);
  }

  public GetOrdensPagamentoRelatorios(request: GetOrdensPagamentoRelatoriosRequest): Observable<RelatoriosOrdensPagamentoListagemResponse> {
    return this.api.post<RelatoriosOrdensPagamentoListagemResponse>('pagamentoExecutado/GetPagamentosRelatorios', request);
  }

  public ExtractToExcelRelatorios(request: GetOrdensPagamentoRelatoriosRequest): Observable<any> {
    return this.api.post<any>('pagamentoExecutado/ExtractToExcelRelatorios', request);
  }

  public GetDropdownContasOGE(request: ReportsDropdownContasOGERequest): Observable<SelectDescriptionResponse> {
    return this.api.post<any>('pagamentoExecutado/GetDropdownContasOGE', request);
  }

  public GetCentrosCusto(request: FilterRequest): Observable<SelectDescriptionResponse> {
    return this.api.post<any>('pagamentoExecutado/GetCentrosCusto', request);
  }

  public GetExecucaoOrcamental(request: GetExecucaoOrcamentalRelatoriosRequest): Observable<RelatoriosExecucaoOrcamentalListagemResponse> {
    return this.api.post<RelatoriosExecucaoOrcamentalListagemResponse>('pagamentoExecutado/GetExecucaoOrcamental', request);
  }

  public GetExecucaoOrcamentalExcel(request: GetExecucaoOrcamentalRelatoriosRequest): Observable<any> {
    return this.api.post<any>('pagamentoExecutado/GetExecucaoOrcamentalExcel', request);
  }

  public GetExecucaoOrcamentalPorClassificacaoEconomica(request: GetExecucaoOrcamentalClassificacaoEconomicaRequest): Observable<ClassificacaoEconomicaExecucaoListagemResponse> {
    return this.api.post<ClassificacaoEconomicaExecucaoListagemResponse>('pagamentoExecutado/GetExecucaoOrcamentalPorClassificacaoEconomica', request);
  }

  public GetListaPagamentosProcesso(request: ListagemPagamentosProcessoRequest): Observable<ListagemPagamentosProcessoResponse>
  {
    return this.api.post<ListagemPagamentosProcessoResponse>('pagamentoExecutado/GetListaPagamentosProcesso', request);
  }

  public GetPagamentoDetails(request: GetDestinatarioPagamentoRequest): Observable<ListagemPagamentosProcessoResponse>
  {
    return this.api.post<ListagemPagamentosProcessoResponse>('pagamentoExecutado/GetPagamentoDetails', request);
  }

  public ClassificacaoContabilisticaRelatorio(request: FilterRequest): Observable<ClassificacaoContabilisticaResponse>
  {
    return this.api.post<ClassificacaoContabilisticaResponse>('pagamentoExecutado/ClassificacaoContabilisticaRelatorios', request);
  }

  public FornecedoresRelatorio(request: FornecedoresRelatoriosRequest): Observable<FornecedoresResponse>
  {
    return this.api.post<FornecedoresResponse>('pagamentoExecutado/FornecedoresRelatorios', request);
  }

  public FornecedoresRelatorioExcel(request: FornecedoresRelatoriosRequest): Observable<any>
  {
    return this.api.post<any>('pagamentoExecutado/FornecedoresRelatoriosExcel', request);
  }

  public ClassificacaoContabilisticaRelatorioExcel(request: FilterRequest): Observable<any>
  {
    return this.api.post<any>('pagamentoExecutado/ClassificacaoContabilisticaRelatoriosExcel', request);
  }

  public BalancoRelatorio(request: BalancoRelatoriosRequest): Observable<BalancoResponse>
  {
    return this.api.post<BalancoResponse>('pagamentoExecutado/BalancoRelatorios', request);
  }

  public BalancoRelatorioExcel(request: BalancoRelatoriosRequest): Observable<any>
  {
    return this.api.post<any>('pagamentoExecutado/BalancoRelatoriosExcel', request);
  }

  public SaveClassificacaoExecucao(request: SaveClassificacaoContabilisticaExecucaoRequest): Observable<any>
  {
    return this.api.post<any>('pagamentoExecutado/SaveClassificacaoExecucao', request);
  }
}
