import { Component, OnInit } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { Router } from "@angular/router";
import { TranslateService } from "@ngx-translate/core";
import { NgxSpinnerService } from "ngx-spinner";
import { TokenStorageService } from "../../services/token-storage.service";
import { ManagerViewService } from "../../services/managerView.service";
import { FilterRequest } from "../../request-models/utils-request";
import {
  EntidadeRelatorioListagem,
  SituacaoContributivaEmpresaListagem,
  ContribuicoesTrendMes,
  BudgetExecutionRelatorioResponse,
  DespesaPipelineEstagio
} from "../../response-models/managerView-response";
import { openErrorsDialog, showExpiredError, formataCurrency } from "../../utils";

type ManagerViewTab = "empresas" | "situacao" | "trends" | "orcamento" | "pipeline";

@Component({
  selector: "app-manager-view",
  templateUrl: "./manager-view.component.html",
  styleUrls: ["./manager-view.component.css"]
})
export class ManagerViewComponent implements OnInit {

  public activeTab: ManagerViewTab = "empresas";
  public errors: string[] = [];
  public loadedTabs: { [key: string]: boolean } = {};

  // Tab 1: Empresas Registadas
  public empresasList: EntidadeRelatorioListagem[] = [];
  public empresasColumns: string[] = ['nome', 'niss', 'dtInscricao', 'totalTrabalhadores', 'totalMasculino', 'totalFeminino', 'ativo'];
  public empresasFilter: FilterRequest = { index: 0, rows: 10 };
  public empresasTotalRows = 0;
  public empresasBeginDate?: string;
  public empresasEndDate?: string;
  public empresasSearch = "";

  // Tab 2: Situação Contributiva
  public situacaoList: SituacaoContributivaEmpresaListagem[] = [];
  public situacaoColumns: string[] = ['nomeEmpregador', 'niss', 'ultimoMesPago', 'mesesEmDivida', 'totalDivida'];
  public situacaoFilter: FilterRequest = { index: 0, rows: 10 };
  public situacaoTotalRows = 0;
  public situacaoSearch = "";

  // Tab 3: Trends
  public trendsMeses: ContribuicoesTrendMes[] = [];

  // Tab 4: Budget Execution
  public budgetExecution?: BudgetExecutionRelatorioResponse;

  // Tab 5: Despesa Pipeline
  public pipelineEstagios: DespesaPipelineEstagio[] = [];

  constructor(
    private router: Router,
    private tokenStorage: TokenStorageService,
    private spinner: NgxSpinnerService,
    public errorDialog: MatDialog,
    public translate: TranslateService,
    private managerViewService: ManagerViewService
  ) { }

  ngOnInit(): void {
    if (!this.tokenStorage.getToken()) {
      this.router.navigate(['/login'], { skipLocationChange: true });
    } else if (this.tokenStorage.tokenExpired()) {
      showExpiredError(this.errorDialog, this.tokenStorage, this.translate);
    } else {
      this.selectTab('empresas');
    }
  }

  public selectTab(tab: ManagerViewTab): void {
    this.activeTab = tab;
    if (this.loadedTabs[tab]) return;
    this.loadedTabs[tab] = true;

    switch (tab) {
      case 'empresas': this.getEmpresasTable(); break;
      case 'situacao': this.getSituacaoTable(); break;
      case 'trends': this.getTrends(); break;
      case 'orcamento': this.getBudgetExecution(); break;
      case 'pipeline': this.getDespesaPipeline(); break;
    }
  }

  public showLoader(): void { this.spinner.show(); }
  public hideLoader(): void { this.spinner.hide(); }

  public showError(): void {
    const dialogRef = openErrorsDialog(this.errors, this.errorDialog);
    this.hideLoader();
    dialogRef.afterClosed().subscribe(() => { this.errors = []; });
  }

  // ============ Tab 1: Empresas Registadas ============
  public getEmpresasTable(): void {
    this.showLoader();
    this.empresasFilter.dateFilterBegin = this.empresasBeginDate ? new Date(this.empresasBeginDate) : undefined;
    this.empresasFilter.dateFilterEnd = this.empresasEndDate ? new Date(this.empresasEndDate) : undefined;
    this.empresasFilter.filterBy = this.empresasSearch;

    this.managerViewService.GetEntidadesRelatorio({ filter: this.empresasFilter }).subscribe(response => {
      this.empresasList = response.entidades || [];
      this.empresasTotalRows = response.rows;
      this.hideLoader();
    }, err => {
      this.hideLoader();
      err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
      this.showError();
    });
  }

  public updateEmpresasTable(event: any): void {
    this.empresasFilter.index = event.pageIndex;
    this.empresasFilter.rows = event.pageSize;
    this.getEmpresasTable();
  }

  // ============ Tab 2: Situação Contributiva ============
  public getSituacaoTable(): void {
    this.showLoader();
    this.managerViewService.GetSituacaoContributivaEmpresasRelatorio({ filter: this.situacaoFilter, search: this.situacaoSearch }).subscribe(response => {
      this.situacaoList = response.empresas || [];
      this.situacaoTotalRows = response.rows;
      this.hideLoader();
    }, err => {
      this.hideLoader();
      err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
      this.showError();
    });
  }

  public updateSituacaoTable(event: any): void {
    this.situacaoFilter.index = event.pageIndex;
    this.situacaoFilter.rows = event.pageSize;
    this.getSituacaoTable();
  }

  // ============ Tab 3: Trends ============
  public getTrends(): void {
    this.showLoader();
    this.managerViewService.GetContribuicoesTrendsRelatorio({ filter: {} }).subscribe(response => {
      this.trendsMeses = response.meses || [];
      this.hideLoader();
    }, err => {
      this.hideLoader();
      err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
      this.showError();
    });
  }

  // ============ Tab 4: Budget Execution ============
  public getBudgetExecution(): void {
    this.showLoader();
    this.managerViewService.GetBudgetExecutionRelatorio({ filter: {} }).subscribe(response => {
      this.budgetExecution = response;
      this.hideLoader();
    }, err => {
      this.hideLoader();
      err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
      this.showError();
    });
  }

  public execPercent(part: number, total: number): number {
    if (!total || total <= 0) return 0;
    return Math.min(100, Math.round((part / total) * 100));
  }

  // ============ Tab 5: Despesa Pipeline ============
  public getDespesaPipeline(): void {
    this.showLoader();
    this.managerViewService.GetDespesaPipelineRelatorio({ filter: {} }).subscribe(response => {
      this.pipelineEstagios = (response.estagios || []).sort((a, b) => a.ordem - b.ordem);
      this.hideLoader();
    }, err => {
      this.hideLoader();
      err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
      this.showError();
    });
  }

  public pipelineMax(): number {
    return Math.max(1, ...this.pipelineEstagios.map(e => e.total));
  }

  public formataCurrency(amount: number): string {
    return formataCurrency(amount);
  }
}
