import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ImportMasterDataTreeResponse } from '../response-models/master-data-import-response';

@Injectable({
  providedIn: 'root'
})
export class MasterDataImportService {

  constructor(private http: HttpClient) { }

  public import(entityPath: string, file: File, orcamentoConfigFk: number): Observable<ImportMasterDataTreeResponse> {
    const formData = new FormData();
    formData.append('File', file);
    formData.append('OrcamentoConfigFk', orcamentoConfigFk.toString());
    return this.http.post<ImportMasterDataTreeResponse>(`${environment.apiUrl}/${entityPath}/Import`, formData);
  }
}
