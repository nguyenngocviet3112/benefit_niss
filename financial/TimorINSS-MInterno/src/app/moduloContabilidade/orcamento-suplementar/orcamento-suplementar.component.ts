import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { OrcamentoSuplementarService } from '../../services/orcamento-suplementar.service';
import { ConfirmDialogService } from '../../services/confirm-dialog.service';
import { OrcamentoSuplementarBatchDataContract, OrcamentoSuplementarLinhaDataContract, RubricaAprovadaParaSuplementarDataContract } from '../../response-models/orcamento-suplementar-response';

const ESTADO_LABELS: { [key: string]: string } = {
  DRAFT: 'orcamentoSuplementar.estadoDraft',
  PENDING_REVIEW: 'orcamentoSuplementar.estadoPendingReview',
  PENDING_APPROVAL: 'orcamentoSuplementar.estadoPendingApproval',
  APPROVED: 'orcamentoSuplementar.estadoApproved'
};

@Component({
  selector: 'app-orcamento-suplementar',
  templateUrl: './orcamento-suplementar.component.html',
  styleUrls: ['./orcamento-suplementar.component.css']
})
export class OrcamentoSuplementarComponent implements OnInit {

  public orcamentoConfigFk = 1;
  public batch: OrcamentoSuplementarBatchDataContract | null = null;
  public estadoLabels = ESTADO_LABELS;
  public loading = false;

  public rubricasAprovadas: RubricaAprovadaParaSuplementarDataContract[] = [];
  public filteredRubricas: RubricaAprovadaParaSuplementarDataContract[] = [];
  public rubricaSearch = '';

  public editingId: number | null = null;
  public formOrcamentoLinhaFk: number | null = null;
  public formOldValue: number | null = null;
  public formAdjustmentValue: number | null = null;
  public showForm = false;

  public showRejectPrompt = false;
  public rejectComment = '';
  public rejectAction: 'review' | 'approve' | null = null;

  constructor(
    private suplementarService: OrcamentoSuplementarService,
    private snackBar: MatSnackBar,
    private translate: TranslateService,
    private confirmDialog: ConfirmDialogService
  ) { }

  ngOnInit(): void {
    this.loadRubricas();
    this.loadBatch();
  }

  public loadRubricas(): void {
    this.suplementarService.getRubricasAprovadas(this.orcamentoConfigFk).subscribe(response => {
      this.rubricasAprovadas = response.items ?? [];
      this.filteredRubricas = this.rubricasAprovadas;
    });
  }

  public loadBatch(): void {
    this.loading = true;
    this.suplementarService.getActiveBatch(this.orcamentoConfigFk).subscribe(
      response => {
        this.batch = response.batch;
        this.loading = false;
      },
      err => {
        this.loading = false;
        this.showError(err);
      }
    );
  }

  public get isDraft(): boolean {
    return this.batch?.estado === 'DRAFT';
  }

  public get finalValuePreview(): number {
    return (this.formOldValue ?? 0) + (this.formAdjustmentValue ?? 0);
  }

  public onRubricaSearchChange(): void {
    const term = (this.rubricaSearch || '').toLowerCase();
    this.filteredRubricas = this.rubricasAprovadas.filter(r =>
      (r.atividadeCodigo || '').toLowerCase().includes(term) ||
      (r.atividadeDesignacao || '').toLowerCase().includes(term) ||
      (r.economicClassificationCodigo || '').toLowerCase().includes(term) ||
      (r.economicClassificationDesignacao || '').toLowerCase().includes(term));
  }

  public selectRubrica(r: RubricaAprovadaParaSuplementarDataContract): void {
    this.formOrcamentoLinhaFk = r.orcamentoLinhaId;
    this.formOldValue = r.valorAtual;
    this.rubricaSearch = `${r.atividadeCodigo} - ${r.economicClassificationCodigo} (${r.organizationNome})`;
  }

  public openAddForm(): void {
    this.editingId = null;
    this.formOrcamentoLinhaFk = null;
    this.formOldValue = null;
    this.formAdjustmentValue = null;
    this.rubricaSearch = '';
    this.filteredRubricas = this.rubricasAprovadas;
    this.showForm = true;
  }

