import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { environment } from 'src/environments/environment';
import { TokenStorageService } from './services/token-storage.service';
import '@angular/common/locales/global/pt';
import { MenuItem } from './models/utils';
import { CreateMenuPermissions } from './utils';
import { Router, NavigationEnd } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  public ssIcon = environment.ssIcon;
  public isLoggedIn = true;
  public isLoginRoute = false;
  // true khi đang ở Módulo Contabilidade (mode mới) — ẩn menu dropdown cũ,
  // vì module mới có treebar tĩnh riêng của nó.
  public isNewMode = false;
  public selectLang: string = "";
  public TransLang: string[] = [];
  public username?: string;
  public profiles?: string;
  public menuItems: MenuItem[] = [];

  constructor(
    public translate: TranslateService,
    private tokenStorageService: TokenStorageService,
    public errorDialog: MatDialog,
    private router: Router
  ) {
    translate.setDefaultLang('PT');
    translate.use('PT');
    // VI chỉ dùng cho giao diện mới (Módulo Contabilidade) — không hiện ở mode cũ,
    // để mode cũ giữ nguyên hành vi/danh sách ngôn ngữ như trước.
    translate.addLangs(['EN', 'PT', 'TET', 'VI']);
    translate.onLangChange.subscribe(() => {
      document.title = this.translate.instant('general.inssCore');
    });
  }

  public setTransLanguage() {
    this.translate.use(this.selectLang);
    localStorage.setItem('selectedLanguage', this.selectLang);
  }
  public getTransLanguage() {
    const allLangs = [...this.translate.getLangs()];
    // VI chỉ hiện trong danh sách chọn khi đang ở giao diện mới.
    this.TransLang = this.isNewMode ? allLangs : allLangs.filter(l => l !== 'VI');
  }

  public ngOnInit(): void {
    this.isLoggedIn = !!this.tokenStorageService.getToken();
    this.isLoginRoute = window.location.pathname == '/login';
    this.isNewMode = window.location.pathname.startsWith('/contabilidade');

    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.isNewMode = event.urlAfterRedirects.startsWith('/contabilidade');
        this.getTransLanguage();
      }
    });

    this.getTransLanguage();

    const savedLanguage = localStorage.getItem('selectedLanguage');
    if (savedLanguage && this.translate.getLangs().includes(savedLanguage) && (savedLanguage !== 'VI' || this.isNewMode)) {
      // Sử dụng ngôn ngữ đã lưu (VI chỉ áp dụng khi đang ở giao diện mới)
      this.selectLang = savedLanguage;
      this.translate.use(savedLanguage);
    } else if (this.isNewMode) {
      // Giao diện mới: tạm thời mặc định tiếng Việt (2026-07-11, user yêu cầu) —
      // không đổi mặc định của mode cũ (vẫn PT như trước).
      this.selectLang = 'VI';
      this.translate.use('VI');
    } else {
      // Mode cũ: giữ nguyên ngôn ngữ mặc định như trước (PT).
      this.selectLang = this.translate.getDefaultLang();
      this.translate.use(this.selectLang);
    }
    
    if (this.isLoggedIn) {
      const user = this.tokenStorageService.getUser();
      this.menuItems = CreateMenuPermissions(user);

      if (user != null) {
        this.username = user.username;
        this.profiles = user.perfil;
      }
    }
  }

  public logout(): void {
  // Lưu ngôn ngữ hiện tại trước khi đăng xuất
  const currentLanguage = this.selectLang;
  
  // Đăng xuất và xóa token
  this.tokenStorageService.signOut();
  
  // Đặt lại ngôn ngữ đã lưu (để đảm bảo nó không bị xóa trong quá trình signOut)
  localStorage.setItem('selectedLanguage', currentLanguage);
  
  // Làm mới trang
  window.location.reload();
}
}
