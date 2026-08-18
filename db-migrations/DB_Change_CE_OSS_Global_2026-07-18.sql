-- ============================================================================
-- DB Migration: CE_OSS_Global (Classificação Económica report) — 2026-07-18
-- ============================================================================
-- [EN] Scope: ONLY the DB changes belonging to the CE_OSS_Global feature that
-- must run on production for the feature to work. Does NOT include:
--   - ALTER TABLE COMPONENTERECEITA_REGISTO ADD InstitutionId -- confirmed
--     production already has this column (only the dev DB was missing it,
--     due to an old backup).
--   - Relinking Conta OSS to the 2026 period -- that was a fix SPECIFIC to the
--     dev DB (two duplicate 2026 budget periods). Check production separately
--     for the same issue before assuming it applies; do not run it blindly.
--
-- Safe to run multiple times (idempotent) -- every step checks existence
-- before creating/inserting.
--
-- [VI] Phạm vi: CHỈ những thay đổi DB thuộc về tính năng CE_OSS_Global, cần chạy
-- trên production để tính năng hoạt động. KHÔNG bao gồm:
--   - ALTER TABLE COMPONENTERECEITA_REGISTO ADD InstitutionId -- đã xác nhận
--     production đã có sẵn cột này (chỉ DB dev thiếu do backup cũ).
--   - Việc relink Conta OSS sang kỳ 2026 -- đó là sửa lỗi ĐẶC THÙ của DB dev
--     (2 kỳ ngân sách 2026 trùng lặp). Cần TỰ kiểm tra trên production có cùng
--     vấn đề không, không nên chạy máy móc theo dev.
--
-- An toàn chạy nhiều lần (idempotent) -- có kiểm tra tồn tại trước khi tạo/thêm.
--
-- ============================================================================
-- [EN] WHAT TO KNOW/DO BEFORE RUNNING (kept to the bare minimum -- the script
-- auto-detects everything it possibly can, so the client does not need to
-- verify any technical detail):
-- ============================================================================
--   1. BACK UP the database before running (standard rule, always do this
--      before any migration -- this is a pure ops step, no client input needed).
--   2. The script AUTOMATICALLY detects: the currently-active budget period
--      (via indActivo + date range, not by guessing an ID), and looks up
--      Conta OGE / CE-code IDs by their CODE PATH, not by hardcoded ID -- so
--      there is NOTHING to verify or confirm about IDs or which period is active.
--   3. The label "A07 - Regime Contributivo de Capitalização" (Part 2) follows
--      the client's own AD form -- still awaiting the client's reply on a
--      possible naming conflict with "A08" (see [[open-questions-for-client]]
--      item 3). This does NOT block running the script now -- it can be
--      updated later with a simple UPDATE once the client replies.
--   4. *** UPDATED 2026-07-19 after reviewing the client's real production data
--      export (extract_result.xlsx) ***: production's Despesa/Receita AGRUPAMENTOCONFIG
--      tree is ALREADY organized directly by the real Classificação Económica codes
--      (root codigo 501-506 for Despesa, 401-409 for Receita) -- there is no separate
--      "old Conta OGE" scheme to cross-walk FROM on production. So on production, Part 3
--      is expected to insert 0 (or very few) rows -- that is CORRECT, not an error, and
--      does not need to be reported back. (On dev, which still has the older/legacy
--      Despesa-Receita numbering the crosswalk was designed for, more rows will match --
--      that difference between environments is expected.) The backend code has been
--      updated accordingly to recognize 401-409/501-506 as already-real-CE directly,
--      so the report works correctly on production even with an empty/near-empty
--      crosswalk table.
--   ==> In short: NOTHING needs to be asked of the client before running this
--   script -- only back up first. Seeing "0 rows added" from Part 3 on production/staging
--   is expected and fine -- do NOT try to force it to reach 33 there.
-- ============================================================================
-- [VI] CẦN LÀM/BIẾT TRƯỚC KHI CHẠY (đã tối giản tối đa phần phải hỏi khách --
-- khách không rành kỹ thuật, script tự động hoá mọi thứ có thể tự xác định được):
-- ============================================================================
--   1. BACKUP DB trước khi chạy (quy tắc chuẩn, luôn làm trước mọi migration --
--      đây là thao tác kỹ thuật thuần, không cần hỏi khách).
--   2. Script TỰ động xác định: kỳ ngân sách hiện hành (theo indActivo + ngày,
--      không đoán theo ID), và tự tra ID của Conta OGE/mã CE theo TÊN MÃ (path),
--      không hardcode ID -- nên KHÔNG cần khách xác minh ID hay chọn kỳ ngân sách.
--   3. Tên gọi "A07 - Regime Contributivo de Capitalização" (Phần 2) đang dùng
--      theo form AD của khách -- CHỜ khách trả lời câu hỏi A07/A08 đã gửi (xem
--      [[open-questions-for-client]] mục 3), nhưng KHÔNG cần chờ mới chạy được --
--      chạy trước, UPDATE lại designacao sau khi có câu trả lời (không ảnh hưởng
--      tới việc báo cáo chạy đúng ngay bây giờ).
--   4. *** CẬP NHẬT 2026-07-19 sau khi xem file export dữ liệu thật của khách
--      (extract_result.xlsx) ***: cây AGRUPAMENTOCONFIG Despesa/Receita trên production
--      ĐÃ được đánh mã trực tiếp theo Classificação Económica thật (mã gốc 501-506 cho
--      Despesa, 401-409 cho Receita) -- KHÔNG có cây "Conta OGE cũ" riêng để đối chiếu
--      trên production. Nên trên production, Phần 3 sẽ chèn được 0 (hoặc rất ít) dòng --
--      đó là ĐÚNG, không phải lỗi, không cần báo lại. (Trên dev thì có cây mã cũ nên sẽ
--      khớp được nhiều dòng hơn -- khác biệt này giữa 2 môi trường là bình thường.) Code
--      backend đã được sửa để tự nhận diện mã 401-409/501-506 là CE thật luôn, nên báo
--      cáo vẫn chạy đúng trên production dù bảng đối chiếu trống/gần trống.
--   ==> Tóm lại: KHÔNG cần hỏi khách gì trước khi chạy script -- chỉ cần backup
--   trước. Nếu Phần 3 báo "0 dòng" khi chạy trên production/staging thì đó là bình
--   thường -- ĐỪNG cố tìm cách cho ra đủ 33 dòng ở môi trường đó.
-- ============================================================================

