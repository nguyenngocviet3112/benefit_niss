-- ============================================================================
-- Enable the "Cabimentar" button on the RD03 Cabimento task          2026-08-18
-- ============================================================================
-- [EN] Problem: on the Cabimento task the "Cabimentar" button never appears, so
-- authorised expenses cannot be committed. The button is shown only when the
-- Despesa component of THAT task has visualizarDespesaAParaC = 2. On the affected
-- environment it is 0 ("Not Visible"). This is per-task CONFIGURATION stored in
-- the database -- it does NOT travel with a code deployment, so it has to be
-- applied on every environment (Dev, Staging, Production).
--
-- The same change can be made through the UI: Tasks Configure -> select the RD03
-- task -> panel "Despesa" -> "Visualizar Despesas Autorizadas para Cabimentação"
-- -> choose "Editável" -> Gravar. This script is the equivalent, for when the
-- same change has to be repeated identically across environments.
--
-- Safe: finds the task BY NAME (task ids differ between environments), touches
-- exactly one column of one row, changes nothing else -- in particular it does
-- not touch which Perfil/Utilizador the task is assigned to. Read-only if the
-- value is already correct. Run it as many times as you like.
--
-- BACK UP THE DATABASE BEFORE RUNNING (standard rule).
--
-- [VI] Nút "Cabimentar" không hiện vì componente Despesa của tarefa Cabimento có
-- visualizarDespesaAParaC = 0, phải là 2. Đây là CẤU HÌNH trong DB theo từng
-- tarefa, KHÔNG đi theo code deploy nên phải chạy trên cả 3 môi trường. Script tìm
-- tarefa THEO TÊN (id tarefa khác nhau giữa các môi trường), chỉ sửa đúng 1 cột
-- của 1 dòng, không đụng phần gán Perfil/Utilizador của tarefa.
-- ============================================================================

SET NOCOUNT ON;

-- ----------------------------------------------------------------------------
-- Fill in the username of the person applying this change. It is written to
-- utilizadorAlteracao so the change history shows who did it. If the username is
-- not found the script stops rather than blaming an arbitrary user id.
-- [VI] Điền username của người thực hiện thay đổi -- ghi vào utilizadorAlteracao
-- để lịch sử sửa đổi biết ai làm. Không tìm thấy username thì script DỪNG, không
-- gán bừa một id nào đó.
-- ----------------------------------------------------------------------------
DECLARE @username VARCHAR(100) = 'PUT-USERNAME-HERE';

DECLARE @utilizadorId INT = (SELECT idUtilizador FROM UTILIZADOR WHERE username = @username AND indActivo = 1);

IF @utilizadorId IS NULL
BEGIN
    PRINT CONCAT('*** STOPPED: no active user named "', @username, '" on this database. Edit the @username line at the top of this script and run it again.');
    PRINT '    (Tip: SELECT idUtilizador, username FROM UTILIZADOR WHERE indActivo = 1 ORDER BY username;)';
    RETURN;
END

PRINT '====================================================================';
PRINT ' BEFORE -- Despesa component configuration per task';
PRINT '====================================================================';

SELECT t.id AS tarefaId, t.nome AS tarefa,
       cd.visualizarDespesaRParaA AS btnAutorizar,
       cd.visualizarDespesaAParaC AS btnCabimentar
FROM COMPONENTEDESPESA cd
JOIN TAREFA t ON t.id = cd.tarefa_fk
WHERE cd.indActivo = 1
ORDER BY t.nome;

-- ----------------------------------------------------------------------------
-- Target: the Cabimento task, matched by name -- NOT by id, which differs
-- between environments.
-- [VI] Đối tượng: tarefa Cabimento, khớp THEO TÊN -- không theo id, vì id khác
-- nhau giữa các môi trường.
-- ----------------------------------------------------------------------------
DECLARE @alvo TABLE (tarefaId INT, tarefa VARCHAR(200), valorActual INT);

INSERT INTO @alvo (tarefaId, tarefa, valorActual)
SELECT t.id, t.nome, cd.visualizarDespesaAParaC
FROM COMPONENTEDESPESA cd
JOIN TAREFA t ON t.id = cd.tarefa_fk
WHERE cd.indActivo = 1
  AND t.indActivo = 1
  AND t.nome LIKE 'RD03%'
  AND t.nome LIKE '%abimento%';

DECLARE @n INT = (SELECT COUNT(*) FROM @alvo);

PRINT '';
IF @n = 0
BEGIN
    PRINT '*** STOPPED: no active task matching "RD03 ... Cabimento" has a Despesa component on this database.';
    PRINT '    Check the task list printed above and apply the change through the UI instead:';
    PRINT '    Tasks Configure -> the Cabimento task -> panel Despesa -> "Visualizar Despesas Autorizadas para Cabimentacao" -> Editavel.';
    RETURN;
END

IF @n > 1
BEGIN
    PRINT CONCAT('*** STOPPED: ', @n, ' tasks match "RD03 ... Cabimento" -- refusing to guess which one to change. Apply it through the UI, or narrow the match after checking the list above.');
    SELECT tarefaId, tarefa, valorActual FROM @alvo;
    RETURN;
END

DECLARE @tarefaId INT   = (SELECT TOP 1 tarefaId FROM @alvo);
DECLARE @tarefaNome VARCHAR(200) = (SELECT TOP 1 tarefa FROM @alvo);
DECLARE @actual INT     = (SELECT TOP 1 valorActual FROM @alvo);

IF @actual = 2
    PRINT CONCAT('Task "', @tarefaNome, '" (id ', @tarefaId, ') already has the Cabimentar button enabled -- nothing to do.');
ELSE
BEGIN
    UPDATE cd
    SET cd.visualizarDespesaAParaC = 2,
        cd.utilizadorAlteracao = @utilizadorId,
        cd.dataAlteracao = GETDATE()
    FROM COMPONENTEDESPESA cd
    WHERE cd.tarefa_fk = @tarefaId AND cd.indActivo = 1;

    PRINT CONCAT('UPDATED: task "', @tarefaNome, '" (id ', @tarefaId, ') -- visualizarDespesaAParaC changed from ', @actual, ' to 2, by user "', @username, '" (id ', @utilizadorId, ').');
END

PRINT '';
PRINT '====================================================================';
PRINT ' AFTER -- verification';
PRINT '====================================================================';

SELECT t.id AS tarefaId, t.nome AS tarefa,
       cd.visualizarDespesaAParaC AS btnCabimentar,
       cd.utilizadorAlteracao AS alteradoPorId,
       cd.dataAlteracao AS alteradoEm
FROM COMPONENTEDESPESA cd
JOIN TAREFA t ON t.id = cd.tarefa_fk
WHERE cd.indActivo = 1 AND t.id = @tarefaId;

PRINT '';
PRINT 'DONE. The Cabimentar button appears once every expense in the process is Authorized.';
