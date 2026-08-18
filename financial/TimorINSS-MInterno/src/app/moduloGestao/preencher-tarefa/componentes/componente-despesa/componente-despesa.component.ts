import { Component, Input, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { saveAs } from 'file-saver';
import * as XLSX from 'xlsx';
import { NgxSpinnerService } from "ngx-spinner";
import { MatDialog } from "@angular/material/dialog";
import { TranslateService } from '@ngx-translate/core';
import { DominioDescricaoString } from 'src/app/response-models/dominios-response';
import { blobExcelSaveAs, customCurrencyMaskConfig, openErrorsDialog, openSnackBar, showExpiredError } from "src/app/utils";
import { TokenStorageService } from "src/app/services/token-storage.service";
import { SelectDescription } from "src/app/models/utils";
import { faFileExcel, faTimesCircle } from "@fortawesome/free-solid-svg-icons";
import { componenteOrcamentoRegistoService } from "src/app/services/componenteOrcamentoRegisto.service";
import { GetComponenteOrcamentoRegistoAprovadoRequest } from "src/app/request-models/componenteOrcamentoRegisto-request";
import { DepartamentoService } from "src/app/services/departamento.service";
import { forkJoin } from "rxjs";
import { MatSnackBar } from "@angular/material/snack-bar";
import { AgrupamentosConfig } from "src/app/models/agrupamentosConfig";
import { CodigoConta } from "src/app/models/codigoConta";
import { AgrupamentoConfigService } from "src/app/services/agrupamentoConfig.service";
import { GetAgrupamentoConfigRequest } from "src/app/request-models/agrupamentoConfig-request";
import { MyErrorStateMatcher } from "src/app/matcher";
import { GetAllDespesaRegistadaRequest, GetDespesasCompromissoRequest, GetValoresDespesaByIdCodigoOrcamentoRequest, RegistoDespesaRequest } from "src/app/request-models/componenteDespesaRegisto-request";
import { DespesaEmCurso } from "src/app/response-models/componenteDespesaRegisto-response";
import { Despesa, ValoresDespesaRegistada } from "src/app/models/despesa";
import { ComponenteDespesaRegistoService } from "src/app/services/componenteDespesaRegisto.service";
import { DespesaCompromisso, DespesaCabimentadasParaExecucao, DespesaRegistada } from "src/app/models/despesaRegistada";
import { PopUpWarningComponent } from "src/app/componentes/pop-up-warning/pop-up-warning.component";
import { ComponenteDespesa } from "src/app/models/tarefa";
import { ComponenteDespesaConfigService } from "src/app/services/componenteDespesaConfig.service";
import { GetComponenteDespesaConfigRequest } from "src/app/request-models/componenteDespesaConfig-request";
import { ComponenteDespesaConfig } from "src/app/models/componenteDespesaConfig";
import { Destinatario } from "src/app/models/destinatario";
import { DestinatarioService } from "src/app/services/destinatario.service";
import { PagamentoExecutado, PagamentoExecutadoDestinatario } from "src/app/models/pagamentos_executados";
import { PopUpExecutarPagamentosComponent } from './pop-up-executar-pagamentos/pop-up-executar-pagamentos.component';
import { PagamentoExecutadoService } from "src/app/services/pagamentoExecutado.service";
import { GetDestinatarioPagamentoRequest, ListagemPagamentosProcessoRequest } from "src/app/request-models/pagamentoExecutado-request";
import { PopUpListarPagamentosExecutadosComponent } from "./pop-up-listar-pagamentos-executados/pop-up-listar-pagamentos-executados.component";
import { Compromisso } from "src/app/models/compromisso";
import { PopUpCompromissosComponent } from './pop-up-compromissos/pop-up-compromissos.component';
import { PopUpEditDespesaCabimentadaComponent } from "./pop-up-edit-despesa-cabimentada/pop-up-edit-despesa-cabimentada.component";




@Component({
  selector: 'app-componente-despesa',
  templateUrl: './componente-despesa.component.html',
  styleUrls: ['./componente-despesa.component.css']
})
export class ComponenteDespesaComponent implements OnInit {
  public faTimesCircle = faTimesCircle;
  public errors: string[] = [];
  public currencyOptions = customCurrencyMaskConfig;
  public faFileExcel = faFileExcel;

  //matcher
  public matcher: MyErrorStateMatcher = new MyErrorStateMatcher();

  public submittedTry: boolean = false;

  //processo id
  public processoId: number = 0;
  //id Componente Orçamento Registo Aprovado
  public idOrcamentoRegisto: number = 0;
  public idInstitution: number = 0;
  public idActidade: number = 0;
  // public idEconomic: number = 0;
  public idFuncional: number = 0;
  //edit
  public idDespesa: number = 0;
  public editar: boolean = false;

  // Departamento INSS
  public departamentoINSS = <SelectDescription>{};
  public departamentoINSSListagem: SelectDescription[] = [];

    // Institution 
  public institution = <SelectDescription>{};
  public institutionListagem: SelectDescription[] = [];

  // Centro de Custo
  public centroCusto = <SelectDescription>{};
  public centroCustoListagem: SelectDescription[] = [];

  // Tipo de Conta
  public tipoConta = <DominioDescricaoString>{};
  public tipoContaListagem: DominioDescricaoString[] = [];
  public tipoContaFiltered: DominioDescricaoString[] = [];


  //Contabilidade
  public contabilidade = <CodigoConta>{};
  public contabilidadeListagem: CodigoConta[] = [];
  public filtersContabilidade: number[] = [];
  public filtersContabilidadeFiltered: CodigoConta[] = [];

  // Conta OSS
  public filtersContaOSS: number[] = [];
  public filtersContaOSSFiltered: AgrupamentosConfig[] = [];
  public contaOSS = <AgrupamentosConfig>{};
  public contaOSSListagem: AgrupamentosConfig[] = [];
  public idContaOssEdit: number = 0;

  // Etidade
  public filtersEtidade: number[] = [];
  public filtersEtidadeFiltered: AgrupamentosConfig[] = [];
  public etidade = <AgrupamentosConfig>{};
  public etidadeListagem: AgrupamentosConfig[] = [];
  public idEtidadeEdit: number = 0;

  // Economic
  // public filtersEconomic: number[] = [];
  // public filtersEconomicFiltered: AgrupamentosConfig[] = [];
  // public economic = <AgrupamentosConfig>{};
  // public economicListagem: AgrupamentosConfig[] = [];
  // public idEconomicEdit: number = 0;

  // Funcional
  public filtersFuncional: number[] = [];
  public filtersFuncionalFiltered: AgrupamentosConfig[] = [];
  public funcional = <AgrupamentosConfig>{};
  public funcionalListagem: AgrupamentosConfig[] = [];
  public idFuncionalEdit: number = 0;
  //Descricao Despesa
  public descricaoDespesa: string = '';

  //Valor da Despesa
  public valorDespesa: number | undefined;

  //existe orcamento aprovado?
  public naoExisteOrcamentoAprovado: boolean = false;

  //despesa registada - Painel 2
  public despesaRegistadaAutorizada = <DespesaRegistada>{};
  public despesaRegistadaAutorizadaListagem: DespesaRegistada[] = [];
  public valorDespesaListagem: ValoresDespesaRegistada[] = [];
  public displayedColumns: string[] = ['descricao', 'valorOrcamentado', 'valorExecutado', 'valorCabimentado', 'valorAutorizado'];
  // Derivado a cada leitura (não é um campo assinalado manualmente) para nunca ficar "preso"
  // num valor antigo depois que todas as despesas passam a Cabimentado.
  public get allDespesasAutorizadas(): boolean {
    return this.listaDespesaRegistadas.length === 0 && this.listaDespesaAutorizadas.length > 0;
  }

  //Listagem Despesas Registadas - Painel 3
  public listaDespesaRegistadas: DespesaRegistada[] = [];
  public displayedColumnsListaDespesaRegistada: string[] = ['descricao', 'contaOSS', 'valor', 'accoes'];
  public totalDespesasRegistadas: number = 0;

  //Listagem Despesas Autorizadas - Painel 4
  public listaDespesaAutorizadas: DespesaRegistada[] = [];
  public totalDespesasAutorizadas: number = 0;

  //Listagem Despesas Cabimentadas - Painel 5
  public listaDespesaCabimentadas: DespesaRegistada[] = [];
  public totalDespesasCabimentadas: number = 0;

  //Despesas Executadas - painel 6
  public listaDespesaAExecutar: DespesaCabimentadasParaExecucao[] = [];
  public totalDespesasExecutadas: number = 0;
  public displayedColumnsDespesasAExecutar: string[] = ['descricao', 'valor', 'valorExecutado', 'faltaExecutar'];

  //Despesas Executadas - painel 6
  public listaDespesaCompromissoAExecutar: DespesaCabimentadasParaExecucao[] = [];
  public totalDespesasCompromissoExecutadas: number = 0;
  public displayedColumnsDespesasCompromissoAExecutar: string[] = ['descricao', 'valor', 'valorExecutado', 'faltaExecutar'];

  //Compromissos - painel 7
  public listaCompromissos: DespesaCompromisso[] = [];
  public totalCompromissos: number = 0;
  public displayedColumnsCompromissos: string[] = ['despesaCabimentada', 'compromisso', 'valorCompromisso', 'accoes'];

  //executar Pagamentos
  public listaPagamentosDestinatario: PagamentoExecutadoDestinatario[] = [];
  public numPagamento: string = '';
  public totalValorCabimentado: number = 0;
  public totalValorExecutado: number = 0;
  public totalValorFaltaExecutar: number = 0;
  public totalValorDestinatarioExecutado: number = 0;


  // Autorizar Despesa
  public isValueCabimentarDespesaNegative: boolean = false;

  @Input() tarefaActivoId: number = 0;
  @Input() isExpanded: boolean = false;

  //configurações componenente despesa
  public componenteDespesaConfig: ComponenteDespesaConfig = <ComponenteDespesaConfig>{};


  constructor(
    private router: Router,
    private spinner: NgxSpinnerService,
    public errorDialog: MatDialog,
    public translate: TranslateService,
    private tokenStorage: TokenStorageService,
    private orcamentoService: componenteOrcamentoRegistoService,
    private departamentoService: DepartamentoService,
    public _snackBar: MatSnackBar,
    public agrupamentoService: AgrupamentoConfigService,
    public componenteDespesaService: ComponenteDespesaRegistoService,
    public warningDialog: MatDialog,
    public componenteDespesaConfigService: ComponenteDespesaConfigService,
    public destinatarioService: DestinatarioService,
    public executarPagamentosDialog: MatDialog,
    public pagamentoExecutadoService: PagamentoExecutadoService,
  ) {
  }

  ngOnInit(): void {

    if (!this.tokenStorage.getToken()) {
      this.router.navigate([''])
    }
    else if (this.tokenStorage.getToken() && !this.tokenStorage.tokenExpired()) {

      //obter configuração despesa
      this.getComponenteDespesaConfig();
      // validar se existe orçamento aprovado para a data de inicio do processo


      this.getOrcamentoAprovado(this.tarefaActivoId);

      this.updateFilteredAgrupamentos(false);
      // alert(1);
      this.updateFilteredEtidades(false);
      // this.updateFilteredEconomic(false);
      this.updateFilteredFuncional(false);

      // TODO: traduzir
      //parte 2- valores Orçamento/Despesa
      this.valorDespesaListagem[0] = <ValoresDespesaRegistada>{};
      this.valorDespesaListagem[0].descricao = "Valor Inicial/Atual";
      this.valorDespesaListagem[1] = <ValoresDespesaRegistada>{};
      this.valorDespesaListagem[1].descricao = "Valor Disponível";
      this.valorDespesaListagem[2] = <ValoresDespesaRegistada>{};
      this.valorDespesaListagem[2].descricao = "Valor Após Execução";
      this.valorDespesaListagem[3] = <ValoresDespesaRegistada>{};
      this.valorDespesaListagem[3].descricao = "Valor Após Cabimentação";
      this.valorDespesaListagem[4] = <ValoresDespesaRegistada>{};
      this.valorDespesaListagem[4].descricao = "Valor Após Autorização";
    }
    else {
      showExpiredError(this.errorDialog, this.tokenStorage, this.translate);
    }

  }

  public getComponenteDespesaConfig() {
    this.showLoader();
    let request = <GetComponenteDespesaConfigRequest>{
      tarefaAtivoId: this.tarefaActivoId,
    };

    this.componenteDespesaConfigService.getComponenteDespesaConfigByTarefaAtivoId(request).subscribe(x => {
      this.componenteDespesaConfig = x.componenteDespesaConfig;
      this.hideLoader();
    },
      err => {

        this.hideLoader();
        err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
        this.showError();
      });
  }

  public getOrcamentoAprovado(idTarefaActivo: number) {
    this.showLoader();
    let request = <GetComponenteOrcamentoRegistoAprovadoRequest>{
      idTarefaActivo: idTarefaActivo,
    };
    this.orcamentoService.GetOrcamentoAprovadoDespesaByIdTarefaActivo(request).subscribe(x => {
      if (x != null && x.existeOrcamentoAprovado) {
        this.idOrcamentoRegisto = x.idOrcamentoRegisto;
        this.centroCustoListagem = x.centrosCusto;
        this.tipoContaListagem = x.tiposDeConta;
        this.tipoContaFiltered = this.tipoContaListagem.filter(tipoConta => tipoConta.descricao == 'Despesa' 
          || tipoConta.descricao == 'Neutro Despesa' || tipoConta.descricao == 'Expense' );
        this.tipoConta.id = this.tipoContaFiltered[0].id;
        this.contabilidadeListagem = x.codigoConta;
        this.filtersContabilidadeFiltered = x.codigoConta;
        this.numPagamento = x.numPagamento;
        this.listaPagamentosDestinatario = x.listaPagamentosDestinatario;
        this.processoId = x.processoId;
        this.getAllDadosDropDown();


        if (this.listaPagamentosDestinatario != null && this.listaPagamentosDestinatario.length > 0) {
          this.listaPagamentosDestinatario.forEach(element => {
           this.totalValorDestinatarioExecutado = Math.round((this.totalValorDestinatarioExecutado + element.valorExecutado) * 100) / 100;
          });
        }
      }
      else if (!x.existeOrcamentoAprovado) {

        this.naoExisteOrcamentoAprovado = true;
      }
      this.hideLoader();

    },
      err => {

        this.hideLoader();
        err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
        this.showError();
      });

  }

  public getAllDadosDropDown() {
    this.showLoader();

    let departamentoInss = this.departamentoService.getAllDepartamentosAtivo();

    let request = <GetAllDespesaRegistadaRequest>{
      tarefaAtivoId: this.tarefaActivoId
    }

    let compromissoRequest = <GetDespesasCompromissoRequest> {
      tarefaAtivoId: this.tarefaActivoId
    }

    let despesaRegistada = this.componenteDespesaService.GetAllDespesaRegistadaByTarefaAtivoId(request);

    let despesaCabimentadaParaExecutar = this.componenteDespesaService.GetAllDespesaCabimentadasParaExecucaoByTarefaAtivoId(request);

    let despesaCompromissoParaExecutar = this.componenteDespesaService.GetAllDespesaCompromissosParaExecucaoByTarefaAtivoId(request);

    let despesaCompromisso = this.componenteDespesaService.GetDespesasCompromissoByTarefaAtivoId(compromissoRequest);

    let institutions = this.departamentoService.getAllInstitutionsAtivo();

    // this.updateFilteredEtidades(true);
    // this.updateFilteredFuncional(true);
    

    forkJoin([departamentoInss, despesaRegistada,institutions, despesaCabimentadaParaExecutar, despesaCompromisso, despesaCompromissoParaExecutar]).subscribe(([departamentoInss, despesaRegistada,institutions, despesaCabimentadaParaExecutar, despesaCompromisso, despesaCompromissoParaExecutar]) => {
      this.departamentoINSSListagem = departamentoInss.selects;
      this.institutionListagem = institutions.selects;

      // [PT] Quando só existe uma opção activa (por ex. o INSS, depois de o FRSS ter sido desactivado),
      // pré-selecciona-a: não há escolha a fazer e o utilizador precisa de VER qual está em uso, em vez
      // de um campo em branco. Só preenche se ainda não houver valor, para não sobrepor a Instituição de
      // uma despesa que esteja a ser editada.
      // [VI] Khi chỉ còn một lựa chọn đang hoạt động (vd chỉ INSS sau khi tắt FRSS) thì chọn sẵn nó:
      // không có gì để chọn, mà người dùng cần THẤY đang dùng cái nào thay vì ô trắng. Chỉ gán khi chưa
      // có giá trị, để không ghi đè Institution của despesa đang được sửa.
      if (this.institutionListagem != null && this.institutionListagem.length === 1 && this.institution.id == null) {
        this.institution = this.institutionListagem[0];
      }
      if (this.departamentoINSSListagem != null && this.departamentoINSSListagem.length === 1 && this.departamentoINSS.id == null) {
        this.departamentoINSS = this.departamentoINSSListagem[0];
      }

      if (despesaRegistada.componenteDespesaRegisto != null && despesaRegistada.componenteDespesaRegisto.length > 0) {
        this.despesaRegistadaAutorizadaListagem = despesaRegistada.componenteDespesaRegisto.filter(estadoDespesa => estadoDespesa.estado == 'R');

        if (this.despesaRegistadaAutorizadaListagem == null || this.despesaRegistadaAutorizadaListagem.length == 0) {
          this.despesaRegistadaAutorizadaListagem = despesaRegistada.componenteDespesaRegisto.filter(estadoDespesa => estadoDespesa.estado == 'A');

        }
        if (this.despesaRegistadaAutorizadaListagem != null && this.despesaRegistadaAutorizadaListagem.length > 0) {
          let ultimaDespesaRegistada: DespesaRegistada[] = this.despesaRegistadaAutorizadaListagem.filter(estadoDespesa => estadoDespesa.estado == 'R');

          if (ultimaDespesaRegistada == null || ultimaDespesaRegistada.length == 0) {
            ultimaDespesaRegistada = this.despesaRegistadaAutorizadaListagem.filter(estadoDespesa => estadoDespesa.estado == 'A');
          }

          if (ultimaDespesaRegistada != null && ultimaDespesaRegistada.length > 0) {
            this.despesaRegistadaAutorizada.id = ultimaDespesaRegistada[ultimaDespesaRegistada.length - 1].id;
            this.despesaRegistadaAutorizada.idOrcamento = ultimaDespesaRegistada[ultimaDespesaRegistada.length - 1].idOrcamento;
            this.despesaRegistadaAutorizada.descricaoOrcamento = ultimaDespesaRegistada[ultimaDespesaRegistada.length - 1].descricaoOrcamento;
            this.despesaRegistadaAutorizada.valorRegistado = ultimaDespesaRegistada[ultimaDespesaRegistada.length - 1].valorRegistado;
            this.despesaRegistadaAutorizada.idDepartamento = ultimaDespesaRegistada[ultimaDespesaRegistada.length - 1].idDepartamento;
            this.despesaRegistadaAutorizada.idCentroCusto = ultimaDespesaRegistada[ultimaDespesaRegistada.length - 1].idCentroCusto;
            this.despesaRegistadaAutorizada.idTipoConta = ultimaDespesaRegistada[ultimaDespesaRegistada.length - 1].idTipoConta;
            this.despesaRegistadaAutorizada.descricaoDespesa = ultimaDespesaRegistada[ultimaDespesaRegistada.length - 1].descricaoDespesa;
            this.despesaRegistadaAutorizada.estado = ultimaDespesaRegistada[ultimaDespesaRegistada.length - 1].estado;

            this.despesaRegistadaAutorizada.idActidade = ultimaDespesaRegistada[ultimaDespesaRegistada.length - 1].idActidade;
            this.despesaRegistadaAutorizada.idInstitution = ultimaDespesaRegistada[ultimaDespesaRegistada.length - 1].idInstitution;
            // this.despesaRegistadaAutorizada.idEconomic = ultimaDespesaRegistada[ultimaDespesaRegistada.length - 1].idEconomic;
            this.despesaRegistadaAutorizada.idFuncional = ultimaDespesaRegistada[ultimaDespesaRegistada.length - 1].idFuncional;

            this.updateDespesaRegistada();
          }
        }
        this.listaDespesaRegistadas = despesaRegistada.componenteDespesaRegisto.filter(estadoDespesa => estadoDespesa.estado == 'R');

        if (this.listaDespesaRegistadas != null && this.listaDespesaRegistadas.length > 0) {
          this.totalDespesasRegistadas = 0;
          this.listaDespesaRegistadas.forEach(element => {
            this.totalDespesasRegistadas = Math.round((this.totalDespesasRegistadas + element.valorRegistado) * 100) / 100;
          });
        }

        this.listaDespesaAutorizadas = despesaRegistada.componenteDespesaRegisto.filter(estadoDespesa => estadoDespesa.estado == 'A');
        if (this.listaDespesaAutorizadas != null && this.listaDespesaAutorizadas.length > 0) {
          this.totalDespesasAutorizadas = 0;
          this.listaDespesaAutorizadas.forEach(element => {
            this.totalDespesasAutorizadas = Math.round((this.totalDespesasAutorizadas + element.valorRegistado) * 100) / 100;
          });
        }

        this.listaDespesaCabimentadas = despesaRegistada.componenteDespesaRegisto.filter(estadoDespesa => estadoDespesa.estado == 'C');
        if (this.listaDespesaCabimentadas != null && this.listaDespesaCabimentadas.length > 0) {
          this.totalDespesasCabimentadas = 0;
          this.listaDespesaCabimentadas.forEach(element => {
            this.totalDespesasCabimentadas = Math.round((this.totalDespesasCabimentadas + element.valorRegistado) * 100) / 100;
          });
        }

        this.listaDespesaCompromissoAExecutar = despesaCompromissoParaExecutar.despesasParaExecucao;
        if (this.listaDespesaCompromissoAExecutar != null && this.listaDespesaCompromissoAExecutar.length > 0) {
          this.listaDespesaCompromissoAExecutar.forEach(element => {
            this.totalValorCabimentado = Math.round((this.totalValorCabimentado + element.valorCabimentado) * 100) / 100;
            this.totalValorExecutado = Math.round((this.totalValorExecutado + element.valorExecutado) * 100) / 100;
            this.totalValorFaltaExecutar = Math.round((this.totalValorFaltaExecutar + element.faltaExecutar) * 100) / 100;
          });
        }

        this.listaCompromissos = despesaCompromisso.componenteDespesaObrigacao;
        this.totalCompromissos = despesaCompromisso.componenteDespesaObrigacao.length;

        this.hideLoader();

      }

      else {
        this.hideLoader();

      }

    },
      err => {
        this.hideLoader();
        err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
        this.showError();
      });
  }


  public filterCustomOptionsContaOSS(event: any) {
    this.filtersContaOSSFiltered = this.contaOSSListagem.filter(p => p.designacao.toLowerCase().startsWith(event.toLowerCase()));
  }

  public filterCustomOptionsEtidade(event: any) {
    this.filtersEtidadeFiltered = this.etidadeListagem.filter(p => p.designacao.toLowerCase().startsWith(event.toLowerCase()));
  }

  // public filterCustomOptionsEconomic(event: any) {
  //   this.filtersEconomicFiltered = this.economicListagem.filter(p => p.designacao.toLowerCase().startsWith(event.toLowerCase()));
  // }

  public filterCustomOptionsFuncional(event: any) {
    this.filtersFuncionalFiltered = this.funcionalListagem.filter(p => p.designacao.toLowerCase().startsWith(event.toLowerCase()));
  }


  public filterCustomOptionsContabilidade(event: any) {
    this.filtersContabilidadeFiltered = this.contabilidadeListagem.filter(p => p.designacao.toLowerCase().startsWith(event.toLowerCase()));
  }

  public clearValorDespesa(): void {
    this.valorDespesa = 0;
  }

  public submitDespesa() {
    this.submittedTry = true;
  }

  // [PT] Rede de segurança: qualquer erro inesperado ao montar o pedido tem de terminar com o
  // spinner desligado e uma mensagem visível. Sem isto, uma excepção entre o showLoader() e o
  // hideLoader() deixa o ecrã eternamente "a processar", sem dizer nada ao utilizador -- foi
  // exactamente o que aconteceu ao editar uma despesa com campos por repor.
  // [VI] Lưới an toàn: mọi lỗi bất ngờ khi dựng request đều phải kết thúc bằng việc tắt spinner và
  // hiện thông báo. Không có nó, một exception rơi vào giữa showLoader() và hideLoader() sẽ khiến
  // màn hình quay mãi mà không báo gì -- đúng những gì đã xảy ra khi sửa despesa thiếu dữ liệu.
  public registarEditarDespesa() {
    this.showLoader();

    try {
      this.montarEGravarDespesa();
    } catch (e) {
      this.hideLoader();
      this.errors.push('-1');
      this.showError();
    }
  }

  private montarEGravarDespesa() {
    let despesaRegisto: Despesa = {
      id: this.editar ? this.idDespesa : 0,
      idOrcamentoRegistoAprovado: this.idOrcamentoRegisto,
      tarefaAtivoFK: this.tarefaActivoId,
      departamentoFk: this.departamentoINSS != null && this.departamentoINSS.id > 0 ? this.departamentoINSS.id : undefined,
      centroCustoFk: this.centroCusto.id,
      tipoContaFk: this.tipoConta.id,
      codigoContaFk: this.contabilidade.id,
      // Acesso defensivo (?.): um campo em falta passa a ser recusado pelo servidor com uma mensagem,
      // em vez de rebentar aqui entre o showLoader() e o hideLoader() e deixar o ecrã a rodar para sempre.
      // [VI] Truy cập phòng thủ (?.): field thiếu sẽ bị server từ chối kèm thông báo, thay vì ném lỗi
      // ngay giữa showLoader() và hideLoader() khiến màn hình quay mãi không dừng.
      agrupamentoConfigFk: this.contaOSS?.id,
      institutionId: this.institution?.id,
      actidadeFk: this.etidade?.id,
      funcionalFk: this.funcional?.id,
      descricao: this.descricaoDespesa,
      valor: this.valorDespesa
    };

    let request: RegistoDespesaRequest = {
      despesa: despesaRegisto
    };

    this.gravarDespesa(request);
  }

  // [PT] Envia o registo. Se o servidor devolver despesas em curso para a mesma combinação dos 5
  // parâmetros (Institution + Centro de Custo + Actividade + Funcional + rubrica), nada foi gravado:
  // mostra-se ao utilizador o que já existe e só se grava se ele confirmar. Antes disto o registo era
  // simplesmente bloqueado, sem sequer dizer que processo estava a causar o bloqueio.
  // [VI] Gửi request đăng ký. Nếu server trả về danh sách despesa đang mở cùng tổ hợp 5 tham số
  // (Institution + Centro de Custo + Actividade + Funcional + rubrica) thì CHƯA lưu gì: hiển thị cho
  // người dùng thấy cái đang tồn tại, chỉ lưu khi họ xác nhận. Trước đây chỗ này chặn thẳng, thậm chí
  // không cho biết processo nào đang gây chặn.
  private gravarDespesa(request: RegistoDespesaRequest) {
    this.componenteDespesaService.addEditComponenteDespesaRegisto(request).subscribe(x => {
      if (x != null && x.despesasEmCurso != null && x.despesasEmCurso.length > 0) {
        this.hideLoader();
        this.confirmarDespesasEmCurso(request, x.despesasEmCurso);
        return;
      }

      this.hideLoader();
      this.contabilidade = <CodigoConta>{};
      this.contaOSS = <AgrupamentosConfig>{};
      this.descricaoDespesa = '';
      this.valorDespesa = undefined;
      this.submittedTry = false;
      this.getAllDadosDropDown();

      if (this.editar) {
        this.editar = false;
        this.departamentoINSS = <SelectDescription>{};
        this.centroCusto = <SelectDescription>{};
        this.tipoConta = <DominioDescricaoString>{};
      }
      openSnackBar(this.translate.instant('snackBar.registoDespesa'), this._snackBar);

    },
      err => {
        this.hideLoader();
        err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
        this.showError();
      });
  }

  // [PT] Traduz o código de estado ESTADODESPESA para uma etiqueta legível. A tabela DOMINIO guarda
  // apenas a letra ("R", "A"), que não diz nada ao utilizador; só os estados que podem aparecer no
  // aviso são tratados (as despesas já Cabimentadas não bloqueiam nem aparecem na lista).
  // [VI] Dịch mã trạng thái ESTADODESPESA thành nhãn đọc được. Bảng DOMINIO chỉ lưu chữ cái ("R",
  // "A") nên người dùng không hiểu; chỉ xử lý các trạng thái có thể xuất hiện trong cảnh báo
  // (despesa đã Cabimentado thì không còn cảnh báo và không nằm trong danh sách).
  private estadoDespesaLabel(estadoValor: number): string {
    switch (estadoValor) {
      case 1: return this.translate.instant('despesas.registada');
      case 2: return this.translate.instant('despesas.autorizada');
      case 3: return this.translate.instant('despesas.cabimentada');
      default: return '';
    }
  }

  // [PT] Aviso (não bloqueio): lista as despesas já em curso com número do processo, valor e estado,
  // para o utilizador perceber o que está a duplicar antes de decidir. Se confirmar, reenvia-se o
  // mesmo pedido com confirmarDespesasEmCurso = true.
  // [VI] Cảnh báo (không phải chặn): liệt kê các despesa đang mở kèm số processo, giá trị, trạng thái
  // để người dùng biết mình đang trùng cái gì trước khi quyết. Nếu xác nhận thì gửi lại chính request
  // đó với confirmarDespesasEmCurso = true.
  private confirmarDespesasEmCurso(request: RegistoDespesaRequest, despesasEmCurso: DespesaEmCurso[]) {
    const linhas = despesasEmCurso
      .map(d => `• ${this.translate.instant('processo.processo')} ${d.numeroProcesso ?? '-'} | ${d.descricao ?? ''} | ${this.formatarValor(d.valor)} | ${this.estadoDespesaLabel(d.estadoValor)}`)
      .join('\n');

    const msg = this.translate.instant('warnings.despesasEmCursoMsg') + '\n\n' + linhas + '\n\n'
      + this.translate.instant('warnings.despesasEmCursoPergunta');

    const dialogRef = this.warningDialog.open(PopUpWarningComponent, {
      id: 'despesasEmCurso',
      minHeight: '300px',
      width: '50%',
      panelClass: 'modalWithBorder',
      data: { msg: msg, hideQuestion: true }
    });

    dialogRef.afterClosed().subscribe(confirmou => {
      if (confirmou) {
        this.showLoader();
        this.gravarDespesa({ ...request, confirmarDespesasEmCurso: true });
      }
    });
  }

  public updateFilteredAgrupamentos(editar: boolean) {
    this.contaOSSListagem = [];
    this.filtersContaOSSFiltered = [];


      this.showLoader();
      let request = <GetAgrupamentoConfigRequest>{
        idOrcamento: this.idOrcamentoRegisto
      };

      this.agrupamentoService.getAgrupamentoConfigByIdCodigoContaTipoConta(request).subscribe(x => {
        if (x != null && x.agrupamentos != null) {
          this.contaOSSListagem = x.agrupamentos;
          this.filtersContaOSSFiltered = x.agrupamentos;
          this.contaOSS = <AgrupamentosConfig>{};
        }

        if (editar) {
          this.contaOSS = this.filtersContaOSSFiltered.filter((c: { id: number; }) => c.id === this.idContaOssEdit)[0] ?? <AgrupamentosConfig>{};
        }
        this.hideLoader();
      },
        err => {

          this.hideLoader();
          err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
          this.showError();
        });
  }

  public updateFilteredEtidades(editar: boolean) {
    this.etidadeListagem = [];

    this.filtersEtidadeFiltered = [];


      this.showLoader();
      let request = <GetAgrupamentoConfigRequest>{
        idOrcamento: -1,
        tipoContaFK: -1
      };
      
      this.agrupamentoService.getAgrupamentoConfigByIdCodigoContaTipoConta(request).subscribe(x => {
        if (x != null && x.agrupamentos != null) {
          // alert(`Id: ${x.agrupamentos[0].id} - Nome: ${x.agrupamentos[0].designacao}`);
          this.etidadeListagem = x.agrupamentos;
          this.filtersEtidadeFiltered = x.agrupamentos;
          this.etidade = <AgrupamentosConfig>{};
        }
        
        if (editar) {
          this.etidade = this.filtersEtidadeFiltered.filter((c: { id: number; }) => c.id === this.idEtidadeEdit)[0] ?? <AgrupamentosConfig>{};
        }
        this.hideLoader();
      },
        err => {

          this.hideLoader();
          err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
          this.showError();
        });
  }

  // public updateFilteredEconomic(editar: boolean) {
  //   // this.economicListagem = [];

  //   this.filtersEconomicFiltered = [];


  //     this.showLoader();
  //     let request = <GetAgrupamentoConfigRequest>{
  //       idOrcamento: 2018,
  //       tipoContaFK: 1113
  //     };

      // this.agrupamentoService.getAgrupamentoConfigByIdCodigoContaTipoConta(request).subscribe(x => {
      //   if (x != null && x.agrupamentos != null) {
      //     this.economicListagem = x.agrupamentos;
      //     this.filtersEconomicFiltered = x.agrupamentos;
      //     this.economic = <AgrupamentosConfig>{};
      //   }

      //   if (editar) {
      //     this.economic = this.filtersEconomicFiltered.filter((c: { id: number; }) => c.id === this.idEconomicEdit)[0];
      //   }
      //   this.hideLoader();
      // },
      //   err => {

      //     this.hideLoader();
      //     err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
      //     this.showError();
      //   });
  // }

  public updateFilteredFuncional(editar: boolean) {
    this.funcionalListagem = [];

    this.filtersFuncionalFiltered = [];


      this.showLoader();
      let request = <GetAgrupamentoConfigRequest>{
        idOrcamento: -1,
        tipoContaFK: -2
      };

      this.agrupamentoService.getAgrupamentoConfigByIdCodigoContaTipoConta(request).subscribe(x => {
        if (x != null && x.agrupamentos != null) {
          this.funcionalListagem = x.agrupamentos;
          this.filtersFuncionalFiltered = x.agrupamentos;
          this.funcional = <AgrupamentosConfig>{};
        }

        if (editar) {
          this.funcional = this.filtersFuncionalFiltered.filter((c: { id: number; }) => c.id === this.idFuncionalEdit)[0] ?? <AgrupamentosConfig>{};
        }
        this.hideLoader();
      },
        err => {

          this.hideLoader();
          err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
          this.showError();
        });
  }

  public updateDespesaRegistada() {
    this.despesaRegistadaAutorizada.id = this.despesaRegistadaAutorizadaListagem.filter((c: { id: number; }) => c.id === this.despesaRegistadaAutorizada.id)[0].id;
    this.despesaRegistadaAutorizada.idContabilidade = this.despesaRegistadaAutorizadaListagem.filter((c: { id: number; }) => c.id === this.despesaRegistadaAutorizada.id)[0].idContabilidade;
    this.despesaRegistadaAutorizada.descricaoContabilidade = this.despesaRegistadaAutorizadaListagem.filter((c: { id: number; }) => c.id === this.despesaRegistadaAutorizada.id)[0].descricaoContabilidade;
    this.despesaRegistadaAutorizada.idOrcamento = this.despesaRegistadaAutorizadaListagem.filter((c: { id: number; }) => c.id === this.despesaRegistadaAutorizada.id)[0].idOrcamento;
    this.despesaRegistadaAutorizada.descricaoOrcamento = this.despesaRegistadaAutorizadaListagem.filter((c: { id: number; }) => c.id === this.despesaRegistadaAutorizada.id)[0].descricaoOrcamento;
    this.despesaRegistadaAutorizada.valorRegistado = this.despesaRegistadaAutorizadaListagem.filter((c: { id: number; }) => c.id === this.despesaRegistadaAutorizada.id)[0].valorRegistado;
    this.despesaRegistadaAutorizada.idDepartamento = this.despesaRegistadaAutorizadaListagem.filter((c: { id: number; }) => c.id === this.despesaRegistadaAutorizada.id)[0].idDepartamento;
    this.despesaRegistadaAutorizada.idCentroCusto = this.despesaRegistadaAutorizadaListagem.filter((c: { id: number; }) => c.id === this.despesaRegistadaAutorizada.id)[0].idCentroCusto;
    this.despesaRegistadaAutorizada.idTipoConta = this.despesaRegistadaAutorizadaListagem.filter((c: { id: number; }) => c.id === this.despesaRegistadaAutorizada.id)[0].idTipoConta;
    this.despesaRegistadaAutorizada.descricaoDespesa = this.despesaRegistadaAutorizadaListagem.filter((c: { id: number; }) => c.id === this.despesaRegistadaAutorizada.id)[0].descricaoDespesa;
    this.despesaRegistadaAutorizada.estado = this.despesaRegistadaAutorizadaListagem.filter((c: { id: number; }) => c.id === this.despesaRegistadaAutorizada.id)[0].estado;

    this.despesaRegistadaAutorizada.idInstitution = this.despesaRegistadaAutorizadaListagem.filter((c: { id: number; }) => c.id === this.despesaRegistadaAutorizada.id)[0].idInstitution;
    this.despesaRegistadaAutorizada.idActidade = this.despesaRegistadaAutorizadaListagem.filter((c: { id: number; }) => c.id === this.despesaRegistadaAutorizada.id)[0].idActidade;
    // this.despesaRegistadaAutorizada.idEconomic = this.despesaRegistadaAutorizadaListagem.filter((c: { id: number; }) => c.id === this.despesaRegistadaAutorizada.id)[0].idEconomic;
    this.despesaRegistadaAutorizada.idFuncional = this.despesaRegistadaAutorizadaListagem.filter((c: { id: number; }) => c.id === this.despesaRegistadaAutorizada.id)[0].idFuncional;


    //obter valor orçamento, somatorio despesas executadas, cabimentadas e autorizadas para o Código de orçamento
    // this.contaOSSListagem = [];
    // this.filtersContaOSSFiltered = [];

    let request = <GetValoresDespesaByIdCodigoOrcamentoRequest>{
      agrupamentoFk: this.despesaRegistadaAutorizadaListagem.filter((c: { id: number; }) => c.id === this.despesaRegistadaAutorizada.id)[0].idOrcamento,
      orcamentoRegistoFk: this.idOrcamentoRegisto,
      institutionId: this.despesaRegistadaAutorizadaListagem.filter((c: { id: number; }) => c.id === this.despesaRegistadaAutorizada.id)[0].idInstitution,
      actidadeFk: this.despesaRegistadaAutorizadaListagem.filter((c: { id: number; }) => c.id === this.despesaRegistadaAutorizada.id)[0].idActidade,
      funcionalFk: this.despesaRegistadaAutorizadaListagem.filter((c: { id: number; }) => c.id === this.despesaRegistadaAutorizada.id)[0].idFuncional,
      // [PT] 5.º parâmetro: sem ele os valores em baixo somavam o orçamento e o consumo de TODOS os
      // centros de custo da mesma rubrica, e um centro de custo consumia a verba de outro.
      // [VI] Tham số thứ 5: thiếu nó thì các giá trị bên dưới gộp ngân sách và mức đã dùng của TẤT CẢ
      // centro de custo trên cùng rubrica, khiến centro de custo này tiêu vào tiền của centro kia.
      centroCustoFk: this.despesaRegistadaAutorizadaListagem.filter((c: { id: number; }) => c.id === this.despesaRegistadaAutorizada.id)[0].idCentroCusto
    };

    this.showLoader();
    this.componenteDespesaService.GetValoresDespesaByIdCodigoOrcamento(request).subscribe(x => {

      //valor inicial/atual
      if (x != null && x.valoresDespesa != null) {
        this.valorDespesaListagem[0].valorOrcamentado = x.valoresDespesa.valorOrcamentado;
        this.valorDespesaListagem[0].valorAutorizado = x.valoresDespesa.valorAutorizado + x.valoresDespesa.valorCabimentado;
        this.valorDespesaListagem[0].valorCabimentado = x.valoresDespesa.valorCabimentado;
        this.valorDespesaListagem[0].valorExecutado = x.valoresDespesa.valorExecutado;
      }


      //valores disponiveis
      if (this.valorDespesaListagem[0].valorOrcamentado != null && this.valorDespesaListagem[0].valorOrcamentado > 0) {

        this.valorDespesaListagem[1].valorOrcamentado = this.valorDespesaListagem[0].valorOrcamentado - this.valorDespesaListagem[0].valorExecutado;
        this.valorDespesaListagem[1].valorExecutado = this.valorDespesaListagem[0].valorOrcamentado - this.valorDespesaListagem[0].valorExecutado;


        if(this.valorDespesaListagem[0].valorCabimentado != null && this.valorDespesaListagem[0].valorCabimentado > 0 ){
          this.valorDespesaListagem[1].valorCabimentado = this.valorDespesaListagem[0].valorOrcamentado  - this.valorDespesaListagem[0].valorCabimentado;
        }
        else{
          this.valorDespesaListagem[1].valorCabimentado = this.valorDespesaListagem[0].valorOrcamentado;
        }


        if(this.valorDespesaListagem[0].valorAutorizado != null && this.valorDespesaListagem[0].valorAutorizado > 0 ){
          this.valorDespesaListagem[1].valorAutorizado = this.valorDespesaListagem[0].valorOrcamentado  - this.valorDespesaListagem[0].valorAutorizado;
        }
        else{
          this.valorDespesaListagem[1].valorAutorizado = this.valorDespesaListagem[0].valorOrcamentado;
        }

      }
      else {
        this.valorDespesaListagem[1].valorOrcamentado = 0;
        this.valorDespesaListagem[1].valorExecutado = 0;
        this.valorDespesaListagem[1].valorCabimentado = 0;
        this.valorDespesaListagem[1].valorAutorizado = 0;
      }


      //valor após execução
      if (this.valorDespesaListagem[0].valorOrcamentado != null && this.valorDespesaListagem[0].valorOrcamentado > 0
        && this.despesaRegistadaAutorizada.valorRegistado != null) {

          if(this.valorDespesaListagem[0].valorExecutado != null && this.valorDespesaListagem[0].valorExecutado > 0){
            this.valorDespesaListagem[2].valorExecutado = this.valorDespesaListagem[1].valorExecutado - this.despesaRegistadaAutorizada.valorRegistado;

          }
          else{
            this.valorDespesaListagem[2].valorExecutado = this.valorDespesaListagem[1].valorOrcamentado - this.despesaRegistadaAutorizada.valorRegistado;
          }

      }
      else {
        this.valorDespesaListagem[2].valorExecutado = 0;
      }


      //valor após cabimentação
      // if (this.valorDespesaListagem[0].valorCabimentado != null && this.valorDespesaListagem[0].valorCabimentado > 0
      //   && this.despesaRegistadaAutorizada.valorRegistado != null) {

      //   this.valorDespesaListagem[3].valorCabimentado = this.valorDespesaListagem[0].valorCabimentado - this.despesaRegistadaAutorizada.valorRegistado;
      // }
      // else {

      //   if(this.valorDespesaListagem[1].valorCabimentado != null && this.valorDespesaListagem[1].valorCabimentado > 0){
      //     this.valorDespesaListagem[3].valorCabimentado = this.valorDespesaListagem[1].valorCabimentado - this.despesaRegistadaAutorizada.valorRegistado;
      //   }
      //   else{
      //     this.valorDespesaListagem[3].valorCabimentado = this.valorDespesaListagem[1].valorOrcamentado - this.despesaRegistadaAutorizada.valorRegistado;

      //   }
      // }

      // 1º caso
      this.valorDespesaListagem[3].valorCabimentado = this.valorDespesaListagem[1].valorCabimentado - this.despesaRegistadaAutorizada.valorRegistado;

      //valor após aprovação
      // 1º caso
      if (this.despesaRegistadaAutorizada.estado == 'A') {
        this.valorDespesaListagem[4].valorAutorizado = this.valorDespesaListagem[1].valorAutorizado
      }
      else {
        this.valorDespesaListagem[4].valorAutorizado = this.valorDespesaListagem[1].valorAutorizado - this.despesaRegistadaAutorizada.valorRegistado;
      }
      // if (this.valorDespesaListagem[0].valorAutorizado != null && this.valorDespesaListagem[0].valorAutorizado > 0
      //   && this.despesaRegistadaAutorizada.valorRegistado != null) {

      //     if(this.despesaRegistadaAutorizada.estado == 'A'){
      //       this.valorDespesaListagem[4].valorAutorizado = this.valorDespesaListagem[0].valorAutorizado;

      //     }
      //     else{
      //       this.valorDespesaListagem[4].valorAutorizado = this.valorDespesaListagem[0].valorAutorizado - this.despesaRegistadaAutorizada.valorRegistado;
      //     }
      // }
      // else {
      //   if(this.valorDespesaListagem[1].valorOrcamentado != null && this.valorDespesaListagem[1].valorOrcamentado > 0){
      //     this.valorDespesaListagem[4].valorAutorizado = this.valorDespesaListagem[1].valorOrcamentado - this.despesaRegistadaAutorizada.valorRegistado;
      //   }
      //   else{
      //     this.valorDespesaListagem[4].valorAutorizado = this.valorDespesaListagem[1].valorOrcamentado - this.despesaRegistadaAutorizada.valorRegistado;

      //   }
      // }
      this.hideLoader();
    },
      err => {

        this.hideLoader();
        err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
        this.showError();
      });

  }

  // [PT] Ao editar, TODOS os campos da chave têm de ser repostos no formulário: Institution,
  // Actividade, Funcional e Conta OSS além dos restantes. Faltavam os três primeiros -- os campos
  // apareciam vazios e, pior, updateFilteredEtidades/Funcional procuravam o id 0, o filter devolvia
  // undefined e this.etidade/this.funcional ficavam undefined; ao gravar, o acesso a .id rebentava
  // depois do showLoader(), pelo que o hideLoader() nunca chegava a correr e o ecrã ficava eternamente
  // "a processar".
  // [VI] Khi sửa, PHẢI nạp lại đủ mọi thành phần của khóa vào form: Institution, Actividade, Funcional
  // và Conta OSS. Trước đây thiếu 3 cái đầu -- ô hiện trống, và tệ hơn: updateFilteredEtidades/Funcional
  // đi tìm id 0, filter trả undefined nên this.etidade/this.funcional = undefined; lúc bấm lưu, việc
  // đọc .id ném lỗi ngay sau showLoader() nên hideLoader() không bao giờ chạy và màn hình quay mãi.
  public editarComponente(componenteDespesa: DespesaRegistada) {
    this.departamentoINSS.id = componenteDespesa.idDepartamento ?? <number>{};
    this.centroCusto.id = componenteDespesa.idCentroCusto;
    this.tipoConta = this.tipoContaFiltered.filter((c: { id: number; }) => c.id === componenteDespesa.idTipoConta)[0];
    this.contabilidade.id = componenteDespesa.idContabilidade;
    this.institution.id = componenteDespesa.idInstitution;
    this.idContaOssEdit = componenteDespesa.idOrcamento;
    this.idEtidadeEdit = componenteDespesa.idActidade;
    this.idFuncionalEdit = componenteDespesa.idFuncional;
    this.updateFilteredAgrupamentos(true);
    this.updateFilteredEtidades(true);
    // this.updateFilteredEconomic(true);
    this.updateFilteredFuncional(true);
    this.descricaoDespesa = componenteDespesa.descricaoDespesa;
    this.valorDespesa = componenteDespesa.valorRegistado;
    this.idDespesa = componenteDespesa.id;
    this.editar = true;
    document.getElementById("registoDepartamento")?.scrollIntoView({ behavior: "smooth" });
  }

  public deleteComponente(componenteDespesa: DespesaRegistada) {
    const dialogRef = this.warningDialog.open(PopUpWarningComponent, {
      id: 'deleteDespesa',
      minHeight: '300px',
      width: '40%',
      height: '30%',
      panelClass: 'warningModal',
      data: { function: this.componenteDespesaService.deleteDespesa({ id: componenteDespesa.id }), msg: this.translate.instant('warnings.despesaWarningMsg') }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.getAllDadosDropDown();
        openSnackBar('Despesa eliminado com sucesso', this._snackBar);
      }
    });
  }


  public deleteAllComponente(ids: number[]) {
    const dialogRef = this.warningDialog.open(PopUpWarningComponent, {
      id: 'deleteDespesa',
      minHeight: '300px',
      width: '40%',
      height: '30%',
      panelClass: 'warningModal',
      data: { function: this.componenteDespesaService.deleteAllDespesasRegistadas({ ids: ids }), msg: this.translate.instant('warnings.allDespesaWarningMsg') }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.getAllDadosDropDown();
        openSnackBar('Despesa eliminado com sucesso', this._snackBar);
      }
    });
  }


  // [PT] Formata um valor monetário para as mensagens de aviso (mesmo formato da tabela de valores).
  // [VI] Định dạng số tiền cho các thông báo cảnh báo (giống định dạng của bảng giá trị).
  private formatarValor(valor: number | null | undefined): string {
    return '$ ' + (valor ?? 0).toFixed(2);
  }

  // [PT] Constrói a mensagem de saldo insuficiente com os valores concretos em vez de um aviso
  // genérico: o utilizador tem de saber QUANTO está disponível, QUANTO está a pedir e QUANTO falta.
  // Distingue "a rubrica não tem orçamento nenhum para esta combinação" (tipicamente Institution /
  // Actividade / Funcional da despesa não coincidem com nenhuma linha de orçamento aprovada) de
  // "tem orçamento mas já está esgotado", porque a acção a tomar é diferente em cada caso.
  // [VI] Tạo thông báo thiếu số dư kèm số liệu cụ thể thay vì cảnh báo chung chung: người dùng phải
  // biết còn BAO NHIÊU, đang xin BAO NHIÊU và thiếu BAO NHIÊU. Phân biệt "rubrica không có ngân sách
  // nào cho tổ hợp này" (thường do Institution/Actividade/Funcional của despesa không khớp dòng ngân
  // sách nào đã duyệt) với "có ngân sách nhưng đã dùng hết", vì cách xử lý khác nhau.
  private msgSaldoInsuficiente(chaveSemOrcamento: string, chaveExcede: string, saldoDisponivel: number): string {
    const conta = this.despesaRegistadaAutorizada.descricaoOrcamento ?? '';
    const valorPedido = this.despesaRegistadaAutorizada.valorRegistado ?? 0;
    const valorOrcamentado = this.valorDespesaListagem[0] != null ? (this.valorDespesaListagem[0].valorOrcamentado ?? 0) : 0;

    if (valorOrcamentado <= 0) {
      return this.translate.instant(chaveSemOrcamento, {
        conta: conta,
        valor: this.formatarValor(valorPedido)
      });
    }

    return this.translate.instant(chaveExcede, {
      conta: conta,
      disponivel: this.formatarValor(saldoDisponivel),
      valor: this.formatarValor(valorPedido),
      falta: this.formatarValor(valorPedido - saldoDisponivel)
    });
  }

  public autorizarDespesa() {
    // [PT] Reposto a cada acção: este flag era um "latch" que ficava preso a true depois do primeiro
    // caso negativo, impedindo o refresh da lista e o snackbar de TODAS as autorizações seguintes.
    // [VI] Reset ở mỗi lần bấm: cờ này trước đây là "latch" bị kẹt ở true sau ca âm đầu tiên, làm
    // MỌI lần duyệt thành công sau đó không refresh danh sách và không hiện snackbar.
    this.isValueCabimentarDespesaNegative = false;

    const saldoAposCabimentacao = this.valorDespesaListagem[3] != null ? this.valorDespesaListagem[3].valorCabimentado : null;
    const saldoAposAutorizacao = this.valorDespesaListagem[4] != null ? this.valorDespesaListagem[4].valorAutorizado : null;
    const saldoDisponivelCabimentacao = this.valorDespesaListagem[1] != null ? (this.valorDespesaListagem[1].valorCabimentado ?? 0) : 0;
    const saldoDisponivelAutorizacao = this.valorDespesaListagem[1] != null ? (this.valorDespesaListagem[1].valorAutorizado ?? 0) : 0;

    const updateDespesa = () => this.componenteDespesaService.UpdateDespesa({ id: this.despesaRegistadaAutorizada.id, estado: this.despesaRegistadaAutorizada.estado });

    const dialogRef = this.warningDialog.open(PopUpWarningComponent, {
      id: 'updateDespesa',
      minHeight: '300px',
      width: '40%',
      height: '30%',
      panelClass: 'warningModal',
      data: saldoAposCabimentacao != null && saldoAposCabimentacao < 0 ?
        // Bloqueio: a despesa não pode ser autorizada. Mensagem com os valores concretos em vez do
        // aviso genérico anterior, que não indicava quanto faltava nem o que fazer a seguir.
        { function: this.autorizarDespesaValue(), msg: this.msgSaldoInsuficiente('warnings.semOrcamentoAutorizar', 'warnings.saldoInsuficienteAutorizar', saldoDisponivelCabimentacao), noConfirmation: true } :
        saldoAposCabimentacao != null && saldoAposCabimentacao >= 0 &&
          saldoAposAutorizacao != null && saldoAposAutorizacao < 0 ?
          // Aviso (não bloqueia): ainda dá para cabimentar, mas o total autorizado passa a exceder o orçamento.
          { function: updateDespesa(), msg: this.msgSaldoInsuficiente('warnings.semOrcamentoAutorizar', 'warnings.saldoAutorizacaoExcedidoDetalhe', saldoDisponivelAutorizacao) } :
          { function: updateDespesa(), msg: this.translate.instant('warnings.autorizacaoDespesa') }

    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (!this.isValueCabimentarDespesaNegative) {
          this.getAllDadosDropDown();
          openSnackBar(this.translate.instant('snackBar.despesaAutorizada'), this._snackBar);
        }
      }
    });
  }

  public autorizarDespesaValue() {
    this.isValueCabimentarDespesaNegative = true;
  }

  public cabimentarDespesa() {
    // Mesmo reset do latch que em autorizarDespesa() -- ver comentário lá.
    this.isValueCabimentarDespesaNegative = false;

    const saldoAposCabimentacao = this.valorDespesaListagem[3] != null ? this.valorDespesaListagem[3].valorCabimentado : null;
    const saldoDisponivelCabimentacao = this.valorDespesaListagem[1] != null ? (this.valorDespesaListagem[1].valorCabimentado ?? 0) : 0;

    const dialogRef = this.warningDialog.open(PopUpWarningComponent, {
      id: 'updateDespesa',
      minHeight: '300px',
      width: '40%',
      height: '30%',
      panelClass: 'warningModal',
      data: saldoAposCabimentacao != null && saldoAposCabimentacao < 0 ?
        { function: this.cabimentarDespesaValue(), msg: this.msgSaldoInsuficiente('warnings.semOrcamentoCabimentar', 'warnings.saldoInsuficienteCabimentar', saldoDisponivelCabimentacao), noConfirmation: true } :
        { function: this.componenteDespesaService.UpdateDespesa({ id: this.despesaRegistadaAutorizada.id, estado: this.despesaRegistadaAutorizada.estado }), msg: this.translate.instant('warnings.cabimentacaoDespesa') }

    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (!this.isValueCabimentarDespesaNegative) {
          this.getAllDadosDropDown();
          openSnackBar(this.translate.instant('snackBar.despesaCabimentada'), this._snackBar);
        }
      }
    });
  }

  public cabimentarDespesaValue() {
    this.isValueCabimentarDespesaNegative = true;
  }

  public deleteDespesaRegistada() {
    let componenteDespesa: DespesaRegistada = <DespesaRegistada>{};
    componenteDespesa.id = this.despesaRegistadaAutorizada.id;

    this.deleteComponente(componenteDespesa);
  }

  public deleteAllDespesasRegistadas() {
    let ids: number[] = [];
    this.despesaRegistadaAutorizadaListagem.forEach(element => {
      if (element.estado == 'R') {
        ids.push(element.id);
      }
    });
    this.deleteAllComponente(ids);
  }

  public editarDespesaRegistada() {
    this.editarComponente(this.despesaRegistadaAutorizada);
  }

  public executarPagamento() {
    const dialogRef = this.executarPagamentosDialog.open(PopUpExecutarPagamentosComponent, {
      id: 'executarPagamentos',
      minHeight: '500px',
      width: '80%',
      height: '70%',
      panelClass: 'modalWithBorder',
      data: {
        listaDespesaAExecutar: this.listaDespesaCompromissoAExecutar,
        naoExisteOrcamentoAprovado: this.naoExisteOrcamentoAprovado,
        numPagamento: this.numPagamento,
        tarefaActivoId: this.tarefaActivoId,
        listaPagamentosDestinatario: this.listaPagamentosDestinatario,
        processoId: this.processoId,
        totalValorCabimentado: this.totalValorCabimentado,
        totalValorExecutado: this.totalValorExecutado,
        totalValorFaltaExecutar: this.totalValorFaltaExecutar,
        totalValorDestinatarioExecutado: this.totalValorDestinatarioExecutado,
        componenteDespesaConfig: this.componenteDespesaConfig,
        despesaCabimentada: this.listaDespesaCabimentadas
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      this.totalValorExecutado = 0;
      this.totalValorFaltaExecutar = 0;
      this.totalValorCabimentado = 0;
      this.getOrcamentoAprovado(this.tarefaActivoId);
      // this.updateFilteredEtidades(true);
      // this.updateFilteredFuncional(true);

    });
  }

  public listarPagamentos() {

    this.showLoader();
    let request = <ListagemPagamentosProcessoRequest>{
      processoAtivo: this.processoId
    }
    this.pagamentoExecutadoService.GetListaPagamentoExcel(request).subscribe(x => {

      this.hideLoader();
      blobExcelSaveAs(x.excelExtraido, 'Lista_Pagamentos_Executados');


    },
      err => {
        this.hideLoader();
        err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
        this.showError();
      });

  }

  public listarPagamentosPDF(){
    const dialogRef = this.executarPagamentosDialog.open(PopUpListarPagamentosExecutadosComponent, {
      id: 'executarPagamentos',
      minHeight: '500px',
      width: '80%',
      height: '70%',
      panelClass: 'modalWithBorder',
      data: {
        processoAtivoId: this.processoId
      }
    });

    dialogRef.afterClosed().subscribe(result => {
    });
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

  public registarCompromisso() {
    const dialogRef = this.executarPagamentosDialog.open(PopUpCompromissosComponent, {
      id: 'registarCompromisso',
      minHeight: '500px',
      width: '80%',
      height: '70%',
      panelClass: 'modalWithBorder',
      data: {
        listaDespesaAExecutar: this.listaDespesaCabimentadas,
        naoExisteOrcamentoAprovado: this.naoExisteOrcamentoAprovado,
        tarefaActivoId: this.tarefaActivoId,
        processoId: this.processoId,
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      this.getAllDadosDropDown();
    });
  }

  public deleteCompromisso(compromisso: DespesaCompromisso) {
    const dialogRef = this.warningDialog.open(PopUpWarningComponent, {
      id: 'deleteDespesa',
      minHeight: '300px',
      width: '40%',
      height: '30%',
      panelClass: 'warningModal',
      data: { function: this.componenteDespesaService.deleteCompromisso({ id: compromisso.id }), msg: this.translate.instant('warnings.warningDeleteCompromisso') }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.getAllDadosDropDown();
        openSnackBar(this.translate.instant('snackBar.removeCompromisso'), this._snackBar);
      }
    });
  }

  public editarCompromisso(compromisso: DespesaCompromisso) {
    const dialogRef = this.executarPagamentosDialog.open(PopUpCompromissosComponent, {
      id: 'registarCompromisso',
      minHeight: '500px',
      width: '80%',
      height: '70%',
      panelClass: 'modalWithBorder',
      data: {
        listaDespesaAExecutar: this.listaDespesaCabimentadas,
        naoExisteOrcamentoAprovado: this.naoExisteOrcamentoAprovado,
        tarefaActivoId: this.tarefaActivoId,
        processoId: this.processoId,
        compromisso: compromisso
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result)
      this.getAllDadosDropDown();
    });
  }

  public exportCompromissosToExcel() {
    const dataRows = this.listaCompromissos.map((element: any) => ({
      [this.translate.instant('componenteDespesa.despesaCabimentada')]: element.descricaoDespesa,
      [this.translate.instant('general.compromisso')]: element.nomeCompromisso,
      [this.translate.instant('componenteDespesa.valorCompromisso')]: element.valorCompromisso,
    }));

    const worksheet = XLSX.utils.json_to_sheet(dataRows);
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, 'Compromissos');
    const excelBuffer = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
    saveAs(new Blob([excelBuffer], { type: 'application/octet-stream' }), 'Compromissos.xlsx');
  }

  public editarDespesaCabimentada(despesaCabimentada: DespesaRegistada) {
    const dialogRef = this.executarPagamentosDialog.open(PopUpEditDespesaCabimentadaComponent, {
      id: 'editarDespesaCabimentada',
      minHeight: '500px',
      width: '60%',
      height: '60%',
      panelClass: 'modalWithBorder',
      data: {
        despesaCabimentada: despesaCabimentada,
        tarefaActivoId: this.tarefaActivoId,
        processoId: this.processoId,
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result)
      this.getAllDadosDropDown();
    });
  }
}
