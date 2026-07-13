import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { AttachmentConfigItem, AttachmentConfigResponse } from '../response-models/attachment-config-response';
import { ResponseBase } from '../response-models/utils-response';

@Injectable({
  providedIn: 'root'
})
export class AttachmentConfigService {

  constructor(private http: HttpClient) { }

  public getConfig(): Observable<AttachmentConfigResponse> {
    return this.http.get<AttachmentConfigResponse>(`${environment.apiUrl}/attachmentconfig/Get`);
  }

  public saveConfig(request: AttachmentConfigItem): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/attachmentconfig/Save`, request);
  }
}
