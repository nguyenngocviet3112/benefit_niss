import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

export interface ConfirmDialogData {
  message: string;
}

// Thay thế cho window.confirm() gốc trình duyệt — Chrome (và các trình duyệt
// khác) tự động chặn im lặng mọi popup confirm()/alert() sau vài lần gọi liên
// tiếp trên cùng 1 trang (trả về false ngay, không hiện gì, không báo lỗi).
// Dùng MatDialog thay thế để không bao giờ bị chặn kiểu đó (2026-07-13, user
// report: nút "Xác nhận" ở Guia Conciliação chỉ có tác dụng lần đầu, sau đó
// im lặng vô tác dụng — xác nhận nguyên nhân là window.confirm() bị trình
// duyệt chặn).
@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './confirm-dialog.component.html',
  styleUrls: ['./confirm-dialog.component.css']
})
export class ConfirmDialogComponent {

  constructor(
    public dialogRef: MatDialogRef<ConfirmDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ConfirmDialogData
  ) { }

  public onCancel(): void {
    this.dialogRef.close(false);
  }

  public onConfirm(): void {
    this.dialogRef.close(true);
  }
}
