import { Component, Inject, OnInit } from "@angular/core";
import { MatDialog } from '@angular/material/dialog';
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { NgxSpinnerService } from "ngx-spinner";
import { faTimesCircle } from '@fortawesome/free-solid-svg-icons';
import { TranslateService } from "@ngx-translate/core";
import { MyErrorStateMatcher } from "src/app/matcher";
import { TokenStorageService } from "src/app/services/token-storage.service";
import { blobExcelSaveAs, customCurrencyMaskConfig, formatDatePT, JsPdf_centerText, openErrorsDialog, openSnackBar, RegexPatterns, showExpiredError } from "src/app/utils";
import { DespesaCabimentadasParaExecucao, DespesaRegistada } from "src/app/models/despesaRegistada";
import { Destinatario } from "src/app/models/destinatario";
import { PagamentoExecutado, PagamentoExecutadoDestinatario } from "src/app/models/pagamentos_executados";
import { GetDestinatarioRequest, SaveDestinatarioRequest } from "src/app/request-models/destinatario-request";
import { DestinatarioService } from "src/app/services/destinatario.service";
import { MatSnackBar } from "@angular/material/snack-bar";
import { PagamentoExecutadoService } from "src/app/services/pagamentoExecutado.service";
import { EditPagamentoExecutadoRequest, GetDestinatarioPagamentoRequest, GetPagamentoExecutadoRequest, SavePagamentoExecutadoRequest } from "src/app/request-models/pagamentoExecutado-request";
import { forkJoin } from "rxjs";
import { GetAllDespesaRegistadaRequest } from "src/app/request-models/componenteDespesaRegisto-request";
import { ComponenteDespesaRegistoService } from "src/app/services/componenteDespesaRegisto.service";
import { faFileExcel } from '@fortawesome/free-solid-svg-icons';
import { PopUpWarningComponent } from "src/app/componentes/pop-up-warning/pop-up-warning.component";
import * as XLSX from 'xlsx';
import { Despesa } from "src/app/models/despesa";
import { ComponenteDespesaConfig } from "src/app/models/componenteDespesaConfig";
import jsPDF from "jspdf";
import { environment } from "src/environments/environment";
import { CodigoConta } from "src/app/models/codigoConta";
import { GetComponenteOrcamentoRegistoAprovadoRequest } from "src/app/request-models/componenteOrcamentoRegisto-request";
import { componenteOrcamentoRegistoService } from "src/app/services/componenteOrcamentoRegisto.service";
import { Router } from "@angular/router";
import { ExcelImporterPopupDestinatarioComponent } from "src/app/componentes/excel-importer/excel-importer-popups/excel-importer-popup-destinatarios/excel-importer-popup-destinatarios.component";




export interface PopUpExecutarPagamentosData {
  listaDespesaAExecutar: DespesaCabimentadasParaExecucao[];
  naoExisteOrcamentoAprovado: boolean;
  numPagamento: string;
  listaPagamentosDestinatario: PagamentoExecutadoDestinatario[];
  tarefaActivoId: number;
  processoId: number;
  totalValorCabimentado: number;
  totalValorExecutado: number;
  totalValorFaltaExecutar: number;
  totalValorDestinatarioExecutado: number;
  componenteDespesaConfig: ComponenteDespesaConfig;
  despesaCabimentada: DespesaRegistada[];
}

@Component({
  selector: 'app-pop-up-executar-pagamentos',
  templateUrl: './pop-up-executar-pagamentos.component.html',
  styleUrls: ['./pop-up-executar-pagamentos.component.css']
})
export class PopUpExecutarPagamentosComponent implements OnInit {

  public importerId?: string;
  public selectedDests?: number;
  public totalAmount?: number;

  public ibanIsNacional: boolean = true;

  public faTimesCircle = faTimesCircle;
  public errors: string[] = [];
  public matcher: MyErrorStateMatcher = new MyErrorStateMatcher();
  public submittedTry: boolean = false;
  public currencyOptions = customCurrencyMaskConfig;
  public faFileExcel = faFileExcel;
  public availableRegex = RegexPatterns;
  public isIban = true;
  public dataregisto: Date = new Date();

