import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { OrcamentoSuplementarService } from '../../services/orcamento-suplementar.service';
import { OrcamentoSuplementarBatchDataContract, OrcamentoSuplementarLinhaDataContract, RubricaAprovadaParaSuplementarDataContract } from '../../response-models/orcamento-suplementar-response';

const ESTADO_LABELS: { [key: string]: string } = {
  DRAFT: 'Nháp',
  PENDING_REVIEW: 'Chờ kiểm tra',
  PENDING_APPROVAL: 'Chờ phê duyệt',
  APPROVED: 'Đã duyệt'
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
    private snackBar: MatSnackBar
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
      this.snackBar.open('Vui lòng chọn rúbrica và nhập Giá trị điều chỉnh.', 'Đóng', { duration: 3000 });
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
          this.snackBar.open(response.errors[0].errorMessage, 'Đóng', { duration: 4000 });
          return;
        }
        this.showForm = false;
        this.loadBatch();
      },
      err => this.showError(err)
    );
  }

  public deleteLinha(linha: OrcamentoSuplementarLinhaDataContract): void {
    if (!confirm(`Xoá dòng điều chỉnh "${linha.atividadeCodigo} / ${linha.economicClassificationCodigo}"?`)) {
      return;
    }
    this.suplementarService.deleteLinha({ id: linha.id }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, 'Đóng', { duration: 4000 });
          return;
        }
        this.loadBatch();
      },
      err => this.showError(err)
    );
  }

  public submitBatch(): void {
    if (!confirm('Gửi duyệt toàn bộ đợt bổ sung ngân sách hiện tại?')) {
      return;
    }
    this.suplementarService.submit({ orcamentoConfigFk: this.orcamentoConfigFk }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, 'Đóng', { duration: 4000 });
          return;
        }
        this.snackBar.open('Đã gửi duyệt.', 'Đóng', { duration: 3000 });
        this.loadBatch();
      },
      err => this.showError(err)
    );
  }

  public reviewApprove(): void {
    if (!this.batch) { return; }
    this.suplementarService.review({ batchId: this.batch.id, approve: true }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, 'Đóng', { duration: 4000 });
          return;
        }
        this.snackBar.open('Đã chuyển sang chờ phê duyệt.', 'Đóng', { duration: 3000 });
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
          this.snackBar.open(response.errors[0].errorMessage, 'Đóng', { duration: 4000 });
          return;
        }
        this.snackBar.open('Đã phê duyệt đợt bổ sung ngân sách.', 'Đóng', { duration: 3000 });
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
          this.snackBar.open(response.errors[0].errorMessage, 'Đóng', { duration: 4000 });
          return;
        }
        this.showRejectPrompt = false;
        this.snackBar.open('Đã từ chối — quay lại trạng thái Nháp.', 'Đóng', { duration: 3000 });
        this.loadBatch();
      },
      err => this.showError(err)
    );
  }

  public cancelReject(): void {
    this.showRejectPrompt = false;
  }

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? 'Có lỗi xảy ra.';
    this.snackBar.open(message, 'Đóng', { duration: 4000 });
  }
}
