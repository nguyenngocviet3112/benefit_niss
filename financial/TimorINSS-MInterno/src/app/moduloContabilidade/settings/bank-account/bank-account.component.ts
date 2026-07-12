import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { BankAccountModel, BankAccountService } from '../../../services/bank-account.service';

@Component({
  selector: 'app-bank-account',
  templateUrl: './bank-account.component.html',
  styleUrls: ['./bank-account.component.css']
})
export class BankAccountComponent implements OnInit {

  public accounts: BankAccountModel[] = [];
  public loading = false;

  public showForm = false;
  public editingId: number | null = null;
  public formEntidadeBancaria = '';
  public formDescricao = '';
  public formSwift = '';
  public formIban = '';
  public formNumero = '';

  constructor(
    private bankAccountService: BankAccountService,
    private snackBar: MatSnackBar,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.load();
  }

  public load(): void {
    this.loading = true;
    this.bankAccountService.getAll().subscribe(
      response => {
        this.accounts = response.items ?? [];
        this.loading = false;
      },
      err => {
        this.loading = false;
        this.showError(err);
      }
    );
  }

  public openAddForm(): void {
    this.editingId = null;
    this.formEntidadeBancaria = '';
    this.formDescricao = '';
    this.formSwift = '';
    this.formIban = '';
    this.formNumero = '';
    this.showForm = true;
  }

  public openEditForm(account: BankAccountModel): void {
    this.editingId = account.id;
    this.formEntidadeBancaria = account.entidadeBancaria ?? '';
    this.formDescricao = account.descricao ?? '';
    this.formSwift = account.swift ?? '';
    this.formIban = account.iban ?? '';
    this.formNumero = account.numero ?? '';
    this.showForm = true;
  }

  public cancelForm(): void {
    this.showForm = false;
  }

  public save(): void {
    if (!this.formEntidadeBancaria.trim() || !this.formIban.trim()) {
      this.snackBar.open(this.translate.instant('bankAccount.errMissingFields'), this.translate.instant('general.close'), { duration: 3000 });
      return;
    }

    this.bankAccountService.save({
      id: this.editingId ?? 0,
      entidadeBancaria: this.formEntidadeBancaria.trim(),
      descricao: this.formDescricao.trim(),
      swift: this.formSwift.trim(),
      iban: this.formIban.trim(),
      numero: this.formNumero.trim()
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.showForm = false;
        this.load();
      },
      err => this.showError(err)
    );
  }

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('bankAccount.errGeneric');
    this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
  }
}
