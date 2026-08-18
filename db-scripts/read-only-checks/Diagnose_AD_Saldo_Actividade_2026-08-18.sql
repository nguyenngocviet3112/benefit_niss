-- ============================================================================
-- DIAGNOSTIC -- "available budget" on Registar Despesa (AD) does not match the
-- budget of the selected Activity                                    2026-08-18
-- ============================================================================
-- [EN] READ-ONLY. This script only runs SELECTs -- it changes nothing.
--
-- Reported case: COMPONENTEORCAMENTOVALOR has two rows for the same account
-- line (tipoConta_fk = 71, agrupamento_fk = 275) -- $72,000 for one Activity
-- and $48,000 for another. Registering an expense against the first Activity
-- is refused, saying the available budget is $48,000.
--
-- How the application computes that number:
--   available = SUM(budget rows matching Institution + Centro de Custo +
--                   Activity + Functional + account line, within the selected
--                   budget period)
--             - SUM(expenses on the same 5 keys, in state Registado /
--                   Autorizado / Cabimentado)
--
-- Two different causes produce the same symptom; this script separates them:
--   (A) the $72,000 row differs from the form in one of the OTHER keys
--       (Institution, Centro de Custo, Functional, or budget period), so the
--       application correctly does not count it.
--   (B) expenses belonging to a DIFFERENT budget period are being subtracted:
--       the budget side filters by period, the reserved side does not.
--
-- [VI] CHỈ ĐỌC, không sửa gì. Tách 2 nguyên nhân: (A) dòng 72,000 lệch ở một
-- khóa khác nên không được tính; (B) despesa thuộc kỳ ngân sách khác bị trừ vào
-- số dư kỳ hiện tại (phần ngân sách có lọc kỳ, phần đã-đặt-trước thì không).
-- ============================================================================

SET NOCOUNT ON;

-- Change these two values to inspect a different account line.
DECLARE @agrupamentoFk INT = 275;
DECLARE @tipoContaFk   INT = 71;

PRINT '====================================================================';
PRINT ' PART 1 -- every BUDGET row for this account line, with all its keys';
PRINT '====================================================================';

SELECT
    ov.id                            AS budgetRowId,
    ov.componenteOrcamentoRegisto_fk AS budgetPeriodId,
    ov.valor                         AS budgetAmount,
    ov.InstitutionId                 AS institutionId,
    ov.centroCusto_fk                AS centroCustoId,
    cc.descricao                     AS centroCusto,
    ov.departamento_fk               AS departamentoId,
    ov.actidade_fk                   AS activityId,
    act.codigo                       AS activityCode,
    LEFT(act.designacao, 35)         AS activity,
    ov.funcional_fk                  AS functionalId,
    fun.codigo                       AS functionalCode,
    ov.indActivo                     AS active
FROM COMPONENTEORCAMENTOVALOR ov
LEFT JOIN CENTROCUSTO       cc  ON cc.id  = ov.centroCusto_fk
LEFT JOIN AGRUPAMENTOCONFIG act ON act.id = ov.actidade_fk
LEFT JOIN AGRUPAMENTOCONFIG fun ON fun.id = ov.funcional_fk
WHERE ov.agrupamento_fk = @agrupamentoFk
ORDER BY ov.componenteOrcamentoRegisto_fk, ov.actidade_fk;

PRINT '';
PRINT '>>> Compare these rows key by key. A budget row is only counted when';
PRINT '>>> Institution, Centro de Custo, Activity, Functional AND budget period';
PRINT '>>> all match the form. One differing value (or a NULL) is enough to';
PRINT '>>> exclude the $72,000 row -- that would be cause (A).';
PRINT '';
PRINT '====================================================================';
PRINT ' PART 2 -- every EXPENSE already booked against this account line';
PRINT '====================================================================';

SELECT
    dr.id                           AS expenseId,
    dr.componenteOrcamentoRegistoFk AS budgetPeriodId,
    dr.valor                        AS expenseAmount,
    dom.valor                       AS stateCode,
    CASE dom.valor WHEN 1 THEN 'Registado' WHEN 2 THEN 'Autorizado'
                   WHEN 3 THEN 'Cabimentado' WHEN 4 THEN 'Executado'
                   ELSE '?' END     AS state,
    dr.InstitutionId                AS institutionId,
    dr.centroCusto_fk               AS centroCustoId,
    dr.actidade_fk                  AS activityId,
    act.codigo                      AS activityCode,
    dr.funcional_fk                 AS functionalId,
    LEFT(dr.descricao, 35)          AS description
