import { Component, OnInit } from '@angular/core';
import { ChartOptions, ChartType } from 'chart.js';
import { Label } from 'ng2-charts';
import { TranslateService } from '@ngx-translate/core';
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

  // Biểu đồ tiêu thụ ngân sách (Đã chi vs Còn lại) — doughnut, dùng chung API
  // ng2-charts v2 (ChartOptions/ChartType/Label) như pop-up-resumo-declaracao,
  // nhưng màu cố định (không random) vì chỉ 2 lát có ý nghĩa ngữ nghĩa rõ ràng.
  public chartType: ChartType = 'doughnut';

  public consumoChartLabels: Label[] = [];
  public consumoChartData: number[] = [];
  public consumoChartColors: any[] = [{ backgroundColor: ['#2A81CC', '#e0e0e0'], borderWidth: 0 }];
  public consumoChartOptions: ChartOptions = this.buildDoughnutOptions(true);

  // Biểu đồ Guia Pagamento đã validate/tổng — cùng màu với dash-dot-approved/
  // dash-dot-progress đã dùng cho card AD/Cabimento/... ở trên, cho nhất quán.
  public gpChartLabels: Label[] = [];
  public gpChartData: number[] = [];
  public gpChartColors: any[] = [{ backgroundColor: ['#2e7d32', '#f57f17'], borderWidth: 0 }];
  public gpChartOptions: ChartOptions = this.buildDoughnutOptions(false);

  constructor(
    private dashboardService: DashboardService,
    private permissionService: PermissionService,
    private translate: TranslateService
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
        this.buildCharts(response.summary);
      },
      () => {
        this.loading = false;
        this.hasAccess = false;
      }
    );
  }

  private buildCharts(summary: DashboardSummaryDataContract): void {
    const consumido = summary.despesaExecutada ?? 0;
    const restante = Math.max((summary.orcamentoTotal ?? 0) - consumido, 0);
    this.consumoChartLabels = [
      this.translate.instant('dashboard.consumidoLabel'),
      this.translate.instant('dashboard.restanteLabel')
    ];
    this.consumoChartData = [consumido, restante];

    const validado = summary.guiaPagamentoValidado ?? 0;
    const pendente = Math.max((summary.guiaPagamentoTotal ?? 0) - validado, 0);
    this.gpChartLabels = [
      this.translate.instant('dashboard.gpValidadoLabel'),
      this.translate.instant('dashboard.gpPendenteLabel')
    ];
    this.gpChartData = [validado, pendente];
  }

  private buildDoughnutOptions(currency: boolean): ChartOptions {
    return {
      responsive: true,
      maintainAspectRatio: false,
      legend: { position: 'bottom' },
      tooltips: {
        enabled: true,
        mode: 'single',
        callbacks: {
          label: (tooltipItem: Chart.ChartTooltipItem, data: Chart.ChartData) => {
            if (!data.datasets || !data.datasets[0] || !data.datasets[0].data || tooltipItem.index === undefined || !data.labels) { return ''; }
            const value = data.datasets[0].data[tooltipItem.index] as number;
            const label = data.labels[tooltipItem.index];
            return `${label}: ${currency ? '$ ' + value.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) : value}`;
          }
        }
      }
    };
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
