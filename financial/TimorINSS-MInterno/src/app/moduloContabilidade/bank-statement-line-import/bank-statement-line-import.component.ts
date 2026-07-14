import { Component, EventEmitter, Input, Output, ViewChild, ElementRef } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { BankStatementLineService } from '../../services/bank-statement-line.service';
import { BankStatementLineImportRow } from '../../response-models/bank-statement-line-response';

// Import Excel sao kê ngân hàng — luồng 2 bước preview → confirm (CLAUDE.md
// §6): 1) chọn file → gọi ImportPreview, chỉ đối chiếu trùng lặp, KHÔNG ghi
// gì cả. 2) hiện danh sách dòng đọc được, đánh dấu dòng nghi trùng lặp
// (cùng Conta+Ngày+Số tiền+Mô tả với 1 dòng đã có) — người dùng tự chọn
// Insert/Bỏ qua cho từng dòng, mặc định Bỏ qua cho dòng nghi trùng (an toàn),
// Insert cho dòng mới. Dữ liệu nạp vào đây phục vụ CHUNG cho cả đối chiếu
// Receita/Pagamento (màn này) lẫn Guia Pagamento (màn Duyệt Guia Pagamento) —
// 2026-07-13, xem memory bank-statement-line-guia-pagamento-unification.
@Component({
  selector: 'app-bank-statement-line-import',
  templateUrl: './bank-statement-line-import.component.html',
  styleUrls: ['./bank-statement-line-import.component.css']
})
export class BankStatementLineImportComponent {

  @Input() contaBancariaFk: number | null = null;
  @Output() imported = new EventEmitter<void>();

  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  public loading = false;
  public saving = false;
  public showPreview = false;
  public rows: BankStatementLineImportRow[] = [];

  constructor(
    private bankStatementLineService: BankStatementLineService,
    private snackBar: MatSnackBar,
    private translate: TranslateService
  ) { }

  public triggerFileSelect(): void {
    if (!this.contaBancariaFk) {
      this.snackBar.open(this.translate.instant('bankStatementLineImport.errMissingBank'), this.translate.instant('general.close'), { duration: 3000 });
      return;
    }
    this.fileInput.nativeElement.click();
  }

  public onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file || !this.contaBancariaFk) {
      return;
    }

    this.loading = true;
    this.bankStatementLineService.importPreview(file, this.contaBancariaFk).subscribe(
      response => {
        this.loading = false;
        input.value = '';
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }

        this.rows = (response.rows ?? []).map(r => ({
          ...r,
          action: r.isDuplicate ? 'Skip' : 'Insert'
        }));
        this.showPreview = true;
      },
      err => {
        this.loading = false;
        input.value = '';
        const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('bankStatementLineImport.errGeneric');
        this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
      }
    );
  }

  public get insertCount(): number {
    return this.rows.filter(r => r.action === 'Insert').length;
  }

  public confirmImport(): void {
    if (!this.contaBancariaFk) { return; }

    const payloadRows = this.rows.map(r => ({
      rowNum: r.rowNum,
      dataValor: r.dataValor,
      descricao: r.descricao,
      credito: r.credito,
      debito: r.debito,
      action: r.action ?? 'Skip'
    }));

    this.saving = true;
    this.bankStatementLineService.importConfirm({ contaBancariaFk: this.contaBancariaFk, rows: payloadRows }).subscribe(
      response => {
        this.saving = false;
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.snackBar.open(this.translate.instant('bankStatementLineImport.importSuccess', { count: this.insertCount }), this.translate.instant('general.close'), { duration: 3000 });
        this.closePreview();
        this.imported.emit();
      },
      err => {
        this.saving = false;
        const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('bankStatementLineImport.errGeneric');
        this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
      }
    );
  }

  public closePreview(): void {
    this.showPreview = false;
    this.rows = [];
  }
}
