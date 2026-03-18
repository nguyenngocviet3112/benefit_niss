import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { GetDeclaracaoByEntidadeAndFilterResponse, RelatoriosDeclaracaoListagemResponse, ResumoDeclaracao } from '../response-models/declaracao-response';
import { GetDeclaracaoByEntidadeAndFilterRequest, GetDeclaracaoRelatoriosRequest, GetResumoDeclaracaoRequest, SaveDeclaracoesRequest } from './../request-models/declaracao-request';
import { ApiHelperService } from './api-helper.service';

@Injectable({
  providedIn: 'root'
})
export class DeclaracaoService {

  constructor(
      private router: Router,
      private http: HttpClient,
      private api: ApiHelperService
  ) {}


  public getDeclaracaoByEntidadeAndFilter(request: GetDeclaracaoByEntidadeAndFilterRequest) : Observable<GetDeclaracaoByEntidadeAndFilterResponse>  {
    const encodedId = this.api.encodeId(request.IdEntidade);
    const payload = { ...request, IdEntidadeStr: encodedId, IdEntidade:0 };
    return this.api.post<GetDeclaracaoByEntidadeAndFilterResponse>('declaracao/GetDeclaracaoByEntidadeAndFilter',payload);
  }

  public getResumoDeclaracao(request: GetResumoDeclaracaoRequest) : Observable<ResumoDeclaracao>  {
    return this.api.post<ResumoDeclaracao>('declaracao/GetResumoDeclaracao',request);
  }

  public saveDeclaracao(request: SaveDeclaracoesRequest) : Observable<ResumoDeclaracao>  {
    return this.api.post<ResumoDeclaracao>('declaracao/SaveDeclaracao',request);
  }

  public GetDeclaracaoRelatorios(request: GetDeclaracaoRelatoriosRequest): Observable<RelatoriosDeclaracaoListagemResponse> {
    return this.api.post<RelatoriosDeclaracaoListagemResponse>('declaracao/GetDeclaracoesRelatorios', request);
  }

  public ExtractToExcelRelatorios(request: GetDeclaracaoRelatoriosRequest): Observable<any> {
    return this.api.post<any>('declaracao/ExtractToExcelRelatorios', request);
  }
}
