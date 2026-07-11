import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { OrcamentoConfigService } from '../../services/orcamento-config.service';
import { OrcamentoConfigDataContract } from '../../response-models/orcamento-config-response';

// Dropdown "Kỳ ngân sách (Ano)" dùng chung cho các màn Master Data
// (Estrutura Programática, Classificação Económica, ...) — lấy danh sách kỳ
// ngân sách thật từ Cấu hình hệ thống (OrcamentoConfig) thay vì cho gõ số tay.
@Component({
  selector: 'app-orcamento-config-select',
  templateUrl: './orcamento-config-select.component.html',
  styleUrls: ['./orcamento-config-select.component.css']
})
export class OrcamentoConfigSelectComponent implements OnInit {

  @Input() value: number | null = null;
  @Output() valueChange = new EventEmitter<number>();

  @Input() label = 'Kỳ ngân sách (Ano)';

  public options: OrcamentoConfigDataContract[] = [];

  constructor(private orcamentoConfigService: OrcamentoConfigService) { }

  ngOnInit(): void {
    this.orcamentoConfigService.getAll().subscribe(response => {
      this.options = (response.items ?? [])
        .filter(o => o.indActivo)
        .sort((a, b) => b.ano - a.ano);

      // Nếu chưa có giá trị chọn (hoặc giá trị cũ không còn active), mặc định
      // chọn kỳ mới nhất để màn hình không bị trống dữ liệu.
      if (this.options.length > 0 && !this.options.some(o => o.id === this.value)) {
        this.select(this.options[0].id);
      }
    });
  }

  public select(id: number): void {
    this.value = id;
    this.valueChange.emit(id);
  }
}
