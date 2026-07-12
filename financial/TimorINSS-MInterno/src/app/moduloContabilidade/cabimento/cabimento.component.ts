import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { CabimentoService } from '../../services/cabimento.service';
import { AdDisponivelParaCabimentoDataContract, CabimentoDataContract } from '../../response-models/cabimento-response';

const ESTADO_LABELS: { [key: string]: string } = {
  DRAFT: 'cabimento.estadoDraft',
  PENDING_APPROVAL: 'cabimento.estadoPendingApproval',
  APPROVED: 'cabimento.estadoApproved'
};

@Component({
  selector: 'app-cabimento',
  templateUrl: './cabimento.component.html',
  styleUrls: ['./cabimento.component.css']
})
export class CabimentoComponent implements OnInit {

  public ano = 2026;
  public estadoLabels = ESTADO_LABELS;
  public loading = false;
  public items: CabimentoDataContract[] = [];

  public availableAds: AdDisponivelParaCabimentoDataContract[] = [];
  public showAdPicker = false;
  public pickedAd: AdDisponivelParaCabimentoDataContract | null = null;
  public formDescritivo = '';
  public formValorCabimentado: number | null = null;
  public formProcessoAprovisionamentoPrevio = false;
  public formMes = 1;

  public showRejectPrompt = false;
  public rejectComment = '';
  public rejectId: number | null = null;

  public editId: number | null = null;
  public editDescritivo = '';
  public editValorCabimentado: number | null = null;
  public editProcessoAprovisionamentoPrevio = false;

  constructor(
    private cabimentoService: CabimentoService,
    private snackBar: MatSnackBar,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.load();
  }

  public load(): void {
    this.loading = true;
    this.cabimentoService.getByAno(this.ano).subscribe(
      response => {
        this.items = response.items ?? [];
        this.loading = false;
      },
      err => {
        this.loading = false;
        this.showError(err);
      }
    );
  }

  public openAdPicker(): void {
    this.cabimentoService.getAdsDisponiveis(this.ano).subscribe(
      response => {
        this.availableAds = response.items ?? [];
        this.showAdPicker = true;
      },
      err => this.showError(err)
    );
  }

  public pickAd(ad: AdDisponivelParaCabimentoDataContract): void {
    this.pickedAd = ad;
    this.formDescritivo = '';
    this.formValorCabimentado = ad.valorRevisto;
    this.formProcessoAprovisionamentoPrevio = false;
    this.formMes = new Date().getMonth() + 1;
  }

  public cancelAdPicker(): void {
    this.showAdPicker = false;
    this.pickedAd = null;
  }

  public createCabimento(): void {
    if (!this.pickedAd || !this.formValorCabimentado) {
      this.snackBar.open(this.translate.instant('cabimento.errMissingAd'), this.translate.instant('general.close'), { duration: 3000 });
      return;
    }

    this.cabimentoService.create({
      expenditureAuthorizationFk: this.pickedAd.expenditureAuthorizationId,
      descritivo: this.formDescritivo,
      valorCabimentado: this.formValorCabimentado,
      processoAprovisionamentoPrevio: this.formProcessoAprovisionamentoPrevio,
      mes: this.formMes,
      ano: this.ano
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.showAdPicker = false;
        this.pickedAd = null;
        this.snackBar.open(this.translate.instant('cabimento.createdSuccess', { numero: response.item.numero }), this.translate.instant('general.close'), { duration: 3000 });
        this.load();
      },
      err => this.showError(err)
    );
  }

  public openEdit(item: CabimentoDataContract): void {
    this.editId = item.id;
    this.editDescritivo = item.descritivo;
    this.editValorCabimentado = item.valorCabimentado;
    this.editProcessoAprovisionamentoPrevio = !!item.processoAprovisionamentoPrevio;
  }

  public cancelEdit(): void {
    this.editId = null;
  }

  public saveEdit(): void {
    if (this.editId == null || !this.editValorCabimentado) { return; }
    this.cabimentoService.save({
      id: this.editId,
      descritivo: this.editDescritivo,
      valorCabimentado: this.editValorCabimentado,
      processoAprovisionamentoPrevio: this.editProcessoAprovisionamentoPrevio
    }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.editId = null;
        this.load();
      },
      err => this.showError(err)
    );
  }

  public submitCabimento(item: CabimentoDataContract): void {
    if (!confirm(this.translate.instant('cabimento.confirmSubmit', { numero: item.numero }))) { return; }
    this.cabimentoService.submit({ id: item.id }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.load();
      },
      err => this.showError(err)
    );
  }

  public approve(item: CabimentoDataContract): void {
    this.cabimentoService.approve({ id: item.id, approve: true }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.load();
      },
      err => this.showError(err)
    );
  }

  public openRejectPrompt(id: number): void {
    this.rejectId = id;
    this.rejectComment = '';
    this.showRejectPrompt = true;
  }

  public confirmReject(): void {
    if (!this.rejectId) { return; }
    this.cabimentoService.approve({ id: this.rejectId, approve: false, comment: this.rejectComment }).subscribe(
      response => {
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, this.translate.instant('general.close'), { duration: 4000 });
          return;
        }
        this.showRejectPrompt = false;
        this.load();
      },
      err => this.showError(err)
    );
  }

  public cancelReject(): void {
    this.showRejectPrompt = false;
  }

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? this.translate.instant('cabimento.errGeneric');
    this.snackBar.open(message, this.translate.instant('general.close'), { duration: 4000 });
  }
}
