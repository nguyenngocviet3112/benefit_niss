import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PageEvent } from '@angular/material/paginator';
import { TranslateService } from '@ngx-translate/core';
import { GuiaConciliacaoService } from '../../services/guia-conciliacao.service';
import { ConfirmDialogService } from '../../services/confirm-dialog.service';
import { GuiaListagem } from '../../response-models/guiaPagamento-response';

// Dominio 'INDPAGO'.Valor — 1=Guia Paga, 2=Guia Gerada, 3=Comprovativo em Validação,
// 4=Comprovativo Parcial em Validação, 5=Guia Parcialmente Paga, 6=Rejeita.
const ESTADO_LABELS: { [key: number]: string } = {
  1: 'receitasGp.estadoGuiaPaga',
  2: 'receitasGp.estadoGuiaGerada',
  3: 'receitasGp.estadoComprovativoValidacao',
  4: 'receitasGp.estadoComprovativoParcial',
  5: 'receitasGp.estadoGuiaParcialPaga',
  6: 'receitasGp.estadoRejeita'
};

// Só Guia Paga/Parcialmente Paga passaram por ConciliarGuiaPagamento — só essas
// podem ser desfeitas (Guia Gerada/Rejeita nunca foram reconciliadas).
const RECONCILIAVEL_STATES = [1, 5];

// Comprovativo em Validação / Comprovativo Parcial em Validação — mesmos 2 estados
// que a tela V-B (Duyệt Guia Pagamento) já filtra como "pendentes".
const PENDENTE_STATES = [3, 4];

@Component({
  selector: 'app-receitas-gp',
  templateUrl: './receitas-gp.component.html',
  styleUrls: ['./receitas-gp.component.css']
})
export class ReceitasGpComponent implements OnInit {

  public loading = false;
  public estadoLabels = ESTADO_LABELS;

  public items: GuiaListagem[] = [];
  public pagedItems: GuiaListagem[] = [];
  public pageIndex = 0;
  public pageSize = 20;

  public years: number[] = [];
  public selectedYear = new Date().getFullYear();

  public entidadeSearch = '';
  public selectedEntidadeNome?: string;
  public filteredEntidades: string[] = [];
  private allEntidadeNomes: string[] = [];

  public estadoOptions: { value: number | null; label: string }[] = [
    { value: null, label: 'receitasGp.estadoTodos' },
    { value: 1, label: 'receitasGp.estadoGuiaPaga' },
    { value: 2, label: 'receitasGp.estadoGuiaGerada' },
    { value: 3, label: 'receitasGp.estadoComprovativoValidacao' },
    { value: 4, label: 'receitasGp.estadoComprovativoParcial' },
    { value: 5, label: 'receitasGp.estadoGuiaParcialPaga' },
    { value: 6, label: 'receitasGp.estadoRejeita' }
  ];
  public selectedEstado: number | null = null;

  private allItems: GuiaListagem[] = [];

  constructor(
    private guiaConciliacaoService: GuiaConciliacaoService,
    private snackBar: MatSnackBar,
    private translate: TranslateService,
    private confirmDialog: ConfirmDialogService,
    private router: Router
  ) { }

  ngOnInit(): void {
    const current = new Date().getFullYear();
    this.years = [current - 1, current, current + 1];
    this.entidadeSearch = this.translate.instant('receitasGp.bothOption');
    this.load();
  }

  public load(): void {
    this.loading = true;
    this.guiaConciliacaoService.getReceitasGpReport({ ano: this.selectedYear }).subscribe(
      response => {
        this.loading = false;
        this.allItems = response.guias ?? [];
        this.allEntidadeNomes = Array.from(new Set(this.allItems.map(i => i.userName).filter((n): n is string => !!n))).sort();
        this.filteredEntidades = this.allEntidadeNomes;
        this.applyFilters();
      },
      err => {
        this.loading = false;
        const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('receitasGp.errGeneric');
        this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
      }
    );
  }

  private normalize(value: string): string {
    return (value || '').toLowerCase();
  }

  public onEntidadeSearchChange(): void {
    const term = this.normalize(this.entidadeSearch);
    this.filteredEntidades = this.allEntidadeNomes.filter(n => this.normalize(n).includes(term));
  }

  public selectEntidade(nome?: string): void {
    this.selectedEntidadeNome = nome;
    this.entidadeSearch = nome ?? this.translate.instant('receitasGp.bothOption');
    this.applyFilters();
  }

  public onEstadoChange(): void {
    this.applyFilters();
  }

  private applyFilters(): void {
    this.items = this.allItems.filter(i =>
      (!this.selectedEntidadeNome || i.userName === this.selectedEntidadeNome) &&
      (this.selectedEstado == null || i.estadoPagamento === this.selectedEstado)
    );
    this.pageIndex = 0;
    this.updatePagedItems();
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

  public podeDesfazer(item: GuiaListagem): boolean {
    return RECONCILIAVEL_STATES.includes(item.estadoPagamento);
  }

  public podeConciliar(item: GuiaListagem): boolean {
    return PENDENTE_STATES.includes(item.estadoPagamento);
  }

  // Đưa officer sang thẳng màn Duyệt Guia Pagamento (V-B) để thực hiện đối chiếu —
  // truyền kèm guiaId + mesAno để màn đó tự mở rộng khoảng lọc ngày và tick sẵn
  // đúng Guia này (2026-07-14, user yêu cầu).
  public irParaConciliacao(item: GuiaListagem): void {
    this.router.navigate(['/contabilidade/receita/guiaConciliacao'], {
      queryParams: { guiaId: item.idGuia, mesAno: new Date(item.mesAno).toISOString() }
    });
  }

  // Ảnh hưởng khi hủy: Guia quay lại chờ duyệt, bút toán kế toán đã sinh (nếu có)
  // bị hủy theo — phải xác nhận rõ ràng trước khi thực hiện (2026-07-13, user yêu cầu).
  public undoConciliacao(item: GuiaListagem): void {
    this.confirmDialog.confirm(this.translate.instant('receitasGp.confirmUndo', { numDocumento: item.numDocumento })).subscribe(confirmed => {
      if (!confirmed) { return; }
      this.guiaConciliacaoService.undoConciliacao({ guiaId: item.idGuia }).subscribe(
        response => {
          if (response.errors && response.errors.length > 0) {
            this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 5000 });
            return;
          }
          this.snackBar.open(this.translate.instant('receitasGp.undoSuccess'), this.translate.instant('general.close'), { duration: 3000 });
          this.load();
        },
        err => {
          const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('receitasGp.errGeneric');
          this.snackBar.open(message, this.translate.instant('general.close'), { duration: 5000 });
        }
      );
    });
  }

  public get totalValor(): number {
    return this.items.reduce((sum, i) => sum + (i.valor ?? 0), 0);
  }
}