FROM COMPONENTEDESPESA_REGISTO dr
LEFT JOIN DOMINIO           dom ON dom.idDominio = dr.estado
LEFT JOIN AGRUPAMENTOCONFIG act ON act.id = dr.actidade_fk
WHERE dr.agrupamentoConfig_fk = @agrupamentoFk
  AND dr.indActivo = 1
ORDER BY dr.componenteOrcamentoRegistoFk, dr.actidade_fk, dr.id;

PRINT '';
PRINT '====================================================================';
PRINT ' PART 3 -- available balance per key combination, computed two ways';
PRINT '====================================================================';
PRINT '  availableAsToday          = what the application computes right now';
PRINT '  availableIfPeriodFiltered = the same, but ignoring expenses that';
PRINT '                              belong to a different budget period';
PRINT '  A difference between these two columns means cause (B).';
PRINT '';

WITH Budget AS (
    SELECT ov.componenteOrcamentoRegisto_fk AS periodId, ov.InstitutionId AS institutionId,
           ov.centroCusto_fk AS centroCustoId, ov.actidade_fk AS activityId,
           ov.funcional_fk AS functionalId, SUM(ov.valor) AS budgetAmount
    FROM COMPONENTEORCAMENTOVALOR ov
    WHERE ov.agrupamento_fk = @agrupamentoFk AND ov.indActivo = 1
    GROUP BY ov.componenteOrcamentoRegisto_fk, ov.InstitutionId, ov.centroCusto_fk,
             ov.actidade_fk, ov.funcional_fk
),
Reserved AS (
    SELECT b.periodId, b.institutionId, b.centroCustoId, b.activityId, b.functionalId, b.budgetAmount,
        ISNULL((SELECT SUM(dr.valor) FROM COMPONENTEDESPESA_REGISTO dr
                JOIN DOMINIO d2 ON d2.idDominio = dr.estado
                WHERE dr.indActivo = 1 AND dr.agrupamentoConfig_fk = @agrupamentoFk
                  AND dr.InstitutionId = b.institutionId AND dr.centroCusto_fk = b.centroCustoId
                  AND dr.actidade_fk = b.activityId AND dr.funcional_fk = b.functionalId
                  AND d2.valor IN (1,2,3)), 0) AS reservedAllPeriods,
        ISNULL((SELECT SUM(dr.valor) FROM COMPONENTEDESPESA_REGISTO dr
                JOIN DOMINIO d2 ON d2.idDominio = dr.estado
                WHERE dr.indActivo = 1 AND dr.agrupamentoConfig_fk = @agrupamentoFk
                  AND dr.InstitutionId = b.institutionId AND dr.centroCusto_fk = b.centroCustoId
                  AND dr.actidade_fk = b.activityId AND dr.funcional_fk = b.functionalId
                  AND dr.componenteOrcamentoRegistoFk = b.periodId
                  AND d2.valor IN (1,2,3)), 0) AS reservedThisPeriod
    FROM Budget b
)
SELECT r.periodId AS budgetPeriodId, act.codigo AS activityCode, r.institutionId,
       r.centroCustoId, r.functionalId, r.budgetAmount,
       r.reservedAllPeriods, r.reservedThisPeriod,
       r.budgetAmount - r.reservedAllPeriods  AS availableAsToday,
       r.budgetAmount - r.reservedThisPeriod  AS availableIfPeriodFiltered
FROM Reserved r
LEFT JOIN AGRUPAMENTOCONFIG act ON act.id = r.activityId
ORDER BY r.periodId, act.codigo;

PRINT '';
PRINT '====================================================================';
PRINT ' PART 4 -- budget periods on this database';
PRINT '====================================================================';

SELECT id AS budgetPeriodId, dataInicio, dataFim, aprovado, indActivo AS active
FROM COMPONENTEORCAMENTO_REGISTO ORDER BY id;

PRINT '';
PRINT 'Please send back the output of all 4 parts.';
