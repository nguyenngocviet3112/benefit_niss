-- ============================================================================
-- DIAGNOSTIC -- "Cabimentar" button does not appear on the Cabimento task
--                                                                   2026-08-18
-- ============================================================================
-- [EN] READ-ONLY. Only SELECTs -- changes nothing.
--
-- The button is rendered only when BOTH of these are true:
--   (1) the Despesa component of THAT task has visualizarDespesaAParaC = 2
--       -- this is per-task CONFIGURATION stored in the database, not code;
--   (2) the process has NO expense left in state Registado, and at least one
--       in state Autorizado.
--
-- Part 1 checks (1), Part 2 checks (2). Part 3 is unrelated to the button but
-- worth capturing while we are here: expenses saved before the Institution /
-- Actividade / Funcional fields existed carry 0 or NULL in those columns, which
-- no longer matches any budget line, so editing or authorising them is refused
-- with "no budget allocated".
--
-- [VI] CHỈ ĐỌC. Nút Cabimentar chỉ hiện khi ĐỒNG THỜI: (1) componente Despesa của
-- đúng tarefa đó có visualizarDespesaAParaC = 2 -- đây là CẤU HÌNH trong DB theo
-- từng tarefa, không phải code; (2) processo không còn despesa nào ở trạng thái
-- Registado và có ít nhất một despesa Autorizado. Phần 3 kiểm tra dữ liệu cũ mang
-- InstitutionId/actidade/funcional = 0 hoặc NULL.
-- ============================================================================

SET NOCOUNT ON;

PRINT '====================================================================';
PRINT ' PART 1 -- Despesa component configuration, per task';
PRINT '====================================================================';
PRINT ' Look at the row for the Cabimento task: visualizarDespesaAParaC must be 2.';
PRINT '';

SELECT
    pc.nome                         AS processo,
    t.id                            AS tarefaId,
    t.nome                          AS tarefa,
    rel.tarefaInicial               AS tarefaInicial,
    cd.registarDespesa              AS registar,
    cd.visualizarDespesaRParaA      AS btnAutorizar_RparaA,
    cd.visualizarDespesaAParaC      AS btnCabimentar_AparaC,
    cd.visualizarDespesaR           AS verRegistadas,
    cd.visualizarDespesaA           AS verAutorizadas,
    cd.visualiazarDespesaC          AS verCabimentadas,
    cd.indActivo                    AS activo
FROM COMPONENTEDESPESA cd
JOIN TAREFA t                       ON t.id  = cd.tarefa_fk
LEFT JOIN RELPROCESSOCONFIGTAREFA rel ON rel.tarefa_fk = t.id AND rel.indActivo = 1
LEFT JOIN PROCESSOCONFIG pc         ON pc.id = rel.processoConfig_fk
WHERE cd.indActivo = 1
ORDER BY pc.nome, t.id;

PRINT '';
PRINT '====================================================================';
PRINT ' PART 2 -- Expenses of the most recent processes, by state';
PRINT '====================================================================';
PRINT ' The button also needs: zero expenses in state R, at least one in state A.';
PRINT '';

SELECT TOP 40
    pa.id                           AS processoAtivoId,
    pa.numeroProcesso               AS numeroProcesso,
    ta.id                           AS tarefaActivoId,
    t.nome                          AS tarefaActual,
    dr.id                           AS despesaId,
    dom.valor                       AS estadoCode,
    CASE dom.valor WHEN 1 THEN 'Registado' WHEN 2 THEN 'Autorizado'
                   WHEN 3 THEN 'Cabimentado' WHEN 4 THEN 'Executado'
                   ELSE '?' END     AS estado,
    dr.valor                        AS valor,
    LEFT(dr.descricao, 30)          AS descricao
FROM COMPONENTEDESPESA_REGISTO dr
JOIN TAREFAATIVO    ta  ON ta.id = dr.tarefaActivo_fk
JOIN PROCESSOATIVO  pa  ON pa.id = ta.processoAtivo_fk
JOIN TAREFA         t   ON t.id  = ta.tarefaconfig_fk
LEFT JOIN DOMINIO   dom ON dom.idDominio = dr.estado
WHERE dr.indActivo = 1
ORDER BY pa.id DESC, dr.id;

PRINT '';
PRINT '====================================================================';
PRINT ' PART 3 -- Expenses whose key columns were never filled (legacy rows)';
PRINT '====================================================================';
PRINT ' These cannot be matched to any budget line: editing or authorising them';
PRINT ' is refused with "no budget allocated" even though budget exists.';
PRINT '';

SELECT
    SUM(CASE WHEN dr.InstitutionId IS NULL OR dr.InstitutionId = 0 THEN 1 ELSE 0 END) AS semInstitution,
    SUM(CASE WHEN dr.actidade_fk   IS NULL OR dr.actidade_fk   = 0 THEN 1 ELSE 0 END) AS semActividade,
    SUM(CASE WHEN dr.funcional_fk  IS NULL OR dr.funcional_fk  = 0 THEN 1 ELSE 0 END) AS semFuncional,
    COUNT(*)                                                                          AS totalDespesasActivas
FROM COMPONENTEDESPESA_REGISTO dr
WHERE dr.indActivo = 1;

SELECT TOP 20
    dr.id, dr.InstitutionId, dr.actidade_fk, dr.funcional_fk, dr.centroCusto_fk,
    dr.agrupamentoConfig_fk, dr.valor, LEFT(dr.descricao, 30) AS descricao
FROM COMPONENTEDESPESA_REGISTO dr
WHERE dr.indActivo = 1
  AND (dr.InstitutionId IS NULL OR dr.InstitutionId = 0
       OR dr.actidade_fk IS NULL OR dr.actidade_fk = 0
       OR dr.funcional_fk IS NULL OR dr.funcional_fk = 0)
ORDER BY dr.id DESC;

PRINT '';
PRINT 'DONE -- please send back the output of all 3 parts.';
