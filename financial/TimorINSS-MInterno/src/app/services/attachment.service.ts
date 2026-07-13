import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { AttachmentListResponse, UploadAttachmentResponse } from '../response-models/attachment-response';

export interface UploadAttachmentRequest {
  entityType: string;
  entityId: number;
  fileName: string;
  contentType: string;
  fileContentBase64: string;
}

@Injectable({
  providedIn: 'root'
})
export class AttachmentService {

  constructor(private http: HttpClient) { }

  public getByEntity(entityType: string, entityId: number): Observable<AttachmentListResponse> {
    return this.http.get<AttachmentListResponse>(`${environment.apiUrl}/attachment/GetByEntity/${entityType}/${entityId}`);
  }

  public upload(request: UploadAttachmentRequest): Observable<UploadAttachmentResponse> {
    return this.http.post<UploadAttachmentResponse>(`${environment.apiUrl}/attachment/Upload`, request);
  }

  // Blob (not base64/JSON) — goes through the same auth interceptor, then the
  // component opens it via an Object URL (inline preview for PDF/PNG, download
  // for Excel — matches the backend's "inline" Content-Disposition).
  public downloadBlob(id: number): Observable<Blob> {
    return this.http.get(`${environment.apiUrl}/attachment/Download/${id}`, { responseType: 'blob' });
  }
}
