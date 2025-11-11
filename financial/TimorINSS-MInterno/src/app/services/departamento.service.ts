import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { SelectDescriptionResponse } from '../response-models/utils-response';
import { DepartamentoListagemResponse } from '../response-models/departamento-response';
import { ApiHelperService } from './api-helper.service';


@Injectable({
  providedIn: 'root'
})
export class DepartamentoService {

  constructor(
    private router: Router,
    private http: HttpClient,
    private api: ApiHelperService
  ) { }


  public getAllDepartamentosAtivo(): Observable<SelectDescriptionResponse>
  {
    return this.api.get<SelectDescriptionResponse>('departamento/GetAllDepartamentosAtivo');
  }

  public getDepartamentosByUserId(id: number): Observable<DepartamentoListagemResponse>
  {
    return this.api.post<DepartamentoListagemResponse>('relUtilizadorDepartamento/GetDepartamentosByUserId', id);
  }
  
}
