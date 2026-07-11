import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { OrcamentoService } from '../../services/orcamento.service';
import { ProgramActivityService } from '../../services/program-activity.service';
import { EconomicClassificationService } from '../../services/economic-classification.service';
import { InstitutionService } from '../../services/institution.service';
import { OrcamentoBatchDataContract, OrcamentoLinhaDataContract } from '../../response-models/orcamento-response';
import { ProgramActivityDataContract } from '../../response-models/program-activity-response';
import { EconomicClassificationDataContract } from '../../response-models/economic-classification-response';
import { SelectDescription } from '../../models/utils';

const ESTADO_LABELS: { [key: string]: string } = {
  DRAFT: 'Nháp',
  PENDING_REVIEW: 'Chờ kiểm tra',
  PENDING_APPROVAL: 'Chờ phê duyệt',
  APPROVED: 'Đã duyệt'
};

@Component({
  selector: 'app-orcamento',
  templateUrl: './orcamento.component.html',
  styleUrls: ['./orcamento.component.css']
})
export class OrcamentoComponent implements OnInit {

  public orcamentoConfigFk = 1;
  public batch: OrcamentoBatchDataContract | null = null;
  public estadoLabels = ESTADO_LABELS;
  public loading = false;

  // Pickers (só folhas — nível mais baixo de cada árvore).
  public atividades: ProgramActivityDataContract[] = [];
  public economicClassifications: EconomicClassificationDataContract[] = [];
  public organizations: SelectDescription[] = [];

  // Danh sách đã lọc theo ô tìm kiếm (autocomplete) — gõ tới đâu lọc tới đó,
  // thay cho kéo-chọn trong mat-select khi danh sách quá dài.
  public filteredAtividades: ProgramActivityDataContract[] = [];
  public filteredEconomicClassifications: EconomicClassificationDataContract[] = [];
  public filteredOrganizations: SelectDescription[] = [];

  public atividadeSearch = '';
  public economicClassificationSearch = '';
  public organizationSearch = '';

  public editingId: number | null = null;
  public formAtividadeFk: number | null = null;
  public formEconomicClassificationFk: number | null = null;
  public formOrganizationFk: number | null = null;
  public formValor: number | null = null;
  public showForm = false;

  public showRejectPrompt = false;
  public rejectComment = '';
  public rejectAction: 'review' | 'approve' | null = null;

  constructor(
    private orcamentoService: OrcamentoService,
    private programActivityService: ProgramActivityService,
    private economicClassificationService: EconomicClassificationService,
    private institutionService: InstitutionService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.loadPickers();
    this.loadBatch();
  }

  private loadPickers(): void {
    this.programActivityService.getTree(this.orcamentoConfigFk).subscribe(response => {
      this.atividades = (response.items ?? []).filter(a => !a.hasKids);
      this.filteredAtividades = this.atividades;
    });
    this.economicClassificationService.getTree(this.orcamentoConfigFk).subscribe(response => {
      this.economicClassifications = (response.items ?? []).filter(e => !e.hasKids);
      this.filteredEconomicClassifications = this.economicClassifications;
    });
    this.institutionService.getAllInstitutionsAtivo().subscribe(response => {
      this.organizations = response.selects ?? [];
      this.filteredOrganizations = this.organizations;
    });
  }

  private normalize(value: string): string {
    return (value || '').toLowerCase();
  }

  public onAtividadeSearchChange(): void {
    const term = this.normalize(this.atividadeSearch);
    this.filteredAtividades = this.atividades.filter(a =>
      this.normalize(a.codigo).includes(term) || this.normalize(a.designacao).includes(term));
  }

  public selectAtividade(a: ProgramActivityDataContract): void {
    this.formAtividadeFk = a.id;
    this.atividadeSearch = `${a.codigo} - ${a.designacao}`;
  }

  public onEconomicClassificationSearchChange(): void {
    const term = this.normalize(this.economicClassificationSearch);
    this.filteredEconomicClassifications = this.economicClassifications.filter(e =>
      this.normalize(e.codigo).includes(term) || this.normalize(e.designacao).includes(term));
  }

