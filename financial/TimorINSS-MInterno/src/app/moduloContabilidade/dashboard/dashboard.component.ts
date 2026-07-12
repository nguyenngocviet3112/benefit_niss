import { Component, OnInit } from '@angular/core';
import { DashboardService } from '../../services/dashboard.service';
import { PermissionService } from '../../services/permission.service';
import { DashboardSummaryDataContract, ProcessSummaryDataContract } from '../../response-models/dashboard-response';

interface ProcessTile {
  labelKey: string;
  icon: string;
  data: ProcessSummaryDataContract;
}

// Màn mặc định khi đăng nhập mode mới. Kiểm tra quyền NGAY TRÊN FRONTEND (giải
// mã JWT, không gọi API) trước — user chưa được gán DASHBOARD_VIEW thì không
// gọi GetSummary luôn, hiện màn trống (2026-07-12, theo yêu cầu user). Backend
// vẫn là lớp chặn thật qua [RequirePerm] — nếu goi lỡ mà bị 403 (vd token cũ
// còn hạn nhưng quyền vừa bị thu hồi) thì cũng coi như không có quyền.
@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  public hasAccess = false;
  public loading = false;
  public summary: DashboardSummaryDataContract | null = null;

  constructor(
    private dashboardService: DashboardService,
    private permissionService: PermissionService
  ) { }

  ngOnInit(): void {
    this.hasAccess = this.permissionService.hasPerm('DASHBOARD_VIEW');
    if (!this.hasAccess) {
      return;
    }

    this.loading = true;
    this.dashboardService.getSummary().subscribe(
      response => {
        this.loading = false;
        if (response.errors && response.errors.length > 0) {
          this.hasAccess = false;
          return;
        }
        this.summary = response.summary;
      },
      () => {
        this.loading = false;
        this.hasAccess = false;
      }
    );
  }

  public get processTiles(): ProcessTile[] {
    if (!this.summary) {
      return [];
    }
    return [
      { labelKey: 'dashboard.processAd', icon: 'description', data: this.summary.ad },
      { labelKey: 'dashboard.processCabimento', icon: 'fact_check', data: this.summary.cabimento },
      { labelKey: 'dashboard.processCompromisso', icon: 'handshake', data: this.summary.compromisso },
      { labelKey: 'dashboard.processObrigacao', icon: 'gavel', data: this.summary.obrigacao },
      { labelKey: 'dashboard.processPagamento', icon: 'payments', data: this.summary.pagamentoAutorizacao },
    ];
  }

  public percentApproved(data: ProcessSummaryDataContract): number {
    if (!data || data.total === 0) {
      return 0;
    }
    return Math.round((data.aprovado / data.total) * 100);
  }

  public percentOf(part: number, total: number): number {
    if (!total) {
      return 0;
    }
    return Math.round((part / total) * 100);
  }
}
