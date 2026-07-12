import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MeuPerfilService } from '../../services/meu-perfil.service';

// Meu Perfil — tự phục vụ: mỗi user chỉ xem/sửa hồ sơ của CHÍNH MÌNH.
// Nome/Departamento chỉ hiển thị (do admin quản lý ở "Quản lý User &
// Phân quyền"); Email và Mật khẩu là 2 khối có thể tự đổi tại đây.
@Component({
  selector: 'app-meu-perfil',
  templateUrl: './meu-perfil.component.html',
  styleUrls: ['./meu-perfil.component.css']
})
export class MeuPerfilComponent implements OnInit {

  public loading = false;
  public username = '';
  public nome = '';
  public departamentoNome = '';

  public formEmail = '';
  public savingEmail = false;

  public formSenhaAtual = '';
  public formSenhaNova = '';
  public formConfirmarSenhaNova = '';
  public savingSenha = false;

  constructor(
    private meuPerfilService: MeuPerfilService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.load();
  }

  public load(): void {
    this.loading = true;
    this.meuPerfilService.getProfile().subscribe(
      response => {
        this.username = response.username;
        this.nome = response.nome;
        this.departamentoNome = response.departamentoNome;
        this.formEmail = response.email ?? '';
        this.loading = false;
      },
      err => {
        this.loading = false;
        this.showError(err);
      }
    );
  }

  public salvarEmail(): void {
    if (!this.formEmail) {
      this.snackBar.open('Vui lòng nhập Email.', 'Đóng', { duration: 3000 });
      return;
    }

    this.savingEmail = true;
    this.meuPerfilService.alterarEmail({ email: this.formEmail }).subscribe(
      response => {
        this.savingEmail = false;
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, 'Đóng', { duration: 4000 });
          return;
        }
        this.snackBar.open('Đã cập nhật Email.', 'Đóng', { duration: 2500 });
      },
      err => {
        this.savingEmail = false;
        this.showError(err);
      }
    );
  }

  public alterarSenha(): void {
    if (!this.formSenhaAtual || !this.formSenhaNova || !this.formConfirmarSenhaNova) {
      this.snackBar.open('Vui lòng nhập đủ Mật khẩu hiện tại, Mật khẩu mới và Xác nhận.', 'Đóng', { duration: 3500 });
      return;
    }
    if (this.formSenhaNova !== this.formConfirmarSenhaNova) {
      this.snackBar.open('Mật khẩu mới và Xác nhận không khớp.', 'Đóng', { duration: 3500 });
      return;
    }

    this.savingSenha = true;
    this.meuPerfilService.alterarSenha({
      senhaAtual: this.formSenhaAtual,
      senhaNova: this.formSenhaNova,
      confirmarSenhaNova: this.formConfirmarSenhaNova
    }).subscribe(
      response => {
        this.savingSenha = false;
        if (response.errors && response.errors.length > 0) {
          this.snackBar.open(response.errors[0].errorMessage, 'Đóng', { duration: 4000 });
          return;
        }
        this.formSenhaAtual = '';
        this.formSenhaNova = '';
        this.formConfirmarSenhaNova = '';
        this.snackBar.open('Đã đổi mật khẩu.', 'Đóng', { duration: 2500 });
      },
      err => {
        this.savingSenha = false;
        this.showError(err);
      }
    );
  }

  private showError(err: any): void {
    const message = err?.error?.errors?.[0]?.errorMessage ?? 'Có lỗi xảy ra.';
    this.snackBar.open(message, 'Đóng', { duration: 4000 });
  }
}
