-- ============================================================================
-- CHECK (and restore) Programa roots wrongly deactivated                2026-08-18
-- ============================================================================
-- [EN] Why you are being asked to run this.
--
-- The CE_OSS_Global script contains a clean-up step that deactivates duplicate
-- Programa root rows coded '04' / '05' / '06'. Those duplicates could be created
-- by a much older copy of that script, which looked for Programa roots by the
-- wrong code format.
--
-- That clean-up was too broad: it deactivated ANY Programa root coded
-- '04' / '05' / '06', without checking that a real 'A04' / 'A05' / 'A06' row
-- existed for it to be a duplicate OF. On a database whose Programa roots are
-- legitimately coded '04' / '05' / '06' -- with the "A04 - ..." prefix carried in
-- the description instead of the code -- the step switched off live master data:
-- the three Programa roots and everything beneath them.
--
-- This has been corrected in the script, but if you already ran an earlier copy,
-- the rows may still be switched off on that database. This script checks, and
-- restores them if needed. It is READ-ONLY unless it finds the exact problem.
--
-- Run it on every environment where the CE_OSS_Global script was run
-- (Dev, Staging and Production).
--
-- [VI] Bước dọn "junk" trong script CE_OSS_Global trước đây tắt MỌI Programa gốc
-- có codigo '04'/'05'/'06' mà không kiểm tra xem có bản 'A04'/'A05'/'A06' thật để
-- coi là trùng hay không. Trên DB vốn đánh mã Programa gốc là '04'/'05'/'06' (tên
-- "A04 - ..." nằm trong designacao), bước này tắt nhầm dữ liệu gốc đang dùng.
-- Script này kiểm tra và bật lại nếu đúng là bị tắt nhầm. Chỉ ghi khi thực sự có
-- vấn đề. Chạy trên cả 3 môi trường đã chạy script CE_OSS_Global.
-- ============================================================================

SET NOCOUNT ON;

PRINT '====================================================================';
PRINT ' Programa roots on this database';
PRINT '====================================================================';

SELECT a.id, a.codigo, LEFT(a.designacao, 45) AS designacao, a.indActivo AS active
FROM AGRUPAMENTOCONFIG a
JOIN RELTIPODECONTAORCAMENTOCONFIG r ON a.reltipoDeContaOrcamentoConfig_fk = r.id
JOIN DOMINIO d ON r.tipoConta_fk = d.idDominio
WHERE d.dominio = 'TIPOCONTA' AND d.descricao = 'Actidade' AND a.parent_fk IS NULL
ORDER BY a.codigo;

-- Affected = root coded 04/05/06, currently INACTIVE, and NO real 'A04'/'A05'/'A06'
-- counterpart exists -- i.e. it was never a duplicate, so it should not have been off.
DECLARE @affected TABLE (id INT);
INSERT INTO @affected (id)
SELECT j.id
FROM AGRUPAMENTOCONFIG j
JOIN RELTIPODECONTAORCAMENTOCONFIG r ON j.reltipoDeContaOrcamentoConfig_fk = r.id
JOIN DOMINIO d ON r.tipoConta_fk = d.idDominio
WHERE d.dominio = 'TIPOCONTA' AND d.descricao = 'Actidade'
  AND j.parent_fk IS NULL
  AND j.codigo IN ('04','05','06')
  AND j.indActivo = 0
  AND NOT EXISTS (
      SELECT 1 FROM AGRUPAMENTOCONFIG real_
      WHERE real_.codigo = 'A' + j.codigo
        AND real_.parent_fk IS NULL
        AND real_.reltipoDeContaOrcamentoConfig_fk = j.reltipoDeContaOrcamentoConfig_fk
  );

DECLARE @n INT = (SELECT COUNT(*) FROM @affected);

PRINT '';
IF @n = 0
BEGIN
    PRINT 'RESULT: nothing to restore on this database -- no Programa root was wrongly deactivated.';
END
ELSE
BEGIN
    PRINT CONCAT('FOUND: ', @n, ' Programa root(s) were deactivated in error. Restoring them and everything beneath them...');

    -- whole subtree, any depth
    WITH Tree AS (
        SELECT id FROM AGRUPAMENTOCONFIG WHERE id IN (SELECT id FROM @affected)
        UNION ALL
        SELECT a.id FROM AGRUPAMENTOCONFIG a JOIN Tree t ON a.parent_fk = t.id
    )
    UPDATE AGRUPAMENTOCONFIG SET indActivo = 1
    WHERE id IN (SELECT id FROM Tree) AND indActivo = 0;

    PRINT CONCAT('RESTORED: ', @@ROWCOUNT, ' row(s) set back to active.');
    PRINT '';
    PRINT 'Programa roots after restoring:';

    SELECT a.id, a.codigo, LEFT(a.designacao, 45) AS designacao, a.indActivo AS active
    FROM AGRUPAMENTOCONFIG a
    JOIN RELTIPODECONTAORCAMENTOCONFIG r ON a.reltipoDeContaOrcamentoConfig_fk = r.id
    JOIN DOMINIO d ON r.tipoConta_fk = d.idDominio
    WHERE d.dominio = 'TIPOCONTA' AND d.descricao = 'Actidade' AND a.parent_fk IS NULL
    ORDER BY a.codigo;
END

PRINT '';
PRINT 'DONE.';
