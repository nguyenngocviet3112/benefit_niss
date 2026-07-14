import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { CodigoContaTreeService } from '../../services/codigo-conta-tree.service';
import { ConfirmDialogService } from '../../services/confirm-dialog.service';
import { CodigoContaTreeItemDataContract } from '../../response-models/codigo-conta-tree-response';

interface CodigoContaRow extends CodigoContaTreeItemDataContract {
  children: CodigoContaRow[];
  fullCodigo: string;
}

// Plano de Contas — tái dùng bảng Codigoconta đã có sẵn (mode cũ:
// componente-despesa/receita/pop-up-executar-pagamentos đã dùng danh mục này
// làm dropdown). Đây chỉ là màn quản lý (CRUD) ở mode mới — dữ liệu và
// backend (CodigoContaTreeController) đã tồn tại từ trước, không phải mới.
@Component({
  selector: 'app-plano-contas',
  templateUrl: './plano-contas.component.html',
  styleUrls: ['./plano-contas.component.css']
})
export class PlanoContasComponent implements OnInit {

  public orcamentoConfigFk = 1;
  public flatItems: CodigoContaTreeItemDataContract[] = [];
  public tree: CodigoContaRow[] = [];
  public loading = false;

  public searchCodigo = '';

  public editingId: number | null = null;
  public formParentFk: number | undefined;
  public formCodigo = '';
  public formDesignacao = '';
  public showForm = false;

  constructor(
    private codigoContaTreeService: CodigoContaTreeService,
    private snackBar: MatSnackBar,
    private translate: TranslateService,
    private confirmDialog: ConfirmDialogService
  ) { }

  ngOnInit(): void {
    this.loadTree();
  }

  public loadTree(): void {
    this.loading = true;
    this.codigoContaTreeService.getTree(this.orcamentoConfigFk).subscribe(
      response => {
        this.flatItems = response.items ?? [];
        this.tree = this.buildTree(this.flatItems);
        this.loading = false;
      },
      err => {
        this.loading = false;
        this.showError(err);
      }
    );
  }

  private buildTree(items: CodigoContaTreeItemDataContract[]): CodigoContaRow[] {
    const rowsById = new Map<number, CodigoContaRow>();
    items.forEach(i => rowsById.set(i.id, { ...i, children: [], fullCodigo: i.codigo }));

    const roots: CodigoContaRow[] = [];
    rowsById.forEach(row => {
      if (row.parentFk && rowsById.has(row.parentFk)) {
        rowsById.get(row.parentFk)!.children.push(row);
      } else {
        roots.push(row);
      }
    });

    // Mã trong DB chỉ là 1 chữ số cục bộ theo từng cấp (vd "1" dưới "Caixa" và
    // "1" dưới "Depósitos à ordem" là 2 tài khoản KHÁC NHAU) — phải NỐI LIỀN
    // (không dấu chấm) từ gốc xuống mới ra đúng mã thật, đúng convention gốc
    // của khách (file Excel "Plano Contas": 1 → 11 → 111/118, 12 → 121 →
    // 1211, 122 → 1221/1222/... cho từng ngân hàng) — verify khớp 100% với
    // dữ liệu DB hiện tại khi nối theo cách này.
    const computeFullCodigo = (node: CodigoContaRow, parentFullCodigo: string): void => {
      node.fullCodigo = `${parentFullCodigo}${node.codigo}`;
      node.children.forEach(child => computeFullCodigo(child, node.fullCodigo));
    };
    roots.forEach(root => computeFullCodigo(root, ''));

    return roots;
  }

  // Khớp theo cả mã cục bộ (codigo) và mã ghép đầy đủ (fullCodigo) — người
  // dùng thường nhớ/gõ mã đầy đủ (vd "1211"), không phải mã cục bộ 1 chữ số.
  public get filteredTree(): CodigoContaRow[] {
    const term = this.searchCodigo.trim().toLowerCase();
    if (!term) {
      return this.tree;
    }
    return this.tree
      .map(node => this.filterNode(node, term))
      .filter((node): node is CodigoContaRow => node !== null);
  }

  private filterNode(node: CodigoContaRow, term: string): CodigoContaRow | null {
    const selfMatches = node.codigo.toLowerCase().includes(term) || node.fullCodigo.toLowerCase().includes(term)
      || (node.designacao || '').toLowerCase().includes(term);
    const filteredChildren = node.children
      .map(child => this.filterNode(child, term))
      .filter((child): child is CodigoContaRow => child !== null);
    if (selfMatches || filteredChildren.length > 0) {
      return { ...node, children: filteredChildren };
    }
    return null;
  }

  public openAddForm(parent?: CodigoContaTreeItemDataContract): void {
    this.editingId = null;
    this.formParentFk = parent?.id;
    this.formCodigo = '';
    this.formDesignacao = '';
    this.showForm = true;
  }

  public openEditForm(item: CodigoContaTreeItemDataContract): void {
    this.editingId = item.id;
    this.formParentFk = item.parentFk;
    this.formCodigo = item.codigo;
    this.formDesignacao = item.designacao;
    this.showForm = true;
  }

  public cancelForm(): void {
    this.showForm = false;
  }

  public save(): void {
    if (!this.formCodigo || !this.formDesignacao) {
      this.snackBar.open(this.translate.instant('planoContas.errMissingFields'), this.translate.instant('general.close'), { duration: 3000 });
      return;
    }

    this.codigoContaTreeService.save({
      id: this.editingId ?? 0,
      codigo: this.formCodigo,
      designacao: this.formDesignacao,
      parentFk: this.formParentFk,
      orcamentoConfigFk: this.orcamentoConfigFk
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.showForm = false;
        this.loadTree();
      },
      err => this.showError(err)
    );
  }

  public deactivate(item: CodigoContaTreeItemDataContract): void {
    this.confirmDialog.confirm(this.translate.instant('planoContas.confirmDelete', { codigo: item.codigo, designacao: item.designacao })).subscribe(confirmed => {
      if (!confirmed) { return; }

      this.codigoContaTreeService.deactivate({ id: item.id }).subscribe(
        response => {
          if (response.errors && response.errors.length > 0) {
            this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
            return;
          }
          this.loadTree();
        },
        err => this.showError(err)
      );
    });
  }

  // Save() ở backend luôn set IndActivo=true bất kể tạo mới hay sửa — dùng
  // lại đúng endpoint đó (gửi nguyên các field hiện có) để kích hoạt lại,
  // không cần thêm API riêng.
  public reactivate(item: CodigoContaTreeItemDataContract): void {
    this.confirmDialog.confirm(this.translate.instant('planoContas.confirmReactivate', { codigo: item.codigo, designacao: item.designacao })).subscribe(confirmed => {
      if (!confirmed) { return; }

      this.codigoContaTreeService.save({
        id: item.id,
        codigo: item.codigo,
        designacao: item.designacao,
        parentFk: item.parentFk,
        orcamentoConfigFk: this.orcamentoConfigFk
      }).subscribe(
        response => {
          if (response.errors && response.errors.length > 0) {
            this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
            return;
          }
          this.loadTree();
        },
        err => this.showError(err)
      );
    });
  }

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('planoContas.errGeneric');
    this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
  }
}
