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
  public groups: TreebarGroup[] = [
    {
      label: 'Master Data',
      icon: 'dns',
      expanded: true,
      items: [
        { label: 'Estrutura Programática', route: '/contabilidade/estruturaProgramatica' },
        { label: 'Classificação Funcional', route: '/contabilidade/classificacaoFuncional' },
        { label: 'Organization', route: '/contabilidade/organization' },
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
      label: 'Quản lý User',
      icon: 'group',
      expanded: true,
      items: [
        { label: 'Quản lý User', comingSoon: true },
      ]
    },
    {
      label: 'Báo cáo',
      icon: 'summarize',
      expanded: true,
      items: [
        { label: 'Ciclo da Despesa', comingSoon: true },
      ]
    },
  ];

  public toggleGroup(group: TreebarGroup): void {
    group.expanded = !group.expanded;
  }
}
