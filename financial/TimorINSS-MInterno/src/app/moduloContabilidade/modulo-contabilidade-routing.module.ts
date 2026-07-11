import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EstruturaProgramaticaComponent } from './estrutura-programatica/estrutura-programatica.component';
import { ClassificacaoFuncionalComponent } from './classificacao-funcional/classificacao-funcional.component';
import { OrganizationComponent } from './organization/organization.component';
import { ContabilidadeShellComponent } from './contabilidade-shell/contabilidade-shell.component';

const routes: Routes = [
  {
    path: '',
    component: ContabilidadeShellComponent,
    children: [
      { path: 'estruturaProgramatica', component: EstruturaProgramaticaComponent },
      { path: 'classificacaoFuncional', component: ClassificacaoFuncionalComponent },
      { path: 'organization', component: OrganizationComponent },
      { path: '', redirectTo: 'estruturaProgramatica', pathMatch: 'full' },
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ModuloContabilidadeRoutingModule { }
