import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { AgrupamentoRubricaService } from '../../services/agrupamento-rubrica.service';
import { AgrupamentoRubricaItemDataContract } from '../../response-models/agrupamento-rubrica-response';

interface AgrupamentoRubricaRow extends AgrupamentoRubricaItemDataContract {
  children: AgrupamentoRubricaRow[];
}

// Mapeamento Rubricas — quản lý Agrupamentoconfig (đã có sẵn) CHỈ cho 4 TipoConta
// Receita/Despesa/Neutro Receita/Neutro Despesa (mapping Codigoconta -> dòng
// Balanço/DR). KHÔNG hiển thị Actidade/Funcional — 2 giá trị đó là dữ liệu cũ
// trùng lặp Programa/Atividade và Classificação Funcional (đã có màn riêng).
@Component({
  selector: 'app-mapeamento-rubricas',
  templateUrl: './mapeamento-rubricas.component.html',
  styleUrls: ['./mapeamento-rubricas.component.css']
})
export class MapeamentoRubricasComponent implements OnInit {

  public tipoContaOptions = ['Receita', 'Despesa', 'Neutro Receita', 'Neutro Despesa'];
  public tipoConta = 'Receita';
  public orcamentoConfigFk = 1;
  public flatItems: AgrupamentoRubricaItemDataContract[] = [];
  public tree: AgrupamentoRubricaRow[] = [];
  public loading = false;

  public editingId: number | null = null;
  public formParentFk: number | undefined;
  public formCodigo = '';
  public formDesignacao = '';
  public showForm = false;

  constructor(
    private agrupamentoRubricaService: AgrupamentoRubricaService,
    private snackBar: MatSnackBar,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.loadTree();
  }

  public loadTree(): void {
    this.loading = true;
    this.agrupamentoRubricaService.getTree(this.orcamentoConfigFk, this.tipoConta).subscribe(
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

  private buildTree(items: AgrupamentoRubricaItemDataContract[]): AgrupamentoRubricaRow[] {
    const rowsById = new Map<number, AgrupamentoRubricaRow>();
    items.forEach(i => rowsById.set(i.id, { ...i, children: [] }));

    const roots: AgrupamentoRubricaRow[] = [];
    rowsById.forEach(row => {
      if (row.parentFk && rowsById.has(row.parentFk)) {
        rowsById.get(row.parentFk)!.children.push(row);
      } else {
        roots.push(row);
      }
    });
    return roots;
  }

  public openAddForm(parent?: AgrupamentoRubricaItemDataContract): void {
    this.editingId = null;
    this.formParentFk = parent?.id;
    this.formCodigo = '';
    this.formDesignacao = '';
    this.showForm = true;
  }

  public openEditForm(item: AgrupamentoRubricaItemDataContract): void {
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
      this.snackBar.open(this.translate.instant('mapeamentoRubricas.errMissingFields'), this.translate.instant('general.close'), { duration: 3000 });
      return;
    }

    this.agrupamentoRubricaService.save({
      id: this.editingId ?? 0,
      codigo: this.formCodigo,
      designacao: this.formDesignacao,
      parentFk: this.formParentFk,
      orcamentoConfigFk: this.orcamentoConfigFk,
      tipoConta: this.tipoConta
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

  public deactivate(item: AgrupamentoRubricaItemDataContract): void {
    if (!confirm(this.translate.instant('mapeamentoRubricas.confirmDelete', { codigo: item.codigo, designacao: item.designacao }))) {
      return;
    }

    this.agrupamentoRubricaService.deactivate({ id: item.id }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.loadTree();
      },
      err => this.showError(err)
    );
  }

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('mapeamentoRubricas.errGeneric');
    this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
  }
}
