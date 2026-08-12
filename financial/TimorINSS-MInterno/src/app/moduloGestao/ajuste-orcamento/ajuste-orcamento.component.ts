import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NgxSpinnerService } from 'ngx-spinner';
import { TranslateService } from '@ngx-translate/core';
import { MyErrorStateMatcher } from 'src/app/matcher';
import { ComponenteOrcamentoAjuste, RubricaDisponivel } from 'src/app/models/componenteOrcamentoAjuste';
import { componenteOrcamentoAjusteService } from 'src/app/services/componenteOrcamentoAjuste.service';
import { customCurrencyMaskConfig, openErrorsDialog, openSnackBar, stringErrorFormat } from 'src/app/utils';

@Component({
  selector: 'app-ajuste-orcamento',
  templateUrl: './ajuste-orcamento.component.html',
  styleUrls: ['./ajuste-orcamento.component.css']
})
export class AjusteOrcamentoComponent implements OnInit {

  public errors: string[] = [];
  public currencyOptions = customCurrencyMaskConfig;
  public matcher: MyErrorStateMatcher = new MyErrorStateMatcher();
  public submitted: boolean = false;

  public componenteOrcamentoRegistoFk: number | null = null;
  public rubricas: RubricaDisponivel[] = [];

  // Listas filtradas pelo texto digitado no ngx-mat-select-search de cada select (o código local
  // sozinho não identifica a rubrica -- ver [[codigoconta-local-vs-full-code]] -- por isso o filtro
  // aqui é sempre por texto livre sobre "código - designação" já formatado, não só pelo nome).
  public filteredRubricasOrigemSearch: RubricaDisponivel[] = [];
  public filteredRubricasDestinoSearch: RubricaDisponivel[] = [];

  // deixar vazio ("") = Bonificação (não subtrai de nenhuma rubrica).
  public rubricaOrigemFk: number | '' = '';
  public rubricaDestinoFk: number | null = null;
  public valor: number | null = null;
  public motivo: string = '';

  public pendentes: ComponenteOrcamentoAjuste[] = [];
  public historico: ComponenteOrcamentoAjuste[] = [];

  public motivoRejeicaoPorId: { [id: number]: string } = {};

  @ViewChild('valorInput') valorInputRef?: ElementRef<HTMLInputElement>;

