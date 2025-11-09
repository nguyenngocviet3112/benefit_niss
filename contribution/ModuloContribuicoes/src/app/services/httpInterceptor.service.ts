import { HttpEvent, HttpHandler, HttpHeaders, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { v4 as guid } from 'uuid';
import { TokenStorageService } from '../services/token-storage.service';


@Injectable()
export class HttpInterceptorService implements HttpInterceptor {
  
    constructor(
        private tokenStorage: TokenStorageService,
      ) { }
    public encodePath(rawPath: string): string {
    return btoa(unescape(encodeURIComponent(rawPath)))
      .replace(/\+/g, '-')   // Base64 URL-safe (+ → -)
      .replace(/\//g, '_')   // (/ → _)
      .replace(/=+$/, '');   // remove trailing '='
    }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const userToken = this.tokenStorage.getToken();
    const userId = this.tokenStorage.getUser()?.id ?? '';
    const requestId = guid();

    // 🔐 Base64 encode userId
    const encodedUserId = this.encodePath(userId.toString());

    const modifiedReq = req.clone({
      headers: new HttpHeaders({
        'Authorization': `Bearer ${userToken}`,
        'Request-Id': requestId,
        'User-Id': encodedUserId, // mã hóa base64
      })
    });

    return next.handle(modifiedReq);
  }
}
//     intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
//         const userToken = this.tokenStorage.getToken();
//         const userId = this.tokenStorage.getUser()?.id;
//         const requestId = guid();
//         const modifiedReq = req.clone({ 
//           headers: new HttpHeaders({
//             'Authorization': `Bearer ${userToken}`,
//             'Request-Id': requestId,
//             'User-Id': `${userId}`,
//           })
//         });
//         return next.handle(modifiedReq);
//       }
// }