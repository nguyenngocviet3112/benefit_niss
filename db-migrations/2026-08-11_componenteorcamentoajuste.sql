-- ============================================================================
-- New table: COMPONENTEORCAMENTOAJUSTE
--
-- Background: rubrica out of budget after execution (AD/Cabimento already
-- registered) has no working fix on prod-dev -- Retificação replaces the
-- whole approved envelope and is hard-blocked as soon as ANY despesa/receita
-- has been executed in the period (confirmed live-test 2026-08-11, error
-- "The Budget cannot be rectified because there are already executed budget
-- movements..."). This table backs a lightweight 2-step (Solicitar ->
-- Aprovar/Rejeitar) adjustment that updates COMPONENTEORCAMENTOVALOR.valor
-- in place on the CURRENT approved envelope, without touching Retificação:
--   - rubricaOrigem_fk IS NULL  -> top-up (adds straight to rubricaDestino_fk)
--   - rubricaOrigem_fk NOT NULL -> transfer (same valor moves out of origem,
--     into destino -- self-balancing, no separate "must sum to zero" check)
--
-- See: [[prod-dev]] "INSS Reference docs/fin and contrib doc/2. Output/
-- Regras_Negocio_Ciclo_Despesa_New_Mode.md" #7 for the full design decision.
--
-- Idempotent: only creates the table if it doesn't already exist.
-- Additive only -- does not touch any existing table/column.
-- BACK UP THE DATABASE BEFORE RUNNING ON STAGING/PRODUCTION.
-- ============================================================================

SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'COMPONENTEORCAMENTOAJUSTE')
BEGIN
    CREATE TABLE COMPONENTEORCAMENTOAJUSTE (
        id                      INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        componenteOrcamentoRegisto_fk INT NOT NULL,
        rubricaOrigem_fk        INT NULL,
        rubricaDestino_fk       INT NOT NULL,
        valor                   DECIMAL(20, 2) NOT NULL,
        estado                  VARCHAR(20) NOT NULL,
        motivo                  NVARCHAR(500) NULL,
        motivoRejeicao          NVARCHAR(500) NULL,
        utilizadorSolicitacao   INT NOT NULL,
        dataSolicitacao         DATETIME NOT NULL,
        utilizadorAprovacao     INT NULL,
        dataAprovacao           DATETIME NULL,
        indActivo               BIT NOT NULL DEFAULT 1,

        CONSTRAINT FK_componenteOrcamentoAjuste_componenteOrcamentoRegisto
            FOREIGN KEY (componenteOrcamentoRegisto_fk) REFERENCES COMPONENTEORCAMENTO_REGISTO(id),
        CONSTRAINT FK_componenteOrcamentoAjuste_rubricaOrigem
            FOREIGN KEY (rubricaOrigem_fk) REFERENCES COMPONENTEORCAMENTOVALOR(id),
        CONSTRAINT FK_componenteOrcamentoAjuste_rubricaDestino
            FOREIGN KEY (rubricaDestino_fk) REFERENCES COMPONENTEORCAMENTOVALOR(id),
        CONSTRAINT FK_componenteOrcamentoAjuste_utilizadorSolicitacao
            FOREIGN KEY (utilizadorSolicitacao) REFERENCES UTILIZADOR(idUtilizador),
        CONSTRAINT FK_componenteOrcamentoAjuste_utilizadorAprovacao
            FOREIGN KEY (utilizadorAprovacao) REFERENCES UTILIZADOR(idUtilizador),
        CONSTRAINT CK_componenteOrcamentoAjuste_estado
            CHECK (estado IN ('Pendente', 'Aprovado', 'Rejeitado')),
        CONSTRAINT CK_componenteOrcamentoAjuste_valor
            CHECK (valor > 0),
        CONSTRAINT CK_componenteOrcamentoAjuste_origemDiferenteDestino
            CHECK (rubricaOrigem_fk IS NULL OR rubricaOrigem_fk <> rubricaDestino_fk)
    );

    CREATE INDEX IX_componenteOrcamentoAjuste_registo ON COMPONENTEORCAMENTOAJUSTE(componenteOrcamentoRegisto_fk);
    CREATE INDEX IX_componenteOrcamentoAjuste_estado ON COMPONENTEORCAMENTOAJUSTE(estado);

    PRINT 'COMPONENTEORCAMENTOAJUSTE table created.';
END
ELSE
BEGIN
    PRINT 'COMPONENTEORCAMENTOAJUSTE already exists, skipping.';
END
