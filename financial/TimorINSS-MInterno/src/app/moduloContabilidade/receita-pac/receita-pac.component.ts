import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { EconomicClassificationDataContract } from '../../response-models/economic-classification-response';
import { ProgramActivityDataContract } from '../../response-models/program-activity-response';
import { ReceitaPacDataContract } from '../../response-models/receita-pac-response';
import { EconomicClassificationService } from '../../services/economic-classification.service';
import { InstitutionService } from '../../services/institution.service';
import { ProgramActivityService } from '../../services/program-activity.service';
import { ReceitaPacService } from '../../services/receita-pac.service';
import { SelectDescription } from '../../models/utils';
import { BankAccountModel, BankAccountService } from '../../services/bank-account.service';
import { CodigoContaOptionDataContract } from '../../response-models/payment-response';

@Component({
  selector: 'app-receita-pac',
  templateUrl: './receita-pac.component.html',
  styleUrls: ['./receita-pac.component.css']
})
export class ReceitaPacComponent implements OnInit {

  public ano = 2026;
  public orcamentoConfigFk = 1;
  public loading = false;

  public items: ReceitaPacDataContract[] = [];
  public expandedId: number | null = null;

  public meses = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];

  public regimes: ProgramActivityDataContract[] = [];
  public atividades: ProgramActivityDataContract[] = [];
  public economicClassifications: EconomicClassificationDataContract[] = [];
  public institutions: SelectDescription[] = [];
  public bankAccounts: BankAccountModel[] = [];
  public codigoContaOptions: CodigoContaOptionDataContract[] = [];

  public showForm = false;
  public editingId = 0;
  public formMes = new Date().getMonth() + 1;
  public formNiss = '';
  public formRegimeFk: number | null = null;
  public formAtividadeFk: number | null = null;
  public formEconomicClassificationFk: number | null = null;
  public formOrganizationFk: number | null = null;
  public formDescritivo = '';
  public formValorPac: number | null = null;
  public formValorCobradoBanco = 0;
  public formValorCobradoCaixa = 0;
  public formContaBancariaFk: number | null = null;
  public formCodigoContaDebitoFk: number | null = null;
  public formCodigoContaCreditoFk: number | null = null;

  constructor(
    private receitaPacService: ReceitaPacService,
    private programActivityService: ProgramActivityService,
    private economicClassificationService: EconomicClassificationService,
    private institutionService: InstitutionService,
    private bankAccountService: BankAccountService,
    private snackBar: MatSnackBar,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.load();
    this.loadMasterData();
  }

  public load(): void {
    this.loading = true;
    this.receitaPacService.getByAno(this.ano).subscribe(
      response => {
        this.items = response.items ?? [];
        this.loading = false;
      },
      err => {
        this.loading = false;
        this.showError(err);
      }
    );
  }

  private loadMasterData(): void {
    this.programActivityService.getTree(this.orcamentoConfigFk).subscribe(
      response => {
        const all = response.items ?? [];
        this.regimes = all.filter(a => !a.parentFk);
        this.atividades = all;
      },
      err => this.showError(err)
    );

    this.economicClassificationService.getTree(this.orcamentoConfigFk).subscribe(
      response => {
        // RECEITAS_PAC only registers Receita codes (4xx) — RECEITAS_GP (401.xx contribution
        // income) already comes from the Contribuições module, not entered here either way,
        // but keep the picker scoped to the Receita branch of the tree for clarity.
        const all = response.items ?? [];
        const receitaRootIds = new Set(all.filter(e => e.tipo === 'Receita').map(e => e.id));
        const isUnderReceita = (node: EconomicClassificationDataContract): boolean => {
          let currentId: number | undefined = node.id;
          while (currentId !== undefined) {
            if (receitaRootIds.has(currentId)) { return true; }
            const parent: EconomicClassificationDataContract | undefined = all.find(e => e.id === currentId);
            currentId = parent ? parent.parentFk : undefined;
          }
          return false;
        };
        this.economicClassifications = all.filter(e => isUnderReceita(e));
      },
      err => this.showError(err)
    );

    this.institutionService.getAllInstitutionsAtivo().subscribe(
      response => this.institutions = response.selects ?? [],
      err => this.showError(err)
    );

    // Ngân hàng để chọn "thu qua ngân hàng nào" khi ValorCobradoBanco > 0 —
    // lấy từ màn cấu hình "Cấu hình hệ thống > Ngân hàng" (user request 2026-07-11).
    this.bankAccountService.getAll().subscribe(
      response => this.bankAccounts = response.items ?? [],
      err => this.showError(err)
    );

    // Tài khoản Nợ/Có để tự sinh Lançamento — dùng chung Plano de Contas với
    // Pagamento (xem memory lancamentos-conciliacao-link-design).
    this.receitaPacService.getCodigoContaOptions().subscribe(
      response => this.codigoContaOptions = response.items ?? [],
      err => this.showError(err)
    );
  }

  public indent(nivel: number): string {
    return `${(nivel - 1) * 16}px`;
  }

  public toggleExpand(item: ReceitaPacDataContract): void {
    this.expandedId = this.expandedId === item.id ? null : item.id;
  }

  public openCreateForm(): void {
    this.editingId = 0;
    this.formMes = new Date().getMonth() + 1;
    this.formNiss = '';
    this.formRegimeFk = null;
    this.formAtividadeFk = null;
    this.formEconomicClassificationFk = null;
    this.formOrganizationFk = null;
    this.formDescritivo = '';
    this.formValorPac = null;
    this.formValorCobradoBanco = 0;
    this.formValorCobradoCaixa = 0;
    this.formContaBancariaFk = null;
    this.formCodigoContaDebitoFk = null;
    this.formCodigoContaCreditoFk = null;
    this.showForm = true;
  }

  public openEditForm(item: ReceitaPacDataContract): void {
    this.editingId = item.id;
    this.formMes = item.mes;
    this.formNiss = item.niss ?? '';
    this.formRegimeFk = item.regimeFk;
    this.formAtividadeFk = item.atividadeFk ?? null;
    this.formEconomicClassificationFk = item.economicClassificationFk;
    this.formOrganizationFk = item.organizationFk;
    this.formDescritivo = item.descritivo;
    this.formValorPac = item.valorPac;
    this.formValorCobradoBanco = item.valorCobradoBanco;
    this.formValorCobradoCaixa = item.valorCobradoCaixa;
    this.formContaBancariaFk = item.contaBancariaFk ?? null;
    this.formCodigoContaDebitoFk = item.codigoContaDebitoFk ?? null;
    this.formCodigoContaCreditoFk = item.codigoContaCreditoFk ?? null;
    this.showForm = true;
  }

  public cancelForm(): void {
    this.showForm = false;
  }

  public saveReceita(): void {
    if (!this.formRegimeFk || !this.formEconomicClassificationFk || !this.formOrganizationFk || !this.formValorPac || !this.formDescritivo) {
      this.snackBar.open(this.translate.instant('receitaPac.errMissingFields'), this.translate.instant('general.close'), { duration: 3500 });
      return;
    }
    if (this.formValorCobradoBanco > 0 && !this.formContaBancariaFk) {
      this.snackBar.open(this.translate.instant('receitaPac.errMissingBankAccount'), this.translate.instant('general.close'), { duration: 3500 });
      return;
    }

    this.receitaPacService.save({
      id: this.editingId,
      mes: this.formMes,
      ano: this.ano,
      niss: this.formNiss || undefined,
      regimeFk: this.formRegimeFk,
      atividadeFk: this.formAtividadeFk ?? undefined,
      economicClassificationFk: this.formEconomicClassificationFk,
      organizationFk: this.formOrganizationFk,
      descritivo: this.formDescritivo,
      valorPac: this.formValorPac,
      valorCobradoBanco: this.formValorCobradoBanco,
      valorCobradoCaixa: this.formValorCobradoCaixa,
      contaBancariaFk: this.formContaBancariaFk ?? undefined,
      codigoContaDebitoFk: this.formCodigoContaDebitoFk ?? undefined,
      codigoContaCreditoFk: this.formCodigoContaCreditoFk ?? undefined
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.showForm = false;
        this.snackBar.open(this.translate.instant('receitaPac.savedSuccess', { numero: response.item.numero }), this.translate.instant('general.close'), { duration: 3000 });
        this.load();
      },
      err => this.showError(err)
    );
  }

  public deactivate(item: ReceitaPacDataContract): void {
    if (!confirm(this.translate.instant('receitaPac.confirmDeactivate', { numero: item.numero }))) { return; }
    this.receitaPacService.deactivate({ id: item.id }).subscribe(
      () => this.load(),
      err => this.showError(err)
    );
  }

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('receitaPac.errGeneric');
    this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
  }
}
