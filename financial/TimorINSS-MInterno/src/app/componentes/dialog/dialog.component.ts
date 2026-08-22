import { browser } from 'protractor';
import { Component, Inject } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { TranslateService } from '@ngx-translate/core';
import { ApiErrorContextService } from 'src/app/services/api-error-context.service';

export interface DialogData {
  errors: string[];
  detalheTecnico?: string | null;
}

@Component({
  selector: 'app-dialog',
  templateUrl: 'dialog.component.html',
  styleUrls: ['./dialog.component.css']
})
export class DialogComponent {
  constructor(
    public dialogRef: MatDialogRef<DialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData,
    public translate: TranslateService,
    public apiErrorContext: ApiErrorContextService,
    ) {
        let newErrors: string[] = [];
        const originais: string[] = [...data.errors];
        data.errors.forEach(element => {
          var erro = element;
          if(erro.includes('ÿ'))
          {
            let splitted: string[] = erro.split('ÿ');
            erro = splitted[0];
            splitted.shift();
            newErrors.push(this.stringFormat(this.translate.instant('error.' + erro), splitted));
          }
          else
            newErrors.push(this.translate.instant('error.' + erro));
        });
        data.errors = newErrors;

        // [PT] Quando o codigo devolvido e generico (-1) ou nao tem traducao, a mensagem nao
        // diz nada a ninguem. Nesse caso mostra-se por baixo o detalhe tecnico que o
        // interceptor guardou -- status HTTP, endpoint e a mensagem real do servidor. Assim um
        // simples screenshot do utilizador ja chega para diagnosticar.
        // [VI] Khi ma loi tra ve la chung chung (-1) hoac khong co ban dich, cau thong bao
        // khong noi len dieu gi. Luc do hien them ben duoi phan chi tiet ky thuat ma
        // interceptor da luu -- status HTTP, endpoint va thong bao that tu server. Nho vay chi
        // can anh chup man hinh cua nguoi dung la du de chan doan.
        const generico = data.errors.some(e => !e || e === 'error.-1' || e.startsWith('error.'))
                      || this.dadosOriginaisSaoGenericos(originais);
        if (generico) {
          this.detalheTecnico = data.detalheTecnico ?? this.apiErrorContext.consumir();
        }
    }

  public detalheTecnico: string | null = null;

  private dadosOriginaisSaoGenericos(originais: string[]): boolean {
    return originais.some(e => e === '-1' || e === '' || e === null || e === undefined);
  }

  public stringFormat(a: any, array: any[]): string {
    for (var k in array) {
      a = a.replace("{" + k + "}", array[k])
    }
    return a;
  }

  public errorTitle = this.translate.instant('error.title');
}