  constructor(
    private ajusteService: componenteOrcamentoAjusteService,
    public translate: TranslateService,
    private spinner: NgxSpinnerService,
    public errorDialog: MatDialog,
    public snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.carregarRubricas();
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

  // err.error.errors nem sempre é um array (ex: excepção não tratada no backend devolve outro
  // formato) -- Array.isArray evita repetir o bug de UI presa em loading confirmado 2026-08-11
  // no ecrã de Retificação de Orçamento (ver [[prod-dev]] Regras_Negocio_Ciclo_Despesa_New_Mode.md #7).
  private handleError(err: any) {
    if (Array.isArray(err?.error?.errors)) {
      err.error.errors.forEach((x: any) => {
        if (typeof x.errorCode === 'string' && x.errorCode.indexOf('ÿ') >= 0) {
          const splitted: string[] = x.errorCode.split('ÿ');
          const code = splitted.shift();
          this.errors.push(stringErrorFormat(this.translate.instant('error.' + code), splitted));
        } else {
          this.errors.push(this.translate.instant('error.' + x.errorCode));
        }
      });
    } else {
      this.errors.push(this.translate.instant('error.-1'));
    }
    this.showError();
  }

  public carregarRubricas() {
    this.showLoader();
    this.ajusteService.GetRubricasDisponiveis().subscribe(x => {
      this.componenteOrcamentoRegistoFk = x.componenteOrcamentoRegistoFk ?? null;
      this.rubricas = x.rubricas;
      this.filteredRubricasOrigemSearch = x.rubricas;
      this.filteredRubricasDestinoSearch = x.rubricas;
      this.hideLoader();

      if (this.componenteOrcamentoRegistoFk) {
        this.carregarListas();
      }
    }, err => {
      this.handleError(err);
    });
  }

  public filterRubricasOrigem(texto: string) {
    this.filteredRubricasOrigemSearch = this.filtrarRubricas(texto);
  }

  public filterRubricasDestino(texto: string) {
    this.filteredRubricasDestinoSearch = this.filtrarRubricas(texto);
  }

  private filtrarRubricas(texto: string): RubricaDisponivel[] {
    if (!texto) {
      return this.rubricas;
    }
    const termo = texto.toLowerCase();
    return this.rubricas.filter(r => r.descricao?.toLowerCase().includes(termo));
  }

  public carregarListas() {
    if (!this.componenteOrcamentoRegistoFk) {
      return;
    }
    this.showLoader();
    this.ajusteService.GetPendentes({ componenteOrcamentoRegistoFk: this.componenteOrcamentoRegistoFk }).subscribe(x => {
      this.pendentes = x.ajustes;
      this.hideLoader();
    }, err => {
      this.handleError(err);
    });

    this.ajusteService.GetHistorico({ componenteOrcamentoRegistoFk: this.componenteOrcamentoRegistoFk }).subscribe(x => {
      this.historico = x.ajustes.filter(a => a.estado !== 'Pendente');
    }, err => {
      this.handleError(err);
    });
  }

  // currencyMask normalmente filtra o que se digita, mas não protege contra valores postos
  // directamente no <input> por fora do teclado (autofill do browser, gestor de password/valores,
  // scripts externos) -- isso deixa o campo com texto livre tipo "dfgdfg" sem o utilizador reparar
  // (bug reproduzido 2026-08-11). Por isso valida aqui de forma independente da mask, tanto no
  // blur (limpa visualmente) como no submit (nunca deixa passar para o backend).
  public onValorBlur() {
    if (typeof this.valor !== 'number' || isNaN(this.valor)) {
      this.valor = null;
      // currencyMask só reformata o <input> quando consegue interpretar um número -- se o texto
      // ficou lá (ex: "dfgdfg" posto por fora do teclado) sem ele conseguir corrigir sozinho,
      // limpa directamente o DOM para o ecrã não continuar a mostrar texto inválido.
      if (this.valorInputRef) {
        this.valorInputRef.nativeElement.value = '';
      }
    }
  }

  public solicitar() {
    this.submitted = true;

    if (typeof this.valor !== 'number' || isNaN(this.valor)) {
      this.valor = null;
    }

    if (!this.rubricaDestinoFk || !this.valor || this.valor <= 0) {
      return;
    }

    this.showLoader();
    this.ajusteService.SolicitarAjuste({
      rubricaOrigemFk: this.rubricaOrigemFk === '' ? undefined : this.rubricaOrigemFk,
      rubricaDestinoFk: this.rubricaDestinoFk,
      valor: this.valor,
      motivo: this.motivo
    }).subscribe(() => {
      this.hideLoader();
      openSnackBar(this.translate.instant('snackBar.ajusteSolicitado'), this.snackBar);
      this.limparFormulario();
      this.carregarRubricas();
    }, err => {
      this.handleError(err);
    });
  }

  public aprovar(ajuste: ComponenteOrcamentoAjuste) {
    this.showLoader();
    this.ajusteService.AprovarAjuste({ id: ajuste.id }).subscribe(() => {
      this.hideLoader();
      openSnackBar(this.translate.instant('snackBar.ajusteAprovado'), this.snackBar);
      this.carregarRubricas();
    }, err => {
      this.handleError(err);
    });
  }

  public rejeitar(ajuste: ComponenteOrcamentoAjuste) {
    this.showLoader();
    this.ajusteService.RejeitarAjuste({ id: ajuste.id, motivoRejeicao: this.motivoRejeicaoPorId[ajuste.id] }).subscribe(() => {
      this.hideLoader();
      openSnackBar(this.translate.instant('snackBar.ajusteRejeitado'), this.snackBar);
      this.carregarRubricas();
    }, err => {
      this.handleError(err);
    });
  }

  public limparFormulario() {
    this.rubricaOrigemFk = '';
    this.rubricaDestinoFk = null;
    this.valor = null;
    this.motivo = '';
    this.submitted = false;
  }

  public rubricaDescricao(id?: number): string {
    if (!id) {
      return this.translate.instant('ajusteOrcamento.semOrigemBonificacao');
    }
    const rubrica = this.rubricas.find(r => r.id === id);
    return rubrica ? rubrica.descricao : String(id);
  }

  // Prévia do impacto antes de aprovar -- usa o saldo/valor CORRENTE de this.rubricas (ainda não
  // mexido por este ajuste pendente) como "antes", e calcula "depois" localmente. Depois de aprovar,
  // carregarRubricas() vai buscar os valores reais actualizados à API -- esta prévia é só indicativa.
  public saldoOrigemAntes(ajuste: ComponenteOrcamentoAjuste): number | null {
    if (!ajuste.rubricaOrigemFk) {
      return null;
    }
    return this.rubricas.find(r => r.id === ajuste.rubricaOrigemFk)?.saldoDisponivel ?? null;
  }

  public saldoOrigemDepois(ajuste: ComponenteOrcamentoAjuste): number | null {
    const antes = this.saldoOrigemAntes(ajuste);
    return antes === null ? null : antes - ajuste.valor;
  }

  public valorDestinoAntes(ajuste: ComponenteOrcamentoAjuste): number | null {
    return this.rubricas.find(r => r.id === ajuste.rubricaDestinoFk)?.valor ?? null;
  }

  public valorDestinoDepois(ajuste: ComponenteOrcamentoAjuste): number | null {
    const antes = this.valorDestinoAntes(ajuste);
    return antes === null ? null : antes + ajuste.valor;
  }
}
