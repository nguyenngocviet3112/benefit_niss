import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LanguageConfigService } from '../../../services/language-config.service';
import { LanguageConfigDataContract } from '../../../response-models/language-config-response';

@Component({
  selector: 'app-idioma-config',
  templateUrl: './idioma-config.component.html',
  styleUrls: ['./idioma-config.component.css']
})
export class IdiomaConfigComponent implements OnInit {

  public languages: LanguageConfigDataContract[] = [];
  public loading = false;

  constructor(
    private languageConfigService: LanguageConfigService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.load();
  }

  public load(): void {
    this.loading = true;
    this.languageConfigService.getAll().subscribe(
      response => {
        this.languages = response.items ?? [];
        this.loading = false;
      },
      err => {
        this.loading = false;
        this.showError(err);
      }
    );
  }

  public toggle(lang: LanguageConfigDataContract): void {
    const nextValue = !lang.indActivo;
    this.languageConfigService.toggle({ id: lang.id, indActivo: nextValue }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, 'Đóng', { duration: 4000 });
          return;
        }
        this.load();
      },
      err => this.showError(err)
    );
  }

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? 'Có lỗi xảy ra.';
    this.snackBar.open(message, 'Đóng', { duration: 4000 });
  }
}
