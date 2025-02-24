import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { GetDeclaracaoByEntidadeAndFilterResponse, ResumoDeclaracao } from '../response-models/declaracao-response';
import { GetDeclaracaoByEntidadeAndFilterRequest, GetResumoDeclaracaoRequest, SaveDeclaracoesRequest } from './../request-models/declaracao-request';

@Injectable({
  providedIn: 'root'
})
export class DeclaracaoService {

  constructor(
      private router: Router,
      private http: HttpClient
  ) {}


  public getDeclaracaoByEntidadeAndFilter(request: GetDeclaracaoByEntidadeAndFilterRequest) : Observable<GetDeclaracaoByEntidadeAndFilterResponse>  {
    return this.http.post<GetDeclaracaoByEntidadeAndFilterResponse>(`${environment.apiUrl}/declaracao/GetDeclaracaoByEntidadeAndFilter`,request);
  }

  public getResumoDeclaracao(request: GetResumoDeclaracaoRequest) : Observable<ResumoDeclaracao>  {
    return this.http.post<ResumoDeclaracao>(`${environment.apiUrl}/declaracao/GetResumoDeclaracao`,request);
  }

  public saveDeclaracao(request: SaveDeclaracoesRequest) : Observable<ResumoDeclaracao>  {
    return this.http.post<ResumoDeclaracao>(`${environment.apiUrl}/declaracao/SaveDeclaracao`,request);
  }
}
