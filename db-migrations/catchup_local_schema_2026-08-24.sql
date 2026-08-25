-- Dua DB local (phuc hoi tu db_backup/TimorINSSModuloContribuicoes_FullBackup_17062025.bak)
-- len ngang bang voi model EF hien tai.
--
-- Ban backup 17/06/2025 cu hon code, thieu 12 cot va 1 bang. Danh sach nay khong phai doan:
-- no duoc sinh ra bang cach doc toan bo anh xa entity->bang va property->cot trong
-- Models/TimorINSSModuloContribuicoesContext.cs roi doi chieu voi sys.columns cua DB.
--
-- An toan chay lai nhieu lan (moi lenh deu co IF NOT EXISTS).
-- CHI dung cho DB local. Staging/production da co san cac cot nay qua cac lan trien khai truoc.

USE TimorINSSModuloContribuicoes_ProdDev;
GO

-- ---------------------------------------------------------------------------
-- 1. Cot bankCode: ngan hang cua tung lenh chi. Bo loc ngan hang o man Despesa
--    va viec gom nhom trong PDF deu doc cot nay.
-- ---------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('PAGAMENTOSEXECUTADOS') AND name = 'bankCode')
    ALTER TABLE PAGAMENTOSEXECUTADOS ADD bankCode VARCHAR(50) NULL;
GO

-- ---------------------------------------------------------------------------
-- 2. InstitutionId: tach du lieu theo to chuc, them vao 4 bang.
-- ---------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('COMPONENTEDESPESA_REGISTO') AND name = 'InstitutionId')
    ALTER TABLE COMPONENTEDESPESA_REGISTO ADD InstitutionId INT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('COMPONENTEORCAMENTOVALOR') AND name = 'InstitutionId')
    ALTER TABLE COMPONENTEORCAMENTOVALOR ADD InstitutionId INT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('COMPONENTERECEITA_REGISTO') AND name = 'InstitutionId')
    ALTER TABLE COMPONENTERECEITA_REGISTO ADD InstitutionId INT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('MOVIMENTOSPORCONCILIAR') AND name = 'InstitutionId')
    ALTER TABLE MOVIMENTOSPORCONCILIAR ADD InstitutionId INT NULL;
GO

-- ---------------------------------------------------------------------------
-- 3. Ba truc phan loai ngan sach (actividade / economic / funcional) tren
--    Cabimento va tren dong gia tri ngan sach.
-- ---------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('COMPONENTEDESPESA_REGISTO') AND name = 'actidade_fk')
    ALTER TABLE COMPONENTEDESPESA_REGISTO ADD actidade_fk INT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('COMPONENTEDESPESA_REGISTO') AND name = 'economic_fk')
    ALTER TABLE COMPONENTEDESPESA_REGISTO ADD economic_fk INT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('COMPONENTEDESPESA_REGISTO') AND name = 'funcional_fk')
    ALTER TABLE COMPONENTEDESPESA_REGISTO ADD funcional_fk INT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('COMPONENTEORCAMENTOVALOR') AND name = 'actidade_fk')
    ALTER TABLE COMPONENTEORCAMENTOVALOR ADD actidade_fk INT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('COMPONENTEORCAMENTOVALOR') AND name = 'economic_fk')
    ALTER TABLE COMPONENTEORCAMENTOVALOR ADD economic_fk INT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('COMPONENTEORCAMENTOVALOR') AND name = 'funcional_fk')
    ALTER TABLE COMPONENTEORCAMENTOVALOR ADD funcional_fk INT NULL;
GO

-- ---------------------------------------------------------------------------
-- 4. Ten file goc cua tai lieu dinh kem.
-- ---------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('COMPONENTEDOCUMENTO_REGISTO') AND name = 'nomeDocumento')
    ALTER TABLE COMPONENTEDOCUMENTO_REGISTO ADD nomeDocumento NVARCHAR(255) NULL;
GO

-- ---------------------------------------------------------------------------
-- 5. Bang doi chieu Agrupamento -> Classificacao Economica, dung cho bao cao
--    CE_OSS_Global. Bang rong sau khi tao: du lieu doi chieu duoc nap rieng.
-- ---------------------------------------------------------------------------
IF OBJECT_ID('RELAGRUPAMENTOCONFIGCLASSIFICACAOECONOMICA', 'U') IS NULL
BEGIN
    CREATE TABLE RELAGRUPAMENTOCONFIGCLASSIFICACAOECONOMICA (
        id                          INT IDENTITY(1,1) NOT NULL,
        agrupamentoConfigOrigem_fk  INT           NOT NULL,
        agrupamentoConfigCE_fk      INT           NOT NULL,
        confianca                   VARCHAR(10)   NOT NULL,
        indActivo                   BIT           NOT NULL,
        utilizadorCriacao           INT           NOT NULL,
        dataCriacao                 DATETIME      NOT NULL,
        CONSTRAINT PK_RelAgrupamentoConfigCE PRIMARY KEY (id),
        CONSTRAINT FK_RelAgrupamentoConfigCE_Origem FOREIGN KEY (agrupamentoConfigOrigem_fk)
            REFERENCES AGRUPAMENTOCONFIG (id),
        CONSTRAINT FK_RelAgrupamentoConfigCE_Ce FOREIGN KEY (agrupamentoConfigCE_fk)
            REFERENCES AGRUPAMENTOCONFIG (id)
    );
END
GO

-- ---------------------------------------------------------------------------
-- 6. Tieu de tuy chon cua danh sach chi tra (INSS-038). Cot nay CUNG can chay
--    tren staging/production truoc khi trien khai tinh nang do.
-- ---------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TAREFAATIVO') AND name = 'TituloListaPagamento')
    ALTER TABLE TAREFAATIVO ADD TituloListaPagamento NVARCHAR(255) NULL;
GO
