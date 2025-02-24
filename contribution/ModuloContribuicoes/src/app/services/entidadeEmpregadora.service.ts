import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';

import { Observable } from 'rxjs';
import { EntidadeEmpregadoraIdRequest, EntidadeEmpregadoraRequest } from '../request-models/entidadeEmpregadora-request';
import { EntidadeEmpregadoraConsultaResponse, EntidadeEmpregadoraDeclaracaoViewResponse } from '../response-models/entidadeEmpregadora-response';
import { map } from 'rxjs/operators';







@Injectable({
  providedIn: 'root'
})
export class EntidadeEmpregadoraService {

  constructor(
    private router: Router,
    private http: HttpClient
  ) { }


  public getEntidadeEmpregadoraByIdEntidade(id: number) : Observable<EntidadeEmpregadoraConsultaResponse>  {
    return this.http.get<EntidadeEmpregadoraConsultaResponse>(`${environment.apiUrl}/entidadeEmpregadora/GetByIdEntidade/`+id)
    .pipe(map(x => {
      let response : EntidadeEmpregadoraConsultaResponse = {
      };
      if(x == null)
        return response;
      response = x;
      return response;
    }))

  }

  public updateEntidadeEmpregadora(entity: EntidadeEmpregadoraRequest) {
    return this.http.post<boolean>(`${environment.apiUrl}/entidadeEmpregadora/UpdateEntidade`, entity);
  }

  public GetEntidadeInfoForDeclaracao(request: EntidadeEmpregadoraIdRequest) {
    return this.http.post<EntidadeEmpregadoraDeclaracaoViewResponse>(`${environment.apiUrl}/entidadeEmpregadora/GetEntidadeInfoForDeclaracao`, request);
  }

}



