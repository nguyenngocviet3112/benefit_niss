import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { ExpenditureAuthorizationService } from '../../services/expenditure-authorization.service';
import { ConfirmDialogService } from '../../services/confirm-dialog.service';
import { PermissionService } from '../../services/permission.service';
import { ExpenditureAuthorizationDataContract, RubricaDisponivelDataContract } from '../../response-models/expenditure-authorization-response';
import { AttachmentConfigService } from '../../services/attachment-config.service';
import { AttachmentConfigItem } from '../../response-models/attachment-config-response';
import { AttachmentItem } from '../../response-models/attachment-response';

const ESTADO_LABELS: { [key: string]: string } = {
  DRAFT: 'adCabimento.estadoDraft',
  PENDING_REVIEW: 'adCabimento.estadoPendingReview',
  PENDING_APPROVAL: 'adCabimento.estadoPendingApproval',
  APPROVED: 'adCabimento.estadoApproved'
};

@Component({
  selector: 'app-ad-cabimento',
  templateUrl: './ad-cabimento.component.html',
  styleUrls: ['./ad-cabimento.component.css']
})
export class AdCabimentoComponent implements OnInit {

  public ano = 2026;
  public orcamentoConfigFk = 1;
  public estadoLabels = ESTADO_LABELS;
  public loading = false;

  public allItems: ExpenditureAuthorizationDataContract[] = [];
  public items: ExpenditureAuthorizationDataContract[] = [];
  public expandedId: number | null = null;

  public estadoOptions: { value: string; label: string }[] = [
    { value: '', label: 'adCabimento.estadoTodos' },
    { value: 'DRAFT', label: 'adCabimento.estadoDraft' },
    { value: 'PENDING_REVIEW', label: 'adCabimento.estadoPendingReview' },
    { value: 'PENDING_APPROVAL', label: 'adCabimento.estadoPendingApproval' },
    { value: 'APPROVED', label: 'adCabimento.estadoApproved' }
  ];
  public selectedEstado = '';

  public availableRubricas: RubricaDisponivelDataContract[] = [];
  public showRubricaPicker = false;
  public pickedRubrica: RubricaDisponivelDataContract | null = null;
  public formDescritivo = '';
  public formValorAutorizado: number | null = null;
  public formTipoDespesa: 'UNICA' | 'CONJUNTO' = 'UNICA';
  public formSolicitaAberturaAprovisionamento = false;
  public formProposta = '';
  public formFundamentacaoLegal = '';
  public formObjetivoDespesa = '';
  public formMes = 1;

  public showApprovePrompt = false;
  public approveComment = '';
  public approveAction: { adId: number; action: 'review' | 'approve' } | null = null;

  public formRegularizacao = 0;

  public formPluriAno: number | null = null;
  public formPluriValor: number | null = null;

  public showRejectPrompt = false;
  public rejectComment = '';
  public rejectAction: { adId: number; action: 'review' | 'approve' } | null = null;

  public attachmentConfig: AttachmentConfigItem | null = null;
  public adHasAttachment: { [id: number]: boolean } = {};

  public onAttachmentsLoaded(adId: number, items: AttachmentItem[]): void {
    this.adHasAttachment[adId] = items.length > 0;
  }

  constructor(
    private expenditureAuthorizationService: ExpenditureAuthorizationService,
    private attachmentConfigService: AttachmentConfigService,
    private snackBar: MatSnackBar,
    private translate: TranslateService,
    private confirmDialog: ConfirmDialogService,
    private permissionService: PermissionService
  ) { }

  ngOnInit(): void {
    this.selectedEstado = this.computeDefaultEstado();
    this.load();
    this.attachmentConfigService.getConfig().subscribe(response => this.attachmentConfig = response.item ?? null);
  }

  // Default the Estado filter to whatever's actionable for this account's role,
  // so a Reviewer/Approver-only account doesn't see not-yet-their-turn records
  // by default (avoids clutter) — accounts that can Submit still need to see
  // their own DRAFT items, and multi-role/ADMIN accounts need full visibility,
  // so both cases fall back to no filter ("Tất cả").
  private computeDefaultEstado(): string {
    const hasSubmit = this.permissionService.hasPerm('AD_SUBMIT');
    const hasReview = this.permissionService.hasPerm('AD_REVIEW');
    const hasApprove = this.permissionService.hasPerm('AD_APPROVE');
    if (hasSubmit || (hasReview && hasApprove)) { return ''; }
    if (hasReview) { return 'PENDING_REVIEW'; }
    if (hasApprove) { return 'PENDING_APPROVAL'; }
    return '';
  }

  public load(): void {
    this.loading = true;
    this.expenditureAuthorizationService.getByAno(this.ano).subscribe(
      response => {
        this.allItems = response.items ?? [];
        this.applyFilters();
        this.loading = false;
      },
      err => {
        this.loading = false;
        this.showError(err);
      }
    );
  }

  public onEstadoChange(): void {
    this.applyFilters();
  }

  private applyFilters(): void {
    this.items = this.selectedEstado
      ? this.allItems.filter(i => i.estado === this.selectedEstado)
      : this.allItems;
  }

  public toggleExpand(item: ExpenditureAuthorizationDataContract): void {
    this.expandedId = this.expandedId === item.id ? null : item.id;
    this.formRegularizacao = item.regularizacao;
  }

