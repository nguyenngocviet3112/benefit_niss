import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { CabimentoService } from '../../services/cabimento.service';
import { ConfirmDialogService } from '../../services/confirm-dialog.service';
import { PermissionService } from '../../services/permission.service';
import { AdDisponivelParaCabimentoDataContract, CabimentoDataContract } from '../../response-models/cabimento-response';
import { AttachmentConfigService } from '../../services/attachment-config.service';
import { AttachmentConfigItem } from '../../response-models/attachment-config-response';
import { AttachmentItem } from '../../response-models/attachment-response';

const ESTADO_LABELS: { [key: string]: string } = {
  DRAFT: 'cabimento.estadoDraft',
  PENDING_APPROVAL: 'cabimento.estadoPendingApproval',
  APPROVED: 'cabimento.estadoApproved'
};

@Component({
  selector: 'app-cabimento',
  templateUrl: './cabimento.component.html',
  styleUrls: ['./cabimento.component.css']
})
export class CabimentoComponent implements OnInit {

  public ano = 2026;
  public estadoLabels = ESTADO_LABELS;
  public loading = false;
  public allItems: CabimentoDataContract[] = [];
  public items: CabimentoDataContract[] = [];

  public estadoOptions: { value: string; label: string }[] = [
    { value: '', label: 'cabimento.estadoTodos' },
    { value: 'DRAFT', label: 'cabimento.estadoDraft' },
    { value: 'PENDING_APPROVAL', label: 'cabimento.estadoPendingApproval' },
    { value: 'APPROVED', label: 'cabimento.estadoApproved' }
  ];
  public selectedEstado = '';

  public availableAds: AdDisponivelParaCabimentoDataContract[] = [];
  public showAdPicker = false;
  public pickedAd: AdDisponivelParaCabimentoDataContract | null = null;
  public formDescritivo = '';
  public formValorCabimentado: number | null = null;
  public formProcessoAprovisionamentoPrevio = false;
  public formProposta = '';
  public formFundamentacaoLegal = '';
  public formMes = 1;

  public showApprovePrompt = false;
  public approveComment = '';
  public approveId: number | null = null;

  public showRejectPrompt = false;
  public rejectComment = '';
  public rejectId: number | null = null;

  public editId: number | null = null;
  public editDescritivo = '';
  public editValorCabimentado: number | null = null;
  public editProcessoAprovisionamentoPrevio = false;

  public attachmentConfig: AttachmentConfigItem | null = null;
  public cabimentoHasAttachment: { [id: number]: boolean } = {};

  constructor(
    private cabimentoService: CabimentoService,
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

  // See ad-cabimento.component.ts for the rationale: only default-restrict
  // approve-only accounts, so they don't see not-yet-submitted records.
  private computeDefaultEstado(): string {
    const hasSubmit = this.permissionService.hasPerm('CABIMENTO_SUBMIT');
    const hasApprove = this.permissionService.hasPerm('CABIMENTO_APPROVE');
    if (hasSubmit) { return ''; }
    if (hasApprove) { return 'PENDING_APPROVAL'; }
    return '';
  }

  public onAttachmentsLoaded(cabimentoId: number, items: AttachmentItem[]): void {
    this.cabimentoHasAttachment[cabimentoId] = items.length > 0;
  }

  public load(): void {
    this.loading = true;
    this.cabimentoService.getByAno(this.ano).subscribe(
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

  public openAdPicker(): void {
    this.cabimentoService.getAdsDisponiveis(this.ano).subscribe(
      response => {
        this.availableAds = response.items ?? [];
        this.showAdPicker = true;
      },
      err => this.showError(err)
    );
  }

  public pickAd(ad: AdDisponivelParaCabimentoDataContract): void {
    this.pickedAd = ad;
    this.formDescritivo = '';
    this.formValorCabimentado = ad.valorRevisto;
    this.formProcessoAprovisionamentoPrevio = false;
    this.formProposta = '';
    this.formFundamentacaoLegal = '';
    this.formMes = new Date().getMonth() + 1;
  }

  public cancelAdPicker(): void {
    this.showAdPicker = false;
    this.pickedAd = null;
  }

  public createCabimento(): void {
    if (!this.pickedAd || !this.formValorCabimentado) {
      this.snackBar.open(this.translate.instant('cabimento.errMissingAd'), this.translate.instant('general.close'), { duration: 3000 });
      return;
    }

    this.cabimentoService.create({
      expenditureAuthorizationFk: this.pickedAd.expenditureAuthorizationId,
      descritivo: this.formDescritivo,
      valorCabimentado: this.formValorCabimentado,
      processoAprovisionamentoPrevio: this.formProcessoAprovisionamentoPrevio,
      proposta: this.formProposta || undefined,
      fundamentacaoLegal: this.formFundamentacaoLegal || undefined,
      mes: this.formMes,
      ano: this.ano
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.showAdPicker = false;
        this.pickedAd = null;
        this.snackBar.open(this.translate.instant('cabimento.createdSuccess', { numero: response.item.numero }), this.translate.instant('general.close'), { duration: 3000 });
        this.load();
      },
      err => this.showError(err)
    );
  }

  public openEdit(item: CabimentoDataContract): void {
    this.editId = item.id;
    this.editDescritivo = item.descritivo;
    this.editValorCabimentado = item.valorCabimentado;
    this.editProcessoAprovisionamentoPrevio = !!item.processoAprovisionamentoPrevio;
  }

  public cancelEdit(): void {
    this.editId = null;
  }

  public saveEdit(): void {
    if (this.editId == null || !this.editValorCabimentado) { return; }
    this.cabimentoService.save({
      id: this.editId,
      descritivo: this.editDescritivo,
      valorCabimentado: this.editValorCabimentado,
      processoAprovisionamentoPrevio: this.editProcessoAprovisionamentoPrevio
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.editId = null;
        this.load();
      },
      err => this.showError(err)
    );
  }

  public submitCabimento(item: CabimentoDataContract): void {
    this.confirmDialog.confirm(this.translate.instant('cabimento.confirmSubmit', { numero: item.numero })).subscribe(confirmed => {
      if (!confirmed) { return; }
      this.cabimentoService.submit({ id: item.id }).subscribe(
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

  public openApprovePrompt(id: number): void {
    this.approveId = id;
    this.approveComment = '';
    this.showApprovePrompt = true;
  }

  public confirmApprove(): void {
    if (!this.approveId) { return; }
    this.cabimentoService.approve({ id: this.approveId, approve: true, comment: this.approveComment || undefined }).subscribe(
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

  public openRejectPrompt(id: number): void {
    this.rejectId = id;
    this.rejectComment = '';
    this.showRejectPrompt = true;
  }

  public confirmReject(): void {
    if (!this.rejectId) { return; }
    this.cabimentoService.approve({ id: this.rejectId, approve: false, comment: this.rejectComment }).subscribe(
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
    const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('cabimento.errGeneric');
    this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
  }
}