  public destinatario = <Destinatario>{};
  public pagamento = <PagamentoExecutado>{};
  public listaPagamentosExecutadosDestinatario: PagamentoExecutadoDestinatario[] = [];
  public totalPagamentoDestinatario: number = 0;
  public bankOptions: { key: string; label: string }[] = [];


  public displayedColumnsDespesasAExecutar: string[] = ['descricao', 'valor', 'valorExecutado', 'faltaExecutar'];
  public displayedColumnsDestinatarios: string[] = ['destinatario', 'banco', 'valorExecutado', 'accoes', 'exportToExcel'];
  public displayedColumnsExcel: string[] = ['destinatario', 'tin', 'niss', 'numPagamento', 'estado', 'descricaoDespesa', 'valorExecutado'];
  public existeDestinatarioBD: boolean = true;

  //Contabilidade
  public contabilidade = <CodigoConta>{};
  public contabilidadeDebito = <CodigoConta>{};
  public contabilidadeListagem: CodigoConta[] = [];
  public filtersContabilidade: number[] = [];
  public filtersContabilidadeFilteredCredito: CodigoConta[] = [];
  public filtersContabilidadeFilteredDebito: CodigoConta[] = [];

  constructor(
    public warningDialog: MatDialog,
    public spinner: NgxSpinnerService,
    public errorDialog: MatDialog,
    private tokenStorage: TokenStorageService,
    public _snackBar: MatSnackBar,
    public translate: TranslateService,
    public destinatarioService: DestinatarioService,
    public dialogRef: MatDialogRef<PopUpExecutarPagamentosComponent>,
    public pagamentoExecutadoService: PagamentoExecutadoService,
    public componenteDespesaService: ComponenteDespesaRegistoService,
    private orcamentoService: componenteOrcamentoRegistoService,
    private router: Router,
    @Inject(MAT_DIALOG_DATA) public data: PopUpExecutarPagamentosData
  ) {
  }

  ngOnInit(): void {

    if (!this.tokenStorage.getToken()) {
      this.router.navigate([''])
    }
    else if (this.tokenStorage.getToken() && !this.tokenStorage.tokenExpired()) {

      this.translate.get('guiaPagamentoListagem.lstBankCode').subscribe((res: any) => {
        this.bankOptions = Object.keys(res).map((key) => ({
          key,
          label: res[key],
        }));
      });

      //obter configuração codigos
      this.getOrcamentoAprovado(this.data.tarefaActivoId);
    }
    else {
      showExpiredError(this.errorDialog, this.tokenStorage, this.translate);
    }

  }

  public novoDestinatario() {
    this.showLoader();
    let request = <SaveDestinatarioRequest>{
      destinatario: this.destinatario
    };

    this.destinatarioService.SaveDestinatario(request).subscribe(x => {
      openSnackBar(this.translate.instant('snackBar.registoDestinatario'), this._snackBar);
      this.hideLoader();

    },
      err => {

        this.hideLoader();
        err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
        this.showError();
      });
  }

  public pesquisarDestinatario() {
    this.showLoader();
    let request = <GetDestinatarioRequest>{
      niss: this.destinatario.niss,
      tin: this.destinatario.tin,
      nome: this.destinatario.nome
    };

    this.destinatarioService.GetDestinatarioByNissTin(request).subscribe(x => {
      this.destinatario = x.destinatario;
      this.existeDestinatarioBD = true;
      this.hideLoader();

    },
      err => {
        this.existeDestinatarioBD = false;
        this.hideLoader();
        err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
        this.showError();
      });
  }

  public submitPagamento() {
    this.submittedTry = true;
  }

