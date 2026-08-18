/* ============================================================================
   LOCAL ONLY -- align the Centro de Custo (Regime) list with the client's
   ----------------------------------------------------------------------------
   The client's own list, from the query they ran on 2026-08-17 13:14, has four
   Regimes for the 2026 budget period:

       ADMINISTRAÇÃO FRSS      (their id 4)
       ADMINISTRAÇÃO INSS
       REGIME CONTRIBUTIVO
       REGIME NÃO CONTRIBUTIVO

   Our local ProdDev copy carried the older English names and had no FRSS row at
   all, which is why the FRSS case could not be reproduced here. This script
   renames the three existing rows and adds the missing FRSS one, so the local
   dropdown matches what the client sees.

   Ids stay as they are (ours differ from the client's, that is fine -- only the
   descriptions and the set of options matter for reproducing the behaviour).
   Only descriptions are touched; no foreign key changes, so existing budget
   lines and expenses keep pointing at the same Regime.

   NOT for Staging or Production. Local development database only.
   ============================================================================ */

SET NOCOUNT ON;

DECLARE @orcConfig2026 INT = 2008;   -- ORCAMENTOCONFIG covering 2026-01-01..2026-12-31
DECLARE @admin         INT = 23;

PRINT '--- BEFORE ---';
SELECT id, descricao, indActivo, orcamentoconfig_Fk
FROM CENTROCUSTO WHERE orcamentoconfig_Fk = @orcConfig2026 ORDER BY id;

-- Rename the existing three to the client's wording
UPDATE CENTROCUSTO SET descricao = N'REGIME CONTRIBUTIVO',     dataAlteracao = GETDATE(), utilizadorAlteracao = @admin
 WHERE orcamentoconfig_Fk = @orcConfig2026 AND descricao = N'Contributotry';

UPDATE CENTROCUSTO SET descricao = N'REGIME NÃO CONTRIBUTIVO', dataAlteracao = GETDATE(), utilizadorAlteracao = @admin
 WHERE orcamentoconfig_Fk = @orcConfig2026 AND descricao = N'Non Contributory';

UPDATE CENTROCUSTO SET descricao = N'ADMINISTRAÇÃO INSS',      dataAlteracao = GETDATE(), utilizadorAlteracao = @admin
 WHERE orcamentoconfig_Fk = @orcConfig2026 AND descricao = N'Administration';

-- Add the missing FRSS Regime (idempotent)
IF NOT EXISTS (SELECT 1 FROM CENTROCUSTO
               WHERE orcamentoconfig_Fk = @orcConfig2026 AND descricao = N'ADMINISTRAÇÃO FRSS')
BEGIN
    INSERT INTO CENTROCUSTO (descricao, dataInicio, dataFim, orcamentoconfig_Fk, indActivo,
                             utilizadorCriacao, dataCriacao, ipv6)
    SELECT N'ADMINISTRAÇÃO FRSS', oc.dataInicio, oc.dataFim, oc.id, 1, @admin, GETDATE(), '127.0.0.1'
    FROM ORCAMENTOCONFIG oc WHERE oc.id = @orcConfig2026;
    PRINT '>> ADMINISTRAÇÃO FRSS added.';
END
ELSE
    PRINT '>> ADMINISTRAÇÃO FRSS already present, nothing added.';

PRINT '--- AFTER ---';
SELECT id, descricao, indActivo, orcamentoconfig_Fk
FROM CENTROCUSTO WHERE orcamentoconfig_Fk = @orcConfig2026 ORDER BY descricao;
