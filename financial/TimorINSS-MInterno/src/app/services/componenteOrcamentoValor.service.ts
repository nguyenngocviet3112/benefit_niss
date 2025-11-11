import { Observable } from 'rxjs';
import { SearchComponentesOrcamentoValorResponse } from './../response-models/componenteOrcamentoValor-response';
import { EliminarComponenteOrcamentoValorRequest, SearchComponenteOrcamentoValorRequest } from './../request-models/componenteOrcamentoValor-request';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { AddComponenteOrcamentoValorRequest } from '../request-models/componenteOrcamentoValor-request';
import { ApiHelperService } from './api-helper.service';

@Injectable({
  providedIn: 'root'
})
export class componenteOrcamentoValorService {

  constructor(
    private router: Router,
    private http: HttpClient,
    private api: ApiHelperService
  ) { }

  public AddOrcamentoValor(request: AddComponenteOrcamentoValorRequest) {
    return this.api.post('componenteOrcamentoValor/AddOrcamentoValor', request);
  }

  public SearchOrcamentoValor(request: SearchComponenteOrcamentoValorRequest): Observable<SearchComponentesOrcamentoValorResponse> {
    return this.api.post<SearchComponentesOrcamentoValorResponse>('componenteOrcamentoValor/SearchOrcamentoValor', request);
  }

  public editarOrcamentoValor(request: AddComponenteOrcamentoValorRequest) {
    return this.api.post<AddComponenteOrcamentoValorRequest>('componenteOrcamentoValor/EditOrcamentoValor', request);
  }

  public eliminarOrcamentoValor(request: EliminarComponenteOrcamentoValorRequest) {
    return this.api.post<EliminarComponenteOrcamentoValorRequest>('componenteOrcamentoValor/EliminarOrcamentoValor', request);
  }
}
