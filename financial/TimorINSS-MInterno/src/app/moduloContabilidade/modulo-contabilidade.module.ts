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
    CompromissoDespesaComponent
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
    MatTooltipModule
  ]
})
export class ModuloContabilidadeModule { }
