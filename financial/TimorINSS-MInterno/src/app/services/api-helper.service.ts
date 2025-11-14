import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { JSEncrypt } from 'jsencrypt';

const PUBLIC_KEY = `-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAvQ4ENPCTcc5/pZZqY5Qx
YRRQ94/mYpyYb5uLd4vfL0tv/4zd0A7nXTKa9qCGaMlMPuE1hvDCdehMe3dS27J2
vg6MiK8DSPLXPiDyNzTVFnRknQJFdPLWpaVnm+Cd08LgWhiW9KFdNKDUO/oWgK05
dEXtpfZaPeHjjoGB+njXCKMmmU+hztCOkoTnpwQhMQs+Vu2jZlDxJt9yTb1337pW
OX3dFuFRtq6eGrLUuhQMtjT0vs5F8s9sk8WVNbk5CgIE/FaUx61stUrDdM70KVU3
Pbjhin/SmqRRK5QzT4vyW3RQ6GlLcTk0J+obNZEqkhlLdCpBmjtQDVfLM5eZrHEN
ZQIDAQAB
-----END PUBLIC KEY-----`;

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

  private readonly NUM_A = 99;
  private readonly NUM_B = 123456789;
  private readonly SECRET_XOR = 0x5a5a5a5a;


  private encodeIdInner(id: number): string {
    const salt = Math.floor(Math.random() * 0xffffffff);
    const xored = id ^ this.SECRET_XOR;
    const mixed = xored * this.NUM_A + this.NUM_B;
    const data = `${salt}:${mixed}`;

    return btoa(data)
      .replace(/\+/g, '-')
      .replace(/\//g, '_')
      .replace(/=+$/, '');
  }


  public encodeId(id: number): string {
    const inner = this.encodeIdInner(id); // ra chuỗi salt:mixed đã base64-url

    const encryptor = new JSEncrypt();
    encryptor.setPublicKey(PUBLIC_KEY);

    const encrypted = encryptor.encrypt(inner);
    if (!encrypted) {
      throw new Error('RSA encrypt failed');
    }

    // chuẩn hóa base64-url nếu bạn muốn bỏ vào URL
    return encrypted
      .replace(/\+/g, '-')
      .replace(/\//g, '_')
      .replace(/=+$/, '');
  }

  // public encodeId(id: number): string {
  //   // 1. Salt ngẫu nhiên (32-bit)
  //   const salt = Math.floor(Math.random() * 0xffffffff);

  //   // 2. Encode ID
  //   const xored = id ^ this.SECRET_XOR;
  //   const mixed = xored * this.NUM_A + this.NUM_B;

  //   // 3. Ghép salt + mixed vào chung
  //   const data = `${salt}:${mixed}`;

  //   // 4. Base64 URL-safe
  //   return btoa(data)
  //     .replace(/\+/g, '-')
  //     .replace(/\//g, '_')
  //     .replace(/=+$/, '');
  // }
}
