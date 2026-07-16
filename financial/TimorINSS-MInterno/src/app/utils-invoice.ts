import { DatePipe } from '@angular/common';
import { TranslateService } from '@ngx-translate/core';
import jsPDF from 'jspdf';
import * as QRCode from 'qrcode';
import { environment } from 'src/environments/environment';
import { formatDate, formatDatePT } from './utils';

// Port 1:1 của contribution/ModuloContribuicoes/src/app/guia-pagamento/guia-pagamento.component.ts's
// gerarPDF() — đây là design "SOCIAL CONTRIBUTIONS PAYMENT GUIDE" chính thức, giữ y hệt layout/toạ độ
// để 2 app luôn ra cùng 1 tài liệu cho cùng 1 Guia. Sửa ở app này thì cũng phải sửa bên contribution.
export function gerarInvoicePDF(element: any, translate: TranslateService, datepipe: DatePipe): void {
  var pdf = new jsPDF();
  // INSS logo
  pdf.addImage(environment.ssIcon, 'JPEG', 90, 5, 25, 20);

  const pageWidth = 210; // Chiều rộng trang A4 tính bằng mm

  //title
  pdf.setFontSize(13);
  pdf.setTextColor(80);
  pdf.text(translate.instant('general.invoiceTitleDoc'), 60, 35);
  pdf.setFontSize(11);
  pdf.setTextColor(10);
  //Número do Documento
  pdf.text(translate.instant('general.invoicePaymentRef') + ': ' + element.paymentRef, 20, 45);
  pdf.text(translate.instant('general.period') + ': ' + formatDate(datepipe, element.mesAno), 160, 45);
  pdf.text(translate.instant('general.invoiceEm'), 20, 52);
  pdf.text(translate.instant('general.invoiceName') + ': ' + element.userName, 20, 59);
  pdf.text(translate.instant('general.invoiceDate') + ': ' + formatDatePT(datepipe, element.dataCriacao), 160, 59);
  pdf.text(translate.instant('general.invoiceNISS') + ': ' + element.niss, 20, 66);
  pdf.text(translate.instant('general.invoiceTIN') + ': ' + element.tin, 20, 73);
  pdf.text(translate.instant('general.invoicePM') + ':', 20, 80);
  pdf.addImage(environment.checkBoxIcon, 'JPEG', 20, 83, 5, 5);
  pdf.text(translate.instant('general.invoiceCash'), 30, 87);
  pdf.addImage(environment.checkBoxIcon, 'JPEG', 20, 90, 5, 5);
  pdf.text(translate.instant('general.invoiceBT'), 30, 94);

  pdf.text(translate.instant('general.invoiceIP'), 20, 101);
  pdf.text(translate.instant('general.invoiceBank') + ':', 20, 108);
  pdf.text(translate.instant('general.invoiceNOT'), 20, 115);
  pdf.text(translate.instant('general.invoiceCCATP'), 20, 122);

  var text1 = translate.instant('general.invoiceCFT') + ':';
  var text2 = (element.total).toFixed(2);
  var text3 = translate.instant('general.invoiceUSD');
  // Tính toán chiều rộng của văn bản
  var textWidth1 = pdf.getTextWidth(text1);
  var textWidth2 = pdf.getTextWidth(text2);
  var textWidth3 = pdf.getTextWidth(text3);

  // Tính toán vị trí căn lề phải
  var rightAlignX1 = pageWidth - textWidth1 - textWidth2 - textWidth3 - 70; // 20 là khoảng cách lề trái

  var rightAlignX2 = pageWidth - textWidth2 - textWidth3 - 42; // 20 là khoảng cách lề trái
  var rightAlignX3 = pageWidth - textWidth3 - 40; // 20 là khoảng cách lề trái

  pdf.text(translate.instant('general.invoiceCFT') + ':', rightAlignX1, 129);
  pdf.text((element.valor).toFixed(2), rightAlignX2, 129);
  pdf.text(translate.instant('general.invoiceUSD'), rightAlignX3, 129);

  text1 = translate.instant('general.invoiceEEC') + ':';
  text2 = (element.valor - element.valorTrabalhador).toFixed(2);

  textWidth1 = pdf.getTextWidth(text1);
  textWidth2 = pdf.getTextWidth(text2);
  rightAlignX1 = pageWidth - textWidth1 - textWidth2 - textWidth3 - 70;
  rightAlignX2 = pageWidth - textWidth2 - textWidth3 - 42;
  rightAlignX3 = pageWidth - textWidth3 - 40;

  pdf.text(translate.instant('general.invoiceEEC') + ':', rightAlignX1, 136);
  pdf.text((element.valor - element.valorTrabalhador).toFixed(2), rightAlignX2, 136);
  pdf.text(translate.instant('general.invoiceUSD'), rightAlignX3, 136);


  text1 = translate.instant('general.invoiceTCO') + ':';
  text2 = element.valorTrabalhador.toFixed(2);

  textWidth1 = pdf.getTextWidth(text1);
  textWidth2 = pdf.getTextWidth(text2);
  rightAlignX1 = pageWidth - textWidth1 - textWidth2 - textWidth3 - 70;
  rightAlignX2 = pageWidth - textWidth2 - textWidth3 - 42;
  rightAlignX3 = pageWidth - textWidth3 - 40;

  pdf.text(translate.instant('general.invoiceTCO') + ':', rightAlignX1, 143);
  pdf.text(element.valorTrabalhador.toFixed(2), rightAlignX2, 143);
  pdf.text(translate.instant('general.invoiceUSD'), rightAlignX3, 143);


  text1 = translate.instant('general.invoiceOPA') + ':';
  text2 = '0';

  textWidth1 = pdf.getTextWidth(text1);
  textWidth2 = pdf.getTextWidth(text2);
  rightAlignX1 = pageWidth - textWidth1 - textWidth2 - textWidth3 - 70;
  rightAlignX2 = pageWidth - textWidth2 - textWidth3 - 42;
  rightAlignX3 = pageWidth - textWidth3 - 40;

  pdf.text(translate.instant('general.invoiceOPA') + ':', rightAlignX1 - 7.5, 150);
  pdf.text('0', rightAlignX2, 150);
  pdf.text(translate.instant('general.invoiceUSD'), rightAlignX3, 150);

  text1 = translate.instant('general.invoiceINCLUDING');
  textWidth1 = pdf.getTextWidth(text1);
  rightAlignX1 = pageWidth - textWidth1 - textWidth2 - textWidth3 - 70;
  pdf.text(translate.instant('general.invoiceINCLUDING'), rightAlignX1 - 7.5, 157);

  text1 = translate.instant('general.invoiceLPI') + ':';
  text2 = '0';

  textWidth1 = pdf.getTextWidth(text1);
  textWidth2 = pdf.getTextWidth(text2);
  rightAlignX1 = pageWidth - textWidth1 - textWidth2 - textWidth3 - 70;
  rightAlignX2 = pageWidth - textWidth2 - textWidth3 - 42;
  rightAlignX3 = pageWidth - textWidth3 - 40;

  pdf.text(translate.instant('general.invoiceLPI') + ':', rightAlignX1 - 7.5, 164);
  pdf.text('0', rightAlignX2, 164);
  pdf.text(translate.instant('general.invoiceUSD'), rightAlignX3, 164);


  text1 = translate.instant('general.invoiceFines') + ':';
  text2 = (element.juros).toFixed(2);

  textWidth1 = pdf.getTextWidth(text1);
  textWidth2 = pdf.getTextWidth(text2);
  rightAlignX1 = pageWidth - textWidth1 - textWidth2 - textWidth3 - 70;
  rightAlignX2 = pageWidth - textWidth2 - textWidth3 - 42;
  rightAlignX3 = pageWidth - textWidth3 - 40;

  pdf.text(translate.instant('general.invoiceFines') + ':', rightAlignX1 - 7.5, 171);
  pdf.text((element.juros).toFixed(2), rightAlignX2, 171);
  pdf.text(translate.instant('general.invoiceUSD'), rightAlignX3, 171);


  text1 = translate.instant('general.invoiceOthers') + ':';
  text2 = '0';

  textWidth1 = pdf.getTextWidth(text1);
  textWidth2 = pdf.getTextWidth(text2);
  rightAlignX1 = pageWidth - textWidth1 - textWidth2 - textWidth3 - 70;
  rightAlignX2 = pageWidth - textWidth2 - textWidth3 - 42;
  rightAlignX3 = pageWidth - textWidth3 - 40;

  pdf.text(translate.instant('general.invoiceOthers') + ':', rightAlignX1 - 7.5, 178);
  pdf.text('0', rightAlignX2, 178);
  pdf.text(translate.instant('general.invoiceUSD'), rightAlignX3, 178);

  text1 = translate.instant('general.invoiceTotal') + ':';
  text2 = (element.total).toFixed(2);

  textWidth1 = pdf.getTextWidth(text1);
  textWidth2 = pdf.getTextWidth(text2);
  rightAlignX1 = pageWidth - textWidth1 - textWidth2 - textWidth3 - 70;
  rightAlignX2 = pageWidth - textWidth2 - textWidth3 - 42;
  rightAlignX3 = pageWidth - textWidth3 - 40;

  pdf.text(translate.instant('general.invoiceTotal') + ':', rightAlignX1, 185);
  pdf.text((element.total).toFixed(2), rightAlignX2, 185);
  pdf.text(translate.instant('general.invoiceUSD'), rightAlignX3, 185);


  text1 = translate.instant('general.invoiceROP') + ':';
  text2 = '0';

  textWidth1 = pdf.getTextWidth(text1);
  textWidth2 = pdf.getTextWidth(text2);
  rightAlignX1 = pageWidth - textWidth1 - textWidth2 - textWidth3 - 70;
  rightAlignX2 = pageWidth - textWidth2 - textWidth3 - 42;
  rightAlignX3 = pageWidth - textWidth3 - 40;

  pdf.text(translate.instant('general.invoiceROP') + ':', rightAlignX1 - 7.5, 192);
  pdf.text('0', rightAlignX2, 192);
  pdf.text(translate.instant('general.invoiceUSD'), rightAlignX3, 192);

  text1 = translate.instant('general.invoiceCCON') + ':';
  text2 = '0';

  textWidth1 = pdf.getTextWidth(text1);
  textWidth2 = pdf.getTextWidth(text2);
  rightAlignX1 = pageWidth - textWidth1 - textWidth2 - textWidth3 - 70;
  rightAlignX2 = pageWidth - textWidth2 - textWidth3 - 42;
  rightAlignX3 = pageWidth - textWidth3 - 40;

  pdf.text(translate.instant('general.invoiceCCON') + ':', rightAlignX1 - 7.5, 199);
  pdf.text('0', rightAlignX2, 199);
  pdf.text(translate.instant('general.invoiceUSD'), rightAlignX3, 199);


  text1 = translate.instant('general.invoiceTTA') + ':';
  text2 = (element.total).toFixed(2);

  textWidth1 = pdf.getTextWidth(text1);
  textWidth2 = pdf.getTextWidth(text2);
  rightAlignX1 = pageWidth - textWidth1 - textWidth2 - textWidth3 - 70;
  rightAlignX2 = pageWidth - textWidth2 - textWidth3 - 42;
  rightAlignX3 = pageWidth - textWidth3 - 40;

  pdf.text(translate.instant('general.invoiceTTA') + ':', rightAlignX1 - 7.5, 206);
  pdf.text((element.total).toFixed(2), rightAlignX2, 206);
  pdf.text(translate.instant('general.invoiceUSD'), rightAlignX3, 206);

  text1 = translate.instant('general.invoiceAPEE') + ':';
  text2 = (element.valorEntidade).toFixed(2);

  textWidth1 = pdf.getTextWidth(text1);
  textWidth2 = pdf.getTextWidth(text2);
  rightAlignX1 = pageWidth - textWidth1 - textWidth2 - textWidth3 - 70;
  rightAlignX2 = pageWidth - textWidth2 - textWidth3 - 42;
  rightAlignX3 = pageWidth - textWidth3 - 40;

  pdf.text(translate.instant('general.invoiceAPEE') + ':', rightAlignX1, 213);
  pdf.text(element.valorEntidade.toFixed(2), rightAlignX2, 213);
  pdf.text(translate.instant('general.invoiceUSD'), rightAlignX3, 213);


  text1 = translate.instant('general.invoiceATCO') + ':';
  text2 = (element.valorTrabalhador).toFixed(2);

  textWidth1 = pdf.getTextWidth(text1);
  textWidth2 = pdf.getTextWidth(text2);
  rightAlignX1 = pageWidth - textWidth1 - textWidth2 - textWidth3 - 70;
  rightAlignX2 = pageWidth - textWidth2 - textWidth3 - 42;
  rightAlignX3 = pageWidth - textWidth3 - 40;

  pdf.text(translate.instant('general.invoiceATCO') + ':', rightAlignX1, 220);
  pdf.text(element.valorTrabalhador.toFixed(2), rightAlignX2, 220);
  pdf.text(translate.instant('general.invoiceUSD'), rightAlignX3, 220);

  pdf.text(translate.instant('general.invoiceSSS'), 83, 230);

  pdf.text(formatDatePT(datepipe, element.dataCriacao), 25, 265);
  pdf.text(translate.instant('general.invoiceDate'), 30, 270);

  const SIGNATURE_CHANGE_DATE = new Date('2026-01-01');
  const invoiceDate = new Date(element.mesAno);
  const signatureIcon = invoiceDate >= SIGNATURE_CHANGE_DATE ? (environment as any).signatureNew2026 : environment.signatureIcon;

  pdf.addImage(signatureIcon, 'JPEG', 165, 245, 25, 20);
  pdf.text(translate.instant('general.invoiceSAS'), 160, 270);

  QRCode.toDataURL(element.qrInvoice)
    .then((url: string) => {
      pdf.addImage(url, 'JPEG', 95, 275, 20, 20);
    })
    .catch((err: any) => {
      console.error(err);
    });

  pdf.save(formatDatePT(datepipe, new Date()) + '_invoice.pdf');

  setTimeout(() => {
    const blob = pdf.output('blob');
    const url = URL.createObjectURL(blob);
    window.open(url, '_blank');
  }, 1000);
}
