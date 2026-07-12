import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { FunctionalClassificationService } from '../../services/functional-classification.service';
import { FunctionalClassificationDataContract } from '../../response-models/functional-classification-response';

interface FunctionalClassificationRow extends FunctionalClassificationDataContract {
  children: FunctionalClassificationRow[];
}

@Component({
  selector: 'app-classificacao-funcional',
  templateUrl: './classificacao-funcional.component.html',
  styleUrls: ['./classificacao-funcional.component.css']
})
export class ClassificacaoFuncionalComponent implements OnInit {

  public tree: FunctionalClassificationRow[] = [];
  public loading = false;

  public searchCodigo = '';

  public editingId: number | null = null;
  public formParentFk: number | undefined;
  public formCodigo = '';
  public formDesignacao = '';
  public showForm = false;

  constructor(
    private functionalClassificationService: FunctionalClassificationService,
    private snackBar: MatSnackBar,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.load();
  }

  public load(): void {
    this.loading = true;
    this.functionalClassificationService.getAllActive().subscribe(
      response => {
        this.tree = this.buildTree(response.items ?? []);
        this.loading = false;
      },
      err => {
        this.loading = false;
        this.showError(err);
      }
    );
  }

  private buildTree(items: FunctionalClassificationDataContract[]): FunctionalClassificationRow[] {
    const rowsById = new Map<number, FunctionalClassificationRow>();
    items.forEach(i => rowsById.set(i.id, { ...i, children: [] }));

    const roots: FunctionalClassificationRow[] = [];
    rowsById.forEach(row => {
      if (row.parentFk && rowsById.has(row.parentFk)) {
        rowsById.get(row.parentFk)!.children.push(row);
      } else {
        roots.push(row);
      }
    });
    return roots;
  }

  public get filteredTree(): FunctionalClassificationRow[] {
    const term = this.searchCodigo.trim().toLowerCase();
    if (!term) {
      return this.tree;
    }
    return this.tree
      .map(node => this.filterNode(node, term))
      .filter((node): node is FunctionalClassificationRow => node !== null);
  }

  private filterNode(node: FunctionalClassificationRow, term: string): FunctionalClassificationRow | null {
    const selfMatches = node.codigo.toLowerCase().includes(term);
    const filteredChildren = node.children
      .map(child => this.filterNode(child, term))
      .filter((child): child is FunctionalClassificationRow => child !== null);
    if (selfMatches || filteredChildren.length > 0) {
      return { ...node, children: filteredChildren };
    }
    return null;
  }

  public openAddForm(parent?: FunctionalClassificationDataContract): void {
    this.editingId = null;
    this.formParentFk = parent?.id;
    this.formCodigo = '';
    this.formDesignacao = '';
    this.showForm = true;
  }

  public openEditForm(item: FunctionalClassificationDataContract): void {
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
      this.snackBar.open(this.translate.instant('classificacaoFuncionalScreen.errMissingFields'), this.translate.instant('general.close'), { duration: 3000 });
      return;
    }

    this.functionalClassificationService.save({
      id: this.editingId ?? 0,
      codigo: this.formCodigo,
      designacao: this.formDesignacao,
      parentFk: this.formParentFk
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.showForm = false;
        this.load();
      },
      err => this.showError(err)
    );
  }

  public deactivate(item: FunctionalClassificationDataContract): void {
    if (!confirm(this.translate.instant('classificacaoFuncionalScreen.confirmDelete', { codigo: item.codigo, designacao: item.designacao }))) {
      return;
    }

    this.functionalClassificationService.deactivate({ id: item.id }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.load();
      },
      err => this.showError(err)
    );
  }

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('classificacaoFuncionalScreen.errGeneric');
    this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
  }
}