SET NOCOUNT ON;

-- ============================================================================
-- CLEANUP / DỌN DẸP — only does something if an OLDER version of this script
-- (before 2026-07-19) was already run on this environment. That version looked
-- up the Programa roots by codigo='04'/'05'/'06', but production's real codigo
-- is 'A04'/'A05'/'A06' -- so it would NOT have found them, and would have
-- created 3 duplicate junk Programa roots (+ their subprograma/atividade
-- children) sitting alongside the real ones. This step detects and deactivates
-- exactly that junk, and is a safe no-op if it never happened (0 rows found).
-- [VI] Chỉ có tác dụng nếu bản CŨ của script này (trước 2026-07-19) đã lỡ chạy
-- trên môi trường này. Bản cũ tra theo codigo='04'/'05'/'06', nhưng mã thật của
-- production là 'A04'/'A05'/'A06' -- nên sẽ không tìm thấy, và tạo ra 3 dòng
-- Programa gốc rác (kèm subprograma/atividade con) nằm cạnh dòng thật. Bước này
-- tìm và vô hiệu hoá đúng số rác đó -- an toàn, không làm gì nếu chưa từng xảy ra.
-- ============================================================================
DECLARE @junkRoots TABLE (id INT);
INSERT INTO @junkRoots (id)
SELECT id FROM AGRUPAMENTOCONFIG
WHERE codigo IN ('04','05','06')
  AND parent_fk IS NULL
  AND indActivo = 1
  AND reltipoDeContaOrcamentoConfig_fk IN (
      SELECT r.id FROM RELTIPODECONTAORCAMENTOCONFIG r
      JOIN DOMINIO d ON r.tipoConta_fk = d.idDominio
      WHERE d.descricao = 'Actidade'
  );

DECLARE @junkCount INT = (SELECT COUNT(*) FROM @junkRoots);

