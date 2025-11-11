import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { RegimeCampoEditaveisSaveRequest, ValorCamposEditaveisDeleteRequest, ValorCamposEditaveisRequest, ValorCamposEditaveisSaveRequest } from '../request-models/camposEditaveis-request';
import { CamposEditaveisListagemResponse, ValueCampoEditavelListagemResponse } from '../response-models/camposEditaveis-response';
import { ApiHelperService } from './api-helper.service';

@Injectable({
  providedIn: 'root'
})
export class CamposEditaveisService {

  constructor(
      private router: Router,
      private http: HttpClient,
      private api: ApiHelperService
  ) {}

  public GetAllCamposEditaveis() : Observable<CamposEditaveisListagemResponse> {
    return this.api.get<CamposEditaveisListagemResponse>('camposEditaveis/GetAllCamposEditaveis');
  }

  public GetValorCampoEditavel(request: ValorCamposEditaveisRequest) : Observable<ValueCampoEditavelListagemResponse> {
    return this.api.post<ValueCampoEditavelListagemResponse>('camposEditaveis/GetValorCampoEditavel', request);
  }

  public SaveValorCampoEditavel(request: ValorCamposEditaveisSaveRequest) {
    return this.api.post<boolean>('camposEditaveis/SaveValorCampoEditavel', request);
  }

  public DeleteValorCampoEditavel(request: ValorCamposEditaveisDeleteRequest) {
    return this.api.post<boolean>('camposEditaveis/DeleteValorCampoEditavel', request);
  }

  public SaveRegimeCampoEditavel(request: RegimeCampoEditaveisSaveRequest) {
    return this.api.post<boolean>('camposEditaveis/SaveRegimeCampoEditavel', request);
  }
}
