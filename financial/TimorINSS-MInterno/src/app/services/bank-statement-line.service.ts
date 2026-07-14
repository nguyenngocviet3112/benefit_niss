import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import {
  AddBankStatementLineRequest,
  ConfirmBankStatementLineImportRequest,
  DeleteBankStatementLineRequest,
  MatchPagamentoRequest,
  MatchReceitaRequest,
  UnmatchRequest
} from '../request-models/bank-statement-line-request';
import {
  BankStatementLineListResponse,
  ImportBankStatementLinePreviewResponse,
  PagamentosDisponiveisParaConciliacaoResponse,
  ReceitasDisponiveisParaConciliacaoResponse
} from '../response-models/bank-statement-line-response';
import { ResponseBase } from '../response-models/utils-response';

@Injectable({
  providedIn: 'root'
})
export class BankStatementLineService {

  constructor(private http: HttpClient) { }

  public getByContaBancaria(contaBancariaFk: number | null, dataInicio?: string, dataFim?: string): Observable<BankStatementLineListResponse> {
    const url = contaBancariaFk
      ? `${environment.apiUrl}/bankstatementline/GetByContaBancaria/${contaBancariaFk}`
      : `${environment.apiUrl}/bankstatementline/GetByContaBancaria`;
    const params: { [key: string]: string } = {};
    if (dataInicio) { params.dataInicio = dataInicio; }
    if (dataFim) { params.dataFim = dataFim; }
    return this.http.get<BankStatementLineListResponse>(url, { params });
  }

  public getReceitasDisponiveis(ano: number): Observable<ReceitasDisponiveisParaConciliacaoResponse> {
    return this.http.get<ReceitasDisponiveisParaConciliacaoResponse>(`${environment.apiUrl}/bankstatementline/GetReceitasDisponiveis/${ano}`);
  }

  public getPagamentosDisponiveis(): Observable<PagamentosDisponiveisParaConciliacaoResponse> {
    return this.http.get<PagamentosDisponiveisParaConciliacaoResponse>(`${environment.apiUrl}/bankstatementline/GetPagamentosDisponiveis`);
  }

  public addLine(request: AddBankStatementLineRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/bankstatementline/AddLine`, request);
  }

  public deleteLine(request: DeleteBankStatementLineRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/bankstatementline/DeleteLine`, request);
  }

  public matchReceita(request: MatchReceitaRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/bankstatementline/MatchReceita`, request);
  }

  public matchPagamento(request: MatchPagamentoRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/bankstatementline/MatchPagamento`, request);
  }

  public unmatch(request: UnmatchRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/bankstatementline/Unmatch`, request);
  }

  public importPreview(file: File, contaBancariaFk: number): Observable<ImportBankStatementLinePreviewResponse> {
    const formData = new FormData();
    formData.append('File', file);
    formData.append('ContaBancariaFk', contaBancariaFk.toString());
    return this.http.post<ImportBankStatementLinePreviewResponse>(`${environment.apiUrl}/bankstatementline/ImportPreview`, formData);
  }

  public importConfirm(request: ConfirmBankStatementLineImportRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/bankstatementline/ImportConfirm`, request);
  }
}
