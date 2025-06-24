import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {customCurrencyMaskConfig, RegexPatterns} from "../../utils";
import {faTimesCircle} from "@fortawesome/free-solid-svg-icons";
import {MyErrorStateMatcher} from "../../matcher";
import {
  PopUpAdicionarEditarContatoData
} from "../../moduloContribuicoes/pop-up-adicionar-editar-contato/pop-up-adicionar-editar-contato.component";

export interface PopUpAddUserData {
  NISS: string;
  Email?: string;
  InternalUser?: boolean;

}
@Component({
  selector: 'app-pop-up-add-user',
  templateUrl: './pop-up-add-user.component.html',
  styleUrls: ['./pop-up-add-user.component.css']
})
export class PopUpAddUserComponent implements OnInit {
  public availableRegex = RegexPatterns;
  public matcher: MyErrorStateMatcher = new MyErrorStateMatcher();
  public submittedTry: boolean = false;
  constructor(
    public dialogRef: MatDialogRef<PopUpAddUserComponent>,
    @Inject(MAT_DIALOG_DATA) public data: PopUpAddUserData
  ) { }

  ngOnInit(): void {
  }

  public closePopUp(value: boolean = false): void {
    this.dialogRef.close(value);
  }


  public approve() {
    this.submittedTry = true;
    if (this.data.NISS && this.data.Email?.match(this.availableRegex.emailPattern)) {
      this.dialogRef.close(this.data); // trả về dữ liệu
    }
  }


  protected readonly currencyOptions = customCurrencyMaskConfig;
}
