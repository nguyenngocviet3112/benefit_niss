import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatAutocompleteModule } from '@angular/material/autocomplete';

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
import { PaymentComponent } from './payment/payment.component';
import { OrcamentoConfigSelectComponent } from './orcamento-config-select/orcamento-config-select.component';
import { ReceitaPacComponent } from './receita-pac/receita-pac.component';
import { DepartamentoConfigComponent } from './departamento-config/departamento-config.component';
import { OrcamentoSuplementarComponent } from './orcamento-suplementar/orcamento-suplementar.component';
import { SaldosAberturaComponent } from './saldos-abertura/saldos-abertura.component';
import { BankAccountComponent } from './settings/bank-account/bank-account.component';
import { PlanoContasComponent } from './plano-contas/plano-contas.component';
import { MapeamentoRubricasComponent } from './mapeamento-rubricas/mapeamento-rubricas.component';
import { ConciliacaoMovimentosComponent } from './conciliacao-movimentos/conciliacao-movimentos.component';

@NgModule({
  declarations: [
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
    PaymentComponent,
    OrcamentoConfigSelectComponent,
    ReceitaPacComponent,
    DepartamentoConfigComponent,
    OrcamentoSuplementarComponent,
    SaldosAberturaComponent,
    BankAccountComponent,
    PlanoContasComponent,
    MapeamentoRubricasComponent,
    ConciliacaoMovimentosComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ModuloContabilidadeRoutingModule,
    MatButtonModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatSelectModule,
    MatSnackBarModule,
    MatTooltipModule,
    MatPaginatorModule,
    MatAutocompleteModule
  ]
})
export class ModuloContabilidadeModule { }
