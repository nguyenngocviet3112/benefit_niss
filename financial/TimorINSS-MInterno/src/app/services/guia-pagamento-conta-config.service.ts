import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ResponseBase } from '../response-models/utils-response';
import { CodigoContaOptionsResponse } from '../response-models/payment-response';

export interface GuiaPagamentoContaConfigModel {
  id: number;
  codigoContaCreditoPrivadoFk: number | null;
  codigoContaCreditoPrivadoDesignacao: string | null;
  codigoContaCreditoPublicoFk: number | null;
  codigoContaCreditoPublicoDesignacao: string | null;
}

export interface GuiaPagamentoContaConfigResponse extends ResponseBase {
  item: GuiaPagamentoContaConfigModel;
}

export interface SaveGuiaPagamentoContaConfigRequest {
  codigoContaCreditoPrivadoFk: number | null;
  codigoContaCreditoPublicoFk: number | null;
}

// Cấu hình tài khoản Có (Crédito) để tự sinh Lançamento khi 1 Guia Pagamento
// được đối chiếu ngân hàng thành công (xem guia-conciliacao). Bản cấu hình 1
// dòng (singleton), nhưng có 2 TÀI KHOẢN CRÉDITO khác nhau theo Setor
// (Público/Privado) của Entidade đóng góp — xác nhận qua sổ sách thật
// (2026-07-13, user chỉ ra có nhiều loại công ty khác nhau; trước đó tưởng
// nhầm là chỉ có 1 tài khoản chung). KHÔNG có Débito ở đây — Débito được
// backend tự tra động từ tài khoản ngân hàng thật đã nhận tiền.
@Injectable({
  providedIn: 'root'
})
export class GuiaPagamentoContaConfigService {

  constructor(private http: HttpClient) { }

  public getConfig(): Observable<GuiaPagamentoContaConfigResponse> {
    return this.http.get<GuiaPagamentoContaConfigResponse>(`${environment.apiUrl}/guiaPagamentoContaConfig/GetConfig`);
  }

  public save(request: SaveGuiaPagamentoContaConfigRequest): Observable<GuiaPagamentoContaConfigResponse> {
    return this.http.post<GuiaPagamentoContaConfigResponse>(`${environment.apiUrl}/guiaPagamentoContaConfig/Save`, request);
  }

  public getCodigoContaOptions(): Observable<CodigoContaOptionsResponse> {
    return this.http.get<CodigoContaOptionsResponse>(`${environment.apiUrl}/pagamento/GetCodigoContaOptions`);
  }
}