  public associarDespesa() {
    if (!this.pagamento.compromissoFk) return;
    this.showLoader();


    let faltaExecutar = this.data.listaDespesaAExecutar.filter(u => u.id == this.pagamento.compromissoFk)[0].faltaExecutar;

    if ((!!this.importerId ? this.totalAmount! : this.pagamento.valorExecutado) > faltaExecutar) {

      this.hideLoader();
      this.errors.push(this.translate.instant('pagamentosExecutados.valorNaoSuperiorError'));
      this.showError();
    }
    else {

      this.pagamento.numeroPagamento = this.data.numPagamento;
      this.pagamento.processoId = this.data.processoId;
      this.pagamento.codigoContaCredito = this.contabilidade.id;
      this.pagamento.codigoContaDebito = this.contabilidadeDebito.id;
      this.pagamento.dataObrigacao = this.dataregisto;
      let request = <SavePagamentoExecutadoRequest>{
        destinatario: this.destinatario,
        pagamento: this.pagamento,
        importId: this.importerId,
      };

      // [PT] Um IBAN nacional contem o proprio numero de conta (TL38 + 3 do banco + 14 da conta
      // + 2 de controlo), por isso os dois campos podem ser confrontados um com o outro. Apareceram
      // na Ordem de Pagamento contas que eram na verdade o NISS do funcionario: o dinheiro sai para
      // o sitio errado e o documento nao tem como o detectar sozinho. Avisa-se e deixa-se decidir --
      // nao se bloqueia, para nao travar um pagamento legitimo por causa de um IBAN estrangeiro ou
      // de um formato que ainda nao conhecemos.
      // [VI] IBAN noi dia chua san so tai khoan (TL38 + 3 so ngan hang + 14 so tai khoan + 2 so kiem
      // tra), nen doi chieu duoc hai o voi nhau. Da xuat hien tren Ordem de Pagamento nhung so tai
      // khoan thuc chat la NISS cua nhan vien: tien di sai cho ma ban than tai lieu khong tu phat
      // hien duoc. Chi canh bao roi de nguoi dung quyet -- khong chan, de khong lam ket mot khoan chi
      // hop le chi vi IBAN nuoc ngoai hoac mot dinh dang ta chua biet.
      const contaNoIban = this.contaDentroDoIban();
      if (contaNoIban !== null) {
        this.confirmarContaDiferenteDoIban(request, contaNoIban);
        return;
      }

      this.executeSavePagamento(request);
    }

  }


  // [PT] Devolve o numero de conta que vem dentro do IBAN quando ele NAO corresponde ao numero
  // de conta introduzido; devolve null quando corresponde, ou quando nao ha nada seguro a
  // comparar (importacao por Excel, IBAN estrangeiro, campos por preencher).
  // [VI] Tra ve so tai khoan nam trong IBAN khi no KHONG khop voi so tai khoan da nhap; tra ve
  // null khi khop, hoac khi khong co gi de so sanh mot cach chac chan (nhap tu Excel, IBAN nuoc
  // ngoai, o con trong).
  private contaDentroDoIban(): string | null {
    if (!!this.importerId) { return null; }

    const conta = (this.pagamento.numeroConta || '').trim();
    const iban = (this.pagamento.iban || '').trim().toUpperCase();
    if (!conta || !iban) { return null; }

    // So o IBAN nacional tem uma estrutura conhecida. [VI] Chi IBAN noi dia moi co cau truc da biet.
    if (!new RegExp('^' + this.availableRegex.IBANPattern + '$').test(iban)) { return null; }

    const contaNoIban = iban.substring(7, 21);          // TL38 + 3 do banco = 7 caracteres
    const contaNormalizada = ('00000000000000' + conta).slice(-14);

    return contaNoIban === contaNormalizada ? null : contaNoIban.replace(/^0+/, '');
  }

  private confirmarContaDiferenteDoIban(request: SavePagamentoExecutadoRequest, contaNoIban: string): void {
    this.hideLoader();
    const dialogRef = this.warningDialog.open(PopUpWarningComponent, {
      id: 'contaNaoCorrespondeIban',
      minHeight: '300px',
      width: '40%',
      height: '30%',
      panelClass: 'warningModal',
      data: {
        function: undefined,
        msg: this.translate.instant('warnings.contaNaoCorrespondeIbanMsg', {
          conta: (this.pagamento.numeroConta || '').trim(),
          contaIban: contaNoIban
        })
      }
    });

    dialogRef.afterClosed().subscribe(confirmou => {
      if (confirmou) {
        this.showLoader();
        this.executeSavePagamento(request);
      }
    });
  }

