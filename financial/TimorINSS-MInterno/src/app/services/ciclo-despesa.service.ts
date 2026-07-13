import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { CicloDespesaListResponse } from '../response-models/ciclo-despesa-response';
import { StringFileResponse } from '../response-models/utils-response';

@Injectable({
  providedIn: 'root'
})
export class CicloDespesaService {

  constructor(private http: HttpClient) { }

  public getByAno(ano: number, institution?: number): Observable<CicloDespesaListResponse> {
    let url = `${environment.apiUrl}/cicloDespesa/GetByAno/${ano}`;
    if (institution) {
      url += `?institution=${institution}`;
    }
    return this.http.get<CicloDespesaListResponse>(url);
  }

  public getByAnoExcel(ano: number, institution?: number): Observable<StringFileResponse> {
    let url = `${environment.apiUrl}/cicloDespesa/GetByAnoExcel/${ano}`;
    if (institution) {
      url += `?institution=${institution}`;
    }
    return this.http.get<StringFileResponse>(url);
  }
}
