import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { AttachmentConfigService } from '../../../services/attachment-config.service';
import { AttachmentConfigItem } from '../../../response-models/attachment-config-response';

// Cấu hình Upload File — độ lớn tối đa (chung cho cả 5 bước) + bước nào bắt
// buộc phải đính kèm file trước khi submit (AD/Cabimento/Compromisso/
// Obrigação/Pagamento). Config dạng singleton (1 dòng duy nhất), không phải
// danh sách nhiều dòng như các màn settings khác.
@Component({
  selector: 'app-attachment-config',
  templateUrl: './attachment-config.component.html',
  styleUrls: ['./attachment-config.component.css']
})
export class AttachmentConfigComponent implements OnInit {

  public loading = false;
  public saving = false;

  public form: AttachmentConfigItem = {
    maxFileSizeMb: 10,
    adObrigatorio: false,
    cabimentoObrigatorio: false,
    compromissoObrigatorio: false,
    obrigacaoObrigatorio: false,
    pagamentoObrigatorio: false
  };

  constructor(
    private attachmentConfigService: AttachmentConfigService,
    private snackBar: MatSnackBar,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.load();
  }

  public load(): void {
    this.loading = true;
    this.attachmentConfigService.getConfig().subscribe(
      response => {
        this.loading = false;
        if (response.item) {
          this.form = response.item;
        }
      },
      err => {
        this.loading = false;
        this.showError(err);
      }
    );
  }

  public save(): void {
    this.saving = true;
    this.attachmentConfigService.saveConfig(this.form).subscribe(
      response => {
        this.saving = false;
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.snackBar.open(this.translate.instant('attachmentConfig.saveSuccess'), this.translate.instant('general.close'), { duration: 3000 });
      },
      err => {
        this.saving = false;
        this.showError(err);
      }
    );
  }

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('attachmentConfig.errGeneric');
    this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
  }
}
