import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { GetComponenteOrcamentoRegistoAprovadoRequest, GetComponenteOrcamentoRegistoRequest, OrcamentoExtractRequest, UpdateComponenteOrcamentoRegistoDatesRequest } from '../request-models/componenteOrcamentoRegisto-request';
import { GetComponenteOrcamentoAprovadoRegistoReponse, GetComponenteOrcamentoRegistoReponse, OrcamentoExtractToExcelReponse, OrcamentoExtractToPDFReponse, UpdateComponenteOrcamentoRegistoDatesResponse } from '../response-models/componenteOrcamentoRegisto-response';
import { Observable } from 'rxjs';
import { ApiHelperService } from './api-helper.service';

@Injectable({
  providedIn: 'root'
})
export class componenteOrcamentoRegistoService {

  constructor(
    private router: Router,
    private http: HttpClient,

    private api: ApiHelperService
  ) { }


  public GetComponenteOrcamento(request: GetComponenteOrcamentoRegistoRequest): Observable<GetComponenteOrcamentoRegistoReponse> {
    return this.api.post<GetComponenteOrcamentoRegistoReponse>('componenteOrcamentoRegisto/GetComponenteOrcamentoRegisto', request);
  }

  public UpdateComponenteOrcamentoRegistoDates(request: UpdateComponenteOrcamentoRegistoDatesRequest): Observable<UpdateComponenteOrcamentoRegistoDatesResponse> {
    return this.api.post<UpdateComponenteOrcamentoRegistoDatesResponse>('componenteOrcamentoRegisto/UpdateComponenteOrcamentoRegistoDates', request);
  }

  public GetOrcamentoAprovadoDespesaByIdTarefaActivo(entity: GetComponenteOrcamentoRegistoAprovadoRequest): Observable<GetComponenteOrcamentoAprovadoRegistoReponse> {
    return this.api.post<GetComponenteOrcamentoAprovadoRegistoReponse>('componenteOrcamentoRegisto/GetOrcamentoAprovadoDespesaByIdTarefaActivo', entity);
  }

  public RetificarOrcamentoAprovado(entity: UpdateComponenteOrcamentoRegistoDatesRequest): Observable<GetComponenteOrcamentoRegistoReponse> {
    return this.api.post<GetComponenteOrcamentoRegistoReponse>('componenteOrcamentoRegisto/RetificarOrcamentoAprovado', entity);
  }

  public AprovarOrcamento(request: GetComponenteOrcamentoRegistoRequest) {
    return this.api.post('componenteOrcamentoRegisto/AprovarOrcamento', request);
  }

  public ExtractToExcel(request: OrcamentoExtractRequest): Observable<OrcamentoExtractToExcelReponse> {
    return this.api.post<OrcamentoExtractToExcelReponse>('componenteOrcamentoRegisto/ExtractToExcel', request);
  }

  public ExtractToPDF(request: OrcamentoExtractRequest): Observable<OrcamentoExtractToPDFReponse> {
    return this.api.post<OrcamentoExtractToPDFReponse>('componenteOrcamentoRegisto/ExtractToPDF', request);
  }

  public GetOrcamentoAprovadoReceitaByIdTarefaActivo(entity: GetComponenteOrcamentoRegistoAprovadoRequest): Observable<GetComponenteOrcamentoAprovadoRegistoReponse> {
    return this.api.post<GetComponenteOrcamentoAprovadoRegistoReponse>('componenteOrcamentoRegisto/GetOrcamentoAprovadoReceitaByIdTarefaActivo', entity);
  }
}
