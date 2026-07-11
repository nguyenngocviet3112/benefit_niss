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

@NgModule({
  declarations: [
    EstruturaProgramaticaComponent,
    ClassificacaoFuncionalComponent,
    OrganizationComponent,
    ContabilidadeShellComponent
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
