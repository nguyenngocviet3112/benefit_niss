import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { CeInssGlobalService } from '../../services/ce-inss-global.service';
import { InstitutionService } from '../../services/institution.service';
import { CeInssGlobalResponse } from '../../response-models/ce-inss-global-response';
import { SelectDescription } from '../../models/utils';

@Component({
  selector: 'app-ce-inss-global',
  templateUrl: './ce-inss-global.component.html',
  styleUrls: ['./ce-inss-global.component.css']
})
export class CeInssGlobalComponent implements OnInit {

  public loading = false;
  public report: CeInssGlobalResponse | null = null;

  public institutions: SelectDescription[] = [];
  public years: number[] = [];

  // undefined = "Cả 2 (Both)" — INSS Global
  public selectedYear = new Date().getFullYear();
  public selectedInstitution?: number;

  constructor(
    private ceInssGlobalService: CeInssGlobalService,
    private institutionService: InstitutionService,
    private snackBar: MatSnackBar,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    const current = new Date().getFullYear();
    this.years = [current - 1, current, current + 1];

    this.institutionService.getAllInstitutionsAtivo().subscribe(
      response => this.institutions = response.selects ?? [],
      () => { /* Organization filter still works with just "Both" if this fails */ }
    );

    this.load();
  }

  public load(): void {
    this.loading = true;
    this.ceInssGlobalService.getReport({
      year: this.selectedYear,
      institution: this.selectedInstitution
    }).subscribe(
      response => {
        this.loading = false;
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.report = response;
      },
      err => {
        this.loading = false;
        const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('ceInssGlobal.errGeneric');
        this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
      }
    );
  }
}
