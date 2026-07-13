import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ResponseBase } from '../response-models/utils-response';
import { CodigoContaOptionsResponse } from '../response-models/payment-response';

export interface LiquidacaoContaConfigModel {
  id: number;
  categoria: string;
  codigoContaFk: number | null;
  codigoContaDesignacao: string | null;
}

export interface LiquidacaoContaConfigListResponse extends ResponseBase {
  items: LiquidacaoContaConfigModel[];
}

export interface SaveLiquidacaoContaConfigRequest {
  categoria: string;
  codigoContaFk: number;
}

// Cấu hình tài khoản Phải trả (trung gian) dùng cho bút toán kép của chu trình
// Despesa/Pagamento — xem PaymentDataManager.Approve/Execute. 5 dòng cố định
// theo Obligation.BeneficiarioCategoria (Fornecedor/Beneficiário/Contribuinte(EE)/
// Pessoal/Outro), khớp với sổ sách thật (FRSSVF.xlsm "Lançamentos": Fornecedores
// c/c cho nhà cung cấp, Com o pessoal cho nhân viên, Outros credores cho khoản
// khác). Chỉ Update từng dòng, không thêm/xoá Categoria.
@Injectable({
  providedIn: 'root'
})
export class LiquidacaoContaConfigService {

  constructor(private http: HttpClient) { }

  public getAll(): Observable<LiquidacaoContaConfigListResponse> {
    return this.http.get<LiquidacaoContaConfigListResponse>(`${environment.apiUrl}/liquidacaoContaConfig/GetAll`);
  }

  public save(request: SaveLiquidacaoContaConfigRequest): Observable<LiquidacaoContaConfigListResponse> {
    return this.http.post<LiquidacaoContaConfigListResponse>(`${environment.apiUrl}/liquidacaoContaConfig/Save`, request);
  }

  public getCodigoContaOptions(): Observable<CodigoContaOptionsResponse> {
    return this.http.get<CodigoContaOptionsResponse>(`${environment.apiUrl}/pagamento/GetCodigoContaOptions`);
  }
}
