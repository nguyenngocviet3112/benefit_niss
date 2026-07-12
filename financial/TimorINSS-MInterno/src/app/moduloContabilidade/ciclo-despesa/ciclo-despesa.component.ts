import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PageEvent } from '@angular/material/paginator';
import { TranslateService } from '@ngx-translate/core';
import { CicloDespesaService } from '../../services/ciclo-despesa.service';
import { InstitutionService } from '../../services/institution.service';
import { CicloDespesaRow } from '../../response-models/ciclo-despesa-response';
import { SelectDescription } from '../../models/utils';

// Ciclo da Despesa — 1 dòng/AD, theo dõi toàn bộ chu trình chấp hành chi tiêu
// (Cabimento -> Compromisso -> Obrigação -> Pagamento) kèm saldo từng bước, đúng
// cấu trúc sheet "Ciclo_Despesa" gốc (INSS_2026_janeiro _original.xlsx). CHỈ XEM.
@Component({
  selector: 'app-ciclo-despesa',
  templateUrl: './ciclo-despesa.component.html',
  styleUrls: ['./ciclo-despesa.component.css']
})
export class CicloDespesaComponent implements OnInit {

  public loading = false;
  public items: CicloDespesaRow[] = [];
  public pagedItems: CicloDespesaRow[] = [];
  public pageIndex = 0;
  public pageSize = 20;

  public institutions: SelectDescription[] = [];
  public years: number[] = [];

  public selectedYear = new Date().getFullYear();
  public selectedInstitution?: number;

  constructor(
    private cicloDespesaService: CicloDespesaService,
    private institutionService: InstitutionService,
    private snackBar: MatSnackBar,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    const current = new Date().getFullYear();
    this.years = [current - 1, current, current + 1];

    this.institutionService.getAllInstitutionsAtivo().subscribe(
      response => this.institutions = response.selects ?? [],
      () => { /* Organization filter still works with just "Both" if this fails */ }
    );

    this.load();
  }

  public load(): void {
    this.loading = true;
    this.cicloDespesaService.getByAno(this.selectedYear, this.selectedInstitution).subscribe(
      response => {
        this.loading = false;
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.items = response.items ?? [];
        this.pageIndex = 0;
        this.updatePagedItems();
      },
      err => {
        this.loading = false;
        const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('cicloDespesa.errGeneric');
        this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
      }
    );
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

  public get totalCabimentos(): number {
    return this.items.reduce((sum, i) => sum + i.cabimentos, 0);
  }

  public get totalCompromissos(): number {
    return this.items.reduce((sum, i) => sum + i.compromissos, 0);
  }

  public get totalObrigacoes(): number {
    return this.items.reduce((sum, i) => sum + i.obrigacoes, 0);
  }

  public get totalPagamentos(): number {
    return this.items.reduce((sum, i) => sum + i.pagamentos, 0);
  }
}
