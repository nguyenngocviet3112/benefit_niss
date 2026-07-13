import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TokenStorageService } from '../../services/token-storage.service';
import { PermissionService } from '../../services/permission.service';

interface TreebarLeaf {
  label: string;
  route?: string;
  comingSoon?: boolean;
  // Token nào trong danh sách này mà user có là đủ để thấy mục — để trống
  // (undefined) nghĩa là ai đăng nhập cũng thấy (không cần quyền riêng),
  // vd Meu Perfil hoặc các màn Đóng góp BHXH tái dùng nguyên bản mode cũ.
  permTokens?: string[];
}

interface TreebarGroup {
  label: string;
  icon: string;
  items: TreebarLeaf[];
  expanded: boolean;
}

@Component({
  selector: 'app-contabilidade-shell',
  templateUrl: './contabilidade-shell.component.html',
  styleUrls: ['./contabilidade-shell.component.css']
})
export class ContabilidadeShellComponent implements OnInit {

  private currentPerms: string[] = [];

  // Tính 1 LẦN trong ngOnInit, không phải getter — một getter gọi lại mỗi
  // vòng change-detection và tạo object mới mỗi lần sẽ khiến *ngFor coi danh
  // sách là "khác hoàn toàn" mỗi vòng, hủy/tạo lại toàn bộ DOM (kể cả
  // routerLinkActive) — routerLinkActive lại tự kích thêm 1 vòng CD khi được
  // tạo mới, tạo thành vòng lặp vô hạn ngay lúc điều hướng (treo tab khi
  // click menu, phát hiện 2026-07-12 với account không phải ADMIN).
  public visibleGroups: TreebarGroup[] = [];

  constructor(
    private tokenStorage: TokenStorageService,
    private permissionService: PermissionService,
    private router: Router
  ) { }

  ngOnInit(): void {
    // Cùng cơ chế bảo vệ route như HomePageComponent (mode cũ): không có token
    // (kể cả sau khi logout) thì đẩy về /login — trước đây shell này thiếu
    // check này nên bấm logout ở mode mới không thoát ra được.
    if (!this.tokenStorage.getToken()) {
      this.router.navigate(['/login']);
    }
    this.currentPerms = this.permissionService.getCurrentPerms();
    this.visibleGroups = this.groups
      .map(group => ({ ...group, items: group.items.filter(item => this.canSee(item)) }))
      .filter(group => group.items.length > 0);
  }

  // Ẩn/hiện mục menu theo quyền hiện có (2026-07-12, user yêu cầu) — chỉ là
  // dọn giao diện, KHÔNG phải lớp bảo mật mới: backend vẫn là nơi chặn thật
  // qua [RequirePerm] trên các hành động ghi/sửa (xem RBAC enforcement).
  // Hầu hết endpoint GET/xem không bị chặn — nên mục không có permTokens
  // (undefined) mặc định LUÔN hiện.
  private canSee(item: TreebarLeaf): boolean {
    if (!item.permTokens || item.permTokens.length === 0) {
      return true;
    }
    if (this.currentPerms.includes('ADMIN')) {
      return true;
    }
    return item.permTokens.some(t => this.currentPerms.includes(t));
  }