  private executeSavePagamento(request: SavePagamentoExecutadoRequest) {
    this.pagamentoExecutadoService.SavePagamentoExecutado(request).subscribe(x => {
      this.onAssociarDespesaSuccess(request);
    },
      err => {
        this.hideLoader();

        const errors = err.error?.errors;
        const errorCode = errors && errors.length > 0 ? errors[0].errorCode : undefined;

        if (errorCode === '-79' && !request.pagamento.confirmDuplicate) {
          const confirmRequest = <SavePagamentoExecutadoRequest>{
            ...request,
            pagamento: { ...request.pagamento, confirmDuplicate: true }
          };

          const dialogRef = this.warningDialog.open(PopUpWarningComponent, {
            id: 'confirmDuplicateObrigacao',
            minHeight: '300px',
            width: '40%',
            height: '30%',
            panelClass: 'warningModal',
            data: {
              function: this.pagamentoExecutadoService.SavePagamentoExecutado(confirmRequest),
              msg: this.translate.instant('warnings.possivelObrigacaoDuplicada')
            }
          });

          dialogRef.afterClosed().subscribe(result => {
            if (result) {
              this.onAssociarDespesaSuccess(confirmRequest);
            }
          });
        }
        else {
          errors ? errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
          this.showError();
        }
      });
  }

  private onAssociarDespesaSuccess(request: SavePagamentoExecutadoRequest) {
    this.destinatario = <Destinatario>{};
    this.pagamento = <PagamentoExecutado>{};
    this.submittedTry = false;
    this.importerId = undefined;
    this.selectedDests = undefined;
    this.totalAmount = undefined;
    this.getDespesaCabimentasdasParaExecucao();
    this.getDestinatariosPagamento();
    openSnackBar(this.translate.instant(`snackBar.${!!request.importId ? 'registoDestinatarios' : 'registoDestinatario'}`), this._snackBar);
    this.hideLoader();
  }

  public getDespesaCabimentasdasParaExecucao() {
    this.showLoader();
    let request = <GetAllDespesaRegistadaRequest>{
      tarefaAtivoId: this.data.tarefaActivoId
    }
    this.componenteDespesaService.GetAllDespesaCabimentadasParaExecucaoByTarefaAtivoId(request).subscribe(x => {
      this.data.listaDespesaAExecutar = x.despesasParaExecucao;

      this.data.totalValorExecutado = 0;
      this.data.totalValorCabimentado = 0;
      this.data.totalValorFaltaExecutar = 0;
      if (this.data.listaDespesaAExecutar != null && this.data.listaDespesaAExecutar.length > 0) {
        this.data.listaDespesaAExecutar.forEach(element => {
          this.data.totalValorExecutado = Math.round((this.data.totalValorExecutado + element.valorExecutado) * 100) / 100;
          this.data.totalValorCabimentado = Math.round((this.data.totalValorCabimentado + element.valorCabimentado) * 100) / 100;
          this.data.totalValorFaltaExecutar = Math.round((this.data.totalValorFaltaExecutar + element.faltaExecutar) * 100) / 100;
        });
      }
      this.hideLoader();

    },
      err => {
        this.existeDestinatarioBD = false;
        this.hideLoader();
        err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
        this.showError();
      });
  }

  public getDestinatariosPagamento() {
    this.showLoader();
    let request = <GetDestinatarioPagamentoRequest>{
      numPagamento: this.data.numPagamento
    }
    this.pagamentoExecutadoService.GetDestinatariosPagamentoByNumPagamento(request).subscribe(x => {
      this.data.listaPagamentosDestinatario = x.destinatarioPagamentosExecutados;

      this.data.totalValorDestinatarioExecutado = 0;
      if (this.data.listaPagamentosDestinatario != null && this.data.listaPagamentosDestinatario.length > 0) {
        this.data.listaPagamentosDestinatario.forEach(element => {
          this.data.totalValorDestinatarioExecutado = Math.round((this.data.totalValorDestinatarioExecutado + element.valorExecutado) * 100) / 100;
        });
      }
      this.hideLoader();

    },
      err => {
        this.existeDestinatarioBD = false;
        this.hideLoader();
        err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
        this.showError();
      });
  }