IF @junkCount > 0
BEGIN
    PRINT CONCAT('Found ', @junkCount, ' duplicate junk Programa root(s) from an older run of this script (codigo 04/05/06) -- deactivating them and their children now.');

    UPDATE AGRUPAMENTOCONFIG SET indActivo = 0
    WHERE indActivo = 1 AND (
        id IN (SELECT id FROM @junkRoots)
        OR parent_fk IN (SELECT id FROM @junkRoots)
        OR parent_fk IN (SELECT id FROM AGRUPAMENTOCONFIG WHERE parent_fk IN (SELECT id FROM @junkRoots))
    );

    PRINT CONCAT(@@ROWCOUNT, ' junk row(s) deactivated (indActivo=0).');
END
ELSE
    PRINT 'No junk Programa entries found from an older script run -- nothing to clean up.';

-- ============================================================================
-- STEP 0 / BƯỚC 0 — Prerequisite check (from a previous session, MUST already exist)
-- Kiểm tra điều kiện tiên quyết (từ phiên làm việc trước, PHẢI đã có)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM DOMINIO WHERE dominio='TIPOCONTA' AND descricao='Actidade')
BEGIN
    RAISERROR('MISSING / THIẾU: DOMINIO "Actidade" (TIPOCONTA) does not exist yet / chưa tồn tại. Seed it first (see memory [[ce-oss-global-prod-dev-feasibility]]) before running this script further.', 16, 1);
    RETURN;
END

IF NOT EXISTS (SELECT 1 FROM DOMINIO WHERE dominio='TIPOCONTA' AND descricao='Funcional')
BEGIN
    RAISERROR('MISSING / THIẾU: DOMINIO "Funcional" (TIPOCONTA) does not exist yet / chưa tồn tại. Seed it first before running this script further.', 16, 1);
    RETURN;
END

PRINT 'Prerequisites OK -- continuing.';

-- ============================================================================
-- PART 1 / PHẦN 1 — New table: crosswalk from old Conta OGE code -> real
-- Classificação Económica code
-- Bảng mới: đối chiếu mã Conta OGE cũ -> mã Classificação Económica thật
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RELAGRUPAMENTOCONFIGCLASSIFICACAOECONOMICA')
BEGIN
    CREATE TABLE dbo.RELAGRUPAMENTOCONFIGCLASSIFICACAOECONOMICA (
        id INT PRIMARY KEY,
        agrupamentoConfigOrigem_fk INT NOT NULL REFERENCES dbo.AGRUPAMENTOCONFIG(id),
        agrupamentoConfigCE_fk INT NOT NULL REFERENCES dbo.AGRUPAMENTOCONFIG(id),
        confianca VARCHAR(10) NOT NULL,
        indActivo BIT NOT NULL DEFAULT 1,
        utilizadorCriacao INT NOT NULL,
        dataCriacao DATETIME NOT NULL
    );
    PRINT 'Table RELAGRUPAMENTOCONFIGCLASSIFICACAOECONOMICA created.';
END
ELSE
    PRINT 'Table RELAGRUPAMENTOCONFIGCLASSIFICACAOECONOMICA already exists, skipping.';

-- ============================================================================
-- PART 2 / PHẦN 2 — Real Programa/Subprograma/Atividade tree (A04-A07)
-- Uses DYNAMIC lookups (no hardcoded IDs) -- safe to run on any environment,
-- not just dev.
-- Cây Programa/Subprograma/Atividade thật (A04-A07). Dùng tra cứu ĐỘNG (không
-- hardcode ID) -- an toàn chạy trên môi trường khác dev.
-- ============================================================================

DECLARE @tipoContaActidade INT = (SELECT idDominio FROM DOMINIO WHERE dominio='TIPOCONTA' AND descricao='Actidade');

