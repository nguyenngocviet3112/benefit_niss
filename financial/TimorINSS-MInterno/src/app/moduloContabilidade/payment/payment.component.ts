import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { PaymentService } from '../../services/payment.service';
import { ConfirmDialogService } from '../../services/confirm-dialog.service';
import { PermissionService } from '../../services/permission.service';
import {
  CodigoContaOptionDataContract,
  ContaBancariaOptionDataContract,
  ObligacaoDisponivelParaPagamentoDataContract,
  PaymentAuthorizationDataContract
} from '../../response-models/payment-response';
import { AttachmentConfigService } from '../../services/attachment-config.service';
import { AttachmentConfigItem } from '../../response-models/attachment-config-response';
import { AttachmentItem } from '../../response-models/attachment-response';

const ESTADO_LABELS: { [key: string]: string } = {
  DRAFT: 'payment.estadoDraft',
  PENDING_APPROVAL: 'payment.estadoPendingApproval',
  APPROVED: 'payment.estadoApproved'
};

@Component({
  selector: 'app-payment',
  templateUrl: './payment.component.html',
  styleUrls: ['./payment.component.css']
})
export class PaymentComponent implements OnInit {

  public ano = 2026;
  public estadoLabels = ESTADO_LABELS;
  public loading = false;
  public allItems: PaymentAuthorizationDataContract[] = [];
  public items: PaymentAuthorizationDataContract[] = [];
  public expandedId: number | null = null;

  public estadoOptions: { value: string; label: string }[] = [
    { value: '', label: 'payment.estadoTodos' },
    { value: 'DRAFT', label: 'payment.estadoDraft' },
    { value: 'PENDING_APPROVAL', label: 'payment.estadoPendingApproval' },
    { value: 'APPROVED', label: 'payment.estadoApproved' }
  ];
  public selectedEstado = '';

  public showCreateForm = false;
  public obligacoesDisponiveis: ObligacaoDisponivelParaPagamentoDataContract[] = [];
  public codigoContaOptions: CodigoContaOptionDataContract[] = [];
  public formObligationFk: number | null = null;
  public formDescritivo = '';
  public formValorAutorizado: number | null = null;
  public formCodigoContaDebitoFk: number | null = null;
  public formCodigoContaCreditoFk: number | null = null;
  public formMes = 1;

  public contaBancariaOptions: ContaBancariaOptionDataContract[] = [];
  public executeTarget: PaymentAuthorizationDataContract | null = null;
  public formDataPagamento: string | null = null;
  public formContaBancariaFk: number | null = null;
  public formNumeroDocumento = '';
  public formObservacao = '';

  public showRejectPrompt = false;
  public rejectComment = '';
  public rejectId: number | null = null;

  // Ghi bù bút toán bị bỏ qua vì thiếu Tài khoản Nợ/Có (2026-07-13) — cấu
  // hình tài khoản không bao giờ chặn Approve/Execute, đây là đường hoàn
  // thiện sổ sách ngay tại màn này sau đó, không cần đi tìm màn khác.
  public completarTarget: { item: PaymentAuthorizationDataContract; origemTipo: string } | null = null;
  public formCompletarDebitoFk: number | null = null;
  public formCompletarCreditoFk: number | null = null;

  public attachmentConfig: AttachmentConfigItem | null = null;
  public paymentHasAttachment: { [id: number]: boolean } = {};

  constructor(
    private paymentService: PaymentService,
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
    this.paymentService.getCodigoContaOptions().subscribe(response => this.codigoContaOptions = response.items ?? []);
  }

  // See ad-cabimento.component.ts for the rationale. PAG_EXECUTE-only accounts
  // default to APPROVED (their actionable stage — pending execution).
  private computeDefaultEstado(): string {
    const hasSubmit = this.permissionService.hasPerm('PAG_SUBMIT');
    const hasApprove = this.permissionService.hasPerm('PAG_APPROVE');
    const hasExecute = this.permissionService.hasPerm('PAG_EXECUTE');
    if (hasSubmit || (hasApprove && hasExecute)) { return ''; }
    if (hasApprove) { return 'PENDING_APPROVAL'; }
    if (hasExecute) { return 'APPROVED'; }
    return '';
  }

  public onAttachmentsLoaded(paymentId: number, items: AttachmentItem[]): void {
    this.paymentHasAttachment[paymentId] = items.length > 0;
  }

