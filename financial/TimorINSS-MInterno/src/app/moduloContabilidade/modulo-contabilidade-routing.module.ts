import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EstruturaProgramaticaComponent } from './estrutura-programatica/estrutura-programatica.component';
import { ClassificacaoFuncionalComponent } from './classificacao-funcional/classificacao-funcional.component';
import { OrganizationComponent } from './organization/organization.component';
import { ContabilidadeShellComponent } from './contabilidade-shell/contabilidade-shell.component';
import { ClassificacaoEconomicaComponent } from './classificacao-economica/classificacao-economica.component';
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

const routes: Routes = [
  {
    path: '',
    component: ContabilidadeShellComponent,
    children: [
      { path: 'estruturaProgramatica', component: EstruturaProgramaticaComponent },
      { path: 'classificacaoFuncional', component: ClassificacaoFuncionalComponent },
      { path: 'classificacaoEconomica', component: ClassificacaoEconomicaComponent },
      { path: 'organization', component: OrganizationComponent },
      { path: 'orcamento', component: OrcamentoComponent },
      { path: 'adCabimento', component: AdCabimentoComponent },
      { path: 'cabimento', component: CabimentoComponent },
      { path: 'compromissoDespesa', component: CompromissoDespesaComponent },
      { path: 'obligation', component: ObligationComponent },
      { path: 'payment', component: PaymentComponent },
      { path: 'relatorios/ceInssGlobal', component: CeInssGlobalComponent },
      { path: 'settings/idioma', component: IdiomaConfigComponent },
      { path: 'settings/kyNganSach', component: KyNganSachComponent },
      { path: 'userPermission', component: UserPermissionComponent },
      { path: '', redirectTo: 'estruturaProgramatica', pathMatch: 'full' },
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ModuloContabilidadeRoutingModule { }
