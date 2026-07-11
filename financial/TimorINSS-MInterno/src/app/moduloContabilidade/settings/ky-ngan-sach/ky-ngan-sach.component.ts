import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PageEvent } from '@angular/material/paginator';
import { OrcamentoConfigService } from '../../../services/orcamento-config.service';
import { OrcamentoConfigDataContract } from '../../../response-models/orcamento-config-response';

@Component({
  selector: 'app-ky-ngan-sach',
  templateUrl: './ky-ngan-sach.component.html',
  styleUrls: ['./ky-ngan-sach.component.css']
})
export class KyNganSachComponent implements OnInit {

  public allItems: OrcamentoConfigDataContract[] = [];
  public pagedItems: OrcamentoConfigDataContract[] = [];
  public loading = false;

  public pageIndex = 0;
  public pageSize = 10;

  public editingId: number | null = null;
  public formAno: number = new Date().getFullYear() + 1;
  public formDataInicio = '';
  public formDataFim = '';
  public showForm = false;

  constructor(
    private orcamentoConfigService: OrcamentoConfigService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.load();
  }

  public load(): void {
    this.loading = true;
    this.orcamentoConfigService.getAll().subscribe(
      response => {
        this.allItems = response.items ?? [];
        this.updatePage();
        this.loading = false;
      },
      err => {
        this.loading = false;
        this.showError(err);
      }
    );
  }

  public onPage(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.updatePage();
  }

  private updatePage(): void {
    const start = this.pageIndex * this.pageSize;
    this.pagedItems = this.allItems.slice(start, start + this.pageSize);
  }

  public openAddForm(): void {
    this.editingId = null;
    this.formAno = new Date().getFullYear() + 1;
    this.formDataInicio = `${this.formAno}-01-01`;
    this.formDataFim = `${this.formAno}-12-31`;
    this.showForm = true;
  }

  public openEditForm(item: OrcamentoConfigDataContract): void {
    this.editingId = item.id;
    this.formAno = item.ano;
    this.formDataInicio = item.dataInicio.substring(0, 10);
    this.formDataFim = item.dataFim ? item.dataFim.substring(0, 10) : '';
    this.showForm = true;
  }

  public cancelForm(): void {
    this.showForm = false;
  }

  public save(): void {
    if (!this.formAno || !this.formDataInicio) {
      this.snackBar.open('Vui lòng nhập đủ Năm và Ngày bắt đầu.', 'Đóng', { duration: 3000 });
      return;
    }

    this.orcamentoConfigService.save({
      id: this.editingId ?? 0,
      ano: this.formAno,
      dataInicio: this.formDataInicio,
      dataFim: this.formDataFim || undefined
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, 'Đóng', { duration: 4000 });
          return;
        }
        this.showForm = false;
        this.load();
      },
      err => this.showError(err)
    );
  }

  public deactivate(item: OrcamentoConfigDataContract): void {
    if (!confirm(`Xoá kỳ ngân sách năm ${item.ano} (${item.tipo})?`)) {
      return;
    }

    this.orcamentoConfigService.deactivate({ id: item.id }).subscribe(
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
