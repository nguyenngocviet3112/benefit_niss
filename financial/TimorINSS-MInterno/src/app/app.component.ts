import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { environment } from 'src/environments/environment';
import { TokenStorageService } from './services/token-storage.service';
import '@angular/common/locales/global/pt';
import { MenuItem } from './models/utils';
import { CreateMenuPermissions } from './utils';
import { Router, NavigationEnd } from '@angular/router';
import { LanguageConfigService } from './services/language-config.service';

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

  // Danh sách ngôn ngữ hiển thị trong dropdown giờ đọc từ cấu hình Admin
  // (bảng LanguageConfig, màn /contabilidade/settings/idioma) thay vì hardcode.
  // Giữ nguyên set 4 ngôn ngữ này làm fallback nếu chưa gọi được API (offline,
  // lỗi mạng) — để không vỡ màn hình như hành vi hardcode trước đây.
  private activeLangCodes: string[] = ['EN', 'PT', 'TET', 'VI'];

  constructor(
    public translate: TranslateService,
    private tokenStorageService: TokenStorageService,
    public errorDialog: MatDialog,
    private router: Router,
    private languageConfigService: LanguageConfigService
  ) {
    translate.setDefaultLang('PT');
    translate.use('PT');
    // Đăng ký với ngx-translate toàn bộ 4 ngôn ngữ có file dịch sẵn
    // (src/assets/i18n/*.json) — việc ngôn ngữ nào THỰC SỰ được phép chọn do
    // activeLangCodes (đọc từ LanguageConfig) quyết định, không phải danh sách này.
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
    // VI chỉ hiện trong danh sách chọn khi đang ở giao diện mới.
    this.TransLang = this.isNewMode ? this.activeLangCodes : this.activeLangCodes.filter(l => l !== 'VI');
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
    this.applySavedOrDefaultLanguage();

    // GetActive là API public (không cần token) vì dropdown này phải hoạt động
    // cả ở màn hình login. Lỗi mạng thì giữ nguyên fallback hardcode ở trên.
    this.languageConfigService.getActive().subscribe(
      response => {
        const codes = (response.items ?? []).map(i => i.codigo);
        if (codes.length > 0) {
          this.activeLangCodes = codes;
        }
        this.getTransLanguage();

        // Ngôn ngữ đang chọn vừa bị Admin tắt (hoặc trước đó lưu ngôn ngữ không
        // còn active) — chuyển về ngôn ngữ đầu tiên còn bật để tránh kẹt UI.
        if (!this.TransLang.includes(this.selectLang) && this.TransLang.length > 0) {
          this.selectLang = this.TransLang[0];
          this.translate.use(this.selectLang);
        }
      },
      () => { /* giữ nguyên fallback hardcode nếu API lỗi */ }
    );

    if (this.isLoggedIn) {
      const user = this.tokenStorageService.getUser();
      this.menuItems = CreateMenuPermissions(user);

      if (user != null) {
        this.username = user.username;
        this.profiles = user.perfil;
      }
    }
  }

  private applySavedOrDefaultLanguage(): void {
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
