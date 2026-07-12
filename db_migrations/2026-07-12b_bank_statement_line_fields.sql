-- Conciliação de Movimentos — thêm 2 field theo yêu cầu user (2026-07-12):
-- Data Transação (ngày thực hiện giao dịch, khác Data Valor — ngân hàng
-- thường phân biệt data valor/ngày ghi nhận giá trị vs. data transação/
-- ngày giao dịch thực tế diễn ra) và Código Transação Bancária (mã tham
-- chiếu giao dịch do ngân hàng cấp, dùng để tra soát khi có sai lệch).

ALTER TABLE [dbo].[BankStatementLine] ADD [DataTransacao] [datetime] NULL
GO
ALTER TABLE [dbo].[BankStatementLine] ADD [CodigoTransacaoBancaria] [varchar](100) NULL
GO
