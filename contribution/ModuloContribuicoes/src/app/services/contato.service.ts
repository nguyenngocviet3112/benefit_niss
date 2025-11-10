import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';

import { Observable } from 'rxjs';
import { ContatoDeleteRequest, ContatoListagemRequest } from '../request-models/contato-request';
import { ContatoRequest } from '../request-models/contato-request';
import { ContatoListagemResponse } from '../response-models/contato-response';
import { ApiHelperService } from './api-helper.service';



@Injectable({
  providedIn: 'root'
})
export class ContatoService {

  constructor(
    private router: Router,
    private http: HttpClient,
    private api: ApiHelperService
  ) { }


  public getContatoByIdEntidadeEmpregadora(request: ContatoListagemRequest) : Observable<ContatoListagemResponse>  {
    const encodedId = this.api.encodeId(request.Id);
    const payload = { ...request, idStr: encodedId };
    return this.api.post<ContatoListagemResponse>('contato/GetByIdEntidadeEmpregadora',payload);
  }

  public getContatoByIdTrabalhador(request: ContatoListagemRequest) : Observable<ContatoListagemResponse>  {
    const encodedId = this.api.encodeId(request.Id);
    const payload = { ...request, idStr: encodedId };
    return this.api.post<ContatoListagemResponse>('contato/GetByIdTrabalhador',payload);
  }

  public saveContato(entity: ContatoRequest) {
    return this.api.post<boolean>('contato/SaveContato', entity);
  }

  public updateContato(entity: ContatoRequest) {
    return this.api.post<boolean>('contato/UpdateContato', entity);
  }

  public deleteContato(entity: ContatoDeleteRequest) {
    return this.api.post('contato/DeleteContato', entity);
  }
}
