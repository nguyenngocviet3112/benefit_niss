import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';

import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { EntidadeEmpregadoraIdRequest, EntidadeEmpregadoraNissRequest, EntidadeEmpregadoraRequest, UpsertEntidadeEmpregadoraRequest } from '../request-models/entidadeEmpregadora-request';
import { EntidadeEmpregadoraConsultaResponse, EntidadeEmpregadoraDeclaracaoViewResponse } from '../response-models/entidadeEmpregadora-response';
import { ApiHelperService } from './api-helper.service';

@Injectable({
  providedIn: 'root'
})
export class EntidadeEmpregadoraService {

  constructor(
    private http: HttpClient,
    private api: ApiHelperService
  ) { }


  public getEntidadeEmpregadoraByIdEntidade(id: number) : Observable<EntidadeEmpregadoraConsultaResponse>  {
    return this.api.get<EntidadeEmpregadoraConsultaResponse>('entidadeEmpregadora/GetByIdEntidade/'+id)
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
    return this.api.post<boolean>('entidadeEmpregadora/UpdateEntidade', entity);
  }

  public GetEntidadeInfoForDeclaracao(request: EntidadeEmpregadoraIdRequest) {
    return this.api.post<EntidadeEmpregadoraDeclaracaoViewResponse>('entidadeEmpregadora/GetEntidadeInfoForDeclaracao', request);
  }

  public GetEntidadeByNiss(request: EntidadeEmpregadoraNissRequest) {
    return this.api.post<EntidadeEmpregadoraDeclaracaoViewResponse>('entidadeEmpregadora/GetEntidadeByNiss', request);
  }

  public upsertEntidadeEmpregadora(request: UpsertEntidadeEmpregadoraRequest) {
    return this.api.post('entidadeEmpregadora/upsert', request);
  }

}



