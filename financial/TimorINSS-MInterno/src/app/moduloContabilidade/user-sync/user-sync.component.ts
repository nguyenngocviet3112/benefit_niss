import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { PermissionService } from '../../services/permission.service';
import { UserSyncListItem, UserSyncService } from '../../services/user-sync.service';
import { SelectDescription } from '../../models/utils';

// "Đồng bộ User từ hệ thống cũ" — công cụ onboarding cho user Internal cũ
// vào mode mới (tạo UserModeAccess + UserProfile, KHÔNG tự gán quyền —
// admin gán tay sau ở "Quản lý User & Phân quyền", vì 2 hệ quyền khác ngữ
// nghĩa nhau). User External chỉ hiển thị để biết, không thao tác gì —
// họ vẫn dùng cổng doanh nghiệp (port 4200) y nguyên. Danh sách có trạng
// thái, chạy lại được bất kỳ lúc nào (không phải batch 1 lần).
@Component({
  selector: 'app-user-sync',
  templateUrl: './user-sync.component.html',
  styleUrls: ['./user-sync.component.css']
})
export class UserSyncComponent implements OnInit {

  public activeTab: 'internal' | 'external' = 'internal';
  public loading = false;

  public internalUsers: UserSyncListItem[] = [];
  public externalUsers: UserSyncListItem[] = [];
  public departamentos: SelectDescription[] = [];

  public syncTarget: UserSyncListItem | null = null;
  public formNome = '';
  public formEmail = '';
  public formDepartamentoFk: number | null = null;

  constructor(
    private userSyncService: UserSyncService,
    private permissionService: PermissionService,
    private snackBar: MatSnackBar,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.loadInternal();
    this.permissionService.getCatalog().subscribe(response => {
      this.departamentos = response.departamentos ?? [];
    });
  }

  public selectTab(tab: 'internal' | 'external'): void {
    this.activeTab = tab;
    if (tab === 'internal' && this.internalUsers.length === 0) {
      this.loadInternal();
    }
    if (tab === 'external' && this.externalUsers.length === 0) {
      this.loadExternal();
    }
  }

  public loadInternal(): void {
    this.loading = true;
    this.userSyncService.getInternalList().subscribe(
      response => {
        this.internalUsers = response.items ?? [];
        this.loading = false;
      },
      err => {
        this.loading = false;
        this.showError(err);
      }
    );
  }

  public loadExternal(): void {
    this.loading = true;
    this.userSyncService.getExternalList().subscribe(
      response => {
        this.externalUsers = response.items ?? [];
        this.loading = false;
      },
      err => {
        this.loading = false;
        this.showError(err);
      }
    );
  }

  public openSyncForm(item: UserSyncListItem): void {
    this.syncTarget = item;
    this.formNome = item.nome || '';
    this.formEmail = item.email || '';
    this.formDepartamentoFk = null;
  }

  public cancelSyncForm(): void {
    this.syncTarget = null;
  }

  public confirmSync(): void {
    if (!this.syncTarget || !this.formNome.trim()) {
      this.snackBar.open(this.translate.instant('userSync.errMissingNome'), this.translate.instant('general.close'), { duration: 3000 });
      return;
    }

    this.userSyncService.syncInternal({
      id: this.syncTarget.id,
      nome: this.formNome,
      email: this.formEmail,
      departamentoFk: this.formDepartamentoFk
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.snackBar.open(this.translate.instant('userSync.syncedSuccess'), this.translate.instant('general.close'), { duration: 3000 });
        this.syncTarget = null;
        this.loadInternal();
      },
      err => this.showError(err)
    );
  }

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('userSync.errGeneric');
    this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
  }
}
