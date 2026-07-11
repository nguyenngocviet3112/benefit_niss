import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ResponseBase } from '../response-models/utils-response';

export interface BankAccountModel {
  id: number;
  entidadeBancaria: string;
  descricao: string;
  swift: string;
  iban: string;
  numero: string;
}

export interface BankAccountListResponse extends ResponseBase {
  items: BankAccountModel[];
}

export interface SaveBankAccountRequest {
  id: number;
  entidadeBancaria: string;
  descricao: string;
  swift: string;
  iban: string;
  numero: string;
}

export interface SaveBankAccountResponse extends ResponseBase {
  id: number;
}

// Dedicated new-mode CRUD for the existing Contabancaria table (reused as-is)
// — Cấu hình hệ thống > Ngân hàng. Other new-mode screens (Pagamento,
// Receita) should read this same list for their "chọn ngân hàng" dropdowns.
@Injectable({
  providedIn: 'root'
})
export class BankAccountService {

  constructor(private http: HttpClient) { }

  public getAll(): Observable<BankAccountListResponse> {
    return this.http.get<BankAccountListResponse>(`${environment.apiUrl}/bankaccount/GetAll`);
  }

  public save(request: SaveBankAccountRequest): Observable<SaveBankAccountResponse> {
    return this.http.post<SaveBankAccountResponse>(`${environment.apiUrl}/bankaccount/Save`, request);
  }
}
