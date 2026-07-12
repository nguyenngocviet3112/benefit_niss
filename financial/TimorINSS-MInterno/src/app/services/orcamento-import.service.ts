import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ConfirmOrcamentoImportRequest } from '../request-models/orcamento-import-request';
import { ImportOrcamentoPreviewResponse } from '../response-models/orcamento-import-response';
import { ImportMasterDataTreeResponse } from '../response-models/master-data-import-response';

@Injectable({
  providedIn: 'root'
})
export class OrcamentoImportService {

  constructor(private http: HttpClient) { }

  public preview(file: File, orcamentoConfigFk: number): Observable<ImportOrcamentoPreviewResponse> {
    const formData = new FormData();
    formData.append('File', file);
    formData.append('OrcamentoConfigFk', orcamentoConfigFk.toString());
    return this.http.post<ImportOrcamentoPreviewResponse>(`${environment.apiUrl}/orcamento/ImportPreview`, formData);
  }

  public confirm(request: ConfirmOrcamentoImportRequest): Observable<ImportMasterDataTreeResponse> {
    return this.http.post<ImportMasterDataTreeResponse>(`${environment.apiUrl}/orcamento/ConfirmImport`, request);
  }
}