  public openEditForm(linha: OrcamentoSuplementarLinhaDataContract): void {
    this.editingId = linha.id;
    this.formOrcamentoLinhaFk = linha.orcamentoLinhaFk;
    this.formOldValue = linha.oldValue;
    this.formAdjustmentValue = linha.adjustmentValue;
    this.rubricaSearch = `${linha.atividadeCodigo} - ${linha.economicClassificationCodigo} (${linha.organizationNome})`;
    this.filteredRubricas = this.rubricasAprovadas;
    this.showForm = true;
  }

  public cancelForm(): void {
    this.showForm = false;
  }

  public saveLinha(): void {
    if (!this.formOrcamentoLinhaFk || this.formAdjustmentValue === null || this.formAdjustmentValue === undefined) {
      this.snackBar.open(this.translate.instant('orcamentoSuplementar.errMissingFields'), this.translate.instant('general.close'), { duration: 3000 });
      return;
    }

    this.suplementarService.saveLinha({
      id: this.editingId ?? 0,
      orcamentoConfigFk: this.orcamentoConfigFk,
      orcamentoLinhaFk: this.formOrcamentoLinhaFk,
      adjustmentValue: this.formAdjustmentValue
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.showForm = false;
        this.loadBatch();
      },
      err => this.showError(err)
    );
  }

  public deleteLinha(linha: OrcamentoSuplementarLinhaDataContract): void {
    this.confirmDialog.confirm(this.translate.instant('orcamentoSuplementar.confirmDelete', { atividade: linha.atividadeCodigo, ec: linha.economicClassificationCodigo })).subscribe(confirmed => {
      if (!confirmed) { return; }
      this.suplementarService.deleteLinha({ id: linha.id }).subscribe(
        response => {
          if (response.errors && response.errors.length > 0) {
            this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
            return;
          }
          this.loadBatch();
        },
        err => this.showError(err)
      );
    });
  }

  public submitBatch(): void {
    this.confirmDialog.confirm(this.translate.instant('orcamentoSuplementar.confirmSubmitBatch')).subscribe(confirmed => {
      if (!confirmed) { return; }
      this.suplementarService.submit({ orcamentoConfigFk: this.orcamentoConfigFk }).subscribe(
        response => {
          if (response.errors && response.errors.length > 0) {
            this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
            return;
          }
          this.snackBar.open(this.translate.instant('orcamentoSuplementar.submittedSuccess'), this.translate.instant('general.close'), { duration: 3000 });
          this.loadBatch();
        },
        err => this.showError(err)
      );
    });
  }

  public reviewApprove(): void {
    if (!this.batch) { return; }
    this.suplementarService.review({ batchId: this.batch.id, approve: true }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.snackBar.open(this.translate.instant('orcamentoSuplementar.reviewedSuccess'), this.translate.instant('general.close'), { duration: 3000 });
        this.loadBatch();
      },
      err => this.showError(err)
    );
  }

  public finalApprove(): void {
    if (!this.batch) { return; }
    this.suplementarService.approve({ batchId: this.batch.id, approve: true }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.snackBar.open(this.translate.instant('orcamentoSuplementar.approvedSuccess'), this.translate.instant('general.close'), { duration: 3000 });
        this.loadBatch();
        this.loadRubricas();
      },
      err => this.showError(err)
    );
  }

  public openRejectPrompt(action: 'review' | 'approve'): void {
    this.rejectAction = action;
    this.rejectComment = '';
    this.showRejectPrompt = true;
  }

  public confirmReject(): void {
    if (!this.batch || !this.rejectAction) { return; }
    const call = this.rejectAction === 'review'
      ? this.suplementarService.review({ batchId: this.batch.id, approve: false, comment: this.rejectComment })
      : this.suplementarService.approve({ batchId: this.batch.id, approve: false, comment: this.rejectComment });

    call.subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.showRejectPrompt = false;
        this.snackBar.open(this.translate.instant('orcamentoSuplementar.rejectedSuccess'), this.translate.instant('general.close'), { duration: 3000 });
        this.loadBatch();
      },
      err => this.showError(err)
    );
  }

  public cancelReject(): void {
    this.showRejectPrompt = false;
  }

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('orcamentoSuplementar.errGeneric');
    this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
  }
}
