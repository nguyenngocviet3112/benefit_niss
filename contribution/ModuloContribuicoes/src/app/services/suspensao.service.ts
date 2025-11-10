import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';

import { SuspensaoDeleteRequest, SuspensaoListagemRequest, SuspensaoRequest } from '../request-models/suspensao-request';
import { SuspensaoListagemResponse } from '../response-models/suspensao-response';
import { Observable } from 'rxjs';
import { ApiHelperService } from './api-helper.service';

@Injectable({
  providedIn: 'root'
})
export class SuspensaoService {
  public savedSuccessfully = false;

  constructor(
    private router: Router,
    private http: HttpClient,
    private api: ApiHelperService
  ) { }

  public saveSuspensao(entity: SuspensaoRequest) {
    return this.api.post<boolean>('suspensao/saveSuspensao', entity);
  }

  public getSuspensaoByIdEntidadeEmpregadora(request: SuspensaoListagemRequest) : Observable<SuspensaoListagemResponse>  {
    const encodedId = this.api.encodeId(request.IdEntidade);
    const payload = { ...request, IdEntidadeStr: encodedId };
    return this.api.post<SuspensaoListagemResponse>('suspensao/GetByIdEntidadeEmpregadora',payload);
  }

  public getSuspensaoByIdTrabalhador(request: SuspensaoListagemRequest) : Observable<SuspensaoListagemResponse>  {
    const encodedId = this.api.encodeId(request.IdEntidade);
    const payload = { ...request, id: encodedId };
    return this.api.post<SuspensaoListagemResponse>('suspensao/GetByIdTrabalhador',payload);
  }

  public deleteSuspensao(entity: SuspensaoDeleteRequest) {
    return this.api.post('suspensao/DeleteSuspensao', entity);
  }

}



