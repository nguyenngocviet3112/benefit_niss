import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ProgramActivityService } from '../../services/program-activity.service';
import { ProgramActivityDataContract } from '../../response-models/program-activity-response';

interface ProgramActivityRow extends ProgramActivityDataContract {
  children: ProgramActivityRow[];
}

@Component({
  selector: 'app-estrutura-programatica',
  templateUrl: './estrutura-programatica.component.html',
  styleUrls: ['./estrutura-programatica.component.css']
})
export class EstruturaProgramaticaComponent implements OnInit {

  public orcamentoConfigFk = 1;
  public flatItems: ProgramActivityDataContract[] = [];
  public tree: ProgramActivityRow[] = [];
  public loading = false;

  public editingId: number | null = null;
  public formParentFk: number | undefined;
  public formNivel = 1;
  public formCodigo = '';
  public formDesignacao = '';
  public showForm = false;

  public copyTargetOrcamentoConfigFk: number | null = null;

  constructor(
    private programActivityService: ProgramActivityService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.loadTree();
  }

  public loadTree(): void {
    this.loading = true;
    this.programActivityService.getTree(this.orcamentoConfigFk).subscribe(
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

  private buildTree(items: ProgramActivityDataContract[]): ProgramActivityRow[] {
    const rowsById = new Map<number, ProgramActivityRow>();
    items.forEach(i => rowsById.set(i.id, { ...i, children: [] }));

    const roots: ProgramActivityRow[] = [];
    rowsById.forEach(row => {
      if (row.parentFk && rowsById.has(row.parentFk)) {
        rowsById.get(row.parentFk)!.children.push(row);
      } else {
        roots.push(row);
      }
    });
    return roots;
  }

  public openAddForm(parent?: ProgramActivityDataContract): void {
    this.editingId = null;
    this.formParentFk = parent?.id;
    this.formNivel = parent ? parent.nivel + 1 : 1;
    this.formCodigo = '';
    this.formDesignacao = '';
    this.showForm = true;
  }

  public openEditForm(item: ProgramActivityDataContract): void {
    this.editingId = item.id;
    this.formParentFk = item.parentFk;
    this.formNivel = item.nivel;
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

    this.programActivityService.save({
      id: this.editingId ?? 0,
      codigo: this.formCodigo,
      designacao: this.formDesignacao,
      nivel: this.formNivel,
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

  public deactivate(item: ProgramActivityDataContract): void {
    if (!confirm(`Vô hiệu hoá "${item.codigo} - ${item.designacao}"?`)) {
      return;
    }

    this.programActivityService.deactivate({ id: item.id }).subscribe(
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

  public copyFromCurrentYear(): void {
    if (!this.copyTargetOrcamentoConfigFk) {
      this.snackBar.open('Nhập Kỳ ngân sách đích trước khi sao chép.', 'Đóng', { duration: 3000 });
      return;
    }

    this.programActivityService.copyYear({
      sourceOrcamentoConfigFk: this.orcamentoConfigFk,
      targetOrcamentoConfigFk: this.copyTargetOrcamentoConfigFk
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, 'Đóng', { duration: 4000 });
          return;
        }
        this.snackBar.open('Đã sao chép cây sang kỳ ngân sách mới.', 'Đóng', { duration: 3000 });
      },
      err => this.showError(err)
    );
  }

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? 'Có lỗi xảy ra.';
    this.snackBar.open(message, 'Đóng', { duration: 4000 });
  }
}
