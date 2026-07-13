import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxMatFileInputModule } from '@angular-material-components/file-input';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { TranslateModule } from '@ngx-translate/core';
import { PdfViewerModule } from 'ng2-pdf-viewer';

import { ModuloContabilidadeRoutingModule } from './modulo-contabilidade-routing.module';
import { EstruturaProgramaticaComponent } from './estrutura-programatica/estrutura-programatica.component';
import { ClassificacaoFuncionalComponent } from './classificacao-funcional/classificacao-funcional.component';
import { OrganizationComponent } from './organization/organization.component';
import { ContabilidadeShellComponent } from './contabilidade-shell/contabilidade-shell.component';
import { ClassificacaoEconomicaComponent } from './classificacao-economica/classificacao-economica.component';
import { MasterDataImportButtonComponent } from './master-data-import-button/master-data-import-button.component';
import { OrcamentoComponent } from './orcamento/orcamento.component';
import { AdCabimentoComponent } from './ad-cabimento/ad-cabimento.component';
import { CeInssGlobalComponent } from './ce-inss-global/ce-inss-global.component';
import { CabimentoComponent } from './cabimento/cabimento.component';
import { CompromissoDespesaComponent } from './compromisso-despesa/compromisso-despesa.component';
import { ObligationComponent } from './obligation/obligation.component';
import { IdiomaConfigComponent } from './settings/idioma-config/idioma-config.component';
import { KyNganSachComponent } from './settings/ky-ngan-sach/ky-ngan-sach.component';
import { UserPermissionComponent } from './user-permission/user-permission.component';
import { UserSyncComponent } from './user-sync/user-sync.component';
import { PaymentComponent } from './payment/payment.component';
import { OrcamentoConfigSelectComponent } from './orcamento-config-select/orcamento-config-select.component';
import { ReceitaPacComponent } from './receita-pac/receita-pac.component';
import { DepartamentoConfigComponent } from './departamento-config/departamento-config.component';
import { OrcamentoSuplementarComponent } from './orcamento-suplementar/orcamento-suplementar.component';
import { SaldosAberturaComponent } from './saldos-abertura/saldos-abertura.component';
import { BankAccountComponent } from './settings/bank-account/bank-account.component';
import { PlanoContasComponent } from './plano-contas/plano-contas.component';
import { MapeamentoRubricasComponent } from './mapeamento-rubricas/mapeamento-rubricas.component';
import { LancamentosComponent } from './lancamentos/lancamentos.component';
import { ConciliacaoMovimentosComponent } from './conciliacao-movimentos/conciliacao-movimentos.component';
import { MeuPerfilComponent } from './meu-perfil/meu-perfil.component';
import { OrcamentoImportPreviewComponent } from './orcamento-import-preview/orcamento-import-preview.component';
import { GuiaConciliacaoComponent } from './guia-conciliacao/guia-conciliacao.component';
import { GuiaPagamentoContaConfigComponent } from './settings/guia-pagamento-conta-config/guia-pagamento-conta-config.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { CicloDespesaComponent } from './ciclo-despesa/ciclo-despesa.component';
import { AttachmentUploadComponent } from './attachment-upload/attachment-upload.component';
import { AttachmentConfigComponent } from './settings/attachment-config/attachment-config.component';
import { LiquidacaoContaConfigComponent } from './settings/liquidacao-conta-config/liquidacao-conta-config.component';

@NgModule({
  declarations: [
    DashboardComponent,
    EstruturaProgramaticaComponent,
    ClassificacaoFuncionalComponent,
    OrganizationComponent,
    ContabilidadeShellComponent,
    ClassificacaoEconomicaComponent,
    MasterDataImportButtonComponent,
    OrcamentoComponent,
    AdCabimentoComponent,
    CeInssGlobalComponent,
    CabimentoComponent,
    CompromissoDespesaComponent,
    ObligationComponent,
    IdiomaConfigComponent,
    KyNganSachComponent,
    UserPermissionComponent,
    UserSyncComponent,
    PaymentComponent,
    OrcamentoConfigSelectComponent,
    ReceitaPacComponent,
    DepartamentoConfigComponent,
    OrcamentoSuplementarComponent,
    SaldosAberturaComponent,
    BankAccountComponent,
    PlanoContasComponent,
    MapeamentoRubricasComponent,
    LancamentosComponent,
    ConciliacaoMovimentosComponent,
    MeuPerfilComponent,
    OrcamentoImportPreviewComponent,
    GuiaConciliacaoComponent,
    GuiaPagamentoContaConfigComponent,
    LiquidacaoContaConfigComponent,
    CicloDespesaComponent,
    AttachmentUploadComponent,
    AttachmentConfigComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NgxMatFileInputModule,
    ModuloContabilidadeRoutingModule,
    MatButtonModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatSelectModule,
    MatSnackBarModule,
    MatTooltipModule,
    MatPaginatorModule,
    MatAutocompleteModule,
    MatCheckboxModule,
    MatDatepickerModule,
    MatNativeDateModule,
    TranslateModule,
    PdfViewerModule
  ]
})
export class ModuloContabilidadeModule { }
