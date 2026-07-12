import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TokenStorageService } from '../../services/token-storage.service';

interface TreebarLeaf {
  label: string;
  route?: string;
  comingSoon?: boolean;
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

  constructor(
    private tokenStorage: TokenStorageService,
    private router: Router
  ) { }

  ngOnInit(): void {
    // Cùng cơ chế bảo vệ route như HomePageComponent (mode cũ): không có token
    // (kể cả sau khi logout) thì đẩy về /login — trước đây shell này thiếu
    // check này nên bấm logout ở mode mới không thoát ra được.
    if (!this.tokenStorage.getToken()) {
      this.router.navigate(['/login']);
    }
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
  public groups: TreebarGroup[] = [
    {
      label: 'Chu trình chi tiêu',
      icon: 'sync_alt',
      expanded: false,
      items: [
        { label: 'AD (Autorização de Despesa)', route: '/contabilidade/adCabimento' },
        { label: 'Cabimento (DIC)', route: '/contabilidade/cabimento' },
        { label: 'Compromisso', route: '/contabilidade/compromissoDespesa' },
        { label: 'Obrigação', route: '/contabilidade/obligation' },
        { label: 'Pagamento', route: '/contabilidade/payment' },
      ]
    },
    {
      label: 'Thu (Receita)',
      icon: 'payments',
      expanded: false,
      items: [
        { label: 'Receita PAC (Outras)', route: '/contabilidade/receita' },
        // Receita GP (contribuições) — 2026-07-11, user-stated: nên có trong tab
        // Receita, không chỉ Receita PAC. Dữ liệu GP đã được nhập/validado qua
        // module Contribuições cũ (ContaCorrente/GuiaPagamento) — tái dùng màn
        // "Guia Pagamento (Validação)" có sẵn (route dưới nhóm Đóng góp BHXH)
        // thay vì xây màn nhập mới, để tránh 2 nơi nhập trùng dữ liệu.
        { label: 'Receita GP (Contribuições)', route: '/contabilidade/contribuicoes/guiaPagamento' },
      ]
    },
    {
      label: 'Ngân hàng',
      icon: 'account_balance',
      expanded: false,
      items: [
        { label: 'Conciliação de Movimentos', route: '/contabilidade/conciliacaoMovimentos' },
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
      label: 'Contabilidade Geral',
      icon: 'menu_book',
      expanded: false,
      items: [
        { label: 'Registo de Lançamentos (Débito/Crédito)', route: '/contabilidade/lancamentos' },
      ]
    },
    {
      label: 'Ngân sách (Orçamento)',
      icon: 'account_balance_wallet',
      expanded: false,
      items: [
        { label: 'Orçamento', route: '/contabilidade/orcamento' },
        { label: 'Suplementar', route: '/contabilidade/orcamentoSuplementar' },
        { label: 'Saldos de Abertura (Cut-over)', route: '/contabilidade/saldosAbertura' },
      ]
    },
    {
      // Danh sách đầy đủ báo cáo cần ra, gộp 43 sheet gốc của Excel thành các
      // report có filter (vd: 1 report "Execução por Atividade" bao 22 sheet
      // Programa/SubPrograma/Atividade/CE_Regime* thay vì 22 màn riêng biệt).
      // Liệt kê hết ở đây trước — làm dần từng cái theo comingSoon. CHỈ
      // CE_OSS_Global có route thật (ưu tiên #1, 2026-07-11) — mọi mục khác
      // dừng ở comingSoon, kể cả 6 báo cáo tài chính mới thêm bên dưới (phát
      // hiện 2026-07-11 từ SCFSSTL2024_VF.xlsm/FRSSVF.xlsm — 2 file khách
      // cung cấp mới nạp, trước đó chưa đọc qua). Xem [[financial-statements-scope-gap]].
      label: 'Báo cáo',
      icon: 'summarize',
      expanded: false,
      items: [
        { label: 'CE_OSS_Global', route: '/contabilidade/relatorios/ceInssGlobal' },
        { label: 'Ciclo da Despesa', comingSoon: true },
        { label: 'Síntese Programas', comingSoon: true },
        { label: 'Classificação Funcional (relatório)', comingSoon: true },
        { label: 'Execução por Atividade / Programa / Regime (4 regimes: Contributivo/Não Contributivo/Administração/Capitalização FRSS)', comingSoon: true },
        { label: 'Registo AD', comingSoon: true },
        { label: 'Registo Cabimentos', comingSoon: true },
        { label: 'Registo Compromissos', comingSoon: true },
        { label: 'Registo Obrigação', comingSoon: true },
        { label: 'Receitas GP (Contribuições)', comingSoon: true },
        { label: 'Receitas PAC (Outras)', comingSoon: true },
        { label: 'Controlo', comingSoon: true },
        { label: 'Extratos Bancários (8 contas)', comingSoon: true },
        { label: 'INTERFACE (Ledger contábil)', comingSoon: true },
        { label: 'Mapa de Transferências OE', comingSoon: true },
        { label: 'Balanço (Bảng cân đối kế toán)', comingSoon: true },
        { label: 'Demonstração de Resultados (DR)', comingSoon: true },
        { label: 'Fluxos de Caixa (mensal/anual)', comingSoon: true },
        { label: 'Balancete (Bảng cân đối thử)', comingSoon: true },
        { label: 'Extrato de Conta Corrente', comingSoon: true },
      ]
    },
    {
      // Kế thừa nguyên bản (BRD §9) — dùng lại NGUYÊN component cũ (không sửa 1
      // dòng logic nào), chỉ route thêm dưới /contabilidade/* (xem
      // modulo-contabilidade-routing.module.ts) để nó hiện trong treebar/chrome
      // mới thay vì rơi về giao diện cũ. Component vẫn được declare duy nhất ở
      // AppModule như trước — ở đây chỉ thêm 1 route thứ 2 trỏ tới cùng class.
      // Đặt dưới Báo cáo, trên Master Data theo yêu cầu user (2026-07-11).
      label: 'Đóng góp BHXH',
      icon: 'badge',
      expanded: false,
      items: [
        { label: 'Tra cứu Entidade (theo NISS)', route: '/contabilidade/contribuicoes/entidade' },
        { label: 'Guia Pagamento (Validação)', route: '/contabilidade/contribuicoes/guiaPagamento' },
        { label: 'Situação Contributiva', route: '/contabilidade/contribuicoes/situacaoContributiva' },
      ]
    },
    {
      label: 'Master Data',
      icon: 'dns',
      expanded: false,
      items: [
        { label: 'Estrutura Programática', route: '/contabilidade/estruturaProgramatica' },
        { label: 'Classificação Funcional', route: '/contabilidade/classificacaoFuncional' },
        { label: 'Classificação Económica', route: '/contabilidade/classificacaoEconomica' },
        { label: 'Organization', route: '/contabilidade/organization' },
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
        { label: 'Plano de Contas (Codigoconta)', route: '/contabilidade/planoContas' },
        { label: 'Mapeamento Rubricas (Agrupamentoconfig)', route: '/contabilidade/mapeamentoRubricas' },
        { label: 'Fornecedores / Clientes', comingSoon: true },
      ]
    },
    {
      // Gộp lại 2026-07-11 (user phát hiện có 2 nhóm "Cấu hình hệ thống"/"Hệ
      // thống" trùng ý nhau) — trước đó "Cấu hình phòng ban/Email/API
      // Integration" nằm ở nhóm "Hệ thống" riêng, còn "Ngôn ngữ/Kỳ ngân sách/
      // Ngân hàng" nằm ở nhóm "Cấu hình hệ thống". Giờ chỉ còn 1 nhóm duy nhất.
      // "Quản lý User" cũng gộp vào đây 2026-07-12 (user yêu cầu không tách
      // riêng nữa) — không còn là nhóm top-level riêng. "Meu Perfil" (tự
      // phục vụ — mọi user, không cần RequirePerm) cũng thêm vào đây cùng
      // ngày, đặt đầu danh sách vì dùng thường xuyên hơn các mục admin còn lại.
      label: 'Cấu hình hệ thống',
      icon: 'settings',
      expanded: false,
      items: [
        { label: 'Meu Perfil (Hồ sơ cá nhân)', route: '/contabilidade/meuPerfil' },
        { label: 'Ngôn ngữ (Idioma)', route: '/contabilidade/settings/idioma' },
        { label: 'Kỳ ngân sách (Orçamento Config)', route: '/contabilidade/settings/kyNganSach' },
        { label: 'Ngân hàng (Contas Bancárias)', route: '/contabilidade/settings/bankAccount' },
        { label: 'Cấu hình phòng ban', route: '/contabilidade/sistema/departamentos' },
        { label: 'Quản lý User & Phân quyền', route: '/contabilidade/userPermission' },
        { label: 'Cấu hình Email', comingSoon: true },
        { label: 'API Integration (Benefit)', comingSoon: true },
      ]
    },
  ];

  public toggleGroup(group: TreebarGroup): void {
    group.expanded = !group.expanded;
  }
}