  public load(): void {
    this.loading = true;
    this.paymentService.getByAno(this.ano).subscribe(
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

  public toggleExpand(item: PaymentAuthorizationDataContract): void {
    this.expandedId = this.expandedId === item.id ? null : item.id;
    this.executeTarget = null;
  }

  public openCreateForm(): void {
    this.formObligationFk = null;
    this.formDescritivo = '';
    this.formValorAutorizado = null;
    this.formCodigoContaDebitoFk = null;
    this.formCodigoContaCreditoFk = null;
    this.formMes = new Date().getMonth() + 1;
    this.showCreateForm = true;

    this.paymentService.getObligacoesDisponiveis(this.ano).subscribe(
      response => { this.obligacoesDisponiveis = response.items ?? []; },
      err => this.showError(err)
    );
    this.paymentService.getCodigoContaOptions().subscribe(
      response => { this.codigoContaOptions = response.items ?? []; },
      err => this.showError(err)
    );
  }

  public cancelCreateForm(): void {
    this.showCreateForm = false;
  }

  public onObligacaoSelected(): void {
    const obr = this.obligacoesDisponiveis.find(o => o.obligationId === this.formObligationFk);
    if (obr) {
      this.formValorAutorizado = obr.valorObrigacao;
    }
  }

  public createAuthorization(): void {
    if (!this.formObligationFk || !this.formValorAutorizado) {
      this.snackBar.open(this.translate.instant('payment.errMissingObligation'), this.translate.instant('general.close'), { duration: 3000 });
      return;
    }

    this.paymentService.create({
      obligationFk: this.formObligationFk,
      descritivo: this.formDescritivo,
      valorAutorizado: this.formValorAutorizado,
      codigoContaDebitoFk: this.formCodigoContaDebitoFk ?? undefined,
      codigoContaCreditoFk: this.formCodigoContaCreditoFk ?? undefined,
      mes: this.formMes,
      ano: this.ano
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.showCreateForm = false;
        this.snackBar.open(this.translate.instant('payment.createdSuccess', { numero: response.item.numero }), this.translate.instant('general.close'), { duration: 3000 });
        this.load();
      },
      err => this.showError(err)
    );
  }

  public submitAuthorization(item: PaymentAuthorizationDataContract): void {
    this.confirmDialog.confirm(this.translate.instant('payment.confirmSubmit', { numero: item.numero })).subscribe(confirmed => {
      if (!confirmed) { return; }
      this.paymentService.submit({ id: item.id }).subscribe(
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

  public approve(item: PaymentAuthorizationDataContract): void {
    this.paymentService.approve({ id: item.id, approve: true }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.showSuccessWithWarnings(this.translate.instant('payment.approveSuccess'), response.warnings);
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
    this.paymentService.approve({ id: this.rejectId, approve: false, comment: this.rejectComment }).subscribe(
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

  public openExecuteForm(item: PaymentAuthorizationDataContract): void {
    this.executeTarget = item;
    this.formDataPagamento = new Date().toISOString().substring(0, 10);
    this.formContaBancariaFk = null;
    this.formNumeroDocumento = '';
    this.formObservacao = '';

    this.paymentService.getContaBancariaOptions().subscribe(
      response => { this.contaBancariaOptions = response.items ?? []; },
      err => this.showError(err)
    );
  }

  public cancelExecuteForm(): void {
    this.executeTarget = null;
  }

  public confirmExecute(): void {
    if (!this.executeTarget || !this.formDataPagamento) {
      this.snackBar.open(this.translate.instant('payment.errMissingDataPagamento'), this.translate.instant('general.close'), { duration: 3000 });
      return;
    }

    this.paymentService.execute({
      paymentAuthorizationFk: this.executeTarget.id,
      dataPagamento: this.formDataPagamento,
      contaBancariaFk: this.formContaBancariaFk ?? undefined,
      numeroDocumento: this.formNumeroDocumento,
      observacao: this.formObservacao
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.executeTarget = null;
        this.showSuccessWithWarnings(this.translate.instant('payment.executedSuccess'), response.warnings);
        this.load();
      },
      err => this.showError(err)
    );
  }

  public openCompletarLancamento(item: PaymentAuthorizationDataContract, origemTipo: string): void {
    this.completarTarget = { item, origemTipo };
    this.formCompletarDebitoFk = null;
    this.formCompletarCreditoFk = null;
  }

  public cancelCompletarLancamento(): void {
    this.completarTarget = null;
  }

  public confirmCompletarLancamento(): void {
    if (!this.completarTarget || !this.formCompletarDebitoFk || !this.formCompletarCreditoFk) {
      this.snackBar.open(this.translate.instant('payment.errMissingCompletarConta'), this.translate.instant('general.close'), { duration: 3000 });
      return;
    }
    this.paymentService.completarLancamento({
      paymentAuthorizationFk: this.completarTarget.item.id,
      origemTipo: this.completarTarget.origemTipo,
      codigoContaDebitoFk: this.formCompletarDebitoFk,
      codigoContaCreditoFk: this.formCompletarCreditoFk
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.completarTarget = null;
        this.snackBar.open(this.translate.instant('payment.completarSuccess'), this.translate.instant('general.close'), { duration: 3000 });
        this.load();
      },
      err => this.showError(err)
    );
  }

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('payment.errGeneric');
    this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
  }

  // Bút toán tự sinh (hoặc bị bỏ qua vì thiếu cấu hình) — luôn thông báo, để
  // kế toán biết mà kiểm tra/cấu hình lại nếu cần (2026-07-13, user yêu cầu).
  private showSuccessWithWarnings(baseMessage: string, warnings: string[] | undefined): void {
    const hasWarnings = warnings && warnings.length > 0;
    const message = hasWarnings ? `${baseMessage} ${(warnings ?? []).join(' ')}` : baseMessage;
    this.snackBar.open(message, this.translate.instant('general.close'), { duration: hasWarnings ? 12000 : 3000 });
  }
}
