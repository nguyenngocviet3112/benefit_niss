import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { CodigoContaOptionDataContract } from '../../../response-models/payment-response';
import { LiquidacaoContaConfigModel, LiquidacaoContaConfigService } from '../../../services/liquidacao-conta-config.service';

const CATEGORIA_LABELS: { [key: string]: string } = {
  FORNECEDOR: 'liquidacaoContaConfig.categoriaFornecedor',
  BENEFICIARIO: 'liquidacaoContaConfig.categoriaBeneficiario',
  CONTRIBUINTE_EE: 'liquidacaoContaConfig.categoriaContribuinteEe',
  PESSOAL: 'liquidacaoContaConfig.categoriaPessoal',
  OUTRO: 'liquidacaoContaConfig.categoriaOutro'
};

// Chỉ là VÍ DỤ GỢI Ý hiển thị mờ (placeholder) — không tự điền giá trị, khách
// vẫn phải tự chọn tài khoản thật trong Plano de Contas của mình. FORNECEDOR/
// PESSOAL/OUTRO lấy đúng tên tài khoản đã thấy trong sổ sách thật của khách
// hàng (FRSSVF.xlsm); BENEFICIARIO/CONTRIBUINTE_EE chưa có ví dụ xác nhận từ sổ
// thật nên chỉ gợi ý chung chung, không bịa tên tài khoản cụ thể.
const PLACEHOLDER_HINTS: { [key: string]: string } = {
  FORNECEDOR: 'liquidacaoContaConfig.placeholderFornecedor',
  BENEFICIARIO: 'liquidacaoContaConfig.placeholderBeneficiario',
  CONTRIBUINTE_EE: 'liquidacaoContaConfig.placeholderContribuinteEe',
  PESSOAL: 'liquidacaoContaConfig.placeholderPessoal',
  OUTRO: 'liquidacaoContaConfig.placeholderOutro'
};

// Gợi ý ĐIỀN SẴN (pre-fill) khi Categoria chưa có tài khoản — vẫn để user tự
// sửa/ghi đè, KHÔNG tự lưu (phải bấm Save mới ghi vào DB). Id cụ thể (không
// match theo tên) vì Plano de Contas có nhiều dòng trùng tên đang active (vd
// 2 dòng "Outros credores" khác mã đầy đủ) — match theo tên dễ chọn nhầm dòng.
// FORNECEDOR/PESSOAL/OUTRO: Id đã xác nhận đúng qua đối chiếu sổ sách thật
// (2026-07-13). BENEFICIARIO/CONTRIBUINTE_EE: chưa có ví dụ riêng trong sổ đã
// quét — gợi ý dùng chung "Outros credores" là lựa chọn tạm user tự chọn qua
// AskUserQuestion cùng ngày, không phải khớp sổ sách, có thể cần xem lại sau.
const SUGGESTED_ACCOUNT_ID: { [key: string]: number } = {
  FORNECEDOR: 75,       // 221 Fornecedores c/c
  PESSOAL: 91,          // 2382 Com o pessoal
  OUTRO: 183,           // 27892 Outros credores
  BENEFICIARIO: 183,    // tạm dùng chung Outros credores — chưa xác nhận riêng
  CONTRIBUINTE_EE: 183  // tạm dùng chung Outros credores — chưa xác nhận riêng
};

interface CategoriaRow {
  categoria: string;
  categoriaLabel: string;
  placeholderKey: string;
  current: LiquidacaoContaConfigModel | null;
  search: string;
  filtered: CodigoContaOptionDataContract[];
  selectedFk: number | null;
  isSuggested: boolean;
}

@Component({
  selector: 'app-liquidacao-conta-config',
  templateUrl: './liquidacao-conta-config.component.html',
  styleUrls: ['./liquidacao-conta-config.component.css']
})
export class LiquidacaoContaConfigComponent implements OnInit {

  public loading = false;
  public savingCategoria: string | null = null;

  public codigoContaOptions: CodigoContaOptionDataContract[] = [];
  public rows: CategoriaRow[] = [];

  constructor(
    private liquidacaoContaConfigService: LiquidacaoContaConfigService,
    private snackBar: MatSnackBar,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.load();
  }

  public load(): void {
    this.loading = true;
    // Phải tải xong codigoContaOptions rồi mới dựng rows (không gọi song song) —
    // nếu không, row.filtered bị gán = mảng options rỗng lúc đó (chưa tải xong)
    // và không tự cập nhật lại khi options tải xong sau, khiến ô autocomplete
    // trống cho tới khi user gõ (onSearchChange mới đọc lại codigoContaOptions
    // mới nhất).
    this.liquidacaoContaConfigService.getCodigoContaOptions().subscribe(
      response => {
        this.codigoContaOptions = response.items ?? [];
        this.loadRows();
      },
      err => { this.loading = false; this.showError(err); }
    );
  }

  private loadRows(): void {
    this.liquidacaoContaConfigService.getAll().subscribe(
      response => {
        this.rows = (response.items ?? []).map(item => {
          const row: CategoriaRow = {
            categoria: item.categoria,
            categoriaLabel: CATEGORIA_LABELS[item.categoria] ?? item.categoria,
            placeholderKey: PLACEHOLDER_HINTS[item.categoria] ?? '',
            current: item,
            search: item.codigoContaDesignacao ?? '',
            filtered: this.codigoContaOptions,
            selectedFk: item.codigoContaFk,
            isSuggested: false
          };
          this.applySuggestionIfUnset(row);
          return row;
        });
        this.loading = false;
      },
      err => { this.loading = false; this.showError(err); }
    );
  }

  private applySuggestionIfUnset(row: CategoriaRow): void {
    if (row.selectedFk) { return; }
    const suggestedId = SUGGESTED_ACCOUNT_ID[row.categoria];
    if (!suggestedId) { return; }
    const suggested = this.codigoContaOptions.find(c => c.id === suggestedId);
    if (!suggested) { return; }
    row.selectedFk = suggested.id;
    row.search = suggested.designacao;
    row.isSuggested = true;
  }

  private normalize(value: string): string {
    return (value || '').toLowerCase();
  }

  public onSearchChange(row: CategoriaRow): void {
    row.isSuggested = false;
    const term = this.normalize(row.search);
    row.filtered = this.codigoContaOptions.filter(c => this.normalize(c.designacao).includes(term));
  }

  public selectConta(row: CategoriaRow, c: CodigoContaOptionDataContract): void {
    row.selectedFk = c.id;
    row.search = c.designacao;
    row.isSuggested = false;
  }

  public save(row: CategoriaRow): void {
    if (!row.selectedFk) { return; }
    this.savingCategoria = row.categoria;
    this.liquidacaoContaConfigService.save({
      categoria: row.categoria,
      codigoContaFk: row.selectedFk
    }).subscribe(
      response => {
        this.savingCategoria = null;
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 5000 });
          return;
        }
        this.snackBar.open(this.translate.instant('liquidacaoContaConfig.savedSuccess'), this.translate.instant('general.close'), { duration: 3000 });
        this.load();
      },
      err => { this.savingCategoria = null; this.showError(err); }
    );
  }

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('liquidacaoContaConfig.errGeneric');
    this.snackBar.open(message, this.translate.instant('general.close'), { duration: 5000 });
  }
}
