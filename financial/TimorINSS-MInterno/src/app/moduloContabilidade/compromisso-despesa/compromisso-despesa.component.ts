import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { CompromissoDespesaService } from '../../services/compromisso-despesa.service';
import { CabimentoDisponivelParaCompromissoDataContract, CompromissoDespesaDataContract } from '../../response-models/compromisso-despesa-response';
import { AttachmentConfigService } from '../../services/attachment-config.service';
import { AttachmentConfigItem } from '../../response-models/attachment-config-response';
import { AttachmentItem } from '../../response-models/attachment-response';

const ESTADO_LABELS: { [key: string]: string } = {
  DRAFT: 'compromisso.estadoDraft',
  PENDING_REVIEW: 'compromisso.estadoPendingReview',
  PENDING_APPROVAL: 'compromisso.estadoPendingApproval',
  APPROVED: 'compromisso.estadoApproved'
};

const ASSUMIDO_COM_LABELS: { [key: string]: string } = {
  CONTRATO: 'compromisso.assumidoContrato',
  LISTA_BENEFICIARIOS: 'compromisso.assumidoListaBeneficiarios',
  OBRIGACAO: 'compromisso.assumidoObrigacao'
};

@Component({
  selector: 'app-compromisso-despesa',
  templateUrl: './compromisso-despesa.component.html',
  styleUrls: ['./compromisso-despesa.component.css']
})
export class CompromissoDespesaComponent implements OnInit {

  public ano = 2026;
  public estadoLabels = ESTADO_LABELS;
  public assumidoComLabels = ASSUMIDO_COM_LABELS;
  public loading = false;
  public items: CompromissoDespesaDataContract[] = [];
  public expandedId: number | null = null;

  public availableCabimentos: CabimentoDisponivelParaCompromissoDataContract[] = [];
  public showCabimentoPicker = false;
  public pickedCabimento: CabimentoDisponivelParaCompromissoDataContract | null = null;
  public formDescritivo = '';
  public formValorCompromissoGlobal: number | null = null;
  public formValorCompromissoAno: number | null = null;
  public formAssumidoCom: 'CONTRATO' | 'LISTA_BENEFICIARIOS' | 'OBRIGACAO' = 'CONTRATO';
  public formMes = 1;

  public formPluriAno: number | null = null;
  public formPluriValor: number | null = null;

  public showApprovePrompt = false;
  public approveComment = '';
  public approveAction: { id: number; action: 'review' | 'approve' } | null = null;

  public showRejectPrompt = false;
  public rejectComment = '';
  public rejectAction: { id: number; action: 'review' | 'approve' } | null = null;

  public editId: number | null = null;
  public editDescritivo = '';
  public editValorCompromissoGlobal: number | null = null;
  public editValorCompromissoAno: number | null = null;
  public editAssumidoCom: 'CONTRATO' | 'LISTA_BENEFICIARIOS' | 'OBRIGACAO' = 'CONTRATO';

  public attachmentConfig: AttachmentConfigItem | null = null;
  public compromissoHasAttachment: { [id: number]: boolean } = {};

  constructor(
    private compromissoDespesaService: CompromissoDespesaService,
    private attachmentConfigService: AttachmentConfigService,
    private snackBar: MatSnackBar,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.load();
    this.attachmentConfigService.getConfig().subscribe(response => this.attachmentConfig = response.item ?? null);
  }

  public onAttachmentsLoaded(compromissoId: number, items: AttachmentItem[]): void {
    this.compromissoHasAttachment[compromissoId] = items.length > 0;
  }