  public openRubricaPicker(): void {
    this.expenditureAuthorizationService.getAvailableRubricas(this.orcamentoConfigFk).subscribe(
      response => {
        this.availableRubricas = response.items ?? [];
        this.showRubricaPicker = true;
      },
      err => this.showError(err)
    );
  }

  public pickRubrica(r: RubricaDisponivelDataContract): void {
    this.pickedRubrica = r;
    this.formDescritivo = '';
    this.formValorAutorizado = r.valor;
    this.formTipoDespesa = 'UNICA';
    this.formSolicitaAberturaAprovisionamento = false;
    this.formProposta = '';
    this.formFundamentacaoLegal = '';
    this.formObjetivoDespesa = '';
    this.formMes = new Date().getMonth() + 1;
  }

  public cancelRubricaPicker(): void {
    this.showRubricaPicker = false;
    this.pickedRubrica = null;
  }

  public createAd(): void {
    if (!this.pickedRubrica || !this.formValorAutorizado) {
      this.snackBar.open(this.translate.instant('adCabimento.errMissingRubrica'), this.translate.instant('general.close'), { duration: 3000 });
      return;
    }

    this.expenditureAuthorizationService.create({
      orcamentoLinhaFk: this.pickedRubrica.orcamentoLinhaId,
      descritivo: this.formDescritivo,
      valorAutorizado: this.formValorAutorizado,
      tipoDespesa: this.formTipoDespesa,
      solicitaAberturaAprovisionamento: this.formSolicitaAberturaAprovisionamento,
      proposta: this.formProposta || undefined,
      fundamentacaoLegal: this.formFundamentacaoLegal || undefined,
      objetivoDespesa: this.formObjetivoDespesa || undefined,
      mes: this.formMes,
      ano: this.ano
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.showRubricaPicker = false;
        this.pickedRubrica = null;
        this.snackBar.open(this.translate.instant('adCabimento.createdSuccess', { numero: response.item.numero }), this.translate.instant('general.close'), { duration: 3000 });
        this.load();
      },
      err => this.showError(err)
    );
  }

  public saveRegularizacao(item: ExpenditureAuthorizationDataContract): void {
    this.expenditureAuthorizationService.save({
      id: item.id,
      descritivo: item.descritivo,
      valorAutorizado: item.valorAutorizado,
      regularizacao: this.formRegularizacao
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.load();
      },
      err => this.showError(err)
    );
  }

  public addPlurianualidade(item: ExpenditureAuthorizationDataContract): void {
    if (!this.formPluriAno || !this.formPluriValor) {
      this.snackBar.open(this.translate.instant('adCabimento.errMissingPluri'), this.translate.instant('general.close'), { duration: 3000 });
      return;
    }
    this.expenditureAuthorizationService.savePlurianualidade({
      id: 0,
      expenditureAuthorizationFk: item.id,
      ano: this.formPluriAno,
      valor: this.formPluriValor
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.formPluriAno = null;
        this.formPluriValor = null;
        this.load();
      },
      err => this.showError(err)
    );
  }

  public deletePlurianualidade(id: number): void {
    this.expenditureAuthorizationService.deletePlurianualidade({ id }).subscribe(
      () => this.load(),
      err => this.showError(err)
    );
  }

  public submitAd(item: ExpenditureAuthorizationDataContract): void {
    this.confirmDialog.confirm(this.translate.instant('adCabimento.confirmSubmit', { numero: item.numero })).subscribe(confirmed => {
      if (!confirmed) { return; }
      this.expenditureAuthorizationService.submit({ id: item.id }).subscribe(
        response => {
          if (response.errors && response.errors.length > 0) {
            this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
            return;
          }
          this.load();
        },
        err => this.showError(err)
      );
    });
  }

  public openApprovePrompt(adId: number, action: 'review' | 'approve'): void {
    this.approveAction = { adId, action };
    this.approveComment = '';
    this.showApprovePrompt = true;
  }

  public confirmApprove(): void {
    if (!this.approveAction) { return; }
    const { adId, action } = this.approveAction;
    const call = action === 'review'
      ? this.expenditureAuthorizationService.review({ id: adId, approve: true, comment: this.approveComment || undefined })
      : this.expenditureAuthorizationService.approve({ id: adId, approve: true, comment: this.approveComment || undefined });

    call.subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.showApprovePrompt = false;
        this.load();
      },
      err => this.showError(err)
    );
  }

  public cancelApprove(): void {
    this.showApprovePrompt = false;
  }

  public openRejectPrompt(adId: number, action: 'review' | 'approve'): void {
    this.rejectAction = { adId, action };
    this.rejectComment = '';
    this.showRejectPrompt = true;
  }

  public confirmReject(): void {
    if (!this.rejectAction) { return; }
    const { adId, action } = this.rejectAction;
    const call = action === 'review'
      ? this.expenditureAuthorizationService.review({ id: adId, approve: false, comment: this.rejectComment })
      : this.expenditureAuthorizationService.approve({ id: adId, approve: false, comment: this.rejectComment });

    call.subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.showRejectPrompt = false;
        this.load();
      },
      err => this.showError(err)
    );
  }

  public cancelReject(): void {
    this.showRejectPrompt = false;
  }

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('adCabimento.errGeneric');
    this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
  }
}
