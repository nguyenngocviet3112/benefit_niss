import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { AgrupamentoRubricaService } from '../../services/agrupamento-rubrica.service';
import { AgrupamentoRubricaItemDataContract } from '../../response-models/agrupamento-rubrica-response';

interface AgrupamentoRubricaRow extends AgrupamentoRubricaItemDataContract {
  children: AgrupamentoRubricaRow[];
  fullCodigo: string;
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

  public searchCodigo = '';

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
    items.forEach(i => rowsById.set(i.id, { ...i, children: [], fullCodigo: i.codigo }));

    const roots: AgrupamentoRubricaRow[] = [];
    rowsById.forEach(row => {
      if (row.parentFk && rowsById.has(row.parentFk)) {
        rowsById.get(row.parentFk)!.children.push(row);
      } else {
        roots.push(row);
      }
    });

    // Mã trong DB chỉ là 1-2 chữ số cục bộ theo từng cấp, lặp lại giống nhau
    // ở mọi nhánh cha ("01","02","03"...) — phải ghép mã đầy đủ mới phân biệt
    // được (vd "401.01.01"), cùng convention với EconomicClassification mà
    // bảng này được mô phỏng theo (dùng dấu chấm, khác Codigoconta nối liền
    // không dấu — Codigoconta dùng đúng 1 chữ số/cấp nên nối liền không mơ hồ,
    // còn bảng này 2 chữ số/cấp nên cần dấu chấm để không đọc nhầm ranh giới).
    const computeFullCodigo = (node: AgrupamentoRubricaRow, parentFullCodigo: string): void => {
      node.fullCodigo = parentFullCodigo ? `${parentFullCodigo}.${node.codigo}` : node.codigo;
      node.children.forEach(child => computeFullCodigo(child, node.fullCodigo));
    };
    roots.forEach(root => computeFullCodigo(root, ''));

    return roots;
  }

  public get filteredTree(): AgrupamentoRubricaRow[] {
    const term = this.searchCodigo.trim().toLowerCase();
    if (!term) {
      return this.tree;
    }
    return this.tree
      .map(node => this.filterNode(node, term))
      .filter((node): node is AgrupamentoRubricaRow => node !== null);
  }

  private filterNode(node: AgrupamentoRubricaRow, term: string): AgrupamentoRubricaRow | null {
    const selfMatches = node.codigo.toLowerCase().includes(term) || node.fullCodigo.toLowerCase().includes(term);
    const filteredChildren = node.children
      .map(child => this.filterNode(child, term))
      .filter((child): child is AgrupamentoRubricaRow => child !== null);
    if (selfMatches || filteredChildren.length > 0) {
      return { ...node, children: filteredChildren };
    }
    return null;
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
