-- ============================================================================
-- LOCAL-ONLY sample data seed for TimorINSSModuloContribuicoes_ProdDev
-- (Docker Compose service `inss-db`, connection: sa / INSSdev@2024! / port 1433)
--
-- Purpose: creates a small, representative set of Despesa + Receita records
-- (with an already-Aprovado 2026 Orcamento) covering different payment states
-- (paid in full, paid PARTIALLY, not yet paid) across a few different
-- Classificacao Economica categories, so reports/screens have something real
-- to render locally without needing the full real client Excel ledger import.
--
-- NOT idempotent -- running this twice creates duplicate rows. Intended to be
-- run ONCE against a fresh/empty local Docker DB. Never run against staging
-- or production. See memory `db_seed_reference_local.md` for the schema
-- gotchas (FK-to-DOMINIO fields, ENTIDADEEMPREGADORA vs DESTINATARIO, etc.)
-- discovered while writing this.
--
-- Creates:
--   - 1 Orcamento aprovado for 2026 (OrcamentoConfigFk=2008), with budget
--     lines for AgrupamentoConfig 69,71,93,105,164 (despesa) and 20,21 (receita)
--   - 4 new Destinatario (clean test payees)
--   - 5 Despesa scenarios (Cabimento->Compromisso->Pagamento chain):
--       1. Salarios pessoais permanentes -- paid in full ($15,000)
--       2. Salarios pessoais estrangeiros -- PARTIAL payment ($2,000 committed,
--          only $1,000 actually paid by bank -- the "committed vs actually
--          paid" discrepancy scenario)
--       3. Aquisicao de bens -- NOT yet paid (Cabimento+Compromisso only)
--       4. Aquisicao de servicos -- paid in full ($4,500, to a supplier)
--       5. Transferencias para Familias -- paid in full ($80,000, social
--          benefit transfer)
--   - 2 Receita scenarios (ComponenteReceitaRegisto + Guiapagamento):
--       1. Regime contributivo -- guia paga (indPago=Guia Paga)
--       2. Regimes Complementares -- guia gerada, ainda NAO paga (divida)
-- ============================================================================

SET NOCOUNT ON;
DECLARE @admin INT = 23;
DECLARE @inst INT = 1; -- INSS
DECLARE @orcCfg2026 INT = 2008;
DECLARE @tipoDespesa INT = 71; -- DOMINIO(dominio='TIPOCONTA', descricao='Despesa').idDominio
DECLARE @tipoReceita INT = 70; -- DOMINIO(dominio='TIPOCONTA', descricao='Receita').idDominio
DECLARE @estadoOrdemEmitida INT = 94; -- DOMINIO(dominio='ESTADOPAGAMENTO', valor=2 'Ordem de pagamento emitida').idDominio -- REQUIRED for the row to show up in Conciliacao de Movimentos (Despesa)
DECLARE @cabimentoEstado INT = 89; -- DOMINIO(dominio='ESTADODESPESA', valor=3 'C').idDominio
DECLARE @centroCusto INT = 5; -- Contributivo
DECLARE @depto INT = 2; -- Departamento Financeiro

-- ===== 1. Processo/Tarefa Ativo para aprovacao do orcamento 2026 =====
INSERT INTO PROCESSOATIVO (processoConfig_fk, arquivado, indActivo, utilizadorCriacao, dataCriacao, ipv6, numeroProcesso)
VALUES (1, 0, 1, @admin, GETDATE(), '127.0.0.1', 'SEED-AO/2026'); -- 1 = AO Aprovar o Orcamento
DECLARE @procAtivoOrc INT = SCOPE_IDENTITY();

INSERT INTO TAREFAATIVO (processoAtivo_fk, tarefaconfig_fk, indActivo, utilizadorCriacao, dataCriacao, ipv6)
VALUES (@procAtivoOrc, 2, 1, @admin, GETDATE(), '127.0.0.1'); -- 2 = AO03 Aprovar Orcamento
DECLARE @tarefaOrc INT = SCOPE_IDENTITY();

-- ===== 2. Orcamento aprovado 2026 =====
INSERT INTO COMPONENTEORCAMENTO_REGISTO (dataInicio, dataFim, indActivo, utilizadorCriacao, dataCriacao, tarefaActivo_fk, aprovado, orcamentoConfigFK)
VALUES ('2026-01-01', '2026-12-31', 1, @admin, GETDATE(), @tarefaOrc, 1, @orcCfg2026);
DECLARE @orcRegisto INT = SCOPE_IDENTITY();

-- Verbas por rubrica (AgrupamentoConfig) usadas nos cenarios abaixo.
-- 69/71 = Salarios pessoais permanentes/estrangeiros (sob 67 DESPESAS COM PESSOAL)
-- 93/105 = Aquisicao de bens/servicos (sob 92 AQUISICAO DE BENS E SERVICOS)
-- 164 = Para Familias (sob 161 TRANSFERENCIAS CORRENTES)
-- 20/21 = Regime contributivo / Regimes Complementares (sob 3 CONTRIBUICOES para SEGURANCA SOCIAL)
-- All of these have an active crosswalk row in RELAGRUPAMENTOCONFIGCLASSIFICACAOECONOMICA
-- (checked at their respective ROOT: 67->258, 92->284, 161->416, 3->554), so they
-- resolve correctly in the CE_OSS_Global report.
DECLARE @vals TABLE (agrup INT, valor DECIMAL(18,2));
INSERT INTO @vals VALUES (69,200000),(71,50000),(93,30000),(105,40000),(164,500000),(20,2000000),(21,300000);

INSERT INTO COMPONENTEORCAMENTOVALOR (componenteOrcamentoRegisto_fk, departamento_fk, centroCusto_fk, agrupamento_fk, valor, indActivo, utilizadorCriacao, dataCriacao, tipoConta_fk, InstitutionId)
SELECT @orcRegisto, @depto, @centroCusto, agrup, valor, 1, @admin, GETDATE(),
       CASE WHEN agrup IN (20,21) THEN @tipoReceita ELSE @tipoDespesa END, @inst
FROM @vals;

PRINT 'Orcamento OK: registo=' + CAST(@orcRegisto AS VARCHAR);

-- ===== 3. Destinatarios novos (limpos -- existing DESTINATARIO rows in this DB are messy test junk) =====
INSERT INTO DESTINATARIO (nome, NISS, indActivo, utilizadorCriacao, dataCriacao, morada)
VALUES ('Domingos da Costa', '100112233', 1, @admin, GETDATE(), 'Dili, Timor-Leste');
DECLARE @destFuncNacional INT = SCOPE_IDENTITY();

INSERT INTO DESTINATARIO (nome, NISS, indActivo, utilizadorCriacao, dataCriacao, morada)
VALUES ('John Smith', '100223344', 1, @admin, GETDATE(), 'Dili, Timor-Leste');
DECLARE @destFuncEstrangeiro INT = SCOPE_IDENTITY();

INSERT INTO DESTINATARIO (nome, indActivo, utilizadorCriacao, dataCriacao, morada)
VALUES ('Timor Telecom, Lda', 1, @admin, GETDATE(), 'Dili, Timor-Leste');
DECLARE @destFornecedor INT = SCOPE_IDENTITY();

INSERT INTO DESTINATARIO (nome, NISS, indActivo, utilizadorCriacao, dataCriacao, morada)
VALUES ('Maria dos Santos', '100445566', 1, @admin, GETDATE(), 'Baucau, Timor-Leste');
DECLARE @destBeneficiario INT = SCOPE_IDENTITY();

-- ===== 4. Processo/Tarefa Ativo compartilhado para os registos de Despesa (RD) =====
INSERT INTO PROCESSOATIVO (processoConfig_fk, arquivado, indActivo, utilizadorCriacao, dataCriacao, ipv6, numeroProcesso)
VALUES (3, 0, 1, @admin, GETDATE(), '127.0.0.1', 'SEED-RD/2026'); -- 3 = RD Registo de Despesas
DECLARE @procAtivoRD INT = SCOPE_IDENTITY();

INSERT INTO TAREFAATIVO (processoAtivo_fk, tarefaconfig_fk, indActivo, utilizadorCriacao, dataCriacao, ipv6)
VALUES (@procAtivoRD, 7, 1, @admin, GETDATE(), '127.0.0.1'); -- 7 = RD05 Liquidacao da despesa
DECLARE @tarefaRD INT = SCOPE_IDENTITY();

-- ===== 5. Cenario Despesa 1: Salarios pessoais permanentes -- pago integralmente =====
INSERT INTO COMPONENTEDESPESA_REGISTO (tarefaActivo_fk, departamento_fk, centroCusto_fk, tipoConta_fk, agrupamentoConfig_fk, descricao, valor, utilizadorCriacao, dataCriacao, indActivo, estado, componenteOrcamentoRegistoFk, InstitutionId)
VALUES (@tarefaRD, @depto, @centroCusto, @tipoDespesa, 69, 'Salarios pessoais permanentes - Julho 2026', 15000.00, @admin, GETDATE(), 1, @cabimentoEstado, @orcRegisto, @inst);
DECLARE @cab1 INT = SCOPE_IDENTITY();

INSERT INTO COMPROMISSO (tarefaAtivo_fk, componenteDespesaRegisto_fk, nome, valor, data, utilizadorCriacao, dataCriacao, IndActivo)
VALUES (@tarefaRD, @cab1, 'Compromisso - Salarios pessoais permanentes', 15000.00, '2026-01-10', @admin, GETDATE(), 1);
DECLARE @comp1 INT = SCOPE_IDENTITY();

INSERT INTO PAGAMENTOSEXECUTADOS (destinatario_fk, numeroPagamento, estado, valorExecutado, indActivo, utilizadorCriacao, dataCriacao, processoAtivo_fk, codigoContaDebito_fk, codigoContaCredito_fk, DataObrigacao, DataExecucao, compromisso_fk, bankCode)
VALUES (@destFuncNacional, 'PGT-SAL-001/2026', @estadoOrdemEmitida, 15000.00, 1, @admin, GETDATE(), @procAtivoRD, 352, 10, '2026-01-15', '2026-01-16', @comp1, 'BNU');

-- ===== 6. Cenario Despesa 2: Salarios pessoais estrangeiros -- PAGAMENTO PARCIAL (2k comprometido, banco so pagou 1k) =====
INSERT INTO COMPONENTEDESPESA_REGISTO (tarefaActivo_fk, departamento_fk, centroCusto_fk, tipoConta_fk, agrupamentoConfig_fk, descricao, valor, utilizadorCriacao, dataCriacao, indActivo, estado, componenteOrcamentoRegistoFk, InstitutionId)
VALUES (@tarefaRD, @depto, @centroCusto, @tipoDespesa, 71, 'Salarios pessoais estrangeiros - Fevereiro 2026', 2000.00, @admin, GETDATE(), 1, @cabimentoEstado, @orcRegisto, @inst);
DECLARE @cab2 INT = SCOPE_IDENTITY();

INSERT INTO COMPROMISSO (tarefaAtivo_fk, componenteDespesaRegisto_fk, nome, valor, data, utilizadorCriacao, dataCriacao, IndActivo)
VALUES (@tarefaRD, @cab2, 'Compromisso - Salarios pessoais estrangeiros', 2000.00, '2026-02-05', @admin, GETDATE(), 1);
DECLARE @comp2 INT = SCOPE_IDENTITY();

INSERT INTO PAGAMENTOSEXECUTADOS (destinatario_fk, numeroPagamento, estado, valorExecutado, indActivo, utilizadorCriacao, dataCriacao, processoAtivo_fk, codigoContaDebito_fk, codigoContaCredito_fk, DataObrigacao, DataExecucao, compromisso_fk, bankCode)
VALUES (@destFuncEstrangeiro, 'PGT-SAL-002/2026', @estadoOrdemEmitida, 1000.00, 1, @admin, GETDATE(), @procAtivoRD, 383, 11, '2026-02-10', '2026-02-12', @comp2, 'MANDIRI');

-- ===== 7. Cenario Despesa 3: Aquisicao de bens -- SEM pagamento ainda (pendente) =====
INSERT INTO COMPONENTEDESPESA_REGISTO (tarefaActivo_fk, departamento_fk, centroCusto_fk, tipoConta_fk, agrupamentoConfig_fk, descricao, valor, utilizadorCriacao, dataCriacao, indActivo, estado, componenteOrcamentoRegistoFk, InstitutionId)
VALUES (@tarefaRD, @depto, @centroCusto, @tipoDespesa, 93, 'Aquisicao de material de escritorio', 3000.00, @admin, GETDATE(), 1, @cabimentoEstado, @orcRegisto, @inst);
DECLARE @cab3 INT = SCOPE_IDENTITY();

INSERT INTO COMPROMISSO (tarefaAtivo_fk, componenteDespesaRegisto_fk, nome, valor, data, utilizadorCriacao, dataCriacao, IndActivo)
VALUES (@tarefaRD, @cab3, 'Compromisso - Aquisicao de material de escritorio', 3000.00, '2026-03-01', @admin, GETDATE(), 1);
DECLARE @comp3 INT = SCOPE_IDENTITY();
-- sem PAGAMENTOSEXECUTADOS -- ainda pendente

-- ===== 8. Cenario Despesa 4: Aquisicao de servicos -- pago integralmente (fornecedor) =====
INSERT INTO COMPONENTEDESPESA_REGISTO (tarefaActivo_fk, departamento_fk, centroCusto_fk, tipoConta_fk, agrupamentoConfig_fk, descricao, valor, utilizadorCriacao, dataCriacao, indActivo, estado, componenteOrcamentoRegistoFk, InstitutionId)
VALUES (@tarefaRD, @depto, @centroCusto, @tipoDespesa, 105, 'Servicos de telecomunicacoes - Marco 2026', 4500.00, @admin, GETDATE(), 1, @cabimentoEstado, @orcRegisto, @inst);
DECLARE @cab4 INT = SCOPE_IDENTITY();

INSERT INTO COMPROMISSO (tarefaAtivo_fk, componenteDespesaRegisto_fk, nome, valor, data, utilizadorCriacao, dataCriacao, IndActivo)
VALUES (@tarefaRD, @cab4, 'Compromisso - Servicos de telecomunicacoes', 4500.00, '2026-03-01', @admin, GETDATE(), 1);
DECLARE @comp4 INT = SCOPE_IDENTITY();

INSERT INTO PAGAMENTOSEXECUTADOS (destinatario_fk, numeroPagamento, estado, valorExecutado, indActivo, utilizadorCriacao, dataCriacao, processoAtivo_fk, codigoContaDebito_fk, codigoContaCredito_fk, DataObrigacao, DataExecucao, compromisso_fk, bankCode)
VALUES (@destFornecedor, 'PGT-SRV-001/2026', @estadoOrdemEmitida, 4500.00, 1, @admin, GETDATE(), @procAtivoRD, 91, 10, '2026-03-05', '2026-03-06', @comp4, 'BNU');

-- ===== 9. Cenario Despesa 5: Transferencias para Familias -- pago integralmente (beneficio social) =====
INSERT INTO COMPONENTEDESPESA_REGISTO (tarefaActivo_fk, departamento_fk, centroCusto_fk, tipoConta_fk, agrupamentoConfig_fk, descricao, valor, utilizadorCriacao, dataCriacao, indActivo, estado, componenteOrcamentoRegistoFk, InstitutionId)
VALUES (@tarefaRD, @depto, @centroCusto, @tipoDespesa, 164, 'Pensao Social - Janeiro 2026', 80000.00, @admin, GETDATE(), 1, @cabimentoEstado, @orcRegisto, @inst);
DECLARE @cab5 INT = SCOPE_IDENTITY();

INSERT INTO COMPROMISSO (tarefaAtivo_fk, componenteDespesaRegisto_fk, nome, valor, data, utilizadorCriacao, dataCriacao, IndActivo)
VALUES (@tarefaRD, @cab5, 'Compromisso - Pensao Social Janeiro', 80000.00, '2026-01-05', @admin, GETDATE(), 1);
DECLARE @comp5 INT = SCOPE_IDENTITY();

INSERT INTO PAGAMENTOSEXECUTADOS (destinatario_fk, numeroPagamento, estado, valorExecutado, indActivo, utilizadorCriacao, dataCriacao, processoAtivo_fk, codigoContaDebito_fk, codigoContaCredito_fk, DataObrigacao, DataExecucao, compromisso_fk, bankCode)
VALUES (@destBeneficiario, 'PGT-TRF-001/2026', @estadoOrdemEmitida, 80000.00, 1, @admin, GETDATE(), @procAtivoRD, 79, 10, '2026-01-20', '2026-01-21', @comp5, 'BNU');

-- ===== 10. Processo/Tarefa Ativo para Receita (RR) =====
INSERT INTO PROCESSOATIVO (processoConfig_fk, arquivado, indActivo, utilizadorCriacao, dataCriacao, ipv6, numeroProcesso)
VALUES (4, 0, 1, @admin, GETDATE(), '127.0.0.1', 'SEED-RR/2026'); -- 4 = RR Registo de Receitas
DECLARE @procAtivoRR INT = SCOPE_IDENTITY();

INSERT INTO TAREFAATIVO (processoAtivo_fk, tarefaconfig_fk, indActivo, utilizadorCriacao, dataCriacao, ipv6)
VALUES (@procAtivoRR, 22, 1, @admin, GETDATE(), '127.0.0.1'); -- 22 = RR01 Registar Receitas Conciliadas
DECLARE @tarefaRR INT = SCOPE_IDENTITY();

-- ===== 11. Cenario Receita 1: Regime contributivo -- guia paga =====
-- guia_entidade_fk aponta para ENTIDADEEMPREGADORA (nao DESTINATARIO) -- reutiliza empregadores de teste ja existentes
DECLARE @entidadeA INT = 2132; -- Pixel Asia Production Dili Unipessoal, Lda
DECLARE @entidadeB INT = 2134; -- TTC Holding
-- indPago e FK para DOMINIO(dominio='INDPAGO'): 29=Guia Paga, 30=Guia Gerada, 31=Comprovativo em Validacao, 39/40=parcial
DECLARE @indPagoGuiaPaga INT = 29;
DECLARE @indPagoGuiaGerada INT = 30;

INSERT INTO COMPONENTERECEITA_REGISTO (tarefaActivo_fk, departamento_fk, centroCusto_fk, tipoConta_fk, codigoConta_fk, agrupamentoConfig_fk, descricao, valor, utilizadorCriacao, dataCriacao, indActivo, componenteOrcamentoRegistoFk, InstitutionId)
VALUES (@tarefaRR, @depto, @centroCusto, @tipoReceita, 1, 20, 'Contribuicoes Regime Contributivo - Julho 2026', 5000.00, @admin, GETDATE(), 1, @orcRegisto, @inst);

INSERT INTO GUIAPAGAMENTO (guia_entidade_fk, numDocumento, dtEmissao, descricao, valor, indPago, dataComprovPag, valorComprovPag, utilizadorCriacao, dataCriacao, dtValidade, tipoGuia, indActivo, mesAno, bankCode, paymentRef)
VALUES (@entidadeA, 'GP-001/2026', '2026-01-05', 'Guia de Pagamento - Contribuicoes Julho 2026', 5000.00, @indPagoGuiaPaga, '2026-01-10', 5000.00, @admin, GETDATE(), '2026-01-31', 1, 1, '2026-01-01', 'BNU', 'GP-001/2026');

-- ===== 12. Cenario Receita 2: Regimes Complementares -- guia AINDA NAO paga (divida) =====
INSERT INTO COMPONENTERECEITA_REGISTO (tarefaActivo_fk, departamento_fk, centroCusto_fk, tipoConta_fk, codigoConta_fk, agrupamentoConfig_fk, descricao, valor, utilizadorCriacao, dataCriacao, indActivo, componenteOrcamentoRegistoFk, InstitutionId)
VALUES (@tarefaRR, @depto, @centroCusto, @tipoReceita, 1, 21, 'Contribuicoes Regimes Complementares - Fevereiro 2026', 2000.00, @admin, GETDATE(), 1, @orcRegisto, @inst);

INSERT INTO GUIAPAGAMENTO (guia_entidade_fk, numDocumento, dtEmissao, descricao, valor, indPago, utilizadorCriacao, dataCriacao, dtValidade, tipoGuia, indActivo, mesAno, bankCode, paymentRef)
VALUES (@entidadeB, 'GP-002/2026', '2026-02-05', 'Guia de Pagamento - Contribuicoes Fevereiro 2026', 2000.00, @indPagoGuiaGerada, @admin, GETDATE(), '2026-02-28', 1, 1, '2026-02-01', 'BNU', 'GP-002/2026');

PRINT 'SEED COMPLETO';
