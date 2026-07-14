import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PageEvent } from '@angular/material/paginator';
import { TranslateService } from '@ngx-translate/core';
import { GuiaConciliacaoService } from '../../services/guia-conciliacao.service';
import { ConfirmDialogService } from '../../services/confirm-dialog.service';
import { GuiaListagem } from '../../response-models/guiaPagamento-response';
import { BankStatementLineDataContract } from '../../response-models/bank-statement-line-response';
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

  public loadingGuias = false;
  public loadingMovimentos = false;
  public conciliando = false;

  public guias: GuiaListagem[] = [];
  public movimentos: BankStatementLineDataContract[] = [];

  public selectedGuiaIds = new Set<number>();
  public selectedMovimentoIds = new Set<number>();

  // Xem chi tiết chứng từ (comprovativo) + số tự khai của 1 Guia — giúp officer
  // đối chiếu mắt thường với dòng sao kê ngân hàng trước khi chọn khớp (2026-07-13).
  public viewingGuia: GuiaComprovativoDataContract | null = null;
  public viewingPdfSrc: Uint8Array | null = null;
  public loadingComprovativo = false;

  // Lọc theo khoảng ngày — cả 2 danh sách (Guia chờ xác nhận + sao kê ngân hàng
  // khả dụng) đều tích lũy dần theo thời gian nên cần lọc để tránh màn nặng dần
  // (2026-07-13, user yêu cầu). Mỗi bên có filter riêng, áp dụng độc lập — lọc bên
  // Guias không được reload/reset bên Movimentos và ngược lại (2026-07-13, user
  // yêu cầu rõ). Mặc định 3 tháng gần nhất mỗi bên.
  public guiaDateFrom: Date | null = null;
  public guiaDateTo: Date | null = null;
  public movDateFrom: Date | null = null;
  public movDateTo: Date | null = null;

  // Lọc theo ngân hàng — mỗi bên 1 dropdown riêng, mặc định "Tất cả" (2026-07-14,
  // user yêu cầu). Danh sách option tính động từ dữ liệu đang tải, không hardcode —
  // 2 bên dùng 2 khái niệm "ngân hàng" khác nhau: Guia.bankCode là mã doanh nghiệp
  // tự khai khi tạo Guia (đối chiếu qua guiaPagamentoListagem.lstBankCode), còn
  // BankStatementLine.entidadeBancaria là Contabancaria.EntidadeBancaria (cấu hình
  // tài khoản ngân hàng thật của INSS) — không dùng chung 1 danh sách được.
  public selectedBankGuia: string | null = null;
  public selectedBankMovimento: string | null = null;

  // Phân trang phía client — mỗi panel 1 paginator riêng (2026-07-14, user yêu
  // cầu: danh sách tích lũy theo thời gian, render hết 1 lần sẽ chậm dần).
  public pageIndexGuias = 0;
  public pageSizeGuias = 20;
  public pageIndexMovimentos = 0;
  public pageSizeMovimentos = 20;

  // Đến từ màn "Receitas GP" (nút "Thực hiện đối chiếu") — tick sẵn đúng Guia này
  // ngay khi danh sách chờ xác nhận tải xong lần đầu (2026-07-14, user yêu cầu).
  private focusGuiaId: number | null = null;

  constructor(
    private guiaConciliacaoService: GuiaConciliacaoService,
    private snackBar: MatSnackBar,
    private translate: TranslateService,
    private confirmDialog: ConfirmDialogService,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    const today = new Date();
    let defaultFrom = new Date(today.getFullYear(), today.getMonth() - 2, 1);
    let defaultTo = new Date(today.getFullYear(), today.getMonth() + 1, 0);

    const guiaIdParam = this.route.snapshot.queryParamMap.get('guiaId');
    const mesAnoParam = this.route.snapshot.queryParamMap.get('mesAno');
    if (guiaIdParam) {
      this.focusGuiaId = Number(guiaIdParam);
      // Mở rộng khoảng lọc để chắc chắn bao trùm tháng phát sinh của Guia này —
      // mặc định 3 tháng gần nhất có thể bỏ sót Guia cũ hơn.
      if (mesAnoParam) {
        const mesAno = new Date(mesAnoParam);
        const fromCandidate = new Date(mesAno.getFullYear(), mesAno.getMonth() - 1, 1);
        const toCandidate = new Date(mesAno.getFullYear(), mesAno.getMonth() + 2, 0);
        if (fromCandidate < defaultFrom) { defaultFrom = fromCandidate; }
        if (toCandidate > defaultTo) { defaultTo = toCandidate; }
      }
    }

    this.guiaDateFrom = defaultFrom;
    this.guiaDateTo = defaultTo;
    this.movDateFrom = new Date(defaultFrom);
    this.movDateTo = new Date(defaultTo);
    this.load();
  }

  public load(): void {
    this.loadGuias();
    this.loadMovimentos();
  }

  public loadGuias(): void {
    this.loadingGuias = true;
    this.selectedGuiaIds.clear();
    this.pageIndexGuias = 0;

    this.guiaConciliacaoService.getGuiasPendentesValidacao({
      filter: { index: 0, rows: 500, dateFilterBegin: this.guiaDateFrom ?? undefined, dateFilterEnd: this.guiaDateTo ?? undefined }
    }).subscribe(
      response => {
        this.guias = (response.guias ?? []).filter(g => g.estadoPagamento === ESTADO_VALIDACAO || g.estadoPagamento === ESTADO_VALIDACAO_PARCIAL);
        this.loadingGuias = false;
        if (this.focusGuiaId != null) {
          if (this.guias.some(g => g.idGuia === this.focusGuiaId)) {
            this.selectedGuiaIds.add(this.focusGuiaId);
          } else {
            this.snackBar.open(this.translate.instant('guiaConciliacao.focusGuiaNotFound'), this.translate.instant('general.close'), { duration: 5000 });
          }
          this.focusGuiaId = null;
        }
      },
      err => { this.loadingGuias = false; this.showError(err); }
    );
  }

  public loadMovimentos(): void {
    this.loadingMovimentos = true;
    this.selectedMovimentoIds.clear();
    this.pageIndexMovimentos = 0;

    // 2026-07-13: đọc từ BankStatementLine (mode mới, cùng bảng với Conciliação
    // de Movimentos) thay vì Movimentosbancarios (bảng cũ) — xem memory
    // bank-statement-line-guia-pagamento-unification. Backend đã lọc sẵn
    // "chưa khớp Guia/Receita/Pagamento nào" + Credito > 0.
    this.guiaConciliacaoService.getLinhasDisponiveis({
      filter: { index: 0, rows: 500, dateFilterBegin: this.movDateFrom ?? undefined, dateFilterEnd: this.movDateTo ?? undefined }
    }).subscribe(
      response => {
        this.movimentos = response.items ?? [];
        this.loadingMovimentos = false;
      },
      err => { this.loadingMovimentos = false; this.showError(err); }
    );
  }

  public applyFilterGuias(): void {
    this.loadGuias();
  }

  public clearFilterGuias(): void {
    this.guiaDateFrom = null;
    this.guiaDateTo = null;
    this.loadGuias();
  }

  public applyFilterMovimentos(): void {
    this.loadMovimentos();
  }

  public clearFilterMovimentos(): void {
    this.movDateFrom = null;
    this.movDateTo = null;
    this.loadMovimentos();
  }

  public get bankOptionsGuias(): string[] {
    return Array.from(new Set(this.guias.map(g => g.bankCode).filter(b => !!b))).sort();
  }

  public get bankOptionsMovimentos(): string[] {
    return Array.from(new Set(this.movimentos.map(m => m.entidadeBancaria).filter((b): b is string => !!b))).sort();
  }

  public get filteredGuias(): GuiaListagem[] {
    return this.selectedBankGuia ? this.guias.filter(g => g.bankCode === this.selectedBankGuia) : this.guias;
  }

  public get filteredMovimentos(): BankStatementLineDataContract[] {
    return this.selectedBankMovimento ? this.movimentos.filter(m => m.entidadeBancaria === this.selectedBankMovimento) : this.movimentos;
  }

  public get pagedGuias(): GuiaListagem[] {
    const start = this.pageIndexGuias * this.pageSizeGuias;
    return this.filteredGuias.slice(start, start + this.pageSizeGuias);
  }

  public get pagedMovimentos(): BankStatementLineDataContract[] {
    const start = this.pageIndexMovimentos * this.pageSizeMovimentos;
    return this.filteredMovimentos.slice(start, start + this.pageSizeMovimentos);
  }

  public onBankFilterGuiaChange(): void {
    this.pageIndexGuias = 0;
  }

  public onBankFilterMovimentoChange(): void {
    this.pageIndexMovimentos = 0;
  }

  public onPageChangeGuias(event: PageEvent): void {
    this.pageIndexGuias = event.pageIndex;
    this.pageSizeGuias = event.pageSize;
  }

  public onPageChangeMovimentos(event: PageEvent): void {
    this.pageIndexMovimentos = event.pageIndex;
    this.pageSizeMovimentos = event.pageSize;
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

  public valorMovimento(m: BankStatementLineDataContract): number {
    return m.credito || m.debito || 0;
  }

  public get valoresIguais(): boolean {
    return this.selectedGuiaIds.size > 0 && this.selectedMovimentoIds.size > 0 &&
      Math.round((this.totalGuias - this.totalMovimentos) * 100) === 0;
  }

  public get selecaoValida(): boolean {
    // Regra igual à ConciliarMovimentos legado: 1 movimento bancário : N guias, OU N movimentos : 1 guia
    return this.valoresIguais && (this.selectedMovimentoIds.size === 1 || this.selectedGuiaIds.size === 1);
  }

  // Nút "Xác nhận" chỉ disable âm thầm — không giải thích lý do, người dùng tưởng
  // bấm không có phản hồi gì (2026-07-13, user report). Hiển thị lý do cụ thể.
  public get disabledReason(): string | null {
    if (this.selectedGuiaIds.size === 0 || this.selectedMovimentoIds.size === 0) { return null; }
    if (this.selectedGuiaIds.size > 1 && this.selectedMovimentoIds.size > 1) {
      return 'guiaConciliacao.errMultiMulti';
    }
    if (!this.valoresIguais) {
      return 'guiaConciliacao.errValoresDiferentes';
    }
    return null;
  }

  // window.confirm() nguyên bản trình duyệt bị Chrome (và nhiều trình duyệt
  // khác) tự động chặn im lặng sau vài lần gọi liên tiếp trên cùng 1 trang —
  // sau khi bị chặn, confirm() luôn trả về false ngay lập tức, không hiện
  // popup, không báo lỗi gì, khiến nút "Xác nhận" trông như bị vô hiệu hoàn
  // toàn (2026-07-13, user report). Dùng ConfirmDialogService (MatDialog)
  // thay thế để không bao giờ bị trình duyệt chặn kiểu đó.
  public confirmar(): void {
    if (!this.selecaoValida) { return; }

    this.confirmDialog.confirm(this.translate.instant('guiaConciliacao.confirmMatch')).subscribe(confirmed => {
      if (!confirmed) { return; }

      this.conciliando = true;
      this.guiaConciliacaoService.conciliarGuiaPagamento({
        guiaIds: Array.from(this.selectedGuiaIds),
        bankStatementLineIds: Array.from(this.selectedMovimentoIds)
      }).subscribe(
        response => {
          this.conciliando = false;
          if (response.errors && response.errors.length > 0) {
            this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 5000 });
            return;
          }
          this.showSuccessWithWarnings(this.translate.instant('guiaConciliacao.matchSuccess'), response.warnings);
          this.load();
        },
        err => { this.conciliando = false; this.showError(err); }
      );
    });
  }

  // Bút toán tự sinh (hoặc bị bỏ qua vì thiếu cấu hình) — luôn thông báo, để
  // kế toán biết mà kiểm tra/cấu hình lại nếu cần (2026-07-13, user yêu cầu).
  private showSuccessWithWarnings(baseMessage: string, warnings: string[] | undefined): void {
    const hasWarnings = warnings && warnings.length > 0;
    const message = hasWarnings ? `${baseMessage} ${(warnings ?? []).join(' ')}` : baseMessage;
    this.snackBar.open(message, this.translate.instant('general.close'), { duration: hasWarnings ? 12000 : 3000 });
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
