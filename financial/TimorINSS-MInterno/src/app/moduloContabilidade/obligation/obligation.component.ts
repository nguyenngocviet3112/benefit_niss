import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ObligationService } from '../../services/obligation.service';
import { CompromissoComSaldoDataContract, ObligationDataContract } from '../../response-models/obligation-response';

const ESTADO_LABELS: { [key: string]: string } = {
  DRAFT: 'Nháp',
  PENDING_APPROVAL: 'Chờ phê duyệt',
  APPROVED: 'Đã duyệt'
};

const LIQUIDACAO_TIPO_LABELS: { [key: string]: string } = {
  SALARIOS: 'Salários',
  PENSOES: 'Pensões',
  SUBSIDIOS_IMEDIATOS: 'Subsídios Imediatos',
  DESPESAS_SEM_CONTRATO: 'Despesas sem contrato',
  OUTRAS_DESPESAS_CONTRATO: 'Outras despesas associadas a contrato'
};

const BENEFICIARIO_CATEGORIA_LABELS: { [key: string]: string } = {
  FORNECEDOR: 'Fornecedor',
  BENEFICIARIO: 'Beneficiário (prestações — usa danh sách)',
  CONTRIBUINTE_EE: 'Contribuinte (EE)',
  PESSOAL: 'Pessoal (salários — usa danh sách)',
  OUTRO: 'Outro'
};

// Categorias mà form gốc dùng danh sách nhiều người thụ hưởng đính kèm
// (ListaObrigação1/2) thay vì 1 khối beneficiário duy nhất trên form chính.
const CATEGORIAS_COM_LISTA = ['BENEFICIARIO', 'PESSOAL'];

@Component({
  selector: 'app-obligation',
  templateUrl: './obligation.component.html',
  styleUrls: ['./obligation.component.css']
})
export class ObligationComponent implements OnInit {

  public ano = 2026;
  public estadoLabels = ESTADO_LABELS;
  public liquidacaoTipoLabels = LIQUIDACAO_TIPO_LABELS;
  public beneficiarioCategoriaLabels = BENEFICIARIO_CATEGORIA_LABELS;
  public loading = false;
  public items: ObligationDataContract[] = [];
  public expandedId: number | null = null;

  public showCreateForm = false;
  public formDescritivo = '';
  public formMes = 1;
  public formLiquidacaoTipo = '';
  public formBeneficiarioNome = '';
  public formBeneficiarioNiss = '';
  public formBeneficiarioCategoria = '';
  public formBeneficiarioNomeConta = '';
  public formBeneficiarioNumeroConta = '';
  public formBeneficiarioIban = '';
  public formBeneficiarioSwift = '';
  public formBeneficiarioBanco = '';
  public formBeneficiarioMontanteAPagar: number | null = null;

  public compromissosComSaldo: CompromissoComSaldoDataContract[] = [];
  public addItemTarget: ObligationDataContract | null = null;
  public formCompromissoFk: number | null = null;
  public formValue: number | null = null;