-- [EN] Pick the bucket tied to the budget period that is ACTUALLY ACTIVE RIGHT
-- NOW (Orcamentoconfig.indActivo=1 AND covers today's date) -- NOT just the
-- highest ID, because we have hit a real case of two periods with overlapping
-- dates where one was inactive yet had a higher ID than the active one
-- (see [[ce-oss-global-prod-dev-feasibility]]). Fully automatic, no manual
-- client confirmation needed.
-- [VI] Chọn bucket gắn với kỳ ngân sách ĐANG THẬT SỰ HOẠT ĐỘNG
-- (Orcamentoconfig.indActivo=1 VÀ phủ ngày hôm nay) -- KHÔNG chỉ lấy ID lớn
-- nhất, vì từng gặp thật trường hợp 2 kỳ trùng ngày, 1 kỳ inactive nhưng ID
-- vẫn lớn hơn kỳ đang active. Tự động, không cần khách xác nhận thủ công.
DECLARE @reltipoActidade INT = (
    SELECT TOP 1 r.id
    FROM RELTIPODECONTAORCAMENTOCONFIG r
    JOIN ORCAMENTOCONFIG oc ON r.orcamentoConfig_fk = oc.id
    WHERE r.tipoConta_fk = @tipoContaActidade
        AND oc.indActivo = 1
        AND GETDATE() >= oc.dataInicio
        AND (GETDATE() <= oc.dataFim OR oc.dataFim IS NULL)
    ORDER BY oc.dataInicio DESC
);

-- [EN] Fallback if no bucket matches today's date exactly (e.g. next year's
-- setup routine hasn't run yet) -- falls back to the most recent bucket that
-- is still flagged active, avoiding an empty/abandoned bucket with no indActivo.
-- [VI] Nếu không tìm thấy theo ngày đang active (VD: routine setup kỳ mới chưa
-- chạy), rơi về bucket gần nhất còn có indActivo=1 -- vẫn tránh lấy bucket
-- rỗng/bỏ hoang không có indActivo.
IF @reltipoActidade IS NULL
    SELECT TOP 1 @reltipoActidade = r.id
    FROM RELTIPODECONTAORCAMENTOCONFIG r
    JOIN ORCAMENTOCONFIG oc ON r.orcamentoConfig_fk = oc.id
    WHERE r.tipoConta_fk = @tipoContaActidade AND oc.indActivo = 1
    ORDER BY oc.dataInicio DESC;

IF @reltipoActidade IS NULL
BEGIN
    RAISERROR('MISSING / THIẾU: RELTIPODECONTAORCAMENTOCONFIG for Actidade (active period) does not exist / chưa tồn tại. Seed it first.', 16, 1);
    RETURN;
END

PRINT CONCAT('Using Actidade bucket reltipo_id = ', @reltipoActidade, ' (currently-active budget period, auto-detected by date -- not ID-dependent).');

-- [EN] *** REVISED 2026-07-19 after reviewing the client's real production data export
-- (Extract_Config_Dev_vs_Prod_2026-07-17 -> extract_result.xlsx, sheet config_dev_vs_prod) ***
-- Production ALREADY HAS A04/A05/A06 fully seeded (root + all subprograma/atividade), but
-- with codigo = 'A04'/'A05'/'A06' (the "A" prefix is PART of codigo, not a display-only
-- prefix on top of a plain '04'/'05'/'06' as this script originally assumed). Also A06's
-- real designacao is "Administração do INSS" (not "...da Segurança Social"), and it has
-- only 2 subprogramas (not 3 -- no "FRSS" subprograma under A06 exists). Only A07 is
-- genuinely missing. So this part now ONLY creates A07 + its subprograma/atividade, and
-- does NOT touch A04/A05/A06 at all (no lookup, no insert, no rename) -- avoids creating
-- duplicate Programa roots with the wrong codigo format ('04' alongside the real 'A04').
-- [VI] *** SỬA LẠI 2026-07-19 sau khi xem file export dữ liệu thật của khách
-- (Extract_Config_Dev_vs_Prod_2026-07-17 -> extract_result.xlsx, sheet config_dev_vs_prod) ***
-- Production ĐÃ CÓ SẴN đầy đủ A04/A05/A06 (gốc + toàn bộ subprograma/atividade), nhưng
-- codigo lưu là 'A04'/'A05'/'A06' (chữ "A" nằm TRONG codigo, không phải chỉ là hiển thị
-- thêm vào trước '04'/'05'/'06' như script bản gốc giả định). Tên thật của A06 là
-- "Administração do INSS" (không phải "...da Segurança Social"), và chỉ có 2 subprograma
-- (không có nhánh "FRSS"). Chỉ A07 là thật sự chưa có. Nên phần này giờ CHỈ tạo A07 +
-- subprograma/atividade của nó, KHÔNG động vào A04/A05/A06 (không tra cứu, không tạo,
-- không đổi tên) -- tránh tạo trùng Programa gốc với sai định dạng codigo ('04' cạnh 'A04' thật).
DECLARE @nextId INT;
DECLARE @a07 INT, @a0701 INT;

-- [EN] AGRUPAMENTOCONFIG.id is an IDENTITY column on the production environment
-- (but NOT on every environment), so explicit id values must be allowed while
-- inserting the 3 new A07 rows below, then turned back off right after. The check
-- below detects it at runtime: SET IDENTITY_INSERT ON errors out on a table with
-- no identity column, so it must NOT be issued unconditionally.
-- [VI] Cột id của AGRUPAMENTOCONFIG là IDENTITY trên môi trường production (nhưng
-- KHÔNG phải mọi môi trường), nên phải cho phép chèn id tường minh khi tạo 3 dòng
-- A07 bên dưới, rồi tắt lại ngay sau đó. Đoạn kiểm tra dưới đây tự phát hiện lúc
-- chạy: SET IDENTITY_INSERT ON sẽ lỗi trên bảng không có cột identity, nên KHÔNG
-- được chạy vô điều kiện.
DECLARE @acIdentity BIT =
    CONVERT(BIT, ISNULL(COLUMNPROPERTY(OBJECT_ID('dbo.AGRUPAMENTOCONFIG'), 'id', 'IsIdentity'), 0));

-- [EN] NOTE: this must NOT be wrapped in EXEC()/dynamic SQL -- SET IDENTITY_INSERT
-- is reverted when the dynamic-SQL scope ends, so the INSERTs below would still
-- fail with Msg 544. A plain IF is fine: the statement is only executed when the
-- column really is an identity column.
-- [VI] LƯU Ý: không được bọc trong EXEC()/dynamic SQL -- SET IDENTITY_INSERT bị
-- trả lại trạng thái cũ khi kết thúc scope dynamic SQL, nên các lệnh INSERT bên
-- dưới vẫn lỗi Msg 544. Dùng IF thường là đúng: lệnh chỉ chạy khi cột thật sự là
-- identity.
IF @acIdentity = 1
    SET IDENTITY_INSERT dbo.AGRUPAMENTOCONFIG ON;

SELECT @a07 = id FROM AGRUPAMENTOCONFIG WHERE reltipoDeContaOrcamentoConfig_fk=@reltipoActidade AND codigo='A07' AND parent_fk IS NULL AND indActivo=1;
IF @a07 IS NULL
BEGIN
    SET @nextId = (SELECT ISNULL(MAX(id),0)+1 FROM AGRUPAMENTOCONFIG);
    INSERT INTO AGRUPAMENTOCONFIG (id, codigo, designacao, reltipoDeContaOrcamentoConfig_fk, parent_fk, indActivo, utilizadorCriacao, dataCriacao)
    VALUES (@nextId, 'A07', 'Regime Contributivo de Capitalização', @reltipoActidade, NULL, 1, 1, GETDATE());
    SET @a07 = @nextId;
END
-- [EN] NOTE: the A07 label follows the client's own AD form ("DIC"); still
-- pending client confirmation about a possible numbering conflict with "A08"
-- (see [[open-questions-for-client]] item 3).
-- [VI] GHI CHÚ: designação A07 theo form AD của khách ("DIC"); vẫn chờ khách
-- xác nhận về khả năng xung đột đánh số với "A08" (xem [[open-questions-for-client]] mục 3).

SELECT @a0701 = id FROM AGRUPAMENTOCONFIG WHERE parent_fk=@a07 AND codigo='01' AND indActivo=1;
IF @a0701 IS NULL
BEGIN
    SET @nextId = (SELECT ISNULL(MAX(id),0)+1 FROM AGRUPAMENTOCONFIG);
    INSERT INTO AGRUPAMENTOCONFIG (id, codigo, designacao, reltipoDeContaOrcamentoConfig_fk, parent_fk, indActivo, utilizadorCriacao, dataCriacao)
    VALUES (@nextId, '01', 'Constituição e gestão do FRSS', @reltipoActidade, @a07, 1, 1, GETDATE());
    SET @a0701 = @nextId;
END

IF NOT EXISTS (SELECT 1 FROM AGRUPAMENTOCONFIG WHERE parent_fk=@a0701 AND codigo='01' AND indActivo=1)
    INSERT INTO AGRUPAMENTOCONFIG (id, codigo, designacao, reltipoDeContaOrcamentoConfig_fk, parent_fk, indActivo, utilizadorCriacao, dataCriacao)
    VALUES ((SELECT ISNULL(MAX(id),0)+1 FROM AGRUPAMENTOCONFIG), '01', 'Gestão do património do FRSS', @reltipoActidade, @a0701, 1, 1, GETDATE());

IF @acIdentity = 1
    SET IDENTITY_INSERT dbo.AGRUPAMENTOCONFIG OFF;

-- [EN] Verify the 3 A07 rows are REALLY there -- do not just print "OK". An earlier
-- version of this script printed a hardcoded OK here, which reported success even
-- when all 3 INSERTs had failed (Msg 544, IDENTITY_INSERT off) -- the errors scrolled
-- past and the run looked fine. This block re-reads the tree and tells the truth.
-- [VI] Kiểm tra 3 dòng A07 có THẬT SỰ tồn tại không -- không in "OK" cứng. Bản script
-- trước in sẵn chữ OK ở đây, nên vẫn báo thành công dù cả 3 lệnh INSERT đã lỗi (Msg 544,
-- IDENTITY_INSERT đang tắt) -- lỗi trôi qua và lần chạy trông như bình thường. Đoạn này
-- đọc lại cây và báo đúng sự thật.
DECLARE @chkA07 INT, @chkA0701 INT, @chkA070101 INT;

SELECT @chkA07 = id FROM AGRUPAMENTOCONFIG
 WHERE reltipoDeContaOrcamentoConfig_fk = @reltipoActidade AND codigo = 'A07' AND parent_fk IS NULL AND indActivo = 1;
SELECT @chkA0701 = id FROM AGRUPAMENTOCONFIG WHERE parent_fk = @chkA07 AND codigo = '01' AND indActivo = 1;
SELECT @chkA070101 = id FROM AGRUPAMENTOCONFIG WHERE parent_fk = @chkA0701 AND codigo = '01' AND indActivo = 1;

IF @chkA07 IS NOT NULL AND @chkA0701 IS NOT NULL AND @chkA070101 IS NOT NULL
    PRINT 'A07 Programa/Subprograma/Atividade: OK -- all 3 rows verified present in the database.';
ELSE
BEGIN
    PRINT '*** A07 Programa/Subprograma/Atividade: NOT CREATED -- see the errors above (typically Msg 544 on AGRUPAMENTOCONFIG). Nothing else in this script depends on it, but the A07 / FRSS branch will be missing from Programa dropdowns and from reports until this part succeeds. ***';
END

-- ============================================================================
-- PART 3 / PHẦN 3 — CE crosswalk table -- DYNAMIC lookup by code/tree position
-- Bảng đối chiếu mã CE (crosswalk) -- TRA CỨU ĐỘNG THEO MÃ/VỊ TRÍ CÂY
-- ============================================================================
-- [EN] No hardcoded IDs -- each row is identified by a "path" (codes
-- concatenated from the root, e.g. "06.01.08") within the correct
-- Despesa/Receita bucket, the same way Part 2 looks up new IDs dynamically.
-- Safe to run on an environment other than dev, as long as the historical
-- Conta OGE code structure + the real CE tree already exist the same way
-- (this is shared historical data, not created by this session).
-- [VI] Không hardcode ID -- xác định mỗi dòng bằng "path" (chuỗi mã ghép từ gốc,
-- VD "06.01.08") trong đúng bucket Despesa/Receita, giống cách script tự tìm ID
-- mới ở Phần 2. An toàn chạy trên môi trường khác dev, miễn là cấu trúc mã
-- (codigo) của Conta OGE gốc + cây CE thật đã tồn tại giống nhau (dữ liệu lịch
-- sử dùng chung, không phải do phiên làm việc này tạo).
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM RELAGRUPAMENTOCONFIGCLASSIFICACAOECONOMICA WHERE id = 1)
BEGIN
    DECLARE @tipoContaDespesa INT = (SELECT idDominio FROM DOMINIO WHERE dominio='TIPOCONTA' AND descricao='Despesa');
    DECLARE @tipoContaReceita INT = (SELECT idDominio FROM DOMINIO WHERE dominio='TIPOCONTA' AND descricao='Receita');

    ;WITH DespesaTree AS (
        SELECT a.id, CAST(a.codigo AS VARCHAR(50)) AS path
        FROM AGRUPAMENTOCONFIG a
        JOIN RELTIPODECONTAORCAMENTOCONFIG r ON a.reltipoDeContaOrcamentoConfig_fk = r.id
        WHERE r.tipoConta_fk = @tipoContaDespesa AND a.parent_fk IS NULL AND a.indActivo = 1
        UNION ALL
        SELECT c.id, CAST(t.path + '.' + c.codigo AS VARCHAR(50))
        FROM AGRUPAMENTOCONFIG c JOIN DespesaTree t ON c.parent_fk = t.id WHERE c.indActivo = 1
    ),
    ReceitaTree AS (
        SELECT a.id, CAST(a.codigo AS VARCHAR(50)) AS path
        FROM AGRUPAMENTOCONFIG a
        JOIN RELTIPODECONTAORCAMENTOCONFIG r ON a.reltipoDeContaOrcamentoConfig_fk = r.id
        WHERE r.tipoConta_fk = @tipoContaReceita AND a.parent_fk IS NULL AND a.indActivo = 1
        UNION ALL
        SELECT c.id, CAST(t.path + '.' + c.codigo AS VARCHAR(50))
        FROM AGRUPAMENTOCONFIG c JOIN ReceitaTree t ON c.parent_fk = t.id WHERE c.indActivo = 1
    ),
    DespesaPares (origemPath, alvoPath, confianca) AS (
        SELECT * FROM (VALUES
            ('01',       '51',    'Alta'),
            ('02',       '52',    'Alta'),
            ('03.01',    '53.06', 'Alta'),
            ('03.02',    '52.28', 'Media'),
            ('04',       '53',    'Alta'),
            ('06',       '52.30', 'Media'),
            ('07.01',    '54',    'Alta'),
            ('07.02',    '55',    'Alta'),
            ('08',       '53.11', 'Alta'),
            ('09',       '53.12', 'Alta'),
            ('10',       '53.13', 'Alta'),
            ('11',       '55',    'Baixa'),
            ('12',       '56',    'Alta'),
            ('06.01.08', '53',    'Media'),
            ('06.01.09', '53',    'Media')
        ) x(origemPath, alvoPath, confianca)
    ),
    ReceitaPares (origemPath, alvoPath, confianca) AS (
        SELECT * FROM (VALUES
            ('02',             '41', 'Media'),
            ('03',             '41', 'Alta'),
            ('04',             '42', 'Alta'),
            ('05',             '43', 'Alta'),
            ('06',             '44', 'Alta'),
            ('07',             '45', 'Alta'),
            ('08',             '43', 'Baixa'),
            ('09',             '45', 'Media'),
            ('10',             '44', 'Media'),
            ('11',             '46', 'Alta'),
            ('12',             '46', 'Alta'),
            ('13',             '45', 'Baixa'),
            ('15',             '47', 'Alta'),
            ('16',             '48', 'Alta'),
            ('04.01',          '43', 'Media'),
            ('04.01.01',       '43', 'Media'),
            ('08.02',          '44', 'Baixa'),
            ('10.02.02.01',    '48', 'Baixa')
        ) x(origemPath, alvoPath, confianca)
    ),
    Combinado AS (
        SELECT o.id AS origemId, a.id AS alvoId, p.confianca
        FROM DespesaPares p
        JOIN DespesaTree o ON o.path = p.origemPath
        JOIN DespesaTree a ON a.path = p.alvoPath
        UNION ALL
        SELECT o.id, a.id, p.confianca
        FROM ReceitaPares p
        JOIN ReceitaTree o ON o.path = p.origemPath
        JOIN ReceitaTree a ON a.path = p.alvoPath
    )
    INSERT INTO dbo.RELAGRUPAMENTOCONFIGCLASSIFICACAOECONOMICA
    (id, agrupamentoConfigOrigem_fk, agrupamentoConfigCE_fk, confianca, indActivo, utilizadorCriacao, dataCriacao)
    SELECT
        ROW_NUMBER() OVER (ORDER BY origemId) + ISNULL((SELECT MAX(id) FROM RELAGRUPAMENTOCONFIGCLASSIFICACAOECONOMICA), 0),
        origemId, alvoId, confianca, 1, 1, GETDATE()
    FROM Combinado;

    PRINT CONCAT('CE crosswalk table: ', @@ROWCOUNT, ' row(s) added (up to 33 possible -- on dev, expect close to 33; on production/staging, 0 or near-0 is EXPECTED because the production tree is already coded directly with the real CE codes, see the note below).');
