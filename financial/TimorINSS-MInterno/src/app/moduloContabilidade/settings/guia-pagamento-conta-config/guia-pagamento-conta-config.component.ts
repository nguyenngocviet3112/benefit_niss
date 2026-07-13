import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { CodigoContaOptionDataContract } from '../../../response-models/payment-response';
import { GuiaPagamentoContaConfigModel, GuiaPagamentoContaConfigService } from '../../../services/guia-pagamento-conta-config.service';

@Component({
  selector: 'app-guia-pagamento-conta-config',
  templateUrl: './guia-pagamento-conta-config.component.html',
  styleUrls: ['./guia-pagamento-conta-config.component.css']
})
export class GuiaPagamentoContaConfigComponent implements OnInit {

  public loading = false;
  public saving = false;

  public current: GuiaPagamentoContaConfigModel | null = null;
  public codigoContaOptions: CodigoContaOptionDataContract[] = [];

  public creditoPrivadoSearch = '';
  public filteredCreditoPrivado: CodigoContaOptionDataContract[] = [];
  public formCodigoContaCreditoPrivadoFk: number | null = null;

  public creditoPublicoSearch = '';
  public filteredCreditoPublico: CodigoContaOptionDataContract[] = [];
  public formCodigoContaCreditoPublicoFk: number | null = null;

  constructor(
    private guiaPagamentoContaConfigService: GuiaPagamentoContaConfigService,
    private snackBar: MatSnackBar,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.load();
  }

  public load(): void {
    this.loading = true;
    // Phải tải xong codigoContaOptions rồi mới đọc config (không gọi song song) —
    // xem bug tương tự đã sửa ở liquidacao-conta-config.component.ts: nếu getConfig
    // trả về trước, filteredCreditoPrivado/Publico bị gán = mảng options rỗng lúc
    // đó và không tự cập nhật lại khi options tải xong sau.
    this.guiaPagamentoContaConfigService.getCodigoContaOptions().subscribe(
      response => {
        this.codigoContaOptions = response.items ?? [];
        this.filteredCreditoPrivado = this.codigoContaOptions;
        this.filteredCreditoPublico = this.codigoContaOptions;
        this.loadConfig();
      },
      err => { this.loading = false; this.showError(err); }
    );
  }

  private loadConfig(): void {
    this.guiaPagamentoContaConfigService.getConfig().subscribe(
      response => {
        this.current = response.item ?? null;
        this.formCodigoContaCreditoPrivadoFk = this.current?.codigoContaCreditoPrivadoFk ?? null;
        this.creditoPrivadoSearch = this.current?.codigoContaCreditoPrivadoDesignacao ?? '';
        this.formCodigoContaCreditoPublicoFk = this.current?.codigoContaCreditoPublicoFk ?? null;
        this.creditoPublicoSearch = this.current?.codigoContaCreditoPublicoDesignacao ?? '';
        this.loading = false;
      },
      err => { this.loading = false; this.showError(err); }
    );
  }

  private normalize(value: string): string {
    return (value || '').toLowerCase();
  }

  public onCreditoPrivadoSearchChange(): void {
    const term = this.normalize(this.creditoPrivadoSearch);
    this.filteredCreditoPrivado = this.codigoContaOptions.filter(c => this.normalize(c.designacao).includes(term));
  }

  public selectCreditoPrivado(c: CodigoContaOptionDataContract): void {
    this.formCodigoContaCreditoPrivadoFk = c.id;
    this.creditoPrivadoSearch = c.designacao;
  }

  public onCreditoPublicoSearchChange(): void {
    const term = this.normalize(this.creditoPublicoSearch);
    this.filteredCreditoPublico = this.codigoContaOptions.filter(c => this.normalize(c.designacao).includes(term));
  }

  public selectCreditoPublico(c: CodigoContaOptionDataContract): void {
    this.formCodigoContaCreditoPublicoFk = c.id;
    this.creditoPublicoSearch = c.designacao;
  }

  public get canSave(): boolean {
    return !!this.formCodigoContaCreditoPrivadoFk || !!this.formCodigoContaCreditoPublicoFk;
  }

  public save(): void {
    if (!this.canSave) { return; }
    this.saving = true;
    this.guiaPagamentoContaConfigService.save({
      codigoContaCreditoPrivadoFk: this.formCodigoContaCreditoPrivadoFk,
      codigoContaCreditoPublicoFk: this.formCodigoContaCreditoPublicoFk
    }).subscribe(
      response => {
        this.saving = false;
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 5000 });
          return;
        }
        this.snackBar.open(this.translate.instant('guiaPagamentoContaConfig.savedSuccess'), this.translate.instant('general.close'), { duration: 3000 });
        this.load();
      },
      err => { this.saving = false; this.showError(err); }
    );
  }

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('guiaPagamentoContaConfig.errGeneric');
    this.snackBar.open(message, this.translate.instant('general.close'), { duration: 5000 });
  }
}
