import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ObligationService } from '../../services/obligation.service';
import { CompromissoComSaldoDataContract, ObligationDataContract } from '../../response-models/obligation-response';

const ESTADO_LABELS: { [key: string]: string } = {
  DRAFT: 'Nháp',
  PENDING_APPROVAL: 'Chờ phê duyệt',
  APPROVED: 'Đã duyệt'
};

@Component({
  selector: 'app-obligation',
  templateUrl: './obligation.component.html',
  styleUrls: ['./obligation.component.css']
})
export class ObligationComponent implements OnInit {

  public ano = 2026;
  public estadoLabels = ESTADO_LABELS;
  public loading = false;
  public items: ObligationDataContract[] = [];
  public expandedId: number | null = null;

  public showCreateForm = false;
  public formDescritivo = '';
  public formMes = 1;

  public compromissosComSaldo: CompromissoComSaldoDataContract[] = [];
  public addItemTarget: ObligationDataContract | null = null;
  public formCompromissoFk: number | null = null;
  public formValue: number | null = null;

  public showRejectPrompt = false;
  public rejectComment = '';
  public rejectId: number | null = null;

  constructor(
    private obligationService: ObligationService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.load();
  }

  public load(): void {
    this.loading = true;
    this.obligationService.getByAno(this.ano).subscribe(
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

  public toggleExpand(item: ObligationDataContract): void {
    this.expandedId = this.expandedId === item.id ? null : item.id;
    this.addItemTarget = null;
  }

  public openCreateForm(): void {
    this.formDescritivo = '';
    this.formMes = new Date().getMonth() + 1;
    this.showCreateForm = true;
  }

  public cancelCreateForm(): void {
    this.showCreateForm = false;
  }

  public createObligation(): void {
    this.obligationService.create({
      descritivoObrigacao: this.formDescritivo,
      mes: this.formMes,
      ano: this.ano
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, 'Đóng', { duration: 4000 });
          return;
        }
        this.showCreateForm = false;
        this.snackBar.open(`Đã tạo Obrigação số ${response.item.numero}.`, 'Đóng', { duration: 3000 });
        this.load();
      },
      err => this.showError(err)
    );
  }

  public openAddItem(item: ObligationDataContract): void {
    this.addItemTarget = item;
    this.formCompromissoFk = null;
    this.formValue = null;
    this.obligationService.getCompromissosComSaldo(this.ano).subscribe(
      response => {
        this.compromissosComSaldo = response.items ?? [];
      },
      err => this.showError(err)
    );
  }

  public cancelAddItem(): void {
    this.addItemTarget = null;
  }

  public confirmAddItem(): void {
    if (!this.addItemTarget || !this.formCompromissoFk || !this.formValue) {
      this.snackBar.open('Vui lòng chọn Compromisso và nhập Giá trị.', 'Đóng', { duration: 3000 });
      return;
    }

    this.obligationService.addItem({
      obligationFk: this.addItemTarget.id,
      compromissoDespesaFk: this.formCompromissoFk,
      value: this.formValue
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, 'Đóng', { duration: 4000 });
          return;
        }
        this.addItemTarget = null;
        this.load();
      },
      err => this.showError(err)
    );
  }

  public removeItem(id: number): void {
    this.obligationService.removeItem({ id }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, 'Đóng', { duration: 4000 });
          return;
        }
        this.load();
      },
      err => this.showError(err)
    );
  }

  public submitObligation(item: ObligationDataContract): void {
    if (!confirm(`Gửi duyệt Obrigação số ${item.numero}?`)) { return; }
    this.obligationService.submit({ id: item.id }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, 'Đóng', { duration: 4000 });
          return;
        }
        this.load();
      },
      err => this.showError(err)
    );
  }

  public approve(item: ObligationDataContract): void {
    this.obligationService.approve({ id: item.id, approve: true }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, 'Đóng', { duration: 4000 });
          return;
        }
        this.load();
      },
      err => this.showError(err)
    );
  }

  public openRejectPrompt(id: number): void {
    this.rejectId = id;
    this.rejectComment = '';
    this.showRejectPrompt = true;
  }

  public confirmReject(): void {
    if (!this.rejectId) { return; }
    this.obligationService.approve({ id: this.rejectId, approve: false, comment: this.rejectComment }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, 'Đóng', { duration: 4000 });
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
    const message = err?.error?.errors?.[0]?.errorMessage ?? 'Có lỗi xảy ra.';
    this.snackBar.open(message, 'Đóng', { duration: 4000 });
  }
}
