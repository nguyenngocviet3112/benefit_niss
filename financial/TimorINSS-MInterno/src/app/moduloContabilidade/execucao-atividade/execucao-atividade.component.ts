import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PageEvent } from '@angular/material/paginator';
import { TranslateService } from '@ngx-translate/core';
import { CicloDespesaService } from '../../services/ciclo-despesa.service';
import { InstitutionService } from '../../services/institution.service';
import { CicloDespesaRow } from '../../response-models/ciclo-despesa-response';
import { SelectDescription } from '../../models/utils';
import * as XLSX from 'xlsx';

export interface ExecucaoGroupRow {
  codigo: string;
  designacao: string;
  regimeCodigo: string;
  regimeDesignacao: string;
  numeroAds: number;
  cabimentos: number;
  compromissos: number;
  saldo1: number;
  obrigacoes: number;
  saldo2: number;
  pagamentos: number;
  saldo3: number;
}

// Execução por Atividade / Programa / Regime — gộp 22 sheet gốc
// (Programa/SubPrograma/Atividade/CE_Regime*) thành 1 report có filter, thay
// vì 22 màn riêng biệt (xem ghi chú tại contabilidade-shell.component.ts).
// 100% frontend: gom nhóm (theo Atividade hoặc theo Regime/Programa) từ đúng
// dữ liệu chấp hành chi tiêu mà Ciclo da Despesa đã tính sẵn (GetByAno) —
// không cần API mới. CHỈ XEM.
@Component({
  selector: 'app-execucao-atividade',
  templateUrl: './execucao-atividade.component.html',
  styleUrls: ['./execucao-atividade.component.css']
})
export class ExecucaoAtividadeComponent implements OnInit {

  public loading = false;
  private rawRows: CicloDespesaRow[] = [];

  public groupedItems: ExecucaoGroupRow[] = [];
  public pagedItems: ExecucaoGroupRow[] = [];
  public pageIndex = 0;
  public pageSize = 20;

  public institutions: SelectDescription[] = [];
  public filteredOrganizations: SelectDescription[] = [];
  public organizationSearch = '';
  public selectedInstitution?: number;
  public years: number[] = [];
  public selectedYear = new Date().getFullYear();

  public groupByOptions: { value: 'atividade' | 'regime'; label: string }[] = [
    { value: 'atividade', label: 'execucaoAtividade.groupByAtividade' },
    { value: 'regime', label: 'execucaoAtividade.groupByRegime' }
  ];
  public groupBy: 'atividade' | 'regime' = 'atividade';

  constructor(
    private cicloDespesaService: CicloDespesaService,
    private institutionService: InstitutionService,
    private snackBar: MatSnackBar,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    const current = new Date().getFullYear();
    this.years = [current - 1, current, current + 1];
    this.organizationSearch = this.translate.instant('execucaoAtividade.bothOption');

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
    this.selectedInstitution = o?.id;
    this.organizationSearch = o ? o.nome : this.translate.instant('execucaoAtividade.bothOption');
    this.load();
  }

  public onGroupByChange(): void {
    this.applyGrouping();
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
        this.rawRows = response.items ?? [];
        this.applyGrouping();
      },
      err => {
        this.loading = false;
        const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('execucaoAtividade.errGeneric');
        this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
      }
    );
  }

  private applyGrouping(): void {
    const byKey = new Map<string, ExecucaoGroupRow>();

    for (const row of this.rawRows) {
      const key = this.groupBy === 'atividade' ? row.atividadeCodigo : row.regimeCodigo;
      let group = byKey.get(key);
      if (!group) {
        group = {
          codigo: this.groupBy === 'atividade' ? row.atividadeCodigo : row.regimeCodigo,
          designacao: this.groupBy === 'atividade' ? row.atividadeDesignacao : row.regimeDesignacao,
          regimeCodigo: row.regimeCodigo,
          regimeDesignacao: row.regimeDesignacao,
          numeroAds: 0,
          cabimentos: 0, compromissos: 0, saldo1: 0,
          obrigacoes: 0, saldo2: 0,
          pagamentos: 0, saldo3: 0
        };
        byKey.set(key, group);
      }
      group.numeroAds += 1;
      group.cabimentos += row.cabimentos;
      group.compromissos += row.compromissos;
      group.saldo1 += row.saldo1;
      group.obrigacoes += row.obrigacoes;
      group.saldo2 += row.saldo2;
      group.pagamentos += row.pagamentos;
      group.saldo3 += row.saldo3;
    }

    this.groupedItems = Array.from(byKey.values())
      .sort((a, b) => (a.regimeCodigo + a.codigo).localeCompare(b.regimeCodigo + b.codigo));
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
    this.pagedItems = this.groupedItems.slice(start, start + this.pageSize);
  }

  public get totalCabimentos(): number {
    return this.groupedItems.reduce((sum, i) => sum + i.cabimentos, 0);
  }

  public get totalCompromissos(): number {
    return this.groupedItems.reduce((sum, i) => sum + i.compromissos, 0);
  }

  public get totalObrigacoes(): number {
    return this.groupedItems.reduce((sum, i) => sum + i.obrigacoes, 0);
  }

  public get totalPagamentos(): number {
    return this.groupedItems.reduce((sum, i) => sum + i.pagamentos, 0);
  }

  // Xuất Excel 100% phía client — dữ liệu đã gộp sẵn trong browser (không có
  // API riêng cho màn này, xem ghi chú đầu file), dùng thư viện "xlsx" đã có
  // sẵn trong package.json (đã dùng ở vài màn mode cũ).
  public exportarExcel(): void {
    const isRegime = this.groupBy === 'regime';
    const header = isRegime
      ? ['Regime', 'N.º AD', 'Cabimentos', 'Compromissos', 'Saldo 1', 'Obrigações', 'Saldo 2', 'Pagamentos', 'Saldo 3']
      : ['Regime', 'Atividade', 'N.º AD', 'Cabimentos', 'Compromissos', 'Saldo 1', 'Obrigações', 'Saldo 2', 'Pagamentos', 'Saldo 3'];

    const rows = this.groupedItems.map(r => isRegime
      ? [`${r.codigo} ${r.designacao}`, r.numeroAds, r.cabimentos, r.compromissos, r.saldo1, r.obrigacoes, r.saldo2, r.pagamentos, r.saldo3]
      : [`${r.regimeCodigo} ${r.regimeDesignacao}`, `${r.codigo} ${r.designacao}`, r.numeroAds, r.cabimentos, r.compromissos, r.saldo1, r.obrigacoes, r.saldo2, r.pagamentos, r.saldo3]);

    const ws = XLSX.utils.aoa_to_sheet([header, ...rows]);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Execução');
    XLSX.writeFile(wb, `ExecucaoAtividade_${this.selectedYear}.xlsx`);
  }
}
