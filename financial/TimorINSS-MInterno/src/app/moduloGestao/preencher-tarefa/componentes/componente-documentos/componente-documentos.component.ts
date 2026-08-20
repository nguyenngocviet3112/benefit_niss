import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { faTimesCircle } from '@fortawesome/free-solid-svg-icons';
import { TranslateService } from '@ngx-translate/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { MyErrorStateMatcher } from 'src/app/matcher';
import { Documento } from 'src/app/models/documento';
import { DocumentosListagemRequest, TarefaDocumentoRequest } from 'src/app/request-models/documentos-request';
import { FilterRequest } from 'src/app/request-models/utils-request';
import { PopUpWarningComponent } from 'src/app/componentes/pop-up-warning/pop-up-warning.component';
import { GetTiposDocumentoPorTarefaAtivaRequest } from 'src/app/request-models/dominio-request';
import { DominiosComGrupos } from 'src/app/response-models/dominios-response';
import { DocumentoService } from 'src/app/services/documento.service';
import { DominiosService } from 'src/app/services/dominios.service';
import { TokenStorageService } from 'src/app/services/token-storage.service';
import { base64ArrayBuffer, openErrorSnackBar, openSnackBar, showExpiredError } from 'src/app/utils';

@Component({
  selector: 'app-componente-documentos',
  templateUrl: './componente-documentos.component.html',
  styleUrls: ['./componente-documentos.component.css']
})
export class ComponenteDocumentosComponent implements OnInit {

  @Input() isExpanded: boolean = false;
  @Input() tarefaActivoId: number = 0;
  @Output() refreshDocTable = new EventEmitter<boolean>();
  public matcher: MyErrorStateMatcher = new MyErrorStateMatcher();
  public wrongFormat: boolean = false;
  public documentosList: DominiosComGrupos = <DominiosComGrupos>{};
  public documento: Documento = <Documento>{};
  // [PT] Ficheiros já inseridos nesta sessão do ecrã. O snackbar de sucesso desaparece em segundos e o
  // campo era limpo, pelo que o utilizador ficava sem saber se a inserção tinha funcionado.
  // [VI] Các file đã chèn trong phiên làm việc này. Snackbar báo thành công tắt sau vài giây và ô file
  // bị xoá trắng, nên người dùng không biết đã chèn được hay chưa.
  public documentosInseridos: { nome: string, tipo: string }[] = [];
  private maxSize: number = 52428800;
  public accept = ".pdf";
  public faTimesCircle = faTimesCircle;
  private error = '';

  constructor(
    public translate: TranslateService,
    public _snackBar: MatSnackBar,
    private documentoService: DocumentoService,
    private dominiosService: DominiosService,
    private spinner: NgxSpinnerService,
    public errorDialog: MatDialog,
    public warningDialog: MatDialog,
    private tokenStorage: TokenStorageService
  ) { }

  ngOnInit(): void {
    this.showLoader();
    let request: GetTiposDocumentoPorTarefaAtivaRequest = {
      tarefaAtivoId: this.tarefaActivoId
    };
    this.dominiosService.GetTiposDocumentoPorTarefaAtiva(request).subscribe(x => {
      this.hideLoader();
      this.documentosList = x.dominio;
      // alert(JSON.stringify(this.documentosList));
      },
    err => {
      this.hideLoader();
      err.error?.errors ? err.error.errors.map((x : any) => this.error = x.errorCode) : this.error = '-1';
      openErrorSnackBar(this.translate.instant('error.'+ this.error), this._snackBar);
    });

  }

  // [PT] Chamado pelo <input type="file"> nativo. Mesmas regras de antes -- só PDF e até ao tamanho
  // máximo -- mas ligado ao input do browser em vez do ngx-mat-file-input, que não abria a janela
  // de selecção de ficheiro.
  // [VI] Được gọi bởi <input type="file"> gốc. Vẫn đúng luật cũ -- chỉ PDF và trong giới hạn dung
  // lượng -- nhưng gắn vào input của trình duyệt thay cho ngx-mat-file-input, thứ không mở nổi cửa
  // sổ chọn file.
  public onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input?.files && input.files.length > 0 ? input.files[0] : undefined;

    if (!file) {
      return;
    }

