import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PageEvent } from '@angular/material/paginator';
import { TranslateService } from '@ngx-translate/core';
import { ObligationService } from '../../services/obligation.service';
import { ObligationDataContract } from '../../response-models/obligation-response';

// Registo Obrigações — sổ đăng ký toàn bộ Obrigação trong 1 năm ngân sách (mọi
// trạng thái), đọc thẳng từ GetByAno đã dùng cho màn nhập liệu — không cần API
// mới. Obrigação không gắn trực tiếp Atividade/Organization (gom từ nhiều
// Compromisso qua items) nên lọc theo Categoria Beneficiário thay vì Organization.
// CHỈ XEM.
@Component({
  selector: 'app-registo-obrigacoes',
  templateUrl: './registo-obrigacoes.component.html',
  styleUrls: ['./registo-obrigacoes.component.css']
})
export class RegistoObrigacoesComponent implements OnInit {

  public loading = false;
  public items: ObligationDataContract[] = [];
  public pagedItems: ObligationDataContract[] = [];
  public pageIndex = 0;
  public pageSize = 20;

  public years: number[] = [];
  public selectedYear = new Date().getFullYear();

  public categoriaOptions: { value: string; label: string }[] = [
    { value: '', label: 'registoObrigacoes.categoriaTodos' },
    { value: 'FORNECEDOR', label: 'registoObrigacoes.categoriaFornecedor' },
    { value: 'BENEFICIARIO', label: 'registoObrigacoes.categoriaBeneficiario' },
    { value: 'CONTRIBUINTE_EE', label: 'registoObrigacoes.categoriaContribuinteEe' },
    { value: 'PESSOAL', label: 'registoObrigacoes.categoriaPessoal' },
    { value: 'OUTRO', label: 'registoObrigacoes.categoriaOutro' }
  ];
  public selectedCategoria = '';

  public estadoOptions: { value: string; label: string }[] = [
    { value: '', label: 'registoObrigacoes.estadoTodos' },
    { value: 'DRAFT', label: 'registoObrigacoes.estadoDraft' },
    { value: 'PENDING_APPROVAL', label: 'registoObrigacoes.estadoPendingApproval' },
    { value: 'APPROVED', label: 'registoObrigacoes.estadoApproved' }
  ];
  public selectedEstado = '';

  constructor(
    private obligationService: ObligationService,
    private snackBar: MatSnackBar,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    const current = new Date().getFullYear();
    this.years = [current - 1, current, current + 1];
    this.load();
  }

  public onCategoriaChange(): void {
    this.applyFilters();
  }

  public onEstadoChange(): void {
    this.applyFilters();
  }

  public load(): void {
    this.loading = true;
    this.obligationService.getByAno(this.selectedYear).subscribe(
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
        const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('registoObrigacoes.errGeneric');
        this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
      }
    );
  }

  private allItems: ObligationDataContract[] = [];

  private applyFilters(): void {
    this.items = this.allItems.filter(i =>
      (!this.selectedCategoria || i.beneficiarioCategoria === this.selectedCategoria) &&
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

  public compromissosLabel(row: ObligationDataContract): string {
    return (row.items ?? []).map(i => `${i.compromissoDespesaNumero}/${i.compromissoDespesaMes}`).join(', ');
  }

  public get totalValorObrigacao(): number {
    return this.items.reduce((sum, i) => sum + (i.valorObrigacao ?? 0), 0);
  }
}
