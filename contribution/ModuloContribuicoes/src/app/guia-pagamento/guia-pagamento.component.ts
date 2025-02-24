import { DatePipe, DecimalPipe } from "@angular/common";
import { Component, OnInit } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import { Router } from "@angular/router";
import { TranslateService } from "@ngx-translate/core";
import { NgxSpinnerService } from "ngx-spinner";
import { forkJoin } from "rxjs";
import { PopUpWarningComponent } from "../componentes/pop-up-warning/pop-up-warning.component";
import { GetAllGuiasStatesFromYearByFilterRequest } from "../request-models/guiaPagamento-request";
import { FilterRequest } from "../request-models/utils-request";
import { DominioDescricaoString } from "../response-models/dominios-response";
import { GuiaListagem } from "../response-models/guiaPagamento-response";
import { DominiosService } from "../services/dominios.service";
import { GuiaPagamentoService } from "../services/guiaPagamento.service";
import { TokenStorageService } from "../services/token-storage.service";
import { formatDate, formatDatePT, formatDecimal, openErrorsDialog } from "../utils";
import { faTimesCircle } from "@fortawesome/free-solid-svg-icons";
import jsPDF from "jspdf";
import { environment } from "src/environments/environment";
import { ReservaCreditoListagemRequest } from "../request-models/reservaCredito-request";
import { ReservaCreditoService } from "../services/reservaCredito.service";
import { PopUpComprovativoPagamentoComponent } from "../pop-up-comprovativo-pagamento/pop-up-comprovativo-pagamento.component";

@Component({
    selector: 'guiaPagamento',
    templateUrl: './guia-pagamento.component.html',
    styleUrls: ['./guia-pagamento.component.css']
})
export class GuiaPagamentoComponent implements OnInit {

    public isLoggedIn = false;
    public faTimesCircle = faTimesCircle;
    public entidadeId: number = 0;
    public date?: Date;
    public filterBy = '';
    public filterField?: string = undefined;
    public errors: string[] = [];
    public errorMessage = "";
    private filter: FilterRequest = {};
    //Region Guia Pagamento table
    public dataSourceGuiaPagamento: GuiaListagem[] = [];
    public displayedColumnsGuiaPagamento: string[] = ['numDocumento', 'mesAno', 'descricao', 'valor', 'juros', 'total', 'dtValidade', 'tipo', 'valorPago', 'estadoPagamento', 'actions'];
    public totalRows: number = 0;
    public pageSize = 5;
    public pageIndex = 0;
    public guiaTipoOptions: DominioDescricaoString[] = [];
    public pagamentoTipoOptions: DominioDescricaoString[] = [];
    public tipoOption?: number = undefined;
    public situacaoOption?: number = undefined;
    public credit?: number;

    constructor(
        private tokenStorageService: TokenStorageService,
        private router: Router,
        private spinner: NgxSpinnerService,
        private datepipe: DatePipe,
        public errorDialog: MatDialog,
        public translate: TranslateService,
        public comprovativoDialog: MatDialog,
        public guiaPagamentoService: GuiaPagamentoService,
        public _snackBar: MatSnackBar,
        public decimalPipe: DecimalPipe,
        public dominiosService: DominiosService,
        private reservaCreditoService: ReservaCreditoService,
    ) {
    }

    public ngOnInit(): void {

        if (!this.tokenStorageService.getToken()) {
            this.router.navigate(['']);
        }
        else if (this.tokenStorageService.getToken() && this.tokenStorageService.tokenExpired()) {
            this.translate.get('error.expired').subscribe((translated: string) => {
                const dialogRef = this.errorDialog.open(PopUpWarningComponent, {
                    id: 'desvincularDialog',
                    minHeight: '300px',
                    width: '40%',
                    height: '30%',
                    panelClass: 'warningModal',
                    data: { msg: translated, noGenericMsg: true }
                });
                dialogRef.afterClosed().subscribe(() => {
                    this.tokenStorageService.signOut();
                    window.location.reload();
                });
            });
        }
        else {
            this.spinner.show();
            this.isLoggedIn = true;
            let idEntidade = this.tokenStorageService.getUser()?.idEntidade;
            let dominioTiposGuia = this.dominiosService.GetAllTiposGuia();
            let dominioTiposPagamento = this.dominiosService.GetAllTiposPagamento();

            forkJoin([dominioTiposGuia, dominioTiposPagamento])
                .subscribe(([dominioTiposGuia, dominioTiposPagamento]) => {
                    this.guiaTipoOptions = dominioTiposGuia.dominios;
                    this.pagamentoTipoOptions = dominioTiposPagamento.dominios;
                },
                    err => {
                        err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
                        this.showError();
                        this.spinner.hide();
                    });

            if (idEntidade != null) {
                this.entidadeId = idEntidade;
                this.getTableGuiaPagamento();
            }
        }
    }

    public getTableGuiaPagamento(filterType?: string, isDate?: boolean) {
        this.spinner.show();
        this.dataSourceGuiaPagamento = [];

        if (this.date != null) {
            this.filter.dateFilterBegin = this.date;
        }

        if (!isDate) {
            if (this.filter.filterField != null && this.filterField != filterType) {
                this.filter.filter = {
                    filterBy: this.filterBy,
                    filterField: this.filterField
                }
            } else {
                this.filter.filterBy = this.filterBy;
                this.filter.filterField = this.filterField;
            }
        }

        this.filter.index = this.pageIndex;
        this.filter.rows = this.pageSize;

        let request: GetAllGuiasStatesFromYearByFilterRequest;

        request = { "idEntidade": this.entidadeId, "filter": this.filter };

        this.guiaPagamentoService.getAllGuiasByEntidade(request).subscribe(x => {
            x.rows == null ? this.totalRows = 0 : this.totalRows = x.rows;
            x.guias == null ? this.dataSourceGuiaPagamento = [] : this.dataSourceGuiaPagamento = x.guias;
            this.spinner.hide();
        },
            err => {
                this.dataSourceGuiaPagamento = [];
                this.spinner.hide();
                err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
                this.showError();
            });
    }

