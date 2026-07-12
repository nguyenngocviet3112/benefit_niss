import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { GuiaConciliacaoService } from '../../services/guia-conciliacao.service';
import { GuiaListagem } from '../../response-models/guiaPagamento-response';
import { MovimentosBancariosData } from '../../models/movimentosBancarios';
import { GuiaComprovativoDataContract } from '../../response-models/guia-conciliacao-response';
import { base64ToArrayBuffer, blobToSaveAs } from '../../utils';

// Dominio 'INDPAGO'.Valor (não o Id) — Comprovativo em Validação = 3,
// Comprovativo Parcial em Validação = 4 (confirmado via SELECT * na Dominio,
// 2026-07-12). getGuiasAporoveByFilter projeta estadoPagamento = IndPagoNavigation.Valor.
const ESTADO_VALIDACAO = 3;
const ESTADO_VALIDACAO_PARCIAL = 4;

@Component({
  selector: 'app-guia-conciliacao',
  templateUrl: './guia-conciliacao.component.html',
  styleUrls: ['./guia-conciliacao.component.css']
})
export class GuiaConciliacaoComponent implements OnInit {

  public loading = false;
  public conciliando = false;

  public guias: GuiaListagem[] = [];
  public movimentos: MovimentosBancariosData[] = [];

  public selectedGuiaIds = new Set<number>();
  public selectedMovimentoIds = new Set<number>();

  // Xem chi tiết chứng từ (comprovativo) + số tự khai của 1 Guia — giúp officer
  // đối chiếu mắt thường với dòng sao kê ngân hàng trước khi chọn khớp (2026-07-13).
  public viewingGuia: GuiaComprovativoDataContract | null = null;
  public viewingPdfSrc: Uint8Array | null = null;
  public loadingComprovativo = false;

  constructor(
    private guiaConciliacaoService: GuiaConciliacaoService,
    private snackBar: MatSnackBar,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.load();
  }

  public load(): void {
    this.loading = true;
    this.selectedGuiaIds.clear();
    this.selectedMovimentoIds.clear();

    this.guiaConciliacaoService.getGuiasPendentesValidacao({ filter: { index: 0, rows: 500 } }).subscribe(
      response => {
        this.guias = (response.guias ?? []).filter(g => g.estadoPagamento === ESTADO_VALIDACAO || g.estadoPagamento === ESTADO_VALIDACAO_PARCIAL);
        this.loading = false;
      },
      err => { this.loading = false; this.showError(err); }
    );

    this.guiaConciliacaoService.getMovimentosBancariosDisponiveis({ tarefaAtivoId: 0, filter: { index: 0, rows: 500 } }).subscribe(
      response => {
        this.movimentos = (response.movimentos ?? []).filter(m => !m.conciliado);
      },
      err => this.showError(err)
    );
  }

  public toggleGuia(id: number): void {
    if (this.selectedGuiaIds.has(id)) { this.selectedGuiaIds.delete(id); } else { this.selectedGuiaIds.add(id); }
  }

  public toggleMovimento(id: number): void {
    if (this.selectedMovimentoIds.has(id)) { this.selectedMovimentoIds.delete(id); } else { this.selectedMovimentoIds.add(id); }
  }

  public get totalGuias(): number {
    return this.guias.filter(g => this.selectedGuiaIds.has(g.idGuia)).reduce((sum, g) => sum + (g.valorPago ?? 0), 0);
  }

  public get totalMovimentos(): number {
    return this.movimentos.filter(m => this.selectedMovimentoIds.has(m.id)).reduce((sum, m) => sum + this.valorMovimento(m), 0);
  }

  public valorMovimento(m: MovimentosBancariosData): number {
    return m.credito ?? m.debito ?? 0;
  }

  public get valoresIguais(): boolean {
    return this.selectedGuiaIds.size > 0 && this.selectedMovimentoIds.size > 0 &&
      Math.round((this.totalGuias - this.totalMovimentos) * 100) === 0;
  }

  public get selecaoValida(): boolean {
    // Regra igual à ConciliarMovimentos legado: 1 movimento bancário : N guias, OU N movimentos : 1 guia
    return this.valoresIguais && (this.selectedMovimentoIds.size === 1 || this.selectedGuiaIds.size === 1);
  }

  public confirmar(): void {
    if (!this.selecaoValida) { return; }
    if (!confirm(this.translate.instant('guiaConciliacao.confirmMatch'))) { return; }

    this.conciliando = true;
    this.guiaConciliacaoService.conciliarGuiaPagamento({
      guiaIds: Array.from(this.selectedGuiaIds),
      movimentosBancarios: Array.from(this.selectedMovimentoIds)
    }).subscribe(
      response => {
        this.conciliando = false;
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 5000 });
          return;
        }
        this.snackBar.open(this.translate.instant('guiaConciliacao.matchSuccess'), this.translate.instant('general.close'), { duration: 3000 });
        this.load();
      },
      err => { this.conciliando = false; this.showError(err); }
    );
  }

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('guiaConciliacao.errGeneric');
    this.snackBar.open(message, this.translate.instant('general.close'), { duration: 5000 });
  }

  public viewComprovativo(id: number, event: Event): void {
    event.stopPropagation();
    this.loadingComprovativo = true;
    this.viewingGuia = null;
    this.viewingPdfSrc = null;
    this.guiaConciliacaoService.getComprovativo(id).subscribe(
      response => {
        this.loadingComprovativo = false;
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 5000 });
          return;
        }
        this.viewingGuia = response.guia;
        if (response.guia.comprovativoPag) {
          this.viewingPdfSrc = new Uint8Array(base64ToArrayBuffer(response.guia.comprovativoPag));
        }
      },
      err => { this.loadingComprovativo = false; this.showError(err); }
    );
  }

  public closeComprovativo(): void {
    this.viewingGuia = null;
    this.viewingPdfSrc = null;
  }

  public downloadComprovativo(): void {
    if (!this.viewingGuia?.comprovativoPag) { return; }
    blobToSaveAs(this.viewingGuia.comprovativoPag, this.viewingGuia.numDocumento || 'comprovativo');
  }
}
