import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PageEvent } from '@angular/material/paginator';
import { TranslateService } from '@ngx-translate/core';
import { CodigoContaOpeningBalanceService } from '../../services/codigo-conta-opening-balance.service';
import { CodigoContaOpeningBalanceDataContract } from '../../response-models/codigo-conta-opening-balance-response';
import { ReceitaPacService } from '../../services/receita-pac.service';
import { ReceitaPacDataContract } from '../../response-models/receita-pac-response';
import { ProgramActivityService } from '../../services/program-activity.service';
import { ProgramActivityDataContract } from '../../response-models/program-activity-response';
import { EconomicClassificationService } from '../../services/economic-classification.service';
import { InstitutionService } from '../../services/institution.service';
import { SelectDescription } from '../../models/utils';

const SALDO_GERENCIA_CODIGO = '408';

@Component({
  selector: 'app-saldos-abertura',
  templateUrl: './saldos-abertura.component.html',
  styleUrls: ['./saldos-abertura.component.css']
})
export class SaldosAberturaComponent implements OnInit {

  public orcamentoConfigFk = 1;
  public ano = 2026;

  // Seção A — Tài khoản GL (Codigoconta), giá trị đầu kỳ.
  public loadingGl = false;
  public glAccounts: CodigoContaOpeningBalanceDataContract[] = [];
  public filteredGlAccounts: CodigoContaOpeningBalanceDataContract[] = [];
  public pagedGlAccounts: CodigoContaOpeningBalanceDataContract[] = [];
  public glSearch = '';
  public glPageIndex = 0;
  public glPageSize = 20;

  // Seção B — Saldo chuyển tiếp ngân hàng/quỹ (408 - Saldo de Gerência),
  // reaproveitando o mesmo mecanismo/tela do Receita (ReceitaPac) — não é
  // uma entidade nova, apenas um assistente guiado que cria registos
  // ReceitaPac com Classificação Económica = 408.
  public loadingSaldo = false;
  public economicClassification408Id: number | null = null;
  public regimes: ProgramActivityDataContract[] = [];
  public institutions: SelectDescription[] = [];
  public existingSaldoEntries: ReceitaPacDataContract[] = [];

  public formRegimeFk: number | null = null;
  public formOrganizationFk: number | null = null;
  public formDescritivo = '';
  public formValor: number | null = null;
  public formMes = new Date().getMonth() + 1;

  constructor(
    private codigoContaOpeningBalanceService: CodigoContaOpeningBalanceService,
    private receitaPacService: ReceitaPacService,
    private programActivityService: ProgramActivityService,
    private economicClassificationService: EconomicClassificationService,
    private institutionService: InstitutionService,
    private snackBar: MatSnackBar,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.loadGlAccounts();
    this.loadSaldoMasterData();
    this.loadExistingSaldoEntries();
  }

  // ===== Seção A — GL opening balance =====

  public loadGlAccounts(): void {
    this.loadingGl = true;
    this.codigoContaOpeningBalanceService.getAll().subscribe(
      response => {
        this.glAccounts = response.items ?? [];
        this.filteredGlAccounts = this.glAccounts;
        this.glPageIndex = 0;
        this.updatePagedGlAccounts();
        this.loadingGl = false;
      },
      err => {
        this.loadingGl = false;
        this.showError(err);
      }
    );
  }

  public onGlSearchChange(): void {
    const term = (this.glSearch || '').toLowerCase();
    this.filteredGlAccounts = this.glAccounts.filter(c =>
      (c.codigo || '').toLowerCase().includes(term) || (c.designacao || '').toLowerCase().includes(term));
    this.glPageIndex = 0;
    this.updatePagedGlAccounts();
  }

  public onGlPageChange(event: PageEvent): void {
    this.glPageIndex = event.pageIndex;
    this.glPageSize = event.pageSize;
    this.updatePagedGlAccounts();
  }

  private updatePagedGlAccounts(): void {
    const start = this.glPageIndex * this.glPageSize;
    this.pagedGlAccounts = this.filteredGlAccounts.slice(start, start + this.glPageSize);
  }

