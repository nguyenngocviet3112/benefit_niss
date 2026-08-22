import { HttpInterceptor, HttpHandler, HttpRequest, HttpEvent, HttpResponse, HttpErrorResponse, HttpHeaders }   from '@angular/common/http';
import { Injectable } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from "@ngx-translate/core";
import { NgxSpinnerService } from 'ngx-spinner';
import { Observable, of, throwError } from "rxjs";
import { catchError, tap } from "rxjs/operators";
import { v4 as guid } from "uuid";
import { TokenStorageService } from '../services/token-storage.service';
import { ApiErrorContextService } from '../services/api-error-context.service';
import { openErrorsDialog, showExpiredError } from "../utils";

@Injectable()
export class HttpInterceptorService implements HttpInterceptor {

  public errors: string[] = [];

    constructor(
        private tokenStorage: TokenStorageService,
        public translate: TranslateService,
        public errorDialog: MatDialog,
        private spinner: NgxSpinnerService,
        private route: ActivatedRoute,
        private apiErrorContext: ApiErrorContextService
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
        const encodedUserId = this.encodePath(userId.toString());
        const modifiedReq = req.clone({
          headers: new HttpHeaders({
            'Authorization': `Bearer ${userToken}`,
            'Request-Id': requestId,
            'User-Id': encodedUserId,
            'Accept-Language': this.translate.currentLang || 'PT',
          })
        });
        return next.handle(modifiedReq).pipe(
                  catchError((err: any) => {
                      if(err instanceof HttpErrorResponse) {
                        // [PT] Guardar o detalhe tecnico antes de propagar: e a unica altura
                        // em que ainda temos o status e o corpo da resposta.
                        // [VI] Luu chi tiet ky thuat truoc khi day loi di tiep: day la luc duy
                        // nhat con giu duoc status va body cua response.
                        this.apiErrorContext.registar(err);
                        if (err.status == 401){
                          this.spinner.hide();
                          showExpiredError(this.errorDialog, this.tokenStorage, this.translate);
                        } else {
                          return throwError(err);
                        }
                      }
                      return of(err);
                  }));
      }

      public showError()
      {
        const dialogRef = openErrorsDialog(this.errors, this.errorDialog);
        this.spinner.hide();

        dialogRef.afterClosed().subscribe(() => {
          this.errors = [];
        });
      }
}
