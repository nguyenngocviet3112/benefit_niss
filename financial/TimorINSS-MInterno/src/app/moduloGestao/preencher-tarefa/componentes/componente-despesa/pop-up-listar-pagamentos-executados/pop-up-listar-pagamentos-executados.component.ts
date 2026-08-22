import { Component, Inject, OnInit } from "@angular/core";
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateService } from "@ngx-translate/core";
import { TokenStorageService } from "src/app/services/token-storage.service";
import { blobExcelSaveAs, customCurrencyMaskConfig, formatDatePT, JsPdf_centerText, openErrorsDialog, openSnackBar, showExpiredError } from "src/app/utils";
import { PagamentoExecutadoService } from "src/app/services/pagamentoExecutado.service";
import jsPDF from "jspdf";
import { environment } from "src/environments/environment";
import { Router } from "@angular/router";
import { faFilePdf } from "@fortawesome/free-solid-svg-icons";
import { ListaPagamentosDoProcesso } from "src/app/models/pagamentos_executados";
import { NgxSpinnerService } from "ngx-spinner";
import { GetDestinatarioPagamentoRequest, ListagemPagamentosProcessoRequest } from "src/app/request-models/pagamentoExecutado-request";

export interface PopUpListagemPagamentosData {
  processoAtivoId: number;
}

@Component({
  selector: 'app-pop-up-listar-pagamentos-executados',
  templateUrl: './pop-up-listar-pagamentos-executados.component.html',
  styleUrls: ['./pop-up-listar-pagamentos-executados.component.css']
})


export class PopUpListarPagamentosExecutadosComponent implements OnInit {
  public errors: string[] = [];
  public faFilePdf = faFilePdf;
  public listaPagamentos: ListaPagamentosDoProcesso[]=[];
  public displayedColumnsPagamentosExecutados: string[] = ['numPagamento', 'valor', 'pdf'];
  public listaPagamentosDetails: ListaPagamentosDoProcesso[]=[];
  public bankOptions: { key: string; label: string }[] = [];


  constructor(
    private tokenStorage: TokenStorageService,
    private router: Router,
    public errorDialog: MatDialog,
    public translate: TranslateService,
    public pagamentoExecutadoService: PagamentoExecutadoService,
    private spinner: NgxSpinnerService,
    @Inject(MAT_DIALOG_DATA) public data: PopUpListagemPagamentosData

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
      this.getListaPagamentosProcesso();

    }
    else {
      showExpiredError(this.errorDialog, this.tokenStorage, this.translate);
    }

  }

  public getListaPagamentosProcesso() {
    this.showLoader();
    let request = <ListagemPagamentosProcessoRequest>{
      processoAtivo: this.data.processoAtivoId,
    };

    this.pagamentoExecutadoService.GetListaPagamentosProcesso(request).subscribe(x => {
      this.listaPagamentos = x.pagamentos;
      this.hideLoader();
    },
      err => {

        this.hideLoader();
        err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
        this.showError();
      });
  }


  public getPagamentoDetails(numPagamento: string): void {

    this.showLoader();
    let request = <GetDestinatarioPagamentoRequest>{
      numPagamento: numPagamento,
    };

    this.pagamentoExecutadoService.GetPagamentoDetails(request).subscribe(x => {
      this.listaPagamentosDetails = x.pagamentos;
      if(this.listaPagamentosDetails != null && this.listaPagamentosDetails.length > 0){
        this.gerarPDF(this.listaPagamentosDetails, numPagamento);

      }
      this.hideLoader();
    },
      err => {

        this.hideLoader();
        err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
        this.showError();
      });

  }

  public gerarPDF(pdfList: ListaPagamentosDoProcesso[], numPagamento: string){

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

      //title (mantido em Tetun, documento oficial fixo)
      JsPdf_centerText(pdf, 'Lista Pagamentu Saláriu Funcionáriu INSS', 35);
      pdf.setFontSize(12);
      pdf.setTextColor(99);

      //Fulan (Mês) + Banco + Tinan (Ano)
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
}
