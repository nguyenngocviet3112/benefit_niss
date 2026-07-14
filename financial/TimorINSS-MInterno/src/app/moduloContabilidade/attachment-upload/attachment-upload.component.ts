import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { AttachmentService } from '../../services/attachment.service';
import { AttachmentConfigService } from '../../services/attachment-config.service';
import { ConfirmDialogService } from '../../services/confirm-dialog.service';
import { AttachmentItem } from '../../response-models/attachment-response';
import { base64ArrayBuffer } from '../../utils';

// Widget đính kèm file dùng chung cho cả chu trình chi tiêu (AD/Cabimento/
// Compromisso/Obrigação/Pagamento) — người submit chọn file (PDF/PNG/Excel),
// xem lại được sau đó. Chỉ nhận entityType/entityId của bản ghi ĐÃ tồn tại
// (đã có Id thật trong DB) — không dùng được cho form tạo mới chưa lưu.
const ALLOWED_CONTENT_TYPES = [
  'application/pdf',
  'image/png',
  'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
  'application/vnd.ms-excel'
];

@Component({
  selector: 'app-attachment-upload',
  templateUrl: './attachment-upload.component.html',
  styleUrls: ['./attachment-upload.component.css']
})
export class AttachmentUploadComponent implements OnInit {

  @Input() entityType!: string;
  @Input() entityId!: number;
  // Vì template ref var (#var) KHÔNG "xuyên" qua ranh giới *ngIf khác nhau
  // (kể cả cùng điều kiện) — parent cần biết "đã có file chưa" để khóa nút
  // Submit thì phải nhận qua Output này, không thể tham chiếu thẳng instance
  // widget từ một *ngIf khác trong template cha.
  @Output() itemsLoaded = new EventEmitter<AttachmentItem[]>();

  public items: AttachmentItem[] = [];
  public loading = false;
  public uploading = false;
  public fileControl = new FormControl(null);
  public accept = '.pdf,.png,.xlsx,.xls';
  private maxSizeBytes = 10 * 1024 * 1024;

  constructor(
    private attachmentService: AttachmentService,
    private attachmentConfigService: AttachmentConfigService,
    private snackBar: MatSnackBar,
    private translate: TranslateService,
    private confirmDialog: ConfirmDialogService
  ) { }

  ngOnInit(): void {
    this.attachmentConfigService.getConfig().subscribe(response => {
      if (response.item) {
        this.maxSizeBytes = response.item.maxFileSizeMb * 1024 * 1024;
      }
    });

    this.load();

    this.fileControl.valueChanges.subscribe((file: File | null) => {
      if (file) {
        this.handleFile(file);
      }
    });
  }

  public load(): void {
    this.loading = true;
    this.attachmentService.getByEntity(this.entityType, this.entityId).subscribe(
      response => {
        this.loading = false;
        this.items = response.items ?? [];
        this.itemsLoaded.emit(this.items);
      },
      () => { this.loading = false; }
    );
  }

  public view(item: AttachmentItem): void {
    this.attachmentService.downloadBlob(item.id).subscribe(
      blob => {
        const url = window.URL.createObjectURL(new Blob([blob], { type: item.contentType }));
        window.open(url, '_blank');
      },
      () => this.showError()
    );
  }

  // Cho phép xoá file lỡ upload sai (2026-07-13, user yêu cầu). Widget này chỉ
  // hiện khi entity gốc còn DRAFT (xem *ngIf ở nơi gọi <app-attachment-upload>),
  // nên nút này tự động không xuất hiện sau khi entity gốc đã duyệt.
  public delete(item: AttachmentItem): void {
    this.confirmDialog.confirm(this.translate.instant('attachmentUpload.confirmDelete', { fileName: item.fileName })).subscribe(confirmed => {
      if (!confirmed) { return; }
      this.attachmentService.delete(item.id).subscribe(
        response => {
          if (response.errors && response.errors.length > 0) {
            this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
            return;
          }
          this.snackBar.open(this.translate.instant('attachmentUpload.deleteSuccess'), this.translate.instant('general.close'), { duration: 3000 });
          this.load();
        },
        () => this.showError()
      );
    });
  }

  public formatSize(bytes: number): string {
    if (bytes < 1024) { return `${bytes} B`; }
    if (bytes < 1024 * 1024) { return `${(bytes / 1024).toFixed(1)} KB`; }
    return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
  }

  private handleFile(file: File): void {
    if (!ALLOWED_CONTENT_TYPES.includes(file.type)) {
      this.snackBar.open(this.translate.instant('attachmentUpload.errType'), this.translate.instant('general.close'), { duration: 4000 });
      this.fileControl.setValue(null);
      return;
    }
    if (file.size > this.maxSizeBytes) {
      const maxMb = Math.round(this.maxSizeBytes / (1024 * 1024));
      this.snackBar.open(this.translate.instant('attachmentUpload.errSize', { maxMb }), this.translate.instant('general.close'), { duration: 4000 });
      this.fileControl.setValue(null);
      return;
    }

    const reader = new FileReader();
    reader.readAsArrayBuffer(file);
    reader.onloadend = (evt) => {
      if (evt.target?.readyState === FileReader.DONE && evt.target.result instanceof ArrayBuffer) {
        this.uploadFile(file.name, file.type, base64ArrayBuffer(evt.target.result));
      }
    };
  }

  private uploadFile(fileName: string, contentType: string, fileContentBase64: string): void {
    this.uploading = true;
    this.attachmentService.upload({
      entityType: this.entityType,
      entityId: this.entityId,
      fileName,
      contentType,
      fileContentBase64
    }).subscribe(
      response => {
        this.uploading = false;
        this.fileControl.setValue(null);
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.snackBar.open(this.translate.instant('attachmentUpload.uploadSuccess'), this.translate.instant('general.close'), { duration: 3000 });
        this.load();
      },
      err => {
        this.uploading = false;
        this.fileControl.setValue(null);
        const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('attachmentUpload.errGeneric');
        this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
      }
    );
  }

  private showError(): void {
    this.snackBar.open(this.translate.instant('attachmentUpload.errGeneric'), this.translate.instant('general.close'), { duration: 4000 });
  }
}
