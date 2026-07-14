import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { BankStatementLineService } from '../../services/bank-statement-line.service';
import { ConfirmDialogService } from '../../services/confirm-dialog.service';
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
  // null = "Tất cả ngân hàng" (mặc định, 2026-07-13 theo yêu cầu user) — trước
  // đây tự chọn ngân hàng đầu tiên trong danh sách, gây thiếu sót nếu có giao
  // dịch ở nhiều ngân hàng khác nhau cùng lúc.
  public contaBancariaFk: number | null = null;
  public dataInicio: Date | null = null;
  public dataFim: Date | null = null;

  public lines: BankStatementLineDataContract[] = [];
  public expandedId: number | null = null;
  // Lọc theo trạng thái đối chiếu — lọc phía client vì dữ liệu đã tải hết
  // theo Ano/Ngân hàng/Ngày rồi, không cần thêm tham số gọi lại backend.
  // Mặc định "Chưa đối chiếu" (2026-07-13, user yêu cầu) — trước đây mặc định
  // "ALL" khiến các dòng đã đối chiếu lẫn vào danh sách cần xử lý.
  public statusFilter: 'ALL' | 'CONCILIADO' | 'NAO_CONCILIADO' = 'NAO_CONCILIADO';
  // Lọc theo chiều Nợ/Có — cũng lọc phía client, cùng lý do trên. Mặc định chỉ
  // Credit (tiền về) vì đây là danh sách "cần đối chiếu", tiền ra (Debit) ít
  // liên quan tới nghiệp vụ đối chiếu Receita hàng ngày — vẫn giữ lựa chọn
  // "Tất cả"/"Debit" để không mất khả năng xem toàn bộ sổ phụ khi cần
  // (2026-07-13, user yêu cầu, theo house rule CLAUDE.md §3 — filter luôn có "Tất cả").
  public directionFilter: 'ALL' | 'CREDITO' | 'DEBITO' = 'CREDITO';

  public showAddForm = false;
  public formContaBancariaFk: number | null = null;
  public formDataValor = new Date().toISOString().substring(0, 10);
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
    private snackBar: MatSnackBar,
    private translate: TranslateService,
    private router: Router,
    private confirmDialog: ConfirmDialogService
  ) { }

  ngOnInit(): void {
    this.bankAccountService.getAll().subscribe(
      response => this.contas = response.items ?? [],
      err => this.showError(err)
    );
    this.load();
  }

  public load(): void {
    this.loading = true;
    const dataInicioStr = this.dataInicio ? this.dataInicio.toISOString().substring(0, 10) : undefined;
    const dataFimStr = this.dataFim ? this.dataFim.toISOString().substring(0, 10) : undefined;
    this.bankStatementLineService.getByContaBancaria(this.contaBancariaFk, dataInicioStr, dataFimStr).subscribe(
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

  public get filteredLines(): BankStatementLineDataContract[] {
    let result = this.lines;
    if (this.statusFilter === 'CONCILIADO') {
      result = result.filter(l => l.isConciliado);
    } else if (this.statusFilter === 'NAO_CONCILIADO') {
      result = result.filter(l => !l.isConciliado);
    }
    if (this.directionFilter === 'CREDITO') {
      result = result.filter(l => l.credito > 0);
    } else if (this.directionFilter === 'DEBITO') {
      result = result.filter(l => l.debito > 0);
    }
    return result;
  }

  public openAddForm(): void {
    this.formContaBancariaFk = this.contaBancariaFk ?? (this.contas.length > 0 ? this.contas[0].id : null);
    this.formDataValor = new Date().toISOString().substring(0, 10);
    this.formCodigoTransacaoBancaria = '';
    this.formDescricao = '';
    this.formCredito = null;
    this.formDebito = null;
    this.showAddForm = true;
  }

  public cancelAddForm(): void {
    this.showAddForm = false;
  }

  public onFormCreditoChange(): void {
    if (this.formCredito && this.formCredito > 0) {
      this.formDebito = null;
    }
  }

  public onFormDebitoChange(): void {
    if (this.formDebito && this.formDebito > 0) {
      this.formCredito = null;
    }
  }

  public addLine(): void {
    if (!this.formContaBancariaFk) {
      this.snackBar.open(this.translate.instant('conciliacao.errMissingBank'), this.translate.instant('general.close'), { duration: 3000 });
      return;
    }
    const hasCredito = !!this.formCredito && this.formCredito > 0;
    const hasDebito = !!this.formDebito && this.formDebito > 0;
    if (!hasCredito && !hasDebito) {
      this.snackBar.open(this.translate.instant('conciliacao.errMissingValue'), this.translate.instant('general.close'), { duration: 3000 });
      return;
    }
    if (hasCredito && hasDebito) {
      this.snackBar.open(this.translate.instant('conciliacao.errBothValues'), this.translate.instant('general.close'), { duration: 3000 });
      return;
    }

    this.bankStatementLineService.addLine({
      contaBancariaFk: this.formContaBancariaFk,
      dataValor: this.formDataValor,
      codigoTransacaoBancaria: this.formCodigoTransacaoBancaria || undefined,
      descricao: this.formDescricao || undefined,
      credito: this.formCredito ?? 0,
      debito: this.formDebito ?? 0
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.showAddForm = false;
        this.load();
      },
      err => this.showError(err)
    );
  }

  public deleteLine(line: BankStatementLineDataContract): void {
    this.confirmDialog.confirm(this.translate.instant('conciliacao.confirmDeleteLine', { descricao: line.descricao || line.dataValor })).subscribe(confirmed => {
      if (!confirmed) { return; }
      this.bankStatementLineService.deleteLine({ id: line.id }).subscribe(
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
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.matchTarget = null;
        this.matchMode = null;
        this.showSuccessWithWarnings(this.translate.instant('conciliacao.matchSuccess'), response.warnings);
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
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.matchTarget = null;
        this.matchMode = null;
        this.showSuccessWithWarnings(this.translate.instant('conciliacao.matchSuccess'), response.warnings);
        this.load();
      },
      err => this.showError(err)
    );
  }

  // Bút toán tự sinh (hoặc bị bỏ qua vì thiếu cấu hình) — luôn thông báo, để
  // kế toán biết mà kiểm tra/cấu hình lại nếu cần (2026-07-13, user yêu cầu).
  // Đối chiếu VẪN được ghi nhận dù thiếu cấu hình — cấu hình tài khoản chỉ
  // phục vụ tự sinh Lançamento, KHÔNG được chặn nghiệp vụ chính (quy tắc
  // chung chốt 2026-07-13, áp dụng cho mọi màn cấu hình tài khoản kế toán).
  private showSuccessWithWarnings(baseMessage: string, warnings: string[] | undefined): void {
    const hasWarnings = warnings && warnings.length > 0;
    const message = hasWarnings ? `${baseMessage} ${(warnings ?? []).join(' ')}` : baseMessage;
    const missingConfig = (warnings ?? []).some(w => w.includes('chưa cấu hình'));
    if (missingConfig) {
      const ref = this.snackBar.open(message, this.translate.instant('conciliacao.goToReceitaConfig'), { duration: 12000 });
      ref.onAction().subscribe(() => this.router.navigate(['/contabilidade/receita']));
      return;
    }
    this.snackBar.open(message, this.translate.instant('general.close'), { duration: hasWarnings ? 12000 : 3000 });
  }

  public unmatch(line: BankStatementLineDataContract): void {
    this.confirmDialog.confirm(this.translate.instant('conciliacao.confirmUnmatch')).subscribe(confirmed => {
      if (!confirmed) { return; }
      this.bankStatementLineService.unmatch({ id: line.id }).subscribe(
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

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('conciliacao.errGeneric');
    this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
  }

}
