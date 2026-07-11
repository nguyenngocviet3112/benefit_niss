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
  // Thứ tự nhóm theo yêu cầu (2026-07-11): Chi tiêu → Thu → Ngân hàng → Cut-over
  // → Ngân sách → Quản lý User → Báo cáo → Master Data (cuối cùng).
  public groups: TreebarGroup[] = [
    {
      label: 'Chu trình chi tiêu',
      icon: 'sync_alt',
      expanded: true,
      items: [
        { label: 'AD / Cabimento', comingSoon: true },
        { label: 'Compromisso', comingSoon: true },
        { label: 'Obrigação', comingSoon: true },
        { label: 'Pagamento', comingSoon: true },
      ]
    },
    {
      label: 'Thu (Receita)',
      icon: 'payments',
      expanded: true,
      items: [
        { label: 'Receita', comingSoon: true },
      ]
    },
    {
      label: 'Ngân hàng',
      icon: 'account_balance',
      expanded: true,
      items: [
        { label: 'Conciliação de Movimentos', comingSoon: true },
      ]
    },
    {
      label: 'Cut-over',
      icon: 'flag',
      expanded: true,
      items: [
        { label: 'Saldos de Abertura', comingSoon: true },
      ]
    },
    {
      label: 'Ngân sách (Orçamento)',
      icon: 'account_balance_wallet',
      expanded: true,
      items: [
        { label: 'Orçamento', comingSoon: true },
        { label: 'Suplementar', comingSoon: true },
      ]
    },
    {
      label: 'Quản lý User',
      icon: 'group',
      expanded: true,
      items: [
        { label: 'Quản lý User', comingSoon: true },
      ]
    },
    {
      // Danh sách đầy đủ báo cáo cần ra, gộp 43 sheet gốc của Excel thành các
      // report có filter (vd: 1 report "Execução por Atividade" bao 22 sheet
      // Programa/SubPrograma/Atividade/CE_Regime* thay vì 22 màn riêng biệt).
      // Liệt kê hết ở đây trước — làm dần từng cái theo comingSoon.
      label: 'Báo cáo',
      icon: 'summarize',
      expanded: true,
      items: [
        { label: 'CE_INSS_Global (ưu tiên #1)', comingSoon: true },
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
      label: 'Master Data',
      icon: 'dns',
      expanded: true,
      items: [
        { label: 'Estrutura Programática', route: '/contabilidade/estruturaProgramatica' },
        { label: 'Classificação Funcional', route: '/contabilidade/classificacaoFuncional' },
        { label: 'Classificação Económica', route: '/contabilidade/classificacaoEconomica' },
        { label: 'Organization', route: '/contabilidade/organization' },
      ]
    },
  ];

  public toggleGroup(group: TreebarGroup): void {
    group.expanded = !group.expanded;
  }
}
