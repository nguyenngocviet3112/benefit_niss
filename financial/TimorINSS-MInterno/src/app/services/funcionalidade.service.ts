import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { FuncionalidadesListagemResponse } from '../response-models/funcionalidade-response';
import { ApiHelperService } from './api-helper.service';


@Injectable({
  providedIn: 'root'
})
export class FuncionalidadeService {

  constructor(
    private router: Router,
    private http: HttpClient,
    private api: ApiHelperService
  ) { }


  public getAllFuncionalidades(): Observable<FuncionalidadesListagemResponse>
  {
    return this.api.get<FuncionalidadesListagemResponse>('funcionalidade/GetAllFuncionalidades');
  }

}
