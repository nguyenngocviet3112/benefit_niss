import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ConfirmDialogComponent } from '../moduloContabilidade/confirm-dialog/confirm-dialog.component';

// Wrapper quanh MatDialog cho mọi màn mode mới cần hỏi xác nhận trước khi
// Submit/Delete/Deactivate/Reactivate — thay thế window.confirm() nguyên bản
// trình duyệt, vốn bị Chrome (và nhiều trình duyệt khác) tự động chặn im
// lặng sau vài lần gọi liên tiếp trên cùng 1 trang (luôn trả về false ngay,
// không hiện popup, không báo lỗi — khiến nút bấm trông như vô hiệu hoàn
// toàn). 2026-07-13, user report trên màn Guia Conciliação, xác nhận áp
// dụng cho toàn bộ ~22 chỗ dùng confirm() khác trong moduloContabilidade.
@Injectable({
  providedIn: 'root'
})
export class ConfirmDialogService {

  constructor(private dialog: MatDialog) { }

  public confirm(message: string): Observable<boolean> {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '420px',
      data: { message }
    });
    return dialogRef.afterClosed().pipe(map(result => !!result));
  }
}
