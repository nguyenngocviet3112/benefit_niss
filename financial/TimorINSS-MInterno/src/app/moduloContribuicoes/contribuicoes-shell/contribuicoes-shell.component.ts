import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { EntidadeEmpregadoraNissRequest } from '../../request-models/entidadeEmpregadora-request';
import { EntidadeEmpregadoraService } from '../../services/entidadeEmpregadora.service';
import { TokenStorageService } from '../../services/token-storage.service';
import { openErrorsDialog } from '../../utils';

@Component({
  selector: 'app-contribuicoes-shell',
  templateUrl: './contribuicoes-shell.component.html',
  styleUrls: ['./contribuicoes-shell.component.css']
})
export class ContribuicoesShellComponent {

  public nissRequest?: string;
  public errors: string[] = [];

  constructor(
    private router: Router,
    private tokenStorage: TokenStorageService,
    private entidadeEmpregadoraService: EntidadeEmpregadoraService,
    private spinner: NgxSpinnerService,
    public errorDialog: MatDialog,
  ) { }

  public searchOtherCompany(): void {
    if (!this.nissRequest) {
      return;
    }

    this.spinner.show();
    const request = <EntidadeEmpregadoraNissRequest>{ niss: this.nissRequest };
    this.entidadeEmpregadoraService.GetEntidadeByNiss(request).subscribe(
      response => {
        const user = this.tokenStorage.getUser();
        if (user) {
          user.idEntidade = response.idEntidadeEmpreg;
          this.tokenStorage.saveUser(user);
        }
        this.nissRequest = undefined;
        window.location.reload();
      },
      err => {
        this.spinner.hide();
        err.error?.errors ? err.error.errors.map((x: any) => this.errors.push(x.errorCode)) : this.errors.push('-1');
        this.showError();
      });
  }

  public showError(): void {
    const dialogRef = openErrorsDialog(this.errors, this.errorDialog);
    this.spinner.hide();
    dialogRef.afterClosed().subscribe(() => {
      this.errors = [];
    });
  }
}
