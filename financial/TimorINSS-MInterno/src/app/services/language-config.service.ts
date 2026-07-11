import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ToggleLanguageConfigRequest } from '../request-models/language-config-request';
import { LanguageConfigListResponse } from '../response-models/language-config-response';
import { ResponseBase } from '../response-models/utils-response';

@Injectable({
  providedIn: 'root'
})
export class LanguageConfigService {

  constructor(private http: HttpClient) { }

  // Admin screen — all languages, including disabled ones.
  public getAll(): Observable<LanguageConfigListResponse> {
    return this.http.get<LanguageConfigListResponse>(`${environment.apiUrl}/languageconfig/GetAll`);
  }

  // Public — drives the language dropdown, works before login.
  public getActive(): Observable<LanguageConfigListResponse> {
    return this.http.get<LanguageConfigListResponse>(`${environment.apiUrl}/languageconfig/GetActive`);
  }

  public toggle(request: ToggleLanguageConfigRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/languageconfig/Toggle`, request);
  }
}
