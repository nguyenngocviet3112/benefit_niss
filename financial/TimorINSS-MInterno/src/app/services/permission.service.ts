import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ResponseBase } from '../response-models/utils-response';
import { TokenStorageService } from './token-storage.service';
import { SelectDescription } from '../models/utils';

export interface PermissionTokenModel {
  token: string;
  label: string;
}

export interface PermissionGroupModel {
  codigo: string;
  nome: string;
  tokens: PermissionTokenModel[];
}

export interface PermissionPresetModel {
  id: number;
  codigo: string;
  nome: string;
  tokens: string[];
}

export interface PermissionCatalogResponse extends ResponseBase {
  groups: PermissionGroupModel[];
  presets: PermissionPresetModel[];
  departamentos: SelectDescription[];
}

export interface UserPermissionListItem {
  id: number;
  username: string;
  indActivo: boolean;
  locked: boolean;
  nome: string;
  email: string;
  departamentoFk: number | null;
  departamentoNome: string;
  tokens: string[];
  presetCodigos: string[];
}

export interface UserPermissionListResponse extends ResponseBase {
  items: UserPermissionListItem[];
}

export interface UserPermissionDetailResponse extends ResponseBase {
  item: UserPermissionListItem;
}

export interface SaveUserPermissionRequest {
  id: number;
  username: string;
  password: string;
  indActivo: boolean;
  nome: string;
  email: string;
  departamentoFk: number | null;
  presetIds: number[];
  tokens: string[];
}

export interface SaveUserPermissionResponse extends ResponseBase {
  id: number;
}

// Backs the "Quản lý User & Phân quyền" admin screen — additive granular RBAC,
// separate from the old Perfil/Funcionalidade permission system and from
// UserModeAccessService (that one is only the binary new-mode gate).
@Injectable({
  providedIn: 'root'
})
export class PermissionService {

  constructor(private http: HttpClient, private tokenStorage: TokenStorageService) { }

  public getCatalog(): Observable<PermissionCatalogResponse> {
    return this.http.get<PermissionCatalogResponse>(`${environment.apiUrl}/userpermission/GetCatalog`);
  }

  public getUsers(): Observable<UserPermissionListResponse> {
    return this.http.get<UserPermissionListResponse>(`${environment.apiUrl}/userpermission/GetUsers`);
  }

  public getUser(id: number): Observable<UserPermissionDetailResponse> {
    return this.http.get<UserPermissionDetailResponse>(`${environment.apiUrl}/userpermission/GetUser/${id}`);
  }

  public saveUser(request: SaveUserPermissionRequest): Observable<SaveUserPermissionResponse> {
    return this.http.post<SaveUserPermissionResponse>(`${environment.apiUrl}/userpermission/SaveUser`, request);
  }

  // Decodes the "perms" claim out of the currently stored JWT client-side (no
  // API roundtrip) — same manual base64 decode approach TokenStorageService
  // already uses for "exp" (tokenExpired()), no jwt-decode dependency needed.
  public getCurrentPerms(): string[] {
    const token = this.tokenStorage.getToken();
    if (!token) {
      return [];
    }
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const perms = payload.perms;
      if (!perms) {
        return [];
      }
      return Array.isArray(perms) ? perms : [perms];
    } catch {
      return [];
    }
  }

  public hasPerm(token: string): boolean {
    const perms = this.getCurrentPerms();
    return perms.includes('ADMIN') || perms.includes(token);
  }
}
