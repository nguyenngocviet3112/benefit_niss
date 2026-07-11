import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CompromissoDespesaService } from '../../services/compromisso-despesa.service';
import { CabimentoDisponivelParaCompromissoDataContract, CompromissoDespesaDataContract } from '../../response-models/compromisso-despesa-response';

const ESTADO_LABELS: { [key: string]: string } = {
  DRAFT: 'Nháp',
  PENDING_REVIEW: 'Chờ kiểm tra',
  PENDING_APPROVAL: 'Chờ phê duyệt',
  APPROVED: 'Đã duyệt'
};

const ASSUMIDO_COM_LABELS: { [key: string]: string } = {
  CONTRATO: 'Contrato',
  LISTA_BENEFICIARIOS: 'Listas Beneficiários (subsídios imediatos)',
  OBRIGACAO: 'Obrigação'
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

  public showRejectPrompt = false;
  public rejectComment = '';
  public rejectAction: { id: number; action: 'review' | 'approve' } | null = null;

  constructor(
    private compromissoDespesaService: CompromissoDespesaService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.load();
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
      this.snackBar.open('Vui lòng chọn Cabimento và nhập đủ Valor.', 'Đóng', { duration: 3000 });
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
          this.snackBar.open(response.errors[0].errorMessage, 'Đóng', { duration: 4000 });
          return;
        }
        this.showCabimentoPicker = false;
        this.pickedCabimento = null;
        this.snackBar.open(`Đã tạo Compromisso số ${response.item.numero}.`, 'Đóng', { duration: 3000 });
        this.load();
      },
      err => this.showError(err)
    );
  }

  public addPlurianualidade(item: CompromissoDespesaDataContract): void {
    if (!this.formPluriAno || !this.formPluriValor) {
      this.snackBar.open('Vui lòng nhập Ano và Valor.', 'Đóng', { duration: 3000 });
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
          this.snackBar.open(response.errors[0].errorMessage, 'Đóng', { duration: 4000 });
          return;
        }
        this.formPluriAno = null;
        this.formPluriValor = null;
        this.load();
      },
      err => this.showError(err)
    );
  }

  public submitCompromisso(item: CompromissoDespesaDataContract): void {
    if (!confirm(`Gửi duyệt Compromisso số ${item.numero}?`)) { return; }
    this.compromissoDespesaService.submit({ id: item.id }).subscribe(
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

  public reviewApprove(item: CompromissoDespesaDataContract): void {
    this.compromissoDespesaService.review({ id: item.id, approve: true }).subscribe(
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

  public finalApprove(item: CompromissoDespesaDataContract): void {
    this.compromissoDespesaService.approve({ id: item.id, approve: true }).subscribe(
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
