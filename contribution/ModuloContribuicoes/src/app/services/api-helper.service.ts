import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ApiHelperService {
  constructor(private http: HttpClient) { }

  /** 🔹 Hàm encode path chung cho toàn project */
  public encodePath(rawPath: string): string {
    return btoa(unescape(encodeURIComponent(rawPath)))
      .replace(/\+/g, '-')   // Base64 URL-safe (+ → -)
      .replace(/\//g, '_')   // (/ → _)
      .replace(/=+$/, '');   // remove trailing '='
  }

  /** 🔹 POST auto-encode */
  public post<T>(rawPath: string, body: any): Observable<T> {
    const encoded = this.encodePath(rawPath);
    return this.http.post<T>(`${environment.apiUrl}/${encoded}`, body);
  }

  /** 🔹 GET auto-encode */
  public get<T>(rawPath: string): Observable<T> {
    const encoded = this.encodePath(rawPath);
    return this.http.get<T>(`${environment.apiUrl}/${encoded}`);
  }

  private readonly NUM_A = 999;
  private readonly NUM_B = 123456789;


  public encodeId(id: number): number {
    return id * this.NUM_A + this.NUM_B;
  }

  // public encodeId(rawPath: number): string {
  //   return btoa(unescape(encodeURIComponent(rawPath)))
  //     .replace(/\+/g, '-')   // Base64 URL-safe (+ → -)
  //     .replace(/\//g, '_')   // (/ → _)
  //     .replace(/=+$/, '');   // remove trailing '='
  // }
 
  // public async encodeIdNumberOnly(id: number): Promise<string> {
  //   const v = ApiHelperService.VERSION;
  //   const r = Math.floor(Math.random() * 100); // 0..99
  //   const key = await this.deriveKey(r, v);
  //   const payload = await this.fpeEncryptNumber(id, key, ApiHelperService.LENGTH);
  //   const c = this.checksum97([v, Math.floor(r/10), r%10, ...payload.split('').map(Number)]);
  //   return `${v}${r.toString().padStart(2,'0')}${c.toString().padStart(2,'0')}${payload}`;
  // }


  
}

