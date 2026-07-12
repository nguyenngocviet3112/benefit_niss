import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PageEvent } from '@angular/material/paginator';
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
    { value: '', label: 'Tất cả' },
    { value: 'PaymentExecution', label: 'Pagamento' },
    { value: 'ReceitaPac', label: 'Receita' }
  ];

  public items: LancamentoDataContract[] = [];
  public pagedItems: LancamentoDataContract[] = [];
  public pageIndex = 0;
  public pageSize = 20;
  public loading = false;

  constructor(
    private lancamentoService: LancamentoService,
    private snackBar: MatSnackBar
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

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? 'Có lỗi xảy ra.';
    this.snackBar.open(message, 'Đóng', { duration: 4000 });
  }
}