  public addBeneficiaryTarget: ObligationDataContract | null = null;
  public formBenNiss = '';
  public formBenNomeContribuinte = '';
  public formBenNomeBeneficiario = '';
  public formBenNomeConta = '';
  public formBenNumeroConta = '';
  public formBenIban = '';
  public formBenSwift = '';
  public formBenBanco = '';
  public formBenSalarioIliquido: number | null = null;
  public formBenCotizacao4: number | null = null;
  public formBenImposto10: number | null = null;
  public formBenSalarioLiquido: number | null = null;
  public formBenOutrosSuplementos: number | null = null;
  public formBenMontanteAPagar: number | null = null;

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
    this.addBeneficiaryTarget = null;
  }

  public precisaLista(categoria: string): boolean {
    return CATEGORIAS_COM_LISTA.includes(categoria);
  }

  public openCreateForm(): void {
    this.formDescritivo = '';
    this.formMes = new Date().getMonth() + 1;
    this.formLiquidacaoTipo = '';
    this.formBeneficiarioNome = '';
    this.formBeneficiarioNiss = '';
    this.formBeneficiarioCategoria = '';
    this.formBeneficiarioNomeConta = '';
    this.formBeneficiarioNumeroConta = '';
    this.formBeneficiarioIban = '';
    this.formBeneficiarioSwift = '';
    this.formBeneficiarioBanco = '';
    this.formBeneficiarioMontanteAPagar = null;
    this.showCreateForm = true;
  }

  public cancelCreateForm(): void {
    this.showCreateForm = false;
  }

  public createObligation(): void {
    this.obligationService.create({
      descritivoObrigacao: this.formDescritivo,
      liquidacaoTipo: this.formLiquidacaoTipo || undefined,
      beneficiarioNome: this.formBeneficiarioNome || undefined,
      beneficiarioNiss: this.formBeneficiarioNiss || undefined,
      beneficiarioCategoria: this.formBeneficiarioCategoria || undefined,
      beneficiarioNomeConta: this.precisaLista(this.formBeneficiarioCategoria) ? undefined : (this.formBeneficiarioNomeConta || undefined),
      beneficiarioNumeroConta: this.precisaLista(this.formBeneficiarioCategoria) ? undefined : (this.formBeneficiarioNumeroConta || undefined),
      beneficiarioIban: this.precisaLista(this.formBeneficiarioCategoria) ? undefined : (this.formBeneficiarioIban || undefined),
      beneficiarioSwift: this.precisaLista(this.formBeneficiarioCategoria) ? undefined : (this.formBeneficiarioSwift || undefined),
      beneficiarioBanco: this.precisaLista(this.formBeneficiarioCategoria) ? undefined : (this.formBeneficiarioBanco || undefined),
      beneficiarioMontanteAPagar: this.precisaLista(this.formBeneficiarioCategoria) ? undefined : (this.formBeneficiarioMontanteAPagar ?? undefined),
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

  public openAddBeneficiary(item: ObligationDataContract): void {
    this.addBeneficiaryTarget = item;
    this.formBenNiss = '';
    this.formBenNomeContribuinte = '';
    this.formBenNomeBeneficiario = '';
    this.formBenNomeConta = '';
    this.formBenNumeroConta = '';
    this.formBenIban = '';
    this.formBenSwift = '';
    this.formBenBanco = '';
    this.formBenSalarioIliquido = null;
    this.formBenCotizacao4 = null;
    this.formBenImposto10 = null;
    this.formBenSalarioLiquido = null;
    this.formBenOutrosSuplementos = null;
    this.formBenMontanteAPagar = null;
  }

  public cancelAddBeneficiary(): void {
    this.addBeneficiaryTarget = null;
  }

  public confirmAddBeneficiary(): void {
    if (!this.addBeneficiaryTarget || !this.formBenMontanteAPagar) {
      this.snackBar.open('Vui lòng nhập Montante a pagar.', 'Đóng', { duration: 3000 });
      return;
    }

    this.obligationService.addBeneficiary({
      obligationFk: this.addBeneficiaryTarget.id,
      niss: this.formBenNiss || undefined,
      nomeContribuinte: this.formBenNomeContribuinte || undefined,
      nomeBeneficiario: this.formBenNomeBeneficiario || undefined,
      nomeConta: this.formBenNomeConta || undefined,
      numeroConta: this.formBenNumeroConta || undefined,
      iban: this.formBenIban || undefined,
      swift: this.formBenSwift || undefined,
      banco: this.formBenBanco || undefined,
      salarioIliquido: this.formBenSalarioIliquido ?? undefined,
      cotizacao4: this.formBenCotizacao4 ?? undefined,
      imposto10: this.formBenImposto10 ?? undefined,
      salarioLiquido: this.formBenSalarioLiquido ?? undefined,
      outrosSuplementos: this.formBenOutrosSuplementos ?? undefined,
      montanteAPagar: this.formBenMontanteAPagar
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, 'Đóng', { duration: 4000 });
          return;
        }
        this.addBeneficiaryTarget = null;
        this.load();
      },
      err => this.showError(err)
    );
  }

  public removeBeneficiary(id: number): void {
    this.obligationService.removeBeneficiary({ id }).subscribe(
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
