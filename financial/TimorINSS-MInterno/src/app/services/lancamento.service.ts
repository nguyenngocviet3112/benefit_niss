import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { LancamentoListResponse } from '../response-models/lancamento-response';

@Injectable({
  providedIn: 'root'
})
export class LancamentoService {

  constructor(private http: HttpClient) { }

  public getList(ano?: number, mes?: number, origemTipo?: string): Observable<LancamentoListResponse> {
    let params = new HttpParams();
    if (ano) { params = params.set('ano', ano.toString()); }
    if (mes) { params = params.set('mes', mes.toString()); }
    if (origemTipo) { params = params.set('origemTipo', origemTipo); }

    return this.http.get<LancamentoListResponse>(`${environment.apiUrl}/lancamento/GetList`, { params });
  }
}
