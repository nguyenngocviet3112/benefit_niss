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
  // Thứ tự nhóm theo yêu cầu (2026-07-11): Chi tiêu → Thu → Ngân hàng
  // → Ngân sách (gồm cả Cut-over/Saldos de Abertura) → Báo cáo → Đóng góp BHXH
  // → Master Data → Quản lý User → Cấu hình hệ thống (cuối cùng). Cut-over gộp
  // chung vào Ngân sách — không còn là nhóm riêng. Đóng góp BHXH đặt dưới Báo
  // cáo, trên Master Data (theo yêu cầu user). Tất cả nhóm mặc định collapse
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
        { label: 'Receita', comingSoon: true },
      ]
    },
    {
      label: 'Ngân hàng',
      icon: 'account_balance',
      expanded: false,
      items: [
        { label: 'Conciliação de Movimentos', comingSoon: true },
      ]
    },
    {
      label: 'Ngân sách (Orçamento)',
      icon: 'account_balance_wallet',
      expanded: false,
      items: [
        { label: 'Orçamento', route: '/contabilidade/orcamento' },
        { label: 'Suplementar', comingSoon: true },
        { label: 'Saldos de Abertura (Cut-over)', comingSoon: true },
      ]
    },
    {
      // Danh sách đầy đủ báo cáo cần ra, gộp 43 sheet gốc của Excel thành các
      // report có filter (vd: 1 report "Execução por Atividade" bao 22 sheet
      // Programa/SubPrograma/Atividade/CE_Regime* thay vì 22 màn riêng biệt).
      // Liệt kê hết ở đây trước — làm dần từng cái theo comingSoon.
      label: 'Báo cáo',
      icon: 'summarize',
      expanded: false,
      items: [
        { label: 'CE_OSS_Global (ưu tiên #1)', route: '/contabilidade/relatorios/ceInssGlobal' },
        { label: 'Ciclo da Despesa', comingSoon: true },
        { label: 'Síntese Programas', comingSoon: true },
        { label: 'Classificação Funcional (relatório)', comingSoon: true },
        { label: 'Execução por Atividade / Programa / Regime', comingSoon: true },
        { label: 'Registo AD', comingSoon: true },
        { label: 'Registo Cabimentos', comingSoon: true },
        { label: 'Registo Compromissos', comingSoon: true },
        { label: 'Registo Obrigação', comingSoon: true },
        { label: 'Receitas GP (Contribuições)', comingSoon: true },
        { label: 'Receitas PAC (Outras)', comingSoon: true },
        { label: 'Controlo', comingSoon: true },
        { label: 'Extratos Bancários (8 contas)', comingSoon: true },
        { label: 'INTERFACE (Ledger contábil)', comingSoon: true },
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
      ]
    },
    {
      label: 'Quản lý User',
      icon: 'group',
      expanded: false,
      items: [
        { label: 'Quản lý User & Phân quyền', route: '/contabilidade/userPermission' },
      ]
    },
    {
      label: 'Cấu hình hệ thống',
      icon: 'settings',
      expanded: false,
      items: [
        { label: 'Ngôn ngữ (Idioma)', route: '/contabilidade/settings/idioma' },
        { label: 'Kỳ ngân sách (Orçamento Config)', route: '/contabilidade/settings/kyNganSach' },
      ]
    },
  ];

  public toggleGroup(group: TreebarGroup): void {
    group.expanded = !group.expanded;
  }
}
