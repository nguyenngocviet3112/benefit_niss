import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { InstitutionService } from '../../services/institution.service';
import { SelectDescription } from '../../models/utils';

@Component({
  selector: 'app-organization',
  templateUrl: './organization.component.html',
  styleUrls: ['./organization.component.css']
})
export class OrganizationComponent implements OnInit {

  public institutions: SelectDescription[] = [];
  public loading = false;

  constructor(
    private institutionService: InstitutionService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.load();
  }

  public load(): void {
    this.loading = true;
    this.institutionService.getAllInstitutionsAtivo().subscribe(
      response => {
        this.institutions = response.selects ?? [];
        this.loading = false;
      },
      err => {
        this.loading = false;
        const message = err?.error?.errors?.[0]?.errorMessage ?? 'Có lỗi xảy ra.';
        this.snackBar.open(message, 'Đóng', { duration: 4000 });
      }
    );
  }
}
