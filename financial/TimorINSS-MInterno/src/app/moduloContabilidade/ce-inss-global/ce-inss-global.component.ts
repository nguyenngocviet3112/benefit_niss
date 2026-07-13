import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { CeInssGlobalService } from '../../services/ce-inss-global.service';
import { InstitutionService } from '../../services/institution.service';
import { CeInssGlobalResponse } from '../../response-models/ce-inss-global-response';
import { SelectDescription } from '../../models/utils';
import { blobToSaveAs } from '../../utils';

@Component({
  selector: 'app-ce-inss-global',
  templateUrl: './ce-inss-global.component.html',
  styleUrls: ['./ce-inss-global.component.css']
})
export class CeInssGlobalComponent implements OnInit {

  public loading = false;
  public report: CeInssGlobalResponse | null = null;

  public institutions: SelectDescription[] = [];
  public filteredOrganizations: SelectDescription[] = [];
  public organizationSearch = '';
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
    this.organizationSearch = this.translate.instant('ceInssGlobal.bothOption');

    this.institutionService.getAllInstitutionsAtivo().subscribe(
      response => {
        this.institutions = response.selects ?? [];
        this.filteredOrganizations = this.institutions;
      },
      () => { /* Organization filter still works with just "Both" if this fails */ }
    );

    this.load();
  }

  private normalize(value: string): string {
    return (value || '').toLowerCase();
  }

  public onOrganizationSearchChange(): void {
    const term = this.normalize(this.organizationSearch);
    this.filteredOrganizations = this.institutions.filter(o => this.normalize(o.nome).includes(term));
  }

  public selectOrganization(o?: SelectDescription): void {
    this.selectedInstitution = o?.id;
    this.organizationSearch = o ? o.nome : this.translate.instant('ceInssGlobal.bothOption');
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

  public exportarExcel(): void {
    this.ceInssGlobalService.getReportExcel({
      year: this.selectedYear,
      institution: this.selectedInstitution
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        blobToSaveAs(response.file, `CE_OSS_Global_${this.selectedYear}.xlsx`);
      },
      err => {
        const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('ceInssGlobal.errGeneric');
        this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
      }
    );
  }
}
