import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { EconomicClassificationService } from '../../services/economic-classification.service';
import { ConfirmDialogService } from '../../services/confirm-dialog.service';
import { EconomicClassificationDataContract } from '../../response-models/economic-classification-response';

interface EconomicClassificationRow extends EconomicClassificationDataContract {
  children: EconomicClassificationRow[];
}

@Component({
  selector: 'app-classificacao-economica',
  templateUrl: './classificacao-economica.component.html',
  styleUrls: ['./classificacao-economica.component.css']
})
export class ClassificacaoEconomicaComponent implements OnInit {

  public orcamentoConfigFk = 1;
  public flatItems: EconomicClassificationDataContract[] = [];
  public tree: EconomicClassificationRow[] = [];
  public loading = false;

  public searchCodigo = '';

  public editingId: number | null = null;
  public formParentFk: number | undefined;
  public formNivel = 1;
  public formCodigo = '';
  public formDesignacao = '';
  public formTipo = '';
  public showForm = false;

  constructor(
    private economicClassificationService: EconomicClassificationService,
    private snackBar: MatSnackBar,
    private translate: TranslateService,
    private confirmDialog: ConfirmDialogService
  ) { }

  ngOnInit(): void {
    this.loadTree();
  }

  public loadTree(): void {
    this.loading = true;
    this.economicClassificationService.getTree(this.orcamentoConfigFk).subscribe(
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

  private buildTree(items: EconomicClassificationDataContract[]): EconomicClassificationRow[] {
    const rowsById = new Map<number, EconomicClassificationRow>();
    items.forEach(i => rowsById.set(i.id, { ...i, children: [] }));

    const roots: EconomicClassificationRow[] = [];
    rowsById.forEach(row => {
      if (row.parentFk && rowsById.has(row.parentFk)) {
        rowsById.get(row.parentFk)!.children.push(row);
      } else {
        roots.push(row);
      }
    });
    return roots;
  }

  // Lọc cây theo Mã (Código) gõ vào ô tìm — giữ lại đường dẫn (ancestor) của
  // các nút khớp để không mất ngữ cảnh phân cấp, không phải chỉ hiện đúng 1 dòng.
  public get filteredTree(): EconomicClassificationRow[] {
    const term = this.searchCodigo.trim().toLowerCase();
    if (!term) {
      return this.tree;
    }
    return this.tree
      .map(node => this.filterNode(node, term))
      .filter((node): node is EconomicClassificationRow => node !== null);
  }

  private filterNode(node: EconomicClassificationRow, term: string): EconomicClassificationRow | null {
    const selfMatches = node.codigo.toLowerCase().includes(term);
    const filteredChildren = node.children
      .map(child => this.filterNode(child, term))
      .filter((child): child is EconomicClassificationRow => child !== null);
    if (selfMatches || filteredChildren.length > 0) {
      return { ...node, children: filteredChildren };
    }
    return null;
  }

  public openAddForm(parent?: EconomicClassificationDataContract): void {
    this.editingId = null;
    this.formParentFk = parent?.id;
    this.formNivel = parent ? parent.nivel + 1 : 1;
    this.formCodigo = '';
    this.formDesignacao = '';
    this.formTipo = parent?.tipo ?? '';
    this.showForm = true;
  }

  public openEditForm(item: EconomicClassificationDataContract): void {
    this.editingId = item.id;
    this.formParentFk = item.parentFk;
    this.formNivel = item.nivel;
    this.formCodigo = item.codigo;
    this.formDesignacao = item.designacao;
    this.formTipo = item.tipo ?? '';
    this.showForm = true;
  }

  public cancelForm(): void {
    this.showForm = false;
  }

  public save(): void {
    if (!this.formCodigo || !this.formDesignacao) {
      this.snackBar.open(this.translate.instant('classificacaoEconomica.errMissingFields'), this.translate.instant('general.close'), { duration: 3000 });
      return;
    }

    this.economicClassificationService.save({
      id: this.editingId ?? 0,
      codigo: this.formCodigo,
      designacao: this.formDesignacao,
      nivel: this.formNivel,
      parentFk: this.formParentFk,
      orcamentoConfigFk: this.orcamentoConfigFk,
      tipo: this.formTipo || undefined
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

  public deactivate(item: EconomicClassificationDataContract): void {
    this.confirmDialog.confirm(this.translate.instant('classificacaoEconomica.confirmDelete', { codigo: item.codigo, designacao: item.designacao })).subscribe(confirmed => {
      if (!confirmed) { return; }

      this.economicClassificationService.deactivate({ id: item.id }).subscribe(
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
    const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('classificacaoEconomica.errGeneric');
    this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
  }
}
