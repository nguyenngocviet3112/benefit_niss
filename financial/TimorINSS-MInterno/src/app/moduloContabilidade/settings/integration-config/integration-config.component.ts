import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { IntegrationConfigService } from '../../../services/integration-config.service';
import { IntegrationConfigItem } from '../../../response-models/integration-config-response';

// Cấu hình tích hợp — bật/tắt cho phép API gọi vào từ module ngoài (hiện chỉ
// có Benefit module gọi vào api/benefit + api/benefit-data để lấy thông tin
// NLĐ/công ty/lịch sử đóng góp). Config dạng singleton (1 dòng duy nhất).
@Component({
  selector: 'app-integration-config',
  templateUrl: './integration-config.component.html',
  styleUrls: ['./integration-config.component.css']
})
export class IntegrationConfigComponent implements OnInit {

  public loading = false;
  public saving = false;

  public form: IntegrationConfigItem = {
    benefitApiEnabled: true
  };

  constructor(
    private integrationConfigService: IntegrationConfigService,
    private snackBar: MatSnackBar,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.load();
  }

  public load(): void {
    this.loading = true;
    this.integrationConfigService.getConfig().subscribe(
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
    this.integrationConfigService.saveConfig(this.form).subscribe(
      response => {
        this.saving = false;
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.snackBar.open(this.translate.instant('integrationConfig.saveSuccess'), this.translate.instant('general.close'), { duration: 3000 });
      },
      err => {
        this.saving = false;
        this.showError(err);
      }
    );
  }

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('integrationConfig.errGeneric');
    this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
  }
}
