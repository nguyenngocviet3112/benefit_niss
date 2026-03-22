import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { DocumentoIdRequest, DocumentoRequest, DocumentosListagemRequest, TarefaDocumentoRequest } from '../request-models/documentos-request';
import { DocumentoResponse, DocumentosListagemResponse, DocumentosTarefaListagemResponse } from '../response-models/documentos-response';
import { ApiHelperService } from './api-helper.service';

@Injectable({
  providedIn: 'root'
})
export class DocumentoService {

  constructor(
      private router: Router,
      private http: HttpClient,
      private api: ApiHelperService
  ) {}


  public getDocumentosByIdTrabalhador(request: DocumentosListagemRequest) : Observable<DocumentosListagemResponse>  {
    const encodedId = this.api.encodeId(request.id);
    const payload = { ...request, idStr: encodedId , id:0};
    return this.api.post<DocumentosListagemResponse>('documentos/GetByIdTrabalhador',payload);
  }

  public getDocumentoById(entity: DocumentoIdRequest) {
    const encodedId = this.api.encodeId(entity.id);
    const payload = { ...entity, idStr: encodedId , id:0};
    return this.api.post<DocumentoResponse>('documentos/GetDocumentoById', payload);
  }

  public saveDocumento(entity: DocumentoRequest) {
    return this.api.post<boolean>('documentos/SaveDocumento', entity);
  }

  public updateDocumento(entity: DocumentoRequest) {
    return this.api.post<boolean>('documentos/UpdateDocumento', entity);
  }

  public deleteDocumento(entity: DocumentoIdRequest) {
    return this.api.post('documentos/DeleteDocumento', entity);
  }

  public getDocumentosByIdTarefaAtivo(request: DocumentosListagemRequest) : Observable<DocumentosTarefaListagemResponse>  {
    const encodedId = this.api.encodeId(request.id);
    const payload = { ...request, idStr: encodedId , id:0};

    return this.api.post<DocumentosTarefaListagemResponse>('documentos/getDocumentosByIdTarefaAtivo',payload);
  }

  public getDocumentosByIdProcessoAtivo(request: DocumentosListagemRequest) : Observable<DocumentosTarefaListagemResponse>  {
    const encodedId = this.api.encodeId(request.id);
    const payload = { ...request, idStr: encodedId , id:0};
    return this.api.post<DocumentosTarefaListagemResponse>('documentos/getDocumentosByIdProcessoAtivo',payload);
  }

  public SaveTarefaDocumento(request: TarefaDocumentoRequest) {
    return this.api.post('documentos/SaveTarefaDocumento', request);
  }

  public deleteDocumentoComponente(entity: DocumentoIdRequest) {
    return this.api.post('documentos/DeleteDocumentoComponente', entity);
  }
}
