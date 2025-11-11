import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { DadosUtilizadorRequest, UserUpdateRequest, UtilizadorListagemRequest, UtilizadorRequest } from '../request-models/utilizador-request';
import { UtilizadoresAcessoListagemResponse, UtilizadoresListagemResponse } from '../response-models/utilizadores-response';
import { DadosUtilizadorResponse } from '../response-models/utilizador-response';
import { map } from 'rxjs/operators';
import { ApiHelperService } from './api-helper.service';


@Injectable({
  providedIn: 'root'
})
export class UtilizadorService {

  constructor(
    private router: Router,
    private http: HttpClient,
    private api: ApiHelperService
  ) { }


  public GetAllUtilizadoresInterno(request: UtilizadorListagemRequest): Observable<UtilizadoresListagemResponse>
  {
    return this.api.post<UtilizadoresListagemResponse>('utilizadores/GetAllUtilizadoresInterno', request);
  }

  public GetUtilizadoresInternoByPerfil(request: UtilizadorListagemRequest): Observable<UtilizadoresListagemResponse>
  {
    return this.api.post<UtilizadoresListagemResponse>('utilizadores/GetUtilizadoresInternoByPerfil', request);
  }

  public GetAllDadosUtilizador(request: DadosUtilizadorRequest) : Observable<DadosUtilizadorResponse>  {
    return this.api.post<DadosUtilizadorResponse>('utilizadores/GetAllDadosUtilizador/',request)
  }

  public AddUtilizador(request: UtilizadorRequest) {
    return this.api.post('utilizadores/AddUtilizador', request);
  }

  public GetAllAcessoUtilizadores(request: UtilizadorListagemRequest): Observable<UtilizadoresAcessoListagemResponse>
  {
    return this.api.post<UtilizadoresAcessoListagemResponse>('utilizadores/GetAllAcessoUtilizadores', request);
  }

  public SwitchUserBlockState(request: UserUpdateRequest) {
    return this.api.post('utilizadores/SwitchUserBlockState', request);
}
}