  public saveGlAccount(c: CodigoContaOpeningBalanceDataContract): void {
    this.codigoContaOpeningBalanceService.update({
      id: c.id,
      initialValue: c.initialValue ?? undefined,
      isCredit: c.isCredit ?? undefined,
      initialValueDate: c.initialValueDate ?? undefined
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.snackBar.open(this.translate.instant('saldosAbertura.glSavedSuccess', { codigo: c.codigo }), this.translate.instant('general.close'), { duration: 2500 });
      },
      err => this.showError(err)
    );
  }

  // ===== Seção B — Saldo chuyển tiếp (408) =====

  private loadSaldoMasterData(): void {
    this.programActivityService.getTree(this.orcamentoConfigFk).subscribe(
      response => {
        const all = response.items ?? [];
        this.regimes = all.filter(a => !a.parentFk);
      },
      err => this.showError(err)
    );

    this.economicClassificationService.getTree(this.orcamentoConfigFk).subscribe(
      response => {
        const found = (response.items ?? []).find(e => e.codigo === SALDO_GERENCIA_CODIGO);
        this.economicClassification408Id = found ? found.id : null;
        if (!found) {
          this.snackBar.open(this.translate.instant('saldosAbertura.errMissing408'), this.translate.instant('general.close'), { duration: 5000 });
        }
      },
      err => this.showError(err)
    );

    this.institutionService.getAllInstitutionsAtivo().subscribe(
      response => this.institutions = response.selects ?? [],
      err => this.showError(err)
    );
  }

  public loadExistingSaldoEntries(): void {
    this.loadingSaldo = true;
    this.receitaPacService.getByAno(this.ano).subscribe(
      response => {
        this.existingSaldoEntries = (response.items ?? []).filter(r => r.economicClassificationCodigo === SALDO_GERENCIA_CODIGO);
        this.loadingSaldo = false;
      },
      err => {
        this.loadingSaldo = false;
        this.showError(err);
      }
    );
  }

  public get totalSaldoTransferido(): number {
    return this.existingSaldoEntries.reduce((sum, r) => sum + r.valorPac, 0);
  }

  public addSaldoEntry(): void {
    if (!this.economicClassification408Id) {
      this.snackBar.open(this.translate.instant('saldosAbertura.errNo408'), this.translate.instant('general.close'), { duration: 4000 });
      return;
    }
    if (!this.formRegimeFk || !this.formOrganizationFk || !this.formDescritivo || !this.formValor) {
      this.snackBar.open(this.translate.instant('saldosAbertura.errMissingFields'), this.translate.instant('general.close'), { duration: 3500 });
      return;
    }

    this.receitaPacService.save({
      id: 0,
      mes: this.formMes,
      ano: this.ano,
      regimeFk: this.formRegimeFk,
      economicClassificationFk: this.economicClassification408Id,
      organizationFk: this.formOrganizationFk,
      descritivo: this.formDescritivo,
      valorPac: this.formValor,
      // Saldo đầu kỳ coi như đã "thu" ngay từ đầu (chuyển từ năm trước sang),
      // không phải khoản phải thu — nên ghi nhận cobradoBanco = valorPac.
      valorCobradoBanco: this.formValor,
      valorCobradoCaixa: 0
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.formRegimeFk = null;
        this.formOrganizationFk = null;
        this.formDescritivo = '';
        this.formValor = null;
        this.snackBar.open(this.translate.instant('saldosAbertura.savedSuccess'), this.translate.instant('general.close'), { duration: 3000 });
        this.loadExistingSaldoEntries();
      },
      err => this.showError(err)
    );
  }

  public removeSaldoEntry(item: ReceitaPacDataContract): void {
    if (!confirm(this.translate.instant('saldosAbertura.confirmDelete', { descritivo: item.descritivo }))) { return; }
    this.receitaPacService.deactivate({ id: item.id }).subscribe(
      () => this.loadExistingSaldoEntries(),
      err => this.showError(err)
    );
  }

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('saldosAbertura.errGeneric');
    this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
  }
}
