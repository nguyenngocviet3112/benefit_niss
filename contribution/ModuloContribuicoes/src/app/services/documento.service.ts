import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { DocumentoIdRequest, DocumentoRequest, DocumentosListagemRequest } from '../request-models/documentos-request';
import { DocumentoResponse, DocumentosListagemResponse } from '../response-models/documentos-response';

@Injectable({
  providedIn: 'root'
})
export class DocumentoService {

  constructor(
      private router: Router,
      private http: HttpClient
  ) {}


  public getDocumentosByIdTrabalhador(request: DocumentosListagemRequest) : Observable<DocumentosListagemResponse>  {
    return this.http.post<DocumentosListagemResponse>(`${environment.apiUrl}/documentos/GetByIdTrabalhador`,request);
  }

  public getDocumentoById(entity: DocumentoIdRequest) {
    return this.http.post<DocumentoResponse>(`${environment.apiUrl}/documentos/GetDocumentoById`, entity);
  }

  public saveDocumento(entity: DocumentoRequest) {
    return this.http.post<boolean>(`${environment.apiUrl}/documentos/SaveDocumento`, entity);
  }

  public updateDocumento(entity: DocumentoRequest) {
    return this.http.post<boolean>(`${environment.apiUrl}/documentos/UpdateDocumento`, entity);
  }

  public deleteDocumento(entity: DocumentoIdRequest) {
    return this.http.post(`${environment.apiUrl}/documentos/DeleteDocumento`, entity);
  }
}
