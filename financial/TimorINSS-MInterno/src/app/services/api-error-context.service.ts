import { Injectable } from "@angular/core";
import { HttpErrorResponse } from "@angular/common/http";

// [PT] Guarda o detalhe tecnico do ultimo erro HTTP para que o dialogo de erro o possa
// mostrar por baixo da mensagem generica. Sem isto, "Something went wrong" e tudo o que o
// utilizador ve e tudo o que nos chega num screenshot -- foi preciso abrir o DevTools ou os
// logs do servidor para descobrir que por tras estava um 413 do nginx ou um erro de GDI+.
// [VI] Luu chi tiet ky thuat cua loi HTTP gan nhat de dialog loi hien duoc ngay duoi cau
// thong bao chung. Khong co no thi "Something went wrong" la tat ca nhung gi nguoi dung thay
// va tat ca nhung gi ta nhan duoc qua anh chup -- phai mo DevTools hoac log server moi biet
// dang sau la 413 cua nginx hay loi GDI+.
@Injectable({ providedIn: "root" })
export class ApiErrorContextService {

  private detalhe: string | null = null;

  public registar(err: HttpErrorResponse): void {
    const linhas: string[] = [];

    const endpoint = this.nomeLegivelDoEndpoint(err.url || "");
    linhas.push(`HTTP ${err.status}${err.statusText ? " " + err.statusText : ""}${endpoint ? " - " + endpoint : ""}`);

    const corpo: any = err.error;

    // Formato normal da API: { errors: [ { errorCode, errorMessage } ] }
    if (corpo && Array.isArray(corpo.errors)) {
      corpo.errors.forEach((e: any) => {
        const msg = e?.errorMessage;
        if (msg && msg !== e?.errorCode) { linhas.push(String(msg)); }
      });
    }
    // Resposta que nao vem da aplicacao -- p.ex. a pagina HTML de erro do nginx num 413.
    else if (typeof corpo === "string" && corpo.trim()) {
      const texto = corpo.replace(/<[^>]*>/g, " ").replace(/\s+/g, " ").trim();
      if (texto) { linhas.push(texto.substring(0, 300)); }
    }

    this.detalhe = linhas.join(" | ");
  }


  // [PT] Os caminhos da API vao em Base64 URL-safe (ver encodePath no HttpInterceptorService),
  // pelo que o ultimo segmento do URL nao diz nada a ninguem. Descodifica-se para o nome real
  // do servico; se nao for Base64 valido, mostra-se tal e qual.
  // [VI] Duong dan API duoc ma hoa Base64 URL-safe (xem encodePath trong HttpInterceptorService)
  // nen doan cuoi cua URL doc len khong hieu gi. Giai ma ve ten service that; neu khong phai
  // Base64 hop le thi giu nguyen.
  private nomeLegivelDoEndpoint(url: string): string {
    const bruto = url.split("?")[0].split("/").slice(-1)[0];
    if (!bruto) { return ""; }
    try {
      const normalizado = bruto.replace(/-/g, "+").replace(/_/g, "/");
      const descodificado = atob(normalizado + "=".repeat((4 - (normalizado.length % 4)) % 4));
      return /^[\x20-\x7e]+$/.test(descodificado) ? descodificado : bruto;
    } catch {
      return bruto;
    }
  }

  // Le e limpa: o detalhe pertence a um so dialogo, nao deve reaparecer no seguinte.
  public consumir(): string | null {
    const d = this.detalhe;
    this.detalhe = null;
    return d;
  }
}
