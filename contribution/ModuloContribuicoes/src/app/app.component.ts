import { DecimalPipe } from '@angular/common';
import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { environment } from 'src/environments/environment';
import { PopUpWarningComponent } from './componentes/pop-up-warning/pop-up-warning.component';
import { TokenStorageService } from './services/token-storage.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  private roles: string[] = [];
  public isLoggedIn = false;
  public showAdminBoard = false;
  public showModeratorBoard = false;
  public username?: string;
  public niss?: string;
  public selected = 'option2';
  public ssIcon = environment.ssIcon;
  public selectLang: string = "";
  public TransLang: string[] = [];
  public isLoginRoute = false;

  constructor(
    private tokenStorageService: TokenStorageService,
    public translate: TranslateService,
    public decimalpipe: DecimalPipe,
    public errorDialog: MatDialog,
    ) {
      translate.setDefaultLang('PT');
      translate.addLangs(['EN', 'PT', 'TET']);
      translate.onLangChange.subscribe(() => {
        document.title = this.translate.instant('general.inssPortal');
      });
     }


  public setTransLanguage(){
    this.translate.use(this.selectLang);
  }
  public getTransLanguage(){
    this.TransLang = [...this.translate.getLangs()];
  }

  public ngOnInit(): void {
    this.isLoggedIn = !!this.tokenStorageService.getToken();
    this.isLoginRoute = window.location.pathname == '/login';

    this.getTransLanguage();
    this.selectLang = this.translate.getDefaultLang();

    if(this.tokenStorageService.getToken() && this.tokenStorageService.tokenExpired()) {
      this.translate.get('error.expired').subscribe((translated: string) => {
        const dialogRef = this.errorDialog.open(PopUpWarningComponent, {
          id: 'desvincularDialog',
          minHeight: '300px',
          width: '40%',
          height: '30%',
          panelClass: 'warningModal',
          data: {msg: translated, noGenericMsg: true}
          });
          dialogRef.afterClosed().subscribe(() => {
            this.logout();
          });
      });
    }
    else if (this.isLoggedIn) {
      const user = this.tokenStorageService.getUser();

      this.showAdminBoard = this.roles.includes('ROLE_ADMIN');
      this.showModeratorBoard = this.roles.includes('ROLE_MODERATOR');

      if(user != null)
      {
        this.username = user.username;
        this.niss = user.niss;
      }
    }
  }

  public logout(): void {
    this.tokenStorageService.signOut();
    window.location.reload();
  }
}
