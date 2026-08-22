import { SelectionModel } from '@angular/cdk/collections';
import { DatePipe } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { NgxSpinnerService } from 'ngx-spinner';

import { ContaBancaria } from 'src/app/models/contaBancaria';
import { MovimentosBancariosData } from 'src/app/models/movimentosBancarios';
import { MovimentosDespesaReceita, MovimentosPorConciliarListagemType } from 'src/app/models/movimentosDespesaReceita';
import { SelectType } from 'src/app/models/utils';
import { PopUpWarningComponent } from 'src/app/componentes/pop-up-warning/pop-up-warning.component';
import { MovimentosDespesaReceitaUpsertRequest, MovimentosListagemRequest, MovimentosUpsertDataRequest, MovimentosAConciliarFilterRequest, FiltroConciliado, ConciliarMovimentosRequest, ConciliarMovimentosPermissionsListRequest } from 'src/app/request-models/movimentosBancarios-request';
import { FilterRequest } from 'src/app/request-models/utils-request';
import { DominioDescricaoString } from 'src/app/response-models/dominios-response';
import { ConciliarMovimentosPermissionsListResponse, SaldoMovimentosResponse } from 'src/app/response-models/movimentosBancarios-response';
import { DominiosService } from 'src/app/services/dominios.service';
import { movimentosBancariosService } from 'src/app/services/movimentosBancarios.service';
import { TokenStorageService } from 'src/app/services/token-storage.service';
import { base64ToArrayBuffer, formatDatePT, openErrorsDialog, openSnackBar, JsPdf_centerText } from 'src/app/utils';
import { gerarInvoicePDF } from 'src/app/utils-invoice';
import { GuiaPagamentoService } from 'src/app/services/guiaPagamento.service';
import { PagamentoExecutadoService } from 'src/app/services/pagamentoExecutado.service';
import { GetDestinatarioPagamentoRequest } from 'src/app/request-models/pagamentoExecutado-request';
import { ListaPagamentosDoProcesso } from 'src/app/models/pagamentos_executados';
import jsPDF from 'jspdf';
import { environment } from 'src/environments/environment';
import { gerarPDF, PopUpMovimentosDesfazerConciliacaoComponent } from './pop-up-movimentos-desfazer-conciliacao/logic';
import { PopUpMovimentosDespesaReceitaUpsertComponent } from './pop-up-movimentos-despesa-receita-upsert/logic';
import { PopUpMovimentosUpsertComponent } from './pop-up-movimentos-upsert/logic';
import { PopUpMovimentosDesfazerConciliacaoComponentData } from './pop-up-movimentos-desfazer-conciliacao/logic'
import { faFilePdf } from '@fortawesome/free-solid-svg-icons';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PopUpClassificacaoContabilisticaComponent } from './pop-up-classificacao-contabilistica/pop-up-classificacao-contabilistica.component';
import { ExcelImporterPopupMovimentosComponent } from "src/app/componentes/excel-importer/excel-importer-popups/excel-importer-popup-movimentos/excel-importer-popup-movimentos.component";
import { catchError } from 'rxjs/operators';
import { forkJoin, of } from 'rxjs';


@Component({
  selector: 'componente-concilicacao',
  templateUrl: './componente-concilicacao.component.html',
  styleUrls: ['./componente-concilicacao.component.css']
})
export class ComponenteConcilicacaoComponent implements OnInit {

  @Input() isExpanded: boolean = false;
  @Input() tarefaActivoId: number = 0;

  //Common
  public errors: string[] = [];
  public totalSum: number = 0;
  public faFilePdf = faFilePdf;
  public isReceita?: boolean;
  public isReceitaSelected?: boolean;

  //Permission Region
  public selectMovimentosTypePermission = false;
  public addEditMovimentosPermission = false;
  public addEditMovimentosBancariosPermission = false;
  public viewSelectedToConciliatePermission = false;
  public conciliatePermission = false;
  public undoConciliationPermission = false;

  //Region of Movimentos Bancários
  public contasList: ContaBancaria[] = [];
  public caixasList: DominioDescricaoString[] = [];
  public movimentosTypeList: DominioDescricaoString[] = [];
  public contaSelectedId?: number;
  public selectedConta: ContaBancaria = <ContaBancaria>{};
  public caixaSelectedId?: number;
  public selectedCaixa: DominioDescricaoString = <DominioDescricaoString>{};
  public noResultsMovimentosBancarios: boolean = false;
  public resultsMovimentosBancarios: boolean = false;
  public movimentosBancariosList: MovimentosBancariosData[] = [];
  public movimentosBancariosListOld: MovimentosBancariosData[] = [];
  public movimentosBancariosdisplayedColumns: string[] = ['descricao', 'dataValor', 'credito', 'debito', 'acoes', 'select'];
  public movimentosBancariostotalRowsTable: number = 0;
  public movimentosBancariospageSizeTable = 20;
  public movimentosBancariospageIndexTable = 0;
  public disabledSearchBank = true;
  public movimentosBancariosFilter: FilterRequest = {};
  public movimentosBancariosfilterBy = '';
  public movimentosBancariosSelection = new SelectionModel<number>(true, []);
  public movimentosBancariosDisplayType = SelectType.multiple;
  public saldoTotal?: SaldoMovimentosResponse;
  public hasConciliados = false;
  public disableCheckAllMovimentosBancarios = false;
  public selectedMovimentosBancarios: { id: number; value?: number }[] = [];
  public totalselectedBancarios: number = 0;
  public activeVerConciliadosBancarios = false;
  public activeVerNaoConciliadosBancarios = false;
  public activeVerTodosBancarios = false;
  public activeVerSeleccionadosBancarios = false;

