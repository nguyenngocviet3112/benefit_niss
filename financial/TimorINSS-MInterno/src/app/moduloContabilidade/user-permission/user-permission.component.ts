import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { forkJoin } from 'rxjs';
import {
  PermissionGroupModel,
  PermissionPresetModel,
  PermissionService,
  UserPermissionListItem
} from '../../services/permission.service';
import { SelectDescription } from '../../models/utils';

// "Quản lý User & Phân quyền" — single combined screen (list + create/edit
// form with permission/preset assignment inline, saved together in one
// action) per the locked UX decision: creating a user and assigning its
// permissions must not be split into two separate steps/screens.
@Component({
  selector: 'app-user-permission',
  templateUrl: './user-permission.component.html',
  styleUrls: ['./user-permission.component.css']
})
export class UserPermissionComponent implements OnInit {

  public users: UserPermissionListItem[] = [];
  public groups: PermissionGroupModel[] = [];
  public presets: PermissionPresetModel[] = [];
  public departamentos: SelectDescription[] = [];
  public loading = false;

  public showForm = false;
  public editingId: number | null = null;
  public formUsername = '';
  public formPassword = '';
  public formIndActivo = true;
  public formNome = '';
  public formEmail = '';
  public formDepartamentoFk: number | null = null;
  public checkedTokens: string[] = [];
  public checkedPresetIds: number[] = [];

  constructor(
    private permissionService: PermissionService,
    private snackBar: MatSnackBar,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.loadAll();
  }

  public loadAll(): void {
    this.loading = true;
    forkJoin([this.permissionService.getCatalog(), this.permissionService.getUsers()]).subscribe(
      ([catalog, users]) => {
        this.groups = catalog.groups ?? [];
        this.presets = catalog.presets ?? [];
        this.departamentos = catalog.departamentos ?? [];
        this.users = users.items ?? [];
        this.loading = false;
      },
      err => {
        this.loading = false;
        this.showError(err);
      }
    );
  }

  public openAddForm(): void {
    this.editingId = null;
    this.formUsername = '';
    this.formPassword = '';
    this.formIndActivo = true;
    this.formNome = '';
    this.formEmail = '';
    this.formDepartamentoFk = null;
    this.checkedTokens = [];
    this.checkedPresetIds = [];
    this.showForm = true;
  }

  public openEditForm(user: UserPermissionListItem): void {
    this.editingId = user.id;
    this.formUsername = user.username;
    this.formPassword = '';
    this.formIndActivo = user.indActivo;
    this.formNome = user.nome ?? '';
    this.formEmail = user.email ?? '';
    this.formDepartamentoFk = user.departamentoFk ?? null;
    this.checkedTokens = [...user.tokens];
    this.checkedPresetIds = this.presets
      .filter(p => user.presetCodigos.includes(p.codigo))
      .map(p => p.id);
    this.showForm = true;
  }

  public cancelForm(): void {
    this.showForm = false;
  }

  public isPresetChecked(preset: PermissionPresetModel): boolean {
    return this.checkedPresetIds.includes(preset.id);
  }

  public togglePreset(preset: PermissionPresetModel): void {
    if (this.isPresetChecked(preset)) {
      this.checkedPresetIds = this.checkedPresetIds.filter(id => id !== preset.id);
      // Simple v1 behaviour: unchecking a preset drops its tokens even if
      // another still-checked preset also grants them — the grid below lets
      // the admin re-check any individual token by hand if needed.
      this.checkedTokens = this.checkedTokens.filter(t => !preset.tokens.includes(t));
    } else {
      this.checkedPresetIds = [...this.checkedPresetIds, preset.id];
      preset.tokens.forEach(t => {
        if (!this.checkedTokens.includes(t)) {
          this.checkedTokens.push(t);
        }
      });
    }
  }

  public isTokenChecked(token: string): boolean {
    return this.checkedTokens.includes(token);
  }

  public toggleToken(token: string): void {
    if (this.checkedTokens.includes(token)) {
      this.checkedTokens = this.checkedTokens.filter(t => t !== token);
    } else {
      this.checkedTokens = [...this.checkedTokens, token];
    }
  }

  public save(): void {
    if (!this.formUsername.trim()) {
      this.snackBar.open(this.translate.instant('userPermission.errMissingUsername'), this.translate.instant('general.close'), { duration: 3000 });
      return;
    }
    if (!this.editingId && !this.formPassword.trim()) {
      this.snackBar.open(this.translate.instant('userPermission.errMissingPassword'), this.translate.instant('general.close'), { duration: 3000 });
      return;
    }

    this.permissionService.saveUser({
      id: this.editingId ?? 0,
      username: this.formUsername.trim(),
      password: this.formPassword,
      indActivo: this.formIndActivo,
      nome: this.formNome.trim(),
      email: this.formEmail.trim(),
      departamentoFk: this.formDepartamentoFk,
      presetIds: this.checkedPresetIds,
      tokens: this.checkedTokens
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.showForm = false;
        this.loadAll();
      },
      err => this.showError(err)
    );
  }

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('userPermission.errGeneric');
    this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
  }
}
