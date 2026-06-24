import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ContaCorrenteListagemRequest, GetAllContasStatesFromYearByFilterRequest } from '../request-models/contaCorrente-request';
import { ContaCorrenteListagemResponse, GetAllContasStatesFromYearByFilterResponse } from '../response-models/contaCorrente-response';
import { ApiHelperService } from './api-helper.service';






@Injectable({
  providedIn: 'root'
})
export class ContaCorrenteService {

  constructor(
    private router: Router,
    private http: HttpClient,
    private api: ApiHelperService
  ) { }

  public getContaCorrenteByIdEntidade(request: ContaCorrenteListagemRequest) : Observable<ContaCorrenteListagemResponse>  {
    const encodedId = this.api.encodeId(request.idEntidade!);
    const payload = { ...request, idEntidadeStr: encodedId , idEntidade: 0};
    return this.api.post<ContaCorrenteListagemResponse>('contaCorrente/GetContaCorrenteByIdEntidade',payload);
  }

  public getAllContasStatesFromYearByFilter(entity: GetAllContasStatesFromYearByFilterRequest) {
    const encodedId = this.api.encodeId(entity.idEntidade);
    const payload = { ...entity, idEntidadeStr: encodedId, idEntidade:0 };
    return this.api.post<GetAllContasStatesFromYearByFilterResponse>('contaCorrente/GetAllContasStatesFromYearByFilter', payload);
  }
}



