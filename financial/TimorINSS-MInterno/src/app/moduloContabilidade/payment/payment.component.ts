import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { PaymentService } from '../../services/payment.service';
import {
  CodigoContaOptionDataContract,
  ContaBancariaOptionDataContract,
  ObligacaoDisponivelParaPagamentoDataContract,
  PaymentAuthorizationDataContract
} from '../../response-models/payment-response';

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
  public items: PaymentAuthorizationDataContract[] = [];
  public expandedId: number | null = null;

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

  constructor(
    private paymentService: PaymentService,
    private snackBar: MatSnackBar,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.load();
  }

  public load(): void {
    this.loading = true;
    this.paymentService.getByAno(this.ano).subscribe(
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
    if (!confirm(this.translate.instant('payment.confirmSubmit', { numero: item.numero }))) { return; }
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
  }

  public approve(item: PaymentAuthorizationDataContract): void {
    this.paymentService.approve({ id: item.id, approve: true }).subscribe(
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
        this.snackBar.open(this.translate.instant('payment.executedSuccess'), this.translate.instant('general.close'), { duration: 3000 });
        this.load();
      },
      err => this.showError(err)
    );
  }

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('payment.errGeneric');
    this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
  }
}
