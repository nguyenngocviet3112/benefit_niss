import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import * as CryptoJS from 'crypto-js';

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

  // private readonly NUM_A = 999;
  // private readonly NUM_B = 123456789;


  // public encodeId(id: number): string {
  //   const encoded = btoa(unescape(encodeURIComponent(id * this.NUM_A + this.NUM_B)));
  //   return btoa(unescape(encodeURIComponent(encoded)))
  //     .replace(/\+/g, '-')   // Base64 URL-safe (+ → -)
  //     .replace(/\//g, '_')   // (/ → _)
  //     .replace(/=+$/, '');   // remove trailing '='
  // }

private readonly NUM_A = 99;
private readonly NUM_B = 123456789;
private readonly SECRET_XOR = 0x5a5a5a5a;


public encodeId(id: number): string {
  // 1. Salt ngẫu nhiên (32-bit)
  const salt = Math.floor(Math.random() * 0xffffffff);

  // 2. Encode ID
  const xored = id ^ this.SECRET_XOR;
  const mixed = xored * this.NUM_A + this.NUM_B;

  // 3. Ghép salt + mixed vào chung
  const data = `${salt}:${mixed}`;

  // 4. Base64 URL-safe
  return btoa(data)
    .replace(/\+/g, '-')
    .replace(/\//g, '_')
    .replace(/=+$/, '');
}
}
