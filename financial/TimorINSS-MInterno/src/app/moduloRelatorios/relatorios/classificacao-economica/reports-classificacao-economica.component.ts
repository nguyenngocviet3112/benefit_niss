import { Component, OnInit } from "@angular/core";
import { MatDatepicker } from "@angular/material/datepicker";
import { MatDialog } from "@angular/material/dialog";
import { Router } from "@angular/router";
import { TranslateService } from "@ngx-translate/core";
import { Moment } from "moment";
import { NgxSpinnerService } from "ngx-spinner";
import { MyErrorStateMatcher } from "src/app/matcher";
import { GetExecucaoOrcamentalClassificacaoEconomicaRequest } from "src/app/request-models/agrupamentoConfig-request";
import { ClassificacaoEconomicaExecucaoListagem } from "src/app/response-models/agrupamentoConfig-response";
import { ComponenteReceitaRegistoService } from "src/app/services/componenteReceitaRegisto.service";
import { DominiosService } from "src/app/services/dominios.service";
import { PagamentoExecutadoService } from "src/app/services/pagamentoExecutado.service";
import { TokenStorageService } from "src/app/services/token-storage.service";
import { formataCurrency, openErrorsDialog, showExpiredError } from "src/app/utils";

/**
 * CE_OSS_Global -- relatório "TOTAL POR CLASSIFICAÇÃO ECONÓMICA", conforme sheet CE_OSS_Global do
 * ficheiro OSS_Global_2026_FINAL_livro.xlsx do cliente: uma secção RECEITAS (mã 401.xx) e uma secção
 * DESPESAS (mã 501-506.xx), cada linha um código de Classificação Económica.
 */
@Component({
  selector: 'app-reports-classificacao-economica',
  templateUrl: './reports-classificacao-economica.component.html',
  styleUrls: ['./reports-classificacao-economica.component.css']
})
export class RelatoriosClassificacaoEconomicaComponent implements OnInit {

  public matcher: MyErrorStateMatcher = new MyErrorStateMatcher();
  public errors: string[] = [];

  public institutionContaOptions: any[] = [];

  public submittedTry: boolean = false;
  public resultsShown: boolean = false;
  public selectedInstitution?: number;
  public year?: number;
  public beginDate?: Date;

  public receitasList: ClassificacaoEconomicaExecucaoListagem[] = [];
  public despesasList: ClassificacaoEconomicaExecucaoListagem[] = [];
  public displayedColumns: string[] = ['codigoCE', 'designacaoCE', 'valorOrcamentoInicial', 'valorOrcamentado', 'janeiro', 'fevereiro', 'marco', 'abril', 'maio', 'junho', 'julho', 'agosto', 'setembro', 'outubro', 'novembro', 'dezembro', 'totalExecucao', 'taxaExecucao'];

  constructor(
    private router: Router,
    private tokenStorage: TokenStorageService,
    private spinner: NgxSpinnerService,
    public errorDialog: MatDialog,
    public translate: TranslateService,
    public pagamentosService: PagamentoExecutadoService,
    public receitaRegistoService: ComponenteReceitaRegistoService,
    public dominiosService: DominiosService,
  ) { }

  ngOnInit(): void {
    if (!this.tokenStorage.getToken()) {
      this.router.navigate(['/login'], { skipLocationChange: true })
    }
    else if (this.tokenStorage.getToken() && !this.tokenStorage.tokenExpired()) {
      this.getInstitutions();
    }
    else {
      showExpiredError(this.errorDialog, this.tokenStorage, this.translate);
    }
  }

  public showLoader() {
    this.spinner.show();
  }

  public hideLoader() {
    this.spinner.hide();
  }

  public showError() {
    const dialogRef = openErrorsDialog(this.errors, this.errorDialog);
    this.hideLoader();
    dialogRef.afterClosed().subscribe(() => {
      this.errors = [];
    });
  }

  public getInstitutions() {
    this.dominiosService.getAllTiposConta().subscribe((response) => {
      this.institutionContaOptions = response.institutions || [];
      this.hideLoader();
    },
      err => {
        this.institutionContaOptions = [];
        this.hideLoader();
        err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
        this.showError();
      });
  }

  public getRelatorio() {
    if (!this.submittedTry) this.submittedTry = true;
    if (!this.year || !this.selectedInstitution) return;

    this.showLoader();

    const request: GetExecucaoOrcamentalClassificacaoEconomicaRequest = {
      year: this.year!,
      institution: this.selectedInstitution!,
    };

    if (!this.resultsShown) this.resultsShown = true;

    this.receitaRegistoService.GetExecucaoOrcamentalPorClassificacaoEconomica(request).subscribe((response) => {
      this.receitasList = response.lista || [];

      this.pagamentosService.GetExecucaoOrcamentalPorClassificacaoEconomica(request).subscribe((responseDespesa) => {
        this.despesasList = responseDespesa.lista || [];
        this.hideLoader();
      },
        err => {
          this.despesasList = [];
          this.hideLoader();
          err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
          this.showError();
        });
    },
      err => {
        this.receitasList = [];
        this.hideLoader();
        err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
        this.showError();
      });
  }

  chosenYearHandler(date: Moment, datepicker: MatDatepicker<Moment>) {
    const normalDate = date.toDate();
    this.year = normalDate.getFullYear();
    this.beginDate = normalDate;
    datepicker.close();
  }

  public clearFilter() {
    this.year = undefined;
    this.selectedInstitution = undefined;
    this.submittedTry = false;
    this.resultsShown = false;
  }

  public formataCurrency(amount: number) {
    return formataCurrency(amount);
  }

}