    public showError() {
        const dialogRef = openErrorsDialog(this.errors, this.errorDialog);

        dialogRef.afterClosed().subscribe(result => {
            this.errors = [];
        });
    }

    public formatDecimal(number: number) {
        return formatDecimal(this.decimalPipe, number);
    }

    public formatDate(date: Date): string {
        return formatDate(this.datepipe, date);
    }

    public formatDatePT(date: Date): string {
        return formatDatePT(this.datepipe, date);
    }

    public dateUpdated() {
        this.getTableGuiaPagamento(undefined, true);
    }

    public updateTable(event: any) {
        this.pageIndex = event.pageIndex;
        this.pageSize = event.pageSize;
        this.getTableGuiaPagamento();
    }

    public applyTipoFilter(filter: string) {
        this.filterField = 'Tipo';
        this.filterBy = filter;
        this.pageSize = 5;
        this.pageIndex = 0;
        this.getTableGuiaPagamento('Tipo');
    }

    public applyPagamentoFilter(filter: string) {
        this.filterField = 'Estado';
        this.filterBy = filter;
        this.pageSize = 5;
        this.pageIndex = 0;
        this.getTableGuiaPagamento('Estado');
    }

    public clearFilterPagamento() {
        this.filter = {};
        this.filterBy = '';
        this.filterField = undefined;
        this.pageSize = 5;
        this.pageIndex = 0;
        this.tipoOption = undefined;
        this.situacaoOption = undefined;
        this.date = undefined;
        this.getTableGuiaPagamento();
    }

    public gerarPDF(element: any) {
        var pdf = new jsPDF();
        // INSS logo
        pdf.addImage(environment.ssIcon, 'JPEG', 90, 5, 25, 20);

        //title
        pdf.text(this.translate.instant('general.guiaDePagamento'), 83, 35);
        pdf.setFontSize(12);
        pdf.setTextColor(99);

        //Número do Documento
        pdf.text(this.translate.instant('guiaPagamentoListagem.numDocumento') + ': ' + element.numDocumento, 75, 45);

        //datas
        pdf.text(this.translate.instant('responsavel_legal.emitDate') + ': ' + this.formatDatePT(element.mesAno), 20, 55);
        pdf.text(this.translate.instant('conta_corrente.dataLimitePagamento') + ': ' + this.formatDatePT(element.dtValidade), 120, 55);

        var body = [[this.formatDatePT(element.mesAno), element.descricao, element.valor, element.juros, element.total, this.formatDatePT(element.dtValidade), this.guiaTipoOptions.find(x => x.value == element.tipo)?.descricao, element.valorPago],
        [{
            content: `TOTAL: ${this.formatDecimal(element.total)}`, colSpan: 8,
            styles: { fillColor: [42, 129, 204] }
        }]]

        {
            (pdf as any).autoTable({
                startY: 60,
                columnStyles: { europe: { halign: 'center' } },
                head: [[this.translate.instant('guiaPagamentoListagem.mesAno'), this.translate.instant('guiaPagamentoListagem.descricao'), this.translate.instant('guiaPagamentoListagem.valor'), this.translate.instant('guiaPagamentoListagem.juros'), this.translate.instant('guiaPagamentoListagem.total'), this.translate.instant('general.dtValidade'), this.translate.instant('guiaPagamentoListagem.type'), this.translate.instant('conta_corrente.valorPago')]],
                body: body,
                theme: 'grid',
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
                didDrawCell: (data: { column: { index: any; }; }) => {
                }

            })
        }

        // Download PDF doc
        pdf.save('table.pdf');
    }

    public openPaymentPopup(guia: GuiaListagem, viewMode: boolean = false): void {
        this.spinner.show();

        let filter: FilterRequest;
        filter = { filterBy: 'indActivo' };

        let request: ReservaCreditoListagemRequest;

        request = { "idEntidade": this.entidadeId, "filter": filter };

        this.reservaCreditoService.getReservaCreditoByIdEntidade(request).subscribe(x => {
            this.credit = x.reservaCredito[0]?.valor;
            this.spinner.hide();
            //open pup with or without valor
            const dialogRef = this.comprovativoDialog.open(PopUpComprovativoPagamentoComponent, {
                id: 'editarDocumento',
                minHeight: '100px',
                width: '70%',
                height: '50%',
                panelClass: 'modalWithBorder',
                data: {
                    adicionar: !viewMode,
                    view: viewMode,
                    data: { guiaId: guia.idGuia, entidadeId: this.entidadeId, valor: guia.valorPago, data: guia.dtValorPago, file: guia.comprovativoPagamento },
                    avaliableCredit: viewMode ? undefined : this.credit,
                }
            });

            dialogRef.afterClosed().subscribe(result => {
                if (result)
                    this.getTableGuiaPagamento();
            });
        },
            err => {
                this.credit = undefined;
                this.spinner.hide();
                err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
                this.showError();
            });

    }
}
