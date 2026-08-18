-- ============================================================================
-- LOCAL-ONLY: full import of the real January 2026 "AD" (Autorização da
-- Despesa) sheet from the client's `INSS_2026_janeiro _original.xlsx`
-- (fin and contrib doc/3. Customer Send/) into COMPONENTEORCAMENTOVALOR, as
-- an approved 2026 budget batch. All 68 real AD rows (Valor Máximo), matched
-- 1:1 by exact designacao text against the local AGRUPAMENTOCONFIG tree
-- (fuzzy-match confidence 1.00 on every row -- see generation notes below).
--
-- NOT idempotent -- creates a NEW ComponenteOrcamentoRegisto (Aprovado=1)
-- each run. Run once against a fresh local Docker DB
-- (TimorINSSModuloContribuicoes_ProdDev). Complements
-- `seed_sample_data_local.sql` (which seeds a small representative set of
-- Despesa/Receita EXECUTION scenarios) -- this file seeds the BUDGET
-- (Cabimento-ceiling / AD) side only, at full real-world scale (68 rows,
-- $131,556,291 total).
--
-- How this was generated (for reproducing with a different month's AD sheet):
--   1. Read the `AD` sheet with openpyxl (data_only=True), columns:
--      C=Regime, E=Classificação económica (code + designação text),
--      G=Objetivo, I=Valor Máximo. Skip rows where Regime is blank/#N/A
--      (template filler rows past the real data).
--   2. Dump the full local AGRUPAMENTOCONFIG tree as id -> "root > ... > leaf"
--      designacao path (recursive CTE, see db_seed_reference_local.md memory).
--   3. For each AD row, take the last " - "-separated segment of the
--      Classificação económica text (the most specific designação) and
--      fuzzy-match it (difflib.SequenceMatcher) against every local leaf
--      designacao. All 68 January rows matched at score 1.00 (exact text
--      match) -- the local AGRUPAMENTOCONFIG designacao strings are a very
--      close copy of the real client chart, just with different `codigo`
--      numbering, so exact-text matching works reliably here.
--   4. CentroCusto assigned by keyword in the Regime column: "não
--      Contributivo"->6, "Contributivo"->5, "Administra..."->7.
-- ============================================================================

SET NOCOUNT ON;
DECLARE @admin INT = 23; DECLARE @inst INT = 1; DECLARE @orcCfg2026 INT = 2008; DECLARE @tipoDespesa INT = 71; DECLARE @depto INT = 2;
INSERT INTO PROCESSOATIVO (processoConfig_fk, arquivado, indActivo, utilizadorCriacao, dataCriacao, ipv6, numeroProcesso) VALUES (1, 0, 1, @admin, GETDATE(), '127.0.0.1', 'SEED-AD-FULL/2026');
DECLARE @procAtivoAD INT = SCOPE_IDENTITY();
INSERT INTO TAREFAATIVO (processoAtivo_fk, tarefaconfig_fk, indActivo, utilizadorCriacao, dataCriacao, ipv6) VALUES (@procAtivoAD, 2, 1, @admin, GETDATE(), '127.0.0.1');
DECLARE @tarefaAD INT = SCOPE_IDENTITY();
INSERT INTO COMPONENTEORCAMENTO_REGISTO (dataInicio, dataFim, indActivo, utilizadorCriacao, dataCriacao, tarefaActivo_fk, aprovado, orcamentoConfigFK) VALUES ('2026-01-01','2026-12-31',1,@admin,GETDATE(),@tarefaAD,1,@orcCfg2026);
DECLARE @orcRegistoFull INT = SCOPE_IDENTITY();
PRINT 'Registo AD-full=' + CAST(@orcRegistoFull AS VARCHAR);
DECLARE @adv TABLE (agrup INT, cc INT, valor DECIMAL(18,2), obj NVARCHAR(200));
INSERT INTO @adv (agrup, cc, valor, obj) VALUES
(430, 6, 8083440, N'PS-Invalidez 2026'),
(431, 6, 95646560, N'PS-Vellise 2026'),
(432, 6, 156000, N'PS-Komplementu Invalidez 2026'),
(433, 6, 364000, N'PS-Komplementu Vellise 2026'),
(436, 5, 188400, N'RTSS-Invalidez 2026'),
(437, 5, 11400000, N'RTSS-Vellise 2026'),
(438, 5, 4800000, N'RTSS-Sobrevivênsia 2026'),
(439, 5, 68100, N'RGSS-Invalidez 2026'),
(440, 5, 788402, N'RGSS-Vellise 2026'),
(441, 5, 977000, N'RGSS-Sobrevivênsia 2026'),
(444, 5, 1000000, N'Subsidiu Maternidade 2026'),
(445, 5, 50000, N'Subsidiu Paternidade 2026'),
(446, 5, 1000, N'Subsidiu Adosaun 2026'),
(174, 5, 550000, N'Subsidiu Mate 2026'),
(449, 5, 1300000, N'Subsidiu Moras 2026'),
(261, 7, 234000, N'Presidente & DE Jan-Dez 2026'),
(262, 7, 1155228, N'Kargu Chefia INSS Jan-Dez 2026'),
(263, 7, 211020, N'Funcionario Permanente Jan-Dez 2026'),
(264, 7, 2621504, N'Kontratadus no Assessores Nasionais Jan-Dez 2026 INSS'),
(265, 7, 342000, N'Assessores Internasionais Jan-Dez 2026 INSS'),
(270, 7, 122518, N'Décimo terceiro mês 2026 INSS'),
(274, 7, 38700, N'Subsidiu Transporte Jan-Dez 2026'),
(282, 7, 102404, N'EE-6% INSS 2026'),
(261, 7, 28800, N'Suplementu Fiskal Uniku Jan-Dez 2026'),
(262, 7, 94248, N'Kargu Chefia FRSS Jan-Dez 2026'),
(264, 7, 163128, N'Kontratadus no Assessores Nasionais Jan-Dez 2026 FRSS'),
(270, 7, 5418, N'Décimo terceiro mês 2026 FRSS'),
(274, 7, 3600, N'Subsidiu Transporte FRSS Jan-Dez 2026'),
(281, 7, 8200, N'Senhas Konsellu Adm & Konsellu Fiskal FRSS'),
(265, 7, 0, N'Assessores Internasionais Jan-Dez 2026 FRSS'),
(282, 7, 4226, N'EE-6% FRSS 2026'),
(281, 7, 4900, N'Senhas Konsellu Adm INSS'),
(378, 7, 19112, N'Ekipa Gab. Presidente INSS'),
(375, 7, 28667, N'Gab. Presidente'),
(402, 7, 8127, N'Aluga Kareta'),
(402, 7, 16250, N'Aluga Kareta'),
(354, 7, 7800, N'Fornese Be''e'),
(377, 7, 72000, N'VL-Funcionarius'),
(377, 7, 48000, N'VL-Gab. Presidente INSS'),
(362, 7, 128140, N'Insternet nst'),
(368, 7, 37000, N'Gab. Presidente'),
(391, 7, 6000, N'Fee Banku'),
(389, 7, 20300, N'Publicidade, Copia+Emprime'),
(322, 7, 30000, N'Outros'),
(303, 7, 40000, N'Material eskritoria'),
(202, 7, 400, N'Manutensaun gerador'),
(400, 7, 188000, N'Aluga Edifisiu INSS-Back Up Servidor'),
(369, 7, 17500, N'Catering'),
(149, 7, 6820, N'Publikasaun RTTL, media'),
(387, 7, 17200, N'Kuota annual ISSA'),
(384, 7, 50000, N'Manutenção SISS'),
(355, 7, 55800, N'Pulsa EDTL'),
(309, 7, 300, N'Oli gerador'),
(327, 7, 21900, N'Manutensaun Kareta no Motor'),
(367, 7, 9944, N'Gab Presidente (2.048) + DE (520) + DARH (464) + D.Financeiro (256) + Prestações (3.072) + D. Serviços Desconcentrados (3.072) + Aprovisionamento (512)'),
(361, 7, 30000, N'Pulsa Telemoveis INSS'),
(306, 7, 37450, N'Kombustivel Kareta no Motor'),
(296, 7, 7000, N''),
(330, 7, 2000, N'Manutensaun Eletrisidade'),
(332, 7, 7200, N'Manutensaun AC'),
(305, 7, 2250, N'Kombustivel ba gerador'),
(298, 7, 3000, N''),
(324, 7, 27000, N'Servisu limpeza-kompania mak fornese ==== fulan 12 x $2,250,00 (pakote 1)  = $27,000,00 '),
(411, 7, 12000, N'Aluga Outros: Tenda, Kadeira nst'),
(334, 7, 10000, N'Manutensaun Ekipamentu Informatika'),
(375, 7, 17600, N'Formasaun '),
(382, 7, 20000, N'Formadores da Academia da Segurança Social (50 mil) + Formação do DARH na Malasia (17 mil)'),
(378, 7, 38735, N'Formasaun ');

INSERT INTO COMPONENTEORCAMENTOVALOR (componenteOrcamentoRegisto_fk, departamento_fk, centroCusto_fk, agrupamento_fk, valor, indActivo, utilizadorCriacao, dataCriacao, tipoConta_fk, InstitutionId)
SELECT @orcRegistoFull, @depto, cc, agrup, valor, 1, @admin, GETDATE(), @tipoDespesa, @inst
FROM @adv;

PRINT 'AD FULL IMPORT COMPLETO: ' + CAST(@@ROWCOUNT AS VARCHAR) + ' linhas';
