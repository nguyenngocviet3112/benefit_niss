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
export class RelPerfilFuncionalidadeService {

  constructor(
    private router: Router,
    private http: HttpClient,
    private api: ApiHelperService
  ) { }


  public getRelPerfilFuncionalidadeByIdPerfil(idPerfil: number): Observable<FuncionalidadesListagemResponse>
  {
    return this.api.post<FuncionalidadesListagemResponse>('relPerfilFuncionalidade/GetRelPerfilFuncionalidadeByIdPerfil', idPerfil);
  }
}