  //Region of Movimentos
  public apenasMovimentosProcesso: boolean = false;
  public noResultsMovimentos: boolean = false;
  public resultsMovimentos: boolean = false;
  public movimentosTypeSelected?: number;
  public disabledSearchMov = true;
  public movimentosSelection = new SelectionModel<number>(true, []);
  public movimentosDisplayType = SelectType.multiple;
  public movimentosList: MovimentosDespesaReceita[] = [];
  public movimentosListOld: MovimentosDespesaReceita[] = [];
  public bankCode?: string;
  public bankOptions: { key: string; label: string }[] = [];

  public get movimentosdisplayedColumns(): string[] {
    const base = ['descricao', 'comprovativo', 'documento', 'numPagamentoGuia'];
    // Bank só có ý nghĩa ở phía Receita (dữ liệu đến từ Guia Pagamento/Invoice)
    return this.isReceitaSelected
      ? [...base, 'bankCode', 'valor', 'acoes', 'select']
      : [...base, 'valor', 'acoes', 'select'];
  }
  public movimentosTotalRowsTable: number = 0;
  public movimentosPageSizeTable = 20;
  public movimentosPageIndexTable = 0;
  public movimentosFilter: FilterRequest = {};
  public movimentosfilterBy = '';
  public hasMovimentosConciliados = false;
  public disableCheckAllMovimentos = false;
  public selectedMovimentos: { id: number; value?: number; type?: number }[] = [];
  public totalselectedMovimentos: number = 0;
  public activeVerConciliados = false;
  public activeVerNaoConciliados = false;
  public activeVerTodos = false;
  public activeVerSeleccionados = false;
  public despesasClassificacaoSelection = new SelectionModel<number>(true, []);
  public selectedDespesas: number[] = [];

  constructor(
    public translate: TranslateService,
    private datepipe: DatePipe,
    private spinner: NgxSpinnerService,
    private tokenStorageService: TokenStorageService,
    private router: Router,
    public errorDialog: MatDialog,
    public dominiosService: DominiosService,
    public movimentosService: movimentosBancariosService,
    public MovimentosBancariosDialog: MatDialog,
    public MovimentosDialog: MatDialog,
    public DesfazerConciliacoesDialog: MatDialog,
    public snackBar: MatSnackBar,
    public warningDialog: MatDialog,
    public classificarContabilisticaDialog: MatDialog,
    public guiaPagamentoService: GuiaPagamentoService,
    public pagamentosService: PagamentoExecutadoService,
  ) { }

  ngOnInit(): void {
    if (!this.tokenStorageService.getToken()) {
      this.router.navigate([''], { skipLocationChange: true });
    }
    else {
      this.spinner.show();
      this.translate.get('guiaPagamentoListagem.lstBankCode').subscribe((res: any) => {
        this.bankOptions = Object.keys(res).map((key) => ({
          key,
          label: res[key],
        }));
      });

      let permissinonsRequest: ConciliarMovimentosPermissionsListRequest = <ConciliarMovimentosPermissionsListRequest>{ tarefaAtivoId: this.tarefaActivoId };
      // let dominioCaixas = this.dominiosService.getAllCaixas();
      // let contasBancarias = this.movimentosService.ListContasBancarias();
      // let dominioMovimentosTypes = this.dominiosService.getAllMovimentosTypes();
      // let permissions = this.movimentosService.ListPermissions(permissinonsRequest);

      // alert(1);

      forkJoin({
        dominioCaixas: this.dominiosService.getAllCaixas().pipe(
          catchError(err => {
            console.error("Lỗi getAllCaixas", err);
            return of(null); // Cho phép tiếp tục forkJoin
          })
        ),
        contasBancarias: this.movimentosService.ListContasBancarias().pipe(
          catchError(err => {
            console.error("Lỗi ListContasBancarias", err);
            return of(null);
          })
        ),
        dominioMovimentosTypes: this.dominiosService.getAllMovimentosTypes().pipe(
          catchError(err => {
            console.error("Lỗi getAllMovimentosTypes", err);
            return of(null);
          })
        ),
        permissions: this.movimentosService.ListPermissions(permissinonsRequest).pipe(
          catchError(err => {
            console.error("Lỗi ListPermissions", err);
            return of(null);
          })
        )
      }).subscribe(({ dominioCaixas, contasBancarias, dominioMovimentosTypes, permissions }) => {

        // ✅ Gán chỉ khi có dữ liệu
        if (dominioCaixas?.dominios) {
          this.caixasList = dominioCaixas.dominios;
        }
        // alert(contasBancarias);
        if (contasBancarias?.contas) {
          // alert(2);
          this.contasList = contasBancarias.contas;
        }

        if (dominioMovimentosTypes?.dominios) {
          this.movimentosTypeList = dominioMovimentosTypes.dominios;
        }

        if (permissions) {
          this.buildPermissions(permissions);
        }

        this.spinner.hide();
      });
      // forkJoin({
      //   contasBancarias: this.movimentosService.ListContasBancarias(),
      //   permissions: this.movimentosService.ListPermissions(permissinonsRequest)
      // }).subscribe(({  contasBancarias,  permissions }) => {
      //   this.contasList = contasBancarias.contas;
      //   this.buildPermissions(permissions);
      //   this.spinner.hide();
      // });

      // forkJoin([dominioCaixas, contasBancarias, dominioMovimentosTypes, permissions])
      //     .subscribe(([dominioCaixas, contasBancarias, dominioMovimentosTypes, permissions]) => {
      //         this.caixasList = dominioCaixas.dominios;
      //         this.contasList = contasBancarias.contas;
      //         this.movimentosTypeList = dominioMovimentosTypes.dominios;
      //         this.buildPermissions(permissions);
      //         this.spinner.hide();
      //     });
    }
  }

  public resetCaixa(): void {
    this.caixaSelectedId = undefined;
    this.disabledSearchBank = false;
    this.resultsMovimentosBancarios = false;
    this.noResultsMovimentosBancarios = false;
    this.movimentosBancariosSelection.clear();
    this.movimentosBancariosList = [];
    this.selectedConta = this.contasList.find(x => x.id == this.contaSelectedId) ?? <ContaBancaria>{};
    this.getSaldoTotalTable();
  }

