import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ResponseBase } from '../response-models/utils-response';

export interface UserModeAccessResponse extends ResponseBase {
  hasAccess: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class UserModeAccessService {

  constructor(private http: HttpClient) { }

  public hasAccess(): Observable<UserModeAccessResponse> {
    return this.http.get<UserModeAccessResponse>(`${environment.apiUrl}/usermodeaccess/HasAccess`);
  }
}
