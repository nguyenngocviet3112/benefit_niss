import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SelectDescription } from '../../models/utils';
import { DepartamentoConfigDataContract } from '../../response-models/departamento-config-response';
import { DepartamentoConfigService } from '../../services/departamento-config.service';
import { InstitutionService } from '../../services/institution.service';

@Component({
  selector: 'app-departamento-config',
  templateUrl: './departamento-config.component.html',
  styleUrls: ['./departamento-config.component.css']
})
export class DepartamentoConfigComponent implements OnInit {

  public loading = false;
  public items: DepartamentoConfigDataContract[] = [];
  public institutions: SelectDescription[] = [];

  public showForm = false;
  public editingId = 0;
  public formNome = '';
  public formInstitutionId: number | null = null;

  constructor(
    private departamentoConfigService: DepartamentoConfigService,
    private institutionService: InstitutionService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.load();
    this.institutionService.getAllInstitutionsAtivo().subscribe(
      response => this.institutions = response.selects ?? [],
      err => this.showError(err)
    );
  }

  public load(): void {
    this.loading = true;
    this.departamentoConfigService.getAll().subscribe(
      response => {
        this.items = (response.items ?? []).filter(i => i.indActivo);
        this.loading = false;
      },
      err => {
        this.loading = false;
        this.showError(err);
      }
    );
  }

  public openCreateForm(): void {
    this.editingId = 0;
    this.formNome = '';
    this.formInstitutionId = null;
    this.showForm = true;
  }

  public openEditForm(item: DepartamentoConfigDataContract): void {
    this.editingId = item.id;
    this.formNome = item.nome;
    this.formInstitutionId = item.institutionId ?? null;
    this.showForm = true;
  }

  public cancelForm(): void {
    this.showForm = false;
  }

  public saveDepartamento(): void {
    if (!this.formNome.trim()) {
      this.snackBar.open('Vui lòng nhập tên phòng ban.', 'Đóng', { duration: 3000 });
      return;
    }

    this.departamentoConfigService.save({
      id: this.editingId,
      nome: this.formNome,
      institutionId: this.formInstitutionId ?? undefined
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, 'Đóng', { duration: 4000 });
          return;
        }
        this.showForm = false;
        this.snackBar.open('Đã lưu phòng ban.', 'Đóng', { duration: 3000 });
        this.load();
      },
      err => this.showError(err)
    );
  }

  public deactivate(item: DepartamentoConfigDataContract): void {
    if (!confirm(`Vô hiệu hóa phòng ban "${item.nome}"?`)) { return; }
    this.departamentoConfigService.deactivate({ id: item.id }).subscribe(
      () => this.load(),
      err => this.showError(err)
    );
  }

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? 'Có lỗi xảy ra.';
    this.snackBar.open(message, 'Đóng', { duration: 4000 });
  }
}
