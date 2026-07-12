import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CodigoContaTreeService } from '../../services/codigo-conta-tree.service';
import { CodigoContaTreeItemDataContract } from '../../response-models/codigo-conta-tree-response';

interface CodigoContaRow extends CodigoContaTreeItemDataContract {
  children: CodigoContaRow[];
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

  public editingId: number | null = null;
  public formParentFk: number | undefined;
  public formCodigo = '';
  public formDesignacao = '';
  public showForm = false;

  constructor(
    private codigoContaTreeService: CodigoContaTreeService,
    private snackBar: MatSnackBar
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
    items.forEach(i => rowsById.set(i.id, { ...i, children: [] }));

    const roots: CodigoContaRow[] = [];
    rowsById.forEach(row => {
      if (row.parentFk && rowsById.has(row.parentFk)) {
        rowsById.get(row.parentFk)!.children.push(row);
      } else {
        roots.push(row);
      }
    });
    return roots;
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
      this.snackBar.open('Vui lòng nhập đủ Mã và Tên gọi.', 'Đóng', { duration: 3000 });
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
          this.snackBar.open(response.errors[0].errorMessage, 'Đóng', { duration: 4000 });
          return;
        }
        this.showForm = false;
        this.loadTree();
      },
      err => this.showError(err)
    );
  }

  public deactivate(item: CodigoContaTreeItemDataContract): void {
    if (!confirm(`Xoá "${item.codigo} - ${item.designacao}"?`)) {
      return;
    }

    this.codigoContaTreeService.deactivate({ id: item.id }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, 'Đóng', { duration: 4000 });
          return;
        }
        this.loadTree();
      },
      err => this.showError(err)
    );
  }

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? 'Có lỗi xảy ra.';
    this.snackBar.open(message, 'Đóng', { duration: 4000 });
  }
}
