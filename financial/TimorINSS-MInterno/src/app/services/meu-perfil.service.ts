import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { AlterarEmailRequest, AlterarSenhaRequest } from '../request-models/meu-perfil-request';
import { MeuPerfilResponse } from '../response-models/meu-perfil-response';
import { ResponseBase } from '../response-models/utils-response';

@Injectable({
  providedIn: 'root'
})
export class MeuPerfilService {

  constructor(private http: HttpClient) { }

  public getProfile(): Observable<MeuPerfilResponse> {
    return this.http.get<MeuPerfilResponse>(`${environment.apiUrl}/meuperfil/GetProfile`);
  }

  public alterarEmail(request: AlterarEmailRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/meuperfil/AlterarEmail`, request);
  }

  public alterarSenha(request: AlterarSenhaRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/meuperfil/AlterarSenha`, request);
  }
}
