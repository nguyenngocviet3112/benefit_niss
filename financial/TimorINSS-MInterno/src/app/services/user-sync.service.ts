import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ResponseBase } from '../response-models/utils-response';

export interface UserSyncListItem {
  id: number;
  username: string;
  indActivo: boolean;
  locked: boolean;
  hasNewModeAccess: boolean;
  nome: string;
  email: string;
  departamentoNome: string;
}

export interface UserSyncListResponse extends ResponseBase {
  items: UserSyncListItem[];
}

export interface SyncInternalUserRequest {
  id: number;
  nome: string;
  email: string;
  departamentoFk: number | null;
}

export interface SyncInternalUserResponse extends ResponseBase {
}

// Backs "Đồng bộ User từ hệ thống cũ" — additive onboarding tool, không đụng
// dữ liệu Utilizador/Perfil cũ. Chỉ tạo UserModeAccess + UserProfile cho user
// internal được chọn; user external chỉ để xem (không gọi SyncInternal).
@Injectable({
  providedIn: 'root'
})
export class UserSyncService {

  constructor(private http: HttpClient) { }

  public getInternalList(): Observable<UserSyncListResponse> {
    return this.http.get<UserSyncListResponse>(`${environment.apiUrl}/usersync/GetInternalList`);
  }

  public getExternalList(): Observable<UserSyncListResponse> {
    return this.http.get<UserSyncListResponse>(`${environment.apiUrl}/usersync/GetExternalList`);
  }

  public syncInternal(request: SyncInternalUserRequest): Observable<SyncInternalUserResponse> {
    return this.http.post<SyncInternalUserResponse>(`${environment.apiUrl}/usersync/SyncInternal`, request);
  }
}