  public editarDestinatario(element: PagamentoExecutadoDestinatario) {
    this.importerId = undefined;
    this.selectedDests = undefined;
    this.totalAmount = undefined;

    this.destinatario = element.destinatario;
    this.pagamento.id = element.id;
    this.pagamento.numeroPagamento = element.numeroPagamento;
    this.pagamento.valorExecutado = element.valorExecutado;
    this.pagamento.estado = element.estado;
    if (element.iban)
      this.isIban = true;
    else
      this.isIban = false;
    this.pagamento.numeroConta = element.numeroConta;
    this.pagamento.iban = element.iban;
    this.pagamento.swift = element.swift;
    this.pagamento.bankCode = element.bankCode;
    this.pagamento.processoId = element.processoId;
    this.pagamento.destinatarioFk = element.destinatario.id;
    this.pagamento.compromissoFk = element.compromissoFk;
    this.contabilidade.id = element.codigoContaCredito ?? <number>{};
    this.contabilidadeDebito.id = element.codigoContaDebito ?? <number>{};
    this.dataregisto = element.dataObrigacao ?? <Date>{};
    this.ibanIsNacional = new RegExp(this.availableRegex.IBANPattern).test(this.pagamento.iban || '');
    document.getElementById("telaDestinatario")?.scrollIntoView({ behavior: "smooth" });
    this.updateContabilidadeDebito(element.codigoContaCredito ?? <number>{});
    this.updateContabilidadeCredito(element.codigoContaDebito ?? <number>{});
  }

