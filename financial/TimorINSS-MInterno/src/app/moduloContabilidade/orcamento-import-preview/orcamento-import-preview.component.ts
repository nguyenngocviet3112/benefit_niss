import { Component, EventEmitter, Input, Output, ViewChild, ElementRef } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { OrcamentoImportService } from '../../services/orcamento-import.service';
import { OrcamentoImportRow } from '../../response-models/orcamento-import-response';

// Import Excel Orçamento — luồng 2 bước preview → confirm (CLAUDE.md §6):
// 1) chọn file → gọi ImportPreview, đối chiếu với DB, KHÔNG ghi gì cả.
// 2) hiện danh sách: New (sẽ thêm), Exists (đã có — chọn Ghi đè/Bỏ qua),
//    Error (không sửa được, phải sửa lại file gốc). Người dùng xem xong mới
//    bấm "Lưu" (ConfirmImport) để thực sự ghi DB.
@Component({
  selector: 'app-orcamento-import-preview',
  templateUrl: './orcamento-import-preview.component.html',
  styleUrls: ['./orcamento-import-preview.component.css']
})
export class OrcamentoImportPreviewComponent {

  @Input() orcamentoConfigFk = 1;
  @Output() imported = new EventEmitter<void>();

  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  public loading = false;
  public saving = false;
  public showPreview = false;
  public rows: OrcamentoImportRow[] = [];
  public totalNew = 0;
  public totalExists = 0;
  public totalErrors = 0;

  constructor(
    private orcamentoImportService: OrcamentoImportService,
    private snackBar: MatSnackBar,
    private translate: TranslateService
  ) { }

  public triggerFileSelect(): void {
    this.fileInput.nativeElement.click();
  }

  public onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) {
      return;
    }

    this.loading = true;
    this.orcamentoImportService.preview(file, this.orcamentoConfigFk).subscribe(
      response => {
        this.loading = false;
        input.value = '';
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }

        this.rows = (response.rows ?? []).map(r => ({
          ...r,
          // New luôn ngầm Insert; Exists mặc định Skip (an toàn — theo CLAUDE.md §6);
          // Error không có action.
          action: r.status === 'New' ? 'Insert' : (r.status === 'Exists' ? 'Skip' : undefined)
        }));
        this.totalNew = response.totalNew;
        this.totalExists = response.totalExists;
        this.totalErrors = response.totalErrors;
        this.showPreview = true;
      },
      err => {
        this.loading = false;
        input.value = '';
        const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('orcamentoImportPreview.errGeneric');
        this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
      }
    );
  }

  public setActionForAllExists(action: 'Overwrite' | 'Skip'): void {
    this.rows.filter(r => r.status === 'Exists').forEach(r => r.action = action);
  }

  public get actionableCount(): number {
    return this.rows.filter(r => r.status === 'New' || (r.status === 'Exists' && r.action !== 'Skip')).length;
  }

  public confirmImport(): void {
    const payloadRows = this.rows
      .filter(r => r.status === 'New' || r.status === 'Exists')
      .map(r => ({
        rowNum: r.rowNum,
        atividadeFk: r.atividadeFk!,
        economicClassificationFk: r.economicClassificationFk!,
        organizationFk: r.organizationFk!,
        valor: r.valor,
        action: r.action!,
        existingOrcamentoLinhaId: r.existingOrcamentoLinhaId
      }));

    this.saving = true;
    this.orcamentoImportService.confirm({ orcamentoConfigFk: this.orcamentoConfigFk, rows: payloadRows }).subscribe(
      response => {
        this.saving = false;
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.snackBar.open(
          this.translate.instant('orcamentoImportPreview.confirmResult', { success: response.success, failed: response.failed }),
          this.translate.instant('general.close'), { duration: 4000 });
        this.closePreview();
        this.imported.emit();
      },
      err => {
        this.saving = false;
        const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('orcamentoImportPreview.errGeneric');
        this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
      }
    );
  }

  public closePreview(): void {
    this.showPreview = false;
    this.rows = [];
  }
}