  // Cây menu tĩnh (hardcoded) — KHÔNG tính toán động theo permissions như menu cũ.
  // Cấu trúc phản ánh đầy đủ wireframe M0-M4; các mục chưa code xong đánh dấu comingSoon.
  // Thứ tự nhóm theo yêu cầu (2026-07-11, cập nhật 2026-07-12): Chi tiêu → Thu
  // → Ngân hàng → Ngân sách (gồm cả Cut-over/Saldos de Abertura) → Báo cáo
  // → Đóng góp BHXH → Master Data → Cấu hình hệ thống (cuối cùng, gồm cả
  // Quản lý User & Phân quyền). Cut-over gộp chung vào Ngân sách — không còn
  // là nhóm riêng. Đóng góp BHXH đặt dưới Báo cáo, trên Master Data. Quản lý
  // User không còn là nhóm top-level riêng — gộp vào Cấu hình hệ thống
  // (2026-07-12, theo yêu cầu user). Tất cả nhóm mặc định collapse
  // (expanded: false), user tự click để mở nhóm đang cần.
  // Dashboard KHÔNG có mục menu riêng (2026-07-12, user yêu cầu) — chỉ là màn
  // mặc định khi đăng nhập qua redirect (xem modulo-contabilidade-routing.module.ts,
  // route '' redirectTo 'dashboard'), không xuất hiện trong treebar.
  public groups: TreebarGroup[] = [
    {
      label: 'shell.groupChiTieu',
      icon: 'sync_alt',
      expanded: false,
      items: [
        { label: 'shell.itemAd', route: '/contabilidade/adCabimento', permTokens: ['AD_SUBMIT', 'AD_REVIEW', 'AD_APPROVE'] },
        { label: 'shell.itemCabimento', route: '/contabilidade/cabimento', permTokens: ['CABIMENTO_SUBMIT', 'CABIMENTO_APPROVE'] },
        { label: 'shell.itemCompromisso', route: '/contabilidade/compromissoDespesa', permTokens: ['COMPROMISSO_SUBMIT', 'COMPROMISSO_REVIEW', 'COMPROMISSO_APPROVE'] },
        { label: 'shell.itemObrigacao', route: '/contabilidade/obligation', permTokens: ['OBRIGACAO_SUBMIT', 'OBRIGACAO_APPROVE'] },
        { label: 'shell.itemPagamento', route: '/contabilidade/payment', permTokens: ['PAG_SUBMIT', 'PAG_APPROVE', 'PAG_EXECUTE'] },
      ]
    },
    {
      label: 'shell.groupReceita',
      icon: 'payments',
      expanded: false,
      items: [
        { label: 'shell.itemReceitaPac', route: '/contabilidade/receita', permTokens: ['REC_SUBMIT'] },
        // Receita GP (contribuições) — 2026-07-12, thay thế: màn "Guia Pagamento
        // (Validação)" cũ (mode cũ, dưới nhóm Đóng góp BHXH) chỉ có tác dụng
        // xem — Duyệt ở đó set Paid/Partial Paid chỉ dựa vào officer đọc chứng từ,
        // không đối chiếu với sao kê ngân hàng thật (gap user chỉ ra 2026-07-12).
        // Màn mới "Duyệt Guia Pagamento (đối chiếu ngân hàng)" bắt buộc chọn khớp
        // dòng sao kê ngân hàng thật trước khi set Paid — dùng chung permTokens
        // 'BANCO_CONCILIAR' vì cùng hoạt động đối chiếu ngân hàng, chỉ khác
        // nguồn tiền vào (GuiaPagamento thay vì ReceitaPac/PaymentExecution).
        { label: 'shell.itemGuiaConciliacao', route: '/contabilidade/receita/guiaConciliacao', permTokens: ['BANCO_CONCILIAR'] },
      ]
    },
    {
      label: 'shell.groupNganHang',
      icon: 'account_balance',
      expanded: false,
      items: [
        { label: 'shell.itemConciliacaoMovimentos', route: '/contabilidade/conciliacaoMovimentos', permTokens: ['BANCO_CONCILIAR'] },
      ]
    },
    {
      // Nhóm mới 2026-07-11 — phát hiện từ SCFSSTL2024_VF.xlsm/FRSSVF.xlsm
      // (2 file khách cung cấp mới nạp): hệ thống kế toán kép (double-entry)
      // hoàn toàn khác với chu trình chấp hành ngân sách (Chu trình chi tiêu/
      // Thu) đã build — sổ Nợ/Có (Lançamentos/DB/CR) là dữ liệu giao dịch,
      // không phải báo cáo, nên tách nhóm riêng thay vì bỏ vào Báo cáo.
      // 2026-07-12: build xong — CHỈ XEM (không nhập tay), bút toán tự sinh
      // từ Pagamento thực hiện (dùng Débito/Crédito đã lưu ở PaymentAuthorization)
      // và Receita (follow-up, ReceitaPac chưa có cột Débito/Crédito — xem
      // memory ce-inss-global-impl-status / financial-statements-scope-gap).
      // Màn chỉ xem, không có RequirePerm nào cả — không gắn permTokens.
      label: 'shell.groupContabilidadeGeral',
      icon: 'menu_book',
      expanded: false,
      items: [
        { label: 'shell.itemLancamentos', route: '/contabilidade/lancamentos' },
      ]
    },
    {
      label: 'shell.groupNganSach',
      icon: 'account_balance_wallet',
      expanded: false,
      items: [
        { label: 'shell.itemOrcamento', route: '/contabilidade/orcamento', permTokens: ['ORC_SUBMIT', 'ORC_REVIEW', 'ORC_APPROVE'] },
        { label: 'shell.itemSuplementar', route: '/contabilidade/orcamentoSuplementar', permTokens: ['ORC_SUBMIT', 'ORC_REVIEW', 'ORC_APPROVE'] },
        { label: 'shell.itemSaldosAbertura', route: '/contabilidade/saldosAbertura', permTokens: ['ABE_SUBMIT', 'REC_SUBMIT'] },
      ]
    },
    {
      // Danh sách đầy đủ báo cáo cần ra, gộp 43 sheet gốc của Excel thành các
      // report có filter (vd: 1 report "Execução por Atividade" bao 22 sheet
      // Programa/SubPrograma/Atividade/CE_Regime* thay vì 22 màn riêng biệt).
      // Liệt kê hết ở đây trước — làm dần từng cái theo comingSoon. CE_OSS_Global
      // (ưu tiên #1, 2026-07-11) và Ciclo da Despesa (2026-07-12) đã có route
      // thật — mọi mục khác dừng ở comingSoon, kể cả 6 báo cáo tài chính mới
      // thêm bên dưới (phát hiện 2026-07-11 từ SCFSSTL2024_VF.xlsm/FRSSVF.xlsm —
      // 2 file khách cung cấp mới nạp, trước đó chưa đọc qua). Xem [[financial-statements-scope-gap]].
      label: 'shell.groupBaoCao',
      icon: 'summarize',
      expanded: false,
      items: [
        { label: 'shell.itemCeOssGlobal', route: '/contabilidade/relatorios/ceInssGlobal', permTokens: ['REPORT_VIEW'] },
        { label: 'shell.itemCicloDespesa', route: '/contabilidade/relatorios/cicloDespesa', permTokens: ['REPORT_VIEW'] },
        { label: 'shell.itemSinteseProgramas', comingSoon: true },
        { label: 'shell.itemClassificacaoFuncionalRelatorio', comingSoon: true },
        { label: 'shell.itemExecucaoAtividade', comingSoon: true },
        { label: 'shell.itemRegistoAd', route: '/contabilidade/relatorios/registoAd', permTokens: ['REPORT_VIEW'] },
        { label: 'shell.itemRegistoCabimentos', route: '/contabilidade/relatorios/registoCabimentos', permTokens: ['REPORT_VIEW'] },
        { label: 'shell.itemRegistoCompromissos', route: '/contabilidade/relatorios/registoCompromissos', permTokens: ['REPORT_VIEW'] },
        { label: 'shell.itemRegistoObrigacao', route: '/contabilidade/relatorios/registoObrigacoes', permTokens: ['REPORT_VIEW'] },
        { label: 'shell.itemReceitasGp', comingSoon: true },
        { label: 'shell.itemReceitasPac', comingSoon: true },
        { label: 'shell.itemControlo', comingSoon: true },
        { label: 'shell.itemExtratosBancarios', comingSoon: true },
        { label: 'shell.itemInterfaceLedger', comingSoon: true },
        { label: 'shell.itemMapaTransferenciasOe', comingSoon: true },
        { label: 'shell.itemBalanco', comingSoon: true },
        { label: 'shell.itemDemonstracaoResultados', comingSoon: true },
        { label: 'shell.itemFluxosCaixa', comingSoon: true },
        { label: 'shell.itemBalancete', comingSoon: true },
        { label: 'shell.itemExtratoContaCorrente', comingSoon: true },
      ]
    },
    {
      // Kế thừa nguyên bản (BRD §9) — dùng lại NGUYÊN component cũ (không sửa 1
      // dòng logic nào), chỉ route thêm dưới /contabilidade/* (xem
      // modulo-contabilidade-routing.module.ts) để nó hiện trong treebar/chrome
      // mới thay vì rơi về giao diện cũ. Component vẫn được declare duy nhất ở
      // AppModule như trước — ở đây chỉ thêm 1 route thứ 2 trỏ tới cùng class.
      // Đặt dưới Báo cáo, trên Master Data theo yêu cầu user (2026-07-11).
      // Backend cũ không có RequirePerm — mở cho mọi internal, không gắn permTokens.
      label: 'shell.groupDongGopBHXH',
      icon: 'badge',
      expanded: false,
      items: [
        { label: 'shell.itemTraCuscaEntidade', route: '/contabilidade/contribuicoes/entidade' },
        { label: 'shell.itemGuiaPagamentoValidacao', route: '/contabilidade/contribuicoes/guiaPagamento' },
        { label: 'shell.itemSituacaoContributiva', route: '/contabilidade/contribuicoes/situacaoContributiva' },
      ]
    },
    {
      label: 'shell.groupMasterData',
      icon: 'dns',
      expanded: false,
      items: [
        { label: 'shell.itemEstruturaProgramatica', route: '/contabilidade/estruturaProgramatica', permTokens: ['MASTERDATA_MANAGE'] },
        { label: 'shell.itemClassificacaoFuncional', route: '/contabilidade/classificacaoFuncional', permTokens: ['MASTERDATA_MANAGE'] },
        { label: 'shell.itemClassificacaoEconomica', route: '/contabilidade/classificacaoEconomica', permTokens: ['MASTERDATA_MANAGE'] },
        // Organization (InstitutionController) không có RequirePerm — chỉ xem.
        { label: 'shell.itemOrganization', route: '/contabilidade/organization' },
        // 2026-07-12 (user corrected): Plano de Contas + Mapeamento Rubricas KHÔNG
        // phải bảng/data mới — đã tồn tại từ trước là Codigoconta (cây tài khoản,
        // đã có InitialValue/IsCredit cho số dư đầu kỳ) + Agrupamentoconfig +
        // Relcodigocontaagrupamentoconfig (bảng quan hệ ánh xạ) — cả 2 đang được
        // dùng làm dropdown thật trong componente-despesa/receita/pop-up-executar-
        // pagamentos (mode cũ). Cả 2 nay đã có màn quản lý mode mới với CRUD riêng
        // (CodigoContaTreeController + AgrupamentoRubricaController, giới hạn 4
        // TipoConta Receita/Despesa/Neutro Receita/Neutro Despesa — không đụng tới
        // Actidade/Funcional, dữ liệu cũ trùng lặp Programa/Atividade và Classificação
        // Funcional). Xem [[financial-statements-scope-gap]].
        { label: 'shell.itemPlanoContas', route: '/contabilidade/planoContas', permTokens: ['MASTERDATA_MANAGE'] },
        { label: 'shell.itemMapeamentoRubricas', route: '/contabilidade/mapeamentoRubricas', permTokens: ['MASTERDATA_MANAGE'] },
        { label: 'shell.itemFornecedoresClientes', comingSoon: true },
      ]
    },
    {
      // Gộp lại 2026-07-11 (user phát hiện có 2 nhóm "Cấu hình hệ thống"/"Hệ
      // thống" trùng ý nhau) — trước đó "Cấu hình phòng ban/Email/API
      // Integration" nằm ở nhóm "Hệ thống" riêng, còn "Ngôn ngữ/Kỳ ngân sách/
      // Ngân hàng" nằm ở nhóm "Cấu hình hệ thống". Giờ chỉ còn 1 nhóm duy nhất.
      // "Quản lý User" cũng gộp vào đây 2026-07-12 (user yêu cầu không tách
      // riêng nữa) — không còn là nhóm top-level riêng.
      label: 'shell.groupCauHinh',
      icon: 'settings',
      expanded: false,
      items: [
        { label: 'shell.itemIdioma', route: '/contabilidade/settings/idioma', permTokens: ['MASTERDATA_MANAGE'] },
        { label: 'shell.itemKyNganSach', route: '/contabilidade/settings/kyNganSach', permTokens: ['MASTERDATA_MANAGE'] },
        { label: 'shell.itemBankAccount', route: '/contabilidade/settings/bankAccount', permTokens: ['MASTERDATA_MANAGE'] },
        { label: 'shell.itemGuiaPagamentoContaConfig', route: '/contabilidade/settings/guiaPagamentoContaConfig', permTokens: ['MASTERDATA_MANAGE'] },
        { label: 'shell.itemLiquidacaoContaConfig', route: '/contabilidade/settings/liquidacaoContaConfig', permTokens: ['MASTERDATA_MANAGE'] },
        { label: 'shell.itemAttachmentConfig', route: '/contabilidade/settings/attachmentConfig', permTokens: ['MASTERDATA_MANAGE'] },
        { label: 'shell.itemDepartamentos', route: '/contabilidade/sistema/departamentos', permTokens: ['MASTERDATA_MANAGE'] },
        { label: 'shell.itemUserPermission', route: '/contabilidade/userPermission', permTokens: ['USER_MANAGE'] },
        { label: 'shell.itemUserSync', route: '/contabilidade/userSync', permTokens: ['USER_MANAGE'] },
        { label: 'shell.itemEmailConfig', comingSoon: true },
        { label: 'shell.itemApiIntegration', comingSoon: true },
      ]
    },
    {
      // 2026-07-12 (user yêu cầu): "Meu Perfil" tách RIÊNG, không nằm dưới
      // "Cấu hình hệ thống" — khác với "Quản lý User" (admin quản lý user
      // KHÁC, RequirePerm), đây là tự phục vụ (self-service, mọi user đăng
      // nhập đều thấy, không cần quyền) nên đặt thành nhóm top-level độc lập.
      label: 'shell.groupMeuPerfil',
      icon: 'account_circle',
      expanded: false,
      items: [
        { label: 'shell.itemMeuPerfil', route: '/contabilidade/meuPerfil' },
      ]
    },
  ];

  // visibleGroups giờ là mảng tính 1 lần, tham chiếu ổn định qua mọi vòng CD
  // (không phải getter nữa) — nên toggle thẳng trên "group" nhận được là đủ,
  // không cần tìm lại object gốc trong this.groups như trước.
  public toggleGroup(group: TreebarGroup): void {
    group.expanded = !group.expanded;
  }
}
