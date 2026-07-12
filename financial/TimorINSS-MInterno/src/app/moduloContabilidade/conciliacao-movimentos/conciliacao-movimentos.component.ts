import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BankStatementLineService } from '../../services/bank-statement-line.service';
import { BankAccountService, BankAccountModel } from '../../services/bank-account.service';
import {
  BankStatementLineDataContract,
  PagamentoDisponivelParaConciliacaoDataContract,
  ReceitaDisponivelParaConciliacaoDataContract
} from '../../response-models/bank-statement-line-response';

@Component({
  selector: 'app-conciliacao-movimentos',
  templateUrl: './conciliacao-movimentos.component.html',
  styleUrls: ['./conciliacao-movimentos.component.css']
})
export class ConciliacaoMovimentosComponent implements OnInit {

  public ano = 2026;
  public loading = false;

  public contas: BankAccountModel[] = [];
  public contaBancariaFk: number | null = null;

  public lines: BankStatementLineDataContract[] = [];
  public expandedId: number | null = null;

  public showAddForm = false;
  public formDataValor = new Date().toISOString().substring(0, 10);
  public formDataTransacao = new Date().toISOString().substring(0, 10);
  public formCodigoTransacaoBancaria = '';
  public formDescricao = '';
  public formCredito: number | null = null;
  public formDebito: number | null = null;

  public matchTarget: BankStatementLineDataContract | null = null;
  public matchMode: 'receita' | 'pagamento' | null = null;
  public receitasDisponiveis: ReceitaDisponivelParaConciliacaoDataContract[] = [];
  public pagamentosDisponiveis: PagamentoDisponivelParaConciliacaoDataContract[] = [];

  constructor(
    private bankStatementLineService: BankStatementLineService,
    private bankAccountService: BankAccountService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.bankAccountService.getAll().subscribe(
      response => {
        this.contas = response.items ?? [];
        if (this.contas.length > 0) {
          this.contaBancariaFk = this.contas[0].id;
          this.load();
        }
      },
      err => this.showError(err)
    );
  }

  public load(): void {
    if (!this.contaBancariaFk) { return; }
    this.loading = true;
    this.bankStatementLineService.getByContaBancaria(this.contaBancariaFk).subscribe(
      response => {
        this.lines = response.items ?? [];
        this.loading = false;
      },
      err => {
        this.loading = false;
        this.showError(err);
      }
    );
  }

  public toggleExpand(line: BankStatementLineDataContract): void {
    this.expandedId = this.expandedId === line.id ? null : line.id;
    this.matchTarget = null;
    this.matchMode = null;
  }

  public get totalCredito(): number {
    return this.lines.reduce((sum, l) => sum + l.credito, 0);
  }

  public get totalDebito(): number {
    return this.lines.reduce((sum, l) => sum + l.debito, 0);
  }

  public get totalNaoConciliado(): number {
    return this.lines.filter(l => !l.isConciliado).length;
  }

  public openAddForm(): void {
    this.formDataValor = new Date().toISOString().substring(0, 10);
    this.formDataTransacao = new Date().toISOString().substring(0, 10);
    this.formCodigoTransacaoBancaria = '';
    this.formDescricao = '';
    this.formCredito = null;
    this.formDebito = null;
    this.showAddForm = true;
  }

  public cancelAddForm(): void {
    this.showAddForm = false;
  }

  public addLine(): void {
    if (!this.contaBancariaFk) { return; }
    if ((!this.formCredito || this.formCredito <= 0) && (!this.formDebito || this.formDebito <= 0)) {
      this.snackBar.open('Vui lòng nhập Crédito hoặc Débito.', 'Đóng', { duration: 3000 });
      return;
    }

    this.bankStatementLineService.addLine({
      contaBancariaFk: this.contaBancariaFk,
      dataValor: this.formDataValor,
      dataTransacao: this.formDataTransacao || undefined,
      codigoTransacaoBancaria: this.formCodigoTransacaoBancaria || undefined,
      descricao: this.formDescricao || undefined,
      credito: this.formCredito ?? 0,
      debito: this.formDebito ?? 0
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, 'Đóng', { duration: 4000 });
          return;
        }
        this.showAddForm = false;
        this.load();
      },
      err => this.showError(err)
    );
  }

  public deleteLine(line: BankStatementLineDataContract): void {
    if (!confirm(`Xoá dòng sao kê "${line.descricao || line.dataValor}"?`)) { return; }
    this.bankStatementLineService.deleteLine({ id: line.id }).subscribe(
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

  public openMatchReceita(line: BankStatementLineDataContract): void {
    this.matchTarget = line;
    this.matchMode = 'receita';
    this.bankStatementLineService.getReceitasDisponiveis(this.ano).subscribe(
      response => this.receitasDisponiveis = response.items ?? [],
      err => this.showError(err)
    );
  }

  public openMatchPagamento(line: BankStatementLineDataContract): void {
    this.matchTarget = line;
    this.matchMode = 'pagamento';
    this.bankStatementLineService.getPagamentosDisponiveis().subscribe(
      response => this.pagamentosDisponiveis = response.items ?? [],
      err => this.showError(err)
    );
  }

  public cancelMatch(): void {
    this.matchTarget = null;
    this.matchMode = null;
  }

  public confirmMatchReceita(receita: ReceitaDisponivelParaConciliacaoDataContract): void {
    if (!this.matchTarget) { return; }
    this.bankStatementLineService.matchReceita({ id: this.matchTarget.id, receitaPacFk: receita.receitaPacId }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, 'Đóng', { duration: 4000 });
          return;
        }
        this.matchTarget = null;
        this.matchMode = null;
        this.load();
      },
      err => this.showError(err)
    );
  }

  public confirmMatchPagamento(pagamento: PagamentoDisponivelParaConciliacaoDataContract): void {
    if (!this.matchTarget) { return; }
    this.bankStatementLineService.matchPagamento({ id: this.matchTarget.id, paymentExecutionFk: pagamento.paymentExecutionId }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, 'Đóng', { duration: 4000 });
          return;
        }
        this.matchTarget = null;
        this.matchMode = null;
        this.load();
      },
      err => this.showError(err)
    );
  }

  public unmatch(line: BankStatementLineDataContract): void {
    if (!confirm('Hủy đối chiếu dòng này?')) { return; }
    this.bankStatementLineService.unmatch({ id: line.id }).subscribe(
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

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? 'Có lỗi xảy ra.';
    this.snackBar.open(message, 'Đóng', { duration: 4000 });
  }
}
