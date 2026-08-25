-- Dua DB local len ngang schema production (3. Customer Send/Inss db prod schema.sql, 16/07/2026).
-- CHI noi rong cot khi local hep hon prod. Khong bao gio thu hep -- thu hep se cat mat du lieu.
USE TimorINSSModuloContribuicoes_ProdDev;
GO
ALTER TABLE [dbo].[AGRUPAMENTOCONFIG] ALTER COLUMN [codigo] varchar(50) NULL;   -- local 2 -> prod 50
ALTER TABLE [dbo].[AGRUPAMENTOCONFIG] ALTER COLUMN [designacao] varchar(200) NULL;   -- local 75 -> prod 200
ALTER TABLE [dbo].[CONTACTO] ALTER COLUMN [email] varchar(100) NULL;   -- local 50 -> prod 100
ALTER TABLE [dbo].[CONTACTO] ALTER COLUMN [telemovel] varchar(50) NULL;   -- local 20 -> prod 50
ALTER TABLE [dbo].[DOCUMENTOIDENTIFICACAO] ALTER COLUMN [nomedocumento] varchar(200) NULL;   -- local 100 -> prod 200
ALTER TABLE [dbo].[DOCUMENTOIDENTIFICACAO] ALTER COLUMN [numero] varchar(50) NULL;   -- local 20 -> prod 50
ALTER TABLE [dbo].[MORADA] ALTER COLUMN [rua] varchar(150) NULL;   -- local 100 -> prod 150
ALTER TABLE [dbo].[TRABALHADOR] ALTER COLUMN [naturalidade] varchar(100) NULL;   -- local 50 -> prod 100
ALTER TABLE [dbo].[TRABALHADOR] ALTER COLUMN [niss] varchar(50) NULL;   -- local 20 -> prod 50
ALTER TABLE [dbo].[TRABALHADOR] ALTER COLUMN [nome] varchar(150) NULL;   -- local 100 -> prod 150
ALTER TABLE [dbo].[TRABALHADOR] ALTER COLUMN [nomemae] varchar(200) NULL;   -- local 100 -> prod 200
ALTER TABLE [dbo].[TRABALHADOR] ALTER COLUMN [nomepai] varchar(200) NULL;   -- local 100 -> prod 200
ALTER TABLE [dbo].[TRABALHADOR] ALTER COLUMN [numinscprovisoria] varchar(50) NULL;   -- local 20 -> prod 50
ALTER TABLE [dbo].[TRABALHADOR] ALTER COLUMN [tin] varchar(50) NULL;   -- local 20 -> prod 50
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID('DEPARTAMENTO') AND name='institutionid') ALTER TABLE [dbo].[DEPARTAMENTO] ADD [institutionid] int NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID('RELUTILIZADORDEPARTAMENTO') AND name='institutionid') ALTER TABLE [dbo].[RELUTILIZADORDEPARTAMENTO] ADD [institutionid] int NULL;
GO