    if (file.size <= this.maxSize && file.type == 'application/pdf') {
      this.wrongFormat = false;
      this.documento.nomeDocumento = file.name;

      const reader = new FileReader();
      reader.onloadend = (evt) => {
        if (evt.target && evt.target.readyState == FileReader.DONE) {
          const arrayBuffer = evt.target.result;
          if (arrayBuffer instanceof ArrayBuffer) {
            this.documento.documento = base64ArrayBuffer(arrayBuffer);
          }
        }
      };
      reader.readAsArrayBuffer(file);
    }
    else {
      // Ficheiro recusado: limpar o que estava e deixar o input pronto para nova tentativa.
      // [VI] File bị từ chối: xoá thứ đang có và trả input về trạng thái sẵn sàng chọn lại.
      this.wrongFormat = true;
      this.documento.documento = "";
      this.documento.nomeDocumento = "";
      input.value = "";
    }
  }

  // [PT] Antes de inserir, verifica se a tarefa já tem um documento do mesmo tipo. Nada impede
  // inserir várias versões do mesmo tipo, mas até aqui era possível inserir o mesmo ficheiro
  // repetidamente sem qualquer aviso -- e como o campo era limpo a seguir, nem se percebia que já
  // lá estava. Agora avisa-se e deixa-se o utilizador decidir.
  // [VI] Trước khi chèn, kiểm tra tarefa đã có tài liệu cùng loại chưa. Không cấm nộp nhiều bản
  // cùng loại, nhưng trước đây chèn đi chèn lại cùng một file mà không có cảnh báo nào -- ô lại bị
  // xoá trắng ngay sau đó nên cũng không biết là đã có rồi. Nay cảnh báo và để người dùng tự quyết.
  public SaveDocument(): void {
    const request: TarefaDocumentoRequest = <TarefaDocumentoRequest>{};
    request.tarefaAtivoId = this.tarefaActivoId;
    request.tipoDocumento = this.documento.tpDocIdentificacao;
    request.documento = this.documento.documento;
    request.nomeDocumento = this.documento.nomeDocumento;

    // valida se está expirado o token antes da chamada
    if (this.tokenStorage.tokenExpired()) 
    {
      showExpiredError(this.errorDialog, this.tokenStorage, this.translate);
    }

    this.showLoader();

    const filtro: FilterRequest = { index: 0, rows: 1000 };
    const pedidoListagem: DocumentosListagemRequest = { id: this.tarefaActivoId, filter: filtro };

    this.documentoService.getDocumentosByIdTarefaAtivo(pedidoListagem).subscribe(listagem => {
        this.hideLoader();

        const tipoSeleccionado = this.descricaoTipoDocumento();
        const jaExiste = (listagem?.documentos ?? [])
          .some(d => d.tpDocIdentificacao === tipoSeleccionado);

        if (jaExiste) {
          this.confirmarDocumentoDuplicado(request, tipoSeleccionado);
          return;
        }

        this.gravarDocumento(request);
      },
      () => {
        // Se a verificação falhar, não bloqueia a inserção -- segue como antes.
        // [VI] Nếu bước kiểm tra lỗi thì không chặn việc chèn -- vẫn chạy như trước.
        this.hideLoader();
        this.gravarDocumento(request);
      });
  }

  private confirmarDocumentoDuplicado(request: TarefaDocumentoRequest, tipo: string): void {
    const dialogRef = this.warningDialog.open(PopUpWarningComponent, {
      id: 'documentoDuplicado',
      minHeight: '300px',
      width: '40%',
      height: '30%',
      panelClass: 'warningModal',
      data: {
        function: undefined,
        msg: this.translate.instant('warnings.documentoDuplicadoMsg', { tipo: tipo })
      }
    });

    dialogRef.afterClosed().subscribe(confirmou => {
      if (confirmou) {
        this.gravarDocumento(request);
      }
    });
  }

  private gravarDocumento(request: TarefaDocumentoRequest): void {
    this.showLoader();
    this.documentoService.SaveTarefaDocumento(request).subscribe(() => {
          this.hideLoader();
          openSnackBar(this.translate.instant('snackBar.saveDocumento'), this._snackBar);
          this.documentosInseridos.push({ nome: request.nomeDocumento, tipo: this.descricaoTipoDocumento() });
          this.clearDocumento();
          this.refreshDocTable.emit(true);
      },
    err => {
        this.hideLoader();
        err.error?.errors ? err.error.errors.map((x : any) => this.error = x.errorCode) : this.error = '-1';
        openErrorSnackBar(this.translate.instant('error.'+ this.error), this._snackBar);        
      });
  }

  public changedDocType() : void {
    this.clearDocumento();
    this.wrongFormat = false;
  }

  // Descrição do tipo de documento seleccionado, para mostrar ao lado do ficheiro já inserido.
  // [VI] Tên loại tài liệu đang chọn, để hiển thị cạnh file đã chèn.
  private descricaoTipoDocumento(): string {
    const grupos = this.documentosList?.groups ?? [];
    for (const grupo of grupos) {
      const encontrado = grupo.values?.find((x: any) => x.id === this.documento.tpDocIdentificacao);
      if (encontrado) {
        return encontrado.descricao;
      }
    }
    return "";
  }

  public clearDocumento() {
    this.documento.documento = "";
    this.documento.nomeDocumento = "";
  }

  public showLoader() {
    this.spinner.show();
  }

  public hideLoader() {
    this.spinner.hide();
  }

  public scroll(e: any)
  {
    e._body.nativeElement.scrollIntoView({behavior: "smooth", block: "center"});
  }
}