  public load(): void {
    this.loading = true;
    this.compromissoDespesaService.getByAno(this.ano).subscribe(
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

  public toggleExpand(item: CompromissoDespesaDataContract): void {
    this.expandedId = this.expandedId === item.id ? null : item.id;
  }

  public openCabimentoPicker(): void {
    this.compromissoDespesaService.getCabimentosDisponiveis(this.ano).subscribe(
      response => {
        this.availableCabimentos = response.items ?? [];
        this.showCabimentoPicker = true;
      },
      err => this.showError(err)
    );
  }

  public pickCabimento(c: CabimentoDisponivelParaCompromissoDataContract): void {
    this.pickedCabimento = c;
    this.formDescritivo = '';
    this.formValorCompromissoGlobal = c.valorCabimentado;
    this.formValorCompromissoAno = c.valorCabimentado;
    this.formAssumidoCom = 'CONTRATO';
    this.formMes = new Date().getMonth() + 1;
  }

  public cancelCabimentoPicker(): void {
    this.showCabimentoPicker = false;
    this.pickedCabimento = null;
  }

  public createCompromisso(): void {
    if (!this.pickedCabimento || !this.formValorCompromissoGlobal || !this.formValorCompromissoAno) {
      this.snackBar.open(this.translate.instant('compromisso.errMissingCabimento'), this.translate.instant('general.close'), { duration: 3000 });
      return;
    }

    this.compromissoDespesaService.create({
      cabimentoFk: this.pickedCabimento.cabimentoId,
      descritivo: this.formDescritivo,
      valorCompromissoGlobal: this.formValorCompromissoGlobal,
      valorCompromissoAno: this.formValorCompromissoAno,
      assumidoCom: this.formAssumidoCom,
      mes: this.formMes,
      ano: this.ano
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.showCabimentoPicker = false;
        this.pickedCabimento = null;
        this.snackBar.open(this.translate.instant('compromisso.createdSuccess', { numero: response.item.numero }), this.translate.instant('general.close'), { duration: 3000 });
        this.load();
      },
      err => this.showError(err)
    );
  }

  public addPlurianualidade(item: CompromissoDespesaDataContract): void {
    if (!this.formPluriAno || !this.formPluriValor) {
      this.snackBar.open(this.translate.instant('compromisso.errMissingPluri'), this.translate.instant('general.close'), { duration: 3000 });
      return;
    }
    this.compromissoDespesaService.savePlurianualidade({
      id: 0,
      compromissoDespesaFk: item.id,
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

  public openEdit(item: CompromissoDespesaDataContract): void {
    this.editId = item.id;
    this.editDescritivo = item.descritivo;
    this.editValorCompromissoGlobal = item.valorCompromissoGlobal;
    this.editValorCompromissoAno = item.valorCompromissoAno;
    this.editAssumidoCom = item.assumidoCom ?? 'CONTRATO';
  }

  public cancelEdit(): void {
    this.editId = null;
  }

  public saveEdit(): void {
    if (this.editId == null || !this.editValorCompromissoGlobal || !this.editValorCompromissoAno) { return; }
    this.compromissoDespesaService.save({
      id: this.editId,
      descritivo: this.editDescritivo,
      valorCompromissoGlobal: this.editValorCompromissoGlobal,
      valorCompromissoAno: this.editValorCompromissoAno,
      assumidoCom: this.editAssumidoCom
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

  public submitCompromisso(item: CompromissoDespesaDataContract): void {
    if (!confirm(this.translate.instant('compromisso.confirmSubmit', { numero: item.numero }))) { return; }
    this.compromissoDespesaService.submit({ id: item.id }).subscribe(
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

  public openApprovePrompt(id: number, action: 'review' | 'approve'): void {
    this.approveAction = { id, action };
    this.approveComment = '';
    this.showApprovePrompt = true;
  }

  public confirmApprove(): void {
    if (!this.approveAction) { return; }
    const { id, action } = this.approveAction;
    const call = action === 'review'
      ? this.compromissoDespesaService.review({ id, approve: true, comment: this.approveComment || undefined })
      : this.compromissoDespesaService.approve({ id, approve: true, comment: this.approveComment || undefined });

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

  public openRejectPrompt(id: number, action: 'review' | 'approve'): void {
    this.rejectAction = { id, action };
    this.rejectComment = '';
    this.showRejectPrompt = true;
  }

  public confirmReject(): void {
    if (!this.rejectAction) { return; }
    const { id, action } = this.rejectAction;
    const call = action === 'review'
      ? this.compromissoDespesaService.review({ id, approve: false, comment: this.rejectComment })
      : this.compromissoDespesaService.approve({ id, approve: false, comment: this.rejectComment });

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
    const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('compromisso.errGeneric');
    this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
  }
}
