import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { FilterRequest } from "../request-models/utils-request";
import { Observable } from 'rxjs';
import { PerfisAtivosListagemResponse, PerfisListagemResponse } from '../response-models/perfis-response';
import { PerfilRequest, PerfilListagemRequest, PerfilUpdateRequest } from '../request-models/perfil-request';
import { SelectDescriptionResponse } from '../response-models/utils-response';
import { ApiHelperService } from './api-helper.service';


@Injectable({
  providedIn: 'root'
})
export class PerfilService {

  constructor(
    private router: Router,
    private http: HttpClient,
    private api: ApiHelperService
  ) { }


  public getAllPerfis(request: PerfilListagemRequest): Observable<PerfisListagemResponse>
  {
    return this.api.post<PerfisListagemResponse>('perfil/GetAllPerfis', request);
  }

  public updatePerfil(entity: PerfilUpdateRequest) {
    return this.api.post('perfil/UpdatePerfil', entity);
  }

  public addPerfil(entity: PerfilRequest) {
    return this.api.post('perfil/AddPerfil', entity);
  }

  public editPerfil(entity: PerfilRequest) {
    return this.api.post('perfil/EditPerfil', entity);
  }

  public getAllPerfisAtivo(): Observable<SelectDescriptionResponse>
  {
    return this.api.get<SelectDescriptionResponse>('perfil/GetAllPerfisAtivo');
  }

  public getPerfisByUserId(id: number): Observable<PerfisListagemResponse>
  {
    return this.api.post<PerfisListagemResponse>('relUtilizadorPerfil/GetPerfisByUserId', id);
  }
}