  public selectEconomicClassification(e: EconomicClassificationDataContract): void {
    this.formEconomicClassificationFk = e.id;
    this.economicClassificationSearch = `${e.codigo} - ${e.designacao}`;
  }

  public onOrganizationSearchChange(): void {
    const term = this.normalize(this.organizationSearch);
    this.filteredOrganizations = this.organizations.filter(o => this.normalize(o.nome).includes(term));
  }

  public selectOrganization(o: SelectDescription): void {
    this.formOrganizationFk = o.id;
    this.organizationSearch = o.nome;
  }

  public loadBatch(): void {
    this.loading = true;
    this.orcamentoService.getActiveBatch(this.orcamentoConfigFk).subscribe(
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

  public openAddForm(): void {
    this.editingId = null;
    this.formAtividadeFk = null;
    this.formEconomicClassificationFk = null;
    this.formOrganizationFk = null;
    this.formValor = null;
    this.atividadeSearch = '';
    this.economicClassificationSearch = '';
    this.organizationSearch = '';
    this.filteredAtividades = this.atividades;
    this.filteredEconomicClassifications = this.economicClassifications;
    this.filteredOrganizations = this.organizations;
    this.showForm = true;
  }

  public openEditForm(linha: OrcamentoLinhaDataContract): void {
    this.editingId = linha.id;
    this.formAtividadeFk = linha.atividadeFk;
    this.formEconomicClassificationFk = linha.economicClassificationFk;
    this.formOrganizationFk = linha.organizationFk;
    this.formValor = linha.valor;
    this.atividadeSearch = `${linha.atividadeCodigo} - ${linha.atividadeDesignacao}`;
    this.economicClassificationSearch = `${linha.economicClassificationCodigo} - ${linha.economicClassificationDesignacao}`;
    this.organizationSearch = linha.organizationNome;
    this.filteredAtividades = this.atividades;
    this.filteredEconomicClassifications = this.economicClassifications;
    this.filteredOrganizations = this.organizations;
    this.showForm = true;
  }

  public cancelForm(): void {
    this.showForm = false;
  }

  public saveLinha(): void {
    if (!this.formAtividadeFk || !this.formEconomicClassificationFk || !this.formOrganizationFk || !this.formValor) {
      this.snackBar.open('Vui lòng nhập đủ Atividade, Classificação Económica, Organization và Valor.', 'Đóng', { duration: 3000 });
      return;
    }

    this.orcamentoService.saveLinha({
      id: this.editingId ?? 0,
      orcamentoConfigFk: this.orcamentoConfigFk,
      atividadeFk: this.formAtividadeFk,
      economicClassificationFk: this.formEconomicClassificationFk,
      organizationFk: this.formOrganizationFk,
      valor: this.formValor
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

  public deleteLinha(linha: OrcamentoLinhaDataContract): void {
    if (!confirm(`Xoá rúbrica "${linha.atividadeCodigo} / ${linha.economicClassificationCodigo}"?`)) {
      return;
    }
    this.orcamentoService.deleteLinha({ id: linha.id }).subscribe(
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
    if (!confirm('Gửi duyệt toàn bộ danh sách rúbrica hiện tại?')) {
      return;
    }
    this.orcamentoService.submit({ orcamentoConfigFk: this.orcamentoConfigFk }).subscribe(
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
    this.orcamentoService.review({ batchId: this.batch.id, approve: true }).subscribe(
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
    this.orcamentoService.approve({ batchId: this.batch.id, approve: true }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, 'Đóng', { duration: 4000 });
          return;
        }
        this.snackBar.open('Đã phê duyệt ngân sách.', 'Đóng', { duration: 3000 });
        this.loadBatch();
        this.loadPickers();
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
      ? this.orcamentoService.review({ batchId: this.batch.id, approve: false, comment: this.rejectComment })
      : this.orcamentoService.approve({ batchId: this.batch.id, approve: false, comment: this.rejectComment });

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
