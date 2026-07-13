import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PageEvent } from '@angular/material/paginator';
import { TranslateService } from '@ngx-translate/core';
import { CompromissoDespesaService } from '../../services/compromisso-despesa.service';
import { InstitutionService } from '../../services/institution.service';
import { CompromissoDespesaDataContract } from '../../response-models/compromisso-despesa-response';
import { SelectDescription } from '../../models/utils';

// Registo Compromissos — sổ đăng ký toàn bộ Compromisso Despesa trong 1 năm
// ngân sách (mọi trạng thái), đọc thẳng từ GetByAno đã dùng cho màn nhập liệu
// — không cần API mới. CHỈ XEM.
@Component({
  selector: 'app-registo-compromissos',
  templateUrl: './registo-compromissos.component.html',
  styleUrls: ['./registo-compromissos.component.css']
})
export class RegistoCompromissosComponent implements OnInit {

  public loading = false;
  public items: CompromissoDespesaDataContract[] = [];
  public pagedItems: CompromissoDespesaDataContract[] = [];
  public pageIndex = 0;
  public pageSize = 20;

  public institutions: SelectDescription[] = [];
  public filteredOrganizations: SelectDescription[] = [];
  public organizationSearch = '';
  public selectedOrganizationNome?: string;
  public years: number[] = [];

  public estadoOptions: { value: string; label: string }[] = [
    { value: '', label: 'registoCompromissos.estadoTodos' },
    { value: 'DRAFT', label: 'registoCompromissos.estadoDraft' },
    { value: 'PENDING_REVIEW', label: 'registoCompromissos.estadoPendingReview' },
    { value: 'PENDING_APPROVAL', label: 'registoCompromissos.estadoPendingApproval' },
    { value: 'APPROVED', label: 'registoCompromissos.estadoApproved' }
  ];
  public selectedEstado = '';

  public selectedYear = new Date().getFullYear();

  constructor(
    private compromissoDespesaService: CompromissoDespesaService,
    private institutionService: InstitutionService,
    private snackBar: MatSnackBar,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    const current = new Date().getFullYear();
    this.years = [current - 1, current, current + 1];
    this.organizationSearch = this.translate.instant('registoCompromissos.bothOption');

    this.institutionService.getAllInstitutionsAtivo().subscribe(
      response => {
        this.institutions = response.selects ?? [];
        this.filteredOrganizations = this.institutions;
      },
      () => { /* Organization filter still works with just "Tất cả" if this fails */ }
    );

    this.load();
  }

  private normalize(value: string): string {
    return (value || '').toLowerCase();
  }

  public onOrganizationSearchChange(): void {
    const term = this.normalize(this.organizationSearch);
    this.filteredOrganizations = this.institutions.filter(o => this.normalize(o.nome).includes(term));
  }

  public selectOrganization(o?: SelectDescription): void {
    this.selectedOrganizationNome = o?.nome;
    this.organizationSearch = o ? o.nome : this.translate.instant('registoCompromissos.bothOption');
    this.applyFilters();
  }

  public onEstadoChange(): void {
    this.applyFilters();
  }

  public load(): void {
    this.loading = true;
    this.compromissoDespesaService.getByAno(this.selectedYear).subscribe(
      response => {
        this.loading = false;
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.allItems = response.items ?? [];
        this.applyFilters();
      },
      err => {
        this.loading = false;
        const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('registoCompromissos.errGeneric');
        this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
      }
    );
  }

  private allItems: CompromissoDespesaDataContract[] = [];

  private applyFilters(): void {
    this.items = this.allItems.filter(i =>
      (!this.selectedOrganizationNome || i.organizationNome === this.selectedOrganizationNome) &&
      (!this.selectedEstado || i.estado === this.selectedEstado)
    );
    this.pageIndex = 0;
    this.updatePagedItems();
  }

  public onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.updatePagedItems();
  }

  private updatePagedItems(): void {
    const start = this.pageIndex * this.pageSize;
    this.pagedItems = this.items.slice(start, start + this.pageSize);
  }

  public get totalValorCompromissoAno(): number {
    return this.items.reduce((sum, i) => sum + (i.valorCompromissoAno ?? 0), 0);
  }

  public get totalValorObrigado(): number {
    return this.items.reduce((sum, i) => sum + (i.valorObrigado ?? 0), 0);
  }

  public get totalSaldoDisponivel(): number {
    return this.items.reduce((sum, i) => sum + (i.saldoDisponivel ?? 0), 0);
  }
}