  public deleteComponente(element: PagamentoExecutadoDestinatario) {
    const dialogRef = this.warningDialog.open(PopUpWarningComponent, {
      id: 'deletePagamento',
      minHeight: '300px',
      width: '40%',
      height: '30%',
      panelClass: 'warningModal',
      data: { function: this.pagamentoExecutadoService.DeletePagamento({ id: element.id }), msg: this.translate.instant('warnings.deletePagamento') }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.getDestinatariosPagamento();
        this.getDespesaCabimentasdasParaExecucao();
        openSnackBar(this.translate.instant('snackBar.removePagamento'), this._snackBar);
      }
    });

  }

  public getPagamentosDestinatario(pagamentosDestinatario: PagamentoExecutadoDestinatario) {
    this.showLoader();
    let request = <GetPagamentoExecutadoRequest>{
      idDestinatario: pagamentosDestinatario.destinatario.id
    }
    this.pagamentoExecutadoService.GetPagamentosExecutadosByIdDestinatario(request).subscribe(x => {

      this.hideLoader();
      blobExcelSaveAs(x.excelExtraido, this.translate.instant('pagamentosExecutados.pagamentosExecutados').replace(' ', '_') + '_' + x.destinatarioPagamentosExecutados[0].destinatario.nome);

    },
      err => {
        this.existeDestinatarioBD = false;
        this.hideLoader();
        err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
        this.showError();
      });
  }

  public emitirOrdemPagamento() {
    var listaIdPagamento: number[] = [];
    this.data.listaPagamentosDestinatario.forEach(element => {
      listaIdPagamento.push(element.id);
    });
    this.showLoader();
    let request = <EditPagamentoExecutadoRequest>{
      listaIdPagamento: listaIdPagamento
    };

    this.pagamentoExecutadoService.EditPagamentoExecutado(request).subscribe(x => {
      openSnackBar(this.translate.instant('snackBar.emitirOrdemPagamento'), this._snackBar);

      this.gerarPDF(this.data.listaPagamentosDestinatario);
      this.closePopUp();
      this.hideLoader();

    },
      err => {

        this.hideLoader();
        err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
        this.showError();
      });
  }

  public closePopUp(): void {
    this.dialogRef.close(true);
  }

  public onImporterComplete({ id, data }: any) {
    const selected = data.Success + (data.Existing || 0);
    if (!selected) return;

    this.importerId = id;
    this.selectedDests = selected;
    this.totalAmount = data.TotalAmount;

    this.destinatario = <Destinatario>{};
    this.pagamento = <PagamentoExecutado>{};

  }

  public removeSelectedImported() {
    this.importerId = undefined;
    this.selectedDests = undefined;
    this.totalAmount = undefined;
  }

  public gerarPDF(element: PagamentoExecutadoDestinatario[]): void {

    var pdf = new jsPDF();
    const monthKeysPT = ['Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho', 'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'];

    // Agrupar por Banco + Mês/Ano (dataObrigacao)
    const groups = new Map<string, PagamentoExecutadoDestinatario[]>();
    element.forEach(row => {
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

      //title (mantido em Tetun, documento oficial fixo)
      JsPdf_centerText(pdf, 'Lista Pagamentu Saláriu Funcionáriu INSS', 35);
      pdf.setFontSize(12);
      pdf.setTextColor(99);

      //Fulan (Mês) + Banco + Tinan (Ano)
      JsPdf_centerText(pdf, `Fulan ${monthLabel} ${bankLabel} Tinan ${year}`, 43);

      // Somar valor por destinatário dentro do grupo (mesmo destinatário pode ter várias despesas no mesmo banco/mês)
      const byDestinatario = new Map<number, { nome: string; niss: string; numeroConta: string; iban: string; total: number }>();
      rowsForGroup.forEach(row => {
        const id = row.destinatario.id;
        if (!byDestinatario.has(id)) {
          byDestinatario.set(id, { nome: row.destinatario.nome, niss: row.destinatario.niss ?? '', numeroConta: row.numeroConta ?? '', iban: row.iban ?? '', total: 0 });
        }
        const entry = byDestinatario.get(id)!;
        entry.total = Math.round((entry.total + row.valorExecutado) * 100) / 100;
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

      // Assinaturas: Visto husi (esquerda) + Aprova husi (direita) — apenas texto, sem carimbo/assinatura em imagem
      pdf.text('Visto husi', 30, finalY + 35);
      pdf.text('Agus Berek', 20, finalY + 55);
      pdf.text('Director do Departamento Financeiro', 20, finalY + 60);

      pdf.text('Aprova husi', 130, finalY + 35);
      pdf.text('Ana Romana Freitas Li', 120, finalY + 55);
      pdf.text('Diretora Executiva de INSS', 120, finalY + 60);
    });

    // Open PDF document in browser's new tab
    window.open(URL.createObjectURL(pdf.output("blob")));

    //pdf.output('dataurlnewwindow');

    // Download PDF doc
    //pdf.save('GuiaPagamento.pdf');
  }



  public getBankLabel(bankCode: string): string {
    return this.bankOptions.find(b => b.key === bankCode)?.label ?? '-';
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

  public scroll(e: any) {
    e._body.nativeElement.scrollIntoView({ behavior: "smooth", block: "start" });
  }

  public keyPressNumbers(event: any) {
    var charCode = (event.which) ? event.which : event.keyCode;
    // Only Numbers 0-9
    if ((charCode < 48 || charCode > 57)) {
      event.preventDefault();
      return false;
    } else {
      return true;
    }
  }

  public getOrcamentoAprovado(idTarefaActivo: number) {
    this.showLoader();
    let request = <GetComponenteOrcamentoRegistoAprovadoRequest>{
        idTarefaActivo: idTarefaActivo,
    };
    this.orcamentoService.GetOrcamentoAprovadoReceitaByIdTarefaActivo(request).subscribe(x => {
        if (x != null && x.existeOrcamentoAprovado) {
            this.contabilidadeListagem = x.codigoConta;
            this.filtersContabilidadeFilteredCredito = x.codigoConta;
            this.filtersContabilidadeFilteredDebito = x.codigoConta;
        }
        this.hideLoader();

    },
        err => {

            this.hideLoader();
            err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
            this.showError();
        });

  }
  public filterCustomOptionsContabilidadeCredito(event: any) {
    this.filtersContabilidadeFilteredCredito = this.contabilidadeListagem.filter(p => p.designacao.toLowerCase().startsWith(event.toLowerCase()));
  }

  public filterCustomOptionsContabilidadeDebito(event: any) {
      this.filtersContabilidadeFilteredDebito = this.contabilidadeListagem.filter(p => p.designacao.toLowerCase().startsWith(event.toLowerCase()));
  }

  public updateContabilidadeDebito(contabilidadeId: number){
    this.filtersContabilidadeFilteredDebito = this.contabilidadeListagem.filter(p => p.id != contabilidadeId);
  }

  public updateContabilidadeCredito(contabilidadeId: number){
      this.filtersContabilidadeFilteredCredito = this.contabilidadeListagem.filter(p => p.id != contabilidadeId);
  }

  // public changeIban(): void {
  //   if (this.isIban)
  //     this.pagamento.numeroConta = undefined;
  //   else {
  //     this.pagamento.iban = undefined;
  //     this.pagamento.swift = undefined;
  //   }
  // }
}
