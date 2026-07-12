import { Component, EventEmitter, Input, Output, ViewChild, ElementRef } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { MasterDataImportService } from '../../services/master-data-import.service';
import { ImportMasterDataTreeResponse } from '../../response-models/master-data-import-response';

@Component({
  selector: 'app-master-data-import-button',
  templateUrl: './master-data-import-button.component.html',
  styleUrls: ['./master-data-import-button.component.css']
})
export class MasterDataImportButtonComponent {

  // entityPath: đoạn route backend, vd 'programactivity' | 'functionalclassification' | 'economicclassification'.
  @Input() entityPath = '';
  @Input() orcamentoConfigFk = 1;
  @Output() imported = new EventEmitter<void>();

  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  public importing = false;
  public lastResult: ImportMasterDataTreeResponse | null = null;
  public showResult = false;

  constructor(
    private masterDataImportService: MasterDataImportService,
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

    this.importing = true;
    this.masterDataImportService.import(this.entityPath, file, this.orcamentoConfigFk).subscribe(
      response => {
        this.importing = false;
        input.value = '';
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.lastResult = response;
        this.showResult = true;
        this.imported.emit();
      },
      err => {
        this.importing = false;
        input.value = '';
        const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('masterDataImportButton.errGeneric');
        this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
      }
    );
  }

  public closeResult(): void {
    this.showResult = false;
    this.lastResult = null;
  }
}
