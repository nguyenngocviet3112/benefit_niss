import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PageEvent } from '@angular/material/paginator';
import { TranslateService } from '@ngx-translate/core';
import { LancamentoService } from '../../services/lancamento.service';
import { LancamentoDataContract } from '../../response-models/lancamento-response';

// Registo de Lançamentos — CHỈ XEM, không có nút Thêm/Sửa/Xóa. Bút toán được
// tự sinh bởi các nghiệp vụ khác (Pagamento thực hiện, Receita xác nhận...)
// đúng nguyên tắc đã chốt: kế toán không nhập tay 2 lần cùng 1 giao dịch
// (xem memory lancamentos-conciliacao-link-design).
@Component({
  selector: 'app-lancamentos',
  templateUrl: './lancamentos.component.html',
  styleUrls: ['./lancamentos.component.css']
})
export class LancamentosComponent implements OnInit {

  public ano: number | null = new Date().getFullYear();
  public mes: number | null = null;
  public origemTipo = '';
  public origemTipoOptions = [
    { value: '', label: 'lancamentos.origemTodos' },
    { value: 'PaymentExecution', label: 'lancamentos.origemPagamento' },
    { value: 'ReceitaPacCaixa', label: 'lancamentos.origemReceitaCaixa' },
    { value: 'ReceitaPacBanco', label: 'lancamentos.origemReceitaBanco' },
    { value: 'GuiaPagamento', label: 'lancamentos.origemGuiaPagamento' }
  ];

  public items: LancamentoDataContract[] = [];
  public pagedItems: LancamentoDataContract[] = [];
  public pageIndex = 0;
  public pageSize = 20;
  public loading = false;

  constructor(
    private lancamentoService: LancamentoService,
    private snackBar: MatSnackBar,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.load();
  }

  public load(): void {
    this.loading = true;
    this.lancamentoService.getList(this.ano ?? undefined, this.mes ?? undefined, this.origemTipo || undefined).subscribe(
      response => {
        this.items = response.items ?? [];
        this.pageIndex = 0;
        this.updatePagedItems();
        this.loading = false;
      },
      err => {
        this.loading = false;
        this.showError(err);
      }
    );
  }

  public onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.updatePagedItems();
  }

  private updatePagedItems(): void {
    const start = this.pageIndex * this.pageSize;
    this.pagedItems = this.items.slice(start, start + this.pageSize);
  }

  public get totalValor(): number {
    return this.items.reduce((sum, i) => sum + i.valor, 0);
  }

  public origemLabel(origemTipo: string): string {
    const opt = this.origemTipoOptions.find(o => o.value === origemTipo);
    return opt ? this.translate.instant(opt.label) : origemTipo;
  }

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('lancamentos.errGeneric');
    this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
  }
}