  public resetConta(): void {
    this.contaSelectedId = undefined;
    this.disabledSearchBank = false;
    this.resultsMovimentosBancarios = false;
    this.noResultsMovimentosBancarios = false;
    this.movimentosBancariosSelection.clear();
    this.movimentosBancariosList = [];
    this.selectedCaixa = this.caixasList.find(x => x.id == this.caixaSelectedId) ?? <DominioDescricaoString>{};
    this.getSaldoTotalTable();
  }

  public selectMovimentoType(): void {
    this.disabledSearchMov = false;
    this.isReceitaSelected = this.movimentosTypeList.find(x => x.id == 
      this.movimentosTypeSelected)?.descricao == 'Receita' || this.movimentosTypeList.find(x => x.id == 
      this.movimentosTypeSelected)?.descricao == 'Revenue';
  }

  public searchMovimentosBancarios(resetSelect: boolean = false): void {
    if (resetSelect) {
      this.movimentosBancariosSelection.clear();
      this.resetMovimentosBancariosActives();
      this.movimentosDisplayType = SelectType.multiple;
      this.disableCheckAllMovimentos = false;
      this.totalselectedBancarios = 0;
      this.selectedMovimentosBancarios = [];
      this.totalSum = this.sumTotalValue();
    }
    this.movimentosBancariospageIndexTable = 0;
    this.movimentosBancariospageSizeTable = 20;
    this.getMovimentosBancariosTable();
  }

  public updateTableMovimentosBancarios(event: any) {
    this.movimentosBancariospageIndexTable = event.pageIndex;
    this.movimentosBancariospageSizeTable = event.pageSize;
    this.activeVerSeleccionadosBancarios = false;
    this.showLoader();
    this.getMovimentosBancariosTable(this.activeVerConciliadosBancarios, this.activeVerTodosBancarios);
  }

  public searchMovimentosAConciliar(resetSelect: boolean = false): void {
    if (resetSelect) {
      this.movimentosSelection.clear();
      this.movimentosBancariosSelection.clear();
      this.resetMovimentosActives();
      this.movimentosBancariosDisplayType = SelectType.multiple;
      this.disableCheckAllMovimentosBancarios = false;
      this.totalselectedMovimentos = 0;
      this.selectedMovimentos = [];
      this.totalselectedBancarios = 0;
      this.selectedMovimentosBancarios = [];
      this.totalSum = this.sumTotalValue();
    }
    this.isReceita = this.movimentosTypeList.find(x => x.id == this.movimentosTypeSelected)?.descricao == 'Receita'
    || this.movimentosTypeList.find(x => x.id == 
      this.movimentosTypeSelected)?.descricao == 'Revenue';
    this.movimentosPageIndexTable = 0;
    this.movimentosPageSizeTable = 20;
    this.getMovimentosTable();
  }

  public getMovimentosBancariosTable(filterByConciliados: boolean = false, filterByTodos: boolean = false) {
    this.showLoader();
    this.movimentosBancariosFilter.index = this.movimentosBancariospageIndexTable;
    this.movimentosBancariosFilter.rows = this.movimentosBancariospageSizeTable;

    this.movimentosBancariosFilter.filterBy = this.movimentosBancariosfilterBy;

    let request: MovimentosListagemRequest;

    request = {
      filter: this.movimentosBancariosFilter
    };

    if (filterByConciliados) {
      request.filter.filterField = 'concilados';
    } else if (filterByTodos)
      request.filter.filterField = undefined;
    else
      request.filter.filterField = 'naoConcilados';

    if (this.contaSelectedId) {
      request.BancoId = this.contaSelectedId;
    }
    else if (this.caixaSelectedId) {
      request.CaixaId = this.caixaSelectedId;
    }

    this.movimentosService.listMovimentosBancarios(request).subscribe(x => {
      x.rows == null ? this.movimentosBancariostotalRowsTable = 0 : this.movimentosBancariostotalRowsTable = x.rows;
      x.movimentos == null ? this.movimentosBancariosList = [] : this.movimentosBancariosList = x.movimentos;
      if (x.movimentos.length == 0) {
        this.noResultsMovimentosBancarios = true;
      }
      else {
        this.movimentosBancariosListOld = this.movimentosBancariosList;
        this.noResultsMovimentosBancarios = false;
        this.resultsMovimentosBancarios = true;
        this.hasConciliados = this.movimentosBancariosList.filter(x => x.conciliado == true).length > 0;
      }


      this.hideLoader();
    });
  }

  public getMovimentosTable(filterByConciliados: boolean = false, filterByTodos: boolean = false) {
    this.showLoader();
    this.movimentosFilter.index = this.movimentosPageIndexTable;
    this.movimentosFilter.rows = this.movimentosPageSizeTable;

    this.movimentosFilter.filterBy = this.movimentosfilterBy;

    let request: MovimentosAConciliarFilterRequest;

    request = {
      tarefaAtivoId: this.apenasMovimentosProcesso ? this.tarefaActivoId : undefined,
      filtroConciliado: filterByConciliados ? FiltroConciliado.Conciliados : FiltroConciliado.NaoConciliados,
      isReceita: this.isReceita,
      bankCode: this.isReceitaSelected ? this.bankCode : undefined,
      filter: this.movimentosFilter
    };

    if (filterByTodos)
      request.filtroConciliado = FiltroConciliado.Todos;

    this.movimentosService.ListMovimentoDespesaReceita(request).subscribe(x => {
      x.rows == null ? this.movimentosTotalRowsTable = 0 : this.movimentosTotalRowsTable = x.rows;
      x.movimentos == null ? this.movimentosList = [] : this.movimentosList = x.movimentos;
      if (x.movimentos.length == 0) {
        this.noResultsMovimentos = true;
      }
      else {
        this.movimentosListOld = this.movimentosList;
        this.noResultsMovimentos = false;
        this.resultsMovimentos = true;
        this.hasMovimentosConciliados = this.movimentosList.filter(x => x.conciliado == true).length > 0;
        if (!this.isReceita)
          this.movimentosList.filter(x => x.isClassificada == true).length > 0 ? this.disableCheckAllMovimentos = false : this.disableCheckAllMovimentos = true;
      }

      this.hideLoader();
    },
      err => {
        this.showError();
      });
  }