END
ELSE
    PRINT 'CE crosswalk table: data already present (id=1 exists), skipping to avoid duplication.';

-- [EN] Note: this crosswalk maps an OLD/legacy Conta OGE numbering (root codes '01'-'16')
-- to real CE codes -- confirmed (2026-07-19, via the client's production data export) that
-- this legacy numbering does NOT exist on production (production already uses the real CE
-- root codes 401-409/501-506 directly), so 0 rows added there is correct, not a bug. If you
-- want to double check what root codes actually exist on an environment, run:
--   SELECT a.codigo, a.designacao FROM AGRUPAMENTOCONFIG a
--   JOIN RELTIPODECONTAORCAMENTOCONFIG r ON a.reltipoDeContaOrcamentoConfig_fk = r.id
--   JOIN DOMINIO d ON r.tipoConta_fk = d.idDominio
--   WHERE d.descricao IN ('Despesa','Receita') AND a.parent_fk IS NULL AND a.indActivo = 1;
-- [VI] Ghi chú: bảng đối chiếu này map cây mã Conta OGE CŨ/legacy (mã gốc '01'-'16') sang mã
-- CE thật -- đã xác nhận (2026-07-19, qua file export dữ liệu production của khách) rằng cây
-- mã cũ này KHÔNG tồn tại trên production (production đã dùng thẳng mã CE gốc thật 401-409/
-- 501-506), nên 0 dòng ở đó là đúng, không phải lỗi. Muốn tự kiểm tra mã gốc thật đang có trên
-- 1 môi trường, chạy:
--   SELECT a.codigo, a.designacao FROM AGRUPAMENTOCONFIG a
--   JOIN RELTIPODECONTAORCAMENTOCONFIG r ON a.reltipoDeContaOrcamentoConfig_fk = r.id
--   JOIN DOMINIO d ON r.tipoConta_fk = d.idDominio
--   WHERE d.descricao IN ('Despesa','Receita') AND a.parent_fk IS NULL AND a.indActivo = 1;

-- ============================================================================
-- FINAL VERIFICATION / KIỂM TRA CUỐI
-- ----------------------------------------------------------------------------
-- [EN] Re-reads the database and reports what is actually there, so "DONE" can
-- never be mistaken for success when an individual statement failed above.
-- [VI] Đọc lại database và báo cáo thực tế đang có gì, để chữ "DONE" không bao
-- giờ bị hiểu nhầm là thành công khi có lệnh nào đó ở trên đã lỗi.
-- ============================================================================
DECLARE @okTable BIT = CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RELAGRUPAMENTOCONFIGCLASSIFICACAOECONOMICA') THEN 1 ELSE 0 END;
DECLARE @okA07   BIT = CASE WHEN @chkA07 IS NOT NULL AND @chkA0701 IS NOT NULL AND @chkA070101 IS NOT NULL THEN 1 ELSE 0 END;

PRINT '--------------------------------------------------------------------';
PRINT CONCAT('  1. Table RELAGRUPAMENTOCONFIGCLASSIFICACAOECONOMICA : ', CASE WHEN @okTable = 1 THEN 'PRESENT' ELSE 'MISSING' END);
PRINT CONCAT('  2. A07 Programa/Subprograma/Atividade (3 rows)      : ', CASE WHEN @okA07 = 1 THEN 'PRESENT' ELSE 'MISSING' END);
PRINT '--------------------------------------------------------------------';

IF @okTable = 1 AND @okA07 = 1
    PRINT 'DONE -- all checks passed.';
ELSE
    PRINT '*** FINISHED WITH PROBLEMS -- one or more items above are MISSING. Scroll up for the error messages, fix the cause, then simply run this script again (it is safe to re-run: it only creates what is missing). ***';