  public getSaldoTotalTable() {
    this.showLoader();

    this.movimentosService.ListSaldoMovimentos({
      ContaId: this.contaSelectedId,
      CaixaId: this.caixaSelectedId
    }).subscribe(x => {
      this.saldoTotal = x;
      this.hideLoader();
    });
  }

  public editMovimentosBancariosPopUp(selectedMovimento: MovimentosBancariosData): void {
    let data: MovimentosUpsertDataRequest = <MovimentosUpsertDataRequest>
      {
        id: selectedMovimento.id,
        bancoId: this.contaSelectedId,
        caixaId: this.caixaSelectedId,
        descricao: selectedMovimento.descricao,
        data: selectedMovimento.dataValor,
        tarefaAtivoId: this.tarefaActivoId,
        valor: selectedMovimento.credito != null ? selectedMovimento.credito : selectedMovimento.debito != null ? selectedMovimento.debito : <number>{}
      };

    const dialogRef = this.MovimentosBancariosDialog.open(PopUpMovimentosUpsertComponent, {
      id: 'gravarCampo',
      minHeight: '300px',
      width: '70%',
      height: '60%',
      panelClass: 'modalWithBorder',
      data: data
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.showLoader();
        this.getMovimentosBancariosTable();
        this.getSaldoTotalTable();
      }
    });
  }

  public openMovimentosBancariosPopUp(): void {
    let data: MovimentosUpsertDataRequest = <MovimentosUpsertDataRequest>{ bancoId: this.contaSelectedId, caixaId: this.caixaSelectedId, tarefaAtivoId: this.tarefaActivoId };

    const dialogRef = this.MovimentosBancariosDialog.open(PopUpMovimentosUpsertComponent, {
      id: 'gravarCampo',
      minHeight: '300px',
      width: '70%',
      height: '60%',
      panelClass: 'modalWithBorder',
      data: data
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.showLoader();
        this.getMovimentosBancariosTable();
        this.getSaldoTotalTable();
      }
    });
  }

  public openMovimentosPopUp(): void {

    let data: MovimentosDespesaReceitaUpsertRequest = <MovimentosDespesaReceitaUpsertRequest>
      {
        tarefaAtivoId: this.tarefaActivoId,
        tipoMovimento: this.movimentosTypeSelected,
        isReceita: this.movimentosTypeList.find(x => x.id == this.movimentosTypeSelected)?.descricao == 'Receita'
        || this.movimentosTypeList.find(x => x.id == 
      this.movimentosTypeSelected)?.descricao == 'Revenue'
      };

    const dialogRef = this.MovimentosDialog.open(PopUpMovimentosDespesaReceitaUpsertComponent, {
      id: 'gravarCampo',
      minHeight: '300px',
      width: '70%',
      height: '60%',
      panelClass: 'modalWithBorder',
      data: data
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.showLoader();
        this.getMovimentosTable();
      }
    });
  }

  public openDesfazerConciliacoesPopUp(): void {
    let data: PopUpMovimentosDesfazerConciliacaoComponentData = <PopUpMovimentosDesfazerConciliacaoComponentData>{ tarefaAtivoId: this.tarefaActivoId };

    const dialogRef = this.DesfazerConciliacoesDialog.open(PopUpMovimentosDesfazerConciliacaoComponent, {
      id: 'gravarCampo',
      minHeight: '300px',
      width: '70%',
      height: '80%',
      panelClass: 'modalWithBorder',
      data: data
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.showLoader();
        this.getMovimentosBancariosTable();
        this.getSaldoTotalTable();
        this.getMovimentosTable();
      }
    });
  }

  public editMovimentosPopUp(selectedMovimento: MovimentosDespesaReceita): void {
    let data: MovimentosDespesaReceitaUpsertRequest = <MovimentosDespesaReceitaUpsertRequest>
      {
        id: selectedMovimento.id,
        tarefaAtivoId: this.tarefaActivoId,
        tipoMovimento: this.movimentosTypeSelected,
        isReceita: this.isReceita,
        movimentoBancarioId: selectedMovimento.movimentoBancarioId,
        valor: selectedMovimento.valor,
        tipoDocumento: selectedMovimento.tipoDocumento,
        numeroDocumento: selectedMovimento.numeroDocumento,
        comprovativo: selectedMovimento.comprovativo,
        nomeComprovativo: selectedMovimento.nomeComprovativo,
        contabilidadeCredito: selectedMovimento.contabilidadeCredito,
        contabilidadeDebito: selectedMovimento.contabilidadeDebito,
        departamentoINSS: selectedMovimento.departamentoINSS,
        centroCusto: selectedMovimento.centroCusto,
        tipoConta: selectedMovimento.tipoConta,
        contaOSS: selectedMovimento.contaOSS,
        guiaOrReserva: !selectedMovimento.editavel,
        type: selectedMovimento.type,
        isGuia: selectedMovimento.type == MovimentosPorConciliarListagemType.GuiaPagamento,
        isReserva: selectedMovimento.type == MovimentosPorConciliarListagemType.ReservaCredito,
      };

    const dialogRef = this.MovimentosBancariosDialog.open(PopUpMovimentosDespesaReceitaUpsertComponent, {
      id: 'gravarCampo',
      minHeight: '300px',
      width: '70%',
      height: '60%',
      panelClass: 'modalWithBorder',
      data: data
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.showLoader();
        this.getMovimentosTable();
      }
    });
  }

  public updateTableMovimentos(event: any): void {
    this.movimentosPageIndexTable = event.pageIndex;
    this.movimentosPageSizeTable = event.pageSize;
    this.activeVerSeleccionados = false;
    this.showLoader();
    this.getMovimentosTable(this.activeVerConciliados, this.activeVerTodos);
  }

  public selectHandlerMovimentosBancarios(selected: number) {
    if (this.movimentosBancariosDisplayType == SelectType.single) {
      if (!this.movimentosBancariosSelection.isSelected(selected)) {
        this.movimentosBancariosSelection.clear();
      }
    }
    this.movimentosBancariosSelection.toggle(selected);
    if (this.movimentosBancariosSelection.isSelected(selected)) {
      if (this.movimentosBancariosDisplayType == SelectType.single)
        this.selectedMovimentosBancarios = [];
      this.selectedMovimentosBancarios.push({
        id: selected,
        value: this.movimentosBancariosList.find(x => x.id == selected)?.credito ?? this.movimentosBancariosList.find(x => x.id == selected)?.debito
      });
    } else {
      this.selectedMovimentosBancarios = this.selectedMovimentosBancarios.filter(x => x.id != selected);
    }

    let totalValue = 0;
    this.selectedMovimentosBancarios.forEach(element => {
      totalValue += element.value ?? 0
    });
    this.totalselectedBancarios = totalValue;
    this.totalSum = this.sumTotalValue();

    if (this.movimentosBancariosSelection.selected.length > 1) {
      this.movimentosDisplayType = SelectType.single;
      this.disableCheckAllMovimentos = true;
    }
    else {
      this.movimentosDisplayType = SelectType.multiple;
      this.disableCheckAllMovimentos = false;
    }
  }

  public selectHandlerMovimentos(selected: number) {
    if (this.movimentosDisplayType == SelectType.single) {
      if (!this.movimentosSelection.isSelected(selected)) {
        this.movimentosSelection.clear();
      }
    }
    this.movimentosSelection.toggle(selected);
    if (this.movimentosSelection.isSelected(selected)) {
      if (this.movimentosDisplayType == SelectType.single)
        this.selectedMovimentos = [];

      this.selectedMovimentos.push({
        id: selected,
        value: this.movimentosList.find(x => x.id == selected)?.valor,
        type: this.movimentosList.find(x => x.id == selected)?.type
      });
    } else {
      this.selectedMovimentos = this.selectedMovimentos.filter(x => x.id != selected);
    }

    let totalValue = 0;
    this.selectedMovimentos.forEach(element => {
      totalValue += element.value ?? 0
    });
    this.totalselectedMovimentos = totalValue;
    this.totalSum = this.sumTotalValue();

    if (this.movimentosSelection.selected.length > 1) {
      this.movimentosBancariosDisplayType = SelectType.single;
      this.disableCheckAllMovimentosBancarios = true;
    }
    else {
      this.movimentosBancariosDisplayType = SelectType.multiple;
      this.disableCheckAllMovimentosBancarios = false;
    }
  }

  /** Whether the number of selected elements matches the total number of rows. */
  public isAllSelected() {
    const numSelected = this.movimentosBancariosSelection.selected.filter(x => this.movimentosBancariosList.find(y => y.id == x)).length;
    const numRows = this.movimentosBancariosList.filter(x => this.isReceita ? x.credito : x.debito && !x.conciliado).length;
    return numSelected === numRows;
  }

  public isMovimentosAllSelected() {
    const numSelected = this.movimentosSelection.selected.filter(x => this.movimentosList.find(y => y.id == x)).length;
    const numRows = this.isReceita ? this.movimentosList.filter(y => !y.conciliado).length : this.movimentosList.filter(y => !y.conciliado && y.isClassificada).length;
    return numSelected === numRows;
  }

  /** Selects all rows if they are not all selected; otherwise clear selection. */
  public masterToggle() {
    if (this.isAllSelected()) {
      this.movimentosBancariosSelection.clear();
      this.totalselectedBancarios = 0;
      this.totalSum = this.sumTotalValue();
      this.selectedMovimentosBancarios = [];
      this.movimentosDisplayType = SelectType.multiple;
      this.disableCheckAllMovimentos = false;
    }
    else {
      const filteredMovimentos = this.movimentosBancariosList.filter(x => this.isReceita ? x.credito : x.debito);
      filteredMovimentos.filter(x => !x.conciliado).forEach(row => {
        this.movimentosBancariosSelection.select(row.id);
        if (!this.selectedMovimentosBancarios.find(x => x.id == row.id)) {
          this.selectedMovimentosBancarios.push({
            id: row.id,
            value: row.credito ?? row.debito
          });
        }
      });
      let totalValue = 0;
      this.selectedMovimentosBancarios.forEach(element => {
        totalValue += element.value ?? 0
      });
      this.totalselectedBancarios = totalValue;
      this.totalSum = this.sumTotalValue();
      this.movimentosDisplayType = SelectType.single;
      this.disableCheckAllMovimentos = true;
    }

  }

  public masterMovimentosToggle() {
    if (this.isMovimentosAllSelected()) {
      this.movimentosSelection.clear();
      this.totalselectedMovimentos = 0;
      this.totalSum = this.sumTotalValue();
      this.selectedMovimentos = [];
      this.movimentosBancariosDisplayType = SelectType.multiple;
      this.disableCheckAllMovimentosBancarios = false;
    }
    else {
      if (this.isReceita) {
        this.movimentosList.filter(x => !x.conciliado).forEach(row => {
          this.movimentosSelection.select(row.id);
          if (!this.selectedMovimentos.find(x => x.id == row.id)) {
            this.selectedMovimentos.push({
              id: row.id,
              value: row.valor,
              type: row.type
            });
          }
        });
      }
      else {
        this.movimentosList.filter(x => !x.conciliado && x.isClassificada).forEach(row => {
          this.movimentosSelection.select(row.id);
          if (!this.selectedMovimentos.find(x => x.id == row.id) && row.isClassificada == true) {
            this.selectedMovimentos.push({
              id: row.id,
              value: row.valor,
              type: row.type
            });
          }
        });
      }

      let totalValue = 0;
      this.selectedMovimentos.forEach(element => {
        totalValue += element.value ?? 0
      });
      this.totalselectedMovimentos = totalValue;
      this.totalSum = this.sumTotalValue();
      this.movimentosBancariosDisplayType = SelectType.single;
      this.disableCheckAllMovimentosBancarios = true;
    }

  }

  public allDespesasSelected() {
    return this.selectedDespesas.filter(e => this.movimentosList.some(a => a.id === e)).length === this.movimentosList.length;
  }

  public toggleAllDespesasAConciliar() {
    let newList: any[] = this.selectedDespesas;

    if (this.allDespesasSelected()) {
      this.movimentosList.forEach(e => {
        this.despesasClassificacaoSelection.toggle(e.id);
        const ind = newList.indexOf(e.id);
        newList.splice(ind, 1);
      });
    } else {
      this.movimentosList.forEach(e => {
        const isSelected = this.despesasClassificacaoSelection.isSelected(e.id);
        if (!isSelected) {
          this.despesasClassificacaoSelection.toggle(e.id);
          newList.push(e.id);
        }
      });
    }

    this.selectedDespesas = newList;
  }

  public conciliarMovimentos(): void {
    let request: ConciliarMovimentosRequest = <ConciliarMovimentosRequest>{ tarefaAtivoId: this.tarefaActivoId };

    request.movimentosBancarios = this.selectedMovimentosBancarios.map(x => x.id);
    request.movimentosAConciliar = this.selectedMovimentos.map(x => {
      return {
        id: x.id,
        type: x.type ?? 0
      }
    });

    const dialogRef = this.warningDialog.open(PopUpWarningComponent, {
      id: 'deleteMoradaDialog',
      minHeight: '300px',
      width: '40%',
      height: '30%',
      panelClass: 'warningModal',
      data: { function: this.movimentosService.ConciliarMovimentos(request), msg: this.translate.instant('warnings.concilate') }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.showLoader();
        this.searchMovimentosBancarios(true);
        this.searchMovimentosAConciliar(true);
        this.getSaldoTotalTable();
        openSnackBar(this.translate.instant('snackBar.registoConciliado'), this.snackBar);
      }
    });
  }

  public filterBySelecionados(): void {
    this.activeVerConciliadosBancarios = false;
    this.activeVerNaoConciliadosBancarios = false;
    this.activeVerTodosBancarios = false;
    this.activeVerSeleccionadosBancarios = true;
    this.movimentosBancariosList = this.movimentosBancariosList.filter(x => this.movimentosBancariosSelection.selected.find(y => y == x.id));
  }

  public filterByMovimentosSelecionados(): void {
    this.activeVerConciliados = false;
    this.activeVerNaoConciliados = false;
    this.activeVerTodos = false;
    this.activeVerSeleccionados = true;
    this.movimentosList = this.movimentosList.filter(x => this.movimentosSelection.selected.find(y => y == x.id));
  }

  public filterByVerTodos(): void {
    // if (this.activeVerConciliadosBancarios)
    //   this.getMovimentosBancariosTable(false, true);
    this.activeVerConciliadosBancarios = false;
    this.activeVerNaoConciliadosBancarios = false;
    this.activeVerTodosBancarios = true;
    this.activeVerSeleccionadosBancarios = false;
    //this.movimentosBancariosList = this.movimentosBancariosListOld;
    this.getMovimentosBancariosTable(false, true);
  }

  public filterByVerMovimentosTodos(): void {
    // if (this.activeVerConciliados)
    //   this.getMovimentosTable();
    this.activeVerConciliados = false;
    this.activeVerNaoConciliados = false;
    this.activeVerTodos = true;
    this.activeVerSeleccionados = false;
    //this.movimentosList = this.movimentosListOld;
    this.getMovimentosTable(false, true);
  }

  public filterByVerConciliados(): void {
    this.activeVerConciliadosBancarios = true;
    this.activeVerNaoConciliadosBancarios = false;
    this.activeVerTodosBancarios = false;
    this.activeVerSeleccionadosBancarios = false;
    this.getMovimentosBancariosTable(true);
  }

  public filterByVerNaoConciliados(): void {
    this.activeVerConciliadosBancarios = false;
    this.activeVerNaoConciliadosBancarios = true;
    this.activeVerTodosBancarios = false;
    this.activeVerSeleccionadosBancarios = false;
    this.clearFilter();
  }

  public filterByVerMovimentosConciliados(): void {
    this.activeVerConciliados = true;
    this.activeVerNaoConciliados = false;
    this.activeVerTodos = false;
    this.activeVerSeleccionados = false;
    this.getMovimentosTable(true);
  }

  public filterByVerMovimentosNaoConciliados(): void {
    this.activeVerConciliados = false;
    this.activeVerNaoConciliados = true;
    this.activeVerTodos = false;
    this.activeVerSeleccionados = false;
    this.clearFilterMovimentos();
  }

  public resetMovimentosActives(): void {
    this.activeVerConciliados = false;
    this.activeVerNaoConciliados = true;
    this.activeVerTodos = false;
    this.activeVerSeleccionados = false;
  }

  public resetMovimentosBancariosActives(): void {
    this.activeVerConciliadosBancarios = false;
    this.activeVerNaoConciliadosBancarios = true;
    this.activeVerTodosBancarios = false;
    this.activeVerSeleccionadosBancarios = false;
  }

  public clearFilter(): void {
    this.movimentosBancariosfilterBy = '';
    this.movimentosBancariospageSizeTable = 20;
    this.movimentosBancariospageIndexTable = 0;
    this.movimentosBancariosFilter = {};
    this.getMovimentosBancariosTable();
  }

  public clearFilterMovimentos(): void {
    this.movimentosfilterBy = '';
    this.movimentosPageSizeTable = 20;
    this.movimentosPageIndexTable = 0;
    this.movimentosFilter = {};
    this.getMovimentosTable();
  }

  public onBancoSelected(event: any): void {
    this.bankCode = event.value;
    this.movimentosPageIndexTable = 0;
    this.getMovimentosTable();
  }

  public clearFilterBanco(): void {
    this.bankCode = undefined;
    this.movimentosPageIndexTable = 0;
    this.getMovimentosTable();
  }

  public formatDatePT(date: Date): string {
    return formatDatePT(this.datepipe, date);
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

    dialogRef.afterClosed().subscribe(result => {
      this.errors = [];
    });
  }

  public gerarPDFDocumento(element: MovimentosDespesaReceita): void {
    // Movimento vem de Guiapagamento (Receita) — buscar registo completo (NISS/TIN/
    // EE-TCO/QR) para gerar o mesmo design "SOCIAL CONTRIBUTIONS PAYMENT GUIDE" do
    // Contribution module. Movimentos de Despesa (PagamentoExecutado/ReservaCredito)
    // usam o mesmo design "Lista Pagamentu Saláriu Funcionáriu INSS" gerado ao Emitir
    // Ordem de Pagamento (ver pop-up-executar-pagamentos), buscando o(s) destinatário(s)
    // pelo número do pagamento/guia. Movimento manual (sem numeroDocumento) mantém o
    // documento simples antigo, pois não existe um pagamento executado para buscar.
    if (element.type === MovimentosPorConciliarListagemType.GuiaPagamento) {
      this.guiaPagamentoService.getGuiasDetailByEntidade({ idGuiaPagamento: element.id, filter: {} }).subscribe(x => {
        const guia = x.guias?.[0];
        if (guia) {
          gerarInvoicePDF(guia, this.translate, this.datepipe);
        } else {
          gerarPDF(element, this.translate);
        }
      },
        () => gerarPDF(element, this.translate));
    } else if (element.numeroDocumento) {
      let request = <GetDestinatarioPagamentoRequest>{
        numPagamento: element.numeroDocumento,
      };
      this.pagamentosService.GetPagamentoDetails(request).subscribe(x => {
        if (x.pagamentos && x.pagamentos.length > 0) {
          this.gerarPDFListaPagamentos(x.pagamentos);
        } else {
          gerarPDF(element, this.translate);
        }
      },
        () => gerarPDF(element, this.translate));
    } else {
      gerarPDF(element, this.translate);
    }
  }

  // Mesmo design "Lista Pagamentu Saláriu Funcionáriu INSS" gerado ao Emitir Ordem de
  // Pagamento (ver pop-up-executar-pagamentos.component.ts / pop-up-listar-pagamentos-
  // executados.component.ts) — replicado aqui para o ícone "Documento" da Conciliação
  // de Movimentos (Despesa), em vez do documento simples antigo (2 colunas Descrição/Valor).
  public gerarPDFListaPagamentos(pdfList: ListaPagamentosDoProcesso[]): void {
    var pdf = new jsPDF();
    const monthKeysPT = ['Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho', 'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'];

    // Agrupar por Banco + Mês/Ano (dataObrigacao)
    const groups = new Map<string, ListaPagamentosDoProcesso[]>();
    pdfList.forEach(row => {
      const date = row.dataObrigacao ? new Date(row.dataObrigacao) : new Date();
      const key = `${row.bankCode ?? ''}_${date.getMonth()}_${date.getFullYear()}`;
      if (!groups.has(key)) groups.set(key, []);
      groups.get(key)!.push(row);
    });

    let isFirstPage = true;
    groups.forEach((rowsForGroup, key) => {
      if (!isFirstPage) pdf.addPage();
      isFirstPage = false;

      const [bankCode, monthIndex, year] = key.split('_');
      // [PT] Banco obrigatorio hoje, mas os pagamentos antigos podem nao ter bankCode:
      // nesse caso o cabecalho saia com um espaco duplo onde devia estar o nome do banco.
      // [VI] Nay bank la bat buoc, nhung cac lenh chi cu co the khong co bankCode: khi do
      // dong tieu de bi hai dau cach lien nhau o cho le ra la ten ngan hang.
      const bankLabel = this.bankOptions.find(b => b.key === bankCode)?.label || bankCode || 'Banku la iha';
      const monthLabel = monthKeysPT[Number(monthIndex)];

      // INSS logo
      pdf.addImage(environment.ssIcon, 'JPEG', 90, 5, 25, 20);

      JsPdf_centerText(pdf, 'Lista Pagamentu Saláriu Funcionáriu INSS', 35);
      pdf.setFontSize(12);
      pdf.setTextColor(99);

      JsPdf_centerText(pdf, `Fulan ${monthLabel} ${bankLabel} Tinan ${year}`, 43);

      // Somar valor por destinatário dentro do grupo
      const byDestinatario = new Map<number, { nome: string; niss: string; numeroConta: string; iban: string; total: number }>();
      rowsForGroup.forEach(row => {
        const id = row.destinatario.id;
        if (!byDestinatario.has(id)) {
          byDestinatario.set(id, { nome: row.destinatario.nome, niss: row.destinatario.niss ?? '', numeroConta: row.numeroConta ?? '', iban: row.iban ?? '', total: 0 });
        }
        const entry = byDestinatario.get(id)!;
        entry.total = Math.round((entry.total + row.valor) * 100) / 100;
      });

      var rows: string[][] = [];
      var stt = 1;
      var totalPagamentu = 0;
      byDestinatario.forEach(entry => {
        rows.push([String(stt++), entry.niss, entry.nome, entry.numeroConta, entry.iban, '$' + entry.total.toFixed(2)]);
        totalPagamentu = Math.round((totalPagamentu + entry.total) * 100) / 100;
      });

      (pdf as any).autoTable({
        startY: 53,
        head: [['No', 'NISS', 'Naran Funsionáriu', 'No. Konta Bankária', 'No. IBAN', 'Total Paga']],
        body: rows,
        foot: [['', '', '', '', 'Total Pagamentu', '$' + totalPagamentu.toFixed(2)]],
        // [PT] O total sai uma unica vez, no fim da lista deste banco. Por omissao o
        // jspdf-autotable usa showFoot 'everyPage', pelo que uma lista que ocupasse
        // varias paginas repetia 'Total Pagamentu' no fundo de cada uma -- e sempre com
        // o total INTEIRO do grupo, o que se lia como se cada pagina tivesse fechado contas.
        // [VI] Dong tong chi in mot lan, o cuoi danh sach cua ngan hang nay. Mac dinh
        // jspdf-autotable la showFoot 'everyPage', nen danh sach dai qua nhieu trang se
        // lap lai 'Total Pagamentu' o cuoi tung trang -- va luon la tong CA nhom, doc len
        // cu tuong moi trang da chot so rieng.
        showFoot: 'lastPage',
        theme: 'grid',
        columnStyles: { 5: { halign: 'right' } },
        // [PT] O nome do funcionario e o numero de conta sao alinhados a esquerda apenas nas
        // celulas de dados -- o cabecalho fica centrado como o das restantes colunas. Nao da
        // para usar columnStyles aqui: essa opcao tem prioridade sobre headStyles e arrastaria
        // tambem o cabecalho para a esquerda.
        // [VI] Ten nhan vien va so tai khoan chi can trai o phan du lieu -- tieu de van can
        // giua nhu cac cot khac. Khong dung columnStyles duoc: no uu tien hon headStyles nen
        // se keo ca tieu de sang trai.
        didParseCell: (data: any) => {
          if (data.section === 'body' && (data.column.index === 2 || data.column.index === 3)) {
            data.cell.styles.halign = 'left';
          }
        },
        headStyles: {
          fillColor: [42, 129, 204],
          textColor: [0, 0, 0],
          fontSize: 8,
          padding: 0,
          valign: 'middle',
          halign: 'center',
        },
        bodyStyles: {
          textColor: [0, 0, 0],
          fontSize: 10,
          padding: 0,
          valign: 'middle',
          halign: 'center',
        },
        footStyles: {
          fillColor: [255, 255, 255],
          textColor: [0, 0, 0],
          fontSize: 10,
          fontStyle: 'bold',
          halign: 'right',
        }
      })

      const finalY = (pdf as any).lastAutoTable?.finalY ?? 53;
      const today = new Date();
      pdf.setFontSize(10);
      pdf.setTextColor(0);
      pdf.text(`Dili, ${today.getDate()} de ${monthKeysPT[today.getMonth()]} de ${today.getFullYear()}`, 20, finalY + 15);

      pdf.text('Visto husi', 30, finalY + 35);
      pdf.text('Agus Berek', 20, finalY + 55);
      pdf.text('Director do Departamento Financeiro', 20, finalY + 60);

      pdf.text('Aprova husi', 130, finalY + 35);
      pdf.text('Ana Romana Freitas Li', 120, finalY + 55);
      pdf.text('Diretora Executiva de INSS', 120, finalY + 60);
    });

    window.open(URL.createObjectURL(pdf.output("blob")));
  }

  public gerarPDFComprovativo(doc: string) {
    // Open PDF document in browser's new tab
    const arrayBuffer = base64ToArrayBuffer(doc);
    const blob = new Blob([arrayBuffer], { type: 'application/pdf' });
    window.open(URL.createObjectURL(blob));
  }

  public sumTotalValue(): number {
    if (this.totalselectedBancarios == 0 && this.totalselectedMovimentos < 0) {
      return Math.round(this.totalselectedMovimentos * 100) / 100;
    }
    else if (this.totalselectedBancarios == 0 && this.totalselectedMovimentos > 0) {
      return -(Math.round(this.totalselectedMovimentos * 100) / 100);
    }
    else
      return Math.round((this.totalselectedBancarios - this.totalselectedMovimentos) * 100) / 100;
  }

  private buildPermissions(permissions: ConciliarMovimentosPermissionsListResponse): void {

    if (permissions.selectMovimentosTypePermission == 2)
      this.selectMovimentosTypePermission = true;
    else
      this.selectMovimentosTypePermission = false;
    if (permissions.addEditMovimentosPermission == 2)
      this.addEditMovimentosPermission = true;
    else
      this.addEditMovimentosPermission = false;
    if (permissions.addEditMovimentosBancariosPermission == 2)
      this.addEditMovimentosBancariosPermission = true;
    else
      this.addEditMovimentosBancariosPermission = false;
    if (permissions.viewSelectedToConciliatePermission == 1)
      this.viewSelectedToConciliatePermission = true;
    else
      this.viewSelectedToConciliatePermission = false;
    if (permissions.conciliatePermission == 2)
      this.conciliatePermission = true;
    else
      this.conciliatePermission = false;
    if (permissions.undoConciliationPermission == 2)
      this.undoConciliationPermission = true;
    else
      this.undoConciliationPermission = false;
  }

  public scroll(e: any) {
    e._body.nativeElement.scrollIntoView({ behavior: "smooth", block: "start" });
  }

  public selectHandlerDespesasAConciliar(selected: number) {
    this.despesasClassificacaoSelection.toggle(selected);
    if (this.despesasClassificacaoSelection.isSelected(selected)) {
      if (this.movimentosDisplayType == SelectType.single)
        this.selectedDespesas = [];

      this.selectedDespesas.push(selected);
    } else {
      this.selectedDespesas = this.selectedDespesas.filter(x => x != selected);
    }
  }

  public classificarDespesaContabilistica() {
    const dialogRef = this.classificarContabilisticaDialog.open(PopUpClassificacaoContabilisticaComponent, {
      id: 'executarPagamentos',
      minHeight: '500px',
      width: '80%',
      height: '70%',
      panelClass: 'modalWithBorder',
      data: {
        listaDespesaAExecutar: this.selectedDespesas,
        tarefaActivoId: this.tarefaActivoId,
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        openSnackBar(this.translate.instant('snackBar.registoContabilistico'), this.snackBar);
        this.showLoader();
        this.selectedDespesas = [];
        this.despesasClassificacaoSelection.clear();
        this.getMovimentosTable();
      }
    });
  }
}
